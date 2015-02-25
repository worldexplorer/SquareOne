using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Core.Optimization;

namespace Sq1.Core.Optimization {
	public class Optimizer {
		public static string OPTIMIZATION_CONTEXT_PREFIX = "OptimizationIteration";

		public	ScriptExecutor												Executor { get; private set; }
				OptimizerParametersSequencer								parametersSequencer;

		public event EventHandler<EventArgs>								OnOneBacktestStarted;
		// since Optimizer.backtests is multithreaded list, I imply OptimizerControl.backtests to keep its own copy for ObjectListView to freely crawl over it without interference (instead of providing Optimizer.BacktestsThreadSafeCopy)  
		public event EventHandler<SystemPerformanceRestoreAbleEventArgs>	OnOneBacktestFinished;
		public event EventHandler<EventArgs>								OnAllBacktestsFinished;
		public event EventHandler<EventArgs>								OnOptimizationAborted;
		
		public string DataRangeAsString { get { return Executor.Strategy.ScriptContextCurrent.DataRange.ToString(); } }
		public string PositionSizeAsString { get { return Executor.Strategy.ScriptContextCurrent.PositionSize.ToString(); } }
		public string StrategyAsString { get {
				return Executor.Strategy.Name
					//+ "   " + executor.Strategy.ScriptParametersAsStringByIdJSONcheck
					//+ executor.Strategy.IndicatorParametersAsStringByIdJSONcheck
				;
			} }
		public string SymbolScaleIntervalAsString { get {
				ContextScript ctx = this.Executor.Strategy.ScriptContextCurrent;
				return ctx.DataSourceName
					+ " :: " + ctx.Symbol
					+ " [" + ctx.ScaleInterval.ToString() + "]";
			} }
		
		public int ScriptParametersTotalNr { get {
				int ret = 0;
				foreach (ScriptParameter sp in Executor.Strategy.Script.ScriptParametersById.Values) {
					if (sp.NumberOfRuns == 0) continue;
					if (ret == 0) ret = 1;
					ret *= sp.NumberOfRuns;
				}
				return ret;
			} }
			
		public int IndicatorParameterTotalNr { get {
				int ret = 0;
				//foreach (Indicator i in executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {	//looks empty on Deserialization
				foreach (IndicatorParameter ip in Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
					if (ip.NumberOfRuns == 0) continue;
					if (ret == 0) ret = 1;
					ret *= ip.NumberOfRuns;
				}
				return ret;
			} }
		public SortedDictionary<string, IndicatorParameter> ScriptAndIndicatorParametersMergedByName { get { return this.Executor.Strategy.ScriptContextCurrent.ParametersMergedByName; } }

		public int BacktestsTotal;
		public int BacktestsRemaining { get { return this.BacktestsTotal - this.BacktestsCompleted; } }
		public int BacktestsScheduled;
		public int BacktestsCompleted;
		public float BacktestsSecondsElapsed;

		bool inNewThread;
		public int CpuCoresAvailable;
		public int ThreadsToUse;
		public bool InitializedProperly;
		
		public Stopwatch stopWatch;
		
		string iWasRunForSymbolScaleIntervalAsString;
		string iWasRunForDataRangeAsString;
		string iWasRunForPositionSizeAsString;
		
