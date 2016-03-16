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
	public partial class BrokerAdapter {
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

		[JsonIgnore]				ConnectionState			upstreamConnectionState;
		[JsonIgnore]	public		ConnectionState			UpstreamConnectionState	{
			get { return this.upstreamConnectionState; }
			protected set {
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
					if (this.UpstreamConnectedOnAppRestart == this.UpstreamConnected) return;
					this.UpstreamConnectedOnAppRestart = this.UpstreamConnected;		// you can override this.UpstreamConnectedOnAppRestart and keep it FALSE to avoid DS serialization
					if (this.DataSource == null) {
						string msg = "SHOULD_NEVER_HAPPEN DataSource=null for streaming[" + this + "]";
						Assembler.PopupException(msg);
						return;
					}
					Assembler.InstanceInitialized.RepositoryJsonDataSources.SerializeSingle(this.DataSource);
				} catch (Exception ex) {
					string msg = "SOMETHING_WENT_WRONG_WHILE_SAVING_DATASOURCE_AFTER_YOU_CHANGED UpstreamConnected for streaming[" + this + "]";
					Assembler.PopupException(msg);
				}
			}
		}
		[JsonProperty]	public	virtual	bool				UpstreamConnectedOnAppRestart		{ get; protected set; }
		[JsonIgnore]	public		bool					UpstreamConnected					{ get {
		    bool ret = false;
		    switch (this.UpstreamConnectionState) {
		        case ConnectionState.UnknownConnectionState:						ret = false;	break;

		        case ConnectionState.Broker_DllConnected:							ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_DllConnecting:							ret = true;		break;	// will trigger UpstreamConnect OnAppRestart
		        case ConnectionState.Broker_DllDisconnected:							ret = false;	break;
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
		[JsonIgnore]	public LivesimBroker			LivesimBroker_ownImplementation			{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public BrokerAdapter() {
			ReasonToExist					= "DUMMY_FOR_LIST_OF_BROKER_PROVIDERS_IN_DATASOURCE_EDITOR";
			//Accounts = new List<Account>();
			this.lockSubmitOrders			= new object();
			this.AccountAutoPropagate		= new Account("ACCTNR_NOT_SET", -1000);
			this.OrderCallbackDupesChecker	= new OrderCallbackDupesCheckerTransparent(this);
		}

		public BrokerAdapter(string reasonToExist) : this() {
			ReasonToExist					= reasonToExist;
		}
		protected void checkOrder_throwIfInvalid(Order orderToCheck) {
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
			if (orderToCheck.PriceRequested == 0 &&
					(orderToCheck.Alert.MarketLimitStop == MarketLimitStop.Stop || orderToCheck.Alert.MarketLimitStop == MarketLimitStop.Limit)) {
				throw new Exception("order[" + orderToCheck + "].Price[" + orderToCheck.PriceRequested + "] should be != 0 for Stop or Limit");
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
			ordersFromAlerts[0].AppendMessage(msg);
			Thread.Sleep(millis);
			this.SubmitOrders_backtest_liveFromProcessor_OPPunlockedSequence_threadEntry(ordersFromAlerts);
		}
		public virtual void SubmitOrders_backtest_liveFromProcessor_OPPunlockedSequence_threadEntry(List<Order> ordersFromAlerts) {
			try {
				if (ordersFromAlerts.Count == 0) {
					Assembler.PopupException("SubmitOrdersThreadEntry should get at least one order to place! List<Order>; got ordersFromAlerts.Count=0; returning");
					return;
				}
				Order firstOrder = ordersFromAlerts[0];
				Assembler.SetThreadName(firstOrder.ToString(), "can not set Thread.CurrentThread.Name=[" + firstOrder + "]");
				this.SubmitOrders(ordersFromAlerts);
			} catch (Exception e) {
				string msg = "SubmitOrdersThreadEntry default Exception Handler";
				Assembler.PopupException(msg, e);
			}
		}
		public virtual void SubmitOrders(IList<Order> orders) { lock (this.lockSubmitOrders) {
			string msig = this.Name + "::SubmitOrders(): ";
			List<Order> ordersToExecute = new List<Order>();
			foreach (Order order in orders) {
				if (order.Alert.IsBacktestingNoLivesimNow_FalseIfNoBacktester == true || this.HasBacktestInName) {
					string msg = "Backtesting orders should not be routed to AnyBrokerAdapters, but simulated using MarketSim; order=[" + order + "]";
					throw new Exception(msg);
				}
				if (String.IsNullOrEmpty(order.Alert.AccountNumber)) {
					string msg = "IsNullOrEmpty(order.Alert.AccountNumber): order=[" + order + "]";
					throw new Exception(msg);
				}
				if (order.Alert.AccountNumber.StartsWith("Paper")) {
					string msg = "NO_PAPER_ORDERS_ALLOWED: order=[" + order + "]";
					throw new Exception(msg);
				}
				if (ordersToExecute.Contains(order)) {
					string msg = "ORDER_DUPLICATE_IN_NEW: order=[" + order + "]";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				ordersToExecute.Add(order);
			}
			foreach (Order order in ordersToExecute) {
				string msg = "Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
					+ " SernoSession[" + order.SernoSession + "]";
				this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msig + msg);

				//Order orderSimilar = this.OrderProcessor.DataSnapshot.OrdersPending.FindSimilarNotSamePendingOrder(order);
				//// Orders.All.ContainForSure: Order orderSimilar = this.OrderProcessor.DataSnapshot.OrdersAll.FindSimilarNotSamePendingOrder(order);
				//if (orderSimilar != null) {
				//	msg = "ORDER_DUPLICATE_IN_SUBMITTED: dropping order [" + order + "] (not submitted) since similar is not executed yet [" + orderSimilar + "] " + msg;
				//	this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
				//	this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderSimilar, msig + msg);
				//	this.OrderProcessor.PopupException(new Exception(msig + msg));
				//	continue;
				//}
				try {
					this.Order_checkThrow_enrichPreSubmit(order);
				} catch (Exception ex) {
					Assembler.PopupException(msg, ex, false);
					this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msig + ex.Message + " //" + msg);
					if (order.State == OrderState.IRefuseOpenTillEmergencyCloses) {
						msg = "looks good, OrderPreSubmitChecker() caught the EmergencyLock exists";
						Assembler.PopupException(msg + msig, ex, false);
						this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msig + msg);
					}
					continue;
				}
				//this.OrderProcessor.DataSnapshot.MoveAlongStateLists(order);
				this.Order_submit(order);
			}
		} }

		public virtual void KillSelectedOrders(IList<Order> victimOrders) {
			foreach (Order victimOrder in victimOrders) {
				if (victimOrder.Alert.IsBacktestingNoLivesimNow_FalseIfNoBacktester == true) {
					string msg = "Backtesting orders should not be routed to MockBrokerAdapters, but simulated using MarketSim; victimOrder=[" + victimOrder + "]";
					throw new Exception(msg);
				}
				this.Order_kill_dispatcher(victimOrder);
			}
		}

		public virtual void Order_checkThrow_enrichPreSubmit(Order order) {
			string msg = Name + "::Order_checkThrow_preSubmitEnrich():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			if (this.StreamingAdapter == null) {
				msg = " StreamingAdapter=null, can't get last/fellow/crossMarket price // " + msg;
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				throw new Exception(msg);
			}
			try {
				this.checkOrder_throwIfInvalid(order);
			} catch (Exception ex) {
				msg = ex.Message + " //" + msg;
				//orderProcessor.updateOrderStatusError(order, OrderState.ErrorOrderInconsistent, msg);
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				throw new Exception(msg, ex);
			}

			order.AbsorbCurrentBidAsk_fromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);
			this.Order_enrichAlert_brokerSpecificInjection(order);

			if (order.Alert.Strategy.Script == null) return;

			string msg1 = "YEAH_THATS_CRAZY__BELOW";
			//Assembler.PopupException(msg, null, false);

			Order reason4lock = this.OrderProcessor.OPPemergency.GetReasonForLock(order);
			bool isEmergencyClosingNow = (reason4lock != null);
			//bool positionWasFilled = this.orderProcessor.positionWasFilled(order);
			bool emergencyShouldKickIn = order.Alert.IsEntryAlert && isEmergencyClosingNow;	// && positionWasFilled
			if (emergencyShouldKickIn == false) return;

			//OrderState IRefuseUntilemrgComplete = this.orderProcessor.OPPemergency.getRefusalForEmergencyState(reason4lock);
			OrderState IRefuseUntilEmrgComplete = OrderState.IRefuseOpenTillEmergencyCloses;
			msg = "Reason4lock: " + reason4lock.ToString();
			OrderStateMessage omsg = new OrderStateMessage(order, IRefuseUntilEmrgComplete, msg);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
			throw new Exception(msg);
		}
		public virtual void Order_modifyOrderType_priceRequesting_accordingToMarketOrderAs(Order order) {
			string msig = " //" + Name + "::Modify_orderType_priceRequesting_accordingToMarketOrderAs():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			string msg = "";

			order.AbsorbCurrentBidAsk_fromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);

			Alert alert = order.Alert;
			double priceBestBidAsk = this.StreamingAdapter.StreamingDataSnapshot.BidOrAsk_forDirection(
				order.Alert.Symbol, alert.PositionLongShortFromDirection);
				
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
							order.PriceRequested = 0;
							msg = "SYMBOL_INFO_CONVERSION_MarketZeroSentToBroker SymbolInfo[" + alert.Symbol + "/" + alert.SymbolClass + "].OverrideMarketPriceToZero==true"
								+ "; setting Price=0 (Slippage=" + order.SlippageFill + ")";
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
							if (symbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(alert) > 0) {
								double slippage = symbolInfo.GetSlippage_signAware_forLimitOrdersOnly(alert, 0);
								order.PriceRequested += slippage;
								msg += "ADDED_FIRST_SLIPPAGE[" + slippage + "]";
							} else {
								msg += "DOING_NOTHING__AS_SymbolInfo[" + symbolInfo.Symbol + "].SlippagesCrossMarketCsv_ARE_NOT_DEFINED";
							}
							msg += " PreSubmit_LimitCrossMarket: Alert.MarketOrderAs=[" + alert.MarketOrderAs + "] ";
							break;

						case MarketOrderAs.LimitTidal:
							alert.MarketLimitStop = MarketLimitStop.Limit;
							alert.MarketLimitStopAsString += " => " + alert.MarketLimitStop + " (" + alert.MarketOrderAs + ")";
							msg = "";
							if (symbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(alert) > 0) {
								double slippage = symbolInfo.GetSlippage_signAware_forLimitOrdersOnly(alert, 0);
								order.PriceRequested += slippage;
								msg += "ADDED_FIRST_SLIPPAGE[" + slippage + "]";
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
					order.SpreadSide = OrderSpreadSide.ERROR;
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk <= order.PriceRequested) order.SpreadSide = OrderSpreadSide.BidTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk >= order.PriceRequested) order.SpreadSide = OrderSpreadSide.AskTidal;
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
					order.SpreadSide = OrderSpreadSide.ERROR;
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk >= order.PriceRequested) order.SpreadSide = OrderSpreadSide.AskTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk <= order.PriceRequested) order.SpreadSide = OrderSpreadSide.BidTidal;
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
			order.AppendMessage(msg + msig);
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
			if (orderFound.Alert.DataSource == null) {
				string msg = "UNREPAIRABLE__ORDER_FOUND_HAS_DataSource_NULL orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, null, false);
				//orderFound.Alert.DataSource = this.DataSource;
				return orderFound;
			}
			if (orderFound.Alert.DataSource.BrokerAdapter == null) {
				string msg = "ORDER_FOUND_HAS_BROKER_NULL__ASSIGNING_MYSELF orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, null, false);
				orderFound.Alert.DataSource.BrokerAdapter = this;
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
	}
}