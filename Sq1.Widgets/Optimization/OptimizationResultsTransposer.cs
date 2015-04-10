using System.Collections.Generic;

using Sq1.Core.Optimization;
using Sq1.Core.Indicators;

namespace Sq1.Widgets.Optimization {
	public class OptimizationResultsTransposer {
		public Dictionary<string, OneParameterAllValuesAveraged> KPIsAveragedForEachParameterValues;

		public OptimizationResultsTransposer(List<SystemPerformanceRestoreAble> optimizationResults) {
			KPIsAveragedForEachParameterValues = new Dictionary<string, OneParameterAllValuesAveraged>();

			int iterationCouter_fixBadDeserialization = 0;
			foreach (SystemPerformanceRestoreAble eachRun in optimizationResults) {
				if (eachRun.OptimizationIterationSerno == 0) eachRun.OptimizationIterationSerno = iterationCouter_fixBadDeserialization;
				iterationCouter_fixBadDeserialization++;

				foreach (string indicatorDotParameter in
						eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator =
						eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double optimizedValue = eachIndicator.ValueCurrent;

					if (this.KPIsAveragedForEachParameterValues.ContainsKey(parameterName) == false) {
						this.KPIsAveragedForEachParameterValues.Add(parameterName, new OneParameterAllValuesAveraged(parameterName));
					}
					OneParameterAllValuesAveraged eachParameter = this.KPIsAveragedForEachParameterValues[parameterName];
					eachParameter.AddKPIsForIndicatorValue(optimizedValue, eachRun);
				}
			}
			foreach (OneParameterAllValuesAveraged eachParameter in this.KPIsAveragedForEachParameterValues.Values) {
				eachParameter.NoMoreGlobalParameters_DivideTotalsByCount();
			}
		}

		internal void OneParameterOneValueUserSelectedChanged_recalculateAllKPIsLocal() {
			//throw new System.NotImplementedException();
		}
	}
}
