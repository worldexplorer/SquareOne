using System;

using Sq1.Core.Execution;
using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik {
	public class OrderCallbackDupesCheckerQuik : OrderCallbackDupesChecker {
		public OrderCallbackDupesCheckerQuik(BrokerProvider brokerProvider) : base(brokerProvider) {
		}
		public override string OrderCallbackIsDupeReson(Order order, OrderStateMessage newOrderStateMessage, double priceFill, double qtyFill) {
			string ret = null;
			switch (newOrderStateMessage.State) {
				//v1
				//case OrderState.Filled:
				//case OrderState.FilledPartially:
				//	if (order.FindStateInOrderMessages(newStateOmsg.State)) {
				//		ret = "DUPE coz state[" + newStateOmsg.State + "] was found in Messages";
				//	}
				//v2
				default:
					// TESTME triggered by QuikTerminalMock.simulateOrderStatusDupes=true
					string whatIsDifferent = "";
					if (order.PriceFill != priceFill) {
						whatIsDifferent += "order.PriceFill[" + order.PriceFill+ "] => priceFill[" + priceFill + "] ";
					}
					if (order.QtyFill != qtyFill) {
						whatIsDifferent += "order.QtyFill[" + order.QtyFill+ "] => qtyFill[" + qtyFill + "] ";
					}
					
					if (whatIsDifferent != "") break;
					ret = "ORDER_PRICE,QTY_FILL_ARE_THE_SAME";
					break;
			}
			return ret;
		}
	}
}
