﻿using System;
using System.Drawing;

using Sq1.Core.Support;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core;
using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikLivesim.DataFeed;
using System.Threading.Tasks;
using System.Threading;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = false)]		// overriding LivesimStreaming's TRUE to have QuikLivesimStreaming appear in DataSourceEditor
	public partial class QuikLivesimStreaming : LivesimStreaming {
		string reasonToExist = "1) use LivesimForm as control "
			+ "2) instantiate QuikStreaming and make it run its DDE server "
			+ "3) push quotes generated using DDE client";

		string ddeTopicsPrefix = "QuikLiveSim-";

		public QuikStreaming QuikStreamingPuppet;
		QuikLivesimDdeClient QuikLivesimDdeClient;

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
			base.UpstreamConnect_LivesimStarting(livesimulator);

			string msig = " //UpstreamConnect_LivesimStarting(" + this.ToString() + ")";
			string msg = "Instantiating QuikStreaming with prefixed DDE tables [...]";
			Assembler.PopupException(msg + msig, null, false);

			this.QuikStreamingPuppet = new QuikStreamingPuppet(this.ddeTopicsPrefix);
			this.QuikStreamingPuppet.Initialize(base.Livesimulator.DataSourceAsLivesimNullUnsafe);	//LivesimDataSource having LivesimBacktester and no-solidifier DataDistributor
			//ALREADY_INVOKED_BY_DataDistributor_Ignored_to_avoid_Solidifier this.quikStreamingPuppet.UpstreamConnect();
			this.QuikStreamingPuppet.UpstreamConnect();

			this.QuikLivesimDdeClient = new QuikLivesimDdeClient(this);
			//this.QuikLivesimDdeClient.DdeClient.Connect();
			//this.tenTimesConnect();
			Thread.Sleep(10 * 1000);
		}

		int attemtpsLeft = 10;
		void tenTimesConnect() {
			if (--this.attemtpsLeft <= 0) {
				string msg = "TEN_TIMES_FAILED__ENOUGH";
				Assembler.PopupException(msg);
				return;
			}
			Task tenAfterTen = new Task(delegate {
				Thread.Sleep(1 * 1000);
				try {
					this.QuikLivesimDdeClient.DdeClient.Connect();
				} catch (Exception ex) {
					throw ex;
				}
				string msg = "SERVICE_REGISTERED[" + this.QuikStreamingPuppet.DdeServiceName + "] TOPICS[" + this.QuikStreamingPuppet.DdeSubscriptionManager.TopicsAsString + "]";
				Assembler.PopupException(msg, null, false);
				this.attemtpsLeft = 0;
			});
			tenAfterTen.ContinueWith(delegate {
				string msg = "FAILED_SERVICE_REGISTERED[" + this.QuikStreamingPuppet.DdeServiceName + "] TOPICS[" + this.QuikStreamingPuppet.DdeSubscriptionManager.TopicsAsString + "]";
				Assembler.PopupException(msg);
				this.tenTimesConnect();
			}, TaskContinuationOptions.OnlyOnFaulted);
			tenAfterTen.Start();
		}
		public override void UpstreamDisconnect_LivesimEnded() {
			string msig = " //UpstreamDisconnect_LivesimEnded(" + this.ToString() + ")";
			string msg = "Disposing QuikStreaming with prefixed DDE tables [...]";
			Assembler.PopupException(msg + msig, null, false);
			this.QuikStreamingPuppet.UpstreamDisconnect();
		}




		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new QuikLivesimStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
	}
}