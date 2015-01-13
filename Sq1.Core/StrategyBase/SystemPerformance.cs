using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformance {
		public ScriptExecutor								Executor			{ get; private set; }
		public Bars											Bars				{ get {
				if (this.Executor.Bars == null) {
					string msg = "don't execute any SystemPerformance calculations before calling InitializeAndScan(): this.Executor.Bars == null";
					throw new Exception(msg);
				}
				return this.Executor.Bars;
			} }
		public Bars											BenchmarkSymbolBars	{ get { return this.Bars; } }

		public SystemPerformanceSlice						SlicesShortAndLong	{ get; private set; }
		public SystemPerformanceSlice						SliceLong			{ get; private set; }
		public SystemPerformanceSlice						SliceShort			{ get; private set; }
		public SystemPerformanceSlice						SliceBuyHold		{ get; private set; }
		
		public SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParameterClonesByName;


		//cumulativeProfitDollar,Percent+CalcEachStreamingUpdate should be moved into SystemPerformanceSlice for other Reporters to be used in their drawings
		Dictionary<Position, double> cumulativeProfitDollar;
		Dictionary<Position, double> cumulativeProfitPercent;



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
			this.SliceLong			= new SystemPerformanceSlice(SystemPerformancePositionsTracking.LongOnly,		"StatsForLongPositionsOnly");
			this.SliceShort			= new SystemPerformanceSlice(SystemPerformancePositionsTracking.ShortOnly,		"StatsForShortPositionsOnly");
			this.SlicesShortAndLong = new SystemPerformanceSlice(SystemPerformancePositionsTracking.LongAndShort,	"StatsForShortAndLongPositions");
			this.SliceBuyHold		= new SystemPerformanceSlice(SystemPerformancePositionsTracking.BuyAndHold,		"StatsForBuyHold");

			//CREATED_IN_this.BuildStatsOnBacktestFinished() ScriptAndIndicatorParameterClonesByName = new SortedDictionary<string, IndicatorParameter>();

			this.cumulativeProfitDollar = new Dictionary<Position, double>();
			this.cumulativeProfitPercent = new Dictionary<Position, double>();
		}
		public void Initialize() {
			//if (this.Executor.Bars == null) {
			//    string msg = "we shouldn't execute strategy on this.Executor.Bars == null";
			//    throw new Exception(msg);
			//}

			this.SliceLong			.Initialize();
			this.SliceShort			.Initialize();
			this.SlicesShortAndLong	.Initialize();
			this.SliceBuyHold		.Initialize();
		}

		public void BuildStatsOnBacktestFinished(List<Position> positionsMaster = null) {
			//// Dictionary<U,V> has no .AsReadonly() (List<T> has it)
			//var positionsMasterByEntryBarSafeCopy = this.Executor.ExecutionDataSnapshot.PositionsMasterByEntryBar;
			//var positionsMasterByExitBarSafeCopy = this.Executor.ExecutionDataSnapshot.PositionsMasterByExitBar;

			//this.DEBUGGING_PositionsMaster = positionsMaster;
			//this.DEBUGGING_PositionsMasterByEntryBarSafeCopy = positionsMasterByEntryBarSafeCopy;
			//this.DEBUGGING_PositionsMasterByExitBarSafeCopy = positionsMasterByExitBarSafeCopy; 
			
			//this.SliceLong.BuildStatsOnBacktestFinished(positionsMasterByEntryBarSafeCopy, positionsMasterByExitBarSafeCopy);
			//this.SliceShort.BuildStatsOnBacktestFinished(positionsMasterByEntryBarSafeCopy, positionsMasterByExitBarSafeCopy);
			//this.SlicesShortAndLong.BuildStatsOnBacktestFinished(positionsMasterByEntryBarSafeCopy, positionsMasterByExitBarSafeCopy);
			//this.SliceBuyHold.BuildStatsOnBacktestFinished(positionsMasterByEntryBarSafeCopy, positionsMasterByExitBarSafeCopy);
			
			if (positionsMaster == null) {
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
				positionsMaster = this.Executor.ExecutionDataSnapshot.PositionsMaster;
			}

			List<Position> positionsClosed = new List<Position>(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			foreach (Position posOpen in this.Executor.ExecutionDataSnapshot.PositionsOpenNow) {
				if (positionsClosed.Contains(posOpen) == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					continue;
				}
				positionsClosed.Remove(posOpen);
			}
			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(null, null,
					this.Executor.ExecutionDataSnapshot.PositionsOpenNow,
					positionsClosed);
			Dictionary<int, List<Position>> posByEntry	= pokeUnit.PositionsOpenedByBarFilled;
			Dictionary<int, List<Position>> posByExit	= pokeUnit.PositionsClosedByBarFilled;
			int absorbedLong	= this.SliceLong			.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedShort	= this.SliceShort			.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedBoth	= this.SlicesShortAndLong	.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedBH		= this.SliceBuyHold			.BuildStatsAfterExec(posByEntry, posByExit);

			double cumProfitDollar = 0;
			double cumProfitPercent = 0;
			this.cumulativeProfitDollar.Clear();
			this.cumulativeProfitPercent.Clear();
			SystemPerformanceSlice both = this.SlicesShortAndLong;
			foreach (Position pos in both.PositionsImTrackingReadonly) {
				if (this.cumulativeProfitDollar.ContainsKey(pos)) {
					string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
						+ both + "].PositionsImTracking contains duplicate " + pos;
					Assembler.PopupException(msg);
					continue;
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;
				this.cumulativeProfitDollar.Add(pos, cumProfitDollar);
				this.cumulativeProfitPercent.Add(pos, cumProfitPercent);
			}
			
			
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

		internal void BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			if (this.Executor.Backtester.IsBacktestingNow) {
				string msg = "DONT_INVOKE_ME_DURING_BACKTEST__BuildStatsOnBacktestFinished()_ALREADY_DID_THIS_JOB";
				Assembler.PopupException(msg);
				return;
			}
			Dictionary<int, List<Position>> posByEntry	= pokeUnit.PositionsOpenedByBarFilled;
			Dictionary<int, List<Position>> posByExit	= pokeUnit.PositionsClosedByBarFilled;
			int absorbedOpenLong	= this.SliceLong			.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedOpenShort	= this.SliceShort			.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedOpenBoth	= this.SlicesShortAndLong	.BuildStatsAfterExec(posByEntry, posByExit);
			int absorbedOpenBH		= this.SliceBuyHold			.BuildStatsAfterExec(posByEntry, posByExit);
			if (absorbedOpenBoth != pokeUnit.PositionsOpened.Count) {
				string msg = pokeUnit.PositionsOpened.Count + " POSITIONS_NOT_ABSORBED_BY " + this.SlicesShortAndLong.ToString() + " HOPE_SLICE_BOTH_WILL_RECEIVE_IT";
				Assembler.PopupException(msg);
			}
		}
		internal void BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(List<Position> positionsUpdatedDueToStreamingNewQuote) {
			int earliestIndexUpdated = -1;
			SystemPerformanceSlice both = this.SlicesShortAndLong;
			foreach (Position pos in positionsUpdatedDueToStreamingNewQuote) {
				int posIndex = both.PositionsImTrackingReadonly.IndexOf(pos);
				if (posIndex == -1) {
					string msg3 = "IS_IT_STILL_RUNNING_IN_ANOTHER_THREAD??___YOU_DIDNT_INVOKE_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3_WITH_THIS_POSITION???";
					Assembler.PopupException(msg3, null, false);
				}
				if (earliestIndexUpdated == -1) {
					earliestIndexUpdated = posIndex;
					continue;
				}
				if (earliestIndexUpdated >= posIndex) continue;
				earliestIndexUpdated = posIndex;
			}
			if (earliestIndexUpdated == -1) {
				string msg2 = "NO_POSITIONS_UPDATED_NO_NEED_TO_RECALCULATE_ANYTHING";
				if (positionsUpdatedDueToStreamingNewQuote.Count > 0) {
					//this.rebuildOLVproperly();
				}
				return;
			}
			string msg = "POSITIONS_UPDATED_SO_I_RECALCULATE_FROM_EARLIEST_INDEX_UP_TO_ZERO";
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;

			//for (int i = earliestIndexUpdated; i >= 0; i--) {
			//    Position pos = this.positionsAllReversedCached[i];
			//    if (i == earliestIndexUpdated && i < this.positionsAllReversedCached.Count) {
			//        Position posPrev = this.positionsAllReversedCached[i + 1];
			//        if (this.cumulativeProfitDollar.ContainsKey(posPrev)) {
			//            cumProfitDollar = this.cumulativeProfitDollar[posPrev];
			//        } else {
			//            string msg1 = "REPORTERS.POSITIONS_NONSENSE#1";
			//            Assembler.PopupException(msg1);
			//        }
			//        if (this.cumulativeProfitPercent.ContainsKey(posPrev)) {
			//            cumProfitPercent = this.cumulativeProfitPercent[posPrev];
			//        } else {
			//            string msg1 = "REPORTERS.POSITIONS_NONSENSE#2";
			//            Assembler.PopupException(msg1);
			//        }
			//    }
			//    cumProfitDollar += pos.NetProfit;
			//    cumProfitPercent += pos.NetProfitPercent;

			//    double oldValue = this.cumulativeProfitDollar[pos];
			//    this.cumulativeProfitDollar[pos] = cumProfitDollar;
			//    this.cumulativeProfitPercent[pos] = cumProfitPercent;

			//    double newValue = this.cumulativeProfitDollar[pos];
			//    double difference = newValue - oldValue;
			//    if (difference == 0) {
			//        string msg2 = "DID_YOU_GET_QUOTE_WITH_SAME_BID_ASK????__YOU_SHOULDVE_IGNORED_IT_IN_STREAMING_PROVIDER";
			//        Assembler.PopupException(msg2, null, false);
			//    }
			//}
		}
		internal void BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(ReporterPokeUnit reporterPokeUnit) {
			//OVERKILL_ALREADY_ADDED_EVERYTHING this.BuildStatsOnBacktestFinished(); BuildStatsAfterExec()
		}

		public double CumulativeNetProfitForPosition(Position position) {
			double ret = this.cumulativeProfitDollar.ContainsKey(position) ? this.cumulativeProfitDollar[position] : -1;
			return ret;
		}

		public double CumulativeNetProfitPercentForPosition(Position position) {
			double ret = this.cumulativeProfitPercent.ContainsKey(position) ? this.cumulativeProfitPercent[position] : -1;
			return ret;
		}
	}
}
