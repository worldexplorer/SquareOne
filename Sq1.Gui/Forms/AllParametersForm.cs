using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Optimization;
using Sq1.Widgets;
using Sq1.Widgets.Optimization;

namespace Sq1.Gui.Forms {
	public partial class AllParametersForm : DockContentImproved {
		private ChartFormManager chartFormManager;

		public AllParametersForm() {
			InitializeComponent();
		}

		public AllParametersForm(ChartFormManager chartFormManager) : this () {
			this.chartFormManager = chartFormManager;
		}

		public AllParametersForm(ChartFormManager chartFormManager
				, SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults) : this(chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.allParametersControl1.Initialize(originalOptimizationResults);
		}
	}
}
