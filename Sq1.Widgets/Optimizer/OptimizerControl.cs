using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.Optimizer {
	public partial class OptimizerControl : UserControl {
		ScriptExecutor executor;
		List<string> colMetricsShouldStay;
		List<SystemPerformance> backtests;
		List<OLVColumn> columnsDynParam;

		public OptimizerControl() {
			InitializeComponent();
			colMetricsShouldStay = new List<string>() {
				this.olvcNetProfit.Name,
				this.olvcWinLoss.Name,
				this.olvcProfitFactor.Name,
				this.olvcRecoveryFactor.Name,
				this.olvcTotalTrades.Name,
				this.olvcAverageProfit.Name,
				this.olvcMaxDrawdown.Name,
				this.olvcMaxConsecutiveWinners.Name,
				this.olvcMaxConsecutiveLosers.Name
			};
			// this will enable show/hide columns by right click on header
//			this.olvBacktests.AllColumns.AddRange(new OLVColumn[] {
//				this.olvcNetProfit,
//				this.olvcWinLoss,
//				this.olvcProfitFactor,
//				this.olvcRecoveryFactor,
//				this.olvcTotalTrades,
//				this.olvcAverageProfit,
//				this.olvcMaxDrawdown,
//				this.olvcMaxConsecutiveWinners,
//				this.olvcMaxConsecutiveLosers});
			this.olvBacktests.AllColumns.Add(this.olvcNetProfit);
			this.olvBacktests.AllColumns.Add(this.olvcWinLoss);
			this.olvBacktests.AllColumns.Add(this.olvcProfitFactor);
			this.olvBacktests.AllColumns.Add(this.olvcRecoveryFactor);
			this.olvBacktests.AllColumns.Add(this.olvcTotalTrades);
			this.olvBacktests.AllColumns.Add(this.olvcAverageProfit);
			this.olvBacktests.AllColumns.Add(this.olvcMaxDrawdown);
			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveWinners);
			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveLosers);

			backtests = new List<SystemPerformance>();
			columnsDynParam = new List<OLVColumn>();
		}

		public void Initialize(ScriptExecutor executor) {
			this.executor = executor;
			if (this.executor == null) {
				this.olvBacktests.EmptyListMsg = "this.executor == null";
				return;
			}
			if (this.executor.Strategy == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.Strategy == null";
				return;
			}
			this.olvBacktests.EmptyListMsg = "";

			Strategy strategy = this.executor.Strategy;
			this.txtDataRange.Text = strategy.ScriptContextCurrent.DataRange.ToString();
			this.txtPositionSize.Text = strategy.ScriptContextCurrent.PositionSize.ToString();
			this.txtStrategy.Text = strategy.Name + "   " + strategy.ScriptParametersAsStringByIdJSONcheck + strategy.IndicatorParametersAsStringByIdJSONcheck;
			this.txtSymbol.Text = strategy.ScriptContextCurrent.DataSourceName + " :: " + strategy.ScriptContextCurrent.Symbol;

			int scriptParametersTotalNr = 0;
			foreach (ScriptParameter sp in executor.Strategy.Script.ParametersById.Values) {
				scriptParametersTotalNr += sp.NumberOfRuns;
			}
			this.txtScriptParameterTotalNr.Text = scriptParametersTotalNr.ToString();
			
			int indicatorParameterTotalNr = 0;
			//foreach (Indicator i in executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {	//looks empty on Deserialization
			foreach (IndicatorParameter ip in executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
				indicatorParameterTotalNr += ip.NumberOfRuns;
			}
			this.txtIndicatorParameterTotalNr.Text = indicatorParameterTotalNr.ToString();

			int totalBacktests = (int)Math.Pow(scriptParametersTotalNr + indicatorParameterTotalNr, 2);
			this.btnRunCancel.Enabled = true;
			this.btnRunCancel.Text = "Run " + totalBacktests + " backtests";
			this.lblStats.Text = "0% complete; 0/" + totalBacktests;
			this.progressBar1.Value = 0;
			this.progressBar1.Maximum = totalBacktests;
			btnRunCancel.Enabled = true;
			
			this.populateColumns();
			this.objectListViewCustomize();
		}
		
		void populateColumns() {
			this.olvBacktests.Items.Clear();
			this.columnsDynParam.Clear();
			
			List<OLVColumn> colParametersToClear = new List<OLVColumn>();
			foreach (OLVColumn col in this.olvBacktests.Columns) {
				if (this.colMetricsShouldStay.Contains(col.Name)) continue;
				colParametersToClear.Add(col);
			}
			foreach (OLVColumn col in colParametersToClear) {
				this.olvBacktests.Columns.Remove(col);
				this.olvBacktests.AllColumns.Remove(col);
			}
			
			
			if (this.executor == null) {
				this.olvBacktests.EmptyListMsg = "this.executor == null";
				return;
			}
			if (this.executor.Strategy == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.Strategy == null";
				return;
			}
			if (this.executor.Strategy.Script == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.Strategy.Script == null";
				return;
			}
			if (this.executor.Strategy.Script.ParametersById == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.Strategy.Script.ParametersById == null";
				return;
			}
			foreach (ScriptParameter sp in this.executor.Strategy.Script.ParametersById.Values) {
				OLVColumn olvcSP = new OLVColumn();
				olvcSP.Name = sp.Name;
				olvcSP.Text = sp.Name;
				olvcSP.Width = 85;
				olvcSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				this.olvBacktests.Columns.Add(olvcSP);
				this.olvBacktests.AllColumns.Add(olvcSP);
				this.columnsDynParam.Add(olvcSP);
			}
			

			if (this.executor.ExecutionDataSnapshot == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.ExecutionDataSnapshot == null";
				return;
			}
//			if (this.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null) {
//				this.olvBacktests.EmptyListMsg = "this.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null";
//				return;
//			}
			if (this.executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null) {
				this.olvBacktests.EmptyListMsg = "this.executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null";
				return;
			}
			
			foreach (string indicatorDotParameter in this.executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Keys) {
				OLVColumn olvcIP = new OLVColumn();
				olvcIP.Name = indicatorDotParameter;
				olvcIP.Text = indicatorDotParameter;
				olvcIP.Width = 85;
				olvcIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				this.olvBacktests.Columns.Add(olvcIP);
				this.olvBacktests.AllColumns.Add(olvcIP);
				this.columnsDynParam.Add(olvcIP);
			}
		}
	}
}
