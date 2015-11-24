using System;
using System.Threading;

using Sq1.Core.Backtesting;
using Sq1.Core.Charting;
using Sq1.Core.Support;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public partial class LivesimStreaming : BacktestStreaming, IDisposable {
		public		ManualResetEvent			Unpaused			{ get; private set; }
					ChartShadow					chartShadow;
					LivesimDataSource			livesimDataSource;
					ScriptExecutor				executor			{ get { return this.livesimDataSource.Executor; } }
		public		Livesimulator				Livesimulator		{ get { return this.livesimDataSource.Executor.Livesimulator; } }
		internal	LivesimStreamingSettings	LivesimSettings		{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
		protected	LivesimBroker				LivesimBroker		{ get { return this.livesimDataSource.BrokerAsLivesimNullUnsafe; } }
		protected	LivesimBrokerDataSnapshot	LivesimBrokerSnap	{ get { return this.livesimDataSource.BrokerAsLivesimNullUnsafe.DataSnapshot; } }

		protected	LivesimLevelTwoGenerator	Level2generator;
		public		Livesimulator				LivesimulatorRedundant;		// used by QuikLivesimStreaming to instantiate QuikStreaming with a fake DataSource (LivesimDataSource having LivesimBacktester and no-solidifier DataDistributor)
		protected	LivesimSpoiler				LivesimSpoiler;

		public LivesimStreaming(LivesimDataSource livesimDataSource) : base() {
			if (livesimDataSource == null) {
				string msg = "DID_ACTIVATOR_PICK_THE_WRONG_CONSTRUCTOR?...";
				Assembler.PopupException(msg);
			}
			base.Name = "LivesimStreaming";
			base.StreamingSolidifier = null;
			base.QuotePumpSeparatePushingThreadEnabled = false;
			this.Unpaused = new ManualResetEvent(true);
			this.livesimDataSource = livesimDataSource;
			this.Level2generator = new LivesimLevelTwoGenerator(this);
			this.LivesimSpoiler = new LivesimSpoiler(this);
		}

		protected LivesimStreaming() : base() {
		    string msg = "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		        + "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		        + " example:QuikLivesimStreaming()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
			base.Name = "LivesimStreaming-child_ACTIVATOR_DLL-SCANNED";
		}

		public void Initialize(ChartShadow chartShadow) {
			this.chartShadow = chartShadow;
			double stepPrice = this.chartShadow.Bars.SymbolInfo.PriceStepFromDecimal;
			double stepSize  = this.chartShadow.Bars.SymbolInfo.VolumeStepFromDecimal;
			SymbolInfo symbolInfo = this.chartShadow.Executor.Bars.SymbolInfo;	//LivesimLevelTwoGenerator needs to align price and volume to Levels
			int howMany = chartShadow.Executor.Strategy.LivesimStreamingSettings.LevelTwoLevelsToGenerate;
			this.Level2generator.Initialize(symbolInfo, howMany, stepPrice, stepSize);
		}

		public override void PushQuoteGenerated(QuoteGenerated quote) {
			bool isUnpaused = this.Unpaused.WaitOne(0);
			if (isUnpaused == false) {
				string msg = "LIVESTREAMING_CAUGHT_PAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg, null, false);
				this.Unpaused.WaitOne();
				string msg2 = "LIVESTREAMING_CAUGHT_UNPAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg2, null, false);
			}

			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			this.LivesimSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}
			this.Level2generator.GenerateAndStoreInStreamingSnap(quote);
			base.PushQuoteGenerated(quote);
	
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

			//v1 WORKED_FOR_NON_LIVE_BACKTEST
			//ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
			//if (snap.AlertsPending.Count > 0) {
			//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
			AlertList notYetScheduled = this.LivesimBrokerSnap.AlertsNotYetScheduledForDelayedFillBy(quote);
			if (notYetScheduled.Count > 0) {
				if (quote.ParentBarStreaming != null) {
					string msg = "I_MUST_HAVE_IT_UNATTACHED_HERE";
					//Assembler.PopupException(msg);
				}
				this.LivesimBroker.ConsumeQuoteOfStreamingBarToFillPending(quote, notYetScheduled);
			} else {
				string msg = "NO_NEED_TO_PING_BROKER_EACH_NEW_QUOTE__EVERY_PENDING_ALREADY_SCHEDULED";
			}

			this.LivesimSpoiler.Spoil_after_PushQuoteGenerated();
			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		}

		#region DISABLING_SOLIDIFIER
		public override void Initialize(DataSource dataSource) {
			base.InitializeFromDataSource(dataSource);
		}
		protected override void SubscribeSolidifier() {
			return;
		}
		#endregion

		public virtual void UpstreamConnect_LivesimStarting() {
			Assembler.DisplayStatus("UpstreamConnect_LivesimStarting(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		public virtual void UpstreamDisconnect_LivesimEnded() {
			Assembler.DisplayStatus("UpstreamDisconnect_LivesimEnded(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.Unpaused.Dispose();
			this.Unpaused = null;
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }

		public virtual void UpstreamConnect_LivesimStarting(Livesimulator livesimulator) {
			this.LivesimulatorRedundant = livesimulator;
		}
	}
}
