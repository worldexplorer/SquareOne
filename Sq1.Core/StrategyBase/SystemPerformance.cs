using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformance {
		public	ScriptExecutor				Executor			{ get; private set; }
		public	Bars						BarsBuyAndHold		{ get { return this.Bars; } }
		public	Bars						Bars				{ get {
				if (this.Executor.Bars == null) {
					string msg = "don't execute any SystemPerformance calculations before calling InitializeAndScan(): this.Executor.Bars == null";
					throw new Exception(msg);
				}
				return this.Executor.Bars;
			} }
		public	SystemPerformanceSlice		SlicesShortAndLong	{ get; private set; }
		public	SystemPerformanceSlice		SliceLong			{ get; private set; }
		public	SystemPerformanceSlice		SliceShort			{ get; private set; }
		public	SystemPerformanceSlice		SliceBuyHold		{ get; private set; }
		
		public	SortedDictionary<int, ScriptParameter>			ScriptParametersById_BuiltOnBacktestFinished						{ get; private set; }
		public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByName_BuiltOnBacktestFinished					{ get; private set; }
		public	SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished		{ get; private set; }
		public	string											ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished_AsString { get {
				SortedDictionary<string, IndicatorParameter> merged = this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished;
				if (merged == null) return "(NotOptimizedYet)";
				if (merged.Count == 0) return "(NoParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }


		public string						NetProfitRecoveryForScriptContextNewName	{ get {
				string netFormatted = Math.Round(this.SlicesShortAndLong.NetProfitForClosedPositionsBoth, 2).ToString();
				if (this.Bars != null) netFormatted = this.Bars.FormatValue(this.SlicesShortAndLong.NetProfitForClosedPositionsBoth);
				netFormatted = netFormatted.Replace(",", "");	// saving comma Net[4,072] will throw SPLITTER_COULDNT_RECOGNIZE_KEY and fail to restore ChartForm
				string ret = "Net[" + netFormatted +"]"
					+ " PF[" + this.SlicesShortAndLong.ProfitFactor + "]"
					+ " RF[" + this.SlicesShortAndLong.RecoveryFactor + "]";
				return ret;
			}
		}

		public SystemPerformance(ScriptExecutor scriptExecutor) {
			if (scriptExecutor == null) {
				string msg = "DONT_PASS_EMPTY_EXECUTOR SystemPerformance(scriptExecutor=null)";
				throw new Exception(msg);
			}

			this.Executor			= scriptExecutor;
			//if (this.Executor.Bars == null) {
			//	string msg = "we shouldn't execute strategy on this.Executor.Bars == null";
			//	throw new Exception(msg);
			//}

			this.SliceLong			= new SystemPerformanceSlice(SystemPerformancePositionsTracking.LongOnly,		"StatsForLongPositionsOnly");
			this.SliceShort			= new SystemPerformanceSlice(SystemPerformancePositionsTracking.ShortOnly,		"StatsForShortPositionsOnly");
			this.SlicesShortAndLong = new SystemPerformanceSlice(SystemPerformancePositionsTracking.LongAndShort,	"StatsForShortAndLongPositions");
			this.SliceBuyHold		= new SystemPerformanceSlice(SystemPerformancePositionsTracking.BuyAndHold,		"StatsForBuyHold");

			//CREATED_IN_this.BuildStatsOnBacktestFinished() ScriptAndIndicatorParameterClonesByName = new SortedDictionary<string, IndicatorParameter>();
		}
		public void Initialize() {
			this.SliceLong			.Initialize();
			this.SliceShort			.Initialize();
			this.SlicesShortAndLong	.Initialize();
			this.SliceBuyHold		.Initialize();
		}

		public void BuildStatsOnBacktestFinished() {
			if (this.Executor.ExecutionDataSnapshot == null) {
				string msg = "this.Executor.ExecutionDataSnapshot == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.ExecutionDataSnapshot.PositionsMaster == null) {
				string msg = "this.Executor.ExecutionDataSnapshot.PositionsMaster == null";
				Assembler.PopupException(msg);
				return;
			}

			// OVERKILL_OPTIMIZE_ME__HAPPENS_ONCE__NO_MULTIPLE_THREADS_INVOLVED__RIGHT?
			PositionList positionsClosedSafe = this.Executor.ExecutionDataSnapshot.PositionsMaster.Clone(this, "BuildStatsOnBacktestFinished(WAIT)");
			List<Position> positionsOpenSafe = this.Executor.ExecutionDataSnapshot.PositionsOpenNow.SafeCopy(this, "BuildStatsOnBacktestFinished(WAIT)");
			foreach (Position posOpen in positionsOpenSafe) {
				if (positionsClosedSafe.Contains(posOpen, this, "BuildStatsOnBacktestFinished(WAIT)") == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					continue;
				}
				positionsClosedSafe.Remove(posOpen, this, "BuildStatsOnBacktestFinished(WAIT)");
			}
			// at the end of backtest, I closed all positions, last may be still open 
			ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(null, null,
					//this.Executor.ExecutionDataSnapshot.PositionsOpenNow,
					this.Executor.ExecutionDataSnapshot.PositionsMaster,
					positionsClosedSafe,
					this.Executor.ExecutionDataSnapshot.PositionsOpenNow
				);
			using(pokeUnit_dontForgetToDispose) {
				int absorbedLong	= this.SliceLong			.BuildStatsOnBacktestFinished(pokeUnit_dontForgetToDispose);
				int absorbedShort	= this.SliceShort			.BuildStatsOnBacktestFinished(pokeUnit_dontForgetToDispose);
				int absorbedBoth	= this.SlicesShortAndLong	.BuildStatsOnBacktestFinished(pokeUnit_dontForgetToDispose);
				int absorbedBH		= this.SliceBuyHold			.BuildStatsOnBacktestFinished(pokeUnit_dontForgetToDispose);

				Strategy strategy = this.Executor.Strategy;
				Script script = strategy.Script;
				if (script.ScriptParametersById_ReflectedCached == null) {
					string msg = "CANT_GRAB_";
					Assembler.PopupException(msg);
					return;
				}
				//WRONG this.ScriptAndIndicatorParameterClonesByName.Clear();
				this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished = new SortedDictionary<string, IndicatorParameter>();
				this.ScriptParametersById_BuiltOnBacktestFinished = new SortedDictionary<int, ScriptParameter>();

				string pids = script.ScriptParametersAsString;
				foreach (ScriptParameter sp in script.ScriptParametersById_ReflectedCached.Values) {
					if (this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.ContainsKey(sp.Name)) {
						string msg = "WONT_ADD_ALREADY_IN_SYSTEM_PERFORMANCE_ScriptParameter[" + sp.Name + "]: " + pids;
						Assembler.PopupException(msg);
						continue;
					}
					ScriptParameter clone = sp.CloneAsScriptParameter("FOR_BuildStatsOnBacktestFinished");
					this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Add(sp.Name, clone);
					this.ScriptParametersById_BuiltOnBacktestFinished.Add(sp.Id, clone);
				}

				//foreach (IndicatorParameter ip in this.Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
				string iids = script.IndicatorParametersAsString;
				//foreach (IndicatorParameter ip in this.Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
				foreach (IndicatorParameter ip in script.IndicatorsParameters_ReflectedCached.Values) {
					if (this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.ContainsKey(ip.FullName)) {
						string msg = "WONT_ADD_ALREADY_IN_SYSTEM_PERFORMANCE_IndicatorParameter[" + ip.Name + "]: " + iids;
						Assembler.PopupException(msg);
						continue;
					}
					IndicatorParameter clone = ip.CloneAsIndicatorParameter("FOR_BuildStatsOnBacktestFinished#1");
					this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Add(ip.FullName, clone);
				}

				this.IndicatorParametersByName_BuiltOnBacktestFinished = new Dictionary<string, List<IndicatorParameter>>();
				ContextScript ctx = strategy.ScriptContextCurrent;
				foreach (string iParamName in ctx.IndicatorParametersByName.Keys) {
					List<IndicatorParameter> iParams = ctx.IndicatorParametersByName[iParamName];
					List<IndicatorParameter> iParamsCloned = new List<IndicatorParameter>();
					foreach (IndicatorParameter ip in iParams) {
						IndicatorParameter clone = ip.CloneAsIndicatorParameter("FOR_BuildStatsOnBacktestFinished#2");
						iParamsCloned.Add(clone);
					}
					this.IndicatorParametersByName_BuiltOnBacktestFinished.Add(iParamName, iParamsCloned);
				}
			}			
		}
		internal void BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(Position position) {
			if (this.Executor.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				string msg = "DONT_INVOKE_ME_DURING_BACKTEST__BuildStatsOnBacktestFinished()_ALREADY_DID_THIS_JOB";
				Assembler.PopupException(msg);
				return;
			}
			int absorbedOpenLong		= this.SliceLong			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenShort		= this.SliceShort			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenBoth		= this.SlicesShortAndLong	.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenBH			= this.SliceBuyHold			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
		}
		public void BuildIncremental_openPositionsUpdated_afterChartConsumedNewQuote_step2of3(PositionList positions) {
			int absorbedUpdatedLong		= this.SliceLong			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedShort	= this.SliceShort			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedBoth		= this.SlicesShortAndLong	.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedBH		= this.SliceBuyHold			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
		}
		internal void BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(Position position) {
			int absorbedClosedLong		= this.SliceLong			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedShort		= this.SliceShort			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedBoth		= this.SlicesShortAndLong	.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedBH		= this.SliceBuyHold			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
		}

		public SystemPerformance CloneForSequencer() {
			// Sequencer takes Clone with Slices ready to use; same (parent) SystemPerformance.Initialize() overwrites with new Slices,
			// while clone keeps pointers to old Slices => Sequencer is happy   
			return (SystemPerformance)base.MemberwiseClone();
		}
		//public SystemPerformance CloneForReporter() {
		//	// Sq1.Reporters.Performance needs full deep copy to maintain its own lists to avoid "Collection Modified Exceptions"
		//	// instead of making ScriptExecutor.DataSnapshot.PositionsMaster and everything synchronized (slows down adding by strategy?),
		//	// each report receives PokeUnit and messes up its own data at its own risk in its own address space
		//	SystemPerformance ret = (SystemPerformance)base.MemberwiseClone();
		//	ret.
		//	return ret;
		//}
		public override string ToString() {
			string msg = "HAS_MEANINFULL_VALUE_ONLY_AFTER int absorbedBH = this.SliceBuyHold.BuildStatsOnBacktestFinished(pokeUnit)";
			return this.NetProfitRecoveryForScriptContextNewName + " " + this.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished_AsString;
		}
	}
}
