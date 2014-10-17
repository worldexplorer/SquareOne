using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;
using Sq1.Support;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		public ExecutionTreeDataSnapshot DataSnapshot;
		public Serializer<ExecutionTreeDataSnapshot> DataSnapshotSerializer;

		//Font fontNormal;
		//Font fontBold;
		public Order OrderSelected {
			get {
				if (this.OrdersTree.SelectedObjects.Count != 1) return null;
				return this.OrdersTree.SelectedObjects[0] as Order;
			}
		}
		public List<Order> OrdersSelected {
			get {
				List<Order> ret = new List<Order>();
				foreach (object obj in this.OrdersTree.SelectedObjects) ret.Add(obj as Order);
				return ret;
			}
		}
		public List<string> SelectedAccountNumbers {
			get {
				var ret = new List<string>();
				foreach (ToolStripItem mni in this.ctxAccounts.Items) {
					if (mni.Selected == false) continue;
					ret.Add(mni.Text);
				}
				return ret;
			}
		}
		Dictionary<ToolStripMenuItem, List<OLVColumn>> columnsByFilters;
		OrdersShadowTreeDerived ordersShadowTree;

		public void buildMniShortcutsAfterInitializeComponent() {
			columnsByFilters = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();
			columnsByFilters.Add(this.mniShowWhenWhat, new List<OLVColumn>() {
				this.colheBarNum,
				this.colheDatetime,
				this.colheSymbol,
				this.colheDirection,
				this.colheOrderType
				});
			columnsByFilters.Add(this.mniShowKilledReplaced, new List<OLVColumn>() {
				this.colheReplacedByGUID,
				this.colheKilledByGUID
				});
			columnsByFilters.Add(this.mniShowPrice, new List<OLVColumn>() {
				this.colhePriceScript,
				this.colheSpreadSide,
				this.colhePriceScriptRequested,
				this.colhePriceFilled
				});
			columnsByFilters.Add(this.mniShowQty, new List<OLVColumn>() {
				this.colheQtyRequested,
				this.colheQtyFilled
				});
			columnsByFilters.Add(this.mniShowExchange, new List<OLVColumn>() {
				this.colhePriceDeposited,
				this.colheSernoSession,
				this.colheSernoExchange,
				this.colheGUID,
				this.colheReplacedByGUID,
				this.colheKilledByGUID
				});
			columnsByFilters.Add(this.mniShowOrigin, new List<OLVColumn>() {
				this.colheStrategyName,
				this.colheSignalName,
				this.colheScale
				});
			columnsByFilters.Add(this.mniShowPosition, new List<OLVColumn>() { });
			columnsByFilters.Add(this.mniShowExtra, new List<OLVColumn>() {
				});
			columnsByFilters.Add(this.mniShowLastMessage, new List<OLVColumn>() {
				this.colheLastMessage
				});
		}
		public ExecutionTreeControl() {
			this.InitializeComponent();
			this.buildMniShortcutsAfterInitializeComponent();
			
			this.orderTreeListViewCustomize();
			this.messagesListViewCustomize();
			
			//this.fontNormal = this.Font;
			//this.fontBold = new Font(this.fontNormal, FontStyle.Bold);
			WindowsFormsUtils.SetDoubleBuffered(this.OrdersTree);
			WindowsFormsUtils.SetDoubleBuffered(this.lvMessages);
			WindowsFormsUtils.SetDoubleBuffered(this);
		}
		//former public void Initialize(), replaced by InitializeWithShadowTreeRebuilt();
		public void PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized() {
			this.mniToggleMessagesPaneSplitHorizontally.Checked = this.DataSnapshot.ToggleMessagePaneSplittedHorizontally;
			Orientation newOrientation = this.DataSnapshot.ToggleMessagePaneSplittedHorizontally
					? Orientation.Horizontal : Orientation.Vertical;
			this.splitContainerMessagePane.Orientation = newOrientation;
			
			this.mniToggleMessagesPane.Checked = this.DataSnapshot.ToggleMessagesPane;
			this.splitContainerMessagePane.Panel2Collapsed = !this.mniToggleMessagesPane.Checked;
			if (this.Width == 0) {
				string msg = "CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExecutionTreeControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
				Assembler.PopupException(msg);
			} else {
				if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
					if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal > 0
							&& this.splitContainerMessagePane.SplitterDistance != this.DataSnapshot.MessagePaneSplitDistanceHorizontal) {
						this.splitContainerMessagePane.SplitterDistance = this.DataSnapshot.MessagePaneSplitDistanceHorizontal;
					}
				} else {
					int newDistance = this.DataSnapshot.MessagePaneSplitDistanceVertical - this.splitContainerMessagePane.SplitterWidth;
					if (this.DataSnapshot.MessagePaneSplitDistanceVertical > 0 && this.DataSnapshot.MessagePaneSplitDistanceVertical != newDistance) {
						this.splitContainerMessagePane.SplitterDistance = newDistance;
					}
				}
			}
			//late binding prevents SplitterMoved() induced by DockContent layouting LoadAsXml()ed docked forms 
			this.splitContainerMessagePane.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerMessagePane_SplitterMoved);
			
			this.mniToggleBrokerTime.Checked = this.DataSnapshot.ToggleBrokerTime;
			this.mniToggleCompletedOrders.Checked = this.DataSnapshot.ToggleCompletedOrders;
			this.mniToggleSyncWithChart.Checked = this.DataSnapshot.ToggleSyncWithChart;
			
			this.DataSnapshot.firstRowShouldStaySelected = true;
			this.RebuildAllTreeFocusOnTopmost();
		}
		public void InitializeWithShadowTreeRebuilt(OrdersShadowTreeDerived ordersShadowTree) {
			//this.PopulateDataSnapshotInitializeSplittersAfterDockContentIsDone();
			this.ordersShadowTree = ordersShadowTree;
			//moved to PopulateDataSnapshotInitializeSplittersAfterDockContentIsDone() this.RebuildAllTreeFocusOnTopmost();

			this.DataSnapshotSerializer = new Serializer<ExecutionTreeDataSnapshot>(Assembler.InstanceInitialized.StatusReporter);
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"Sq1.Widgets.ExecutionTreeDataSnapshot.json", "Workspaces" ,
				Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			if (createdNewFile) {
				this.DataSnapshot.ToggleMessagePaneSplittedHorizontally = (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) ? true : false;
				this.DataSnapshot.MessagePaneSplitDistanceHorizontal = this.splitContainerMessagePane.SplitterDistance;
				int newDistance = this.splitContainerMessagePane.SplitterDistance - this.splitContainerMessagePane.SplitterWidth;
				this.DataSnapshot.MessagePaneSplitDistanceVertical = newDistance;
				this.DataSnapshotSerializer.Serialize();
			}
		}
		public void PopulateAccountsMenuFromBrokerProvider(ToolStripMenuItem[] ctxAccountsAllCheckedFromUnderlyingBrokerProviders) {
			this.ctxAccounts.SuspendLayout();
			this.ctxAccounts.Items.Clear();
			this.ctxAccounts.Items.AddRange(ctxAccountsAllCheckedFromUnderlyingBrokerProviders);
			this.ctxAccounts.ResumeLayout();
		}	
		public void OrderUpdateListItem(Order order) {
			this.OrdersTree.RefreshObject(order);
			// without Invalidate/Refresh, I'll see status change only after mouseover the row with updated Status... :(
			this.OrdersTree.Invalidate();
			//this.OrdersTree.Refresh();
			this.PopulateMessagesFromSelectedOrder(order);
			//this.lvOrders_SelectedIndexChanged(this.lvOrders, EventArgs.Empty);
			this.raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification(this, order);
		}
		public void OrderInsertToListView(Order order) {
			this.RebuildAllTreeFocusOnTopmost();
			this.raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification(this, order);
		}
		public void PopulateMessagesFromSelectedOrder(Order order) {
			if (this.splitContainerMessagePane.Panel2Collapsed == true) return;
			string msig = " PopulateMessagesFromSelectedOrder(" + order + ")";
			if (this.lvMessages == null) {
				string msg = "this.lvMessages=null";
				//throw new Exception(msg);
				Assembler.PopupException(msg + msig);
				return;
			}
			if (order == null) {
				string msg = "order=null || (OrdersTree.SelectedObject as Order)=null";
				//throw new Exception(msg);
				Assembler.PopupException(msg + msig);
				return;
			}
			//ConcurrentStack<OrderStateMessage> messagesSafeCopy = order.MessagesSafeCopy;
			ConcurrentQueue<OrderStateMessage> messagesSafeCopy = order.MessagesSafeCopy;
			if (messagesSafeCopy == null) {
				string msg = "order.MessagesSafeCopy=null; must be at least empty list";
				//throw new Exception(msg);
				Assembler.PopupException(msg + msig);
				return;
			}
			
			// TODO: neutralize Sort() downstack 
			this.lvMessages.SetObjects(messagesSafeCopy);
			// SetObjects() doesn't require Invalidate(), unlike RefreshObject()  
			//this.lvMessages.Invalidate();

//				//	order.Messages.Sort((x, y) => y.DateTime.CompareTo(x.DateTime));
		}
		public void OrderRemoveFromListView(Order order) {
			this.OrdersTree.RemoveObject(order);
			this.raiseOrderStatsChangedRecalculateWindowTitleExecutionFormNotification(this, order);
		}
		public void RebuildAllTreeFocusOnTopmost() {
			this.OrdersTree.SetObjects(this.ordersShadowTree);
			this.OrdersTree.RebuildAll(true);
			//foreach (var order in this.ordersShadowTree) this.OrdersTree.ToggleExpansion(order);
			this.OrdersTree.ExpandAll();
			this.populateLastOrderMessages();
		}
		public void populateLastOrderMessages() {
			if (this.DataSnapshot == null) return;
			if (this.DataSnapshot.firstRowShouldStaySelected == false) return;
			if (this.ordersShadowTree.Count == 0) return;
			var orderTopmost = this.ordersShadowTree[0];
			if (this.OrdersTree == null) return;
			this.OrdersTree.SelectObject(orderTopmost, true);
			this.PopulateMessagesFromSelectedOrder(orderTopmost);
		}
		public void RebuildOneRootNodeChildAdded(Order orderParentToRepaint) {
			this.OrdersTree.RefreshObject(orderParentToRepaint);
			// apparently, a node with a child, doesn't require RebuildAdd/Invalidate/Refresh...
			//this.OrdersTree.RebuildAll(true);
			//this.OrdersTree.Invalidate();
			this.OrdersTree.Expand(orderParentToRepaint);
		}		
	}
}