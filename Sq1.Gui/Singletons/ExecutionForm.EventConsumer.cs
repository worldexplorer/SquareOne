using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;

namespace Sq1.Gui.Singletons {
	public partial class ExecutionForm {
		public void orderProcessor_OnOrderAdded(object sender, OrdersListEventArgs eventOrderList) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OnOrderAdded(sender, eventOrderList); });
				return;
			}
			if (eventOrderList.Orders.Count == 0) return;
			
			this.ExecutionTreeControl.OnOrdersInserted_asyncAutoFlush(eventOrderList.Orders);

			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush 
			//foreach (Order orderAdded in eventOrderList.Orders) {
			//    this.ExecutionTreeControl.OlvOrdersTree_insertOrder(o);
			//}
			
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.PopulateWindowTitle();
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (base.IsCoveredOrAutoHidden) return;
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.ExecutionTreeControl.RebuildAllTree_focusOnTopmost();
		}
		void orderProcessor_OnOrderMessageAdded(object sender, OrderStateMessageEventArgs eventOsm) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				if (base.IsDisposed) return;
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OnOrderMessageAdded(sender, eventOsm); });
				return;
			}

			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.PopulateWindowTitle();
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (base.IsCoveredOrAutoHidden) return;
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.ExecutionTreeControl.SelectOrder_populateMessages(e.OrderStateMessage.Order);
			//v2
			this.ExecutionTreeControl.OnOrderMessageAppended_immediate(eventOsm.OrderStateMessage);
		}
		void orderProcessor_OnOrderStateChanged(object sender, OrdersListEventArgs eventOrderList) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OnOrderStateChanged(sender, eventOrderList); });
				return;
			}
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.PopulateWindowTitle();
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (base.IsCoveredOrAutoHidden) return;	// could've been checked before switching to gui thread?...
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush
			this.ExecutionTreeControl.OlvOrdersTree_updateState_immediate(eventOrderList.Orders);
		}
		void orderProcessor_OnOrdersRemoved(object sender, OrdersListEventArgs eventOrderList) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OnOrdersRemoved(sender, eventOrderList); });
				return;
			}
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (base.IsCoveredOrAutoHidden) return;
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush this.ExecutionTreeControl.OrderRemoved_alreadyFromBothLists_rebuildOrdersTree_cleanMessagesView(eventOrderList.Orders);
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush sthis.PopulateWindowTitle();
			this.ExecutionTreeControl.OnOrdersRemoved_asyncAutoFlush(eventOrderList.Orders);
		}
		void orderProcessor_OnDelaylessLivesimEnded_shouldRebuildExecutionOLV(object sender, EventArgs e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OnDelaylessLivesimEnded_shouldRebuildExecutionOLV(sender, e); });
				return;
			}
			this.ExecutionTreeControl.RebuildAllTree_focusOnRecent();
		}
		void executionTree_OnOrderSingleClicked_ChartControlShouldPopupPosition(object sender, OrderEventArgs e) {
			try {
				ChartShadow chartFound = Assembler.InstanceInitialized.AlertsForChart.FindContainerFor_throws(e.Order.Alert);
				chartFound.SelectPosition(e.Order.Alert.PositionAffected);
			} catch (Exception ex) {
				//string msg = "TODO: add chartManager to Assembler, tunnel Execution.DoubleClick => select Chart.Trade; orderDoubleClicked[" + e.Order + "]";
				string msg = "HISTORICAL_ORDER_ISNT_LINKED_TO_ANY_CHART__CANT_POPUP_POSITION orderDoubleClicked[" + e.Order + "]";
				//Assembler.PopupException(msg, ex, false);
			}
		}
		void executionForm_Load(object sender, EventArgs e) {
			this.orderProcessor.OnOrderAdded_executionControlShouldRebuildOLV_scheduled						+= this.orderProcessor_OnOrderAdded;
			this.orderProcessor.OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled					+= this.orderProcessor_OnOrdersRemoved;
			this.orderProcessor.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately	+= this.orderProcessor_OnOrderStateChanged;
			this.orderProcessor.OnOrderMessageAdded_executionControlShouldPopulate_scheduled				+= this.orderProcessor_OnOrderMessageAdded;
			this.orderProcessor.OnDelaylessLivesimEnded_shouldRebuildOLV_immediately						+= this.orderProcessor_OnDelaylessLivesimEnded_shouldRebuildExecutionOLV;
			this.ExecutionTreeControl.OnOrderSingleClicked_ChartControlShouldPopupPosition					+= this.executionTree_OnOrderSingleClicked_ChartControlShouldPopupPosition;
		}
		void executionForm_Closed(object sender, FormClosedEventArgs e) {
			string msg = "ExecutionForm_Closed(): all self-hiding singletons are closed() on MainForm.Close()?";
			//ExecutionForm.Instance = null;
		}
		void executionForm_Closing(object sender, FormClosingEventArgs e) {
			this.orderProcessor.OnOrderAdded_executionControlShouldRebuildOLV_scheduled						-= this.orderProcessor_OnOrderAdded;
			this.orderProcessor.OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled					-= this.orderProcessor_OnOrdersRemoved;
			this.orderProcessor.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately	-= this.orderProcessor_OnOrderStateChanged;
			this.orderProcessor.OnOrderMessageAdded_executionControlShouldPopulate_scheduled				-= this.orderProcessor_OnOrderMessageAdded;
			this.ExecutionTreeControl.OnOrderSingleClicked_ChartControlShouldPopupPosition					-= this.executionTree_OnOrderSingleClicked_ChartControlShouldPopupPosition;

			string msg = "ExecutionForm_Closing(): unsubscribed from orderProcessor.OnOrder{Added/Removed/StateChanged/MessageAdded}";
			Assembler.PopupException(msg);
		}
	}
}