using System;
using System.Linq;

using Sq1.Core.Sequencing;
using Sq1.Core.Indicators;

namespace Sq1.Core.Correlation {
	public class KPIsCalculator {
		Correlator correlator;

		internal KPIsCalculator(Correlator correlator) {
			this.correlator = correlator;
		}

		internal void calculateGlobals() {
			// not creating new OneParameterAllValuesAveraged() aims to refreshOlv and UseWaitCursor=false;
			// looks stupid, copypaste from rebuildParametersByName() AFFRAID_OF_LOSING_OnParameterRecalculatedLocalsAndDeltas_EVENT
			foreach (SystemPerformanceRestoreAble eachRun in this.correlator.SequencedBacktestOriginal.Subset) {
				foreach (string indicatorDotParameter in eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter eachIndicator = eachRun.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[indicatorDotParameter];
					string parameterName = indicatorDotParameter;	// eachIndicator.Name is "NOT_ATTACHED_TO_ANY_INDICATOR_YET" when deserialized
					double backtestedValue = eachIndicator.ValueCurrent;

					if (this.correlator.ParametersByName.ContainsKey(parameterName) == false) {
						string msg = "I_REFUSE_TO_CREATE_NEW_AFFRAID_OF_LOSING_OnParameterRecalculatedLocalsAndDeltas_EVENT"
							+ " RETHINK_rebuildParametersByName() ParametersByName.ContainsKey(" + parameterName + ")=false";
						Assembler.PopupException(msg);
						continue;
					}
					OneParameterAllValuesAveraged eachParameterRebuilt = this.correlator.ParametersByName[parameterName];
				}
			}
		}
		internal void CalculateLocalsAndDeltas(bool raiseAllEvents = true, bool isUserClick_Serialize = true) {
			foreach (OneParameterAllValuesAveraged eachParam in this.correlator.ParametersByName.Values) {
				if (eachParam.OneParamOneValueByValues.Count <= 1) continue;
				eachParam.CalculateLocalsAndDeltas_forEachValue_and3artificials();
			}
		}

	}
}
