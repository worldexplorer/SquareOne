using System;
using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		public event EventHandler<OrderEventArgs> OnOrderStatsChangedRecalculateWindowTitleExecutionFormNotification;
		public event EventHandler<OrderEventArgs> OnOrderDoubleClickedChartFormNotification;
		
		void raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification(object sender, Order order) {
			if (this.OnOrderStatsChangedRecalculateWindowTitleExecutionFormNotification == null) return;
			try {
				this.OnOrderStatsChangedRecalculateWindowTitleExecutionFormNotification(sender, new OrderEventArgs(order));
			} catch (Exception ex) {
				Assembler.PopupException("ExecutionTree::raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification() Failed to deliver an event", ex);
			}
		}

		void raiseOnOrderDoubleClickedChartFormNotification(object sender, Order order) {
			if (this.OnOrderDoubleClickedChartFormNotification == null) return;
			try {
				this.OnOrderDoubleClickedChartFormNotification(sender, new OrderEventArgs(order));
			} catch (Exception ex) {
				Assembler.PopupException("ExecutionTree::raiseOnOrderDoubleClickedChartFormNotification() Failed to deliver an event", ex);
			}
		}

	}
}
