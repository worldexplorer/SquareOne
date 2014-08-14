using System;

namespace Sq1.Core.DataTypes {
	public class BarEventArgs : EventArgs {
		public Bar Bar { get; private set; }
		public BarEventArgs(Bar bar) {
			this.Bar = bar;
		}
	}
}