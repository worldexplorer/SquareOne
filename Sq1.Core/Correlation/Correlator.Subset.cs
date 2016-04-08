using System;

namespace Sq1.Core.Correlation {
	public partial class Correlator {

		public	double				SubsetPercentage		{ get { return this.SequencedBacktestOriginal.SubsetPercentage; } }
		public	bool				SubsetPercentageFromEnd { get { return this.SequencedBacktestOriginal.SubsetPercentageFromEnd; } }
		public	DateTime			SubsetWaterLineDateTime { get { return this.SequencedBacktestOriginal.SubsetWaterLineDateTime; } }

		public void SubsetPercentagePropagate(double subsetPercentage) {
			this.SequencedBacktestOriginal.SubsetPercentageSetInvalidate(subsetPercentage);
			//this.repositoryJsonCorrelator.SerializeSingle(this.sequencedBacktestOriginal);		// OVERHEAD_RESAVING_FEW_MEGABYTES_FOR_ONE_BOOLEAN_CHANGED
			this.InvalidateBacktestsMinusUnchosen();
			this.calculateGlobalsAndLocals();	//rebuildParametersByName() invoked only once per lifetime, koz I subscribe to event late (3 steps omitted here but you'll fig)
		}
		public void SubsetPercentageFromEndPropagate(bool subsetPercentageFromEnd) {
			this.SequencedBacktestOriginal.SubsetPercentageFromEndSetInvalidate(subsetPercentageFromEnd);
			//this.repositoryJsonCorrelator.SerializeSingle(this.sequencedBacktestOriginal);		// OVERHEAD_RESAVING_FEW_MEGABYTES_FOR_ONE_BOOLEAN_CHANGED
			this.InvalidateBacktestsMinusUnchosen();
			this.calculateGlobalsAndLocals();	//rebuildParametersByName() invoked only once per lifetime, koz I subscribe to event late (3 steps omitted here but you'll fig)		}
		}

	}
}
