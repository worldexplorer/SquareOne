using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void olvOrdersTree_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				this.DataSnapshot.FirstRowShouldStaySelected = (this.OlvOrdersTree.SelectedIndex == 0) ? true : false;
				int selectedIndex = this.OlvOrdersTree.SelectedIndex;
				if (selectedIndex == -1) {
					// PRESSING_DEL_KEY_DOESNT_TREAT_LOST_SELECTION_AS_CHANGED??? this.olvMessages.Clear();
					return;		// when selection changes, old selected is unselected; we got here twice on every click
				}
				
				//this.OrdersTreeOLV.RedrawItems(selectedIndex, selectedIndex, true);
				this.OlvOrdersTree.RefreshSelectedObjects();
				this.populateMessagesFor(this.OlvOrdersTree.SelectedObject as Order);
				
				/*bool removeEmergencyLockEnabled = false;
				foreach (Order selectedOrder in this.OrdersSelected) {
					if (selectedOrder.StateChangeableToSubmitted) submitEnabled = true;
					//Order reason4lock = Assembler.Constructed.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					//if (reason4lock != null) removeEmergencyLockEnabled = true;
					break;
				}
				this.mniStopEmergencyClose.Enabled = removeEmergencyLockEnabled;*/
				
				if (this.DataSnapshot.SingleClickSyncWithChart) {
					//v1 this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTree.SelectedObject as Order);
					this.olvOrdersTree_DoubleClick(this, null);
				}
			} catch (Exception ex) {
				Assembler.PopupException("ordersTree_SelectedIndexChanged()", ex);
			}
		}
		void ctxColumnsGrouped_ItemClicked(object sender, ToolStripItemClickedEventArgs e)	{
			try {
				// F4.CheckOnClick=False because mni stays unchecked after I just checked
				ToolStripMenuItem mni = e.ClickedItem as ToolStripMenuItem;
				if (mni == null) {
					string msg = "You clicked on something not being a ToolStripMenuItem";
					Assembler.PopupException(msg);
					return;
				}
				mni.Checked = !mni.Checked;
				if (columnsByFilter.ContainsKey(mni) == false) {
					string msg = "Add ToolStripMenuItem[" + mni.Name + "] into columnsByFilter";
					Assembler.PopupException(msg);
					return;
				}
				bool newCheckedState = mni.Checked;
				// F4.CheckOnClick=true mni.Checked = newState;
	//			this.settingsManager.Set("ExecutionForm." + mni.Name + ".Checked", mni.Checked);
	//			this.settingsManager.SaveSettings();

				List<OLVColumn> columns = columnsByFilter[mni];
				if (columns.Count == 0) return;

				foreach (OLVColumn column in columns) {
					column.IsVisible = newCheckedState;
				}
				this.OlvOrdersTree.RebuildColumns();
			} catch (Exception ex) {
				Assembler.PopupException(" //ctxColumnsGrouped_ItemClicked", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxColumnsGrouped.Show();
			}
		}
		void mniToggleBrokerTime_Click(object sender, EventArgs e) {
			try {
				// F4.CheckOnClick=True this.mniBrokerTime.Checked = !this.mniBrokerTime.Checked; 
				this.DataSnapshot.ShowBrokerTime = this.mniToggleBrokerTime.Checked;
				this.DataSnapshotSerializer.Serialize();
				this.RebuildAllTree_focusOnTopmost();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleBrokerTime_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void mniToggleSyncWithChart_Click(object sender, EventArgs e) {
			try {
				this.DataSnapshot.SingleClickSyncWithChart = this.mniToggleSyncWithChart.Checked;
				this.DataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleSyncWithChart_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void mniToggleMessagesPane_Click(object sender, EventArgs e) {
			try {
				this.splitContainerMessagePane.Panel2Collapsed = !this.mniToggleMessagesPane.Checked;
				this.DataSnapshot.ShowMessagesPane = this.mniToggleMessagesPane.Checked;
				this.DataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleMessagesPane_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void mniToggleMessagesPaneSplitHorizontally_Click(object sender, EventArgs e) {
			Orientation newOrientation = this.mniToggleMessagesPaneSplitHorizontally.Checked
					? Orientation.Horizontal : Orientation.Vertical;
			this.splitContainerMessagePane.Orientation = newOrientation;
			this.DataSnapshot.ShowMessagePaneSplittedHorizontally = this.mniToggleMessagesPaneSplitHorizontally.Checked;
			this.DataSnapshotSerializer.Serialize();
		}		
		void mniToggleCompletedOrders_Click(object sender, EventArgs e) {
			try {
				// do something with filters
				this.RebuildAllTree_focusOnTopmost();
				this.DataSnapshot.ShowCompletedOrders = this.mniToggleCompletedOrders.Checked;
				this.DataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleCompletedOrders_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void ctxAccounts_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			try {
				Assembler.PopupException("NYI");
			} catch (Exception ex) {
				Assembler.PopupException(" //ctxAccounts_ItemClicked", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxAccounts.Show();
			}
		}
		void mniEmergencyLockRemove_Click(object sender, EventArgs e) {
			try {
				foreach (Order selectedOrder in this.OrdersSelected) {
					Order reason4lock = Assembler.InstanceInitialized.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					if (reason4lock != null) {
						Assembler.InstanceInitialized.OrderProcessor.OPPemergency.RemoveEmergencyLock_userInterrupted(reason4lock);
						this.mniStopEmergencyClose.Enabled = false;
						//ListViewItem lvi = findListviewItemForOrder(reason4lock);
						//lvi.Selected = true;
					}
					break;
				}
			} catch (Exception ex) {
				Assembler.PopupException(" //mniEmergencyLockRemove_Click", ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniOrdersRemoveSelected_Click(object sender, EventArgs e) {
			try {
				List<Order> ordersNonPending = new List<Order>();
				foreach (Order eachNonPending in this.OrdersSelected) {
					if (eachNonPending.InState_expectingBrokerCallback || eachNonPending.InState_emergency) continue;
					ordersNonPending.Add(eachNonPending);
				}
				if (ordersNonPending.Count == 0) return;
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersRemove(ordersNonPending);
				this.RebuildAllTree_focusOnTopmost();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrdersRemoveSelected_Click", ex);
			} finally {
				//this.ctxOrder.Show();
			}
		}
		void olvOrdersTree_KeyDown(object sender, KeyEventArgs e) {
			// .Del is already assigned to mniRemoveSelectedPending in .Designer.cs
			//if (e.KeyCode == Keys.Delete) {
			//    //this.btnRemoveSelected.PerformClick();
			//    this.mniOrdersRemoveCompleted_Click(this, null);
			//}
		}
		void mniOrdersRemoveCompleted_Click(object sender, EventArgs e) {
			try {
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot
					.OrdersRemove_nonPending_forAccounts(this.SelectedAccountNumbers);
				this.RebuildAllTree_focusOnTopmost();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrdersRemoveCompleted_Click", ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniOrderReplace_Click(object sender, EventArgs e) {
			string msig = " //mniOrdersRemoveCompleted_Click";
			try {
				if (this.OlvOrdersTree.SelectedObjects.Count != 0) {
					string msg = "SELECTED_OBJECT_MUST_BE_AN_ORDER got[" + this.OlvOrdersTree.SelectedObject + "]";
					Assembler.PopupException(msg + msig, null, false);
				}

				Order order = this.OlvOrdersTree.SelectedObject as Order;
				if (order == null) {
					string msg = "SELECTED_OBJECT_MUST_BE_AN_ORDER got[" + this.OlvOrdersTree.SelectedObject + "]";
					Assembler.PopupException(msg + msig, null, false);
				}
				Assembler.InstanceInitialized.OrderProcessor.GuiClick_orderReplace(order);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniKillPendingSelected_Click(object sender, EventArgs e) {
			string msig = " //mniOrderKill_Click";
			try {
				if (this.OrdersSelected.Count == 0) return;
				Assembler.InstanceInitialized.OrderProcessor.GuiClick_killPendingSelected(this.OrdersSelected);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniKillPendingAll_Click(object sender, EventArgs e) {
			string msig = " //mniOrdersCancel_Click";
			try {
				Assembler.InstanceInitialized.OrderProcessor.GuiClick_killPendingAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniKillPendingAll_stopEmitting_Click(object sender, EventArgs e) {
			string msig = " //mniKillAllStopAutoSubmit_Click";
			try {
				Assembler.InstanceInitialized.OrderProcessor.GuiClick_killAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}


		void olvOrdersTree_Click(object sender, EventArgs e) {

		}
		void olvOrdersTree_DoubleClick(object sender, EventArgs e) {
			//if (this.mniOrderEdit.Enabled) this.mniOrderEdit_Click(sender, e);
			if (this.OlvOrdersTree.SelectedItem == null) {
				string msg = "OrdersTree.SelectedItem == null";
				Assembler.PopupException(msg);
				return;
			}
			//if (this.OlvOrdersTree.SelectedItem.ForeColor == Color.DimGray) {
			//    string msg = "I_REFUSE_TO_KILL_AN_ORDER_AFTER_APPRESTART"
			//        + " tree_FormatRow() sets Item.ForeColor=Color.DimGray when AlertsForChart.IsItemRegisteredForAnyContainer(order.Alert)==false"
			//        + " (all JSON-deserialized orders have no chart to get popped-up)";
			//    Assembler.PopupException(msg, null, false);
			//    return;
			//}
			//otherwize if you'll see REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR - dont forget to use Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, pos.ExitAlert);
			this.raiseOnOrderDoubleClicked_OrderProcessorShouldKillOrder(this, this.OlvOrdersTree.SelectedObject as Order);
		}


		void mniTreeCollapseAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeCollapseAll_Click";
			try {
				this.OlvOrdersTree.CollapseAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniTreeExpandAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeExpandAll_Click";
			try {
				this.OlvOrdersTree.ExpandAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniTreeRebuildAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeRebuildAll_Click";
			try {
				this.RebuildAllTree_focusOnTopmost();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		
		void splitContainerMessagePane_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.DataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 WHATT??? BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) {
				return;
			}
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3 NOT_UNDER_WINDOWS if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished == false) return;
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

		void mniltbDelaySerializationSync_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbDelaySerializationSync_UserTyped";
			try {
				int userTyped = e.IntegerUserTyped;		// makes it red if failed to parse; "an event is a passive POCO" concept is broken here
				this.DataSnapshot.SerializationInterval = userTyped;
				this.DataSnapshotSerializer.Serialize();

				SerializerLogrotatePeriodic<Order> logrotate = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders;
				logrotate.PeriodMillis = this.DataSnapshot.SerializationInterval;
				string msg = "NEW_INTERVAL_ACTIVATED SAVED_FOR_APPRESTART SerializerLogrotatePeriodic<Order>.SerializationInterval=[" + logrotate.PeriodMillis + "]";
				Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}

	}
}
