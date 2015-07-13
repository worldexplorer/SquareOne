using System;
using System.IO;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Correlation {
	public partial class Correlator {
				object												lockForAllRecalculations;
				SequencedBacktests									sequencedBacktestOriginal;
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

		public	double				SubsetPercentage		{ get { return this.sequencedBacktestOriginal.SubsetPercentage; } }
		public	bool				SubsetPercentageFromEnd { get { return this.sequencedBacktestOriginal.SubsetPercentageFromEnd; } }
		public	DateTime			SubsetWaterLineDateTime { get { return this.sequencedBacktestOriginal.SubsetWaterLineDateTime; } }

		public void SubsetPercentagePropagate(double subsetPercentage) {
			this.sequencedBacktestOriginal.SubsetPercentageSetInvalidate(subsetPercentage);
			//this.repositoryJsonCorrelator.SerializeSingle(this.sequencedBacktestOriginal);		// OVERHEAD_RESAVING_FEW_MEGABYTES_FOR_ONE_BOOLEAN_CHANGED
			this.InvalidateBacktestsMinusUnchosen();
			this.calculateGlobals_runMomentumCalculator();	//rebuildParametersByName() invoked only once per lifetime, koz I subscribe to event late (3 steps omitted here but you'll fig)
		}
		public void SubsetPercentageFromEndPropagate(bool subsetPercentageFromEnd) {
			this.sequencedBacktestOriginal.SubsetPercentageFromEndSetInvalidate(subsetPercentageFromEnd);
			//this.repositoryJsonCorrelator.SerializeSingle(this.sequencedBacktestOriginal);		// OVERHEAD_RESAVING_FEW_MEGABYTES_FOR_ONE_BOOLEAN_CHANGED
			this.InvalidateBacktestsMinusUnchosen();
			this.calculateGlobals_runMomentumCalculator();	//rebuildParametersByName() invoked only once per lifetime, koz I subscribe to event late (3 steps omitted here but you'll fig)		}
		}

		private SequencedBacktests									sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		private	Dictionary<string, OneParameterAllValuesAveraged>	keepDeserializedChosen;
		public	SequencedBacktests									SequencedBacktestsOriginalMinusParameterValuesUnchosen { get { lock(this.lockForAllRecalculations) {
			if (this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached != null)
				return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = new SequencedBacktests();

			// SubsetAsString="[0..75%]" is forwared to the SequencerControl.txtDataRange.Text
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached
				.SubsetPercentageSetInvalidate(this.sequencedBacktestOriginal.SubsetPercentage);
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached
				.SubsetPercentageFromEndSetInvalidate(this.sequencedBacktestOriginal.SubsetPercentageFromEnd);

			this.valuesUnchosenByParameter_cached = null;

			if (this.sequencedBacktestOriginal == null) {
				string msg = "INITIALIZE_WASNT_INVOKED this.sequencedBacktestOriginal=null";
				Assembler.PopupException(msg);
			}
			//foreach (SystemPerformanceRestoreAble eachBacktest in this.sequencedBacktestsOriginal.Backtests) {
			foreach (SystemPerformanceRestoreAble eachBacktest in this.sequencedBacktestOriginal.Subset) {
				bool foundInUnchosen = this.hasUnchosenParametersExceptFor(eachBacktest);
				if (foundInUnchosen) continue;
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached.Add(eachBacktest);
			}
			return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		} } }
		public ScriptExecutor	Executor	{ get; private set; } 

		
		Correlator() {
			ParametersByName = new Dictionary<string, OneParameterAllValuesAveraged>();
			//MUST_BE_NULL__HANDLED_INSIDE_ValuesUnchosenByParameter.get valuesUnchosenByParameter_cached = new Dictionary<string, List<double>>();
			lockForAllRecalculations = new object();
			repositoryJsonCorrelator = new RepositoryJsonCorrelator();
			AvgMomentumsCalculator = new AvgCorMomentumsCalculator(this);
		}
		public Correlator(ScriptExecutor scriptExecutor) : this() {
			this.Executor = scriptExecutor;
		}

		public void Initialize(SequencedBacktests sequencedBacktests, string relPathAndNameForCorrelatorResults, string fileName) {
			if (sequencedBacktests == null) {
				string msg = "DONT_PASS_NULL_originalSequencedBacktests";
				Assembler.PopupException(msg);
				return;
			}
			if (this.sequencedBacktestOriginal != null && this.sequencedBacktestOriginal.ToString() == sequencedBacktests.ToString()) {
				string msg = "YOU_ALREADY_INVOKED_ME_EARLIER_WITH_SAME_SEQUENCED_HISTORY";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.sequencedBacktestOriginal = sequencedBacktests;

			this.repositoryJsonCorrelator.Initialize(Assembler.InstanceInitialized.AppDataPath
				, Path.Combine("Correlator", relPathAndNameForCorrelatorResults), fileName);

			this.keepDeserializedChosen = this.repositoryJsonCorrelator.DeserializeSortedDictionary();

			//INVOKED_DOWNSTACK this.InvalidateBacktestsMinusUnchosen();
			//INVOKED_DOWNSTACK this.AvgMomentumsCalculator.reset();
			//FIXES_ZEROES_AFTER_SECOND_CLICK_BUT_MOVED_TO_AvgMomentumsCalculator.reset() this.AvgMomentumsCalculator = new AvgCorMomentumsCalculator(this);

			this.rebuildParametersByName();
			this.calculateGlobals_runMomentumCalculator();
		}

		void calculateGlobals_runMomentumCalculator() {
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.ClearBacktestsForAllMyValue_step1of3();
			}
			// not creating new OneParameterAllValuesAveraged() aims to refreshOlv and UseWaitCursor=false;
			// looks stupid, copypaste from rebuildParametersByName() AFFRAID_OF_LOSING_OnParameterRecalculatedLocalsAndDeltas_EVENT
			foreach (SystemPerformanceRestoreAble eachRun in this.sequencedBacktestOriginal.Subset) {
				foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double backtestedValue = eachIndicator.ValueCurrent;

					if (this.ParametersByName.ContainsKey(parameterName) == false) {
						string msg = "I_REFUSE_TO_CREATE_NEW_AFFRAID_OF_LOSING_OnParameterRecalculatedLocalsAndDeltas_EVENT"
							+ " RETHINK_rebuildParametersByName() ParametersByName.ContainsKey(" + parameterName + ")=false";
						Assembler.PopupException(msg);
						continue;
					}
					OneParameterAllValuesAveraged eachParameterRebuilt = this.ParametersByName[parameterName];
					//InvalidateBacktestsMinusUnchosen() invoked upstack already
					eachParameterRebuilt.AddBacktestForValue_KPIsGlobalAddForIndicatorValue_step2of3(backtestedValue, eachRun);
				}
			}

			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.KPIsGlobalNoMoreParameters_DivideTotalsByCount_step3of3();
			}

			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				this.calculateLocalsAndDeltas(false, false);	// WHY??? raiseAllEvents=false, serialize=false
			} else {
				this.calculateLocalsAndDeltas(false);			// WHY??? raiseAllEvents=false, serialize=true
			}

			this.AvgMomentumsCalculator.Initialize_runEachValueAgainstAllParametersFullyChosen();

			int chosenWereFound_runCalculateLocalAndDeltas = this.absorbChosenFlagFromDeserialized();
			if (chosenWereFound_runCalculateLocalAndDeltas > 0) {
				this.calculateLocalsAndDeltas();
			} else {
				string msg = "SO_WHAT_DIDNT_CHANGE_AFTER_this.AvgMomentumsCalculator.Initialize_runEachValueAgainstAllParametersFullyChosen() ???";
				Assembler.PopupException(msg, null, false);
			}
		}

		void rebuildParametersByName() { lock(this.lockForAllRecalculations) { // DEADLOCK_OTHERWIZE_WHEN_CALCULATOR_RUN_IN_A_SEPARATE_TASK
			int iterationCouter_fixBadDeserialization = 0;
			Dictionary<string, OneParameterAllValuesAveraged> rebuilt = new Dictionary<string, OneParameterAllValuesAveraged>();

			foreach (SystemPerformanceRestoreAble eachRun in this.sequencedBacktestOriginal.BacktestsReadonly) {
			//foreach (SystemPerformanceRestoreAble eachRun in this.sequencedBacktestOriginal.Subset) {
				if (eachRun.SequenceIterationSerno == 0) eachRun.SequenceIterationSerno = iterationCouter_fixBadDeserialization;
				iterationCouter_fixBadDeserialization++;
				foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double backtestedValue = eachIndicator.ValueCurrent;

					if (rebuilt.ContainsKey(parameterName) == false) {
						rebuilt.Add(parameterName, new OneParameterAllValuesAveraged(this, parameterName));
					}
					OneParameterAllValuesAveraged eachParameterRebuilt = rebuilt[parameterName];
					eachParameterRebuilt.AddBacktestForValue_KPIsGlobalAddForIndicatorValue_step2of3(backtestedValue, eachRun);
				}

			}
			this.ParametersByName = rebuilt;
		} }

		int absorbChosenFlagFromDeserialized() {
			int chosenWereFound_runCalculateLocalAndDeltas = 0;
			foreach (OneParameterAllValuesAveraged paramRebuilt in this.ParametersByName.Values) {
				OneParameterAllValuesAveraged paramDeserialized = this.keepDeserializedChosen.ContainsKey(paramRebuilt.ParameterName)
					? this.keepDeserializedChosen[paramRebuilt.ParameterName]
					: null;
				if (paramDeserialized == null) {
					string msg = "I_REFUSE_TO_ABSORB_CHOSEN__DESERIALIZED_DOESNT_CONTAIN paramRebuilt[" + paramRebuilt.ParameterName + "]";
					Assembler.PopupException(msg);
					continue;
				}
				if (paramDeserialized.ValuesByParam == null) {
					string msg = "I_REFUSE_TO_ABSORB_NO_VALUES_FOR___DESERIALIZED [" + paramDeserialized + "] paramRebuilt[" + paramRebuilt.ParameterName + "]";
					Assembler.PopupException(msg);
					continue;
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
			this.calculateLocalsAndDeltas();
		}

		public void ChooseThisOneResetOthers_RecalculateAllKPIsLocalAndDelta(OneParameterOneValue onlyOneParamValueClicked) {
			var paramClickedFromItsValue = this.ParametersByName[onlyOneParamValueClicked.ParameterName];
			foreach (OneParameterOneValue eachValue in paramClickedFromItsValue.ValuesByParam.Values) {
				bool willSetTo = (eachValue == onlyOneParamValueClicked) ? true : false;
				if (eachValue.Chosen != willSetTo) {
					eachValue.Chosen  = willSetTo;		// breakpoint to confirm on one click just one gets selected and just one gets deselected
				}
			}
			this.calculateLocalsAndDeltas();
		}

		void calculateLocalsAndDeltas(bool raiseAllEvents = true, bool isUserClick_Serialize = true) {
			if (isUserClick_Serialize) this.repositoryJsonCorrelator.SerializeSortedDictionary(this.ParametersByName);
			lock (this.lockForAllRecalculations) {
				this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;

				foreach (OneParameterAllValuesAveraged eachParam in this.ParametersByName.Values) {
					if (eachParam.ValuesByParam.Count <= 1) continue;
					eachParam.CalculateLocalsAndDeltas_forEachValue_and3artificials();
				}
			}

			if (raiseAllEvents == false) return;
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.RaiseOnEachParameterRecalculatedLocalsAndDeltas();
			}
			this.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
		}

		public void InvalidateBacktestsMinusUnchosen() {
			this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;
		}
	}
}
