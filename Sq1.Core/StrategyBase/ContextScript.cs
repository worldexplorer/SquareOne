using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Backtesting;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.Sequencing;

namespace Sq1.Core.StrategyBase {
	public class ContextScript : ContextChart {
		[JsonIgnore]	public const string DEFAULT_NAME = "Default";
		
		[JsonProperty]	public PositionSize										PositionSize;
		[JsonProperty]	public SortedDictionary<int, ScriptParameter>			ScriptParametersById;
		[JsonProperty]	public Dictionary<string, List<IndicatorParameter>>		IndicatorParametersByName;
		
		[JsonProperty]	public bool							IsCurrent;
		[JsonProperty]	public bool							StrategyEmittingOrders;

		[JsonProperty]	public List<string>					ReporterShortNamesUserInvokedJSONcheck;
		[JsonProperty]	public bool							BacktestOnTriggeringYesWhenNotSubscribed;
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

		[JsonProperty]	public bool							MinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim;

		[JsonIgnore]	public List<IndicatorParameter>		ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders { get {
				List<IndicatorParameter> ret = new List<IndicatorParameter>();
				ret.AddRange(this.ScriptParametersById.Values);
				//v1 foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) ret.AddRange(iParams);
				//v2 fixes OneParameterControl.indicatorParameterNullUnsafe: ScriptAndIndicatorParametersMergedUnclonedForSequencerByName.ContainsKey(" + this.parameter.ParameterName + ") == false becomes true
				foreach (string indicatorName in this.IndicatorParametersByName.Keys) {
					List<IndicatorParameter> iParams = this.IndicatorParametersByName[indicatorName];
					foreach (IndicatorParameter iParam in iParams) {
						if (iParam.IndicatorName == indicatorName) continue;
						iParam.IndicatorName = indicatorName;
					}
					ret.AddRange(iParams);
				}
				return ret;
			} }
		[JsonIgnore]	public SortedDictionary<string, IndicatorParameter> ScriptAndIndicatorParametersMergedUnclonedForSequencerByName { get {
				SortedDictionary<string, IndicatorParameter> ret = new SortedDictionary<string, IndicatorParameter>();
				foreach (IndicatorParameter iParam in this.ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders) {
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
			} }
		[JsonProperty]	public	string										ScriptAndIndicatorParametersMergedUnclonedForSequencerByName_AsString { get {
				SortedDictionary<string, IndicatorParameter> merged = this.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName;
				if (merged.Count == 0) return "(NoParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }

		[JsonIgnore]	public bool WillBacktestOnAppRestart { get {
				return	this.BacktestOnRestart
					&&	this.IsStreaming
					&&	this.IsStreamingTriggeringScript;
		} }
		[JsonProperty]	public string						SequenceIterationName;
		[JsonProperty]	public int							SequenceIterationSerno;

		public ContextScript(ContextChart upgradingFromSimpleChart = null, string name = "UNDEFINED") : this(name) {
			base.AbsorbFrom(upgradingFromSimpleChart);
		}
		public ContextScript(string name = "UNDEFINED") : this() {
			string msig = "THIS_CTOR_IS_IVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC";
			this.Name = name;
		}
		ContextScript() : base() {
			PositionSize							= new PositionSize(PositionSizeMode.SharesConstantEachTrade, 1);
			ScriptParametersById					= new SortedDictionary<int, ScriptParameter>();
			IndicatorParametersByName				= new Dictionary<string, List<IndicatorParameter>>();
			
			IsCurrent								= false;
			StrategyEmittingOrders					= false;
			BacktestOnTriggeringYesWhenNotSubscribed				= false;
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

			SequenceIterationName = "it was a F5 GUI invoked backtest, not a sequencer-generated run";
			SequenceIterationSerno = -1;	// it was a F5 GUI invoked backtest, not a sequencer-generated run;
		}
		
		public ContextScript CloneAndAbsorbFromSystemPerformanceRestoreAble(SystemPerformanceRestoreAble sysPerfOptimized, string newScriptContextName = null) {
			Assembler.PopupException("TESTME //CloneAndAbsorbFromSystemPerformanceRestoreAble()", null, false);
			ContextScript clone = (ContextScript)base.MemberwiseClone();
			if (string.IsNullOrEmpty(newScriptContextName) == false) {
				clone.Name = newScriptContextName;
			}
			clone.ScriptParametersById			= sysPerfOptimized.ScriptParametersById_BuiltOnBacktestFinished;
			clone.IndicatorParametersByName		= sysPerfOptimized.IndicatorParametersByName_BuiltOnBacktestFinished;
			clone.replaceWithClonesScriptAndIndicatorParameters("NEW_CTX_FROM_OPTIMIZED", false);
			return clone;
		}
		public void AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerOnly(string reasonToClone, ContextScript found) {
			this.ScriptParametersById			= found.ScriptParametersById;
			this.IndicatorParametersByName		= found.IndicatorParametersByName;
			this.replaceWithClonesScriptAndIndicatorParameters("FOR_" + reasonToClone, false);
		}
		public void AbsorbFrom_duplicatedInSliders_or_importedFromSequencer(ContextScript found, bool absorbScriptAndIndicatorParams = true) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name = found.Name;
			base.AbsorbFrom(found);
			
			this.PositionSize = found.PositionSize.Clone();
			if (absorbScriptAndIndicatorParams) {
				this.AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerOnly("FOR_userClickedDuplicateCtx", found);
			}
			
			//some of these guys can easily be absorbed by object.MemberwiseClone(), why do I prefer to maintain the growing list manually?... 
			//this.ChartBarSpacing							= found.ChartBarSpacing;
			this.StrategyEmittingOrders						= found.StrategyEmittingOrders;
			
			this.BacktestOnTriggeringYesWhenNotSubscribed	= found.BacktestOnTriggeringYesWhenNotSubscribed;
			this.BacktestOnRestart							= found.BacktestOnRestart;
			this.BacktestOnSelectorsChange					= found.BacktestOnSelectorsChange;
			this.BacktestOnDataSourceSaved					= found.BacktestOnDataSourceSaved;

			this.ReporterShortNamesUserInvokedJSONcheck		= new List<string>(found.ReporterShortNamesUserInvokedJSONcheck);
			this.FillOutsideQuoteSpreadParanoidCheckThrow	= found.FillOutsideQuoteSpreadParanoidCheckThrow;
			this.BacktestStrokesPerBar						= found.BacktestStrokesPerBar;
			this.SpreadModelerClassName						= found.SpreadModelerClassName;
			this.SpreadModelerPercent						= found.SpreadModelerPercent;
		}
		public ContextScript CloneResetAllToMin_ForSequencer(string reasonToClone) {
			ContextScript ret = (ContextScript)base.MemberwiseClone();
			ret.replaceWithClonesScriptAndIndicatorParameters(reasonToClone, true, true);
			return ret;
		}
		void replaceWithClonesScriptAndIndicatorParameters(string reasonToClone, bool resetAllToMin = true, bool leaveNonSequencedAsCurrent = true) {
			//this.ScriptParametersByIdNonCloned = this.ScriptParametersById;
			SortedDictionary<int, ScriptParameter> scriptParametersByIdClonedReset = new SortedDictionary<int, ScriptParameter>();
			foreach (int id in this.ScriptParametersById.Keys) {
				ScriptParameter sp = this.ScriptParametersById[id];
				ScriptParameter spClone = sp.CloneAsScriptParameter(reasonToClone);
				if (resetAllToMin) {
					if (spClone.WillBeSequenced == false && leaveNonSequencedAsCurrent == true) {
						// don't reset to min
					} else {
						spClone.ValueCurrent = spClone.ValueMin;
					}
				}
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
					if (resetAllToMin) {
						if (ipClone.WillBeSequenced == false && leaveNonSequencedAsCurrent == true) {
							// don't reset to min
						} else {
							ipClone.ValueCurrent = ipClone.ValueMin;
						}
					}
					iParamsCloned.Add(ipClone);
				}
			}
			this.IndicatorParametersByName = indicatorParametersByNameClonedReset;
		}
		public int AbsorbOnlyScriptAndIndicatorParameterCurrentValues_toDisposableFromSequencer(ContextScript ctxSequencerSequenced) {
			int ret = 0;
			try {
				this.SequenceIterationName = ctxSequencerSequenced.Name;
				this.SequenceIterationSerno = ctxSequencerSequenced.SequenceIterationSerno;
				foreach (int id in ctxSequencerSequenced.ScriptParametersById.Keys) {
					ScriptParameter spOpt  = ctxSequencerSequenced	.ScriptParametersById[id];
					ScriptParameter spMine = this					.ScriptParametersById[id];
					if (spMine.ValueCurrent == spOpt.ValueCurrent) continue;	//looks stupig but inserting breakpoint on next line is useful
					spMine.ValueCurrent = spOpt.ValueCurrent;
					ret++;
				}
				foreach (string indicatorName in ctxSequencerSequenced.IndicatorParametersByName.Keys) {
					List<IndicatorParameter> ipsOpt  = ctxSequencerSequenced	.IndicatorParametersByName[indicatorName];
					List<IndicatorParameter> ipsMine = this						.IndicatorParametersByName[indicatorName];
					foreach (IndicatorParameter ipOpt in ipsOpt) {
						foreach (IndicatorParameter ipMine in ipsMine) {
							if (ipMine.FullName != ipOpt.FullName) continue;
							if (ipMine.ValueCurrent == ipOpt.ValueCurrent) continue;	//looks stupig but inserting breakpoint on next line is useful
							ipMine.ValueCurrent = ipOpt.ValueCurrent;
							ret++;
							break;
						}
					}
				}
			} catch (Exception ex) {
				string msg = "SCRIPT_RECOMPILED__SEQUENCER_WAS_NOT_NOTIFIED_ABOUT_CHANGED_SCRIPT_OR_INDICATOR_PARAMETERS";
				Assembler.PopupException(msg);
			}
			return ret;
		}
		public int AbsorbOnlyScriptAndIndicatorParameterCurrentValues_fromSequencer(SystemPerformanceRestoreAble sperfParametersToAbsorbIntoDefault) {
			int ret = 0;
			try {
				int paramsRebuit = sperfParametersToAbsorbIntoDefault.Ensure_OnBacktestFinisheds_AreRebuiltAfterDeserialization();
				if (sperfParametersToAbsorbIntoDefault.ScriptParametersById_BuiltOnBacktestFinished == null) {
					string msg = "MUST_BE_EMPTY_LIST_INSTEAD_OF_NULL_AFTER Ensure_OnBacktestFinisheds_AreRebuiltAfterDeserialization()";
					Assembler.PopupException(msg);
					return ret;
				}
				foreach (int id in sperfParametersToAbsorbIntoDefault.ScriptParametersById_BuiltOnBacktestFinished.Keys) {
					ScriptParameter spOpt = sperfParametersToAbsorbIntoDefault.ScriptParametersById_BuiltOnBacktestFinished[id];
					ScriptParameter spMine = this.ScriptParametersById[id];
					if (spMine.ValueCurrent == spOpt.ValueCurrent) continue;	//looks stupig but inserting breakpoint on next line is useful
					spMine.ValueCurrent = spOpt.ValueCurrent;
					ret++;
				}

				if (sperfParametersToAbsorbIntoDefault.ScriptParametersById_BuiltOnBacktestFinished == null) {
					string msg = "MUST_BE_EMPTY_SORTED_DICTIONARY_INSTEAD_OF_NULL_AFTER Ensure_OnBacktestFinisheds_AreRebuiltAfterDeserialization()";
					Assembler.PopupException(msg);
					return ret;
				}
				foreach (string indicatorName in sperfParametersToAbsorbIntoDefault.IndicatorParametersByName_BuiltOnBacktestFinished.Keys) {
					List<IndicatorParameter> ipsOpt  = sperfParametersToAbsorbIntoDefault	.IndicatorParametersByName_BuiltOnBacktestFinished[indicatorName];
					List<IndicatorParameter> ipsMine = this									.IndicatorParametersByName[indicatorName];
					foreach (IndicatorParameter ipOpt in ipsOpt) {
						foreach (IndicatorParameter ipMine in ipsMine) {
							if (ipMine.FullName != ipOpt.FullName) continue;
							if (ipMine.ValueCurrent == ipOpt.ValueCurrent) continue;	//looks stupig but inserting breakpoint on next line is useful
							ipMine.ValueCurrent = ipOpt.ValueCurrent;
							ret++;
							break;
						}
					}
				}
			} catch (Exception ex) {
				string msg = "SCRIPT_RECOMPILED__SEQUENCER_WAS_NOT_NOTIFIED_ABOUT_CHANGED_SCRIPT_OR_INDICATOR_PARAMETERS";
				Assembler.PopupException(msg);
			}
			return ret;
		}
		public object FindOrCreateReportersSnapshot(Reporter reporterActivated) {
			string reporterName = reporterActivated.TabText;
			if (this.ReportersSnapshots.ContainsKey(reporterName) == false) {
				this.ReportersSnapshots.Add(reporterName, reporterActivated.CreateSnapshotToStoreInScriptContext());
			}
			return this.ReportersSnapshots[reporterName];
		}
		public string SymbolScaleIntervalDataRangeForScriptContextNewName { get {
			string ret = this.Symbol + " " + this.ScaleInterval + " " + this.DataRange;
			return ret;
		} }

		internal int ScriptParametersReflectedAbsorbFromCurrentContextReplace(SortedDictionary<int, ScriptParameter> scriptParametersById_ReflectedCached) {
			string msig = " //ScriptParametersAbsorbFromReflectedReplace()";
			int ret = 0;
			if (scriptParametersById_ReflectedCached.Count == 0) return ret;
			foreach (ScriptParameter spReflected in scriptParametersById_ReflectedCached.Values) {
				if (this.ScriptParametersById.ContainsKey(spReflected.Id) == false) continue;
				ScriptParameter spContext = this.ScriptParametersById[spReflected.Id];
				bool valueCurrentAbsorbed = spReflected.AbsorbCurrentFixBoundariesIfChanged(spContext);
				if (valueCurrentAbsorbed) ret++;
			}
			this.ScriptParametersById = scriptParametersById_ReflectedCached;
//			bool dontSaveWeSequence = this.Name.Contains(Sequencer.ITERATION_PREFIX);
//			if (dontSaveWeSequence) {
//				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_SEQUENCER_PROVIDED_IN_SCRIPTCONTEXT #1";
//				Assembler.PopupException(msg + msig, null, true);
//				//strategySerializeRequired = false;
//			}
			if (ret == 0) {
				string msg = "NO_SCRIPT_PARAMETER_VALUES_ABSORBED_SAME_FOR_Count[" + scriptParametersById_ReflectedCached.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			return ret;
		}
		internal int PushIndicatorParamsCurrentValuesIntoReflectedIndicators(
				Dictionary<string, List<IndicatorParameter>> indicatorParametersByIndicator_ReflectedCached) {
			string msig = " //IndicatorParamsAbsorbFromReflectedReplace()";
			int ret = 0;
			if (indicatorParametersByIndicator_ReflectedCached.Count == 0) return ret;
			foreach (string indicatorName in indicatorParametersByIndicator_ReflectedCached.Keys) {
				if (this.IndicatorParametersByName.ContainsKey(indicatorName) == false) continue;
				List<IndicatorParameter> iParamsCtx = this.IndicatorParametersByName[indicatorName];
				List<IndicatorParameter> iParamsReflected = indicatorParametersByIndicator_ReflectedCached[indicatorName];

				foreach (IndicatorParameter iParamCtx in iParamsCtx) {
					//DIFFERENT_POINTERS_100%_COMPARING_BY_NAME_IN_LOOP if (iParamsReflected.Contains(iParamInstantiated) == false) continue;
					foreach (IndicatorParameter iParamReflected in iParamsReflected) {
						//v1 WILL_ALWAYS_CONTINUE_KOZ_iParamCtx.IndicatorName="NOT_ATTACHED_TO_ANY_INDICATOR"__LAZY_TO_SET_AFTER_DESERIALIZATION_KOZ_WILL_THROW_IT_10_LINES_BELOW_AND_PARAM_NAMES_ARE_UNIQUE_WITHIN_INDICATOR if (iParamReflected.FullName != iParamCtx.FullName) {
						if (iParamReflected.Name != iParamCtx.Name) {
							string msg = "iParamReflected[" + iParamReflected	.ToString() + "].Name[" + iParamReflected	.Name + "]"
								   + " != iParamCtx["		+ iParamCtx			.ToString() + "].Name[" + iParamCtx			.Name + "]";
							Assembler.PopupException(msg);
							continue;
						}
						bool valueCurrentAbsorbed = iParamReflected.AbsorbCurrentFixBoundariesIfChanged(iParamCtx);
						if (valueCurrentAbsorbed) ret++;
						//WOZU?... break;
					}
				}
			}
			//YOU_JUST_SYNCHED_IN_TWO_INNER_LOOPS__WHY_DO_YOU_OVERWRITE_BRO????v1? this.IndicatorParametersByName = indicatorParametersByIndicator_ReflectedCached;

//			bool dontSaveWeSequence = this.Name.Contains(Sequencer.ITERATION_PREFIX);
//			if (dontSaveWeSequence) {
//				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_SEQUENCER_PROVIDED_IN_SCRIPTCONTEXT #2";
//				Assembler.PopupException(msg + msig, null, true);
//				//strategySerializeRequired = false;
//			}
			if (ret == 0) {
				string msg = "NO_INDICATOR_PARAMETER_VALUES_ABSORBED_SAME_FOR_Count[" + indicatorParametersByIndicator_ReflectedCached.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			return ret;
		}
	}
}
