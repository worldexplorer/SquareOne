using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorRejected {
		OrderProcessor orderProcessor;

		public OrderPostProcessorRejected(OrderProcessor orderProcessor) {
			this.orderProcessor = orderProcessor;
		}
		public void HandleReplaceRejected(Order order) {
			if (order.State != OrderState.Rejected) {
				//Assembler.PopupException("Man, I resubmit  only REJECTED orders, you fed me with State=[" + order.State + "] for order[" + order+ "]");
				return;
			}
			if (order.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				//Assembler.PopupException("Symbol[" + order.Alert.Bars.Symbol + "] has ReSubmitRejected=[" + reSubmitRejected +  "]; returning");
				return;
			}
			if (order.noMoreSlippagesAvailable) {
				AddMessageNoMoreSlippagesAvailable(order);
				//return;
			}
			this.ReplaceRejectedOrder(order);
		}
		public void ReplaceRejectedOrder(Order rejectedOrderToReplace) {
			if (rejectedOrderToReplace.State != OrderState.Rejected) {
				string msg = "will not ReplaceRejectedOrder(" + rejectedOrderToReplace + ") which is not Rejected; continuing";
				this.orderProcessor.AppendOrderMessage_propagateToGui(rejectedOrderToReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			if (rejectedOrderToReplace.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				string msg = "SymbolInfo[" + rejectedOrderToReplace.Alert.Symbol + "/" + rejectedOrderToReplace.Alert.SymbolClass + "].ReSubmitRejected==false"
					+ " will not ReplaceRejectedOrder(" + rejectedOrderToReplace + "); continuing";
				this.orderProcessor.AppendOrderMessage_propagateToGui(rejectedOrderToReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			Order replacement = this.CreateReplacementOrder_insteadOfRejected(rejectedOrderToReplace);
			if (replacement == null) {
				string msg = "ReplaceRejectedOrder(" + rejectedOrderToReplace + ") got NULL from CreateReplacementOrder()"
					+ "; broker reported twice about rejection, ignored this second callback";
				Assembler.PopupException(msg);
				//orderToReplace.addMessage(new OrderMessage(msg));
				return;
			}

			double priceScript = replacement.Alert.DataSource.StreamingAdapter.StreamingDataSnapshot
				.BidOrAsk_getAligned_forTidalOrCrossMarket_fromStreamingSnap(
				replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, true);

			if (replacement.Alert.PositionAffected != null) {	// alert.PositionAffected = null when order created by chart-click-mni
				if (replacement.Alert.IsEntryAlert) {
					replacement.Alert.PositionAffected.EntryPriceScript = priceScript;
				} else {
					replacement.Alert.PositionAffected.ExitPriceScript = priceScript;
				}
			}

			if (replacement.Alert.Bars.SymbolInfo.ReSubmittedUsesNextSlippage == true) {
				replacement.SlippageIndex++;
			}
			string msg_replacement = "This is a replacement for order["
				+ replacement.ReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageIndex + "]";
			this.orderProcessor.AppendOrderMessage_propagateToGui(replacement, msg_replacement);

			if (replacement.noMoreSlippagesAvailable) {
				AddMessageNoMoreSlippagesAvailable(replacement);
				//return;
			}

			double slippage = replacement.Alert.Bars.SymbolInfo.GetSlippage_signAware_forLimitOrdersOnly(
				priceScript, replacement.Alert.Direction, replacement.Alert.MarketOrderAs, replacement.SlippageIndex);
			replacement.SlippageFill = slippage;
			replacement.PriceRequested = priceScript + slippage;
			this.SubmitReplacementOrder_insteadOfRejected(replacement);
		}
		public Order CreateReplacementOrder_insteadOfRejected(Order rejectedOrderToReplace) {
			if (rejectedOrderToReplace == null) {
				Assembler.PopupException("order2replace=null why did you call me?");
				return null;
			}
			Order replacement = this.findReplacementOrder_forRejectedOrder(rejectedOrderToReplace);
			if (replacement != null) {
				string msg = "Rejected[" + rejectedOrderToReplace + "] already has a replacement[" + replacement + "] with State[" + replacement.State + "]; ignored rejection duplicates from broker";
				this.orderProcessor.AppendOrderMessage_propagateToGui(rejectedOrderToReplace, msg);
				return null;
			}
			//DateTime todayDate = DateTime.Now.Date;
			//if (order.ReplacedByGUID != "" && order.OriginalAlertDate.Date == todayDate) {
			//	string msg = "order[" + order.ToString() + "] was already replaced today by [" + order.ReplacedByGUID + "]; continuing generating new order";
			//	Assembler.PopupException(msg);
			//order.addMessage(new OrderMessage(msg));
			//return null;
			//}
			if (rejectedOrderToReplace.hasBrokerAdapter("CreateReplacementOrderInsteadOfRejected(): ") == false) {
				return null;
			}
			Order replacementOrder = rejectedOrderToReplace.DeriveReplacementOrder();
			this.orderProcessor.DataSnapshot.OrderInsert_notifyGuiAsync(replacementOrder);
			this.orderProcessor.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){rejectedOrderToReplace});
			//this.orderProcessor.RaiseOrderReplacementOrKillerCreatedForVictim(this, rejectedOrderToReplace);
			return replacementOrder;
		}
		public Order findReplacementOrder_forRejectedOrder(Order orderRejected) {
			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			
			Order rejected = this.orderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(orderRejected.GUID, out suggestedLane, out suggestion, true);
			if (rejected == null) {
				throw new Exception("OrderRejected[" + orderRejected + "] wasn't found!!! suggestion[" + suggestion + "]");
			}
			if (string.IsNullOrEmpty(rejected.ReplacedByGUID)) return null;
			Order replacement = this.orderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(rejected.ReplacedByGUID, out suggestedLane, out suggestion, true);
			return replacement;
		}
		public void SubmitReplacementOrder_insteadOfRejected(Order replacementOrder) {
			if (replacementOrder == null) {
				Assembler.PopupException("replacementOrder == null why did you call me?");
				return;
			}
			if (replacementOrder.hasBrokerAdapter("PlaceReplacementOrderInsteadOfRejected(): ") == false) {
				return;
			}

			string msg = "Scheduling SubmitOrdersThreadEntry [" + replacementOrder.ToString() + "] slippageIndex["
				+ replacementOrder.SlippageIndex + "] through [" + replacementOrder.Alert.DataSource.BrokerAdapter + "]";
			OrderStateMessage newOrderState = new OrderStateMessage(replacementOrder, OrderState.PreSubmit, msg);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);

			//this.BrokerAdapter.SubmitOrdersThreadEntry(ordersFromAlerts);
			//ThreadPool.QueueUserWorkItem(new WaitCallback(replacementOrder.Alert.DataSource.BrokerAdapter.SubmitOrdersThreadEntry),
			//	new object[] { new List<Order>() { replacementOrder } });
			List<Order> replacementOrder_oneInTheList = new List<Order>() { replacementOrder };
			BrokerAdapter broker = replacementOrder.Alert.DataSource.BrokerAdapter;
			this.orderProcessor.SubmitToBrokerAdapter_inNewThread(replacementOrder_oneInTheList, broker);

			//this.orderProcessor.UpdateActiveOrdersCountEvent();
		}
		public void AddMessageNoMoreSlippagesAvailable(Order order) {
			SymbolInfo symbolInfo = order.Alert.Bars.SymbolInfo;
			int slippageIndexMax = symbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(order.Alert);
			string msg2 = "Reached max slippages available for [" + order.Alert.Bars.Symbol + "]"
				+ " order.SlippageIndex[" + order.SlippageIndex + "] > slippageIndexMax[" + slippageIndexMax + "]"
				+ "; Order will have slippageIndexMax[" + slippageIndexMax + "]"; 
			Assembler.PopupException(msg2);
			//orderProcessor.updateOrderStatusError(orderExecuted, OrderState.RejectedLimitReached, msg2);
			OrderStateMessage newOrderStateRejected = new OrderStateMessage(order, OrderState.RejectedLimitReached, msg2);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderStateRejected);
		}
	}
}