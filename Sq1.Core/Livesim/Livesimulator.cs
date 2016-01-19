using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Execution;

namespace Sq1.Core.Livesim {
	public class Livesimulator : Backtester, IDisposable {
		const string REASON_TO_EXIST = "VISUALLY CONFIRM THAT CORE CAN COPE WITH FOUR THREADS SIMULTANEOUSLY:"
			+ " 1) STREAMING_ADAPTER"
			+ " 2) ITS QUOTE_PUMP INVOKING SCRIPT.OVERRIDES"
			+ " 3) BROKER_ADAPTER ALSO INVOKING SCRIPT.OVERRIDES WITHOUT TASKS"
			+ " 4) CHARTING/REPORTING IN GUI THREAD;"
			+ " ALL OF THOSE SEQUENTIALLY AND NON-CONTRADICTIVELY DEALING WITH EXECUTION_DATA_SNAPSHOT"
			+ " AND OTHER INTERNALS AT 1000 QUOTES/SECOND WOULD BE CONSIDERED A HUGE SUCCESS";

		public	Backtester				BacktesterBackup				{ get; private set; }
		public	LivesimDataSource		DataSourceAsLivesimNullUnsafe	{ get { return base.BacktestDataSource as LivesimDataSource; } }

				ToolStripButton			btnStartStop;
				ToolStripButton			btnPauseResume;

				ChartShadow				chartShadow;
				LivesimQuoteBarConsumer livesimQuoteBarConsumer;


		public Livesimulator(ScriptExecutor executor) : base(executor) {
			base.BacktestDataSource			= new LivesimDataSource(executor);
			this.DataSourceAsLivesimNullUnsafe.Initialize(Assembler.InstanceInitialized.OrderProcessor);
			//base.SeparatePushingThreadEnabled = false;
			this.livesimQuoteBarConsumer	= new LivesimQuoteBarConsumer(this);
			// DONT_MOVE_TO_CONSTRUCTOR!!!WORKSPACE_LOAD_WILL_INVOKE_YOU_THEN!!! base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
		}

