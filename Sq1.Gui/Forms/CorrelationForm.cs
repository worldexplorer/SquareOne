using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Sequencing;
using Sq1.Widgets;
using Sq1.Widgets.Sequencing;

namespace Sq1.Gui.Forms {
	public partial class CorrelationForm : DockContentImproved {
		private ChartFormManager chartFormManager;

		public CorrelationForm() {
			InitializeComponent();
		}

		CorrelationForm(ChartFormManager chartFormManager) : this () {
			this.chartFormManager = chartFormManager;
		}

		public CorrelationForm(ChartFormManager chartFormManager
				, SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults) : this(chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.allParametersControl1.Initialize(originalOptimizationResults);
		}
	}
}
