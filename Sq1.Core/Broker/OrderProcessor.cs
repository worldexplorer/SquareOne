using System;
using System.Collections.Generic;
//using System.Threading;
using System.Threading.Tasks;

using Sq1.Core.Accounting;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public bool										AlwaysExitAllSharesInPosition;
		public OrderProcessorDataSnapshot				DataSnapshot					{ get; private set; }
		public OrderPostProcessorEmergency				OPPemergency					{ get; private set; }
		public OrderPostProcessorRejected				OPPrejected						{ get; private set; }
		public OrderPostProcessorSequencerCloseThenOpen	OPPsequencer					{ get; private set; }
		public OrderPostProcessorStateChangedTrigger	OPPstatusCallbacks				{ get; private set; }

		public OrderProcessor() {
			this.OPPsequencer			= new OrderPostProcessorSequencerCloseThenOpen(this);
			this.OPPemergency			= new OrderPostProcessorEmergency(this, this.OPPsequencer);
			this.OPPrejected			= new OrderPostProcessorRejected(this);
			this.OPPstatusCallbacks		= new OrderPostProcessorStateChangedTrigger(this);
			this.DataSnapshot			= new OrderProcessorDataSnapshot(this);
		}

		public void Initialize(string rootPath) {
			//if (rootPath.EndsWith(Path.DirectorySeparatorChar) == false) rootPath += Path.DirectorySeparatorChar;
			this.DataSnapshot.Initialize(rootPath);
		}

		bool isExitOrderConsistent_logInconsistency(Order order) {
			bool exitOrderHasNoErrors = true;
			string errormsg = "";
			Position positionShouldBeFilled = order.Alert.PositionAffected;
			if (positionShouldBeFilled == null) {
				errormsg += "positionShouldBeFilled[" + positionShouldBeFilled + "]=null, ERROR filling order.Alert.PositionAffected !!! ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseNonStreamingPosition);
			}
			if (positionShouldBeFilled.Shares <= 0) {
				errormsg += "Shares<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (positionShouldBeFilled.EntryFilledPrice <= 0) {
				errormsg += "EntryPrice<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (positionShouldBeFilled.EntryAlert == null) {
				errormsg += "EntryAlert=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (errormsg == "" && positionShouldBeFilled.EntryAlert.OrderFollowed == null) {
				errormsg += "EntryAlert.OrderFollowed=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (errormsg == "") {
				//if (positionShouldBeFilled.EntryAlert.OrderFollowed.StateFilledOrPartially == false) {
				//	errormsg += "EntryAlert.OrderFollowed.State[" + positionShouldBeFilled.EntryAlert.OrderFollowed.State + "]"
				//		+ " must be [Filled] or [Partially]; skipping PositionClose"
				//		//+ " for positionShouldBeFilled[" + positionShouldBeFilled + "]"
				//		;
				//	order.State = OrderState.IRefuseToCloseUnfilledEntry;
				//}
				if (positionShouldBeFilled.EntryAlert.OrderFollowed.QtyFill != positionShouldBeFilled.EntryAlert.Qty) {
					errormsg += "EntryAlert.OrderFollowed.QtyFill[" + positionShouldBeFilled.EntryAlert.OrderFollowed.QtyFill + "]"
							+ " EntryAlert.Qty[" + positionShouldBeFilled.EntryAlert.Qty + "]"
							+ "; skipping PositionClose"
							//+ " for positionShouldBeFilled[" + positionShouldBeFilled + "]"
							;
					//order.State = OrderState.IRefuseToCloseUnfilledEntry;
				}
			}
			if (errormsg != "") {
				order.AppendMessage(errormsg);
				exitOrderHasNoErrors = false;
			}
			return exitOrderHasNoErrors;
		}
		Order createOrder_propagateToGui_fromAlert(Alert alert, bool setStatusSubmitting, bool emittedByScript) {
			Order newborn = new Order(alert, emittedByScript, false);
			try {
				newborn.Alert.DataSource.BrokerAdapter.Modify_orderType_priceRequesting_accordingToMarketOrderAs(newborn);
			} catch (Exception e) {
				string msg = "hoping that MarketOrderAs.MarketMinMax influenced order.Alert.MarketLimitStop["
					+ newborn.Alert.MarketLimitStop + "]=MarketLimitStop.Limit for further match; PREV=" + newborn.LastMessage;
				this.AppendOrderMessage_propagateToGui(newborn, msg);
			}
			if (alert.IsExitAlert) {
				if (this.isExitOrderConsistent_logInconsistency(newborn) == false) {
					this.DataSnapshot.OrderInsert_notifyGuiAsync(newborn);
					string reason = newborn.LastMessage;
					string msg = "ALERT_INCONSISTENT_ORDER_PROCESSOR_DIDNT_SUBMIT reason[" + reason + "] " + alert;
					Assembler.PopupException(msg, null, false);

					string msg2 = "IM_USING_ALERTS_EXIT_BAR_NOW__NOT_STREAMING__DO_I_HAVE_TO_ADJUST_HERE?";
					alert.Strategy.Script.Executor.RemovePendingExitAlerts_closePositionsBacktestLeftHanging(alert);
					msg = "DID_I_CLOSE_THIS_PENDING_ALERT_HAVING_NO_LIVE_POSITION? " + alert;
					Assembler.PopupException(msg, null, false);
					return null;
				}
				//adjustExitOrderQtyRequestedToMatchEntry(order);
				alert.PositionAffected.EntryAlert.OrderFollowed.DerivedOrdersAdd(newborn);
			}

			OrderState newbornOrderState = OrderState.EmitOrdersNotClicked;
			string newbornMessage = "alert[" + alert + "]";

			if (setStatusSubmitting == true) {
				if (newborn.hasBrokerAdapter("createOrder_propagateToGui_fromAlert(): ") == false) {
					string msg = "ORDER_HAS_NO_BROKER_ADAPDER__SELECT_AND_CONFIGURE_IN_DATASOURCE_EDITOR__DLL_MIGHT_HAVE_DISAPPEARED";
					Assembler.PopupException(msg);
					return null;
				}
				newbornOrderState = this.isOrderEatable(newborn) ? OrderState.Submitting : OrderState.ErrorSubmittingNotEatable;
				//string isPastDue = newborn.Alert.IsAlertCreatedOnPreviousBar;
				//if (emittedByScript && String.IsNullOrEmpty(isPastDue) == false) {
				//	newbornMessage += "; " + isPastDue;
				//	newbornOrderState = OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted;
				//}
			}
			this.Order_updateState_mustBeTheSame_dontPostProcess(new OrderStateMessage(newborn, newbornOrderState, newbornMessage));
			this.DataSnapshot.OrderInsert_notifyGuiAsync(newborn);
			return newborn;
		}
		public List<Order> CreateOrders_submitToBrokerAdapter_eachInNewThread(List<Alert> alertsBatch, bool setStatusSubmitting, bool emittedByScript) {
			if (alertsBatch.Count == 0) {
				string msg = "no alerts to Add; why did you call me? make sure you invoke using a synchronized Queue";
				Assembler.PopupException(msg);
				//return;
			}

			bool ordersClosingAllSameDirection = true;
			PositionLongShort ordersClosingPositionLongShort = PositionLongShort.Unknown;
			bool ordersOpeningAllSameDirection = true;
			PositionLongShort ordersOpeningPositionLongShort = PositionLongShort.Unknown;

			List<Order> orders_polarSequenceAgnostic = new List<Order>();
			List<Order> orders_polarClosingFirst = new List<Order>();
			List<Order> orders_polarOpeningSecond = new List<Order>();
			BrokerAdapter broker = null;
			foreach (Alert alert in alertsBatch) {
				// I only needed alert.OrderFollowed=newOrder... mb even CreatePropagateOrderFromAlert() should be reduced for backtest
				if (alert.Strategy.Script.Executor.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
					string msg = "BACKTEST_DOES_NOT_SUBMIT_ORDERS__CHECK_QUIK_MOCK_FOR_LIVE_SIMULATION";
					Assembler.PopupException(msg);
					alert.Strategy.Script.Executor.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted(msg);
					continue;
				}

				Order newOrder;
				try {
					newOrder = this.createOrder_propagateToGui_fromAlert(alert, setStatusSubmitting, emittedByScript);
				} catch (Exception ex) {
					string msg = "THROWN_this.createPropagateOrder_fromAlert";
					Assembler.PopupException(msg, ex, true);
					continue;
				}
				if (newOrder == null) {
					string msg = "ALERT_INCONSISTENT_ORDER_PROCESSOR_DIDNT_SUBMIT ";
					//ALREADY_COMPLAINED Assembler.PopupException(msg, null, false);
					continue;
				}
				if (newOrder.State != OrderState.Submitting) {
					if (newOrder.State == OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted) {
						alert.Strategy.Script.Executor.CallbackCreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably(alert, alert.Bars.Count);
						continue;
					}
					if (newOrder.State == OrderState.EmitOrdersNotClicked) continue;
					//if (newOrder.Alert.Strategy.Script.Executor.IsAutoSubmitting == true
					//&& newOrder.Alert.Strategy.Script.Executor.IsStreaming == true
					//&& newOrder.FromAutoTrading == true
					string msg = "Unexpected newOrder.State[" + newOrder.State + "] from CreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably()";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				if (broker == null) {
					broker = alert.DataSource.BrokerAdapter;
				} else {
					if (broker != alert.DataSource.BrokerAdapter) {
						string msg = "CROSS_EXCHANGE_ALERTS_NYI alertsBatch MUST contain alerts for the same broker"
							+ "; prevAlert.Broker[" + broker + "] while thisAlert.DataSource.BrokerAdapter[" + alert.DataSource.BrokerAdapter + "]";
						throw new Exception(msg);
					}
				}
				if (alert.Bars.SymbolInfo.SameBarPolarCloseThenOpen) {
					if (alert.IsExitAlert) {
						orders_polarClosingFirst.Add(newOrder);
						if (ordersClosingPositionLongShort == PositionLongShort.Unknown) {
							ordersClosingPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersClosingPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersClosingAllSameDirection = false;
						}
					} else {
						orders_polarOpeningSecond.Add(newOrder);
						if (ordersOpeningPositionLongShort == PositionLongShort.Unknown) {
							ordersOpeningPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersOpeningPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersOpeningAllSameDirection = false;
						}
					}
				} else {
					orders_polarSequenceAgnostic.Add(newOrder);
				}
			}
			if (orders_polarSequenceAgnostic.Count == 0 && (orders_polarClosingFirst.Count + orders_polarOpeningSecond.Count) == 0) {
				string msg = "NO_ORDERS_TO_SUBMIT newBornOrdersToSubmit.Count=0 while alertsBatch.Count[" + alertsBatch.Count + "]>0 "
					+ ": orders_polarSequenceAgnostic.Count[" + orders_polarSequenceAgnostic.Count + "]"
					+ " && orders_polarClosingFirst.Count[" + orders_polarClosingFirst.Count + "]"
					+   "  orders_polarOpeningSecond.Count[" + orders_polarOpeningSecond.Count + "]";
				//ALREADY_COMPLAINED Assembler.PopupException(msg, null, false);
				return orders_polarSequenceAgnostic;
			}

			if (orders_polarSequenceAgnostic.Count > 0 && (orders_polarClosingFirst.Count > 0 || orders_polarOpeningSecond.Count > 0)) {
				string msg = "got mix of orderAware/Agnostic securities in AlertsBatch"
					+ "orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] :: orders_polarClosingFirst[" + orders_polarClosingFirst.Count
					+ "] orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count+ "]";
				Assembler.PopupException(msg);
				return orders_polarSequenceAgnostic;
			}

			if (orders_polarSequenceAgnostic.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] through [" + broker + "]";
				//Assembler.PopupException(msg, null, false);
				this.SubmitToBrokerAdapter_inNewThread(orders_polarSequenceAgnostic, broker);
				return orders_polarSequenceAgnostic;
			}
			if (orders_polarClosingFirst.Count > 0 && orders_polarOpeningSecond.Count == 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarClosingFirst[" + orders_polarClosingFirst.Count + "] through [" + broker + "]";
				Assembler.PopupException(msg, null, false);
				this.SubmitToBrokerAdapter_inNewThread(orders_polarClosingFirst, broker);
				return orders_polarClosingFirst;
			}
			if (orders_polarClosingFirst.Count == 0 && orders_polarOpeningSecond.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count + "] through [" + broker + "]";
				Assembler.PopupException(msg, null, false);
				//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersOpening });
				this.SubmitToBrokerAdapter_inNewThread(orders_polarOpeningSecond, broker);
				return orders_polarOpeningSecond;
			}

			if (ordersClosingAllSameDirection == true && ordersOpeningAllSameDirection == true) {
				if (ordersClosingPositionLongShort != ordersOpeningPositionLongShort) {
					this.OPPsequencer.InitializeSequence(orders_polarClosingFirst, orders_polarOpeningSecond);
					string msg = "Scheduling SubmitOrdersThreadEntry orders_polarClosingFirst[" + orders_polarClosingFirst.Count
						+ "] through [" + broker + "], then  orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersClosing });
					this.SubmitToBrokerAdapter_inNewThread(orders_polarClosingFirst, broker);
					return orders_polarClosingFirst;
				} else {
					List<Order> ordersMerged = new List<Order>(orders_polarClosingFirst);
					ordersMerged.AddRange(orders_polarOpeningSecond);
					string msg = "Scheduling SubmitOrdersThreadEntry ordersMerged[" + ordersMerged.Count + "] through [" + broker + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersMerged });
					this.SubmitToBrokerAdapter_inNewThread(ordersMerged, broker);
					return ordersMerged;
				}
			}
			string msg2 = "UNTESTED_COMPLAIN DONT_PASS_NULL_AS_LIST_OF_ORDERS_TO_EVENT_CONSUMERS"
				+ " DANGEROUS MIX, NOT OPTIMIZED: Scheduling SubmitOrdersThreadEntry"
				+ " orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] through [" + broker + "]"
				+ " (ordersClosingAllSameDirection=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersClosingAllSameDirection=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersClosingPositionLongShort[" + ordersClosingPositionLongShort + "]"
				+ " != ordersOpeningPositionLongShort[" + ordersOpeningPositionLongShort + "]) == FALSE";
			Assembler.PopupException(msg2);
			return null;
		}

		public void SubmitToBrokerAdapter_inNewThread(List<Order> orders, BrokerAdapter broker) {
			string msig = " //SubmitToBrokerAdapter_inNewThread(" + broker + ")";
			if (orders.Count == 0) {
				string msg = "DONT_SUMBIT_orders.Count==0";
				Assembler.PopupException(msg);
			}
			Order firstOrder = orders[0];
			msig = " //SubmitToBrokerAdapter_inNewThread(" + firstOrder.Alert.DataSourceName + ", " + broker + ")";

			// !!!THERE_MUST_BE_NO_DIFFERENCE_BETWEEN_LIVEISIMBROKER_AND_LIVEBROKER!!!
			bool brokerIsLivesim = (broker as LivesimBroker) != null;
			if (brokerIsLivesim) {
				//broker.SubmitOrdersThreadEntry(new object[] { orders });
				//broker.SubmitOrders(orders);
				//return;
				
				string msg1 = "";
				//Assembler.PopupException(msg1, null, false);
			}
			// !!!THERE_MUST_BE_NO_DIFFERENCE_BETWEEN_LIVEISIMBROKER_AND_LIVEBROKER!!!
			//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { orders });
			Task taskEmittingOrders = new Task(delegate {
				broker.SubmitOrders_threadEntry(orders);
			});
			taskEmittingOrders.Start();
		}
		BrokerAdapter extractSameBrokerAdapter_throwIfDifferent(List<Order> orders, string callerMethod) {
			BrokerAdapter broker = null;
			foreach (Order order in orders) {
				if (order.hasBrokerAdapter(callerMethod) == false) {
					string msg = "CRAZY #64";
					Assembler.PopupException(msg);
					continue;
				}
				if (broker == null) broker = order.Alert.DataSource.BrokerAdapter;
				if (broker != order.Alert.DataSource.BrokerAdapter) {
					throw new Exception(callerMethod + "NIY: orderProcessor can not handle orders for several brokers"
						+ "; prevOrder.Broker[" + broker + "] while someOrderBroker[" + order.Alert.DataSource.BrokerAdapter + "]");
				}
			}
			return broker;
		}
		bool isOrderEatable(Order order) {
			if (order.Alert.Strategy == null) return true;
			if (order.IsKiller) return true;
			if (order.Alert.Direction == Direction.Sell || order.Alert.Direction == Direction.Cover) {
				return true;
			}
			Account account = null;
			if (account == null) return true;
			if (account.CashAvailable <= 0) {
				string msg = "ACCOUNT_CASH_ZERO";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.Order_updateState_mustBeDifferent_postProcess(newOrderState);
				return false;
			}
			return true;
		}
		Order createKillerOrder(Order victimOrder) {
			string msig = " //createKillerOrder()";
			if (victimOrder == null) {
				string msg = "DONT_ANNOY_KILLER_IF_YOU_DONT_WANT_TO_KILL_ANYONE victimOrder=null";
				Assembler.PopupException(msg + msig);
				return null;
			}
			if (victimOrder.hasBrokerAdapter(msig) == false) {
				string msg = "VICTIM_DOESNT_HAVE_BROKER " + victimOrder;
				Assembler.PopupException(msg + msig);
				return null;
			}
			//this.RemovePendingAlertsForVictimOrderMustBePostKill(victimOrder, msig);

			Order killerOrder = victimOrder.DeriveKillerOrder();
			this.DataSnapshot.OrderInsert_notifyGuiAsync(killerOrder);
			//this.RaiseOrderReplacementOrKillerCreatedForVictim(victimOrder);
			this.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>() {victimOrder});
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
			return killerOrder;
		}
		public void KillOrder_usingKillerOrder(Order victimOrder) {
			Order killerOrder = this.createKillerOrder(victimOrder);
			//killerOrder.FromAutoTrading = false;
			if (killerOrder.hasBrokerAdapter("KillOrder():") == false) {
				string msg = "CRAZY #63";
				Assembler.PopupException(msg);
				return;
			};
			killerOrder.Alert.DataSource.BrokerAdapter.Kill_usingKiller(killerOrder);
		}
		public Order Order_appendPropagateMessage_updateStateByGuid_dontPostProcess(string orderGUID, OrderState orderState, string message) {
			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			Order orderFound = this.DataSnapshot.OrdersSubmitting.ScanRecent_forGuid(orderGUID, out suggestedLane, out suggestion, false);
			if (orderFound == null) {
				 orderFound = this.DataSnapshot.OrdersAll.ScanRecent_forGuid(orderGUID, out suggestedLane, out suggestion, true);
			}
			if (orderFound == null) {
				string msg = "order[" + orderGUID + "] wasn't found; OrderProcessorDataSnapshot.OrderCount=[" + this.DataSnapshot.OrderCount + "]";
				throw new Exception(msg);
				//log.Fatal(msg, new Exception(msg));
				//return;
			}
			OrderState orderStateAbsorbed = (orderState == OrderState.LeaveTheSame) ? orderFound.State : orderState;
			if (orderStateAbsorbed != orderFound.State) {
				OrderStateMessage osm = new OrderStateMessage(orderFound, orderStateAbsorbed, message);
				this.Order_updateState_mustBeTheSame_dontPostProcess(osm);
			} else {
				this.AppendOrderMessage_propagateToGui(orderFound, message);
			}
			return orderFound;
		}
		void appendOrderMessage_propagateToGui(OrderStateMessage omsg) {
			//log.Debug(omsg.Message);
			if (string.IsNullOrEmpty(omsg.Message)) {
				string msg = "I_REFUSE_TO_APPEND_AND_DISPLAY_EMPTY_MESSAGE omsg[" + omsg.ToString() + "]";
				Assembler.PopupException(msg);
				return;
			}
			Order order = omsg.Order;
			order.AppendMessageSynchronized(omsg);
			if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.RaiseOrderMessageAdded_executionControlShouldPopulate(this, omsg);
		}
		public void AppendOrderMessage_propagateToGui(Order order, string msg) {
			if (order == null) {
				throw new Exception("order=NULL! you don't want to get NullPointerException and debug it");
			}
			OrderStateMessage omsg = new OrderStateMessage(order, msg);
			this.appendOrderMessage_propagateToGui(omsg);
		}

		void postProcess_victimOrder(OrderStateMessage newStateOmsg) {
			Order victimOrder = newStateOmsg.Order;
			this.Order_updateState_mustBeTheSame_dontPostProcess(newStateOmsg);
			switch (victimOrder.State) {
				case OrderState.KillPendingPreSubmit:
				case OrderState.KillPendingSubmitting:
				case OrderState.KillPendingSubmitted:
				case OrderState.SLAnnihilated:
				case OrderState.TPAnnihilated:
					break;
				case OrderState.Submitting:
				case OrderState.WaitingBrokerFill:
				case OrderState.Filled:
					break;
				case OrderState.KillTransSubmittedOK:
					if (victimOrder.FindState_inOrderMessages(OrderState.SLAnnihilated)) {
						this.Order_updateState_mustBeTheSame_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.SLAnnihilated,
								"Setting State to the reason why it was killed"));
					}
					if (victimOrder.FindState_inOrderMessages(OrderState.TPAnnihilated)) {
						this.Order_updateState_mustBeTheSame_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.TPAnnihilated,
								"Setting State to the reason why it was killed"));
					}

					Order killerOrder = victimOrder.KillerOrder;
					this.Order_updateState_mustBeTheSame_dontPostProcess(
						new OrderStateMessage(killerOrder, OrderState.KillerDone,
							"Victim.Killed => Killer.KillerDone"));
					break;
				default:
					string msg = "no handler for victimOrder[" + victimOrder + "]'s state[" + victimOrder.State + "]"
						+ "your BrokerAdapter should call for Victim.States:{"
						//+ OrderState.KillSubmitting + ","
						+ OrderState.KillTransSubmittedOK + ","
						//+ OrderState.Killed + ","
						//+ OrderState.SLAnnihilated + ","
						//+ OrderState.TPAnnihilated + "}";
						;
					throw new Exception(msg);
					break;
			}
		}
		void postProcess_killerOrder(OrderStateMessage newStateOmsg) {
			Order killerOrder = newStateOmsg.Order;
			switch (killerOrder.State) {
				case OrderState.JustConstructed:
				case OrderState.KillerPreSubmit:
				case OrderState.KillerSubmitting:
				case OrderState.KillerBulletFlying:
				case OrderState.KillerDone:
					this.Order_updateState_mustBeTheSame_dontPostProcess(newStateOmsg);
					break;
				default:
					string msg = "no handler for killerOrder[" + killerOrder + "]'s state[" + killerOrder.State + "]"
						+ "your BrokerAdapter should call for Killer.States:{"
						+ OrderState.KillerBulletFlying + ","
						+ OrderState.KillerDone + "}";
					break;
					//throw new Exception(msg);
			}
		}
		public void Order_updateState_mustBeTheSame_dontPostProcess(OrderStateMessage newStateWithReason) {
			Order order = newStateWithReason.Order;
			if (order == null) {
				string msg = "how come ORDER=NULL?";
				Assembler.PopupException(msg, null, false);
			}
			if (order.Alert == null) {
				string msg = "how come ORDER.Alert=NULL?";
				Assembler.PopupException(msg, null, false);
			}
			//if (order.Alert.OrderFollowed == null) {
			//        string msg = "";
			//        Assembler.PopupException(msg, null, false);
			//} else {
			//    if (order.Alert.OrderFollowed != order) {
			//        string msg = "ORDER.Alert.OrderFollowed[" + order.Alert.OrderFollowed + "] != order[" + order + "]";
			//        Assembler.PopupException(msg, null, false);
			//    }
			//}

			// append message in any case; in messages log it will have the underscored state (message log refreshed), while the order itself will never have it (orders tree is not updated)
			this.appendOrderMessage_propagateToGui(newStateWithReason);

			string state_asString = Enum.GetName(typeof(OrderState), newStateWithReason.State);
			bool newState_isUnderscored_thatOrderNeverGets_asyncCallbacksFromBroker = state_asString.StartsWith("_");
			if (newState_isUnderscored_thatOrderNeverGets_asyncCallbacksFromBroker) return;

			if (order.State == newStateWithReason.State) {
				string msg = "I_REFUSE_TO_UPDATE_ORDER_WITH_SAME_STATE USE_INSTEAD__OrderProcessor.AppendOrderMessage_propagateToGui()  //Order_updateState_switchLanes_appendMessage_propagateIfGuiHasTime__dontPostProcess()";
				Assembler.PopupException(msg, null, false);
				return;
			}

			OrderState orderState_priorToUpdate = order.State;
			order.SetState_localTime_fromMessage(newStateWithReason);
			this.DataSnapshot.SwitchLanes_forOrder_postStatusUpdate(order, orderState_priorToUpdate);

			if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){order});
		}
		public void Order_updateState_mustBeDifferent_postProcess(OrderStateMessage newStateOmsg, double priceFill = 0, double qtyFill = 0) {
			Order order = newStateOmsg.Order;
			string msig = "UpdateOrderState_postProcess(): ";

			if (order == null) {
				string msg = "POSSIBLE_END_OF_LIVESIM_LATE_FILL_AFTER_CTX_RESTORED_LIVEBROKER_GONE " + newStateOmsg.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (order.VictimToBeKilled != null) {
				this.postProcess_killerOrder(newStateOmsg);
				return;
			}
			if (order.KillerOrder != null) {
				this.postProcess_victimOrder(newStateOmsg);
				return;
			}

			if (newStateOmsg.State == OrderState.Rejected && order.State == OrderState.EmergencyCloseLimitReached) {
				string prePostErrorMsg = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for EmergencyCloseLimitReached "
					//+ "; skipping PostProcess for [" + order + "]"
					;
				this.AppendOrderMessage_propagateToGui(order, prePostErrorMsg);
				return;
			}
			if (newStateOmsg.State == OrderState.Rejected && order.InState_emergency) {
				string prePostErrorMsg = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for"
					+ " order.inEmergencyState[" + order.State + "] "
					//+ "; skipping PostProcess for [" + order + "]"
					;
				this.AppendOrderMessage_propagateToGui(order, prePostErrorMsg);
				return;
			}

			if (order.State == newStateOmsg.State) {
				string msg = "I_REFUSE_TO_POST_PROCESS USE_INSTEAD__OrderProcessor.Order_appendPropagateMessage_updateStateIfDifferent_switchLanes___dontPostProcess()  //Order_appendPropagateMessage_updateStateMustBeDifferent_switchLanes_postProcess()";
				Assembler.PopupException(msg, null, false);
				return;
			}

			if (priceFill != 0) {
				if (newStateOmsg.State != OrderState.Filled) {
					string msg = "YOU_MUST_INTENT_TO_SET_STATE_FILLED_WHEN_PRICE_FILL!=0";
					Assembler.PopupException(msg, null, true);
					order.AppendMessage(msg);
				}
				if (order.PriceFill != 0) {
					string msg1 = "ORDER_WAS_ALREADY_FILLED NYI:PARTIAL_FILL_WITH_DIFFERENT_FILL_PRICE";
					Assembler.PopupException(msg1, null, true);
					order.AppendMessage(msg1);

					bool marketWasSubstituted = order.Alert.MarketLimitStop == MarketLimitStop.Limit
							&& order.Alert.Bars.SymbolInfo.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
					if (order.PriceFill != priceFill && marketWasSubstituted == false) {
						string msg = "got priceFill[" + priceFill + "] while order.PriceFill=[" + order.PriceFill + "]"
							+ "; weird for Order.Alert.MarketLimitStop=[" + order.Alert.MarketLimitStop + "]";
						order.AppendMessage(msg);
					}
				}
				order.FillWith(priceFill, qtyFill);
			}
			this.Order_updateState_mustBeTheSame_dontPostProcess(newStateOmsg);
			this.postProcess_invokeScript_scheduleReplacement(order, priceFill, qtyFill);
		}
		void postProcessAccounting(Order order) {
			if (order.Alert.Direction == Direction.Unknown) {
				string msg = "Direction.Unknown can't be here; Unknown is default for Deserialization errors!";
				Assembler.PopupException(msg);
			}
			//moved to Order.FillPositionAffected() to make MarketSim to fill without orderProcessor
			//if (order.Alert.PositionAffected != null) { 	// alert.PositionAffected = null when order created by chart-click-mni
			//	if (order.Alert.isEntryAlert) {
			//		order.Alert.PositionAffected.EntryFilledWith(order.PriceFill, order.SlippageFill, 0);
			//	} else {
			//		order.Alert.PositionAffected.ExitFilledWith(order.PriceFill, order.SlippageFill, 0);
			//	}
			//} else {
			//	log.Fatal("NO POSITION AFFECTED; order[" + order + "] alert[" + order.Alert + "]");
			//}
			// FIXME: UNCOMMENT AND FIX DataSource == null here...
			/*
			Account account = this.DataSnapshot.FindAccountByNumber(order.Alert.AccountNumber);
			if (account == null) {
				string msg = "Account not found for order[" + order.ToString() + "]";
				log.Fatal(msg);
				//throw new Exception(msg);
			} else {
				AccountPosition positionAlready = this.DataSnapshot.DataSnapshot.FindAccountPositionForOrder(order);
				if (positionAlready != null) {
					double _SharesFilledDiff = qtyFill - order.QtyFill;
					if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Short) _SharesFilledDiff = -_SharesFilledDiff;
					log.Warn("Adding Shares[" + _SharesFilledDiff + "] to existing Position[" + positionAlready + "]");
					positionAlready.QtyFill += _SharesFilledDiff;
					// FIXME: UNCOMMENT AND FIX DataSource == null here...
					order.Alert.DataSource.BrokerAdapter.AccountPositionModified(account);
					if (this.AccountPositionChanged != null) {
						this.AccountPositionChanged(this, new AccountPositionEventArgs(positionAlready));
					}
				} else {
					AccountPosition positionNew = new AccountPosition(order);
					log.Info("Adding new Position[" + positionNew + "]");
					positionNew.Account = account;
					account.Positions.Add(positionNew);
					// FIXME: UNCOMMENT AND FIX DataSource == null here...
					order.Alert.DataSource.BrokerAdapter.AccountPositionAdded(account);
					if (this.AccountPositionAdded != null) {
						this.AccountPositionAdded(this, new AccountPositionEventArgs(positionNew));
					}
				}
			}*/
		}
		void postProcess_invokeScript_scheduleReplacement(Order order, double priceFill, double qtyFill) {
			string msig = " " + order.State + " " + order.LastMessage + " //postProcess_invokeScript_scheduleReplacement()";
			//if (order.Alert.isExitAlert || order.IsEmergencyClose) {
			//	order.State = OrderState.Rejected;
			//}
			//if (order.Alert.isEntryAlert && order.State == OrderState.Rejected) {
			//	order.State = OrderState.Filled;
			//}
			if (OrderStatesCollections.NoInterventionRequired.Contains(order.State) == false) {
				this.DataSnapshot.OrdersExpectingBrokerUpdateCount--;
			}
			switch (order.State) {
				case OrderState.Filled:
				case OrderState.FilledPartially:
					double slippageByFact = 0;
					// you should save it to SlippageEffective! calc "implied" slippage from executed price, instead of assumed for LimitCrossMarket
					if (order.SlippageFill == 0
						// && order.Alert.MarketLimitStop == MarketLimitStop.Market
						// && order.Alert.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker
							) {
						if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
							slippageByFact = order.PriceFill - order.CurrentBid;
						} else {
							slippageByFact = order.PriceFill - order.CurrentAsk;
						}
					}

					order.FillWith(priceFill, qtyFill, slippageByFact);
					this.postProcessAccounting(order);

					if (order.IsEmergencyClose) {
						this.OPPemergency.RemoveEmergencyLockFilled(order);
					}
					this.OPPsequencer.OrderFilled_unlockSequence_submitOpening(order);
					try {
						order.Alert.Strategy.Script.Executor.CallbackAlertFilled_moveAround_invokeScriptNonReenterably(order.Alert, null,
							order.PriceFill, order.QtyFill, order.SlippageFill, order.CommissionFill);
					} catch (Exception ex) {
						string msg3 = "PostProcessOrderState caught from CallbackAlertFilledMoveAroundInvokeScript() ";
						Assembler.PopupException(msg3 + msig, ex);
					}
					break;

				case OrderState.ErrorCancelReplace:
					this.DataSnapshot.OrdersRemove(new List<Order>() { order });
					this.RaiseAsyncOrderRemoved_executionControlShouldRebuildOLV(this, new List<Order>(){order});
					Assembler.PopupException(msig);
					break;

				case OrderState.Error:
				case OrderState.ErrorMarketPriceZero:
				case OrderState.ErrorSubmitOrder:
				case OrderState.ErrorSlippageCalc:
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig);
					order.PriceFill = 0;
					//NEVER order.PricePaid = 0;
					break;

				case OrderState.ErrorSubmittingBroker:
				case OrderState.Rejected:
				case OrderState.RejectedLimitReached:
					if (order.State == OrderState.Rejected) {
						bool a = order.IsEmergencyClose;
						bool b = string.IsNullOrEmpty(order.EmergencyReplacedByGUID) == false;
						bool c = string.IsNullOrEmpty(order.ReplacedByGUID) == false;
						if (b || c) {
							string msg = "";
							if (b) msg += " BrokerAdapter CALLBACK DUPE: Rejected was already replaced by"
								+ " EmergencyReplacedByGUID[" + order.EmergencyReplacedByGUID + "]"
								//+ "; skipping PostProcess for [" + order + "]"
								;
							if (c) msg += " BrokerAdapter CALLBACK DUPE: Rejected was already replaced by"
								+ " ReplacedByGUID[" + order.ReplacedByGUID + "]"
								//+ "; skipping PostProcess for [" + order + "]"
								;
							this.AppendOrderMessage_propagateToGui(order, msg);
							this.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){order});
							return;
						}
					}
			
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig, null, false);
					order.PriceFill = 0;
					//NEVER order.PricePaid = 0;

					if (order.IsEmergencyClose) {
						this.OPPemergency.CreateEmergencyReplacement_resubmitFor(order);
					} else {
						if (order.Alert.IsExitAlert) {
							this.OPPemergency.AddLockAndCreate_emergencyReplacement_resubmitFor(order);
						} else {
							this.OPPrejected.HandleReplaceRejected(order);
						}
					}
					break;

				case OrderState.Submitting:
					string msg2 = "all Orders.State!=Submitting aren't sent to BrokerAdapter;"
						+ " we shouldn't be here in a broker-originated State change handler...";
					break;
				case OrderState.SubmittingSequenced:
				case OrderState.Submitted:
				case OrderState.SubmittedNoFeedback:
				case OrderState.WaitingBrokerFill:
					break;

				case OrderState.IRefuseOpenTillEmergencyCloses:
				case OrderState.IRefuseToCloseNonStreamingPosition:
				case OrderState.IRefuseToCloseUnfilledEntry:
					break;

				case OrderState.EmergencyCloseSheduledForErrorSubmittingBroker:
				case OrderState.EmergencyCloseSheduledForNoReason:
				case OrderState.EmergencyCloseSheduledForRejected:
				case OrderState.EmergencyCloseSheduledForRejectedLimitReached:
				case OrderState.EmergencyCloseComplete:
				case OrderState.EmergencyCloseLimitReached:
				case OrderState.EmergencyCloseUserInterrupted:
					break;

				case OrderState.PreSubmit:
				case OrderState.KillPendingPreSubmit:
				case OrderState.KillerPreSubmit:
					break;

				case OrderState.JustConstructed:
					break;

				case OrderState._TradeStatus:
					break;

				default:
					string msg4 = "NO_HANDLER_FOR_ORDER.STATE";
					Assembler.PopupException(msg4 + msig, null, false);
					break;
			}
		}

		public void StopLoss_move(PositionPrototype proto, double newActivationOffset, double newStopLoss_negativeOffset) {
			if (proto.StopLossAlert_forMoveAndAnnihilation == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = "StopLossMove(" + proto.StopLossActivation_negativeOffset + "/" + proto.StopLoss_negativeOffset
				+ "=>" + newActivationOffset + "/" + newStopLoss_negativeOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook stopLossGotKilledHook = new OrderPostProcessorStateHook("StopLossGotKilledHook",
				order2killAndReplace, OrderState.KillTransSubmittedOK,
				delegate(Order stopLossKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "StopLossGotKilledHook(): invoking onStopLossKilled_createNewStopLoss_andAddToPokeUnit() "
						+ "[" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + stopLossKilled.State + "]";
					stopLossKilled.AppendMessage(msg);
					this.onStopLossKilled_createNewStopLoss_andAddToPokeUnit(stopLossKilled, newActivationOffset, newStopLoss_negativeOffset, pokeUnit);
				}
			);

			// 2. hook onActive=>kill
			OrderPostProcessorStateHook stopLossReceivedActiveCallback = new OrderPostProcessorStateHook("StopLossReceivedActiveCallback",
				order2killAndReplace, OrderState.WaitingBrokerFill,
				delegate(Order stopLossToBeKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "StopLossReceivedActiveCallback(): invoking KillOrder_usingKillerOrder() "
						+ " [" + stateBeforeActiveAssummingSubmitting + "] => "
						+ "[" + stopLossToBeKilled.State + "]";
					stopLossToBeKilled.AppendMessage(msg);
					stateBeforeKilledAssumingActive = stopLossToBeKilled.State;
					this.KillOrder_usingKillerOrder(order2killAndReplace);
				}
			);

			this.OPPstatusCallbacks.AddStateChangedHook(stopLossReceivedActiveCallback);
			this.OPPstatusCallbacks.AddStateChangedHook(stopLossGotKilledHook);

			this.AppendOrderMessage_propagateToGui(proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed, msig + "hooked stopLossReceivedActiveCallback() and stopLossGotKilledHook()");
		}
		void onStopLossKilled_createNewStopLoss_andAddToPokeUnit(Order killedStopLoss, double newActivationOffset, double newStopLossNegativeOffset, ReporterPokeUnit pokeUnit) {
			string msig = "onStopLossKilled_createNewStopLoss_andAddToPokeUnit(): ";
			ScriptExecutor executor = killedStopLoss.Alert.Strategy.Script.Executor;
			Position position = killedStopLoss.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new StopLossAlert for SellOrCoverRegisterAlerts()
			position.Prototype.StopLossAlert_forMoveAndAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewStopLossOffsets(newStopLossNegativeOffset, newActivationOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateStopLoss_fromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() {replacement}, true, true);
			pokeUnit.AlertsNew.AddNoDupe(replacement, this, "onStopLossKilledCreateNewStopLossAndAddToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedStopLoss.AppendMessage(msg + msig);
		}
		public void TakeProfit_move(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			if (proto.TakeProfitAlert_forMoveAndAnnihilation == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = "TakeProfitMove(" + proto.TakeProfit_positiveOffset + "=>" + newTakeProfit_positiveOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook takeProfitGotKilledHook = new OrderPostProcessorStateHook("TakeProfitGotKilledHook",
				order2killAndReplace, OrderState.KillTransSubmittedOK,
				delegate(Order takeProfitKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "takeProfitGotKilledHook(): invoking OnTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit() "
						+ " [" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + takeProfitKilled.State + "]";
					takeProfitKilled.AppendMessage(msg);
					this.onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(takeProfitKilled, newTakeProfit_positiveOffset, pokeUnit);
				}
			);

			// 2. hook onActive=>kill
			OrderPostProcessorStateHook takeProfitReceivedActiveCallback = new OrderPostProcessorStateHook("TakeProfitReceivedActiveCallback",
				order2killAndReplace, OrderState.WaitingBrokerFill,
				delegate(Order takeProfitToBeKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "takeProfitReceivedActiveCallback(): invoking KillOrderUsingKillerOrder() "
						+ " [" + stateBeforeActiveAssummingSubmitting + "] => "
						+ "[" + takeProfitToBeKilled.State + "]";
					takeProfitToBeKilled.AppendMessage(msg);
					stateBeforeKilledAssumingActive = takeProfitToBeKilled.State;
					this.KillOrder_usingKillerOrder(order2killAndReplace);
				}
			);

			this.OPPstatusCallbacks.AddStateChangedHook(takeProfitReceivedActiveCallback);
			this.OPPstatusCallbacks.AddStateChangedHook(takeProfitGotKilledHook);

			this.AppendOrderMessage_propagateToGui(proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed, msig + ": hooked takeProfitReceivedActiveCallback() and takeProfitGotKilledHook()");
		}
		void onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(Order killedTakeProfit, double newTakeProfitPositiveOffset, ReporterPokeUnit pokeUnit) {
			string msig = "onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(): ";
			ScriptExecutor executor = killedTakeProfit.Alert.Strategy.Script.Executor;
			Position position = killedTakeProfit.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new TakeProfitAlert for SellOrCoverRegisterAlerts()
			position.Prototype.TakeProfitAlert_forMoveAndAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewTakeProfitOffset(newTakeProfitPositiveOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateTakeProfit_fromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() { replacement }, true, true);
			pokeUnit.AlertsNew.AddNoDupe(replacement, this, "onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedTakeProfit.AppendMessage(msg + msig);
		}

		public void KillPending_withoutKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(Order orderPendingKilled, string msig) {
			if (orderPendingKilled.State != OrderState.KillTransSubmittedOK) {
				string msg = "I_SERVE_ONLY_OrderState.KilledPending";
				Assembler.PopupException(msg);
				return;
			}
			Alert alertForOrder = orderPendingKilled.Alert;
			if (alertForOrder == null) {
				string msg = "orderKilled.Alert=null; dunno what to remove from PendingAlerts";
				orderPendingKilled.AppendMessage(msg + msig + " " + orderPendingKilled);
				return;
			}
			ScriptExecutor executor = alertForOrder.Strategy.Script.Executor;
			try {
				executor.CallbackAlertKilled_invokeScript_nonReenterably(alertForOrder);
				string msg = orderPendingKilled.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d";
				orderPendingKilled.AppendMessage(msg + msig);
			} catch (Exception e) {
				string msg = orderPendingKilled.State + " is a Cemetery but [" + e.Message + "]"
					+ "; comment the State out; alert[" + alertForOrder + "]";
				orderPendingKilled.AppendMessage(msg + msig);
			}
		}
		public void PendingOrder_killWithoutKiller(Order orderPending) {
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;

			string msgPending = "EXPECTING_THREE_MESSAGES_FROM_BROKER_FOR[" + orderPending + "] "
				+ OrderState.KillPendingSubmitting + ">"
				+ OrderState.KillPendingSubmitted + ">"
				+ OrderState.KillTransSubmittedOK;

			if (orderPending.State == OrderState.KillPendingPreSubmit) {
				msgPending = "DUPLICATING_ORDERKILL_SEND__EXPECT_ALREADYKILLED_ERRORMSG_FROM_BROKER_ADAPTER " + msgPending;
				OrderStateMessage omsg_WontBePostProcessed = new OrderStateMessage(orderPending, msgPending);
				//WILL_COMPLAIN_ON_SAME_STATE_AND_SUGGEST_JUST_APPEND_MSG this.Order_appendPropagateMessage_updateStateIfDifferent_switchLanes___dontPostProcess(omsg_WontBePostProcessed);
				this.AppendOrderMessage_propagateToGui(orderPending, msgPending);
			} else {
				OrderStateMessage omsg_WillNotBePostProcessed = new OrderStateMessage(orderPending, OrderState.KillPendingPreSubmit, msgPending);
				this.Order_updateState_mustBeDifferent_postProcess(omsg_WillNotBePostProcessed);
			}

			if (orderPending.Alert.Bars == null) {
				string msg = "I_REFUSE_TO_KILL_PENDING__AFTER_APPRESTART orderPending.Alert.Bars=null";
				Assembler.PopupException(msg, null, false);
				return;
			}

			try {
				orderPending.Alert.DataSource.BrokerAdapter.KillPending_withoutKiller(orderPending);
			} catch (Exception ex) {
				string msg = "DID_YOU_IMPLEMENT_IN_YOUR_BROKER_KillPending_withoutKiller(orderPending)?";
				Assembler.PopupException(msg, ex, false);
			}
		}
	}
}
