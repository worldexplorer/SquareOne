using System;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Backtesting;
using Sq1.Core.Charting;
using Sq1.Core.Support;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public abstract partial class LivesimStreaming : BacktestStreaming, IDisposable {
		// without [JsonIgnore] Livesim children will have these properties in JSON
		[JsonIgnore]	public		ManualResetEvent			UnpausedMre					{ get; private set; }
		//[JsonIgnore]				ChartShadow					chartShadow_notUsed;
		[JsonIgnore]				LivesimDataSource			livesimDataSource			{ get { return base.DataSource as LivesimDataSource; } }
		[JsonIgnore]	public		Livesimulator				Livesimulator				{ get { return this.livesimDataSource.Executor.Livesimulator; } }
		[JsonIgnore]	internal	LivesimStreamingSettings	LivesimStreamingSettings	{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
		[JsonIgnore]	protected	LivesimBroker				LivesimBroker				{ get { return this.livesimDataSource.BrokerAsLivesim_nullUnsafe; } }
		[JsonIgnore]	protected	LivesimBrokerDataSnapshot	LivesimBrokerSnap			{ get { return this.livesimDataSource.BrokerAsLivesim_nullUnsafe.DataSnapshot; } }

		[JsonIgnore]	protected	LevelTwoGenerator			LevelTwoGenerator;
		[JsonIgnore]	protected	LivesimStreamingSpoiler		LivesimStreamingSpoiler;

		[JsonIgnore]	public		bool						IsDisposed					{ get; private set; }
		[JsonIgnore]	public		StreamingAdapter			StreamingOriginal			{ get; private set; }

		//protected LivesimStreaming() : base("DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING") {
		//    string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING"
		//        //+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		//        + "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		//        + " example:QuikLivesimStreaming()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
		//    base.Name = "LivesimStreaming-child_ACTIVATOR_DLL-SCANNED";
		//}
		public LivesimStreaming(string reasonToExist) : base(reasonToExist) {
			base.Name						= "LivesimStreaming-NOT_ATTACHED_TO_DATASOURCE_INVOKE-InitializeDataSource_inverse()";
			base.StreamingSolidifier		= null;
			base.QuotePumpSeparatePushingThreadEnabled = true;
			this.UnpausedMre				= new ManualResetEvent(true);
			this.LevelTwoGenerator			= new LevelTwoGeneratorLivesim(this);
			this.LivesimStreamingSpoiler	= new LivesimStreamingSpoiler(this);
		}
		public virtual void InitializeLivesim(LivesimDataSource livesimDataSource, StreamingAdapter streamingOriginalPassed, string symbolLivesimming) {
			if (livesimDataSource == null) {
				string msg = "DID_ACTIVATOR_PICK_THE_WRONG_CONSTRUCTOR?...";
				Assembler.PopupException(msg);
			}
			//this.livesimDataSource = livesimDataSource;
			base.DataSource = livesimDataSource;
			this.StreamingOriginal = streamingOriginalPassed;

			LevelTwoGeneratorLivesim levelTwoGeneratorLivesim = this.LevelTwoGenerator as LevelTwoGeneratorLivesim;
			if (levelTwoGeneratorLivesim != null) {
				levelTwoGeneratorLivesim.InitializeLevelTwo(symbolLivesimming);
			} else {
				string msg = "WHERE_AM_I?";
			}
		}
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			LivesimStreamingEditorEmpty emptyEditor = new LivesimStreamingEditorEmpty();
			emptyEditor.Initialize(this, dataSourceEditor);
			this.StreamingEditorInstance = emptyEditor;
			return emptyEditor;
		}

		// invoked after LivesimFormShow(), but must have meaning "Executor.Bars changed"...
		public void PushSymbolInfoToLevel2generator(SymbolInfo symbolInfo_fromExecutor) {
			int howMany = this.LivesimStreamingSettings.LevelTwoLevelsToGenerate;
			this.LevelTwoGenerator.Initialize(symbolInfo_fromExecutor, howMany);
		}

		public override void PushQuoteGenerated(QuoteGenerated quote) {
			bool isUnpaused = this.UnpausedMre.WaitOne(0);
			if (isUnpaused == false) {
				string msg = "LIVESTREAMING_CAUGHT_PAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg, null, false);
				this.UnpausedMre.WaitOne();
				string msg2 = "LIVESTREAMING_CAUGHT_UNPAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg2, null, false);
			}

			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
			this.LivesimStreamingSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}
			this.LevelTwoGenerator.GenerateForQuote(quote);
			base.PushQuoteGenerated(quote);
	
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

			this.LivesimStreamingSpoiler.Spoil_after_PushQuoteGenerated();
			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		}

		#region DISABLING_SOLIDIFIER__NOT_REALLY_USED_WHEN_STREAMING_ADAPTER_PROVIDES_ITS_OWN_LIVESIM_STREAMING
		public override void InitializeDataSource_inverse(DataSource dataSource, bool subscribeSolidifier = true) {
			base.InitializeFromDataSource(dataSource);
			base.Name						= "LivesimStreaming_IAM_ABSTRACT_ALWAYS_OVERRIDE_IN_CHILDREN";
			if (subscribeSolidifier) {
				string msg = "RELAX_IM_NOT_FORWARING_IT_TO_BASE_BUT_I_HANDLE_InitializeDataSource()_IN_LivesimStreaming";
			}
		}
		protected override void SolidifierAllSymbolsSubscribe_onAppRestart() {
			string msg = "LIVESIM_MUST_NOT_SAVE_ANY_BARS EMPTY_HERE_TO_PREVENT_BASE_FROM_SUBSCRIBING_SOLIDIFIER";
		}
		#endregion

		public virtual void UpstreamConnect_LivesimStarting() {
			Assembler.DisplayStatus("UpstreamConnect_LivesimStarting(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		public virtual void UpstreamDisconnect_LivesimTerminatedOrAborted() {
			Assembler.DisplayStatus("UpstreamDisconnect_LivesimEnded(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.UnpausedMre.Dispose();
			this.UnpausedMre = null;
			this.IsDisposed = true;
		}

		protected void				SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor() {
			bool chartBarsSubscribeSelected = true;		//, this.Executor.Strategy.ScriptContextCurrent.DownstreamSubscribed
			this.StreamingOriginal.	SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor(this, chartBarsSubscribeSelected);
		}

		protected void				SubstituteDistributorForSymbolsLivesimming_restoreOriginalDistributor() {
			this.StreamingOriginal.	SubstituteDistributorForSymbolsLivesimming_restoreOriginalDistributor();
		}

		//mandantory DataSource.Streaming=LivesimStreamingDefault allowed Streaming implementors to play with BarsOriginal
		//1. BacktesterStreaming will
		//2. LivesimStreamingDefault will not pause anything and will follow the QuotesDelay user specified in LivesimControl
		//3. LivesimQuik will 
		public override bool BacktestContextInitialize_pauseQueueForBacktest_leavePumpUnpausedForLivesimDefault_overrideable(ScriptExecutor executor, Bars barsEmptyButWillGrow) {
			return false;
		}

		public override bool BacktestContextRestore_unpauseQueueForBacktest_leavePumpUnPausedForLivesimDefault_overrideable(ScriptExecutor executor) {
			return false;
		}

		//public void SubscribeLivesimQuoteBarConsumer_toDataDistributor_replacedForLivesim(Livesimulator livesimulator) {
		//    if (base.DataDistributorsAreReplacedByLivesim_ifYesDontPauseNeighborsOnBacktestContextInitRestore == false) return;

		//    bool runningOnLivesimStreamingDefault = this.StreamingOriginal is LivesimStreamingDefault;
		//         runningOnLivesimStreamingDefault = false;
		//    if (runningOnLivesimStreamingDefault) return;
	
		//    // will allow to reach DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol()
		//    // in PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread()

		//    // DONT_DELETE NOT_A_GARBAGE
		//    livesimulator.BarsSimulating.DataSource = livesimulator.DataSourceAsLivesim_nullUnsafe;	// may not need to restore (base.BarsSimulating is not needed after Livesim is done)
		//    // DONT_DELETE NOT_A_GARBAGE

			
		//    string					symbol			= livesimulator.BarsSimulating.Symbol;
		//    BarScaleInterval		scaleInterval	= livesimulator.BarsSimulating.ScaleInterval;
		//    StreamingConsumer		chartless		= livesimulator.LivesimQuoteBarConsumer;
		//    ScriptExecutor			executor		= livesimulator.Executor;

		//    //DataDistributor distr = livesimulator.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.DataDistributor_replacedForLivesim;
		//    DataDistributor distr = base.DataDistributor_replacedForLivesim;
		//    bool livesimIsSubscribed_toBarsSimulated = distr.DistributionChannels.Count > 0;
		//    if (livesimIsSubscribed_toBarsSimulated == false) {
		//        bool willPumpInNewThread = true;	// false led to deadlock on BeginInvoke both in DDE Server and in my GuiThread
		//        distr.ConsumerQuoteSubscribe(symbol, scaleInterval, chartless, willPumpInNewThread);
		//        distr.ConsumerBarSubscribe	(symbol, scaleInterval, chartless, willPumpInNewThread);
		//        distr.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbol, scaleInterval);
		//    } else {
		//        string msg1 = " USER_SUBSCRIBED_CHART_TO_QUOTES&BARS"
		//            + " 1) DID_YOU_INVOKE_ALREADY_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()"
		//            + " 2) DUPLICATE_CALL_TO_SimulationPreBarsSubstitute_overrideable()";
		//        Assembler.PopupException(msg1, null, false);
		//    }
		//}

		//public void UnSubscribeLivesimQuoteBarConsumer_fromDataDistributor_replacedForLivesim(Livesimulator livesimulator) {
		//    if (base.DataDistributorsAreReplacedByLivesim_ifYesDontPauseNeighborsOnBacktestContextInitRestore == false) return;

		//    bool runningOnLivesimStreamingDefault = this.StreamingOriginal is LivesimStreamingDefault;
		//         runningOnLivesimStreamingDefault = false;
		//    if (runningOnLivesimStreamingDefault) return;


		//    //DataDistributor distr = this.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.DataDistributor_replacedForLivesim;
		//    DataDistributor distr = base.DataDistributor_replacedForLivesim;
		//    bool livesimIsSubscribed_toBarsSimulated = distr.DistributionChannels.Count == 1;
		//    if (livesimIsSubscribed_toBarsSimulated) {
		//        string					symbol			= livesimulator.BarsSimulating.Symbol;
		//        BarScaleInterval		scaleInterval	= livesimulator.BarsSimulating.ScaleInterval;
		//        StreamingConsumer		chartless		= livesimulator.LivesimQuoteBarConsumer;

		//        distr.ConsumerQuoteUnsubscribe	(symbol, scaleInterval, chartless);
		//        distr.ConsumerBarUnsubscribe	(symbol, scaleInterval, chartless);
		//    } else {
		//        string msg1 = "WHO_UNSUBSCRIBED_LIVESIM.STREAMING_TO_BARS_SIMULATING??? DUPLICATE_CALL_TO_SimulationPostBarsRestore_overrideable()";
		//        Assembler.PopupException(msg1);
		//    }
	
		//}
	}
}
