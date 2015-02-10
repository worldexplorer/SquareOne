using System;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public abstract class OrderCallbackDupesChecker {
		protected BrokerAdapter brokerAdapter;
		public OrderCallbackDupesChecker(BrokerAdapter brokerAdapter) {
			this.brokerAdapter = brokerAdapter;
		}
		public abstract string OrderCallbackIsDupeReson(
			Order order, OrderStateMessage newStateOmsg, double priceFill, double qtyFill);
	}
}
