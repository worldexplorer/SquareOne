using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class OptimizerForm : DockContentImproved {
		private ChartFormManager chartFormManager;
		
		public OptimizerForm() {
			InitializeComponent();
		}
		
		public OptimizerForm(ChartFormManager chartFormsManager) : this() {
			this.Initialize(chartFormsManager);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Optimizer:" + this.optimizerControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.Text = "Optimizer :: " + this.chartFormManager.Strategy.Name;
			this.optimizerControl.Initialize(this.chartFormManager.Executor);
		}
	}
}
