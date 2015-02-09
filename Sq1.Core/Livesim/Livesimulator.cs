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

		protected override void SimulationPreBarsSubstitute() {
		    if (base.BarsOriginal == base.Executor.Bars) {
		        string msg = "DID_YOU_FORGET_TO_RESET_base.BarsOriginal_TO_NULL_AFTER_BACKTEST_FINISHED??";
		        Assembler.PopupException(msg);
		    }
		    try {
		        base.BarsOriginal	= base.Executor.Bars;
		        base.BarsSimulating = base.Executor.Bars.CloneNoBars(BARS_BACKTEST_CLONE_PREFIX + base.BarsOriginal);
		        base.Executor.EventGenerator.RaiseBacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4();
				
		        #region candidate for this.DataSourceAsLivesimNullUnsafeBuildFromUserSelection()
		        BacktestSpreadModeler spreadModeler;
		        // kept it on the surface and didn't pass ScriptContextCurrent.SpreadModelerPercent to "new DataSourceAsLivesimNullUnsafe()" because later DataSourceAsLivesimNullUnsafe:
		        // 1) will support different SpreadModelers with not only 1 parameter like SpreadModelerPercentage;
		        // 2) will support different BacktestModes like 12strokes, not only 4Stroke 
		        // 3) will poke StreamingProvider-derived implementations 12 times a bar with platform-generated quotes for backtests with regulated poke delay
		        // 4) will need to be provide visualized 
		        // v1 this.DataSourceAsLivesimNullUnsafe.BacktestStreamingProvider.InitializeSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
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

				this.DataSourceAsLivesimNullUnsafe.StreamingProvider.ConsumerQuoteSubscribe(
					base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval,
					this.livesimQuoteBarConsumer);
				this.DataSourceAsLivesimNullUnsafe.StreamingProvider.ConsumerBarSubscribe(
					base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval,
					this.livesimQuoteBarConsumer);
				
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

		        base.Executor.EventGenerator.RaiseBacktesterSimulationContextInitializedStep2of4();
		    } catch (Exception ex) {
		        string msg = "PreBarsSubstitute(): Backtester caught a long beard...";
		        base.Executor.PopupException(msg, ex);
		    } finally {
		        base.BarsSimulatedSoFar = 0;
		        SetBacktestAborted = false;
		        base.BacktestAborted.Reset();
		        base.RequestingBacktestAbort.Reset();
		        base.BacktestIsRunning.Set();
		        //if (this.IsBacktestingNow == false) {
		        //	string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
		        //	Debugger.Break();
		        //}

		        //COPIED_UPSTACK_FOR_BLOCKING_MOUSEMOVE_AFTER_BACKTEST_NOW_CLICK__BUT_ALSO_STAYS_HERE_FOR_SLIDER_CHANGE_NON_INVALIDATION
		        //WONT_BE_RESET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
		        if (base.Executor.ChartShadow != null) base.Executor.ChartShadow.BacktestIsRunning.Set();
		        // Calling ManualResetEvent.Reset closes the gate.
		        // Threads that call WaitOne on a closed gate will block
		        // REPLACED_BY_QUOTEPUMP_PAUSED base.BacktestCompletedQuotesCanGo.Reset();
		    }
		}
		protected override void SimulationPostBarsRestore() {
		    try {
		        LivesimStreaming streamingBacktest = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe;
		        StreamingProvider streamingOriginal = base.BarsOriginal.DataSource.StreamingProvider;
		        string msg = "NOW_INSERT_BREAKPOINT_TO_this.channel.PushQuoteToConsumers(quoteDequeued) CATCHING_BACKTEST_END_UNPAUSE_PUMP";
		        //if (streamingOriginal.

		        streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(streamingBacktest, base.BarsOriginal.Symbol, base.BarsOriginal.ScaleInterval);

				this.DataSourceAsLivesimNullUnsafe.StreamingProvider.ConsumerQuoteUnSubscribe(
					base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);
				this.DataSourceAsLivesimNullUnsafe.StreamingProvider.ConsumerBarUnSubscribe(
					base.BarsSimulating.Symbol, base.BarsSimulating.ScaleInterval, this.livesimQuoteBarConsumer);

		        base.Executor.BacktestContextRestore();
		        base.Executor.EventGenerator.RaiseBacktesterSimulatedAllBarsStep4of4();
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
		        if (SetBacktestAborted) {
		            base.BacktestAborted.Set();
		            base.RequestingBacktestAbort.Reset();
		        }
		    }
		}
	
		
		public void Start_inGuiThread(Button btnStartStop, ChartShadow chartShadow) {
			this.btnStartStop = btnStartStop;
			this.chartShadow = chartShadow;
			this.chartShadow.RangeBarCollapseToAccelerateLivesim();
			this.BacktesterBackup = base.Executor.Backtester;
			base.Executor.Backtester = this;
			base.Executor.EventGenerator.BacktesterContextInitializedStep2of4
				+= new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
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
			try {
				ManualResetEvent unpausedMR = this.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Unpaused;
				bool isUnpaused = unpausedMR.WaitOne(0);
				if (isUnpaused == false) {
					string msg = "AVOIDING_TO_WAIT_FOREVER: LivesimStreaming.GeneratedQuoteEnrichSymmetricallyAndPush()";
					unpausedMR.Set();
				}
				base.AbortRunningBacktestWaitAborted("USER_CLICKED_STOP_IN_LIVESIM_FORM TIME_TO_UNPAUSE_QUOTE_PUMP");
			} catch (Exception ex) {
				Assembler.PopupException("Livesimulator.Stop_inGuiThread()", ex);
			}
			base.Executor.Backtester = this.BacktesterBackup;
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
