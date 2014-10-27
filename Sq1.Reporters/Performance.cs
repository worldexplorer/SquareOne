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
		public override void BuildOnceAfterFullBlindBacktestFinished(SystemPerformance performance) {
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
			IList<Position> positionsAllReadOnly = slice.PositionsImTrackingReadOnly;  

			// NO_FORMATTING_PRINT_AS_IT_IS !!!! YOULL_NEVER_FIND_ROUNDING_ERROR_IF_YOU_ROUND_JUST_BEFORE_PRINTING
			this.addCurrency("Net Profit", slice.NetProfitForClosedPositionsBoth, Color.Empty, Color.Empty, this.getLviForeColor(slice.NetProfitForClosedPositionsBoth), FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Win/Loss Ratio", slice.WinLossRatio, this.getLviForeColor(slice.WinLossRatio, 1));
			this.addNumeric("Profit Factor", slice.ProfitFactor, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric("Recovery Factor", slice.RecoveryFactor, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addNumeric("Payoff Ratio", slice.PayoffRatio, this.getLviForeColor(slice.ProfitFactor, 1));
			this.addCurrency("Commission", -slice.CommissionBoth, this.getLviForeColor(-slice.CommissionBoth));

			this.addNumeric("All Trades", slice.PositionsCountBoth, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Avg Profit", slice.AvgProfitBoth, this.getLviForeColor(slice.AvgProfitBoth));
			this.addPercent("Avg Profit %", slice.AvgProfitPctBoth, this.getLviForeColor(slice.AvgProfitPctBoth));
			this.addNumeric("Avg Bars Held", slice.AvgBarsHeldBoth);
			this.addNumeric("Profit per Bar", slice.ProfitPerBarBoth, this.getLviForeColor(slice.NetProfitForClosedPositionsBoth));
			this.addCurrency("Max Drawdown", slice.MaxDrawDown, Color.Empty, Color.Empty, this.getLviForeColor(slice.MaxDrawDown));
			this.addDateTime("Max Drawdown Date", slice.MaxDrawDownLastLossDate);

			this.addNumeric("Winners", (double)slice.PositionsCountWinners, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Win Rate", slice.WinRatePct, this.getLviForeColor(slice.WinRatePct, 50));
			this.addCurrency("Net Profit", slice.NetProfitWinners, this.getLviForeColor(slice.NetProfitWinners));
			this.addNumeric("Avg Profit", slice.AvgProfitWinners, this.getLviForeColor(slice.AvgProfitWinners));
			this.addPercent("Avg Profit %", slice.AvgProfitPctWinners, this.getLviForeColor(slice.AvgProfitPctWinners));
			this.addNumeric("Avg Bars Held", slice.AvgBarsHeldWinners);
			this.addNumeric("Max Consecutive Winners", (double)slice.MaxConsecWinners);

			this.addNumeric("Losers", (double)slice.PositionsCountLosers, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.addNumeric("Loss Rate", slice.LossRatePct, this.getLviForeColor(slice.LossRatePct, 50));
			this.addCurrency("Net Loss", slice.NetLossLosers, this.getLviForeColor(slice.NetLossLosers));
			this.addNumeric("Avg Loss", slice.AvgLossLosers, this.getLviForeColor(slice.AvgLossLosers));
			this.addPercent("Avg Loss %", slice.AvgLossPctLosers, this.getLviForeColor(slice.AvgLossPctLosers));
			this.addNumeric("Avg Bars Held", slice.AvgBarsHeldLosers);
			this.addNumeric("Max Consecutive Losses", (double)slice.MaxConsecLosers);
		}
		
		Color getLviForeColor(double value, double ethalonRedIfLessBlueIfGreater = 0.0) {
			if (value == ethalonRedIfLessBlueIfGreater) return this.ForeColor;
			return (value > ethalonRedIfLessBlueIfGreater) ? Color.Blue : Color.Red;
		}
		void addCurrency(string label, double value, Color itemFontColor) {
			this.addCurrency(label, value, Color.Empty, Color.Empty, itemFontColor);
		}
		void addCurrency(string label, double value,
				Color backColor, Color labelFontColor, Color itemFontColor,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			string format = systemPerformance.Bars.SymbolInfo.FormatPrice;
			string valueFormatted = value.ToString(format);
			this.RenderCell(label, valueFormatted, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addNumeric(string label, double value) {
			this.RenderCell(label, value.ToString(), Color.Empty, Color.Empty, Color.Empty);
		}
		void addNumeric(string label, double value, Color itemFontColor) {
			this.RenderCell(label, value.ToString(), Color.Empty, Color.Empty, itemFontColor);
		}
		void addNumeric(string label, double value, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			this.RenderCell(label, value.ToString(), backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void addPercent(string label, double value, Color itemFontColor) {
			this.RenderCell(label, value.ToString(), Color.Empty, Color.Empty, itemFontColor);
		}
		void addDateTime(string label, DateTime value) {
			this.RenderCell(label, value.ToShortDateString() + " " + value.ToShortTimeString(), Color.Empty, Color.Empty, Color.Empty);
		}
		void RenderCell(string label, string valueAlreadyFormatted,
				Color backColor, Color labelFontColor, Color itemFontColor,
				FontStyle labelFontStyle = FontStyle.Regular, FontStyle itemFontStyle = FontStyle.Regular) {
			ListViewItem lvi;
			if (this.currentColumn == 0) {
				lvi = this.lvPerformance.Items.Add(label);
				lvi.UseItemStyleForSubItems = false;
				lvi.ForeColor = labelFontColor;
				if (backColor != Color.Empty) {
					lvi.BackColor = backColor;
				}
				if (this.fontsByStyle.ContainsKey(labelFontStyle)) {
					lvi.Font = this.fontsByStyle[labelFontStyle];
				} else {
					Font font = new Font(this.Font, labelFontStyle);
					this.fontsByStyle.Add(labelFontStyle, font);
					lvi.Font = font;
				}
			} else {
				if (this.currentRow >= this.lvPerformance.Items.Count) {
					//Debugger.Break();
					return;
				}
				lvi = this.lvPerformance.Items[this.currentRow];
				this.currentRow++;
			}
			lvi.SubItems.Add(valueAlreadyFormatted);
			if (backColor != Color.Empty) {
				lvi.SubItems[lvi.SubItems.Count - 1].BackColor = backColor;
			}
			if (itemFontColor != Color.Empty) {
				lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = itemFontColor;
			}
			if (this.fontsByStyle.ContainsKey(itemFontStyle) == false) {
				Font newFont = new Font(this.Font, itemFontStyle);
				this.fontsByStyle.Add(itemFontStyle, newFont);
			}
			lvi.SubItems[lvi.SubItems.Count - 1].Font = this.fontsByStyle[itemFontStyle];
		}
		void lvReport_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvPerformance.SelectedItems.Count != 1) return;
			ListViewItem listViewItem = this.lvPerformance.SelectedItems[0];
			string text = listViewItem.Text;
			text = text.Trim();
			listViewItem.ToolTipText = this.GetItemDescription(text);
		}
		string GetItemDescription(string itemName) {
			//int num = Performance.string_0.IndexOf(itemName + "=");
			//string text = Performance.string_0.Substring(num + itemName.Length + 1);
			//num = text.IndexOf('\n');
			//return text.Substring(0, num - 1);
			return "DUMMY_TEXT_TO_DESCRIBE_ITEM itemName[" + itemName + "]";
		}
		public override void BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit pokeUnit) {
		}
		void CtxPopupOpening(object sender, System.ComponentModel.CancelEventArgs e) {
			throw new NotImplementedException();
		}
		public override object CreateSnapshotToStoreInScriptContext() {
			return null;
		}
	}
}
