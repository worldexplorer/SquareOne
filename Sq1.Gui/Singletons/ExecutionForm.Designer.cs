using System.ComponentModel;

using Sq1.Core;

namespace Sq1.Gui.Singletons {
	public partial class ExecutionForm {
		private IContainer components;

		protected override void Dispose(bool disposing) {
			if (this.orderProcessor != null) {
				this.orderProcessor.OnOrderAdded_executionControlShouldRebuildOLV_scheduled -= this.orderProcessor_OnOrderAdded;
				this.orderProcessor.OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled -= this.orderProcessor_OnOrdersRemoved;
				this.orderProcessor.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately -= this.orderProcessor_OnOrderStateChanged;
				this.orderProcessor.OnOrderMessageAdded_executionControlShouldPopulate_scheduled -= this.orderProcessor_OnOrderMessageAdded;
//				this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification -= this.orderProcessor_OrderAdded;
//				this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderRemovedExecutionFormNotification -= this.orderProcessor_OrderRemoved;
//				this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderStateChangedExecutionFormNotification -= this.orderProcessor_OrderStateChanged;
//				this.orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderMessageAddedExecutionFormNotification -= this.orderProcessor_OrderMessageAdded;
				string msg = "ExecutionForm::Dispose(): unsubscribed from orderProcessor.DataSnapshot.OrdersTree.OrderEventDistributor.OnOrderAddedExecutionFormNotification";
				//SEEN_IN_FILE_THANK_YOU Assembler.PopupException(msg);
			} else {
				string msg = "I think you had Instance in the base static class many derived Forms referred to?";
				Assembler.PopupException(msg);
			}

			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.ExecutionTreeControl = new Sq1.Widgets.Execution.ExecutionTreeControl();
			this.SuspendLayout();
			// 
			// ExecutionTree
			// 
			this.ExecutionTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ExecutionTreeControl.Location = new System.Drawing.Point(0, 0);
			this.ExecutionTreeControl.Name = "ExecutionTree";
			this.ExecutionTreeControl.Size = new System.Drawing.Size(1276, 337);
			this.ExecutionTreeControl.TabIndex = 17;
			// 
			// ExecutionForm
			// 
			this.ClientSize = new System.Drawing.Size(1276, 337);
			this.Controls.Add(this.ExecutionTreeControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "ExecutionForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
			this.Text = "Order Execution";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.executionForm_Closing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.executionForm_Closed);
			this.Load += new System.EventHandler(this.executionForm_Load);
			this.ResumeLayout(false);
		}
		public Sq1.Widgets.Execution.ExecutionTreeControl ExecutionTreeControl;

	}
}