using System;
using System.Collections.Generic;
using System.Threading;

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

		bool isExitOrderConsistentLogInconsistency(Order order) {
			bool exitOrderHasNoErrors = true;
			string errormsg = "";
			Position positionShouldBeFilled = order.Alert.PositionAffected;
			if (positionShouldBeFilled == null) {
				errormsg += "positionShouldBeFilled[" + positionShouldBeFilled + "]=null, ERROR filling order.Alert.PositionAffected !!! ";
				order.State = OrderState.IRefuseToCloseNonStreamingPosition;
			}
			if (positionShouldBeFilled.Shares <= 0) {
				errormsg += "Shares<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.State = OrderState.IRefuseToCloseUnfilledEntry;
			}
			if (positionShouldBeFilled.EntryFilledPrice <= 0) {
				errormsg += "EntryPrice<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.State = OrderState.IRefuseToCloseUnfilledEntry;
			}
			if (positionShouldBeFilled.EntryAlert == null) {
				errormsg += "EntryAlert=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.State = OrderState.IRefuseToCloseUnfilledEntry;
			}
			if (errormsg == "" && positionShouldBeFilled.EntryAlert.OrderFollowed == null) {
				errormsg += "EntryAlert.OrderFollowed=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.State = OrderState.IRefuseToCloseUnfilledEntry;
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
		Order createPropagateOrderFromAlert(Alert alert, bool setStatusSubmitting, bool emittedByScript) {
			if (alert.MarketLimitStop == MarketLimitStop.AtClose) {
				string msg = "NYI: alert.OrderType= OrderType.AtClose [" + alert + "]";
				throw new Exception(msg);
			}
			Order newborn = new Order(alert, emittedByScript, false);
			try {
				newborn.Alert.DataSource.BrokerAdapter.ModifyOrderTypeAccordingToMarketOrderAs(newborn);
			} catch (Exception e) {
				string msg = "hoping that MarketOrderAs.MarketMinMax influenced order.Alert.MarketLimitStop["
					+ newborn.Alert.MarketLimitStop + "]=MarketLimitStop.Limit for further match; PREV=" + newborn.LastMessage;
				this.AppendOrderMessageAndPropagateCheckThrowOrderNull(newborn, msg);
			}
			if (alert.IsExitAlert) {
				if (this.isExitOrderConsistentLogInconsistency(newborn) == false) {
					this.DataSnapshot.OrderInsertNotifyGuiAsync(newborn);
					string reason = newborn.LastMessage;
					string msg = "ALERT_INCONSISTENT_ORDER_PROCESSOR_DIDNT_SUBMIT reason[" + reason + "] " + alert;
					Assembler.PopupException(msg, null, false);

					string msg2 = "IM_USING_ALERTS_EXIT_BAR_NOW__NOT_STREAMING__DO_I_HAVE_TO_ADJUST_HERE?";
					alert.Strategy.Script.Executor.RemovePendingExitAlertAndClosePositionAfterBacktestLeftItHanging(alert);
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
				if (newborn.hasBrokerAdapter("CreatePropagateOrderFromAlert(): ") == false) {
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
			this.UpdateOrderStateDontPostProcess(newborn, new OrderStateMessage(newborn, newbornOrderState, newbornMessage));
			this.DataSnapshot.OrderInsertNotifyGuiAsync(newborn);
			return newborn;
		}
		public List<Order> CreateOrdersSubmitToBrokerAdapterInNewThreads(List<Alert> alertsBatch, bool setStatusSubmitting, bool emittedByScript) {
			if (alertsBatch.Count == 0) {
				string msg = "no alerts to Add; why did you call me? make sure you invoke using a synchronized Queue";
				Assembler.PopupException(msg);
				//return;
			}

			bool ordersClosingAllSameDirection = true;
			PositionLongShort ordersClosingPositionLongShort = PositionLongShort.Unknown;
			bool ordersOpeningAllSameDirection = true;
			PositionLongShort ordersOpeningPositionLongShort = PositionLongShort.Unknown;

			List<Order> ordersAgnostic = new List<Order>();
			List<Order> ordersClosing = new List<Order>();
			List<Order> ordersOpening = new List<Order>();
			BrokerAdapter broker = null;
			foreach (Alert alert in alertsBatch) {
				// I only needed alert.OrderFollowed=newOrder... mb even CreatePropagateOrderFromAlert() should be reduced for backtest
				if (alert.Strategy.Script.Executor.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
					string msg = "BACKTEST_DOES_NOT_SUBMIT_ORDERS__CHECK_QUIK_MOCK_FOR_LIVE_SIMULATION";
					Assembler.PopupException(msg);
					alert.Strategy.Script.Executor.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted(msg);
					continue;
				}

				Order newOrder;
				try {
					newOrder = this.createPropagateOrderFromAlert(alert, setStatusSubmitting, emittedByScript);
				} catch (Exception ex) {
					string msg = "THROWN_this.CreatePropagateOrderFromAlert";
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
						alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScriptNonReenterably(alert, alert.Bars.Count);
						continue;
					}
					if (newOrder.State == OrderState.EmitOrdersNotClicked) continue;
					//if (newOrder.Alert.Strategy.Script.Executor.IsAutoSubmitting == true
					//&& newOrder.Alert.Strategy.Script.Executor.IsStreaming == true
					//&& newOrder.FromAutoTrading == true
					string msg = "Unexpected newOrder.State[" + newOrder.State + "] from CreatePropagateOrderFromAlert()";
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
						ordersClosing.Add(newOrder);
						if (ordersClosingPositionLongShort == PositionLongShort.Unknown) {
							ordersClosingPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersClosingPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersClosingAllSameDirection = false;
						}
					} else {
						ordersOpening.Add(newOrder);
						if (ordersOpeningPositionLongShort == PositionLongShort.Unknown) {
							ordersOpeningPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersOpeningPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersOpeningAllSameDirection = false;
						}
					}
				} else {
					ordersAgnostic.Add(newOrder);
				}
			}
			if (ordersAgnostic.Count == 0 && (ordersClosing.Count + ordersOpening.Count) == 0) {
				string msg = "NO_ORDERS_TO_SUBMIT newBornOrdersToSubmit.Count=0 while alertsBatch.Count[" + alertsBatch.Count + "]>0 "
					+ ": ordersAgnostic.Count[" + ordersAgnostic.Count + "]"
					+ " && ordersClosing.Count[" + ordersClosing.Count + "]"
					+   "  ordersOpening.Count[" + ordersOpening.Count + "]";
				//ALREADY_COMPLAINED Assembler.PopupException(msg, null, false);
				return ordersAgnostic;
			}

			if (ordersAgnostic.Count > 0 && (ordersClosing.Count > 0 || ordersOpening.Count > 0)) {
				string msg = "got mix of orderAware/Agnostic securities in AlertsBatch"
					+ "ordersAgnostic[" + ordersAgnostic.Count + "] :: ordersClosing[" + ordersClosing.Count
					+ "] ordersOpening[" + ordersOpening.Count+ "]";
				Assembler.PopupException(msg);
				return ordersAgnostic;
			}

			if (ordersAgnostic.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry ordersAgnostic[" + ordersAgnostic.Count + "] through [" + broker + "]";
				//Assembler.PopupException(msg, null, false);
				this.SubmitToBrokerAdapter_inNewThreadOrStraight(ordersAgnostic, broker);
				return ordersAgnostic;
			}
			if (ordersClosing.Count > 0 && ordersOpening.Count == 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry ordersClosing[" + ordersClosing.Count + "] through [" + broker + "]";
				Assembler.PopupException(msg, null, false);
				this.SubmitToBrokerAdapter_inNewThreadOrStraight(ordersClosing, broker);
				return ordersClosing;
			}
			if (ordersClosing.Count == 0 && ordersOpening.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry ordersOpening[" + ordersOpening.Count + "] through [" + broker + "]";
				Assembler.PopupException(msg, null, false);
				//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersOpening });
				this.SubmitToBrokerAdapter_inNewThreadOrStraight(ordersOpening, broker);
				return ordersClosing;
			}

			if (ordersClosingAllSameDirection == true && ordersOpeningAllSameDirection == true) {
				if (ordersClosingPositionLongShort != ordersOpeningPositionLongShort) {
					this.OPPsequencer.InitializeSequence(ordersClosing, ordersOpening);
					string msg = "Scheduling SubmitOrdersThreadEntry ordersClosing[" + ordersClosing.Count
						+ "] through [" + broker + "], then  ordersOpening[" + ordersOpening.Count + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersClosing });
					this.SubmitToBrokerAdapter_inNewThreadOrStraight(ordersClosing, broker);
					return ordersClosing;
				} else {
					List<Order> ordersMerged = new List<Order>(ordersClosing);
					ordersMerged.AddRange(ordersOpening);
					string msg = "Scheduling SubmitOrdersThreadEntry ordersMerged[" + ordersMerged.Count + "] through [" + broker + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersMerged });
					this.SubmitToBrokerAdapter_inNewThreadOrStraight(ordersClosing, broker);
					return ordersClosing;
				}
			}
			string msg2 = "DONT_PASS_NULL_AS_LIST_OF_ORDERS_TO_EVENT_CONSUMERS"
				+ " DANGEROUS MIX, NOT OPTIMIZED: Scheduling SubmitOrdersThreadEntry"
				+ " ordersAgnostic[" + ordersAgnostic.Count + "] through [" + broker + "]"
				+ " (ordersClosingAllSameLongOrShort=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersOpeningAllSameLongOrShort=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersClosingPositionLongShort[" + ordersClosingPositionLongShort + "]"
				+ " != ordersOpeningPositionLongShort[" + ordersOpeningPositionLongShort + "]) == FALSE";
			Assembler.PopupException(msg2);
			return null;
		}

		public void SubmitToBrokerAdapter_inNewThreadOrStraight(List<Order> orders, BrokerAdapter broker) {
			bool brokerIsLivesim = (broker as LivesimBroker) != null;
			if (brokerIsLivesim) {
				//broker.SubmitOrdersThreadEntry(new object[] { orders });
				broker.SubmitOrders(orders);
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { orders });
		}
		BrokerAdapter extractSameBrokerAdapterThrowIfDifferent(List<Order> orders, string callerMethod) {
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
				this.UpdateOrderStateAndPostProcess(order, newOrderState);
				return false;
			}
			return true;
		}
		Order createKillerOrder(Order victimOrder) {
			string msig = " //CreateKillerOrder()";
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
			this.DataSnapshot.OrderInsertNotifyGuiAsync(killerOrder);
			//this.RaiseOrderReplacementOrKillerCreatedForVictim(victimOrder);
			this.RaiseOrderStateOrPropertiesChangedExecutionFormShouldDisplay(this, new List<Order>() {victimOrder});
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
			return killerOrder;
		}
		public void KillOrderUsingKillerOrder(Order victimOrder) {
			Order killerOrder = this.createKillerOrder(victimOrder);
			//killerOrder.FromAutoTrading = false;
			if (killerOrder.hasBrokerAdapter("KillOrder():") == false) {
				string msg = "CRAZY #63";
				Assembler.PopupException(msg);
				return;
			};
			killerOrder.Alert.DataSource.BrokerAdapter.OrderKillSubmitUsingKillerOrder(killerOrder);
		}
		public Order UpdateOrderStateByGuidNoPostProcess(string orderGUID, OrderState orderState, string message) {
			Order orderFound = this.DataSnapshot.OrdersSubmitting.ScanRecentForGUID(orderGUID);
			if (orderFound == null) {
				 orderFound = this.DataSnapshot.OrdersAll.ScanRecentForGUID(orderGUID);
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
				UpdateOrderStateDontPostProcess(orderFound, osm);
			} else {
				this.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderFound, message);
			}
			return orderFound;
		}
		void appendOrderMessageAndPropagate(Order order, OrderStateMessage omsg) {
			//log.Debug(omsg.Message);
			if (string.IsNullOrEmpty(omsg.Message)) {
				string msg = "I_REFUSE_TO_APPEND_AND_DISPLAY_EMPTY_MESSAGE omsg[" + omsg.ToString() + "]";
				Assembler.PopupException(msg);
				return;
			}
			omsg.Order.AppendMessageSynchronized(omsg);
			if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.RaiseOrderMessageAddedExecutionFormNotification(this, omsg);
		}
		public void AppendOrderMessageAndPropagateCheckThrowOrderNull(Order order, string msg) {
			if (order == null) {
				throw new Exception("order=NULL! you don't want to get NullPointerException and debug it");
			}
			OrderStateMessage omsg = new OrderStateMessage(order, order.State, msg);
			this.appendOrderMessageAndPropagate(order, omsg);
		}

		void postProcessVictimOrder(Order victimOrder, OrderStateMessage newStateOmsg) {
			this.UpdateOrderStateDontPostProcess(victimOrder, newStateOmsg);
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
				case OrderState.KilledPending:
					if (victimOrder.FindStateInOrderMessages(OrderState.SLAnnihilated)) {
						this.UpdateOrderStateDontPostProcess(victimOrder,
							new OrderStateMessage(victimOrder, OrderState.SLAnnihilated,
								"Setting State to the reason why it was killed"));
					}
					if (victimOrder.FindStateInOrderMessages(OrderState.TPAnnihilated)) {
						this.UpdateOrderStateDontPostProcess(victimOrder,
							new OrderStateMessage(victimOrder, OrderState.TPAnnihilated,
								"Setting State to the reason why it was killed"));
					}

					Order killerOrder = victimOrder.KillerOrder;
					this.UpdateOrderStateDontPostProcess(killerOrder,
						new OrderStateMessage(killerOrder, OrderState.KillerDone,
							"Victim.Killed => Killer.KillerDone"));
					break;
				default:
					string msg = "no handler for victimOrder[" + victimOrder + "]'s state[" + victimOrder.State + "]"
						+ "your BrokerAdapter should call for Victim.States:{"
						//+ OrderState.KillSubmitting + ","
						+ OrderState.KilledPending + ","
						//+ OrderState.Killed + ","
						//+ OrderState.SLAnnihilated + ","
						//+ OrderState.TPAnnihilated + "}";
						;
					throw new Exception(msg);
					break;
			}
		}
		void postProcessKillerOrder(Order killerOrder, OrderStateMessage newStateOmsg) {
			switch (killerOrder.State) {
				case OrderState.JustConstructed:
				case OrderState.KillerPreSubmit:
				case OrderState.KillerSubmitting:
				case OrderState.KillerBulletFlying:
				case OrderState.KillerDone:
					this.UpdateOrderStateDontPostProcess(killerOrder, newStateOmsg);
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
		object orderUpdateLock = new object();
		public void UpdateOrderStateDontPostProcess(Order order, OrderStateMessage newStateOmsg) { lock (this.orderUpdateLock) {
			if (newStateOmsg.Order != order) {
				string msg = "sorry for athavism, but OrderStateMessage.Order should be equal to order here";
				throw new Exception(msg);
			}
			if (order == null) {
				string msg = "how come ORDER=NULL?";
			}
			if (order.Alert == null) {
				string msg = "how come ORDER.AlertNULL?";
			}
			if (order.Alert.OrderFollowed != order) {
				string msg = "order.Alert.OrderFollowed[" + order.Alert.OrderFollowed + "] != order[" + order + "]";
				//throw new Exception(msg);
			}
			if (order.State == newStateOmsg.State) {
				string msg = "Replace with AppendOrderMessage()! UpdateOrderStateNoPostProcess(): got the same OrderState[" + order.State + "]?";
				//ROUGH_BRO throw new Exception(msg);
				Assembler.PopupException(msg);
				this.appendOrderMessageAndPropagate(order, newStateOmsg);
				return;
			}

			// REPLACED_WITH_OUTER_this.orderUpdateLock
			//if (newStateOmsg.State == OrderState.WaitingBrokerFill) {
			//	// blocking the BrokerAdapter thread updateing OrderState??? protecting against "thread twist" inside of BrokerAdapter implementation?...
			//	bool signalled = order.MreActiveCanCome.WaitOne(-1);
			//}

			OrderState orderStatePriorToUpdate = order.State;
			order.State = newStateOmsg.State;
			order.StateUpdateLastTimeLocal = newStateOmsg.DateTime;
			this.DataSnapshot.SwitchLanesForOrderPostStatusUpdate(order, orderStatePriorToUpdate);

			if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.RaiseOrderStateOrPropertiesChangedExecutionFormShouldDisplay(this, new List<Order>(){order});
			this.appendOrderMessageAndPropagate(order, newStateOmsg);
			// REPLACED_WITH_OUTER_this.orderUpdateLock if (order.State == OrderState.Submitted) order.MreActiveCanCome.Set();}
		} }
		public void UpdateOrderStateAndPostProcess(Order order, OrderStateMessage newStateOmsg,
								double priceFill = 0, double qtyFill = 0) { lock (this.orderUpdateLock) {
			string msig = "UpdateOrderStateAndPostProcess(): ";
			try {
				if (order == null) {
					string msg = "POSSIBLE_END_OF_LIVESIM_LATE_FILL_AFTER_CTX_RESTORED_LIVEBROKER_GONE " + newStateOmsg.ToString();
					Assembler.PopupException(msg);
					return;
				}
				if (order.VictimToBeKilled != null) {
					this.postProcessKillerOrder(order, newStateOmsg);
					return;
				}
				if (order.KillerOrder != null) {
					this.postProcessVictimOrder(order, newStateOmsg);
					return;
				}
				if (order.hasBrokerAdapter("UpdateOrderStateAndPostProcess():") == false) {
					string msg = "most likely QuikTerminal.CallbackOrderStatus got something wrong...";
					Assembler.PopupException(msg);
					return;
				}

				if (newStateOmsg.State == OrderState.Rejected && order.State == OrderState.EmergencyCloseLimitReached) {
					string prePostErrorMsg = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for EmergencyCloseLimitReached "
						//+ "; skipping PostProcess for [" + order + "]"
						;
					this.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, prePostErrorMsg);
					return;
				}
				if (newStateOmsg.State == OrderState.Rejected && order.InStateEmergency) {
					string prePostErrorMsg = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for"
						+ " order.inEmergencyState[" + order.State + "] "
						//+ "; skipping PostProcess for [" + order + "]"
						;
					this.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, prePostErrorMsg);
					return;
				}

				OrderCallbackDupesChecker dupesChecker = order.Alert.DataSource.BrokerAdapter.OrderCallbackDupesChecker;
				if (dupesChecker != null) {
					string whyIthinkBrokerIsSpammingMe = dupesChecker.OrderCallbackIsDupeReson(order, newStateOmsg, priceFill, qtyFill);
					if (string.IsNullOrEmpty(whyIthinkBrokerIsSpammingMe) == false) {
						string msgChecker = "SKIPPING_POSTPROCESS_DUPE[" + whyIthinkBrokerIsSpammingMe + "]; for [" + order + "]";
						this.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msgChecker);
						return;
					}
				}

				if (priceFill != 0) {
					if (order.PriceFill == 0 || order.PriceFill == -999.99) {
						order.PriceFill = priceFill;
						order.QtyFill = qtyFill;
					} else {
						bool marketWasSubstituted = order.Alert.MarketLimitStop == MarketLimitStop.Limit
								&& order.Alert.Bars.SymbolInfo.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
						if (order.PriceFill != priceFill && marketWasSubstituted == false) {
							string msg = "got priceFill[" + priceFill + "] while order.PriceFill=[" + order.PriceFill + "]"
								+ "; weird for Order.Alert.MarketLimitStop=[" + order.Alert.MarketLimitStop + "]";
							order.AppendMessage(msg);
						}
					}
				}
				if (order.State == newStateOmsg.State) {
					this.appendOrderMessageAndPropagate(order, newStateOmsg);
				} else {
					this.UpdateOrderStateDontPostProcess(order, newStateOmsg);
				}
				this.postProcessOrderState(order, priceFill, qtyFill);
			} catch (Exception ex) {
				string msg = "trying to figure out why SL is not getting placed - we didn't reach PostProcess??";
				Assembler.PopupException(msg, ex, true);
			}
		} }
		void postProcessAccounting(Order order, double qtyFill) {
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
		void postProcessOrderState(Order order, double priceFill, double qtyFill) {
			string msig = " " + order.State + " " + order.LastMessage + " //postProcessOrderState()";
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
					order.FilledWith(priceFill, qtyFill);
					this.postProcessAccounting(order, qtyFill);

					if (order.IsEmergencyClose) {
						this.OPPemergency.RemoveEmergencyLockFilled(order);
					}
					this.OPPsequencer.OrderFilledUnlockSequenceSubmitOpening(order);
					try {
						order.Alert.Strategy.Script.Executor.CallbackAlertFilledMoveAroundInvokeScriptNonReenterably(order.Alert, null,
							order.PriceFill, order.QtyFill, order.SlippageFill, order.CommissionFill);
					} catch (Exception ex) {
						string msg3 = "PostProcessOrderState caught from CallbackAlertFilledMoveAroundInvokeScript() ";
						Assembler.PopupException(msg3 + msig, ex);
					}
					break;

				case OrderState.ErrorCancelReplace:
					this.DataSnapshot.OrdersRemove(new List<Order>() { order });
					this.RaiseAsyncOrderRemovedExecutionFormExecutionFormShouldRebuildOLV(this, new List<Order>(){order});
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
							this.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg);
							this.RaiseOrderStateOrPropertiesChangedExecutionFormShouldDisplay(this, new List<Order>(){order});
							return;
						}
					}
			
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig, null, false);
					order.PriceFill = 0;
					//NEVER order.PricePaid = 0;

					if (order.IsEmergencyClose) {
						this.OPPemergency.CreateEmergencyReplacementAndResubmitFor(order);
					} else {
						if (order.Alert.IsExitAlert) {
							this.OPPemergency.AddLockAndCreateEmergencyReplacementAndResubmitFor(order);
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

				default:
					string msg4 = "NO_HANDLER_FOR_ORDER.STATE";
					Assembler.PopupException(msg4 + msig);
					break;
			}
		}

		public void MoveStopLoss(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			if (proto.StopLossAlertForAnnihilation == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.StopLossAlertForAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.StopLossAlertForAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = "MoveStopLoss(" + proto.StopLossActivationNegativeOffset + "/" + proto.StopLossNegativeOffset
				+ "=>" + newActivationOffset + "/" + newStopLossNegativeOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook stopLossGotKilledHook = new OrderPostProcessorStateHook("StopLossGotKilledHook",
				order2killAndReplace, OrderState.KilledPending,
				delegate(Order stopLossKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "StopLossGotKilledHook(): invoking OnStopLossKilledCreateNewStopLossAndAddToPokeUnit() "
						+ "[" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + stopLossKilled.State + "]";
					stopLossKilled.AppendMessage(msg);
					this.onStopLossKilledCreateNewStopLossAndAddToPokeUnit(stopLossKilled, newActivationOffset, newStopLossNegativeOffset, pokeUnit);
				}
			);

			// 2. hook onActive=>kill
			OrderPostProcessorStateHook stopLossReceivedActiveCallback = new OrderPostProcessorStateHook("StopLossReceivedActiveCallback",
				order2killAndReplace, OrderState.WaitingBrokerFill,
				delegate(Order stopLossToBeKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "StopLossReceivedActiveCallback(): invoking KillOrderUsingKillerOrder() "
						+ " [" + stateBeforeActiveAssummingSubmitting + "] => "
						+ "[" + stopLossToBeKilled.State + "]";
					stopLossToBeKilled.AppendMessage(msg);
					stateBeforeKilledAssumingActive = stopLossToBeKilled.State;
					this.KillOrderUsingKillerOrder(order2killAndReplace);
				}
			);

			this.OPPstatusCallbacks.AddStateChangedHook(stopLossReceivedActiveCallback);
			this.OPPstatusCallbacks.AddStateChangedHook(stopLossGotKilledHook);

			this.AppendOrderMessageAndPropagateCheckThrowOrderNull(proto.StopLossAlertForAnnihilation.OrderFollowed, msig + "hooked stopLossReceivedActiveCallback() and stopLossGotKilledHook()");
		}
		void onStopLossKilledCreateNewStopLossAndAddToPokeUnit(Order killedStopLoss, double newActivationOffset, double newStopLossNegativeOffset, ReporterPokeUnit pokeUnit) {
			string msig = "OnStopLossKilledCreateNewStopLossAndAddToPokeUnit(): ";
			ScriptExecutor executor = killedStopLoss.Alert.Strategy.Script.Executor;
			Position position = killedStopLoss.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new StopLossAlert for SellOrCoverRegisterAlerts()
			position.Prototype.StopLossAlertForAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewStopLossOffsets(newStopLossNegativeOffset, newActivationOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateStopLossFromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() {replacement}, true, true);
			pokeUnit.AlertsNew.AddNoDupe(replacement, this, "onStopLossKilledCreateNewStopLossAndAddToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedStopLoss.AppendMessage(msg + msig);
		}
		public void MoveTakeProfit(PositionPrototype proto, double newTakeProfitPositiveOffset) {
			if (proto.TakeProfitAlertForAnnihilation == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.TakeProfitAlertForAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = "MoveTakeProfit(" + proto.TakeProfitPositiveOffset + "=>" + newTakeProfitPositiveOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook takeProfitGotKilledHook = new OrderPostProcessorStateHook("TakeProfitGotKilledHook",
				order2killAndReplace, OrderState.KilledPending,
				delegate(Order takeProfitKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "takeProfitGotKilledHook(): invoking OnTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit() "
						+ " [" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + takeProfitKilled.State + "]";
					takeProfitKilled.AppendMessage(msg);
					this.onTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit(takeProfitKilled, newTakeProfitPositiveOffset, pokeUnit);
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
					this.KillOrderUsingKillerOrder(order2killAndReplace);
				}
			);

			this.OPPstatusCallbacks.AddStateChangedHook(takeProfitReceivedActiveCallback);
			this.OPPstatusCallbacks.AddStateChangedHook(takeProfitGotKilledHook);

			this.AppendOrderMessageAndPropagateCheckThrowOrderNull(proto.TakeProfitAlertForAnnihilation.OrderFollowed, msig + ": hooked takeProfitReceivedActiveCallback() and takeProfitGotKilledHook()");
		}
		void onTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit(Order killedTakeProfit, double newTakeProfitPositiveOffset, ReporterPokeUnit pokeUnit) {
			string msig = "OnTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit(): ";
			ScriptExecutor executor = killedTakeProfit.Alert.Strategy.Script.Executor;
			Position position = killedTakeProfit.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new TakeProfitAlert for SellOrCoverRegisterAlerts()
			position.Prototype.TakeProfitAlertForAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewTakeProfitOffset(newTakeProfitPositiveOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateTakeProfitFromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() { replacement }, true, true);
			pokeUnit.AlertsNew.AddNoDupe(replacement, this, "onTakeProfitKilledCreateNewTakeProfitAndAddToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedTakeProfit.AppendMessage(msg + msig);
		}
		//public void InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter(Order orderWithNewState) {
		//    ScriptExecutor executor = orderWithNewState.Alert.Strategy.Script.Executor;
		//    ReporterPokeUnit afterHooksInvokedPokeUnit = new ReporterPokeUnit();
		//    int hooksInvoked = this.OPPstatusCallbacks.InvokeOnceHooksForOrderStateAndDelete(orderWithNewState, afterHooksInvokedPokeUnit);
		//    if (executor.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) return;

		//    List<Alert> alertsCreatedByHooks = afterHooksInvokedPokeUnit.AlertsNew.SafeCopy(this, "InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter(WAIT)");
		//    if (alertsCreatedByHooks.Count == 0) {
		//        string msg = "NOT_AN_ERROR: ZERO alerts from [" + hooksInvoked + "] hooks invoked; order[" + orderWithNewState + "]";
		//        //this.PopupException(new Exception(msg));
		//        return;
		//    }
		//    bool setStatusSubmitting = executor.IsStreamingTriggeringScript && executor.IsStrategyEmittingOrders;
		//    this.CreateOrdersSubmitToBrokerAdapterInNewThreads(alertsCreatedByHooks, setStatusSubmitting, true);
		//    //ONLY_ON_FILL orderWithNewState.Alert.Strategy.Script.Executor.AddPositionsToChartShadowAndPushPositionsOpenedClosedToReportersAsyncUnsafe(afterHooksInvokedPokeUnit);
		//}

		//[Obsolete("COMPLETE_MESS")]
		//public void PostKillUsingKiller_forBothKillerAndVictim_removeAlertsPendingFromExecutorDataSnapshot(Order orderDeployedKilled_orKillerOrder, string msig) {
		//    string msg = "";
		//    if (orderDeployedKilled_orKillerOrder.State != OrderState.KillerDone) {
		//        if (orderDeployedKilled_orKillerOrder.State == OrderState.KillerDone) {
		//            if (orderDeployedKilled_orKillerOrder.VictimToBeKilled != null) {
		//                msg = "unhealthy killer; VictimToBeKilled=null";
		//            } else {
		//                string msg1 = "healthy killer; we can use killer.Victim.Alert for one rabied victim having no Alert";
		//            }
		//        } else {
		//            msg = "not a killer";
		//            if (orderDeployedKilled_orKillerOrder.IsKiller == true) {
		//                msg = "not a killer but claims to be a killer";
		//            }
		//        }
		//        orderDeployedKilled_orKillerOrder.AppendMessage(msg + msig + " " + orderDeployedKilled_orKillerOrder);
		//        return;
		//    }
		//    Alert alertForOrder = orderDeployedKilled_orKillerOrder.Alert;
		//    if (alertForOrder == null) {
		//        msg = "orderKilled.Alert=null; dunno what to remove from PendingAlerts";
		//        orderDeployedKilled_orKillerOrder.AppendMessage(msg + msig + " " + orderDeployedKilled_orKillerOrder);
		//        return;
		//    }
		//    ScriptExecutor executor = alertForOrder.Strategy.Script.Executor;
		//    try {
		//        executor.CallbackAlertKilledInvokeScriptNonReenterably(alertForOrder);
		//        msg = orderDeployedKilled_orKillerOrder.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d";
		//        orderDeployedKilled_orKillerOrder.AppendMessage(msg + msig);
		//    } catch (Exception e) {
		//        msg = orderDeployedKilled_orKillerOrder.State + " is a Cemetery but [" + e.Message + "]"
		//            + "; comment the State out; alert[" + alertForOrder + "]";
		//        orderDeployedKilled_orKillerOrder.AppendMessage(msg + msig);
		//    }
		//}

		public void PostKillWithoutKiller_removeAlertsPendingFromExecutorDataSnapshot(Order orderPendingKilled, string msig) {
			if (orderPendingKilled.State != OrderState.KilledPending) {
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
				executor.CallbackAlertKilledInvokeScriptNonReenterably(alertForOrder);
				string msg = orderPendingKilled.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d";
				orderPendingKilled.AppendMessage(msg + msig);
			} catch (Exception e) {
				string msg = orderPendingKilled.State + " is a Cemetery but [" + e.Message + "]"
					+ "; comment the State out; alert[" + alertForOrder + "]";
				orderPendingKilled.AppendMessage(msg + msig);
			}
		}
		public void KillPendingOrderWithoutKiller(Order orderPending) {
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;

			string msgPending = "EXPECTING_THREE_MESSAGES_FROM_BROKER_FOR[" + orderPending + "] "
				+ OrderState.KillPendingSubmitting + ">"
				+ OrderState.KillPendingSubmitted + ">"
				+ OrderState.KilledPending;
			OrderStateMessage omsg = new OrderStateMessage(orderPending, OrderState.KillPendingPreSubmit, msgPending);
			this.UpdateOrderStateAndPostProcess(orderPending, omsg);

			orderPending.Alert.DataSource.BrokerAdapter.OrderPendingKillWithoutKillerSubmit(orderPending);
		}
	}
}
