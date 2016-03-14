using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;
using Sq1.Adapters.Quik.Streaming.Livesim;
using Sq1.Adapters.Quik.Streaming.Monitor;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreaming : StreamingAdapter {
		[JsonProperty]	public		string					DdeServiceName;				//QuikStreamingLivesim needs it public	{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicQuotes;				//QuikStreamingLivesim needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicTrades;				//QuikStreamingLivesim needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicSuffixDom;			//QuikStreamingLivesim needs it public { get; internal set; }
		[JsonProperty]	public		int						DdeMonitorRefreshRateMs;
		[JsonProperty]	public		bool					DdeMonitorPopupOnRestart;

		[JsonIgnore]	public		XlDdeServer				DdeServer					{ get; private set; }
		[JsonIgnore]	public		bool					DdeServerIsRegistered		{ get { return this.DdeServer != null && this.DdeServer.IsRegistered; } }

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
				ret = this.DdeBatchSubscriber.SymbolsDOMsSubscribed;
				return ret;
			} }


		[JsonIgnore]	private	QuikStreamingMonitorForm monitorForm;
		[JsonIgnore]	public	QuikStreamingMonitorForm MonitorForm { get {
				if (this.monitorForm == null || this.monitorForm.IsDisposed) {
					this.monitorForm = new QuikStreamingMonitorForm(this);
				}
				return this.monitorForm;
			} }

		[JsonIgnore]	public	string					IdentForMonitorWindowTitle { get {
				string ret = "";
				if (this.DataSource				!= null) ret += this.DataSource.Name;
				if (this.DataSource.MarketInfo	!= null) ret += " :: " + this.DataSource.MarketInfo.Name;
				ret += " [" + (string.IsNullOrEmpty(this.DdeTopicQuotes) ? "NO_DDE_TOPIC_QUOTES" : this.DdeTopicQuotes) + "]";
				ret += " //TablesReceived[" + this.DdeBatchSubscriber.AllDdeMessagesReceivedCounters_total + "]";
				return ret;
			} }

		[JsonIgnore]	public	string					DdeServerStartStop_oppositeAction { get {
			return this.DdeServerIsRegistered ? "Stop DDE Server (now started)" : "Start DDE Server (now stopped)";
			} }

		public QuikStreaming() : base() {
			base.Name						= "QuikStreaming-DllScanned";
			base.Icon						= (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingAdapter;
			this.DdeServiceName				= "SQ1";
			this.DdeTopicQuotes				= "quotes";
			this.DdeTopicTrades				= "trades";
			this.DdeTopicSuffixDom			= "dom";
			this.DdeMonitorRefreshRateMs	= 200;
			this.DdeMonitorPopupOnRestart	= false;
			base.StreamingDataSnapshot		= new QuikStreamingDataSnapshot(this);
			this.UpstreamConnectionState	= ConnectionState.Streaming_DisconnectedJustConstructed;
		}

		public void DdeServerRegister() {
			if (string.IsNullOrWhiteSpace(this.DdeServer.Service)) {
				string msg1 = "can't start DdeServer with IsNullOrWhiteSpace(server.Service)";
				Assembler.PopupException(msg1);
				return;
			}

			if (this.DdeServerIsRegistered) {
				string msg1 = "OKAY_TO_IGNORE_IF_YOU_SEE_UpstreamConnect_LivesimStarting_UPSTACK"
					+ " DDE_SERVER_ALREADY_REGISTERED_FOR_QUIK_STREAMING_DATASOURCE[" + this.DataSource + "] WITH_TOPICS[" + this.ToString() + "]";
				Assembler.PopupException(msg1, null, false);
				return;
			}

			try {
				this.DdeServer.Register();
				base.UpstreamConnectionState = ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribed;		// will result in StreamingConnected=true
				string msg = "DDE_SERVER_STARTED " + this.ToString();
				Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				this.UpstreamConnectionState = ConnectionState.FailedToConnect;
				Assembler.PopupException("DDE_SERVER_REGISTRATION_FAILED " + this.ToString(), ex);
				return;
			}
		}

		public void DdeServerUnregister() {
			if (this.DdeServerIsRegistered == false) {
				string msg1 = "ARE_YOU_LIVESIMMING? DDE_SERVER_ALREADY_UNREGISTERRED_FOR_QUIK_STREAMING_DATASOURCE[" + this.DataSource + "] WITH_TOPICS[" + this.ToString() + "]";
				Assembler.PopupException(msg1);
				return;
			}

			try {
				this.DdeServer.Disconnect();
				this.DdeServer.Unregister();
				//NO_I_WILL_RESTART_IT this.DdeServer.Dispose();
				//NO_I_WILL_RESTART_IT this.ddeServer = null;
				base.UpstreamConnectionState = ConnectionState.Streaming_UpstreamDisconnected_downstreamUnsubscribed;
			} catch (Exception ex) {
				this.UpstreamConnectionState = ConnectionState.FailedToDisconnect;
				Assembler.PopupException("DDE_SERVER_STOPPING_ERROR " + this.ToString(), ex);
				return;
			}

			// I didn't mention/implement unsubscribeAll()?
			//this.ConnectionState = ConnectionState.UpstreamDisconnected_downstreamUnsubscribed;
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
			return this.Name + "/[" + this.UpstreamConnectionState + "]: Symbols[" + base.SymbolsUpstreamSubscribedAsString + "]"
				+ " DDE[" + this.ddeChannelsEstablished + "]";
		}
	}
}