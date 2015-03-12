using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using Newtonsoft.Json;
using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter {
		public const string NO_BROKER_ADAPTER = "--- No Broker Adapter ---";

		[JsonIgnore]		   object				lockSubmitOrders;
		[JsonIgnore]	public string				Name				{ get; protected set; }
		[JsonIgnore]	public bool					HasMockInName		{ get { return Name.Contains("Mock"); } }
		[JsonIgnore]	public bool					HasBacktestInName	{ get { return Name.Contains("Backtest"); } }
		[JsonIgnore]	public Bitmap				Icon				{ get; protected set; }
		[JsonIgnore]	public DataSource			DataSource			{ get; protected set; }
		[JsonIgnore]	public OrderProcessor		OrderProcessor		{ get; protected set; }
		[JsonIgnore]	public StreamingAdapter	StreamingAdapter	{ get; protected set; }
//		[JsonIgnore]	public List<Account>		Accounts			{ get; protected set; }
		[JsonProperty]	public Account				Account;
		[JsonIgnore]	public Account				AccountAutoPropagate {
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

		[JsonIgnore]	public OrderCallbackDupesChecker OrderCallbackDupesChecker { get; protected set; }
		[JsonIgnore]	public bool SignalToTerminateAllOrderTryFillLoopsInAllMocks = false;

		public BrokerAdapter() {
			//Accounts = new List<Account>();
			this.lockSubmitOrders = new object();
			this.AccountAutoPropagate = new Account("ACCTNR_NOT_SET", -1000);
			this.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerTransparent(this);
		}
		public virtual void Initialize(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			this.DataSource = dataSource;
			this.StreamingAdapter = streamingAdapter;
			this.OrderProcessor = orderProcessor;
			this.AccountAutoPropagate.Initialize(this);
		}
		public virtual void Connect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Disconnect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}
		protected void checkOrderThrowInvalid(Order orderToCheck) {
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
		public void SubmitOrdersThreadEntryDelayed(object ordersMillisAsObject) {
			object[] ordersMillisObjectArray = (object[])ordersMillisAsObject;
			if (ordersMillisObjectArray.Length < 2) {
				Assembler.PopupException("SubmitOrdersThreadEntryDelayed should contain an array of 2 elements: List<Order> and millis; got ordersObjectArray.Length<2; returning");
				return;
			}
			List<Order> ordersFromAlerts = (List<Order>)ordersMillisObjectArray[0];
			if (ordersFromAlerts.Count == 0) {
				Assembler.PopupException("SubmitOrdersThreadEntry should get at least one order to place! List<Order>; got ordersFromAlerts.Count=0; returning");
				return;
			}
			int millis = (int)ordersMillisObjectArray[1];
			string msg = "SubmitOrdersThreadEntryDelayed: sleeping [" + millis +
				"]millis before SubmitOrdersThreadEntry [" + ordersFromAlerts.Count + "]ordersFromAlerts";
			Assembler.PopupException(msg);
			ordersFromAlerts[0].AppendMessage(msg);
			Thread.Sleep(millis);
			this.SubmitOrdersThreadEntry(ordersMillisAsObject);
		}
		public virtual void SubmitOrdersThreadEntry(object ordersAsObject) {
			try {
				object[] ordersObjectArray = (object[])ordersAsObject;
				if (ordersObjectArray.Length < 1) {
					Assembler.PopupException("SubmitOrdersThreadEntry should get first element of array as List<Order>; got ordersObjectArray.Length<1; returning");
					return;
				}
				List<Order> ordersFromAlerts = (List<Order>)ordersObjectArray[0];
				if (ordersFromAlerts.Count == 0) {
					Assembler.PopupException("SubmitOrdersThreadEntry should get at least one order to place! List<Order>; got ordersFromAlerts.Count=0; returning");
					return;
				}
				Order firstOrder = ordersFromAlerts[0];
				try {
					if (Thread.CurrentThread.Name != firstOrder.ToString()) Thread.CurrentThread.Name = firstOrder.ToString();
				} catch (Exception e) {
					Assembler.PopupException("can not set Thread.CurrentThread.Name=[" + firstOrder + "]", e);
				}
				this.SubmitOrders(ordersFromAlerts);
			} catch (Exception e) {
				string msg = "SubmitOrdersThreadEntry default Exception Handler";
				Assembler.PopupException(msg, e);
			}
		}
		public virtual void SubmitOrders(IList<Order> orders) {
			string msig = this.Name + "::SubmitOrders(): ";
			List<Order> ordersToExecute = new List<Order>();
			foreach (Order order in orders) {
				if (order.Alert.IsExecutorBacktestingNow == true || this.HasBacktestInName) {
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
			lock (this.lockSubmitOrders) {
				foreach (Order order in ordersToExecute) {
					string msg = "Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
						+ " SernoSession[" + order.SernoSession + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);

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
						this.OrderPreSubmitEnrichCheckThrow(order);
					} catch (Exception ex) {
						Assembler.PopupException(msg, ex, false);
						this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + ex.Message + " //" + msg);
						if (order.State == OrderState.IRefuseOpenTillEmergencyCloses) {
							msg = "looks good, OrderPreSubmitChecker() caught the EmergencyLock exists";
							Assembler.PopupException(msg + msig, ex, false);
							this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
						}
						continue;
					}
					//this.OrderProcessor.DataSnapshot.MoveAlongStateLists(order);
					this.OrderSubmit(order);
				}
			}
		}
		public virtual void OrderSubmit(Order order) {
			throw new Exception("please override BrokerAdapter::SubmitOrder() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void CancelReplace(Order order, Order newOrder) {
			throw new Exception("please override BrokerAdapter::CancelReplace() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void KillSelectedOrders(IList<Order> victimOrders) {
			foreach (Order victimOrder in victimOrders) {
				if (victimOrder.Alert.IsExecutorBacktestingNow == true) {
					string msg = "Backtesting orders should not be routed to MockBrokerAdapters, but simulated using MarketSim; victimOrder=[" + victimOrder + "]";
					throw new Exception(msg);
				}
				this.OrderKillSubmit(victimOrder);
			}
		}
		//public virtual void KillAll(IList<Order> victimOrders) {
		//	foreach (Order victimOrder in victimOrders) {
		//		this.SubmitKillOrder(victimOrder);
		//	}
		//}
		[Obsolete("use OrderKillSubmitUsingKillerOrder instead; Execution will visualise the state of victim and killer separately")]
		public virtual void OrderKillSubmit(Order victimOrder) {
			throw new Exception("please override BrokerAdapter::OrderKillSubmit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void OrderKillSubmitUsingKillerOrder(Order killerOrder) {
			if (string.IsNullOrEmpty(killerOrder.VictimGUID)) {
				throw new Exception("killerOrder.KillerForGUID=EMPTY");
			}
			if (killerOrder.VictimToBeKilled == null) {
				throw new Exception("killerOrder.VictimToBeKilled=null");
			}

			string msg = "State[" + killerOrder.State + "]"
				+ " [" + killerOrder.Alert.Symbol + "/" + killerOrder.Alert.SymbolClass + "]"
				+ " VictimToBeKilled.SernoExchange[" + killerOrder.VictimToBeKilled.SernoExchange + "]";
			msg = Name + "::UsingKillerOrder(): " + msg;
			Assembler.PopupException(msg);
			OrderStateMessage omsgKiller = new OrderStateMessage(killerOrder, OrderState.KillerPreSubmit, msg);
			this.OrderProcessor.UpdateOrderStateAndPostProcess(killerOrder, omsgKiller);

			this.OrderKillSubmit(killerOrder.VictimToBeKilled);
		}

		public virtual void OrderPreSubmitEnrichBrokerSpecificInjection(Order order) {
		}
		public virtual void OrderPreSubmitEnrichCheckThrow(Order order) {
			string msg = Name + "::OrderPreSubmitChecker():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			if (this.StreamingAdapter == null) {
				msg = " StreamingAdapter=null, can't get last/fellow/crossMarket price // " + msg;
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
				throw new Exception(msg);
			}
			try {
				this.checkOrderThrowInvalid(order);
			} catch (Exception ex) {
				msg = ex.Message + " //" + msg;
				//orderProcessor.updateOrderStatusError(order, OrderState.ErrorOrderInconsistent, msg);
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
				throw new Exception(msg, ex);
			}

			order.AbsorbCurrentBidAskFromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);

			this.OrderPreSubmitEnrichBrokerSpecificInjection(order);

			// moved to orderProcessor::CreatePropagateOrderFromAlert()
			// this.ModifyOrderTypeAccordingToMarketOrderAs(order);

			if (order.Alert.Strategy.Script != null) {
				Order reason4lock = this.OrderProcessor.OPPemergency.GetReasonForLock(order);
				bool isEmergencyClosingNow = (reason4lock != null);
				//bool positionWasFilled = this.orderProcessor.positionWasFilled(order);
				if (order.Alert.IsEntryAlert && isEmergencyClosingNow) {	// && positionWasFilled
					//OrderState IRefuseUntilemrgComplete = this.orderProcessor.OPPemergency.getRefusalForEmergencyState(reason4lock);
					OrderState IRefuseUntilemrgComplete = OrderState.IRefuseOpenTillEmergencyCloses;
					msg = "Reason4lock: " + reason4lock.ToString();
					OrderStateMessage omsg = new OrderStateMessage(order, IRefuseUntilemrgComplete, msg);
					this.OrderProcessor.UpdateOrderStateAndPostProcess(order, omsg);
					throw new Exception(msg);
				}
			}
		}

		public virtual string ModifyOrderTypeAccordingToMarketOrderAsBrokerSpecificInjection(Order order) {
			return "";
		}
		public virtual void ModifyOrderTypeAccordingToMarketOrderAs(Order order) {
			string msig = " //" + Name + "::ModifyOrderTypeAccordingToMarketOrderAs():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			string msg = "";

			order.AbsorbCurrentBidAskFromStreamingSnapshot(this.StreamingAdapter.StreamingDataSnapshot);

			double priceBestBidAsk = this.StreamingAdapter.StreamingDataSnapshot.BidOrAskFor(
				order.Alert.Symbol, order.Alert.PositionLongShortFromDirection);
				
			switch (order.Alert.MarketLimitStop) {
				case MarketLimitStop.Market:
					//if (order.PriceRequested != 0) {
					//	string msg1 = Name + "::OrderSubmit(): order[" + order + "] is MARKET, dropping Price[" + order.PriceRequested + "] replacing with current Bid/Ask ";
					//	order.addMessage(new OrderStateMessage(order, order.State, msg1));
					//	Assembler.PopupException(msg1);
					//	order.PriceRequested = 0;
					//}
					if (order.Alert.Bars == null) {
						msg = "order.Bars=null; can't align order and get Slippage; returning with error // " + msg;
						Assembler.PopupException(msg);
						//order.AppendMessageAndChangeState(new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg));
						this.OrderProcessor.UpdateOrderStateAndPostProcess(order, new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg));
						throw new Exception(msg);
					}

					switch (order.Alert.MarketOrderAs) {
						case MarketOrderAs.MarketZeroSentToBroker:
							order.PriceRequested = 0;
							msg = "SYMBOL_INFO_CONVERSION_MarketZeroSentToBroker SymbolInfo[" + order.Alert.Symbol + "/" + order.Alert.SymbolClass + "].OverrideMarketPriceToZero==true"
								+ "; setting Price=0 (Slippage=" + order.SlippageFill + ")";
							break;
						case MarketOrderAs.MarketMinMaxSentToBroker:
							MarketLimitStop beforeBrokerSpecific = order.Alert.MarketLimitStop;
							string brokerSpecificDetails = this.ModifyOrderTypeAccordingToMarketOrderAsBrokerSpecificInjection(order);
							if (brokerSpecificDetails != "") {
								msg = "BROKER_SPECIFIC_CONVERSION_MarketMinMaxSentToBroker: [" + beforeBrokerSpecific + "]=>[" + order.Alert.MarketLimitStop + "](" + order.Alert.MarketOrderAs
									+ ") brokerSpecificDetails[" + brokerSpecificDetails + "]";
							} else {
								order.Alert.MarketLimitStop = MarketLimitStop.Limit;
								order.Alert.MarketLimitStopAsString += " => " + order.Alert.MarketLimitStop + " (" + order.Alert.MarketOrderAs + ")";
								msg = "SYMBOL_INFO_CONVERSION_MarketMinMaxSentToBroker: [" + beforeBrokerSpecific + "]=>[" + order.Alert.MarketLimitStop + "](" + order.Alert.MarketOrderAs + ")";
							}
							break;
						case MarketOrderAs.LimitCrossMarket:
							order.Alert.MarketLimitStop = MarketLimitStop.Limit;
							order.Alert.MarketLimitStopAsString += " => " + order.Alert.MarketLimitStop + " (" + order.Alert.MarketOrderAs + ")";
							msg = "PreSubmit_LimitCrossMarket: doing nothing for Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]";
							break;
						case MarketOrderAs.LimitTidal:
							order.Alert.MarketLimitStop = MarketLimitStop.Limit;
							order.Alert.MarketLimitStopAsString += " => " + order.Alert.MarketLimitStop + " (" + order.Alert.MarketOrderAs + ")";
							msg = "PreSubmit_LimitTidal: doing nothing for Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]";
							break;
						default:
							msg = "no handler for Market Order with Alert.MarketOrderAs[" + order.Alert.MarketOrderAs + "]";
							OrderStateMessage newOrderState2 = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
							this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState2);
							throw new Exception(msg);
					}
					//if (order.Alert.Bars.SymbolInfo.OverrideMarketPriceToZero == true) {
					//} else {
					//	if (order.PriceRequested == 0) {
					//		base.StreamingAdapter.StreamingDataSnapshot.getAlignedBidOrAskTidalOrCrossMarketFromStreaming(
					//			order.Alert.Symbol, order.Alert.Direction, out order.PriceRequested, out order.SpreadSide, ???);
					//		order.PriceRequested += order.Slippage;
					//		order.PriceRequested = order.Alert.Bars.alignOrderPriceToPriceLevel(order.PriceRequested, order.Alert.Direction, order.Alert.MarketLimitStop);
					//	}
					//}
					//order.addMessage(new OrderStateMessage(order, order.State, msg));
					//Assembler.PopupException(msg);
					break;

				case MarketLimitStop.Limit:
					order.SpreadSide = OrderSpreadSide.ERROR;
					switch (order.Alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk <= order.PriceRequested) order.SpreadSide = OrderSpreadSide.BidTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk >= order.PriceRequested) order.SpreadSide = OrderSpreadSide.AskTidal;
							break;
						default:
							msg += " No Direction[" + order.Alert.Direction + "] handler for order[" + order.ToString() + "]"
								+ "; must be one of those: Buy/Cover/Sell/Short";
							//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Error, msg);
							this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
							throw new Exception(msg);
					}
					break;

				case MarketLimitStop.Stop:
				case MarketLimitStop.StopLimit:
					order.SpreadSide = OrderSpreadSide.ERROR;
					switch (order.Alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceBestBidAsk >= order.PriceRequested) order.SpreadSide = OrderSpreadSide.AskTidal;
							break;
						case Direction.Sell:
						case Direction.Short:
							if (priceBestBidAsk <= order.PriceRequested) order.SpreadSide = OrderSpreadSide.BidTidal;
							break;
						default:
							msg += " No Direction[" + order.Alert.Direction + "] handler for order[" + order.ToString() + "]"
								+ "; must be one of those: Buy/Cover/Sell/Short";
							//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Error, msg);
							this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
							throw new Exception(msg);
					}
					break;

				default:
					msg += " No MarketLimitStop[" + order.Alert.MarketLimitStop + "] handler for order[" + order.ToString() + "]"
						+ "; must be one of those: Market/Limit/Stop";
					//orderProcessor.updateOrderStatusError(order, OrderState.Error, msg);
					OrderStateMessage omsg = new OrderStateMessage(order, OrderState.Error, msg);
					this.OrderProcessor.UpdateOrderStateAndPostProcess(order, omsg);
					throw new Exception(msg);
			}
			order.AppendMessage(msg + msig);
		}

		public void CallbackOrderStateReceived(Order orderWithNewState) {
			string msig = "BrokerAdapter::CallbackOrderStateReceived(): orderExecuted.State=[" + orderWithNewState.State + "]: ";
			string msg = "";
			try {
				switch (orderWithNewState.State) {
					case OrderState.WaitingBrokerFill:
						Order mustBeSame = this.OrderProcessor.DataSnapshot.OrdersPending.ScanRecentForSimilarNotSamePendingOrder(orderWithNewState);
						//Order mustBeSame = this.OrderProcessor.DataSnapshot.OrdersAll.FindSimilarNotSamePendingOrder(orderWithNewState);
						if (mustBeSame == null) break;
						bool identical = mustBeSame.Alert.IsIdenticalOrderlessPriceless(orderWithNewState.Alert);
						if (identical == false) {
							msg += "PENDING_MISSING: How come it wasn't added in OrderSubmit()??? orderExecuted["
								+ orderWithNewState + "] mustBeSame[" + mustBeSame + "]";
							//orderExecuted.AppendMessage(msig + msg);
							//Assembler.PopupException(msg);
						} else {
							msg += "SECOND_PENDING_ADDED_OK";
							//orderExecuted.AppendMessage(msig + msg);
							//Assembler.PopupException(msg);
						}
						break;
					case OrderState.Killed:
						this.removeOrdersPendingOnFilledCallback(orderWithNewState, msig);
						this.OrderProcessor.RemovePendingAlertsForVictimOrderMustBePostKill(orderWithNewState, msig);
						break;
					case OrderState.Rejected:
					case OrderState.SLAnnihilated:
					case OrderState.TPAnnihilated:
					case OrderState.FilledPartially:
					case OrderState.Filled:
						//this.RemoveOrdersPendingOnFilledCallback(orderWithNewState, msig);
						break;
					default:
						msg += "STATE_UNEXPECTED";
						orderWithNewState.AppendMessage(msig + msg);
						break;
				}
			} catch (Exception e) {
				Assembler.PopupException(msig, e);
			}
			try {
				this.OrderProcessor.InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter(orderWithNewState);
			} catch (Exception e) {
				Assembler.PopupException("InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter()" + msig, e);
			}
		}

		void removeOrdersPendingOnFilledCallback(Order orderExecuted, string msig) {
			msig = "RemoveOrdersPendingOnFilledCallback(): ";
			//if (OrderStatesCollections.CemeteryHealthy.OrderStates.Contains(orderExecuted.State) == false) return;

			//string msg = this.OrderProcessor.DataSnapshot.OrdersPending.ToStringSummary();
			//bool removed = this.OrderProcessor.DataSnapshot.OrdersPending.Remove(orderExecuted);
			//msg += " ...REMOVED(" + removed + ")=> " + this.OrderProcessor.DataSnapshot.OrdersPending.ToStringSummary();
			OrderLaneByState lane = this.OrderProcessor.DataSnapshot.SuggestLaneByOrderStateNullUnsafe(orderExecuted.State);
			string msg = (lane != null) ? lane.ToString() : "NO_SUGGESTION_FOR[" + orderExecuted.State + "]";

			if (orderExecuted.Alert.IsExitAlert && orderExecuted.Alert.PositionAffected.IsExitFilled == false) {
				msg = "WARNING_POSITION_STILL_OPEN "
					//+ " Alert.isExitAlert && PositionAffected.IsExitFilled=false"
					+ msg;
			}
			orderExecuted.AppendMessage(msig + msg);
		}
		public Order ScanEvidentLanesForGuidNullUnsafe(string GUID, List<OrderLane> orderLanes = null, char separator = ';') {
			string msig = " //" + this.Name;
			string orderLanesSearchedAsString = "";
			Order orderFound = null;
			
			if (orderLanes == null) {
				var snap = this.OrderProcessor.DataSnapshot;
				orderLanes = new List<OrderLane>() {snap.OrdersPending, snap.OrdersSubmitting, snap.OrdersAll};
			}
			foreach (OrderLane orderLane in orderLanes) {
				orderLanesSearchedAsString += orderLane.GetType().Name.Substring(5) + separator;	// removing "Orders" from "OrdersSubmitting"
				orderFound = orderLane.ScanRecentForGUID(GUID);
				if (orderFound != null) break;
			}
			orderLanesSearchedAsString = orderLanesSearchedAsString.TrimEnd(separator);
			msig += ".FindOrderLaneOptimizedNullUnsafe(" + GUID + "),orderLanesSearchedAsString[" + orderLanesSearchedAsString + "]";
			
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
			if (orderFound.Alert.DataSource.BrokerAdapter == null) {
				string msg = "ORDER_FOUND_HAS_BROKER_NULL__ASSIGNING_MYSELF orderFound[" + orderFound.ToString()
					+ "] this[" + this.ToString() + "]";
				orderFound.Alert.DataSource.BrokerAdapter = this;
			}
			return orderFound;
		}

		public virtual void MoveStopLossOverrideable(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.MoveStopLoss(proto, newActivationOffset, newStopLossNegativeOffset);
		}
		public void MoveTakeProfitOverrideable(PositionPrototype proto, double newTakeProfitPositiveOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.MoveTakeProfit(proto, newTakeProfitPositiveOffset);
		}
	}
}