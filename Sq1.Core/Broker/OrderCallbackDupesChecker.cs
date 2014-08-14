using System;
using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public abstract class OrderCallbackDupesChecker {
		protected BrokerProvider brokerProvider;
		public OrderCallbackDupesChecker(BrokerProvider brokerProvider) {
			this.brokerProvider = brokerProvider;
		}
		public abstract string OrderCallbackDupeResonWhy(
			Order order, OrderStateMessage newStateOmsg, double priceFill, double qtyFill);
	}
}
