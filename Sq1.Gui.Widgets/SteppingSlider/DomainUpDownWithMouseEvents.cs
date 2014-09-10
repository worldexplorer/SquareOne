using System;
using System.Windows.Forms;

namespace Sq1.Widgets.SteppingSlider {
	public class DomainUpDownWithMouseEvents : DomainUpDown {
		public event EventHandler OnArrowUpStepAdd;
		public event EventHandler OnArrowDownStepSubstract;

		public override void UpButton() {
			if (this.OnArrowUpStepAdd == null) return;
			this.OnArrowUpStepAdd(this, null);
		}
		public override void DownButton() {
			if (this.OnArrowDownStepSubstract == null) return;
			this.OnArrowDownStepSubstract(this, null);
		}
	}
}
