using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;
using Sq1.Core.Broker;
using Sq1.Core.Support;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void olvOrdersTree_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				this.ignoreEvents_dontSerialize = true;

				//this.dataSnapshot.RecentOrderShouldStaySelected = (this.olvOrdersTree.SelectedIndex == 0) ? true : false;
				int selectedIndex = this.olvOrdersTree.SelectedIndex;
				if (selectedIndex == -1) {
					// PRESSING_DEL_KEY_DOESNT_TREAT_LOST_SELECTION_AS_CHANGED??? this.olvMessages.SetObjects(null);
					return;		// when selection changes, old selected is unselected; we got here twice on every click
				}
				
				//this.OrdersTreeOLV.RedrawItems(selectedIndex, selectedIndex, true);
				//this.olvOrdersTree.RefreshSelectedObjects();
				this.populateMessagesFor(this.olvOrdersTree.SelectedObject as Order);
				
				/*bool removeEmergencyLockEnabled = false;
				foreach (Order selectedOrder in this.OrdersSelected) {
					if (selectedOrder.StateChangeableToSubmitted) submitEnabled = true;
					//Order reason4lock = Assembler.Constructed.OrderProcessor.OPPemergency.GetReasonForLock(selectedOrder);
					//if (reason4lock != null) removeEmergencyLockEnabled = true;
					break;
				}
				this.mniStopEmergencyClose.Enabled = removeEmergencyLockEnabled;*/
				
				if (this.dataSnapshot.SingleClickSyncWithChart) {
					//v1 this.raiseOnOrderDoubleClickedChartFormNotification(this, this.OrdersTree.SelectedObject as Order);
					this.olvOrdersTree_DoubleClick(this, null);
				}
			} catch (Exception ex) {
				Assembler.PopupException("olvOrdersTree_SelectedIndexChanged()", ex);
			} finally {
				this.ignoreEvents_dontSerialize = false;
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

				List<OLVColumn> columns = columnsByFilter[mni];
				if (columns.Count == 0) return;

				foreach (OLVColumn column in columns) {
					column.IsVisible = newCheckedState;
				}
				this.olvOrdersTree.RebuildColumns();
			} catch (Exception ex) {
				Assembler.PopupException(" //ctxColumnsGrouped_ItemClicked", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxColumnsGrouped.Show();
			}
		}
		void mniRecentAlwaysSelected_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.RecentAlwaysSelected = this.mniRecentAlwaysSelected.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniRecentAlwaysSelected_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void mniToggleBrokerTime_Click(object sender, EventArgs e) {
			try {
				// F4.CheckOnClick=True this.mniBrokerTime.Checked = !this.mniBrokerTime.Checked; 
				this.dataSnapshot.ShowBrokerTime = this.mniToggleBrokerTime.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleBrokerTime_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}
		void mniToggleSyncWithChart_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.SingleClickSyncWithChart = this.mniToggleSyncWithChart.Checked;
				this.dataSnapshotSerializer.Serialize();
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
				this.dataSnapshot.ShowMessagesPane = this.mniToggleMessagesPane.Checked;
				this.dataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleMessagesPane_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}

		bool splitterDistance_ignoreDueToChangeOrientation = false;
		void mniToggleMessagesPaneSplitHorizontally_Click(object sender, EventArgs e) {
			try {
				Orientation newOrientation = this.mniToggleMessagesPaneSplitHorizontally.Checked
						? Orientation.Horizontal : Orientation.Vertical;

				this.splitterDistance_ignoreDueToChangeOrientation = true;
				this.splitContainerMessagePane.Orientation = newOrientation;
				this.splitContainerMessagePane_populate_splitterDistance_forOrientation();
				this.splitterDistance_ignoreDueToChangeOrientation = false;

				this.dataSnapshot.ShowMessagePaneSplittedHorizontally = this.mniToggleMessagesPaneSplitHorizontally.Checked;
				this.dataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleMessagesPaneSplitHorizontally_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}		
		void mniToggleCompletedOrders_Click(object sender, EventArgs e) {
			try {
				// do something with filters
				this.RebuildAllTree_focusOnRecent();
				this.dataSnapshot.ShowCompletedOrders = this.mniToggleCompletedOrders.Checked;
				this.dataSnapshotSerializer.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleCompletedOrders_Click", ex);
			} finally {
				//this.ctxOrder.Show();
				this.ctxToggles.Show();
			}
		}

		void mniToggleColorifyOrdersTree_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.ColorifyOrderTree_positionNet = this.mniToggleColorifyOrdersTree.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.olvOrdersTree_customizeColors();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleColorifyOrdersTree_Click", ex);
			} finally {
				this.ctxToggles.Show();
			}
		}

		void mniToggleColorifyMessages_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.ColorifyMessages_askBrokerProvider = this.mniToggleColorifyMessages.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.olvMessages_customizeColors();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleColorifyMessages_Click", ex);
			} finally {
				this.ctxToggles.Show();
			}
		}

		void mniToggleKillerOrders_click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.ShowKillerOrders = this.mniToggleKillerOrders.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniToggleKillerOrders_click", ex);
			} finally {
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
				foreach (Order selectedOrder in this.ordersSelected) {
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
				foreach (Order eachNonPending in this.ordersSelected) {
					if (eachNonPending.InState_expectingBrokerCallback || eachNonPending.InState_emergency) continue;
					ordersNonPending.Add(eachNonPending);
				}
				if (ordersNonPending.Count == 0) return;
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersRemoveRange_fromAllLanes(ordersNonPending);
				//this.OrderRemoved_alreadyFromBothLists_rebuildOrdersTree_cleanMessagesView();
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrdersRemoveSelected_Click", ex);
			} finally {
				//this.ctxOrder.Show();
			}
		}
		void mniDeleteAllLogrotatedJsons_Click(object sender, EventArgs e) {
			try {
				OrderProcessorDataSnapshot snap = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot;
				snap.SerializerLogrotateOrders.FindAndDelete_allLogrotatedFiles_butNotMainJson();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniDeleteAllLogrotatedJsons_Click", ex);
			} finally {
				this.ctxOrder.Show();
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
					.OrdersRemove_forAccounts_nonPending(this.selectedAccountNumbers);
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrdersRemoveCompleted_Click", ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniOrderReplace_Click(object sender, EventArgs e) {
			string msig = " //mniOrdersRemoveCompleted_Click";
			try {
				if (this.olvOrdersTree.SelectedObjects.Count != 0) {
					string msg = "SELECTED_OBJECT_MUST_BE_AN_ORDER got[" + this.olvOrdersTree.SelectedObject + "]";
					Assembler.PopupException(msg + msig, null, false);
				}

				Order order = this.olvOrdersTree.SelectedObject as Order;
				if (order == null) {
					string msg = "SELECTED_OBJECT_MUST_BE_AN_ORDER got[" + this.olvOrdersTree.SelectedObject + "]";
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
				List<Order> selectedKillable = this.OrdersSelected_killable_unfilled;
				if (selectedKillable.Count == 0) return;
				Assembler.InstanceInitialized.OrderProcessor.GuiClick_killPendingSelected(selectedKillable);
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

		void olvOrdersTree_DoubleClick(object sender, EventArgs e) {
			string msig = " //olvOrdersTree_DoubleClick()";

			//if (this.mniOrderEdit.Enabled) this.mniOrderEdit_Click(sender, e);
			if (this.olvOrdersTree.SelectedItem == null) {
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

			Order order_doubleClicked = this.olvOrdersTree.SelectedObject as Order;
			Alert alert_doubleClicked = order_doubleClicked.Alert;
			if (alert_doubleClicked.IsEntryAlert && alert_doubleClicked.PriceFilled_fromPosition > 0) {
				if (alert_doubleClicked.PositionAffected.ExitFilled_price > 0) return;
				ScriptExecutor executor  = order_doubleClicked.Alert.Strategy.Script.Executor;
				executor.Position_closeImmediately_forOrderInExecution_clickGuiThread(order_doubleClicked.Alert, msig);
			} else {
				this.raiseOnOrderDoubleClicked_OrderProcessorShouldKillOrder(this, order_doubleClicked);
			}
		}

		void mniTreeCollapseAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeCollapseAll_Click";
			try {
				this.olvOrdersTree.CollapseAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniTreeExpandAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeExpandAll_Click";
			try {
				this.olvOrdersTree.ExpandAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniTreeRebuildAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeRebuildAll_Click";
			try {
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		
		void splitContainerMessagePane_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.dataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
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
			if (this.splitterDistance_ignoreDueToChangeOrientation) return;
			if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
				//if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal == e.SplitY) return;
				//this.DataSnapshot.MessagePaneSplitDistanceHorizontal = e.SplitY;
				if (this.dataSnapshot.MessagePane_splitDistance_horizontal == this.splitContainerMessagePane.SplitterDistance) return;
					this.dataSnapshot.MessagePane_splitDistance_horizontal =  this.splitContainerMessagePane.SplitterDistance;
			} else {
				//if (this.DataSnapshot.MessagePaneSplitDistanceVertical == e.SplitX) return;
				//this.DataSnapshot.MessagePaneSplitDistanceVertical = e.SplitX;
				if (this.dataSnapshot.MessagePane_splitDistance_vertical == this.splitContainerMessagePane.SplitterDistance) return;
					this.dataSnapshot.MessagePane_splitDistance_vertical =  this.splitContainerMessagePane.SplitterDistance;
			}
			this.dataSnapshotSerializer.Serialize();
		}

		void mniltbFlushToGuiDelayMsec_UserTyped(object sender, LabeledTextBox.LabeledTextBoxUserTypedArgs e) {
			MenuItemLabeledTextBox mnilbDelay = sender as MenuItemLabeledTextBox;
			string typed = e.StringUserTyped;
			int typedMsec = this.dataSnapshot.FlushToGuiDelayMsec;
			bool parsed = Int32.TryParse(typed, out typedMsec);
			if (parsed == false) {
				mnilbDelay.InputFieldValue = this.dataSnapshot.FlushToGuiDelayMsec.ToString();
				mnilbDelay.TextRed = true;
				return;
			}
			this.dataSnapshot.FlushToGuiDelayMsec = typedMsec;
			this.dataSnapshotSerializer.Serialize();
			this.Timed_flushingToGui.DelayMillis = this.dataSnapshot.FlushToGuiDelayMsec;
			mnilbDelay.TextRed = false;
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
			this.PopulateWindowTitle();
			this.ctxOrder.Visible = true;	// keep it open
		}
		void mniltbDelaySerializationSync_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbDelaySerializationSync_UserTyped";
			try {
				int userTyped = e.UserTyped_asInteger;		// makes it red if failed to parse; "an event is a passive POCO" concept is broken here
				this.dataSnapshot.SerializationInterval_Millis = userTyped;
				this.dataSnapshotSerializer.Serialize();

				SerializerLogrotatePeriodic<Order> logrotate = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders;
				logrotate.PeriodMillis = this.dataSnapshot.SerializationInterval_Millis;
				string msg = "NEW_INTERVAL_ACTIVATED SAVED_FOR_APPRESTART SerializerLogrotatePeriodic<Order>.SerializationInterval_Millis=[" + logrotate.PeriodMillis + "]";
				Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}
		void mniltbLogrotateLargerThan_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbLogrotateLargerThan_UserTyped";
			try {
				float userTyped = e.UserTyped_asFloat;		// makes it red if failed to parse; "an event is a passive POCO" concept is broken here
				this.dataSnapshot.LogRotateSizeLimit_Mb = userTyped;
				this.dataSnapshotSerializer.Serialize();

				SerializerLogrotatePeriodic<Order> logrotator = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders;
				logrotator.LogRotateSizeLimit_Mb = this.dataSnapshot.LogRotateSizeLimit_Mb;
				string msg = "NEW_INTERVAL_ACTIVATED__SAVED SerializerLogrotatePeriodic<Order>.LogRotateSizeLimit_Mb=[" + logrotator.LogRotateSizeLimit_Mb + "]";
				Assembler.PopupException(msg, null, false);

				this.mniltbLogrotateLargerThan.InputFieldValue = logrotator.LogRotateSizeLimit_Mb.ToString();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxOrder.Show();
			}
		}


		void ctxOrder_Opening(object sender, CancelEventArgs e) {
			bool strategy_hasPendingAlerts = false;
			bool orderClicked_hasPendingAlert = false;
			string mniOrderAlert_removeFromPending_text = "Remove from PendingAlerts (NO_PENDING__FOR_DESERIALIZED_ORDER)";

			bool orderOrReplacement_hasPositionOpen = false;
			string mniOrderPositionClose_text = "Close Position (NO_POSITION__FOR_DESERIALIZED_ORDER)";

			string mniPosition_info_text = "NO_POSITION_OPEN EntryAlert notYetFilled";
			string mniExitAlert_info_text = "NO_ExitAlert_YET";

			int pendingAlertsFound_inExecutorDataSnap_forAlertClicked = 0;

			bool alertIsBoldTrue_positionIsBoldFalse = true;

			Order orderRightClicked = this.olvOrdersTree.SelectedObject as Order;
			if (orderRightClicked != null) {
				Alert alertClicked = orderRightClicked.Alert;
				if (alertClicked != null) {
					bool position_doubleClickable =
							alertClicked.IsEntryAlert &&
							alertClicked.PriceFilled_fromPosition > 0 &&
							alertClicked.PositionAffected != null &&
							alertClicked.PositionAffected.ExitFilled_price == -1.0;
					string position_doubleClickable_asString = position_doubleClickable ? "     [DoubleClick to close]" : "";
					alertIsBoldTrue_positionIsBoldFalse = position_doubleClickable ? false : true;

					mniOrderPositionClose_text				= alertClicked.ExecutionControl_PositionClose_knowHow + position_doubleClickable_asString;
					mniOrderAlert_removeFromPending_text	= alertClicked.ExecutionControl_AlertsPendingClear_knowHow;

					pendingAlertsFound_inExecutorDataSnap_forAlertClicked = alertClicked.PendingFound_inMyExecutorsDataSnap;
					strategy_hasPendingAlerts				= true;

					if (alertClicked.PositionAffected != null) {
						Position pos = alertClicked.PositionAffected;
						mniPosition_info_text = pos.ToString();

						if (pos.ExitAlert != null) {
							mniExitAlert_info_text = pos.ExitAlert.ToString();
							if (pos.IsExitFilled == false) {
								orderOrReplacement_hasPositionOpen = true;
							}
						} else {
							orderOrReplacement_hasPositionOpen = true;
						}
					}
				}
			}
			this.mniOrderAlert_removeFromPending.Enabled = strategy_hasPendingAlerts;
			this.mniOrderAlert_removeFromPending.Text	 = mniOrderAlert_removeFromPending_text;

			this.mniOrderPositionClose			.Enabled = orderOrReplacement_hasPositionOpen;
			this.mniOrderPositionClose			.Text	 = mniOrderPositionClose_text;

			this.mniKillPendingSelected			.Enabled = strategy_hasPendingAlerts;

			this.mniPosition_info				.Text = "POSITION    "		+ mniPosition_info_text;
			this.mniExitAlert_info				.Text = "ExitAlert      "	+ mniExitAlert_info_text;


			//DO_YOU_WANT_TO_STOP_EMITTING_EVERY_STRATEGY??? int pendingAllCount = this.orderProcessor_forToStringOnly.DataSnapshot.OrdersPending.Count;

			this.mniKillPendingAll				.Enabled	= pendingAlertsFound_inExecutorDataSnap_forAlertClicked > 0;
			this.mniKillPendingAll				.Text		= "Kill Pending AllForAllStrat[" + pendingAlertsFound_inExecutorDataSnap_forAlertClicked + "],  Continue Emitting";
			this.mniKillPendingAll_stopEmitting	.Enabled	= pendingAlertsFound_inExecutorDataSnap_forAlertClicked > 0;
			this.mniKillPendingAll_stopEmitting	.Text		= "Kill Pending AllForAllStrat[" + pendingAlertsFound_inExecutorDataSnap_forAlertClicked + "],  Stop Emitting - PANIC";

			
			int selectedKillable = this.OrdersSelected_killable_unfilled.Count;
			string alert_doubleClickable = selectedKillable > 0 ? "     [Double Click]" : "";

			this.mniKillPendingSelected			.Enabled	= selectedKillable > 0;
			this.mniKillPendingSelected			.Text		= "Kill Pending Selected[" + selectedKillable + "],        Continue Emitting" + alert_doubleClickable;

			SerializerLogrotatePeriodic<Order> logrotator	= Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders;
			int		logrotatedFiles_count					= logrotator.AllLogrotatedAbsFnames_butNotMainJson_scanned.Count;
			string	logrotatedFiles_size					= logrotator.AllLogrotatedSize_butNotMainJson_scanned;
			this.mniDeleteAllLogrotatedOrderJsons.Text		= "Delete All[" + logrotatedFiles_count + "] logrotated Order*.json [" + logrotatedFiles_size + "]";
			this.mniDeleteAllLogrotatedOrderJsons.Enabled	= logrotatedFiles_count > 0;


			System.Drawing.Font font_Bold		= new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			System.Drawing.Font font_Regular	= new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			if (alertIsBoldTrue_positionIsBoldFalse == true) {
				this.mniKillPendingSelected	.Font = font_Bold;
				this.mniOrderPositionClose	.Font = font_Regular;
			} else {
				this.mniKillPendingSelected	.Font = font_Regular;
				this.mniOrderPositionClose	.Font = font_Bold;
			}
		}


		void ctxOrderStates_Opening(object sender, CancelEventArgs e) {
			this.ctxOrderStates_rebuildFrom_currentOrders();
		}
		void ctxOrderStates_rebuildFrom_currentOrders() {
			string msig = " //ctxOrderStates_rebuildFrom_currentOrders()";

			List<ToolStripMenuItem> mnisToBeRemoved = new List<ToolStripMenuItem>();
			foreach (ToolStripItem tsi_canBeSeparator in this.ctxOrderStates.Items) {
				ToolStripMenuItem mniDynamic = tsi_canBeSeparator as ToolStripMenuItem;
				if (mniDynamic == null) continue;
				OrderStateDisplayed representsState = tsi_canBeSeparator.Tag as OrderStateDisplayed;
				if (representsState == null) continue;
				mniDynamic.Click -= new EventHandler(this.mniOrderState_Click);
				mnisToBeRemoved.Add(mniDynamic);
			}
			foreach (ToolStripMenuItem eachDynamic in mnisToBeRemoved) {
				this.ctxOrderStates.Items.Remove(eachDynamic);
			}

			OrdersByState ordersByState = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.OrdersByState;
			int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT;
			//List<OrderState> states_sorted = ordersByState.KeysSorted(this, msig, waitMillis);
			List<OrderStateDisplayed> states_sorted = ordersByState.Keys(this, msig, waitMillis);
			foreach (OrderStateDisplayed eachStateDisplayed in states_sorted) {
				int ordersNumber_inState = 0;
				if (ordersByState.ContainsKey(eachStateDisplayed, this, msig)) {
					ConcurrentList<Order> orders_inState = ordersByState.GetAtKey_nullUnsafe(eachStateDisplayed, this, msig);
					ordersNumber_inState = orders_inState.Count;
				}
				string ordersNumber_inState_formatted = string.Format("{0:00}", ordersNumber_inState);
				string ordersState_formatted = string.Format("{0:000}", (int)eachStateDisplayed.OrderState);
				string mniLabel = "#" + ordersNumber_inState_formatted + "    [" + ordersState_formatted + "]    " + eachStateDisplayed.OrderState.ToString();
				ToolStripMenuItem mni = new ToolStripMenuItem();
				mni.Text = mniLabel;
				mni.Tag = eachStateDisplayed;
				mni.Checked = eachStateDisplayed.Displayed;
				mni.CheckOnClick = true;
				mni.Click += new EventHandler(this.mniOrderState_Click);
				this.ctxOrderStates.Items.Add(mni);
			}
		}

		void mniOrderState_Click(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) return;
			OrderStateDisplayed osd = mni.Tag as OrderStateDisplayed;
			osd.Displayed = mni.Checked;
			this.ctxOrderStates.Show();
		}

		void mniClosePosition_Click(object sender, EventArgs e) {
			string msig = " //mniClosePosition_Click()";
			Order orderRightClicked = this.olvOrdersTree.SelectedObject as Order;
			if (orderRightClicked == null) {
				string msg = "ORDER SELECTED GOT LOST";
				Assembler.PopupException(msg);
				return;
			}

			ScriptExecutor executor = orderRightClicked.Alert.Strategy.Script.Executor;
			executor.Position_closeImmediately_forOrderInExecution_clickGuiThread(orderRightClicked.Alert, msig);
		}

		void mniRemoveFromPendingAlerts_Click(object sender, EventArgs e) {
			string msig = " //mniRemoveFromPendingAlerts_Click()";
			Order orderRightClicked = this.olvOrdersTree.SelectedObject as Order;
			if (orderRightClicked == null) {
				string msg = "ORDER SELECTED GOT LOST";
				Assembler.PopupException(msg);
				return;
			}
			Alert alertClicked = orderRightClicked.Alert;
			if (alertClicked == null) {
				string msg = "ALERT CLICKED MUST NOT BE NULL deserialized?";
				Assembler.PopupException(msg);
				return;
			}


		//void removePendingExitAlert_backtestEnded(Alert alert, string msig) {
			string msg1 = "";
			ExecutorDataSnapshot snap = alertClicked.Strategy.Script.Executor.ExecutionDataSnapshot;
			//this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			string orderState = (alertClicked.OrderFollowed_orCurrentReplacement == null) ? "alert.OrderFollowed=NULL" : alertClicked.OrderFollowed_orCurrentReplacement.State.ToString();
			if (snap.AlertsUnfilled.Contains(alertClicked, this, "RemovePendingExitAlert(WAIT)")) {
				bool removed = snap.AlertsUnfilled.Remove(alertClicked, this, "RemovePendingExitAlert(WAIT)");
				msg1 = "RighClick_REMOVED " + orderState + " Pending alert[" + alertClicked + "] ";
			} else {
				msg1 = "RighClick_CANT_BE_REMOVED " + orderState + " isn't Pending alert[" + alertClicked + "] ";
			}
			if (alertClicked.OrderFollowed_orCurrentReplacement == null) {
				Assembler.PopupException("NONSENSE alertClicked.OrderFollowed==null " + msg1);
				return;
			}
			// OrderFollowed=null when executeStrategyBacktestEntryPoint() is in the call stack
			Assembler.InstanceInitialized.OrderProcessor.AppendMessage_propagateToGui(alertClicked.OrderFollowed_orCurrentReplacement, msig + msg1);
		//}

		}


		void mniSerializeNow_Click(object sender, EventArgs e) {
			SerializerLogrotatePeriodic<Order> slo = Assembler.InstanceInitialized.OrderProcessor.DataSnapshot
				.SerializerLogrotateOrders;
			slo.HasChangesToSave = true;
			slo.Serialize();
		}

		void mniOrderStateAll_Click(object sender, EventArgs e) {
			try {
				//this.dataSnapshotSerializer.Serialize();
				//this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrderStateAll_Click", ex);
			} finally {
				this.ctxOrderStates.Show();
			}
		}

		void mniOrderStateNone_Click(object sender, EventArgs e) {
			try {
				//this.dataSnapshotSerializer.Serialize();
				//this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniOrderStateNone_Click", ex);
			} finally {
				this.ctxOrderStates.Show();
			}
		}
	}
}
