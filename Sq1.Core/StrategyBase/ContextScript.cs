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
		[JsonProperty]	public Dictionary<string, List<IndicatorParameter>>		IndicatorParametersByIndicatorName;
		
		[JsonProperty]	public bool							IsCurrent;
		[JsonProperty]	public bool							StrategyEmittingOrders;

		[JsonProperty]	public List<string>					ReporterShortNamesUserInvokedJSONcheck;
		[JsonProperty]	public bool							BacktestOnTriggeringYesWhenNotSubscribed;
		[JsonProperty]	public bool							BacktestOnRestart;
		[JsonProperty]	public bool							BacktestOnSelectorsChange;
		[JsonProperty]	public bool							BacktestOnDataSourceSaved;
		[JsonProperty]	public bool							BacktestAfterSubscribed;

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

		[JsonProperty]	public bool							MinimizeGuiExtensiveExecutionAllReportersForTheDurationOfLiveSim;

		[JsonIgnore]	public List<IndicatorParameter>		ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders { get {
				List<IndicatorParameter> ret = new List<IndicatorParameter>();
				ret.AddRange(this.ScriptParametersById.Values);
				//v1 foreach (List<IndicatorParameter> iParams in this.IndicatorParametersByName.Values) ret.AddRange(iParams);
				//v2 fixes OneParameterControl.indicatorParameter_nullUnsafe: ScriptAndIndicatorParametersMergedUnclonedForSequencerByName.ContainsKey(" + this.parameter.ParameterName + ") == false becomes true
				foreach (string indicatorName in this.IndicatorParametersByIndicatorName.Keys) {
					List<IndicatorParameter> iParams = this.IndicatorParametersByIndicatorName[indicatorName];
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
		[JsonProperty]	public string										ScriptAndIndicatorParametersMergedUnclonedForSequencerByName_AsString { get {
				SortedDictionary<string, IndicatorParameter> merged = this.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName;
				if (merged.Count == 0) return "(NoParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }

		[JsonIgnore]	public bool		WillBacktestOnAppRestart { get {
				return	this.BacktestOnRestart
					&&	this.DownstreamSubscribed
					&&	this.StreamingIsTriggeringScript;
		} }
		[JsonProperty]	public string	SequenceIterationName;
		[JsonProperty]	public int		SequenceIterationSerno;

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
			IndicatorParametersByIndicatorName				= new Dictionary<string, List<IndicatorParameter>>();
			
			IsCurrent								= false;
			StrategyEmittingOrders					= false;
			BacktestOnTriggeringYesWhenNotSubscribed				= false;
			BacktestOnRestart						= false;
			BacktestOnSelectorsChange				= true;
			BacktestOnDataSourceSaved				= true;
			BacktestAfterSubscribed					= true;
			
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
		
		public ContextScript CloneAndAbsorb_fromSystemPerformanceRestoreAble(SystemPerformanceRestoreAble sysPerfOptimized, string newScriptContextName = null) {
			Assembler.PopupException("TESTME //CloneAndAbsorb_fromSystemPerformanceRestoreAble()", null, false);
			ContextScript clone = (ContextScript)base.MemberwiseClone();
			if (string.IsNullOrEmpty(newScriptContextName) == false) {
				clone.Name = newScriptContextName;
			}
			clone.ScriptParametersById					= sysPerfOptimized.ScriptParametersById_BuiltOnBacktestFinished;
			clone.IndicatorParametersByIndicatorName	= sysPerfOptimized.IndicatorParametersByName_BuiltOnBacktestFinished;
			clone.replaceWithCloned_scriptAndIndicatorParameters("NEW_CTX_FROM_OPTIMIZED", false);
			return clone;
		}
		public void AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerOnly(string reasonToClone, ContextScript found) {
			this.ScriptParametersById			= found.ScriptParametersById;
			this.IndicatorParametersByIndicatorName		= found.IndicatorParametersByIndicatorName;
			this.replaceWithCloned_scriptAndIndicatorParameters("FOR_" + reasonToClone, false);
		}
		public void AbsorbFrom_duplicatedInSliders_orImportedFromSequencer(ContextScript found, bool absorbScriptAndIndicatorParams = true) {
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
			ret.replaceWithCloned_scriptAndIndicatorParameters(reasonToClone, true, true);
			return ret;
		}
		void replaceWithCloned_scriptAndIndicatorParameters(string reasonToClone, bool resetAllToMin = true, bool leaveNonSequencedAsCurrent = true) {
			//this.ScriptParametersByIdNonCloned = this.ScriptParametersById;
			SortedDictionary<int, ScriptParameter> scriptParameters_byIdClonedReset = new SortedDictionary<int, ScriptParameter>();
			foreach (int id in this.ScriptParametersById.Keys) {
				ScriptParameter sp = this.ScriptParametersById[id];
				ScriptParameter spClone = sp.Clone_asScriptParameter(reasonToClone, "SWITCHABLE_REUSABLE_CTX[" + this.ToString() + "]");
				if (resetAllToMin) {
					if (spClone.WillBeSequenced == false && leaveNonSequencedAsCurrent == true) {
						// don't reset to min
					} else {
						spClone.ValueCurrent = spClone.ValueMin;
					}
				}
				scriptParameters_byIdClonedReset.Add(id, spClone);
			}
			this.ScriptParametersById = scriptParameters_byIdClonedReset;

			Dictionary<string, List<IndicatorParameter>> indicatorParameters_byNameClonedReset = new Dictionary<string, List<IndicatorParameter>>();
			foreach (string indicatorName in this.IndicatorParametersByIndicatorName.Keys) {
				List<IndicatorParameter> iParams = this.IndicatorParametersByIndicatorName[indicatorName];
				List<IndicatorParameter> iParamsCloned = new List<IndicatorParameter>();
				indicatorParameters_byNameClonedReset.Add(indicatorName, iParamsCloned);
				foreach (IndicatorParameter iParam in iParams) {
					IndicatorParameter ipClone = iParam.Clone_asIndicatorParameter(reasonToClone, "SWITCHABLE_REUSABLE_CTX[" + this.ToString() + "]");
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
			this.IndicatorParametersByIndicatorName = indicatorParameters_byNameClonedReset;
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
				foreach (string indicatorName in ctxSequencerSequenced.IndicatorParametersByIndicatorName.Keys) {
					List<IndicatorParameter> ipsOpt  = ctxSequencerSequenced	.IndicatorParametersByIndicatorName[indicatorName];
					List<IndicatorParameter> ipsMine = this						.IndicatorParametersByIndicatorName[indicatorName];
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
					List<IndicatorParameter> ipsMine = this									.IndicatorParametersByIndicatorName[indicatorName];
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
				this.ReportersSnapshots.Add(reporterName, reporterActivated.CreateSnapshot_toStore_inScriptContext());
			}
			return this.ReportersSnapshots[reporterName];
		}
		public string SymbolScaleInterval_dataRangeForScriptContext_newName { get {
			string ret = this.Symbol + " " + this.ScaleInterval + " " + this.DataRange;
			return ret;
		} }
	}
}
