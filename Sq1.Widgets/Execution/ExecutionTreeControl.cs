using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

namespace Sq1.Widgets.Execution {

#if USE_CONTROL_IMPROVED
	public partial class ExecutionTreeControl : UserControlPeriodicFlush {
#else
	public partial class ExecutionTreeControl : UserControl {
#endif

				ExecutionTreeDataSnapshot						dataSnapshot;
				Serializer<ExecutionTreeDataSnapshot>			dataSnapshotSerializer;
				Dictionary<ToolStripMenuItem, List<OLVColumn>>	columnsByFilter;
				OrdersRootOnly									ordersRoot;
				OrderProcessor									orderProcessor_forToStringOnly;
				Order											orderSelected			{ get {
					if (this.olvOrdersTree.SelectedObjects.Count != 1) return null;
					return this.olvOrdersTree.SelectedObjects[0] as Order;
				} }
				List<Order>										ordersSelected			{ get {
				List<Order> ret = new List<Order>();
					foreach (object obj in this.olvOrdersTree.SelectedObjects) ret.Add(obj as Order);
					return ret;
				} }
				List<string>									selectedAccountNumbers	{ get {
					var ret = new List<string>();
					foreach (ToolStripItem mni in this.ctxAccounts.Items) {
						if (mni.Selected == false) continue;
						ret.Add(mni.Text);
					}
					return ret;
				} }

		public ExecutionTreeControl() {
			this.InitializeComponent();
			this.buildMniShortcuts_afterInitializeComponent();
			
			this.fontCache = new FontCache(base.Font);

			this.olvOrderTree_customize();
			this.olvMessages_customize();

			// THROWS this.olvMessages.AllColumns.AddRange(new List<OLVColumn>() {
			//	this.colheMessageDateTime,
			//	this.colheMessageState,
			//	this.colheMessageText});
			// DOESNT_BUILD this.olvMessages.RebuildColumns();	// hoping to eliminate RebuildColumns() in populateMessagesFor()
			
			WindowsFormsUtils.SetDoubleBuffered(this.olvOrdersTree);
			WindowsFormsUtils.SetDoubleBuffered(this.olvMessages);
			WindowsFormsUtils.SetDoubleBuffered(this);
		}

		//former public void Initialize(), replaced by InitializeWithShadowTreeRebuilt();
		public void PopulateDataSnapshot_initializeSplitters_ifDockContentDeserialized() {
			//IM_INVOKED_AFTER_WORKSPACE_LOAD
			//if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
			try {
				this.SuspendLayout();
				this.mniToggleMessagesPaneSplitHorizontally.Checked = this.dataSnapshot.ShowMessagePaneSplittedHorizontally;
				Orientation newOrientation = this.dataSnapshot.ShowMessagePaneSplittedHorizontally ? Orientation.Horizontal : Orientation.Vertical;			
				try {
					if (this.splitContainerMessagePane.Orientation != newOrientation) {
						this.splitContainerMessagePane.Orientation  = newOrientation;
					}
				} catch (Exception ex) {
					string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
					Assembler.PopupException(msg, ex);
				}
				
				this.mniToggleMessagesPane.Checked = this.dataSnapshot.ShowMessagesPane;
				this.splitContainerMessagePane.Panel2Collapsed = !this.mniToggleMessagesPane.Checked;
				if (this.Width == 0) {
					DockContentImproved executionForm = base.Parent as DockContentImproved;
					if (executionForm != null) {
						if (executionForm.IsCoveredOrAutoHidden) {
							string msg = "INTAO_HIDDEN_HAS_NO_WIDTH";
						}
					} else {
						string msg = "IDENTIFY_AND_WRITE_IMPLICITLY MY_PARENT_CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExecutionTreeControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
						Assembler.PopupException(msg);
					}
				} else {
					try {
						if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
							if (this.dataSnapshot.MessagePaneSplitDistanceHorizontal > 0) {
								string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
								int newDistance = this.dataSnapshot.MessagePaneSplitDistanceHorizontal;	// + 67 this.splitContainerMessagePane.SplitterWidth;
								if (this.splitContainerMessagePane.SplitterDistance != newDistance) {
									this.splitContainerMessagePane.SplitterDistance  = newDistance;
								}
							}
						} else {
							if (this.dataSnapshot.MessagePaneSplitDistanceVertical > 0) {
								string msg = "+151_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
								int newDistance = this.dataSnapshot.MessagePaneSplitDistanceVertical;		// + 151 this.splitContainerMessagePane.SplitterWidth;
								if (this.splitContainerMessagePane.SplitterDistance != newDistance) {
									this.splitContainerMessagePane.SplitterDistance  = newDistance;
								}
							}
						}
					} catch (Exception ex) {
						string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
						Assembler.PopupException(msg, ex);
					}
				}
				//late binding prevents SplitterMoved() induced by DockContent layouting LoadAsXml()ed docked forms
				//unbinding just in case, to avoid double handling in case of multiple PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized()
				this.splitContainerMessagePane.SplitterMoved -= new System.Windows.Forms.SplitterEventHandler(this.splitContainerMessagePane_SplitterMoved);
				this.splitContainerMessagePane.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerMessagePane_SplitterMoved);
			} finally {
				this.ResumeLayout(true);
			}
			
