using System;
using System.IO;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Core.Serializers;

namespace Sq1.Core.Correlation {
	public partial class Correlator {
					object											lockForAllRecalculations;
		internal	SequencedBacktests								SequencedBacktestOriginal;
					RepositoryJsonCorrelator						repositoryJsonCorrelator;

		public		KPIsCalculator										KPIsCalculator		{ get; private set; }
		public		MomentumsCalculator									MomentumsCalculator	{ get; private set; }

		public		Dictionary<string, OneParameterAllValuesAveraged>	ParametersByName		{ get; private set; }
		public		List<OneParameterAllValuesAveraged>					Parameters				{ get { return new List<OneParameterAllValuesAveraged>(this.ParametersByName.Values); } }

					Dictionary<string, List<double>> valuesUnchosenByParameter_cached;
					Dictionary<string, List<double>> ValuesUnchosenByParameter { get { lock(this.lockForAllRecalculations) {
			if (this.valuesUnchosenByParameter_cached != null) return this.valuesUnchosenByParameter_cached;
			this.valuesUnchosenByParameter_cached = new Dictionary<string,List<double>>();

			foreach (OneParameterAllValuesAveraged param in this.ParametersByName.Values) {
				foreach (OneParameterOneValue val in param.OneParamOneValueByValues.Values) {
					if (val.Chosen == true) continue;
					if (this.valuesUnchosenByParameter_cached.ContainsKey(param.ParameterName) == false) {
						this.valuesUnchosenByParameter_cached.Add(param.ParameterName, new List<double>());
					}
					this.valuesUnchosenByParameter_cached[param.ParameterName].Add(val.ValueSequenced);
				}
			}

			return this.valuesUnchosenByParameter_cached;
		} } }

		internal	Dictionary<string, OneParameterAllValuesAveraged>	KeepDeserializedChosen;

					SequencedBacktests									sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		public		SequencedBacktests									SequencedBacktestsOriginalMinusParameterValuesUnchosen { get { lock(this.lockForAllRecalculations) {
			if (this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached != null)
				return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = new SequencedBacktests();

			// SubsetAsString="[0..75%]" is forwared to the SequencerControl.txtDataRange.Text
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached
				.SubsetPercentageSetInvalidate(this.SequencedBacktestOriginal.SubsetPercentage);
			this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached
				.SubsetPercentageFromEndSetInvalidate(this.SequencedBacktestOriginal.SubsetPercentageFromEnd);

			this.valuesUnchosenByParameter_cached = null;

			if (this.SequencedBacktestOriginal == null) {
				string msg = "INITIALIZE_WASNT_INVOKED this.sequencedBacktestOriginal=null";
				Assembler.PopupException(msg);
			}
			//foreach (SystemPerformanceRestoreAble eachBacktest in this.sequencedBacktestsOriginal.Backtests) {
			foreach (SystemPerformanceRestoreAble eachBacktest in this.SequencedBacktestOriginal.Subset) {
				bool foundInUnchosen = this.HasUnchosenParametersExceptFor(eachBacktest, null);
				if (foundInUnchosen) continue;
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached.Add(eachBacktest);
			}
			return this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached;
		} } }
		public		ScriptExecutor	Executor	{ get; private set; }

		public		Dictionary<string, CorrelatorOneParameterSnapshot>				CorrelatorDataSnapshot				{ get; private set; }
		public		Serializer<Dictionary<string, CorrelatorOneParameterSnapshot>>	CorrelatorDataSnapshotSerializer	{ get; private set; }
		
		Correlator() {
			ParametersByName					= new Dictionary<string, OneParameterAllValuesAveraged>();
			//MUST_BE_NULL__HANDLED_INSIDE_ValuesUnchosenByParameter.get valuesUnchosenByParameter_cached = new Dictionary<string, List<double>>();
			lockForAllRecalculations			= new object();
			repositoryJsonCorrelator			= new RepositoryJsonCorrelator();
			MomentumsCalculator					= new MomentumsCalculator(this);
			KPIsCalculator						= new KPIsCalculator(this);
			CorrelatorDataSnapshot				= new Dictionary<string, CorrelatorOneParameterSnapshot>();
			CorrelatorDataSnapshotSerializer	= new Serializer<Dictionary<string, CorrelatorOneParameterSnapshot>>();
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
			if (this.SequencedBacktestOriginal != null && this.SequencedBacktestOriginal.ToString() == sequencedBacktests.ToString()) {
				string msg = "YOU_ALREADY_INVOKED_ME_EARLIER_WITH_SAME_SEQUENCED_HISTORY";
				Assembler.PopupException(msg, null, false);
				//return;
			}
			this.SequencedBacktestOriginal = sequencedBacktests;

			this.repositoryJsonCorrelator.Initialize(Assembler.InstanceInitialized.AppDataPath
				, Path.Combine("Correlator", relPathAndNameForCorrelatorResults), fileName);

			this.KeepDeserializedChosen = this.repositoryJsonCorrelator.DeserializeSortedDictionary();


			// C:\SquareOne\Data-debug\Correlator\Sq1.Strategies.Demo.dll\TwoMAsCompiled.json
			this.CorrelatorDataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath, this.Executor.Strategy.StoredInJsonRelName
				, "Correlator"
				, this.Executor.Strategy.StoredInFolderRelName
				);
			this.CorrelatorDataSnapshot = this.CorrelatorDataSnapshotSerializer.Deserialize();

