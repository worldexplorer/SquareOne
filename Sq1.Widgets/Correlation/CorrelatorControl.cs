using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl : UserControl {
				SystemPerformanceRestoreAbleListEventArgs	sequencedOriginal;
				SystemPerformanceRestoreAbleListEventArgs	sequencedChosen;
		public	Correlator									Correlator			{ get; private set; }
		public  string										PriceFormat			{ get; private set; }

		public CorrelatorControl() {
			InitializeComponent();
		}

		public void Initialize(Correlator correlator) {
			this.Correlator = correlator;
			this.oneParameterControl1.Initialize_byMovingControlsToInner();
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
				this.setCursorWait();
				this.Correlator.Initialize(originalOptimizationResults.SystemPerformanceRestoreAbleList
													, relPathAndNameForSequencerResults, fileName);
				this.flushCalculationsToGui();
			} finally {
				this.setCursorDefault();
			}
		}

		void flushCalculationsToGui() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.flushCalculationsToGui(); });
				return;
			}
			this.flowLayoutPanel1.Controls.Clear();
			foreach (string parameterName in this.Correlator.ParametersByName.Keys) {
				OneParameterAllValuesAveraged oneParamForOneOLV = this.Correlator.ParametersByName[parameterName];
				if (oneParamForOneOLV.ValuesByParam.Count <= 1) continue;
				//List<OneParameterOneValue> objectsForList = oneParamForOneOLV.AllValuesForOneParameterWithAverages;
				OneParameterControl adding = new OneParameterControl(this, oneParamForOneOLV);
				//OneParameterControlAsMdiForm adding = new OneParameterControlAsMdiForm(this, oneParamForOneOLV);
				//OneParameterControl adding = resizableWrapper.OneParameterControl;
				this.flowLayoutPanel1.Controls.Add(adding);
			}
		}

		void setCursorDefault() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.setCursorDefault(); });
				return;
			}
			this.Cursor = Cursors.Default;
		}

		void setCursorWait() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.setCursorWait(); });
				return;
			}
			this.Cursor = Cursors.WaitCursor;
		}
	}
}