			this.mniRecentAlwaysSelected		.Checked = this.dataSnapshot.RecentAlwaysSelected;
			this.mniToggleBrokerTime			.Checked = this.dataSnapshot.ShowBrokerTime;
			this.mniToggleCompletedOrders		.Checked = this.dataSnapshot.ShowCompletedOrders;
			this.mniToggleSyncWithChart			.Checked = this.dataSnapshot.SingleClickSyncWithChart;
			this.mniToggleColorifyOrdersTree	.Checked = this.dataSnapshot.ColorifyOrderTree_positionNet;
			this.mniToggleColorifyMessages		.Checked = this.dataSnapshot.ColorifyMessages_askBrokerProvider;

			this.olvOrdersTree_customizeColors();
			this.olvMessages_customizeColors();
			
			this.RebuildAllTree_focusOnRecent();
		}
		public void InitializeWith_shadowTreeRebuilt(OrdersRootOnly ordersTree, OrderProcessor orderProcessor) {
			this.ordersRoot = ordersTree;
			this.orderProcessor_forToStringOnly = orderProcessor;
			// NOPE_DOCK_CONTENT_HASNT_BEEN_DESERIALiZED_YET_I_DONT_KNOW_IF_IM_SHOWN_OR_NOT this.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();

			this.dataSnapshotSerializer = new Serializer<ExecutionTreeDataSnapshot>();
			bool createdNewFile = this.dataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"Sq1.Widgets.Execution.ExecutionTreeDataSnapshot.json", "Workspaces" ,
				Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);
			this.dataSnapshot = this.dataSnapshotSerializer.Deserialize();
			if (createdNewFile) {
				this.dataSnapshot.ShowMessagePaneSplittedHorizontally = (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) ? true : false;
				//this.DataSnapshot.MessagePaneSplitDistanceHorizontal = this.splitContainerMessagePane.SplitterDistance;
				//int newDistance = this.splitContainerMessagePane.SplitterDistance - this.splitContainerMessagePane.SplitterWidth;
				//this.DataSnapshot.MessagePaneSplitDistanceVertical = newDistance;
				//this.DataSnapshotSerializer.Serialize();
			} else {
				//v1 prior to using this.OrdersTreeOLV.SaveState();
				//// reversing "each cell go find one criminal to imprison" game; columnsByText will avoid full scan each column while setting 6 lines later  
				//Dictionary<string, OLVColumn> columnsByText = new Dictionary<string, OLVColumn>();
				//foreach (OLVColumn col in this.OrdersTreeOLV.Columns) {
				//    if (this.DataSnapshot.ColumnsShown.ContainsKey(col.Text) == false) continue;
				//    columnsByText.Add(col.Text, col);
				//}
				//// now the game is "the cell knows the criminal" which is an easier task for cell to imprison
				//foreach (string colText in columnsByText.Keys) {
				//    if (this.DataSnapshot.ColumnsShown.ContainsKey(colText) == false) continue;
				//    bool visible = this.DataSnapshot.ColumnsShown[colText];
				//    OLVColumn col = columnsByText[colText];
				//    col.IsVisible = visible;
				//}
				//v2
				// http://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
				if (this.dataSnapshot.OrdersTreeOlvStateBase64.Length > 0) {
					byte[] olvStateBinary = ObjectListViewStateSerializer.Base64Decode(this.dataSnapshot.OrdersTreeOlvStateBase64);
					this.olvOrdersTree.RestoreState(olvStateBinary);
				}

				this.mniltbSerializationInterval.InputFieldValue = this.dataSnapshot.SerializationInterval.ToString();
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders.PeriodMillis = this.dataSnapshot.SerializationInterval;
			}

