using System;

using Sq1.Core;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl {
		void toolStripItemTrackBar1_ValueCurrentChanged(object sender, EventArgs e) {
			this.Correlator.SubsetPercentagePropagate((double)this.toolStripItemTrackBarWalkForward.ValueCurrent);
			this.toolStripItemTrackBarWalkForward.LabelText = this.Correlator.SubsetWaterLineDateTime.ToString(Assembler.DateTimeFormatToMinutes);
			// DEAD_END: 90% => click WalkForward => 100% => WalkForward gets disabled => what's next???
			//this.toolStripItemTrackBarWalkForward.WalkForwardEnabled = this.toolStripItemTrackBarWalkForward.ValueCurrent < 100;
		}
		void toolStripItemTrackBar1_WalkForwardCheckedChanged(object sender, EventArgs e) {
			this.Correlator.SubsetPercentageFromEndPropagate(this.toolStripItemTrackBarWalkForward.WalkForwardChecked);
			this.toolStripItemTrackBarWalkForward.FillFromCurrentToMax = this.toolStripItemTrackBarWalkForward.WalkForwardChecked;
		}
	}
}
