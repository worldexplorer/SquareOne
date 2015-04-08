using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Execution;

namespace Sq1.Core.Livesim {
	public class Livesimulator : Backtester {
		const string REASON_TO_EXIST = "VISUALLY CONFIRM THAT CORE CAN COPE WITH FOUR THREADS SIMULTANEOUSLY:"
			+ " 1) STREAMING_ADAPTER"
			+ " 2) ITS QUOTE_PUMP INVOKING SCRIPT.OVERRIDES"
			+ " 3) BROKER_ADAPTER ALSO INVOKING SCRIPT.OVERRIDES WITHOUT TASKS"
			+ " 4) CHARTING/REPORTING IN GUI THREAD;"
			+ " ALL OF THOSE SEQUENTIALLY AND NON-CONTRADICTIVELY DEALING WITH EXECUTION_DATA_SNAPSHOT"
			+ " AND OTHER INTERNALS AT 1000 QUOTES/SECOND WOULD BE CONSIDERED A HUGE SUCCESS";

		public	Backtester				BacktesterBackup				{ get; private set; }
		public	LivesimDataSource		DataSourceAsLivesimNullUnsafe	{ get { return base.BacktestDataSource as LivesimDataSource; } }
				//v1
				// Button					btnStartStop;
				// Button					btnPauseResume;
				//v2
				ToolStripButton			btnStartStop;
				ToolStripButton			btnPauseResume;

				ChartShadow				chartShadow;
				LivesimQuoteBarConsumer livesimQuoteBarConsumer;


		public Livesimulator(ScriptExecutor executor) : base(executor) {
			base.BacktestDataSource = new LivesimDataSource(executor);
			base.BacktestDataSource.Initialize(Assembler.InstanceInitialized.OrderProcessor);
			//base.SeparatePushingThreadEnabled = false;
			this.livesimQuoteBarConsumer = new LivesimQuoteBarConsumer(this);
			// DONT_MOVE_TO_CONSTRUCTOR!!!WORKSPACE_LOAD_WILL_INVOKE_YOU_THEN!!! base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
		}

