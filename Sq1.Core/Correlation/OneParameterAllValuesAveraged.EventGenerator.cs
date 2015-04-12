using System;

namespace Sq1.Core.Correlation {
	public partial class OneParameterAllValuesAveraged {
		public event EventHandler<OneParameterAllValuesAveragedEventArgs> OnParameterRecalculatedLocalsAndDeltas;
		
		public void RaiseOnEachParameterRecalculatedLocalsAndDeltas() {
			if (this.OnParameterRecalculatedLocalsAndDeltas == null) return;
			try {
				this.OnParameterRecalculatedLocalsAndDeltas(this, new OneParameterAllValuesAveragedEventArgs(this));
			} catch (Exception ex) {
				string msg = "OneParameterControl_WASNT_READY_TO_GET_BACK_RECALCULATED_KPIs //RaiseOnEachParameterRecalculatedLocalsAndDeltas()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
