using System.Reflection;
using Sq1.Core;
using System.Collections.Generic;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.QuikAdapter.StreamingDdeApi.XlDde;

namespace Sq1.QuikAdapter.StreamingDdeApi {
	public class DdeProvider {
		XlDdeServer server;
		public DdeChannelLastQuote ChannelQuotes { get; protected set; }
		public DdeChannelTrades ChannelTrades { get; protected set; }
		Dictionary<string, List<XlDdeChannel>> IndividualSymbolChannels;
		protected QuikStreamingProvider quikStreamingProvider;
		public IStatusReporter StatusReporter { get; protected set; }
		public ConnectionState ConnectionState { get; protected set; }
		public DdeProvider(QuikStreamingProvider streamingProvider, IStatusReporter StatusReporter) {
			this.quikStreamingProvider = streamingProvider;
			this.StatusReporter = StatusReporter;
			this.server = new XlDdeServer(this.quikStreamingProvider.DdeServerPrefix);
			this.IndividualSymbolChannels = new Dictionary<string, List<XlDdeChannel>>();
			this.ConnectionState = ConnectionState.JustInitialized;
		}
		public void AddIndividualSymbolChannels(string symbol) {
			if (IndividualSymbolChannels.ContainsKey(symbol)) return;
			DdeChannelDepth channelDepth = null;
			string domTopic = this.quikStreamingProvider.DdeServerPrefix + "-" + this.quikStreamingProvider.DdeTopicPrefixDom + "-" + symbol;
			channelDepth = new DdeChannelDepth(domTopic, this.quikStreamingProvider, symbol);
			server.AddChannel(domTopic, channelDepth);
			IndividualSymbolChannels.Add(symbol, new List<XlDdeChannel>() { channelDepth });
			channelDepth.IsConnected = true;
		}
		public void RemoveIndividualSymbolChannels(string symbol) {
			if (IndividualSymbolChannels.ContainsKey(symbol) == false) return;
			List<XlDdeChannel> channels = IndividualSymbolChannels[symbol];
			if (channels == null) return;
			if (channels.Count == 0) return;
			foreach (XlDdeChannel channel in channels) {
				server.RemoveChannel(channel.Topic);
				channel.IsConnected = false;
			}
		}
		public bool SymbolHasIndividualChannels(string symbol) {
			return IndividualSymbolChannels.ContainsKey(symbol);
		}
		public string IndividualChannelsForSymbol(string symbol) {
			string ret = "Symbol[" + symbol + "]channels: ";
			if (SymbolHasIndividualChannels(symbol) == false) return ret + " is not registered with any individual channels";
			List<XlDdeChannel> channels = IndividualSymbolChannels[symbol];
			if (channels == null) return ret + " is registered with NULL list of individual channels";
			if (channels.Count == 0) return ret + " is registered with individualChannels.Count=0";
			foreach (XlDdeChannel channel in channels) {
				ret += " " + channel.ToString();
			}
			return ret;
		}
		public List<string> SymbolsHavingIndividualChannels {
			get {
				return new List<string>(this.IndividualSymbolChannels.Keys);
			}
		}
		public void StartDdeServer() {
			string msg = "";
			if (string.IsNullOrWhiteSpace(server.Service)) {
				Assembler.PopupException("can't start DdeServer with IsNullOrWhiteSpace(server.Service)");
				return;
			}

			this.ChannelQuotes = new DdeChannelLastQuote(this.quikStreamingProvider.DdeTopicQuotes, this.quikStreamingProvider);
			this.ChannelTrades = new DdeChannelTrades(this.quikStreamingProvider.DdeTopicTrades, this.quikStreamingProvider);

			server.AddChannel(ChannelQuotes.Topic, ChannelQuotes);
			server.AddChannel(this.quikStreamingProvider.DdeTopicTrades, this.ChannelTrades);

			try {
				server.Register();
			} catch (System.Exception e) {
				this.ConnectionState = ConnectionState.ErrorSymbolSubscribing;
				msg = "ERROR starting " + this + " " + e.ToString();
				this.UpdateConnectionStatus(0, msg, e);
				return;
			}
			ChannelQuotes.IsConnected = true;
			ChannelTrades.IsConnected = true;

			this.ConnectionState = ConnectionState.SymbolSubscribed;
			msg = "Started " + this;
			this.UpdateConnectionStatus(0, msg, null);
		}
		public void StopDdeServer() {
			string msg = "";
			try {
				server.Disconnect();
				server.Dispose();
				server = null;
			} catch (System.Exception e) {
				this.ConnectionState = ConnectionState.ErrorSymbolUnsubscribing;
				msg = "ERROR stopping " + this + " " + e.ToString();
				this.UpdateConnectionStatus(0, msg, e);
				return;
			}
			ChannelQuotes.IsConnected = false;

			this.ConnectionState = ConnectionState.SymbolUnsubscribed;
			msg = "Stopped " + this;
			this.UpdateConnectionStatus(0, msg, null);
			Assembler.PopupException("StopDdeServer() Disconnected DdeProvider " + msg);
		}
		public void UpdateConnectionStatus(int code, string msg, System.Exception e) {
			if (this.ConnectionState >= ConnectionState.ErrorConnecting || e != null) {
				Assembler.PopupException(this.ConnectionState + " " + msg, e);
			} else {
				Assembler.PopupException(this.ConnectionState + " " + msg);
			}
			if (StatusReporter == null) return;
			StatusReporter.UpdateConnectionStatus(this.ConnectionState, code, msg);
		}
		public override string ToString() {
			string ret = "DdeServerPrefix[" + this.quikStreamingProvider.DdeServerPrefix + "]/[" + this.ConnectionState + "]:";
			if (ChannelQuotes != null)  ret += " " + ChannelQuotes.ToString() + " ";
			if (ChannelTrades != null) ret += " " + ChannelTrades.ToString();
			string individualChannels = "";
			foreach (string symbol in this.SymbolsHavingIndividualChannels) {
				ret += " {"
					//+ " Symbol[" + symbol + "]"
					+ this.IndividualChannelsForSymbol(symbol) + "}";
			}
			if (individualChannels == "") individualChannels = " NO_INDIVIDUAL_CHANNELS";
			return ret;
		}
	}
}
