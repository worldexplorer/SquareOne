using System;

namespace Sq1.Core.Correlation {
	public class OneParameterOneValueEventArgs : EventArgs {
		public OneParameterOneValue OneParameterOneValueUserSelectedChanged;

		public OneParameterOneValueEventArgs(OneParameterOneValue oneParameterOneValueUserSelectedChanged) {
			this.OneParameterOneValueUserSelectedChanged = oneParameterOneValueUserSelectedChanged;
		}

	}
}