using System;

namespace Sq1.Core.Sequencing {
	public class SequencedBacktestsEventArgs : EventArgs {
		public SequencedBacktests	SequencedBacktests	{ get; private set; }

		public SequencedBacktestsEventArgs(SequencedBacktests systemPerformanceRestoreAbleList) {
			SequencedBacktests = systemPerformanceRestoreAbleList;
		}
	}
}
