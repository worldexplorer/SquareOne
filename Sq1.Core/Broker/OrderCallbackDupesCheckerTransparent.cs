using System;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderCallbackDupesCheckerTransparent : OrderCallbackDupesChecker {
		public OrderCallbackDupesCheckerTransparent(BrokerProvider brokerProvider)
			: base(brokerProvider) {
		}
		public override string OrderCallbackIsDupeReson(
				Order order, OrderStateMessage newStateOmsg, double priceFill, double qtyFill) {
			return null;
		}
	}
}
