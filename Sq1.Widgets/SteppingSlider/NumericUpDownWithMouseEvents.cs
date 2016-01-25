using System;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Widgets.SteppingSlider {
	public class NumericUpDownWithMouseEvents : NumericUpDown {
		public event EventHandler OnArrowUpStepAdd;
		public event EventHandler OnArrowDownStepSubstract;

		public override void UpButton() {
			base.UpButton();
			if (this.OnArrowUpStepAdd == null) return;
			try {
				this.OnArrowUpStepAdd(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("ONE_OF_SUBSCRIBERS_THREW //DomainUpDownWithMouseEvents.UpButton()", ex);
			}
		}
		public override void DownButton() {
			base.DownButton();
			if (this.OnArrowDownStepSubstract == null) return;
			try {
				this.OnArrowDownStepSubstract(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("ONE_OF_SUBSCRIBERS_THREW //DomainUpDownWithMouseEvents.DownButton()", ex);
			}
		}
	}
}
