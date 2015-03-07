using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void ordersTree_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				this.DataSnapshot.firstRowShouldStaySelected = (this.OrdersTreeOLV.SelectedIndex == 0) ? true : false;
				int selectedIndex = this.OrdersTreeOLV.SelectedIndex;
				if (selectedIndex == -1) return;		// when selection changes, old selected is unselected; we got here twice on every click
				
				this.OrdersTreeOLV.RedrawItems(selectedIndex, selectedIndex, true);
				this.PopulateMessagesFromSelectedOrder(this.OrdersTreeOLV.SelectedObject as Order);
				
				/*bool removeEmergencyLockEnabled = false;
				foreach (Order selectedOrder in this.OrdersSelected) {
					if (selectedOrder.StateChangeableToSubmitted) submitEnabled = true;
					//Order reason4lock = Assembler.Constructed.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					//if (reason4lock != null) removeEmergencyLockEnabled = true;
					break;
				}
				this.mniStopEmergencyClose.Enabled = removeEmergencyLockEnabled;*/
				
				if (this.DataSnapshot.ToggleSingleClickSyncWithChart) {
					//v1 this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTree.SelectedObject as Order);
					this.ordersTree_DoubleClick(this, null);
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
			this.OrdersTreeOLV.RebuildColumns();
		}
		void mniToggleBrokerTime_Click(object sender, EventArgs e) {
			// F4.CheckOnClick=True this.mniBrokerTime.Checked = !this.mniBrokerTime.Checked; 
			this.DataSnapshot.ToggleBrokerTime = this.mniToggleBrokerTime.Checked;
			this.DataSnapshotSerializer.Serialize();
			this.RebuildAllTreeFocusOnTopmost();
		}
		void mniToggleSyncWithChart_Click(object sender, EventArgs e) {
			this.DataSnapshot.ToggleSingleClickSyncWithChart = this.mniToggleSyncWithChart.Checked;
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
		void mniEmergencyLockRemove_Click(object sender, EventArgs e) {
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
		void mniOrdersRemoveSelected_Click(object sender, EventArgs e) {
			List<Order> ordersNonPending = new List<Order>();
			foreach (Order eachNonPending in this.OrdersSelected) {
				if (eachNonPending.InStateExpectingCallbackFromBroker || eachNonPending.InStateEmergency) continue;
				ordersNonPending.Add(eachNonPending);
			}
			if (ordersNonPending.Count == 0) return;
			Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersRemove(ordersNonPending);
			this.RebuildAllTreeFocusOnTopmost();
		}
		void ordersTree_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete) {
				//this.btnRemoveSelected.PerformClick();
				this.mniOrdersRemoveCompleted_Click(this, null);
			}
		}
		void mniOrdersRemoveCompleted_Click(object sender, EventArgs e) {
			Assembler.InstanceInitialized.OrderProcessor.DataSnapshot
				.OrdersRemoveNonPendingForAccounts(this.SelectedAccountNumbers);
			this.RebuildAllTreeFocusOnTopmost();
		}
		void mniOrderReplace_Click(object sender, EventArgs e) {
			if (this.OrdersTreeOLV.SelectedItems.Count != 1) {
				return;
			}
			ListViewItem listViewItem = this.OrdersTreeOLV.SelectedItems[0];
			listViewItem.BeginEdit();
			//Order order = (Order)listViewItem.Tag;
			//Order replacement = order.DeriveReplacementOrder();
			Assembler.PopupException("NOT_IMPLEMETED");
		}
		void mniOrderKill_Click(object sender, EventArgs e) {
			if (this.OrdersSelected.Count == 0) return;
			Assembler.InstanceInitialized.OrderProcessor.KillPending(this.OrdersSelected);
		}
		void mniOrdersCancel_Click(object sender, EventArgs e) {
			//DialogResult dialogResult = MessageBox.Show(this,
			//	"This action will Cancel any Active Orders and turn Auto-Trading off - Continue?", "Confirm"
			//	, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			//if (dialogResult == DialogResult.No) return;
			//if (this.cmbAutoTrading.SelectedIndex != 0) {
			//	this.cmbAutoTrading.SelectedIndex = 0;
			//}
			Assembler.InstanceInitialized.OrderProcessor.CancelAllPending();
		}
		void mniKillAllStopAutoSubmit_Click(object sender, EventArgs e) {
			Assembler.InstanceInitialized.OrderProcessor.KillAll();
		}

		void ordersTree_DoubleClick(object sender, EventArgs e) {
			//if (this.mniOrderEdit.Enabled) this.mniOrderEdit_Click(sender, e);
			if (this.OrdersTreeOLV.SelectedItem == null) {
				string msg = "OrdersTree.SelectedItem == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.OrdersTreeOLV.SelectedItem.ForeColor == Color.DimGray) {
				string msg = "tree_FormatRow() sets Item.ForeColor=Color.DimGray when AlertsForChart.IsItemRegisteredForAnyContainer(order.Alert)==false"
					+ " (all JSON-deserialized orders have no chart to get popped-up)";
				//Debugger.Break();
				return;
			}
			//otherwize if you'll see REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR - dont forget to use Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, pos.ExitAlert);
			this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTreeOLV.SelectedObject as Order);
		}
		void mniTreeExpandAll_Click(object sender, EventArgs e) {
			this.OrdersTreeOLV.ExpandAll();
		}
		void mniTreeCollapseAll_Click(object sender, EventArgs e) {
			this.OrdersTreeOLV.CollapseAll();
		}
		void mniRebuildAll_Click(object sender, EventArgs e) {
			this.OrdersTreeOLV.RebuildAll(true);
		}		
		
		void splitContainerMessagePane_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.DataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
			//	#if DEBUG
			//	Debugger.Break();
			//	#endif
				return;
			}
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3
			if (Assembler.InstanceInitialized.SplitterEventsAreAllowedAssumingInitialInnerDockResizingFinished == false) return;
			//Debugger.Break();
			if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
				//if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal == e.SplitY) return;
				//this.DataSnapshot.MessagePaneSplitDistanceHorizontal = e.SplitY;
				if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal == this.splitContainerMessagePane.SplitterDistance) return;
					this.DataSnapshot.MessagePaneSplitDistanceHorizontal =  this.splitContainerMessagePane.SplitterDistance;
			} else {
				//if (this.DataSnapshot.MessagePaneSplitDistanceVertical == e.SplitX) return;
				//this.DataSnapshot.MessagePaneSplitDistanceVertical = e.SplitX;
				if (this.DataSnapshot.MessagePaneSplitDistanceVertical == this.splitContainerMessagePane.SplitterDistance) return;
					this.DataSnapshot.MessagePaneSplitDistanceVertical =  this.splitContainerMessagePane.SplitterDistance;
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
