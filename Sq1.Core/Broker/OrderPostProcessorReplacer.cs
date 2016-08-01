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

		protected BrokerAdapter GetBrokerAdapter_fromOrder_nullUnsafe(Order orderStuckInSubmitted_brokerMustBeReconnected, string msig_invoker) {
			BrokerAdapter ret = null;

			if (orderStuckInSubmitted_brokerMustBeReconnected.Alert == null) {
				string msg = "ALERT_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderStuckInSubmitted_brokerMustBeReconnected, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderStuckInSubmitted_brokerMustBeReconnected.Alert.Bars == null) {
				string msg = "BARS_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderStuckInSubmitted_brokerMustBeReconnected, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderStuckInSubmitted_brokerMustBeReconnected.Alert.DataSource_fromBars == null) {
				string msg = "DATASOURCE_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderStuckInSubmitted_brokerMustBeReconnected, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}
			if (orderStuckInSubmitted_brokerMustBeReconnected.Alert.DataSource_fromBars.BrokerAdapter == null) {
				string msg = "BROKER_ADAPTER_MUST_NOT_BE_NULL";
				this.OrderProcessor.AppendMessage_propagateToGui(orderStuckInSubmitted_brokerMustBeReconnected, msg);
				Assembler.PopupException(msg + msig_invoker);
				return ret;
			}

			ret = orderStuckInSubmitted_brokerMustBeReconnected.Alert.DataSource_fromBars.BrokerAdapter;
			return ret;
		}

		protected Order CreateReplacementOrder_forExpired(Order orderExpired_toReplace) {
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
		protected bool EmitReplacementOrder_insteadOfExpired(Order replacementOrder, bool inNewThread = true) {
			bool submittedOrScheduled = false;

			string msig = " //SubmitReplacementOrder_insteadOfExpired()";
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


		protected bool ReplaceOrder_withoutHook(Order orderStuckInSubmitted,
						string invoker,
						bool recalculatePriceEmitted_fromCurrentMarket = true,
						bool recalculatePriceEmitted_applyNextSlippage = false,
						bool inNewThread = false) {

			bool replacementEmitted = false;
			//string msig = " //ReplaceOrder_withoutHook(" + orderStuckInSubmitted + ") << " + invoker;
			string msig = " //ReplaceOrder_withoutHook(recalculatePriceEmitted_fromCurrentMarket[" + recalculatePriceEmitted_fromCurrentMarket + "],"
				+ " recalculatePriceEmitted_applyNextSlippage[" + recalculatePriceEmitted_applyNextSlippage + "],"
				+ " inNewThread[" + inNewThread + "] " + orderStuckInSubmitted + ") << " + invoker;

			try {
				Order replacement = this.CreateReplacementOrder_forExpired(orderStuckInSubmitted);
				if (replacement == null) {
					string msg = "got NULL from CreateReplacementOrder()"
						+ "; broker reported twice about rejection, ignored this second callback";
					Assembler.PopupException(msg + msig);
					//orderToReplace.addMessage(new OrderMessage(msg));
					return replacementEmitted;
				}

				bool newPriceEmitted_onlyForLimitOrders_orMarketReplacedAsLimit = orderStuckInSubmitted.Alert.MarketLimitStop != MarketLimitStop.Market;

				string diff = "";
				if (recalculatePriceEmitted_fromCurrentMarket && newPriceEmitted_onlyForLimitOrders_orMarketReplacedAsLimit) {
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

					string msg_replacement = "REPLACEMENT_FOR_STUCK_IN_SUBMITTED["
						+ replacement.ReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageAppliedIndex + "]";
					if (replacement.SlippagesLeftAvailable_noMore) {
						msg_replacement += " THIS_IS_THE_LAST_POSSIBLE_SLIPPAGE";
					}
					this.OrderProcessor.AppendMessage_propagateToGui(replacement, msg_replacement);

					if (recalculatePriceEmitted_applyNextSlippage) {
						//double slippage = replacement.Alert.Bars.SymbolInfo.GetSlippage_signAware_forLimitOrdersOnly(
						//	priceScript, replacement.Alert.Direction, replacement.Alert.MarketOrderAs, replacement.SlippageAppliedIndex);
						double slippageNext_NanUnsafe = replacement.Alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(replacement.SlippageAppliedIndex);
						if (double.IsNaN(slippageNext_NanUnsafe)) {
							string msg = "IRREPAIRABLE__YOU_SHOULD_JAVE_NOT_CREATED_REPLACEMENT_ORDER__SEE_reasonCanNotBeReplaced_20_LINES_ABOVE";
							Assembler.PopupException(msg);
						}

						replacement.SlippageApplied = slippageNext_NanUnsafe;
						double priceBasedOnLastQuote = priceStreaming + slippageNext_NanUnsafe;
						double difference_withExpiredOrder_signInprecise = orderStuckInSubmitted.PriceEmitted - priceBasedOnLastQuote;
						replacement.PriceEmitted = priceBasedOnLastQuote;

						diff = " diffToExpired[" + difference_withExpiredOrder_signInprecise + "] ";
					}
				}

				replacement.Alert.SetNew_OrderFollowed_PriceEmitted_fromReplacementOrder(replacement);	// will repaint the circle at the new order-emitted price PanelPrice.Rendering.cs:86

				string verdict = "REPLACING_NOHOOK " + diff + replacement + invoker;
				OrderStateMessage osm = new OrderStateMessage(orderStuckInSubmitted, OrderState.EmittingReplacement, verdict);
				this.OrderProcessor.AppendOrderMessage_propagateToGui(osm);

				replacementEmitted = this.EmitReplacementOrder_insteadOfExpired(replacement, inNewThread);

				replacement.Alert.Strategy.Script.Executor.CallbackOrderReplaced_invokeScript_nonReenterably(
															orderStuckInSubmitted, replacement, replacementEmitted);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			} finally {
				orderStuckInSubmitted.OrderReplacement_Emitted_afterOriginalKilled__orError.Set();
			}

			return replacementEmitted;
		}	
	}
}