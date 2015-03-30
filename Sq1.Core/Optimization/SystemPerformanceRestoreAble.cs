using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public class SystemPerformanceRestoreAble : NamedObjectJsonSerializable {
		[JsonIgnore]	public	bool		DontForgetEverythingNonIgnoredIsSerialized;

		[JsonProperty]	public	SortedDictionary<int, ScriptParameter>			ScriptParametersById_BuiltOnBacktestFinished;
		[JsonProperty]	public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByName_BuiltOnBacktestFinished;
		[JsonProperty]	public	SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
		
		[JsonProperty]	public	string	NetProfitRecovery;
		[JsonProperty]	public	string	StrategyName;
		[JsonProperty]	public	string	SymbolScaleIntervalDataRange;
		
		// REMOVE_NON-FORMATTED_AFTER_YOU_FIND_OUT_SORTING_IN_OBJECTLISTVIEW
		[JsonProperty]	public	string	Format;
		[JsonProperty]	public	double	NetProfitForClosedPositionsBoth;
		[JsonProperty]	public	string	NetProfitForClosedPositionsBothFormatted;
		[JsonIgnore]	private	double	PositionsCountBoth;
		[JsonProperty]	public	string	PositionsCountBothFormatted;
		[JsonIgnore]	private	double	AvgProfitBoth;
		[JsonProperty]	public	string	AvgProfitBothFormatted;
		[JsonIgnore]	private	double	WinLossRatio;
		[JsonProperty]	public	string	WinLossRatioFormatted;
		[JsonProperty]	public	double	ProfitFactor;
		[JsonProperty]	public	string	ProfitFactorFormatted;
		[JsonIgnore]	private	double	RecoveryFactor;
		[JsonProperty]	public	string	RecoveryFactorFormatted;
		[JsonIgnore]	private	double	MaxDrawDown;
		[JsonProperty]	public	string	MaxDrawDownFormatted;
		[JsonIgnore]	private	double	MaxConsecWinners;
		[JsonProperty]	public	string	MaxConsecWinnersFormatted;
		[JsonIgnore]	private	double	MaxConsecLosers;
		[JsonProperty]	public	string	MaxConsecLosersFormatted;
		
		public SystemPerformanceRestoreAble() {
			string msg = "IF_THIS_CONSTRUCTOR_IS_ABSENT_JSON_CREATOR_WILL_INVOKE_THE_OTHER_WITH_NULL_PARAMETER. WHY_NOT_USE_DEFAULT(TYPE)???";
		}
		public SystemPerformanceRestoreAble(SystemPerformance sysPerfBacktestResult) {
			if (sysPerfBacktestResult == null) {
				Assembler.PopupException("DONT_INVOKE_ME_WITH_NULL AVOIDING_NPE");
				return;
			}
			this.ScriptParametersById_BuiltOnBacktestFinished		= sysPerfBacktestResult.ScriptParametersById_BuiltOnBacktestFinished;
			this.IndicatorParametersByName_BuiltOnBacktestFinished	= sysPerfBacktestResult.IndicatorParametersByName_BuiltOnBacktestFinished;
			this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished = sysPerfBacktestResult.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				
			this.NetProfitRecovery				= sysPerfBacktestResult.NetProfitRecoveryForScriptContextNewName;
			this.StrategyName					= sysPerfBacktestResult.Executor.StrategyName;
			//this.ContextScript				= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent;
			this.SymbolScaleIntervalDataRange	= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			base.Name							= this.SymbolScaleIntervalDataRange;
			
			this.Format = sysPerfBacktestResult.Bars.SymbolInfo.PriceFormat;
			
			this.NetProfitForClosedPositionsBoth = sysPerfBacktestResult.SlicesShortAndLong.NetProfitForClosedPositionsBoth;
			this.NetProfitForClosedPositionsBothFormatted = this.NetProfitForClosedPositionsBoth.ToString(this.Format);
			
			this.PositionsCountBoth = sysPerfBacktestResult.SlicesShortAndLong.PositionsCountBoth;
			this.PositionsCountBothFormatted = this.PositionsCountBoth.ToString();
			
			this.AvgProfitBoth = sysPerfBacktestResult.SlicesShortAndLong.AvgProfitBoth;
			this.AvgProfitBothFormatted = this.AvgProfitBoth.ToString(this.Format);
			
			this.WinLossRatio = sysPerfBacktestResult.SlicesShortAndLong.WinLossRatio;
			this.WinLossRatioFormatted = this.WinLossRatio.ToString(this.Format);
			
			this.ProfitFactor = sysPerfBacktestResult.SlicesShortAndLong.ProfitFactor;
			this.ProfitFactorFormatted = this.ProfitFactor.ToString();
			
			this.RecoveryFactor = sysPerfBacktestResult.SlicesShortAndLong.RecoveryFactor;
			this.RecoveryFactorFormatted = this.RecoveryFactor.ToString();
			
			this.MaxDrawDown = sysPerfBacktestResult.SlicesShortAndLong.MaxDrawDown;
			this.MaxDrawDownFormatted = this.MaxDrawDown.ToString(this.Format);
			
			this.MaxConsecWinners = sysPerfBacktestResult.SlicesShortAndLong.MaxConsecWinners;
			this.MaxConsecWinnersFormatted = this.MaxConsecWinners.ToString();
			
			this.MaxConsecLosers = sysPerfBacktestResult.SlicesShortAndLong.MaxConsecLosers;
			this.MaxConsecLosersFormatted = this.MaxConsecLosers.ToString();
		}
	}
}
