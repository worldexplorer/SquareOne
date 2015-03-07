using System;
using System.ComponentModel;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.Optimization;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl {
		void btnRunCancel_Click(object sender, EventArgs e) {
			if (this.optimizer.IsRunningNow) {
				this.optimizer.OptimizationAbort();
				this.btnPauseResume.Enabled = false;
				return;
			}
			
			string staleReason = this.optimizer.StaleReason;
			//bool clickedToClearAndPrepareForNewOptimization = string.IsNullOrEmpty(staleReason) == false;
			//if (clickedToClearAndPrepareForNewOptimization) {
			//    //this.backtests.Clear();
			//    //this.olvBacktests.SetObjects(this.backtests, true);
			//    this.SyncBacktestAndListWithOptimizationResultsByContextIdent();
			//    this.optimizer.ClearIWasRunFor();
			//    this.PopulateTextboxesFromExecutorsState();
			//    this.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
			//    this.btnPauseResume.Text = "Pause/Resume";
			//    return;
			//}
			
			this.backtests.Clear();
			this.olvBacktests.SetObjects(this.backtests, true);
			//this.olvBacktests.RebuildColumns();
			//this.olvBacktests.BuildList();
			int threadsLaunched = this.optimizer.OptimizationRun();
			this.btnRunCancel.Text = "Cancel " + this.optimizer.BacktestsRemaining + " backtests";
			//this.btnPauseResume.Enabled = true;
			this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
			//this.olvBacktests.UseWaitCursor = true;
			try {
				this.splitContainer1.SplitterDistance = this.heightCollapsed;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}
//		void optimizer_OnBacktestStarted(object sender, EventArgs e) {
//			if (base.InvokeRequired) {
//				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnBacktestStarted(sender, e); });
//				return;
//			}
//			this.splitContainer1.SplitterDistance = heightCollapsed;
//		}
		void optimizer_OnOneBacktestFinished(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnOneBacktestFinished(sender, e); });
				return;
			}
			this.backtests.Add(e.SystemPerformanceRestoreAble);
			this.olvBacktests.SetObjects(this.backtests, true); //preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			//this.olvBacktests.Refresh();
			
			int backtestsRemaninig	= this.optimizer.BacktestsRemaining;
			int backtestsTotal		= this.optimizer.BacktestsTotal;
			int backtestsCompleted	= this.optimizer.BacktestsCompleted;  
			this.btnRunCancel.Text = "Cancel " + backtestsRemaninig + " backtests";
			double pctComplete = (backtestsTotal > 0) ? Math.Round(100 * backtestsCompleted / (double)backtestsTotal) : 0;
			this.lblStats.Text = pctComplete + "% complete    " + backtestsCompleted + "/" + backtestsTotal;
			if (backtestsCompleted >= this.progressBar1.Minimum && backtestsCompleted <= this.progressBar1.Maximum) {
				this.progressBar1.Value = backtestsCompleted;
			}
			this.btnPauseResume.Text = this.optimizer.BacktestsSecondsElapsed + " seconds elapsed";
		}
		void optimizer_OnAllBacktestsFinished(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnAllBacktestsFinished(sender, e); });
				return;
			}
			this.btnPauseResume.Enabled = false;
			int totalBacktests = this.optimizer.BacktestsTotal;
			this.btnRunCancel.Text = "Run " + totalBacktests + " backtests";
			this.olvBacktests.EmptyListMsg = "";
			//this.lblStats.Text = "0% complete   0/" + totalBacktests;
			//this.progressBar1.Value = 0;
			//this.olvBacktests.UseWaitCursor = false;
			this.btnPauseResume.Text = this.optimizer.BacktestsSecondsElapsed + " seconds elapsed";
			
			Strategy strategy = this.optimizer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//v1
			//if (strategy.OptimizationResultsByContextIdent.ContainsKey(symbolScaleRange) == false) {
			//    strategy.OptimizationResultsByContextIdent.Add(symbolScaleRange, this.backtests);
			//} else {
			//    strategy.OptimizationResultsByContextIdent[symbolScaleRange] = this.backtests;
			//}
			//strategy.Serialize();
			//v2
			this.RepositoryJsonOptimizationResults.SerializeList(this.backtests, symbolScaleRange);
			this.olvHistoryRescanRefillSelect(symbolScaleRange);
			try {
				this.splitContainer1.SplitterDistance = this.heightExpanded;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}
		void optimizer_OnOptimizationAborted(object sender, EventArgs e) {
			this.optimizer_OnAllBacktestsFinished(sender, e);
		}
        void optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild(object sender, EventArgs e) {
            this.txtScriptParameterTotalNr.Text = this.optimizer.ScriptParametersTotalNr.ToString();
            this.txtIndicatorParameterTotalNr.Text = this.optimizer.IndicatorParameterTotalNr.ToString();
            int backtestsTotal = this.optimizer.BacktestsTotal;
            this.btnRunCancel.Text = "Run " + backtestsTotal + " backtests";
        }
		void btnPauseResume_Click(object sender, EventArgs e) {
			Assembler.PopupException(null, new NotImplementedException());
		}
		void nudCpuCoresToUse_ValueChanged(object sender, EventArgs e) {
			int newValue = (int)this.nudThreadsToRun.Value;
			int valueMin = 1;
			int valueMax = this.optimizer.CpuCoresAvailable * 2;
			if (newValue < valueMin) {
				newValue = valueMin;
				this.nudThreadsToRun.Value = newValue;
			}
			if (newValue > valueMax) {
				newValue = valueMax;
				this.nudThreadsToRun.Value = valueMax;
			}
			this.optimizer.ThreadsToUse = newValue;
		}
		void mniCopyToDefaultCtxBacktest_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtxBacktest_Click()";
			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
			if (selectedClone == null) return;
			this.RaiseOnCopyToContextDefaultBacktest(selectedClone);
		}
		void mniCopyToDefaultCtx_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtx_Click()";
			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
			if (selectedClone == null) return;
			this.RaiseOnCopyToContextDefault(selectedClone);
		}
		void mniltbCopyToNewContext_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContext_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";
			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
			if (selectedClone == null) return;
			this.RaiseOnCopyToContextNew(selectedClone);
		}
		void mniltbCopyToNewContextBacktest_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContextBacktest_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";
			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
			if (selectedClone == null) return;
			this.RaiseOnCopyToContextNewBacktest(selectedClone);
		}
		
		ContextScript convertOptimizationResultToScriptContext(string msig) {
			SystemPerformanceRestoreAble sysPerfRestoreAble = (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject;
			if (sysPerfRestoreAble == null) {
				string msg = "IS_NULL (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return null;
			}
			
			#if DEBUG	// inline unittest
			foreach (OLVColumn olvc in this.columnsDynParam) {
				string iParamName = olvc.Text;

				var iDisplayedByName = this.optimizer.ScriptAndIndicatorParametersMergedByName;
				if (iDisplayedByName.ContainsKey(iParamName) == false) {
					string msg = "NEVER_HAPPENED_SO_FAR iDisplayedByName.ContainsKey(" +  iParamName + ") == false";
					Assembler.PopupException(msg);
				}
				IndicatorParameter iDisplayed = iDisplayedByName[iParamName];

				var iPropagatingByName = sysPerfRestoreAble.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				if (iPropagatingByName.ContainsKey(iParamName) == false) {
					string msg = "NEVER_HAPPENED_SO_FAR iPropagatingByName.ContainsKey(" + iParamName + ") == false";
					Assembler.PopupException(msg);
				}
				IndicatorParameter iPropagating = iPropagatingByName[iParamName];

				if (iDisplayed.ValueCurrent != iPropagating.ValueCurrent) {
					string msg = "both are wrong; I clicked on MaSlow=20,MaFast=11; iDisplayed=MaFast=33, iPropagating=MaFast=22; replacing executorPool with newExecutor() each backtest";
					Assembler.PopupException(msg, null, false);
				}
			}
			#endif

			if (sysPerfRestoreAble.ScriptParametersById_BuiltOnBacktestFinished == null) {
				string msg = "BACKTEST_WAS_ABORTED_CANT_POPUPLATE";
				Assembler.PopupException(msg + msig);
				return null;
			}
			ContextScript selectedClone = this.optimizer.Executor.Strategy.ScriptContextCurrent.CloneAndAbsorbFromSystemPerformanceRestoreAble(sysPerfRestoreAble);
			return selectedClone;
		}
		void olvBacktests_CellClick(object sender, CellClickEventArgs e) {
		}
		void olvBacktests_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;	// right click on the blank space (not on a row with data)
			e.MenuStrip = this.ctxOneBacktestResult;
		}
		void mniCopyToClipboard_Click(object sender, EventArgs e) {
		}
		void mniSaveCsv_Click(object sender, EventArgs e) {
		}
		void ctxOneBacktestResult_Opening(object sender, CancelEventArgs e) {
			string msig = " /ctxOneBacktestResult_Opening()";
			SystemPerformanceRestoreAble sysPerfRestoreAble = (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject;
			if (sysPerfRestoreAble == null) {
				string msg = "IS_NULL (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			
			string uniqueBacktestNumbers						= sysPerfRestoreAble.NetProfitRecovery;
			this.mniltbCopyToNewContext.InputFieldValue			= uniqueBacktestNumbers;
			this.mniltbCopyToNewContextBacktest.InputFieldValue	= uniqueBacktestNumbers;

			string stratSymbolScaleRange	= sysPerfRestoreAble.StrategyName + " " + sysPerfRestoreAble.SymbolScaleIntervalDataRange;
			this.mniInfo.Text				= uniqueBacktestNumbers + " => " + stratSymbolScaleRange;
		}
		void olvHistory_ItemActivate(object sender, EventArgs e) {
			FnameDateSizeColor fname = this.olvHistory.SelectedObject as FnameDateSizeColor;
			if (fname == null) return;
			this.backtests = this.RepositoryJsonOptimizationResults.DeserializeList(fname.Name);
			this.olvBacktests.SetObjects(this.backtests, true);
		}
	}
}
