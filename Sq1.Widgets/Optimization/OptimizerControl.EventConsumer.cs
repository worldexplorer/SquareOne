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
		void cbxRunCancel_Click(object sender, EventArgs e) {
			if (this.optimizer.IsRunningNow) {
				if (this.optimizer.Unpaused == false) {
					// WHILE_AVOIDING_DEADLOCK__DISABLE_BUTTONS_FOR_USER_NOT_TO_WORSEN__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
					this.cbxRunCancel.Enabled = false;
					this.cbxPauseResume.Enabled = false;
				}
				this.optimizer.OptimizationAbort();		// WILL_UNPAUSE_AND__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
				this.cbxPauseResume.Enabled = false;
				this.cbxPauseResume.Checked = false;
				this.olvBacktests.UseWaitCursor = false;
				this.cbxRunCancel.Enabled = true;		// NOW_UNPAUSED_ABORTED_AND_READY_TO_RUN_AGAIN
				this.cbxRunCancel.Checked = this.optimizer.IsRunningNow;
				//ALREADY_DUNNO_WHO this.expand();
				return;
			}
			
			//string staleReason = this.optimizer.StaleReason;
			//bool clickedToClearAndPrepareForNewOptimization = string.IsNullOrEmpty(staleReason) == false;
			//if (clickedToClearAndPrepareForNewOptimization) {
			//	//this.backtests.Clear();
			//	//this.olvBacktests.SetObjects(this.backtests, true);
			//	this.SyncBacktestAndListWithOptimizationResultsByContextIdent();
			//	this.optimizer.ClearIWasRunFor();
			//	this.PopulateTextboxesFromExecutorsState();
			//	this.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
			//	this.btnPauseResume.Text = "Pause/Resume";
			//	return;
			//}
			
			this.backtestsLocalEasierToSync.Clear();
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync, true);
			//this.olvBacktests.RebuildColumns();
			//this.olvBacktests.BuildList();
			int threadsLaunched = this.optimizer.OptimizationRun();

			this.lblStats.Text = threadsLaunched + " THREADS LAUNCHED";

			string spawnedRunning = this.optimizer.DisposableExecutorsRunningNow + "/" + this.optimizer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
			
			this.cbxRunCancel.Text = "Cancel " + this.optimizer.BacktestsRemaining + " backtests";
			this.cbxRunCancel.Checked = this.optimizer.IsRunningNow;
			this.progressBar1.Value = 0;

			this.cbxPauseResume.Enabled = true;
			this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
			this.olvBacktests.UseWaitCursor = true;
			
			this.statsAndHistoryCollapse();
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
			this.backtestsLocalEasierToSync.Add(e.SystemPerformanceRestoreAble);
			//v1 this.olvBacktests.SetObjects(this.backtests, true); //preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			this.olvBacktests.AddObject(e.SystemPerformanceRestoreAble);
			
			int backtestsTotal		= this.optimizer.BacktestsTotal;
			int backtestsFinished	= this.optimizer.BacktestsFinished;
			
			string spawnedRunning = this.optimizer.DisposableExecutorsRunningNow + "/" + this.optimizer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
			
			this.cbxRunCancel.Text = "Cancel " + this.optimizer.BacktestsRemaining + " backtests";
			
			double pctComplete = (backtestsTotal > 0) ? Math.Round(100 * backtestsFinished / (double)backtestsTotal) : 0;
			this.lblStats.Text = pctComplete + "% complete   " + backtestsFinished + "/" + backtestsTotal;
			if (backtestsFinished >= this.progressBar1.Minimum && backtestsFinished <= this.progressBar1.Maximum) {
				this.progressBar1.Value = backtestsFinished;
			}
			if (this.optimizer.Unpaused == false) return;
			this.cbxPauseResume.Text = this.optimizer.BacktestsSecondsElapsed + " seconds elapsed";
		}
		void optimizer_OnAllBacktestsFinished(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.optimizer_OnAllBacktestsFinished(sender, e); });
				return;
			}
			
			string spawnedRunning = this.optimizer.DisposableExecutorsRunningNow + "/" + this.optimizer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Use CPU Cores";
			
			this.cbxRunCancel.Text = "Run " + this.optimizer.BacktestsTotal + " backtests";
			this.olvBacktests.EmptyListMsg = "";
			//this.lblStats.Text = "0% complete   0/" + totalBacktests;
			//this.progressBar1.Value = 0;
			this.olvBacktests.UseWaitCursor = false;
			this.cbxPauseResume.Text = this.optimizer.BacktestsSecondsElapsed + " seconds elapsed";
			this.cbxPauseResume.Enabled = false;
			this.cbxRunCancel.Checked = this.optimizer.IsRunningNow;

			Strategy strategy = this.optimizer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//v1
			//if (strategy.OptimizationResultsByContextIdent.ContainsKey(symbolScaleRange) == false) {
			//	strategy.OptimizationResultsByContextIdent.Add(symbolScaleRange, this.backtests);
			//} else {
			//	strategy.OptimizationResultsByContextIdent[symbolScaleRange] = this.backtests;
			//}
			//strategy.Serialize();
			//v2
			this.RepositoryJsonOptimizationResults.SerializeList(this.backtestsLocalEasierToSync, symbolScaleRange);
			this.olvHistoryRescanRefillSelect(symbolScaleRange);
			this.statsAndHistoryExpand();
		}
		void optimizer_OnOptimizationAborted(object sender, EventArgs e) {
			this.optimizer_OnAllBacktestsFinished(sender, e);
		}
		void optimizer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild(object sender, EventArgs e) {
			this.txtScriptParameterTotalNr.Text = this.optimizer.ScriptParametersTotalNr.ToString();
			this.txtIndicatorParameterTotalNr.Text = this.optimizer.IndicatorParameterTotalNr.ToString();
			this.cbxRunCancel.Text = "Run " + this.optimizer.BacktestsTotal + " backtests";
		}
		void cbxPauseResume_Click(object sender, EventArgs e) {
			this.optimizer.Unpaused		= !this.optimizer.Unpaused;
			this.cbxPauseResume.Text	=  this.optimizer.Unpaused ? "Pause" : "Resume";
			this.cbxPauseResume.Checked	= !this.optimizer.Unpaused; 
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
			
			string spawnedRunning = this.optimizer.DisposableExecutorsRunningNow + "/" + this.optimizer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
		}
		void mniCopyToDefaultCtxBacktest_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtxBacktest_Click()";
