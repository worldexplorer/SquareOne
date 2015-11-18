using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Dde;
using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik {
	public partial class QuikStreaming : StreamingAdapter {
		[JsonProperty]	public		string					DdeServiceName;	//QuikLivesimStreaming needs it public	{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicQuotes;		//QuikLivesimStreaming needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicTrades;		//QuikLivesimStreaming needs it public{ get; internal set; }
		[JsonProperty]	public		string					DdeTopicPrefixDom;	//QuikLivesimStreaming needs it public { get; internal set; }

		[JsonIgnore]	public		XlDdeServer				DdeServer					{ get; private set; }
		[JsonIgnore]	public		ConnectionState			ConnectionState				{ get; private set; }
		[JsonIgnore]	public		DdeSubscriptionManager	DdeSubscriptionManager		{ get; private set; }
		[JsonIgnore]				string					ddeChannelsEstablished		{ get {
				string ret = "DDE_CHANNELS_NULL__STREAMING_QUIK_NOT_YET_INITIALIZED";
				if (this.DdeSubscriptionManager == null) return ret;
				lock (base.SymbolsSubscribedLock) {
					ret = this.DdeSubscriptionManager.ToString();
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
				if (this.DdeSubscriptionManager == null) {
					string msg = "NO_SYMBOLS__STREAMING_QUIK_NOT_INITIALIZED_DDE_CHANNELS_NULL";
					return ret;
				}
				ret = this.DdeSubscriptionManager.SymbolsHavingIndividualChannels;
				return ret;
			} }
		
		public QuikStreaming() : base() {
			base.Name = "QuikStreaming-DllScanned";
			base.Icon = (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingAdapter;
			this.DdeServiceName		= "SQ1";
			this.DdeTopicQuotes		= "quotes";
			this.DdeTopicTrades		= "trades";
			this.DdeTopicPrefixDom	= "dom";
			base.StreamingDataSnapshot	= new QuikStreamingDataSnapshot(this);
			this.DdeServer				= new XlDdeServer(this.DdeServiceName);
			this.ConnectionState		= ConnectionState.InitiallyDisconnected;
		}
		public override void Initialize(DataSource dataSource) {
			base.Name = "QuikStreaming";
			if (this.DdeSubscriptionManager != null) {
				string msg = "RETHINK_INITIALIZATION_AND_DdeTables_LIFECYCLE";
				Assembler.PopupException(msg);
				this.stopDdeServer();
			} else {
				this.DdeSubscriptionManager = new DdeSubscriptionManager(this);
			}
			base.Initialize(dataSource);
			//MOVED_TO_MainForm.WorkspaceLoad() this.Connect();
			this.ConnectionState		= ConnectionState.JustInitialized;
		}

		void startDdeServer() {
			string msg = "";
			if (string.IsNullOrWhiteSpace(this.DdeServer.Service)) {
				Assembler.PopupException("can't start DdeServer with IsNullOrWhiteSpace(server.Service)");
				return;
			}

			try {
				this.DdeServer.Register();
			} catch (Exception ex) {
				this.ConnectionState = ConnectionState.ConnectFailed;
				Assembler.PopupException(this.ToString(), ex);
				return;
			}

			this.ConnectionState = ConnectionState.ConnectedSubscribedAll;
			msg = DateTime.Now + " DDE_REGISTERED " + this.ToString();
			Assembler.PopupException(msg, null, false);
		}
		public void stopDdeServer() {
			string msg = "";
			try {
				this.DdeServer.Disconnect();
				this.DdeServer.Unregister();
				//this.ddeServer.Dispose();
				//this.ddeServer = null;
			} catch (Exception e) {
				this.ConnectionState = ConnectionState.DisconnectFailed;
				msg = "ERROR stopping " + this + " " + e.ToString();
				Assembler.PopupException(this.ConnectionState + " " + msg, e);
				return;
			}

			this.ConnectionState = ConnectionState.DisconnectedUnsubscribedAll;
			msg = "Stopped " + this;
			Assembler.PopupException(this.ConnectionState + " " + msg, null);
			Assembler.PopupException("StopDdeServer() Disconnected DdeChannels " + msg);
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

		
		
		public void TradeDeliveredDdeCallback(string skey, DdeTrade trade) {
			string msg = "FINALLY_I_CAN_SET_SET_FILLED_STATE_FOR_AN_ORDER REDIRECT_PROCESSING_TO_QuikBroker_ASAP";
			Assembler.PopupException(msg);
		}

		public override string ToString() {
			return this.Name + "/[" + this.ConnectionState + "]: Symbols[" + base.SymbolsUpstreamSubscribedAsString + "]"
				+ " DDE[" + this.ddeChannelsEstablished + "]";
		}
	}
}