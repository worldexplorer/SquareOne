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
			WindowsFormsUtils.SetDoubleBuffered(this.olvReport);
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
				this.olvReport.BeginUpdate();
				this.olvReport.Items.Clear();
				
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
				this.olvReport.EndUpdate();
			}
		}

		void AdjustColumnSize() {
			foreach (ColumnHeader colHeader in this.olvReport.Columns) {
				colHeader.Width = -1;
			}
		}
		protected virtual void GenerateReportForOneColumn(SystemPerformanceSlice slice) {
			IList<Position> positionsAllReadOnly = slice.PositionsImTrackingReadOnly;  

			this.AddCurrencyValue("Net Profit", slice.NetProfitForClosedPositionsBoth, Color.Empty, Color.Empty, this.GetItemColor(slice.NetProfitForClosedPositionsBoth), FontStyle.Bold, FontStyle.Regular);
			this.AddNumericValue("Win/Loss Ratio", slice.WinLossRatio, 2, this.GetItemColor(slice.WinLossRatio, 1));
			this.AddNumericValue("Profit Factor", slice.ProfitFactor, 2, this.GetItemColor(slice.ProfitFactor, 1));
			this.AddNumericValue("Recovery Factor", slice.RecoveryFactor, 2, this.GetItemColor(slice.ProfitFactor, 1));
			this.AddNumericValue("Payoff Ratio", slice.PayoffRatio, 2, this.GetItemColor(slice.ProfitFactor, 1));
			this.AddCurrencyValue("Commission", -slice.CommissionBoth, this.GetItemColor(-slice.CommissionBoth));

			this.AddNumericValue("All Trades", slice.PositionsCountBoth, 0, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.AddNumericValue("Avg Profit", slice.AvgProfitBoth, 2, this.GetItemColor(slice.AvgProfitBoth));
			this.AddPercentValue("Avg Profit %", slice.AvgProfitPctBoth, 2, this.GetItemColor(slice.AvgProfitPctBoth));
			this.AddNumericValue("Avg Bars Held", slice.AvgBarsHeldBoth, 2);
			this.AddNumericValue("Profit per Bar", slice.ProfitPerBarBoth, 2, this.GetItemColor(slice.NetProfitForClosedPositionsBoth));
			this.AddCurrencyValue("Max Drawdown", slice.MaxDrawDown, Color.Empty, Color.Empty, this.GetItemColor(slice.MaxDrawDown), FontStyle.Regular, FontStyle.Regular);
			this.AddDateTimeValue("Max Drawdown Date", slice.MaxDrawDownLastLossDate);

			this.AddNumericValue("Winners", (double)slice.PositionsCountWinners, 0, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.AddPercentValue("Win Rate", slice.WinRatePct, 2, this.GetItemColor(slice.WinRatePct, 50));
			this.AddCurrencyValue("Net Profit", slice.NetProfitWinners, this.GetItemColor(slice.NetProfitWinners));
			this.AddNumericValue("Avg Profit", slice.AvgProfitWinners, 2, this.GetItemColor(slice.AvgProfitWinners));
			this.AddPercentValue("Avg Profit %", slice.AvgProfitPctWinners, 2, this.GetItemColor(slice.AvgProfitPctWinners));
			this.AddNumericValue("Avg Bars Held", slice.AvgBarsHeldWinners, 2);
			this.AddNumericValue("Max Consecutive Winners", (double)slice.MaxConsecWinners, 0);

			this.AddNumericValue("Losers", (double)slice.PositionsCountLosers, 0, Color.Gainsboro, Color.Empty, Color.Empty, FontStyle.Bold, FontStyle.Regular);
			this.AddPercentValue("Loss Rate", slice.LossRatePct, 2, this.GetItemColor(slice.LossRatePct, 50));
			this.AddCurrencyValue("Net Loss", slice.NetLossLosers, this.GetItemColor(slice.NetLossLosers));
			this.AddNumericValue("Avg Loss", slice.AvgLossLosers, 2, this.GetItemColor(slice.AvgLossLosers));
			this.AddPercentValue("Avg Loss %", slice.AvgLossPctLosers, 2, this.GetItemColor(slice.AvgLossPctLosers));
			this.AddNumericValue("Avg Bars Held", slice.AvgBarsHeldLosers, 2);
			this.AddNumericValue("Max Consecutive Losses", (double)slice.MaxConsecLosers, 0);
		}
		
		protected Color GetItemColor(double value, double ethalonRedIfLessBlueIfGreater = 0.0) {
			if (value == ethalonRedIfLessBlueIfGreater) return this.ForeColor;
			return (value > ethalonRedIfLessBlueIfGreater) ? Color.Blue : Color.Red;
		}
		protected void AddCurrencyValue(string label, double value, Color itemFontColor) {
			this.AddCurrencyValue(label, value, Color.Empty, Color.Empty, itemFontColor, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddCurrencyValue(string label, double value, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			string format = systemPerformance.Bars.SymbolInfo.FormatPrice;
			string valueFormatted = value.ToString(format);
			this.RenderCell(label, valueFormatted, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		protected void AddNumericValue(string label, double value, int decimalPlaces) {
			this.AddNumericValue(label, value, decimalPlaces, Color.Empty, Color.Empty, Color.Empty, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddNumericValue(string label, double value, int decimalPlaces, Color itemFontColor) {
			this.AddNumericValue(label, value, decimalPlaces, Color.Empty, Color.Empty, itemFontColor, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddNumericValue(string label, double value, int decimalPlaces, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			string valueFormatted = value.ToString("N" + decimalPlaces);
			this.RenderCell(label, valueFormatted, backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		protected void AddPercentValue(string label, double value, int decimalPlaces) {
			this.AddPercentValue(label, value, decimalPlaces, Color.Empty, Color.Empty, Color.Empty, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddPercentValue(string label, double value, int decimalPlaces, Color itemFontColor) {
			this.AddPercentValue(label, value, decimalPlaces, Color.Empty, Color.Empty, itemFontColor, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddPercentValue(string label, double value, int decimalPlaces, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			this.RenderCell(label, value.ToString("N" + decimalPlaces) + "%", backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		protected void AddDateTimeValue(string label, DateTime value) {
			this.AddDateTimeValue(label, value, Color.Empty, Color.Empty, Color.Empty, FontStyle.Regular, FontStyle.Regular);
		}
		protected void AddDateTimeValue(string label, DateTime value, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			this.RenderCell(label, value.ToShortDateString() + " " + value.ToShortTimeString(), backColor, labelFontColor, itemFontColor, labelFontStyle, itemFontStyle);
		}
		void RenderCell(string label, string value, Color backColor, Color labelFontColor, Color itemFontColor, FontStyle labelFontStyle, FontStyle itemFontStyle) {
			ListViewItem lvi;
			if (this.currentColumn == 0) {
				lvi = this.olvReport.Items.Add(label);
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
				if (this.currentRow >= this.olvReport.Items.Count) {
					//Debugger.Break();
					return;
				}
				lvi = this.olvReport.Items[this.currentRow];
				this.currentRow++;
			}
			lvi.SubItems.Add(value);
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
			if (this.olvReport.SelectedItems.Count != 1) return;
			ListViewItem listViewItem = this.olvReport.SelectedItems[0];
			string text = listViewItem.Text;
			text = text.Trim();
			listViewItem.ToolTipText = this.GetItemDescription(text);
		}
		protected virtual string GetItemDescription(string itemName) {
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
