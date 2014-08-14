using System;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		public event EventHandler<EventArgs> OnStreamingButtonStateChanged;
		
		void RaiseStreamingButtonStateChanged() {
			if (this.OnStreamingButtonStateChanged == null) return;
			this.OnStreamingButtonStateChanged(this, null);
		}
	}
}
