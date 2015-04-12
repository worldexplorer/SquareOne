using System;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public partial class Correlator {

		public event EventHandler<SystemPerformanceRestoreAbleListEventArgs> OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters;

		void RaiseOptimizedBacktestsListIsRebuiltWithoutUnchosenParameters() {
			if (this.OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters == null) return;
			try {
				this.OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters(this, new SystemPerformanceRestoreAbleListEventArgs(this.SequencedBacktestsOriginalMinusParameterValuesUnchosen, "SHRINKED_OPTIMIZED_NO_FNAME"));
			} catch (Exception ex) {
				string msg = "SEQUENCER_WASNT_READY_TO_GET_BACK_SHRINKED_SEQUENCED_BACKTESTS //RaiseOptimizedBacktestsListIsRebuiltWithoutUnchosenParameters()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
