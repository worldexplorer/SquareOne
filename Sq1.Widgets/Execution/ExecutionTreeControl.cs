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
	public partial class ExecutionTreeControl {
		public	ExecutionTreeDataSnapshot						DataSnapshot;
		public	Serializer<ExecutionTreeDataSnapshot>			DataSnapshotSerializer;
				Dictionary<ToolStripMenuItem, List<OLVColumn>>	columnsByFilter;
				OrdersAutoTree									ordersTree;
		public	Order											OrderSelected			{ get {
				if (this.OlvOrdersTree.SelectedObjects.Count != 1) return null;
				return this.OlvOrdersTree.SelectedObjects[0] as Order;
			} }
		public	List<Order>										OrdersSelected			{ get {
				List<Order> ret = new List<Order>();
				foreach (object obj in this.OlvOrdersTree.SelectedObjects) ret.Add(obj as Order);
				return ret;
			} }
		public	List<string>									SelectedAccountNumbers	{ get {
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
			
			this.OlvOrderTree_customize();
			this.messagesListView_customize();

			// THROWS this.olvMessages.AllColumns.AddRange(new List<OLVColumn>() {
			//	this.colheMessageDateTime,
			//	this.colheMessageState,
			//	this.colheMessageText});
			// DOESNT_BUILD this.olvMessages.RebuildColumns();	// hoping to eliminate RebuildColumns() in populateMessagesFor()
			
			WindowsFormsUtils.SetDoubleBuffered(this.OlvOrdersTree);
			WindowsFormsUtils.SetDoubleBuffered(this.olvMessages);
			WindowsFormsUtils.SetDoubleBuffered(this);

			this.fontCache = new FontCache(this.Font);
		}
		void buildMniShortcuts_afterInitializeComponent() {
			columnsByFilter = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();
			columnsByFilter.Add(this.mniShowWhenWhat, new List<OLVColumn>() {
				this.olvcBarNum,
				this.olvcOrderCreated,
				this.olvcSymbol,
				this.olvcDirection,
				this.olvcOrderType
				});
			columnsByFilter.Add(this.mniShowKilledReplaced, new List<OLVColumn>() {
				this.olvcReplacedByGUID,
				this.olvcKilledByGUID
				});
			columnsByFilter.Add(this.mniShowPrice, new List<OLVColumn>() {
				this.olvcSpreadSide,
				this.olvcPriceScript,
				this.olvcPriceCurBidOrAsk,
				this.olvcSlippageApplied,
				this.olvcPriceRequested_withSlippageApplied,
				this.olvcPriceFilled,
				this.olvcSlippageFilledMinusApplied,
				this.olvcPriceDeposited_DollarForPoint
				});
			columnsByFilter.Add(this.mniShowQty, new List<OLVColumn>() {
				this.olvcQtyRequested,
				this.olvcQtyFilled
				});
			columnsByFilter.Add(this.mniShowExchange, new List<OLVColumn>() {
				this.olvcSernoSession,
				this.olvcSernoExchange,
				this.olvcGUID,
				this.olvcReplacedByGUID,
				this.olvcKilledByGUID
				});
			columnsByFilter.Add(this.mniShowOrigin, new List<OLVColumn>() {
				this.olvcBrokerAdapterName,
				this.olvcDataSourceName,
				this.olvcStrategyName,
				this.olvcSignalName,
				this.olvcScale
				});
			columnsByFilter.Add(this.mniShowPosition, new List<OLVColumn>() { });
			columnsByFilter.Add(this.mniShowExtra, new List<OLVColumn>() {
				});
			columnsByFilter.Add(this.mniShowLastMessage, new List<OLVColumn>() {
				this.olvcLastMessage
				});
		}
		//former public void Initialize(), replaced by InitializeWithShadowTreeRebuilt();
		public void PopulateDataSnapshot_initializeSplitters_ifDockContentDeserialized() {
			//IM_INVOKED_AFTER_WORKSPACE_LOAD
			//if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
			try {
				this.SuspendLayout();
				this.mniToggleMessagesPaneSplitHorizontally.Checked = this.DataSnapshot.ShowMessagePaneSplittedHorizontally;
				Orientation newOrientation = this.DataSnapshot.ShowMessagePaneSplittedHorizontally ? Orientation.Horizontal : Orientation.Vertical;			
				try {
					if (this.splitContainerMessagePane.Orientation != newOrientation) {
						this.splitContainerMessagePane.Orientation =  newOrientation;
					}
				} catch (Exception ex) {
					string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
					Assembler.PopupException(msg, ex);
				}
				
				this.mniToggleMessagesPane.Checked = this.DataSnapshot.ShowMessagesPane;
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
							if (this.DataSnapshot.MessagePaneSplitDistanceHorizontal > 0) {
								string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
								int newDistance = this.DataSnapshot.MessagePaneSplitDistanceHorizontal;	// + 67 this.splitContainerMessagePane.SplitterWidth;
				//Debugger.Break();
								if (this.splitContainerMessagePane.SplitterDistance != newDistance) {
									this.splitContainerMessagePane.SplitterDistance =  newDistance;
								}
							}
						} else {
							if (this.DataSnapshot.MessagePaneSplitDistanceVertical > 0) {
								string msg = "+151_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
								int newDistance = this.DataSnapshot.MessagePaneSplitDistanceVertical;		// + 151 this.splitContainerMessagePane.SplitterWidth;
				//Debugger.Break();
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
			} finally {
				this.ResumeLayout(true);
			}
			
			this.mniToggleBrokerTime		.Checked = this.DataSnapshot.ShowBrokerTime;
			this.mniToggleCompletedOrders	.Checked = this.DataSnapshot.ShowCompletedOrders;
			this.mniToggleSyncWithChart		.Checked = this.DataSnapshot.SingleClickSyncWithChart;
			
			this.DataSnapshot.FirstRowShouldStaySelected = true;
			this.RebuildAllTree_focusOnTopmost();
		}
		public void InitializeWith_shadowTreeRebuilt(OrdersAutoTree ordersTree) {
			this.ordersTree = ordersTree;
			// NOPE_DOCK_CONTENT_HASNT_BEEN_DESERIALiZED_YET_I_DONT_KNOW_IF_IM_SHOWN_OR_NOT this.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();

			this.DataSnapshotSerializer = new Serializer<ExecutionTreeDataSnapshot>();
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"Sq1.Widgets.Execution.ExecutionTreeDataSnapshot.json", "Workspaces" ,
				Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			if (createdNewFile) {
				this.DataSnapshot.ShowMessagePaneSplittedHorizontally = (this.splitContainerMessagePane.Orientation == Orientation.Horizontal) ? true : false;
				//this.DataSnapshot.MessagePaneSplitDistanceHorizontal = this.splitContainerMessagePane.SplitterDistance;
				//int newDistance = this.splitContainerMessagePane.SplitterDistance - this.splitContainerMessagePane.SplitterWidth;
				//this.DataSnapshot.MessagePaneSplitDistanceVertical = newDistance;
				//this.DataSnapshotSerializer.Serialize();
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
					this.OlvOrdersTree.RestoreState(olvStateBinary);
				}

				this.mniltbSerializationInterval.InputFieldValue = this.DataSnapshot.SerializationInterval.ToString();
				Assembler.InstanceInitialized.OrderProcessor.DataSnapshot.SerializerLogrotateOrders.PeriodMillis = this.DataSnapshot.SerializationInterval;
			}
		}
		public void PopulateMenuAccounts_fromBrokerAdapter(ToolStripMenuItem[] ctxAccountsAllCheckedFromUnderlyingBrokerAdapters) {
			this.ctxAccounts.SuspendLayout();
			this.ctxAccounts.Items.Clear();
			this.ctxAccounts.Items.AddRange(ctxAccountsAllCheckedFromUnderlyingBrokerAdapters);
			this.ctxAccounts.ResumeLayout();
		}	
		public void OlvOrdersTree_updateState_forOrders(List<Order> orders) {
			if (orders.Count == 0) return;
			foreach (Order order in orders) {
				this.OlvOrdersTree.RefreshObject(order);
			}
			// without Invalidate/Refresh, I'll see status change only after mouseover the row with updated Status... :(
			this.OlvOrdersTree.Invalidate();
			//this.OrdersTree.Refresh();
			Order firstOrNull =  (orders.Count > 0) ? orders[0] : null;
			this.SelectOrder_populateMessages(firstOrNull);
			//this.lvOrders_SelectedIndexChanged(this.lvOrders, EventArgs.Empty);
		}
		public void SelectOrder_populateMessages(Order order_nullMeansClearMessages) {
			bool firstRowShouldStaySelected = (this.DataSnapshot != null) ? this.DataSnapshot.FirstRowShouldStaySelected : true;
			if (firstRowShouldStaySelected) {
				//THROWS REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR this.OrdersTree.SelectObject(orderTopmost, true);
				//THROWS REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR this.OrdersTree.SelectedIndex = 0;
				// as far as I remember, this doesn't work: this.OrdersTreeOLV.SelectedItem = orderNullMeansClear;
				int indexToSelect = this.OlvOrdersTree.IndexOf(order_nullMeansClearMessages);
				if (indexToSelect == -1) {
					this.populateMessagesFor(null);
					return;
				}
				this.OlvOrdersTree.EnsureVisible(indexToSelect);
				this.OlvOrdersTree.Expand(order_nullMeansClearMessages);
				this.OlvOrdersTree.SelectedIndex = indexToSelect;
				this.OlvOrdersTree.RefreshSelectedObjects();
				// SelectedIndex=X above will invoke ordersTree_SelectedIndexChanged() => populateMessagesFor(theSameOrderWeJustSelected) 
			}
			this.populateMessagesFor(order_nullMeansClearMessages);
		}
		void populateMessagesFor(Order order_nullMeansClearMessages) {
			string msig = " populateMessagesFor(" + order_nullMeansClearMessages + ")";
			if (order_nullMeansClearMessages == null) {
				this.olvMessages.Clear();
				this.olvMessages.RebuildColumns();	// after appRestart, olv header is missing
				return;
			}

			if (this.splitContainerMessagePane.Panel2Collapsed == true) return;

			ConcurrentStack<OrderStateMessage> orderMessages = order_nullMeansClearMessages.MessagesSafeCopy;
			if (orderMessages == null) {
			    string msg = "MUST_BE_AT_LEAST_EMPTY_LIST order.MessagesSafeCopy=null";
			    //throw new Exception(msg);
			    Assembler.PopupException(msg + msig);
			    return;
			}
			if (orderMessages.Count == 0) {
			    string msg = "NO_MESSAGES_TO_POPULATE order.MessagesSafeCopy.Count==0";
			    //throw new Exception(msg);
			    Assembler.PopupException(msg + msig);
			    return;
			}
			// TODO: neutralize Sort() downstack 
			this.olvMessages.SetObjects(orderMessages, true);

			// SetObjects() doesn't require Invalidate(), unlike RefreshObject()  
			//this.lvMessages.Invalidate();
			this.olvMessages.Refresh();

			bool wasEmpty = this.olvMessages.GetItemCount() == 0;
			if (wasEmpty && orderMessages.Count > 0) {
				this.olvMessages.RebuildColumns();	// TIRED_OF_FORGETTING_THAT_SET_OBJECTS_DOESNT_REFRESH_ITSELF
			}
			//	order.Messages.Sort((x, y) => y.DateTime.CompareTo(x.DateTime));
		}
		public void OlvOrdersTree_insertOrder(Order order) {
			//if (this.OrdersTreeOLV.Items.Count == 0) {
			//	this.RebuildAllTreeFocusOnTopmost();
			//	return;
			//}
			//if (order.DerivedFrom == null) {
			//	// copypaste from BuildList()
			//	this.OrdersTreeOLV.BeginUpdate();
			//	try {
			//		OLVListItem lvi = new OLVListItem(order);
			//		this.OrdersTreeOLV.Items.Insert(0, lvi);
			//	} finally {
			//		this.OrdersTreeOLV.EndUpdate();
			//	}
			//	this.SelectOrderAndOrPopulateMessages(order);
			//} else {
			//	int index = this.OrdersTreeOLV.TreeModel.GetObjectIndex(order.DerivedFrom);
			//	if (index == -1) {
			//		this.RebuildAllTreeFocusOnTopmost();
			//		return;
			//	}
			//	// copypaste from BuildList()
			//	try {
			//		OLVListItem lvi = new OLVListItem(order);
			//		// when in virtual mode, use model :(
			//		this.OrdersTreeOLV.Items.Insert(index + 1, lvi);
			//	} finally {
			//		this.OrdersTreeOLV.EndUpdate();
			//	}
			//	this.SelectOrderAndOrPopulateMessages(order);
			//	this.RebuildOneRootNodeChildAdded(order.DerivedFrom);
			//}
			//v2
			try {
				//this.OrdersTreeOLV.SetObjects(this.ordersTree.SafeCopy);
				string msg = "DID_YOU_INSERT????";
				this.OlvOrdersTree.Refresh();
				//this.selectLastOrderPopulateMessagesSafe();
			} catch (Exception ex) {
				string msg = " //ExecutionTreeControl.OrderInsertToListView()";
				Assembler.PopupException(msg, ex, false);
			}
		}
		public void OrderRemoved_alreadyFromBothLists_rebuildOrdersTree_cleanMessagesView(List<Order> orders) {
			try {
				if (orders.Count == 0) {
					string msg = "WILL_JUST_OrdersTreeOLV.RebuildAll(true)_IN_OrderRemoveFromListView()";
				} else {
					//AREADY_REMOVED_EH??? this.ordersTree.RemoveAll(orders);
					this.OlvOrdersTree.RemoveObjects(orders);
				}
				//v1 this.RebuildAllTreeFocusOnTopmost();
				this.SelectOrder_populateMessages(null);
			} catch (Exception ex) {
				string msg = " //ExecutionTreeControl.OrderRemoveFromListView()";
				Assembler.PopupException(msg, ex, false);
			}
		}
		public void RebuildAllTree_focusOnTopmost() {
			try {
				this.OlvOrdersTree.SetObjects(this.ordersTree.SafeCopy);
				//this.OrdersTreeOLV.RebuildAll();	//, true we will refocus
				this.OlvOrdersTree.Refresh();
				//this.OrdersTreeOLV.RebuildColumns();
				//foreach (var order in this.ordersShadowTree) this.OrdersTree.ToggleExpansion(order);
				this.OlvOrdersTree.ExpandAll();
				this.SelectOrder_populateMessages(null);
			} catch (Exception ex) {
				string msg = " //ExecutionTreeControl.RebuildAllTreeFocusOnTopmost()";
				Assembler.PopupException(msg, ex, false);
			}
		}
		void selectLastOrder_populateMessagesSafe() {
			//NOPE I WANT TO CLEAR MESSAGES AFTER I WIPED OUT ALL THE ORDERS if (this.ordersTree.InnerOrderList.Count == 0) return;
			if (this.ordersTree.Count == 0) {
				//DONT_MIX_RESPONSIBILITIES this.OrdersTreeOLV.Clear();
				this.SelectOrder_populateMessages(null);
				return;
			}
			var orderTopmost = this.ordersTree.First_nullUnsafe;
			this.SelectOrder_populateMessages(orderTopmost);
		}
//		public void RebuildOneRootNodeChildAdded(Order orderParentToRepaint) {
//			this.OrdersTreeOLV.RefreshObject(orderParentToRepaint);
//			// apparently, a node with a child, doesn't require RebuildAdd/Invalidate/Refresh...
//			//this.OrdersTree.RebuildAll(true);
//			//this.OrdersTree.Invalidate();
//			this.OrdersTreeOLV.Expand(orderParentToRepaint);
//		}
		public void SplitterDistance_resetToSaved() {
			this.splitContainerMessagePane.SplitterDistance = 
				this.splitContainerMessagePane.Orientation == Orientation.Horizontal
				? this.DataSnapshot.MessagePaneSplitDistanceHorizontal
				: this.DataSnapshot.MessagePaneSplitDistanceVertical;
		}
	}
}