		public string StaleReason { get {
				if (this.backtests.Count == 0) {
					return null;	//optimization hasn't started yet => it isn't stale, nothing to make red;
				}
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
		public void ClearIWasRunFor() {
			this.iWasRunForSymbolScaleIntervalAsString = null;
			this.iWasRunForDataRangeAsString = null;
			this.iWasRunForPositionSizeAsString = null;
			this.backtests.Clear();
		}
		
		public Optimizer(ScriptExecutor executor) {
			this.Executor = executor;
			backtests = new List<SystemPerformance>();
			executorsRunning = new List<ScriptExecutor>();
			backtestsLock = new object();
			inNewThread = true;
			CpuCoresAvailable = inNewThread ? Environment.ProcessorCount : 1;
			ThreadsToUse = CpuCoresAvailable;
			stopWatch = new Stopwatch();
		}
		
		public SortedDictionary<int, ScriptParameter> ParametersById;
		public Dictionary<string, IndicatorParameter> IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;
		
		public void Initialize() {
			//this.BacktestsRemaining = -1;
			this.InitializedProperly = false;
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
			if (this.Executor.Strategy.Script.ScriptParametersById == null) {
				string msg = "Optimizer.executor.Strategy.Script.ParametersById == null";
				Assembler.PopupException(msg);
				return;
			}
			this.ParametersById = this.Executor.Strategy.Script.ScriptParametersById;


			if (this.Executor.ExecutionDataSnapshot == null) {
				string msg = "Optimizer.executor.ExecutionDataSnapshot == null";
				Assembler.PopupException(msg);
				return;
			}
//			if (this.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null) {
//				string msg = "Optimizer.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null";
//				Assembler.PopupException(msg);
//				return;
//			}
			if (this.Executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null) {
				string msg = "Optimizer.executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null";
				Assembler.PopupException(msg);
				return;
			}
			this.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders = this.Executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;
			this.InitializedProperly = true;

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
		
		object backtestsLock;
		List<SystemPerformance> backtests;
		List<ScriptExecutor> executorsRunning;
		public bool IsRunningNow;
		public bool AbortedDontScheduleNewBacktests;
		public void OptimizationAbort() {
			this.AbortedDontScheduleNewBacktests = true;
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
			this.parametersSequencer = new OptimizerParametersSequencer(this.Executor.Strategy.ScriptContextCurrent);
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
					ScriptExecutor ex = this.Executor.CloneForOptimizer(ctxNext);
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
				executorCompletePooled = this.Executor;
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
					this.backtests.Add(executorCompletePooled.Performance);
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
							ScriptExecutor ex1 = this.Executor.CloneForOptimizer(ctxNext1);
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
						this.IsRunningNow = false;
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
					ScriptExecutor ex = this.Executor.CloneForOptimizer(ctxNext);
					this.executorsRunning.Add(ex);
					this.BacktestsScheduled++;
					ex.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.RaiseOnOneBacktestFinished(executorCompletePooled.Performance);
				if (this.BacktestsCompleted >= this.BacktestsTotal) {
					if (this.executorsRunning.Count > 0) {
						string msg = "FYI AFTER_OPTIMIZATION_ITERATION_FINISHED_YOU_STILL_HAVE_MORE_EXECUTORS_RUNNING " + this.executorsRunning.Count;
						Assembler.PopupException(msg);
					}
					this.RaiseOnAllBacktestsFinished();
				}
			}
		}
		public void RaiseOnBacktestStarted() {
			if (this.AbortedDontScheduleNewBacktests) return;
			if (this.OnOneBacktestStarted == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_BACKTEST_STARTED";
				//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK  Assembler.PopupException(msg);
				return;
			}
			try {
				this.OnOneBacktestStarted(this, EventArgs.Empty);
			} catch (Exception ex) {
				string msg = "OPTIMIZER_CONTROL_THREW_ON_BACKTEST_STARTED";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnOneBacktestFinished(SystemPerformance sysPerf) {
			if (this.AbortedDontScheduleNewBacktests) return;
			if (this.OnOneBacktestFinished == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_BACKTEST_COMPLETED";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.BacktestsSecondsElapsed = (float) Math.Round(stopWatch.ElapsedMilliseconds / 1000d, 1);
				SystemPerformanceRestoreAble performanceEssence = new SystemPerformanceRestoreAble(sysPerf);
				this.OnOneBacktestFinished(this, new SystemPerformanceRestoreAbleEventArgs(performanceEssence));
			} catch (Exception ex) {
				string msg = "OPTIMIZER_CONTROL_THREW_ON_BACKTEST_COMPLETE";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnAllBacktestsFinished() {
			if (this.OnAllBacktestsFinished == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_OPTIMIZATION_COMPLETE";
				Assembler.PopupException(msg);
				return;
			}
			try {
				stopWatch.Stop();
				this.BacktestsSecondsElapsed = (float) Math.Round(stopWatch.ElapsedMilliseconds / 1000d, 1);
				this.OnAllBacktestsFinished(this, EventArgs.Empty);
			} catch (Exception ex) {
				string msg = "OPTIMIZER_CONTROL_THREW_ON_OPTIMIZATION_COMPLETE";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnOptimizationAborted() {
			if (this.OnOptimizationAborted == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_OPTIMIZATION_ABORTED";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.OnOptimizationAborted(this, EventArgs.Empty);
			} catch (Exception ex) {
				string msg = "OPTIMIZER_CONTROL_THREW_ON_OPTIMIZATION_ABORTED";
				Assembler.PopupException(msg, ex);
			}
		}

        public EventHandler<EventArgs> OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild;

        public bool ScriptRecompiledColumnsRebuildPostponed;
        public void RaiseScriptRecompiledUpdateHeaderPostponeColumnsRebuild() {
            this.ScriptRecompiledColumnsRebuildPostponed = true;

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

            if (this.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild == null) return;
            this.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild(this, null);
        }
    }
}
