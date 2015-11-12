using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Gui.Forms {
	public partial class SequencerForm {
        void sequencerForm_FormClosing(object sender, FormClosingEventArgs e) {
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
        void sequencerForm_FormClosed(object sender, FormClosedEventArgs e) {
            // both at FormCloseByX and MainForm.onClose()
            this.chartFormsManager.ChartForm.MniShowSequencer.Checked = false;
            this.chartFormsManager.MainForm.MainFormSerialize();
			// NOT_MY_JOB__FORM.CLOSE()_SENDS_MESSAGE_WHICH_DISPOSES_ALL_INNER_COMPONENTS/CONTROLS this.Dispose(true); this.Dispose();

			this.chartFormsManager.SequencerForm = null;
        }
		void sequencer_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt(object sender, SequencedBacktestsEventArgs e) {
			this.chartFormsManager.SequencerFormConditionalInstance.SequencerControl.BacktestsReplaceWithCorrelated(e.SequencedBacktests);
		}

	}
}
