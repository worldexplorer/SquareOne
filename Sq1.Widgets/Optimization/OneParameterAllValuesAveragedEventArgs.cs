using System;

namespace Sq1.Widgets.Optimization {
	public class OneParameterAllValuesAveragedEventArgs : EventArgs {
		public OneParameterAllValuesAveraged parameterChangedOptimizationCriterion;

		public OneParameterAllValuesAveragedEventArgs(OneParameterAllValuesAveraged parameterChangedOptimizationCriterion) {
			this.parameterChangedOptimizationCriterion = parameterChangedOptimizationCriterion;
		}
	}
}
