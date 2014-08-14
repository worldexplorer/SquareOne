using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void ordersTree_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				this.DataSnapshot.firstRowShouldStaySelected = (this.OrdersTree.SelectedIndex == 0) ? true : false;
				int selectedIndex = this.OrdersTree.SelectedIndex;
				if (selectedIndex == -1) return;		// when selection changes, old selected is unselected; we got here twice on every click
				
				this.OrdersTree.RedrawItems(selectedIndex, selectedIndex, true);
				this.PopulateMessagesFromSelectedOrder(this.OrdersTree.SelectedObject as Order);
				
				/*bool removeEmergencyLockEnabled = false;
				foreach (Order selectedOrder in this.OrdersSelected) {
					if (selectedOrder.StateChangeableToSubmitted) submitEnabled = true;
					//Order reason4lock = Assembler.Constructed.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					//if (reason4lock != null) removeEmergencyLockEnabled = true;
					break;
				}
				this.mniStopEmergencyClose.Enabled = removeEmergencyLockEnabled;*/
				
				if (this.DataSnapshot.ToggleSyncWithChart) {
					this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTree.SelectedObject as Order);
				}
			} catch (Exception ex) {
				Assembler.PopupException("ordersTree_SelectedIndexChanged()", ex);
			}
		}
		void ctxColumns_ItemClicked(object sender, ToolStripItemClickedEventArgs e)	{
			// F4.CheckOnClick=False because mni stays unchecked after I just checked
			ToolStripMenuItem mni = e.ClickedItem as ToolStripMenuItem;
			if (mni == null) {
				string msg = "You clicked on something not being a ToolStripMenuItem";
				Assembler.PopupException(msg);
				return;
			}
			mni.Checked = !mni.Checked;
			if (columnsByFilters.ContainsKey(mni) == false) {
				string msg = "Add ToolStripMenuItem[" + mni.Name + "] into columnsByFilters";
				Assembler.PopupException(msg);
				return;
			}
			bool newCheckedState = mni.Checked;
			// F4.CheckOnClick=true mni.Checked = newState;
//			this.settingsManager.Set("ExecutionForm." + mni.Name + ".Checked", mni.Checked);
//			this.settingsManager.SaveSettings();

			List<OLVColumn> columns = columnsByFilters[mni];
			if (columns.Count == 0) return;

			foreach (OLVColumn column in columns) {
				column.IsVisible = newCheckedState;
			}
			this.OrdersTree.RebuildColumns();
		}
		void mniToggleBrokerTime_Click(object sender, EventArgs e) {
			// F4.CheckOnClick=True this.mniBrokerTime.Checked = !this.mniBrokerTime.Checked; 
			this.DataSnapshot.ToggleBrokerTime = this.mniToggleBrokerTime.Checked;
			this.DataSnapshotSerializer.Serialize();
			this.RebuildAllTreeFocusOnTopmost();
		}
		void mniToggleSyncWithChart_Click(object sender, EventArgs e) {
			this.DataSnapshot.ToggleSyncWithChart = this.mniToggleSyncWithChart.Checked;
			this.DataSnapshotSerializer.Serialize();
		}
		void mniToggleMessagesPane_Click(object sender, EventArgs e) {
			this.splitContainerMessagePane.Panel2Collapsed = !this.mniToggleMessagesPane.Checked;
			this.DataSnapshot.ToggleMessagesPane = this.mniToggleMessagesPane.Checked;
			this.DataSnapshotSerializer.Serialize();
		}
		void mniToggleMessagesPaneSplitHorizontally_Click(object sender, EventArgs e) {
			Orientation newOrientation = this.mniToggleMessagesPaneSplitHorizontally.Checked
					? Orientation.Horizontal : Orientation.Vertical;
			this.splitContainerMessagePane.Orientation = newOrientation;
			this.DataSnapshot.ToggleMessagePaneSplittedHorizontally = this.mniToggleMessagesPaneSplitHorizontally.Checked;
			this.DataSnapshotSerializer.Serialize();
		}		
		void mniToggleCompletedOrders_Click(object sender, EventArgs e) {
			// do something with filters
			this.RebuildAllTreeFocusOnTopmost();
			this.DataSnapshot.ToggleCompletedOrders = this.mniToggleCompletedOrders.Checked;
			this.DataSnapshotSerializer.Serialize();
		}
		void ctxAccounts_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			throw new Exception("NYI");
		}
		void btnRemoveEmergencyLock_Click(object sender, EventArgs e) {
			try {
				foreach (Order selectedOrder in this.OrdersSelected) {
					Order reason4lock = Assembler.InstanceInitialized.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					if (reason4lock != null) {
						Assembler.InstanceInitialized.OrderProcessor.OPPemergency.RemoveEmergencyLockUserInterrupted(reason4lock);
						this.mniStopEmergencyClose.Enabled = false;
						//ListViewItem lvi = findListviewItemForOrder(reason4lock);
						//lvi.Selected = true;
					}
					break;
				}
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}
		void btnRemoveSelected_Click(object sender, EventArgs e) {
			List<Order> orders = new List<Order>();
			foreach (Order eachNonPending in this.OrdersSelected) {
				if (eachNonPending.ExpectingCallbackFromBroker || eachNonPending.InEmergencyState) continue;
				orders.Add(eachNonPending);
			}
			if (orders.Count > 0) {
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersRemove(orders);
				this.RebuildAllTreeFocusOnTopmost();
			}
		}
		void ordersTree_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete) {
				//this.btnRemoveSelected.PerformClick();
				this.mniOrdersRemoveCompleted_Click(this, null);
			}
		}
		void mniOrdersRemoveCompleted_Click(object sender, EventArgs e) {
			//Assembler.Constructed.OrderProcessor.DataSnapshot.OrdersRemoveNonPendingForAccountNumber(this.SelectedAccountNumbers);
			//thread unsafe? just notifying instead Assembler.Constructed.OrderProcessor.DataSnapshot.Serializer.OrdersBuffered.Serialize();
//			this.populateOrders();
//			this.lvMessages.Items.Clear();
			this.RebuildAllTreeFocusOnTopmost();
		}
		void mniOrderCancelReplace_Click(object sender, EventArgs e) {
			if (this.OrdersTree.SelectedItems.Count != 1) {
				return;
			}
			ListViewItem listViewItem = this.OrdersTree.SelectedItems[0];
			Order order = (Order)listViewItem.Tag;
			Order replacement = order.DeriveReplacementOrder();
//			EditOrderForm editOrderForm = new EditOrderForm(replacement);
//			editOrderForm.Text = "Cancel/Replace Order";
//			editOrderForm.method_1(true);
//			if (editOrderForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
//				Assembler.Constructed.OrderProcessor.CancelReplaceOrder(order, replacement);
//			}
		}
		void mniOrderSubmit_Click(object sender, EventArgs e) {
			List<Order> orders = new List<Order>();
			foreach (Order current in this.OrdersSelected) {
				if (current.StateChangeableToSubmitted) {
					if (current.Alert.AccountNumber.StartsWith("Paper")) {
						//current.AlertDate = MainModule.Instance._getAuthenticationProvider().GetCurrentDateTime;
						DateTime serverTime = current.Alert.Bars.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
						current.TimeCreatedBroker = serverTime;
					}
					orders.Add(current);
				}
			}
			if (orders.Count == 0) return;
			Assembler.InstanceInitialized.OrderProcessor.SubmitEatableOrdersFromGui(orders);
		}
		void mniOrderEdit_Click(object sender, EventArgs e) {
			//DONT_USE_COUNT_IN_VIRTUAL_MODE if (this.OrdersTree.SelectedItems.Count != 1) {
			//	return;
			//}
			//Order order = (Order)this.OrdersTree.SelectedItems[0].Tag;
//			EditOrderForm editOrderForm = new EditOrderForm(order);
//			if (editOrderForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
//				this.populateListViewItemColumnsFromOrder(this.ordersTree.SelectedItems[0], order);
//				if (order.State == OrderState.Error) {
//					//tradeManager.updateOrderStatus(order.GUID, OrderStatus.Staged, DateTime.Now, 0.0, 0.0, 0, "");
//					//tradeManager.updateOrderStatusAndPropagate(order, OrderState.Staged, DateTime.Now, 0.0, 0.0, "");
//					string msg = "Error while updating Order using EditOrderForm";
//					OrderStateMessage newOrderStaged = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
//					Assembler.Constructed.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderStaged);
//					throw new Exception("CRAZY#7: how is it possible that after editing Order.State = OrderState.Error???");
//				}
//				this.ordersTree_SelectedIndexChanged(this, e);
//			}
		}
		void btnCancelSelected_Click(object sender, EventArgs e) {
			if (this.OrdersSelected.Count == 0) return;
			Assembler.InstanceInitialized.OrderProcessor.KillSelectedOrders(this.OrdersSelected);
		}
		void btnCancelAll_Click(object sender, EventArgs e) {
			//DialogResult dialogResult = MessageBox.Show(this,
			//	"This action will Cancel any Active Orders and turn Auto-Trading off - Continue?", "Confirm"
			//	, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			//if (dialogResult == DialogResult.No) return;
			//if (this.cmbAutoTrading.SelectedIndex != 0) {
			//	this.cmbAutoTrading.SelectedIndex = 0;
			//}
			Assembler.InstanceInitialized.OrderProcessor.CancelAllPending();
		}
		void mniKillAlStopAutoSubmit_Click(object sender, EventArgs e) {
			Assembler.InstanceInitialized.OrderProcessor.KillAll();
		}

		void ordersTree_DoubleClick(object sender, EventArgs e) {
			//if (this.mniOrderEdit.Enabled) this.mniOrderEdit_Click(sender, e);
			this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTree.SelectedObject as Order);
		}