		protected override void SimulationPreBarsSubstitute_overrideable() {
			if (base.BarsOriginal == base.Executor.Bars) {
				string msg = "DID_YOU_FORGET_TO_RESET_base.BarsOriginal_TO_NULL_AFTER_BACKTEST_FINISHED??";
				Assembler.PopupException(msg);
			}
			try {
				base.BarsOriginal	= base.Executor.Bars;
				base.BarsSimulating = base.Executor.Bars.CloneNoBars(BARS_BACKTEST_CLONE_PREFIX + base.BarsOriginal);
				base.Executor.EventGenerator.RaiseOnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4();
				
				#region candidate for this.DataSourceAsLivesimNullUnsafeBuildFromUserSelection()
				BacktestSpreadModeler spreadModeler;
				// kept it on the surface and didn't pass ScriptContextCurrent.SpreadModelerPercent to "new DataSourceAsLivesimNullUnsafe()" because later DataSourceAsLivesimNullUnsafe:
				// 1) will support different SpreadModelers with not only 1 parameter like SpreadModelerPercentage;
				// 2) will support different BacktestModes like 12strokes, not only 4Stroke 
				// 3) will poke StreamingAdapter-derived implementations 12 times a bar with platform-generated quotes for backtests with regulated poke delay
				// 4) will need to be provide visualized 
				// v1 this.DataSourceAsLivesimNullUnsafe.BacktestStreamingAdapter.InitializeSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
				// v2 UI-controlled in the future, right now the stub  
				ContextScript ctx = base.Executor.Strategy.ScriptContextCurrent;
				string msig = "Strategy[" + base.Executor.Strategy + "].ScriptContextCurrent[" + ctx + "]";
				switch (ctx.SpreadModelerClassName) {
					case "BacktestSpreadModelerPercentage":
						spreadModeler = new BacktestSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
					default:
						string msg = "SPREAD_MODELER_NOT_YET_SUPPORTED[" + ctx.SpreadModelerClassName + "]"
							+ ", instantiatind default BacktestSpreadModelerPercentage("
							+ base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent + ")";
						Assembler.PopupException(msg + msig);
						spreadModeler = new BacktestSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
				}
				this.DataSourceAsLivesimNullUnsafe.Initialize(base.BarsSimulating, spreadModeler);
				#endregion

				base.BarsSimulating.DataSource = this.DataSourceAsLivesimNullUnsafe;

				DataDistributor distr = this.DataSourceAsLivesimNullUnsafe.StreamingAdapter.DataDistributor;
				distr.ConsumerQuoteSubscribe(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer, false);
				distr.ConsumerBarSubscribe	(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer, false);
				//streaming.SetQuotePumpThreadNameSinceNoMoreSubscribersWillFollowFor(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval);

				base.Executor.BacktestContextInitialize(base.BarsSimulating);

				if (base.BarsOriginal == null) {
					string msg = "consumers will expect base.BarsOriginal != null";
					Assembler.PopupException(msg);
				}
				if (base.BarsOriginal.Count == 0) {
					string msg = "consumers will expect base.BarsOriginal.Count > 0";
					Assembler.PopupException(msg);
				}
				if (base.BarsSimulating == null) {
					string msg = "consumers will expect base.BarsSimulating != null";
					Assembler.PopupException(msg);
				}
				if (base.BarsSimulating.Count > 0) {
					string msg = "consumers will expect base.BarsSimulating.Count = 0";
					Assembler.PopupException(msg);
				}
				if (base.Executor.Bars == null) {
					string msg = "consumers will expect base.Bars != null";
					Assembler.PopupException(msg);
				}
				if (base.Executor.Bars.Count > 0) {
					string msg = "consumers will expect base.Bars.Count = 0";
					Assembler.PopupException(msg);
				}
			} catch (Exception ex) {
				string msg = "PreBarsSubstitute(): Backtester caught a long beard...";
				base.Executor.PopupException(msg, ex);
			} finally {
				base.BarsSimulatedSoFar = 0;
				base.BacktestWasAbortedByUserInGui = false;
				base.BacktestAborted.Reset();
				base.RequestingBacktestAbort.Reset();
				base.BacktestIsRunning.Set();
				if (this.IsBacktestRunning == false) {
					string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
					Assembler.PopupException(msg);
				}

				//COPIED_UPSTACK_FOR_BLOCKING_MOUSEMOVE_AFTER_BACKTEST_NOW_CLICK__BUT_ALSO_STAYS_HERE_FOR_SLIDER_CHANGE_NON_INVALIDATION
				//WONT_BE_RESET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
				//if (base.Executor.ChartShadow != null) base.Executor.ChartShadow.PaintAllowedAfterBacktestFinished = true;
			}
		}
		protected override void SimulationPostBarsRestore_overrideable() {
			try {
				//v2 moved from 10 lines below and added unsyncHappenedNotAsResultOfAbort to IndicatorMovingAverageSimple.checkPopupOnResetAndSync()
				if (base.BacktestWasAbortedByUserInGui) {
					base.BacktestAborted.Set();
					base.RequestingBacktestAbort.Reset();
				}

				LivesimStreaming streamingBacktest = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe;
				StreamingAdapter streamingOriginal = base.BarsOriginal.DataSource.StreamingAdapter;
				string msg = "NOW_INSERT_BREAKPOINT_TO_this.channel.PushQuoteToConsumers(quoteDequeued) CATCHING_BACKTEST_END_UNPAUSE_PUMP";
				//if (streamingOriginal.

				if (base.WasBacktestAborted) {
					string msg2 = "NOT_ABSORBING_LAST_LIVESIM_STREAMING_INTO_BARS_ORIGINAL__SOLIDIFIER_WILL_COMPLAIN_OTHERWISE";
					Assembler.PopupException(msg2, null, false);
				} else {
					string msg2 = "BRO_THIS_IS_NONSENSE!!!";
					//streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(
					//	streamingBacktest, base.BarsOriginal.Symbol, base.BarsOriginal.ScaleInterval);
				}

				DataDistributor distr = this.DataSourceAsLivesimNullUnsafe.StreamingAdapter.DataDistributor;
				distr.ConsumerQuoteUnsubscribe(base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);
				distr.ConsumerBarUnsubscribe  (base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);

				//if (this.Executor.Backtester.QuotesGenerator.BacktestStrokesPerBar != this.Executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar) {
				//	string msg2 = "PARANOID_CHECK QuotesGenerator.BacktestStrokesPerBar[" + this.Executor.Backtester.QuotesGenerator.BacktestStrokesPerBar
				//		+ "] != ScriptContextCurrent.BacktestStrokesPerBar[" + this.Executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar + "]";
				//	msg += "BacktestContextRestore will DisplayStatus how many StrokesPerBar was backtested; but since in GUI thread QuoteGenerator will be reset, I do equality check here :(";
				//	Assembler.PopupException(msg2);
				//}

				double sec = Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				string strokesPerBar = base.QuotesGenerator.BacktestStrokesPerBar + "/Bar";
				string stats = "Livesim took [" + sec + "]sec at " + strokesPerBar;
				this.Executor.LastBacktestStatus = stats + this.Executor.LastBacktestStatus;

				// down there, OnAllBarsBacktested will be raised and ChartFormManager will push performance to reporters.
				base.Executor.BacktestContextRestore();

				//if (this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.settings.DelayBetweenSerialQuotesEnabled) {
				if (base.Executor.Strategy.LivesimStreamingSettings.DelayBetweenSerialQuotesEnabled == false) {
					base.Executor.OrderProcessor.RaiseDelaylessLivesimEndedShouldRebuildOLV(this);
				}
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			} finally {
				// Calling ManualResetEvent.Set opens the gate,
				// allowing any number of threads calling WaitOne to be let through
				//moved to this.NotifyWaitingThreads()
				//base.BacktestCompletedQuotesCantGo.Set();
				//v2 moved to 10 lines above and added unsyncHappenedNotAsResultOfAbort to IndicatorMovingAverageSimple.checkPopupOnResetAndSync()
				//if (base.BacktestWasAbortedByUserInGui) {
				//	base.BacktestAborted.Set();
				//	base.RequestingBacktestAbort.Reset();
				//}
				//v3 moved from Stop_inGuiThread koz there it's too early
				base.Executor.Backtester = this.BacktesterBackup;
			}
		}


