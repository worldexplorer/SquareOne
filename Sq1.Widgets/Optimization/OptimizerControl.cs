using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl : UserControl {
		Optimizer optimizer;
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
//			this.olvBacktests.AllColumns.Add(this.olvcNetProfit);
//			this.olvBacktests.AllColumns.Add(this.olvcWinLoss);
//			this.olvBacktests.AllColumns.Add(this.olvcProfitFactor);
//			this.olvBacktests.AllColumns.Add(this.olvcRecoveryFactor);
//			this.olvBacktests.AllColumns.Add(this.olvcTotalTrades);
//			this.olvBacktests.AllColumns.Add(this.olvcAverageProfit);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxDrawdown);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveWinners);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveLosers);

			backtests = new List<SystemPerformance>();
			columnsDynParam = new List<OLVColumn>();
		}
		public void Initialize(Optimizer optimizer) {
			this.optimizer = optimizer;
			if (this.optimizer == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer == null";
				return;
			}
			if (this.optimizer.InitializedProperly == false) {
				this.olvBacktests.EmptyListMsg = "this.optimizerInitializedProperly == false";
				return;
			}

			// removing first to avoid reception of same SystemResults reception due to multiple Initializations by OptimizerControl.Initialize() 
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.optimizer.OnBacktestStarted -= new EventHandler<EventArgs>(optimizer_OnBacktestStarted);
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.optimizer.OnBacktestStarted += new EventHandler<EventArgs>(optimizer_OnBacktestStarted);
			
			this.optimizer.OnBacktestFinished -= new EventHandler<SystemPerformanceEventArgs>(this.optimizer_OnBacktestFinished);
			// since Optimizer.backtests is multithreaded list, I keep own copy here OptimizerControl.backtests for ObjectListView to freely crawl over it without interference (instead of providing Optimizer.BacktestsThreadSafeCopy)  
			this.optimizer.OnBacktestFinished += new EventHandler<SystemPerformanceEventArgs>(this.optimizer_OnBacktestFinished);
			
			this.optimizer.OnOptimizationComplete -= new EventHandler<EventArgs>(Optimizer_OnOptimizationComplete);
			this.optimizer.OnOptimizationComplete += new EventHandler<EventArgs>(Optimizer_OnOptimizationComplete);
			
			this.optimizer.OnOptimizationAborted -= new EventHandler<EventArgs>(Optimizer_OnOptimizationAborted);
			this.optimizer.OnOptimizationAborted += new EventHandler<EventArgs>(Optimizer_OnOptimizationAborted);

            this.optimizer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild -= new EventHandler<EventArgs>(Optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
            this.optimizer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild += new EventHandler<EventArgs>(Optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
            
            this.PopulateTextboxesFromExecutorsState();
			
			this.populateColumns();
			this.objectListViewCustomize();
		}
		public string PopulateTextboxesFromExecutorsState() {
			if (this.splitContainer1.SplitterDistance != this.heightExpanded) {
				this.splitContainer1.SplitterDistance  = this.heightExpanded;
			}
			
			string staleReason = this.optimizer.StaleReason;
			if (string.IsNullOrEmpty(staleReason) == false) {
				return staleReason;
			}
			
			this.txtDataRange.Text = this.optimizer.DataRangeAsString;
			this.txtPositionSize.Text = this.optimizer.PositionSizeAsString;
			this.txtStrategy.Text = this.optimizer.StrategyAsString;
			this.txtSymbol.Text = this.optimizer.SymbolScaleIntervalAsString;
			this.txtScriptParameterTotalNr.Text = this.optimizer.ScriptParametersTotalNr.ToString();
			this.txtIndicatorParameterTotalNr.Text = this.optimizer.IndicatorParameterTotalNr.ToString();

			int backtestsTotal = this.optimizer.BacktestsTotal;
			this.btnRunCancel.Text = "Run " + backtestsTotal + " backtests";
			this.lblStats.Text = "0% complete    0/" + backtestsTotal;
			this.progressBar1.Value = 0;
			this.progressBar1.Maximum = backtestsTotal;
			
			this.nudThreadsToRun.Value = this.optimizer.ThreadsToUse;
			
			this.btnRunCancel.Enabled = true;
			this.btnPauseResume.Enabled = false;

			return null;
		}
		void populateColumns() {
			//DONT_CLEAR_RESULTS_AFTER_TAB_SWITCHING_ONLY_RUN_WILL_CLEAR_OLD_TABLE this.olvBacktests.Items.Clear();
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
			
			
			if (this.optimizer == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer == null";
				return;
			}
			if (this.optimizer.ParametersById == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer.ParametersById == null";
				return;
			}
			
			var sparams = this.optimizer.ParametersById;
			if (sparams == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer.ParametersById == null";
				return;
			}
			foreach (ScriptParameter sp in sparams.Values) {
				OLVColumn olvcSP = new OLVColumn();
				olvcSP.Name = sp.Name;
				olvcSP.Text = sp.Name;
				olvcSP.Width = 85;
				olvcSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				this.olvBacktests.Columns.Add(olvcSP);
				//this.olvBacktests.AllColumns.Add(olvcSP);
				this.columnsDynParam.Add(olvcSP);
			}
			
			var iparams = this.optimizer.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;
			if (iparams == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders == null";
				return;
			}
			
			foreach (string indicatorDotParameter in iparams.Keys) {
				OLVColumn olvcIP = new OLVColumn();
				olvcIP.Name = indicatorDotParameter;
				olvcIP.Text = indicatorDotParameter;
				olvcIP.Width = 85;
				olvcIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				this.olvBacktests.Columns.Add(olvcIP);
				//this.olvBacktests.AllColumns.Add(olvcIP);
				this.columnsDynParam.Add(olvcIP);
			}
		}
		
		int heightExpanded { get { return this.splitContainer1.Panel1MinSize * 5; } }
		int heightCollapsed { get { return this.splitContainer1.Panel1MinSize; } }
		public void NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize() {
			string staleReason = this.optimizer.StaleReason;
			this.txtStaleReason.Text = staleReason; // TextBox doesn't display "null" for null-string
			
			bool userClickedAnotherSymbolScaleIntervalRangePositionSize = string.IsNullOrEmpty(staleReason) == false;
			this.splitContainer1.Panel1.BackColor = userClickedAnotherSymbolScaleIntervalRangePositionSize
				? Color.LightSalmon : SystemColors.Control;
			this.splitContainer1.SplitterDistance = userClickedAnotherSymbolScaleIntervalRangePositionSize || this.backtests.Count == 0
			 	? this.heightExpanded : this.heightCollapsed;
			this.btnRunCancel.Text = userClickedAnotherSymbolScaleIntervalRangePositionSize
				? "Clear to Optimize" : "Run " + this.optimizer.BacktestsTotal + " backtests";
		}
	}
}
