using System;
using System.ComponentModel;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Core.Repositories;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Sequencing {
	public partial class SequencerControl {
		void cbxRunCancel_Click(object sender, EventArgs e) {
			if (this.sequencer.IsRunningNow) {
				if (this.sequencer.Unpaused == false) {
					// WHILE_AVOIDING_DEADLOCK__DISABLE_BUTTONS_FOR_USER_NOT_TO_WORSEN__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
					this.cbxRunCancel.Enabled = false;
					this.cbxPauseResume.Enabled = false;
				}
				this.sequencer.SequencerAbort();		// WILL_UNPAUSE_AND__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH
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

			//v1 KEPT_STRATEGY_NAME=SYMBOL,SYMBOL=NULL__STUCK_IN_SERIALIZED_FILE
			//this.backtestsLocalEasierToSync.Clear();
			//v2
			this.backtestsLocalEasierToSync = new SequencedBacktests(this.sequencer.Executor, this.sequencer.Executor.Strategy.ScriptContextCurrentName);

			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync.BacktestsReadonly, true);
			//this.olvBacktests.RebuildColumns();
			//this.olvBacktests.BuildList();
			int threadsLaunched = this.sequencer.SequencerRun();

			this.lblStats.Text = threadsLaunched + " THREADS LAUNCHED";

			string spawnedRunning = this.sequencer.DisposableExecutorsRunningNow + "/" + this.sequencer.DisposableExecutorsSpawnedNow;
			this.lblThreadsToRun.Text = spawnedRunning + " Threads Used";
			
			this.cbxRunCancel.Text = "Cancel " + this.sequencer.BacktestsRemaining + " backtests";
			this.cbxRunCancel.Checked = this.sequencer.IsRunningNow;
			this.progressBar1.Value = 0;

			this.cbxPauseResume.Enabled = true;
			this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
			this.olvBacktests.UseWaitCursor = true;
			
			//v1 this.statsAndHistoryCollapse();
			this.cbxExpanded.Checked = false;
		}

//		void sequencer_OnBacktestStarted(object sender, EventArgs e) {
//			if (base.InvokeRequired) {
//				base.BeginInvoke((MethodInvoker)delegate { this.sequencer_OnBacktestStarted(sender, e); });
//				return;
//			}
//			this.splitContainer1.SplitterDistance = heightCollapsed;
//		}
		void sequencer_OnOneBacktestFinished(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			string msig = " //SequencerControl.sequencer_OnOneBacktestFinished()";
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.sequencer_OnOneBacktestFinished(sender, e); });
				return;
			}
			this.backtestsLocalEasierToSync.Add(e.SystemPerformanceRestoreAble);

			//v1 this.olvBacktests.SetObjects(this.backtests, true); //preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			try {
				this.olvBacktests.AddObject(e.SystemPerformanceRestoreAble);
			} catch (Exception ex) {
				string msg = "YOU_FORGOT_TO_UNSUBSCRIBE_ME_FROM_ALL_EVENTS__GC_CANT_DISPOSE_ME"
					+ " EVENT_GENERATORS_KEEP_SENDING_EVENTS_WHICH_MY_GUTS_ARENT_READY_TO_DIGEST"
					+ " RETURNING_FROM_CLOSED_SUBSCRIBER_NO_ACTION_DONE";
				Assembler.PopupException(msg + msig, ex, false);
				return;
			}
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
			string symbolScaleRange = strategy.ScriptContextCurrent.SymbolScaleIntervalDataRangeForScriptContextNewName;
			//v1
			//if (strategy.OptimizationResultsByContextIdent.ContainsKey(symbolScaleRange) == false) {
			//	strategy.OptimizationResultsByContextIdent.Add(symbolScaleRange, this.backtests);
			//} else {
			//	strategy.OptimizationResultsByContextIdent[symbolScaleRange] = this.backtests;
			//}
			//strategy.Serialize();
			//v2
			//this.RepositoryJsonSequencer.SerializeSingle(this.backtestsLocalEasierToSync, symbolScaleRange);
			//v3
			//if (this.backtestsLocalEasierToSync.ProfitFactorAverage == 0) {
			//	this.backtestsLocalEasierToSync.CalculateProfitFactorAverage();
			//}
			//string fnameWithPFappended = FnameDateSizeColorPFavg.AppendProfitFactorAverage(
			//	symbolScaleRange, this.backtestsLocalEasierToSync.ProfitFactorAverage);
			//this.RepositoryJsonSequencer.SerializeSingle(this.backtestsLocalEasierToSync, fnameWithPFappended);
			//v4
			this.RepositoryJsonSequencer.SerializeSingle(this.backtestsLocalEasierToSync);

			this.backtestsLocalEasierToSync.CheckPositionsCountMustIncreaseOnly();
			this.olvHistoryRescanRefillSelect(symbolScaleRange);

			//v1 this.statsAndHistoryExpand();
			this.cbxExpanded.Checked = true;
			this.raiseOnCorrelatorShouldPopulate(this.backtestsLocalEasierToSync);
		}
		void sequencer_OnSequencerAborted(object sender, EventArgs e) {
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
			this.raiseOnCopyToContextDefaultBacktest(sysPerfRestoreAble);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}
		void mniCopyToDefaultCtx_Click(object sender, EventArgs e) {
			string msig = " /mniCopyToDefaultCtx_Click()";
//			ContextScript selectedClone = this.convertOptimizationResultToScriptContext(msig);
//			if (selectedClone == null) return;
//			this.RaiseOnCopyToContextDefault(selectedClone);
			SystemPerformanceRestoreAble sysPerfRestoreAble = this.olvBacktests.SelectedObject as SystemPerformanceRestoreAble;
			if (sysPerfRestoreAble == null) return;
			this.raiseOnCopyToContextDefault(sysPerfRestoreAble);
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
			this.raiseOnCopyToContextNew(sysPerfRestoreAble, scriptContextNewName);
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
			this.raiseOnCopyToContextNewBacktest(sysPerfRestoreAble, scriptContextNewName);
			this.olvParameters.Refresh();	//otherwize you'll see CURRENT changed only after mouseover on the CHANGEDs	MUST_BE_AN_INTERFORM_EVENT_BUT_LAZY
		}

		void olvBacktests_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;	// right click on the blank space (not on a row with data)
			e.MenuStrip = this.ctxBacktests_OneResult;
		}
		void mniCopyToClipboard_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI SequencerControl.mniCopyToClipboard_Click()");
		}
		void mniSaveCsv_Click(object sender, EventArgs e) {
			Assembler.DisplayStatus("USER_OLVs_CTRL+A,CTRL+C NYI SequencerControl.mniSaveCsv_Click()");
		}
		void ctxOneBacktestResult_Opening(object sender, CancelEventArgs e) {
			string msig = " /ctxOneBacktestResult_Opening()";
			//if (this.sequencer.Executor.ExecutionDataSnapshot.CorrelatorFormIsVisible) {
			//	this.mni_showInSequencedBacktest_ScriptIndicatorParameters_All.Enabled = false;
			//	this.mni_showInSequencedBacktests_ScriptIndicatorParameters_CorrelatorChecked.Enabled = false;
			//	// and show ALL even if we 1) clicked "chosen" 2) closed the CorrelatorForm
			//	...
			//}

			SystemPerformanceRestoreAble sysPerfRestoreAble = (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject;
			if (sysPerfRestoreAble == null && this.olvBacktests.Items.Count > 0) {
				this.olvBacktests.SelectedObject = this.olvBacktests.GetModelObject(0);
				sysPerfRestoreAble = (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject;
				string msg = "I_EMULATED_CLICK_ON_FIRST_AVAILABLE_BACKTEST WEIRD_ITS_HAPPENED";
				Assembler.PopupException(msg + msig, null, false);
			}
			if (sysPerfRestoreAble == null) {
				string msg = "IS_NULL (SystemPerformanceRestoreAble)this.olvBacktests.SelectedObject";
				//Assembler.PopupException(msg + msig, null, false);
				return;
			}
			
			string uniqueBacktestNumbers						= sysPerfRestoreAble.NetProfitRecovery;
			this.mniltbCopyToNewContext.InputFieldValue			= uniqueBacktestNumbers;
			this.mniltbCopyToNewContextBacktest.InputFieldValue	= uniqueBacktestNumbers;

			//string stratSymbolScaleRange	= sysPerfRestoreAble.StrategyName + " " + sysPerfRestoreAble.SymbolScaleIntervalDataRange;
			//string stratSymbolScaleRange = "FIX_MY_StrategyName + sysPerfRestoreAble.SymbolScaleIntervalDataRange";
			string stratSymbolScaleRange	= this.sequencer.Executor.StrategyName + " " + this.sequencer.SymbolScaleIntervalAsString;
			this.mniInfo.Text				= uniqueBacktestNumbers + " => " + stratSymbolScaleRange;
		}
		void olvHistory_ItemActivate(object sender, EventArgs e) {
			try {
				FnameDateSizeColorPFavg fname = this.olvHistory.SelectedObject as FnameDateSizeColorPFavg;
				if (fname == null) return;
				this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange(fname.SymbolScaleRange);
			} catch (Exception ex) {
				string msg = "FIXME //olvHistory_ItemActivate()";
				Assembler.PopupException(msg, ex);
			}
		}
		void olvHistory_KeyDown(object sender, KeyEventArgs e) {
			try {
				if (e.KeyCode == Keys.Delete) {
					FnameDateSizeColorPFavg fname = this.olvHistory.SelectedObject as FnameDateSizeColorPFavg;
					if (fname == null) return;
					bool deleted = this.RepositoryJsonSequencer.ItemDelete_dirtyImplementation(fname);
					if (deleted == false) {
						string msg = "FIXME/REFACTOR RepositoryJsonSequencer.ItemDelete_dirtyImplementation(" + fname + ") //olvHistory_KeyDown()";
						Assembler.PopupException(msg);
					}
					this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
				}
			} catch (Exception ex) {
				string msg = "FIXME //olvHistory_KeyDown()";
				Assembler.PopupException(msg, ex);
			}
		}

		void mniOneSequencedBacktest_delete_Click(object sender, EventArgs e) {
			this.olvHistory_KeyDown(this.olvHistory, new KeyEventArgs(Keys.Delete));
		}
		
		void olvParameters_Click(object sender, EventArgs e) {
			IndicatorParameter paramClicked = this.olvParameters.SelectedObject as IndicatorParameter;
			if (paramClicked == null) return;
			// ITEM_CHECKED_WILL_BE_GENERATED paramClicked.WillBeSequencedDuringOptimization = !paramClicked.WillBeSequencedDuringOptimization;
			if (paramClicked.WillBeSequenced) {
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
			found.IsVisible = paramClicked.WillBeSequenced;
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
			if (this.cbxExpanded.Checked == true) {
				this.statsAndHistoryExpand(false);
			} else {
				this.statsAndHistoryCollapse(false);
			}
		}
		bool showAllScriptIndicatorParametersInSequencedBacktest;
		void mni_showInSequencedBacktests_ScriptIndicatorParameters_All_Click(object sender, EventArgs e) {
			//if (this.mni_showInSequencedBacktest_ScriptIndicatorParameters_All.Checked == true) {
			//	string msg = "ChartForm > Menu > Show Correlator > Click any checkbox to shrink again";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			//this.showAllScriptIndicatorParametersInOptimizationResults = this.mni_showAllScriptIndicatorParametersInOptimizationResults.Checked;
			//this.olvParameters.Refresh();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.populateColumns_onlyWillBeSequencedDuringOptimization_orAll();
			//CHANGING_COLUMN_VISIBILITY_INSTEAD this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync);

			//if (this.mni_showInSequencedBacktest_ScriptIndicatorParameters_All.Checked == true) {
				this.BacktestsShowAll_regardlessWhatIsChosenInCorrelator();
			//} else {
			//	this.BacktestsShowCorrelatorChosen();
			//}
			this.ctxBacktests_OneResult.Show();
		}
		void mni_showInSequencedBacktests_ScriptIndicatorParameters_CorrelatorChecked_Click(object sender, EventArgs e) {
			this.BacktestsShowCorrelatorChosen();
			this.ctxBacktests_OneResult.Show();
		}
	}
}
