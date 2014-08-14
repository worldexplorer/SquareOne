using System;
using System.Windows.Forms;

namespace Sq1.Widgets.SteppingSlider {
	public class DomainUpDownWithMouseEvents : DomainUpDown {
		public event EventHandler onDomainUp;
		public event EventHandler onDomainDown;

		public override void UpButton() {
			if (onDomainUp == null) return;
			onDomainUp(this, null);
		}
		public override void DownButton() {
			if (onDomainDown == null) return;
			onDomainDown(this, null);
		}
	}
}
