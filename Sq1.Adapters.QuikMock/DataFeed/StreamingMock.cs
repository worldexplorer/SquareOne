using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikMock.Dde;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.QuikMock {
	public class StreamingMock : StreamingProvider {
		[JsonIgnore]	protected	Dictionary<string, DdeChannelsMock> MockProvidersBySymbol;
		[JsonProperty]				int				QuoteDelay;
		[JsonIgnore]	public		int				QuoteDelayAutoPropagate {
			get { return this.QuoteDelay; }
			internal set {
				this.QuoteDelay = value;
				lock (base.SymbolsSubscribedLock) {
					foreach (DdeChannelsMock quoteGenerator in MockProvidersBySymbol.Values) {
						quoteGenerator.ChannelQuote.setNextQuoteDelayMs(this.QuoteDelayAutoPropagate);
					}
				}
			}
		}
		[JsonProperty]	public		List<string>	GenerateOnlySymbols { get; internal set; }
		[JsonIgnore]	private		string			GenerateOnlySymbolsAsString { get {
				string ret = "";
				foreach (string symbol in GenerateOnlySymbols) ret += symbol + ",";
				ret = ret.TrimEnd(',');
				return ret;
			} }
		[JsonIgnore]	public		StreamingDataSnapshotQuik StreamingDataSnapshotQuik { get {
				if (base.StreamingDataSnapshot is StreamingDataSnapshotQuik == false) {
					string msg = "base.StreamingDataSnapshot[" + base.StreamingDataSnapshot
						+ "] got modified while should remain of type StreamingDataSnapshotQuik"
						+ " since QuikStreamingProvider is constructed";
					throw new Exception(msg);
				}
				return base.StreamingDataSnapshot as StreamingDataSnapshotQuik;
			} }
		[JsonIgnore]	public		string			DdeChannelsEstablished { get { lock (MockProvidersBySymbol) {
					string ret = "";
					foreach (string DdeChannelName in MockProvidersBySymbol.Keys) {
						if (ret != "") ret += " ";
						ret += DdeChannelName;
					}
					return ret;
				} } }
		[JsonProperty]	public		bool			GeneratingNow { get; internal set; }
		[JsonIgnore]	public		bool			GeneratingNowAutoPropagate {
			get { return this.GeneratingNow; }
			internal set {
				this.GeneratingNow = value;
				lock (base.SymbolsSubscribedLock) {
					foreach (DdeChannelsMock quoteGenerator in this.MockProvidersBySymbol.Values) {
						if (this.GeneratingNow) {
							quoteGenerator.ChannelQuote.MockStart();
						} else {
							quoteGenerator.ChannelQuote.MockStop();
						}
					}
				}
			}
		}
		
		public StreamingMock() : base() {
			this.GenerateOnlySymbols = new List<string>();
			this.MockProvidersBySymbol = new Dictionary<string, DdeChannelsMock>();
			base.Name = "Mock StreamingDummy";
			base.Description = "MOCK generating quotes, QuikTerminalMock is still used";
			base.Icon = (Bitmap)Sq1.Adapters.QuikMock.Properties.Resources.imgMockQuikStaticProvider;
			base.PreferredStaticProviderName = "MockStaticProvider";
			base.StreamingDataSnapshot = new StreamingDataSnapshotQuik(this);
			StreamingDataSnapshotQuik throwAtEarlyStage = this.StreamingDataSnapshotQuik;
		}
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new StreamingMockEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
		public override void Initialize(DataSource dataSource) {
			base.Initialize(dataSource);
			base.Name = "QuikMock Streaming";
			if (this.GeneratingNow) {
				this.Connect();
			}
		}
		public override void Connect() {
			if (base.IsConnected == true) return;
			string ddeChannels = this.subscribeAllSymbols();
			base.ConnectionState = ConnectionState.Connected;
			base.UpdateConnectionStatus(0, "Started ddeChannels[" + ddeChannels + "]");
			base.IsConnected = true;
		}
		public override void Disconnect() {
			if (base.IsConnected == false) return;
			string ddeChannels = unsubscribeAllSymbols();
			base.ConnectionState = ConnectionState.Disconnected;
			base.UpdateConnectionStatus( 0, "Stopped DdeChannels[" + ddeChannels + "]");
			base.IsConnected = false;
		}

		string subscribeAllSymbols() { lock (base.SymbolsSubscribedLock) {
				string ret = "";
				foreach (string symbol in base.DataSource.Symbols) {
					DdeChannelsMock ddeQuotesProviderMock = null;
					if (this.MockProvidersBySymbol.ContainsKey(symbol) != false) {
						ddeQuotesProviderMock = this.MockProvidersBySymbol[symbol];
						string msg = "ALREADY_STARTED_DDE_QUOTES_FOR[" + symbol + "]: [" + ddeQuotesProviderMock + "]";
						Assembler.PopupException(msg, null, false);
						continue;
					}
						
					ddeQuotesProviderMock = new DdeChannelsMock(this, symbol);
					this.MockProvidersBySymbol.Add(symbol, ddeQuotesProviderMock);
					//this.UpstreamSubscribe(symbol);
					
					if (ret != "") ret += " ";
					ret += symbol;
				}
				return ret;
			} }
		string unsubscribeAllSymbols() { lock (base.SymbolsSubscribedLock) {
				string ret = "";
				foreach (string symbol in base.DataSource.Symbols) {
					if (this.MockProvidersBySymbol.ContainsKey(symbol)) {
						//this.MockProvidersBySymbol[DdeChannelName].StopDdeServer();
						//this.MockProvidersBySymbol.Remove(DdeChannelName);
						Assembler.PopupException("stopAllDdeServers(): won't StopDdeServer(" + symbol + ") coz we need to press CTRL+SHIFT+L in QUIK again");
					}
					if (ret != "") ret += " ";
					ret += symbol;
				}
				return ret;
			} }

		public override void UpstreamSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
				string msig = " //UpstreamSubscribe(" + symbol + ")";
				
				if (String.IsNullOrEmpty(symbol)) {
					string msg = "I_REFUSE_TO_PING_UPSTREAM_WITH_EMPTY_SYMBOL";
					throw new Exception(msg + msig);
				}
				if (this.MockProvidersBySymbol.ContainsKey(symbol)) {
					string msg2 = "DDE_QUOTES_ALREADY_STARTED";
					Assembler.PopupException(msg2 + msig, null, false);
					//this.StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorSymbolSubscribing, 1, msg2);
					return;
				}
				DdeChannelsMock ddeChannelsMock = new DdeChannelsMock(this, symbol);
				msig += ": [" + ddeChannelsMock.ToString() + "]";
				
				if (this.QuoteDelayAutoPropagate > 0) ddeChannelsMock.ChannelQuote.setNextQuoteDelayMs(this.QuoteDelayAutoPropagate);
				this.MockProvidersBySymbol.Add(symbol, ddeChannelsMock);

				string msg3 = "STARTING_NEW_DDE_QUOTES";
				string started = (this.GeneratingNow) ? ddeChannelsMock.DdeServerStart() : "NOT_GENERATING_NOW";
				Assembler.PopupException(started + "... <= " + msg3 + msig, null, false);
			} }
		public override void UpstreamUnSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
				if (String.IsNullOrEmpty(symbol)) {
					string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamUnSubscribe()";
					throw new Exception(msg);
				}
				string ddeChannelName = symbol;
				if (this.MockProvidersBySymbol.ContainsKey(ddeChannelName)) {
					string msg = "UnSubscribe(" + symbol + "): won't StopDdeServer[" + ddeChannelName + "] coz we need to press CTRL+SHIFT+L in QUIK again";
					Assembler.PopupException(msg, null, false);
					Assembler.DisplayStatus("ConnectionState.ErrorSymbolUnsubscribing " + msg);
					return;
				}
			} }
		public override bool UpstreamIsSubscribed(string symbol) { lock (base.SymbolsSubscribedLock) {
				if (String.IsNullOrEmpty(symbol)) {
					string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamIsSubscribed()";
					throw new Exception(msg);
				}
				string DdeChannelName = symbol;
				return this.MockProvidersBySymbol.ContainsKey(DdeChannelName);
			} }
		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}
		public void PushQuoteGenerated(Quote quote) {
			if (this.GenerateOnlySymbols.Count > 0 && this.GenerateOnlySymbols.Contains(quote.Symbol) == false) return;
			quote.Source = "MockQuik";
			//if (quote.PriceLastDeal == 0) {
			//	Assembler.PopupException("this.ConsumeDdeDeliveredQuote(" + quote.Symbol + "): returning since CHARTS will screw up painting it; price=0: quote=[" + quote + "]");
			//	return;
			//}
			if (string.IsNullOrEmpty(quote.Source)) quote.Source = "Quik";
			base.PushQuoteReceived(quote);
		}
		public override string ToString() {
			return "StreamingMock: SymbolsSubscribed[" + this.SymbolsUpstreamSubscribedAsString + "]"
				+ "/SymbolsGenerating[" + this.GenerateOnlySymbolsAsString + "]"
				+ " DDE[" + this.DdeChannelsEstablished + "]";
		}
	}
}