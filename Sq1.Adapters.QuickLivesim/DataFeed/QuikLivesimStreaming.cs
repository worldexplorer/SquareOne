using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

using NDde.Client;

using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Backtesting;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikLivesim.DataFeed;
using Sq1.Core.Execution;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = false)]		// overriding LivesimStreaming's TRUE to have QuikLivesimStreaming appear in DataSourceEditor
	public partial class QuikLivesimStreaming : LivesimStreaming {
		string reasonToExist = "1) use LivesimForm as control "
			+ "2) instantiate QuikStreaming and make it run its DDE server "
			+ "3) push quotes generated using DDE client";

		string ddeTopicsPrefix = "QuikLiveSim-";

		public QuikStreaming		QuikStreamingPuppet;
		public QuikLivesimDdeClient	QuikLivesimDdeClient;

		//private QuikLivesimQuoteBarConsumer quikLivesimQuoteBarConsumer;

		//		QuikLivesimStreamingSettings	settings			{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		public QuikLivesimStreaming() : base() {
			base.Name = "QuikLivesimStreaming-DllFound";
			base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimStreaming;
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

		public override void UpstreamConnect_LivesimStarting(Livesimulator livesimulator) {
			//REDIRECTING_PushQuoteGenerated_RADICAL_PARENT_DETACHED
			//if (this.quikLivesimQuoteBarConsumer == null) {
			//    //I_REGISTERED_MY_OWN_CONSUMER OTHERWIZE_IGNORES_LIVESIM_DELAYS_AND_DOESNT_INJECT_QUOTES_TO_FILL_ALERTS
			//    this.quikLivesimQuoteBarConsumer = new QuikLivesimQuoteBarConsumer(livesimulator);
			//    livesimulator.ReplaceConsumerAndResubscribe_forLivesimStreamingChildren(this.quikLivesimQuoteBarConsumer);
			//}

			base.UpstreamConnect_LivesimStarting(livesimulator);

			string msig = " //UpstreamConnect_LivesimStarting(" + this.ToString() + ")";
			string msg = "Instantiating QuikStreaming with prefixed DDE tables [...]";
			//Assembler.PopupException(msg + msig, null, false);

			this.QuikStreamingPuppet = new QuikStreamingPuppet(this.ddeTopicsPrefix, this.DataDistributor);
			this.QuikStreamingPuppet.Initialize(base.LivesimulatorRedundant.DataSourceAsLivesimNullUnsafe);	//LivesimDataSource having LivesimBacktester and no-solidifier DataDistributor
			//this.QuikStreamingPuppet.DataDistributor.ConsumerQuoteSubscribe();
			//this.QuikStreamingPuppet.DataDistributor.ConsumerBarSubscribe();
			this.QuikStreamingPuppet.UpstreamConnect();

			this.QuikLivesimDdeClient = new QuikLivesimDdeClient(this);
			this.QuikLivesimDdeClient.DdeClient.Connect();
			msg = "DDE_CLIENT_CONNECTED[" + this.QuikStreamingPuppet.DdeServiceName + "] TOPICS[" + this.QuikStreamingPuppet.DdeBatchSubscriber.TopicsAsString + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		public override void UpstreamDisconnect_LivesimEnded() {
			string msig = " //UpstreamDisconnect_LivesimEnded(" + this.ToString() + ")";
			string msg = "Disposing QuikStreaming with prefixed DDE tables [...]";
			Assembler.PopupException(msg + msig, null, false);
			this.QuikLivesimDdeClient.DdeClient.Disconnect();
			this.QuikLivesimDdeClient.DdeClient.Dispose();
			this.QuikStreamingPuppet.UpstreamDisconnect();	// not disposed, QuikStreaming.ddeServerStart() is reusable
		}

		public override void PushQuoteGenerated(QuoteGenerated quote) {
			//NOPE_REDIRECT_TO_DDE_CLIENT_ALREADY_CONNECTED_TO_QUIK_PUPPET
			//v1 NOPE_THIS_IGNORES_LIVESIM_DELAYS_AND_DOESNT_INJECT_QUOTES_TO_FILL_ALERTS WILL_PROCEED_TO_MY_EXECUTOR_VIA_DATA_DISTRIBUTOR this.QuikLivesimDdeClient.DdeClientWillSendQuoteToDdeServer(quote);
			
			//v2 I_REGISTERED_MY_OWN_CONSUMER
			//base.PushQuoteGenerated(quote);

			//v3 REDIRECTING_PushQuoteGenerated_RADICAL_PARENT_DETACHED this.Level2generator.GenerateAndStoreInStreamingSnap(quote);

			base.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			base.LivesimSpoiler.Spoil_priorTo_PushQuoteGenerated();

			if (quote.IamInjectedToFillPendingAlerts) {
				string msg = "PROOF_THAT_IM_SERVING_ALL_QUOTES__REGULAR_AND_INJECTED";
			}

			//v2 base.PushQuoteGenerated(quote);
			this.QuikLivesimDdeClient.DdeClientWillSendQuoteToDdeServer(quote);
	
			// NO_NEED_IN_THIS_AT_ALL => STREAMING_WILL_INVALIDATE_ALL_PANELS,ORDERPROCESSOR_WILL_REBUILD_EXECUTION,EXECUTOR_REBUILDS_REPORTERS
			//if (base.chartShadow == null) {
			//    string msg = "DIDNT_YOU_FORGET_TO_LET_QuikStreaming_KNOW_ABOUT_CHART_CONTROL__TO_WAIT_FOR_REPAINT_COMPLETED_BEFORE_FEEDING_NEXT_QUOTE_TO_EXECUTOR_VIA_PUMP";
			//    Assembler.PopupException(msg);
			//    return;
			//}
			//12.9secOff vs 14.7secOn base.chartShadow.RefreshAllPanelsWaitFinishedSoLivesimCouldGenerateNewQuote();
			//base.chartShadow.Invalidate();

			//v2 HACK#1_BEFORE_I_INVENT_THE_BICYCLE_CREATE_MARKET_MODEL_WITH_SIMULATED_LEVEL2
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


		ConcurrentDictionaryGeneric<double, double> LevelTwoAsks { get { return base.StreamingDataSnapshot.LevelTwoAsks; } }
		ConcurrentDictionaryGeneric<double, double> LevelTwoBids { get { return base.StreamingDataSnapshot.LevelTwoBids; } }

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new QuikLivesimStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
	}
}
