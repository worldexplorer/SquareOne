using System;
using System.ComponentModel;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.Sequencing {
	public partial class SequencerControl {
		void cbxRunCancel_Click(object sender, EventArgs e) {
			if (this.sequencer.IsRunningNow) {
				if (this.sequencer.Unpaused == false) {
					// WHILE_AVOIDING_DEADLOCK__DISABLE_BUTTONS_FOR_USER_NOT_TO_WORSEN__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
					this.cbxRunCancel.Enabled = false;
					this.cbxPauseResume.Enabled = false;
				}
				this.sequencer.OptimizationAbort();		// WILL_UNPAUSE_AND__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
				this.cbxPauseResume.Enabled = false;
				this.cbxPauseResume.Checked = false;
				this.olvBacktests.UseWaitCursor = false;
				this.cbxRunCancel.Enabled = true;		// NOW_UNPAUSED_ABORTED_AND_READY_TO_RUN_AGAIN
				this.cbxRunCancel.Checked = this.sequencer.IsRunningNow;
				//ALREADY_DUNNO_WHO this.expand();
				return;
			}
			
			//string staleReason = this.sequencer.StaleReason;
			//bool clickedToClearAndPrepareForNewOptimization = string.IsNullOrEmpty(staleReason) == false;
			//if (clickedToClearAndPrepareForNewOptimization) {
			//	//this.backtests.Clear();
			//	//this.olvBacktests.SetObjects(this.backtests, true);
			//	this.SyncBacktestAndListWithOptimizationResultsByContextIdent();
			//	this.sequencer.ClearIWasRunFor();
			//	this.PopulateTextboxesFromExecutorsState();
			//	this.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
			//	this.btnPauseResume.Text = "Pause/Resume";
			//	return;
			//}
			
			this.backtestsLocalEasierToSync.Clear();
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync, true);
			//this.olvBacktests.RebuildColumns();
			//this.olvBacktests.BuildList();
			int threadsLaunched = this.sequencer.OptimizationRun();

			this.lblStats.Text = threadsLaunched + " THREADS LAUNCHED";

			string spawnedRunning = this.sequencer.DisposableExecutorsRunningNow + "/" + this.sequencer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
			
			this.cbxRunCancel.Text = "Cancel " + this.sequencer.BacktestsRemaining + " backtests";
			this.cbxRunCancel.Checked = this.sequencer.IsRunningNow;
			this.progressBar1.Value = 0;

			this.cbxPauseResume.Enabled = true;
			this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
			this.olvBacktests.UseWaitCursor = true;
			
			this.statsAndHistoryCollapse();
		}

//		void sequencer_OnBacktestStarted(object sender, EventArgs e) {
//			if (base.InvokeRequired) {
//				base.BeginInvoke((MethodInvoker)delegate { this.sequencer_OnBacktestStarted(sender, e); });
//				return;
//			}
//			this.splitContainer1.SplitterDistance = heightCollapsed;
//		}
		void sequencer_OnOneBacktestFinished(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.sequencer_OnOneBacktestFinished(sender, e); });
				return;
			}
			this.backtestsLocalEasierToSync.Add(e.SystemPerformanceRestoreAble);
			//v1 this.olvBacktests.SetObjects(this.backtests, true); //preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			this.olvBacktests.AddObject(e.SystemPerformanceRestoreAble);
			
			int backtestsTotal		= this.sequencer.BacktestsTotal;
			int backtestsFinished	= this.sequencer.BacktestsFinished;
			
			string spawnedRunning = this.sequencer.DisposableExecutorsRunningNow + "/" + this.sequencer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
			
			this.cbxRunCancel.Text = "Cancel " + this.sequencer.BacktestsRemaining + " backtests";
			
			double pctComplete = (backtestsTotal > 0) ? Math.Round(100 * backtestsFinished / (double)backtestsTotal) : 0;
			this.lblStats.Text = pctComplete + "% complete   " + backtestsFinished + "/" + backtestsTotal;
			if (backtestsFinished >= this.progressBar1.Minimum && backtestsFinished <= this.progressBar1.Maximum) {
				this.progressBar1.Value = backtestsFinished;
			}
			if (this.sequencer.Unpaused == false) return;
			this.cbxPauseResume.Text = this.sequencer.BacktestsSecondsElapsed + " seconds elapsed";
		}
		void sequencer_OnAllBacktestsFinished(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.sequencer_OnAllBacktestsFinished(sender, e); });
				return;
			}
			
			string spawnedRunning = this.sequencer.DisposableExecutorsRunningNow + "/" + this.sequencer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Use CPU Cores";
			
			this.cbxRunCancel.Text = "Run " + this.sequencer.BacktestsTotal + " backtests";
			this.olvBacktests.EmptyListMsg = "";
			//this.lblStats.Text = "0% complete   0/" + totalBacktests;
			//this.progressBar1.Value = 0;
			this.olvBacktests.UseWaitCursor = false;
			this.cbxPauseResume.Text = this.sequencer.BacktestsSecondsElapsed + " seconds elapsed";
			this.cbxPauseResume.Enabled = false;
			this.cbxRunCancel.Checked = this.sequencer.IsRunningNow;

			Strategy strategy = this.sequencer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//v1
			//if (strategy.OptimizationResultsByContextIdent.ContainsKey(symbolScaleRange) == false) {
			//	strategy.OptimizationResultsByContextIdent.Add(symbolScaleRange, this.backtests);
			//} else {
			//	strategy.OptimizationResultsByContextIdent[symbolScaleRange] = this.backtests;
			//}
			//strategy.Serialize();
			//v2
			this.RepositoryJsonSequencer.SerializeList(this.backtestsLocalEasierToSync, symbolScaleRange);
			this.olvHistoryRescanRefillSelect(symbolScaleRange);
			this.statsAndHistoryExpand();
		}
		void sequencer_OnOptimizationAborted(object sender, EventArgs e) {
			this.sequencer_OnAllBacktestsFinished(sender, e);
		}
		void sequencer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild(object sender, EventArgs e) {
			this.txtScriptParameterTotalNr.Text = this.sequencer.ScriptParametersTotalNr.ToString();
			this.txtIndicatorParameterTotalNr.Text = this.sequencer.IndicatorParameterTotalNr.ToString();
			this.cbxRunCancel.Text = "Run " + this.sequencer.BacktestsTotal + " backtests";
		}
		void cbxPauseResume_Click(object sender, EventArgs e) {
			this.sequencer.Unpaused		= !this.sequencer.Unpaused;
			this.cbxPauseResume.Text	=  this.sequencer.Unpaused ? "Pause" : "Resume";
			this.cbxPauseResume.Checked	= !this.sequencer.Unpaused; 
		}
		void nudCpuCoresToUse_ValueChanged(object sender, EventArgs e) {
			int newValue = (int)this.nudThreadsToRun.Value;
			int valueMin = 1;
			int valueMax = this.sequencer.CpuCoresAvailable * 2;
			if (newValue < valueMin) {
				newValue = valueMin;
				this.nudThreadsToRun.Value = newValue;
			}
			if (newValue > valueMax) {
				newValue = valueMax;
				this.nudThreadsToRun.Value = valueMax;
			}
			this.sequencer.ThreadsToUse = newValue;
			
			string spawnedRunning = this.sequencer.DisposableExecutorsRunningNow + "/" + this.sequencer.DisposableExecutorsSpawnedNow;
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
//				var iDisplayedByName = this.sequencer.ScriptAndIndicatorParametersMergedByName;
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
//			ContextScript selectedClone = this.sequencer.Executor.Strategy.ScriptContextCurrent.CloneAndAbsorbFromSystemPerformanceRestoreAble(sysPerfRestoreAble);
//			return selectedClone;
//		}
		void olvBacktests_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;	// right click on the blank space (not on a row with data)
			e.MenuStrip = this.ctxOneBacktestResult;
		}
		void mniCopyToClipboard_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI SequencerControl.mniCopyToClipboard_Click()");
		}
		void mniSaveCsv_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI SequencerControl.mniSaveCsv_Click()");
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
			try {
				FnameDateSizeColor fname = this.olvHistory.SelectedObject as FnameDateSizeColor;
				if (fname == null) return;
                this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange(fname.Name);
			} catch (Exception ex) {
				string msg = "FIXME //olvHistory_ItemActivate()";
				Assembler.PopupException(msg, ex);
			}
		}
		void olvHistory_KeyDown(object sender, KeyEventArgs e) {
			try {
				if (e.KeyCode == Keys.Delete) {
					FnameDateSizeColor fname = this.olvHistory.SelectedObject as FnameDateSizeColor;
					if (fname == null) return;
					bool deleted = this.RepositoryJsonSequencer.ItemDelete_dirtyImplementation(fname);
					if (deleted == false) {
						string msg = "FIXME/REFACTOR RepositoryJsonOptimizationResults.ItemDelete_dirtyImplementation(" + fname + ") //olvHistory_KeyDown()";
						Assembler.PopupException(msg);
					}
					this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
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
		//	//IndicatorParameter paramClicked = this.fastOLVparametersYesNoMinMaxStep.SelectedObject as IndicatorParameter;
		//	if (e.Index < 0 || e.Index >= this.scriptAndIndicatorParametersMergedCloned.Count) return;
		//	IndicatorParameter paramClicked = this.scriptAndIndicatorParametersMergedCloned[e.Index];
		//	paramClicked.WillBeSequencedDuringOptimization = e.NewValue.CompareTo(CheckState.Checked) == 0;
		//	this.sequencer.TotalsCalculate();
		//	this.totalsPropagateAdjustSplitterDistance();
		//	// for HeaderAllCheckBox.Clicked => Strategy.Serialize()d as many times as you got (Script+Indicator)Parameters
		//	this.sequencer.ExecutorCloneToBeSpawned.Strategy.Serialize();
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
			if (this.mni_showAllScriptIndicatorParametersInOptimizationResults.Checked == true) {
				string msg = "ChartForm > Menu > Show Correlator > Click any checkbox to shrink again";
				Assembler.PopupException(msg);
				return;
			}
			//this.showAllScriptIndicatorParametersInOptimizationResults = this.mni_showAllScriptIndicatorParametersInOptimizationResults.Checked;
			//this.olvParameters.Refresh();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.populateColumns_onlyWillBeSequencedDuringOptimization_orAll();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync);
			this.BacktestsRestoreCorrelatedClosed();	// can't access here: chartFormsManager.CorrelatorFormConditionalInstance.Close();
		}
	}
}
