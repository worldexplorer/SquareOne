using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;
using Sq1.Core.Backtesting;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		Bars		preBacktestBars;
		DataSource	preDataSource;
		//LEAVE_IT_AS_USER_SELECTED	bool		preBacktestIsStreaming;

		internal void BacktestContext_initialize(Bars barsEmptyButWillGrow) {
			string msig = " //BacktestContextInitialize(" + barsEmptyButWillGrow + ")";

			DataSource dataSourceFromBars = this.DataSource_fromBars;

			// will only work for DataSource.Streaming=StreamingLivesimDefault; for quik will throw
			//if (dataSourceFromBars == null) {
			//	string msg = "MUST_NEVER_BE_NULL DataSource_fromBars[" + dataSourceFromBars + "]";
			//	Assembler.PopupException(msg + msig);
			//	this.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted(msg + msig, 0);
			//	return;
			//}
			//if (dataSourceFromBars.StreamingAsBacktest_nullUnsafe == null) {
			//	string msg = "MUST_NEVER_BE_NULL DataSource_fromBars.StreamingAsBacktest_nullUnsafe[" + dataSourceFromBars.StreamingAsBacktest_nullUnsafe + "]";
			//	Assembler.PopupException(msg + msig);
			//	this.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted(msg + msig, 0);
			//	return;
			//}

			dataSourceFromBars.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this, barsEmptyButWillGrow);

			this.preBacktestBars = this.Bars;	// this.preBacktestBars != null will help ignore this.IsStreaming saving IsStreaming state to json
			this.preDataSource = this.DataSource_fromBars;
			// LEAVE_IT_AS_USER_SELECTED this.preBacktestIsStreaming = this.IsStreamingTriggeringScript;

			if (this.Bars == barsEmptyButWillGrow) {
				string msg = "LIFECYCLE_INCONSISTENT__BARS_ALREADY_INITIALIZED " + this.Bars;
				Assembler.PopupException(msg);
			} else {
				this.Bars = barsEmptyButWillGrow;
				bool indicatorsHaveNoErrorsCanStartBacktesting = true;
				foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_ReflectedCached.Values) {
					indicatorsHaveNoErrorsCanStartBacktesting &= indicator.BacktestStartingConstructOwnValuesValidateParameters(this);
				}
				if (indicatorsHaveNoErrorsCanStartBacktesting == false) {
					string msg = "I_SHOULD_ABORT_BACKTEST_NOW_HERE_BUT_DONT_HAVE_A_MECHANISM indicatorsHaveNoErrorsCanStartBacktesting=false";
					Assembler.PopupException(msg);
					throw new Exception(msg);
				}
			}
			// LEAVE_IT_AS_USER_SELECTED
			//if (this.preBacktestBars != null) {
			//	string msg = "NOT_SAVING_IsStreamingTriggeringScript=ON_FOR_BACKTEST"
			//		+ " preBacktestIsStreaming[" + this.preBacktestIsStreaming + "] preBacktestBars[" + this.preBacktestBars + "]";
			//	//Assembler.PopupException(msg, null, false);
			//}
			//this.IsStreamingTriggeringScript = true;

			//this.Strategy.ScriptBase.Initialize(this);

			this.EventGenerator.RaiseOnBacktesterSimulationContextInitialized_step2of4();
		}
		internal void BacktestContext_restore() {
			this.Bars = this.preBacktestBars;
			string msig = " //BacktestContextRestore(" + this.Bars + ")";
			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_ReflectedCached.Values) {
				if (indicator.OwnValuesCalculated.Count != this.Bars.Count) {
					string state = "MA.OwnValues.Count=499, MA.BarsEffective.Count=500[0...499], MA.BarsEffective.BarStreaming=null <= that's why indicator has 1 less";
					string msg = "YOU_ABORTED_LIVESIM_BUT_DIDNT_RECALCULATE_INDICATORS? REMOVE_HOLES_IN_INDICATOR " + indicator;
					Assembler.PopupException(msg + msig, null, false);
				}
				indicator.BacktestContextRestoreSwitchToOriginalBarsContinueToLiveNorecalculate();
			}

			//this.DataSource = this.preDataSource;
			// LEAVE_IT_AS_USER_SELECTED this.IsStreamingTriggeringScript = preBacktestIsStreaming;
			// MOVED_HERE_AFTER_ASSIGNING_IS_STREAMING_TO"avoiding saving strategy each backtest due to streaming simulation switch on/off"
			this.preBacktestBars = null;	// will help ignore this.IsStreaming saving IsStreaming state to json

			DataSource dataSourceFromBars = this.DataSource_fromBars;
			// will only work for DataSource.Streaming=StreamingLivesimDefault; for quik will throw
			//if (dataSourceFromBars == null) {
			//	string msg = "MUST_NEVER_BE_NULL DataSource_fromBars[" + dataSourceFromBars + "]";
			//	Assembler.PopupException(msg + msig);
			//	this.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted(msg + msig, 0);
			//	return;
			//}
			//if (dataSourceFromBars.StreamingAsBacktest_nullUnsafe == null) {
			//	string msg = "MUST_NEVER_BE_NULL BacktestDataSource_fromBars_nullUnsafe.StreamingAsBacktest_nullUnsafe[" + this.DataSource_fromBars.StreamingAsBacktest_nullUnsafe + "]";
			//	Assembler.PopupException(msg + msig);
			//	this.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted(msg + msig, 0);
			//	return;
			//}

			dataSourceFromBars.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this);

			this.EventGenerator.RaiseOnBacktesterContextRestoredAfterExecutingAllBars_step4of4(null);
		}
		public void Backtester_abortIfRunning_restoreContext() {
			if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == false) return;
			// TODO INTRODUCE_NEW_MANUAL_RESET_SO_THAT_NEW_BACKTEST_WAITS_UNTIL_TERMINATION_OF_THIS_METHOD_TO_AVOID_BROKEN_DISTRIBUTION_CHANNELS
			this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted("USER_CHANGED_SELECTORS_IN_GUI_NEW_BACKTEST_IS_ALMOST_TASK.SCHEDULED");
			//ALREADY_RESTORED_BY_simulationPostBarsRestore() this.BacktestContextRestore();
		}
		public void BacktesterRunSimulation_threadEntry_exceptionCatcher() {
			Exception backtestException = null;
			try {
				this.barStaticExecutedLast = null;
				this.ExecutionDataSnapshot.Initialize();
				this.PerformanceAfterBacktest.Initialize();
				this.Strategy.Script.InitializeBacktestWrapper();

				if (this.ChartShadow != null) this.ChartShadow.SetIndicators(this.Strategy.Script.IndicatorsByName_ReflectedCached);

				this.BacktesterOrLivesimulator.InitializeAndRun_step1of2();
			} catch (Exception exBacktest) {
				backtestException = exBacktest;
				string msg = "BACKTEST_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]";
				Assembler.PopupException(msg, exBacktest);
			}
			
			if (backtestException == null) {
				if (this.BacktesterOrLivesimulator.ImRunningLivesim == false) {		// && this.WasBacktestAborted
					try {
						this.PerformanceAfterBacktest.BuildStatsOnBacktestFinished();
					} catch (Exception exPerformance) {
						string msg = "PERFORMANCE_THREW_AFTER_BACKTEST_FINISHED_OKAY__NOT_RE-THROWING_NEED_TO_RESTORE_BACKTEST_CONTEXT_FINALLY";
						Assembler.PopupException(msg, exPerformance);
						//throw new Exception(msg, exPerformance);
					}
				}
			} else {
				string msg = "NOT_BUILDING_PERFORMANCE_ON_FAILED_BACKTEST___DOOMED_TO_THROW_AS_WELL";
				Assembler.PopupException(msg, backtestException);
			}
			try {
				this.BacktesterOrLivesimulator.BacktestRestore_step2of2();
			} catch (Exception exRestoringBars) {
				string msg = "BARS_RESTORE_AFTER_BACKTEST_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]";
				Assembler.PopupException(msg, exRestoringBars);
			}
			if (this.ChartShadow != null) this.ChartShadow.PaintAllowedDuringLivesimOrAfterBacktestFinished = true;
		}

		public void Livesim_compileRun_trampoline(Action<ScriptExecutor> executeAfterSimulationEvenIfIFailed = null, bool inNewThread = true) {
			// copypaste & refactor from ChartFormManager:StrategyCompileActivateBeforeShow()
			if (this.Strategy.ActivatedFromDll == false) {
				if (string.IsNullOrEmpty(this.Strategy.ScriptSourceCode)) {
					string msg = "WONT_COMPILE_STRATEGY_HAS_EMPTY_SOURCE_CODE_PLEASE_TYPE_SOMETHING";
					Assembler.PopupException(msg, null, false);
					return;
				}
				this.Strategy.CompileInstantiate();
				if (this.Strategy.Script != null) {
					this.Strategy.Script.Initialize(this);
					this.Sequencer.RaiseScriptRecompiledUpdateHeaderPostponeColumnsRebuild();
					this.Sequencer.Initialize();
				}
			}
			this.BacktesterRun_trampoline(executeAfterSimulationEvenIfIFailed, inNewThread);
		}

		public void BacktesterRun_trampoline(Action<ScriptExecutor> executeAfterSimulationEvenIfIFailed = null, bool inNewThread = true) {
			if (this.Strategy == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy=null; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.Strategy.Script == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy.Script=null, didn't compile; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.Bars == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Bars=null; select 1) TimeFrame 2) Range 3) PositionSize - for corresponding Chart; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted("ALREADY_BACKTESTING_this.Backtester.IsBacktestingNow");
			}

			//???????
			//AFTER F6 I want to run backtest with one slider changed; I click on the slider and get "did you forget to initialize Executor?..." error
			//TOO_LATE_MOVED_TO_AFTER_Strategy.CompileInstantiate() this.Strategy.Script.Initialize(this);
			// only to reset the Glyphs and Positions
			//this.ChartForm.Chart.Renderer.InitializeBarsInvalidateChart(this.Executor);
			//this.Executor.Renderer.InitializeBarsInvalidateChart(this.Executor);
			if (this.ChartShadow != null) this.ChartShadow.ClearAllScriptObjectsBeforeBacktest();

			//if (this.Strategy.ActivatedFromDll) {
				// FIXED "EnterEveryBar doesn't draw MAfast"; editor-typed strategies already have indicators in SNAP after pre-backtest compilation
				// DONT_COMMENT_LINE_BELOW indicators get lost when BacktestOnRestart = true
				//SEQUENCER_ALREADY_DONE_IT_CloneForSequencer this.Strategy.Script.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
			//}
			//SEQUENCER_ALREADY_DONE_IT_CloneForSequencer this.Strategy.ScriptParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy();
			
			//inNewThread = false;
			if (inNewThread) {
				int ThreadPoolAvailablePercentageLimit = 20;
				int threadPoolAvailablePercentage = this.getThreadPoolAvailable_percentage();
				if (threadPoolAvailablePercentage < ThreadPoolAvailablePercentageLimit) {
					string msg = "NOT_SCHEDULING_RUN_SIMULATION QueueUserWorkItem(backtesterRunSimulationThreadEntryPoint)"
						+ " because threadPoolAvailablePercentage[" + threadPoolAvailablePercentage
						+ "]<" + ThreadPoolAvailablePercentageLimit + "%";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				//this.MainForm.PopupException("SCHEDULING_RUN_SIMULATION for Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]");

				//v1
				//ThreadPool.QueueUserWorkItem(new WaitCallback(this.backtesterRunSimulationThreadEntryPoint));
				
				//v2
				//http://stackoverflow.com/questions/7582853/what-wpf-threading-approach-should-i-go-with/7584422#7584422
				//Task.Factory.StartNew(() => {
				//	// Background work
				//	this.backtesterRunSimulationThreadEntryPoint();
				//}).ContinueWith((t) => {
				//	// Update UI thread
				//	executeAfterSimulation();
				//}, TaskScheduler.FromCurrentSynchronizationContext());

				//v3
				Task backtesterTask = new Task(this.BacktesterRunSimulation_threadEntry_exceptionCatcher);
				if (executeAfterSimulationEvenIfIFailed != null) {
					backtesterTask.ContinueWith((t) => {
						executeAfterSimulationEvenIfIFailed(this);
						//this.PerformanceAfterBacktest = new SystemPerformance(this);	// MULTITHREADING_ISSUE__YOU_MUST_PASS_CLONE_AND_THEN_LET_OTHER_DISPOSABLE_EXECUTOR_TO_RUN_ANOTHER_BACKTEST
					});
				}
				//ON_REQUESTING_ABORT_TASK_DIES_WITHOUT_INVOKING_CONTINUE_WITH started.Start(TaskScheduler.FromCurrentSynchronizationContext());
				backtesterTask.Start();		// WHO_DOES t.Dispose() ?
			} else {
				//this.Executor.BacktesterRunSimulation();
				//this.ChartForm.Chart.DoInvalidate();
				this.BacktesterRunSimulation_threadEntry_exceptionCatcher();
				if (executeAfterSimulationEvenIfIFailed != null) {
					executeAfterSimulationEvenIfIFailed(this);
				}
			}
		}
		int getThreadPoolAvailable_percentage() {
			int workerThreadsAvailable, completionPortThreadsAvailable = 0;
			ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
			int workerThreadsMax, completionPortThreadsMax = 0;
			ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);
			return (completionPortThreadsMax / completionPortThreadsAvailable) * 100;
		}
	}
}
