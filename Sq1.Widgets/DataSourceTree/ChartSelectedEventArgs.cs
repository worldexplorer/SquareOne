using System;

using Sq1.Core.Charting;

namespace Sq1.Widgets.DataSourceTree {
	public class ChartSelectedEventArgs : EventArgs {
		public ChartShadow ChartShadow { get; private set; }

		public ChartSelectedEventArgs(ChartShadow chartShadow) {
			this.ChartShadow = chartShadow;
		}
	}
}