//			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
//			if (selectedClone == null) return;
//			this.RaiseOnCopyToContextDefaultBacktest(selectedClone);
			SystemPerformanceRestoreAble sysPerfRestoreAble = this.olvBacktests.SelectedObject as SystemPerformanceRestoreAble;
			if (sysPerfRestoreAble == null) return;
			this.RaiseOnCopyToContextDefaultBacktest(sysPerfRestoreAble);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}
		void mniCopyToDefaultCtx_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtx_Click()";
//			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
//			if (selectedClone == null) return;
//			this.RaiseOnCopyToContextDefault(selectedClone);
			SystemPerformanceRestoreAble sysPerfRestoreAble = this.olvBacktests.SelectedObject as SystemPerformanceRestoreAble;
			if (sysPerfRestoreAble == null) return;
			this.RaiseOnCopyToContextDefault(sysPerfRestoreAble);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}
		void mniltbCopyToNewContext_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContext_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";
//			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
//			if (selectedClone == null) return;
//			this.RaiseOnCopyToContextNew(selectedClone);
			SystemPerformanceRestoreAble sysPerfRestoreAble = this.olvBacktests.SelectedObject as SystemPerformanceRestoreAble;
			if (sysPerfRestoreAble == null) return;
			this.RaiseOnCopyToContextNew(sysPerfRestoreAble, scriptContextNewName);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}
		void mniltbCopyToNewContextBacktest_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			string msig = " /mniltbCopyToNewContextBacktest_UserTyped() scriptContextNewName[" + scriptContextNewName + "]";
