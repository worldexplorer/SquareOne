using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

using NDde.Client;
using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Backtesting;
using Sq1.Core.Execution;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikLivesim.DataFeed;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = false)]		// overriding LivesimStreaming's TRUE to have QuikLivesimStreaming appear in DataSourceEditor
	public partial class QuikLivesimStreaming : LivesimStreaming {
		[JsonIgnore]	string reasonToExist =    "1) use LivesimForm as control "
												+ "2) instantiate QuikStreaming and make it run its DDE server "
												+ "3) push quotes generated using DDE client";

		[JsonIgnore]	string ddeTopicsPrefix = "QuikLiveSim-";

		[JsonIgnore]	public QuikStreaming		QuikStreamingPuppet;
		[JsonIgnore]	public QuikLivesimDdeClient	QuikLivesimDdeClient;

		[JsonIgnore]	ConcurrentDictionaryGeneric<double, double> LevelTwoAsks { get { return base.StreamingDataSnapshot.LevelTwoAsks; } }
		[JsonIgnore]	ConcurrentDictionaryGeneric<double, double> LevelTwoBids { get { return base.StreamingDataSnapshot.LevelTwoBids; } }

		public QuikLivesimStreaming() : base() {
			base.Name = "QuikLivesimStreaming-DllFound";
			base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimStreaming;

			//NO_DESERIALIZATION_WILL_THROW_YOULL_NULLIFY_ME_IN_UpstreamConnect YES_I_PROVOKE_NPE__NEED_TO_KNOW_WHERE_SNAPSHOT_IS_USED WILL_POINT_IT_TO_QUIK_REAL_STREAMING_IN_UpstreamConnect_LivesimStarting()
			//this.StreamingDataSnapshot = null;
		}

		public QuikLivesimStreaming(LivesimDataSource livesimDataSource) : base(livesimDataSource) {
			base.Name = "QuikLivesimStreaming-recreatedWithLDSpointer";
			base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimStreaming;
		}

		public override void Initialize(DataSource deserializedDataSource) {
			base.Name = "QuikLivesimStreaming";
			base.Initialize(deserializedDataSource);
		}

		protected override void SubscribeSolidifier() {
			string msg = "OTHERWIZE_BASE_WILL_SUBSCRIBE_SOLIDIFIER LIVESIM_MUST_NOT_SAVE_ANY_BARS";
		}

		public override void UpstreamConnect_LivesimStarting() {
			string msig = " //UpstreamConnect_LivesimStarting(" + this.ToString() + ")";

			this.QuikStreamingPuppet = new QuikStreamingPuppet(this.ddeTopicsPrefix, this.DataDistributor);
			this.QuikStreamingPuppet.Initialize(base.Livesimulator.DataSourceAsLivesimNullUnsafe);	//LivesimDataSource having LivesimBacktester and no-solidifier DataDistributor
			//this.QuikStreamingPuppet.DataDistributor.ConsumerQuoteSubscribe();
			//this.QuikStreamingPuppet.DataDistributor.ConsumerBarSubscribe();
			this.QuikStreamingPuppet.UpstreamConnect();


			// MarketLive checks for LastQuote, which I don't save anymore in QuikLivesimStreaming
			// QuikLivesimStreaming is a handicap without StreamingDataSnapshot; normally Snap is maintained by
			// 1) DataDistributorChart.StreamingDataSnapshot.LastQuoteCloneInitialize(symbol)
			//		but QuikLivesimStreaming.DataDistributor is donated to the Puppet
			// 2) StreamingAdapter(base).PushQuoteGenerated(): StreamingDataSnapshot.LastQuoteCloneSetForSymbol(quote);
			//		but QuikLivesimStreaming doesnt invoke base.PushQuoteGenerated(quote) because it shoots the quote to DDE and doesnt deal with Distributor
			//if (this.StreamingDataSnapshot != null) {
			//    string msg1 = "MUST_BE_NULL__ONLY_INITIALIZED_FOR_MarketLive_FOR_A_LIVESIM_SESSION__OTHERWIZE_MUST_BE_NULL";
			//    Assembler.PopupException(msg1);
			//}
			this.StreamingDataSnapshot = this.QuikStreamingPuppet.StreamingDataSnapshot;

			this.QuikLivesimDdeClient = new QuikLivesimDdeClient(this);
			this.QuikLivesimDdeClient.DdeClient.Connect();
			string msg = "DDE_CLIENT_CONNECTED[" + this.QuikStreamingPuppet.DdeServiceName + "] TOPICS[" + this.QuikStreamingPuppet.DdeBatchSubscriber.TopicsAsString + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		public override void UpstreamDisconnect_LivesimEnded() {
			string msig = " //UpstreamDisconnect_LivesimEnded(" + this.ToString() + ")";
			string msg = "Disposing QuikStreaming with prefixed DDE tables [...]";
			Assembler.PopupException(msg + msig, null, false);
			this.QuikLivesimDdeClient.DdeClient.Disconnect();
			this.QuikLivesimDdeClient.DdeClient.Dispose();
			this.QuikStreamingPuppet.UpstreamDisconnect();	// not disposed, QuikStreaming.ddeServerStart() is reusable

			// YES_I_PROVOKE_NPE__NEED_TO_KNOW_WHERE_SNAPSHOT_IS_USED WILL_POINT_IT_TO_QUIK_REAL_STREAMING_IN_UpstreamConnect_LivesimStarting()
			this.StreamingDataSnapshot = null;
		}

		public override void PushQuoteGenerated(QuoteGenerated quote) {
			//v3 REDIRECTING_PushQuoteGenerated_RADICAL_PARENT_DETACHED this.Level2generator.GenerateAndStoreInStreamingSnap(quote);

			bool isUnpaused = this.Unpaused.WaitOne(0);
			if (isUnpaused == false) {
				string msg = "QuikLIVESTREAMING_CAUGHT_PAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg, null, false);
				this.Unpaused.WaitOne();
				string msg2 = "QuikLIVESTREAMING_CAUGHT_UNPAUSE_BUTTON_PRESSED_IN_LIVESIM_CONTROL";
				//Assembler.PopupException(msg2, null, false);
			}

			base.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			base.LivesimSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}

			//LivesimStreaming.cs does {base.PushQuoteGenerated(quote);} here
			this.QuikLivesimDdeClient.SendQuote_DdeClientPokesDdeServer_waitServerProcessed(quote);

			AlertList notYetScheduled = base.LivesimBrokerSnap.AlertsNotYetScheduledForDelayedFillBy(quote);
			if (notYetScheduled.Count > 0) {
				if (quote.ParentBarStreaming != null) {
					string msg = "I_MUST_HAVE_IT_UNATTACHED_HERE";
					//Assembler.PopupException(msg);
				}
				base.LivesimBroker.ConsumeQuoteOfStreamingBarToFillPending(quote, notYetScheduled);
			} else {
				string msg = "NO_NEED_TO_PING_BROKER_EACH_NEW_QUOTE__EVERY_PENDING_ALREADY_SCHEDULED";
			}

			base.LivesimSpoiler.Spoil_after_PushQuoteGenerated();
			this.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;

			string msg1 = "CAN_PUSH_LEVEL2_NOW__AFTER_base.PushQuoteGenerated(quote)";
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new QuikLivesimStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
	}
}
