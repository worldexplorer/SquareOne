using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformance {
		public ScriptExecutor				Executor			{ get; private set; }
		public Bars							BarsBuyAndHold		{ get { return this.Bars; } }
		public Bars							Bars				{ get {
				if (this.Executor.Bars == null) {
					string msg = "don't execute any SystemPerformance calculations before calling InitializeAndScan(): this.Executor.Bars == null";
					throw new Exception(msg);
				}
				return this.Executor.Bars;
			} }
		public SystemPerformanceSlice		SlicesShortAndLong	{ get; private set; }
		public SystemPerformanceSlice		SliceLong			{ get; private set; }
		public SystemPerformanceSlice		SliceShort			{ get; private set; }
		public SystemPerformanceSlice		SliceBuyHold		{ get; private set; }
		public SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParameterClonesByName;

		public string						EssentialsForScriptContextNewName	{ get {
				return "Net[" + this.SlicesShortAndLong.NetProfitForClosedPositionsBoth + "]"
				+ " PF[" + this.SlicesShortAndLong.ProfitFactor + "]"
				+ " RF[" + this.SlicesShortAndLong.RecoveryFactor + "]";
			} }

		public SystemPerformance(ScriptExecutor scriptExecutor) {
			if (scriptExecutor == null) {
				string msg = "DONT_PASS_EMPTY_EXECUTOR SystemPerformance(scriptExecutor=null)";
				throw new Exception(msg);
			}

			this.Executor			= scriptExecutor;
			//if (this.Executor.Bars == null) {
			//    string msg = "we shouldn't execute strategy on this.Executor.Bars == null";
			//    throw new Exception(msg);
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

			PositionList positionsClosed = this.Executor.ExecutionDataSnapshot.PositionsMaster.Clone();
			foreach (Position posOpen in this.Executor.ExecutionDataSnapshot.PositionsOpenNow.InnerList) {
				if (positionsClosed.Contains(posOpen) == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					continue;
				}
				positionsClosed.Remove(posOpen);
			}
			// at the end of backtest, I closed all positions, last may be still open 
			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(null, null,
					//this.Executor.ExecutionDataSnapshot.PositionsOpenNow,
					this.Executor.ExecutionDataSnapshot.PositionsMaster,
					positionsClosed,
					this.Executor.ExecutionDataSnapshot.PositionsOpenNow
				);
			
			int absorbedLong	= this.SliceLong			.BuildStatsOnBacktestFinished(pokeUnit);
			int absorbedShort	= this.SliceShort			.BuildStatsOnBacktestFinished(pokeUnit);
			int absorbedBoth	= this.SlicesShortAndLong	.BuildStatsOnBacktestFinished(pokeUnit);
			int absorbedBH		= this.SliceBuyHold			.BuildStatsOnBacktestFinished(pokeUnit);

			if (this.Executor.Strategy.Script.ScriptParametersById == null) {
				string msg = "CANT_GRAB_";
				Assembler.PopupException(msg);
				return;
			}
			//WRONG this.ScriptAndIndicatorParameterClonesByName.Clear();
			this.ScriptAndIndicatorParameterClonesByName = new SortedDictionary<string, IndicatorParameter>();

			string pids = this.Executor.Strategy.Script.ScriptParametersByIdAsString;
			foreach (ScriptParameter sp in this.Executor.Strategy.Script.ScriptParametersById.Values) {
				if (this.ScriptAndIndicatorParameterClonesByName.ContainsKey(sp.Name)) {
					string msg = "WONT_ADD_ALREADY_IN_SYSTEM_PERFORMANCE_ScriptParameter[" + sp.Name + "]: " + pids;
					Assembler.PopupException(msg);
					continue;
				}
				this.ScriptAndIndicatorParameterClonesByName.Add(sp.Name, sp.Clone());
			}

			//foreach (IndicatorParameter ip in this.Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
			string iids = this.Executor.Strategy.Script.IndicatorParametersAsString;
			foreach (IndicatorParameter ip in this.Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
				if (this.ScriptAndIndicatorParameterClonesByName.ContainsKey(ip.FullName)) {
					string msg = "WONT_ADD_ALREADY_IN_SYSTEM_PERFORMANCE_IndicatorParameter[" + ip.Name + "]: " + iids;
					Assembler.PopupException(msg);
					continue;
				}
				this.ScriptAndIndicatorParameterClonesByName.Add(ip.FullName, ip.Clone());
			}
		}
		internal void BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(Position position) {
			if (this.Executor.Backtester.IsBacktestingNow) {
				string msg = "DONT_INVOKE_ME_DURING_BACKTEST__BuildStatsOnBacktestFinished()_ALREADY_DID_THIS_JOB";
				Assembler.PopupException(msg);
				return;
			}
			int absorbedOpenLong	= this.SliceLong			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenShort	= this.SliceShort			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenBoth	= this.SlicesShortAndLong	.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
			int absorbedOpenBH		= this.SliceBuyHold			.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(position);
		}
		internal void BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(PositionList positions) {
			int absorbedUpdatedLong		= this.SliceLong			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedShort	= this.SliceShort			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedBoth		= this.SlicesShortAndLong	.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
			int absorbedUpdatedBH		= this.SliceBuyHold			.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(positions);
		}
		internal void BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(Position position) {
			int absorbedClosedLong	= this.SliceLong			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedShort	= this.SliceShort			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedBoth	= this.SlicesShortAndLong	.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
			int absorbedClosedBH	= this.SliceBuyHold			.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(position);
		}

		public SystemPerformance CloneForOptimizer() {
			// Optimizer takes Clone with Slices ready to use; same (parent) SystemPerformance.Initialize() overwrites with new Slices,
			// while clone keeps pointers to old Slices => Optimizer is happy   
			return (SystemPerformance)base.MemberwiseClone();
		}
		//public SystemPerformance CloneForReporter() {
		//    // Sq1.Reporters.Performance needs full deep copy to maintain its own lists to avoid "Collection Modified Exceptions"
		//    // instead of making ScriptExecutor.DataSnapshot.PositionsMaster and everything synchronized (slows down adding by strategy?),
		//    // each report receives PokeUnit and messes up its own data at its own risk in its own address space
		//    SystemPerformance ret = (SystemPerformance)base.MemberwiseClone();
		//    ret.
		//    return ret;
		//}
	}
}
