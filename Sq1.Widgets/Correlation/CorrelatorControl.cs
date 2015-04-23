using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl : UserControl {
				SystemPerformanceRestoreAbleListEventArgs	sequencedOriginal;
				SystemPerformanceRestoreAbleListEventArgs	sequencedChosen;
		public	Correlator									Correlator					{ get; private set; }
		public  string										PriceFormat					{ get; private set; }

		public CorrelatorControl() {
			InitializeComponent();
		}

		public void Initialize(SystemPerformanceRestoreAbleListEventArgs originalOptimizationResults
					, string relPathAndNameForSequencerResults, string fileName) {
			this.sequencedOriginal = originalOptimizationResults;
			if (this.sequencedOriginal.SystemPerformanceRestoreAbleList.Count > 0) {
				int i = 0;
				int scanFirstLimit = 200;
				string symbolFound = null;
				foreach (SystemPerformanceRestoreAble firstScanned in this.sequencedOriginal.SystemPerformanceRestoreAbleList) {
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

			try {
				this.Cursor = Cursors.WaitCursor;
				this.Correlator = new Correlator(originalOptimizationResults.SystemPerformanceRestoreAbleList
				, relPathAndNameForSequencerResults, fileName);

				this.flowLayoutPanel1.Controls.Clear();
				foreach (string parameterName in this.Correlator.ParametersByName.Keys) {
					OneParameterAllValuesAveraged oneParamForOneOLV = this.Correlator.ParametersByName[parameterName];
					if (oneParamForOneOLV.ValuesByParam.Count <= 1) continue;
					//List<OneParameterOneValue> objectsForList = oneParamForOneOLV.AllValuesForOneParameterWithAverages;
					OneParameterControl adding = new OneParameterControl(this, oneParamForOneOLV);
					this.flowLayoutPanel1.Controls.Add(adding);
				}
			} finally {
				this.Cursor = Cursors.Default;
			}
		}
	}
}
