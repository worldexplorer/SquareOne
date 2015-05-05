using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Gui.Forms {
	public partial class CorrelatorForm {
        void correlatorForm_FormClosing(object sender, FormClosingEventArgs e) {
            // only when user closed => allow scriptEditorForm_FormClosed() to serialize
            if (this.chartFormsManager.MainForm.MainFormClosingSkipChartFormsRemoval) {
                e.Cancel = true;
                return;
            }
            if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
                e.Cancel = true;
                return;
            }
        }
        void correlatorForm_FormClosed(object sender, FormClosedEventArgs e) {
            // both at FormCloseByX and MainForm.onClose()
            this.chartFormsManager.ChartForm.MniShowCorrelator.Checked = false;
            this.chartFormsManager.MainForm.MainFormSerialize();
			this.chartFormsManager.SequencerFormConditionalInstance.SequencerControl.BacktestsRestoreCorrelatedClosed();
        }
		void correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt(object sender, SequencedBacktestsEventArgs e) {
			this.chartFormsManager.SequencerFormConditionalInstance.SequencerControl.BacktestsReplaceWithCorrelated(e.SequencedBacktests.BacktestsReadonly);
		}

	}
}
