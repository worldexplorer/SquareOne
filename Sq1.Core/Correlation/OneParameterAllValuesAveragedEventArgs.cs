using System;

namespace Sq1.Core.Correlation {
	public class OneParameterAllValuesAveragedEventArgs : EventArgs {
		public OneParameterAllValuesAveraged parameterChangedOptimizationCriterion;

		public OneParameterAllValuesAveragedEventArgs(OneParameterAllValuesAveraged parameterChangedOptimizationCriterion) {
			this.parameterChangedOptimizationCriterion = parameterChangedOptimizationCriterion;
		}
	}
}
