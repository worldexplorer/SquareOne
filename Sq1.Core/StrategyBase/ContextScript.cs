using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.Backtesting;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.Optimization;

namespace Sq1.Core.StrategyBase {
	public class ContextScript : ContextChart {
		[JsonIgnore]	public const string DEFAULT_NAME = "Default";
		
		[JsonProperty]	public PositionSize					PositionSize;
		[JsonProperty]	public SortedDictionary<int, ScriptParameter>			ScriptParametersById;
		[JsonProperty]	public Dictionary<string, List<IndicatorParameter>>		IndicatorParametersByName;
		
		[JsonProperty]	public bool							IsCurrent;
		[JsonProperty]	public bool							StrategyEmittingOrders;

		[JsonProperty]	public List<string>					ReporterShortNamesUserInvokedJSONcheck;
		[JsonProperty]	public bool							BacktestOnRestart;
		[JsonProperty]	public bool							BacktestOnSelectorsChange;
		[JsonProperty]	public bool							BacktestOnDataSourceSaved;
		[JsonProperty]	public Dictionary<string, object>	ReportersSnapshots;
		
		[JsonProperty]	public bool							ApplyCommission;
		[JsonProperty]	public bool							EnableSlippage;
		[JsonProperty]	public bool							LimitOrderSlippage;
		[JsonProperty]	public bool							RoundEquityLots;
		[JsonProperty]	public bool							RoundEquityLotsToUpperHundred;
		[JsonProperty]	public bool							NoDecimalRoundingForLimitStopPrice;
		[JsonProperty]	public double						SlippageUnits;
		[JsonProperty]	public int							SlippageTicks;
		[JsonProperty]	public int							PriceStep;
		
		[JsonProperty]	public bool							FillOutsideQuoteSpreadParanoidCheckThrow;
		[JsonProperty]	public string						SpreadModelerClassName;
		[JsonProperty]	public double						SpreadModelerPercent;
		[JsonProperty]	public BacktestStrokesPerBar		BacktestStrokesPerBar;

		//[JsonProperty]	Dictionary<int,ScriptParameter>		ScriptParametersByIdNonCloned;
		//[JsonIgnore]	public List<IndicatorParameter>		ScriptAndIndicatorParametersMergedNonClonedForSequencer { get {
		//		List<IndicatorParameter> ret = new List<IndicatorParameter>();
		//		ret.AddRange(this.ScriptParametersByIdNonCloned.Values);
		//		foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) {
		//			ret.AddRange(iParams);
		//		}
		//		return ret;
		//	} }

		[JsonIgnore]	public List<IndicatorParameter>		ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders { get {
				// MAKE_SURE_YOU_DONT_KEEP_THE_REFERENCE; use ParametersMergedCloned otherwize
				List<IndicatorParameter> ret = new List<IndicatorParameter>();
				ret.AddRange(this.ScriptParametersById.Values);
				foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) {
					ret.AddRange(iParams);
				}
				return ret;
			} }
		//PARASITE! clicking on a slider directly affects strategy.Context and re-backtests it; no clones
		//[JsonIgnore]	public List<IndicatorParameter> ParametersMergedCloned { get {
		//		List<IndicatorParameter> ret = new List<IndicatorParameter>();
		//		foreach (IndicatorParameter iParam in this.ParametersMerged) ret.Add(iParam.Clone());
		//		return ret;
		//	} }
		[JsonIgnore]	public SortedDictionary<string, IndicatorParameter> ScriptAndIndicatorParametersMergedClonedForSequencerByName { get {
				SortedDictionary<string, IndicatorParameter> ret = new SortedDictionary<string, IndicatorParameter>();
				foreach (IndicatorParameter iParam in this.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders) ret.Add(iParam.FullName, iParam);
				return ret;
			} }

		[JsonIgnore]	public bool WillBacktestOnAppRestart { get {
				return	this.BacktestOnRestart
					&&	this.IsStreaming
					&&	this.IsStreamingTriggeringScript;
		} }

