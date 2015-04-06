using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void oLVColumn_VisibilityChanged(object sender, EventArgs e) {
			OLVColumn oLVColumn = sender as OLVColumn;
			if (oLVColumn == null) return;
			
				//v1 prior to using this.OrdersTreeOLV.SaveState();
//			if (this.DataSnapshot.ColumnsShown.ContainsKey(oLVColumn.Text) == false) {
//				this.DataSnapshot.ColumnsShown.Add(oLVColumn.Text, oLVColumn.IsVisible);
//			} else {
//				this.DataSnapshot.ColumnsShown[oLVColumn.Text] = oLVColumn.IsVisible;
//			}
			byte[] olvStateBinary = this.OrdersTreeOLV.SaveState();
			this.DataSnapshot.OrdersTreeOlvStateBase64 = ObjectListViewStateSerializer.Base64Encode(olvStateBinary);

			this.DataSnapshotSerializer.Serialize();
		}

		void orderTreeListViewCustomize() {
			//v2
			// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
			List<OLVColumn> allColumns = new List<OLVColumn>();
			foreach (ColumnHeader columnHeader in this.OrdersTreeOLV.Columns) {
				OLVColumn oLVColumn = columnHeader as OLVColumn; 
				oLVColumn.VisibilityChanged += oLVColumn_VisibilityChanged;
				if (oLVColumn == null) continue;
				//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
				if (this.OrdersTreeOLV.AllColumns.Contains(oLVColumn)) continue;
				allColumns.Add(oLVColumn);
			}
			if (allColumns.Count > 0) {
				//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
				this.OrdersTreeOLV.AllColumns.AddRange(allColumns);
			}

			//	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.OrdersTreeOLV.CanExpandGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) {
					Assembler.PopupException("treeListView.CanExpandGetter: order=null");
					return false;
				}
				return order.DerivedOrders.Count > 0;
			};
			this.OrdersTreeOLV.ChildrenGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) {
					Assembler.PopupException("treeListView.ChildrenGetter: order=null");
					return null;
				}
				return order.DerivedOrders;
			};

			this.colheAccount.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheAccount.AspectGetter: order=null";
				return order.Alert.AccountNumber;
			};
			this.colheBarNum.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheBarNum.AspectGetter: order=null";
				return order.Alert.PlacedBarIndex.ToString();
			};
			this.colheOrderCreated.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheDatetime.AspectGetter: order=null";
				DateTime orderCreated;
				if (this.mniToggleBrokerTime.Checked) {
					orderCreated = (order.Alert.QuoteCreatedThisAlertServerTime != DateTime.MinValue)
						? order.Alert.QuoteCreatedThisAlertServerTime : order.TimeCreatedBroker;
				} else {
					if (order.Alert.Bars != null) {
						orderCreated = order.Alert.Bars.MarketInfo.ConvertServerTimeToLocal(order.TimeCreatedBroker);
					} else {
						orderCreated = order.TimeCreatedBroker;
					}
				}
				return orderCreated.ToString(Assembler.DateTimeFormatLong);
			};
			this.colheSymbol.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSymbol.AspectGetter: order=null";
				return order.Alert.Symbol;
			};
			this.colheDirection.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheDirection.AspectGetter: order=null";
				return order.IsKiller ? "KILLER" : order.Alert.Direction.ToString();
			};
			this.colheDirection.ImageGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheDirection.ImageGetter: order=null";
				return (int)order.Alert.Direction - 1;
			};
			this.colheOrderType.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheOrderType.AspectGetter: order=null";
				return order.IsKiller ? "" : order.Alert.MarketLimitStop.ToString();
			};
			this.colheSpreadSide.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSpreadSide.AspectGetter: order=null";
				return order.IsKiller ? "" : formatOrderPriceSpreadSide(order, this.DataSnapshot.PricingDecimalForSymbol);
			};
			this.colhePriceScript.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colhePriceScript.AspectGetter: order=null";
				return order.IsKiller ? "" : order.Alert.PriceScript.ToString("N" + this.DataSnapshot.PricingDecimalForSymbol);
			};
			this.colheSlippage.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSlippage.AspectGetter: order=null";
				return order.IsKiller ? "" : order.SlippageFill.ToString();
			};
			this.colhePriceScriptRequested.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colhePriceScriptRequested.AspectGetter: order=null";
				return order.IsKiller ? "" : order.PriceRequested.ToString("N" + this.DataSnapshot.PricingDecimalForSymbol);
			};
			this.colhePriceFilled.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colhePriceFilled.AspectGetter: order=null";
				return order.IsKiller ? "" : order.PriceFill.ToString("N" + this.DataSnapshot.PricingDecimalForSymbol);
			};
			this.colheStateTime.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheStateTime.AspectGetter: order=null";
				return order.StateUpdateLastTimeLocal.ToString(Assembler.DateTimeFormatLong);
			};
			this.colheState.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheState.AspectGetter: order=null";
				//return (order.InStateExpectingCallbackFromBroker ? "* " : "") + order.State.ToString();
				return order.State.ToString();
			};
