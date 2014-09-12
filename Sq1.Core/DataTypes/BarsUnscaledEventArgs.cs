using System;

namespace Sq1.Core.DataTypes {
	public class BarsUnscaledEventArgs : EventArgs {
		public BarsUnscaled BarsUnscaled { get; private set; }
		public BarsUnscaledEventArgs(BarsUnscaled barsUnscaled) {
			this.BarsUnscaled = barsUnscaled;
		}
	}
}