		//v1 public void Start_inGuiThread(Button btnStartStop, Button btnPauseResume, ChartShadow chartShadow) {
		public void Start_inGuiThread(ToolStripButton btnStartStop, ToolStripButton btnPauseResume, ChartShadow chartShadow) {
			this.btnStartStop = btnStartStop;
			this.btnPauseResume = btnPauseResume;
			this.chartShadow = chartShadow;
			this.chartShadow.RangeBarCollapseToAccelerateLivesim();
			this.BacktesterBackup = base.Executor.Backtester;
			base.Executor.Backtester = this;

			// DONT_MOVE_TO_CONSTRUCTOR!!!WORKSPACE_LOAD_WILL_INVOKE_IT_THEN_INAPPROPRIETLY!!!  WITHOUT_UNSUBSCRIPTON_I_WAS_GETTING_MANY_INVOCATIONS_BAD
			base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 -= new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
			base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
	
			base.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
		}

		void executor_BacktesterContextInitializedStep2of4(object sender, EventArgs e) {
			if (this.chartShadow.InvokeRequired) {
				// will always InvokeRequied since we RaiseOnBacktesterSimulationContextInitialized_step2of4
				// from a just started thread with a new Backtest BacktesterRunSimulation_threadEntry_exceptionCatcher() SEE_CALL_STACK_NOW
				// too late to do it in GUI thread; switch takes a tons of time; do gui-unrelated preparations NOW
				List<Order> ordersStale = this.DataSourceAsLivesimNullUnsafe.BrokerAsLivesimNullUnsafe.OrdersSubmittedForOneLivesimBacktest;
				if (ordersStale.Count > 0) {
					int beforeCleanup = this.Executor.OrderProcessor.DataSnapshot.OrdersAll.Count;
					this.Executor.OrderProcessor.DataSnapshot.OrdersRemove(ordersStale);
					int afterCleanup = this.Executor.OrderProcessor.DataSnapshot.OrdersAll.Count;
					if (beforeCleanup > 0 && beforeCleanup <= afterCleanup)  {
						string msg = "STALE_ORDER_CLEANUP_DOESNT_WORK__LIVESIM";
						Assembler.PopupException(msg);
					}
					ordersStale.Clear();
				}
				base.Stopwatch.Restart();

				this.chartShadow.BeginInvoke((MethodInvoker)delegate { this.executor_BacktesterContextInitializedStep2of4(sender, e); });
				return;
			}
			this.chartShadow.Initialize(base.Executor.Bars, true);

			this.btnPauseResume.Enabled = true;
			this.btnPauseResume.Text = "Pause";
		}
		void afterBacktesterComplete(ScriptExecutor executorCompletePooled) {
			string msig = " //Livesimulator.afterBacktesterComplete()";

			if (executorCompletePooled == null) {
				string msg = "CAN_NOT_BE_NULL_executorCompletePooled";
				Assembler.PopupException(msg + msig);
				return;
			}
			//IM_UNPAUSED_AFTER_LIVESIM_FINISHED Assembler.PopupException("TIME_TO_UNPAUSE_ORIGNIAL_QUOTE_PUMP_executorCompletePooled: " + executorCompletePooled.ToStringWithCurrentParameters() + msig, null, false);

			base.Executor.Backtester = this.BacktesterBackup;
			base.BarsOriginal = null;	// I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL WILL_AFFECT_ChartForm.TsiProgressBarETA

			//v1 this.btnStartStop.BeginInvoke((MethodInvoker)delegate {
			this.chartShadow.BeginInvoke((MethodInvoker)delegate {
				this.btnStartStop.Text = "Start";
				this.btnStartStop.CheckState = CheckState.Unchecked;
				this.chartShadow.Initialize(base.Executor.Bars, true);

				float seconds = (float)Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				this.btnPauseResume.Text = seconds.ToString() + " sec";
				this.btnPauseResume.Enabled = false;
			});
		}

