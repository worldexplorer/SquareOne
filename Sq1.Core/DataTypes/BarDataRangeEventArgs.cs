using System;

namespace Sq1.Core.DataTypes {
	public class BarDataRangeEventArgs : EventArgs {
		public BarDataRange BarDataRange { get; private set; }
		public BarDataRangeEventArgs(BarDataRange barDataRange) {
			this.BarDataRange = barDataRange.Clone();
		}
	}
}