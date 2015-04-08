using System;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Forms;
using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Reporters {
	public partial class Positions {
		Color colorBackgroundRedForPositionLoss;
		Color colorBackgroundGreenForPositionProfit;
		
		void olvPositions_FormatRow(object sender, FormatRowEventArgs e) {
			Position position = e.Model as Position;
			if (position == null) return;
			e.Item.BackColor = (position.NetProfit > 0.0) ? this.colorBackgroundGreenForPositionProfit : this.colorBackgroundRedForPositionLoss;
		}
		void objectListViewCustomizeColors() {
			if (this.snap.Colorify) {
				this.colorBackgroundRedForPositionLoss = Color.FromArgb(255, 230, 230);
				this.colorBackgroundGreenForPositionProfit = Color.FromArgb(230, 255, 230);
				this.olvPositions.UseCellFormatEvents = true;
				this.olvPositions.FormatRow += new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			} else {
				this.olvPositions.UseCellFormatEvents = false;
				this.olvPositions.FormatRow -= new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			}
		}
		
		void oLVColumn_VisibilityChanged(object sender, EventArgs e) {
			OLVColumn oLVColumn = sender as OLVColumn;
			if (oLVColumn == null) return;
			byte[] olvStateBinary = this.olvPositions.SaveState();
			this.snap.PositionsOlvStateBase64 = ObjectListViewStateSerializer.Base64Encode(olvStateBinary);
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			base.RaiseContextScriptChangedContainerShouldSerialize();
		}
		void objectListViewCustomize() {
			this.objectListViewCustomizeColors();
			
			try {
				// #1/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
				List<OLVColumn> allColumnsOtherwizeEmptyListAndRowFormatException = new List<OLVColumn>();
				foreach (ColumnHeader columnHeader in this.olvPositions.Columns) {
					OLVColumn oLVColumn = columnHeader as OLVColumn; 
					oLVColumn.VisibilityChanged += oLVColumn_VisibilityChanged;
					if (oLVColumn == null) continue;
					//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
					if (this.olvPositions.AllColumns.Contains(oLVColumn)) continue;
					allColumnsOtherwizeEmptyListAndRowFormatException.Add(oLVColumn);
				}
				if (allColumnsOtherwizeEmptyListAndRowFormatException.Count > 0) {
					//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
					this.olvPositions.AllColumns.AddRange(allColumnsOtherwizeEmptyListAndRowFormatException);
				}
				// #2/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				if (this.snap.PositionsOlvStateBase64.Length > 0) {
					byte[] olvStateBinary = ObjectListViewStateSerializer.Base64Decode(this.snap.PositionsOlvStateBase64);
					this.olvPositions.RestoreState(olvStateBinary);
				}
			} catch (Exception ex) {
				string msg = "this.olvPositions.RestoreState(olvStateBinary)";
				Assembler.PopupException(msg, ex);
			}

			this.olvcPosition.ImageGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcPosition.ImageGetter: position=null";

				//this.tradeTypes.Images.SetKeyName(0, "45degrees3-LongEntryUnknown.png");
				//this.tradeTypes.Images.SetKeyName(1, "45degrees3-LongEntryProfit.png");
				//this.tradeTypes.Images.SetKeyName(2, "45degrees3-LongEntryLoss.png");
				//this.tradeTypes.Images.SetKeyName(3, "45degrees3-ShortEntryUnknown.png");
				//this.tradeTypes.Images.SetKeyName(4, "45degrees3-ShortEntryProfit.png");
				//this.tradeTypes.Images.SetKeyName(5, "45degrees3-ShortEntryLoss.png");
				int imgIndex = -1;
				if (position.PositionLongShort == PositionLongShort.Long) {
					imgIndex = 0;
					if (position.ExitFilledBarIndex > -1) {
						imgIndex = position.NetProfit > 0 ? 1 : 2;
					}
				} else {
					imgIndex = 3;
					if (position.ExitFilledBarIndex > -1) {
						imgIndex = position.NetProfit > 0 ? 4 : 5;
					}
				}
				return imgIndex;
			};
			this.olvcPosition.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcPosition.AspectGetter: position=null";
				return position.PositionLongShort.ToString();
			};
			this.olvcSerno.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcSerno.AspectGetter: position=null";
				return position.SernoAbs.ToString();
			};
			this.olvcSymbol.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcSymbol.AspectGetter: position=null";
				return position.Bars.Symbol;
			};
			this.olvcQuantity.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcQuantity.AspectGetter: position=null";
				//string format = "N" + position.Bars.SymbolInfo.DecimalsVolume;
				return position.Shares.ToString(base.FormatPrice);
			};
			this.olvcCost.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCost.AspectGetter: position=null";
				return position.PositionCost.ToString(base.FormatPrice);
			};

			this.olvcEntryDate.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntryDate.AspectGetter: position=null";
				string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				return position.EntryDateBarTimeOpen.ToString(format);
			};
			this.olvcEntryPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntryPrice.AspectGetter: position=null";
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				return position.EntryFilledPrice.ToString(base.FormatPrice);
			};
			this.olvcEntryOrder.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntryOrder.AspectGetter: position=null";
				return position.EntryMarketLimitStop.ToString();
			};
			this.olvcEntrySignalName.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntrySignalName.AspectGetter: position=null";
				return position.EntrySignal.ToString();
			};

			this.olvcExitDate.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcExitDate.AspectGetter: position=null";
				if (position.ExitDateBarTimeOpen == DateTime.MinValue) return "STILL_OPEN";
				string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				return position.EntryDateBarTimeOpen.ToString(format);
			};
			this.olvcExitPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcExitPrice.AspectGetter: position=null";
				//if (position.ExitDate == DateTime.MinValue) return "STILL_OPEN"
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				return position.ExitFilledPrice.ToString(base.FormatPrice);
			};
			this.olvcExitOrder.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcExitOrder.AspectGetter: position=null";
				if (position.ExitDateBarTimeOpen == DateTime.MinValue) return "STILL_OPEN";
				return position.ExitMarketLimitStop.ToString();
			};
			this.olvcExitSignalName.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcExitSignalName.AspectGetter: position=null";
				if (position.ExitDateBarTimeOpen == DateTime.MinValue) return null;
				return position.ExitSignal.ToString();
			};

			this.olvcProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitPct.AspectGetter: position=null";
				return position.NetProfitPercent.ToString("F2") + " %";
			};
			this.olvcProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitDollar.AspectGetter: position=null";
				return position.NetProfit.ToString(base.FormatPrice) + " $";
			};

			this.olvcBarsHeld.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcBarsHeld.AspectGetter: position=null";
				return position.BarsHeld.ToString("N0");
			};
			this.olvcProfitPerBar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitPerBar.AspectGetter: position=null";
				return position.ProfitPerBar.ToString(base.FormatPrice) + " $";
			};

			this.olvcMae.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMae.AspectGetter: position=null";
				return position.MAE.ToString(base.FormatPrice) + " $";
			};
			this.olvcMfe.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMfe.AspectGetter: position=null";
				return position.MFE.ToString(base.FormatPrice) + " $";
			};
			this.olvcMaePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMaePct.AspectGetter: position=null";
				return position.MAEPercent.ToString("F2") + " %";
			};
			this.olvcMfePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMfePct.AspectGetter: position=null";
				return position.MFEPercent.ToString("F2") + " %";
			};

			this.olvcCumNetProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCumProfitDollar.AspectGetter: position=null";
				double cumProfit = this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitForPosition(position);
				return cumProfit.ToString(base.FormatPrice) + " $";
			};
			this.olvcCumNetProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCumProfitPct.AspectGetter: position=null";
				double cumProfitPct = this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitPercentForPosition(position);
				return cumProfitPct.ToString("F2") + " %";
			};

			this.olvcComission.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhComission.AspectGetter: position=null";
				double commission = position.EntryFilledCommission + position.ExitFilledCommission;
				return commission.ToString("F2") + " $";
			};
		}
	}
}