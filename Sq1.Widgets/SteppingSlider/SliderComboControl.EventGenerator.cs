using System;
using Sq1.Core;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SliderComboControl {
		public event EventHandler<EventArgs> ValueCurrentChanged;
		public event EventHandler<EventArgs> ShowBorderChanged;
		public event EventHandler<EventArgs> ShowNumericUpdownChanged;
		
		public void RaiseValueChanged() {
			if (this.ValueCurrentChanged == null) return;
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.ValueCurrentChanged(this, EventArgs.Empty);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}

		public void RaiseShowBorderChanged() {
			if (this.ShowBorderChanged == null) return;
			this.ShowBorderChanged(this, EventArgs.Empty);
		}

		public void RaiseShowNumericUpdownChanged() {
			if (this.ShowNumericUpdownChanged == null) return;
			this.ShowNumericUpdownChanged(this, EventArgs.Empty);
		}
	}
}