		[JsonIgnore]	public StreamingAdapter				StreamingOriginal;
		protected override void SimulationPreBarsSubstitute_overrideable() {
			if (base.BarsOriginal == base.Executor.Bars) {
				string msg = "DID_YOU_FORGET_TO_RESET_base.BarsOriginal_TO_NULL_AFTER_BACKTEST_FINISHED??";
				Assembler.PopupException(msg);
			}
			try {
				base.BarsOriginal	= base.Executor.Bars;
				base.BarsSimulating = base.Executor.Bars.CloneNoBars(BARS_BACKTEST_CLONE_PREFIX + base.BarsOriginal);
				base.Executor.EventGenerator.RaiseOnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4();
				
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
				// each time I change bars on chart switching to 
				//LivesimStreaming streamingAsLivesimChild = this.DataSourceAsLivesimNullUnsafe.StreamingAdapter_instantiatedForLivesim;
				//LivesimBroker		brokerAsLivesimChild = this.DataSourceAsLivesimNullUnsafe.BrokerAdapter_instantiatedForLivesim;
				//this.RedirectDataSource_reactivateLivesimsWithLivesimDataSource(streamingAsLivesimChild, brokerAsLivesimChild);
				this.DataSourceAsLivesimNullUnsafe.PropagatePreInstantiatedLivesimAdapter_intoLivesimDataSource();	// no need to restore (this.DataSourceAsLivesimNullUnsafe will be re-initialized at next Livesim)
				// NO!!! KEEP_THEM_TO_ORIGINAL_DATASOURCE_BECAUSE_DDE_SERVER_DELIVERS_LEVEL2_TO_ORIGINAL_DATA_SNAPSHOT base.BarsSimulating.DataSource = this.DataSourceAsLivesimNullUnsafe;	// will need to restore (base.BarsSimulating is not needed after Livesim is done)
				this.DataSourceAsLivesimNullUnsafe.Initialize(base.BarsSimulating, spreadModeler);
				this.StreamingOriginal = this.Executor.DataSource.StreamingAdapter;		// will have to restore
				
				if (this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe != null) {
					string msg = "LivesimStreaming_HAS_REFERENCE_TO_LivesimDataSource_SHOULD_BE_NO_NPE_IN_eventGenerator_OnStrategyExecutedOneQuote_unblinkDataSourceTree";
					Assembler.PopupException(msg, null, false);
				}
				// now I have those two assigned from Streaming/Broker-own-implemented instantiated adapters

				this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe	.InitializeLivesim	(this.DataSourceAsLivesimNullUnsafe, this.StreamingOriginal, base.BarsSimulating.Symbol);
				this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe	.UpstreamConnect_LivesimStarting();

				this.DataSourceAsLivesimNullUnsafe.BrokerAsLivesimNullUnsafe	.Initialize			(this.DataSourceAsLivesimNullUnsafe);

				//DataDistributor distr = this.StreamingOriginal.DataDistributor;
				//List<SymbolScaleDistributionChannel> mustBeOneTimeframe = distr.GetDistributionChannelsFor(base.Executor.Bars.Symbol);
				//if (mustBeOneTimeframe.Count != 1) {
				//    string msg1 = "BAD_JOB#1_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()";
				//    Assembler.PopupException(msg1);
				//} else {
				//    SymbolScaleDistributionChannel mustBeChartSubscribedToQuotesAndBars = mustBeOneTimeframe[0];
				//    if (mustBeChartSubscribedToQuotesAndBars.ConsumersQuoteCount != 1) {
				//        string msg1 = "BAD_JOB#2_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()";
				//        Assembler.PopupException(msg1);
				//    } else {
				//        bool mustBeChartSubscribedToQuotes = mustBeChartSubscribedToQuotesAndBars.ConsumersQuoteContains(base.Executor.ChartShadow.ChartStreamingConsumer);
				//        if (mustBeChartSubscribedToQuotes == false) {
				//            string msg1 = "BAD_JOB#3_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()";
				//            Assembler.PopupException(msg1);
				//        }
				//    }

				//    if (mustBeChartSubscribedToQuotesAndBars.ConsumersBarCount != 1) {
				//        string msg1 = "BAD_JOB#4_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()";
				//        Assembler.PopupException(msg1);
				//    } else {
				//        bool mustBeChartSubscribedToBars = mustBeChartSubscribedToQuotesAndBars.ConsumersBarContains(base.Executor.ChartShadow.ChartStreamingConsumer);
				//        if (mustBeChartSubscribedToBars == false) {
				//            string msg1 = "BAD_JOB#5_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()";
				//            Assembler.PopupException(msg1);
				//        }
				//    }
				//}

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

				this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.PushSymbolInfoToLevel2generator(this.Executor.Bars.SymbolInfo);
			} catch (Exception ex) {
				string msg = "PreBarsSubstitute(): Backtester caught a long beard...";
				base.Executor.PopupException(msg, ex);
			} finally {
				base.BarsSimulatedSoFar = 0;
				base.BacktestWasAbortedByUserInGui = false;
				base.BacktestAbortedMre.Reset();
				base.RequestingBacktestAbortMre.Reset();
				base.BacktestIsRunningMre.Set();
				if (this.IsBacktestRunning == false) {
					string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
					Assembler.PopupException(msg);
				}
			}
		}
		protected override void SimulationPostBarsRestore_overrideable() {
			try {
				if (base.WasBacktestAborted || base.RequestingBacktestAbortMre.WaitOne(0) == true) {
					string msg2 = "NOT_ABSORBING_LAST_LIVESIM_STREAMING_INTO_BARS_ORIGINAL__SOLIDIFIER_WILL_COMPLAIN_OTHERWISE";
					Assembler.PopupException(msg2, null, false);
				} else {
					string msg2 = "LOOKS_LIKE_WE_USED_UP_ALL_BARS_AND_SUCCESSFULLY_FINISHED_LIVESIM";
					//streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(
					//	streamingBacktest, base.BarsOriginal.Symbol, base.BarsOriginal.ScaleInterval);
				}

				double sec = Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				string strokesPerBar = base.QuotesGenerator.BacktestStrokesPerBar + "/Bar";
				string stats = "Livesim took [" + sec + "]sec at " + strokesPerBar;
				this.Executor.LastBacktestStatus = stats + this.Executor.LastBacktestStatus;

				// down there, OnAllBarsBacktested will be raised and ChartFormManager will push performance to reporters.
				base.Executor.BacktestContextRestore();

				this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe	.UpstreamDisconnect_LivesimTerminatedOrAborted();
				this.Executor.DataSource.StreamingAdapter = this.StreamingOriginal;

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
				base.Executor.Backtester = this.BacktesterBackup;
				if (base.BacktestWasAbortedByUserInGui) {
					base.BacktestAbortedMre.Set();
					base.RequestingBacktestAbortMre.Reset();
				}
			}
		}


