using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeChannels {
		protected	StreamingQuik							quikStreamingAdapter;
		public		ConnectionState							ConnectionState { get; protected set; }
		
					XlDdeServer								server;
		
		public		DdeChannelLastQuote						ChannelQuotes { get; protected set; }
		public		DdeChannelTrades						ChannelTrades { get; protected set; }
					Dictionary<string, List<XlDdeChannel>>	channelsBySymbols;
		
		public DdeChannels(StreamingQuik streamingAdapter) {
			this.quikStreamingAdapter = streamingAdapter;
			this.server = new XlDdeServer(this.quikStreamingAdapter.DdeServerPrefix);
			this.channelsBySymbols = new Dictionary<string, List<XlDdeChannel>>();
			this.ConnectionState = ConnectionState.JustInitialized;
		}
		public void AddIndividualSymbolChannels(string symbol) {
			if (channelsBySymbols.ContainsKey(symbol)) return;
			DdeChannelDepth channelDepth = null;
			string domTopic = this.quikStreamingAdapter.DdeServerPrefix + "-" + this.quikStreamingAdapter.DdeTopicPrefixDom + "-" + symbol;
			channelDepth = new DdeChannelDepth(domTopic, this.quikStreamingAdapter, symbol);
			server.AddChannel(domTopic, channelDepth);
			channelsBySymbols.Add(symbol, new List<XlDdeChannel>() { channelDepth });
			channelDepth.IsConnected = true;
		}
		public void RemoveIndividualSymbolChannels(string symbol) {
			if (channelsBySymbols.ContainsKey(symbol) == false) return;
			List<XlDdeChannel> channels = channelsBySymbols[symbol];
			if (channels == null) return;
			if (channels.Count == 0) return;
			foreach (XlDdeChannel channel in channels) {
				server.RemoveChannel(channel.Topic);
				channel.IsConnected = false;
			}
		}
		public bool SymbolHasIndividualChannels(string symbol) {
			return channelsBySymbols.ContainsKey(symbol);
		}
		public string IndividualChannelsForSymbol(string symbol) {
			string ret = "Symbol[" + symbol + "]channels: ";
			if (SymbolHasIndividualChannels(symbol) == false) return ret + " is not registered with any individual channels";
			List<XlDdeChannel> channels = channelsBySymbols[symbol];
			if (channels == null) return ret + " is registered with NULL list of individual channels";
			if (channels.Count == 0) return ret + " is registered with individualChannels.Count=0";
			foreach (XlDdeChannel channel in channels) {
				ret += " " + channel.ToString();
			}
			return ret;
		}
		public List<string> SymbolsHavingIndividualChannels { get { return new List<string>(this.channelsBySymbols.Keys); } }
		public void StartDdeServer() {
			string msg = "";
			if (string.IsNullOrWhiteSpace(server.Service)) {
				Assembler.PopupException("can't start DdeServer with IsNullOrWhiteSpace(server.Service)");
				return;
			}

			this.ChannelQuotes = new DdeChannelLastQuote(this.quikStreamingAdapter.DdeTopicQuotes, this.quikStreamingAdapter);
			this.ChannelTrades = new DdeChannelTrades(this.quikStreamingAdapter.DdeTopicTrades, this.quikStreamingAdapter);

			server.AddChannel(ChannelQuotes.Topic, ChannelQuotes);
			server.AddChannel(this.quikStreamingAdapter.DdeTopicTrades, this.ChannelTrades);

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
			Assembler.PopupException("StopDdeServer() Disconnected DdeChannels " + msg);
		}
		public void UpdateConnectionStatus(int code, string msg, System.Exception e) {
			if (this.ConnectionState >= ConnectionState.ErrorConnecting || e != null) {
				Assembler.PopupException(this.ConnectionState + " " + msg, e);
			} else {
				Assembler.PopupException(this.ConnectionState + " " + msg);
			}
			Assembler.DisplayStatus(msg);
		}
		public override string ToString() {
			string ret = "DdeServerPrefix[" + this.quikStreamingAdapter.DdeServerPrefix + "]/[" + this.ConnectionState + "]:";
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
