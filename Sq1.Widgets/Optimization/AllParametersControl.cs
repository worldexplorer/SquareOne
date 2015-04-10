using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public partial class AllParametersControl : UserControl {
		private SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults;
		private OptimizationResultsTransposer transposer;

		public AllParametersControl() {
			InitializeComponent();
		}

		public void Initialize(SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults) {
			this.originalOptimizationResults = originalOptimizationResults;
			this.transposer = new OptimizationResultsTransposer(originalOptimizationResults.SystemPerformanceRestoreAbleList);

			this.flowLayoutPanel1.Controls.Clear();
			foreach (string parameterName in this.transposer.KPIsAveragedForEachParameterValues.Keys) {
				OneParameterAllValuesAveraged oneParamForOneOLV = this.transposer.KPIsAveragedForEachParameterValues[parameterName];
				//List<OneParameterOneValue> objectsForList = oneParamForOneOLV.AllValuesForOneParameterWithAverages;
				OneParameterControl adding = new OneParameterControl(oneParamForOneOLV);
				adding.OnKPIsLocalRecalculate += new EventHandler<EventArgs>(adding_OnKPIsLocalRecalculate);
				this.flowLayoutPanel1.Controls.Add(adding);
			}
		}

		void adding_OnKPIsLocalRecalculate(object sender, EventArgs e) {
			OneParameterControl refreshMe = sender as OneParameterControl;
			transposer.OneParameterOneValueUserSelectedChanged_recalculateAllKPIsLocal();
			refreshMe.KPIsLocalRecalculateDone_refreshOLV();
		}

	}
}
