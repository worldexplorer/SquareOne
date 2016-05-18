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

			int expiredMillis = order.Alert.Bars.SymbolInfo.ApplyNextSlippage_ifLimitNotFilledWithin;
			if (expiredMillis == 0) {
				string msg = "Symbol[" + order.Alert.Bars.Symbol + "].ApplyNextSlippageIfLimitNotFilledWithin=[" + expiredMillis +  "]; returning";
				Assembler.PopupException(msg + msig_invoker, null, false);
				return ret;
			}

			string reasonCanNotBeReplaced = "";
			if (order.QtyFill > 0 || order.PriceFilled > 0) {
				reasonCanNotBeReplaced += "ORDER_WAS_FILLED ";
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
				base.AddMessage_noMoreSlippagesAvailable(order);
			}

			if (string.IsNullOrEmpty(reasonCanNotBeReplaced)) {
				ret = true;		// you CAN replace the order
				return ret;
			}

			this.removeTimer_forOrder(order, reasonCanNotBeReplaced, msig_invoker);
			return ret;
		}

		void removeTimer_forOrder(Order order, string reasonCanNotBeReplaced = "", string msig_invoker = "") {
			// "ORDER_CAN_NOT_BE_REPLACED"
			if (this.timeredOrders.ContainsKey(order)) {
				reasonCanNotBeReplaced = "CANCELLING_TIMER " + reasonCanNotBeReplaced;
				this.OrderProcessor.AppendMessage_propagateToGui(order, reasonCanNotBeReplaced);

				// timer exists => now I have to stop & dispose the timer
				this.timeredOrders[order].Dispose();
				this.timeredOrders.Remove(order);
			} else {
				reasonCanNotBeReplaced = "NO_TIMER_TO_CANCEL " + reasonCanNotBeReplaced;
				this.OrderProcessor.AppendMessage_propagateToGui(order, reasonCanNotBeReplaced);

				string msg2 = "LAST_SLIPPAGE_BASED_ORDER__HAS_NO_TIMER_TO_DELETE";
				Assembler.PopupException(msg2 + msig_invoker + reasonCanNotBeReplaced, null, false);
			}

			bool mostLikelyUnpaused = base.replacementComplete.WaitOne(0);
			if (mostLikelyUnpaused) return;
			base.UnpauseAll_signalReplacementComplete(order, null);		// otherwize after last slippage => rejected => stays paused => backlog grew[10]
		}
		public bool ScheduleReplace_ifExpired(Order order) {
			string msig = " //ScheduleReplace_ifExpired(" + order + ")";
			bool replacementScheduled = false;

			bool orderEligible_forReplacement = this.orderCanBeReplaced_disposeRemoveIfFilled(order, msig);
			if (orderEligible_forReplacement == false) return replacementScheduled;

			int expiredMillis = order.Alert.Bars.SymbolInfo.ApplyNextSlippage_ifLimitNotFilledWithin;
			TimerSimplifiedThreading_withOrder timerForOrder = new TimerSimplifiedThreading_withOrder(order, expiredMillis);
			timerForOrder.OnLastScheduleExpired += new EventHandler<EventArgs>(timerForOrder_OnLastScheduleExpired);
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
			base.ReplaceOrder_withNextSlippage(orderExpired);
		}
	}
}