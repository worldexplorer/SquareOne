using System.Collections.Generic;

using Sq1.Core.Optimization;
using Sq1.Core.Indicators;


		using System;
using Sq1.Core;

namespace Sq1.Widgets.Optimization {
	public class Optimizer2 {

		public event EventHandler<SystemPerformanceRestoreAbleListEventArgs> OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters;

		private void RaiseOptimizedBacktestsListIsRebuiltWithoutUnchosenParameters() {
			if (this.OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters == null) return;
			try {
				this.OptimizedBacktestsListIsRebuiltWithoutUnchosenParameters(this, new SystemPerformanceRestoreAbleListEventArgs(this.SequencedBacktestsOriginalMinusParameterValuesUnchosen, "SHRINKED_OPTIMIZED_NO_FNAME"));
			} catch (Exception ex) {
				string msg = "SEQUENCER_WASNT_READY_TO_GET_BACK_SHRINKED_SEQUENCED_BACKTESTS //RaiseOptimizedBacktestsListIsRebuiltWithoutUnchosenParameters()";
				Assembler.PopupException(msg, ex);
			}
		}

		
		object lockForAllRecalculations;
		List<SystemPerformanceRestoreAble> sequencedBacktestsOriginal;

		public Dictionary<string, OneParameterAllValuesAveraged> ParametersByName { get; private set; }


		Dictionary<string, List<double>> valuesUnchosenByParameter_cached;
		Dictionary<string, List<double>> ValuesUnchosenByParameter { get { lock(this.lockForAllRecalculations) {
			if (this.valuesUnchosenByParameter_cached != null) return this.valuesUnchosenByParameter_cached;
			this.valuesUnchosenByParameter_cached = new Dictionary<string,List<double>>();

			foreach (OneParameterAllValuesAveraged param in this.ParametersByName.Values) {
				foreach (OneParameterOneValue val in param.ValuesByParam.Values) {
					if (val.Chosen == true) continue;
					if (this.valuesUnchosenByParameter_cached.ContainsKey(param.ParameterName) == false) {
						this.valuesUnchosenByParameter_cached.Add(param.ParameterName, new List<double>());
					}
					this.valuesUnchosenByParameter_cached[param.ParameterName].Add(val.ValueSequenced);
				}
			}

			return this.valuesUnchosenByParameter_cached;
		} } }


		internal bool HasUnchosenParametersExceptFor(SystemPerformanceRestoreAble eachRegardless) {
			return this.HasUnchosenParametersExceptFor(eachRegardless, null);
		}
		internal bool HasUnchosenParametersExceptFor(SystemPerformanceRestoreAble eachRegardless, OneParameterAllValuesAveraged oneParameterAllValuesAveraged) {
			bool foundInUnchosen = false;
			foreach (string indicatorDotParameter in eachRegardless.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
				IndicatorParameter eachIndicator = eachRegardless.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
				string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized

				if (oneParameterAllValuesAveraged != null) {
					if (oneParameterAllValuesAveraged.ParameterName == parameterName) {
						continue;			// "ExceptFor"_WORKS_IF_CACHED_WAS_RESET
						//foundInUnchosen = true;
						//break;
					}
				}
				if (this.ValuesUnchosenByParameter.ContainsKey(parameterName) == false) continue;	// all param values are chosen

				List<double> unchosenValues = this.ValuesUnchosenByParameter[parameterName];
				double backtestedValue = eachIndicator.ValueCurrent;

				if (unchosenValues.Contains(backtestedValue)) {
					foundInUnchosen = true;
					break;
				}
			}
			return foundInUnchosen;
		}

		List<SystemPerformanceRestoreAble> sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		List<SystemPerformanceRestoreAble> SequencedBacktestsOriginalMinusParameterValuesUnchosen { get { lock(this.lockForAllRecalculations) {
			if (this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached != null)
				return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = new List<SystemPerformanceRestoreAble>();
			this.valuesUnchosenByParameter_cached = null;

			foreach (SystemPerformanceRestoreAble eachBacktest in this.sequencedBacktestsOriginal) {
				bool foundInUnchosen = this.HasUnchosenParametersExceptFor(eachBacktest);
				if (foundInUnchosen) continue;
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached.Add(eachBacktest);
			}

			return  this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		} } }

		
		Optimizer2() {
			ParametersByName = new Dictionary<string, OneParameterAllValuesAveraged>();
			valuesUnchosenByParameter_cached = new Dictionary<string, List<double>>();
			lockForAllRecalculations = new object();
		}
		public Optimizer2(List<SystemPerformanceRestoreAble> optimizationResults) : this() {
			this.sequencedBacktestsOriginal = optimizationResults;
			this.Initialize_classifyAndCalculateGlobals();
		}
		public void Initialize_classifyAndCalculateGlobals() { lock(this.lockForAllRecalculations) {
			int iterationCouter_fixBadDeserialization = 0;
			this.ParametersByName.Clear();
			foreach (SystemPerformanceRestoreAble eachRun in this.sequencedBacktestsOriginal) {
				if (eachRun.OptimizationIterationSerno == 0) eachRun.OptimizationIterationSerno = iterationCouter_fixBadDeserialization;
				iterationCouter_fixBadDeserialization++;

				foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double backtestedValue = eachIndicator.ValueCurrent;

					if (this.ParametersByName.ContainsKey(parameterName) == false) {
						this.ParametersByName.Add(parameterName, new OneParameterAllValuesAveraged(this, parameterName, eachRun.PriceFormat));
					}
					OneParameterAllValuesAveraged relatedParameter = this.ParametersByName[parameterName];
					relatedParameter.AddBacktestForValue_KPIsGlobalAddForIndicatorValue(backtestedValue, eachRun);
				}
			}
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.KPIsGlobalNoMoreParameters_DivideTotalsByCount();
			}
		} }

		internal void OneParameterOneValueUserSelectedChanged_recalculateAllKPIsLocal(OneParameterOneValue oneParameterOneValueUserSelectedChanged) {
			this.CalculateLocalsAndDeltas();
		}

		internal void ChoseThisOneResetOthers_RecalculateAllKPIsLocalAndDelta(OneParameterOneValue onlyOneParamValueClicked) {
			foreach (OneParameterOneValue eachValue in this.ParametersByName[onlyOneParamValueClicked.ParameterName].ValuesByParam.Values) {
				eachValue.Chosen = (eachValue == onlyOneParamValueClicked) ? true : false;
			}
			this.CalculateLocalsAndDeltas();
		}

		internal void CalculateLocalsAndDeltas() { lock(this.lockForAllRecalculations) {
			this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;

			foreach (OneParameterAllValuesAveraged eachParam in this.ParametersByName.Values) {
				eachParam.CalculateLocalsAndDeltas();
			}

			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.RaiseOnEachParameterRecalculatedLocalsAndDeltas();
			}
			this.RaiseOptimizedBacktestsListIsRebuiltWithoutUnchosenParameters();
		} }
	}
}
