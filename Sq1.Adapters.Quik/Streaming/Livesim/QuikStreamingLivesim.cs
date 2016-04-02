using System;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Backtesting;

namespace Sq1.Adapters.Quik.Streaming.Livesim {
	[SkipInstantiationAt(Startup = true)]		// overriding LivesimStreaming's TRUE to have QuikStreamingLivesim appear in DataSourceEditor
	public sealed partial class QuikStreamingLivesim : LivesimStreaming {
		// reasonToExist =	"1) use LivesimForm as control "
		//					+ "2) instantiate QuikStreaming and make it run its DDE server "
		//					+ "3) push quotes generated using DDE client";

		[JsonIgnore]	public	QuikLivesimBatchPublisher	QuikLivesimBatchPublisher;
		[JsonIgnore]	public	QuikStreaming				QuikStreamingOriginal								{ get { return base.StreamingOriginal as QuikStreaming; } }
		[JsonIgnore]			bool						upstreamWasSubscribed_preLivesim;
		[JsonIgnore]			DataSource					dataSourcePreLivesim;
		
		// or 1) override DdeClient.EndPoke()
		// or 2) create AsyncContext to deal with Invoke/BeginInvoke/EndInvoke just like GuiThread's message queue
		// (lazy to dig in; google for ppl complaining on NDde sync'ed to GuiThread - but what else should it be locked onto?... Dde is MessageQueue-based data transfer, if I sync on my own thread there is no reliable BeginPoke/EndPoke)
		[JsonIgnore]	public	bool						DdePokerShouldSyncWaitForDdeServerToReceiveMessage_falseToAvoidDeadlocks	{ get; private set; }

		public QuikStreamingLivesim(string reasonToExist) : base(reasonToExist) {
			base.Name = "QuikStreamingLivesim";
			//base.Icon = (Bitmap)Sq1.Adapters.Quik.Streaming.Livesim.Properties.Resources.imgQuikStreamingLivesim;
			// true is preferred; true/false both are not causing a deadlock after QuotePumpPerChannel turns HasQuote=false after pause/unpause (last commit)
			this.DdePokerShouldSyncWaitForDdeServerToReceiveMessage_falseToAvoidDeadlocks = true;

			//NO_DESERIALIZATION_WILL_THROW_YOULL_NULLIFY_ME_IN_UpstreamConnect YES_I_PROVOKE_NPE__NEED_TO_KNOW_WHERE_SNAPSHOT_IS_USED WILL_POINT_IT_TO_QUIK_REAL_STREAMING_IN_UpstreamConnect_LivesimStarting()
			//this.StreamingDataSnapshot = null;

			base.LevelTwoGenerator = new LevelTwoGenerator();		// this one has it's own LevelTwoAsks,LevelTwoBids NOT_REDIRECTED to StreamingDatasnapshot => sending Level2 via DDE to QuikStreaming.StreamingDatasnapshot
		}

		public override void InitializeLivesim(LivesimDataSource livesimDataSource, StreamingAdapter streamingOriginalPassed, string symbolLivesimming) {
			int usefulThingsDone = 0;
			if (livesimDataSource == null) {
				string msg = "DID_ACTIVATOR_PICK_THE_WRONG_CONSTRUCTOR?...";
				Assembler.PopupException(msg);
			}
			//this.livesimDataSource = livesimDataSource;
			if (base.DataSource != livesimDataSource) {
				base.DataSource  = livesimDataSource;
				usefulThingsDone++;
			}
			if (this.StreamingOriginal != streamingOriginalPassed) {
				this.StreamingOriginal  = streamingOriginalPassed;
				usefulThingsDone++;
			} else {
				string msg = "DO_YOU_STILL_SEE_SOME_PROBLEMS_WHILE_LIVESIMMING_SECOND_TIME?... HERE_YOU_RESOLVE_THEM";
				Assembler.PopupException(msg, null, false);
			}

			// disabled comparing to base.InitializeLivesim()
			//LevelTwoGeneratorLivesim levelTwoGeneratorLivesim = this.LevelTwoGenerator as LevelTwoGeneratorLivesim;
			//if (levelTwoGeneratorLivesim == null) {
			//	string msg = "WHERE_AM_I? NPE_GUARANTEED";
			//	Assembler.PopupException(msg);
			//}
			//levelTwoGeneratorLivesim.InitializeLevelTwo(symbolLivesimming);

			if (usefulThingsDone == 0) {
				Assembler.PopupException("DONT_INVOKE_ME_UPSTACK //QuikStreamingLivesim.InitializeLivesim()");
			}
		}

