using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacerRejected : OrderPostProcessorReplacer {

		public OrderPostProcessorReplacerRejected(OrderProcessor orderProcessor) : base(orderProcessor) {
		}
		public bool Replace_AnyOrderRejected_ifRejectedResubmit(Order anyOrderRejected_willBeKilled_andReplaced) {
			string msig = " //OrderPostProcessorReplacerRejected.Replace_AnyOrderRejected_ifRejectedResubmit(" + anyOrderRejected_willBeKilled_andReplaced + ")";
			bool replacementScheduled = false;

			if (anyOrderRejected_willBeKilled_andReplaced.State != OrderState.Rejected) {
				string msg = "WONT_REPLACE_REJECTED[" + anyOrderRejected_willBeKilled_andReplaced.State + "] MUST_BE_IN_STATE[Rejected]; continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(anyOrderRejected_willBeKilled_andReplaced, msg + msig);
				//Assembler.PopupException(msg);
				return replacementScheduled;
			}
			
			//TODO right now submitting only with the same slippage
			//if (orderRejected_toReplace.Alert.Bars.SymbolInfo.RejectedResubmitWithNextSlippage) {
			//    if (orderRejected_toReplace.SlippagesLeftAvailable_noMore) {
			//        base.AddMessage_noMoreSlippagesAvailable(orderRejected_toReplace, "USING_LAST_SLIPPAGE ");
			//        emitted = base.Replace_AnyOrder_withSameSlippage(orderRejected_toReplace);
			//    } else {
			//        emitted = base.Replace_LimitOrder_withNextSlippage(orderRejected_toReplace);
			//    }
			//    return emitted;
			//}

			replacementScheduled = this.Replace_AnyOrder_withSameSlippage_killWithoutHook(anyOrderRejected_willBeKilled_andReplaced);
			return replacementScheduled;
		}

		protected bool Replace_AnyOrder_withSameSlippage_killWithoutHook(Order anyOrderRejected_willBeKilled_andReplaced) {
			bool replacementScheduled = false;
			string msig = " //OrderPostProcessorReplacerRejected.Replace_AnyOrder_withSameSlippage_killWithoutHook(" + anyOrderRejected_willBeKilled_andReplaced + ")";
			string symbolClass = anyOrderRejected_willBeKilled_andReplaced.Alert.Symbol + "/" + anyOrderRejected_willBeKilled_andReplaced.Alert.SymbolClass;

			StreamingAdapter streaming = base.GetStreamingAdapter_fromOrder_nullUnsafe(anyOrderRejected_willBeKilled_andReplaced, msig);
			if (streaming == null) {
				return replacementScheduled; // already reported into the Order and ExceptionsForm
			}

						
			try {
				bool shallKill = anyOrderRejected_willBeKilled_andReplaced.Alert.Bars.SymbolInfo.RejectedKill;
				if (shallKill == false) {
					string msg = "WONT_KILL_REJECTED_KOZ_SymbolInfo[" + symbolClass + "].RejectedKill==false; continuing";
					base.OrderProcessor.AppendMessage_propagateToGui(anyOrderRejected_willBeKilled_andReplaced, msg + msig);
					//Assembler.PopupException(msg);
					//return replacementScheduled;
				} else {
					bool killerEmitted = this.OrderProcessor.Emit_killOrderPending_usingKiller_hookNeededAfterwards(anyOrderRejected_willBeKilled_andReplaced, msig);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}

	
			if (anyOrderRejected_willBeKilled_andReplaced.ReplacedByGUID != "") return replacementScheduled;

			//bool previousReplacementFinished = orderExpired_willBeReplaced.OrderReplacement_Emitted_afterOriginalKilled__orError.WaitOne(0);
			//if (previousReplacementFinished) {
			//    string msg = "previousReplacementFinished[" + previousReplacementFinished + "]";
			//    Assembler.PopupException(msg + msig);
			//    throw new Exception(msg + msig);
			//}

			switch (anyOrderRejected_willBeKilled_andReplaced.State) {
				case OrderState.Rejected:
					string msg4 = "ANOTHER_BRANCH_TO_GET_EMERGENCY_SETTINGS";
					break;

				case OrderState.RejectedKilled:
					string msg5 = "OKAY_NOW_I_CAN_RESUBMIT_THE_ORIGINAL_REJECTED_CLONE";
					break;

				case OrderState.Filled:
				case OrderState.FilledPartially:
					string msg2 = "ORDER_FILLED_WHILE_I_WAS_PREPARING_TO_REPLACE_IT";
					this.OrderProcessor.AppendMessage_propagateToGui(anyOrderRejected_willBeKilled_andReplaced, msg2);
					return replacementScheduled;

				default:
					string msg3 = "MUST_NEVER_HAPPEN__TOO_LATE_TO_KILL__DESPITE_WASNT_FILLED_UPSTACK [" + anyOrderRejected_willBeKilled_andReplaced.State + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(anyOrderRejected_willBeKilled_andReplaced, msg3 + msig);
					return replacementScheduled;
			}

			try {
				bool shallReplace = anyOrderRejected_willBeKilled_andReplaced.Alert.Bars.SymbolInfo.RejectedResubmit;
				if (shallReplace == false) {
					string msg = "WONT_REPLACE_REJECTED_KOZ_SymbolInfo[" + symbolClass + "].RejectedResubmit==false; continuing";
					base.OrderProcessor.AppendMessage_propagateToGui(anyOrderRejected_willBeKilled_andReplaced, msg + msig);
					//Assembler.PopupException(msg);
				} else {
					//v1 replacementScheduled = this.replaceOrder_withSameSlippage(anyOrderRejected_willBeKilled_andReplaced, null);
					replacementScheduled = base.ReplaceOrder_withoutHook(anyOrderRejected_willBeKilled_andReplaced, msig, true, true, false);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
	
			return replacementScheduled;
		}

		// moved to base.ReplaceOrder_withoutHook()
		//bool replaceOrder_withSameSlippage(Order anyOrderRejected, ReporterPokeUnit pokeUnit_nullHere) {
		//    bool replacementScheduled = false;
		//    string msig = " //replaceOrder_withSameSlippage(" + anyOrderRejected + ")";

		//    try {
		//        Order replacement = this.CreateReplacementOrder_insteadOfReplaceExpired(anyOrderRejected);
		//        if (replacement == null) {
		//            string msg = "got NULL from CreateReplacementOrder()"
		//                + "; broker reported twice about rejection, ignored this second callback";
		//            Assembler.PopupException(msg + msig);
		//            //orderToReplace.addMessage(new OrderMessage(msg));
		//            return replacementScheduled;
		//        }

		//        double priceStreaming = replacement.Alert.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot
		//            .GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteLast(
		//                replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, false);

		//        if (replacement.Alert.PositionAffected != null) {	// alert.PositionAffected = null when order created by chart-click-mni
		//            if (replacement.Alert.IsEntryAlert) {
		//                replacement.Alert.PositionAffected.EntryEmitted_price = priceStreaming;
		//            } else {
		//                replacement.Alert.PositionAffected.ExitEmitted_price = priceStreaming;
		//            }
		//        }

		//        string msg_replacement = "REPLACEMENT_FOR_REJECTED["
		//            + replacement.ReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageAppliedIndex + "]";
		//        if (replacement.SlippagesLeftAvailable_noMore) {
		//            msg_replacement += " THIS_IS_THE_LAST_POSSIBLE_SLIPPAGE";
		//        }
		//        this.OrderProcessor.AppendMessage_propagateToGui(replacement, msg_replacement);

		//        //double slippage = replacement.Alert.Bars.SymbolInfo.GetSlippage_signAware_forLimitOrdersOnly(
		//        //	priceScript, replacement.Alert.Direction, replacement.Alert.MarketOrderAs, replacement.SlippageAppliedIndex);
		//        double slippageNext_NanUnsafe = replacement.Alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(replacement.SlippageAppliedIndex);
		//        if (double.IsNaN(slippageNext_NanUnsafe)) {
		//            string msg = "IRREPAIRABLE__YOU_SHOULD_JAVE_NOT_CREATED_REPLACEMENT_ORDER__SEE_reasonCanNotBeReplaced_20_LINES_ABOVE";
		//            Assembler.PopupException(msg);
		//        }


		//        replacement.SlippageApplied = slippageNext_NanUnsafe;
		//        double priceBasedOnLastQuote = priceStreaming + slippageNext_NanUnsafe;
		//        double difference_withExpiredOrder_signInprecise = anyOrderRejected.PriceEmitted - priceBasedOnLastQuote;
		//        replacement.PriceEmitted = priceBasedOnLastQuote;
		//        replacement.Alert.SetNewPriceEmitted_fromReplacementOrder(replacement);	// will repaint the circle at the new order-emitted price PanelPrice.Rendering.cs:86

		//        string verdict = "REPLACING_REJECTED_NOHOOK diffToExpired[" + difference_withExpiredOrder_signInprecise + "] " + replacement;
		//        OrderStateMessage osm = new OrderStateMessage(anyOrderRejected, OrderState.EmittingReplacement, verdict);
		//        this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

		//        bool inNewThread = false;
		//        replacementScheduled = base.SubmitReplacementOrder_insteadOfReplaceExpired(replacement, inNewThread);

		//        replacement.Alert.Strategy.Script.Executor.CallbackOrderReplaced_invokeScript_nonReenterably(
		//                                                    anyOrderRejected, replacement, replacementScheduled);
		//    } catch (Exception ex) {
		//        Assembler.PopupException(msig, ex, false);
		//    } finally {
		//        anyOrderRejected.OrderReplacement_Emitted_afterOriginalKilled__orError.Set();
		//    }

		//    return replacementScheduled;
		//}
	
	}
}