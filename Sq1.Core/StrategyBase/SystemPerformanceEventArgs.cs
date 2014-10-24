using System;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformanceEventArgs : EventArgs {
		public SystemPerformance SystemPerformance {get; private set;}
		public SystemPerformanceEventArgs(SystemPerformance systemPerformance) {
			this.SystemPerformance=systemPerformance;
		}
	}
}
