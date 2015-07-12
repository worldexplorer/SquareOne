#if DEBUG

using System;
using System.Collections.Generic;
using System.Reflection;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public partial class Script {
		#region Only ReusableExecutor uses it ToCheck; copypaste from ContextScript
		List<IndicatorParameter> ScriptAndIndicatorParametersMergedUnclonedForReusableExecutorToCheck {
			get {
				List<IndicatorParameter> ret = new List<IndicatorParameter>();
				this.scriptParametersById_ReflectionForced = true;
				ret.AddRange(this.ScriptParametersById_ReflectedCached.Values);
				//v1 foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) ret.AddRange(iParams);
				//v2 fixes OneParameterControl.indicatorParameterNullUnsafe: ScriptAndIndicatorParametersMergedUnclonedForSequencerByName.ContainsKey(" + this.parameter.ParameterName + ") == false becomes true
				this.indicatorsByName_ReflectionForced = true;
				foreach (string indicatorName in this.IndicatorsByName_ReflectedCached.Keys) {
					List<IndicatorParameter> iParams = new List<IndicatorParameter>(this.IndicatorsByName_ReflectedCached[indicatorName].ParametersByName.Values);
					foreach (IndicatorParameter iParam in iParams) {
						if (iParam.IndicatorName == indicatorName) continue;
						iParam.IndicatorName = indicatorName;
					}
					ret.AddRange(iParams);
				}
				return ret;
			}
		}
		SortedDictionary<string, IndicatorParameter> ScriptAndIndicatorParametersMergedUnclonedForReusableExecutorToCheckByName {
			get {
				SortedDictionary<string, IndicatorParameter> ret = new SortedDictionary<string, IndicatorParameter>();
				foreach (IndicatorParameter iParam in this.ScriptAndIndicatorParametersMergedUnclonedForReusableExecutorToCheck) {
					if (ret.ContainsKey(iParam.FullName)) {
						if (iParam.FullName.Contains("NOT_ATTACHED_TO_ANY_INDICATOR_YET")) {
							string msg2 = "IM_CLONING_A_CONTEXT_TO_PUSH_FROM_SEQUENCER__NO_IDEA_HOW_TO_FIX";
							Assembler.PopupException(msg2);
							continue;
						}
						string msg = "AVOIDING_KEY_ALREADY_EXISTS [" + iParam.FullName + "]";
						Assembler.PopupException(msg);
						continue;
					}
					ret.Add(iParam.FullName, iParam);
				}
				return ret;
			}
		}
		public string ScriptAndIndicatorParametersMergedUnclonedForReusableExecutorToCheckByName_AsString {
			get {
				SortedDictionary<string, IndicatorParameter> merged = this.ScriptAndIndicatorParametersMergedUnclonedForReusableExecutorToCheckByName;
				if (merged.Count == 0) return "(NoParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			}
		}
		#endregion
	}
}
#endif