using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Accounting;
using Sq1.Core.Execution;
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
			if (positionShouldBeFilled.EntryFilled_price <= 0) {
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
				order.appendMessage(errormsg);
				exitOrderHasNoErrors = false;
			}
			return exitOrderHasNoErrors;
		}
		Order createOrder_propagateToGui_fromAlert(Alert alert, bool setStatusSubmitting, bool emittedByScript) {
			Order newborn = new Order(alert, emittedByScript, false);
			try {
				newborn.Alert.DataSource_fromBars.BrokerAdapter.Order_modifyOrderType_priceRequesting_accordingToMarketOrderAs(newborn);
			} catch (Exception e) {
				string msg = "hoping that MarketOrderAs.MarketMinMax influenced order.Alert.MarketLimitStop["
					+ newborn.Alert.MarketLimitStop + "]=MarketLimitStop.Limit for further match; PREV=" + newborn.LastMessage;
				this.AppendMessage_propagateToGui(newborn, msg);
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
			this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(new OrderStateMessage(newborn, newbornOrderState, newbornMessage));
			this.DataSnapshot.OrderInsert_notifyGuiAsync(newborn);
			return newborn;
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
			Task taskEmittingOrders = new Task(delegate {
				broker.SubmitOrders_backtest_liveFromProcessor_OPPunlockedSequence_threadEntry(orders);
			});
			taskEmittingOrders.Start();
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
				this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				return false;
			}
			return true;
		}

		public void AppendOrderMessage_propagateToGui(OrderStateMessage omsg) {
			//log.Debug(omsg.Message);
			if (string.IsNullOrEmpty(omsg.Message)) {
				string msg = "I_REFUSE_TO_APPEND_AND_DISPLAY_EMPTY_MESSAGE omsg[" + omsg.ToString() + "]";
				Assembler.PopupException(msg);
				return;
			}
			Order order = omsg.Order;
			order.appendOrderMessage(omsg);
			if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.RaiseOrderMessageAdded_executionControlShouldPopulate(this, omsg);
		}
		public void AppendMessage_propagateToGui(Order order, string msg) {
			if (order == null) {
				throw new Exception("order=NULL! you don't want to get NullPointerException and debug it");
			}
			OrderStateMessage omsg = new OrderStateMessage(order, msg);
			this.AppendOrderMessage_propagateToGui(omsg);
		}

		void postProcess_victimOrder(OrderStateMessage newStateOmsg) {
			Order victimOrder = newStateOmsg.Order;
			this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(newStateOmsg);
			switch (victimOrder.State) {
				case OrderState.VictimsBulletPreSubmit:
				case OrderState.VictimsBulletSubmitted:
				case OrderState.VictimsBulletConfirmed:
				case OrderState.VictimsBulletFlying:
				case OrderState.SLAnnihilated:
				case OrderState.TPAnnihilated:
					break;

				case OrderState.Submitting:
				case OrderState.WaitingBrokerFill:
				case OrderState.Filled:
					break;

				case OrderState.VictimKilled:
					if (victimOrder.FindState_inOrderMessages(OrderState.SLAnnihilating)) {
						this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.SLAnnihilated,
								"PROTOTYPE_FILLED__COUNTERPARTY_ANNIHILATION_SUCCEEDED"));
					}
					if (victimOrder.FindState_inOrderMessages(OrderState.TPAnnihilating)) {
						this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.TPAnnihilated,
								"PROTOTYPE_FILLED__COUNTERPARTY_ANNIHILATION_SUCCEEDED"));
					}

					Order killerOrder = victimOrder.KillerOrder;

					string msg_killer = "orderKiller[" + killerOrder.SernoExchange + "]=>[" + OrderState.KillerDone + "] <= orderVictim[" + victimOrder.SernoExchange + "][" + victimOrder.State + "]";
					OrderStateMessage omg_done_killer = new OrderStateMessage(killerOrder, OrderState.KillerDone, msg_killer + " //postProcess_victimOrder()");
					this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(omg_done_killer);
					this.BrokerCallback_pendingKilled_withKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(victimOrder, msg_killer);
					break;

				default:
					string msg = "postProcess_victimOrder() NO_HANDLER_FOR_ORDER_VICTIM [" + victimOrder + "]'s state[" + victimOrder.State + "]"
						+ "your BrokerAdapter should call for Victim.States:{"
						//+ OrderState.KillSubmitting + ","
						+ OrderState.VictimsBulletFlying + ","
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
				case OrderState.KillerTransSubmittedOK:
				case OrderState.KillerBulletFlying:
				case OrderState.KillerDone:
					this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(newStateOmsg);
					break;

				default:
					string msg = "postProcess_killerOrder(): NO_HANDLER_FOR_KILLE_ORDER [" + killerOrder + "]'s state[" + killerOrder.State + "]"
						+ "your BrokerAdapter should call for Killer.States:{"
						+ OrderState.KillerTransSubmittedOK + ","
						+ OrderState.KillerBulletFlying + ","
						+ OrderState.KillerDone + "}";
					break;
					//throw new Exception(msg);
			}
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
					if (order.SlippageApplied == 0
						// && order.Alert.MarketLimitStop == MarketLimitStop.Market
						// && order.Alert.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker
							) {
						if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
							slippageByFact = order.PriceFilled - order.CurrentBid;
						} else {
							slippageByFact = order.PriceFilled - order.CurrentAsk;
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
							order.PriceFilled, order.QtyFill, order.SlippageFilled, order.CommissionFill);
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
				case OrderState.ErrorSubmittingOrder:
				case OrderState.ErrorSlippageCalc:
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig);
					order.PriceFilled = 0;
					//NEVER order.PricePaid = 0;
					break;

				case OrderState.ErrorSubmitting_BrokerTerminalDisconnected:
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
							this.AppendMessage_propagateToGui(order, msg);
							this.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){order});
							return;
						}
					}
			
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig, null, false);
					order.PriceFilled = 0;
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
				case OrderState.VictimsBulletPreSubmit:
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

	}
}
