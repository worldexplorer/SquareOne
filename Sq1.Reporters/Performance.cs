using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Support;
using Sq1.Core.Support;

namespace Sq1.Reporters {
	public partial class Performance : Reporter {
		int currentColumn;
		int currentRow;

		//v1 Dictionary<FontStyle, Font> fontsByStyle_dontDisposeReusableGDI;
		FontCache fontCache;

		public Performance(ChartShadow chart): this() {
			this.Initialize(chart, null);
		}
		public Performance() : base() {
			base.TabText = "Performance";
			this.InitializeComponent();
			//this.fontsByStyle_dontDisposeReusableGDI = new Dictionary<FontStyle, Font>();
			WindowsFormsUtils.SetDoubleBuffered(this.lvPerformance);
			this.objectListViewCustomize();
			fontCache = new FontCache(this.Font);
		}
		public override void BuildFullOnBacktestFinished() {
			this.propagatePerformanceReport();
		}
		void propagatePerformanceReport() {
			if (base.SystemPerformance == null) {
				string msg = "YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES";
				Assembler.PopupException(msg);
				return;
			}
			
			//this.fontsByStyle_dontDisposeReusableGDI.Clear();
			//this.fontsByStyle_dontDisposeReusableGDI.Add(this.Font.Style, this.Font);
			try {
				this.lvPerformance.BeginUpdate();
				this.lvPerformance.Items.Clear();
				
				this.currentColumn = 0;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(base.SystemPerformance.SlicesShortAndLong);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(base.SystemPerformance.SliceLong);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(base.SystemPerformance.SliceShort);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(base.SystemPerformance.SliceBuyHold);

				if (base.SystemPerformance.BarsBuyAndHold != null) {
					this.colBuyHold.Text = base.SystemPerformance.BarsBuyAndHold.Symbol;
					this.colBuyHold.Width = 50;
				} else {
					this.colBuyHold.Text = "Buy & Hold";
					this.colBuyHold.Width = 50;
				}
				//AdjustColumnSize();
			} finally {
				this.lvPerformance.EndUpdate();
			}
		}

