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
		[JsonIgnore]	protected	Dictionary<string, DdeChannelsMock> MockDdeChannelsBySymbol;
		[JsonProperty]				int				QuoteDelay;
		[JsonIgnore]	public		int				QuoteDelayAutoPropagate {
			get { return this.QuoteDelay; }
			internal set {
				this.QuoteDelay = value;
				lock (base.SymbolsSubscribedLock) {
					foreach (DdeChannelsMock quoteGenerator in MockDdeChannelsBySymbol.Values) {
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
		[JsonIgnore]	public		string			DdeChannelsEstablished { get { lock (MockDdeChannelsBySymbol) {
					string ret = "";
					foreach (string DdeChannelName in MockDdeChannelsBySymbol.Keys) {
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
					foreach (DdeChannelsMock quoteGenerator in this.MockDdeChannelsBySymbol.Values) {
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
			this.MockDdeChannelsBySymbol = new Dictionary<string, DdeChannelsMock>();
			base.Name = "StreamingQuikMockDummy";
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
			base.Name = "StreamingQuikMock";
			//MOVED_TO_MainForm.WorkspaceLoad() if (this.GeneratingNow) this.Connect();
		}
		public override void UpstreamConnect() {
			if (base.IsConnected == true) return;
			string stats = this.subscribeAllSymbols();
			base.ConnectionState = ConnectionState.ConnectedUnsubscribed;
			Assembler.DisplayConnectionStatus(base.ConnectionState, "Started [" + stats + "]");
			base.IsConnected = true;
		}
		public override void UpstreamDisconnect() {
			if (base.IsConnected == false) return;
			string stats = unsubscribeAllSymbols();
			base.ConnectionState = ConnectionState.Disconnected;
			Assembler.DisplayConnectionStatus(base.ConnectionState, "Stopped [" + stats + "]");
			base.IsConnected = false;
		}

		string subscribeAllSymbols() { lock (base.SymbolsSubscribedLock) {
				string ret = "";
				foreach (string symbol in base.DataSource.Symbols) {
					if (this.MockDdeChannelsBySymbol.ContainsKey(symbol) == false) {
						DdeChannelsMock channels = new DdeChannelsMock(this, symbol);
						string msg = "SHOULD_HAVE_BEEN_ADDED_ALREADY " + channels;
						Assembler.PopupException(msg);
						this.MockDdeChannelsBySymbol.Add(symbol, channels);
					}
					DdeChannelsMock ddeChannelsMock = this.MockDdeChannelsBySymbol[symbol];
					string started = this.GeneratingNow ? ddeChannelsMock.AllChannelsForSymbolStart() : "GENERATING_NOW=FALSE";
					string msg2 = "subscribed[" + symbol + "]: [" + started + "]";
					Assembler.PopupException(msg2, null, false);
					//this.UpstreamSubscribe(symbol);
					
					if (ret != "") ret += " ";
					ret += symbol;
				}
				return ret;
			} }
		string unsubscribeAllSymbols() { lock (base.SymbolsSubscribedLock) {
				string ret = "";
				foreach (string symbol in base.DataSource.Symbols) {
					if (this.MockDdeChannelsBySymbol.ContainsKey(symbol) == false) {
						string msg = "stopAllDdeServers(): won't StopDdeServer(" + symbol + ") coz we need to press CTRL+SHIFT+L in QUIK again";
						Assembler.PopupException(msg);
						continue;
					}
					ret += this.MockDdeChannelsBySymbol[symbol].AllChannelsForSymbolStop();
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
				if (this.MockDdeChannelsBySymbol.ContainsKey(symbol)) {
					string msg2 = "SYMBOL_ALREADY_SUBSCRIBED " + this.MockDdeChannelsBySymbol[symbol].ToString();
					Assembler.PopupException(msg2 + msig, null, false);
					//this.StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorSymbolSubscribing, 1, msg2);
					return;
				}
				DdeChannelsMock ddeChannelsMock = new DdeChannelsMock(this, symbol);
				msig += ": [" + ddeChannelsMock.ToString() + "]";
				
				if (this.QuoteDelayAutoPropagate > 0) ddeChannelsMock.ChannelQuote.setNextQuoteDelayMs(this.QuoteDelayAutoPropagate);
				this.MockDdeChannelsBySymbol.Add(symbol, ddeChannelsMock);

				string msg3 = "SYMBOL_SUBSCRIBED";
				string started = (base.IsConnected && this.GeneratingNow) ? ddeChannelsMock.AllChannelsForSymbolStart() : "NOT_GENERATING_NOW";
				Assembler.PopupException(started + "... <= " + msg3 + msig, null, false);
			} }
		public override void UpstreamUnSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
				if (String.IsNullOrEmpty(symbol)) {
					string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamUnSubscribe()";
					throw new Exception(msg);
				}
				string ddeChannelName = symbol;
				if (this.MockDdeChannelsBySymbol.ContainsKey(ddeChannelName)) {
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
				return this.MockDdeChannelsBySymbol.ContainsKey(DdeChannelName);
			} }
		public override void PushQuoteReceived(Quote quote) {
			if (this.GenerateOnlySymbols.Count > 0 && this.GenerateOnlySymbols.Contains(quote.Symbol) == false) return;
			DateTime thisDayClose = this.DataSource.MarketInfo.getThisDayClose(quote);
			DateTime preMarketQuotePoitingToThisDayClose = quote.ServerTime.AddSeconds(1);
			bool isQuikPreMarketQuote = preMarketQuotePoitingToThisDayClose >= thisDayClose;
			if (isQuikPreMarketQuote) {
				string msg = "skipping pre-market quote"
					+ " quote.ServerTime[" + quote.ServerTime + "].AddSeconds(1) >= thisDayClose[" + thisDayClose + "]"
					+ " quote=[" + quote + "]";
				Assembler.PopupException(msg);
				return;
			}
			//if (quote.PriceLastDeal == 0) {
			//    string msg = "skipping pre-market quote since CHARTS will screw up painting price=0;"
			//        + " quote=[" + quote + "]";
			//    Assembler.PopupException(msg);
			//    Assembler.PopupException(new Exception(msg));
			//    return;
			//}
			if (string.IsNullOrEmpty(quote.Source)) quote.Source = "Quik";
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			this.StreamingDataSnapshotQuik.StoreFortsSpecifics(quikQuote);
			base.PushQuoteReceived(quote);
		}
		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}
		public override string ToString() {
			return "StreamingMock: SymbolsSubscribed[" + this.SymbolsUpstreamSubscribedAsString + "]"
				+ "/SymbolsGenerating[" + this.GenerateOnlySymbolsAsString + "]"
				+ " DDE[" + this.DdeChannelsEstablished + "]";
		}
	}
}