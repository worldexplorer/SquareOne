using System;
using System.Collections.Generic;

using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public class MomentumsCalculator {
				Correlator										correlator;

		public MomentumsCalculator(Correlator correlator) {
			this.correlator = correlator;
		}
		public void CalculateGlobals_runEachValueAgainstAllParametersFullyChosen_restoreChosenFromDeserialized() {
			this.reset();

			int momentumsDumped = 0;
			foreach (OneParameterAllValuesAveraged varyingThisWhileOthersFullyChosen in this.correlator.ParametersByName.Values) {
				if (varyingThisWhileOthersFullyChosen.OneParamOneValueByValues.Count <= 1) continue;
				this.chooseAllOthersFullOnExcept(varyingThisWhileOthersFullyChosen);

				foreach (double oneValueChosenOthersNot in varyingThisWhileOthersFullyChosen.OneParamOneValueByValues.Keys) {
					OneParameterOneValue thisChosenOthersNot = varyingThisWhileOthersFullyChosen.OneParamOneValueByValues[oneValueChosenOthersNot];
					this.correlator.InvalidateBacktestsMinusUnchosen();
					varyingThisWhileOthersFullyChosen.chooseThisUnchooseOthers(thisChosenOthersNot);

					//SortedDictionary<int, SystemPerformanceRestoreAble> chosenOnly = thisChosenOthersNot.BacktestsWithMyValueAndOnlyChosenOtherValues;
					List<SystemPerformanceRestoreAble> chosenOnly_subset = this.correlator.SequencedBacktestsOriginalMinusParameterValuesUnchosen.Subset;

					#if DEBUG		// inline test
					System.Collections.ObjectModel.ReadOnlyCollection<SystemPerformanceRestoreAble> chosen = this.correlator.SequencedBacktestsOriginalMinusParameterValuesUnchosen.BacktestsReadonly;
					System.Collections.ObjectModel.ReadOnlyCollection<SystemPerformanceRestoreAble> all = this.correlator.SequencedBacktestOriginal.BacktestsReadonly;
					List<SystemPerformanceRestoreAble> all_subset = this.correlator.SequencedBacktestOriginal.Subset;
					if (thisChosenOthersNot.ParameterNameValue == "MAslow.Period=20") {
						string msg = "Here I have an Excel with manually calculated Stdev(DD)";
						if (chosenOnly_subset[0].MaxDrawDown != chosen[0].MaxDrawDown) {
							string msg2 = "CHOSE_100%_SO_THAT_I_CAN_VERIFY_ALL_VA_SUBSET";
						}
					}
					#endif

					thisChosenOthersNot.KPIsMomentumDispersionGlobal.Reset_addBacktests_getMyMembersReady(chosenOnly_subset);

					momentumsDumped++;

					#if DEBUG		// inline test
					if (thisChosenOthersNot.KPIsMomentumAverage.NetProfit != 0) {
						string msg = "MUST_NOT_HAVE_BEEN_FILLED_EARLIER";
						Assembler.PopupException(msg);
					}
					#endif
				}
			}

			int chosenWereFound_runCalculateLocalAndDeltas = this.absorbChosenFlagFromDeserialized_restoreAfterAllWereChosenForGlobals();
			if (chosenWereFound_runCalculateLocalAndDeltas > 0) {
				//this.CalculateLocalsAndDeltas_RaiseAllEvents_Serialize(true, false);	// WHY??? raiseAllEvents=true, serialize=false
				string msg = "WILL_DO_UPSTACK CalculateLocalsAndDeltas_RaiseAllEvents_Serialize()";
				Assembler.PopupException(msg, null, false);
			} else {
				string msg = "SO_WHAT_DIDNT_CHANGE_AFTER_this.AvgMomentumsCalculator.Initialize_runEachValueAgainstAllParametersFullyChosen() ???";
				Assembler.PopupException(msg, null, false);
			}


			#if DEBUG		// inline test
			if (this.correlator.Parameters.Count <= 0) {
				string msg = "FOR_THE_CHART_OPENED_CORRELATOR_DOESNT_HAVE_ANY_BACKTESTS " + this.correlator.Executor.ChartShadow;
				msg += this.correlator.Executor.Strategy.ScriptContextCurrent.SymbolScaleIntervalDataRangeForScriptContextNewName;
				Assembler.PopupException(msg, null, false);
				return;
			}

			var paramVarying	= this.correlator.Parameters[0];

			if (paramVarying.OneParamOneValues.Count <= 1) {
				string msg = "DONT_INVOKE_ME_FOR_A_NON_VARYING_PARAMETER";
				// UNIMPORTANT Assembler.PopupException(msg, null, false);
				return;
			}

			var valueFirst	= paramVarying.OneParamOneValues[0];
			var valueSecond	= paramVarying.OneParamOneValues[1];

			if (valueFirst.KPIsMomentumAverage.PositionsCount == valueSecond.KPIsMomentumAverage.PositionsCount) {
				if (valueFirst.KPIsMomentumAverage.PositionsCount == 0) {
					string msg1 = "FIXED_BY_OneParameterAllValuesAveraged.ClearBacktestsForAllMyValue_step1of3() YOU_FORGOT_TO_RESET_SOMETHING_IN_CORRELATOR__SECOND_CLICK_ON_SEQUENCER_RESULTS";
					Assembler.PopupException(msg1, null, false);
					return;
				}
				string msg = "TRYING_TO_CATCH_WHERE_MOMENTUMS_BECOME_EQUAL_WHILE_THEY_MUSTNT_BE";
				Assembler.PopupException(msg, null, false);
			}
			#endif
		}

		internal void CalculateLocalsAndDeltas() {
			this.reset();
			foreach (OneParameterAllValuesAveraged eachParam in this.correlator.ParametersByName.Values) {
				foreach (OneParameterOneValue oneParamOneValue in eachParam.OneParamOneValueByValues.Values) {
					oneParamOneValue.KPIsMomentumDispersionLocal.Reset_addBacktests_getMyMembersReady(oneParamOneValue.BacktestsWithMyValueAndOnlyChosenOtherValues);
				}
			}
		}
		void chooseAllOthersFullOnExcept(OneParameterAllValuesAveraged varyingThisWhileOthersFullyChosen) {
			foreach (OneParameterAllValuesAveraged eachParam in this.correlator.ParametersByName.Values) {
				if (eachParam == varyingThisWhileOthersFullyChosen) continue;
				eachParam.chooseAllValues();
			}
		}
		int absorbChosenFlagFromDeserialized_restoreAfterAllWereChosenForGlobals() {
			int chosenWereFound_runCalculateLocalAndDeltas = 0;
			foreach (OneParameterAllValuesAveraged paramRebuilt in this.correlator.ParametersByName.Values) {
				OneParameterAllValuesAveraged paramDeserialized = this.correlator.KeepDeserializedChosen.ContainsKey(paramRebuilt.ParameterName)
					? this.correlator.KeepDeserializedChosen[paramRebuilt.ParameterName]
					: null;
				if (paramDeserialized == null) {
					string msg = "I_REFUSE_TO_ABSORB_CHOSEN__DESERIALIZED_DOESNT_CONTAIN paramRebuilt[" + paramRebuilt.ParameterName + "]";
					Assembler.PopupException(msg);
					continue;
				}
				if (paramDeserialized.OneParamOneValueByValues == null) {
					string msg = "I_REFUSE_TO_ABSORB_NO_VALUES_FOR___DESERIALIZED [" + paramDeserialized + "] paramRebuilt[" + paramRebuilt.ParameterName + "]";
					Assembler.PopupException(msg);
					continue;
				}
				foreach (OneParameterOneValue eachValue in paramRebuilt.OneParamOneValueByValues.Values) {
					bool chosen = true;
					if (paramDeserialized.OneParamOneValueByValues.ContainsKey(eachValue.ValueSequenced)) {
						chosen = paramDeserialized.OneParamOneValueByValues[eachValue.ValueSequenced].Chosen;
					}
					if (eachValue.Chosen != chosen) {
						eachValue.Chosen = chosen;
						chosenWereFound_runCalculateLocalAndDeltas++;
					}
				}
			}
			return chosenWereFound_runCalculateLocalAndDeltas;
		}

		void reset() {
			if (this.correlator.ParametersByName.Count == 0) {
				string msg = "NOTHING_TO_RESET ParametersByName.Count=0 //AvgCorMomentumsCalculator.reset()";
				Assembler.PopupException(msg, null, false);
			}
			foreach (OneParameterAllValuesAveraged eachParameter in this.correlator.ParametersByName.Values) {
				if (eachParameter.OneParamOneValueByValues.Count <= 1) continue;
				foreach (double eachParameterEachValue in eachParameter.OneParamOneValueByValues.Keys) {
					OneParameterOneValue eachValue = eachParameter.OneParamOneValueByValues[eachParameterEachValue];
					eachValue.KPIsMomentumAverage			.Reset();
					eachValue.KPIsMomentumDispersionGlobal	.Reset();
					eachValue.KPIsMomentumDispersionLocal	.Reset();
				}
			}
		}
	}
}
