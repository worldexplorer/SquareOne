using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacerRejected : OrderPostProcessorReplacer {

		public OrderPostProcessorReplacerRejected(OrderProcessor orderProcessor) : base(orderProcessor) {
		}
		public void ReplaceRejected_ifResubmitRejected_setInSymbolInfo(Order orderRejected_toReplace) {
			if (orderRejected_toReplace.State != OrderState.Rejected) {
				//Assembler.PopupException("Man, I resubmit  only REJECTED orders, you fed me with State=[" + order.State + "] for order[" + order+ "]");
				return;
			}
			if (orderRejected_toReplace.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				//Assembler.PopupException("Symbol[" + order.Alert.Bars.Symbol + "] has ReSubmitRejected=[" + reSubmitRejected +  "]; returning");
				return;
			}
			if (orderRejected_toReplace.SlippagesLeftAvailable_noMore) {
				base.AddMessage_noMoreSlippagesAvailable(orderRejected_toReplace);
				return;
			}

			if (orderRejected_toReplace.State != OrderState.Rejected) {
				string msg = "will not ReplaceRejectedOrder(" + orderRejected_toReplace + ") which is not Rejected; continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(orderRejected_toReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			if (orderRejected_toReplace.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				string msg = "SymbolInfo[" + orderRejected_toReplace.Alert.Symbol + "/" + orderRejected_toReplace.Alert.SymbolClass + "].ReSubmitRejected==false"
					+ " will not ReplaceRejectedOrder(" + orderRejected_toReplace + "); continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(orderRejected_toReplace, msg);
				Assembler.PopupException(msg);
				return;
			}
			base.ReplaceOrder_withNextSlippage(orderRejected_toReplace);
		}
	}
}