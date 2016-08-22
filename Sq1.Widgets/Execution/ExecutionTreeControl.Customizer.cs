using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.Support;
using Sq1.Core.Broker;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
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
				this.olvcBidAndAsk_whenEmitted,
				this.olvcBidAndAsk_whenFilled,
				this.olvcSlippageApplied,
				this.olvcPriceEmitted_withSlippageApplied,
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
		
		void olvOrdersTree_columnVisibilityChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ignoreEvents_dontSerialize) return;
			OLVColumn olvColumn = sender as OLVColumn;
			if (olvColumn == null) return;
			byte[] olvcTree_binaryState = this.olvOrdersTree.SaveState();
			string newTreeState = ObjectListViewStateSerializer.Base64Encode(olvcTree_binaryState);
			if (this.dataSnapshot.OrdersTreeOlvStateBase64 == newTreeState) return;
			this.dataSnapshot.OrdersTreeOlvStateBase64 = newTreeState;
			this.dataSnapshotSerializer.Serialize();
		}
		void olvOrdersTree_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ignoreEvents_dontSerialize) return;
			byte[] olvcTree_binaryState = this.olvOrdersTree.SaveState();
			string newTreeState = ObjectListViewStateSerializer.Base64Encode(olvcTree_binaryState);
			if (this.dataSnapshot.OrdersTreeOlvStateBase64 == newTreeState) return;
			this.dataSnapshot.OrdersTreeOlvStateBase64 = newTreeState;
			this.dataSnapshotSerializer.Serialize();
		}
		void olvOrdersTree_customize() {
			// MOVED_TO_PopulateDataSnapshot this.olvOrdersTree_customizeColors();

			// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
			List<OLVColumn> allColumns = new List<OLVColumn>();
			foreach (ColumnHeader columnHeader in this.olvOrdersTree.Columns) {
				OLVColumn olvColumn = columnHeader as OLVColumn; 
				if (olvColumn == null) continue;
				olvColumn.VisibilityChanged += this.olvOrdersTree_columnVisibilityChanged;
				//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
				if (this.olvOrdersTree.AllColumns.Contains(olvColumn)) continue;
				allColumns.Add(olvColumn);
			}
			foreach (OLVColumn eachColumn in allColumns) {
			    if (this.olvOrdersTree.AllColumns.Contains(eachColumn)) continue;
			    this.olvOrdersTree.AllColumns.Add(eachColumn);
			}
			this.olvOrdersTree.ColumnWidthChanged += new ColumnWidthChangedEventHandler(this.olvOrdersTree_ColumnWidthChanged);

			//	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.olvOrdersTree.CanExpandGetter = delegate(object o) {
				if (this.ordersViewProxy.ShowingTreeTrue_FlatFalse == false) return false;

				var order = o as Order;
				if (order == null) {
					Assembler.PopupException("treeListView.CanExpandGetter: order=null");
					return false;
				}
				if (this.dataSnapshot.ShowKillerOrders == false) {
					return order.DerivedOrders_noKillers.Count > 0;
				}
				return order.DerivedOrders.Count > 0;
			};
			this.olvOrdersTree.ChildrenGetter = delegate(object o) {
				List<Order> noChildren = new List<Order>();
				if (this.ordersViewProxy.ShowingTreeTrue_FlatFalse == false) return noChildren;

				var order = o as Order;
				if (order == null) {
					Assembler.PopupException("treeListView.ChildrenGetter: order=null");
					return noChildren;
				}
				if (this.dataSnapshot.ShowKillerOrders == false) {
					return order.DerivedOrders_noKillers;
				}
				return order.DerivedOrders;
			};

			this.olvcAccount.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcAccount.AspectGetter: order=null";
				return order.Alert.AccountNumber;
			};

			this.olvcBarNum.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcBarNum.AspectGetter: order=null";
				//v1 SORTING_GOES_WRONG__STRINGS_COMPARED return order.Alert.PlacedBarIndex.ToString();
				//v2 SORTING_GOES_RIGHT__INTEGERS_COMPARED
				return order.Alert.PlacedBarIndex;
			};
			this.olvcBarNum.AspectToStringFormat = "{0:N0}";

			this.olvcOrderCreated.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcDatetime.AspectGetter: order=null";
				DateTime orderCreated;
				if (this.mniToggleBrokerTime.Checked) {
					orderCreated = (order.Alert.QuoteCreatedThisAlertServerTime != DateTime.MinValue)
						? order.Alert.QuoteCreatedThisAlertServerTime : order.CreatedBrokerTime;
				} else {
					if (order.Alert.Bars != null) {
						orderCreated = order.Alert.Bars.MarketInfo.Convert_serverTime_toLocalTime(order.CreatedBrokerTime);
					} else {
						orderCreated = order.CreatedBrokerTime;
					}
				}
				return orderCreated.ToString(Assembler.DateTimeFormat_toMillis);
			};
			this.olvcSymbol.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSymbol.AspectGetter: order=null";
				return order.Alert.Symbol;
			};
			this.olvcDirection.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcDirection.AspectGetter: order=null";
				return order.IsKiller ? "KILLER" : order.Alert.Direction.ToString();
			};
			//this.olvcDirection.ImageGetter = delegate(object o) {
			//    var order = o as Order;
			//    if (order == null) return "olvcDirection.ImageGetter: order=null";
			//    return (int)order.Alert.Direction - 1;
			//};
			this.olvcOrderType.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcOrderType.AspectGetter: order=null";
				//v1 return order.IsKiller ? "" : order.Alert.MarketLimitStop.ToString();
				//if (order.IsKiller == false) return order.Alert.MarketLimitStop.ToString();
				//if (order.IsKiller == false) return order.Alert.MarketLimitStop_asString;	// includes "Market => Limit (BidTidal)" transformations
				if (order.IsKiller == false) return order.Alert.MarketLimitStopFinal_asString;	// this.MarketLimitStop / this.MarketOrderAs

				Order orderKiller = order;
				//v2
				bool deserializedDoesntHaveIt = order.VictimToBeKilled != null;
				bool thisKillerReplaces_LimitExpired_orStuckInSubmitted = deserializedDoesntHaveIt && string.IsNullOrEmpty(orderKiller.VictimToBeKilled.ReplacedByGUID) == false;
				//return thisKillerReplacesLimitExpired ? "LimitExpired" : "";
				//v3
				OrderStateMessage stuckInSubmitted_wasReplaced =
					orderKiller.FindFirstMessageContaining_inOrderMessages_nullUnsafe("__STUCK_IN_SUBMITTED__REPLACED_WITH[");
				string replacementScenario = stuckInSubmitted_wasReplaced != null ? "StuckInSubmitted" : "LimitExpired";
				return thisKillerReplaces_LimitExpired_orStuckInSubmitted ? replacementScenario : "";	 //"<TODO: RESTORE_VictimToBeKilled_fromGuid_onTreeBuild>";
			};
			this.olvcSpreadSide.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSpreadSide.AspectGetter: order=null";
				//v1 return order.IsKiller ? "" : formatOrderPriceSpreadSide(order, this.dataSnapshot.PricingDecimalForSymbol);
				if (order.IsKiller == false) {
					switch (order.Alert.MarketLimitStop) {
						case MarketLimitStop.Market:
							if (order.Alert.QuoteCurrent_whenThisAlertFilled != null) {
								return order.Alert.QuoteCurrent_whenThisAlertFilled.TradedPrice_asString;
							} else {
								return "MARKET_FILL_LAGGING";
							}

						case MarketLimitStop.Stop:
						case MarketLimitStop.StopLimit:
							string priceSL_asString = "WASNT_SERIALIZED";
							PositionPrototype proto = order.Alert.PositionPrototype;
							if (proto == null) return priceSL_asString;

							double priceSL = proto.PriceStopLossActivation;
							priceSL_asString = priceSL.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
							priceSL_asString += " SLactivation [" + proto.StopLossActivation_distanceFromStopLoss + "]";
							return priceSL_asString;

						case MarketLimitStop.Limit:
						default:
							return order.BidOrAsk_whenEmitted_spreadSide_forMarketSubstitutedByLimit(this.dataSnapshot.PricingDecimalForSymbol);
					}
				}

				bool thisKillerReplacesLimitExpired = order.VictimToBeKilled != null && string.IsNullOrEmpty(order.VictimToBeKilled.ReplacedByGUID) == false;
				if (thisKillerReplacesLimitExpired == false) return "";
				double nextSlippage = order.VictimToBeKilled.SlippageNextAvailable_forLimitAlertsOnly_NanWhenNoMore;
				string msg = double.IsNaN(nextSlippage)
					? "noMoreSlippagesAvailable"
					: "nextSlip[" + nextSlippage.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol) + "]";
				return msg;
			};
			this.olvcPriceScript.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcPriceScript.AspectGetter: order=null";
				return order.IsKiller ? "" : order.Alert.PriceScript.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcBidAndAsk_whenEmitted.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcBidAndAsk_whenEmitted.AspectGetter: order=null";
				//v1 return order.IsKiller ? "" : order.PriceEmitted_BidOrAsk_zeroForMarket.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
				return order.IsKiller ? "" : order.BidAndAsk_whenEmitted(this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcBidAndAsk_whenFilled.AspectGetter = delegate(object o) {
			    var order = o as Order;
			    if (order == null) return "olvcBidAndAsk_whenFilled.AspectGetter: order=null";
			    //v1 return order.IsKiller ? "" : order.PriceEmitted_BidOrAsk_zeroForMarket.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			    return order.IsKiller ? "" : order.BidAndAsk_whenFilled(this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcPriceEmitted_withSlippageApplied.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcPriceEmitted_withSlippageApplied.AspectGetter: order=null";
				return order.IsKiller ? "" : order.PriceEmitted.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcPriceFilled.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcPriceFilled.AspectGetter: order=null";
				return order.IsKiller ? "" : order.PriceFilled.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcSlippageApplied.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSlippageApplied.AspectGetter: order=null";
				return order.IsKiller ? "" : order.SlippageApplied.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcSlippageFilled.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSlippageFilled.AspectGetter: order=null";
				return order.IsKiller ? "" : order.SlippageFilled.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcSlippageFilledMinusApplied.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSlippageFilledMinusApplied.AspectGetter: order=null";
				double difference = order.SlippageFilled - order.SlippageApplied;
				return order.IsKiller ? "" : difference.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcCommission.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcCommission.AspectGetter: order=null";
				return order.IsKiller ? "" : order.CommissionFill.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcStateTime.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcStateTime.AspectGetter: order=null";
				return order.StateUpdateLastTimeLocal.ToString(Assembler.DateTimeFormat_toMillis);
			};
			this.olvcState.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcState.AspectGetter: order=null";
				//return (order.InStateExpectingCallbackFromBroker ? "* " : "") + order.State.ToString();
				return order.State.ToString();
			};
