using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;

namespace Sq1.Gui.Singletons {
	public partial class ExecutionForm {
		public void orderProcessor_OrderAdded(object sender, OrdersListEventArgs e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OrderAdded(sender, e); });
				return;
			}
			//WDYM? this.ShowPopupSwitchToGuiThreadRunDelegateInIt();
			//if (e.Order.State == OrderState.AutoSubmitNotEnabled) return;
			if (this.IsHiddenHandlingRepopulate) return;
			//if (this.executionTree.SelectedAccountNumbers.Contains(e.Order.Alert.AccountNumber) == false) return;
			//if (this.ExecutionTreeControl.DataSnapshot.ToggleSingleClickSyncWithChart) {
			//	this.executionTree_OnOrderDoubleClickedChartFormNotification(sender, e);
			//}
			if (e.Orders.Count == 0) return;
			
			this.PopulateWindowText();
			
			//v1 when in virtual mode, use model :(
			foreach (Order o in e.Orders) {
				this.ExecutionTreeControl.OrderInsertToListView(o);
			}
			//v2 ADDED_anyHasTime_FILTER__SAFE_ENOUGH_TO_SKIP_SOME_SINCE_ENDOFBACKTEST_WILL_RUIBUILD_FULLY  TOO_SLOW
			//bool safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = false;
			//foreach (Order order in e.Orders) {
			//	if (order.Alert.IsBacktestingLivesimNow_FalseIfNoBacktester == false) break;
			//	if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) continue;
			//	safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = true;
			//	break;
			//}
			//if (safeToIgnoreForLivesimSinceBacktestEndRebuildsAll == true) return;

			this.ExecutionTreeControl.RebuildAllTreeFocusOnTopmost();
		}
		void orderProcessor_OrderMessageAdded(object sender, OrderStateMessageEventArgs e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				if (base.IsDisposed) return;
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OrderMessageAdded(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate) return;
			//this.executionTree.OrderInsertMessage(e.OrderStateMessage);
			//this.executionTree.PopulateMessagesFromSelectedOrder(e.OrderStateMessage.Order);

			this.PopulateWindowText();

			//Alert alert = e.OrderStateMessage.Order.Alert;
			//bool safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = (alert.IsBacktestingLivesimNow_FalseIfNoBacktester == true && alert.GuiHasTimeRebuildReportersAndExecution == false);
			//if (safeToIgnoreForLivesimSinceBacktestEndRebuildsAll == true) return;

			this.ExecutionTreeControl.SelectOrderAndOrPopulateMessages(e.OrderStateMessage.Order);
		}
		void orderProcessor_OrderStateChanged(object sender, OrdersListEventArgs e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OrderStateChanged(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate) return;	// could've been checked before switching to gui thread?...

			this.PopulateWindowText();

			//bool safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = false;
			//foreach (Order order in e.Orders) {
			//	if (order.Alert.IsBacktestingLivesimNow_FalseIfNoBacktester == false) break;
			//	//if (order.Alert.GuiHasTimeRebuildReportersAndExecution == false) continue;
			//	safeToIgnoreForLivesimSinceBacktestEndRebuildsAll = true;
			//	break;
			//}
			//if (safeToIgnoreForLivesimSinceBacktestEndRebuildsAll == true) return;

			this.ExecutionTreeControl.OrderStateUpdateOLV(e.Orders);
		}
		void orderProcessor_OrderRemoved(object sender, OrdersListEventArgs e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OrderRemoved(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate) return;
			this.ExecutionTreeControl.OrderAlreadyRemovedFromBothLists_JustRebuildListView(e.Orders);
			this.PopulateWindowText();
		}
		void orderProcessor_OnDelaylessLivesimEndedShouldRebuildOLV(object sender, EventArgs e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OnDelaylessLivesimEndedShouldRebuildOLV(sender, e); });
				return;
			}
			this.ExecutionTreeControl.RebuildAllTreeFocusOnTopmost();
			this.PopulateWindowText();
		}
		void executionTree_OnOrderDoubleClickedChartFormNotification(object sender, OrderEventArgs e) {
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
			this.orderProcessor.OnOrderAddedExecutionFormNotification			+= this.orderProcessor_OrderAdded;
			this.orderProcessor.OnOrderRemovedExecutionFormNotification			+= this.orderProcessor_OrderRemoved;
			this.orderProcessor.OnOrderStateChangedExecutionFormNotification	+= this.orderProcessor_OrderStateChanged;
			this.orderProcessor.OnOrderMessageAddedExecutionFormNotification	+= this.orderProcessor_OrderMessageAdded;
			this.orderProcessor.OnDelaylessLivesimEndedShouldRebuildOLV			+= this.orderProcessor_OnDelaylessLivesimEndedShouldRebuildOLV;

			//this.ExecutionTreeControl.OnOrderStatsChangedRecalculateWindowTitleExecutionFormNotification += delegate { this.PopulateWindowText(); };
			this.ExecutionTreeControl.OnOrderDoubleClickedChartFormNotification += this.executionTree_OnOrderDoubleClickedChartFormNotification;

			isHiddenPrevState = base.IsHidden;
			//BETTER_IN_MainForm.WorkspaceLoad(): this.ExecutionTreeControl.MoveStateColumnToLeftmost();
		}
		void executionForm_Closed(object sender, FormClosedEventArgs e) {
			string msg = "ExecutionForm_Closed(): all self-hiding singletons are closed() on MainForm.Close()?";
			//ExecutionForm.Instance = null;
		}
		void executionForm_Closing(object sender, FormClosingEventArgs e) {
			this.orderProcessor.OnOrderAddedExecutionFormNotification -= this.orderProcessor_OrderAdded;
			this.orderProcessor.OnOrderRemovedExecutionFormNotification -= this.orderProcessor_OrderRemoved;
			this.orderProcessor.OnOrderStateChangedExecutionFormNotification -= this.orderProcessor_OrderStateChanged;
			this.orderProcessor.OnOrderMessageAddedExecutionFormNotification -= this.orderProcessor_OrderMessageAdded;
			string msg = "ExecutionForm_Closed(): unsubscribed from orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification";
			Assembler.PopupException(msg);
		}
	}
}