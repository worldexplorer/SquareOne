using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl : UserControl {
				SequencedBacktests		sequencedOriginal;
				SequencedBacktests		sequencedChosen;
		public	Correlator				Correlator			{ get; private set; }
		public  string					PriceFormat			{ get; private set; }

		public CorrelatorControl() {
			InitializeComponent();
			this.toolStripItemTrackBarWalkForward.ValueCurrentChanged += new EventHandler<EventArgs>(toolStripItemTrackBar1_ValueCurrentChanged);
			this.toolStripItemTrackBarWalkForward.WalkForwardCheckedChanged += new EventHandler<EventArgs>(toolStripItemTrackBar1_WalkForwardCheckedChanged);
		}

		public void Initialize(Correlator correlator) {
			this.Correlator = correlator;
			this.oneParameterControl1.Initialize_byMovingControlsToInner();
		}

		public void Initialize(SequencedBacktests originalSequencedBacktests, string relPathAndNameForSequencerResults, string fileName) {
			if (originalSequencedBacktests == null) {
				string msg = "DONT_PASS_NULL_originalSequencedBacktests";
				Assembler.PopupException(msg);
				return;
			}
			if (this.sequencedOriginal != null && this.sequencedOriginal.ToString() == originalSequencedBacktests.ToString()) return;
			this.sequencedOriginal = originalSequencedBacktests;
			if (this.sequencedOriginal.Count > 0) {
				//v1
//				int i = 0;
//				int scanFirstLimit = 200;
//				string symbolFound = null;
//				foreach (SystemPerformanceRestoreAble firstScanned in this.sequencedOriginal.Backtests) {
//					if (string.IsNullOrEmpty(firstScanned.Symbol) == false) {
//						symbolFound = firstScanned.Symbol;
//					}
//					this.PriceFormat = firstScanned.PriceFormat;
//					if (string.IsNullOrEmpty(this.PriceFormat) == false) break;
//					if (++i >= scanFirstLimit) break;
//				}
				string symbolFound = this.sequencedOriginal.Symbol;
				if (string.IsNullOrEmpty(this.PriceFormat) && string.IsNullOrEmpty(symbolFound) == false) {
					this.PriceFormat = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(symbolFound).PriceFormat;
				}
			}
			if (string.IsNullOrEmpty(this.PriceFormat)) {
				this.PriceFormat = "N1";
			}

			try {
				this.setCursorWait();
				this.Correlator.Initialize(originalSequencedBacktests, relPathAndNameForSequencerResults, fileName);

				// only available after Correlator.Initialize otherwize NPE due to this.sequencedOriginal=null
				this.toolStripItemTrackBarWalkForward.ValueCurrent = (decimal)this.Correlator.SubsetPercentage;
				this.toolStripItemTrackBarWalkForward.WalkForwardChecked = this.Correlator.SubsetPercentageFromEnd;
				this.toolStripItemTrackBarWalkForward.WalkForwardEnabled = this.toolStripItemTrackBarWalkForward.ValueCurrent < 100;

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