		public ContextScript(ContextChart upgradingFromSimpleChart = null, string name = "UNDEFINED") : this(name) {
			base.AbsorbFrom(upgradingFromSimpleChart);
		}
		public ContextScript(string name = "UNDEFINED") : this() {
			this.Name = name;
		}
		protected ContextScript() : base() {
			PositionSize				= new PositionSize(PositionSizeMode.SharesConstantEachTrade, 1);
			ScriptParametersById		= new SortedDictionary<int, ScriptParameter>();
			IndicatorParametersByName	= new Dictionary<string, List<IndicatorParameter>>();
			
			IsCurrent					= false;
			StrategyEmittingOrders		= false;
			BacktestOnRestart			= false;
			BacktestOnSelectorsChange	= true;
			BacktestOnDataSourceSaved	= true;
			
			ReporterShortNamesUserInvokedJSONcheck	= new List<string>();
			ReportersSnapshots						= new Dictionary<string, object>();
			
			ApplyCommission					= false;
			EnableSlippage					= false;
			LimitOrderSlippage				= false;
			RoundEquityLots					= false;
			RoundEquityLotsToUpperHundred	= false;
			SlippageTicks					= 1;
			SlippageUnits					= 1.0;

			FillOutsideQuoteSpreadParanoidCheckThrow = false;
			SpreadModelerClassName		= typeof(BacktestSpreadModelerPercentage).Name;
			SpreadModelerPercent		= BacktestStreaming.PERCENTAGE_DEFAULT;
			BacktestStrokesPerBar				= BacktestStrokesPerBar.FourStrokeOHLC;
		}
		
		public ContextScript CloneAndAbsorbFromSystemPerformanceRestoreAble(SystemPerformanceRestoreAble sysPerfOptimized) {
			Assembler.PopupException("TESTME //CloneAndAbsorbFromSystemPerformanceRestoreAble()");
			ContextScript clone = this.MemberwiseCloneMadePublic();
			clone.absorbScriptAndIndicatorParamsOnlyFrom(
				sysPerfOptimized.ScriptParametersById_BuiltOnBacktestFinished,
				sysPerfOptimized.IndicatorParametersByName_BuiltOnBacktestFinished);
			return clone;
		}
		void absorbScriptAndIndicatorParamsOnlyFrom(
					SortedDictionary<int, ScriptParameter>			scriptParametersById,
					Dictionary<string, List<IndicatorParameter>>	indicatorParametersByName) {
			this.ScriptParametersById			= scriptParametersById;
			this.IndicatorParametersByName		= indicatorParametersByName;
			this.cloneReferenceTypes(false);
		}
		public void AbsorbFrom(ContextScript found, bool absorbScriptAndIndicatorParams = true) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name = found.Name;
			base.AbsorbFrom(found);
			
			this.PositionSize = found.PositionSize.Clone();
			if (absorbScriptAndIndicatorParams) {
				//v1
				//this.ScriptParametersById					= new Dictionary<int, ScriptParameter>(found.ScriptParametersById);
				//this.IndicatorParametersByName			= new Dictionary<string, List<IndicatorParameter>>(found.IndicatorParametersByName);
				//v2
				//this.ScriptParametersById					= found.ScriptParametersById;
				//this.IndicatorParametersByName			= found.IndicatorParametersByName;
				//this.CloneReferenceTypes(false);
				this.absorbScriptAndIndicatorParamsOnlyFrom(found.ScriptParametersById, found.IndicatorParametersByName);
			}
			