//			this.olvcState.FontGetter = delegate(object o) {
//				var order = o as Order;
//				if (order == null) {
//					Assembler.PopupException("olvcState.FontGetter: order=null");
//					return null;
//				}
//				return (order.ExpectingCallbackFromBroker) ? this.fontBold : this.fontNormal;
//			};
			
			this.olvcPriceDeposited_DollarForPoint.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcPriceDeposited.AspectGetter: order=null";
				return (order.QtyFill == Order.INITIAL_QtyFill)
					? Order.INITIAL_QtyFill.ToString()
					: order.Alert.PriceDeposited.ToString("N" + this.dataSnapshot.PricingDecimalForSymbol);
			};
			this.olvcQtyRequested.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcQtyRequested.AspectGetter: order=null";
				return order.IsKiller ? "" : order.Qty.ToString();
			};
			this.olvcQtyFilled.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcQtyFilled.AspectGetter: order=null";
				return order.IsKiller ? "" : order.QtyFill.ToString();
			};

			this.olvcSernoSession.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSernoSession.AspectGetter: order=null";				
				//v1 SORTING_GOES_WRONG__STRINGS_COMPARED return order.SernoSession.ToString();;
				//v2 SORTING_GOES_RIGHT__INTEGERS_COMPARED
				return order.SernoSession;
			};
			this.olvcSernoSession.AspectToStringFormat = "{0:N0}";

			this.olvcSernoExchange.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSernoExchange.AspectGetter: order=null";
				//v1 SORTING_GOES_WRONG__STRINGS_COMPARED return order.SernoExchange.ToString();;
				//v2 SORTING_GOES_RIGHT__INTEGERS_COMPARED
				return order.SernoExchange;
			};
			this.olvcSernoExchange.AspectToStringFormat = "{0:N0}";

			this.olvcGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcGUID.AspectGetter: order=null";
				return order.GUID;
			};
			this.olvcKilledByGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcKilledByGUID.AspectGetter: order=null";
				return order.KillerGUID;
			};
			this.olvcReplacedByGUID.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcReplacedByGUID.AspectGetter: order=null";
				return order.ReplacedByGUID;
			};
			this.olvcBrokerAdapterName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcBrokerAdapterName.AspectGetter: order=null";
				return order.BrokerAdapterName;
			};
			this.olvcDataSourceName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcDataSourceName.AspectGetter: order=null";
				return order.Alert.DataSourceName;
			};
			this.olvcStrategyName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcStrategyName.AspectGetter: order=null";
				return order.Alert.StrategyName;
			};
			this.olvcSignalName.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcSignalName.AspectGetter: order=null";
				return order.Alert.SignalName;
			};
			this.olvcScale.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcScale.AspectGetter: order=null";
				return (order.Alert.BarsScaleInterval == null)
							? "Alert.BarsScaleInterval == null"
							: order.Alert.BarsScaleInterval.ToString();
			};
			this.olvcLastMessage.AspectGetter = delegate(object o) {
				var order = o as Order;
				if (order == null) return "olvcLastMessage.AspectGetter: order=null";
				return order.LastMessage;
			};
		}


		void olvMessages_columnVisibilityChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ignoreEvents_dontSerialize) return;
			OLVColumn olvColumn = sender as OLVColumn;
			if (olvColumn == null) return;
			byte[] olvcMessages_binaryState = this.olvMessages.SaveState();
			string newMessagesState = ObjectListViewStateSerializer.Base64Encode(olvcMessages_binaryState);
			if (this.dataSnapshot.OrderMessagesOlvStateBase64 == newMessagesState) return;
			this.dataSnapshot.OrderMessagesOlvStateBase64 = newMessagesState;
			this.dataSnapshotSerializer.Serialize();
		}
		void olvMessages_columnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ignoreEvents_dontSerialize) return;
			byte[] olvcMessages_binaryState = this.olvMessages.SaveState();
			string newMessagesState = ObjectListViewStateSerializer.Base64Encode(olvcMessages_binaryState);
			if (this.dataSnapshot.OrderMessagesOlvStateBase64 == newMessagesState) return;
			this.dataSnapshot.OrderMessagesOlvStateBase64 = newMessagesState;
			this.dataSnapshotSerializer.Serialize();
		}
		void olvMessages_customize() {
			// MOVED_TO_PopulateDataSnapshot this.olvMessages_customizeColors();

			// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
			List<OLVColumn> allColumns = new List<OLVColumn>();
			foreach (ColumnHeader columnHeader in this.olvMessages.Columns) {
				OLVColumn olvColumn = columnHeader as OLVColumn; 
				if (olvColumn == null) continue;
				olvColumn.VisibilityChanged += this.olvMessages_columnVisibilityChanged;
				if (this.olvOrdersTree.AllColumns.Contains(olvColumn)) continue;
				allColumns.Add(olvColumn);
			}
			foreach (OLVColumn eachColumn in allColumns) {
			    if (this.olvMessages.AllColumns.Contains(eachColumn)) continue;
			    this.olvMessages.AllColumns.Add(eachColumn);
			}
			this.olvMessages.ColumnWidthChanged += new ColumnWidthChangedEventHandler(this.olvMessages_columnWidthChanged);

			this.olvcMessageSerno.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "olvcMessageSerno.AspectGetter: omsg=null";
				return omsg.Serno;
			};
			this.olvcMessageSerno.AspectToStringFormat = "{0:N0}";

			this.olvcMessageDateTime.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "olvcMessageDateTime.AspectGetter: omsg=null";
				//v1 return omsg.DateTime.ToString(Assembler.DateTimeFormat_toMillis);
				return omsg.DateTime;
			};
			this.olvcMessageDateTime.AspectToStringFormat = "{0:" + Assembler.DateTimeFormat_toMillis + "}";

			this.olvcMessageState.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "olvcMessageState.AspectGetter: omsg=null";
				return omsg.State.ToString();
			};
			this.olvcMessageText.AspectGetter = delegate(object o) {
				var omsg = o as OrderStateMessage;
				if (omsg == null) return "olvcMessageText.AspectGetter: omsg=null";
				return omsg.Message;
			};
		}

		void olvOrdersTree_customizeColors() {
			//this.colorBackgroundRed_forPositionLoss		= Color.FromArgb(255, 230, 230);
			//this.colorBackgroundGreen_forPositionProfit	= Color.FromArgb(230, 255, 230);
			this.colorBackgroundRed_forPositionLoss		= Assembler.InstanceInitialized.ColorBackgroundRed_forPositionLoss;
			this.colorBackgroundGreen_forPositionProfit	= Assembler.InstanceInitialized.ColorBackgroundGreen_forPositionProfit;

			// unconditional because Font=bold is set for order.InState_expectingBrokerCallback
			this.olvOrdersTree.FormatRow += new EventHandler<FormatRowEventArgs>(this.olvOrdersTree_FormatRow);

			if (this.dataSnapshot.ColorifyOrderTree_positionNet) {
				this.olvOrdersTree.UseCellFormatEvents = true;
			} else {
				this.olvOrdersTree.UseCellFormatEvents = false;
			}
		}

		void olvMessages_customizeColors() {
			if (this.dataSnapshot.ColorifyMessages_askBrokerProvider) {
				this.olvMessages.UseCellFormatEvents = true;
				this.olvMessages.FormatRow += new EventHandler<FormatRowEventArgs>(this.olvMessages_FormatRow);
			} else {
				this.olvMessages.UseCellFormatEvents = false;
				this.olvMessages.FormatRow -= new EventHandler<FormatRowEventArgs>(this.olvMessages_FormatRow);
			}
		}		
		void olvMessages_FormatRow(object sender, FormatRowEventArgs e) {
			OrderStateMessage osm = e.Model as OrderStateMessage;
			if (osm == null) return;

			Color backColor = osm.BackColor_fromBroker_neverNull;
			if (backColor != Color.Empty) {
				e.Item.BackColor = backColor;
			} else {
				if (osm.Message_LedToPostProcessing) {
					e.Item.BackColor = this.dataSnapshot.MessagesColorBackground_ledToProcessing;
				}
			}

			if (osm.Message_ChangedOrderState) {
				e.Item.ForeColor = this.dataSnapshot.MessagesColorForeground_changedOrderState;
			}
		}

		FontCache fontCache;
		Color colorBackgroundRed_forPositionLoss;
		Color colorBackgroundGreen_forPositionProfit;

		void olvOrdersTree_FormatRow(object sender, FormatRowEventArgs e) {
			Order order = e.Model as Order;
			if (order == null) return;
			if (order.InState_expectingBrokerCallback) {
				//v1 e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);
				e.Item.Font = this.fontCache.Bold;
			}

			//v1 if (Assembler.InstanceInitialized.AlertsForChart.IsItemRegisteredForAnyContainer(order.Alert)) return;
			//v2 ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.STRATEGY=NULL,BARS=NULL
			if (order.Alert.Bars == null) e.Item.ForeColor = Color.DimGray;
			// replaced with new column if (order.Alert.MyBrokerIsLivesim) e.Item.BackColor = Color.Gainsboro;

			// I already set if when unclicked but didn't unsubscribe from the event... hm...
			if (this.olvOrdersTree.UseCellFormatEvents == false) return;

			if (order.Alert == null) return;
			if (order.Alert.PositionAffected == null) return;
			e.Item.BackColor = order.Alert.PositionAffected.NetProfit > 0.0
				? this.colorBackgroundGreen_forPositionProfit
				: this.colorBackgroundRed_forPositionLoss;

		}

		//http://objectlistview.sourceforge.net/cs/recipes.html#how-can-i-change-the-colours-of-a-row-or-just-a-cell
		//readonly Color BACKGROUND_GREEN = Color.FromArgb(230, 255, 230);
		//readonly Color BACKGROUND_RED = Color.FromArgb(255, 230, 230);
		//void ordersTree_FormatRow(object sender, FormatRowEventArgs e) {
		//    var order = e.Model as Order;
		//    if (order == null) {
		//        Assembler.PopupException("ordersTree_FormatRow(): (e.Model as Order =null");
		//        return;
		//    }
		//    e.Item.BackColor = (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long)
		//        ? BACKGROUND_GREEN : BACKGROUND_RED;
		//}

	
		// WRONG WAY TO MOVE COLUMNS AROUND: AFTER I did RestoreState(), Column(3) is not State and I add State twice => exception
//		public void MoveStateColumnToLeftmost() {
//			//moving State as we drag-n-dropped it; tree will grow in second column
//			//NOT_NEEDED this.OrdersTree.BuildList();
//			//NOT_NEEDED this.RebuildAllTreeFocusOnTopmost();
//			this.OrdersTreeOLV.SetObjects(this.ordersTree.InnerOrderList);
//			//NOT_NEEDED this.OrdersTree.RebuildAll(true);
//			this.OrdersTreeOLV.Columns.RemoveAt(3);
//			this.OrdersTreeOLV.Columns.Insert(0, this.olvcState);
//			//NOT_NEEDED this.OrdersTree.BuildList();
//			//NOT_NEEDED this.RebuildAllTreeFocusOnTopmost();
//		}
	}
}
