using System;

namespace Sq1.Core.DataTypes {
	public class BarScaleIntervalEventArgs : EventArgs {
		public BarScaleInterval BarScaleInterval { get; private set; }
		public BarScaleIntervalEventArgs(BarScaleInterval barScaleInterval) {
			this.BarScaleInterval = barScaleInterval.Clone();
		}
	}
}