			//some of these guys can easily be absorbed by object.MemberwiseClone(), why do I prefer to maintain the growing list manually?... 
			//this.ChartBarSpacing							= found.ChartBarSpacing;
			this.StrategyEmittingOrders						= found.StrategyEmittingOrders;
			this.BacktestOnRestart							= found.BacktestOnRestart;
			this.BacktestOnSelectorsChange					= found.BacktestOnSelectorsChange;
			this.BacktestOnDataSourceSaved					= found.BacktestOnDataSourceSaved;
			this.ReporterShortNamesUserInvokedJSONcheck		= new List<string>(found.ReporterShortNamesUserInvokedJSONcheck);
			this.FillOutsideQuoteSpreadParanoidCheckThrow	= found.FillOutsideQuoteSpreadParanoidCheckThrow;
			this.BacktestStrokesPerBar						= found.BacktestStrokesPerBar;
			this.SpreadModelerClassName						= found.SpreadModelerClassName;
			this.SpreadModelerPercent						= found.SpreadModelerPercent;
		}
		public new ContextScript MemberwiseCloneMadePublic() {
			return (ContextScript)base.MemberwiseClone();
		}
		public ContextScript CloneResetAllToMinForOptimizer() {
			ContextScript ret = (ContextScript)base.MemberwiseClone();
			ret.cloneReferenceTypes();
			return ret;
		}
		public ContextScript CloneThatUserPushesFromOptimizerToStrategy(string scriptContextNewName) {
			ContextScript ret = new ContextScript(scriptContextNewName);
			ret.AbsorbFrom(this);
			return ret;
		}
		void cloneReferenceTypes(bool resetAllToMin = true) {
			//this.ScriptParametersByIdNonCloned = this.ScriptParametersById;
			SortedDictionary<int, ScriptParameter> scriptParametersByIdClonedReset = new SortedDictionary<int, ScriptParameter>();
			foreach (int id in this.ScriptParametersById.Keys) {
				ScriptParameter sp = this.ScriptParametersById[id];
				ScriptParameter spClone = sp.Clone();
				if (resetAllToMin) spClone.ValueCurrent = spClone.ValueMin;
				scriptParametersByIdClonedReset.Add(id, spClone);
			}
			this.ScriptParametersById = scriptParametersByIdClonedReset;

			Dictionary<string, List<IndicatorParameter>> indicatorParametersByNameClonedReset = new Dictionary<string, List<IndicatorParameter>>();
			foreach (string indicatorName in this.IndicatorParametersByName.Keys) {
				List<IndicatorParameter> iParams = this.IndicatorParametersByName[indicatorName];
				List<IndicatorParameter> iParamsCloned = new List<IndicatorParameter>();
				indicatorParametersByNameClonedReset.Add(indicatorName, iParamsCloned);
				foreach (IndicatorParameter iParam in iParams) {
					IndicatorParameter ipClone = iParam.Clone();
					if (resetAllToMin) ipClone.ValueCurrent = ipClone.ValueMin;
					iParamsCloned.Add(ipClone);
				}
			}
			this.IndicatorParametersByName = indicatorParametersByNameClonedReset;
		}
		public object FindOrCreateReportersSnapshot(Reporter reporterActivated) {
			string reporterName = reporterActivated.TabText;
			if (this.ReportersSnapshots.ContainsKey(reporterName) == false) {
				this.ReportersSnapshots.Add(reporterName, reporterActivated.CreateSnapshotToStoreInScriptContext());
			}
			return this.ReportersSnapshots[reporterName];
		}
		public string ToStringSymbolScaleIntervalDataRangeForScriptContextNewName() {
			string ret = this.Symbol + " " + this.ScaleInterval + " " + this.DataRange;
			return ret;
		}

		internal bool ScriptParametersAbsorbFromReflectedReplace(SortedDictionary<int, ScriptParameter> scriptParametersById_ReflectedCached) {
			string msig = " //ScriptParametersAbsorbFromReflectedReplace()";
			bool strategySerializeRequired = true;
			foreach (ScriptParameter spReflected in scriptParametersById_ReflectedCached.Values) {
				if (this.ScriptParametersById.ContainsKey(spReflected.Id) == false) continue;
				ScriptParameter spContext = this.ScriptParametersById[spReflected.Id];
				spReflected.AbsorbCurrentFixBoundariesIfChanged(spContext);
			}
			// 
			this.ScriptParametersById = scriptParametersById_ReflectedCached;
			bool dontSaveWeOptimize = this.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
			if (dontSaveWeOptimize) {
				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT";
				Assembler.PopupException(msg + msig, null, false);
				strategySerializeRequired = false;
			}
			return strategySerializeRequired;
		}
		//internal bool ScriptParametersAbsorbMergeFromReflected_halfMerge_doesntRemoveLeftoversAfterRecompilation(SortedDictionary<int, ScriptParameter> scriptParametersById_ReflectedCached) {
		//	string msig = " //ScriptParametersAbsorbFromReflected_notSameObjects()";
		//	bool storeStrategySinceParametersGottenFromScript = false;
		//	foreach (ScriptParameter spReflected in scriptParametersById_ReflectedCached.Values) {
		//		if (this.ScriptParametersById.ContainsKey(spReflected.Id)) {
		//			ScriptParameter spContext = this.ScriptParametersById[spReflected.Id];
		//			//if (spContext.ValueCurrent != spReflected.ValueCurrent) {
		//			//	string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueCurrent=[" + spContext.ValueCurrent + "] => spReflected.ValueCurrent[" + spReflected.ValueCurrent + "] " + this.ToString();
		//			//	#if DEBUG
		//			//	Assembler.PopupException(msg + msig, null, false);
		//			//	#endif
		//			//	spContext.ValueCurrent = spReflected.ValueCurrent;
		//			//	storeStrategySinceParametersGottenFromScript = true;
		//			//}
		//			//if (spContext.ValueMax != spReflected.ValueMax) {
		//			//	string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueMax=[" + spContext.ValueMax + "] => spReflected.ValueMax[" + spReflected.ValueMax + "] " + this.ToString();
		//			//	#if DEBUG
		//			//	Assembler.PopupException(msg + msig, null, false);
		//			//	#endif
		//			//	spContext.ValueCurrent = spReflected.ValueMax;
		//			//	storeStrategySinceParametersGottenFromScript = true;
		//			//}
		//			//if (spContext.ValueMin != spReflected.ValueMin) {
		//			//	string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueMin=" + spContext.ValueMin + "] => spReflected.ValueMin[" + spReflected.ValueMin + "] " + this.ToString();
		//			//	#if DEBUG
		//			//	Assembler.PopupException(msg + msig, null, false);
		//			//	#endif
		//			//	spContext.ValueMin = spReflected.ValueMin;
		//			//	storeStrategySinceParametersGottenFromScript = true;
		//			//}
		//			//if (spContext.Name != spReflected.Name) {
		//			//	string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.Name=[" + spContext.Name + "] => spReflected.Name[" + spReflected.Name + "] " + this.ToString();
		//			//	#if DEBUG
		//			//	Assembler.PopupException(msg + msig, null, false);
		//			//	#endif
		//			//	spContext.Name = spReflected.Name;
		//			//	storeStrategySinceParametersGottenFromScript = true;
		//			//}
		//			spReflected.AbsorbCurrentFixBoundariesIfChanged(spContext);
		//			if (spReflected != spContext) {
		//				// WANNA_EMPLOY_BRAKING_WATCH_DOG?... Sq1.Core.Support.ConcurrentDictionaryGeneric<int, ScriptParameter>
		//				this.ScriptParametersById.Remove(spContext.Id);					// instead of JsonDeserialized,
		//				this.ScriptParametersById.Add(spReflected.Id, spReflected);		// ...put Instantiated pointer into the Context, so ctx saved will take WillBeOptimized and ValueCurrent from UI
		//			}
		//		} else {
		//			this.ScriptParametersById.Add(spReflected.Id, spReflected);
		//			string msg = "ADDED_ScriptParameter[Id=" + spReflected.Id + " value=" + spReflected.ValueCurrent + "] " + this.ToString();
		//			#if DEBUG
		//			Assembler.PopupException(msg + msig, null, false);
		//			#endif
		//			storeStrategySinceParametersGottenFromScript = true;
		//		}
		//	}
		//	if (storeStrategySinceParametersGottenFromScript) {
		//		bool dontSaveWeOptimize = this.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
		//		if (dontSaveWeOptimize) {
		//			string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT";
		//			Assembler.PopupException(msg + msig, null, false);
		//			storeStrategySinceParametersGottenFromScript = false;
		//		}
		//	}
		//	return storeStrategySinceParametersGottenFromScript;
		//}

		internal bool IndicatorParamsAbsorbFromReflectedReplace(Dictionary<string, List<IndicatorParameter>> indicatorParametersByIndicator_ReflectedCached) {
			string msig = " //IndicatorParamsAbsorbFromReflectedReplace()";
			bool strategySerializeRequired = false;
			foreach (string indicatorName in indicatorParametersByIndicator_ReflectedCached.Keys) {
				if (this.IndicatorParametersByName.ContainsKey(indicatorName) == false) continue;
				List<IndicatorParameter> iParamsCtx = this.IndicatorParametersByName[indicatorName];
				List<IndicatorParameter> iParamsReflected = indicatorParametersByIndicator_ReflectedCached[indicatorName];

				foreach (IndicatorParameter iParamCtx in iParamsCtx) {
					//DIFFERENT_POINTERS_100%_COMPARING_BY_NAME_IN_LOOP if (iParamsReflected.Contains(iParamInstantiated) == false) continue;
					foreach (IndicatorParameter iParamReflected in iParamsReflected) {
						if (iParamReflected.FullName != iParamCtx.FullName) continue;
						iParamReflected.AbsorbCurrentFixBoundariesIfChanged(iParamCtx);
						break;
					}
				}
			}
			this.IndicatorParametersByName = indicatorParametersByIndicator_ReflectedCached;
			bool dontSaveWeOptimize = this.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
			if (dontSaveWeOptimize) {
				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT";
				Assembler.PopupException(msg + msig, null, false);
				strategySerializeRequired = false;
			}
			return strategySerializeRequired;
		}
		//internal bool IndicatorParamsAbsorbMergeFromReflected_halfSync_doesntRemoveLeftoversAfterRecompilation(Dictionary<string, Indicator> indicatorsByNameReflected) {
		//	bool strategySaveRequired = false;
		//	foreach (Indicator indicatorInstance in indicatorsByNameReflected.Values) {
		//		if (this.IndicatorParametersByName.ContainsKey(indicatorInstance.Name)) {
		//			string msg = "IndicatorsInitializedInDerivedConstructor are getting initialized from ContextCurrent and will be kept in sync with user clicks"
		//				+ "; ScriptContextCurrent.IndicatorParametersByName are assigned to PanelSlider.Tag and click will save to JSON by StrategyRepo.Save(Strategy)";
		//			List<IndicatorParameter> iParamsCtx = this.IndicatorParametersByName[indicatorInstance.Name];
		//			Dictionary<string, IndicatorParameter> iParamsCtxLookup = new Dictionary<string, IndicatorParameter>();
		//			foreach (IndicatorParameter iParamCtx in iParamsCtx) iParamsCtxLookup.Add(iParamCtx.Name, iParamCtx);

		//			foreach (IndicatorParameter iParamInstantiated in indicatorInstance.ParametersByName.Values) {
		//				if (iParamsCtxLookup.ContainsKey(iParamInstantiated.Name) == false) {
		//					msg = "JSONStrategy_UNCHANGED_BUT_INDICATOR_EVOLVED_AND_INRODUCED_NEW_PARAMETER__APPARENTLY_STORING_DEFAULT_VALUE_IN_CURRENT_CONTEXT"
		//						+ "; CLONE_OF_INSTANTIATED_GOES_TO_CONTEXT_AND_TO_SLIDER__THIS_CLONE_HAS_SHORTER_LIFECYCLE_WILL_REMAIN_IN_SYNC_FROM_WITHIN_CLICK_HANLDER";
		//					iParamsCtx.Add(iParamInstantiated.Clone());
		//					continue;
		//				}
		//				msg = "ABSORBING_CONTEXT_INDICATOR_VALUE_INTO_INSTANTIATED_INDICATOR_PARAMETER";
		//				IndicatorParameter iParamFoundCtx = iParamsCtxLookup[iParamInstantiated.Name];
		//				iParamInstantiated.AbsorbCurrentFixBoundariesIfChanged(iParamFoundCtx);

		//				//WRONG_CONTEXT_AND_SLIDER_ARE_SAME__KEEPING_INSTANTIATED_CHANGING_SEPARATELY
		//				//TRYING_TO_HACK_AGAIN_BEFORE_I_IMPLEMENT_SEPARATE_Script.AbsorbValuesFromCurrentContextAndReplacePointers()
		//				if (iParamInstantiated != iParamFoundCtx) {
		//					#if DEBUG
		//					//Debugger.Break();			//NOPE_ITS_A_CLONE
		//					#endif
		//					iParamsCtx.Remove(iParamFoundCtx);	// instead of JsonDeserialized,
		//					iParamsCtx.Add(iParamInstantiated);	// ...put Instantiated pointer into the Context, so ctx saved will take WillBeOptimized and ValueCurrent from UI
		//				}
		//			}
		//		} else {
		//			string msg = "JSONStrategy_JUST_ADDED_NEW_INDICATOR_WITH_ITS_NEW_PARAMETERS[" + indicatorInstance.Name + "]";
		//			Assembler.PopupException(msg, null, false);
		//			this.IndicatorParametersByName.Add(indicatorInstance.Name, new List<IndicatorParameter>(indicatorInstance.ParametersByName.Values));
		//			strategySaveRequired = true;
		//		}

		//		// WHO_ARE_YOU?? this.indicatorsByName_ReflectedCached.Add(indicatorInstance.Name, indicatorInstance);
		//	}
		//	return strategySaveRequired;
		//}
	}
}