//			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
//			if (selectedClone == null) return;
//			this.RaiseOnCopyToContextNewBacktest(selectedClone);
			SystemPerformanceRestoreAble sysPerfRestoreAble = this.olvBacktests.SelectedObject as SystemPerformanceRestoreAble;
			if (sysPerfRestoreAble == null) return;
			this.RaiseOnCopyToContextNewBacktest(sysPerfRestoreAble, scriptContextNewName);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}
		
// MOVED_TO_CONTEXT_SCRIPT
//		ContextScript convertOptimizationResultToScriptContext(string msig) {
//			SystemPerformanceRestoreAble sysPerfRestoreAble = (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject;
//			if (sysPerfRestoreAble == null) {
//				string msg = "IS_NULL (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject";
//				Assembler.PopupException(msg + msig);
//				return null;
//			}
//			
//			#if DEBUG	// inline unittest
//			foreach (OLVColumn olvc in this.columnsDynParams) {
//				string iParamName = olvc.Text;
//
//				var iDisplayedByName = this.optimizer.ScriptAndIndicatorParametersMergedByName;
//				if (iDisplayedByName.ContainsKey(iParamName) == false) {
//					string msg = "NEVER_HAPPENED_SO_FAR iDisplayedByName.ContainsKey(" +  iParamName + ") == false";
//					Assembler.PopupException(msg);
//				}
//				IndicatorParameter iDisplayed = iDisplayedByName[iParamName];
//
//				var iPropagatingByName = sysPerfRestoreAble.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
//				if (iPropagatingByName.ContainsKey(iParamName) == false) {
//					string msg = "NEVER_HAPPENED_SO_FAR iPropagatingByName.ContainsKey(" + iParamName + ") == false";
//					Assembler.PopupException(msg);
//				}
//				IndicatorParameter iPropagating = iPropagatingByName[iParamName];
//
//				if (iDisplayed.ValueCurrent != iPropagating.ValueCurrent) {
//					string msg = "both are wrong; I clicked on MaSlow=20,MaFast=11; iDisplayed=MaFast=33, iPropagating=MaFast=22; replacing executorPool with newExecutor() each backtest";
//					Assembler.PopupException(msg, null, false);
//				}
//			}
//			#endif
//
//			if (sysPerfRestoreAble.ScriptParametersById_BuiltOnBacktestFinished == null) {
//				string msg = "BACKTEST_WAS_ABORTED_CANT_POPUPLATE";
//				Assembler.PopupException(msg + msig);
//				return null;
//			}
//			ContextScript selectedClone = this.optimizer.Executor.Strategy.ScriptContextCurrent.CloneAndAbsorbFromSystemPerformanceRestoreAble(sysPerfRestoreAble);
//			return selectedClone;
//		}
		void olvBacktests_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;	// right click on the blank space (not on a row with data)
			e.MenuStrip = this.ctxOneBacktestResult;
		}
		void mniCopyToClipboard_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI OptimizerControl.mniCopyToClipboard_Click()");
		}
		void mniSaveCsv_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI OptimizerControl.mniSaveCsv_Click()");
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

			this.backtestsLocalEasierToSync = this.RepositoryJsonOptimizationResults.DeserializeList(fname.Name);
			if (this.backtestsLocalEasierToSync == null || this.backtestsLocalEasierToSync.Count == 0) {
				string msg = "NO_BACKTESTS_FOUND_INSIDE_FILE " + fname.Name;
				Assembler.PopupException(msg);
				return;
			}
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync, true);
		}
		private void olvHistory_DoubleClick(object sender, EventArgs e) {
			FnameDateSizeColor fname = this.olvHistory.SelectedObject as FnameDateSizeColor;
			if (fname == null) return;
			if (this.backtestsLocalEasierToSync.Count == 0) {
				string msg = "NO_RECORDS_IN_OPTIMIZATION_HISTORY_FILE[" + fname.ToString() + "]_DESERIALIZATION_HAPPES_ON_ITEM_ACTIVATE__DOUBLECLICK_AGAIN";
				Assembler.PopupException(msg);
				return;
			}
			this.RaiseOnAllParametersControlOpen(this.backtestsLocalEasierToSync, fname.Name);
		}
		void olvHistory_KeyDown(object sender, KeyEventArgs e) {
			try {
				if (e.KeyCode == Keys.Delete) {
					FnameDateSizeColor fname = this.olvHistory.SelectedObject as FnameDateSizeColor;
					if (fname == null) return;
					bool deleted = this.RepositoryJsonOptimizationResults.ItemDelete_dirtyImplementation(fname);
					if (deleted == false) {
						string msg = "FIXME/REFACTOR RepositoryJsonOptimizationResults.ItemDelete_dirtyImplementation(" + fname + ") //olvHistory_KeyDown()";
						Assembler.PopupException(msg);
					}
					this.SyncBacktestAndListWithOptimizationResultsByContextIdent();
				}
				if (e.KeyCode == Keys.Enter) {
					this.olvHistory_DoubleClick(this, null);
				}
			} catch (Exception ex) {
				string msg = "FIXME //olvHistory_KeyDown()";
				Assembler.PopupException(msg, ex);
			}
		}
		
		void olvParameters_Click(object sender, EventArgs e) {
			IndicatorParameter paramClicked = this.olvParameters.SelectedObject as IndicatorParameter;
			if (paramClicked == null) return;
			// ITEM_CHECKED_WILL_BE_GENERATED paramClicked.WillBeSequencedDuringOptimization = !paramClicked.WillBeSequencedDuringOptimization;
			if (paramClicked.WillBeSequencedDuringOptimization) {
				this.olvParameters.UncheckObject(paramClicked);
			} else {
				this.olvParameters.CheckObject(paramClicked);
			}
			OLVColumn found = null;
			foreach (OLVColumn each in columnsDynParams) {
				if (each.Name != paramClicked.FullName) continue;
				found = each;
				break;
			}
			if (found == null) {
				string msg = "MUST_EXIST_columnsDynParams[" + paramClicked.FullName+ "]";
				Assembler.PopupException(msg);
				return;
			}
			found.IsVisible = paramClicked.WillBeSequencedDuringOptimization;
			this.olvBacktests.RebuildColumns();
			//v2
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.populateColumns_onlyWillBeSequencedDuringOptimization_orAll();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync);
		}
		// implemented as this.fastOLVparametersYesNoMinMaxStep.CheckStatePutter = delegate(object o, CheckState newState) {}
		//void fastOLVparametersYesNoMinMaxStep_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e) {
		//    //IndicatorParameter paramClicked = this.fastOLVparametersYesNoMinMaxStep.SelectedObject as IndicatorParameter;
		//    if (e.Index < 0 || e.Index >= this.scriptAndIndicatorParametersMergedCloned.Count) return;
		//    IndicatorParameter paramClicked = this.scriptAndIndicatorParametersMergedCloned[e.Index];
		//    paramClicked.WillBeSequencedDuringOptimization = e.NewValue.CompareTo(CheckState.Checked) == 0;
		//    this.optimizer.TotalsCalculate();
		//    this.totalsPropagateAdjustSplitterDistance();
		//    // for HeaderAllCheckBox.Clicked => Strategy.Serialize()d as many times as you got (Script+Indicator)Parameters
		//    this.optimizer.ExecutorCloneToBeSpawned.Strategy.Serialize();
		//}
		void cbxExpandCollapse_CheckedChanged(object sender, EventArgs e) {
			if (this.cbxExpandCollapse.Checked == true) {
				this.statsAndHistoryExpand();
			} else {
				this.statsAndHistoryCollapse();
			}
		}
		bool showAllScriptIndicatorParametersInOptimizationResults;
		void mni_showAllScriptIndicatorParametersInOptimizationResultsClick(object sender, EventArgs e) {
			this.showAllScriptIndicatorParametersInOptimizationResults = this.mni_showAllScriptIndicatorParametersInOptimizationResults.Checked;
			this.olvParameters.Refresh();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.populateColumns_onlyWillBeSequencedDuringOptimization_orAll();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync);
		}
	}
}
