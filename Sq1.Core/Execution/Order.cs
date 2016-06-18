using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Streaming;
using Sq1.Core.Broker;

namespace Sq1.Core.Execution {
	public partial class Order {
		[JsonProperty]	public DateTime		CreatedBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED		[JsonProperty]	public DateTime		PlacedBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED		[JsonProperty]	public DateTime		FilledBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
//SEARCH_MESSAGES_FOR_STATE_YOU_NEED 		[JsonProperty]	public DateTime		KilledBrokerTime;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
		
		[JsonProperty]	public	double		PriceEmitted;				// SET_IN_BROKER_ADAPDER	{ get; private set; }
		[JsonProperty]	public	double		PriceFilled					{ get; private set; }
		[JsonProperty]	public	double		Qty							{ get; private set; }
		[JsonProperty]	public	double		QtyFill						{ get; private set; }

		[JsonProperty]	public	string		GUID						{ get; private set; }
		[JsonProperty]	public	OrderState	State						{ get; private set; }
		[JsonProperty]	public	DateTime	StateUpdateLastTimeLocal	{ get; private set; }
		[JsonProperty]	public	int			SernoSession;				// SET_IN_BROKER_QUIK	{ get; private set; }
		[JsonProperty]	public	long		SernoExchange;				// SET_IN_BROKER_QUIK	{ get; private set; }

		[JsonProperty]	public	bool		IsReplacement;				// SET_IN_ORDER_PROCESSOR   { get; private set; }
		[JsonProperty]	public	string		ReplacementForGUID			{ get; private set; }
		[JsonProperty]	public	string		ReplacedByGUID				{ get; private set; }

		[JsonProperty]	public	bool		IsEmergencyClose;				// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
		[JsonProperty]	public	int			EmergencyCloseAttemptSerno;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
		[JsonProperty]	public	string		EmergencyReplacementForGUID;	// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }
		[JsonProperty]	public	string		EmergencyReplacedByGUID;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; private set; }

		[JsonProperty]	public	string		VictimGUID					{ get; private set; }
		[JsonIgnore]	public	Order		VictimToBeKilled			{ get; private set; }
		[JsonProperty]	public	bool		IsKiller					{ get { return this.VictimToBeKilled != null && string.IsNullOrEmpty(this.VictimGUID) == false; } }

		[JsonProperty]	public	string		KillerGUID					{ get; private set; }
		[JsonIgnore]	public	Order		KillerOrder					{ get; private set; }
		[JsonProperty]	public	bool		IsVictim					{ get { return this.KillerOrder != null && string.IsNullOrEmpty(this.KillerGUID) == false; } }

		[JsonProperty]	public	DateTime	DateServerLastFillUpdate;	// SET_IN_BROKER_QUIK	{ get; private set; }
		[JsonProperty]	public	bool		EmittedByScript 			{ get; private set; }

		[JsonProperty]	public	double		CurrentAsk					{ get; private set; }
		[JsonProperty]	public	double		CurrentBid					{ get; private set; }
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
		[JsonProperty]	public	SpreadSide	SpreadSide;					// SET_IN_BROKER_ADAPDER	{ get; private set; }
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
		[JsonIgnore]	public	ConcurrentStack<OrderStateMessage> MessagesSafeCopy { get { lock (this.messages) {
			 return new ConcurrentStack<OrderStateMessage>(this.messages);
			//SECOND_CLICK_MAKES_THEM_EMPTY??? this.messages
		} } }
		
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

		[JsonIgnore]	public bool FilledOrPartially_inOrderMessages { get {
			bool filled				= this.FindState_inOrderMessages(OrderState.Filled);
			bool partiallyFilled	= this.FindState_inOrderMessages(OrderState.FilledPartially);
			return filled || partiallyFilled;
		} }

