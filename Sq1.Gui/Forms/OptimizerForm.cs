using System;
using System.Windows.Forms;

using Sq1.Widgets;
using Sq1.Core;

namespace Sq1.Gui.Forms {
	public partial class OptimizerForm : DockContentImproved {
		ChartFormManager chartFormManager;
		
		public OptimizerForm() {
			InitializeComponent();
		}
		
		public OptimizerForm(ChartFormManager chartFormManager) : this() {
			this.Initialize(chartFormManager);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Optimizer:" + this.OptimizerControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.WindowTitlePullFromStrategy();
			this.OptimizerControl.Initialize(this.chartFormManager.Executor.Optimizer);
		}

		public void WindowTitlePullFromStrategy() {
			string windowTitle = "Optimizer :: " + this.chartFormManager.Strategy.Name;
			if (this.chartFormManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
			if (this.chartFormManager.ScriptEditedNeedsSaving) {
				windowTitle = ChartFormManager.PREFIX_FOR_UNSAVED_STRATEGY_SOURCE_CODE + windowTitle;
			}
			this.Text = windowTitle;
		}
	}
}
