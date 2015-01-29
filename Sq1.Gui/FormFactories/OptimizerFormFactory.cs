using System;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui.FormFactories {
	public class OptimizerFormFactory {	// REASON_TO_EXIST: way to refresh Sliders and run Chart.Backtest() for added ContextScript from Sq1.Widgets.dll:OptimizationControl
		ChartFormManager chartFormManager;

		public OptimizerFormFactory(ChartFormManager chartFormsManager) {
			this.chartFormManager = chartFormsManager;
		}

		OptimizerForm OptimizerForm {
			get { return this.chartFormManager.OptimizerFormConditionalInstance; }
			set { this.chartFormManager.OptimizerForm = value; }
		}

		public void CreateOptimizerFormSubscribePushToManager(ChartFormManager chartFormsManager) {
			this.OptimizerForm = new OptimizerForm(chartFormsManager);
			this.OptimizerForm.OptimizerControl.OnCopyToContextDefault			+= new EventHandler<ContextScriptEventArgs>(optimizerControl_OnCopyToContextDefault);
			this.OptimizerForm.OptimizerControl.OnCopyToContextDefaultBacktest	+= new EventHandler<ContextScriptEventArgs>(optimizerControl_OnCopyToContextDefaultBacktest);
			this.OptimizerForm.OptimizerControl.OnCopyToContextNew				+= new EventHandler<ContextScriptEventArgs>(optimizerControl_OnCopyToContextNew);
			this.OptimizerForm.OptimizerControl.OnCopyToContextNewBacktest		+= new EventHandler<ContextScriptEventArgs>(optimizerControl_OnCopyToContextNewBacktest);
			this.OptimizerForm.Disposed += optimizerForm_Disposed;
		}
		void optimizerControl_OnCopyToContextNewBacktest(object sender, ContextScriptEventArgs e) {
			ContextScript ctxAdding = e.ContextScript;
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			strategyOnChart.ScriptContextAdd(ctxAdding.Name, ctxAdding, true);
			this.chartFormManager.BacktesterRunSimulation();
			SlidersForm.Instance.Show();
			SlidersForm.Instance.SlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ctxAdding.Name);
		}
		void optimizerControl_OnCopyToContextNew(object sender, ContextScriptEventArgs e) {
			ContextScript ctxAdding = e.ContextScript;
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			strategyOnChart.ScriptContextAdd(ctxAdding.Name, ctxAdding, false);
			SlidersForm.Instance.Show();
			SlidersForm.Instance.SlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ctxAdding.Name);
		}
		void optimizerControl_OnCopyToContextDefaultBacktest(object sender, ContextScriptEventArgs e) {
			this.optimizerControl_OnCopyToContextDefault(sender, e);
			this.chartFormManager.BacktesterRunSimulation();
		}
		void optimizerControl_OnCopyToContextDefault(object sender, ContextScriptEventArgs e) {
			ContextScript ctxAdding = e.ContextScript;
			Strategy strategyOnChart = this.chartFormManager.Strategy;
			if (strategyOnChart.ScriptContextsByName.ContainsKey(ContextScript.DEFAULT_NAME) == false) {
				string msg = "strategyOnChart.ScriptContextsByName.ContainsKey(Default) == false";
				Assembler.PopupException(msg);
				return;
			}
			strategyOnChart.ScriptContextsByName[ContextScript.DEFAULT_NAME].AbsorbFrom(ctxAdding);
			SlidersForm.Instance.Show();
			SlidersForm.Instance.Initialize(SlidersForm.Instance.SlidersAutoGrowControl.Strategy);
			SlidersForm.Instance.SlidersAutoGrowControl.PopupScriptContextsToConfirmAddedOptimized(ContextScript.DEFAULT_NAME);
		}
		void optimizerForm_Disposed(object sender, EventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowOptimizer.Checked = false;
			this.OptimizerForm = null;
		}
	}
}
