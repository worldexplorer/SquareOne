using System;
using System.Drawing;

using BrightIdeasSoftware;
using Sq1.Core.Execution;

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
		void objectListViewCustomize() {
			this.objectListViewCustomizeColors();
			
			this.olvcPosition.ImageGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.AspectGetter: position=null";

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
				if (position == null) return "clhPosition.AspectGetter: position=null";
				return position.PositionLongShort.ToString();
			};
			this.olvcSerno.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhSerno.AspectGetter: position=null";
				return position.SernoAbs.ToString();
			};
			this.olvcSymbol.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhSymbol: position=null";
				return position.Bars.Symbol;
			};
			this.olvcQuantity.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhQuantity: position=null";
				//string format = "N" + position.Bars.SymbolInfo.DecimalsVolume;
				return position.Shares.ToString(base.FormatPrice);
			};
			this.olvcEntryDate.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhEntryDate: position=null";
				string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				return position.EntryDate.ToString(format);
			};
			this.olvcEntryPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhEntryPrice: position=null";
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				return position.EntryFilledPrice.ToString(base.FormatPrice);
			};
			this.olvcEntryOrder.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhEntryOrder.AspectGetter: position=null";
				return position.EntryMarketLimitStop.ToString();
			};
			this.olvcExitDate.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhExitDate: position=null";
				if (position.ExitDate == DateTime.MinValue) return "STILL_OPEN";
				string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				return position.EntryDate.ToString(format);
			};
			this.olvcExitPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhExitPrice: position=null";
				//if (position.ExitDate == DateTime.MinValue) return "STILL_OPEN"
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				return position.ExitFilledPrice.ToString(base.FormatPrice);
			};
			this.olvcExitOrder.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhExitOrder.AspectGetter: position=null";
				if (position.ExitDate == DateTime.MinValue) return "STILL_OPEN";
				return position.ExitMarketLimitStop.ToString();
			};
			this.olvcProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhProfitPct: position=null";
				return position.NetProfitPercent.ToString("F2");
			};
			this.olvcProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhProfitDollar: position=null";
				return position.NetProfit.ToString(base.FormatPrice);
			};
			this.olvcBarsHeld.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhBarsHeld: position=null";
				return position.BarsHeld.ToString("N0");
			};
			this.olvcProfitPerBar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhPosition.clhProfitPerBar: position=null";
				return position.ProfitPerBar.ToString(base.FormatPrice);
			};
			this.olvcEntrySignalName.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhEntrySignalName.AspectGetter: position=null";
				return position.EntrySignal.ToString();
			};
			this.olvcExitSignalName.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhExitSignalName.AspectGetter: position=null";
				if (position.ExitDate == DateTime.MinValue) return null;
				return position.ExitSignal.ToString();
			};
			this.olvcMaePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhMaePct.AspectGetter: position=null";
				return position.MAEPercent.ToString("F2");
			};
			this.olvcMfePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhMfePct.AspectGetter: position=null";
				return position.MFEPercent.ToString("F2");
			};
			this.olvcCumProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhCumProfitDollar.AspectGetter: position=null";
				//double equityAtThisPositionClosed = this.systemPerformance.SlicesShortAndLong.EquityCurve.HOW_TO_FIND?
				double equityAtThisPositionClosed = -1;
				if (this.cumulativeProfitDollar.ContainsKey(position)) equityAtThisPositionClosed = this.cumulativeProfitDollar[position];
				return equityAtThisPositionClosed.ToString(base.FormatPrice);
			};
			this.olvcCumProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhCumProfitPct.AspectGetter: position=null";
				double equityPercentAtThisPositionClosed = -1;
				if (this.cumulativeProfitPercent.ContainsKey(position)) equityPercentAtThisPositionClosed = this.cumulativeProfitPercent[position];
				return equityPercentAtThisPositionClosed.ToString("F2");
			};
			this.olvcComission.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhComission.AspectGetter: position=null";
				double commission = position.EntryFilledCommission + position.ExitFilledCommission;
				return commission.ToString("F2");
			};
		}
	}
}