using System;

using Sq1.Core;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl {
		void toolStripItemTrackBar1_ValueCurrentChanged(object sender, EventArgs e) {
			this.Correlator.SubsetPercentagePropagate((double)this.toolStripItemTrackBar1.ValueCurrent);
			this.toolStripItemTrackBar1.LabelText = this.Correlator.SubsetWaterLineDateTime.ToString(Assembler.DateTimeFormatToMinutes);
		}
		void toolStripItemTrackBar1_WalkForwardCheckedChanged(object sender, EventArgs e) {
			this.Correlator.SubsetPercentageFromEndPropagate(this.toolStripItemTrackBar1.WalkForwardChecked);
		}
	}
}
