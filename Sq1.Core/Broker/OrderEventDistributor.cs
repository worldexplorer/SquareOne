using System;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderEventDistributor {
		public event EventHandler<OrderEventArgs> OnOrderAddedExecutionFormNotification;
		public event EventHandler<OrderEventArgs> OnOrderRemovedExecutionFormNotification;
		public event EventHandler<OrderEventArgs> OnOrderStateChangedExecutionFormNotification;
		public event EventHandler<OrderStateMessageEventArgs> OnOrderMessageAddedExecutionFormNotification;
		
		public OrderEventDistributor() {
		}
		
		public void RaiseOrderAddedExecutionFormNotification(object sender, Order orderAdded) {
			if (this.OnOrderAddedExecutionFormNotification == null) return;
			this.OnOrderAddedExecutionFormNotification(sender, new OrderEventArgs(orderAdded));
		}
		public void RaiseOrderRemovedExecutionFormNotification(object sender, Order orderRemoved) {
			if (this.OnOrderRemovedExecutionFormNotification == null) return;
			this.OnOrderRemovedExecutionFormNotification(sender, new OrderEventArgs(orderRemoved));
		}
		public void RaiseOrderStateChangedExecutionFormNotification(object sender, Order orderUpdated) {
			if (this.OnOrderStateChangedExecutionFormNotification == null) return;
			this.OnOrderStateChangedExecutionFormNotification(sender, new OrderEventArgs(orderUpdated));
		}
		public void RaiseOrderMessageAddedExecutionFormNotification(object sender, OrderStateMessage orderStateMessage) {
			if (this.OnOrderMessageAddedExecutionFormNotification == null) return;
			this.OnOrderMessageAddedExecutionFormNotification(sender, new OrderStateMessageEventArgs(orderStateMessage));
		}
		
	}
}
