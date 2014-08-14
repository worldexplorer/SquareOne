using System;
using Sq1.Core.Accounting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderProcessorEventDistributor : OrderEventDistributor {
		private OrderProcessor orderProcessor;

		public OrderProcessorEventDistributor(OrderProcessor orderProcessor) : base() {
			this.orderProcessor = orderProcessor;
		}

		public event EventHandler<OrderEventArgs> TOBE_DEPRECATED_OrderPropertiesUpdatedByExecutionButSameState;
		public event EventHandler<OrderEventArgs> TOBE_DEPRECATED_OrderReplacementOrKillerCreatedForVictim;

		public new void RaiseOrderStateChangedExecutionFormNotification(object sender, Order order) {
			base.RaiseOrderStateChangedExecutionFormNotification(sender, order);
		}
		public void RaiseOrderStateChanged(object sender, Order rejectedOrderToReplace) {
			//return;
			this.RaiseOrderStateChangedExecutionFormNotification(sender, rejectedOrderToReplace);
			this.orderProcessor.DataSnapshot.UpdateActiveOrdersCountEvent();
			this.orderProcessor.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
		}
		public void RaiseOrderReplacementOrKillerCreatedForVictim(object sender, Order rejectedOrderToReplace) {
			this.RaiseOrderStateChangedExecutionFormNotification(sender, rejectedOrderToReplace);
		}
		public void RaiseOrderPropertiesUpdatedByExecutionButSameState(Order order) {
			if (this.TOBE_DEPRECATED_OrderPropertiesUpdatedByExecutionButSameState != null) {
				this.TOBE_DEPRECATED_OrderPropertiesUpdatedByExecutionButSameState(this, new OrderEventArgs(order));
			}
		}
		public void RaiseOrderReplacementOrKillerCreatedForVictim(Order victimOrder) {
			if (this.TOBE_DEPRECATED_OrderReplacementOrKillerCreatedForVictim != null) {
				this.TOBE_DEPRECATED_OrderReplacementOrKillerCreatedForVictim(this, new OrderEventArgs(victimOrder));
			}
		}
	}
}