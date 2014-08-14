using System;

namespace Sq1.Core.DataTypes {
	public class BarsEventArgs : EventArgs {
		public Bars Bars { get; private set; }
		public BarsEventArgs(Bars bars) {
			this.Bars = bars;
		}
	}
}
