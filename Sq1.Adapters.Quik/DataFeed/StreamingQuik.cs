using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Adapters.Quik.Dde;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik {
	public class StreamingQuik : StreamingProvider {
		[JsonProperty]	public string DdeServerPrefix { get; internal set; }
		[JsonProperty]	public string DdeTopicQuotes { get; internal set; }
		[JsonProperty]	public string DdeTopicTrades { get; internal set; }
		[JsonProperty]	public string DdeTopicPrefixDom { get; internal set; }
		[JsonIgnore]	DdeChannels DdeChannels;
		[JsonIgnore]	public string DdeChannelsEstablished { get {
				string ret = "DDE_CHANNELS_NULL__STREAMING_QUIK_NOT_YET_INITIALIZED";
				if (this.DdeChannels == null) return ret;
				lock (base.SymbolsSubscribedLock) {
					ret = this.DdeChannels.ToString();
				}
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
		[JsonProperty]	public override List<string> SymbolsUpstreamSubscribed { get {
				List<string> ret = new List<string>();
				if (this.DdeChannels == null) {
					string msg = "NO_SYMBOLS__STREAMING_QUIK_NOT_INITIALIZED_DDE_CHANNELS_NULL";
					return ret;
				}
				ret = this.DdeChannels.SymbolsHavingIndividualChannels;
				return ret;
			} }
		
		public StreamingQuik() : base() {
			base.Name = "Quik StreamingDummy";
			base.Icon = (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingProvider;
			base.PreferredStaticProviderName = "QuikStaticProvider";
			this.DdeServerPrefix = "SQ1";
			this.DdeTopicQuotes = "quotes";
			this.DdeTopicTrades = "trades";
			this.DdeTopicPrefixDom = "dom";
			base.StreamingDataSnapshot = new StreamingDataSnapshotQuik(this);
			StreamingDataSnapshotQuik throwAtEarlyStage = this.StreamingDataSnapshotQuik;
		}
		public override void Initialize(DataSource dataSource) {
			base.Name = "Quik Streaming";
			base.Initialize(dataSource);
			this.DdeChannels = new DdeChannels(this);
			this.Connect();
		}
		public override void Connect() {
			if (base.IsConnected == true) return;
			string symbolsSubscribed = subscribeAllSymbols();
			this.DdeChannels.StartDdeServer();
			base.ConnectionState = ConnectionState.Connected;
			base.UpdateConnectionStatus(0, "Started symbolsSubscribed[" + symbolsSubscribed + "]");
			Assembler.PopupException("QUIK started DdeChannels[" + this.DdeChannels.ToString() + "]");
			base.IsConnected = true;
		}
		public override void Disconnect() {
			if (base.IsConnected == false) return;
			Assembler.PopupException("QUIK stopping DdeChannels[" + this.DdeChannels.ToString() + "]");
			string symbolsUnsubscribed = unsubscribeAllSymbols();
			base.ConnectionState = ConnectionState.Disconnected;
			base.UpdateConnectionStatus(0, "Stopped symbolsUnsubscribed[" + symbolsUnsubscribed + "]");
			this.DdeChannels.StopDdeServer();
			Assembler.PopupException("QUIK stopped DdeChannels[" + this.DdeChannels.ToString() + "]");
			base.IsConnected = false;
		}

		private string subscribeAllSymbols() {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					this.UpstreamSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}
		private string unsubscribeAllSymbols() {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					this.UpstreamUnSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}

		public override void UpstreamSubscribe(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't subscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			lock (base.SymbolsSubscribedLock) {
				if (this.DdeChannels.SymbolHasIndividualChannels(symbol)) {
					String msg = "QUIK: ALREADY SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeChannels.IndividualChannelsForSymbol(symbol) + "]";
					Assembler.PopupException(msg);
					//this.StatusReporter.UpdateConnectionStatus(ConnectionState.OK, 0, msg);
					return;
				}
				this.DdeChannels.AddIndividualSymbolChannels(symbol);
			}
		}
		public override void UpstreamUnSubscribe(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't unsubscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			lock (base.SymbolsSubscribedLock) {
				if (this.DdeChannels.SymbolHasIndividualChannels(symbol) == false) {
					string errormsg = "QUIK: NOTHING TO REMOVE SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeChannels.IndividualChannelsForSymbol(symbol) + "]";
					Assembler.PopupException(errormsg);
					return;
				}
				this.DdeChannels.RemoveIndividualSymbolChannels(symbol);
			}
		}
		public override bool UpstreamIsSubscribed(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("IsSubscribed() symbol=[" + symbol + "]=IsNullOrEmpty; returning");
				return false;
			}
			lock (base.SymbolsSubscribedLock) {
				return this.DdeChannels.SymbolHasIndividualChannels(symbol);
			}
		}

		public void FilterAndDistributeDdeQuote(QuoteQuik quoteQuik) {
			DateTime thisDayClose = this.DataSource.MarketInfo.getThisDayClose(quoteQuik);
			DateTime preMarketQuotePoitingToThisDayClose = quoteQuik.ServerTime.AddSeconds(1);
			bool isQuikPreMarketQuote = preMarketQuotePoitingToThisDayClose >= thisDayClose;
			if (isQuikPreMarketQuote) {
				string msg = "skipping pre-market quote"
					+ " quote.ServerTime[" + quoteQuik.ServerTime + "].AddSeconds(1) >= thisDayClose[" + thisDayClose + "]"
					+ " quote=[" + quoteQuik + "]";
				Assembler.PopupException(msg);
				return;
			}
//			if (quote.PriceLastDeal == 0) {
//				string msg = "skipping pre-market quote since CHARTS will screw up painting price=0;"
//					+ " quote=[" + quote + "]";
//				Assembler.PopupException(msg);
//				Assembler.PopupException(new Exception(msg));
//				return;
//			}
			if (string.IsNullOrEmpty(quoteQuik.Source)) quoteQuik.Source = "Quik";
			this.StreamingDataSnapshotQuik.StoreFortsSpecifics_NOT_USED(quoteQuik);
			base.PushQuoteReceived(quoteQuik);
		}
		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			if (quote is QuoteQuik == false) {
				string msg = "Should be of a type Sq1.Adapters.Quik.QuoteQuik instead of Sq1.Core.DataTypes.Quote: "
					+ quote;
				throw new Exception(msg);
			}
			QuoteQuik quikQuote = quote as QuoteQuik;
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new StreamingQuikEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
		
		
		//IQuikDataReceiver
		public void TradeDeliveredDdeCallback(string skey, DdeTrade trade) { }
		public override string ToString() {
			return Name + "/[" + this.ConnectionState + "]: Symbols[" + base.SymbolsUpstreamSubscribedAsString + "]"
				+ " DDE[" + this.DdeChannelsEstablished + "]";
		}
	}
}