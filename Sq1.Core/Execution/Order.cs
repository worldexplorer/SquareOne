using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using Newtonsoft.Json;
using Sq1.Core.Streaming;

namespace Sq1.Core.Execution {
	public class Order {
		[JsonProperty]	public DateTime		TimeCreatedBroker;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public double		PriceRequested;				// SET_IN_BROKER_PROVIDER	{ get; protected set; }
		[JsonProperty]	public double		PriceFill;					// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public double		QtyRequested;				// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public double		QtyFill;						// SET_IN_ORDER_PROCESSOR   { get; protected set; }

		[JsonProperty]	public string		GUID						{ get; protected set; }
		[JsonProperty]	public OrderState	State;						// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public DateTime		StateUpdateLastTimeLocal;	// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public int			SernoSession;				// SET_IN_BROKER_QUIK	{ get; protected set; }
		[JsonProperty]	public long			SernoExchange;				// SET_IN_BROKER_QUIK	{ get; protected set; }

		[JsonProperty]	public bool			IsReplacement;				// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public string		ReplacementForGUID			{ get; protected set; }
		[JsonProperty]	public string		ReplacedByGUID				{ get; protected set; }

		[JsonProperty]	public bool			IsEmergencyClose;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public int			EmergencyCloseAttemptSerno;	// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public string		EmergencyReplacementForGUID;	// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public string		EmergencyReplacedByGUID;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }

		[JsonProperty]	public bool			IsKiller					{ get; protected set; }
		[JsonProperty]	public string		VictimGUID					{ get; protected set; }
		[JsonIgnore]	public Order		VictimToBeKilled			{ get; protected set; }
		[JsonProperty]	public string		KillerGUID					{ get; protected set; }
		[JsonIgnore]	public Order		KillerOrder					{ get; protected set; }

		[JsonProperty]	public DateTime		DateServerLastFillUpdate;	// SET_IN_BROKER_QUIK	{ get; protected set; }
		[JsonProperty]	public bool			EmittedByScript 			{ get; protected set; }
		[JsonProperty]	public double		SlippageFill;				// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public int			SlippageIndex;				// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public double		CurrentAsk					{ get; protected set; }
		[JsonProperty]	public double		CurrentBid					{ get; protected set; }
		[JsonProperty]	public OrderSpreadSide SpreadSide;				// SET_IN_BROKER_PROVIDER	{ get; protected set; }
		[JsonProperty]	public string		PriceSpreadSideAsString		{ get {
				string ret = "";
				switch (this.SpreadSide) {
					case OrderSpreadSide.AskCrossed:
					case OrderSpreadSide.AskTidal:
						ret = CurrentAsk + " " + SpreadSide;
						break;
					case OrderSpreadSide.BidCrossed:
					case OrderSpreadSide.BidTidal:
						ret = CurrentBid + " " + SpreadSide;
						break;
					default:
						ret = SpreadSide + " bid[" + CurrentBid + "] ask[" + CurrentAsk + "]";
						break;
				}
				return ret;
			} }
		[JsonProperty]	public Alert		Alert						{ get; private set; }
	
		// why Concurrent: OrderProcessor adds while GUI reads (a copy); why Stack: ExecutionTree displays Messages RecentOnTop;
		// TODO: revert to List (with lock(privateLock) { messages.Add/Remove/Count}) when:
		//	1) ConcurrentQueue's Interlocked.CompareExchange<>() is slower than lock(privateLock),
		//	2) you'll need sorting by date/state BEFORE ExecutionTree (ExecutionTree simulates sorted lists by ),
		//	3) you'll prove that removing lock() won't cause "CollectionModifiedException"
		[JsonIgnore]			ConcurrentQueue<OrderStateMessage> messages;
		[JsonIgnore]	public	ConcurrentQueue<OrderStateMessage> MessagesSafeCopy { get { return new ConcurrentQueue<OrderStateMessage>(this.messages); } }
		
