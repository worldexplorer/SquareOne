using System;
using Sq1.Core;

namespace Sq1.Charting.MultiSplit {
	public partial class MultiSplitter {
		protected override void OnResize(EventArgs e) {
			string msg = "WHO_INVOKES_ME?";
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_SOLVED_AS_WELL NO_EFFECT
			base.OnResize(e);
			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_SOLVED_AS_WELL GOES_VERY_BAD
			base.Invalidate();	// NO_TRANSFER_TO_THEM_RESIZE_HOPING_THEY_WILL_REPAINT_FULL_NEW_SIZE
		}
	}
}