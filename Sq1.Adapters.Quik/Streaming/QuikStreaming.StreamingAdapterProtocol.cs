using System;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreaming : StreamingAdapter {

		public override void UpstreamConnect() { lock (base.SymbolsSubscribedLock) {
			if (base.UpstreamConnected == true) return;
			// MOVED_TO_CTOR_TO_AVOID_NPE_IN_MONITOR this.DdeBatchSubscriber.Tables_CommonForAllSymbols_Add();
			string symbolsSubscribed = this.upstreamSubscribeAllDataSourceSymbols();
			this.DdeServerRegister();	// ConnectionState.UpstreamConnected_downstreamUnsubscribed;		// will result in StreamingConnected=true
			this.DdeBatchSubscriber.AllDdeMessagesReceivedCounter_reset();
			this.UpstreamConnectionState = ConnectionState.UpstreamConnected_downstreamSubscribedAll;
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " started DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]");
		} }
		public override void UpstreamDisconnect() { lock (base.SymbolsSubscribedLock) {
			if (base.UpstreamConnected == false) return;
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == false) {
				Assembler.PopupException("QUIK stopping DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]", null, false);
			}
			string symbolsUnsubscribed = this.upstreamUnsubscribeAllDataSourceSymbols();
			this.UpstreamConnectionState = ConnectionState.UpstreamConnected_downstreamUnsubscribedAll;
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " symbolsUnsubscribedAll[" + symbolsUnsubscribed + "]");
			this.DdeServerUnregister();
			// MOVED_TO_CTOR_TO_AVOID_NPE_IN_MONITOR this.DdeBatchSubscriber.Tables_CommonForAllSymbols_Add();
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " stopped DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]");
		} }

		public override void UpstreamSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't subscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			if (this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol)) {
				String msg = "QUIK: ALREADY SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeBatchSubscriber.Level2ForSymbol(symbol) + "]";
				Assembler.PopupException(msg);
				//this.StatusReporter.UpdateConnectionStatus(ConnectionState.OK, 0, msg);
				return;
			}
			// NO_SERVER_ISNOT_STARTED_HERE_YET NB adding another DdeConversation into the registered DDE server - is NDDE capable of registering receiving topics on-the-fly?
			this.DdeBatchSubscriber.TableIndividual_DepthOfMarket_ForSymbolAdd(symbol);
			this.UpstreamConnectionState = this.UpstreamConnected
				?    ConnectionState.UpstreamConnected_downstreamSubscribed
				: ConnectionState.UpstreamDisconnected_downstreamSubscribed;
		} }
		public override void UpstreamUnSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't unsubscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			if (this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol) == false) {
				string errormsg = "QUIK: NOTHING TO REMOVE SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeBatchSubscriber.Level2ForSymbol(symbol) + "]";
				Assembler.PopupException(errormsg);
				return;
			}
			this.DdeBatchSubscriber.TableIndividual_DepthOfMarket_ForSymbolRemove(symbol);
			this.UpstreamConnectionState = this.UpstreamConnected
				?    ConnectionState.UpstreamConnected_downstreamUnsubscribed
				: ConnectionState.UpstreamDisconnected_downstreamUnsubscribed;
		} }
		public override bool UpstreamIsSubscribed(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (String.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("IsSubscribed() symbol=[" + symbol + "]=IsNullOrEmpty; returning");
				return false;
			}
			return this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol);
		} }

		public override void PushQuoteReceived(Quote quote) {
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
			QuoteQuik quoteQuik = QuoteQuik.SafeUpcast(quote);
			this.StreamingDataSnapshotQuik.StoreFortsSpecifics(quoteQuik);
			this.syncSymbolClass_toSymbolInfo(quoteQuik);
			base.PushQuoteReceived(quote);
		}

		void syncSymbolClass_toSymbolInfo(QuoteQuik quoteQuik) {
			if (string.IsNullOrEmpty(quoteQuik.Symbol		) == false) return;
			if (string.IsNullOrEmpty(quoteQuik.SymbolClass	) == false) return;
			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(quoteQuik.Symbol);
			if (symbolInfo == null) return;
			if (symbolInfo.SymbolClass == quoteQuik.SymbolClass) return;
			symbolInfo.SymbolClass = quoteQuik.SymbolClass;
			Assembler.InstanceInitialized.RepositorySymbolInfos.Serialize();
		}

		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.StreamingEditorInstance = new QuikStreamingEditor(this, dataSourceEditor);
			return base.StreamingEditorInstance;
		}

	}
}