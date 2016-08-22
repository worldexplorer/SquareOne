using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Singletons;

namespace Sq1.Gui.Forms {
	public partial class SequencerForm {
		//MOVED_SequencerFormFactory_HERE_INSTEAD_OF_THESE
		//void sequencerForm_FormClosing(object sender, FormClosingEventArgs e) {
		//    // only when user closed => allow scriptEditorForm_FormClosed() to serialize
		//    if (this.chartFormsManager.MainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
		//        e.Cancel = true;
		//        return;
		//    }
		//    if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
		//        e.Cancel = true;
		//        return;
		//    }
		//}
		//void sequencerForm_FormClosed(object sender, FormClosedEventArgs e) {
		//    // both at FormCloseByX and MainForm.onClose()
		//    this.chartFormsManager.ChartForm.MniShowSequencer.Checked = false;
		//    this.chartFormsManager.MainForm.MainFormSerialize();
		//    // NOT_MY_JOB__FORM.CLOSE()_SENDS_MESSAGE_WHICH_DISPOSES_ALL_INNER_COMPONENTS/CONTROLS this.Dispose(true); this.Dispose();
		//    this.chartFormsManager.SequencerForm = null;
		//}

		void sequencer_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt(object sender, SequencedBacktestsEventArgs e) {
			this.chartFormManager.SequencerFormSingletonized_nullUnsafe.SequencerControl.BacktestsReplaceWithCorrelated(e.SequencedBacktests);
		}

		void sequencerControl_OnCorrelatorShouldPopulate(object sender, SequencedBacktestsEventArgs e) {
			if (this.chartFormManager.CorrelatorForm == null) {
				string msg = "don't force Correlator showup if I clicked only mniShowSequencer and it wasn't restored from XML";
				return;
			}
			CorrelatorForm correlatorForm = this.chartFormManager.CorrelatorFormSingletonized_nullUnsafe;
			correlatorForm.PopulateSequencedHistory(e.SequencedBacktests);
			if (this.chartFormManager.SequencerForm.Visible) {
				correlatorForm.ActivateDockContent_popupAutoHidden(false);
			}

			//DockState designedDockState = allParametersForm.ShowHint;
			//if (designedDockState != DockState.Float) {
			//	designedDockState  = DockState.Float;
			//}
			//allParametersForm.Text = "Correlator :: " + chartFormManager.Strategy.Name + " :: " + e.FileName;
			//Size designedSize = allParametersForm.Size;
			//allParametersForm.Show(this.chartFormManager.MainForm.DockPanel, designedDockState);
			////allParametersForm.Size = designedSize;
			////allParametersForm.FloatPane.Size = designedSize;
			////allParametersForm.Pane.Size = designedSize;
			////NULL allParametersForm.PanelPane.Size = designedSize;
			////allParametersForm.DockPanel.Size = designedSize;
			////allParametersForm.ParentForm.Size = designedSize;
		}

		void sequencerControl_OnCopyToContextNewBacktest(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			this.sequencerControl_OnCopyToContextNew(sender, e);
			this.chartFormManager.BacktesterRunSimulation();
		}
		void sequencerControl_OnCopyToContextNew(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			SystemPerformanceRestoreAble ctxAdding = e.SystemPerformanceRestoreAble;
			string scriptContextNewName = e.ScriptContextNewName;
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			strategyOnChart.ScriptContextAdd_cloneAndAbsorbCurrentValues_fromSequencer(scriptContextNewName, ctxAdding, false);
			SlidersForm.Instance.Show();
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(e.ScriptContextNewName);
		}
		void sequencerControl_OnCopyToContextDefaultBacktest(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			this.sequencerControl_OnCopyToContextDefault_handler(e, true);
		}
		void sequencerControl_OnCopyToContextDefault(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			this.sequencerControl_OnCopyToContextDefault_handler(e, false);
		}

