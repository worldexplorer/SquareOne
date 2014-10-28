using System;
using Sq1.Core.Execution;
using Sq1.Core.Broker;

namespace Sq1.QuikAdapter {
	public class OrderCallbackDupesCheckerQuik : OrderCallbackDupesChecker {
		public OrderCallbackDupesCheckerQuik(BrokerProvider brokerProvider)
			: base(brokerProvider) {
		}
		public override string OrderCallbackDupeResonWhy(
				Order order, OrderStateMessage newStateOmsg, double priceFill, double qtyFill) {

			string ret = null;
			switch (newStateOmsg.State) {
				case OrderState.Filled:
				case OrderState.FilledPartially:
					if (order.FindStateInOrderMessages(newStateOmsg.State)) {
						//ret = "DUPE coz state[" + newStateOmsg.State + "] was found in Messages";
					}
					break;
			}
			return ret;
		}
	}
}
