using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		Bars		preBacktestBars;
		DataSource	preDataSource;

		internal void BacktestContext_initialize(Bars barsEmptyButWillGrow) {
			string msig = " //BacktestContext_initialize(" + barsEmptyButWillGrow + ")";

			DataSource dataSourceFromBars = this.DataSource_fromBars;
			dataSourceFromBars.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this, barsEmptyButWillGrow);

			this.preBacktestBars = this.Bars;	// this.preBacktestBars != null will help ignore this.IsStreaming saving IsStreaming state to json
			this.preDataSource = this.DataSource_fromBars;
			// LEAVE_IT_AS_USER_SELECTED this.preBacktestIsStreaming = this.IsStreamingTriggeringScript;

			if (this.Bars == barsEmptyButWillGrow) {
				string msg = "LIFECYCLE_INCONSISTENT__BARS_ALREADY_INITIALIZED " + this.Bars;
				Assembler.PopupException(msg);
				this.EventGenerator.RaiseOnBacktesterSimulationContextInitialized_step2of4();
				return;
			}

			this.Bars = barsEmptyButWillGrow;
			bool indicatorsHaveNoErrorsCanStartBacktesting = true;

			if (this.Strategy.Script.IndicatorsByName_reflectedCached_primary.Count == 0) {
				this.Strategy.Script.IndicatorsByName_reflectionForced_byClearingCache();		// otherwize no Indicators are seen inside InvokeIndicators*()
			}

			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_reflectedCached_primary.Values) {
				if (indicator.Executor != null) {
					string msg = "already initialized Executor and OwnValues in PreCalculateIndicators_forLoadedBars_backtestWontFollow()" + indicator;
					//INIT_ANYWAY Assembler.PopupException(msg, null, false);
					//INIT_ANYWAY continue;
				}
					
				indicator.Initialize(this);
				indicatorsHaveNoErrorsCanStartBacktesting &= indicator.BacktestStarting_validateParameters();
			}
			if (indicatorsHaveNoErrorsCanStartBacktesting == false) {
				string msg = "I_SHOULD_ABORT_BACKTEST_NOW_HERE_BUT_DONT_HAVE_A_MECHANISM indicatorsHaveNoErrorsCanStartBacktesting=false";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			this.EventGenerator.RaiseOnBacktesterSimulationContextInitialized_step2of4();
		}
		internal void BacktestContext_restore() {
			if (this.preBacktestBars == null) {
				string msg = "AVOIDING_NPE YOU_DIDNT_BACKUP_BARS_BEFORE_LIVESIM";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.Bars = this.preBacktestBars;
			string msig = " //BacktestContextRestore(" + this.Bars + ")";
			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_reflectedCached_primary.Values) {
				if (indicator.OwnValuesCalculated.Count != this.Bars.Count) {
					string state = "MA.OwnValues.Count=499, MA.BarsEffective.Count=500[0...499], MA.BarsEffective.BarStreaming=null <= that's why indicator has 1 less";
					string msg = "YOU_ABORTED_LIVESIM_BUT_DIDNT_RECALCULATE_INDICATORS? REMOVE_HOLES_IN_INDICATOR " + indicator;
					Assembler.PopupException(msg + msig, null, false);
				}
				indicator.BacktestContextRestore_backToOriginalBarsEffectiveProxy_continueToLive_noRecalculate();
			}

			this.preBacktestBars = null;	// will help ignore this.IsStreaming saving IsStreaming state to json

			DataSource dataSourceFromBars = this.DataSource_fromBars;
			dataSourceFromBars.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this);

			this.EventGenerator.RaiseOnBacktesterContextRestoredAfterExecutingAllBars_step4of4(null);
		}
		public void Backtester_abortIfRunning_restoreContext() {
			if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) return;
			// TODO INTRODUCE_NEW_MANUAL_RESET_SO_THAT_NEW_BACKTEST_WAITS_UNTIL_TERMINATION_OF_THIS_METHOD_TO_AVOID_BROKEN_DISTRIBUTION_CHANNELS
			this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted("USER_CHANGED_SELECTORS_IN_GUI_NEW_BACKTEST_IS_ALMOST_TASK.SCHEDULED");
			//ALREADY_RESTORED_BY_simulationPostBarsRestore() this.BacktestContextRestore();
		}
		public void BacktesterRunSimulation_threadEntry_exceptionCatcher() {
			Exception backtestException = null;
			try {
				this.barStatic_lastExecuted = null;
				this.ExecutionDataSnapshot.Initialize();
				this.PerformanceAfterBacktest.Initialize();
				this.Strategy.Script.InitializeBacktestWrapper();

				// better place is BacktestRestore_step2of2 but may be Script will address ChartShadow.Indicators?... who knows
				if (this.ChartShadow != null) this.ChartShadow.SetIndicators(this.Strategy.Script.IndicatorsByName_reflectedCached_primary);

				this.BacktesterOrLivesimulator.Initialize_runSimulation_backtestAndLivesim_step1of2();
			} catch (Exception exBacktest) {
				backtestException = exBacktest;
				string msg = "BACKTEST_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]";
				Assembler.PopupException(msg, exBacktest);
			}
			
			if (backtestException == null) {
				//if (this.BacktesterOrLivesimulator.ImRunningLivesim == false) {					// && this.WasBacktestAborted
				if (this.BacktesterOrLivesimulator.IsLivesimulator == false) {	// && this.WasBacktestAborted
					try {
						this.PerformanceAfterBacktest.BuildStats_onBacktestFinished();
					} catch (Exception exPerformance) {
						string msg = "PERFORMANCE_THREW_AFTER_BACKTEST_FINISHED_OKAY__NOT_RE-THROWING_NEED_TO_RESTORE_BACKTEST_CONTEXT_FINALLY";
						Assembler.PopupException(msg, exPerformance);
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

			if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
				this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted("ALREADY_BACKTESTING_this.Backtester.IsBacktestingNow");
			}

			if (this.ChartShadow != null) this.ChartShadow.Clear_allScriptObjects_beforeBacktest();

			//inNewThread = false;
			if (inNewThread) {
				int threadPoolAvailablePercentageLimit = 20;
				int threadPoolAvailablePercentage = this.getThreadPoolAvailable_percentage();
				if (threadPoolAvailablePercentage < threadPoolAvailablePercentageLimit) {
					string msg = "NOT_SCHEDULING_RUN_SIMULATION QueueUserWorkItem(backtesterRunSimulationThreadEntryPoint)"
						+ " because threadPoolAvailablePercentage[" + threadPoolAvailablePercentage
						+ "]<" + threadPoolAvailablePercentageLimit + "%";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				//this.MainForm.PopupException("SCHEDULING_RUN_SIMULATION for Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]");

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
