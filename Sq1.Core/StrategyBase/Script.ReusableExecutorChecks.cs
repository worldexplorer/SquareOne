using System;
//using System.Collections.Generic;

//using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public partial class Script {
		// this was causing clearing indicatorParametersByIndicator or indicatosByName...

		//List<IndicatorParameter> scriptAndIndicatorParameters_reflectedMergedUncloned_forReusableExecutorToCheck { get {
		//    List<IndicatorParameter> ret = new List<IndicatorParameter>();
		//    this.ScriptParametersById_reflectionForced_byClearingCache();
		//    ret.AddRange(this.ScriptParametersById_reflectedCached_primary.Values);
		//    //v1 foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) ret.AddRange(iParams);
		//    //v2 fixes OneParameterControl.indicatorParameter_nullUnsafe: ScriptAndIndicatorParametersMergedUnclonedForSequencerByName.ContainsKey(" + this.parameter.ParameterName + ") == false becomes true
		//    this.IndicatorsByName_reflectionForced_byClearingCache();
		//    foreach (string indicatorName in this.IndicatorsByName_reflectedCached_primary.Keys) {
		//        List<IndicatorParameter> iParams = new List<IndicatorParameter>(this.IndicatorsByName_reflectedCached_primary[indicatorName].ParametersByName.Values);
		//        foreach (IndicatorParameter iParam in iParams) {
		//            if (iParam.IndicatorName != indicatorName) {
		//                iParam.IndicatorName  = indicatorName;
		//            }
		//            if (string.IsNullOrEmpty(iParam.Owner_asString)) {
		//                iParam.Owner_asString = "script[" + this.StrategyName + "].IndicatorsByName_ReflectedCached[" + indicatorName + "]";
		//            }
		//        }
		//        ret.AddRange(iParams);
		//    }
		//    return ret;
		//} }
		//SortedDictionary<string, IndicatorParameter> scriptAndIndicatorParameters_reflectedMergedUncloned_forReusableExecutorToCheckByName { get {
		//    SortedDictionary<string, IndicatorParameter> ret = new SortedDictionary<string, IndicatorParameter>();
		//    foreach (IndicatorParameter iParam in this.scriptAndIndicatorParameters_reflectedMergedUncloned_forReusableExecutorToCheck) {
		//        if (ret.ContainsKey(iParam.FullName)) {
		//            if (iParam.FullName.Contains("NOT_ATTACHED_TO_ANY_INDICATOR_YET")) {
		//                string msg2 = "IM_CLONING_A_CONTEXT_TO_PUSH_FROM_SEQUENCER__NO_IDEA_HOW_TO_FIX";
		//                Assembler.PopupException(msg2);
		//                continue;
		//            }
		//            string msg = "AVOIDING_KEY_ALREADY_EXISTS [" + iParam.FullName + "]";
		//            Assembler.PopupException(msg);
		//            continue;
		//        }
		//        ret.Add(iParam.FullName, iParam);
		//    }
		//    return ret;
		//} }
		//public string ScriptAndIndicatorParameters_mergedUncloned_forReusableExecutorToCheckByName_AsString { get {
		//    SortedDictionary<string, IndicatorParameter> merged = this.scriptAndIndicatorParameters_reflectedMergedUncloned_forReusableExecutorToCheckByName;
		//    if (merged.Count == 0) return "(NoParameters)";
		//    string ret = "";
		//    foreach (string indicatorDotParameter in merged.Keys) {
		//        ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
		//    }
		//    ret = ret.TrimEnd(",".ToCharArray());
		//    return "(" + ret + ")";
		//} }
	}
}
