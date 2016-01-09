using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;
using Sq1.Adapters.Quik.Streaming.Monitor;
using Sq1.Adapters.Quik.Streaming.Livesim;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreaming : StreamingAdapter {
		[JsonProperty]	public		string					DdeServiceName;		//QuikStreamingLivesim needs it public	{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicQuotes;		//QuikStreamingLivesim needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicTrades;		//QuikStreamingLivesim needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicPrefixDom;	//QuikStreamingLivesim needs it public { get; internal set; }

		[JsonIgnore]	public		XlDdeServer				DdeServer					{ get; private set; }
		[JsonProperty]	public		bool					DdeServerStarted			{ get; private set; }

		[JsonIgnore]	public		DdeBatchSubscriber		DdeBatchSubscriber			{ get; private set; }
		[JsonIgnore]				string					ddeChannelsEstablished		{ get {
				string ret = "DDE_CHANNELS_NULL__STREAMING_QUIK_NOT_YET_INITIALIZED";
				if (this.DdeBatchSubscriber == null) return ret;
				lock (base.SymbolsSubscribedLock) {
					ret = this.DdeBatchSubscriber.ToString();
				}
				return ret;
			} }
		
		[JsonIgnore]	protected	QuikStreamingDataSnapshot StreamingDataSnapshotQuik { get {
				if (base.StreamingDataSnapshot is QuikStreamingDataSnapshot == false) {
					string msg = "base.StreamingDataSnapshot[" + base.StreamingDataSnapshot
						+ "] got modified while should remain of type StreamingDataSnapshotQuik"
						+ " since QuikStreamingAdapter is constructed";
					throw new Exception(msg);
				}
				return base.StreamingDataSnapshot as QuikStreamingDataSnapshot;
			} }
		[JsonProperty]	public override List<string> SymbolsUpstreamSubscribed { get {
				List<string> ret = new List<string>();
				if (this.DdeBatchSubscriber == null) {
					string msg = "NO_SYMBOLS__STREAMING_QUIK_NOT_INITIALIZED_DDE_CHANNELS_NULL";
					return ret;
				}
				ret = this.DdeBatchSubscriber.SymbolsHavingIndividualTables;
				return ret;
			} }


		[JsonIgnore]	private	QuikStreamingMonitorForm monitorForm;
		[JsonIgnore]	public	QuikStreamingMonitorForm MonitorForm { get {
				if (this.monitorForm == null) {
					this.monitorForm = new QuikStreamingMonitorForm(this);
					this.monitorForm.FormClosed += delegate(object sender, System.Windows.Forms.FormClosedEventArgs e) {
						this.monitorForm = null;
					};
				}
				return this.monitorForm;
			} }

		[JsonIgnore]	public	string					IdentForMonitorWindowTitle { get {
			string ret = "";
			if (this.DataSource				!= null) ret += this.DataSource.Name;
			if (this.DataSource.MarketInfo	!= null) ret += " :: " + this.DataSource.MarketInfo.Name;
			ret += " [" + (string.IsNullOrEmpty(this.DdeTopicQuotes) ? "NO_DDE_TOPIC_QUOTES" : this.DdeTopicQuotes) + "]";
			ret += " //TablesReceived[" + this.DdeBatchSubscriber.AllDdeTablesReceivedCountersTotal + "]";
			return ret;
		} }

		[JsonIgnore]	public	string					DdeServerStartStopOppositeAction { get {
			return this.DdeServerStarted ? "Stop DDE Server (now started)" : "Start DDE Server (now stopped)";
			} }

		public QuikStreaming() : base() {
			base.Name					= "QuikStreaming-DllScanned";
			base.Icon					= (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingAdapter;
			this.DdeServiceName			= "SQ1";
			this.DdeTopicQuotes			= "quotes";
			this.DdeTopicTrades			= "trades";
			this.DdeTopicPrefixDom		= "dom";
			base.StreamingDataSnapshot	= new QuikStreamingDataSnapshot(this);
			this.ConnectionState		= ConnectionState.InitiallyDisconnected;
		}
		public override void Initialize(DataSource dataSource) {
			base.Name			= "QuikStreaming";
			this.DdeServer		= new XlDdeServer(this.DdeServiceName);	// MOVED_FROM_CTOR_TO_HAVE_QuikStreamingPuppet_PREFIX_SERVICE_AND_TOPICS DUMMY_STREAMING_ISNT_INITIALIZED_WITH_DATASOURCE_SO_IN_CTOR_IT_WOULD_HAVE_OCCUPIED_SERVICE_NAME_FOR_NO_USE

			if (this.DdeBatchSubscriber != null) {
				string msg = "RETHINK_INITIALIZATION_AND_DdeTables_LIFECYCLE";
				Assembler.PopupException(msg);
				this.DdeServerStop();
			} else {
				this.DdeBatchSubscriber = new DdeBatchSubscriber(this);
			}
			base.Initialize(dataSource);
			//MOVED_TO_MainForm.WorkspaceLoad() this.Connect();
			this.ConnectionState		= ConnectionState.JustInitialized;
			base.LivesimStreaming		= new QuikStreamingLivesim();
		}

		public void DdeServerStart() {
			if (string.IsNullOrWhiteSpace(this.DdeServer.Service)) {
				string msg1 = "can't start DdeServer with IsNullOrWhiteSpace(server.Service)";
				Assembler.PopupException(msg1);
				return;
			}

			if (string.IsNullOrWhiteSpace(this.DdeServer.Service)) {
				string msg1 = "DDE_SERVER_ALREADY_STARTED_FOR_QUIK_STREAMING_DATASOURCE[" + this.DataSource + "] WITH_TOPICS[" + this.ToString() + "]";
				Assembler.PopupException(msg1);
				return;
			}

			try {
				this.DdeServer.Register();
				this.DdeServerStarted = true;
				this.DdeBatchSubscriber.AllDdeTablesReceivedCountersReset();
			} catch (Exception ex) {
				this.ConnectionState = ConnectionState.ConnectFailed;
				Assembler.PopupException("DDE_SERVER_REGISTRATION_FAILED " + this.ToString(), ex);
				return;
			}

			this.ConnectionState = ConnectionState.ConnectedSubscribedAll;
			string msg = "DDE_SERVER_STARTED " + this.ToString();
			Assembler.PopupException(msg, null, false);
		}

		public void DdeServerStop() {
			try {
				this.DdeServer.Disconnect();
				this.DdeServer.Unregister();
				//NO_I_WILL_RESTART_IT this.DdeServer.Dispose();
				//NO_I_WILL_RESTART_IT this.ddeServer = null;
				this.DdeServerStarted = false;
			} catch (Exception ex) {
				this.ConnectionState = ConnectionState.DisconnectFailed;
				Assembler.PopupException("DDE_SERVER_STOPPING_ERROR " + this.ToString(), ex);
				return;
			}

			this.ConnectionState = ConnectionState.DisconnectedUnsubscribedAll;
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == false) {
				Assembler.PopupException("DDE_SERVER_STOPPED " + this.ToString(), null, false);
			}
		}


		string upstreamSubscribeAllDataSourceSymbols(bool avoidDuplicates_openChartsAlreadySubscribed = true) {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					if (avoidDuplicates_openChartsAlreadySubscribed && this.UpstreamIsSubscribed(symbol)) continue;
					this.UpstreamSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}
		string upstreamUnsubscribeAllDataSourceSymbols(bool avoidDuplicates_openChartsAlreadyUnsubscribed = true) {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					if (avoidDuplicates_openChartsAlreadyUnsubscribed && this.UpstreamIsSubscribed(symbol) == false) continue;
					this.UpstreamUnSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}
		
		public void TradeDeliveredDdeCallback(QuikTrade trade) {
			string msg = "FINALLY_I_CAN_SET_SET_FILLED_STATE_FOR_AN_ORDER REDIRECT_PROCESSING_TO_QuikBroker_ASAP";
			Assembler.PopupException(msg);
		}

		public override string ToString() {
			return this.Name + "/[" + this.ConnectionState + "]: Symbols[" + base.SymbolsUpstreamSubscribedAsString + "]"
				+ " DDE[" + this.ddeChannelsEstablished + "]";
		}
	}
}