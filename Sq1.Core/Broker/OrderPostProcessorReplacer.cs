using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;
using Sq1.Core.Streaming;

namespace Sq1.Core.Broker {
	public abstract class OrderPostProcessorReplacer {
		protected	OrderProcessor		OrderProcessor			{ get; private set; }

		protected OrderPostProcessorReplacer(OrderProcessor orderProcessor_passed) {
			this.OrderProcessor			= orderProcessor_passed;
		}

		protected StreamingAdapter GetStreamingAdapter_fromOrder_nullUnsafe(Order orderExpired_willBeReplaced, string msig_invoker) {
			StreamingAdapter ret = null;

			if (orderExpired_willBeReplaced.Alert == null) {
				string msg = "ALERT_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderExpired_willBeReplaced.Alert.Bars == null) {
				string msg = "BARS_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderExpired_willBeReplaced.Alert.DataSource_fromBars == null) {
				string msg = "DATASOURCE_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderExpired_willBeReplaced.Alert.DataSource_fromBars.StreamingAdapter == null) {
				string msg = "STREAMING_ADAPTER_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}

			ret = orderExpired_willBeReplaced.Alert.DataSource_fromBars.StreamingAdapter;
			return ret;
		}

		protected Order CreateReplacementOrder_insteadOfReplaceExpired(Order orderExpired_toReplace) {
			if (orderExpired_toReplace == null) {
				Assembler.PopupException("order2replace=null why did you call me?");
				return null;
			}
			Order replacement = this.findReplacementOrder_forReplaceExpiredOrder(orderExpired_toReplace);
			if (replacement != null) {
				string msg = "ReplaceExpired[" + orderExpired_toReplace + "] already has a replacement[" + replacement + "] with State[" + replacement.State + "]; ignored rejection duplicates from broker";
				this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_toReplace, msg);
				return null;
			}

			Order replacementOrder = orderExpired_toReplace.DeriveReplacementOrder();
			this.OrderProcessor.DataSnapshot.OrderInsert_notifyGuiAsync(replacementOrder);
			this.OrderProcessor.RaiseOnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(this, new List<Order>(){orderExpired_toReplace});
			//this.orderProcessor.RaiseOrderReplacementOrKillerCreatedForVictim(this, ReplaceExpiredOrderToReplace);
			return replacementOrder;
		}

		Order findReplacementOrder_forReplaceExpiredOrder(Order orderReplaceExpired) {
			OrderLane	suggestedLane_nullUnsafe = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			
			Order ReplaceExpired = this.OrderProcessor.DataSnapshot.OrdersAll.ScanRecent_forOrderGuid(orderReplaceExpired.GUID, out suggestedLane_nullUnsafe, out suggestion, true);
			if (ReplaceExpired == null) {
				throw new Exception("OrderReplaceExpired[" + orderReplaceExpired + "] wasn't found!!! suggestion[" + suggestion + "]");
			}
			if (string.IsNullOrEmpty(ReplaceExpired.ReplacedByGUID)) return null;
			Order replacement = this.OrderProcessor.DataSnapshot.OrdersAll.ScanRecent_forOrderGuid(ReplaceExpired.ReplacedByGUID, out suggestedLane_nullUnsafe, out suggestion, true);
			return replacement;
		}
		protected bool SubmitReplacementOrder_insteadOfReplaceExpired(Order replacementOrder, bool inNewThread = true) {
			bool submittedOrScheduled = false;

			string msig = " //SubmitReplacementOrder_insteadOfReplaceExpired()";
			try {
				if (replacementOrder == null) {
					Assembler.PopupException("replacementOrder == null why did you call me?");
					return submittedOrScheduled;
				}

				string msg = "Scheduling SubmitOrdersThreadEntry [" + replacementOrder.ToString() + "] slippageIndex["
					+ replacementOrder.SlippageAppliedIndex + "] through [" + replacementOrder.Alert.DataSource_fromBars.BrokerAdapter + "]";
				OrderStateMessage newOrderState = new OrderStateMessage(replacementOrder, OrderState.PreSubmit, msg);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);

				List<Order> replacementOrder_oneInTheList = new List<Order>() { replacementOrder };
				BrokerAdapter broker = replacementOrder.Alert.DataSource_fromBars.BrokerAdapter;

				StreamingAdapter streaming = this.GetStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
				if (streaming == null) {
					return submittedOrScheduled; // already reported into the Order and ExceptionsForm
				}

				string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
				OrderPostProcessorStateHook hook_replacementOrder_emitted = new OrderPostProcessorStateHook(hookReason,
					replacementOrder, OrderState.WaitingBrokerFill, this.UnpauseAll_signalReplacementComplete);
				// UNPAUSE_DISABLED_ANYWAY__KOZ_CHART_NEEDS_TO_DISPLAY_QUOTES_WHILE_BROKER_EXECUTES__I_SHOULD_FACE_THE_PROBLEM_FIRST this.OrderProcessor.OPPstatusCallbacks.AddStateChangedHook(hook_replacementOrder_emitted);

				int ordersSubmittedOrScheduled = this.OrderProcessor.SubmitToBroker_inNewThread_waitUntilConnected(
																	replacementOrder_oneInTheList, broker, inNewThread);
				submittedOrScheduled = ordersSubmittedOrScheduled == 1;
			} finally {
				// TOO_EARLY this.replacementOrderEmitted_afterOriginalKilled__orError_multiOrderUnsupported.Set();
			}
			return submittedOrScheduled;
		}

		protected void UnpauseAll_signalReplacementComplete(Order replacementOrder, ReporterPokeUnit pokeUnit_ignored) {
			string msig = " //UnpauseAll_signalReplacementComplete()";
			//try {
				StreamingAdapter streaming = this.GetStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
				string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
				// CHART_IS_PAUSED_TOO__USE_POSITIONS_PENDING_THEY_STAY_STABLE_DURING_REPLACEMENT int unpaused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Unpause_forAllSymbol_afterLivesimmingOne(hookReason);
			//} finally {
			//    replacementOrder.ReplacementForGUID;
			//    orderExpiredKilled..Set();
			//}
		}
		protected void AddMessage_noMoreSlippagesAvailable(Order order, string prefix = "") {
			int slippageIndexMax = order.Alert.Slippage_maxIndex_forLimitOrdersOnly;
			string msg2 = "Reached max slippages available for [" + order.Alert.Bars.Symbol + "]"
				+ " order.SlippageIndex[" + order.SlippageAppliedIndex + "] > slippageIndexMax[" + slippageIndexMax + "]"
				+ "; Order will have slippageIndexMax[" + slippageIndexMax + "]";
			Assembler.PopupException(prefix + msg2, null, false);

			OrderStateMessage newOrderStateReplaceExpired = new OrderStateMessage(order, OrderState.LimitExpiredRejected, msg2);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderStateReplaceExpired);
		}
	}
}