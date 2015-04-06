using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Sq1.Core.Indicators;
using Sq1.Core.Optimization;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public partial class Optimizer {
		public static string OPTIMIZATION_CONTEXT_PREFIX = "OptimizationIteration";

		public	ScriptExecutor					ExecutorCloneToBeSpawned	{ get; private set; }
				OptimizerParametersSequencer	parametersSequencer;
				object							backtestsLock;
				//List<SystemPerformance>			backtestsUnused;
				List<ScriptExecutor>			executorsRunning;
		

		public	string		DataRangeAsString				{ get { return this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent.DataRange.ToString(); } }
		public	string		PositionSizeAsString			{ get { return this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent.PositionSize.ToString(); } }
		public	string		StrategyAsString				{ get {
				return this.ExecutorCloneToBeSpawned.Strategy.Name
					//+ "   " + executor.Strategy.ScriptParametersAsStringByIdJSONcheck
					//+ executor.Strategy.IndicatorParametersAsStringByIdJSONcheck
				;
			} }
		public	string		SymbolScaleIntervalAsString		{ get {
				ContextScript ctx = this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent;
				string ret = "[" + ctx.ScaleInterval.ToString() + "] "
					+ ctx.Symbol
					+ " :: " + ctx.DataSourceName;
				return ret;
			} }
		
		public	int			ScriptParametersTotalNr			{ get {
				int ret = 0;
				foreach (ScriptParameter sp in this.ExecutorCloneToBeSpawned.Strategy.Script.ScriptParametersById_ReflectedCached.Values) {
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
				foreach (IndicatorParameter ip in this.ExecutorCloneToBeSpawned.Strategy.Script.IndicatorsParameters_ReflectedCached.Values) {
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
		
		public	string		SpreadPips						{ get { return this.ExecutorCloneToBeSpawned.SpreadPips; } }
		public	string		BacktestStrokesPerBar			{ get { return this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent.BacktestStrokesPerBar.ToString(); } }

		public	int			AllParameterLinesToDraw			{ get {
				return this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders.Count;
			} }

		public	SortedDictionary<string, IndicatorParameter>	ScriptAndIndicatorParametersMergedByName { get {
			return this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedClonedForSequencerByName; } }
		
		public	SortedDictionary<int, ScriptParameter>			ParametersById;

		public	int			BacktestsTotal;
		public	int			BacktestsRemaining { get { return this.BacktestsTotal - this.BacktestsCompleted; } }
		public	int			BacktestsScheduled;
		public	int			BacktestsCompleted;
		public	float		BacktestsSecondsElapsed;

				bool		inNewThread;
		public	int			CpuCoresAvailable;
		public	int			ThreadsToUse;
		public	bool		InitializedProperly;
		
				Stopwatch	stopWatch;
		
				string		iWasRunForSymbolScaleIntervalAsString;
				string		iWasRunForDataRangeAsString;
				string		iWasRunForPositionSizeAsString;
		
		public	string		StaleReason { get {
				//if (this.backtestsUnused.Count == 0) {
				//	return null;	//optimization hasn't started yet => it isn't stale, nothing to make red;
				//}
				if (this.iWasRunForSymbolScaleIntervalAsString != this.SymbolScaleIntervalAsString) {
					string msg = "iWasRunFor[" + this.iWasRunForSymbolScaleIntervalAsString
						+ "] != [" + this.SymbolScaleIntervalAsString + "]ScriptContextCurrent.Symbol+ScaleInterval";
					return msg;
				}
				if (this.iWasRunForDataRangeAsString != this.DataRangeAsString) {
					string msg = "iWasRunFor[" + this.iWasRunForDataRangeAsString
						+ "] != [" + this.DataRangeAsString + "]ScriptContextCurrent.DataRange";
					return msg;
				}
				if (this.iWasRunForPositionSizeAsString != this.PositionSizeAsString) {
					string msg = "iWasRunFor[" + this.iWasRunForPositionSizeAsString
						+ "] != [" + this.PositionSizeAsString + "]ScriptContextCurrent.PositionSize";
					return msg;
				}
				return null;
			} }
		public	bool		IsRunningNow;
		public	bool		AbortedDontScheduleNewBacktests;
		
				ManualResetEvent unpaused;
		public	bool UnpausedBlocking { get { return this.unpaused.WaitOne(-1); } }
		public	bool Unpaused {
			get {
				return this.unpaused.WaitOne(0);
			}
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
		
		public Optimizer(ScriptExecutor executor) {
			this.ExecutorCloneToBeSpawned = executor;
			//backtestsUnused = new List<SystemPerformance>();
			executorsRunning = new List<ScriptExecutor>();
			backtestsLock = new object();
			inNewThread = true;
			CpuCoresAvailable = inNewThread ? Environment.ProcessorCount : 1;
			ThreadsToUse = CpuCoresAvailable;
			stopWatch = new Stopwatch();
			unpaused = new ManualResetEvent(true);
		}
		
		public void Initialize() {
			//this.BacktestsRemaining = -1;
			this.InitializedProperly = false;
			if (this.ExecutorCloneToBeSpawned.Strategy == null) {
				string msg = "Optimizer.executor.Strategy == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ExecutorCloneToBeSpawned.Strategy.Script == null) {
				string msg = "Optimizer.executor.Strategy.Script == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ExecutorCloneToBeSpawned.Strategy.Script.ScriptParametersById_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.ParametersById == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ExecutorCloneToBeSpawned.ExecutionDataSnapshot == null) {
				string msg = "Optimizer.executor.ExecutionDataSnapshot == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ExecutorCloneToBeSpawned.Strategy.Script.IndicatorsByName_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.IndicatorsByName_ReflectedCached == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ExecutorCloneToBeSpawned.Strategy.Script.ScriptParametersById_ReflectedCached == null) {
				string msg = "Optimizer.executor.Strategy.Script.ScriptParametersById_ReflectedCached == null";
				Assembler.PopupException(msg);
				return;
			}
			this.InitializedProperly = true;
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
		
		public void OptimizationAbort() {
			this.AbortedDontScheduleNewBacktests = true;
			if (this.Unpaused == false) {
				this.Unpaused = true;	// DEADLOCK_OTHERWIZE__LET_ALL_SCHEDULED_PAUSED_STILL_INERTIOUSLY_FINISH__DISABLE_BUTTONS_FOR_USER_NOT_TO_WORSEN
			}
			lock (this.backtestsLock) {
				foreach (var ex in this.executorsRunning) {
					string msg = "OPTIMIZER_CANCELLED " + this.executorsRunning.IndexOf(ex) + "/" + this.executorsRunning.Count;
					ex.Backtester.AbortRunningBacktestWaitAborted(msg, 0);
				}
			}
			//this.AbortInProgress = false;
			this.IsRunningNow = false;
			this.RaiseOnOptimizationAborted();
		}
		public int OptimizationRun() {
			this.parametersSequencer = new OptimizerParametersSequencer(this.ExecutorCloneToBeSpawned.Strategy.ScriptContextCurrent);
			this.BacktestsCompleted = 0;
			this.BacktestsScheduled = 0;
			lock(this.backtestsLock) {
				this.AbortedDontScheduleNewBacktests = false;
				this.IsRunningNow = true;
				//this.BacktestsRemaining = this.BacktestsTotal;
				this.executorsRunning.Clear();
				for(int i=0; i<this.ThreadsToUse; i++) {
					//this.BacktestsRemaining--;
					if (this.BacktestsScheduled >= this.BacktestsTotal) break;
					string ctxName = OPTIMIZATION_CONTEXT_PREFIX + " " + (this.BacktestsScheduled + 1) + "/" + this.BacktestsTotal;
					ContextScript ctxNext = (i == 0)
						? this.parametersSequencer.GetFirstScriptContext(ctxName)
						: this.parametersSequencer.GetNextScriptContextSequenced(ctxName);
					ScriptExecutor ex = this.ExecutorCloneToBeSpawned.CloneForOptimizer(ctxNext);
					//ex.Strategy.Script.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel(false);
					//ex.Strategy.Script.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel(false);

					string ctxParamsAsString = ex.ToStringWithCurrentParameters();

					this.executorsRunning.Add(ex);
					ex.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
					this.BacktestsScheduled++;
					if (this.inNewThread == false) break;
				}
			}
			stopWatch.Restart();

			this.iWasRunForSymbolScaleIntervalAsString	= this.SymbolScaleIntervalAsString;
			this.iWasRunForDataRangeAsString			= this.DataRangeAsString;
			this.iWasRunForPositionSizeAsString			= this.PositionSizeAsString;
			this.RaiseOnBacktestStarted();

			return this.BacktestsScheduled;
		}
		void afterBacktesterComplete(ScriptExecutor executorCompletePooled) {
			string msig = " //Optimizer.afterBacktesterComplete()";
			if (executorCompletePooled == null) {
				string msg = "CAN_NOT_BE_NULL_executorCompletePooled";
				Assembler.PopupException(msg + msig);
				executorCompletePooled = this.ExecutorCloneToBeSpawned;
			}
			msig = executorCompletePooled.ToStringWithCurrentParameters() + msig;
			string msg2 = " ANOTHER_IN_SEQUENCE_executorCompletePooled";
			//Assembler.PopupException(msg2 + msig, null, false);

			if (executorCompletePooled.Bars == null) {
				string msg = "DONT_RUN_BACKTEST_BEFORE_BARS_ARE_LOADED";
				Assembler.PopupException(msg);
				return;
			}

			try {
				// MOVED_TO_BacktesterRunSimulation() executorCompletePooled.Performance.BuildStatsOnBacktestFinished();
				//SystemPerformance clone = executorCompletePooled.Performance.CloneForOptimizer;
				lock (this.backtestsLock) {	// helps also with atomic BacktestsCompleted operations
					//this.backtestsUnused.Add(executorCompletePooled.Performance);
					this.RaiseOnOneBacktestFinished(executorCompletePooled.Performance);
					this.BacktestsCompleted++;	//Optimizer_OnBacktestComplete() will display it
					//v2 
					this.executorsRunning.Remove(executorCompletePooled);

					//}
					//this.BacktestsRemaining--;
					int moreToAdd = this.ThreadsToUse - this.executorsRunning.Count;
					if (moreToAdd > 0) {
						for (int i = moreToAdd; i <= this.ThreadsToUse; i++) {
							if (this.BacktestsScheduled >= this.BacktestsTotal) break;
							if (this.AbortedDontScheduleNewBacktests) break;
							string ctxName1 = OPTIMIZATION_CONTEXT_PREFIX + " " + (this.BacktestsScheduled + 1 + i) + "/" + this.BacktestsTotal;
							ContextScript ctxNext1 = this.parametersSequencer.GetNextScriptContextSequenced(ctxName1);
							ScriptExecutor ex1 = this.ExecutorCloneToBeSpawned.CloneForOptimizer(ctxNext1);
							
							bool unpaused = this.UnpausedBlocking;
							this.executorsRunning.Add(ex1);
							this.BacktestsScheduled++;
							ex1.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
						}
					}
					if (moreToAdd < 0) {
						//this.executorsRunning.Remove(executorCompletePooled);
						return;
					}
					if (this.BacktestsScheduled >= this.BacktestsTotal) {
						//this.IsRunningNow = false;	// leave .IsRunningNow=true until last backtest finished (strategy is happy but someone on the UI may be not)
						//NOPE_INVOKE_IT_AFTER_LAST_BACKTEST_SCHEDULED_TERMINATES this.RaiseOnOptimizationComplete();
						return;
					}
					if (this.AbortedDontScheduleNewBacktests) return;
					if (this.inNewThread == false && this.BacktestsScheduled + 1 >= this.BacktestsTotal) return;
					string ctxName = OPTIMIZATION_CONTEXT_PREFIX + " " + (this.BacktestsScheduled + 1) + "/" + this.BacktestsTotal;
					//v1 - only last 4 backtests in optimization are truly unique; previous are pointing to these 4 (4 threads)
					//executorCompletePooled.Strategy.ScriptContextsByName.Clear();
					//executorCompletePooled.Strategy.ScriptContextAdd(ctxName, this.parametersSequencer.GetNextScriptContextSequenced(ctxName), true);
					//this.BacktestsScheduled++;
					//executorCompletePooled.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
					//v2 
					//WENT_UP this.executorsRunning.Remove(executorCompletePooled);
					ContextScript ctxNext = this.parametersSequencer.GetNextScriptContextSequenced(ctxName);
					ScriptExecutor ex = this.ExecutorCloneToBeSpawned.CloneForOptimizer(ctxNext);
					
					bool unpaused2 = this.UnpausedBlocking;
					this.executorsRunning.Add(ex);
					this.BacktestsScheduled++;
					ex.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				//I_DONT_NEED_REPORTERS_TO_BE_REBUILT__OPTIMIZATION_CONTROLS_CHARTLESS_BACKTESTS
				//v1
				//if (this.BacktestsCompleted >= this.BacktestsTotal) {
				//    if (this.executorsRunning.Count > 0) {
				//        string msg = "FYI AFTER_OPTIMIZATION_ITERATION_FINISHED_YOU_STILL_HAVE_MORE_EXECUTORS_RUNNING " + this.executorsRunning.Count;
				//        Assembler.PopupException(msg);
				//    }
				//    this.RaiseOnAllBacktestsFinished();
				//}
				//v2 leave .IsRunningNow=true until last backtest finished (strategy is happy but someone on the UI may be not)
				if (this.executorsRunning.Count == 0) {
					this.IsRunningNow = false;
					this.RaiseOnAllBacktestsFinished();
				}
			}
		}
	}
}
