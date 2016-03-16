using System;

using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		public event EventHandler<OrderEventArgs> OnOrderSingleClicked_ChartControlShouldPopupPosition;
		public event EventHandler<OrderEventArgs> OnOrderDoubleClicked_OrderProcessorShouldKillOrder;
		
		void raiseOnOrderSingleClicked_ChartControlShouldPopupPosition(object sender, Order order) {
			if (this.OnOrderSingleClicked_ChartControlShouldPopupPosition == null) return;
			try {
				this.OnOrderSingleClicked_ChartControlShouldPopupPosition(sender, new OrderEventArgs(order));
			} catch (Exception ex) {
				Assembler.PopupException("ExecutionTree::raiseOnOrderSingleClicked_ChartControlShouldPopupPosition() Failed to deliver an event", ex);
			}
		}
		void raiseOnOrderDoubleClicked_OrderProcessorShouldKillOrder(object sender, Order order) {
			if (this.OnOrderDoubleClicked_OrderProcessorShouldKillOrder == null) return;
			try {
				this.OnOrderDoubleClicked_OrderProcessorShouldKillOrder(sender, new OrderEventArgs(order));
			} catch (Exception ex) {
				Assembler.PopupException("ExecutionTree::raiseOnOrderDoubleClicked_OrderProcessorShouldKillOrder() Failed to deliver an event", ex);
			}
		}

	}
}
