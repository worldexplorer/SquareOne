using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacer_Expired_WithoutFill : OrderPostProcessorReplacer {
		Dictionary<Order, TimerSimplifiedThreading_withOrder> timeredOrders;

		public OrderPostProcessorReplacer_Expired_WithoutFill(OrderProcessor orderProcessor) : base(orderProcessor) {
			timeredOrders = new Dictionary<Order, TimerSimplifiedThreading_withOrder>();
		}

		bool orderCanBeReplaced_disposeRemoveIfFilled(Order order, string msig_invoker) {
			bool ret = false;

			bool shallReplace = order.Alert.Bars.SymbolInfo.LimitExpiredResubmit;
			if (shallReplace == false) {
				string msg = "Symbol[" + order.Alert.Bars.Symbol + "].ReSubmitLimitNotFilled=[" + shallReplace +  "]; returning";
				//TOO_VERBOSE Assembler.PopupException(msg + msig_invoker, null, false);
				return ret;
			}

			int expiredMillis = order.Alert.Bars.SymbolInfo.LimitExpiresAfterMillis;
			if (expiredMillis <= 0) {
				string msg = "Symbol[" + order.Alert.Bars.Symbol + "].ReSubmitLimitNotFilledWithinMillis=[" + expiredMillis +  "]; returning";
				Assembler.PopupException(msg + msig_invoker, null, false);
				return ret;
			}

			string reasonCanNotBeReplaced = "";
			if (order.QtyFill > Order.INITIAL_QtyFill || order.PriceFilled > Order.INITIAL_PriceFill) {
				reasonCanNotBeReplaced += "order.QtyFill[" + order.QtyFill + "] > [" + Order.INITIAL_QtyFill + "] || order.PriceFilled[" + order.PriceFilled + "] > [" + Order.INITIAL_PriceFill + "] ";
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

			this.OrderProcessor.AppendMessage_propagateToGui(order, "LIMIT_EXPIRED_REPLACEMENT__NO_NEED " + reasonCanNotBeReplaced);
			this.removeTimer_forOrder(order, msig_invoker);
			return ret;
		}

		public void AllTimers_stopDispose_LivesimEnded(List<Alert> alertsPending, string scriptStopped) {
			string msig = " //AllTimers_stopDispose_LivesimEnded(" + scriptStopped + ")";
			foreach (Alert pending in alertsPending) {
				if (pending.OrderFollowed_orCurrentReplacement == null) continue;
				bool pumpMustAlreadyStopped_atLivesimEndedOrAborted = false;
				this.removeTimer_forOrder(pending.OrderFollowed_orCurrentReplacement, msig, pumpMustAlreadyStopped_atLivesimEndedOrAborted);
			}
		}

		void removeTimer_forOrder(Order order, string msig_invoker = "", bool waitForReplacementComplete_andUnpauseAll = true) {
			// "ORDER_CAN_NOT_BE_REPLACED"
			if (this.timeredOrders.ContainsKey(order)) {
				string reasonCanNotBeReplaced = "LIMIT_EXPIRED_REPLACEMENT__CANCELLING_TIMER ";

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
				//Assembler.PopupException(msg2 + msig_invoker + reasonCanNotBeReplaced, null, false);
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

			int expiredMillis = order.Alert.Bars.SymbolInfo.LimitExpiresAfterMillis;
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

			this.Replace_LimitOrder_withNextSlippage_hookOnVictimKilled(orderExpired);
		}

		public bool LimitExpired_replaceWith_nextSlippage(Order orderLimitExpired_toReplace) {
			string msig = " //OrderPostProcessorReplacerExpired.LimitExpired_replaceWith_nextSlippage(" + orderLimitExpired_toReplace + ")";
			bool emitted = false;
			string symbolClass = orderLimitExpired_toReplace.Alert.Symbol + "/" + orderLimitExpired_toReplace.Alert.SymbolClass;

			if (orderLimitExpired_toReplace.State != OrderState.LimitExpired) {
				string msg = "WONT_REPLACE_LIMIT_EXPIRED [" + orderLimitExpired_toReplace.State + "] MUST_BE_IN_STATE[LimitExpired] ; continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(orderLimitExpired_toReplace, msg + msig);
				//Assembler.PopupException(msg);
				return emitted;
			}
			if (orderLimitExpired_toReplace.SlippagesLeftAvailable_noMore) {
				base.AddMessage_noMoreSlippagesAvailable(orderLimitExpired_toReplace);
				bool shouldKillLastExpired = orderLimitExpired_toReplace.Alert.Bars.SymbolInfo.LimitExpired_KillUnfilledWithLastSlippage;
				string killReason = ": SymbolInfo[" + symbolClass + "].LimitExpired_KillUnfilledWithLastSlippage[" + shouldKillLastExpired + "]";
				if (shouldKillLastExpired) {
					string killAction = "KILLING_LAST_HOPE";
					base.OrderProcessor.AppendMessage_propagateToGui(orderLimitExpired_toReplace, killAction + killReason);
					string msg = "KILLING_UNFILLED__LIMIT_EXPIRED_WITH_LAST_SLIPPAGE";
					emitted = this.OrderProcessor.Emit_killOrderPending_usingKiller_hookNeededAfterwards(orderLimitExpired_toReplace, msg + msig);
				} else {
					string killAction = "NOT_KILLING_LAST_HOPE(DANGEROUS_UNFILL_FOREVER)";
					base.OrderProcessor.AppendMessage_propagateToGui(orderLimitExpired_toReplace, killAction + killReason);
				}
				return emitted;
			}

			emitted = this.Replace_LimitOrder_withNextSlippage_hookOnVictimKilled(orderLimitExpired_toReplace);
			return emitted;
		}
	
		protected bool Replace_LimitOrder_withNextSlippage_hookOnVictimKilled(Order limitOrderExpired_willBeReplaced) {
			bool emitted = false;
			string msig = " //Replace_LimitOrder_withNextSlippage_hookOnVictimKilled(" + limitOrderExpired_willBeReplaced + ")";

			if (limitOrderExpired_willBeReplaced.ReplacedByGUID != "") return emitted;

			//bool previousReplacementFinished = orderExpired_willBeReplaced.OrderReplacement_Emitted_afterOriginalKilled__orError.WaitOne(0);
			//if (previousReplacementFinished) {
			//    string msg = "previousReplacementFinished[" + previousReplacementFinished + "]";
			//    Assembler.PopupException(msg + msig);
			//    throw new Exception(msg + msig);
			//}

			StreamingAdapter streaming = base.GetStreamingAdapter_fromOrder_nullUnsafe(limitOrderExpired_willBeReplaced, msig);
			if (streaming == null) {
				return emitted; // already reported into the Order and ExceptionsForm
			}

			switch (limitOrderExpired_willBeReplaced.State) {
				case OrderState.WaitingBrokerFill:
					string msg1 = "THIS_IS_THE_ONLY_CASE_WE_REPLACE";
					break;

				case OrderState.Rejected:
					string msg4 = "ANOTHER_BRANCH_TO_GET_EMERGENCY_SETTINGS";
					break;

				case OrderState.Filled:
				case OrderState.FilledPartially:
					string msg2 = "ORDER_FILLED_WHILE_I_WAS_PREPARING_TO_REPLACE_IT";
					this.OrderProcessor.AppendMessage_propagateToGui(limitOrderExpired_willBeReplaced, msg2);
					return emitted;

				default:
					string msg3 = "MUST_NEVER_HAPPEN__TOO_LATE_TO_KILL__DESPITE_WASNT_FILLED_UPSTACK [" + limitOrderExpired_willBeReplaced.State + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(limitOrderExpired_willBeReplaced, msg3 + msig);
					return emitted;
			}

			try {
				bool shallReplace				= limitOrderExpired_willBeReplaced.Alert.Bars.SymbolInfo.LimitExpiredResubmit;
				int expiredMillis				= limitOrderExpired_willBeReplaced.Alert.Bars.SymbolInfo.LimitExpiresAfterMillis;

				if (shallReplace == false || expiredMillis <= 0) {
					string msg = "MUST_NEVER_HAPPEN (shallReplace==false || expiredMillis<=0)";
					Assembler.PopupException(msg + msig);
					return emitted;
				}


				double slippageNext_NaNunsafe	= limitOrderExpired_willBeReplaced.SlippageNextAvailable_forLimitAlertsOnly_NanWhenNoMore;

				string hookReason = "APPLYING_NEXT_SLIPPAGE__WILL_REPLACE_KILLED_ORDER__ORDER_WAS_NOT_FILLED_WITHIN[" + expiredMillis + "]ms slippageNext_NaNunsafe[" + slippageNext_NaNunsafe + "]";
				// CHART_IS_PAUSED_TOO__USE_POSITIONS_PENDING_THEY_STAY_STABLE_DURING_REPLACEMENT int paused = streaming.DistributorCharts_substitutedDuringLivesim.TwoPushingPumpsPerSymbol_Pause_forAllSymbol_duringLivesimmingOne(hookReason);

				limitOrderExpired_willBeReplaced.DontRemoveMyPending_afterImKilled_IwillBeReplaced = true;

				OrderPostProcessorStateHook hook_orderOriginal_killed = new OrderPostProcessorStateHook(hookReason,
					limitOrderExpired_willBeReplaced, OrderState.VictimKilled, this.replaceOrder_withNextSlippage_onOriginalWasKilled_hookEntry);
				this.OrderProcessor.OPPstatusCallbacks.HookRegister(hook_orderOriginal_killed);

				string verdict = "REPLACING_LIMIT_ORDER__WASNT_FILLED_DURING[" + expiredMillis + "]ms <= SymbolInfo[" + limitOrderExpired_willBeReplaced.Alert.Symbol + "].ApplyNextSlippage_ifLimitNotFilledWithin";
				OrderStateMessage osm = new OrderStateMessage(limitOrderExpired_willBeReplaced, OrderState.KillingUnfilledExpired, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				emitted = this.OrderProcessor.Emit_killOrderPending_usingKiller_hookNeededAfterwards(limitOrderExpired_willBeReplaced, msig);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
			return emitted;
		}

		void replaceOrder_withNextSlippage_onOriginalWasKilled_hookEntry(Order expiredOrderKilled_replaceMe, ReporterPokeUnit pokeUnit_nullHere) {
			string msig = " //replaceOrder_withNextSlippage_onOriginalWasKilled_hookEntry(" + expiredOrderKilled_replaceMe + ")";

			try {
				string reasonCanNotBeReplaced = null;
				if (expiredOrderKilled_replaceMe.HasSlippagesDefined == false) {
					reasonCanNotBeReplaced = "expiredOrderKilled_replaceMe.HasSlippagesDefined = FALSE";
				}
				if (expiredOrderKilled_replaceMe.SlippagesLeftAvailable_noMore) {
					reasonCanNotBeReplaced = "NoMoreSlippagesAvailable "
						+ " expiredOrderKilled_replaceMe.SlippageAppliedIndex["		+ expiredOrderKilled_replaceMe.SlippageAppliedIndex + "]=[" + expiredOrderKilled_replaceMe.SlippageApplied + "]"
						+ " expiredOrderKilled_replaceMe.SlippagesLeftAvailable["	+ expiredOrderKilled_replaceMe.SlippagesLeftAvailable + "]";
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
				Order replacement = this.CreateReplacementOrder_forExpired(expiredOrderKilled_replaceMe);
				if (replacement == null) {
					string msg = "got NULL from CreateReplacementOrder()"
						+ "; broker reported twice about rejection, ignored this second callback";
					Assembler.PopupException(msg + msig);
					//orderToReplace.addMessage(new OrderMessage(msg));
					return;
				}

				double priceStreaming = replacement.Alert.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot
					.GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteLast(
						replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, false);

				if (replacement.Alert.PositionAffected != null) {	// alert.PositionAffected = null when order created by chart-click-mni
					if (replacement.Alert.IsEntryAlert) {
						replacement.Alert.PositionAffected.EntryEmitted_price = priceStreaming;
					} else {
						replacement.Alert.PositionAffected.ExitEmitted_price = priceStreaming;
					}
				}

				replacement.SlippageAppliedIndex++;
				string msg_replacement = "REPLACEMENT_FOR_LIMIT_EXPIRED["
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
				replacement.Alert.SetNew_OrderFollowed_PriceEmitted_fromReplacementOrder(replacement);	// will repaint the circle at the new order-emitted price PanelPrice.Rendering.cs:86

				string verdict = "REPLACING_LIMIT_EXPIRED_HOOK diffToExpired[" + difference_withExpiredOrder_signInprecise + "] " + replacement;
				OrderStateMessage osm = new OrderStateMessage(expiredOrderKilled_replaceMe, OrderState.EmittingReplacement, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				bool inNewThread = false;
				bool scheduled = this.EmitReplacementOrder_insteadOfExpired(replacement, inNewThread);

				replacement.Alert.Strategy.Script.Executor.CallbackOrderReplaced_invokeScript_nonReenterably(
															expiredOrderKilled_replaceMe, replacement, scheduled);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			} finally {
				expiredOrderKilled_replaceMe.OrderReplacement_Emitted_afterOriginalKilled__orError.Set();
			}
		}
	}
}