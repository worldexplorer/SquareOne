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
		
		[JsonProperty]	public PositionSize										PositionSize;
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

		[JsonIgnore]	public List<IndicatorParameter>		ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders { get {
				List<IndicatorParameter> ret = new List<IndicatorParameter>();
				ret.AddRange(this.ScriptParametersById.Values);
				foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) ret.AddRange(iParams);
				return ret;
			} }
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
			PositionSize							= new PositionSize(PositionSizeMode.SharesConstantEachTrade, 1);
			ScriptParametersById					= new SortedDictionary<int, ScriptParameter>();
			IndicatorParametersByName				= new Dictionary<string, List<IndicatorParameter>>();
			
			IsCurrent								= false;
			StrategyEmittingOrders					= false;
			BacktestOnRestart						= false;
			BacktestOnSelectorsChange				= true;
			BacktestOnDataSourceSaved				= true;
			
			ReporterShortNamesUserInvokedJSONcheck	= new List<string>();
			ReportersSnapshots						= new Dictionary<string, object>();
			
			ApplyCommission							= false;
			EnableSlippage							= false;
			LimitOrderSlippage						= false;
			RoundEquityLots							= false;
			RoundEquityLotsToUpperHundred			= false;
			SlippageTicks							= 1;
			SlippageUnits							= 1.0;

			FillOutsideQuoteSpreadParanoidCheckThrow = false;
			SpreadModelerClassName					= typeof(BacktestSpreadModelerPercentage).Name;
			SpreadModelerPercent					= BacktestStreaming.PERCENTAGE_DEFAULT;
			BacktestStrokesPerBar					= BacktestStrokesPerBar.FourStrokeOHLC;
		}
		
		public ContextScript CloneAndAbsorbFromSystemPerformanceRestoreAble(SystemPerformanceRestoreAble sysPerfOptimized) {
			Assembler.PopupException("TESTME //CloneAndAbsorbFromSystemPerformanceRestoreAble()");
			ContextScript clone = (ContextScript)base.MemberwiseClone();
			clone.AbsorbScriptAndIndicatorParamsOnlyFrom("FOR_ImportedFromOptimizer",
				sysPerfOptimized.ScriptParametersById_BuiltOnBacktestFinished,
				sysPerfOptimized.IndicatorParametersByName_BuiltOnBacktestFinished);
			return clone;
		}
		public void AbsorbScriptAndIndicatorParamsOnlyFrom(string reasonToClone,
					SortedDictionary<int, ScriptParameter>			scriptParametersById,
					Dictionary<string, List<IndicatorParameter>>	indicatorParametersByName) {
			this.ScriptParametersById			= scriptParametersById;
			this.IndicatorParametersByName		= indicatorParametersByName;
			this.replaceWithClonesScriptAndIndicatorParameters("FOR_" + reasonToClone, false);
		}
		public void AbsorbFrom_duplicatedInSliders_or_importedFromOptimizer(ContextScript found, bool absorbScriptAndIndicatorParams = true) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name = found.Name;
			base.AbsorbFrom(found);
			
			this.PositionSize = found.PositionSize.Clone();
			if (absorbScriptAndIndicatorParams) {
				this.AbsorbScriptAndIndicatorParamsOnlyFrom("FOR_userClickedDuplicateCtx", found.ScriptParametersById, found.IndicatorParametersByName);
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
		public ContextScript CloneResetAllToMinForOptimizer() {
			ContextScript ret = (ContextScript)base.MemberwiseClone();
			ret.replaceWithClonesScriptAndIndicatorParameters("FOR_OptimizerParametersSequencer");
			return ret;
		}
		void replaceWithClonesScriptAndIndicatorParameters(string reasonToClone, bool resetAllToMin = true) {
			//this.ScriptParametersByIdNonCloned = this.ScriptParametersById;
			SortedDictionary<int, ScriptParameter> scriptParametersByIdClonedReset = new SortedDictionary<int, ScriptParameter>();
			foreach (int id in this.ScriptParametersById.Keys) {
				ScriptParameter sp = this.ScriptParametersById[id];
				ScriptParameter spClone = sp.CloneAsScriptParameter(reasonToClone);
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
					IndicatorParameter ipClone = iParam.CloneAsIndicatorParameter(reasonToClone);
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

		internal int ScriptParametersReflectedAbsorbFromCurrentContextReplace(SortedDictionary<int, ScriptParameter> scriptParametersById_ReflectedCached) {
			string msig = " //ScriptParametersAbsorbFromReflectedReplace()";
			int ret = 0;
			foreach (ScriptParameter spReflected in scriptParametersById_ReflectedCached.Values) {
				if (this.ScriptParametersById.ContainsKey(spReflected.Id) == false) continue;
				ScriptParameter spContext = this.ScriptParametersById[spReflected.Id];
				bool valueCurrentAbsorbed = spReflected.AbsorbCurrentFixBoundariesIfChanged(spContext);
				if (valueCurrentAbsorbed) ret++;
			}
			this.ScriptParametersById = scriptParametersById_ReflectedCached;
//			bool dontSaveWeOptimize = this.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
//			if (dontSaveWeOptimize) {
//				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT #1";
//				Assembler.PopupException(msg + msig, null, true);
//				//strategySerializeRequired = false;
//			}
			return ret;
		}
		internal int IndicatorParamsReflectedAbsorbFromCurrentContextReplace(Dictionary<string, List<IndicatorParameter>> indicatorParametersByIndicator_ReflectedCached) {
			string msig = " //IndicatorParamsAbsorbFromReflectedReplace()";
			int ret = 0;
			foreach (string indicatorName in indicatorParametersByIndicator_ReflectedCached.Keys) {
				if (this.IndicatorParametersByName.ContainsKey(indicatorName) == false) continue;
				List<IndicatorParameter> iParamsCtx = this.IndicatorParametersByName[indicatorName];
				List<IndicatorParameter> iParamsReflected = indicatorParametersByIndicator_ReflectedCached[indicatorName];

				foreach (IndicatorParameter iParamCtx in iParamsCtx) {
					//DIFFERENT_POINTERS_100%_COMPARING_BY_NAME_IN_LOOP if (iParamsReflected.Contains(iParamInstantiated) == false) continue;
					foreach (IndicatorParameter iParamReflected in iParamsReflected) {
						//v1 WILL_ALWAYS_CONTINUE_KOZ_iParamCtx.IndicatorName="NOT_ATTACHED_TO_ANY_INDICATOR"__LAZY_TO_SET_AFTER_DESERIALIZATION_KOZ_WILL_THROW_IT_10_LINES_BELOW_AND_PARAM_NAMES_ARE_UNIQUE_WITHIN_INDICATOR if (iParamReflected.FullName != iParamCtx.FullName) {
						if (iParamReflected.Name != iParamCtx.Name) {
							string msg = "iParamReflected[" + iParamReflected.ToString() + "].Name[" + iParamReflected.Name + "] != iParamCtx[" + iParamCtx.ToString() + "].Name[" + iParamCtx.Name + "]";
							Assembler.PopupException(msg);
							continue;
						}
						bool valueCurrentAbsorbed = iParamReflected.AbsorbCurrentFixBoundariesIfChanged(iParamCtx);
						if (valueCurrentAbsorbed) ret++;
						break;
					}
				}
			}
			this.IndicatorParametersByName = indicatorParametersByIndicator_ReflectedCached;
//			bool dontSaveWeOptimize = this.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
//			if (dontSaveWeOptimize) {
//				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT #2";
//				Assembler.PopupException(msg + msig, null, true);
//				//strategySerializeRequired = false;
//			}
			return ret;
		}
	}
}
