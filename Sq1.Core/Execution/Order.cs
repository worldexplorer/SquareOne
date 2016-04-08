using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Streaming;

namespace Sq1.Core.Execution {
	public class Order {
		[JsonProperty]	public DateTime		CreatedBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED		[JsonProperty]	public DateTime		PlacedBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED		[JsonProperty]	public DateTime		FilledBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED 		[JsonProperty]	public DateTime		KilledBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		
		[JsonProperty]	public	double		PriceRequested;				// SET_IN_BROKER_ADAPDER	{ get; protected set; }
		[JsonProperty]	public	double		PriceFilled;					// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public	double		Qty;				// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public	double		QtyFill;						// SET_IN_ORDER_PROCESSOR   { get; protected set; }

		[JsonProperty]	public	string		GUID						{ get; protected set; }
		[JsonProperty]	public	OrderState	State						{ get; protected set; }
		[JsonProperty]	public	DateTime	StateUpdateLastTimeLocal	{ get; protected set; }
		[JsonProperty]	public	int			SernoSession;				// SET_IN_BROKER_QUIK	{ get; protected set; }
		[JsonProperty]	public	long		SernoExchange;				// SET_IN_BROKER_QUIK	{ get; protected set; }

		[JsonProperty]	public	bool		IsReplacement;				// SET_IN_ORDER_PROCESSOR   { get; protected set; }
		[JsonProperty]	public	string		ReplacementForGUID			{ get; protected set; }
		[JsonProperty]	public	string		ReplacedByGUID				{ get; protected set; }

		[JsonProperty]	public	bool		IsEmergencyClose;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	int			EmergencyCloseAttemptSerno;	// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	string		EmergencyReplacementForGUID;	// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	string		EmergencyReplacedByGUID;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }

		[JsonProperty]	public	bool		IsKiller					{ get; protected set; }
		[JsonProperty]	public	bool		IsVictim					{ get; protected set; }
		[JsonProperty]	public	string		VictimGUID					{ get; protected set; }
		[JsonIgnore]	public	Order		VictimToBeKilled			{ get; protected set; }
		[JsonProperty]	public	string		KillerGUID					{ get; protected set; }
		[JsonIgnore]	public	Order		KillerOrder					{ get; protected set; }

		[JsonProperty]	public	DateTime	DateServerLastFillUpdate;	// SET_IN_BROKER_QUIK	{ get; protected set; }
		[JsonProperty]	public	bool		EmittedByScript 			{ get; protected set; }

