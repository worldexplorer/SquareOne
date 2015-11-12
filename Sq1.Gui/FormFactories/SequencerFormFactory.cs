using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui.FormFactories {
	public class SequencerFormFactory {	// REASON_TO_EXIST: way to refresh Sliders and run Chart.Backtest() for added ContextScript from Sq1.Widgets.dll:SequencerControl
		ChartFormsManager chartFormManager;

		public SequencerFormFactory(ChartFormsManager chartFormsManager) {
			this.chartFormManager = chartFormsManager;
		}

		public SequencerForm CreateSequencerFormSubscribe() {
			SequencerForm sequencerForm = new SequencerForm(this.chartFormManager);
			sequencerForm.SequencerControl.OnCopyToContextDefault			+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextDefault);
			sequencerForm.SequencerControl.OnCopyToContextDefaultBacktest	+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextDefaultBacktest);
			sequencerForm.SequencerControl.OnCopyToContextNew				+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextNew);
			sequencerForm.SequencerControl.OnCopyToContextNewBacktest		+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextNewBacktest);
			sequencerForm.SequencerControl.OnCorrelatorShouldPopulate		+= new EventHandler<SequencedBacktestsEventArgs>(sequencerControl_OnCorrelatorShouldPopulate);
			sequencerForm.FormClosing										+= new FormClosingEventHandler(sequencerForm_FormClosing);
			sequencerForm.FormClosed										+= new FormClosedEventHandler(sequencerForm_FormClosed);
			return sequencerForm;
		}

		void sequencerControl_OnCorrelatorShouldPopulate(object sender, SequencedBacktestsEventArgs e) {
			if (this.chartFormManager.CorrelatorForm == null) {
				string msg = "don't force Correlator showup if I clicked only mniShowSequencer and it wasn't restored from XML";
				return;
			}
			CorrelatorForm correlatorForm = this.chartFormManager.CorrelatorFormConditionalInstance;
			correlatorForm.PopulateSequencedHistory(e.SequencedBacktests);
			if (this.chartFormManager.SequencerForm.Visible) {
				correlatorForm.ActivateDockContentPopupAutoHidden(false);
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
			strategyOnChart.ScriptContextAdd_cloneAndAbsorbCurrentValuesFromSequencer(scriptContextNewName, ctxAdding, false);
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
				strategyOnChart.ContextSwitchCurrentToNamedAndSerialize(ContextScript.DEFAULT_NAME, false);
				this.chartFormManager.BacktesterRunSimulation();
			}
			SlidersForm.Instance.Show();
			SlidersForm.Instance.Initialize(SlidersForm.Instance.SteppingSlidersAutoGrowControl.Strategy);
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ContextScript.DEFAULT_NAME);
		}
		void sequencerForm_FormClosing(object sender, FormClosingEventArgs e) {
			// only when user closed => allow scriptEditorForm_FormClosed() to serialize
			if (this.chartFormManager.MainForm.MainFormClosingSkipChartFormsRemoval) {
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
			this.chartFormManager.Strategy.Serialize();
			this.chartFormManager.MainForm.MainFormSerialize();

			SequencerForm dontLeakMemory = sender as SequencerForm;
			if (dontLeakMemory == null) {
				string msg = "CLOSED_SequencerControl_CANT_BE_DISPOSED_SINCE_IS_STILL_SUBSCRIBED (sender[" + sender.GetType() + "] as SequencerForm)=null";
				Assembler.PopupException(msg);
				return;
			}

			// DONT_FORGET_TO_MENTION_ALL_SUBSCRIPTIONS__COPYPASTE_FROM_SequencerFormFactory.CreateSequencerFormSubscribe()
			dontLeakMemory.SequencerControl.OnCopyToContextDefault			-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextDefault);
			dontLeakMemory.SequencerControl.OnCopyToContextDefaultBacktest	-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextDefaultBacktest);
			dontLeakMemory.SequencerControl.OnCopyToContextNew				-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextNew);
			dontLeakMemory.SequencerControl.OnCopyToContextNewBacktest		-= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(sequencerControl_OnCopyToContextNewBacktest);
			dontLeakMemory.SequencerControl.OnCorrelatorShouldPopulate		-= new EventHandler<SequencedBacktestsEventArgs>(sequencerControl_OnCorrelatorShouldPopulate);
		}
	}
}
