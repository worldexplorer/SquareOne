using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.Execution;

namespace Sq1.Gui.Singletons {
	public partial class ExecutionForm : DockContentSingleton<ExecutionForm> {
		OrderProcessor orderProcessor;
		bool isHiddenPrevState;

		public ExecutionForm() {
			this.InitializeComponent();
		}
		
		public void Initialize(OrderProcessor orderProcessor) {
			this.orderProcessor = orderProcessor;
			
			//this.executionTree.Initialize(this.orderProcessor.DataSnapshot.OrdersAll.SafeCopy);
			this.ExecutionTreeControl.InitializeWithShadowTreeRebuilt(this.orderProcessor.DataSnapshot.OrdersTree);
			this.ExecutionTreeControl.PopulateAccountsMenuFromBrokerProvider(Assembler.InstanceInitialized.RepositoryJsonDataSource.CtxAccountsAllCheckedFromUnderlyingBrokerProviders);			
		}
		
		[Obsolete("if the form is hidden mark it needs to be repopulated OnActivate() and do full TreeRebuild there")]
		bool IsHiddenHandlingRepopulate() {
			if (this.isHiddenPrevState != base.IsHidden) {
				//if (base.IsHidden == false) this.executionTree.RebuildTree();
				if (base.IsHidden == false) this.ExecutionTreeControl.RebuildAllTreeFocusOnTopmost();
				this.isHiddenPrevState = base.IsHidden;
			}
			return base.IsHidden;
		}
		public void orderProcessor_OrderAdded(object sender, OrderEventArgs e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OrderAdded(sender, e); });
				return;
			}
			//if (e.Order.State == OrderState.AutoSubmitNotEnabled) return;
			if (this.IsHiddenHandlingRepopulate()) return;
			//if (this.executionTree.SelectedAccountNumbers.Contains(e.Order.Alert.AccountNumber) == false) return;
			this.ExecutionTreeControl.OrderInsertToListView(e.Order);
			if (this.ExecutionTreeControl.DataSnapshot.ToggleSingleClickSyncWithChart) {
				this.executionTree_OnOrderDoubleClickedChartFormNotification(sender, e);
			}
		}
		void orderProcessor_OrderMessageAdded(object sender, OrderStateMessageEventArgs e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				if (base.IsDisposed) return;
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OrderMessageAdded(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate()) return;
			//this.executionTree.OrderInsertMessage(e.OrderStateMessage);
			//this.executionTree.PopulateMessagesFromSelectedOrder(e.OrderStateMessage.Order);
			this.ExecutionTreeControl.PopulateMessagesFromSelectedOrder(e.OrderStateMessage.Order);
			this.PopulateWindowText();
		}
		void orderProcessor_OrderStateChanged(object sender, OrderEventArgs e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.orderProcessor_OrderStateChanged(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate()) return;
			this.ExecutionTreeControl.OrderUpdateListItem(e.Order);
		}
		void orderProcessor_OrderRemoved(object sender, OrderEventArgs e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { orderProcessor_OrderRemoved(sender, e); });
				return;
			}
			if (this.IsHiddenHandlingRepopulate()) return;
			this.ExecutionTreeControl.OrderRemoveFromListView(e.Order);
			this.PopulateWindowText();
		}

		public void PopulateWindowText() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.PopulateWindowText));
				return;
			}
			string ret = "";
			int itemsCnt			= this.ExecutionTreeControl.OrdersTree.Items.Count;
			int allCnt				= this.orderProcessor.DataSnapshot.OrdersAll.InnerOrderList.Count;
			int submittingCnt		= this.orderProcessor.DataSnapshot.OrdersSubmitting.InnerOrderList.Count;
			int pendingCnt			= this.orderProcessor.DataSnapshot.OrdersPending.InnerOrderList.Count;
			int pendingFailedCnt	= this.orderProcessor.DataSnapshot.OrdersPendingFailed.InnerOrderList.Count;
			int cemeteryHealtyCnt	= this.orderProcessor.DataSnapshot.OrdersCemeteryHealthy.InnerOrderList.Count;
			int cemeterySickCnt		= this.orderProcessor.DataSnapshot.OrdersCemeterySick.InnerOrderList.Count;
			int fugitive			= allCnt - (submittingCnt + pendingCnt + pendingFailedCnt + cemeteryHealtyCnt + cemeterySickCnt);

										ret +=		   cemeteryHealtyCnt + " Filled/Killed/Killers";
										ret += " | " + pendingCnt + " Pending";
			if (submittingCnt > 0)		ret += " | " + submittingCnt + " Submitting";
			if (pendingFailedCnt > 0)	ret += " | " + pendingFailedCnt + " PendingFailed";
			if (cemeterySickCnt > 0)	ret += " | " + cemeterySickCnt + " DeadFromSickness";
										ret += " :: "+ allCnt + " Total";
			if (itemsCnt != allCnt)		ret += " | " + itemsCnt + " Displayed";
			if (fugitive > 0)			ret += ", " + fugitive + " DeserializedPrevLaunch";

			this.Text = "Execution :: " + ret;
		}

		void executionTree_OnOrderDoubleClickedChartFormNotification(object sender, OrderEventArgs e) {
			try {
				ChartShadow chartFound = Assembler.InstanceInitialized.AlertsForChart.FindContainerFor(e.Order.Alert);
				//chartFound.SelectAlert(e.Order.Alert);
				chartFound.SelectPosition(e.Order.Alert.PositionAffected);
			} catch (Exception ex) {
				//string msg = "TODO: add chartManager to Assembler, tunnel Execution.DoubleClick => select Chart.Trade; orderDoubleClicked[" + e.Order + "]";
				string msg = "HISTORICAL_ORDER_ISNT_LINKED_TO_ANY_CHART__CANT_POPUP_POSITION orderDoubleClicked[" + e.Order + "]";
				Assembler.PopupException(msg, ex, false);
			}
		}

		void ExecutionForm_Load(object sender, EventArgs e) {
			this.orderProcessor.EventDistributor.OnOrderAddedExecutionFormNotification += this.orderProcessor_OrderAdded;
			this.orderProcessor.EventDistributor.OnOrderRemovedExecutionFormNotification += this.orderProcessor_OrderRemoved;
			this.orderProcessor.EventDistributor.OnOrderStateChangedExecutionFormNotification += this.orderProcessor_OrderStateChanged;
			this.orderProcessor.EventDistributor.OnOrderMessageAddedExecutionFormNotification += this.orderProcessor_OrderMessageAdded;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification += this.orderProcessor_OrderAdded;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderRemovedExecutionFormNotification += this.orderProcessor_OrderRemoved;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderStateChangedExecutionFormNotification += this.orderProcessor_OrderStateChanged;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderMessageAddedExecutionFormNotification += this.orderProcessor_OrderMessageAdded;

			this.ExecutionTreeControl.OnOrderStatsChangedRecalculateWindowTitleExecutionFormNotification += delegate { this.PopulateWindowText(); };
			this.ExecutionTreeControl.OnOrderDoubleClickedChartFormNotification += this.executionTree_OnOrderDoubleClickedChartFormNotification;

			isHiddenPrevState = base.IsHidden;
			//BETTER_IN_MainForm.WorkspaceLoad(): this.ExecutionTreeControl.MoveStateColumnToLeftmost();
		}
		void ExecutionForm_Closed(object sender, FormClosedEventArgs e) {
			string msg = "ExecutionForm_Closed(): all self-hiding singletons are closed() on MainForm.Close()?";
			//ExecutionForm.Instance = null;
		}
		void ExecutionForm_Closing(object sender, FormClosingEventArgs e) {
			this.orderProcessor.EventDistributor.OnOrderAddedExecutionFormNotification -= this.orderProcessor_OrderAdded;
			this.orderProcessor.EventDistributor.OnOrderRemovedExecutionFormNotification -= this.orderProcessor_OrderRemoved;
			this.orderProcessor.EventDistributor.OnOrderStateChangedExecutionFormNotification -= this.orderProcessor_OrderStateChanged;
			this.orderProcessor.EventDistributor.OnOrderMessageAddedExecutionFormNotification -= this.orderProcessor_OrderMessageAdded;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification -= this.orderProcessor_OrderAdded;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderRemovedExecutionFormNotification -= this.orderProcessor_OrderRemoved;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderStateChangedExecutionFormNotification -= this.orderProcessor_OrderStateChanged;
//			this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderMessageAddedExecutionFormNotification -= this.orderProcessor_OrderMessageAdded;
			string msg = "ExecutionForm_Closed(): unsubscribed from orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification";
			Assembler.PopupException(msg);
		}

	}
}