		//v1 public void Start_inGuiThread(Button btnStartStop, Button btnPauseResume, ChartShadow chartShadow) {
		public void Start_inGuiThread(ToolStripButton btnStartStopPassed, ToolStripButton btnPauseResumePassed, ChartShadow chartShadow) {
			this.btnStartStop = btnStartStopPassed;
			this.btnPauseResume = btnPauseResumePassed;
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
			this.chartShadow.Initialize(base.Executor.Bars, base.Executor.StrategyName, false, true);

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
				this.chartShadow.Initialize(base.Executor.Bars, base.Executor.StrategyName, false, true);

				float seconds = (float)Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				this.btnPauseResume.Text = seconds.ToString() + " sec";
				this.btnPauseResume.Enabled = false;
			});
		}

		public void Stop_inGuiThread() {
			string msig = " //Livesimulator.Stop_inGuiThread()";
			if (this.IsBacktestingLivesimNow == false) {
				Assembler.PopupException("WHY_DID_YOU_INVOKE_ME? LIVESIM_IS_NOT_RUNNING_NOW" + msig);
				return;
			}
			try {
				if (this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.QuotePumpSeparatePushingThreadEnabled) {
					string msg = "YOU_STOPPED_PAUSED_LIVESIM_FROM_GUI UNPAUSED_LIVESIM_QUOTE_PUMP__WOZU???_ORIGINAL_BARS_SOLIDIFIER_STILL_RUNNING_IN_ITS_THREAD";
					ManualResetEvent unpausedMre = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.UnpausedMre;
					bool isUnpaused = unpausedMre.WaitOne(0);
					if (isUnpaused == false) {
						msg = "AVOIDING_TO_WAIT_FOREVER: LivesimStreaming.GeneratedQuoteEnrichSymmetricallyAndPush() " + msg;
						unpausedMre.Set();
					}
					base.AbortRunningBacktestWaitAborted(msg + msig);
				} else {
					string msg2 = "LIVESIM_HAS_SINGLE_THREADED_QUOTE_PUMP__NOT_EVEN_USED_IN_QuikLivesimStreaming DONT_REQUEST_AND_DONT_WAIT_JUST_CONTINUE";
					base.AbortRunningBacktestWaitAborted(msg2 + msig, 0);
				}
			} catch (Exception ex) {
				Assembler.PopupException("USER_OR_MAINFORM_CLICKED_STOP_IN_LIVESIM_FORM" + msig, ex);
			}
		}

		public void Pause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.UnpausedMre.Reset();
			string msg = "AlertsScheduledForDelayedFill.Count=" + this.DataSourceAsLivesimNullUnsafe.BrokerAsLivesimNullUnsafe.DataSnapshot.AlertsScheduledForDelayedFill.Count  + " LEAKED_HANDLES_HUNTER";
			//Assembler.PopupException(msg);
		}
		public void Unpause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.UnpausedMre.Set();
		}
		public const string TO_STRING_PREFIX = "LIVESIMULATOR_FOR_";
		public bool LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
		public override string ToString() {
			string ret = TO_STRING_PREFIX + base.Executor.ToString();
			return ret;
		}
	}
}
