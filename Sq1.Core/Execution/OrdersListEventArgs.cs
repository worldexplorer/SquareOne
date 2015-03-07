using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class OrdersListEventArgs : EventArgs {
		public List<Order>	Orders				{ get; private set; }
		public bool			GuiHasTimeToRebuild	{ get; private set;}
		public OrdersListEventArgs(List<Order> order, bool guiHasTimeToRebuild = true) {
			this.Orders = order;
			this.GuiHasTimeToRebuild = guiHasTimeToRebuild;
		}
	}
}
