using System;
using System.Threading;

using Sq1.Core.Backtesting;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public class LivesimStreaming : BacktestStreaming {
		public	ManualResetEvent			Unpaused			{ get; private set; }
				ChartShadow					chartShadow;
				LivesimDataSource			livesimDataSource;
				LivesimStreamingSettings	settings			{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		public LivesimStreaming(LivesimDataSource livesimDataSource) : base() {
			base.Name = "LivesimStreaming";
			base.StreamingSolidifier = null;
			base.QuotePumpSeparatePushingThreadEnabled = false;
			this.Unpaused = new ManualResetEvent(true);
			this.livesimDataSource = livesimDataSource;
		}

		public void Initialize(ChartShadow chartShadow) {
			this.chartShadow = chartShadow;
		}

		public override void GeneratedQuoteEnrichSymmetricallyAndPush(QuoteGenerated quote, Bar bar2simulate, double priceForSymmetricFillAtOpenOrClose = -1) {
			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}

			bool isUnpaused = this.Unpaused.WaitOne(0);
			if (isUnpaused == false) {
				string msg = "LIVESTREAMING_CAUGHT_PAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				Assembler.PopupException(msg, null, false);
				this.Unpaused.WaitOne();
				string msg2 = "LIVESTREAMING_CAUGHT_UNPAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				Assembler.PopupException(msg2, null, false);
			}

			ScriptExecutor executor = this.livesimDataSource.Executor;
			int delay = 0;
			if (this.settings.DelayBetweenSerialQuotesEnabled) {
				delay = settings.DelayBetweenSerialQuotesMin;
				if (settings.DelayBetweenSerialQuotesMax > 0) {
					int range = Math.Abs(settings.DelayBetweenSerialQuotesMax - settings.DelayBetweenSerialQuotesMin);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					delay += rangePart;
				}
			}
			if (delay > 0) {
				executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
			}
			base.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
	
			if (this.chartShadow == null) {
				string msg = "YOU_FORGOT_TO_LET_LivesimStreaming_KNOW_ABOUT_CHART_CONTROL__TO_WAIT_FOR_REPAINT_COMPLETED_BEFORE_FEEDING_NEXT_QUOTE_TO_EXECUTOR_VIA_PUMP";
				Assembler.PopupException(msg);
				return;
			}

			// NO_NEED_IN_THIS_AT_ALL => STREAMING_WILL_INVALIDATE_ALL_PANELS,ORDERPROCESSOR_WILL_REBUILD_EXECUTION,EXECUTOR_REBUILDS_REPORTERS
			//12.9secOff vs 14.7secOn this.chartShadow.RefreshAllPanelsWaitFinishedSoLivesimCouldGenerateNewQuote();
			//this.chartShadow.Invalidate();

			//SLEEP_IS_VITAL__OTHERWISE_FAST_LIVESIM_AND_100%CPU_AFTERWARDS
			//Thread.Sleep(50);	// 50ms_ENOUGH_FOR_3.3GHZ_TO_KEEP_GUI_RESPONSIVE LET_WinProc_TO_HANDLE_ALL_THE_MESSAGES I_HATE_Application.DoEvents()_IT_KEEPS_THE_FORM_FROZEN

			//WARNING WARNING WARNING!!!!!!!!!!!!! Application.DoEvents();
			//NOT_ENOUGH_TO_UNFREEZE_PAUSE_BUTTON PAINTS_OKAY_AFTER_INVOKING_RangeBarCollapseToAccelerateLivesim()
			// Thread.Sleep(1)_REDUCES_CPU_USAGE_DURING_LIVESIM_FROM_60%_TO_3%_DUAL_CORE__Application.DoEvents()_IS_USELESS

			ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
			if (snap.AlertsPending.Count > 0) {
				if (quote.ParentBarStreaming != null) {
					string msg = "I_MUST_HAVE_IT_UNATTACHED_HERE";
					//Assembler.PopupException(msg);
				}
				this.livesimDataSource.BrokerAsLivesimNullUnsafe.ConsumeQuoteOfStreamingBarToFillPending(quote, bar2simulate);
			}

			if (delay > 0) {
				System.Windows.Forms.Application.DoEvents();
				Thread.Sleep(delay);
			}
			executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		}
		#region DISABLING_SOLIDIFIER
		public override void Initialize(DataSource dataSource) {
			base.InitializeFromDataSource(dataSource);
		}
		protected override void SubscribeSolidifier() {
			return;
		}
		#endregion
	}
}
