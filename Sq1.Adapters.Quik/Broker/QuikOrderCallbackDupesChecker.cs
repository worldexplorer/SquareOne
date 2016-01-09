using System;

using Sq1.Core.Execution;
using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik.Broker {
	public class OrderCallbackDupesCheckerQuik : OrderCallbackDupesChecker {
		public OrderCallbackDupesCheckerQuik(BrokerAdapter brokerAdapter) : base(brokerAdapter) {
		}
		public override string OrderCallbackIsDupeReson(Order order, OrderStateMessage newOrderStateMessage, double priceFill, double qtyFill) {
			string whyIthinkBrokerIsSpammingMe = null;

			string whatIsDifferent = "";
			if (order.State != newOrderStateMessage.State) {
				whatIsDifferent += "order.State[" + order.State + "] => omsg.State[" + newOrderStateMessage.State + "] ";
			}

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
					if (order.PriceFill != priceFill) {
						whatIsDifferent += "order.PriceFill[" + order.PriceFill+ "] => priceFill[" + priceFill + "] ";
					}
					if (order.QtyFill != qtyFill) {
						whatIsDifferent += "order.QtyFill[" + order.QtyFill+ "] => qtyFill[" + qtyFill + "] ";
					}
					break;
			}

			if (whatIsDifferent == "") {
				whyIthinkBrokerIsSpammingMe = "NOTHING_TO_UPDATE_SAME State[" + order.State + "] priceFill[" + priceFill + "] qtyFill[" + qtyFill + "]";
			}
			return whyIthinkBrokerIsSpammingMe;
		}
	}
}
