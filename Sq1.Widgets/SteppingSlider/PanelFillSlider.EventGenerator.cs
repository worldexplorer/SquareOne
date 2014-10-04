using System;

namespace Sq1.Widgets.SteppingSlider {
	public partial class PanelFillSlider {
		public EventHandler ValueCurrentChanged;
		
		public void RaiseValueCurrentChanged() {
			if (this.ValueCurrentChanged == null) return;
			this.ValueCurrentChanged(this, null);
		}
	}
}
