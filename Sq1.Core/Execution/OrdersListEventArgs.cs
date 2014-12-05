using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class OrdersListEventArgs : EventArgs {
		public List<Order> Orders { get; private set; }
		public OrdersListEventArgs(List<Order> order) {
			this.Orders = order;
		}
	}
}
