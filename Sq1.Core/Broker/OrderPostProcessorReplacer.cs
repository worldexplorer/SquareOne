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

			if (orderExpired_willBeReplaced.ReplacedByGUID != "") return;

			//bool previousReplacementFinished = orderExpired_willBeReplaced.OrderReplacement_Emitted_afterOriginalKilled__orError.WaitOne(0);
			//if (previousReplacementFinished) {
			//    string msg = "previousReplacementFinished[" + previousReplacementFinished + "]";
			//    Assembler.PopupException(msg + msig);
			//    throw new Exception(msg + msig);
			//}

			StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(orderExpired_willBeReplaced, msig);
			if (streaming == null) {
				return; // already reported into the Order and ExceptionsForm
			}

			switch (orderExpired_willBeReplaced.State) {
				case OrderState.WaitingBrokerFill:
					string msg1 = "THIS_IS_THE_ONLY_CASE_WE_REPLACE";
					break;

				case OrderState.Filled:
				case OrderState.FilledPartially:
					string msg2 = "ORDER_FILLED_WHILE_I_WAS_PREPARING_TO_REPLACE_IT";
					this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg2);
					return;

				default:
					string msg3 = "MUST_NEVER_HAPPEN__TOO_LATE_TO_KILL__DESPITE_WASNT_FILLED_UPSTACK";
					this.OrderProcessor.AppendMessage_propagateToGui(orderExpired_willBeReplaced, msg3);
					return;
			}

			try {
				int expiredMillis				= orderExpired_willBeReplaced.Alert.Bars.SymbolInfo.ApplyNextSlippage_ifLimitNotFilledWithin;
				double slippageNext_NaNunsafe	= orderExpired_willBeReplaced.SlippageNextAvailable_NanWhenNoMore;

				string hookReason = "APPLYING_NEXT_SLIPPAGE__WILL_REPLACE_KILLED_ORDER__ORDER_WAS_NOT_FILLED_WITHIN[" + expiredMillis + "]ms slippageNext_NaNunsafe[" + slippageNext_NaNunsafe + "]";
				// CHART_IS_PAUSED_TOO__USE_POSITIONS_PENDING_THEY_STAY_STABLE_DURING_REPLACEMENT int paused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Pause_forAllSymbol_duringLivesimmingOne(hookReason);

				orderExpired_willBeReplaced.DontRemoveMyPendingAfterImKilled_IwillBeReplaced = true;

				OrderPostProcessorStateHook hook_orderOriginal_killed = new OrderPostProcessorStateHook(hookReason,
					orderExpired_willBeReplaced, OrderState.VictimKilled, this.replaceOrder_withNextSlippage_onOriginalWasKilled);
				this.OrderProcessor.OPPstatusCallbacks.AddStateChangedHook(hook_orderOriginal_killed);

				string verdict = "REPLACING_LIMIT_ORDER__WASNT_FILLED_DURING[" + expiredMillis + "]ms <= SymbolInfo[" + orderExpired_willBeReplaced.Alert.Symbol + "].ApplyNextSlippage_ifLimitNotFilledWithin";
				OrderStateMessage osm = new OrderStateMessage(orderExpired_willBeReplaced, OrderState.KillingUnfilledExpired, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				this.OrderProcessor.Emit_killOrderPending_usingKiller(orderExpired_willBeReplaced, msig);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}

		void replaceOrder_withNextSlippage_onOriginalWasKilled(Order expiredOrderKilled_replaceMe, ReporterPokeUnit pokeUnit_nullHere) {
			string msig = " //replaceOrder_withNextSlippage_onOriginalWasKilled(" + expiredOrderKilled_replaceMe + ")";


			try {
				string reasonCanNotBeReplaced = null;
				if (expiredOrderKilled_replaceMe.HasSlippagesDefined == false) {
					reasonCanNotBeReplaced = "expiredOrderKilled_replaceMe.HasSlippagesDefined = FALSE";
				}
				if (expiredOrderKilled_replaceMe.SlippagesLeftAvailable_noMore) {
					reasonCanNotBeReplaced = "NoMoreSlippagesAvailable "
						+ " expiredOrderKilled_replaceMe.SlippageAppliedIndex[" + expiredOrderKilled_replaceMe.SlippageAppliedIndex + "]=[" + expiredOrderKilled_replaceMe.SlippageApplied + "]"
						+ " expiredOrderKilled_replaceMe.SlippagesLeftAvailable[" + expiredOrderKilled_replaceMe.SlippagesLeftAvailable + "]";
				}
				if (string.IsNullOrEmpty(reasonCanNotBeReplaced) == false) {
					Assembler.PopupException(reasonCanNotBeReplaced, null, false);
					this.AddMessage_noMoreSlippagesAvailable(expiredOrderKilled_replaceMe);
					return;
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
				return;
			}

			try {
				Order replacement = this.CreateReplacementOrder_insteadOfReplaceExpired(expiredOrderKilled_replaceMe);
				if (replacement == null) {
					string msg = "got NULL from CreateReplacementOrder()"
						+ "; broker reported twice about rejection, ignored this second callback";
					Assembler.PopupException(msg + msig);
					//orderToReplace.addMessage(new OrderMessage(msg));
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
				double slippageNext_NanUnsafe = replacement.Alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(replacement.SlippageAppliedIndex);
				if (double.IsNaN(slippageNext_NanUnsafe)) {
					string msg = "IRREPAIRABLE__YOU_SHOULD_JAVE_NOT_CREATED_REPLACEMENT_ORDER__SEE_reasonCanNotBeReplaced_20_LINES_ABOVE";
					Assembler.PopupException(msg);
				}


				replacement.SlippageApplied = slippageNext_NanUnsafe;
				double priceBasedOnLastQuote = priceStreaming + slippageNext_NanUnsafe;
				double difference_withExpiredOrder_signInprecise = expiredOrderKilled_replaceMe.PriceEmitted - priceBasedOnLastQuote;
				replacement.PriceEmitted = priceBasedOnLastQuote;
				replacement.Alert.SetNewPriceEmitted_fromReplacementOrder(replacement);	// will repaint the circle at the new order-emitted price PanelPrice.Rendering.cs:86

				string verdict = "EMITTING_REPLACEMENT_ORDER diff[" + difference_withExpiredOrder_signInprecise + "] " + replacement;
				OrderStateMessage osm = new OrderStateMessage(expiredOrderKilled_replaceMe, OrderState.EmittingReplacement, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				bool inNewThread = false;
				int orderSubmitted = this.SubmitReplacementOrder_insteadOfReplaceExpired(replacement, inNewThread);

				replacement.Alert.Strategy.Script.Executor.CallbackOrderReplaced_invokeScript_nonReenterably(expiredOrderKilled_replaceMe, replacement, orderSubmitted);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			} finally {
				expiredOrderKilled_replaceMe.OrderReplacement_Emitted_afterOriginalKilled__orError.Set();
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
		protected int SubmitReplacementOrder_insteadOfReplaceExpired(Order replacementOrder, bool inNewThread = true) {
			int orderSubmitted = 0;

			string msig = " //SubmitReplacementOrder_insteadOfReplaceExpired()";
			try {
				if (replacementOrder == null) {
					Assembler.PopupException("replacementOrder == null why did you call me?");
					return orderSubmitted;
				}

				string msg = "Scheduling SubmitOrdersThreadEntry [" + replacementOrder.ToString() + "] slippageIndex["
					+ replacementOrder.SlippageAppliedIndex + "] through [" + replacementOrder.Alert.DataSource_fromBars.BrokerAdapter + "]";
				OrderStateMessage newOrderState = new OrderStateMessage(replacementOrder, OrderState.PreSubmit, msg);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);

				List<Order> replacementOrder_oneInTheList = new List<Order>() { replacementOrder };
				BrokerAdapter broker = replacementOrder.Alert.DataSource_fromBars.BrokerAdapter;

				StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
				if (streaming == null) {
					return orderSubmitted; // already reported into the Order and ExceptionsForm
				}

				string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
				OrderPostProcessorStateHook hook_replacementOrder_emitted = new OrderPostProcessorStateHook(hookReason,
					replacementOrder, OrderState.WaitingBrokerFill, this.UnpauseAll_signalReplacementComplete);
				// UNPAUSE_DISABLED_ANYWAY__KOZ_CHART_NEEDS_TO_DISPLAY_QUOTES_WHILE_BROKER_EXECUTES__I_SHOULD_FACE_THE_PROBLEM_FIRST this.OrderProcessor.OPPstatusCallbacks.AddStateChangedHook(hook_replacementOrder_emitted);

				orderSubmitted = this.OrderProcessor.SubmitToBroker_waitForConnected(replacementOrder_oneInTheList, broker, inNewThread);
				return orderSubmitted;
			} finally {
				// TOO_EARLY this.replacementOrderEmitted_afterOriginalKilled__orError_multiOrderUnsupported.Set();
			}
		}

		protected void UnpauseAll_signalReplacementComplete(Order replacementOrder, ReporterPokeUnit pokeUnit_ignored) {
			string msig = " //UnpauseAll_signalReplacementComplete()";
			//try {
				StreamingAdapter streaming = this.getStreamingAdapter_fromOrder_nullUnsafe(replacementOrder, msig);
				string hookReason = "UNBLOCKING_SCRIPT_TO_TAKE_CONTROL__AFTER_REPLACEMENT_ORDER_EXPECTS_FILL";
				// CHART_IS_PAUSED_TOO__USE_POSITIONS_PENDING_THEY_STAY_STABLE_DURING_REPLACEMENT int unpaused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Unpause_forAllSymbol_afterLivesimmingOne(hookReason);
			//} finally {
			//    replacementOrder.ReplacementForGUID;
			//    orderExpiredKilled..Set();
			//}
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