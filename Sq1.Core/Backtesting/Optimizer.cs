using System;
using System.Collections.Generic;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using System.Diagnostics;

namespace Sq1.Core.Backtesting {
	public class Optimizer {
		public static string OPTIMIZATION_CONTEXT_PREFIX = "OptimizationIteration";

		ScriptExecutor executor;
		OptimizerParametersSequencer parametersSequencer;

		// since Optimizer.backtests is multithreaded list, I imply OptimizerControl.backtests to keep its own copy for ObjectListView to freely crawl over it without interference (instead of providing Optimizer.BacktestsThreadSafeCopy)  
		public event EventHandler<SystemPerformanceEventArgs>	OnBacktestComplete;
		public event EventHandler<EventArgs>					OnOptimizationComplete;
		public event EventHandler<EventArgs>					OnOptimizationAborted;
		
		public string DataRangeAsString { get { return executor.Strategy.ScriptContextCurrent.DataRange.ToString(); } }
		public string PositionSizeAsString { get { return executor.Strategy.ScriptContextCurrent.PositionSize.ToString(); } }
		public string StrategyAsString { get {
				return executor.Strategy.Name
					//+ "   " + executor.Strategy.ScriptParametersAsStringByIdJSONcheck
					//+ executor.Strategy.IndicatorParametersAsStringByIdJSONcheck
				;
			} }
		public string SymbolAsString { get {
				ContextScript ctx = this.executor.Strategy.ScriptContextCurrent;
				return ctx.DataSourceName
					+ " :: " + ctx.Symbol
					+ " [" + ctx.ScaleInterval.ToString() + "]";
			} }
		
		public int ScriptParametersTotalNr { get {
				int ret = 0;
				foreach (ScriptParameter sp in executor.Strategy.Script.ParametersById.Values) {
					if (sp.NumberOfRuns == 0) continue;
					if (ret == 0) ret = 1;
					ret *= sp.NumberOfRuns;
				}
				return ret;
			} }
			
		public int IndicatorParameterTotalNr { get {
				int ret = 0;
				//foreach (Indicator i in executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {	//looks empty on Deserialization
				foreach (IndicatorParameter ip in executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders.Values) {
					if (ip.NumberOfRuns == 0) continue;
					if (ret == 0) ret = 1;
					ret *= ip.NumberOfRuns;
				}
				return ret;
			} }

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
		
