using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter : IDisposable {
		[JsonIgnore]	public const string TESTING_BY_OWN_LIVESIM = "TESTING_BY_OWN_LIVESIM :: ";

		[JsonIgnore]				object				lockSubmitOrders;
		[JsonIgnore]	public		string				Name				{ get; protected set; }
		[JsonIgnore]	public		string				ReasonToExist		{ get; protected set; }
		[JsonIgnore]	public		bool				HasBacktestInName	{ get { return Name.Contains("Backtest"); } }
		[JsonIgnore]	public		Bitmap				Icon				{ get; protected set; }
		[JsonIgnore]	public		DataSource			DataSource			{ get; protected set; }
		[JsonIgnore]	public		OrderProcessor		OrderProcessor		{ get; protected set; }
		[JsonIgnore]	public		StreamingAdapter	StreamingAdapter	{ get; protected set; }
//		[JsonIgnore]	public		List<Account>		Accounts			{ get; protected set; }
		[JsonProperty]	public		Account				Account;
		[JsonIgnore]	public		Account				AccountAutoPropagate {
			get { return this.Account; }
			set {
				this.Account = value;
				this.Account.Initialize(this);
			}
		}
//		public virtual string AccountsAsString { get {
//				string ret = "";
//				foreach (Account account in this.Accounts) {
//					ret += account.AccountNumber + ":" + account.Positions.Count + "positions,";
//				}
//				ret = ret.TrimEnd(',');
//				if (ret == "") {
//					ret = "NO_ACCOUNTS";
//				}
//				return ret;
//			} }

		[JsonIgnore]	public OrderCallbackDupesChecker	OrderCallbackDupesChecker { get; protected set; }
		//[JsonIgnore]	public bool SignalToTerminateAllOrderTryFillLoopsInAllMocks = false;

		[JsonIgnore]				ConnectionState		upstreamConnectionState;
		[JsonIgnore]	public		ConnectionState		UpstreamConnectionState	{
			get { return this.upstreamConnectionState; }
			private set {		// use ConnectionState_update();
				if (this.upstreamConnectionState == value) return;	//don't invoke StateChanged if it didn't change
				if (this.upstreamConnectionState == ConnectionState.Streaming_UpstreamConnected_downstreamSubscribedAll
								&& value == ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed) {
					Assembler.PopupException("YOU_ARE_RESETTING_ORIGINAL_DATASOURCE_WITH_LIVESIM_DATASOURCE", null, false);
				}
				if (this.upstreamConnectionState == ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribed
								&& value == ConnectionState.Streaming_JustInitialized_solidifiersSubscribed) {
					Assembler.PopupException("WHAT_DID_YOU_INITIALIZE? IT_WAS_ALREADY_INITIALIZED_AND_UPSTREAM_CONNECTED", null, false);
				}
				this.upstreamConnectionState = value;
				this.RaiseOnBrokerConnectionStateChanged();	// consumed by QuikStreamingMonitorForm,QuikStreamingEditor

				try {
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
					if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
					if (this.UpstreamConnect_onAppRestart == this.UpstreamConnected) return;
					this.UpstreamConnect_onAppRestart = this.UpstreamConnected;		// you can override this.UpstreamConnectedOnAppRestart and keep it FALSE to avoid DS serialization
					if (this.DataSource == null) {
						string msg = "DataSource=null_WHEN_QUIK_FOLDER_NOT_FOUND for broker[" + this + "]";
						Assembler.PopupException(msg, null, false);
						return;
					}
					if (this is LivesimBroker) {
						string whyReturn = "I simulate ConnectionState.Broker_TerminalConnected on first order, and each next spoiledDisconnect()"
							+ "; but LivesimDataSource.json doesnt exist";
						return;
					}
					Assembler.InstanceInitialized.RepositoryJsonDataSources.SerializeSingle(this.DataSource);
				} catch (Exception ex) {
					string msg = "SOMETHING_WENT_WRONG_WHILE_SAVING_DATASOURCE_AFTER_YOU_CHANGED UpstreamConnected for streaming[" + this + "]";
					Assembler.PopupException(msg, ex);
				}
			}
		}
		[JsonProperty]	public	virtual	bool			UpstreamConnect_onAppRestart		{ get; protected set; }
		[JsonProperty]	public			bool			UpstreamConnect_onFirstOrder;//		{ get; internal  set; } internal will help if you won't throw in BrokerAdapter.PushEditedSettingsToBrokerAdapter()
		[JsonIgnore]	public			bool			UpstreamConnected					{ get {
		    bool ret = false;
		    switch (this.UpstreamConnectionState) {
		        case ConnectionState.UnknownConnectionState:						ret = false;	break;

		        case ConnectionState.Broker_DllConnected:							ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_DllConnecting:							ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_DllDisconnected:						ret = false;	break;
		        case ConnectionState.Broker_TerminalConnected:						ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_TerminalDisconnected:					ret = true;		break;	// set by callback after first order?... I want the button in Editor pressed, linked to DllConnected only

		        // used in QuikBrokerAdapter
		        case ConnectionState.Broker_Connected_SymbolsSubscribedAll:			ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_Connected_SymbolSubscribed:				ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_Connected_SymbolsUnsubscribedAll:		ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_Connected_SymbolUnsubscribed:			ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_Disconnected_SymbolsSubscribedAll:		ret = false;	break;
		        case ConnectionState.Broker_Disconnected_SymbolsUnsubscribedAll:	ret = false;	break;

		        case ConnectionState.BrokerErrorConnectingNoRetriesAnymore:			ret = false;	break;

		        // used in QuikBrokerAdapter
		        case ConnectionState.FailedToConnect:						ret = false;	break;
		        case ConnectionState.FailedToDisconnect:					ret = false;	break;		// can still be connected but by saying NotConnected I prevent other attempt to subscribe symbols; use "Connect" button to resolve

		        default:
		            Assembler.PopupException("ADD_HANDLER_FOR_NEW_ENUM_VALUE this.ConnectionState[" + this.UpstreamConnectionState + "]");
		            ret = false;
		            break;
		    }
		    return ret;
		} }

		[JsonIgnore]	public LivesimBroker	LivesimBroker_ownImplementation				{ get; protected set; }
		[JsonIgnore]	public	bool			ImBeingTested_byOwnLivesimImplementation	{ get; private set; }
		[JsonIgnore]	public	string			ImBeingTested_PREFIX						{ get { return this.ImBeingTested_byOwnLivesimImplementation ? BrokerAdapter.TESTING_BY_OWN_LIVESIM : ""; } }
		public void ImBeingTested_byOwnLivesimImplementation_set(bool setIn_SimulationPreBarsSubstitute) {
			if (this is LivesimBroker) {
				string msg = "I_REFUSE_TO_SET_ImBeingTested_BECAUSE_I_AM_A_TESTER(MASTER)_AND_YOU_MUST_ADDRESS_BROKER_ORIGINAL_FROM_DATASOURCE";
				Assembler.PopupException(msg);
				return;
			}
			this.ImBeingTested_byOwnLivesimImplementation = setIn_SimulationPreBarsSubstitute;
		}

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public BrokerAdapter() {
			this.ReasonToExist				= "DUMMY_FOR_LIST_OF_BROKER_PROVIDERS_IN_DATASOURCE_EDITOR";
			this.lockSubmitOrders			= new object();
			//Accounts = new List<Account>();
			this.AccountAutoPropagate		= new Account("ACCTNR_NOT_SET", -1000);
			this.OrderCallbackDupesChecker	= new OrderCallbackDupesCheckerTransparent(this);

			this.UpstreamConnect_onAppRestart = false;
			this.UpstreamConnect_onFirstOrder = true;

			this.EmittingCapable_mre		= new ManualResetEvent(false);
			this.EmittingIncapable_mre		= new ManualResetEvent(true);
		}

		public BrokerAdapter(string reasonToExist) : this() {
			ReasonToExist					= reasonToExist;
		}
		void checkOrder_throwIfInvalid(Order orderToCheck) {
			if (orderToCheck.Alert == null) {
				throw new Exception("order[" + orderToCheck + "].Alert == Null");
			}
			if (string.IsNullOrEmpty(orderToCheck.Alert.AccountNumber)) {
				throw new Exception("order[" + orderToCheck + "].Alert.AccountNumber IsNullOrEmpty");
			}
			//if (this.Accounts.Count == 0) {
			//	throw new Exception("No account for Order[" + orderToCheck.GUID + "]");
			//}
			if (string.IsNullOrEmpty(orderToCheck.Alert.Symbol)) {
				throw new Exception("order[" + orderToCheck + "].Alert.Symbol IsNullOrEmpty");
			}
			if (orderToCheck.Alert.Direction == null) {
				throw new Exception("order[" + orderToCheck + "].Alert.Direction IsNullOrEmpty");
			}
			if (orderToCheck.PriceEmitted == 0 &&
					(orderToCheck.Alert.MarketLimitStop == MarketLimitStop.Stop || orderToCheck.Alert.MarketLimitStop == MarketLimitStop.Limit)) {
				throw new Exception("order[" + orderToCheck + "].Price[" + orderToCheck.PriceEmitted + "] should be != 0 for Stop or Limit");
			}
		}
		public void Orders_submitOpeners_afterClosedUnlocked_threadEntry_delayed(List<Order> ordersFromAlerts, int millis) {
			if (ordersFromAlerts.Count == 0) {
				Assembler.PopupException("SubmitOrdersThreadEntry should get at least one order to place! List<Order>; got ordersFromAlerts.Count=0; returning");
				return;
			}
			string msg = "SubmitOrdersThreadEntryDelayed: sleeping [" + millis +
				"]millis before SubmitOrdersThreadEntry [" + ordersFromAlerts.Count + "]ordersFromAlerts";
			Assembler.PopupException(msg);
			ordersFromAlerts[0].appendMessage(msg);
			Thread.Sleep(millis);
			this.SubmitOrders_liveAndLiveSim_fromProcessor_OPPunlockedSequence_threadEntry(ordersFromAlerts);
		}
		public virtual void SubmitOrders_liveAndLiveSim_fromProcessor_OPPunlockedSequence_threadEntry(List<Order> ordersFromAlerts) {
			try {
				if (ordersFromAlerts.Count == 0) {
					Assembler.PopupException("SubmitOrdersThreadEntry should get at least one order to place! List<Order>; got ordersFromAlerts.Count=0; returning");
					return;
				}
				Order firstOrder = ordersFromAlerts[0];
				Assembler.SetThreadName(firstOrder.ToString(), "can not set Thread.CurrentThread.Name=[" + firstOrder + "]");
				this.SubmitOrders_ownOneThread_forAllNewAlerts(ordersFromAlerts);
			} catch (Exception e) {
				string msg = "SubmitOrdersThreadEntry default Exception Handler";
				Assembler.PopupException(msg, e);
			}
		}
		public virtual void SubmitOrders_ownOneThread_forAllNewAlerts(List<Order> orders) { lock (this.lockSubmitOrders) {
			string msig = " //" + this.Name + "::SubmitOrders()";
			List<Order> ordersToExecute = new List<Order>();
			foreach (Order order in orders) {
				if (order.Alert.IsBacktestingNoLivesimNow_FalseIfNoBacktester == true || this.HasBacktestInName) {
					string msg = "Backtesting orders should not be routed to AnyBrokerAdapters, but simulated using MarketSim; order=[" + order + "]";
					throw new Exception(msg + msig);
				}
				if (String.IsNullOrEmpty(order.Alert.AccountNumber)) {
					string msg = "IsNullOrEmpty(order.Alert.AccountNumber): order=[" + order + "]";
					throw new Exception(msg + msig);
				}
				if (order.Alert.AccountNumber.StartsWith("Paper")) {
					string msg = "NO_PAPER_ORDERS_ALLOWED: order=[" + order + "]";
					throw new Exception(msg + msig);
				}
				if (ordersToExecute.Contains(order)) {
					string msg = "REMOVED_DUPLICATED_ORDER_IN_WHAT_YOU_PASSED_TO_SubmitOrders(): order=[" + order + "]";
					Assembler.PopupException(msg + msig, null, false);
					continue;
				}
				ordersToExecute.Add(order);
			}
			foreach (Order order in ordersToExecute) {
				//string msg_order = " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "] SernoSession[" + order.SernoSession + "]";
				//this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msg_order + msig);

				try {
					this.Order_checkThrow_enrichPreSubmit(order);
				} catch (Exception ex) {
					string msg_order = " " + order.ToString();
					string msg = "CAUGHT[" + ex.Message + "] " + msg_order + msig;
					Assembler.PopupException(msg_order, ex, false);
					this.OrderProcessor.AppendMessage_propagateToGui(order, msg_order);

					if (order.State == OrderState.IRefuseOpenTillEmergencyCloses) {
						msg = "looks good, OrderPreSubmitChecker() caught the EmergencyLock exists";
						Assembler.PopupException(msg + msg_order + msig, ex, false);
						this.OrderProcessor.AppendMessage_propagateToGui(order, msg + msg_order + msig);
					}
					continue;
				}
				//this.OrderProcessor.DataSnapshot.SwitchLanes_forOrder_postStatusUpdate(order);
				this.Order_submit_oneThread_forAllNewAlerts(order);
			}
		} }

		public virtual void KillSelectedOrders(IList<Order> victimOrders) {
			foreach (Order victimOrder in victimOrders) {
				if (victimOrder.Alert.IsBacktestingNoLivesimNow_FalseIfNoBacktester == true) {
					string msg = "Backtesting orders should not be routed to MockBrokerAdapters, but simulated using MarketSim; victimOrder=[" + victimOrder + "]";
					throw new Exception(msg);
				}
				this.Order_killPending_usingKiller(victimOrder);
			}
		}

		public virtual void Order_checkThrow_enrichPreSubmit(Order order) {
			string msig = " //" + Name + "::Order_checkThrow_preSubmitEnrich():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";

			if (this.StreamingAdapter == null) {
				string msg = "StreamingAdapter=null, can't get last/fellow/crossMarket price";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg + msig);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				throw new Exception(msg + msig);
			}

			try {
				this.checkOrder_throwIfInvalid(order);
			} catch (Exception ex) {
				string msg = ex.Message + " //" + msig;
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg + msig);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				throw new Exception("Order_checkThrow_enrichPreSubmit(" + order + ")" + msig, ex);
			}


			if (order.Alert.Bars.SymbolInfo.CheckForSimilarAlreadyPending) {
				string msg_order = " " + order.ToString();

				OrderLane lane_withSimilarOrder;
				string suggestion_SimilarOrder;
				Order orderSimilar = this.OrderProcessor.DataSnapshot.OrdersPending.
					ScanRecent_forSimilarPendingOrder_notSame(order, out lane_withSimilarOrder, out suggestion_SimilarOrder);

				if (orderSimilar == null) {
					string msg1 = "SIMILAR_NOT_FOUND_IN_PENDINGS [" + suggestion_SimilarOrder + "]";
					//Assembler.PopupException(msg1 + msg_order + msig, null, false);

					if (lane_withSimilarOrder == null) lane_withSimilarOrder = this.OrderProcessor.DataSnapshot.OrdersAll;

					orderSimilar = lane_withSimilarOrder.ScanRecent_forSimilarPendingOrder_notSame(order, out lane_withSimilarOrder, out suggestion_SimilarOrder);
				}

				if (orderSimilar != null) {
					string msg = "ORDER_DUPLICATE_IN_SUBMITTED: your strategy didnt check if it has already emitted a similar order [" + order + "]"
						+ " I should drop this order since similar is not executed yet [" + orderSimilar + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(order			, msg + msg_order + msig);
					this.OrderProcessor.AppendMessage_propagateToGui(orderSimilar	, msg + msg_order + msig);
					Assembler.PopupException(msg + msig);
					//continue;
				}
			}


			order.AbsorbCurrentBidAsk_fromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);
			this.Order_enrichAlert_brokerSpecificInjection(order);

			if (order.Alert.Strategy.Script == null) return;

			string msg3 = "YEAH_THATS_CRAZY__BELOW";
			//Assembler.PopupException(msg, null, false);

			Order reason4lock = this.OrderProcessor.OPPemergency.GetReasonForLock(order);
			bool isEmergencyClosingNow = (reason4lock != null);
			//bool positionWasFilled = this.orderProcessor.positionWasFilled(order);
			bool emergencyShouldKickIn = order.Alert.IsEntryAlert && isEmergencyClosingNow;	// && positionWasFilled
			if (emergencyShouldKickIn == false) return;

			//OrderState IRefuseUntilemrgComplete = this.orderProcessor.OPPemergency.getRefusalForEmergencyState(reason4lock);
			OrderState IRefuseUntilEmrgComplete = OrderState.IRefuseOpenTillEmergencyCloses;
			string msg2 = "Reason4lock: " + reason4lock.ToString();
			OrderStateMessage omsg = new OrderStateMessage(order, IRefuseUntilEmrgComplete, msg2);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
			throw new Exception(msg2 + msig);
		}
		public virtual void Order_modifyOrderType_priceRequesting_accordingToMarketOrderAs(Order order) {
			string msig = " //" + Name + "::Modify_orderType_priceRequesting_accordingToMarketOrderAs():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			string msg = "";

			order.AbsorbCurrentBidAsk_fromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);

			Alert alert = order.Alert;
			double priceBestBidAsk = this.StreamingAdapter.StreamingDataSnapshot.GetBidOrAsk_forDirection_fromQuoteLast(
				alert.Symbol, alert.PositionLongShortFromDirection);
				
			SymbolInfo symbolInfo = alert.Bars.SymbolInfo;
			switch (alert.MarketLimitStop) {
				case MarketLimitStop.Market:
					//if (order.PriceRequested != 0) {
					//	string msg1 = Name + "::OrderSubmit(): order[" + order + "] is MARKET, dropping Price[" + order.PriceRequested + "] replacing with current Bid/Ask ";
					//	order.addMessage(new OrderStateMessage(order, msg1));
					//	Assembler.PopupException(msg1);
					//	order.PriceRequested = 0;
					//}
					if (alert.Bars == null) {
						msg = "order.Bars=null; can't align order and get Slippage; returning with error // " + msg;
						Assembler.PopupException(msg);
						//order.AppendMessageAndChangeState(new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg));
						this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg));
						throw new Exception(msg);
					}

					switch (alert.MarketOrderAs) {
						case MarketOrderAs.MarketZeroSentToBroker:
							order.PriceEmitted = 0;
							msg = "SYMBOL_INFO_CONVERSION_MarketZeroSentToBroker SymbolInfo[" + alert.Symbol + "/" + alert.SymbolClass + "].OverrideMarketPriceToZero==true"
								+ "; setting Price=0 (Slippage=" + order.SlippageApplied + ")";
							break;

						case MarketOrderAs.MarketMinMaxSentToBroker:
							MarketLimitStop beforeBrokerSpecific = alert.MarketLimitStop;
							string brokerSpecificDetails = this.Order_modifyType_accordingToMarketOrder_asBrokerSpecificInjection(order);
							if (brokerSpecificDetails != "") {
								msg = "BROKER_SPECIFIC_CONVERSION_MarketMinMaxSentToBroker: [" + beforeBrokerSpecific + "]=>[" + alert.MarketLimitStop + "](" + alert.MarketOrderAs
									+ ") brokerSpecificDetails[" + brokerSpecificDetails + "]";
							} else {
								alert.MarketLimitStop = MarketLimitStop.Limit;
								alert.MarketLimitStopAsString += " => " + alert.MarketLimitStop + " (" + alert.MarketOrderAs + ")";
								msg = "SYMBOL_INFO_CONVERSION_MarketMinMaxSentToBroker: [" + beforeBrokerSpecific + "]=>[" + alert.MarketLimitStop + "](" + alert.MarketOrderAs + ")";
							}
							break;

						case MarketOrderAs.LimitCrossMarket:
							alert.MarketLimitStop = MarketLimitStop.Limit;
							alert.MarketLimitStopAsString += " => " + alert.MarketLimitStop + " (" + alert.MarketOrderAs + ")";
							msg = "";
							if (alert.Slippage_maxIndex_forLimitOrdersOnly > 0) {
								//double slippage = symbolInfo.GetSlippage_signAware_forLimitAlertsOnly(alert, 0);
								double slippage = alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(0);
								order.PriceEmitted += slippage;
								msg += "ADDED_FIRST_SLIPPAGE[" + slippage + "]";
								if (order.Alert.PriceEmitted != order.PriceEmitted) {
									msg += "!=Alert.PriceRequested[" + order.Alert.PriceEmitted + "]";
								}
							} else {
								msg += "SLIPPAGES_NOT_DEFINED__SymbolInfo[" + symbolInfo.Symbol + "].SlippagesCrossMarketCsv";
							}
							msg += " PreSubmit_LimitCrossMarket: Alert.MarketOrderAs=[" + alert.MarketOrderAs + "] ";
							break;

						case MarketOrderAs.LimitTidal:
							alert.MarketLimitStop = MarketLimitStop.Limit;
							alert.MarketLimitStopAsString += " => " + alert.MarketLimitStop + " (" + alert.MarketOrderAs + ")";
							msg = "";
							if (alert.Slippage_maxIndex_forLimitOrdersOnly > 0) {
								//double slippage = symbolInfo.GetSlippage_signAware_forLimitAlertsOnly(alert, 0);
								double slippage = alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(0);
								order.PriceEmitted += slippage;
								msg += "ADDED_FIRST_SLIPPAGE[" + slippage + "]";
								if (order.Alert.PriceEmitted != order.PriceEmitted) {
									msg += "!=Alert.PriceRequested[" + order.Alert.PriceEmitted + "]";
								}
							} else {
								msg += "DOING_NOTHING__AS_SymbolInfo[" + symbolInfo.Symbol + "].SlippagesTidalCsv_ARE_NOT_DEFINED";
							}
							msg += " PreSubmit_LimitTidal: Alert.MarketOrderAs=[" + alert.MarketOrderAs + "] ";
							break;

						default:
							msg = "no handler for Market Order with Alert.MarketOrderAs[" + alert.MarketOrderAs + "]";
							OrderStateMessage newOrderState2 = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
							this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState2);
							throw new Exception(msg);
					}
					//if (alert.Bars.SymbolInfo.OverrideMarketPriceToZero == true) {
					//} else {
					//	if (order.PriceRequested == 0) {
					//		base.StreamingAdapter.StreamingDataSnapshot.BidOrAsk_getAligned_forTidalOrCrossMarket_fromStreamingSnap(
					//			alert.Symbol, alert.Direction, out order.PriceRequested, out order.SpreadSide, ???);
					//		order.PriceRequested += order.Slippage;
					//		order.PriceRequested = alert.Bars.alignOrderPriceToPriceLevel(order.PriceRequested, alert.Direction, alert.MarketLimitStop);
					//	}
					//}
					//order.addMessage(new OrderStateMessage(order, msg));
					//Assembler.PopupException(msg);
					break;

				case MarketLimitStop.Limit:
					order.SpreadSide = SpreadSide.ERROR;
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk <= order.PriceEmitted) order.SpreadSide = SpreadSide.BidTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk >= order.PriceEmitted) order.SpreadSide = SpreadSide.AskTidal;
							break;
						default:
							msg += " No Direction[" + alert.Direction + "] handler for order[" + order.ToString() + "]"
								+ "; must be one of those: Buy/Cover/Sell/Short";
							//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Error, msg);
							this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
							throw new Exception(msg);
					}
					break;

				case MarketLimitStop.Stop:
				case MarketLimitStop.StopLimit:
					order.SpreadSide = SpreadSide.ERROR;
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk >= order.PriceEmitted) order.SpreadSide = SpreadSide.AskTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk <= order.PriceEmitted) order.SpreadSide = SpreadSide.BidTidal;
							break;
						default:
							msg += " No Direction[" + alert.Direction + "] handler for order[" + order.ToString() + "]"
								+ "; must be one of those: Buy/Cover/Sell/Short";
							//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Error, msg);
							this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
							throw new Exception(msg);
					}
					break;

				default:
					msg += " No MarketLimitStop[" + alert.MarketLimitStop + "] handler for order[" + order.ToString() + "]"
						+ "; must be one of those: Market/Limit/Stop";
					//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
					OrderStateMessage omsg = new OrderStateMessage(order, OrderState.Error, msg);
					this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
					throw new Exception(msg);
			}
			order.appendMessage(msg + msig);
		}

		public Order ScanEvidentLanes_forGuid_nullUnsafe(string GUID, List<OrderLane> orderLanes = null, char separator = ';') {
			string msig = " //" + this.Name;
			string orderLanesSearchedAsString = "";
			Order orderFound = null;
			
			if (orderLanes == null) {
				var snap = this.OrderProcessor.DataSnapshot;
				orderLanes = new List<OrderLane>() {snap.OrdersPending, snap.OrdersSubmitting, snap.OrdersAll};
			}
			foreach (OrderLane orderLane in orderLanes) {
				orderLanesSearchedAsString += orderLane.GetType().Name.Substring(5) + separator;	// removing "Orders" from "OrdersSubmitting"
				OrderLane	suggestedLane = null;
				string		suggestion = "PASS_suggestLane=TRUE";
				orderFound = orderLane.ScanRecent_forGuid(GUID, out suggestedLane, out suggestion, false);
				if (orderFound != null) break;
			}
			orderLanesSearchedAsString = orderLanesSearchedAsString.TrimEnd(separator);
			msig += ".FindOrderLaneOptimized_nullUnsafe(" + GUID + "),orderLanesSearchedAsString[" + orderLanesSearchedAsString + "]";
			
			if (orderFound == null) {
				string msg = "PENDING_ORDER_NOT_FOUND__RETURNING_ORDER_NULL";
				//msg		  += "; OrdersAll.SessionSernos[" + this.OrderProcessor.DataSnapshot.OrdersAll.SessionSernos + "]";
				Assembler.PopupException(msg + msig, null, true);
				return null; 
			}
			if (orderFound.Alert == null) {
				string msg = "ORDER_FOUND_HAS_ALERT_NULL_UNRECOVERABLE__RETURNING_ORDER_NULL orderFound[" + orderFound.ToString() + "]";
				Assembler.PopupException(msg + msig);
				return null; 
			}
//			if (orderFound.Alert.DataSource == null) {
//				string msg = "ORDER_FOUND_HAS_DATASOURCE_NULL__ASSIGNING_MINE orderFound[" + orderFound.ToString()
//					+ "] this.DataSource[" + this.DataSource.ToString() + "]";
//				Assembler.PopupException(msg + msig);
//				orderFound.Alert.DataSource = this.DataSource;
//			}
			if (orderFound.Alert.Bars == null) {
				string msg = "UNREPAIRABLE__ORDER_FOUND_HAS_Bars_NULL orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, null, false);
				//orderFound.Alert.Bars = orderFound.;
				return orderFound;
			}
			if (orderFound.Alert.DataSource_fromBars == null) {
				string msg = "UNREPAIRABLE__ORDER_FOUND_HAS_DataSource_NULL orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, null, false);
				//orderFound.Alert.DataSource = this.DataSource;
				return orderFound;
			}
			if (orderFound.Alert.DataSource_fromBars.BrokerAdapter == null) {
				string msg = "ORDER_FOUND_HAS_BROKER_NULL__ASSIGNING_MYSELF orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, null, false);
				orderFound.Alert.DataSource_fromBars.BrokerAdapter = this;
			}
			return orderFound;
		}

		public override string ToString() {
			string dataSourceAsString = this.DataSource != null ? this.DataSource.ToString() : "NOT_INITIALIZED_YET";
			string ret = this.Name + "/[" + this.UpstreamConnectionState + "]"
				//+ ": UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]"
				//+ "DataSource[" + dataSourceAsString + "]"
				+ " (" + this.ReasonToExist + ")"
				;
			return ret;
		}
		
		[JsonIgnore]	public bool IsDisposed { get; private set; }
		public virtual void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
		}



		[JsonIgnore]	public		abstract bool		EmittingCapable			{ get; }	// always false, override?
		[JsonIgnore]	protected	ManualResetEvent	EmittingCapable_mre;
		[JsonIgnore]	protected	ManualResetEvent	EmittingIncapable_mre;

		public bool ConnectionState_waitFor_emittingCapable(int waitMillis = -1) {
			bool capable =	this.EmittingCapable_mre.WaitOne(waitMillis);
			if (capable)	this.EmittingCapable_mre.Reset();		// it's a MANUAL reset, not AUTO
			return capable;
		}
		public bool ConnectionState_waitFor_emittingIncapable(int waitMillis = -1) {
		    bool incapable =	this.EmittingIncapable_mre.WaitOne(waitMillis);
			if (incapable)		this.EmittingIncapable_mre.Reset();		// it's a MANUAL reset, not AUTO
		    return incapable;
		}
		public void ConnectionState_update(ConnectionState state, string message) {
			this.UpstreamConnectionState = state;
			Assembler.DisplayConnectionStatus(state, message);
			if (this.EmittingCapable) {
				this.EmittingCapable_mre.Set();
				this.EmittingIncapable_mre.Reset();
			} else {
			    bool incapable = this.EmittingIncapable_mre.WaitOne(0);
				this.EmittingIncapable_mre.Reset();
				this.EmittingCapable_mre.Set();
			}
		}

		public void SerializeDatasource_streamingBrokerMarketInfo() {
			if (this.DataSource == null) {
				string msg = "DATASOURCE_NULL_FOR_BROKER__CAN_NOT_SAVE [" + this + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.DataSource.Serialize();
		}
	}
}