			//base.TimedTask_flushingToGui.Delay = this.dataSnapshot.TreeRefreshDelayMsec;	// may be already started?

			base.Initialize_periodicFlushing("FLUSH_EXECUTION_CONTROL",
				new Action(this.RebuildAllTree_focusOnRecent), this.dataSnapshot.FlushToGuiDelayMsec);
			//base.Timed_flushingToGui.Start();
		}
		public void PopulateMenuAccounts_fromBrokerAdapter(ToolStripMenuItem[] ctxAccountsAllCheckedFromUnderlyingBrokerAdapters) {
			this.ctxAccounts.SuspendLayout();
			this.ctxAccounts.Items.Clear();
			this.ctxAccounts.Items.AddRange(ctxAccountsAllCheckedFromUnderlyingBrokerAdapters);
			this.ctxAccounts.ResumeLayout();
		}	
		public void OlvOrdersTree_updateState_immediate(List<Order> ordersWithStateChanged) {
			if (ordersWithStateChanged.Count == 0) return;
			//this.OlvOrdersTree.RefreshObjects(orders);

			// without Invalidate/Refresh, I'll see status change only after mouseover the row with updated Status... :(
			//this.OlvOrdersTree.Invalidate();
			//this.OrdersTree.Refresh();
			//Order firstOrNull =  (orders.Count > 0) ? orders[0] : null;
			//this.SelectOrder_populateMessages(firstOrNull);
			//this.lvOrders_SelectedIndexChanged(this.lvOrders, EventArgs.Empty);

			//foreach (Order order in ordersWithStateChanged) { }
			this.olvOrdersTree.RefreshObjects(ordersWithStateChanged);		// naive??
		}

		
		public void OnOrderMessageAppended_immediate(OrderStateMessage orderStateMessage) {
			if (orderStateMessage.Order != this.orderSelected) {
				string msg = "I keep only last-inserted order => selected; closing order is in the middle of the tree; the rest are ignored; KILLERS?...";
				return;
			}
			this.populateMessagesFor(orderStateMessage.Order);
		}

		public void OnOrdersRemoved_asyncAutoFlush(List<Order> ordersRemoved) {
			base.Timed_flushingToGui.ScheduleOnce_dontPostponeIfAlreadyScheduled();
		}

		public void OnOrdersInserted_asyncAutoFlush(List<Order> ordersAdded) {
			base.Timed_flushingToGui.ScheduleOnce_dontPostponeIfAlreadyScheduled();
		}

		void populateMessagesFor(Order order_nullMeansClearMessages) {
			string msig = " populateMessagesFor(" + order_nullMeansClearMessages + ")";
			if (order_nullMeansClearMessages == null) {
				// DOESNT_CLEAR!!! this.olvMessages.Clear();
				this.olvMessages.SetObjects(null);
				//CLICKED_ORDERS_STOPPED_TO_POPULATE??? AFTER_I_CLICK_DELETE_I_WANNA_KEEP_IT_CLEAR
				this.olvMessages.RebuildColumns();	// after appRestart, olv header is missing
				return;
			}

			if (this.splitContainerMessagePane.Panel2Collapsed == true) return;

			ConcurrentStack<OrderStateMessage> orderMessages = order_nullMeansClearMessages.MessagesSafeCopy;
			this.olvMessages.SetObjects(orderMessages, true);

			bool wasEmpty = this.olvMessages.GetItemCount() == 0;
			if (wasEmpty && orderMessages.Count > 0) {
				this.olvMessages.RebuildColumns();
			}
		}
		//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush 
		//public void OlvOrdersTree_insertOrder(Order order) {
		//    string msig = " //ExecutionTreeControl.OlvOrdersTree_insertOrder()";
		//    try {
		//        //this.OrdersTreeOLV.SetObjects(this.ordersTree.SafeCopy);
		//        string msg = "DID_YOU_INSERT????";
		//        this.olvOrdersTree.Refresh();
		//        //this.selectLastOrderPopulateMessagesSafe();
		//    } catch (Exception ex) {
		//        Assembler.PopupException(msig, ex, false);
		//    }
		//}
		public void OrderRemoved_alreadyFromBothLists_rebuildOrdersTree_cleanMessagesView(List<Order> orders) {
			string msig = " //ExecutionTreeControl.OrderRemoved_alreadyFromBothLists_rebuildOrdersTree_cleanMessagesView()";
			try {
				if (orders.Count == 0) {
					string msg = "WILL_JUST_OrdersTreeOLV.RebuildAll(true)_IN_OrderRemoveFromListView()";
				} else {
					//AREADY_REMOVED_EH??? this.ordersTree.RemoveAll(orders);
					this.olvOrdersTree.RemoveObjects(orders);
				}
				this.RebuildAllTree_focusOnRecent();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		public void RebuildAllTree_focusOnRecent() {
			string msig = " //ExecutionTreeControl.RebuildAllTree_focusOnRecent()";
			try {
				base.HowLongTreeRebuilds.Restart();

				this.olvOrdersTree.SetObjects(this.ordersRoot.SafeCopy);
				//this.OrdersTreeOLV.RebuildAll();	//, true we will refocus
				this.olvOrdersTree.Refresh();
				//this.OrdersTreeOLV.RebuildColumns();
				//foreach (var order in this.ordersShadowTree) this.OrdersTree.ToggleExpansion(order);
				this.olvOrdersTree.ExpandAll();


				if (this.dataSnapshot.RecentAlwaysSelected == false) return;

				Order recentOrder_includingKillers = this.ordersRoot.OrdersAll.MostRecent_nullUnsafe;
				if (recentOrder_includingKillers == null) {
					string msg = "Initialize() with zero deserialized?? Removed all deserialized? DONT_INVOKE_ME_THEN";
					//Assembler.PopupException(msg + msig, null, false);
					//return;
					this.populateMessagesFor(null);
				}
				int indexToSelect = this.olvOrdersTree.IndexOf(recentOrder_includingKillers);
				if (indexToSelect == -1) {
					this.populateMessagesFor(null);
					return;
				}
				this.olvOrdersTree.EnsureVisible(indexToSelect);
				this.olvOrdersTree.Expand(recentOrder_includingKillers);
				this.olvOrdersTree.SelectedIndex = indexToSelect;					// will invoke ordersTree_SelectedIndexChanged() => populateMessagesFor(theSameOrderWeJustSelected)
				this.olvOrdersTree.SelectedObject = recentOrder_includingKillers;	// will invoke ordersTree_SelectedIndexChanged() => populateMessagesFor(theSameOrderWeJustSelected)
				//this.olvOrdersTree.RefreshSelectedObjects();
				this.populateMessagesFor(recentOrder_includingKillers);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			} finally {
				base.HowLongTreeRebuilds.Stop();
				this.PopulateWindowTitle();
			}
		}

		public void PopulateWindowTitle() {
			Form parentForm = this.Parent as Form;
			if (parentForm == null) {
				string msg = "all that was probably needed for messy LivesimControl having splitContainer3<splitContainer1<LivesimControl - deleted; otherwize no idea why so many nested splitters";
				Assembler.PopupException(msg);
				return;
			}
			parentForm.Text = "Execution :: " + this.ToString();
		}		
		public override string ToString() {
			string ret = "";
			// ALWAYS_SCHEDULED_AFTER_ANY_NEWCOMER_BUFFERED_OR_FLUSHED ret += this.timerFlushToGui_noNewcomersWithinDelay.Scheduled ? "BUFFERING " : "";
			// ALREADY_PRINTED_2_LINES_LATER ret += this.exceptions_notFlushedYet.Count ? "BUFFERING " : "";

			ret += this.orderProcessor_forToStringOnly.ToString();

			int itemsCnt = this.olvOrdersTree.Items.Count;
			ret += "   " + itemsCnt.ToString("000");
			//if (this.exceptions_notFlushedYet.Count > 0)
			ret += "/" 
				+ "000" //+ this.exceptions_notFlushedYet.Count.ToString("000")
				+ "buffered";
			ret += base.FlushingStats;
			return ret;
		}
	}
}