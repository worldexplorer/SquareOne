using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacerExpired : OrderPostProcessorReplacer {
		Dictionary<Order, TimerSimplifiedThreading_withOrder> timeredOrders;

		public OrderPostProcessorReplacerExpired(OrderProcessor orderProcessor) : base(orderProcessor) {
			timeredOrders = new Dictionary<Order, TimerSimplifiedThreading_withOrder>();
		}

		bool orderCanBeReplaced_disposeRemoveIfFilled(Order order, string msig_invoker) {
			bool ret = false;

			bool shallReplace = order.Alert.Bars.SymbolInfo.ReSubmitLimitNotFilled;
			if (shallReplace == false) {
				string msg = "Symbol[" + order.Alert.Bars.Symbol + "].ReSubmitLimitNotFilled=[" + shallReplace +  "]; returning";
				Assembler.PopupException(msg + msig_invoker, null, false);
				return ret;
			}

			int expiredMillis = order.Alert.Bars.SymbolInfo.ReSubmitLimitNotFilledWithinMillis;
			if (expiredMillis <= 0) {
				string msg = "Symbol[" + order.Alert.Bars.Symbol + "].ReSubmitLimitNotFilledWithinMillis=[" + expiredMillis +  "]; returning";
				Assembler.PopupException(msg + msig_invoker, null, false);
				return ret;
			}

			string reasonCanNotBeReplaced = "";
			if (order.QtyFill > 0 || order.PriceFilled > 0) {
				reasonCanNotBeReplaced += "order.QtyFill[" + order.QtyFill + "] > 0 || order.PriceFilled[" + order.PriceFilled + "] > 0 ";
			}
			if (order.State != OrderState.WaitingBrokerFill) {
				reasonCanNotBeReplaced += "ORDER_ISNT_WAITING_FOR_FILL_ANYMORE[" + order.State + "] ";
			}
			if (string.IsNullOrEmpty(order.ReplacedByGUID) == false) {
				reasonCanNotBeReplaced += "YOU_SHOULD_HAVE_CANCELLED_TIMER_EARLIER__KOZ_ORDER_WAS_ALREADY_REPLACED order.ReplacedByGUID[" + order.ReplacedByGUID + "] ";
			}
			if (order.HasSlippagesDefined && order.SlippagesLeftAvailable_noMore) {
				reasonCanNotBeReplaced += "NoMoreSlippagesAvailable "
					+ " order.SlippageAppliedIndex[" + order.SlippageAppliedIndex + "]=[" + order.SlippageApplied + "]"
					+ " order.SlippagesLeftAvailable[" + order.SlippagesLeftAvailable + "]";
				//REMOVES_PENDING_WHILE_WaitingBrokerFill__MOVED_TO_ReplaceRejected_ifResubmitRejected_setInSymbolInfo() base.AddMessage_noMoreSlippagesAvailable(order);
			}

			if (string.IsNullOrEmpty(reasonCanNotBeReplaced)) {
				ret = true;		// you CAN replace the order
				return ret;
			}

			this.OrderProcessor.AppendMessage_propagateToGui(order, "CAN_NOT_BE_REPLACED " + reasonCanNotBeReplaced);
			this.removeTimer_forOrder(order, msig_invoker);
			return ret;
		}

		public void AllTimers_stopDispose_LivesimEnded(List<Alert> alertsPending, string scriptStopped) {
			string msig = " //AllTimers_stopDispose_LivesimEnded(" + scriptStopped + ")";
			foreach (Alert pending in alertsPending) {
				if (pending.OrderFollowed == null) continue;
				bool pumpMustAlreadyStopped_atLivesimEndedOrAborted = false;
				this.removeTimer_forOrder(pending.OrderFollowed, msig, pumpMustAlreadyStopped_atLivesimEndedOrAborted);
			}
		}

		void removeTimer_forOrder(Order order, string msig_invoker = "", bool waitForReplacementComplete_andUnpauseAll = true) {
			// "ORDER_CAN_NOT_BE_REPLACED"
			if (this.timeredOrders.ContainsKey(order)) {
				string reasonCanNotBeReplaced = "CANCELLING_TIMER ";

				// timer exists => now I have to stop & dispose the timer
				TimerSimplifiedThreading_withOrder timerItself = this.timeredOrders[order];
				if (timerItself != null) {
					timerItself.OnLastScheduleExpired -= new EventHandler<EventArgs>(this.timerForOrder_OnLastScheduleExpired);
					reasonCanNotBeReplaced += timerItself.ElapsedVsDelayed_asString;
					timerItself.Dispose();
				}
				this.timeredOrders.Remove(order);

				this.OrderProcessor.AppendMessage_propagateToGui(order, reasonCanNotBeReplaced);
			} else {
				string reasonCanNotBeReplaced = "NO_TIMER_TO_CANCEL ";
				this.OrderProcessor.AppendMessage_propagateToGui(order, reasonCanNotBeReplaced);

				string msg2 = "LAST_SLIPPAGE_BASED_ORDER__HAS_NO_TIMER_TO_DELETE";
				Assembler.PopupException(msg2 + msig_invoker + reasonCanNotBeReplaced, null, false);
			}

			if (waitForReplacementComplete_andUnpauseAll == false) return;

			bool mostLikelyUnpaused = order.OrderReplacement_Emitted_afterOriginalKilled__orError.WaitOne(0);
			if (mostLikelyUnpaused) return;
			base.UnpauseAll_signalReplacementComplete(order, null);		// otherwize after last slippage => rejected => stays paused => backlog grew[10]
		}
		public bool ScheduleReplace_ifExpired(Order order) {
			string msig = " //ScheduleReplace_ifExpired(" + order + ")";
			bool replacementScheduled = false;

			bool orderEligible_forReplacement = this.orderCanBeReplaced_disposeRemoveIfFilled(order, msig);
			if (orderEligible_forReplacement == false) return replacementScheduled;

			int expiredMillis = order.Alert.Bars.SymbolInfo.ReSubmitLimitNotFilledWithinMillis;
			TimerSimplifiedThreading_withOrder timerForOrder = new TimerSimplifiedThreading_withOrder(order, expiredMillis);
			timerForOrder.OnLastScheduleExpired += new EventHandler<EventArgs>(this.timerForOrder_OnLastScheduleExpired);
			timerForOrder.ScheduleOnce_postponeIfAlreadyScheduled();
			this.timeredOrders.Add(order, timerForOrder);
			replacementScheduled = true;

			return replacementScheduled;
		}

		void timerForOrder_OnLastScheduleExpired(object sender, EventArgs e) {
			string msig = " //timerForOrder_OnLastScheduleExpired(" + sender + ")";

			TimerSimplifiedThreading_withOrder timerForOrder = sender as TimerSimplifiedThreading_withOrder;
			if (timerForOrder == null) {
				string msg = "SENDER_MUST_BE_TimerSimplifiedThreading_withOrder";
				Assembler.PopupException(msg + msig);
				return;
			}
			Order orderExpired = timerForOrder.Order;
			if (orderExpired == null) {
				string msg = "MUST_BE_NON_NULL TimerSimplifiedThreading_withOrder.Order";
				Assembler.PopupException(msg + msig);
				return;
			}

			bool orderCanBeReplaced = this.orderCanBeReplaced_disposeRemoveIfFilled(orderExpired, msig);
			if (orderCanBeReplaced == false) return;

			if (this.timeredOrders.ContainsKey(orderExpired) == false) {
				string msg = "MUST_BE_ALREADY_IN_THE_DICTIONARY";
				Assembler.PopupException(msg + msig);
				return;
			}

			string reasonCanNotBeReplaced = "REMOVING_EXPIRED_TIMER after[" + timerForOrder.DelayMillis + "]ms REPLACING_ORDER_WITH_NEXT_SLIPPAGE";
			this.OrderProcessor.AppendMessage_propagateToGui(orderExpired, reasonCanNotBeReplaced);
			//NO this.timeredOrders[orderExpired].Dispose();
			this.timeredOrders.Remove(orderExpired);

			base.ReplaceOrder_withNextSlippage(orderExpired);
		}
	}
}