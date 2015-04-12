using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Optimization;
using Sq1.Core;

namespace Sq1.Widgets.Optimization {
	public partial class AllParametersControl : UserControl {
		private SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults;
		public Optimizer2 Optimizer { get; private set; }
		public string PriceFormat { get; private set; }

		public AllParametersControl() {
			InitializeComponent();
		}

		public void Initialize(SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults) {
			this.originalOptimizationResults = originalOptimizationResults;
			if (this.originalOptimizationResults.SystemPerformanceRestoreAbleList.Count > 0) {
				int i = 0;
				int scanFirstLimit = 200;
				string symbolFound = null;
				foreach (SystemPerformanceRestoreAble firstScanned in this.originalOptimizationResults.SystemPerformanceRestoreAbleList) {
					if (string.IsNullOrEmpty(firstScanned.Symbol) == false) {
						symbolFound = firstScanned.Symbol;
					}
					this.PriceFormat = firstScanned.PriceFormat;
					if (string.IsNullOrEmpty(this.PriceFormat) == false) break;
					if (++i >= scanFirstLimit) break;
				}
				if (string.IsNullOrEmpty(this.PriceFormat) && string.IsNullOrEmpty(symbolFound) == false) {
					this.PriceFormat = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(symbolFound).PriceFormat;
				}
			}
			if (string.IsNullOrEmpty(this.PriceFormat)) {
				this.PriceFormat = "N1";
			}

			this.Optimizer = new Optimizer2(originalOptimizationResults.SystemPerformanceRestoreAbleList);

			this.flowLayoutPanel1.Controls.Clear();
			foreach (string parameterName in this.Optimizer.ParametersByName.Keys) {
				OneParameterAllValuesAveraged oneParamForOneOLV = this.Optimizer.ParametersByName[parameterName];
				if (oneParamForOneOLV.ValuesByParam.Count <= 1) continue;
				//List<OneParameterOneValue> objectsForList = oneParamForOneOLV.AllValuesForOneParameterWithAverages;
				OneParameterControl adding = new OneParameterControl(this, oneParamForOneOLV);
				this.flowLayoutPanel1.Controls.Add(adding);
			}
		}
	}
}
