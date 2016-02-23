using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class OrdersListEventArgs : EventArgs {
		public List<Order>	Orders				{ get; private set; }
		//public bool			GuiHasTimeToRebuild	{ get; private set;}
		//public bool			GuiHasTimeToRebuild	{ get {
		//	bool safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = false;
		//	foreach (Order order in e.Orders) {
		//		if (order.Alert.IsBacktestingLivesimNow_FalseIfNoBacktester == false) break;
		//		//if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) continue;
		//		safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = true;
		//		break;
		//	}
		//} }
		public OrdersListEventArgs(List<Order> order, bool guiHasTimeToRebuild = true) {
			this.Orders = order;
			//this.GuiHasTimeToRebuild = guiHasTimeToRebuild;
		}
	}
}
