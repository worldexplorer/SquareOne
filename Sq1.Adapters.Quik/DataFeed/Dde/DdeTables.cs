using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTables {
		protected	StreamingQuik							quikStreamingAdapter;
		public		ConnectionState							ConnectionState { get; protected set; }
		
					XlDdeServer								ddeServer;
		
		public		DdeTableLastQuotes						TableQuotes { get; protected set; }
		public		DdeTableTrades							TableTrades { get; protected set; }
					Dictionary<string, List<XlDdeTable>>	channelsBySymbol;
		
		public DdeTables(StreamingQuik streamingAdapter) {
			this.quikStreamingAdapter = streamingAdapter;
			this.ddeServer = new XlDdeServer(this.quikStreamingAdapter.DdeServerPrefix);
			this.channelsBySymbol = new Dictionary<string, List<XlDdeTable>>();
			this.ConnectionState = ConnectionState.JustInitialized;
		}
		public void AddIndividualSymbolChannels(string symbol) {
			if (this.channelsBySymbol.ContainsKey(symbol)) return;
			string domTopic = this.quikStreamingAdapter.DdeServerPrefix + "-" + this.quikStreamingAdapter.DdeTopicPrefixDom + "-" + symbol;
			DdeTableDepth channelDepth = new DdeTableDepth(domTopic, this.quikStreamingAdapter, symbol);
			this.ddeServer.AddChannel(domTopic, channelDepth);
			this.channelsBySymbol.Add(symbol, new List<XlDdeTable>() { channelDepth });
			channelDepth.IsConnected = true;
		}
		public void RemoveIndividualSymbolChannels(string symbol) {
			if (this.channelsBySymbol.ContainsKey(symbol) == false) return;
			List<XlDdeTable> channels = this.channelsBySymbol[symbol];
			if (channels == null) return;
			if (channels.Count == 0) return;
			foreach (XlDdeTable channel in channels) {
				this.ddeServer.RemoveChannel(channel.Topic);
				channel.IsConnected = false;
			}
		}
		public bool SymbolHasIndividualChannels(string symbol) {
			return this.channelsBySymbol.ContainsKey(symbol);
		}
		public string IndividualChannelsForSymbol(string symbol) {
			string ret = "Symbol[" + symbol + "]channels: ";
			if (SymbolHasIndividualChannels(symbol) == false) return ret + " is not registered with any individual channels";
			List<XlDdeTable> channels = this.channelsBySymbol[symbol];
			if (channels == null) return ret + " is registered with NULL list of individual channels";
			if (channels.Count == 0) return ret + " is registered with individualChannels.Count=0";
			foreach (XlDdeTable channel in channels) {
				ret += " " + channel.ToString();
			}
			return ret;
		}
		public List<string> SymbolsHavingIndividualChannels { get { return new List<string>(this.channelsBySymbol.Keys); } }
		public void StartDdeServer() {
			string msg = "";
			if (string.IsNullOrWhiteSpace(ddeServer.Service)) {
				Assembler.PopupException("can't start DdeServer with IsNullOrWhiteSpace(server.Service)");
				return;
			}

			this.TableQuotes = new DdeTableLastQuotes(	this.quikStreamingAdapter.DdeTopicQuotes, this.quikStreamingAdapter);
			this.TableTrades = new DdeTableTrades(		this.quikStreamingAdapter.DdeTopicTrades, this.quikStreamingAdapter);

			this.ddeServer.AddChannel(this.TableQuotes.Topic, this.TableQuotes);
			this.ddeServer.AddChannel(this.quikStreamingAdapter.DdeTopicTrades, this.TableTrades);

			try {
				this.ddeServer.Register();
			} catch (System.Exception e) {
				this.ConnectionState = ConnectionState.ErrorSymbolSubscribing;
				msg = "ERROR starting " + this + " " + e.ToString();
				this.PrintConnectionStatus(msg, e);
				return;
			}
			this.TableQuotes.IsConnected = true;
			this.TableTrades.IsConnected = true;

			this.ConnectionState = ConnectionState.SymbolSubscribed;
			msg = "Started " + this;
			this.PrintConnectionStatus(msg, null);
		}
		public void StopDdeServer() {
			string msg = "";
			try {
				this.ddeServer.Disconnect();
				this.ddeServer.Dispose();
				this.ddeServer = null;
			} catch (System.Exception e) {
				this.ConnectionState = ConnectionState.ErrorSymbolUnsubscribing;
				msg = "ERROR stopping " + this + " " + e.ToString();
				this.PrintConnectionStatus(msg, e);
				return;
			}
			this.TableQuotes.IsConnected = false;

			this.ConnectionState = ConnectionState.SymbolUnsubscribed;
			msg = "Stopped " + this;
			this.PrintConnectionStatus(msg, null);
			Assembler.PopupException("StopDdeServer() Disconnected DdeChannels " + msg);
		}
		public void PrintConnectionStatus(string msg, Exception e) {
			if (this.ConnectionState >= ConnectionState.ErrorConnecting || e != null) {
				Assembler.PopupException(this.ConnectionState + " " + msg, e);
			} else {
				Assembler.PopupException(this.ConnectionState + " " + msg);
			}
			Assembler.DisplayStatus(msg);
		}
		public override string ToString() {
			string ret = "DdeServerPrefix[" + this.quikStreamingAdapter.DdeServerPrefix + "]/[" + this.ConnectionState + "]:";
			if (this.TableQuotes != null)  ret += " " + this.TableQuotes.ToString() + " ";
			if (this.TableTrades != null) ret += " " + this.TableTrades.ToString();
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
