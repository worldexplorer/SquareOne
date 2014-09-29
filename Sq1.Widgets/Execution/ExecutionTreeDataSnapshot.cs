using System;

namespace Sq1.Widgets.Execution {
	public class ExecutionTreeDataSnapshot {
		public bool firstRowShouldStaySelected = true;
		public int pricingDecimalForSymbol = 0;
		
		public bool ToggleBrokerTime = false;
		public bool ToggleCompletedOrders = true;
		public bool ToggleMessagesPane = true;
		public bool ToggleMessagePaneSplittedHorizontally = false;
		public bool ToggleSyncWithChart = true;
		
		public int MessagePaneSplitDistanceHorizontal = 0;
		public int MessagePaneSplitDistanceVertical = 0;
	}
}
