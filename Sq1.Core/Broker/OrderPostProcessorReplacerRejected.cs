using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacerRejected : OrderPostProcessorReplacer {

		public OrderPostProcessorReplacerRejected(OrderProcessor orderProcessor) : base(orderProcessor) {
		}
		public void ReplaceRejected_ifResubmitRejected_setInSymbolInfo(Order order) {
			if (order.State != OrderState.Rejected) {
				//Assembler.PopupException("Man, I resubmit  only REJECTED orders, you fed me with State=[" + order.State + "] for order[" + order+ "]");
				return;
			}
			if (order.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				//Assembler.PopupException("Symbol[" + order.Alert.Bars.Symbol + "] has ReSubmitRejected=[" + reSubmitRejected +  "]; returning");
				return;
			}
			if (order.SlippagesLeftAvailable_noMore) {
				base.AddMessage_noMoreSlippagesAvailable(order);
				//return;
			}
			this.replaceRejectedOrder(order);
		}
		void replaceRejectedOrder(Order rejectedOrderToReplace) {
			if (rejectedOrderToReplace.State != OrderState.Rejected) {
				string msg = "will not ReplaceRejectedOrder(" + rejectedOrderToReplace + ") which is not Rejected; continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(rejectedOrderToReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			if (rejectedOrderToReplace.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				string msg = "SymbolInfo[" + rejectedOrderToReplace.Alert.Symbol + "/" + rejectedOrderToReplace.Alert.SymbolClass + "].ReSubmitRejected==false"
					+ " will not ReplaceRejectedOrder(" + rejectedOrderToReplace + "); continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(rejectedOrderToReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			base.ReplaceOrder_withNextSlippage(rejectedOrderToReplace);
		}
	}
}