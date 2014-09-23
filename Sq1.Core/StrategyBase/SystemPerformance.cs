using System;
using System.Collections.Generic;
using System.Reflection;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using System.Diagnostics;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformance {
		public ScriptExecutor Executor { get; private set; }
		public Bars Bars {
			get {
				if (this.Executor.Bars == null) {
					string msg = "don't execute any SystemPerformance calculations before calling InitializeAndScan(): this.Executor.Bars == null";
					throw new Exception(msg);
				}
				return this.Executor.Bars;
			}
		}
		public Bars BenchmarkSymbolBars { get { return this.Bars; } }
		public PositionSize PositionSize { get { return this.Executor.PositionSize; } }

		public List<Position> DEBUGGING_PositionsMaster { get; internal set; }
		public Dictionary<int, List<Position>> DEBUGGING_PositionsMasterByEntryBarSafeCopy { get; internal set; }
		public Dictionary<int, List<Position>> DEBUGGING_PositionsMasterByExitBarSafeCopy { get; internal set; }

		public SystemPerformanceSlice SlicesShortAndLong { get; private set; }
		public SystemPerformanceSlice SliceLong { get; private set; }
		public SystemPerformanceSlice SliceShort { get; private set; }
		public SystemPerformanceSlice SliceBuyHold { get; private set; }

		public SystemPerformance(ScriptExecutor scriptExecutor) {
			if (scriptExecutor == null) {
				string msg = "we shouldn't execute strategy when this.Executor == null";
				throw new Exception(msg);
			}

			this.Executor = scriptExecutor;
			this.SliceLong			= new SystemPerformanceSlice(this, PositionLongShort.Long,		"StatsForLongPositionsOnly");
			this.SliceShort			= new SystemPerformanceSlice(this, PositionLongShort.Short,		"StatsForShortPositionsOnly");
			this.SlicesShortAndLong	= new SystemPerformanceSlice(this, PositionLongShort.Unknown,	"StatsForShortAndLongPositions");
			this.SliceBuyHold		= new SystemPerformanceSlice(this, PositionLongShort.Unknown,	"StatsForBuyHold");
		}
		public void Initialize() {
			if (this.Executor.Bars == null) {
				string msg = "we shouldn't execute strategy on this.Executor.Bars == null";
				throw new Exception(msg);
			}

			this.SliceLong.Initialize();
			this.SliceShort.Initialize();
			this.SlicesShortAndLong.Initialize();
			this.SliceBuyHold.Initialize();
		}
		public void BuildStatsIncrementallyOnEachBarExecFinished(ReporterPokeUnit pokeUnit) {
			Dictionary<int, List<Position>> posByEntry = pokeUnit.PositionsOpenedByBarFilled;
			Dictionary<int, List<Position>> posByExit = pokeUnit.PositionsClosedByBarFilled;
			int absorbedLong	= this.SliceLong.BuildStatsIncrementallyOnEachBarExecFinished(posByEntry, posByExit);
			int absorbedShort	= this.SliceShort.BuildStatsIncrementallyOnEachBarExecFinished(posByEntry, posByExit);
			int absorbedBoth	= this.SlicesShortAndLong.BuildStatsIncrementallyOnEachBarExecFinished(posByEntry, posByExit);
			int absorbedBH	= this.SliceBuyHold.BuildStatsIncrementallyOnEachBarExecFinished(posByEntry, posByExit);
		}

		public void BuildStatsOnBacktestFinished(List<Position> positionsMaster) {
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

			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(null);

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
			pokeUnit.PositionsOpened = this.Executor.ExecutionDataSnapshot.PositionsOpenNow;
			pokeUnit.PositionsClosed = positionsClosed;
			this.BuildStatsIncrementallyOnEachBarExecFinished(pokeUnit);
		}
	}
}
