using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using Newtonsoft.Json;
using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikMock.Dde;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock {
	public class StreamingMock : StreamingProvider {
		[JsonIgnore]	protected Dictionary<string, DdeChannelsMock> MockProvidersBySymbol = new Dictionary<string, DdeChannelsMock>();
		[JsonProperty]	private int QuoteDelay;
		[JsonIgnore]	public int QuoteDelayAutoPropagate {
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
		[JsonProperty]	public List<string> GenerateOnlySymbols { get; internal set; }
		[JsonIgnore]	private string GenerateOnlySymbolsAsString { get {
				string ret = "";
				foreach (string symbol in GenerateOnlySymbols) ret += symbol + ",";
				ret = ret.TrimEnd(',');
				return ret;
			} }
		[JsonIgnore]	public StreamingDataSnapshotQuik StreamingDataSnapshotQuik { get {
				if (base.StreamingDataSnapshot is StreamingDataSnapshotQuik == false) {
					string msg = "base.StreamingDataSnapshot[" + base.StreamingDataSnapshot
						+ "] got modified while should remain of type StreamingDataSnapshotQuik"
						+ " since QuikStreamingProvider is constructed";
					throw new Exception(msg);
				}
				return base.StreamingDataSnapshot as StreamingDataSnapshotQuik;
			} }
		[JsonIgnore]	public string DdeChannelsEstablished { get {
				string ret = "";
				lock (MockProvidersBySymbol) {
					foreach (string DdeChannelName in MockProvidersBySymbol.Keys) {
						if (ret != "") ret += " ";
						ret += DdeChannelName;
					}
				}
				return ret;
			} }
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new StreamingMockEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}

		public StreamingMock() : base() {
			this.GenerateOnlySymbols = new List<string>();
			base.Name = "Mock StreamingDummy";
			base.Description = "MOCK generating quotes, QuikTerminalMock is still used";
			base.Icon = (Bitmap)Sq1.Adapters.QuikMock.Properties.Resources.imgMockQuikStaticProvider;
			base.PreferredStaticProviderName = "MockStaticProvider";
			base.StreamingDataSnapshot = new StreamingDataSnapshotQuik(this);
			StreamingDataSnapshotQuik throwAtEarlyStage = this.StreamingDataSnapshotQuik;
		}
		public override void Initialize(DataSource dataSource, IStatusReporter statusReporter) {
			base.Initialize(dataSource, statusReporter);
			base.Name = "Mock StreamingProvider";
		}
		public override void Connect() {
			if (base.IsConnected == true) return;
			string ddeChannels = subscribeAllSymbols();
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

		private string subscribeAllSymbols() {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					DdeChannelsMock ddeQuotesProviderMock = null;
					if (this.MockProvidersBySymbol.ContainsKey(symbol) == false) {
						ddeQuotesProviderMock = new DdeChannelsMock(this, symbol, this.StatusReporter);
						this.StatusReporter = StatusReporter;

						this.MockProvidersBySymbol.Add(symbol, ddeQuotesProviderMock);
						Assembler.PopupException("MOCK: starting new DdeQuotesProvider[" + symbol + "]: " + ddeQuotesProviderMock);
						ddeQuotesProviderMock.StartDdeServer();
					} else {
						ddeQuotesProviderMock = this.MockProvidersBySymbol[symbol];
						Assembler.PopupException("MOCK: won't start existing DdeQuotesProvider[" + symbol + "]: " + ddeQuotesProviderMock);
					}
					if (ret != "") ret += " ";
					ret += symbol;
				}
			}
			return ret;
		}
		private string unsubscribeAllSymbols() {
			lock (base.SymbolsSubscribedLock) {
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
			}
		}

		public override void UpstreamSubscribe(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamSubscribe()";
				throw new Exception(msg);
			}
			lock (base.SymbolsSubscribedLock) {
				string DdeChannelName = symbol;
				if (this.MockProvidersBySymbol.ContainsKey(DdeChannelName)) {
					String msg = "already started DdeServer[" + DdeChannelName + "]";
					Assembler.PopupException(msg);
					this.StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorSymbolSubscribing, 1, msg);
					return;
				}
				DdeChannelsMock DdeQuotesProvider = new DdeChannelsMock(this, DdeChannelName, this.StatusReporter);
				if (QuoteDelayAutoPropagate > 0) DdeQuotesProvider.ChannelQuote.setNextQuoteDelayMs(QuoteDelayAutoPropagate);
				this.MockProvidersBySymbol.Add(DdeChannelName, DdeQuotesProvider);
				DdeQuotesProvider.StartDdeServer();
			}
		}
		public override void UpstreamUnSubscribe(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamUnSubscribe()";
				throw new Exception(msg);
			}
			lock (base.SymbolsSubscribedLock) {
				string DdeChannelName = symbol;
				if (this.MockProvidersBySymbol.ContainsKey(DdeChannelName)) {
					string msg = "UnSubscribe(" + symbol + "): won't StopDdeServer[" + DdeChannelName + "] coz we need to press CTRL+SHIFT+L in QUIK again";
					Assembler.PopupException(msg);
					this.StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorSymbolUnsubscribing, 1, msg);
					return;
				}
			}
		}
		public override bool UpstreamIsSubscribed(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamIsSubscribed()";
				throw new Exception(msg);
			}
			lock (base.SymbolsSubscribedLock) {
				string DdeChannelName = symbol;
				return this.MockProvidersBySymbol.ContainsKey(DdeChannelName);
			}
		}
		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}
		public void PropagateGeneratedQuoteCallback(Quote quote) {
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