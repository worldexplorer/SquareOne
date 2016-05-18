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

		public Order BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(string orderGUID, OrderState orderState, string message) {
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
				this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(osm);
			} else {
				this.AppendMessage_propagateToGui(orderFound, message);
			}
			return orderFound;
		}
		public void BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(OrderStateMessage newStateWithReason) {
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
			//		string msg = "";
			//		Assembler.PopupException(msg, null, false);
			//} else {
			//	if (order.Alert.OrderFollowed != order) {
			//		string msg = "ORDER.Alert.OrderFollowed[" + order.Alert.OrderFollowed + "] != order[" + order + "]";
			//		Assembler.PopupException(msg, null, false);
			//	}
			//}

			// append message in any case; in messages log it will have the underscored state (message log refreshed), while the order itself will never have it (orders tree is not updated)
			this.AppendOrderMessage_propagateToGui(newStateWithReason);

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
		public void BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(OrderStateMessage newStateOmsg, double priceFill = 0, double qtyFill = 0) {
			Order order = newStateOmsg.Order;
			string msig = "BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(): ";

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

			if (newStateOmsg.State == OrderState.Rejected) {
				string stateChange = " order.State[" + order.State + "]=>OrderState.Rejected";
				switch (order.State) {
					case OrderState.EmergencyCloseLimitReached:
						string msg1 = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for EmergencyCloseLimitReached "
							//+ "; skipping PostProcess for [" + order + "]"
							 + stateChange;
						this.AppendMessage_propagateToGui(order, msg1);
						return;

					case OrderState.WaitingBrokerFill:
						//v1
						//string msg2 = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for EmergencyCloseLimitReached " + stateChange;;
						//this.AppendMessage_propagateToGui(order, msg2);
						//this.postProcess_invokeScriptCallback(newStateOmsg.Order, -1, -1);
						//v2
						//LET_IT_FALL_THROUGH_30_LINES_BELOW_I_WILL_HANDLE_THIS_IN postProcess_invokeScriptCallback()
						break;

					case OrderState.EmergencyCloseSheduledForRejected:					//THESE_THREE_ARE_EQUIVALENT_TO if (order.InState_emergency) {
					case OrderState.EmergencyCloseSheduledForRejectedLimitReached:
					case OrderState.EmergencyCloseSheduledForErrorSubmittingBroker:
						string msg3 = "BrokerAdapter CALLBACK DUPE: Status[" + newStateOmsg.State + "] delivered for"
							+ " order.inEmergencyState[" + order.State + "] "
							//+ "; skipping PostProcess for [" + order + "]"
							 + stateChange;;
						this.AppendMessage_propagateToGui(order, msg3);
						return;

					default:
						string msg_default = "NO_HANDLER_WHILE" + stateChange;
						this.AppendMessage_propagateToGui(order, msg_default);
						Assembler.PopupException(msg_default, null, true);
						break;
				}
			}

			if (order.State == newStateOmsg.State) {
				string msg = "I_REFUSE_TO_POST_PROCESS USE_INSTEAD__OrderProcessor.Order_appendPropagateMessage_updateStateIfDifferent_switchLanes___dontPostProcess()  //Order_appendPropagateMessage_updateStateMustBeDifferent_switchLanes_postProcess()";
				Assembler.PopupException(msg, null, false);
				return;
			}

			if (priceFill != 0) {
				if (order.PriceFilled != 0) {
					string msg1 = "ORDER_WAS_ALREADY_FILLED NYI:PARTIAL_FILL_WITH_DIFFERENT_FILL_PRICE";
					Assembler.PopupException(msg1, null, false);
					order.appendMessage(msg1);

					bool marketWasSubstituted = order.Alert.MarketLimitStop == MarketLimitStop.Limit
							&& order.Alert.Bars.SymbolInfo.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
					if (order.PriceFilled != priceFill && marketWasSubstituted == false) {
						string msg = "got priceFill[" + priceFill + "] while order.PriceFill=[" + order.PriceFilled + "]"
							+ "; weird for Order.Alert.MarketLimitStop=[" + order.Alert.MarketLimitStop + "]";
						order.appendMessage(msg);
					}
				}
			}

			// for LIMIT orders, quik reports price!=0 & qty=0
			if (qtyFill != 0 && order.QtyFill == 0) {
				if (newStateOmsg.State != OrderState.Filled || newStateOmsg.State != OrderState.Filled) {
					string msg = "YOU_MUST_INTENT_TO_SET_STATE_FILLED_WHEN_QTY_FILL!=0";
					Assembler.PopupException(msg, null, false);
					order.appendMessage(msg);
				}
				// postProcess()_WILL_DO_IT order.FillWith(priceFill, qtyFill);
			}

			this.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(newStateOmsg);
			this.postProcess_invokeScriptCallback(order, priceFill, qtyFill);
			this.OPPstatusCallbacks.InvokeHooks_forOrderState_deleteInvoked(order, null);
		}
		//public void BrokerCallback_pendingKilled_withoutKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(Order orderPending_victimKilled, string msigInvoker) {
		//	if (orderPending_victimKilled.State != OrderState.KillerTransSubmittedOK) {
		//		string msg = "I_SERVE_ONLY_BROKER_CONFIRMED_VICTIMS_WITH_STATE=KillTransSubmittedOK";
		//		Assembler.PopupException(msg);
		//		return;
		//	}
		//	Alert alert_forVictim = orderPending_victimKilled.Alert;
		//	if (alert_forVictim == null) {
		//		string msg = "orderPending_victimKilled.Alert=null; dunno what to remove from PendingAlerts";
		//		orderPending_victimKilled.AppendMessage(msg + msigInvoker + " " + orderPending_victimKilled);
		//		return;
		//	}
		//	ScriptExecutor executor = alert_forVictim.Strategy.Script.Executor;
		//	try {
		//		executor.CallbackAlertKilled_invokeScript_nonReenterably(alert_forVictim);
		//		string msg = orderPending_victimKilled.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d";
		//		orderPending_victimKilled.AppendMessage(msg + msigInvoker);
		//	} catch (Exception e) {
		//		string msg = orderPending_victimKilled.State + " is a Cemetery but [" + e.Message + "]"
		//			+ "; comment the State out; alert[" + alert_forVictim + "]";
		//		orderPending_victimKilled.AppendMessage(msg + msigInvoker);
		//	}
		//}
		public void BrokerCallback_pendingKilled_withKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(Order orderPending_victimKilled, string msigInvoker) {
			bool inRightState =
			    //orderPending_victimKilled.State == OrderState.KillerTransSubmittedOK ||		// ???
			    orderPending_victimKilled.State == OrderState.SLAnnihilated ||	// prototype hit TP
			    orderPending_victimKilled.State == OrderState.TPAnnihilated ||	// prototype hit TP
				orderPending_victimKilled.State == OrderState.VictimKilled;		// ScriptExecutor.PositionCloseImmediately()

			if (inRightState == false) {
				string msg = "I_SERVE_ONLY_BROKER_CONFIRMED_VICTIMS_WITH_STATE {KillTransSubmittedOK,SLAnnihilated,TPAnnihilated}";
				Assembler.PopupException(msg);
				return;
			}

			Alert alert_forVictim = orderPending_victimKilled.Alert;
			if (alert_forVictim == null) {
				string msg = "orderPending_victimKilled.Alert=null; dunno what to remove from PendingAlerts";
				orderPending_victimKilled.appendMessage(msg + msigInvoker + " " + orderPending_victimKilled);
				return;
			}
			ScriptExecutor executor = alert_forVictim.Strategy.Script.Executor;
			try {
				executor.CallbackAlertKilled_invokeScript_nonReenterably(alert_forVictim);
				string msg = orderPending_victimKilled.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d";
				orderPending_victimKilled.appendMessage(msg + msigInvoker);
			} catch (Exception e) {
				string msg = orderPending_victimKilled.State + " is a Cemetery but [" + e.Message + "]"
					+ "; comment the State out; alert[" + alert_forVictim + "]";
				orderPending_victimKilled.appendMessage(msg + msigInvoker);
			}
		}
	}
}
