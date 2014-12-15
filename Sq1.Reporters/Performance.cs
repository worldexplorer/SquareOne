using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Support;

namespace Sq1.Reporters {
	public partial class Performance : Reporter {
		private int currentColumn;
		private int currentRow;
		private Dictionary<FontStyle, Font> fontsByStyle;
		private SystemPerformance systemPerformance;

		public Performance(ChartShadow chart): this() {
			this.Initialize(chart, null);
		}
		public Performance() : base() {
			base.TabText = "Performance";
			this.InitializeComponent();
			this.fontsByStyle = new Dictionary<FontStyle, Font>();
			WindowsFormsUtils.SetDoubleBuffered(this.lvPerformance);
			this.objectListViewCustomize();
		}
		public override void BuildFullOnBacktestFinished(SystemPerformance performance) {
			this.systemPerformance = performance;
			this.propagatePerformanceReport(performance);
		}
		void propagatePerformanceReport(SystemPerformance performance) {
			DataSeriesTimeBased equityCurve = performance.SlicesShortAndLong.EquityCurve;
			this.fontsByStyle.Clear();
			this.fontsByStyle.Add(this.Font.Style, this.Font);
			try {
				this.lvPerformance.BeginUpdate();
				this.lvPerformance.Items.Clear();
				
				this.currentColumn = 0;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(performance.SlicesShortAndLong);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(performance.SliceLong);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(performance.SliceShort);
				
				this.currentColumn++;
				this.currentRow = 0;
				this.GenerateReportForOneColumn(performance.SliceBuyHold);
				
				if (performance.BenchmarkSymbolBars != null) {
					this.colBuyHold.Text = performance.BenchmarkSymbolBars.Symbol;
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
			IList<Position> positionsAllReadOnly = slice.PositionsImTrackingReadonly;  

			// NO_FORMATTING_PRINT_AS_IT_IS !!!! YOULL_NEVER_FIND_ROUNDING_ERROR_IF_YOU_ROUND_JUST_BEFORE_PRINTING
			this.addCurrency("Net Profit", "NetProfitForClosedPositionsBoth", slice.NetProfitForClosedPositionsBoth, Color.Empty, Color.Empty, this.getLviForeColor(slice.NetProfitForClosedPositionsBoth), FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Win/Loss Ratio", "Win/Loss Ratio = PositionsCountWinners / PositionsCountLosers", slice.WinLossRatio, this.getLviForeColor(slice.WinLossRatio, 1));
			this.addNumeric("Profit Factor", "Profit Factor = NetProfitWinners / Abs(NetLossLosers)", slice.ProfitFactor, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric("Recovery Factor", "Recovery Factor = Abs(NetProfitForClosedPositionsBoth / MaxDrawDown)", slice.RecoveryFactor, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric("Payout", "Payout = Abs(AvgProfitPctBoth / AvgLossPctLosers)", slice.PayoffRatio, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addCurrency("Commission", "-CommissionBoth", -slice.CommissionBoth, this.getLviForeColor(-slice.CommissionBoth));

			this.addNumeric("All Trades", "PositionsCountBoth", slice.PositionsCountBoth, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Avg Profit", "Avg Profit = NetProfitForClosedPositionsBoth / PositionsCountBoth", slice.AvgProfitBoth, this.getLviForeColor(slice.AvgProfitBoth));
			this.addPercent("Avg Profit %", "Avg Profit % = NetProfitPctForClosedPositionsBoth / PositionsCountBoth", slice.AvgProfitPctBoth, this.getLviForeColor(slice.AvgProfitPctBoth));
			this.addNumeric("Avg Bars Held", "Avg Bars Held = BarsHeldTotalForClosedPositionsBoth / PositionsCountBoth", slice.AvgBarsHeldBoth);
			this.addNumeric("Profit per Bar", "Profit per Bar = NetProfitForClosedPositionsBoth / BarsHeldTotalForClosedPositionsBoth", slice.ProfitPerBarBoth, this.getLviForeColor(slice.NetProfitForClosedPositionsBoth));
			this.addCurrency("Max Drawdown", "Max Drawdown = Min(NetProfitForClosedPositionsBoth - NetProfitPeak)", slice.MaxDrawDown, Color.Empty, Color.Empty, this.getLviForeColor(slice.MaxDrawDown));
			this.addDateTime("Max Drawdown Date", "Max Drawdown Date = Date(Max Drawdown)", slice.MaxDrawDownLastLossDate);

			this.addNumeric("Winners", "PositionsCountWinners", (double)slice.PositionsCountWinners, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Win Rate", "Win Rate = 100 * PositionsCountWinners / PositionsCountBoth", slice.WinRatePct, this.getLviForeColor(slice.WinRatePct, 50));
			this.addCurrency("Net Profit", "NetProfitWinners", slice.NetProfitWinners, this.getLviForeColor(slice.NetProfitWinners));
			this.addNumeric("Avg Profit", "Winners Avg Profit = NetProfitWinners / PositionsCountWinners", slice.AvgProfitWinners, this.getLviForeColor(slice.AvgProfitWinners));
			this.addPercent("Avg Profit %", "Winners Avg Profit % = NetProfitPctForClosedPositionsLong / PositionsCountWinners", slice.AvgProfitPctWinners, this.getLviForeColor(slice.AvgProfitPctWinners));
			this.addNumeric("Avg Bars Held", "Winners Avg Bars Held = BarsHeldTotalForClosedPositionsWinners / PositionsCountWinners", slice.AvgBarsHeldWinners);
			this.addNumeric("Max Consecutive Winners", "Winners Max Consecutive = MaxLen(exitPosition.NetProfit > 0)", (double)slice.MaxConsecWinners);

			this.addNumeric("Losers", "PositionsCountLosers", (double)slice.PositionsCountLosers, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Loss Rate", "Loss Rate = 100 * this.PositionsCountLosers / PositionsCountBoth", slice.LossRatePct, this.getLviForeColor(slice.LossRatePct, 50));
			this.addCurrency("Net Loss", "NetLossLosers", slice.NetLossLosers, this.getLviForeColor(slice.NetLossLosers));
			this.addNumeric("Avg Loss", "Losers Avg Loss = NetLossLosers / PositionsCountLosers", slice.AvgLossLosers, this.getLviForeColor(slice.AvgLossLosers));
			this.addPercent("Avg Loss %", "Losers Avg Loss % = NetProfitPctForClosedPositionsLosers / PositionsCountLosers", slice.AvgLossPctLosers, this.getLviForeColor(slice.AvgLossPctLosers));
			this.addNumeric("Avg Bars Held", "Losers Avg Bars Held = BarsHeldTotalForClosedPositionsLosers / PositionsCountLosers", slice.AvgBarsHeldLosers);
			this.addNumeric("Max Consecutive Losers", "Losers Max Consecutive = MaxLen(exitPosition.NetProfit < 0)", (double)slice.MaxConsecLosers);
		}
		
		Color getLviForeColor(double value, double ethalonRedIfLessBlueIfGreater = 0.0) {
			if (value == ethalonRedIfLessBlueIfGreater) return this.ForeColor;
			return (value > ethalonRedIfLessBlueIfGreater) ? Color.Blue : Color.Red;
		}
		void addCurrency(string label, string tooltip, double value, Color itemFontColor) {
			this.addCurrency(label, tooltip, value, Color.Empty, Color.Empty, itemFontColor);
		}
		void addCurrency(string label, string tooltip, double value,
				Color backColor, Color labelFontColor, Color itemFontColor,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			string format = systemPerformance.Bars.SymbolInfo.FormatPrice;
			string valueFormatted = value.ToString(format);
			this.addLvi(label, tooltip, valueFormatted, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addNumeric(string label, string tooltip, double value) {
			this.addLvi(label, tooltip, value.ToString(), Color.Empty, Color.Empty, Color.Empty);
		}
		void addNumeric(string label, string tooltip, double value, Color itemFontColor) {
			this.addLvi(label, tooltip, value.ToString(), Color.Empty, Color.Empty, itemFontColor);
		}
		void addNumeric(string label, string tooltip, double value, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			this.addLvi(label, tooltip, value.ToString(), backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addPercent(string label, string tooltip, double value, Color itemFontColor) {
			this.addLvi(label, tooltip, value.ToString(), Color.Empty, Color.Empty, itemFontColor);
		}
		void addDateTime(string label, string tooltip, DateTime value) {
			this.addLvi(label, tooltip, value.ToShortDateString() + " " + value.ToShortTimeString(), Color.Empty, Color.Empty, Color.Empty);
		}
		void addLvi(string label, string tooltip, string valueAlreadyFormatted,
				Color colorBack, Color colorForeLabel, Color colorFore,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			ListViewItem lvi;
			if (this.currentColumn == 0) {
				lvi = this.lvPerformance.Items.Add(label);
				lvi.ToolTipText = tooltip;
				lvi.UseItemStyleForSubItems = false;
				if (colorForeLabel != Color.Empty) lvi.ForeColor = colorForeLabel;
				if (colorBack != Color.Empty) lvi.BackColor = colorBack;
				if (this.fontsByStyle.ContainsKey(labelFontStyle) == false) {
					Font font = new Font(this.Font, labelFontStyle);
					this.fontsByStyle.Add(labelFontStyle, font);
				}
				lvi.Font = this.fontsByStyle[labelFontStyle];
			} else {
				if (this.currentRow >= this.lvPerformance.Items.Count) {
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}
				lvi = this.lvPerformance.Items[this.currentRow];
				this.currentRow++;
			}
			
			if (this.fontsByStyle.ContainsKey(itemFontStyle) == false) {
				Font newFont = new Font(this.Font, itemFontStyle);
				this.fontsByStyle.Add(itemFontStyle, newFont);
			}
			Font subLviFont = this.fontsByStyle[itemFontStyle];
			ListViewItem.ListViewSubItem subLvi = new ListViewItem.ListViewSubItem(lvi, valueAlreadyFormatted, colorFore, colorBack, subLviFont);
//			subLvi.ToolTipText = tooltip;
//			if (colorBack	!= Color.Empty) subLvi.BackColor = colorBack;
//			if (colorFore	!= Color.Empty) subLvi.ForeColor = colorFore;
			lvi.SubItems.Add(subLvi);
		}
		public override void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
		}
		public override object CreateSnapshotToStoreInScriptContext() {
			return null;
		}
		public override void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(List<Position> positionsUpdatedDueToStreamingNewQuote) {
		}
		public override void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
		}
	}
}
