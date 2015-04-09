using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Optimization;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui.FormFactories {
	public class OptimizerFormFactory {	// REASON_TO_EXIST: way to refresh Sliders and run Chart.Backtest() for added ContextScript from Sq1.Widgets.dll:OptimizationControl
		ChartFormManager chartFormManager;

		public OptimizerFormFactory(ChartFormManager chartFormsManager) {
			this.chartFormManager = chartFormsManager;
		}

		public OptimizerForm CreateOptimizerFormSubscribe() {
			OptimizerForm optimizerForm = new OptimizerForm(this.chartFormManager);
			optimizerForm.OptimizerControl.OnCopyToContextDefault			+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(optimizerControl_OnCopyToContextDefault);
			optimizerForm.OptimizerControl.OnCopyToContextDefaultBacktest	+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(optimizerControl_OnCopyToContextDefaultBacktest);
			optimizerForm.OptimizerControl.OnCopyToContextNew				+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(optimizerControl_OnCopyToContextNew);
			optimizerForm.OptimizerControl.OnCopyToContextNewBacktest		+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(optimizerControl_OnCopyToContextNewBacktest);
			optimizerForm.FormClosing										+= new FormClosingEventHandler(optimizerForm_FormClosing);
			optimizerForm.FormClosed										+= new  FormClosedEventHandler(optimizerForm_FormClosed);
			return optimizerForm;
		}

		void optimizerControl_OnCopyToContextNewBacktest(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			this.optimizerControl_OnCopyToContextNew(sender, e);
			this.chartFormManager.BacktesterRunSimulation();
		}
		void optimizerControl_OnCopyToContextNew(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			SystemPerformanceRestoreAble ctxAdding = e.SystemPerformanceRestoreAble;
			string scriptContextNewName = e.ScriptContextNewName;
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			strategyOnChart.ScriptContextAdd_cloneAndAbsorbCurrentValuesFromOptimizer(scriptContextNewName, ctxAdding, false);
			SlidersForm.Instance.Show();
			SlidersForm.Instance.SlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ctxAdding.Name);
		}
		void optimizerControl_OnCopyToContextDefaultBacktest(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			this.optimizerControl_OnCopyToContextDefault(sender, e);
			this.chartFormManager.BacktesterRunSimulation();
		}
		void optimizerControl_OnCopyToContextDefault(object sender, SystemPerformanceRestoreAbleEventArgs e) {
			SystemPerformanceRestoreAble sperfParametersToAbsorbIntoDefault = e.SystemPerformanceRestoreAble;
			string mustBeNull = e.ScriptContextNewName;
			if (string.IsNullOrEmpty(mustBeNull) == false) {
				string msg = "MUST_BE_NULL e.ScriptContextNewName[" + mustBeNull+ "] //optimizerControl_OnCopyToContextDefault()";
				Assembler.PopupException(msg);
			}
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			if (strategyOnChart.ScriptContextsByName.ContainsKey(ContextScript.DEFAULT_NAME) == false) {
				string msg = "strategyOnChart.ScriptContextsByName.ContainsKey(Default) == false";
				Assembler.PopupException(msg);
				return;
			}
			ContextChart  ctxChart  = strategyOnChart.ScriptContextsByName[ContextScript.DEFAULT_NAME];
			ContextScript ctxScript = ctxChart as ContextScript;
			if (ctxScript == null) {
				string msg = "NONSENSE_THAT_YOU_WERE_OPTIMIZING_CHART_HAVING_NO_SCRIPT_CONTEXT...";
				Assembler.PopupException(msg);
				return;
			}
			ctxScript.AbsorbOnlyScriptAndIndicatorParameterCurrentValues_fromOptimizer(sperfParametersToAbsorbIntoDefault);
			strategyOnChart.Serialize();
			SlidersForm.Instance.Show();
			SlidersForm.Instance.Initialize(SlidersForm.Instance.SlidersAutoGrowControl.Strategy);
			SlidersForm.Instance.SlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ContextScript.DEFAULT_NAME);
		}
		void optimizerForm_FormClosing(object sender, FormClosingEventArgs e) {
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
		void optimizerForm_FormClosed(object sender, FormClosedEventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowOptimizer.Checked = false;
			this.chartFormManager.MainForm.MainFormSerialize();
		}

	}
}
