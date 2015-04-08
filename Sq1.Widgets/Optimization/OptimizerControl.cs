using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Optimization;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl : UserControl {
				Optimizer							optimizer;
				List<string>						colMetricsShouldStay;
				List<SystemPerformanceRestoreAble>	backtestsLocalEasierToSync;
				List<OLVColumn>						columnsDynParams;
		public	RepositoryJsonOptimizationResults	RepositoryJsonOptimizationResults			{ get; private set;}
				List<IndicatorParameter>			scriptAndIndicatorParametersMergedCloned;

		public OptimizerControl() {
			InitializeComponent();
			this.colMetricsShouldStay = new List<string>() {
				this.olvcSerno.Name,
				this.olvcTotalPositions.Name,
				this.olvcProfitPerPosition.Name,
				this.olvcNetProfit.Name,
				this.olvcWinLoss.Name,
				this.olvcProfitFactor.Name,
				this.olvcRecoveryFactor.Name,
				this.olvcMaxDrawdown.Name,
				this.olvcMaxConsecutiveWinners.Name,
				this.olvcMaxConsecutiveLosers.Name
			};
			// IF_NOT_ADDED_CLICKING_PARAMETER_WILL_REMOVE_THESE_ADDED_FOREVER_OLV_GLITCH this will enable show/hide columns by right click on header
			this.olvBacktests.AllColumns.AddRange(new OLVColumn[] {
				this.olvcSerno,
				this.olvcTotalPositions,
				this.olvcProfitPerPosition,
				this.olvcNetProfit,
				this.olvcWinLoss,
				this.olvcProfitFactor,
				this.olvcRecoveryFactor,
				this.olvcMaxDrawdown,
				this.olvcMaxConsecutiveWinners,
				this.olvcMaxConsecutiveLosers});
//			this.olvBacktests.AllColumns.Add(this.olvcNetProfit);
//			this.olvBacktests.AllColumns.Add(this.olvcWinLoss);
//			this.olvBacktests.AllColumns.Add(this.olvcProfitFactor);
//			this.olvBacktests.AllColumns.Add(this.olvcRecoveryFactor);
//			this.olvBacktests.AllColumns.Add(this.olvcTotalTrades);
//			this.olvBacktests.AllColumns.Add(this.olvcAverageProfit);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxDrawdown);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveWinners);
//			this.olvBacktests.AllColumns.Add(this.olvcMaxConsecutiveLosers);

			//this.fastOLVparametersYesNoMinMaxStep.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.fastOLVparametersYesNoMinMaxStep_ItemCheck);

			backtestsLocalEasierToSync				= new List<SystemPerformanceRestoreAble>();
			columnsDynParams						= new List<OLVColumn>();
			RepositoryJsonOptimizationResults		= new RepositoryJsonOptimizationResults();
		}
		public void Initialize(Optimizer optimizer) {
			this.optimizer = optimizer;
			if (this.optimizer == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer == null";
				return;
			}
			if (this.optimizer.InitializedProperly_executorHasScript_readyToOptimize == false) {
				this.olvBacktests.EmptyListMsg = "this.optimizerInitializedProperly == false";
				return;
			}

			// removing first to avoid reception of same SystemResults reception due to multiple Initializations by OptimizerControl.Initialize() 
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.optimizer.OnBacktestStarted -= new EventHandler<EventArgs>(optimizer_OnBacktestStarted);
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.optimizer.OnBacktestStarted += new EventHandler<EventArgs>(optimizer_OnBacktestStarted);
			
			this.optimizer.OnOneBacktestFinished -= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.optimizer_OnOneBacktestFinished);
			// since Optimizer.backtests is multithreaded list, I keep own copy here OptimizerControl.backtests for ObjectListView to freely crawl over it without interference (instead of providing Optimizer.BacktestsThreadSafeCopy)  
			this.optimizer.OnOneBacktestFinished += new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.optimizer_OnOneBacktestFinished);
			
			this.optimizer.OnAllBacktestsFinished -= new EventHandler<EventArgs>(this.optimizer_OnAllBacktestsFinished);
			this.optimizer.OnAllBacktestsFinished += new EventHandler<EventArgs>(this.optimizer_OnAllBacktestsFinished);
			
			this.optimizer.OnOptimizationAborted -= new EventHandler<EventArgs>(this.optimizer_OnOptimizationAborted);
			this.optimizer.OnOptimizationAborted += new EventHandler<EventArgs>(this.optimizer_OnOptimizationAborted);

			this.optimizer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild -= new EventHandler<EventArgs>(this.optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
			this.optimizer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild += new EventHandler<EventArgs>(this.optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
			
			this.populateTextboxesFromExecutorsState();

			this.olvParametersCustomize();
			this.olvParameterPopulate();

			this.populateColumns();
			this.olvBacktestsCustomize();
		
			this.olvHistoryCustomize();
			this.RepositoryJsonOptimizationResults.Initialize(Assembler.InstanceInitialized.AppDataPath
				, Path.Combine("OptimizationResults", this.optimizer.Executor.Strategy.RelPathAndNameForOptimizationResults));
			string symbolScaleRange = this.optimizer.Executor.Strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//this.olvHistoryRescanRefillSelect(symbolScaleRange);
				
			this.SyncBacktestAndListWithOptimizationResultsByContextIdent();
		}
		void olvParameterPopulate() {
			this.scriptAndIndicatorParametersMergedCloned = this.optimizer.Executor.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders;
			this.olvPrameters.SetObjects(this.scriptAndIndicatorParametersMergedCloned);
		}
		void olvHistoryRescanRefillSelect(string symbolScaleRange) {
			this.RepositoryJsonOptimizationResults.RescanFolderStoreNamesFound();
			this.olvHistoryComputeAverage();
			this.olvHistory.SetObjects(this.RepositoryJsonOptimizationResults.ItemsFound);
			FnameDateSizeColor found = null;
			foreach (FnameDateSizeColor each in this.RepositoryJsonOptimizationResults.ItemsFound) {
				if (each.Name != symbolScaleRange) continue;
				found = each;
				break;
			}
			if (found == null) {
				this.olvHistory.SelectedIndex = -1;
			} else {
				this.olvHistory.SelectObject(found, true);
				this.olvHistory.RefreshSelectedObjects();
			}
		}
		void olvHistoryComputeAverage() {
			foreach (FnameDateSizeColor each in this.RepositoryJsonOptimizationResults.ItemsFound) {
				double profitFactorTotal = 0;		// netProfit could overflow outside double for 1000000000 backtests in one deserialized list; profit factor is expected [-10...10];
				int backtestsInOptimization = 0;
				List<SystemPerformanceRestoreAble> eachOptimization = this.RepositoryJsonOptimizationResults.DeserializeList(each.Name);
				foreach (SystemPerformanceRestoreAble backtest in eachOptimization) {
					if (double.IsNaN(backtest.ProfitFactor)) continue;
					if (double.IsPositiveInfinity(backtest.ProfitFactor)) continue;
					if (double.IsNegativeInfinity(backtest.ProfitFactor)) continue;
					profitFactorTotal += backtest.ProfitFactor;
					backtestsInOptimization++;
				}
				double profitFactorAverage = 0;
				if (backtestsInOptimization > 0) {		// AVOIDING_OUR_GOOD_FRIEND_DIVISION_TO_ZERO_EXCEPTION
					profitFactorAverage = profitFactorTotal / (double)backtestsInOptimization;
				}
				each.PFavg = Math.Round(profitFactorAverage, 2);
				if (profitFactorTotal == 0) {
					string msg= "SystemPerformanceRestoreAble didn't have ProfitFactor calculated/deserialized for[" + each + "] //olvHistoryComputeAverage()";
					Assembler.PopupException(msg, null, false);
				}
			}
		}
		public void SyncBacktestAndListWithOptimizationResultsByContextIdent() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.SyncBacktestAndListWithOptimizationResultsByContextIdent(); });
				return;
			}
			Strategy strategy = this.optimizer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//v1
			//if (strategy.OptimizationResultsByContextIdent.ContainsKey(symbolScaleRange)) {
			//	this.backtests = strategy.OptimizationResultsByContextIdent[symbolScaleRange];
			//} else {
			//	this.backtests.Clear();
			//}
			//v2
			if (this.RepositoryJsonOptimizationResults.ItemsFoundContainsName(symbolScaleRange)) {
				this.backtestsLocalEasierToSync = this.RepositoryJsonOptimizationResults.DeserializeList(symbolScaleRange);
			} else {
				this.backtestsLocalEasierToSync.Clear();
			}
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync); //preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			this.olvHistoryRescanRefillSelect(symbolScaleRange);
			this.populateTextboxesFromExecutorsState();
		}

		void populateTextboxesFromExecutorsState() {
			if (this.splitContainer1.SplitterDistance != this.heightExpanded) {
				this.splitContainer1.SplitterDistance  = this.heightExpanded;
			}
			
			this.cbxRunCancel.Enabled				= true;
			this.cbxPauseResume.Enabled				= false;

			//string staleReason = this.optimizer.StaleReason;
			//if (string.IsNullOrEmpty(staleReason) == false) {
			//	return staleReason;
			//}
			//this.lblStats.Text				= staleReason;
			this.txtDataRange.Text			= this.optimizer.DataRangeAsString;
			this.txtPositionSize.Text		= this.optimizer.PositionSizeAsString;
			this.txtStrategy.Text			= this.optimizer.StrategyAsString;
			this.txtSymbol.Text				= this.optimizer.SymbolScaleIntervalAsString;
			this.txtSpread.Text				= this.optimizer.SpreadPips;
			this.txtQuotesGenerator.Text	= this.optimizer.BacktestStrokesPerBar;
			this.totalsPropagateAdjustSplitterDistance();
		}

		void totalsPropagateAdjustSplitterDistance() {
			this.txtScriptParameterTotalNr.Text = this.optimizer.ScriptParametersTotalNr.ToString();
			this.txtIndicatorParameterTotalNr.Text = this.optimizer.IndicatorParameterTotalNr.ToString();

			int backtestsTotal = this.optimizer.BacktestsTotal;
			this.cbxRunCancel.Text = "Run " + backtestsTotal + " backtests";
			this.cbxRunCancel.Enabled = backtestsTotal > 0 ? true : false;
			this.lblStats.Text = "0% complete   0/" + backtestsTotal;
			this.progressBar1.Value = 0;
			this.progressBar1.Maximum = (backtestsTotal != -1) ? backtestsTotal : 0;

			this.nudThreadsToRun.Value = this.optimizer.ThreadsToUse;

			if (backtestsTotal == -1) {
				this.olvBacktests.EmptyListMsg = "RUN_-1_BACKTESTS_IS_DUE_TO MULTIPLICATION_OF_POSSIBLE_PARAMETERS"
					+ " IS_MORE_THAN_2,14_BLN_COMBINATIONS WENT_OUT_OF_INT32_CAPACITY"
					+ " SEE_EXCEPTIONS_FORM_FOR_OFFENSIVE_PARAMETERS DECREASE_RANGE_OR_INCREASE_STEP_FOR_xxx RECOMPILE";
			}

			this.adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled();
		}

		void adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled() {
			//int rowsShown = this.fastOLVparametersYesNoMinMaxStep.RowsPerPage;
			int splitterDistanceForThreeLines = 212;
			int allParameterLinesToDraw = this.optimizer.AllParameterLinesToDraw;
			int heightEachNewLine = this.olvPrameters.RowHeightEffective;
			if (this.olvPrameters.GridLines) heightEachNewLine++;
			int inAdditionToThree = (allParameterLinesToDraw - 3) * heightEachNewLine;
			this.heightExpanded = splitterDistanceForThreeLines + inAdditionToThree;
			
			if (allParameterLinesToDraw <= 3) {
				this.splitContainer1.SplitterDistance = splitterDistanceForThreeLines;
				return;
			}
			this.statsAndHistoryExpand();
		}
		void populateColumns() {
			//DONT_CLEAR_RESULTS_AFTER_TAB_SWITCHING_ONLY_RUN_WILL_CLEAR_OLD_TABLE this.olvBacktests.Items.Clear();
			this.columnsDynParams.Clear();
			
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
			SortedDictionary<int, ScriptParameter> sparams = this.optimizer.Executor.Strategy.Script.ScriptParametersById_ReflectedCached;
			if (sparams == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer.ExecutorCloneToBeSpawned.Strategy.Script.ScriptParametersById_ReflectedCached == null";
				return;
			}
			Dictionary<string, IndicatorParameter> iparams = this.optimizer.Executor.Strategy.Script.IndicatorsParameters_ReflectedCached;
			if (iparams == null) {
				this.olvBacktests.EmptyListMsg = "this.optimizer.ExecutorCloneToBeSpawned.Strategy.Script.IndicatorsByName_ReflectedCached == null";
				return;
			}

			foreach (ScriptParameter sp in sparams.Values) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD if (this.showAllScriptIndicatorParametersInOptimizationResults == false) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	if (sp.WillBeSequencedDuringOptimization == false) continue;
				//CHANGING_COLUMN_VISIBILITY_INSTEAD }
				OLVColumn olvcSP = new OLVColumn();
				olvcSP.Name = sp.Name;
				olvcSP.Text = sp.Name;
				olvcSP.Width = 85;
				olvcSP.TextAlign = HorizontalAlignment.Right;
				olvcSP.IsVisible = sp.WillBeSequencedDuringOptimization;
				this.olvBacktests.Columns.Add(olvcSP);
				this.olvBacktests.AllColumns.Add(olvcSP);
				this.columnsDynParams.Add(olvcSP);
			}
			
			foreach (string indicatorDotParameter in iparams.Keys) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD if (this.showAllScriptIndicatorParametersInOptimizationResults == false) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	IndicatorParameter ip = iparams[indicatorDotParameter];
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	if (ip.WillBeSequencedDuringOptimization == false) continue;
				//CHANGING_COLUMN_VISIBILITY_INSTEAD }
				OLVColumn olvcIP = new OLVColumn();
				olvcIP.Name = indicatorDotParameter;
				olvcIP.Text = indicatorDotParameter;
				olvcIP.Width = 85;
				olvcIP.TextAlign = HorizontalAlignment.Right;
				IndicatorParameter ip = iparams[indicatorDotParameter];
				olvcIP.IsVisible = ip.WillBeSequencedDuringOptimization;
				this.olvBacktests.Columns.Add(olvcIP);
				this.olvBacktests.AllColumns.Add(olvcIP);
				this.columnsDynParams.Add(olvcIP);
			}
			this.olvBacktests.RebuildColumns();	// OTHERWIZE_FIRST_TIME_SHOWN_INVISIBLES_ARE_VISIBLE__TIRED_OF_OLV_ILLOGICALITIES_RRRR
		}
		
		int heightExpanded;	//REPLACED_BY_this.adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled() { get { return this.splitContainer1.Panel1MinSize * 8; } }
		int heightCollapsed { get { return this.splitContainer1.Panel1MinSize; } }
		public void NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize() {
			Strategy strategy = this.optimizer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			if (this.RepositoryJsonOptimizationResults.ItemsFoundContainsName(symbolScaleRange)) return;

			//string staleReason = this.optimizer.StaleReason;
			//this.lblStats.Text = staleReason; // TextBox doesn't display "null" for null-string
			
			//bool userClickedAnotherSymbolScaleIntervalRangePositionSize = string.IsNullOrEmpty(staleReason) == false;
			//this.splitContainer1.Panel1.BackColor = userClickedAnotherSymbolScaleIntervalRangePositionSize
			//    ? Color.LightSalmon : SystemColors.Control;
			//this.splitContainer1.SplitterDistance = userClickedAnotherSymbolScaleIntervalRangePositionSize || this.backtestsLocalEasierToSync.Count == 0
			//    ? this.heightExpanded : this.heightCollapsed;
			//this.cbxRunCancel.Text = userClickedAnotherSymbolScaleIntervalRangePositionSize
			//    ? "Clear to Optimize" : "Run " + this.optimizer.BacktestsTotal + " backtests";
		}
		
		void statsAndHistoryExpand() {
			try {
				this.splitContainer1.SplitterDistance = this.heightExpanded;
				this.cbxExpandCollapse.Text = "-";
				this.cbxExpandCollapse.Checked = true;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}
		void statsAndHistoryCollapse() {
			try {
				this.splitContainer1.SplitterDistance = this.heightCollapsed;
				this.cbxExpandCollapse.Text = "+";
				this.cbxExpandCollapse.Checked = false;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}
	}
}
