using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacer_Expired_WithoutCallback_WaitingForBrokerFill : OrderPostProcessorReplacer {
		Dictionary<Alert, TimerSimplifiedThreading_withAlert> timeredOrders_forAlert;

		public OrderPostProcessorReplacer_Expired_WithoutCallback_WaitingForBrokerFill(OrderProcessor orderProcessor) : base(orderProcessor) {
			timeredOrders_forAlert = new Dictionary<Alert, TimerSimplifiedThreading_withAlert>();
		}

		TimerSimplifiedThreading_withAlert findOrCreatedTimer_forAlert(Alert alert_forOrder_freshOrStuck) {
			Alert alert = alert_forOrder_freshOrStuck;
			if (this.timeredOrders_forAlert.ContainsKey(alert) == false) {
				int millis_allowedInSubmittedState = alert.Bars.SymbolInfo.IfNoTransactionCallback_MillisAllowed;
				TimerSimplifiedThreading_withAlert newTimer = new TimerSimplifiedThreading_withAlert(alert, millis_allowedInSubmittedState);
				newTimer.OnLastScheduleExpired += new EventHandler<EventArgs>(this.newTimer_OnLastScheduleExpired);
				this.timeredOrders_forAlert.Add(alert, newTimer);
			}
			TimerSimplifiedThreading_withAlert timer = this.timeredOrders_forAlert[alert];
			return timer;
		}

		void newTimer_OnLastScheduleExpired(object sender, EventArgs e) {
			TimerSimplifiedThreading_withAlert timer = sender as TimerSimplifiedThreading_withAlert;
			this.replace_StuckInSubmitted_thereWasNoCallback_afterMillis(timer.Alert.OrderFollowed_orCurrentReplacement);
		}

		public bool ScheduleTimer_forStuckInSubmitted_ifMillisAllowed_nonZero(Order everyOrder) {
			bool timerScheduled = false;
			Alert alert = everyOrder.Alert;
			int millisAllowed_inSubmittedState = alert.Bars.SymbolInfo.IfNoTransactionCallback_MillisAllowed;
			if (millisAllowed_inSubmittedState > 0) {
				TimerSimplifiedThreading_withAlert timer = this.findOrCreatedTimer_forAlert(alert);
				timer.ScheduleOnce_postponeIfAlreadyScheduled();
				timerScheduled = timer.Scheduled;
			}

			string msg = "1/6__STUCK_IN_SUBMITTED__timerScheduled[" + timerScheduled + "]"
				+ " millisAllowed_inSubmittedState[" + millisAllowed_inSubmittedState + "]"
				+ " for SybmolEditor[" + alert.SymbolAndClass + "].IfNoTransactionCallback_MillisAllowed";
			this.OrderProcessor.AppendMessage_propagateToGui(everyOrder, msg);

			return timerScheduled;
		}

		bool replace_StuckInSubmitted_thereWasNoCallback_afterMillis(Order order_stuckInSubmitted_BrokerReconnect) {
			string msig = " //OrderPostProcessorReplacer_Expired_WithoutCallback_WaitingForBrokerFill.replace_StuckInSubmitted_thereWasNoCallback_afterMillis(" + order_stuckInSubmitted_BrokerReconnect + ")";
			bool replacementEmitted = false;

			if (order_stuckInSubmitted_BrokerReconnect.State != OrderState.Submitted) {
				string msg1 = "6/6__NOT_STUCK_IN_SUBMITTED__CANCELLING_REPLACEMENT_NO_BROKER_DISCONNECT[" + order_stuckInSubmitted_BrokerReconnect.State + "]";
				base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg1 + msig);
				//Assembler.PopupException(msg1 + msig, null, false);
				return replacementEmitted;
			}

			string symbolAndClass = order_stuckInSubmitted_BrokerReconnect.Alert.SymbolAndClass;
			SymbolInfo symbolInfo = order_stuckInSubmitted_BrokerReconnect.Alert.Bars.SymbolInfo;

			string msg2 = "2/6__STUCK_IN_SUBMITTED__REPLACING[" + order_stuckInSubmitted_BrokerReconnect.State + "]";
			base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg2 + msig);
			//Assembler.PopupException(msg2 + msig, null, false);

			try {
				bool shallReconnect = symbolInfo.IfNoTransactionCallback_ReconnectBrokerDll;
				string reason = " KOZ_SymbolInfo[" + symbolAndClass + "].IfNoTransactionCallback_ReconnectBrokerDll=[" + shallReconnect + "]";
				BrokerAdapter broker = base.GetBrokerAdapter_fromOrder_nullUnsafe(order_stuckInSubmitted_BrokerReconnect, msig);
				string brokerName = broker != null ? broker.Name : "BROKER_IS_NULL_FOR_ORDER[" + order_stuckInSubmitted_BrokerReconnect.GUID + "]";
				if (shallReconnect && broker != null) {
					string reasonReconnect = "3/6__STUCK_IN_SUBMITTED__RECONNECTING_BROKER[" + brokerName + "]";
					broker.Broker_reconnect_waitConnected(reasonReconnect + reason + msig);
					bool emittingCapable = broker.EmittingCapable;
					reasonReconnect += " emittingCapable[" + emittingCapable + "]";
					base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, reasonReconnect + reason + msig);
				} else {
					string msg = "3/6__STUCK_IN_SUBMITTED__WONT_RECONNECT_BROKER[" + brokerName + "]";
					if (broker == null) reason = " KOZ_BROKER_IS_NULL_FOR_ORDER[" + order_stuckInSubmitted_BrokerReconnect.GUID  + "]";
					base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + reason + msig);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}

	

			try {
				bool shallKill = symbolInfo.IfNoTransactionCallback_Kill_StuckInSubmitted;
				string reason = " KOZ_SymbolInfo[" + symbolAndClass + "].IfNoTransactionCallback_Kill_StuckInSubmitted[" + shallKill + "]";
				if (shallKill == false) {
					string msg = "4/6__STUCK_IN_SUBMITTED__WONT_KILL";
					base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + reason + msig);
					//Assembler.PopupException(msg + msig);
					//return replacementScheduled;
				} else {
					bool killerEmitted = this.OrderProcessor.Emit_killOrderPending_usingKiller_hookNeededAfterwards(order_stuckInSubmitted_BrokerReconnect, msig);
					string msg = "4/6__STUCK_IN_SUBMITTED__KillerGUID[" + order_stuckInSubmitted_BrokerReconnect.KillerGUID + "] killerEmitted[" + killerEmitted + "]";
					base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + reason + msig);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}



			if (order_stuckInSubmitted_BrokerReconnect.ReplacedByGUID != "") {
				string msg = "?? THIS_ORDER_WAS_ALREADY_REPLACED[" + order_stuckInSubmitted_BrokerReconnect.GUID  + "]";
				base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + msig);
				Assembler.PopupException(msg + msig);
				return replacementEmitted;
			}

			Alert alert = order_stuckInSubmitted_BrokerReconnect.Alert;
			if (this.timeredOrders_forAlert.ContainsKey(alert) == false) {
				string msg = "STUCK_IN_SUBMITTED__YOU_SHOULD_HAVE_CREATED_TIMER_BEFORE_EMITTING_ORIGINAL__IN_EmitOrders_ownOneThread_forAllNewAlerts()";
				base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + msig);
				Assembler.PopupException(msg + msig);
				return replacementEmitted;
			}
			TimerSimplifiedThreading_withAlert timer = this.timeredOrders_forAlert[alert];

			int limit = symbolInfo.IfNoTransactionCallback_ResubmitLimit;
			int attemptedEmitted = timer.ReplacementOrders_attempted.Count;
			if (limit > 0 &&  attemptedEmitted >= limit) {
				string msg = "5/6__STUCK_IN_SUBMITTED__WONT_REPLACE__LIMIT_EXCEEDED attemptedEmitted[" + attemptedEmitted + "]>=limit[" + limit + "]"
					+ " timer.ReplacementOrders_alreadyEmitted_asGuidCSV[" + timer.ReplacementOrders_alreadyEmitted_asGuidCSV + "]";
				base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + msig);
				return replacementEmitted;
			}

			try {
				bool shallReplace = symbolInfo.IfNoTransactionCallback_Resubmit;
				string reason = " KOZ_SymbolInfo[" + symbolAndClass + "].IfNoTransactionCallback_Resubmit[" + shallReplace + "]";
				if (shallReplace == false) {
					string msg = "5/6__STUCK_IN_SUBMITTED__WONT_REPLACE";
					base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + reason + msig);
					//Assembler.PopupException(msg);
				} else {
					string msg1 = "5/6__STUCK_IN_SUBMITTED__ATTEMPT[" + (attemptedEmitted + 1) + "]/limit[" + limit + "]";
					OrderStateMessage osm1 = new OrderStateMessage(order_stuckInSubmitted_BrokerReconnect,
						OrderState.StuckInSubmitted_Replacing, msg1 + msig);
					base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(osm1);

					replacementEmitted = base.ReplaceOrder_withoutHook(order_stuckInSubmitted_BrokerReconnect, msig, true, false, false);
					Order replacement = order_stuckInSubmitted_BrokerReconnect.DerivedOrder_Last;
					timer.ReplacementOrders_attempted.Add(replacement);
					string msg = "6/6__STUCK_IN_SUBMITTED__REPLACED_WITH[" + replacement + "] emitted[" + replacementEmitted + "]";
					//base.OrderProcessor.AppendMessage_propagateToGui(order_stuckInSubmitted_BrokerReconnect, msg + reason + msig);
					OrderStateMessage osm2 = new OrderStateMessage(order_stuckInSubmitted_BrokerReconnect,
						OrderState.StuckInSubmitted_Replaced, msg + reason + msig);
					base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(osm2);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
	
			return replacementEmitted;
		}	
	}
}