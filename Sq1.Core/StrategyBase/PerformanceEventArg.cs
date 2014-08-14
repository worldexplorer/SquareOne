using System;

namespace Sq1.Core.StrategyBase {
	public class PerformanceEventArg : EventArgs {
		public SystemPerformance Performance;

		public PerformanceEventArg(SystemPerformance systemPerformance) {
			this.Performance = systemPerformance;
		}
	}
}
