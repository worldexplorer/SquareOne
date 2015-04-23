using System.IO;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public partial class Correlator {
				object												lockForAllRecalculations;
				List<SystemPerformanceRestoreAble>					sequencedBacktestsOriginal;
				RepositoryJsonCorrelator							repositoryJsonCorrelator;

				List<OneParameterAllValuesAveraged>					parametersByNameChosenDeserialized;

		public	AvgCorMomentumsCalculator							AvgMomentumsCalculator	{ get; private set; }

		public	Dictionary<string, OneParameterAllValuesAveraged>	ParametersByName		{ get; private set; }
		public	List<OneParameterAllValuesAveraged>					Parameters				{ get { return new List<OneParameterAllValuesAveraged>(this.ParametersByName.Values); } }

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


				bool hasUnchosenParametersExceptFor(SystemPerformanceRestoreAble eachRegardless) {
			return this.HasUnchosenParametersExceptFor(eachRegardless, null);
		}
		public	bool HasUnchosenParametersExceptFor(SystemPerformanceRestoreAble eachRegardless, OneParameterAllValuesAveraged oneParameterAllValuesAveraged) {
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

		List<SystemPerformanceRestoreAble>					sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		Dictionary<string, OneParameterAllValuesAveraged>	keepDeserializedChosen;
		List<SystemPerformanceRestoreAble>					SequencedBacktestsOriginalMinusParameterValuesUnchosen { get { lock(this.lockForAllRecalculations) {
			if (this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached != null)
				return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = new List<SystemPerformanceRestoreAble>();
			this.valuesUnchosenByParameter_cached = null;

			foreach (SystemPerformanceRestoreAble eachBacktest in this.sequencedBacktestsOriginal) {
				bool foundInUnchosen = this.hasUnchosenParametersExceptFor(eachBacktest);
				if (foundInUnchosen) continue;
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached.Add(eachBacktest);
			}

			return  this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		} } }

		
		Correlator() {
			ParametersByName = new Dictionary<string, OneParameterAllValuesAveraged>();
			valuesUnchosenByParameter_cached = new Dictionary<string, List<double>>();
			lockForAllRecalculations = new object();
			repositoryJsonCorrelator = new RepositoryJsonCorrelator();
			AvgMomentumsCalculator = new AvgCorMomentumsCalculator(this);
		}
		public Correlator(List<SystemPerformanceRestoreAble> optimizationResults
				, string relPathAndNameForSequencerResults, string fileName) : this() {
			this.sequencedBacktestsOriginal = optimizationResults;

			this.repositoryJsonCorrelator.Initialize(Assembler.InstanceInitialized.AppDataPath
				, Path.Combine("Correlator", relPathAndNameForSequencerResults), fileName);

			this.keepDeserializedChosen = this.repositoryJsonCorrelator.DeserializeSortedDictionary();

			this.initialize_classifyAndCalculateGlobals_runMomentumCalculator();
		}

		void initialize_classifyAndCalculateGlobals_runMomentumCalculator() { lock(this.lockForAllRecalculations) {
			int chosenWereFound_runCalculateLocalAndDeltas = 0;

			int iterationCouter_fixBadDeserialization = 0;
			Dictionary<string, OneParameterAllValuesAveraged> rebuilt = new Dictionary<string, OneParameterAllValuesAveraged>();

			foreach (SystemPerformanceRestoreAble eachRun in this.sequencedBacktestsOriginal) {
				if (eachRun.OptimizationIterationSerno == 0) eachRun.OptimizationIterationSerno = iterationCouter_fixBadDeserialization;
				iterationCouter_fixBadDeserialization++;

				//v1
				//foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
				//    IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
				//    string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
				//    double backtestedValue = eachIndicator.ValueCurrent;

				//    if (this.ParametersByName.ContainsKey(parameterName) == false) {
				//        this.ParametersByName.Add(parameterName, new OneParameterAllValuesAveraged(this, parameterName));
				//    }
				//    OneParameterAllValuesAveraged relatedParameter = this.ParametersByName[parameterName];
				//    relatedParameter.AddBacktestForValue_KPIsGlobalAddForIndicatorValue(backtestedValue, eachRun);
				//}

				//v2
				//foreach (string indicatorDotParameter in	eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
				//    IndicatorParameter eachIndicator =		eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
				//    string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
				//    double backtestedValue = eachIndicator.ValueCurrent;

				//    if (rebuilt.ContainsKey(parameterName) == false) {
				//        rebuilt.Add(parameterName, new OneParameterAllValuesAveraged(this, parameterName));
				//    }
				//    OneParameterAllValuesAveraged eachParameterRebuilt = rebuilt[parameterName];

				//    bool chosen = true;
				//    if (this.ParametersByName.ContainsKey(parameterName)) {
				//        var allValues = this.ParametersByName[parameterName];
				//        if (allValues.ValuesByParam.ContainsKey(backtestedValue)) {
				//            chosen = allValues.ValuesByParam[backtestedValue].Chosen;
				//            chosenWereFound_runCalculateLocalAndDeltas++;
				//        }
				//    }

				//    eachParameterRebuilt.AddBacktestForValue_KPIsGlobalAddForIndicatorValue(backtestedValue, chosen, eachRun);
				//}

				//v3
				foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double backtestedValue = eachIndicator.ValueCurrent;

					if (rebuilt.ContainsKey(parameterName) == false) {
						rebuilt.Add(parameterName, new OneParameterAllValuesAveraged(this, parameterName));
					}
					OneParameterAllValuesAveraged eachParameterRebuilt = rebuilt[parameterName];
					eachParameterRebuilt.AddBacktestForValue_KPIsGlobalAddForIndicatorValue(backtestedValue, eachRun);
				}

			}
			this.ParametersByName = rebuilt;
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.KPIsGlobalNoMoreParameters_DivideTotalsByCount();
			}
			this.CalculateLocalsAndDeltas(false);

			this.AvgMomentumsCalculator.Initialize_runEachValueAgainstAllParametersFullyChosen();

			//v3
			//this.ParametersByName = keepDeserializedChosen;
			chosenWereFound_runCalculateLocalAndDeltas = this.absorbChosenFlagFromDeserialized();

			// LAZY_DO_CLEAN_UP_FROM_EXCEPTIONS
			//List<string> keysWithOneValue = new List<string>();
			//foreach (string indicatorParameterName in rebuilt.Keys) {
			//    OneParameterAllValuesAveraged eachParameter = this.ParametersByName[indicatorParameterName];
			//    if (eachParameter.ValuesByParam.Count > 1) continue;
			//    keysWithOneValue.Add(indicatorParameterName);
			//}
			//foreach (string indicatorParameterName in keysWithOneValue) {
			//    rebuilt.Remove(indicatorParameterName);
			//}

			//foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
			//	eachParameter.KPIsGlobalNoMoreParameters_DivideTotalsByCount();
			//}
			if (chosenWereFound_runCalculateLocalAndDeltas > 0) {
				this.CalculateLocalsAndDeltas();
			}

		} }

		int absorbChosenFlagFromDeserialized() {
			int chosenWereFound_runCalculateLocalAndDeltas = 0;
			foreach (OneParameterAllValuesAveraged paramRebuilt in this.ParametersByName.Values) {
				OneParameterAllValuesAveraged paramDeserialized = this.keepDeserializedChosen.ContainsKey(paramRebuilt.ParameterName)
					? this.keepDeserializedChosen[paramRebuilt.ParameterName]
					: null;
				if (paramDeserialized == null || paramDeserialized.ValuesByParam == null) {
					string msg = "WHOS_THE_MOLE_HERE? AVOIDING_NPE";
					Assembler.PopupException(msg);
				}
				foreach (OneParameterOneValue eachValue in paramRebuilt.ValuesByParam.Values) {
					bool chosen = true;
					if (paramDeserialized.ValuesByParam.ContainsKey(eachValue.ValueSequenced)) {
						chosen = paramDeserialized.ValuesByParam[eachValue.ValueSequenced].Chosen;
					}
					if (eachValue.Chosen != chosen) {
						eachValue.Chosen  = chosen;
						chosenWereFound_runCalculateLocalAndDeltas++;
					}
				}
			}
			return chosenWereFound_runCalculateLocalAndDeltas;
		}

		public void OneParameterOneValueUserSelectedChanged_recalculateAllKPIsLocal(OneParameterOneValue oneParameterOneValueUserSelectedChanged) {
			this.CalculateLocalsAndDeltas();
		}

		public void ChooseThisOneResetOthers_RecalculateAllKPIsLocalAndDelta(OneParameterOneValue onlyOneParamValueClicked) {
			var paramClickedFromItsValue = this.ParametersByName[onlyOneParamValueClicked.ParameterName];
			foreach (OneParameterOneValue eachValue in paramClickedFromItsValue.ValuesByParam.Values) {
				bool willSetTo = (eachValue == onlyOneParamValueClicked) ? true : false;
				if (eachValue.Chosen != willSetTo) {
					eachValue.Chosen  = willSetTo;		// breakpoint to confirm on one click just one gets selected and just one gets deselected
				}
			}
			this.CalculateLocalsAndDeltas();
		}

		public void CalculateLocalsAndDeltas(bool raiseAllEvents = true, bool isUserClick_Serialize = true) { lock(this.lockForAllRecalculations) {
			if (isUserClick_Serialize) this.repositoryJsonCorrelator.SerializeSortedDictionary(this.ParametersByName);

			this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;

			foreach (OneParameterAllValuesAveraged eachParam in this.ParametersByName.Values) {
				if (eachParam.ValuesByParam.Count <= 1) continue;
				eachParam.CalculateLocalsAndDeltas_forEachValue_and3artificials();
			}

			if (raiseAllEvents == false) return;

			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.RaiseOnEachParameterRecalculatedLocalsAndDeltas();
			}
			this.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
		} }

		public void InvalidateBactestsMinusUnchosen() {
			this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;
		}
	}
}