		[JsonProperty]	public	int			SlippageAppliedIndex;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double		SlippageApplied;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double		SlippageFilled;				// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double		SlippageFilledMinusApplied	{ get {
				if (this.SlippageApplied	== 0) return 0;
				if (this.PriceFilled		== 0) return 0;
				if (this.PriceRequested		== 0) return 0;
				double slippageFilled = this.PriceFilled - this.PriceRequested;
				return slippageFilled - this.SlippageApplied;
		} }
		[JsonProperty]	public	double		CurrentAsk					{ get; protected set; }
		[JsonProperty]	public	double		CurrentBid					{ get; protected set; }
		[JsonProperty]	public	double		PriceCurBidOrAsk			{ get {
				double ret = 0;
				switch (this.SpreadSide) {
					case SpreadSide.AskCrossed:
					case SpreadSide.AskTidal:
						ret = this.CurrentAsk;
						break;
					case SpreadSide.BidCrossed:
					case SpreadSide.BidTidal:
						ret = this.CurrentBid;
						break;
					default:
						break;
				}
				return ret;
		} }
		[JsonProperty]	public	SpreadSide	SpreadSide;					// SET_IN_BROKER_ADAPDER	{ get; protected set; }
		[JsonProperty]	public	string		PriceSpreadSideAsString		{ get {
				string ret = "";
				switch (this.SpreadSide) {
					case SpreadSide.AskCrossed:
					case SpreadSide.AskTidal:
						ret = this.CurrentAsk + " " + this.SpreadSide;
						break;
					case SpreadSide.BidCrossed:
					case SpreadSide.BidTidal:
						ret = this.CurrentBid + " " + this.SpreadSide;
						break;
					default:
						ret = this.SpreadSide + " bid[" + CurrentBid + "] ask[" + CurrentAsk + "]";
						break;
				}
				return ret;
			} }

		[JsonProperty]	public	Alert		Alert						{ get; private set; }
		[JsonIgnore]	public	bool		OnlyDeserializedHasNoBars	{ get { return this.Alert.Bars == null; } }

		// why Concurrent: OrderProcessor adds while GUI reads (a copy); why Stack: ExecutionTree displays Messages RecentOnTop;
		// TODO: revert to List (with lock(privateLock) { messages.Add/Remove/Count}) when:
		//	1) ConcurrentQueue's Interlocked.CompareExchange<>() is slower than lock(privateLock),
		//	2) you'll need sorting by date/state BEFORE ExecutionTree (ExecutionTree simulates sorted lists by ),
		//	3) you'll prove that removing lock() won't cause "CollectionModifiedException"
		[JsonIgnore]			ConcurrentStack<OrderStateMessage> messages;
		[JsonIgnore]	public	ConcurrentStack<OrderStateMessage> MessagesSafeCopy { get { return
			new ConcurrentStack<OrderStateMessage>(this.messages)
			//SECOND_CLICK_MAKES_THEM_EMPTY??? this.messages
			; } }
		
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
				this.messages = new ConcurrentStack<OrderStateMessage>(value);
			}
		}

		// no search among lvOrders.Items[] is required to populate the order update
		//[JsonIgnore]	public ListViewItem		ListViewItemInExecutionForm	{ get; protected set; }
		//[JsonIgnore]	public int				StateImageIndex				{ get; protected set; }
		
		[JsonIgnore]	public	List<Order>		DerivedOrders				{ get; protected set; }		// rebuilt on app restart from	DerivedOrdersGuids 
		[JsonProperty]	public	List<string>	DerivedOrdersGuids			{ get; protected set; }
		[JsonProperty]	public	Order			DerivedFrom	;				// SET_IN_OrdersShadowTreeDerived	{ get; protected set; }		// one parent with possibly its own parent, but not too deep; lazy to restore from DerivedFromGui only to rebuild Tree after restart

		
		#region TODO OrderStateCollections: REFACTOR states to be better named/handled in OrderStateCollections.cs
		[JsonProperty]	public bool				InState_limitOrStopCanBeReplaced { get {
				return this.State == OrderState.WaitingBrokerFill
					|| this.State == OrderState.Submitted
					|| this.State == OrderState.FilledPartially
					;
			} }
		[JsonProperty]	public bool				InState_expectingBrokerCallback { get {
				return this.State == OrderState.WaitingBrokerFill
					|| this.State == OrderState.Submitting
					|| this.State == OrderState.Submitted
				//	|| this.State == OrderState.KillerPreSubmit
					|| this.State == OrderState.KillerSubmitting
					|| this.State == OrderState.KillerBulletFlying
				//	|| this.State == OrderState.KillPendingPreSubmit
					|| this.State == OrderState.VictimsBulletConfirmed
					|| this.State == OrderState.VictimsBulletSubmitted
					;
			} }
		[JsonProperty]	public bool				InStateChangeableToSubmitted { get {
				if (this.State == OrderState.PreSubmit
					|| this.State == OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted
					|| this.State == OrderState.EmitOrdersNotClicked
					//&& this._IsLoggedInOrPaperAccount(current3)
					) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public bool				InState_emergency { get {
				if (this.State == OrderState.EmergencyCloseSheduledForRejected
						|| this.State == OrderState.EmergencyCloseSheduledForRejectedLimitReached
						|| this.State == OrderState.EmergencyCloseSheduledForErrorSubmittingBroker) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public OrderState		InState_errorOrRejected_convertToComplementaryEmergencyState { get {
				OrderState newState = OrderState.Error;
				if (this.State == OrderState.Rejected)				newState = OrderState.EmergencyCloseSheduledForRejected;
				if (this.State == OrderState.RejectedLimitReached)	newState = OrderState.EmergencyCloseSheduledForRejectedLimitReached;
				if (this.State == OrderState.ErrorSubmitting_BrokerTerminalDisconnected)	newState = OrderState.EmergencyCloseSheduledForErrorSubmittingBroker;
				return newState;
			} }
		[JsonProperty]	public bool				InState_changeableToEmergency { get { return (InState_errorOrRejected_convertToComplementaryEmergencyState != OrderState.Error); } }
		#endregion



		[JsonProperty]	public	int				AddedToOrdersListCounter;	// SET_IN_ORDER_LIST { get; protected set; }
		[JsonProperty]	public	string			LastMessage;

		[JsonIgnore]	public	bool			HasSlippagesDefined { get {
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
				int slippageIndexMax = this.Alert.Bars.SymbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(this.Alert);
				return (slippageIndexMax == -1) ? false : true;
			} }
		[JsonIgnore]	public	bool			NoMoreSlippagesAvailable { get {
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
				int slippageIndexMax = this.Alert.Bars.SymbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(this.Alert);
				if (slippageIndexMax == -1) return false;
				return (this.SlippageAppliedIndex > slippageIndexMax) ? true : false;
			} }
		[JsonIgnore]	public	bool			EmergencyCloseAttemptSernoExceedLimit { get {
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
		[JsonProperty]	public	double			CommissionFill				{ get; protected set; }
		[JsonProperty]	public	string			BrokerAdapterName			{ get { return this.Alert.BrokerName; } }

 		public Order() {	// called by Json.Deserialize(); what if I'll make it protected?
			GUID = newGUID();
			messages = new ConcurrentStack<OrderStateMessage>();
			PriceRequested = 0;
			PriceFilled = 0;
			Qty = 0;
			QtyFill = 0;

			State = OrderState.Unknown;
			SernoSession = 0;		//QUIK
			SernoExchange = 0;		//QUIK

			IsReplacement = false;
			ReplacementForGUID = "";
			ReplacedByGUID = "";

			IsKiller = false;
			VictimGUID = "";
			KillerGUID = "";

			//StateImageIndex = 0;
			StateUpdateLastTimeLocal = DateTime.MinValue;
			EmittedByScript = false;
			SlippageApplied = 0;
			SlippageAppliedIndex = 0;
			CurrentAsk = 0;
			CurrentBid = 0;
			SpreadSide = SpreadSide.Unknown;
			//MreActiveCanCome = new ManualResetEvent(false);
			DerivedOrders = new List<Order>();
			DerivedOrdersGuids = new List<string>();
		}
		public Order(Alert alert, bool emittedByScript, bool forceOverwriteAlertOrderFollowedToNewlyCreatedOrder = false) : this() {
			if (alert == null) {
				string msg = "DONT_PASS_ALERT_NULL_TO_Order()  btw, OrderSerializerLogrotate will also refuse to serialize an empty Order";
				throw new Exception(msg);
			}
			if (alert.OrderFollowed != null && forceOverwriteAlertOrderFollowedToNewlyCreatedOrder == false) {
				string msg = "YOU_DONT_NEED_TWO_ORDERS_PER_ALERT alert.OrderFollowed!=null; alert[" + alert + "] alert.OrderFollowed[" + alert.OrderFollowed + "]";
				alert.OrderFollowed.appendMessage(msg);
				throw new Exception(msg);
			}
			
			if (		alert.QuoteCreatedThisAlert != null
					 && alert.QuoteCreatedThisAlert.ParentBarStreaming != null
					 && alert.QuoteCreatedThisAlert.ParentBarStreaming.ParentBars == null) {
				string msg = "FOR_PROTOTYPED_ALERTS_ACTIVATED_ON_TRIGGERING_FILL__CHECK_ONCE_AGAIN__EXECUTOR.CALLBACK_FILL_BLABLABLA";
				Assembler.PopupException(msg);
			}
			
			this.Qty					= alert.Qty;

			//if (alert.DataSource != null && alert.DataSource.StreamingAdapter != null) {
			//    this.CurrentBid = alert.DataSource.StreamingAdapter.StreamingDataSnapshot.BestBid_getForMarketOrder(alert.Symbol);
			//    this.CurrentAsk = alert.DataSource.StreamingAdapter.StreamingDataSnapshot.BestAsk_getForMarketOrder(alert.Symbol);
			//}
			this.CurrentBid				= alert.CurrentBid;
			this.CurrentAsk				= alert.CurrentAsk;

			this.SpreadSide				= alert.SpreadSide;

			this.PriceRequested			= alert.PriceEmitted;
			this.SlippageAppliedIndex	= 0;
			this.SlippageApplied		= alert.SlippageApplied;

			this.EmittedByScript		= emittedByScript;
			//due to serverTime lagging, replacements orders are born before the original order... this.TimeCreatedServer = alert.TimeCreatedLocal;
			if (alert.Bars == null) {
				string msg1 = "NYI:KILLING_ORDERS_AFTER_APPRESTART ORDER_DESERIALIZED_IS_NOT_ATTACHED_TO_BAR";
				Assembler.PopupException(msg1, null, false);
			} else {
				this.CreatedBrokerTime		= alert.Bars.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
			}

			this.Alert = alert;
			alert.OrderFollowed = this;
			
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
			killer.State = OrderState.JustConstructed;
			killer.PriceRequested = 0;
			killer.PriceFilled = 0;
			killer.Qty = 0;
			killer.QtyFill = 0;

			killer.VictimToBeKilled = this;
			killer.VictimGUID = this.GUID;
			killer.Alert.SignalName = "IAM_KILLER_FOR " + this.Alert.SignalName;
			killer.IsKiller = true;

			this.KillerOrder = killer;
			this.KillerGUID = killer.GUID;
			this.IsVictim = true;
			
			this.DerivedOrdersAdd(killer);
			
			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
			killer.CreatedBrokerTime = serverTimeNow;

			return killer;
		}
		public Order DeriveReplacementOrder() {
			if (this.Alert == null) {
				string msg = "DeriveReplacementOrder(): Alert=null (serializer will get upset) for " + this.ToString();
				throw new Exception(msg);
			}
			Order replacement = new Order(this.Alert, this.EmittedByScript, true);
			replacement.State = OrderState.JustConstructed;
			replacement.SlippageAppliedIndex = this.SlippageAppliedIndex;
			replacement.ReplacementForGUID = this.GUID;
			this.ReplacedByGUID = replacement.GUID;
			
			this.DerivedOrdersAdd(replacement);

			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
			replacement.CreatedBrokerTime = serverTimeNow;

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
		
		//internal for Sq1.Core.dll to use ONLY; BrokerAdapters should use OrderProcessor.AppendMessage_propagateToGui(osm_deserialized);
		internal void appendMessage(string msg) {
			this.appendOrderMessage(new OrderStateMessage(this, msg));
		}
		//internal for Sq1.Core.dll to use ONLY; BrokerAdapters should use OrderProcessor.AppendOrderMessage_propagateToGui(osm_deserialized);
		internal void appendOrderMessage(OrderStateMessage omsg) {
			this.LastMessage = omsg.Message;	// KISS; mousemove over OlvOrdersTree won't bother calculating
			this.messages.Push(omsg);
			//this.messages.Enqueue(omsg);
		}

		public override string ToString() {
			string ret = "";
			ret += this.GUID + " ";
			if (this.IsKiller) ret += "KILLER_FOR ";
			if (this.Alert != null) {
				ret += this.Alert.ToString_forOrder();
			} else {
				ret += " this.Alert=null_CONSTRUCTOR_NOT_COMPLETE";
			}

			string formatPrice = "N0";
			if (this.Alert.Bars != null) formatPrice = this.Alert.Bars.SymbolInfo.PriceFormat;

			ret += " @ " + this.PriceRequested.ToString(formatPrice);
			ret += " " + this.State;
			if (this.SernoSession != 0) ret += " SernoSession[" + this.SernoSession + "]";
			//if (GUID != "") ret += " GUID[" + GUID + "]";
			//if (SernoExchange != 0) ret += " SernoExchange[" + SernoExchange + "]";
			if (this.QtyFill != 0.0) ret += " FillQty[" + this.QtyFill + "]";
			if (this.PriceFilled != 0.0) ret += " PriceFilled[" + this.PriceFilled.ToString(formatPrice) + "]";
			//if (this.Alert.PriceDeposited != 0) ret += " PricePaid[" + this.Alert.PriceDeposited + "]";
			if (this.EmittedByScript) ret += " EmittedByScript";
			if (this.Alert.MyBrokerIsLivesim) ret += " Livesim";
			return ret;
		}
		public bool hasBrokerAdapter(string callerMethod) {
			bool ret = true;
			string errormsg = "";
			if (this.Alert.DataSource_fromBars == null) {
				errormsg += "order.Alert[" + this.Alert + "].DataSource property must be set ";
			}
			if (this.Alert.DataSource_fromBars.BrokerAdapter == null) {
				errormsg += "order.Alert.DataSource[" + this.Alert.DataSource_fromBars + "].BrokerAdapter property must be set ";
			}
			if (errormsg != "") {
				this.appendMessage(callerMethod + errormsg);
				ret = false;
			}
			return ret;
		}
		public void FillWith(double priceFill, double qtyFill, double slippageFill = 0, double commissionFill = 0) {
			this.PriceFilled = priceFill;
			this.QtyFill = qtyFill;
			if (this.SlippageFilled == 0 && slippageFill != 0) {
				this.SlippageFilled = slippageFill;
			}
			if (this.CommissionFill == 0 && commissionFill != 0) {
				this.CommissionFill = commissionFill;
			}
			// NOPE THATS_WHATS_WHAT_CallbackAlertFilledMoveAroundInvokeScript_WILL_DO_LATER_IN_OrderProcessor
			//Bar barStreaming = this.Alert.Bars.BarStreaming;
			//if (barStreaming == null) {
			//	string msg = "ORDER_FILLED_HAS_ALERT_WITHOUT_STREAMING_BAR order[" + this.ToString() + "].Alert[" + this.Alert + "]";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			//this.Alert.FillPositionAffectedEntryOrExitRespectively(barStreaming, -1, priceFill, qtyFill, slippageFill, commissionFill);
		}
		public bool FindState_inOrderMessages(OrderState orderState, int occurenciesLooking = 1) {
			lock (this.messages) {
				int occurenciesFound = 0;
				foreach (OrderStateMessage osm in this.messages) {
					if (osm.State != orderState) continue;
					occurenciesFound++;
				}
				return (occurenciesFound >= occurenciesLooking);
			}
		}
		public void AbsorbCurrentBidAsk_fromStreamingSnapshot(StreamingDataSnapshot snap) {
			this.CurrentBid = snap.GetBestBid_notAligned_forMarketOrder_fromQuoteCurrent(this.Alert.Symbol);
			this.CurrentAsk = snap.GetBestAsk_notAligned_forMarketOrder_fromQuoteCurrent(this.Alert.Symbol);
		}

		internal void SetState_localTime_fromMessage(OrderStateMessage newStateWithReason) {
			this.setState_localTime(newStateWithReason.State, newStateWithReason.DateTime);
		}

		internal void SetState_localTimeNow(OrderState newOrderState) {
			this.setState_localTime(newOrderState, DateTime.Now);
		}

		void setState_localTime(OrderState newOrderState, DateTime localTime_updated) {
			this.State						= newOrderState;
			this.StateUpdateLastTimeLocal	= localTime_updated;
		}
	}
}