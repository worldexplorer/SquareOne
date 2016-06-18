using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.Livesim;
using Sq1.Core.Execution;

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
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (this.Strategy.Script == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy.Script=null, didn't compile; " + this;
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (this.Bars == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Bars=null; select 1) TimeFrame 2) Range 3) PositionSize - for corresponding Chart; " + this;
				Assembler.PopupException(msg);
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
				Assembler.PopupException(msg);
					throw new Exception(msg);
				}
				//this.MainForm.PopupException("SCHEDULING_RUN_SIMULATION for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]");

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
				//this.BacktesterRunSimulation();
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



		internal void CloseOpenPositions_backtestEnded() {
			//return;
			List<Alert> alertsSafe = this.ExecutionDataSnapshot.AlertsUnfilled.SafeCopy(this, "CloseOpenPositions_backtestEnded(WAIT)");
			foreach (Alert alertPending in alertsSafe) {
				try {
					//if (alertPending.IsEntryAlert) {
					//	this.ClosePositionWithAlertClonedFromEntryBacktestEnded(alertPending);
					//} else {
					//	string msg = "checkPositionCanBeClosed() will later interrupt the flow saying {Sorry I don't serve alerts.IsExitAlert=true}";
					//	this.RemovePendingExitAlertPastDueClosePosition(alertPending);
					//}
					//bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alertPending);
					this.AlertPending_kill(alertPending);
				} catch (Exception ex) {
					string msg = "NOT_AN_ERROR BACKTEST_POSITION_FINALIZER: check innerException: most likely you got POSITION_ALREADY_CLOSED on counterparty alert's force-close?";
					this.PopupException(msg, ex, false);
				}
			}
			if (this.ExecutionDataSnapshot.AlertsUnfilled.Count > 0) {
				string msg = "KILLING_LEFTOVER_ALERTS_DIDNT_WORK_OUT snap.AlertsPending.Count["
					+ this.ExecutionDataSnapshot.AlertsUnfilled.Count + "] should be ZERO";
				Assembler.PopupException(msg, null, false);
			}

			List<Position> positionsSafe = this.ExecutionDataSnapshot.Positions_OpenNow.SafeCopy(this, "closePositionsLeftOpenAfterBacktest(WAIT)");
			foreach (Position positionOpen in positionsSafe) {
				//v1 List<Alert> alertsSubmittedToKill = this.Strategy.Script.PositionCloseImmediately(positionOpen, );
				//v2 WONT_CLOSE_POSITION_EARLIER_THAN_OPENED Alert exitAlert = this.Strategy.Script.ExitAtMarket(this.Bars.BarStaticLast_nullUnsafe, positionOpen, "BACKTEST_ENDED_EXIT_FORCED");
				Alert exitAlert = this.Strategy.Script.ExitAtMarket(this.Bars.BarStreaming_nullUnsafe, positionOpen, "BACKTEST_ENDED_EXIT_FORCED");
				if (exitAlert != positionOpen.ExitAlert) {
					string msg = "FIXME_SOMEHOW";
					Assembler.PopupException(msg);
				}
				// BETTER WOULD BE TO KILL PREVIOUS PENDING ALERT FROM A CALLBACK AFTER MARKET EXIT ORDER GETS FILLED, IT'S UNRELIABLE EXIT IF WE KILL IT HERE
				// LOOK AT EMERGENCY CLASSES, SOLUTION MIGHT BE THERE ALREADY
				//List<Alert> alertsSubmittedToKill = this.Strategy.Script.PositionKillExitAlert(positionOpen, "BACKTEST_ENDED_EXIT_FORCED");
				//v3 this.ExecutionDataSnapshot.MovePositionOpenToClosed(positionOpen);
				//v4
				if (positionOpen.ExitAlert == null) continue;
				try {
					this.RemovePendingExitAlerts_closePositionsBacktestLeftHanging(positionOpen.ExitAlert);
				} catch (Exception ex) {
					Assembler.PopupException("closePositionsLeftOpenAfterBacktest()", ex, false);
				}
			}
			if (this.ExecutionDataSnapshot.Positions_OpenNow.Count > 0) {
				string msg = "DIDNT_CLOSE_BACKTEST_LEFTOVER_POSITIONS snap.PositionsOpenNow.Count["
					+ this.ExecutionDataSnapshot.Positions_OpenNow.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
		}

		internal void LivesimEnded_invalidateUnfilledOrders_ClearPendingAlerts() {
			string msig = " //LivesimEnded_invalidateUnfilledOrders_ClearPendingAlerts() " + this.ToString();
			List<Alert> mostLikelyUnfilled = this.ExecutionDataSnapshot.AlertsUnfilled.SafeCopy(this, msig);
			foreach (Alert alertWithUnfilledOrder in mostLikelyUnfilled) {
				Order unfilled = alertWithUnfilledOrder.OrderFollowed;
				if (unfilled == null) continue;

				OrderStateMessage omsg = new OrderStateMessage(unfilled, OrderState.Invalidated_LivesimEnded, msig);
				this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
			}
			this.OrderProcessor.OPPexpired.AllTimers_stopDispose_LivesimEnded(mostLikelyUnfilled, msig);
			// some other PostProcessors might need to clear their queues
		}
	}
}