		void sequencerControl_OnCopyToContextDefault_handler(SystemPerformanceRestoreAbleEventArgs e, bool switchToDefaultAndBacktest = false) {
			SystemPerformanceRestoreAble sperfParametersToAbsorbIntoDefault = e.SystemPerformanceRestoreAble;
			string mustBeNull = e.ScriptContextNewName;
			if (string.IsNullOrEmpty(mustBeNull) == false) {
				string msg = "MUST_BE_NULL e.ScriptContextNewName[" + mustBeNull + "] //sequencerControl_OnCopyToContextDefault()";
				Assembler.PopupException(msg);
			}
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			if (strategyOnChart.ScriptContextsByName.ContainsKey(ContextScript.DEFAULT_NAME) == false) {
				string msg = "strategyOnChart.ScriptContextsByName.ContainsKey(Default) == false";
				Assembler.PopupException(msg);
				return;
			}
			ContextChart ctxChartDefault = strategyOnChart.ScriptContextsByName[ContextScript.DEFAULT_NAME];
			ContextScript ctxScriptDefault = ctxChartDefault as ContextScript;
			if (ctxScriptDefault == null) {
				string msg = "NONSENSE_THAT_YOU_WERE_OPTIMIZING_CHART_HAVING_NO_SCRIPT_CONTEXT...";
				Assembler.PopupException(msg);
				return;
			}
			ctxScriptDefault.AbsorbOnlyScriptAndIndicatorParameterCurrentValues_fromSequencer(sperfParametersToAbsorbIntoDefault);
			strategyOnChart.Serialize();

			if (switchToDefaultAndBacktest) {
				strategyOnChart.ContextSwitch_currentToNamed_serialize(ContextScript.DEFAULT_NAME, false);
				this.chartFormManager.BacktesterRunSimulation();
			}
			SlidersForm.Instance.Show();
			SlidersForm.Instance.Initialize(SlidersForm.Instance.SteppingSlidersAutoGrowControl.Strategy);
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ContextScript.DEFAULT_NAME);
		}

		void sequencerForm_FormClosing(object sender, FormClosingEventArgs e) {
			// only when user closed => allow scriptEditorForm_FormClosed() to serialize
			if (this.chartFormManager.MainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
				e.Cancel = true;
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				e.Cancel = true;
				return;
			}
		}
		void sequencerForm_FormClosed(object sender, FormClosedEventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowSequencer.Checked = false;
			this.chartFormManager.SequencerForm = null;
			this.chartFormManager.Strategy.Serialize();
			this.chartFormManager.MainForm.MainFormSerialize();

			SequencerForm dontLeakMemory = sender as SequencerForm;
			if (dontLeakMemory == null) {
				string msg = "CLOSED_SequencerControl_CANT_BE_DISPOSED_SINCE_IS_STILL_SUBSCRIBED (sender[" + sender.GetType() + "] as SequencerForm)=null";
				Assembler.PopupException(msg);
				return;
			}

			// DONT_FORGET_TO_MENTION_ALL_SUBSCRIPTIONS__COPYPASTE_FROM_SequencerFormFactory.CreateSequencerFormSubscribe()
			dontLeakMemory.SequencerControl.OnCopyToContextDefault			-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextDefault);
			dontLeakMemory.SequencerControl.OnCopyToContextDefaultBacktest	-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextDefaultBacktest);
			dontLeakMemory.SequencerControl.OnCopyToContextNew				-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextNew);
			dontLeakMemory.SequencerControl.OnCopyToContextNewBacktest		-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextNewBacktest);
			dontLeakMemory.SequencerControl.OnCorrelatorShouldPopulate		-= new EventHandler<SequencedBacktestsEventArgs>(this.sequencerControl_OnCorrelatorShouldPopulate);
			dontLeakMemory.FormClosing										-= new FormClosingEventHandler(this.sequencerForm_FormClosing);
			dontLeakMemory.FormClosed										-= new FormClosedEventHandler(this.sequencerForm_FormClosed);
		}
	}
}
