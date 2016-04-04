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
		[JsonIgnore]	public		LivesimStreamingSettings	LivesimStreamingSettings	{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
		//[JsonIgnore]	protected	LivesimBroker				LivesimBroker				{ get { return this.livesimDataSource.BrokerAsLivesim_nullUnsafe; } }
		//[JsonIgnore]	protected	LivesimBrokerDataSnapshot	LivesimBrokerSnap			{ get { return this.livesimDataSource.BrokerAsLivesim_nullUnsafe.DataSnapshot; } }

		[JsonIgnore]	protected	LevelTwoGenerator			LevelTwoGenerator;
		[JsonIgnore]	protected	LivesimStreamingSpoiler		LivesimStreamingSpoiler;

		[JsonIgnore]	public		bool						IsDisposed					{ get; private set; }
		[JsonIgnore]	public		StreamingAdapter			StreamingOriginal			{ get; protected set; }

		//protected LivesimStreaming() : base("DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING") {
		//	string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING"
		//		//+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		//		+ "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		//		+ " example:QuikLivesimStreaming()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
		//	base.Name = "LivesimStreaming-child_ACTIVATOR_DLL-SCANNED";
		//}
		public LivesimStreaming(string reasonToExist) : base(reasonToExist) {
			base.Name						= "LivesimStreaming-NOT_ATTACHED_TO_DATASOURCE_INVOKE-InitializeDataSource_inverse()";
			base.StreamingSolidifier_oneForAllSymbols		= null;
			base.QuotePumpSeparatePushingThreadEnabled = true;
			this.UnpausedMre				= new ManualResetEvent(true);
			//USE_ATOMIC_FROM_BASE
			this.LevelTwoGenerator			= new LevelTwoGenerator();
			this.LivesimStreamingSpoiler	= new LivesimStreamingSpoiler(this);
		}
		public virtual void InitializeLivesim(LivesimDataSource livesimDataSource, StreamingAdapter streamingOriginalPassed, string symbol_iAmLivesimming) {
			if (livesimDataSource == null) {
				string msg = "DID_ACTIVATOR_PICK_THE_WRONG_CONSTRUCTOR?...";
				Assembler.PopupException(msg);
			}
			//this.livesimDataSource = livesimDataSource;
			base.DataSource = livesimDataSource;
			this.StreamingOriginal = streamingOriginalPassed;

			//v1 USE_ATOMIC_FROM_BASE
			//LevelTwoGeneratorLivesim levelTwoGeneratorLivesim = this.LevelTwoGenerator as LevelTwoGeneratorLivesim;
			//if (levelTwoGeneratorLivesim == null) {
			//    string msg = "WHERE_AM_I? NPE_GUARANTEED";
			//    Assembler.PopupException(msg);
			//}
			//levelTwoGeneratorLivesim.InitializeLevelTwo(symbol_iAmLivesimming);
			//v2
			base.StreamingDataSnapshot.Initialize_levelTwo_lastPrevQuotes_forSymbol(symbol_iAmLivesimming);
			LevelTwo sureItExists = base.StreamingDataSnapshot.GetLevelTwo_forSymbol_nullUnsafe(symbol_iAmLivesimming);
			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(symbol_iAmLivesimming);
			this.LevelTwoGenerator.Initialize(sureItExists, symbolInfo, symbolInfo.Level2PriceLevels, this.ToString());
		}
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			LivesimStreamingEditorEmpty emptyEditor = new LivesimStreamingEditorEmpty();
			emptyEditor.Initialize(this, dataSourceEditor);
			this.StreamingEditorInstance = emptyEditor;
			return emptyEditor;
		}

		// invoked after LivesimFormShow(), but must have meaning "Executor.Bars changed"...
		public void PushSymbolInfo_toLevelTwoGenerator(SymbolInfo symbolInfo_fromExecutor) {
			int howMany = this.LivesimStreamingSettings.LevelTwoLevelsToGenerate;
			LevelTwo levelTwo_livesimsOwn_notQuikStreamings = this.StreamingDataSnapshot.GetLevelTwo_forSymbol_nullUnsafe(symbolInfo_fromExecutor.Symbol);
			this.LevelTwoGenerator.Initialize(levelTwo_livesimsOwn_notQuikStreamings, symbolInfo_fromExecutor, howMany, this.ToString());
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


			this.LevelTwoGenerator.GenerateForQuote(quote, this.LivesimStreamingSettings.LevelTwoLevelsToGenerate);
			base.PushQuoteGenerated(quote);
	
			this.LivesimStreamingSpoiler.Spoil_after_PushQuoteGenerated();

			if (this.LivesimStreamingSettings.GenerateWideSpreadWithZeroSize) {
				// generate again another one for same quote, widen the spread and send it; each second level2 you must see new unfilled quote is generated by reconstructSpreadQuote_pushToStreaming_ifChanged();
				this.LevelTwoGenerator.GenerateForQuote(quote, this.LivesimStreamingSettings.LevelTwoLevelsToGenerate);
				base.PushQuoteGenerated(quote);
			}

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
		protected override void SolidifierSubscribe_toAllSymbols_ofDataSource_onAppRestart() {
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

		public void				Original_SubstituteDistributor_forSymbolLivesimming_extractChartIntoSeparateDistributor_subscribe() {
			bool chartBars_subscribeSelected_cbxChecked_whenLivesimStarts = true;
				 chartBars_subscribeSelected_cbxChecked_whenLivesimStarts = this.Livesimulator.Executor.Strategy.ScriptContextCurrent.DownstreamSubscribed;
			this.StreamingOriginal.	SubstituteDistributor_withOneSymbolLivesimming__extractChart_intoSeparateDistributor(this, chartBars_subscribeSelected_cbxChecked_whenLivesimStarts);
		}

		public void				Original_SubstituteDistributor_forSymbolLivesimming_restoreOriginalDistributor() {
			this.StreamingOriginal.	SubstituteDistributor_withOneSymbolLivesimming_restoreOriginalDistributor();
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
	}
}
