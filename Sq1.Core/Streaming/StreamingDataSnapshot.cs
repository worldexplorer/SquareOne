using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Streaming {
	public class StreamingDataSnapshot {
		[JsonIgnore]	StreamingAdapter			streamingAdapter;
		[JsonIgnore]	object						lockLastQuote;
		[JsonProperty]	Dictionary<string, Quote>	lastQuoteClonesReceivedUnboundBySymbol;	// { get; private set; }
		[JsonProperty]	public string				SymbolsSubscribedAndReceiving		{ get {
				string ret = "";
				foreach (string symbol in lastQuoteClonesReceivedUnboundBySymbol.Keys) {
					if (ret.Length > 0) ret += ",";
					ret += symbol + ":" + ((lastQuoteClonesReceivedUnboundBySymbol[symbol] == null) ? "NULL" : lastQuoteClonesReceivedUnboundBySymbol[symbol].AbsnoPerSymbol.ToString());
				}
				return ret;
			} }

		private StreamingDataSnapshot() {
			lastQuoteClonesReceivedUnboundBySymbol = new Dictionary<string, Quote>();
			lockLastQuote = new object();
		}

		public StreamingDataSnapshot(StreamingAdapter streamingAdapter) : this() {
			this.streamingAdapter = streamingAdapter;
		}

		public void InitializeLastQuoteReceived(List<string> symbols) {
			foreach (string symbol in symbols) {
				//v1
				//if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(symbol)) continue;
				//this.lastQuoteClonesReceivedUnboundBySymbol.Add(symbol, null);
				//v2
				this.LastQuoteInitialize(symbol);
			}
		}
		public void LastQuoteInitialize(string symbol) {
			lock (lockLastQuote) {
				if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(symbol)) {
					Quote prevQuote = this.lastQuoteClonesReceivedUnboundBySymbol[symbol];
					this.lastQuoteClonesReceivedUnboundBySymbol[symbol] = null;
				} else {
					this.lastQuoteClonesReceivedUnboundBySymbol.Add(symbol, null);
				}
			}
		}
		public void LastQuoteCloneSetForSymbol(Quote quote) {
			string msig = " StreamingDataSnapshot.LastQuoteSetForSymbol(" + quote.ToString() + ")";

			if (quote == null) {
				string msg = "USE_LastQuoteInitialize_INSTEAD_OF_PASSING_NULL_TO_LastQuoteCloneSetForSymbol";
				Assembler.PopupException(msg + msig);
			}
			if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(quote.Symbol) == false) {
				this.lastQuoteClonesReceivedUnboundBySymbol.Add(quote.Symbol, null);
				string msg = "SUBSCRIBER_SHOULD_HAVE_INVOKED_LastQuoteInitialize()__FOLLOW_THIS_LIFECYCLE__ITS_A_RELIGION_NOT_OPEN_FOR_DISCUSSION";
				Assembler.PopupException(msg + msig);
			}

			Quote lastQuote = this.lastQuoteClonesReceivedUnboundBySymbol[quote.Symbol];
			if (lastQuote == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#2 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				//Assembler.PopupException(msg, null, false);
				this.lastQuoteClonesReceivedUnboundBySymbol[quote.Symbol] = quote;
				return;
			}
			if (lastQuote == quote) {
				string msg = "DONT_PUT_SAME_QUOTE_TWICE";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (lastQuote.AbsnoPerSymbol >= quote.AbsnoPerSymbol) {
				string msg = "DONT_FEED_ME_WITH_OLD_QUOTES (????QuoteQuik #-1/0 AUTOGEN)";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.lastQuoteClonesReceivedUnboundBySymbol[quote.Symbol] = quote;
		}
		public Quote LastQuoteCloneGetForSymbol(string Symbol) { lock (this.lockLastQuote) {
				if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(Symbol) == false) return null;
				Quote weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy = this.lastQuoteClonesReceivedUnboundBySymbol[Symbol];
				return weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy;
			} }
		public double LastQuoteGetPriceForMarketOrder(string Symbol) {
			Quote lastQuote = LastQuoteCloneGetForSymbol(Symbol);
			if (lastQuote == null) return 0;
			if (lastQuote.TradedAt == BidOrAsk.UNKNOWN) {
				string msg = "NEVER_HAPPENED_SO_FAR LAST_QUOTE_MUST_BE_BID_OR_ASK lastQuote.TradeOccuredAt[" + lastQuote.TradedAt + "]=BidOrAsk.UNKNOWN";
				Assembler.PopupException(msg);
				return 0;
			}
			return lastQuote.TradedPrice;
		}

		public double BestBidGetForMarketOrder(string Symbol) { lock (this.lockLastQuote) {
				double ret = -1;
				//lock (this.bestBidBySymbol) { if (this.bestBidBySymbol.ContainsKey(Symbol)) { ret = this.bestBidBySymbol[Symbol]; } }
				if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(Symbol)) {
					Quote lastQuote = this.lastQuoteClonesReceivedUnboundBySymbol[Symbol];
					ret = lastQuote.Bid;
				}
				return ret;
			} }
		public double BestAskGetForMarketOrder(string Symbol) { lock (this.lockLastQuote) {
				double ret = -1;
				//lock (this.bestAskBySymbol) { if (this.bestAskBySymbol.ContainsKey(Symbol)) { ret = this.bestAskBySymbol[Symbol]; } }
				if (this.lastQuoteClonesReceivedUnboundBySymbol.ContainsKey(Symbol)) {
					Quote lastQuote = this.lastQuoteClonesReceivedUnboundBySymbol[Symbol];
					ret = lastQuote.Ask;
				}
				return ret;
			} }

		public double BidOrAskFor(string Symbol, PositionLongShort direction) {
			if (direction == PositionLongShort.Unknown) {
				string msg = "BidOrAskFor(" + Symbol + ", " + direction + "): Bid and Ask are wrong to return for [" + direction + "]";
				throw new Exception(msg);
			}
			double price = (direction == PositionLongShort.Long)
				? this.BestBidGetForMarketOrder(Symbol) : this.BestAskGetForMarketOrder(Symbol);
			return price;
		}
		public virtual double GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(string symbol, Direction direction
				, out OrderSpreadSide oss, bool forceCrossMarket) {
			string msig = " GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(" + symbol + ", " + direction + ")";
			double priceLastQuote = this.LastQuoteGetPriceForMarketOrder(symbol);
			if (priceLastQuote == 0) {
				string msg = "QuickCheck ZERO priceLastQuote=" + priceLastQuote + " for Symbol=[" + symbol + "]"
					+ " from streamingAdapter[" + this.streamingAdapter.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}
			double currentBid = this.BestBidGetForMarketOrder(symbol);
			double currentAsk = this.BestAskGetForMarketOrder(symbol);
			if (currentBid == 0) {
				string msg = "ZERO currentBid=" + currentBid + " for Symbol=[" + symbol + "]"
					+ " while priceLastQuote=[" + priceLastQuote + "]"
					+ " from streamingAdapter[" + this.streamingAdapter.Name + "].StreamingDataSnapshot";
				;
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}
			if (currentAsk == 0) {
				string msg = "ZERO currentAsk=" + currentAsk + " for Symbol=[" + symbol + "]"
					+ " while priceLastQuote=[" + priceLastQuote + "]"
					+ " from streamingAdapter[" + this.streamingAdapter.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}

			double price = 0;
			oss = OrderSpreadSide.ERROR;

			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfo(symbol);
			MarketOrderAs spreadSide;
			if (forceCrossMarket) {
				spreadSide = MarketOrderAs.LimitCrossMarket;
			} else {
				spreadSide = (symbolInfo == null) ? MarketOrderAs.LimitCrossMarket : symbolInfo.MarketOrderAs;
			}
			if (spreadSide == MarketOrderAs.ERROR || spreadSide == MarketOrderAs.Unknown) {
				string msg = "Set Symbol[" + symbol + "].SymbolInfo.LimitCrossMarket; should not be spreadSide[" + spreadSide + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
				//return;
			}

			switch (direction) {
				case Direction.Buy:
				case Direction.Cover:
					switch (spreadSide) {
						case MarketOrderAs.LimitTidal:
							oss = OrderSpreadSide.AskTidal;
							price = currentAsk;
							break;
						case MarketOrderAs.LimitCrossMarket:
							oss = OrderSpreadSide.BidCrossed;
							price = currentBid;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketMinMaxSentToBroker:
							oss = OrderSpreadSide.MaxPrice;
							price = currentAsk;
							break;
						case MarketOrderAs.MarketZeroSentToBroker:
							oss = OrderSpreadSide.MarketPrice;
							price = currentAsk;		// looks like default, must be crossmarket to fill it right now
							break;
						default:
							string msg2 = "no handler for spreadSide[" + spreadSide + "] direction[" + direction + "]";
							throw new Exception(msg2);
					}
					break;
				case Direction.Short:
				case Direction.Sell:
					switch (spreadSide) {
						case MarketOrderAs.LimitTidal:
							oss = OrderSpreadSide.BidTidal;
							price = currentBid;
							break;
						case MarketOrderAs.LimitCrossMarket:
							oss = OrderSpreadSide.AskCrossed;
							price = currentAsk;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketMinMaxSentToBroker:
							oss = OrderSpreadSide.MinPrice;
							price = currentBid;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketZeroSentToBroker:
							oss = OrderSpreadSide.MarketPrice;
							price = currentBid;		// looks like default, must be crossmarket to fill it right now
							break;
						default:
							string msg2 = "no handler for spreadSide[" + spreadSide + "] direction[" + direction + "]";
							throw new Exception(msg2);
					}
					break;
				default:
					string msg = "no handler for direction[" + direction + "]";
					throw new Exception(msg);
			}

			if (double.IsNaN(price)) {
				string msg = "NEVER_HAPPENED_SO_FAR PRICE_MUST_BE_POSITIVE_NOT_NAN";
				Debugger.Break();
			}
			symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(symbol);
			//v2
			price = symbolInfo.AlignAlertToPriceLevelSimplified(price, direction, MarketLimitStop.Market);

			//v1
			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_DECEMBER_15TH_2014
			double price1 = symbolInfo.AlignOrderToPriceLevel(price, direction, MarketLimitStop.Market);
			if (price1 != price) {
				string msg3 = "FIX_DEFINITELY_DIFFERENT_POSTPONE_TILL_ORDER_EXECUTOR_BACK_FOR_QUIK_BROKER";
				Assembler.PopupException(msg3 + msig, null);
			}
			#endif
			
			return price;
		}
	}
}
