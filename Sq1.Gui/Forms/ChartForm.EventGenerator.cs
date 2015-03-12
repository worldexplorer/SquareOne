using System;

using Sq1.Core;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		public event EventHandler<EventArgs> OnStreamingButtonStateChanged;
		
		void RaiseStreamingButtonStateChanged() {
			if (this.OnStreamingButtonStateChanged == null) return;
			string msg = "IF_IM_NOT_POPPED_UP_THEN_THERE_IS_NO_CONSUMERS_ANYMORE_IT_WAS_INTERNAL_LOOP_FROM_CHARTFROM_TO_CHARTFORM DELETEME_AFTER_JAN_2015";
			Assembler.PopupException(msg);
			this.OnStreamingButtonStateChanged(this, null);
		}
	}
}
