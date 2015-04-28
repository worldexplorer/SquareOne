using System;
using System.Collections.Generic;

namespace Sq1.Core.Correlation {
	public class AvgCorMomentumsCalculator {
		Correlator correlator;

		public	Dictionary<string, OneParameterAllValuesAveraged>	ParametersByName { get { return this.correlator.ParametersByName; } }

				Dictionary<string, OneParameterAllAvgCorMomentums>	momentumsAveragedByParameter_cached;
		public	Dictionary<string, OneParameterAllAvgCorMomentums>	MomentumsAveragedByParameter { get {
			if (this.momentumsAveragedByParameter_cached != null) return this.momentumsAveragedByParameter_cached;
			this.momentumsAveragedByParameter_cached = new Dictionary<string, OneParameterAllAvgCorMomentums>();
			foreach (OneParameterAllValuesAveraged param in this.ParametersByName.Values) {
				OneParameterAllAvgCorMomentums momentumsForAllValuesOneParam = new OneParameterAllAvgCorMomentums(param.ParameterName);
				foreach (OneParameterOneValue eachValue in param.ValuesByParam.Values) {
					AvgCorMomentums momForEachValue = new AvgCorMomentums(momentumsForAllValuesOneParam, eachValue);
					try {
						momentumsForAllValuesOneParam.MomentumsByValue.Add(eachValue.ValueSequenced, momForEachValue);
					} catch (Exception ex) {
						string msg = "CATCHING_KEY_ALREADY_ADDED";
						Assembler.PopupException(msg, ex);
					}
				}
				this.momentumsAveragedByParameter_cached.Add(param.ParameterName, momentumsForAllValuesOneParam);
			}
			return this.momentumsAveragedByParameter_cached;
		} }

