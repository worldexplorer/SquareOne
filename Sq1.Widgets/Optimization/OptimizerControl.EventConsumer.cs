using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl {
		void btnRunCancel_Click(object sender, EventArgs e) {
			if (this.optimizer.IsRunningNow) {
				this.optimizer.OptimizationAbort();
				this.btnPauseResume.Enabled = false;
				return;
			}
			
			string staleReason = this.optimizer.StaleReason;
			bool clickedToClearAndPrepareForNewOptimization = string.IsNullOrEmpty(staleReason) == false;
			if (clickedToClearAndPrepareForNewOptimization) {
				this.backtests.Clear();
				this.olvBacktests.SetObjects(this.backtests, true);
				this.optimizer.ClearIWasRunFor();
				this.PopulateTextboxesFromExecutorsState();
				this.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
				this.btnPauseResume.Text = "Pause/Resume";
				return;
			}
			
			this.backtests.Clear();
			this.olvBacktests.SetObjects(this.backtests, true);
			int threadsLaunched = this.optimizer.OptimizationRun();
			this.btnRunCancel.Text = "Cancel " + this.optimizer.BacktestsRemaining + " backtests";
			//this.btnPauseResume.Enabled = true;
			this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
			//this.olvBacktests.UseWaitCursor = true;
			this.splitContainer1.SplitterDistance = this.heightCollapsed;
		}
//		void optimizer_OnBacktestStarted(object sender, EventArgs e) {
//			if (base.InvokeRequired) {
//				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnBacktestStarted(sender, e); });
//				return;
//			}
//			this.splitContainer1.SplitterDistance = heightCollapsed;
//		}
		void optimizer_OnBacktestComplete(object sender, SystemPerformanceEventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnBacktestComplete(sender, e); });
				return;
			}
			this.backtests.Add(e.SystemPerformance);
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
		void Optimizer_OnOptimizationComplete(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.Optimizer_OnOptimizationComplete(sender, e); });
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
		}
		void Optimizer_OnOptimizationAborted(object sender, EventArgs e) {
			this.Optimizer_OnOptimizationComplete(sender, e);
		}
        void Optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild(object sender, EventArgs e) {
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
			SystemPerformance perf = (SystemPerformance)this.olvBacktests.SelectedObject;
			if (perf == null) {
				string msg = "IS_NULL (SystemPerformance)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selected = perf.Executor.Strategy.ScriptContextCurrent;
			if (selected == null) {
				string msg = "IS_NULL (ContextScript) this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selectedClone = selected.CloneThatUserPushesFromOptimizerToStrategy(perf.EssentialsForScriptContextNewName);
			this.RaiseOnCopyToContextDefaultBacktest(selectedClone);
		}
		void mniCopyToDefaultCtx_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtx_Click()";
			SystemPerformance perf = (SystemPerformance)this.olvBacktests.SelectedObject;
			if (perf == null) {
				string msg = "IS_NULL (SystemPerformance)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selected = perf.Executor.Strategy.ScriptContextCurrent;
			if (selected == null) {
				string msg = "IS_NULL (ContextScript) this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			#if DEBUG	// inline unittest
			foreach (OLVColumn olvc in this.columnsDynParam) {
				string iParamName = olvc.Text;

				var iDisplayedByName = this.optimizer.ScriptAndIndicatorParametersMergedByName;
				if (iDisplayedByName.ContainsKey(iParamName) == false) {
					Debugger.Break();
				}
				IndicatorParameter iDisplayed = iDisplayedByName[iParamName];

				var iPropagatingByName = selected.ParametersMergedByName;
				if (iPropagatingByName.ContainsKey(iParamName) == false) {
					Debugger.Break();
				}
				IndicatorParameter iPropagating = iPropagatingByName[iParamName];

				if (iDisplayed.ValueCurrent != iPropagating.ValueCurrent) {
					Debugger.Break();	// both are wrong; I clicked on MaSlow=20,MaFast=11; iDisplayed=MaFast=33, iPropagating=MaFast=22; replacing executorPool with newExecutor() each backtest
				}
			}
			#endif
			ContextScript selectedClone = selected.CloneThatUserPushesFromOptimizerToStrategy(perf.EssentialsForScriptContextNewName);
			this.RaiseOnCopyToContextDefault(selectedClone);
		}
		void mniltbCopyToNewContext_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContext_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";

			SystemPerformance perf = (SystemPerformance)this.olvBacktests.SelectedObject;
			if (perf == null) {
				string msg = "IS_NULL (SystemPerformance)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selected = perf.Executor.Strategy.ScriptContextCurrent;
			if (selected == null) {
				string msg = "IS_NULL (ContextScript) this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selectedClone = selected.CloneThatUserPushesFromOptimizerToStrategy(perf.EssentialsForScriptContextNewName);
			this.RaiseOnCopyToContextNew(selectedClone);
		}
		void mniltbCopyToNewContextBacktest_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContextBacktest_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";

			SystemPerformance perf = (SystemPerformance)this.olvBacktests.SelectedObject;
			if (perf == null) {
				string msg = "IS_NULL (SystemPerformance)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selected = perf.Executor.Strategy.ScriptContextCurrent;
			if (selected == null) {
				string msg = "IS_NULL (ContextScript) this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			ContextScript selectedClone = selected.CloneThatUserPushesFromOptimizerToStrategy(perf.EssentialsForScriptContextNewName);
			this.RaiseOnCopyToContextNewBacktest(selectedClone);
		}
		void olvBacktests_CellClick(object sender, CellClickEventArgs e) {
		}
		void olvBacktests_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;	// right click on the blank space (not on a row with data)
			e.MenuStrip = this.ctxOneBacktestResult;
		}
		private void mniCopyToClipboard_Click(object sender, EventArgs e) {
		}
		private void mniSaveCsv_Click(object sender, EventArgs e) {
		}
		void ctxOneBacktestResult_Opening(object sender, CancelEventArgs e) {
			string msig = " /ctxOneBacktestResult_Opening()";
			SystemPerformance perf = (SystemPerformance)this.olvBacktests.SelectedObject;
			if (perf == null) {
				string msg = "IS_NULL (SystemPerformance)this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				return;
			}
			string stratIdent = perf.Executor.StrategyName;
			ContextScript selected = perf.Executor.Strategy.ScriptContextCurrent;
			if (selected == null) {
				string msg = "IS_NULL (ContextScript) this.olvBacktests.SelectedObject";
				Assembler.PopupException(msg + msig);
				//return;
			} else {
				stratIdent += " " + selected.ToStringEssentialsForScriptContextNewName();
			}
			string ident = perf.EssentialsForScriptContextNewName;
			this.mniInfo.Text = ident + " => " + stratIdent;
			this.mniltbCopyToNewContext.InputFieldValue = ident;
			this.mniltbCopyToNewContextBacktest.InputFieldValue = ident;
		}
	}
}
