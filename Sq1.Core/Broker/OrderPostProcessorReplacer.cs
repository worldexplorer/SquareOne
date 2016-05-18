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
		public		ManualResetEvent	replacementComplete		{ get; private set; }

		protected OrderPostProcessorReplacer(OrderProcessor orderProcessor_passed) {
			this.OrderProcessor			= orderProcessor_passed;
			this.replacementComplete	= new ManualResetEvent(true);
		}

		StreamingAdapter getStreamingAdapter_fromOrder_nullUnsafe(Order orderExpired_willBeReplaced, string msig_invoker) {
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

		protected void ReplaceOrder_withNextSlippage(Order orderExpired_willBeReplaced) {
			string msig = " //ReplaceOrder_withNextSlippage(" + orderExpired_willBeReplaced + ")";

			this.replacementComplete.WaitOne(-1);

			StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(orderExpired_willBeReplaced, msig);
			if (streaming == null) {
				this.replacementComplete.Set();
				return; // already reported into the Order and ExceptionsForm
			}

			if (orderExpired_willBeReplaced.State != OrderState.WaitingBrokerFill) {
				string msg = "MUST_NEVER_HAPPEN__TOO_LATE_TO_KILL__DESPITE_WASNT_FILLED_UPSTACK";
				return;
			}

			try {
				this.replacementComplete.Reset();

				string hookReason = "APPLYING_NEXT_SLIPPAGE__WILL_REPLACE_KILLED_ORDER__ORDER_WAS_NOT_FILLED_WITHIN[moveHookReasonToParameter]ms";
				int paused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Pause_forAllSymbol_duringLivesimmingOne(hookReason);

				OrderPostProcessorStateHook hook_orderOriginal_killed = new OrderPostProcessorStateHook(hookReason,
					orderExpired_willBeReplaced, OrderState.VictimKilled, this.replaceOrder_withNextSlippage_onOriginalWasKilled);
				this.OrderProcessor.OPPstatusCallbacks.AddStateChangedHook(hook_orderOriginal_killed);

				int expiredMillis = orderExpired_willBeReplaced.Alert.Bars.SymbolInfo.ApplyNextSlippage_ifLimitNotFilledWithin;
				string verdict = "REPLACING_LIMIT_ORDER__WASNT_FILLED_DURING[" + expiredMillis + "]ms <= SymbolInfo[" + orderExpired_willBeReplaced.Alert.Symbol + "].ApplyNextSlippage_ifLimitNotFilledWithin";
				OrderStateMessage osm = new OrderStateMessage(orderExpired_willBeReplaced, OrderState.KillingUnfilledExpired, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				this.OrderProcessor.Emit_killOrderPending_usingKiller(orderExpired_willBeReplaced, msig);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
				this.replacementComplete.Set();
			}
		}

		void replaceOrder_withNextSlippage_onOriginalWasKilled(Order expiredOrderKilled_replaceMe, ReporterPokeUnit pokeUnit_nullHere) {
			string msig = " //replaceOrder_withNextSlippage_onOriginalWasKilled(" + expiredOrderKilled_replaceMe + ")";

			try {
				Order replacement = this.CreateReplacementOrder_insteadOfReplaceExpired(expiredOrderKilled_replaceMe);
				if (replacement == null) {
					string msg = "got NULL from CreateReplacementOrder()"
						+ "; broker reported twice about rejection, ignored this second callback";
					Assembler.PopupException(msg + msig);
					//orderToReplace.addMessage(new OrderMessage(msg));
					this.replacementComplete.Set();
					return;
				}

				double priceStreaming = replacement.Alert.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot
					.GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteLast(
					replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, true);

				if (replacement.Alert.PositionAffected != null) {	// alert.PositionAffected = null when order created by chart-click-mni
					if (replacement.Alert.IsEntryAlert) {
						replacement.Alert.PositionAffected.EntryEmitted_price = priceStreaming;
					} else {
						replacement.Alert.PositionAffected.ExitEmitted_price = priceStreaming;
					}
				}

				replacement.SlippageAppliedIndex++;
				string msg_replacement = "This is a replacement for order["
					+ replacement.ReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageAppliedIndex + "]";
				if (replacement.SlippagesLeftAvailable_noMore) {
					msg_replacement += " THIS_IS_THE_LAST_POSSIBLE_SLIPPAGE";
				}
				this.OrderProcessor.AppendMessage_propagateToGui(replacement, msg_replacement);

				//double slippage = replacement.Alert.Bars.SymbolInfo.GetSlippage_signAware_forLimitOrdersOnly(
				//	priceScript, replacement.Alert.Direction, replacement.Alert.MarketOrderAs, replacement.SlippageAppliedIndex);
				double slippage = replacement.Alert.GetSlippage_signAware_forLimitAlertsOnly(priceStreaming, replacement.SlippageAppliedIndex);
				replacement.SlippageApplied = slippage;
				replacement.PriceRequested = priceStreaming + slippage;

				string verdict = "EMITTING_REPLACEMENT_ORDER " + replacement;
				OrderStateMessage osm = new OrderStateMessage(expiredOrderKilled_replaceMe, OrderState.EmittingReplacement, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				this.SubmitReplacementOrder_insteadOfReplaceExpired(replacement);

			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
				this.replacementComplete.Set();
			}
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
			this.OrderProcessor.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){orderExpired_toReplace});
			//this.orderProcessor.RaiseOrderReplacementOrKillerCreatedForVictim(this, ReplaceExpiredOrderToReplace);
			return replacementOrder;
		}

		Order findReplacementOrder_forReplaceExpiredOrder(Order orderReplaceExpired) {
			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			
			Order ReplaceExpired = this.OrderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(orderReplaceExpired.GUID, out suggestedLane, out suggestion, true);
			if (ReplaceExpired == null) {
				throw new Exception("OrderReplaceExpired[" + orderReplaceExpired + "] wasn't found!!! suggestion[" + suggestion + "]");
			}
			if (string.IsNullOrEmpty(ReplaceExpired.ReplacedByGUID)) return null;
			Order replacement = this.OrderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(ReplaceExpired.ReplacedByGUID, out suggestedLane, out suggestion, true);
			return replacement;
		}
		protected void SubmitReplacementOrder_insteadOfReplaceExpired(Order replacementOrder) {
			string msig = " //SubmitReplacementOrder_insteadOfReplaceExpired()";
			if (replacementOrder == null) {
				Assembler.PopupException("replacementOrder == null why did you call me?");
				this.replacementComplete.Set();
				return;
			}

			//if (replacementOrder.hasBrokerAdapter(msig) == false) return;

			string msg = "Scheduling SubmitOrdersThreadEntry [" + replacementOrder.ToString() + "] slippageIndex["
				+ replacementOrder.SlippageAppliedIndex + "] through [" + replacementOrder.Alert.DataSource_fromBars.BrokerAdapter + "]";
			OrderStateMessage newOrderState = new OrderStateMessage(replacementOrder, OrderState.PreSubmit, msg);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);

			List<Order> replacementOrder_oneInTheList = new List<Order>() { replacementOrder };
			BrokerAdapter broker = replacementOrder.Alert.DataSource_fromBars.BrokerAdapter;

			StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
			if (streaming == null) {
				this.replacementComplete.Set();
				return; // already reported into the Order and ExceptionsForm
			}

			string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
			OrderPostProcessorStateHook hook_replacementOrder_emitted = new OrderPostProcessorStateHook(hookReason,
				replacementOrder, OrderState.WaitingBrokerFill, this.UnpauseAll_signalReplacementComplete);
			this.OrderProcessor.OPPstatusCallbacks.AddStateChangedHook(hook_replacementOrder_emitted);

			this.OrderProcessor.SubmitToBrokerAdapter_inNewThread(replacementOrder_oneInTheList, broker);
		}

		protected void UnpauseAll_signalReplacementComplete(Order replacementOrder, ReporterPokeUnit pokeUnit_ignored) {
			string msig = " //UnpauseAll_signalReplacementComplete()";
			try {
				StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
				string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
				int unpaused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Unpause_forAllSymbol_afterLivesimmingOne(hookReason);
			} finally {
				this.replacementComplete.Set();
			}
		}
		protected void AddMessage_noMoreSlippagesAvailable(Order order) {
			int slippageIndexMax = order.Alert.Slippage_maxIndex_forLimitOrdersOnly;
			string msg2 = "Reached max slippages available for [" + order.Alert.Bars.Symbol + "]"
				+ " order.SlippageIndex[" + order.SlippageAppliedIndex + "] > slippageIndexMax[" + slippageIndexMax + "]"
				+ "; Order will have slippageIndexMax[" + slippageIndexMax + "]"; 
			Assembler.PopupException(msg2, null, false);

			OrderStateMessage newOrderStateReplaceExpired = new OrderStateMessage(order, OrderState.RejectedLimitReached, msg2);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderStateReplaceExpired);
		}
	}
}