		public AvgCorMomentumsCalculator(Correlator correlator) {
			this.correlator = correlator;
		}
		public void Initialize_runEachValueAgainstAllParametersFullyChosen() {
			this.reset();

			int momentumsDumped = 0;
			foreach (OneParameterAllValuesAveraged varyingThisWhileOthersFullyChosen in this.ParametersByName.Values) {
				if (varyingThisWhileOthersFullyChosen.ValuesByParam.Count <= 1) continue;
				OneParameterAllAvgCorMomentums momentumsForVariatedParameter = this.MomentumsAveragedByParameter[varyingThisWhileOthersFullyChosen.ParameterName];
				this.chooseAllOthersFullOnExcept(varyingThisWhileOthersFullyChosen);
				foreach (double oneValueChosenOthersNot in varyingThisWhileOthersFullyChosen.ValuesByParam.Keys) {
					OneParameterOneValue thisChosenOthersNot = varyingThisWhileOthersFullyChosen.ValuesByParam[oneValueChosenOthersNot];
					varyingThisWhileOthersFullyChosen.chooseThisUnchooseOthers(thisChosenOthersNot);

					AvgCorMomentums momentumForVariatedValue = momentumsForVariatedParameter.MomentumsByValue[oneValueChosenOthersNot];
					#if DEBUG		// inline test
					if (momentumForVariatedValue.KPIsAvgAverage.NetProfit != 0) {
						string msg = "MUST_NOT_HAVE_BEEN_FILLED_EARLIER";
						Assembler.PopupException(msg);
					}
					#endif
					momentumForVariatedValue.KPIsAvgAverage		.Reset();
					momentumForVariatedValue.KPIsAvgDispersion	.Reset();
					momentumForVariatedValue.KPIsAvgVariance	.Reset();

					int allParamsExceptForVarying = 0;
					foreach (string eachParamExceptForVaryingName in this.ParametersByName.Keys) {
						OneParameterAllValuesAveraged eachParamExceptForVarying = this.ParametersByName[eachParamExceptForVaryingName];
					
						if (eachParamExceptForVarying.ValuesByParam.Count <= 1) continue;
						if (eachParamExceptForVarying == varyingThisWhileOthersFullyChosen) continue;

						#if DEBUG		// inline test
						if (eachParamExceptForVarying.ChosenCount != eachParamExceptForVarying.ValuesByParam.Count) {
							string msg = "ALL_MUST_BE_CHOSEN ChosenCount[" + eachParamExceptForVarying.ChosenCount + "] MUST_BE_EQUAL_TO [" + eachParamExceptForVarying.ValuesByParam.Count + "]";
							Assembler.PopupException(msg);
						}
						#endif

						//v1 SAME_MOMENTUMS_ACROSS_ALL_VALUES
						this.correlator.InvalidateBacktestsMinusUnchosen();
						eachParamExceptForVarying.CalculateLocalsAndDeltas_forEachValue_and3artificials();
						//foreach (OneParameterAllValuesAveraged eachParam in this.ParametersByName.Values) {
						//    eachParam.CalculateLocalsAndDeltas();
						//}

						#if DEBUG		// inline test
						if (eachParamExceptForVarying.ArtificialRowAverage.KPIsDelta.NetProfit == 0) {
							string msg = "MUST_BE_CALCULATED_BY_NOW_ArtificialRowAverage.KPIsDelta.NetProfit";
							//Assembler.PopupException(msg);
						}
						#endif

						momentumForVariatedValue.KPIsAvgAverage		.AddKPIs(eachParamExceptForVarying.ArtificialRowAverage		.KPIsDelta);
						momentumForVariatedValue.KPIsAvgDispersion	.AddKPIs(eachParamExceptForVarying.ArtificialRowDispersion	.KPIsDelta);
						momentumForVariatedValue.KPIsAvgVariance	.AddKPIs(eachParamExceptForVarying.ArtificialRowVariance	.KPIsDelta);

						momentumsDumped++;
						allParamsExceptForVarying++;

						//break;
					}

					momentumForVariatedValue.KPIsAvgAverage		.DivideTotalByCount(allParamsExceptForVarying);
					momentumForVariatedValue.KPIsAvgDispersion	.DivideTotalByCount(allParamsExceptForVarying);
					momentumForVariatedValue.KPIsAvgVariance	.DivideTotalByCount(allParamsExceptForVarying);

					//this.correlator.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
					//System.Threading.Thread.Sleep(1000);
				}
			}

			#if DEBUG		// inline test
			if (this.correlator.Parameters.Count <= 0) {
				string msg = "DONT_INVOKE_ME_WITH_NO_PARAMETERS";
				Assembler.PopupException(msg);
			}

			var paramVarying	= this.correlator.Parameters[1];

			if (paramVarying.Values.Count <= 1) {
				string msg = "DONT_INVOKE_ME_FOR_A_NON_VARYING_PARAMETER";
				Assembler.PopupException(msg);
			}

			var valueFirst	= paramVarying.Values[0];
			var valueSecond	= paramVarying.Values[1];

			if (valueFirst.KPIsMomentumsAverage.NetProfit == valueSecond.KPIsMomentumsAverage.NetProfit) {
				if (valueFirst.KPIsMomentumsAverage.NetProfit == 0) {
					string msg1 = "YOU_FORGOT_TO_RESET_SOMETHING_IN_CORRELATOR__SECOND_CLICK_ON_SEQUENCER_RESULTS";
					Assembler.PopupException(msg1);
					return;
				}
				string msg = "TRYING_TO_CATCH_WHERE_MOMENTUMS_BECOME_EQUAL_WHILE_THEY_MUSTNT_BE";
				Assembler.PopupException(msg);
			}
			#endif
		}
		void chooseAllOthersFullOnExcept(OneParameterAllValuesAveraged varyingThisWhileOthersFullyChosen) {
			foreach (OneParameterAllValuesAveraged eachParam in this.ParametersByName.Values) {
				if (eachParam == varyingThisWhileOthersFullyChosen) continue;
				eachParam.chooseAllValues();
			}
		}
		void reset() {
			this.momentumsAveragedByParameter_cached = null;
			if (this.ParametersByName.Count == 0) {
				string msg = "NOTHING_TO_RESET ParametersByName.Count=0 //AvgCorMomentumsCalculator.reset()";
				Assembler.PopupException(msg);
			}
			foreach (OneParameterAllValuesAveraged varyingThisWhileOthersFullyChosen in this.ParametersByName.Values) {
				if (varyingThisWhileOthersFullyChosen.ValuesByParam.Count <= 1) continue;
				OneParameterAllAvgCorMomentums momentumsForVariatedParameter = this.MomentumsAveragedByParameter[varyingThisWhileOthersFullyChosen.ParameterName];
				foreach (double oneValueCheckedOthersOff in varyingThisWhileOthersFullyChosen.ValuesByParam.Keys) {
					OneParameterOneValue thisCheckedOthersOff = varyingThisWhileOthersFullyChosen.ValuesByParam[oneValueCheckedOthersOff];
					AvgCorMomentums momentumForVariatedValue = momentumsForVariatedParameter.MomentumsByValue[oneValueCheckedOthersOff];
					momentumForVariatedValue.KPIsAvgAverage		.Reset();
					momentumForVariatedValue.KPIsAvgDispersion	.Reset();
					momentumForVariatedValue.KPIsAvgVariance	.Reset();
				}
			}
		}
	}
}
