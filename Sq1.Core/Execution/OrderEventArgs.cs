using System;

namespace Sq1.Core.Execution {
	public class OrderEventArgs : EventArgs {
		public Order Order { get; private set; }
		public OrderEventArgs(Order order) {
			this.Order = order;
		}
	}
}
