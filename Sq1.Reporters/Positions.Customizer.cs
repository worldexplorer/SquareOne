using System;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Reporters {
	public partial class Positions {
		Color colorBackgroundRed_forPositionLoss;
		Color colorBackgroundGreen_forPositionProfit;
		
		void olvPositions_FormatRow(object sender, FormatRowEventArgs e) {
			Position position = e.Model as Position;
			if (position == null) return;
			e.Item.BackColor = (position.NetProfit > 0.0) ? this.colorBackgroundGreen_forPositionProfit : this.colorBackgroundRed_forPositionLoss;
		}
		void olv_customizeColors() {
			if (this.snap.Colorify) {
				this.colorBackgroundRed_forPositionLoss		= Color.FromArgb(255, 230, 230);
				this.colorBackgroundGreen_forPositionProfit	= Color.FromArgb(230, 255, 230);
				this.olvPositions.UseCellFormatEvents = true;
				this.olvPositions.FormatRow += new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			} else {
				this.olvPositions.UseCellFormatEvents = false;
				this.olvPositions.FormatRow -= new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			}
		}
		void olvColumn_VisibilityChanged(object sender, EventArgs e) {
			OLVColumn olvColumn = sender as OLVColumn;
			if (olvColumn == null) return;
			this.olvBinaryState_save_raiseStrategySerialize();
		}
		void olvReCustomize_OnPriceDecimalsChanged() {
			if (this.olvcEntryPrice			.AspectToStringFormat == "{0:" + base.FormatPrice + "}") {
				string msg = "FORMAT_PRICE_DIDNT_CHANGE__WHY_DID_YOU_INVOKE_ME???";
				Assembler.PopupException(msg);
			}
			this.olvcEntryPrice			.AspectToStringFormat = "{0:" + base.FormatPrice + "}";
			this.olvcExitPrice			.AspectToStringFormat = "{0:" + base.FormatPrice + "}";
			this.olvcProfitPct			.AspectToStringFormat = "{0:" + base.FormatPrice + "} %";
			//this.olvcQuantity			.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcProfitPerBar		.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcMae				.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcMfe				.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcCumNetProfitDollar	.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcComission			.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";
			this.olvcCost				.AspectToStringFormat = "{0:" + base.FormatPrice + "}";
		}
		void olvCustomize() {
			this.olv_customizeColors();
			this.olvBinaryState_restore();

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
				return position.SernoPerStrategy;
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
				return position.Shares;
			};
			this.olvcQuantity.AspectToStringFormat = "{0:" + base.FormatVolume + "}";

			this.olvcCost.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCost.AspectGetter: position=null";
				//return position.PositionCost.ToString(base.FormatPrice);
				return position.PositionCost;
			};
			this.olvcCost.AspectToStringFormat = "{0:" + base.FormatPrice + "}";

			this.olvcEntryDate.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntryDate.AspectGetter: position=null";
				//v1 string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				string dateFormat_entryExit = (position.Bars.IsIntraday)
					? Assembler.DateTimeFormat_toSeconds_dayFirst
					: Assembler.DateTimeFormat_toDays_dayFirst;
				return position.EntryDateBarTimeOpen.ToString(dateFormat_entryExit);
				//return position.EntryDateBarTimeOpen;
			};
			//this.olvcEntryDate.AspectToStringConverter = delegate(object o) {
			//    Position position = o as DateTime;
			//    if (position == null) return "olvcEntryDate.AspectGetter: position=null";
			//    string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
			//}


			this.olvcEntryPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcEntryPrice.AspectGetter: position=null";
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				return position.EntryFilled_price;
			};
			this.olvcEntryPrice.AspectToStringFormat = "{0:" + base.FormatPrice + "}";

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
				//v1 string format = (position.Bars.IsIntraday) ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy";
				string dateFormat_entryExit = (position.Bars.IsIntraday)
					? Assembler.DateTimeFormat_toSeconds_dayFirst
					: Assembler.DateTimeFormat_toDays_dayFirst;
				return position.EntryDateBarTimeOpen.ToString(dateFormat_entryExit);
			};

			this.olvcExitPrice.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcExitPrice.AspectGetter: position=null";
				//if (position.ExitDate == DateTime.MinValue) return "STILL_OPEN"
				//string format = "N" + position.Bars.SymbolInfo.DecimalsPrice;
				//return position.ExitFilledPrice.ToString("{0:" + base.FormatPrice + "}");
				return position.ExitFilled_price;
			};
			this.olvcExitPrice.AspectToStringFormat = "{0:" + base.FormatPrice + "}";

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
				return position.ExitSignal;
			};

			this.olvcProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitPct.AspectGetter: position=null";
				//return position.NetProfitPercent.ToString("F2") + " %";
				return position.NetProfitPercent;
			};
			this.olvcProfitPct.AspectToStringFormat = "{0:" + base.FormatPrice + "} %";

			this.olvcProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitDollar.AspectGetter: position=null";
				//return position.NetProfit.ToString("{0:" + base.FormatPrice + "}") + " $";
				return position.NetProfit;
			};
			this.olvcProfitDollar.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";

			this.olvcBarsHeld.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcBarsHeld.AspectGetter: position=null";
				return position.BarsHeld;
			};
			//this.olvcBarsHeld.AspectToStringFormat = "{0:N0}";

			this.olvcProfitPerBar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcProfitPerBar.AspectGetter: position=null";
				//return position.ProfitPerBar.ToString("{0:" + base.FormatPrice + "}") + " $";
				return position.ProfitPerBar;
			};
			this.olvcProfitPerBar.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";


			this.olvcMae.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMae.AspectGetter: position=null";
				//return position.MAE.ToString("{0:" + base.FormatPrice + "}") + " $";
				return position.MAE;
			};
			this.olvcMae.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";

			this.olvcMfe.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMfe.AspectGetter: position=null";
				//return position.MFE.ToString("{0:" + base.FormatPrice + "}") + " $";
				return position.MFE;
			};
			this.olvcMfe.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";

			this.olvcMaePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMaePct.AspectGetter: position=null";
				//return position.MAEPercent.ToString("F2") + " %";
				return position.MAEPercent;
			};
			this.olvcMaePct.AspectToStringFormat = "{0:" + base.FormatPrice + "} %";

			this.olvcMfePct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcMfePct.AspectGetter: position=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return position.MFEPercent;
			};
			this.olvcMfePct.AspectToStringFormat = "{0:" + base.FormatPrice + "} %";


			this.olvcCumNetProfitDollar.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCumProfitDollar.AspectGetter: position=null";
				double cumProfit = this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitForPosition(position);
				//return cumProfit.ToString("{0:" + base.FormatPrice + "}") + " $";
				return cumProfit;
			};
			this.olvcCumNetProfitDollar.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";

			this.olvcCumNetProfitPct.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "olvcCumProfitPct.AspectGetter: position=null";
				double cumProfitPct = this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitPercentForPosition(position);
				//return cumProfitPct.ToString("F2") + " %";
				return cumProfitPct;
			};
			this.olvcCumNetProfitPct.AspectToStringFormat = "{0:" + base.FormatPrice + "} %";

			this.olvcComission.AspectGetter = delegate(object o) {
				var position = o as Position;
				if (position == null) return "clhComission.AspectGetter: position=null";
				double commission = position.EntryFilled_commission + position.ExitFilled_commission;
				//return commission.ToString("F2") + " $";
				return commission;
			};
			this.olvcComission.AspectToStringFormat = "{0:" + base.FormatPrice + "} $";

			this.olvPositions.ColumnWidthChanged += new ColumnWidthChangedEventHandler(olvPositions_ColumnWidthChanged);
		}

		void olvPositions_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
			if (this.ignoreColumnWidthChanged_onNewReportDataFlushed_dontSaveStrategy) return;
			this.olvBinaryState_save_raiseStrategySerialize();
		}
	}
}