//			this.colheState.FontGetter = delegate(object o) {
//				var order = o as Order;
//				if (order == null) {
//					Assembler.PopupException("colheState.FontGetter: order=null");
//					return null;
//				}
//				return (order.ExpectingCallbackFromBroker) ? this.fontBold : this.fontNormal;
//			};
			
			this.colhePriceDeposited.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colhePriceDeposited.AspectGetter: order=null";
				return (order.QtyFill == 0) ? "0" : order.Alert.PriceDeposited.ToString("N" + this.DataSnapshot.PricingDecimalForSymbol);
			};
			this.colheQtyRequested.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheQtyRequested.AspectGetter: order=null";
				return order.IsKiller ? "" : order.QtyRequested.ToString();
			};
			this.colheQtyFilled.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheQtyFilled.AspectGetter: order=null";
				return order.IsKiller ? "" : order.QtyFill.ToString();
			};
			this.colheSernoSession.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSernoSession.AspectGetter: order=null";
				return order.SernoSession.ToString();
			};
			this.colheSernoExchange.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSernoExchange.AspectGetter: order=null";
				return order.SernoExchange.ToString();
			};
			this.colheGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheGUID.AspectGetter: order=null";
				return order.GUID;
			};
			this.colheKilledByGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheKilledByGUID.AspectGetter: order=null";
				return order.KillerGUID;
			};
			this.colheReplacedByGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheReplacedByGUID.AspectGetter: order=null";
				return order.ReplacedByGUID;
			};
			this.colheStrategyName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheStrategyName.AspectGetter: order=null";
				return order.Alert.StrategyName;
			};
			this.colheSignalName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheSignalName.AspectGetter: order=null";
				return order.Alert.SignalName;
			};
			this.colheScale.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheScale.AspectGetter: order=null";
				return (order.Alert.BarsScaleInterval == null)
							? "Alert.BarsScaleInterval == null"
							: order.Alert.BarsScaleInterval.ToString();
			};
			this.colheLastMessage.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "colheLastMessage.AspectGetter: order=null";
				return order.LastMessage;
			};
		}
		string formatOrderPriceSpreadSide(Order order, int pricingDecimalForSymbol) {
			string ret = "";
			switch (order.SpreadSide) {
				case OrderSpreadSide.AskCrossed:
				case OrderSpreadSide.AskTidal:
					ret = order.CurrentAsk.ToString("N" + pricingDecimalForSymbol) + " " + order.SpreadSide;
					break;
				case OrderSpreadSide.BidCrossed:
				case OrderSpreadSide.BidTidal:
					ret = order.CurrentBid.ToString("N" + pricingDecimalForSymbol) + " " + order.SpreadSide;
					break;
				default:
					ret = order.SpreadSide + " bid[" + order.CurrentBid + "] ask[" + order.CurrentAsk + "]";
					break;
			}
			return ret;
		}
		void messagesListViewCustomize() {
			// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
			List<OLVColumn> allColumns = new List<OLVColumn>();
			foreach (ColumnHeader columnHeader in this.olvMessages.Columns) {
				OLVColumn oLVColumn = columnHeader as OLVColumn; 
				if (oLVColumn == null) continue;
				allColumns.Add(oLVColumn);
			}
			if (allColumns.Count > 0) {
				this.olvMessages.AllColumns.AddRange(allColumns);
			}

			this.colheMessageText.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "colheMessageText.AspectGetter: omsg=null";
				return omsg.Message;
			};
			this.colheMessageState.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "colheMessageState.AspectGetter: omsg=null";
				return omsg.State.ToString();
			};
			this.colheMessageDateTime.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "colheMessageDateTime.AspectGetter: omsg=null";
				return omsg.DateTime.ToString(Assembler.DateTimeFormatLong);
			};
		}

		FontCache fontCache;

		void tree_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e) {
			Order order = e.Model as Order;
			if (order == null) return;
			if (order.InStateExpectingCallbackFromBroker) {
				//v1 e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);
				e.Item.Font = this.fontCache.Bolden();
			}

			//v1 if (Assembler.InstanceInitialized.AlertsForChart.IsItemRegisteredForAnyContainer(order.Alert)) return;
			//v2 ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.STRATEGY=NULL,BARS=NULL
			if (order.Alert.Bars == null) e.Item.ForeColor = Color.DimGray;
			if (order.Alert.MyBrokerIsLivesim) e.Item.BackColor = Color.Gainsboro;
		}
		// WRONG WAY TO MOVE COLUMNS AROUND: AFTER I did RestoreState(), Column(3) is not State and I add State twice => exception
//		public void MoveStateColumnToLeftmost() {
//			//moving State as we drag-n-dropped it; tree will grow in second column
//			//NOT_NEEDED this.OrdersTree.BuildList();
//			//NOT_NEEDED this.RebuildAllTreeFocusOnTopmost();
//			this.OrdersTreeOLV.SetObjects(this.ordersTree.InnerOrderList);
//			//NOT_NEEDED this.OrdersTree.RebuildAll(true);
//			this.OrdersTreeOLV.Columns.RemoveAt(3);
//			this.OrdersTreeOLV.Columns.Insert(0, this.colheState);
//			//NOT_NEEDED this.OrdersTree.BuildList();
//			//NOT_NEEDED this.RebuildAllTreeFocusOnTopmost();
//		}
	}
}
