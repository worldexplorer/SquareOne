using System;

namespace Sq1.Charting {
	public partial class PanelBase {
		public event EventHandler<EventArgs> OnPanelPriceSqueezed;
		
		public void RaisePanelPriceSqueezed() {
			if (this.OnPanelPriceSqueezed == null) return;
			this.OnPanelPriceSqueezed(this, null);
		}
	}
}