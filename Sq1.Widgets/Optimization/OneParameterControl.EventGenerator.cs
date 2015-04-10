using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public partial class OneParameterControl {
		public event EventHandler<EventArgs> OnKPIsLocalRecalculate;

		public void RaiseOnKPIsLocalRecalculate() {
			if (this.OnKPIsLocalRecalculate == null) return;
			try {
				this.OnKPIsLocalRecalculate(this, new EventArgs());
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnKPIsLocalRecalculate()", ex);
			}
		}
	}
}