		void AdjustColumnSize() {
			foreach (ColumnHeader colHeader in this.lvPerformance.Columns) {
				colHeader.Width = -1;
			}
		}
		protected virtual void GenerateReportForOneColumn(SystemPerformanceSlice slice) {
			List<Position> positionsAllReadOnly = slice.PositionsImTrackingReadonly;  

			// NO_FORMATTING_PRINT_AS_IT_IS !!!! YOULL_NEVER_FIND_ROUNDING_ERROR_IF_YOU_ROUND_JUST_BEFORE_PRINTING
			this.addCurrency(		slice.NetProfitForClosedPositionsBoth, "Net Profit", "NetProfitForClosedPositionsBoth", Color.Empty, Color.Empty, this.getLviForeColor(slice.NetProfitForClosedPositionsBoth), FontStyle.Bold, FontStyle.Regular);
			this.addNumeric(		slice.WinLossRatio,			"Win/Loss Ratio", "Win/Loss Ratio = PositionsCountWinners / PositionsCountLosers", this.getLviForeColor(slice.WinLossRatio, 1));
			this.addNumeric(		slice.ProfitFactor,			"Profit Factor", "Profit Factor = NetProfitWinners / Abs(NetLossLosers)", this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric(		slice.RecoveryFactor,		"Recovery Factor", "Recovery Factor = Abs(NetProfitForClosedPositionsBoth / MaxDrawDown)", this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric(		slice.PayoffRatio,			"Payout", "Payout = Abs(AvgProfitPctBoth / AvgLossPctLosers)", this.getLviForeColor(slice.ProfitFactor, 1));
			this.addCurrency(	   -slice.CommissionBoth,		"Commission", "-CommissionBoth", this.getLviForeColor(-slice.CommissionBoth));

			this.addNumeric(		slice.PositionsCount,		 "All Trades", "PositionsCountClosed", Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric(		slice.AvgProfitBoth,		"Avg Profit", "Avg Profit = NetProfitForClosedPositionsBoth / PositionsCountBoth", this.getLviForeColor(slice.AvgProfitBoth));
			this.addPercent(		slice.AvgProfitPctBoth,		"Avg Profit %", "Avg Profit % = NetProfitPctForClosedPositionsBoth / PositionsCountBoth", this.getLviForeColor(slice.AvgProfitPctBoth));
			this.addNumeric(		slice.AvgBarsHeldBoth,		"Avg Bars Held", "Avg Bars Held = BarsHeldTotalForClosedPositionsBoth / PositionsCountBoth");
			this.addNumeric(		slice.ProfitPerBarBoth, 	"Profit per Bar", "Profit per Bar = NetProfitForClosedPositionsBoth / BarsHeldTotalForClosedPositionsBoth",	this.getLviForeColor(slice.NetProfitForClosedPositionsBoth));
			this.addCurrency(		slice.MaxDrawDown,			"Max Drawdown", "Max Drawdown = Min(NetProfitForClosedPositionsBoth - NetProfitPeak)", Color.Empty, Color.Empty, this.getLviForeColor(slice.MaxDrawDown));
			this.addDateTime(		slice.MaxDrawDownLastLossDate, "Max Drawdown Date", "Max Drawdown Date = Date(Max Drawdown)");

			this.addNumeric((double)slice.PositionsCountWinners, "Winners", "PositionsCountWinners", Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric(		slice.WinRatePct,			"Win Rate", "Win Rate = 100 * PositionsCountWinners / PositionsCountBoth", this.getLviForeColor(slice.WinRatePct, 50));
			this.addCurrency(		slice.NetProfitWinners,		"Net Profit", "NetProfitWinners", this.getLviForeColor(slice.NetProfitWinners));
			this.addNumeric(		slice.AvgProfitWinners,		"Avg Profit", "Winners Avg Profit = NetProfitWinners / PositionsCountWinners", this.getLviForeColor(slice.AvgProfitWinners));
			this.addPercent(		slice.AvgProfitPctWinners,	"Avg Profit %", "Winners Avg Profit % = NetProfitPctForClosedPositionsLong / PositionsCountWinners", this.getLviForeColor(slice.AvgProfitPctWinners));
			this.addNumeric(		slice.AvgBarsHeldWinners,	"Avg Bars Held", "Winners Avg Bars Held = BarsHeldTotalForClosedPositionsWinners / PositionsCountWinners");
			this.addNumeric((double)slice.MaxConsecWinners,		"Max Consecutive Winners", "Winners Max Consecutive = MaxLen(exitPosition.NetProfit > 0)");

			this.addNumeric((double)slice.PositionsCountLosers, "Losers", "PositionsCountLosers", Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric(		slice.LossRatePct,			"Loss Rate", "Loss Rate = 100 * this.PositionsCountLosers / PositionsCountBoth", this.getLviForeColor(slice.LossRatePct, 50));
			this.addCurrency(		slice.NetLossLosers,		"Net Loss", "NetLossLosers", this.getLviForeColor(slice.NetLossLosers));
			this.addNumeric(		slice.AvgLossLosers,		"Avg Loss", "Losers Avg Loss = NetLossLosers / PositionsCountLosers", this.getLviForeColor(slice.AvgLossLosers));
			this.addPercent(		slice.AvgLossPctLosers,		"Avg Loss %", "Losers Avg Loss % = NetProfitPctForClosedPositionsLosers / PositionsCountLosers", this.getLviForeColor(slice.AvgLossPctLosers));
			this.addNumeric(		slice.AvgBarsHeldLosers,	"Avg Bars Held", "Losers Avg Bars Held = BarsHeldTotalForClosedPositionsLosers / PositionsCountLosers");
			this.addNumeric((double)slice.MaxConsecLosers,		"Max Consecutive Losers", "Losers Max Consecutive = MaxLen(exitPosition.NetProfit < 0)");
		}
		
		Color getLviForeColor(double value, double ethalonRedIfLessBlueIfGreater = 0.0) {
			if (value == ethalonRedIfLessBlueIfGreater) return this.ForeColor;
			return (value > ethalonRedIfLessBlueIfGreater) ? Color.Blue : Color.Red;
		}
		void addCurrency(double value, string label, string tooltip, Color itemFontColor) {
			this.addCurrency(value, label, tooltip, Color.Empty, Color.Empty, itemFontColor);
		}
		void addCurrency(double value, string label, string tooltip,
				Color backColor, Color labelFontColor, Color itemFontColor,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			string format = SystemPerformance.Bars.SymbolInfo.PriceFormat;
			string valueFormatted = value.ToString(format);
			this.addLvi(valueFormatted, label, tooltip, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addNumeric(double value, string label, string tooltip) {
			this.addLvi(value.ToString(), label, tooltip, Color.Empty, Color.Empty, Color.Empty);
		}
		void addNumeric(double value, string label, string tooltip, Color itemFontColor) {
			this.addLvi(value.ToString(), label, tooltip, Color.Empty, Color.Empty, itemFontColor);
		}
		void addNumeric(double value, string label, string tooltip, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			this.addLvi(value.ToString(), label, tooltip, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addPercent(double value, string label, string tooltip, Color itemFontColor) {
			this.addLvi(value.ToString(), label, tooltip, Color.Empty, Color.Empty, itemFontColor);
		}
		void addDateTime(DateTime value, string label, string tooltip) {
			this.addLvi(value.ToShortDateString() + " " + value.ToShortTimeString(), label, tooltip, Color.Empty, Color.Empty, Color.Empty);
		}
		void addLvi(string valueAlreadyFormatted, string label, string tooltip,
				Color colorBack, Color colorForeLabel, Color colorFore,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			ListViewItem lvi;
			if (this.currentColumn == 0) {
				lvi = this.lvPerformance.Items.Add(label);
				lvi.ToolTipText = tooltip;
				lvi.UseItemStyleForSubItems = false;
				if (colorForeLabel != Color.Empty) lvi.ForeColor = colorForeLabel;
				if (colorBack != Color.Empty) lvi.BackColor = colorBack;

				//v1
				//if (this.fontsByStyle_dontDisposeReusableGDI.ContainsKey(labelFontStyle) == false) {
				//    Font font = new Font(this.Font, labelFontStyle);
				//    this.fontsByStyle_dontDisposeReusableGDI.Add(labelFontStyle, font);
				//}
				//lvi.Font = this.fontsByStyle_dontDisposeReusableGDI[labelFontStyle];
				//v2
				lvi.Font = this.fontCache.GetCachedFontWithStyle(labelFontStyle);
			} else {
				if (this.currentRow >= this.lvPerformance.Items.Count) {
					#if DEBUG
					Debugger.Launch();
					#endif
					return;
				}
				lvi = this.lvPerformance.Items[this.currentRow];
				this.currentRow++;
			}

			//v1
			//if (this.fontsByStyle_dontDisposeReusableGDI.ContainsKey(itemFontStyle) == false) {
			//    Font newFont = new Font(this.Font, itemFontStyle);
			//    this.fontsByStyle_dontDisposeReusableGDI.Add(itemFontStyle, newFont);
			//}
			//Font subLviFont = this.fontsByStyle_dontDisposeReusableGDI[itemFontStyle];
			//v2
			Font subLviFont = this.fontCache.GetCachedFontWithStyle(itemFontStyle);

			ListViewItem.ListViewSubItem subLvi = new ListViewItem.ListViewSubItem(lvi, valueAlreadyFormatted, colorFore, colorBack, subLviFont);
//			subLvi.ToolTipText = tooltip;
//			if (colorBack	!= Color.Empty) subLvi.BackColor = colorBack;
//			if (colorFore	!= Color.Empty) subLvi.ForeColor = colorFore;
			lvi.SubItems.Add(subLvi);
		}
		public override object CreateSnapshotToStoreInScriptContext() {
			return null;
		}
		double lastKnownCashAvailable = -1;
		public override void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			if (base.SystemPerformance == null) {
				string msg = "YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES";
				Assembler.PopupException(msg, null, false);
				return;
			} else {
				this.lastKnownCashAvailable = base.SystemPerformance.SlicesShortAndLong.CashAvailable;
			}
			this.propagatePerformanceReport();
		}
		public override void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
			if (base.SystemPerformance == null) {
				string msg = "YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES";
				Assembler.PopupException(msg);
				return;
			} else {
				this.lastKnownCashAvailable = base.SystemPerformance.SlicesShortAndLong.CashAvailable;
			}
			this.propagatePerformanceReport();
		}
		public override void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
			if (base.SystemPerformance == null) {
				string msg = "YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES__QUOTES_PUMP_SHOULD_BE_UNPAUSED_AFTER_BACKTEST_COMPLETES";
				Assembler.PopupException(msg);
				return;
			} else {
				//if (this.lastKnownCashAvailable == base.SystemPerformance.SlicesShortAndLong.CashAvailable) return;
				//	this.lastKnownCashAvailable = base.SystemPerformance.SlicesShortAndLong.CashAvailable;
			}
			//PERFORMANCE_ISNT_CHANGED_ON_EACH_QUOTE__RECALCULATED_ONCE_AFTER_POSITION_CLOSE_AND_STAYS_UNCHANGED
			return;

			////v1 this.propagatePerformanceReport();
			////v2 ACCELERATING_ON_POSITION_FILLED
			//if (this.lvPerformance.Items.Count == 0) {
			//     this.propagatePerformanceReport();
			//     return;
			//}

			//if (pokeUnit.PositionsOpened.Count == 0) return;

			//// MINIMIZED_IF_STREAMING_QUOTE_DELAY<100ms_UNMINIMIZE_AND_WATCH__REALTIME_UNAFFECTED
			////bool livesimStreamingIsSleeping = pokeUnit.PositionsOpened.AlertsEntry.GuiHasTimeToRebuild(
			////    this, "Reporters.Performance.BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(WAIT)");
			////if (livesimStreamingIsSleeping == false) {
			////    return;
			////}
			//if (base.Visible == false) return;		//DockContent is minimized / "autohidden"
			//this.propagatePerformanceReport();
			//this.lvPerformance.Refresh();
		}
	}
}
