using System;

namespace Sq1.Widgets.Optimization {
	public class OneParameterOneValueEventArgs : EventArgs {
		public OneParameterOneValue OneParameterOneValueUserSelectedChanged;

		public OneParameterOneValueEventArgs(OneParameterOneValue oneParameterOneValueUserSelectedChanged) {
			this.OneParameterOneValueUserSelectedChanged = oneParameterOneValueUserSelectedChanged;
		}

	}
}