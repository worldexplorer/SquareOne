using System;

namespace Sq1.Core.Optimization {
	public class SystemPerformanceRestoreAbleEventArgs : EventArgs {
		public SystemPerformanceRestoreAble SystemPerformanceRestoreAble {get; private set;}
		public SystemPerformanceRestoreAbleEventArgs(SystemPerformanceRestoreAble systemPerformanceRestoreAble) {
			this.SystemPerformanceRestoreAble = systemPerformanceRestoreAble;
		}
	}
}