		//[JsonIgnore]
		// List because Json.Net doesn't serialize ConcurrentQueue as []; I wanted deserialization compability
		[JsonProperty]	public	List<OrderStateMessage> MessagesSerializationProxy {
			get {
				// don't return {new List(empty)} as the next line; if JsonConvert.DeserializeObject gets NULL it'll SET a deserialized list  
				if (this.messages.Count == 0) return null; 
				return new List<OrderStateMessage>(this.messages);
				//string msg = "JsonConvert.DeserializeObject gets the deserialized Messages exactly once and it should get NULL"
				//	+ "; never access MessagesSafeCopy.set manually"
				//	+ "; it's a [JsonProperty]	used by LogrotateSerializer<Order>.Deserialize() and .Serialize()";
				//throw new Exception(msg);
			}
			set {
				if (this.messages.Count > 0) {
					string msg = "JsonConvert.DeserializeObject sets the deserialized Messages exactly once"
						+ "; never access MessagesSafeCopy.set manually"
						+ "; it's a [JsonProperty]	used by LogrotateSerializer<Order>.Deserialize() and .Serialize()";
					throw new Exception(msg);
					//return;
				}
				this.messages = new ConcurrentQueue<OrderStateMessage>(value);
			}
		}

		// no search among lvOrders.Items[] is required to populate the order update
		[JsonIgnore]	public ListViewItem		ListViewItemInExecutionForm	{ get; protected set; }
		[JsonIgnore]	public int				StateImageIndex				{ get; protected set; }
		