		public void Stop_inGuiThread() {
			string msig = "USER_CLICKED_STOP_IN_LIVESIM_FORM";
			try {
				ManualResetEvent unpausedMR = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused;
				bool isUnpaused = unpausedMR.WaitOne(0);
				if (isUnpaused == false) {
					string msg = "AVOIDING_TO_WAIT_FOREVER: LivesimStreaming.GeneratedQuoteEnrichSymmetricallyAndPush()";
					msg += " YOU_STOPPED_PAUSED_LIVESIM_FROM_GUI UNPAUSED_LIVESIM_QUOTE_PUMP__WOZU???_ORIGINAL_BARS_SOLIDIFIER_STILL_RUNNING_IN_ITS_THREAD";
					unpausedMR.Set();
				}
				string msg2 = " LIVESIM_HAS_SINGLE_THREADED_QUOTE_PUMP__DONT_REQUEST_AND_DONT_WAIT_JUST_CONTINUE";
				base.AbortRunningBacktestWaitAborted(msig + msg2, 0);
			} catch (Exception ex) {
				Assembler.PopupException("Livesimulator.Stop_inGuiThread()", ex);
			}
		}

		public void Pause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused.Reset();
			string msg = "AlertsScheduledForDelayedFill.Count=" + this.DataSourceAsLivesimNullUnsafe.BrokerAsLivesimNullUnsafe.DataSnapshot.AlertsScheduledForDelayedFill.Count  + " LEAKED_HANDLES_HUNTER";
			Assembler.PopupException(msg);
		}
		public void Unpause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused.Set();
		}
		public const string TO_STRING_PREFIX = "LIVESIMULATOR_FOR_";
		public bool LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
		public override string ToString() {
			string ret = TO_STRING_PREFIX + base.Executor.ToString();
			return ret;
		}
	}
}
