using System;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.QuikAdapter.StreamingDdeApi;

namespace Sq1.QuikAdapter {
	[DataContract]
	public class MockStreamingProvider : StreamingProvider {
		protected Dictionary<string, DdeProviderMock> MockProvidersBySymbol = new Dictionary<string, DdeProviderMock>();
		[DataMember]
		private int QuoteDelay;
		public int QuoteDelayAutoPropagate {
			get { return this.QuoteDelay; }
			internal set {
				this.QuoteDelay = value;
				lock (base.SymbolsSubscribedLock) {
					foreach (DdeProviderMock quoteGenerator in MockProvidersBySymbol.Values) {
						quoteGenerator.ChannelQuote.setNextQuoteDelayMs(this.QuoteDelayAutoPropagate);
					}
				}
			}
		}
		[DataMember]
		public List<string> GenerateOnlySymbols { get; internal set; }
		private string GenerateOnlySymbolsAsString {
			get {
				string ret = "";
				foreach (string symbol in GenerateOnlySymbols) ret += symbol + ",";
				ret = ret.TrimEnd(',');
				return ret;
			}
		}
		public QuikStreamingDataSnapshot QuikStreamingDataSnapshot {
			get {
				if (base.StreamingDataSnapshot is QuikStreamingDataSnapshot == false) {
					string msg = "base.StreamingDataSnapshot[" + base.StreamingDataSnapshot
						+ "] got modified while should remain of type QuikStreamingDataSnapshot"
						+ " since QuikStreamingProvider is constructed";
					throw new Exception(msg);
				}
				return base.StreamingDataSnapshot as QuikStreamingDataSnapshot;
			}
		}
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new MockStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}

		public MockStreamingProvider() : base() {
			this.GenerateOnlySymbols = new List<string>();
			base.Name = "Mock StreamingDummy";
			base.Description = "MOCK generating quotes, QuikTerminalMock is still used";
			base.Icon = (Bitmap)Sq1.QuikAdapter.Properties.Resources.imgMockStaticProvider;
			base.PreferredStaticProviderName = "MockStaticProvider";
			base.StreamingDataSnapshot = new QuikStreamingDataSnapshot(this);
			QuikStreamingDataSnapshot throwAtEarlyStage = this.QuikStreamingDataSnapshot;
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
					DdeProviderMock ddeQuotesProviderMock = null;
					if (this.MockProvidersBySymbol.ContainsKey(symbol) == false) {
						ddeQuotesProviderMock = new DdeProviderMock(this, symbol, this.StatusReporter);
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
				DdeProviderMock DdeQuotesProvider = new DdeProviderMock(this, DdeChannelName, this.StatusReporter);
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
			QuikQuote quikQuote = QuikQuote.SafeUpcast(quote);
			quikQuote.EnrichFromQuikStreamingDataSnapshot(this.QuikStreamingDataSnapshot);
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
			return "MockStreamingProvider: Symbols[" + this.SymbolsUpstreamSubscribedAsString + "]/only[" + this.GenerateOnlySymbolsAsString + "]"
				+ " DDE[" + this.DdeChannelsEstablished + "]";
		}
		public string DdeChannelsEstablished {
			get {
				string ret = "";
				lock (MockProvidersBySymbol) {
					foreach (string DdeChannelName in MockProvidersBySymbol.Keys) {
						if (ret != "") ret += " ";
						ret += DdeChannelName;
					}
				}
				return ret;
			}
		}
	}
}