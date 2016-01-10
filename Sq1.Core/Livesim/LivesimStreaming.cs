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
	public partial class LivesimStreaming : BacktestStreaming, IDisposable {
		// without [JsonIgnore] Livesim children will have these properties in JSON
		[JsonIgnore]	public		ManualResetEvent			Unpaused					{ get; private set; }
		//[JsonIgnore]				ChartShadow					chartShadow_notUsed;
		[JsonIgnore]				LivesimDataSource			livesimDataSource			{ get { return base.DataSource as LivesimDataSource; } }
		[JsonIgnore]	public		Livesimulator				Livesimulator				{ get { return this.livesimDataSource.Executor.Livesimulator; } }
		[JsonIgnore]	internal	LivesimStreamingSettings	LivesimStreamingSettings	{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
		[JsonIgnore]	protected	LivesimBroker				LivesimBroker				{ get { return this.livesimDataSource.BrokerAsLivesimNullUnsafe; } }
		[JsonIgnore]	protected	LivesimBrokerDataSnapshot	LivesimBrokerSnap			{ get { return this.livesimDataSource.BrokerAsLivesimNullUnsafe.DataSnapshot; } }

		[JsonIgnore]	protected	LevelTwoGenerator			Level2generator;
		[JsonIgnore]	protected	LivesimStreamingSpoiler		LivesimStreamingSpoiler;

		[JsonIgnore]	public		bool						IsDisposed					{ get; private set; }
		[JsonIgnore]	public		StreamingAdapter			StreamingAdapterOriginal	{ get; private set; }

		protected LivesimStreaming() : base() {
		    string msg = "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		        + "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		        + " example:QuikLivesimStreaming()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
			base.Name = "LivesimStreaming-child_ACTIVATOR_DLL-SCANNED";
		}
		public LivesimStreaming(bool IamNotAdummy) : base() {
			base.Name = "LivesimStreaming";
			base.StreamingSolidifier = null;
			base.QuotePumpSeparatePushingThreadEnabled = false;
			this.Unpaused = new ManualResetEvent(true);
			this.Level2generator = new LevelTwoGeneratorLivesim(this);
			this.LivesimStreamingSpoiler = new LivesimStreamingSpoiler(this);
		}
		public virtual void InitializeLivesim(LivesimDataSource livesimDataSource, StreamingAdapter streamingAdapterOriginalPassed) {
			if (livesimDataSource == null) {
				string msg = "DID_ACTIVATOR_PICK_THE_WRONG_CONSTRUCTOR?...";
				Assembler.PopupException(msg);
			}
			//this.livesimDataSource = livesimDataSource;
			base.DataSource = livesimDataSource;
			this.StreamingAdapterOriginal = streamingAdapterOriginalPassed;
		}

		// invoked after LivesimFormShow(), but must have meaning "Executor.Bars changed"...
		public void PushSymbolInfoToLevel2generator(SymbolInfo symbolInfo_fromExecutor) {
			int howMany = this.LivesimStreamingSettings.LevelTwoLevelsToGenerate;
			this.Level2generator.Initialize(symbolInfo_fromExecutor, howMany);
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

			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
			this.LivesimStreamingSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}
			this.Level2generator.GenerateForQuote(quote);
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

		#region DISABLING_SOLIDIFIER
		public override void InitializeDataSource(DataSource dataSource, bool subscribeSolidifier = true) {
			base.InitializeFromDataSource(dataSource);
		}
		protected override void SolidifierSubscribe() {
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
	}
}