		[JsonIgnore]	public List<Order>		DerivedOrders				{ get; protected set; }		// rebuilt on app restart from	DerivedOrdersGuids 
		[JsonProperty]	public List<string>		DerivedOrdersGuids			{ get; protected set; }
		[JsonProperty]	public Order			DerivedFrom	;				// SET_IN_OrdersShadowTreeDerived	{ get; protected set; }		// one parent with possibly its own parent, but not too deep; lazy to restore from DerivedFromGui only to rebuild Tree after restart

		
		#region TODO OrderStateCollections: REFACTOR states to be better named/handled in OrderStateCollections.cs
		[JsonProperty]	public bool				InStateCanBeReplacedLimitStop { get {
				return this.State == OrderState.WaitingBrokerFill
					|| this.State == OrderState.Submitted
					|| this.State == OrderState.FilledPartially;
			} }
		[JsonProperty]	public bool				InStateExpectingCallbackFromBroker { get {
				return this.State == OrderState.WaitingBrokerFill
					|| this.State == OrderState.Submitting
					|| this.State == OrderState.Submitted
					|| this.State == OrderState.KillPending;
			} }
		[JsonProperty]	public bool				InStateChangeableToSubmitted { get {
				if (this.State == OrderState.PreSubmit
					|| this.State == OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted
					|| this.State == OrderState.AutoSubmitNotEnabled
					//&& this._IsLoggedInOrPaperAccount(current3)
					) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public bool				InStateEmergency { get {
				if (this.State == OrderState.EmergencyCloseSheduledForRejected
						|| this.State == OrderState.EmergencyCloseSheduledForRejectedLimitReached
						|| this.State == OrderState.EmergencyCloseSheduledForErrorSubmittingBroker) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public OrderState		InStateErrorComplementaryEmergencyState { get {
				OrderState newState = OrderState.Error;
				if (this.State == OrderState.Rejected)				newState = OrderState.EmergencyCloseSheduledForRejected;
				if (this.State == OrderState.RejectedLimitReached)	newState = OrderState.EmergencyCloseSheduledForRejectedLimitReached;
				if (this.State == OrderState.ErrorSubmittingBroker)	newState = OrderState.EmergencyCloseSheduledForErrorSubmittingBroker;
				return newState;
			} }
		[JsonProperty]	public bool				InStateChangeableToEmergency { get { return (InStateErrorComplementaryEmergencyState != OrderState.Error); } }
		#endregion



		[JsonProperty]	public int				AddedToOrdersListCounter;	// SET_IN_ORDER_LIST { get; protected set; }
		[JsonProperty]	public string			LastMessage { get {
				int count = this.messages.Count; 
				if (count == 0) return "";
				OrderStateMessage lastOmsg;
				bool success = this.messages.TryPeek(out lastOmsg);
				if (!success) {
					throw new Exception("messages.TryPeek() failed while messages.Count[" + count + "/" + messages.Count + "]");
				}
				return lastOmsg.Message; 
			} }
		[JsonIgnore]	public bool				hasSlippagesDefined { get {
				string msg1 = "[JsonIgnore]	to let orders restored after app restart fly over it; they don't have alert.Bars restored yet";
				if (this.Alert == null) {
					string msg = "PROBLEMATIC_Order.Alert=NULL_hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.BARS=NULL__ADDED_[JsonIgnore]; //Order.hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars.SymbolInfo == null) {
					string msg = "PROBLEMATIC_Order.Alert.Bars.SymbolInfo=NULL_hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				int slippageIndexMax = this.Alert.Bars.SymbolInfo.getSlippageIndexMax(this.Alert.Direction);
				return (slippageIndexMax == -1) ? false : true;
			} }
		[JsonIgnore]	public bool				noMoreSlippagesAvailable { get {
				string msg1 = "[JsonIgnore]	to let orders restored after app restart fly over it; they don't have alert.Bars restored yet";
				if (this.Alert == null) {
					string msg = "PROBLEMATIC_Order.Alert=NULL_noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.BARS=NULL__ADDED_[JsonIgnore] //Order.noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars.SymbolInfo == null) {
					string msg = "PROBLEMATIC_Order.Alert.Bars.SymbolInfo=NULL_noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				string msg2 = "slippagesNotDefinedOr?";
				int slippageIndexMax = this.Alert.Bars.SymbolInfo.getSlippageIndexMax(this.Alert.Direction);
				if (slippageIndexMax == -1) return false;
				return (this.SlippageIndex > slippageIndexMax) ? true : false;
			} }
		[JsonIgnore]	public bool				EmergencyCloseAttemptSernoExceedLimit { get {
				if (this.Alert.Bars == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.BARS=NULL__ADDED_[JsonIgnore] //Order.EmergencyCloseAttemptSernoExceedLimit";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax <= 0) return false;
				if (this.EmergencyCloseAttemptSerno > this.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax) {
					return true;
				}
				return false;
			} }

		[JsonProperty]	static int absno = 0;
		[JsonProperty]	public double CommissionFill				{ get; protected set; }
		[JsonIgnore]	public ManualResetEvent MreActiveCanCome	{ get; protected set; }		// JSON restore of a ManualResetEvent makes GC thread throw SEHException (thanx for a great free library anyway)

 		public Order() {	// called by Json.Deserialize(); what if I'll make it protected?
			this.GUID = newGUID();
			this.messages = new ConcurrentQueue<OrderStateMessage>();
			this.PriceRequested = 0;
			this.PriceFill = 0;
			this.QtyRequested = 0;
			this.QtyFill = 0;

			this.State = OrderState.Unknown;
			this.SernoSession = 0;		//QUIK
			this.SernoExchange = 0;		//QUIK

			this.IsReplacement = false;
			this.ReplacementForGUID = "";
			this.ReplacedByGUID = "";

			this.IsKiller = false;
			this.VictimGUID = "";
			this.KillerGUID = "";

			this.StateImageIndex = 0;
			this.StateUpdateLastTimeLocal = DateTime.MinValue;
			this.EmittedByScript = false;
			this.SlippageFill = 0;
			this.SlippageIndex = 0;
			this.CurrentAsk = 0;
			this.CurrentBid = 0;
			this.SpreadSide = OrderSpreadSide.Unknown;
			this.MreActiveCanCome = new ManualResetEvent(false);
			this.DerivedOrders = new List<Order>();
			this.DerivedOrdersGuids = new List<string>();
		}
		public Order(Alert alert, bool emittedByScript, bool forceOverwriteAlertOrderFollowedToNewlyCreatedOrder = false) : this() {
			if (alert == null) {
				string msg = "DONT_PASS_ALERT_NULL_TO_Order()  btw, OrderSerializerLogrotate will also refuse to serialize an empty Order";
				throw new Exception(msg);
			}
			if (alert.OrderFollowed != null && forceOverwriteAlertOrderFollowedToNewlyCreatedOrder == false) {
				string msg = "YOU_DONT_NEED_TWO_ORDERS_PER_ALERT alert.OrderFollowed!=null; alert[" + alert + "] alert.OrderFollowed[" + alert.OrderFollowed + "]";
				alert.OrderFollowed.AppendMessage(msg);
				throw new Exception(msg);
			}
			if (alert.QuoteCreatedThisAlert.ParentBarStreaming.ParentBars == null) {
				string msg = "EARLY_BINDER_DIDN_DO_ITS_JOB#5";
				Assembler.PopupException(msg);
			}
			this.PriceRequested			= alert.PriceScript;
			this.QtyRequested			= alert.Qty;
			this.EmittedByScript		= emittedByScript;
			//due to serverTime lagging, replacements orders are born before the original order... this.TimeCreatedServer = alert.TimeCreatedLocal;
			this.TimeCreatedBroker		= alert.Bars.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
			//Order.PositionFollowed will be created when order.State becomes OrderState.Filled this.PositionFollowed		= new Position()
			this.SpreadSide				= alert.OrderSpreadSide;

			// Bid/Ask Dictionaries are synchronized => no exceptions
			if (alert.DataSource != null && alert.DataSource.StreamingProvider != null) {
				this.CurrentBid = alert.DataSource.StreamingProvider.StreamingDataSnapshot.BestBidGetForMarketOrder(alert.Symbol);
				this.CurrentAsk = alert.DataSource.StreamingProvider.StreamingDataSnapshot.BestAskGetForMarketOrder(alert.Symbol);
			}

			this.Alert = alert;
			alert.OrderFollowed = this;
			
//MOVED_TO OrderProcessor::CreatePropagateOrderFromAlert() for {if (alert.IsExitAlert) alert.PositionAffected.EntryAlert.OrderFollowed.DerivedOrdersAdd(newborn);}
//			if (alert.PositionAffected != null && alert.PositionAffected.EntryAlert != null && alert.PositionAffected.EntryAlert.OrderFollowed != null) {
//				alert.PositionAffected.EntryAlert.OrderFollowed.DerivedOrdersAdd(this);
//				// TODO will also have to notify ExecutionForm on this Order, which will close a Position 
//			}
			alert.MreOrderFollowedIsAssignedNow.Set();	// simple alert submission is single threaded, including proto.StopLossAlertForAnnihilation!
		}
		public static string newGUID() {
			string ret = DateTime.Now.ToString("Hmmssfff");
			try {
				int noLeadingZero = Int32.Parse(ret);
				noLeadingZero += ++absno;
				ret = noLeadingZero.ToString();
			} catch (Exception e) {
				int a = 1;
			}
			return ret;
		}
		public Order DeriveKillerOrder() {
			if (this.Alert == null) {
				string msg = "DeriveKillerOrder(): Alert=null (serializer will get upset) for " + this.ToString();
				throw new Exception(msg);
			}
			Order killer = new Order(this.Alert, this.EmittedByScript, true);
			killer.State = OrderState.JustCreated;
			killer.PriceRequested = 0;
			killer.PriceFill = 0;
			killer.QtyRequested = 0;
			killer.QtyFill = 0;

			this.KillerOrder = killer;
			this.KillerGUID = killer.GUID;
			killer.VictimToBeKilled = this;
			killer.VictimGUID = this.GUID;
			killer.Alert.SignalName = "Killer4: " + this.Alert.SignalName;
			killer.IsKiller = true;
			
			this.DerivedOrdersAdd(killer);
			
			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
			killer.TimeCreatedBroker = serverTimeNow;

			return killer;
		}
		public Order DeriveReplacementOrder() {
			if (this.Alert == null) {
				string msg = "DeriveReplacementOrder(): Alert=null (serializer will get upset) for " + this.ToString();
				throw new Exception(msg);
			}
			Order replacement = new Order(this.Alert, this.EmittedByScript, true);
			replacement.State = OrderState.JustCreated;
			replacement.SlippageIndex = this.SlippageIndex;
			replacement.ReplacementForGUID = this.GUID;
			this.ReplacedByGUID = replacement.GUID;
			
			this.DerivedOrdersAdd(replacement);

			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
			replacement.TimeCreatedBroker = serverTimeNow;

			return replacement;
		}
		public void DerivedOrdersAdd(Order killerReplacementPositionclose) {
			if (this.DerivedOrdersGuids.Contains(killerReplacementPositionclose.GUID)) {
				string msg = "ALREADY_ADDED DerivedOrder.GUID[" + killerReplacementPositionclose.GUID + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.DerivedOrdersGuids.Add(killerReplacementPositionclose.GUID);
			this.DerivedOrders.Add(killerReplacementPositionclose);
			killerReplacementPositionclose.DerivedFrom = this;
		}

		public bool RebuildDerivedOrdersGuids() {
			List<string> backup = this.DerivedOrdersGuids;
			this.DerivedOrdersGuids = new List<string>();
			foreach (Order order in this.DerivedOrders) {
				this.DerivedOrdersGuids.Add(order.GUID);
			}
			return backup.Count != this.DerivedOrders.Count;
		}
		
		public Order FindOrderGuidAmongDerivedsRecursively(string Guid) {
			Order ret = null;
			foreach (Order derived in this.DerivedOrders) {
				if (derived.GUID != Guid) continue;
				ret = derived;
				break;
			}
			
			Order foundAmongChildrenOfDerived = null;
			if (ret == null) {
				foreach (Order derived in this.DerivedOrders) {
					foundAmongChildrenOfDerived = derived.FindOrderGuidAmongDerivedsRecursively(Guid);
					if (foundAmongChildrenOfDerived == null) continue;
					break;
				}
				if (foundAmongChildrenOfDerived != null) ret = foundAmongChildrenOfDerived; 
			}
			return ret;
		}
		
		public void AppendMessage(string msg) {
			this.AppendMessageSynchronized(new OrderStateMessage(this, msg));
		}
		public void AppendMessageSynchronized(OrderStateMessage omsg) {
			//this.messages.Push(omsg);
			this.messages.Enqueue(omsg);
		}
		public override string ToString() {
			string ret = "";
			ret += this.GUID + " ";
			if (this.IsKiller) ret += "KILLER_FOR ";
			if (this.Alert != null) {
				ret += this.Alert.ToStringForOrder();
			} else {
				ret += " this.Alert=null_CONSTRUCTOR_NOT_COMPLETE";
			}

			string formatPrice = "N0";
			if (this.Alert.Bars != null) formatPrice = this.Alert.Bars.SymbolInfo.FormatPrice;

			ret += " @ " + this.PriceRequested.ToString(formatPrice);
			ret += " " + this.State;
			if (this.SernoSession != 0) ret += " SernoSession[" + this.SernoSession + "]";
			//if (GUID != "") ret += " GUID[" + GUID + "]";
			//if (SernoExchange != 0) ret += " SernoExchange[" + SernoExchange + "]";
			if (this.QtyFill != 0.0) ret += " FillQty[" + this.QtyFill + "]";
			if (this.PriceFill != 0.0) ret += " PriceFilled[" + this.PriceFill.ToString(formatPrice) + "]";
			//if (this.Alert.PriceDeposited != 0) ret += " PricePaid[" + this.Alert.PriceDeposited + "]";
			if (this.EmittedByScript) ret += " EmittedByScript";
			return ret;
		}
		public bool hasBrokerProvider(string callerMethod) {
			bool ret = true;
			string errormsg = "";
			if (this.Alert.DataSource == null) {
				errormsg += "order.Alert[" + this.Alert + "].DataSource property must be set ";
			}
			if (this.Alert.DataSource.BrokerProvider == null) {
				errormsg += "order.Alert.DataSource[" + this.Alert.DataSource + "].BrokerProvider property must be set ";
			}
			if (errormsg != "") {
				this.AppendMessage(callerMethod + errormsg);
				ret = false;
			}
			return ret;
		}
		public void FilledWith(double priceFill, double qtyFill, double slippageFill = 0, double commissionFill = 0) {
			this.PriceFill = priceFill;
			this.QtyFill = qtyFill;
			if (this.SlippageFill == 0 && slippageFill != 0) {
				this.SlippageFill = slippageFill;
			}
			if (this.CommissionFill == 0 && commissionFill != 0) {
				this.CommissionFill = commissionFill;
			}
			// NOPE THATS_WHATS_WHAT_CallbackAlertFilledMoveAroundInvokeScript_WILL_DO_LATER_IN_OrderProcessor
			//Bar barStreaming = this.Alert.Bars.BarStreaming;
			//if (barStreaming == null) {
			//    string msg = "ORDER_FILLED_HAS_ALERT_WITHOUT_STREAMING_BAR order[" + this.ToString() + "].Alert[" + this.Alert + "]";
			//    Assembler.PopupException(msg);
			//    return;
			//}
			//this.Alert.FillPositionAffectedEntryOrExitRespectively(barStreaming, -1, priceFill, qtyFill, slippageFill, commissionFill);
		}
		public bool FindStateInOrderMessages(OrderState orderState, int occurenciesLooking = 1) {
			lock (this.messages) {
				int occurenciesFound = 0;
				foreach (OrderStateMessage osm in this.messages) {
					if (osm.State != orderState) continue;
					occurenciesFound++;
				}
				return (occurenciesFound >= occurenciesLooking);
			}
		}
		public void AbsorbCurrentBidAskFromStreamingSnapshot(StreamingDataSnapshot snap) {
			this.CurrentBid = snap.BestBidGetForMarketOrder(this.Alert.Symbol);
			this.CurrentAsk = snap.BestAskGetForMarketOrder(this.Alert.Symbol);
		}
	}
}