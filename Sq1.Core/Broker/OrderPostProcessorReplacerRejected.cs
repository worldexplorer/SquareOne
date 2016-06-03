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
				string msg = "will not ReplaceOrder_withNextSlippage(" + orderRejected_toReplace + ") is now [" + orderRejected_toReplace.State + "] mustBe[Rejected] ; continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(orderRejected_toReplace, msg);
				//Assembler.PopupException(msg);
				return;
			}
			if (orderRejected_toReplace.Alert.Bars.SymbolInfo.ReSubmitRejected == false) {
				string msg = "SymbolInfo[" + orderRejected_toReplace.Alert.Symbol + "/" + orderRejected_toReplace.Alert.SymbolClass + "].RejectedExpiredReSubmit==false"
					+ " will not ReplaceOrder_withNextSlippage(" + orderRejected_toReplace + "); continuing";
				base.OrderProcessor.AppendMessage_propagateToGui(orderRejected_toReplace, msg);
				//Assembler.PopupException(msg);
				return;
			}
			if (orderRejected_toReplace.SlippagesLeftAvailable_noMore) {
				base.AddMessage_noMoreSlippagesAvailable(orderRejected_toReplace);
				return;
			}

			base.ReplaceOrder_withNextSlippage(orderRejected_toReplace);
		}
	}
}