using System;

namespace Sq1.Core.Indicators {
	public partial class Indicator {
		public event EventHandler	OnIndicatorPeriodChanged;
		
		protected void RaiseOnIndicatorPeriodChanged() {
		    if (this.OnIndicatorPeriodChanged == null) return;
		    try {
		        this.OnIndicatorPeriodChanged(this, null);
		    } catch (Exception ex) {
		        string msg = "Indicator.OnIndicatorPeriodChanged(" + this + ")";
		        Assembler.PopupException(msg, ex, false);
		    }
		}
	}
}
