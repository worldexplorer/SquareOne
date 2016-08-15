using System;

namespace Sq1.Core.Execution {
	public class OrderStateChangedEventArgs : EventArgs {
		public Order		Order						{ get; private set; }
		public OrderState	OrderState_beforeChanged	{ get; private set; }
		public OrderStateChangedEventArgs(Order order, OrderState orderState_beforeChanged) {
			this.Order = order;
			this.OrderState_beforeChanged = orderState_beforeChanged;
		}
	}
}
