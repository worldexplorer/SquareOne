using System;
using System.Windows.Forms;
using System.Threading;

using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Streaming;
using System.Diagnostics;

namespace Sq1.Core.Livesim {
	public class Livesimulator : Backtester {
		public	Backtester				BacktesterBackup				{ get; private set; }
		public	LivesimDataSource		DataSourceAsLivesimNullUnsafe	{ get { return base.BacktestDataSource as LivesimDataSource; } }
				Button					btnStartStop;
				ChartShadow				chartShadow;
				LivesimQuoteBarConsumer livesimQuoteBarConsumer;

		public Livesimulator(ScriptExecutor executor) : base(executor) {
			base.BacktestDataSource = new LivesimDataSource(executor);
			base.BacktestDataSource.Initialize(Assembler.InstanceInitialized.OrderProcessor);
			//base.SeparatePushingThreadEnabled = false;
			this.livesimQuoteBarConsumer = new LivesimQuoteBarConsumer(this);
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

				StreamingAdapter streaming = this.DataSourceAsLivesimNullUnsafe.StreamingAdapter;
				streaming.ConsumerQuoteSubscribe(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);
				streaming.ConsumerBarSubscribe	(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);
				streaming.SetQuotePumpThreadNameSinceNoMoreSubscribersWillFollowFor(this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval);

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
		        LivesimStreaming streamingBacktest = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe;
		        StreamingAdapter streamingOriginal = base.BarsOriginal.DataSource.StreamingAdapter;
		        string msg = "NOW_INSERT_BREAKPOINT_TO_this.channel.PushQuoteToConsumers(quoteDequeued) CATCHING_BACKTEST_END_UNPAUSE_PUMP";
		        //if (streamingOriginal.

		        streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(streamingBacktest, base.BarsOriginal.Symbol, base.BarsOriginal.ScaleInterval);

				StreamingAdapter streaming = this.DataSourceAsLivesimNullUnsafe.StreamingAdapter;
				streaming.ConsumerQuoteUnSubscribe	(base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);
				streaming.ConsumerBarUnSubscribe	(base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);

				//v2 moved from 10 lines below and added unsyncHappenedNotAsResultOfAbort to IndicatorMovingAverageSimple.checkPopupOnResetAndSync()
				if (base.BacktestWasAbortedByUserInGui) {
				    base.BacktestAborted.Set();
				    base.RequestingBacktestAbort.Reset();
				}

				base.Executor.BacktestContextRestore();
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
				//    base.BacktestAborted.Set();
				//    base.RequestingBacktestAbort.Reset();
				//}
				//v3 moved from Stop_inGuiThread koz there it's too early
				base.Executor.Backtester = this.BacktesterBackup;
			}
		}
	
		
		public void Start_inGuiThread(Button btnStartStop, ChartShadow chartShadow) {
			this.btnStartStop = btnStartStop;
			this.chartShadow = chartShadow;
			this.chartShadow.RangeBarCollapseToAccelerateLivesim();
			this.BacktesterBackup = base.Executor.Backtester;
			base.Executor.Backtester = this;
			base.Executor.EventGenerator.OnBacktesterContextInitializedStep2of4 +=
				new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
			base.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
		}
		void executor_BacktesterContextInitializedStep2of4(object sender, EventArgs e) {
			if (this.chartShadow.InvokeRequired) {
				this.chartShadow.BeginInvoke((MethodInvoker)delegate { this.executor_BacktesterContextInitializedStep2of4(sender, e); });
				return;
			}
			this.chartShadow.Initialize(base.Executor.Bars, true);
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
			base.BarsOriginal = null;

			this.btnStartStop.BeginInvoke((MethodInvoker)delegate {
				this.btnStartStop.Text = "Start";
				this.chartShadow.Initialize(base.Executor.Bars, true);
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
				// LIVESIM_HAS_SINGLE_THREADED_QUOTE_PUMP__DONT_REQUEST_AND_DONT_WAIT_JUST_CONTINUE
				base.AbortRunningBacktestWaitAborted(msig, 0);
			} catch (Exception ex) {
				Assembler.PopupException("Livesimulator.Stop_inGuiThread()", ex);
			}
		}

		public void Pause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused.Reset();
		}
		public void Unpause_inGuiThread() {
			this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused.Set();
		}
		public override string ToString() {
			string ret = "LIVESIMULATOR_FOR_" + base.Executor.ToString();
			return ret;
		}

	}
}