		public Optimizer(ScriptExecutor executor) {
			this.executor = executor;
			backtests = new List<SystemPerformance>();
			executorPool = new List<ScriptExecutor>();
			backtestsLock = new object();
			executorPoolLock = new object();
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
			if (this.executor.Strategy == null) {
				string msg = "Optimizer.executor.Strategy == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.executor.Strategy.Script == null) {
				string msg = "Optimizer.executor.Strategy.Script == null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.executor.Strategy.Script.ParametersById == null) {
				string msg = "Optimizer.executor.Strategy.Script.ParametersById == null";
				Assembler.PopupException(msg);
				return;
			}
			this.ParametersById = this.executor.Strategy.Script.ParametersById;


			if (this.executor.ExecutionDataSnapshot == null) {
				string msg = "Optimizer.executor.ExecutionDataSnapshot == null";
				Assembler.PopupException(msg);
				return;
			}
//			if (this.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null) {
//				string msg = "Optimizer.executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances == null";
//				Assembler.PopupException(msg);
//				return;
//			}
			if (this.executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null) {
				string msg = "Optimizer.executor.Strategy.ScriptContextCurrent.IndicatorParametersByName == null";
				Assembler.PopupException(msg);
				return;
			}
			this.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders = this.executor.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;
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
		object executorPoolLock;
		List<SystemPerformance> backtests;
		List<ScriptExecutor> executorPool;
		public bool IsRunningNow;
		public bool AbortedDontScheduleNewBacktests;
		public void OptimizationAbort() {
			this.AbortedDontScheduleNewBacktests = true;
			lock(executorPoolLock) {
				foreach (var ex in executorPool) {
					string msg = "OPTIMIZER_CANCELLED " + this.executorPool.IndexOf(ex) + "/" + this.executorPool.Count;
					ex.Backtester.AbortRunningBacktestWaitAborted(msg, 0);
				}
			}
			//this.AbortInProgress = false;
			this.IsRunningNow = false;
			this.RaiseOnOptimizationAborted();
		}
		public int OptimizationRun() {
			this.parametersSequencer = new OptimizerParametersSequencer(this.executor.Strategy.ScriptContextCurrent);
			this.BacktestsCompleted = 0;
			this.BacktestsScheduled = 0;
			//lock(executorPoolLock) {
			lock(this.backtestsLock) {
				this.AbortedDontScheduleNewBacktests = false;
				this.IsRunningNow = true;
				//this.BacktestsRemaining = this.BacktestsTotal;
				this.executorPool.Clear();
				for(int i=0; i<this.ThreadsToUse; i++) {
					//this.BacktestsRemaining--;
					if (this.BacktestsScheduled >= this.BacktestsTotal) break;
					string ctxName = OPTIMIZATION_CONTEXT_PREFIX + " " + (this.BacktestsScheduled + 1) + "/" + this.BacktestsTotal;
					ContextScript ctxNext = (i == 0)
						? this.parametersSequencer.GetFirstScriptContext(ctxName)
						: this.parametersSequencer.GetNextScriptContextSequenced(ctxName);
					ScriptExecutor ex = this.executor.CloneForOptimizer(ctxNext);
					this.executorPool.Add(ex);
					ex.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
					this.BacktestsScheduled++;
					if (this.inNewThread == false) break;
				}
			}
			stopWatch.Start();
			return this.BacktestsScheduled;
		}
		void afterBacktesterComplete(ScriptExecutor executorCompletePooled) {
			#if DEBUG
			string dbg = "complete: " + executorCompletePooled.Strategy.ScriptContextCurrent.Name + " "
				+ executorCompletePooled.Strategy.Script.IndicatorParametersAsString;
			string dbg2 = "";
			foreach (string iName in executorCompletePooled.Performance.ScriptAndIndicatorParameterClonesByName.Keys) {
				IndicatorParameter ip = executorCompletePooled.Performance.ScriptAndIndicatorParameterClonesByName[iName];
				dbg2 += iName + "[" + ip.ValueCurrent + "]";
			}
			Assembler.PopupException(dbg + dbg2, null, false);
			#endif

			if (executorCompletePooled == null) {
				Debugger.Break();
				executorCompletePooled = this.executor;
			}
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
					//}
					//lock(this.executorPoolLock) {	// helps also with atomic BacktestsRemaining operations
					//this.BacktestsRemaining--;
					int moreToAdd = this.ThreadsToUse - this.executorPool.Count;
					if (moreToAdd > 0) {
						for (int i = moreToAdd; i <= this.ThreadsToUse; i++) {
							if (this.BacktestsScheduled >= this.BacktestsTotal) break;
							if (this.AbortedDontScheduleNewBacktests) break;
							string ctxName1 = OPTIMIZATION_CONTEXT_PREFIX + " " + (this.BacktestsScheduled + 1 + i) + "/" + this.BacktestsTotal;
							ContextScript ctxNext1 = this.parametersSequencer.GetNextScriptContextSequenced(ctxName1);
							ScriptExecutor ex = this.executor.CloneForOptimizer(ctxNext1);
							this.executorPool.Add(ex);
							this.BacktestsScheduled++;
							executorCompletePooled.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
						}
					}
					if (moreToAdd < 0) {
						this.executorPool.Remove(executorCompletePooled);
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
					executorCompletePooled.Strategy.ScriptContextsByName.Clear();
					executorCompletePooled.Strategy.ScriptContextAdd(ctxName, this.parametersSequencer.GetNextScriptContextSequenced(ctxName), true);
					this.BacktestsScheduled++;
					executorCompletePooled.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), this.inNewThread);
				}
			} catch (Exception ex) {
				Assembler.PopupException(dbg + dbg2, ex);
			} finally {
				this.RaiseOnBacktestComplete(executorCompletePooled.Performance);
				if (this.BacktestsCompleted >= this.BacktestsTotal) {
					this.RaiseOnOptimizationComplete();
				}
			}
		}
		public void RaiseOnBacktestComplete(SystemPerformance clone) {
			if (this.AbortedDontScheduleNewBacktests) return;
			if (this.OnBacktestComplete == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_BACKTEST_COMPLETED";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.BacktestsSecondsElapsed = (float) Math.Round(stopWatch.ElapsedMilliseconds / 1000d, 1);
				this.OnBacktestComplete(this, new SystemPerformanceEventArgs(clone));
			} catch (Exception ex) {
				string msg = "OPTIMIZER_CONTROL_THREW_ON_BACKTEST_COMPLETE";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnOptimizationComplete() {
			if (this.OnBacktestComplete == null) {
				string msg = "OPTIMIZER_HAS_NO_SUBSCRIBERS_TO_NOTIFY_ABOUT_OPTIMIZATION_COMPLETE";
				Assembler.PopupException(msg);
				return;
			}
			try {
				stopWatch.Stop();
				this.BacktestsSecondsElapsed = (float) Math.Round(stopWatch.ElapsedMilliseconds / 1000d, 1);
				this.OnOptimizationComplete(this, EventArgs.Empty);
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
		
	}
}