		protected override void SolidifierSubscribe_toAllSymbols_ofDataSource_onAppRestart() {
			string msg = "LIVESIM_MUST_NOT_SAVE_ANY_BARS EMPTY_HERE_TO_PREVENT_BASE_FROM_SUBSCRIBING_SOLIDIFIER";
		}

		public override void UpstreamConnect_LivesimStarting() {
			string msig = " //UpstreamConnect_LivesimStarting(" + this.ToString() + ")";

			this.upstreamWasSubscribed_preLivesim	= this.QuikStreamingOriginal.UpstreamConnected;
			this.dataSourcePreLivesim				= this.QuikStreamingOriginal.DataSource;
			
			//FIRST_LINES_OF_QuikStreamingLivesim.UpstreamConnect_LivesimStarting()_MAKE_SENSE_FOR_OTHERS
			//base.SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor();
			//LivesimDataSource is now having LivesimBacktester and no-solidifier Distributor

			//v1 this.QuikStreamingOriginal.InitializeDataSource_inverse(base.Livesimulator.DataSourceAsLivesim_nullUnsafe, false);
			//v2 this.QuikStreamingOriginal.InitializeFromDataSource(base.Livesimulator.DataSourceAsLivesim_nullUnsafe);
			//v3 this.InitializeFromDataSource(base.Livesimulator.DataSourceAsLivesim_nullUnsafe);
			this.UpstreamConnectionState = ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed;
			
			//NO,GOOD_FOR_BOTH GOOD_FOR_LivesimStreamingDefault_BUT_BAD_FOR_QuikStreamingLivesim
			//YOU_PARASITE this.StreamingDataSnapshot = this.QuikStreamingOriginal.StreamingDataSnapshot;
			if (this.StreamingDataSnapshot != null) {
			    if (this.StreamingDataSnapshot.ToString().Contains("OWN_IMPLEME")) {
			        bool good = true;
			    } else {
			        // first time you set it was in base.base.base.ctor(); now I'm letting Level2 be generated on MY OWN snap, because DDE-received Level2 will go to streamingOriginal.snap
			        this.StreamingDataSnapshot = new StreamingDataSnapshot(this);
			    }
			}
			


			if (this.upstreamWasSubscribed_preLivesim == false) {
				this.QuikStreamingOriginal.UpstreamConnect();
			}

			// MarketLive checks for QuoteLast, which I don't save anymore in QuikStreamingLivesim
			// QuikStreamingLivesim is a handicap without StreamingDataSnapshot; normally Snap is maintained by
			// 1) DistributorChart.StreamingDataSnapshot.QuoteLastCloneInitialize(symbol)
			//		but QuikStreamingLivesim.Distributor is donated to the Puppet
			// 2) StreamingAdapter(base).PushQuoteGenerated(): StreamingDataSnapshot.QuoteLastCloneSetForSymbol(quote);
			//		but QuikStreamingLivesim doesnt invoke base.PushQuoteGenerated(quote) because it shoots the quote to DDE and doesnt deal with Distributor
			//if (this.StreamingDataSnapshot != null) {
			//	string msg1 = "MUST_BE_NULL__ONLY_INITIALIZED_FOR_MarketLive_FOR_A_LIVESIM_SESSION__OTHERWIZE_MUST_BE_NULL";
			//	Assembler.PopupException(msg1);
			//}
			//NOT_USED_IN_LIVESIM_SINCE_NO_PUSH_QUOTE_INVOKED this.StreamingDataSnapshot = this.QuikStreamingOriginal.StreamingDataSnapshot;

			if (this.QuikLivesimBatchPublisher == null) {
				string msg1 = "AVOIDING_CAN_NOT_ACCESS_DISPOSED_OBJECT__INSIDE_DDE => CREATING_AGAIN_ANYWAY";
				//this.QuikLivesimBatchPublisher = new QuikLivesimBatchPublisher(this);
			}
			this.QuikLivesimBatchPublisher = new QuikLivesimBatchPublisher(this);
			this.QuikLivesimBatchPublisher.ConnectAll();
			string msg = "DDE_CLIENT_PUBLISHING_ALL_TOPICS_NEEDED_FOR_LIVESIM: [" + this.QuikLivesimBatchPublisher.TopicsAsString + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		public override void UpstreamDisconnect_LivesimTerminatedOrAborted() {
			string msig = " //UpstreamDisconnect_LivesimEnded(" + this.ToString() + ")";
			string msg = "Disposing QuikStreaming with prefixed DDE tables [...]";
			Assembler.PopupException(msg + msig, null, false);
			this.QuikLivesimBatchPublisher.DisconnectAll();
			this.QuikLivesimBatchPublisher.DisposeAll();

			if (this.upstreamWasSubscribed_preLivesim == false) {
				this.QuikStreamingOriginal.UpstreamDisconnect();	// not disposed, QuikStreaming.ddeServerStart() is reusable
			}
			//this.QuikStreamingOriginal.SolidifierSubscribeOneSymbol_iFinishedLivesimming();
			this.QuikStreamingOriginal.InitializeFromDataSource(this.dataSourcePreLivesim);

			//LAST_LINE_OF_QuikStreamingLivesim.UpstreamConnect_LivesimStarting()_MAKES_SENSE_FOR_OTHERS
			//base.SubstituteDistributorForSymbolsLivesimming_restoreOriginalDistributor();

			// YES_I_PROVOKE_NPE__NEED_TO_KNOW_WHERE_SNAPSHOT_IS_USED WILL_POINT_IT_TO_QUIK_REAL_STREAMING_IN_UpstreamConnect_LivesimStarting()
			// NO_LEAVE_IT__SECOND_LIVESIM_RUN_THROWS_NPE_IN_base.InitializeFromDataSource() this.StreamingDataSnapshot = null;
		}

		public override void PushQuoteGenerated(QuoteGenerated quote) {
			string threadName = "LIVESIM_QUOTE_GENERATING_FOR " + this.StreamingOriginal.DistributorCharts_substitutedDuringLivesim.ReasonIwasCreated;
			Assembler.SetThreadName(threadName, "SETTING_LIVESIM_THREAD_NAME_FROM_STREAMING_IS_TOO_LATE__AND_DONT_DO_IT_TWICE");

			//second Livesim gets NPE - fixed but the caveat is when you clicked on "stopping" disabled button, new livesim restarts with lots of NPE...)
			if (base.Livesimulator.RequestingBacktestAbortMre.WaitOne(0) == true) {
				string msg = "MUST_NEVER_HAPPEN PUSHING_QUOTE_DENERATED_AFTER_LIVESIM_REQUESTED_TO_STOP";
				Assembler.PopupException(msg);
				return;
			}
			if (base.Livesimulator.BacktestAbortedMre.WaitOne(0) == true) {
				string msg = "MUST_NEVER_HAPPEN PUSHING_QUOTE_DENERATED_AFTER_LIVESIM_CONFIRMED_TO_STOP";
				Assembler.PopupException(msg);
				return;
			}


			#region otherwize LivesimulatorForm.PAUSE button doesn't pause livesim (copypaste from LivesimStreaming)
			bool isUnpaused = this.UnpausedMre.WaitOne(0);
			if (isUnpaused == false) {
				string msg = "QuikLIVESTREAMING_CAUGHT_PAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg, null, false);
				this.UnpausedMre.WaitOne();	// 1CORE=100% while Livesim Paused

				string msg2 = "QuikLIVESTREAMING_CAUGHT_UNPAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg2, null, false);
			}

			base.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
			base.LivesimStreamingSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}
			#endregion

			//LivesimStreaming.cs does {base.PushQuoteGenerated(quote);} here

			if (this.QuikLivesimBatchPublisher == null) {
				string msg = "AVOIDING_NPE QuikLivesimBatchPublisher_WANST_CREATED_NORMALLY_IN_UpstreamConnect_LivesimStarting()";
				Assembler.PopupException(msg);
				//DONT_EVEN_WANNA_TRY_DEALOCK base.Livesimulator.AbortRunningBacktestWaitAborted();
				base.Livesimulator.RequestingBacktestAbortMre.Set();
			}

			
			// FUNDAMENTAL: QuikStreamingLivesim doesn't use base.DataDitstributor AT ALL; I push to the DDE and I expect the QuikStreaming to:
			// 1. extract only chart subscribed to bars and quotes for the Symbol-livesimming
			// 2. assign this new Distributor to QuikStreamingOriginal; restore old Distributor+Streaming at the Livesim end/abort
			// 3. other charts open for the livesimming Symbol (same or different timeframes) won't receive anything
			// 4. solidifiers for original datasource timeframe won't receive anything

			string msg1 = "I_PREFER_TO_PUSH_LEVEL2_NOW__BEFORE_base.PushQuoteGenerated(quote)";
			//v3 REDIRECTING_PushQuoteGenerated_RADICAL_PARENT_DETACHED base.Level2generator.GenerateAndStoreInStreamingSnap(quote);

			//LivesimStreamingSettings settings = base.Livesimulator.Executor.Strategy.LivesimStreamingSettings;
			//if (base.LivesimStreamingSettings == settings) {
			//    string msg = "EVEN_WHEN_NOT_PARASITING_ON_STREAMING_ORIGINAL__YOU_HAVE_SETTINGS_VIA_LIVESIM_DATASOURCE";
			//    //Assembler.PopupException(msg);
			//}

			base.LevelTwoGenerator.GenerateForQuote(quote, base.LivesimStreamingSettings.LevelTwoLevelsToGenerate);

			// two dde tables from quik will always be received asynchronously; I'm seding them here asynchronously, too; synchronization must be done on the server side (beware of one table suddenly being disconnected; don't ever rely they ARE synchronized in your strategy)
			this.QuikLivesimBatchPublisher.SendLevelTwo_DdeClientPokesDdeServer_waitServerProcessed(base.LevelTwoGenerator.LevelTwo_fromStreaming);
			this.QuikLivesimBatchPublisher.SendQuote_DdeClientPokesDdeServer_waitServerProcessed(quote);
			base.LivesimStreamingSpoiler.Spoil_after_PushQuoteGenerated();

			if (base.LivesimStreamingSettings.GenerateWideSpreadWithZeroSize) {
				// generate again another one for same quote, widen the spread and send it; each second level2 you must see new unfilled quote is generated by reconstructSpreadQuote_pushToStreaming_ifChanged();
				base.LevelTwoGenerator.GenerateForQuote(quote, base.LivesimStreamingSettings.LevelTwoLevelsToGenerate, 1);
				this.QuikLivesimBatchPublisher.SendLevelTwo_DdeClientPokesDdeServer_waitServerProcessed(base.LevelTwoGenerator.LevelTwo_fromStreaming);
				base.LivesimStreamingSpoiler.Spoil_after_PushQuoteGenerated();
			}

			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			string msg = "YOU_FORGOT_TO_SET_[SkipInstantiationAt(Startup=true)]_FOR " + this.GetType()
				+ " LIVESIM_STREAMING_ADAPTERS_SHOULD_NOT_HAVE_ANY_EDITORS__SETTINGS_ARE_EDITED_IN_LIVESIM_FORM";
			throw new Exception(msg);
		}
	}
}