		// no search among lvOrders.Items[] is required to populate the order update
		//[JsonIgnore]	public ListViewItem		ListViewItemInExecutionForm	{ get; private set; }
		//[JsonIgnore]	public int				StateImageIndex				{ get; private set; }

		
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
					|| this.State == OrderState.VictimBulletConfirmed
					|| this.State == OrderState.VictimBulletSubmitted
					;
			} }
		[JsonProperty]	public bool				InStateChangeableToSubmitted { get {
				if (	this.State == OrderState.PreSubmit ||
						this.State == OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted ||
						this.State == OrderState.EmitOrdersNotClicked
					//&& this._IsLoggedInOrPaperAccount(current3)
					) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public bool				InState_emergency { get {
				if (	this.State == OrderState.EmergencyCloseSheduledForRejected ||
						this.State == OrderState.EmergencyCloseSheduledForRejectedLimitReached ||
						this.State == OrderState.EmergencyCloseSheduledForErrorSubmittingBroker) {
					return true;
				}
				return false;
			} }
		[JsonProperty]	public OrderState		InState_errorOrRejected_convertToComplementaryEmergencyState { get {
				OrderState newState = OrderState.Error;
				if (this.State == OrderState.Rejected)										newState = OrderState.EmergencyCloseSheduledForRejected;
				if (this.State == OrderState.RejectedLimitReached)							newState = OrderState.EmergencyCloseSheduledForRejectedLimitReached;
				if (this.State == OrderState.ErrorSubmitting_BrokerTerminalDisconnected)	newState = OrderState.EmergencyCloseSheduledForErrorSubmittingBroker;
				return newState;
			} }
		[JsonProperty]	public bool				InState_changeableToEmergency { get { return (InState_errorOrRejected_convertToComplementaryEmergencyState != OrderState.Error); } }
		#endregion

		[JsonProperty]	public	int				AddedToOrdersListCounter;	// SET_IN_ORDER_LIST { get; private set; }
		[JsonProperty]	public	string			LastMessage;


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

		[JsonProperty]	static	int				absno = 0;
		[JsonProperty]	public	bool			DontRemoveMyPending_afterImKilled_IwillBeReplaced;
		[JsonProperty]	public	double			CommissionFill				{ get; private set; }
		[JsonProperty]	public	string			BrokerAdapterName			{ get {
			if (this.Alert == null) return "ALERT_NULL";
			return this.Alert.BrokerName;
		} }

		[JsonIgnore]	public	ManualResetEvent	OrderReplacement_Emitted_afterOriginalKilled__orError		{ get; private set; }


		[JsonIgnore]	public const int	INITIAL_PriceFill	= 0;
		[JsonIgnore]	public const int	INITIAL_QtyFill		= 0;

 		public Order() {	// called by Json.Deserialize(); what if I'll make it protected?
			GUID			= newGUID();
			messages		= new ConcurrentStack<OrderStateMessage>();
			PriceEmitted	= 0;
			PriceFilled		= Order.INITIAL_PriceFill;
			Qty				= 0;
			QtyFill			= Order.INITIAL_QtyFill;

			State			= OrderState.Unknown;
			SernoSession	= 0;		//QUIK
			SernoExchange	= 0;		//QUIK

			IsReplacement	= false;
			ReplacementForGUID	= "";
			ReplacedByGUID		= "";

			VictimGUID		= "";
			KillerGUID		= "";

			//StateImageIndex = 0;
			StateUpdateLastTimeLocal = DateTime.MinValue;
			EmittedByScript	= false;

			// Alert() applies first slippage (for the backtester to use it);
			// Broker.Order_modifyOrderType_priceRequesting_accordingToMarketOrderAs() will set it to 0;
			// replacement will pick that up and ++
			SlippageAppliedIndex = -1;
			SlippageApplied	= 0;

			CurrentAsk		= double.NaN;
			CurrentBid		= double.NaN;
			SpreadSide		= SpreadSide.Unknown;
			DerivedOrders	= new List<Order>();
			DerivedOrdersGuids = new List<string>();

			DontRemoveMyPending_afterImKilled_IwillBeReplaced = false;
			OrderReplacement_Emitted_afterOriginalKilled__orError	= new ManualResetEvent(false);
		}
		public Order(Alert alert, bool emittedByScript, bool setAlertOrderFollowed_toNewlyCreatedOrder_falseForKiller = true) : this() {
			if (alert == null) {
				string msg = "DONT_PASS_ALERT_NULL_TO_Order()  btw, OrderSerializerLogrotate will also refuse to serialize an empty Order";
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

			this.SpreadSide				= alert.SpreadSide;

			// too late => Market Alert became Limit already
			//if (alert.Check_sumIsZero_BidAsk_SlippageApplied_PriceEmitted == false) {
			//    string msg = "ALERT_PriceEmitted_MISALIGNED";
			//    Assembler.PopupException(msg, null, false);
			//}

			this.CurrentBid				= alert.CurrentBid;
			this.CurrentAsk				= alert.CurrentAsk;
			this.PriceEmitted			= alert.PriceEmitted;
			this.SlippageApplied		= alert.SlippageApplied;
			this.SlippageAppliedIndex	= alert.SlippageAppliedIndex;	// first slippage already applied inside the Alert

			this.EmittedByScript		= emittedByScript;
			//due to serverTime lagging, replacements orders are born before the original order... this.TimeCreatedServer = alert.TimeCreatedLocal;
			if (alert.Bars == null) {
				string msg1 = "NYI:KILLING_ORDERS_AFTER_APPRESTART ORDER_DESERIALIZED_IS_NOT_ATTACHED_TO_BAR";
				Assembler.PopupException(msg1, null, false);
			} else {
				this.CreatedBrokerTime		= alert.Bars.MarketInfo.ServerTimeNow;
			}

			this.Alert = alert;

			if (setAlertOrderFollowed_toNewlyCreatedOrder_falseForKiller) {
				if (alert.OrderFollowed != null) {
					string msg = null;
					string msig = " alert.OrderFollowed.State[" + alert.OrderFollowed.State + "] alert[" + alert + "]";
					if (alert.OrderFollowed.State == OrderState.VictimKilled) {
						msg = "I_ASSIGN_A_REPLACEMENT_ORDER_INSTEAD_OF_EXPIRED_KILLED";
					} else {
						msg = "ONLY_REPLACEMENT_ORDER_CAN_OVERWRITE_THE_KILLED_EXPIRED";
					}
					alert.OrderFollowed.AppendMessage(msg + msig);
					Assembler.PopupException(msg + msig, null, false);
				}
				alert.OrderFollowed = this;
				alert.OrderFollowed_isAssignedNow_Mre.Set();	// simple alert submission is single threaded, including proto.StopLossAlertForAnnihilation!
			}
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
		
		//internal for Sq1.Core.dll to use ONLY; BrokerAdapters should use OrderProcessor.AppendMessage_propagateToGui(osm_deserialized);
		public void AppendMessage(string msg) {
			this.AppendOrderMessage(new OrderStateMessage(this, msg));
		}
		//internal for Sq1.Core.dll to use ONLY; BrokerAdapters should use OrderProcessor.AppendOrderMessage_propagateToGui(osm_deserialized);
		public void AppendOrderMessage(OrderStateMessage omsg) { lock (this.messages) {
			this.LastMessage = omsg.Message;	// KISS; mousemove over OlvOrdersTree won't bother calculating
			this.messages.Push(omsg);
			//this.messages.Enqueue(omsg);
		} }

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

			ret += " " + this.Qty;
			ret += "@" + this.PriceEmitted.ToString(formatPrice);
			ret += " " + this.State;

			if (this.IsVictim)				ret += " Victim";
			if (this.IsKiller)				ret += " Killer";

			if (this.SernoSession	!= 0)	ret += " SernoSession[" + this.SernoSession + "]";
			//if (GUID				!= "")	ret += " GUID["			+ GUID + "]";
			//if (SernoExchange		!= 0)	ret += " SernoExchange["+ SernoExchange + "]";
			if (this.QtyFill		!= 0.0)	ret += " FillQty["		+ this.QtyFill + "]";
			if (this.PriceFilled	!= 0.0) ret += " PriceFilled["	+ this.PriceFilled.ToString(formatPrice) + "]";
			//if (this.Alert.PriceDeposited != 0) ret += " PricePaid[" + this.Alert.PriceDeposited + "]";
			if (this.EmittedByScript) ret += " EmittedByScript";
			//if (this.Alert.MyBrokerIsLivesim) ret += " Livesim";
			ret += this.Alert.BrokerName;
			
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
				this.AppendMessage(callerMethod + errormsg);
				ret = false;
			}
			return ret;
		}
		public void FillWith(double priceFill, double qtyFill = Order.INITIAL_QtyFill, double slippageFill = 0, double commissionFill = 0) {
			this.PriceFilled	= priceFill;
			if (qtyFill != Order.INITIAL_QtyFill) {
				this.QtyFill		= qtyFill;
			}
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
		public bool FindState_inOrderMessages(OrderState orderState, int occurenciesLooking = 1) { lock (this.messages) {
			int occurenciesFound = 0;
			foreach (OrderStateMessage osm in this.messages) {
				if (osm.State != orderState) continue;
				occurenciesFound++;
			}
			return (occurenciesFound >= occurenciesLooking);
		} }
		public OrderStateMessage FindFirstMessageContaining_inOrderMessages_nullUnsafe(string msg_killer) {
			OrderStateMessage ret = null;
			foreach (OrderStateMessage osm in this.messages) {
				if (osm.Message.Contains(msg_killer) == false) continue;
				ret = osm;
				break;
			}
			return ret;
		}
		public void AbsorbCurrentBidAsk_fromStreamingSnapshot_ifNotPropagatedFromAlert(StreamingDataSnapshot snap) {
			bool shouldGetFromLastQuote = double.IsNaN(this.CurrentBid) || double.IsNaN(this.CurrentAsk);
			if (shouldGetFromLastQuote == false) return;
			this.CurrentBid = snap.GetBestBid_notAligned_forMarketOrder_fromQuoteLast(this.Alert.Symbol);
			this.CurrentAsk = snap.GetBestAsk_notAligned_forMarketOrder_fromQuoteLast(this.Alert.Symbol);
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

		public Order EnsureOrder_isLiveOrLivesim_nullIfDeserialized(BrokerAdapter broker = null) {
			string brokerName = broker != null ? broker.Name : "NO_BROKER_PASSED__WONT_ASSIGN_TO_ORPHAN_ORDERS";
			string msig = " //EnsureOrder_isLiveOrLivesim_nullIfDeserialized(" + this + "," + brokerName + ")";

			if (this.Alert == null) {
				string msg = "ORDER_FOUND_HAS_ALERT_NULL_UNRECOVERABLE__RETURNING_ORDER_NULL";
				Assembler.PopupException(msg + msig);
				return null; 
			}
			if (this.Alert.Bars == null) {
				string msg = "UNREPAIRABLE__ORDER_FOUND_HAS_Bars_NULL";
				Assembler.PopupException(msg + msig);
				return null;
			}
			if (this.Alert.DataSource_fromBars == null) {
				string msg = "UNREPAIRABLE__ORDER_FOUND_HAS_DataSource_NULL";
				Assembler.PopupException(msg + msig);
				return null;
			}
			if (this.Alert.DataSource_fromBars.BrokerAdapter == null) {
				string msg = "ORDER_FOUND_HAS_BROKER_NULL__ASSIGNING";
				Assembler.PopupException(msg + msig);
				this.Alert.DataSource_fromBars.BrokerAdapter = broker;
			}
			return this;
		}

		[JsonIgnore]	public	bool				IsDisposed;
		public void Dispose() {
			string err = "YOU_SHOULD_NOT_DISPOSE_ORDERS_WHEN_CLEANING_OrderList " + this.ToString();
			Assembler.PopupException(err);
			return;

			if (this.IsDisposed || this.OrderReplacement_Emitted_afterOriginalKilled__orError == null) {
				string msg = "ORDER_WAS_ALREADY_DISPOSED__ACCESSING_NULL_WAIT_HANDLE_WILL_THROW_NPE " + this.ToString();
				//Assembler.PopupException(msg);
				return;
			}
			this.OrderReplacement_Emitted_afterOriginalKilled__orError.Dispose();	// BASTARDO_ESTA_AQUI !!!! LEAKED_HANDLES_HUNTER
			this.OrderReplacement_Emitted_afterOriginalKilled__orError = null;
			this.IsDisposed = true;
		}
	}
}