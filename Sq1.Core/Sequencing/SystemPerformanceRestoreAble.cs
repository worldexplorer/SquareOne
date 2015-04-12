using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class SystemPerformanceRestoreAble : NamedObjectJsonSerializable {
		[JsonIgnore]	public	bool		DontForgetEverythingNonIgnoredIsSerialized;

		[JsonProperty]	public	SortedDictionary<int, ScriptParameter>			ScriptParametersById_BuiltOnBacktestFinished	{ get; private set; }
		[JsonProperty]	public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByName_BuiltOnBacktestFinished	{ get; private set; }
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
		
		[JsonProperty]	public	string	NetProfitRecovery				{ get; private set; }
		[JsonProperty]	public	string	StrategyName					{ get; private set; }
		// if PriceFormat.IsNullOrEmpty, grab from RepositorySymbolInfo.FindSymbolInfoOrNew(first.Symbol)
		[JsonProperty]	public	string	Symbol							{ get; private set; }
		[JsonProperty]	public	string	SymbolScaleIntervalDataRange 	{ get; private set; }
		
		[JsonProperty]	public	string	PriceFormat						{ get; private set; }

		[JsonProperty]	public	int		PositionsCountBoth				{ get; private set; }
		[JsonProperty]	public	double	AvgProfitBoth					{ get; private set; }
		[JsonProperty]	public	double	NetProfitForClosedPositionsBoth	{ get; private set; }
		[JsonProperty]	public	double	WinLossRatio					{ get; private set; }
		[JsonProperty]	public	double	ProfitFactor					{ get; private set; }
		[JsonProperty]	public	double	RecoveryFactor					{ get; private set; }
		[JsonProperty]	public	double	MaxDrawDown						{ get; private set; }
		[JsonProperty]	public	double	MaxConsecWinners				{ get; private set; }
		[JsonProperty]	public	double	MaxConsecLosers					{ get; private set; }
		
		[JsonProperty]	public	string	OptimizationIterationName		{ get; private set; }
		[JsonProperty]	public	int		OptimizationIterationSerno;		//ASSIGNED_FROM_ABROAD { get; private set; }

		public SystemPerformanceRestoreAble() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
		}
		public SystemPerformanceRestoreAble(SystemPerformance sysPerfBacktestResult) {
			if (sysPerfBacktestResult == null) {
				Assembler.PopupException("DONT_INVOKE_ME_WITH_NULL AVOIDING_NPE");
				return;
			}
			this.ScriptParametersById_BuiltOnBacktestFinished						= sysPerfBacktestResult.ScriptParametersById_BuiltOnBacktestFinished;
			this.IndicatorParametersByName_BuiltOnBacktestFinished					= sysPerfBacktestResult.IndicatorParametersByName_BuiltOnBacktestFinished;
			this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished	= sysPerfBacktestResult.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				
			this.NetProfitRecovery				= sysPerfBacktestResult.NetProfitRecoveryForScriptContextNewName;
			this.StrategyName					= sysPerfBacktestResult.Executor.StrategyName;
			//this.ContextScript				= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent;
			this.StrategyName					= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.Symbol;
			this.SymbolScaleIntervalDataRange	= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			base.Name							= this.SymbolScaleIntervalDataRange;
			this.OptimizationIterationName		= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.OptimizationIterationName;
			this.OptimizationIterationSerno		= sysPerfBacktestResult.Executor.Strategy.ScriptContextCurrent.OptimizationIterationSerno;

			this.PriceFormat = sysPerfBacktestResult.Bars.SymbolInfo.PriceFormat;
			
			this.PositionsCountBoth	= sysPerfBacktestResult.SlicesShortAndLong.PositionsCountBoth;
			this.AvgProfitBoth		= sysPerfBacktestResult.SlicesShortAndLong.AvgProfitBoth;
			this.NetProfitForClosedPositionsBoth = sysPerfBacktestResult.SlicesShortAndLong.NetProfitForClosedPositionsBoth;
			this.WinLossRatio = sysPerfBacktestResult.SlicesShortAndLong.WinLossRatio;
			this.ProfitFactor		= sysPerfBacktestResult.SlicesShortAndLong.ProfitFactor;
			this.RecoveryFactor		= sysPerfBacktestResult.SlicesShortAndLong.RecoveryFactor;
			this.MaxDrawDown		= sysPerfBacktestResult.SlicesShortAndLong.MaxDrawDown;
			this.MaxConsecWinners	= sysPerfBacktestResult.SlicesShortAndLong.MaxConsecWinners;
			this.MaxConsecLosers	= sysPerfBacktestResult.SlicesShortAndLong.MaxConsecLosers;
		}
	}
}
