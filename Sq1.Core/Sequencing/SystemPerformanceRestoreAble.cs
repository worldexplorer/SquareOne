using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class SystemPerformanceRestoreAble : KPIs {
		public const string WATERLINE = "WaterlineNoKPIs";

		[JsonIgnore]	public	bool		DontForgetEverythingNonIgnoredIsSerialized;

		[JsonIgnore]	public	SortedDictionary<int, ScriptParameter>			ScriptParametersById_BuiltOnBacktestFinished					{ get; private set; }
		[JsonIgnore]	public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByName_BuiltOnBacktestFinished				{ get; private set; }
		[JsonProperty]	public	SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished	{ get; private set; }
		[JsonProperty]	public	string											ParametersAsString { get {
				SortedDictionary<string, IndicatorParameter> merged = this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				if (merged.Count == 0) return "(NoParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }
		
		[JsonProperty]	public	string	NetProfitRecovery			{ get; private set; }
		[JsonIgnore]	public	bool	IsSubset					{ get { return this.ToString().Contains(WATERLINE); } }
		[JsonProperty]	public	string	PriceFormat					{ get; private set; }
		[JsonProperty]	public	string	SequenceIterationName		{ get; private set; }
		[JsonProperty]	public	int		SequenceIterationSerno;		//ASSIGNED_FROM_ABROAD { get; private set; }

		//having KPIsCumulativeByDateIncreasing allows {Subset}s => CorrelatorControl can display WalkForward and propagate back to Sequencer
		[JsonProperty]	public	SortedDictionary<DateTime, KPIs>	KPIsCumulativeByDateIncreasing	{ get; private set; }
		
		public SystemPerformanceRestoreAble() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
			KPIsCumulativeByDateIncreasing	= new SortedDictionary<DateTime, KPIs>();
		}
		public SystemPerformanceRestoreAble(SystemPerformance sysPerfBacktestResult) : this() {
			if (sysPerfBacktestResult == null) {
				Assembler.PopupException("DONT_INVOKE_ME_WITH_NULL AVOIDING_NPE PARAMETERLESS_CTOR_MUST_BE_PUBLIC_SystemPerformanceRestoreAble(){}_FOR_DESERIALIZATION");
				return;
			}
			this.ScriptParametersById_BuiltOnBacktestFinished						= sysPerfBacktestResult.ScriptParametersById_BuiltOnBacktestFinished;
			this.IndicatorParametersByName_BuiltOnBacktestFinished					= sysPerfBacktestResult.IndicatorParametersByName_BuiltOnBacktestFinished;
			this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished	= sysPerfBacktestResult.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				
			this.NetProfitRecovery		= sysPerfBacktestResult.NetProfitRecoveryForScriptContextNewName;
			this.SequenceIterationName	= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.SequenceIterationName;
			this.SequenceIterationSerno	= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.SequenceIterationSerno;

			base.ReasonToExist			= "SEQUENCED_" + this.SequenceIterationName;

			this.PriceFormat			= sysPerfBacktestResult.Bars.SymbolInfo.PriceFormat;
			
			base.PositionsCount			= sysPerfBacktestResult.SlicesShortAndLong.PositionsCount;
			base.PositionAvgProfit		= sysPerfBacktestResult.SlicesShortAndLong.PositionAvgProfitBoth;
			base.NetProfit				= sysPerfBacktestResult.SlicesShortAndLong.NetProfitForClosedPositionsBoth;
			base.WinLossRatio			= sysPerfBacktestResult.SlicesShortAndLong.WinLossRatio;
			base.ProfitFactor			= sysPerfBacktestResult.SlicesShortAndLong.ProfitFactor;
			base.RecoveryFactor			= sysPerfBacktestResult.SlicesShortAndLong.RecoveryFactor;
			base.MaxDrawDown			= sysPerfBacktestResult.SlicesShortAndLong.MaxDrawDown;
			base.MaxConsecWinners		= sysPerfBacktestResult.SlicesShortAndLong.MaxConsecWinners;
			base.MaxConsecLosers		= sysPerfBacktestResult.SlicesShortAndLong.MaxConsecLosers;
			
			// for %backtest / walkForward analysis
			this.KPIsCumulativeByDateIncreasing				= sysPerfBacktestResult.SlicesShortAndLong.KPIsCumulativeByDate;

			// Added_[JsonIgnore] to:
			// [JsonIgnore]	public	SortedDictionary<int, ScriptParameter>			ScriptParametersById_BuiltOnBacktestFinished					{ get; private set; }
			// [JsonIgnore]	public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByName_BuiltOnBacktestFinished				{ get; private set; }
		}

		[JsonProperty]	public	DateTime	KPIsCumulativeDateFirst_DateTimeMinUnsafe		{ get {
			DateTime ret = DateTime.MinValue;
			if (this.KPIsCumulativeByDateIncreasing == null) {
				string msg = "DESERIALIZED_WITH_KPIsCumulativeByDateIncreasing_NULL";
				return ret;
			}
			if (this.KPIsCumulativeByDateIncreasing.Count > 0) {
				ret = new List<DateTime>(this.KPIsCumulativeByDateIncreasing.Keys)[0];
			}
			return ret;
		} }
		[JsonProperty]	public	DateTime	KPIsCumulativeDateLast_DateTimeMaxUnsafe		{ get {
			DateTime ret = DateTime.MaxValue;
			if (this.KPIsCumulativeByDateIncreasing == null) {
				string msg = "DESERIALIZED_WITH_KPIsCumulativeByDateIncreasing_NULL";
				return ret;
			}
			if (this.KPIsCumulativeByDateIncreasing.Count > 0) {
				int lastIndex = this.KPIsCumulativeByDateIncreasing.Count - 1;
				ret = new List<DateTime>(this.KPIsCumulativeByDateIncreasing.Keys)[lastIndex];
			}
			return ret;
		} }

		internal SystemPerformanceRestoreAble CreateSubsetBelowWaterline__nullUnsafe(DateTime waterlineDateTime) {
			KPIs lastBelowWaterlineFound = null;
			foreach (DateTime eachDatePositionClosed in this.KPIsCumulativeByDateIncreasing.Keys) {
				lastBelowWaterlineFound = this.KPIsCumulativeByDateIncreasing[eachDatePositionClosed];
				if (eachDatePositionClosed > waterlineDateTime) break;
			}
			if (lastBelowWaterlineFound == null) {
				string msg = "WATERLINE_NOT_FOUND_AMONG_KPIsCumulativeByDate["
					+ this.KPIsCumulativeDateFirst_DateTimeMinUnsafe + "]...["
					+ this.KPIsCumulativeDateLast_DateTimeMaxUnsafe + "] waterlineDateTime=[" + waterlineDateTime + "]";
				//MORE_MEANINGFUL_MESSAGE_UPSTACK Assembler.PopupException(msg);
				return null;
			}
			SystemPerformanceRestoreAble ret = this.CloneForSubset("Below" + WATERLINE + "("
				+ waterlineDateTime.ToString(Assembler.DateTimeFormatToMinutes) + ")");
			ret.AbsorbFrom(lastBelowWaterlineFound);
			if (this.NetProfit == ret.NetProfit && ret.NetProfit > 0) {	// inline test
				TimeSpan lesser = this.KPIsCumulativeDateLast_DateTimeMaxUnsafe.Subtract(waterlineDateTime);
				string msg = "MY_OWN_SUBSET_MUST_HAVE_DIFFERENT_NUMBERS_FOR[" + lesser.ToString() + "]lesser";
				Assembler.PopupException(msg + this.NetProfitRecovery, null, false);
			}
			return ret;
		}
		internal SystemPerformanceRestoreAble CreateSubsetAboveWaterline__nullUnsafe(DateTime waterlineDateTime) {
			SystemPerformanceRestoreAble ret = this.CloneForSubset("Above" + WATERLINE + "("
				+ waterlineDateTime.ToString(Assembler.DateTimeFormatToMinutes) + ")");
			SystemPerformanceRestoreAble lastBelowWaterlineFound = this.CreateSubsetBelowWaterline__nullUnsafe(waterlineDateTime);
			if (lastBelowWaterlineFound == null) {
				return null;		//MORE_MEANINGFUL_MESSAGE_UPSTACK
			}
			KPIs cloneToSubtract = lastBelowWaterlineFound.Clone();
			cloneToSubtract.Invert();
			DateTime dateLast = this.KPIsCumulativeDateLast_DateTimeMaxUnsafe;
			if (dateLast == DateTime.MaxValue) {
				string msg = "I_REFUSE_TO_GET_LAST_KPIs__this.KPIsCumulativeDateLast_DateTimeMaxUnsafe";
				Assembler.PopupException(msg);
				return null;
			}
			KPIs lastCumulativeMinusWaterline_Clone = this.KPIsCumulativeByDateIncreasing[dateLast].Clone();
			lastCumulativeMinusWaterline_Clone.AddKPIs(cloneToSubtract);
			ret.AbsorbFrom(lastCumulativeMinusWaterline_Clone);
			if (this.NetProfit == ret.NetProfit && ret.NetProfit > 0) {	// inline test
				string msg = "MY_OWN_SUBSET_MUST_HAVE_DIFFERENT_NUMBERS";
				Assembler.PopupException(msg + this.NetProfitRecovery, null, false);
			}
			return ret;
		}
		public SystemPerformanceRestoreAble CloneForSubset(string reasonToExist) {
			if (this.KPIsCumulativeByDateIncreasing == null) {
				string msg = "I_REFUSE_TO_CREATE_SUBSET__I_AM_A_SUBSET_MYSELF__ASK_PARENT_TO_SPAWN_ANOTHER_SUBSET_INSTEAD_OF_ME";
				Assembler.PopupException(msg);
				return null;
			}
			SystemPerformanceRestoreAble ret = this.Clone(reasonToExist + "_CLONED_FOR_SUBSET");
			// reset to disable creating a subset from a subset; ask parent to spawn another subset instead
			ret.KPIsCumulativeByDateIncreasing = null;
			return ret;
		}
		public SystemPerformanceRestoreAble Clone(string reasonToExist) {
			SystemPerformanceRestoreAble ret = (SystemPerformanceRestoreAble) base.MemberwiseClone();
			ret.ReasonToExist = reasonToExist;
			//this.KPIsCumulativeByDate = new SortedDictionary<DateTime, KPIs>();
			return ret;
		}
		public void CheckPositionsCountMustIncreaseOnly() {
			int positionsCountMustIncreaseOnly = 0;
			DateTime dateCantDecrease = DateTime.MinValue;

			if (this.KPIsCumulativeByDateIncreasing == null) {
				if (this.IsSubset) {
					string msg = "NOT_AN_ERROR__BACKTESTS_IN_SUBSET_WILL_NOT_HAVE_KPIS [" + this.ToString() + "].KPIsCumulativeByDateIncreasing=null";
					//Assembler.PopupException(msg, null, false);
				} else {
					string msg = "BACKTESTS_MUST_HAVE_NON_NULL_KPIS [" + this.ToString() + "].KPIsCumulativeByDateIncreasing=null";
					Assembler.PopupException(msg);
				}
				return;
			}

			foreach (DateTime datePositionClosed in this.KPIsCumulativeByDateIncreasing.Keys) {
				if (dateCantDecrease >= datePositionClosed) {
					string msg = "DATE_POSITION_CLOSED_CANT_DECREASE [" + dateCantDecrease + "] >= [" + datePositionClosed + "]";
					Assembler.PopupException(msg);
				}
				dateCantDecrease = datePositionClosed;

				bool dateExists = this.KPIsCumulativeByDateIncreasing.ContainsKey(datePositionClosed);
				if (dateExists == false) {
					return;
				}

				KPIs KPIs = this.KPIsCumulativeByDateIncreasing[datePositionClosed];
				if (positionsCountMustIncreaseOnly > 0 && positionsCountMustIncreaseOnly >= KPIs.PositionsCount) {
					string msg = "POSITIONS_COUNT_CANT_DECREASE [" + positionsCountMustIncreaseOnly + "] >= [" + KPIs.PositionsCount + "]";
					Assembler.PopupException(msg);
				}
			}
		}

		internal int Ensure_OnBacktestFinisheds_AreRebuiltAfterDeserialization() {
			int ret = 0;
			if (this.ScriptParametersById_BuiltOnBacktestFinished != null || this.IndicatorParametersByName_BuiltOnBacktestFinished != null) {
				string msg = "ALREADY_RESTORED";
				return ret;
			}
			this.ScriptParametersById_BuiltOnBacktestFinished		= new SortedDictionary<int, ScriptParameter>();
			this.IndicatorParametersByName_BuiltOnBacktestFinished	= new Dictionary<string, List<IndicatorParameter>>();
			foreach (IndicatorParameter indicatorOrScriptParameter in this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Values) {
				ScriptParameter scriptParameter = indicatorOrScriptParameter as ScriptParameter;
				if (scriptParameter != null) {
					this.ScriptParametersById_BuiltOnBacktestFinished.Add(scriptParameter.Id, scriptParameter);
				} else {
					string indicatorName = indicatorOrScriptParameter.IndicatorName;
					if (this.IndicatorParametersByName_BuiltOnBacktestFinished.ContainsKey(indicatorName) == false) {
						this.IndicatorParametersByName_BuiltOnBacktestFinished.Add(indicatorName, new List<IndicatorParameter>());
					}
					List<IndicatorParameter> parametersForIndicator = this.IndicatorParametersByName_BuiltOnBacktestFinished[indicatorName];
					parametersForIndicator.Add(indicatorOrScriptParameter);
				}
				ret++;
			}
			return ret;
		}
	}
}
