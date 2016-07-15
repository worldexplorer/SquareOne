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
			OrderLane	suggestedLane_nullUnsafe = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			Order orderFound = this.DataSnapshot.OrdersSubmitting.ScanRecent_forOrderGuid(orderGUID, out suggestedLane_nullUnsafe, out suggestion, false);
			if (orderFound == null) {
				 orderFound = this.DataSnapshot.OrdersAll.ScanRecent_forOrderGuid(orderGUID, out suggestedLane_nullUnsafe, out suggestion, false);		//pass suggestLane=true 
			}
			if (orderFound == null) {
				string msg = "order[" + orderGUID + "] wasn't found; OrderProcessorDataSnapshot.OrderCount=[" + this.DataSnapshot.OrderCount + "]"
					+ " pass suggestLane=true to ScanRecent_forOrderGuid() 4 lines above";
				throw new Exception(msg);
				//log.Fatal(msg, new Exception(msg));
				//return;
			}
			OrderState orderStateAbsorbed = (orderState == OrderState.LeaveTheSame) ? orderFound.State : orderState;
			if (orderStateAbsorbed != orderFound.State) {
				OrderStateMessage osm = new OrderStateMessage(orderFound, orderStateAbsorbed, message);
				this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(osm);
			} else {
				this.AppendMessage_propagateToGui(orderFound, message);
			}
			return orderFound;
		}
		public void BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(OrderStateMessage omsg_sameState) {
			Order order = omsg_sameState.Order;
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
			this.AppendOrderMessage_propagateToGui(omsg_sameState);

			string state_asString = Enum.GetName(typeof(OrderState), omsg_sameState.State);
			bool newState_isUnderscored_thatOrderNeverGets_asyncCallbacksFromBroker = state_asString.StartsWith("_");
			if (newState_isUnderscored_thatOrderNeverGets_asyncCallbacksFromBroker) return;

			if (omsg_sameState.State == order.State) {
				string msg = "I_REFUSE_TO_UPDATE_ORDER_WITH_SAME_STATE USE_INSTEAD__OrderProcessor.AppendOrderMessage_propagateToGui()  //Order_updateState_switchLanes_appendMessage_propagateIfGuiHasTime__dontPostProcess()";
				Assembler.PopupException(msg, null, false);
				return;
			}

			//if (omsg_sameState.State == OrderState.Filled) {
			//    string msg = "CATCHING_PENDING_ALERT_NON_REMOVAL";
			//    Assembler.PopupException(msg);
			//}

			OrderState orderState_priorToUpdate = order.State;
			order.SetState_localTime_fromMessage(omsg_sameState);
			this.DataSnapshot.SwitchLanes_forOrder_postStatusUpdate(order, orderState_priorToUpdate);

			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (order.Alert.GuiHasTime_toRebuildReportersAndExecution == false) return;
			this.RaiseOnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(this, new List<Order>(){order});
		}
		public void BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(OrderStateMessage omsg_newState, double priceFill = 0, double qtyFill = 0) {
			Order order = omsg_newState.Order;
			string msig = " // " + order.State + "=>" + omsg_newState.State + " BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(" + order + ")";

			if (order == null) {
				string msg = "POSSIBLE_END_OF_LIVESIM_LATE_FILL_AFTER_CTX_RESTORED_LIVEBROKER_GONE " + omsg_newState.ToString();
				Assembler.PopupException(msg + msig);
				return;
			}

			if (order.IsKiller) {
				this.postProcess_killerOrder(omsg_newState);
				return;
			}
			if (order.IsVictim) {
				bool tryOrderFill = this.postProcess_victimOrder(omsg_newState);
				if (tryOrderFill == false) {
					// irrelevant after postProcess_victimOrder() returns TRUE by default
					//if (omsg_newState.State == OrderState.VictimKilled && omsg_newState.State == OrderState.VictimKilled) {
					//    return;					// VictimKilled here - from emulateBulletHitVictim<=
					//}
					//// still set the status... what a mess!
					//this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(omsg_newState);
					return;
				}
			}

			bool rejectedOrExpired =	omsg_newState.State == OrderState.Rejected ||
										omsg_newState.State == OrderState.LimitExpired;
			if (rejectedOrExpired) {
				string stateChange = " order.State[" + order.State + "]=>Rejected || LimitExpired";
				switch (order.State) {
					case OrderState.EmergencyCloseLimitReached:
						string msg1 = "BrokerAdapter CALLBACK DUPE: Status[" + omsg_newState.State + "] delivered for EmergencyCloseLimitReached "
							//+ "; skipping PostProcess for [" + order + "]"
							 + stateChange;
						this.AppendMessage_propagateToGui(order, msg1 + msig);
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
						string msg3 = "BrokerAdapter CALLBACK DUPE: Status[" + omsg_newState.State + "] delivered for"
							+ " order.inEmergencyState[" + order.State + "] "
							//+ "; skipping PostProcess for [" + order + "]"
							 + stateChange;
						this.AppendMessage_propagateToGui(order, msg3 + msig);
						return;

					default:
						string msg_default = "NO_HANDLER_WHILE" + stateChange;
						this.AppendMessage_propagateToGui(order, msg_default);
						Assembler.PopupException(msg_default + msig, null, true);
						break;
				}
			}

			if (order.State == omsg_newState.State) {
				string msg = "I_REFUSE_TO_POST_PROCESS USE_INSTEAD__OrderProcessor.Order_appendPropagateMessage_updateStateIfDifferent_switchLanes___dontPostProcess()  //Order_appendPropagateMessage_updateStateMustBeDifferent_switchLanes_postProcess()";
				//Assembler.PopupException(msg + msig, null, false);
				//this.AppendMessage_propagateToGui(order, msg + msig);
				this.AppendOrderMessage_propagateToGui(omsg_newState);
				return;
			}

			if (priceFill != 0) {
				if (order.PriceFilled != 0) {
					string msg1 = "ORDER_WAS_ALREADY_FILLED NYI:PARTIAL_FILL_WITH_DIFFERENT_FILL_PRICE";
					Assembler.PopupException(msg1, null, false);
					this.AppendMessage_propagateToGui(order, msg1 + msig);

					bool marketWasSubstituted = order.Alert.MarketLimitStop == MarketLimitStop.Limit
							&& order.Alert.Bars.SymbolInfo.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
					if (order.PriceFilled != priceFill && marketWasSubstituted == false) {
						string msg = "got priceFill[" + priceFill + "] while order.PriceFill=[" + order.PriceFilled + "]"
							+ "; weird for Order.Alert.MarketLimitStop=[" + order.Alert.MarketLimitStop + "]";
						this.AppendMessage_propagateToGui(order, msg + msig);
					}
				}
			}

			// for LIMIT orders, quik reports price!=0 & qty=0
			if (qtyFill != 0) {
				bool stateIsOkay =	omsg_newState.State != OrderState.Filled ||
									omsg_newState.State != OrderState.FilledPartially;
				if (stateIsOkay == false) {
					string msg = "YOU_MUST_INTENT_TO_SET_STATE_FILLED_WHEN_QTY_FILL!=0";
					Assembler.PopupException(msg, null, false);
					this.AppendMessage_propagateToGui(order, msg + msig);
				}
				// postProcess()_WILL_DO_IT order.FillWith(priceFill, qtyFill);

				if (order.QtyFill != 0) {
					string msg = "ORDER_WAS_ALREADY_FILLED order.QtyFill[" + order.QtyFill + "] qtyFill[" + qtyFill + "]!= 0";
					Assembler.PopupException(msg, null, false);
					this.AppendMessage_propagateToGui(order, msg + msig);
				}
			}

			bool hasFilledMsg			= order.FindState_inOrderMessages(OrderState.Filled);
			if (omsg_newState.State == OrderState.Filled && hasFilledMsg) {
				string msg = "IM_NOT_FILLING_ORDER_TWICE";
				this.AppendMessage_propagateToGui(order, msg + omsg_newState.Message);
				return;
			}

			bool hasFilledPartiallyMsg	= order.FindState_inOrderMessages(OrderState.FilledPartially);
			if (omsg_newState.State == OrderState.FilledPartially && hasFilledPartiallyMsg) {
				string msg = "IM_NOT_PARTIALLY_FILLING_ORDER_TWICE (TODO I didn't check for different size)";
				this.AppendMessage_propagateToGui(order, msg);
				return;
			}

			this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(omsg_newState);
			this.postProcess_invokeScriptCallback(order, priceFill, qtyFill);
			this.OPPstatusCallbacks.InvokeHooks_forOrderState_unregisterInvoked(order, null);
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
			Alert alert_forVictim = orderPending_victimKilled.Alert;
			if (alert_forVictim == null) {
				string msg = "orderPending_victimKilled.Alert=null; dunno what to remove from PendingAlerts";
				orderPending_victimKilled.AppendMessage(msg + msigInvoker + " " + orderPending_victimKilled);
				Assembler.PopupException(msg);
				return;
			}
			ScriptExecutor executor = alert_forVictim.Strategy.Script.Executor;

			#region ALERT_PENDING_REMOVAL_FOR_REJECTED__POSTPONED_FOR_REPLACEMENT_GETS_FILL
			bool killedAfterRejected		= orderPending_victimKilled.State == OrderState.Rejected;
			bool replacementWillBeSubmitted	= orderPending_victimKilled.Alert.Bars.SymbolInfo.RejectedResubmit;
			bool replacementFilledWillRemove = killedAfterRejected && replacementWillBeSubmitted;
			if (replacementFilledWillRemove) {
				executor.CallbackOrderKilled_orBrokerDeniedSubmission_addGrayCross_onChart(orderPending_victimKilled);
				return;
			}
			#endregion

			bool inRightState =
			    //orderPending_victimKilled.State == OrderState.KillerTransSubmittedOK ||		// ???
			    orderPending_victimKilled.State == OrderState.SLAnnihilated ||	// prototype hit TP
			    orderPending_victimKilled.State == OrderState.TPAnnihilated ||	// prototype hit TP
				orderPending_victimKilled.State == OrderState.VictimKilled		// ScriptExecutor.PositionCloseImmediately()
				// REMOVED_ONSUBMIT_AND_IN_CALLBACK
				|| orderPending_victimKilled.State == OrderState.Rejected		// kill-by-DoubleClick and ReplacerRejected+RejectedKillBeforeReplacing
				;

			if (inRightState == false) {
				string msg = "VICTIM_NOT_REMOVED_FROM_PENDINGS "
					+ "State[" + orderPending_victimKilled.State + "] MUST_BE{KillTransSubmittedOK,SLAnnihilated,TPAnnihilated}";
				Assembler.PopupException(msg);
				return;
			}

			try {
				executor.CallbackAlertKilled_invokeScript_nonReenterably(alert_forVictim);
				string msg = orderPending_victimKilled.State + " => AlertsPending.Remove.Remove(orderExecuted.Alert)'d ";
				orderPending_victimKilled.AppendMessage(msg + msigInvoker);
			} catch (Exception e) {
				string msg = orderPending_victimKilled.State + " is a Cemetery but [" + e.Message + "]"
					+ "; comment the State out; alert[" + alert_forVictim + "] ";
				orderPending_victimKilled.AppendMessage(msg + msigInvoker);
			}
		}
	}
}