//		void populateOrders() {
//			List<ListViewItem> ret = new List<ListViewItem>();
//			List<Order> ordersSafe = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersAll.SafeCopy;
//			if (ordersSafe.Count == 0) return;
//			foreach (Order order in ordersSafe) {
//				if (order.Alert == null) {
//					string msg = "ExecutionForm won't be able to display Sell/Buy if order.Alert is not deserialized properly"
//						+ "; you MUST save order.Alert with [DataMember]"
//						+ "; " + order;
//					Assembler.PopupException(msg);
//					continue;
//				}
//				if (this.SelectedAccountNumbers.Contains(order.Alert.AccountNumber) == false) {
//					string msg = "Skipping order[" + order + "].Alert.AccountNumber[" + order.Alert.AccountNumber + "]"
//						+ " is not in the list of selected accounts, select this account in ctxAccounts and invoke populateOrders() again";
//					//base.statusReporter.PopupException(msg);
//					continue;
//				}
//
//				ListViewItem lvi = new ListViewItem();
//				//this.populateListViewItemColumnsFromOrder(lvi, order);
//				lvi.Tag = order;
//				order.ListViewItemInExecutionForm = lvi;
//				//lvi.UseItemStyleForSubItems = false;
//				ret.Add(lvi);
//			}
//			if (ret.Count == 0) return;
//
//			this.OrdersTree.BeginUpdate();
//			this.OrdersTree.SuspendLayout();
//			this.SuspendLayout();
//			try {
//				this.OrdersTree.Items.Clear();
//				this.OrdersTree.Items.AddRange(ret.ToArray());
//				if (this.firstRowShouldStaySelected == true && this.OrdersTree.Items.Count > 0) {
//					this.OrdersTree.Items[0].Selected = true;
//				}
//			} catch (Exception e) {
//				Assembler.PopupException(null, e);
//			} finally {
//				this.ResumeLayout();
//				this.OrdersTree.ResumeLayout();
//				this.OrdersTree.EndUpdate();
//			}
//			this.PopulateMessagesFromSelectedOrder(null);
//			this.raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification();
//		}
		void mniExpandAllClick(object sender, EventArgs e) {
			this.OrdersTree.ExpandAll();
		}
		void mniCollapseAllClick(object sender, EventArgs e) {
			this.OrdersTree.CollapseAll();
		}
		void RebuildAllToolStripMenuItemClick(object sender, EventArgs e) {
			this.OrdersTree.RebuildAll(true);
		}		
		
		void SplitContainerMessagePane_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.DataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
				if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal == e.SplitY) return;
				this.DataSnapshot.MessagePaneSplitDistanceHorizontal = e.SplitY;
			} else {
				if (this.DataSnapshot.MessagePaneSplitDistanceVertical == e.SplitX) return;
				this.DataSnapshot.MessagePaneSplitDistanceVertical = e.SplitX;
			}
			this.DataSnapshotSerializer.Serialize();
		}

		//http://objectlistview.sourceforge.net/cs/recipes.html#how-can-i-change-the-colours-of-a-row-or-just-a-cell
		readonly Color BACKGROUND_GREEN = Color.FromArgb(230, 255, 230);
		readonly Color BACKGROUND_RED = Color.FromArgb(255, 230, 230);
		void ordersTree_FormatRow(object sender, FormatRowEventArgs e) {
			var order = e.Model as Order;
			if (order == null) {
				Assembler.PopupException("ordersTree_FormatRow(): (e.Model as Order =null");
				return;
			}
			e.Item.BackColor = (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long)
				? BACKGROUND_GREEN : BACKGROUND_RED;
		}
	}
}
