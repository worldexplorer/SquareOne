using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Indicators;
using Sq1.Core.Optimization;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public partial class Optimizer {
		public static string OPTIMIZATION_CONTEXT_PREFIX = "OptimizationIteration";

		public	ScriptExecutor					Executor	{ get; private set; }
				DisposableExecutorsPool			disposableExecutorsPool;
				//List<SystemPerformance>			backtestsUnused;

		public	string		DataRangeAsString				{ get { return this.Executor.Strategy.ScriptContextCurrent.DataRange.ToString(); } }
		public	string		PositionSizeAsString			{ get { return this.Executor.Strategy.ScriptContextCurrent.PositionSize.ToString(); } }
		public	string		StrategyAsString				{ get {
				return this.Executor.Strategy.Name
					//+ "   " + executor.Strategy.ScriptParametersAsStringByIdJSONcheck
					//+ executor.Strategy.IndicatorParametersAsStringByIdJSONcheck
				;
			} }
		public	string		SymbolScaleIntervalAsString		{ get {
				ContextScript ctx = this.Executor.Strategy.ScriptContextCurrent;
				string ret = "[" + ctx.ScaleInterval.ToString() + "] "
					+ ctx.Symbol
					+ " :: " + ctx.DataSourceName;
				return ret;
			} }
		
		public	int			ScriptParametersTotalNr			{ get {
				int ret = 0;
				foreach (ScriptParameter sp in this.Executor.Strategy.Script.ScriptParametersById_ReflectedCached.Values) {
					if (sp.NumberOfRuns == 0) continue;
					if (sp.WillBeSequencedDuringOptimization == false) continue;
					if (ret == 0) ret = 1;
					try {
						ret *= sp.NumberOfRuns;
					} catch (Exception ex) {
						//Debugger.Break();
						Assembler.PopupException("DECREASE_RANGE_OR_INCREASE_STEP_FOR_SCRIPT_PARAMETERS_PRIOR_TO " + sp.ToString(), ex, false);
						ret = -1;
						break;
					}
				}
				return ret;
			} }
		public	int			IndicatorParameterTotalNr		{ get {
				int ret = 0;
				//foreach (Indicator i in executor.Strategy.Script.indicatorsByName_ReflectedCached.Values) {	//looks empty on Deserialization
				foreach (IndicatorParameter ip in this.Executor.Strategy.Script.IndicatorsParameters_ReflectedCached.Values) {
					if (ip.NumberOfRuns == 0) continue;
					if (ip.WillBeSequencedDuringOptimization == false) continue;
					if (ret == 0) ret = 1;
					try {
						ret *= ip.NumberOfRuns;
					} catch (Exception ex) {
						//Debugger.Break();
						Assembler.PopupException("INCREASE_STEP_FOR_INDICATOR_PARAMETERS_PRIOR_TO " + ip.ToString(), ex, false);
						ret = -1;
						break;
					}
				}
				return ret;
			} }
		
		public	string		SpreadPips						{ get { return this.Executor.SpreadPips; } }
		public	string		BacktestStrokesPerBar			{ get { return this.Executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar.ToString(); } }

		public	int			AllParameterLinesToDraw			{ get {
				return this.Executor.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders.Count;
			} }

		public	SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParametersMergedByName { get {
			return this.Executor.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedClonedForSequencerByName; } }
		
		public	int			BacktestsTotal					{ get; private set; }
		public	int			BacktestsRemaining				{ get { return this.BacktestsTotal - this.BacktestsFinished; } }
		public	int			DisposableExecutorsRunningNow	{ get {
				return (this.disposableExecutorsPool != null)
					? this.disposableExecutorsPool.ExecutorsRunningNow
					: 0;
			} }
		public	int			DisposableExecutorsSpawnedNow	{ get {
				return (this.disposableExecutorsPool != null)
					? this.disposableExecutorsPool.ExecutorsSpawnedNow
					: 0;
			} }
		public	int			BacktestsFinished				{ get; private set; }
		public	float		BacktestsSecondsElapsed			{ get; private set; }

		public	int			CpuCoresAvailable				{ get; private set; }
		public	int			ThreadsToUse;	// UPDATED_FROM_GUI	{ get; private set; }
		public	bool		InitializedProperly_executorHasScript_readyToOptimize;
		
				Stopwatch	stopWatch;
		
				string		iWasRunForSymbolScaleIntervalAsString;
				string		iWasRunForDataRangeAsString;
				string		iWasRunForPositionSizeAsString;
		
		public	bool		IsRunningNow					{ get { return this.DisposableExecutorsRunningNow > 0; } }
		public	bool		AbortedDontScheduleNewBacktests { get; private set; }
		
				ManualResetEvent unpaused;
		public	bool UnpausedBlocking { get { return this.unpaused.WaitOne(-1); } }
		public	bool Unpaused {
			get { return this.unpaused.WaitOne(0); }
			set {
				if (value == true) {
					this.unpaused.Set();
				} else {
					this.unpaused.Reset();
				}
			} }

		public void ClearIWasRunFor() {
			this.iWasRunForSymbolScaleIntervalAsString = null;
			this.iWasRunForDataRangeAsString = null;
			this.iWasRunForPositionSizeAsString = null;
			//this.backtestsUnused.Clear();
		}
		
		Optimizer() {
			//backtestsUnused = new List<SystemPerformance>();
			CpuCoresAvailable = Environment.ProcessorCount;
			ThreadsToUse = CpuCoresAvailable;
			stopWatch = new Stopwatch();
			unpaused = new ManualResetEvent(true);
		}
		public Optimizer(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}
		
		public void Initialize() {
			//this.BacktestsRemaining = -1;
			this.InitializedProperly_executorHasScript_readyToOptimize = false;
			if (this.Executor.Strategy == null) {
				string msg = "Optimizer.executor.Strategy == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.Strategy.Script == null) {
				string msg = "Optimizer.executor.Strategy.Script == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.Strategy.Script.ScriptParametersById_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.ParametersById == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.ExecutionDataSnapshot == null) {
				string msg = "Optimizer.executor.ExecutionDataSnapshot == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.Strategy.Script.IndicatorsByName_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.IndicatorsByName_ReflectedCached == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.Executor.Strategy.Script.ScriptParametersById_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.ScriptParametersById_ReflectedCached == null";
				Assembler.PopupException(msg);
				return;
			}
			this.InitializedProperly_executorHasScript_readyToOptimize = true;
			this.TotalsCalculate();
		}

		public void TotalsCalculate() {
			int scriptParametersTotalNr = this.ScriptParametersTotalNr;
			int indicatorParameterTotalNr = this.IndicatorParameterTotalNr;
			if (scriptParametersTotalNr == 0) {
				this.BacktestsTotal = indicatorParameterTotalNr;
			}
			if (indicatorParameterTotalNr == 0) {
				this.BacktestsTotal = scriptParametersTotalNr;
			}
			if (scriptParametersTotalNr > 0 && indicatorParameterTotalNr > 0) {
				this.BacktestsTotal = scriptParametersTotalNr * indicatorParameterTotalNr;
			}
		}

		public int OptimizationRun() {
			this.AbortedDontScheduleNewBacktests = false;
			this.BacktestsFinished = 0;
			this.disposableExecutorsPool = new DisposableExecutorsPool(this);
			this.disposableExecutorsPool.SpawnAndLaunch(this.ThreadsToUse);
			stopWatch.Restart();

			this.iWasRunForSymbolScaleIntervalAsString = this.SymbolScaleIntervalAsString;
			this.iWasRunForDataRangeAsString = this.DataRangeAsString;
			this.iWasRunForPositionSizeAsString = this.PositionSizeAsString;
			this.RaiseOnBacktestStarted();

			return this.DisposableExecutorsRunningNow;
		}

		public void OptimizationAbort() {
			this.AbortedDontScheduleNewBacktests = true;
			if (this.Unpaused == false) {
				this.Unpaused = true;	// DEADLOCK_OTHERWIZE__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH__DISABLE_BUTTONS_FOR_USER_NOT_TO_WORSEN
			}
			if (this.disposableExecutorsPool == null) {
				string msg = "OPTIMIZATION_DIDNT_START_YET_optimizerExecutorsPool=null";
				Assembler.PopupException(msg);
			} else {
				this.disposableExecutorsPool.AbortAllTasksAndDispose();
				this.disposableExecutorsPool = null;
			}
			this.RaiseOnOptimizationAborted();
		}

		internal void PoolFinishedBacktestOne(SystemPerformance systemPerformance) {
			//this.backtestsUnused.Add(executorCompletePooled.Performance);
			this.BacktestsFinished++;	//Optimizer_OnBacktestComplete() will display it
			this.RaiseOnOneBacktestFinished(systemPerformance);

			if (this.AbortedDontScheduleNewBacktests) {
				string msg = "DONT_INVOKE_PoolFinishedBacktestOne_FROM_POOL"
					+ ",POOL_INTERRUPTS_ALL_TASKS_AND_DISPOSES_ITSELF_AQAP_TO_UNBLOCK_OptimizationAbort()";
				Assembler.PopupException(msg);
				return;
			}

			if (this.BacktestsFinished >= this.BacktestsTotal) {
				this.PoolFinishedBacktestsAll();
				return;
			}

			if (this.BacktestsFinished + this.DisposableExecutorsRunningNow >= this.BacktestsTotal) {
				return;
			}

			bool unpaused2 = this.UnpausedBlocking;
			this.disposableExecutorsPool.LaunchToReachTotalNr(this.ThreadsToUse);
		}

		public void PoolFinishedBacktestsAll() {
			if (this.disposableExecutorsPool == null) {
				string msg = "POOL_AREADY_DISPOSED_FIX_COUNTERS";
				Assembler.PopupException(msg);
				return;
			}
			this.disposableExecutorsPool.Dispose();
			this.disposableExecutorsPool = null;
			this.RaiseOnAllBacktestsFinished();
		}

	}
}
