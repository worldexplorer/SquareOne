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
using Sq1.Support;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		public ExecutionTreeDataSnapshot				DataSnapshot;
		public Serializer<ExecutionTreeDataSnapshot>	DataSnapshotSerializer;
		Dictionary<ToolStripMenuItem, List<OLVColumn>>	columnsByFilters;
		OrdersAutoTree									ordersTree;
		public Order									OrderSelected			{ get {
				if (this.OrdersTreeOLV.SelectedObjects.Count != 1) return null;
				return this.OrdersTreeOLV.SelectedObjects[0] as Order;
			} }
		public List<Order>								OrdersSelected			{ get {
				List<Order> ret = new List<Order>();
				foreach (object obj in this.OrdersTreeOLV.SelectedObjects) ret.Add(obj as Order);
				return ret;
			} }
		public List<string>								SelectedAccountNumbers	{ get {
				var ret = new List<string>();
				foreach (ToolStripItem mni in this.ctxAccounts.Items) {
					if (mni.Selected == false) continue;
					ret.Add(mni.Text);
				}
				return ret;
			} }

		public ExecutionTreeControl() {
			this.InitializeComponent();
			this.buildMniShortcutsAfterInitializeComponent();
			
			this.orderTreeListViewCustomize();
			this.messagesListViewCustomize();
			
			WindowsFormsUtils.SetDoubleBuffered(this.OrdersTreeOLV);
			WindowsFormsUtils.SetDoubleBuffered(this.olvMessages);
			WindowsFormsUtils.SetDoubleBuffered(this);
		}
		void buildMniShortcutsAfterInitializeComponent() {
			columnsByFilters = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();
			columnsByFilters.Add(this.mniShowWhenWhat, new List<OLVColumn>() {
				this.colheBarNum,
				this.colheOrderCreated,
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
		//former public void Initialize(), replaced by InitializeWithShadowTreeRebuilt();
		public void PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized() {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				return;
			}
			
			this.mniToggleMessagesPaneSplitHorizontally.Checked = this.DataSnapshot.ToggleMessagePaneSplittedHorizontally;
			Orientation newOrientation = this.DataSnapshot.ToggleMessagePaneSplittedHorizontally ? Orientation.Horizontal : Orientation.Vertical;
			try {
				if (this.splitContainerMessagePane.Orientation != newOrientation) {
					this.splitContainerMessagePane.Orientation =  newOrientation;
				}
			} catch (Exception ex) {
				string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
				Assembler.PopupException(msg, ex);
			}
			
			this.mniToggleMessagesPane.Checked = this.DataSnapshot.ToggleMessagesPane;
			this.splitContainerMessagePane.Panel2Collapsed = !this.mniToggleMessagesPane.Checked;
			if (this.Width == 0) {
				string msg = "CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExecutionTreeControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
				Assembler.PopupException(msg);
			} else {
				try {
					if (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) {
						if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal > 0) {
							string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
							int newDistance = this.DataSnapshot.MessagePaneSplitDistanceHorizontal + 67;	//this.splitContainerMessagePane.SplitterWidth;
							if (this.splitContainerMessagePane.SplitterDistance != newDistance) {
								this.splitContainerMessagePane.SplitterDistance =  newDistance;
							}
						}
					} else {
						if (this.DataSnapshot.MessagePaneSplitDistanceVertical > 0) {
							string msg = "+151_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
							int newDistance = this.DataSnapshot.MessagePaneSplitDistanceVertical + 151;		//this.splitContainerMessagePane.SplitterWidth;
						    if (this.splitContainerMessagePane.SplitterDistance != newDistance) {
								this.splitContainerMessagePane.SplitterDistance =  newDistance;
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
			
			this.mniToggleBrokerTime.Checked = this.DataSnapshot.ToggleBrokerTime;
			this.mniToggleCompletedOrders.Checked = this.DataSnapshot.ToggleCompletedOrders;
			this.mniToggleSyncWithChart.Checked = this.DataSnapshot.ToggleSingleClickSyncWithChart;
			
			this.DataSnapshot.firstRowShouldStaySelected = true;
			this.RebuildAllTreeFocusOnTopmost();
		}
		public void InitializeWithShadowTreeRebuilt(OrdersAutoTree ordersTree) {
			this.ordersTree = ordersTree;
			// NOPE_DOCK_CONTENT_HASNT_BEEN_DESERIALiZED_YET_I_DONT_KNOW_IF_IM_SHOWN_OR_NOT this.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();

			this.DataSnapshotSerializer = new Serializer<ExecutionTreeDataSnapshot>();
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
			} else {
				//v1 prior to using this.OrdersTreeOLV.SaveState();
//				// reversing "each cell go find one criminal to imprison" game; columnsByText will avoid full scan each column while setting 6 lines later  
//				Dictionary<string, OLVColumn> columnsByText = new Dictionary<string, OLVColumn>();
//				foreach (OLVColumn col in this.OrdersTreeOLV.Columns) {
//					if (this.DataSnapshot.ColumnsShown.ContainsKey(col.Text) == false) continue;
//					columnsByText.Add(col.Text, col);
//				}
//				// now the game is "the cell knows the criminal" which is an easier task for cell to imprison
//				foreach (string colText in columnsByText.Keys) {
//					if (this.DataSnapshot.ColumnsShown.ContainsKey(colText) == false) continue;
//					bool visible = this.DataSnapshot.ColumnsShown[colText];
//					OLVColumn col = columnsByText[colText];
//					col.IsVisible = visible;
//				}
				//v2
				// http://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
				if (this.DataSnapshot.OrdersTreeOlvStateBase64.Length > 0) {
					byte[] olvStateBinary = ObjectListViewStateSerializer.Base64Decode(this.DataSnapshot.OrdersTreeOlvStateBase64);
					this.OrdersTreeOLV.RestoreState(olvStateBinary);
				}
			}
		}
		public void PopulateAccountsMenuFromBrokerProvider(ToolStripMenuItem[] ctxAccountsAllCheckedFromUnderlyingBrokerProviders) {
			this.ctxAccounts.SuspendLayout();
			this.ctxAccounts.Items.Clear();
			this.ctxAccounts.Items.AddRange(ctxAccountsAllCheckedFromUnderlyingBrokerProviders);
			this.ctxAccounts.ResumeLayout();
		}	
		public void OrderStateUpdateOLV(List<Order> orders) {
			if (orders.Count == 0) return;
			foreach (Order order in orders) {
				this.OrdersTreeOLV.RefreshObject(order);
			}
			// without Invalidate/Refresh, I'll see status change only after mouseover the row with updated Status... :(
			this.OrdersTreeOLV.Invalidate();
			//this.OrdersTree.Refresh();
			Order firstOrNull =  (orders.Count > 0) ? orders[0] : null;
			this.PopulateMessagesFromSelectedOrder(firstOrNull);
			//this.lvOrders_SelectedIndexChanged(this.lvOrders, EventArgs.Empty);
		}
		public void PopulateMessagesFromSelectedOrder(Order orderNullMeansClear) {
			if (this.splitContainerMessagePane.Panel2Collapsed == true) return;
			string msig = " PopulateMessagesFromSelectedOrder(" + orderNullMeansClear + ")";
			if (this.olvMessages == null) {
				string msg = "this.lvMessages=null";
				//throw new Exception(msg);
				Assembler.PopupException(msg + msig);
				return;
			}
			if (orderNullMeansClear == null) {
				//string msg = "order=null || (OrdersTree.SelectedObject as Order)=null";
				//throw new Exception(msg);
				//Assembler.PopupException(msg + msig);
				this.olvMessages.Clear();
				return;
			}
			ConcurrentQueue<OrderStateMessage> messagesSafeCopy = orderNullMeansClear.MessagesSafeCopy;
			if (messagesSafeCopy == null) {
				string msg = "order.MessagesSafeCopy=null; must be at least empty list";
				//throw new Exception(msg);
				Assembler.PopupException(msg + msig);
				return;
			}
			
			// TODO: neutralize Sort() downstack 
			this.olvMessages.SetObjects(messagesSafeCopy);
			// SetObjects() doesn't require Invalidate(), unlike RefreshObject()  
			//this.lvMessages.Invalidate();

//				//	order.Messages.Sort((x, y) => y.DateTime.CompareTo(x.DateTime));
		}
		public void OrderInsertToListView(Order order) {
			this.RebuildAllTreeFocusOnTopmost();
		}
		public void OrderRemoveFromListView(List<Order> orders) {
			this.ordersTree.RemoveAll(orders);
			this.OrdersTreeOLV.RemoveObjects(orders);
		}
		public void RebuildAllTreeFocusOnTopmost() {
			this.OrdersTreeOLV.SetObjects(this.ordersTree.InnerOrderList);
			this.OrdersTreeOLV.RebuildAll(true);
			//foreach (var order in this.ordersShadowTree) this.OrdersTree.ToggleExpansion(order);
			this.OrdersTreeOLV.ExpandAll();
			this.populateLastOrderMessages();
		}
		public void populateLastOrderMessages() {
			if (this.DataSnapshot == null) return;
			if (this.DataSnapshot.firstRowShouldStaySelected == false) return;
			//NOPE I WANT TO CLEAR MESSAGES AFTER I WIPED OUT ALL THE ORDERS if (this.ordersTree.InnerOrderList.Count == 0) return;
			if (this.OrdersTreeOLV == null) return;
			if (this.ordersTree.InnerOrderList.Count == 0) {
				this.OrdersTreeOLV.Clear();
				return;
			}
			var orderTopmost = this.ordersTree.InnerOrderList[0];
			//THROWS REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR this.OrdersTree.SelectObject(orderTopmost, true);
			//THROWS REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR this.OrdersTree.SelectedIndex = 0;
			this.PopulateMessagesFromSelectedOrder(orderTopmost);
		}
		public void RebuildOneRootNodeChildAdded(Order orderParentToRepaint) {
			this.OrdersTreeOLV.RefreshObject(orderParentToRepaint);
			// apparently, a node with a child, doesn't require RebuildAdd/Invalidate/Refresh...
			//this.OrdersTree.RebuildAll(true);
			//this.OrdersTree.Invalidate();
			this.OrdersTreeOLV.Expand(orderParentToRepaint);
		}		
	}
}