			//INVOKED_DOWNSTACK this.InvalidateBacktestsMinusUnchosen();
			//INVOKED_DOWNSTACK this.AvgMomentumsCalculator.reset();
			//FIXES_ZEROES_AFTER_SECOND_CLICK_BUT_MOVED_TO_AvgMomentumsCalculator.reset() this.AvgMomentumsCalculator = new AvgCorMomentumsCalculator(this);
			this.rebuildParametersByName();
			this.calculateGlobalsAndLocals();
		}

		void calculateGlobalsAndLocals() {
			this.MomentumsCalculator.CalculateGlobals_runEachValueAgainstAllParametersFullyChosen_restoreChosenFromDeserialized();

			this.KPIsCalculator.calculateGlobals();
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.KPIsGlobalNoMoreParameters_CalculateGlobalsAndCloneToLocals_step3of3();
			}
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				this.CalculateLocalsAndDeltas_RaiseAllEvents_Serialize(false, false);	// WHY??? raiseAllEvents=false, serialize=false
			} else {
				this.CalculateLocalsAndDeltas_RaiseAllEvents_Serialize(false);			// WHY??? raiseAllEvents=false, serialize=true
			}
		}

		void rebuildParametersByName() { lock(this.lockForAllRecalculations) { // DEADLOCK_OTHERWIZE_WHEN_CALCULATOR_RUN_IN_A_SEPARATE_TASK
			int iterationCouter_fixBadDeserialization = 0;
			Dictionary<string, OneParameterAllValuesAveraged> rebuilt = new Dictionary<string, OneParameterAllValuesAveraged>();

			foreach (SystemPerformanceRestoreAble eachRun in this.SequencedBacktestOriginal.BacktestsReadonly) {
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

		public void OneParameterOneValueUserSelectedChanged_recalculateLocalKPIsMomentums(OneParameterOneValue oneParameterOneValueUserSelectedChanged) {
			this.CalculateLocalsAndDeltas_RaiseAllEvents_Serialize();
		}

		public void ChooseThisOneResetOthers_RecalculateAllKPIsLocalAndDelta(OneParameterOneValue onlyOneParamValueClicked) {
			var paramClickedFromItsValue = this.ParametersByName[onlyOneParamValueClicked.ParameterName];
			foreach (OneParameterOneValue eachValue in paramClickedFromItsValue.OneParamOneValueByValues.Values) {
				bool willSetTo = (eachValue == onlyOneParamValueClicked) ? true : false;
				if (eachValue.Chosen != willSetTo) {
					eachValue.Chosen  = willSetTo;		// breakpoint to confirm on one click just one gets selected and just one gets deselected
				}
			}
			this.CalculateLocalsAndDeltas_RaiseAllEvents_Serialize();
		}

		internal void CalculateLocalsAndDeltas_RaiseAllEvents_Serialize(bool raiseAllEvents = true, bool isUserClick_Serialize = true) {
			if (isUserClick_Serialize) this.repositoryJsonCorrelator.SerializeSortedDictionary(this.ParametersByName);
			lock (this.lockForAllRecalculations) {
				this.valuesUnchosenByParameter_cached = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;

				this.KPIsCalculator.CalculateLocalsAndDeltas();
				this.MomentumsCalculator.CalculateLocalsAndDeltas();
			}
			if (raiseAllEvents == false) return;
			foreach (OneParameterAllValuesAveraged eachParameter in this.ParametersByName.Values) {
				eachParameter.RaiseOnEachParameterRecalculatedLocalsAndDeltas();
			}
			this.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
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

		public void InvalidateBacktestsMinusUnchosen() {
			if (this.valuesUnchosenByParameter_cached != null) {
				this.valuesUnchosenByParameter_cached  = null;	//"ExceptFor"_WORKS_IF_CACHED_WAS_RESET
			}
			if (this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached != null) {
				this.sequencedBacktestsOriginalMinusParameterValuesUnchosen_cached = null;
			}
		}
	}
}
