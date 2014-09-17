using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Streaming {
	[DataContract]
	public class StreamingDataSnapshot {
		private StreamingProvider streamingProvider;
		[DataMember] protected Dictionary<string, double> BestBid { get; private set; }
		[DataMember] protected Dictionary<string, double> BestAsk { get; private set; }
		[DataMember] protected Dictionary<string, Quote> LastQuotesReceived { get; private set; }

		protected Object LockLastQuote = new Object();
		protected Object LockBestBid = new Object();
		protected Object LockBestAsk = new Object();

		public string SymbolsSubscribedAndReceiving { get {
				string ret = "";
				foreach (string symbol in LastQuotesReceived.Keys) {
					if (ret.Length > 0) ret += ",";
					ret += symbol + ":" + ((LastQuotesReceived[symbol] == null) ? "NULL" : LastQuotesReceived[symbol].Absno.ToString());
				}
				return ret;
			} }
		public StreamingDataSnapshot(StreamingProvider streamingProvider) {
			this.streamingProvider = streamingProvider;
			this.LastQuotesReceived = new Dictionary<string, Quote>();
			this.BestBid = new Dictionary<string, double>();
			this.BestAsk = new Dictionary<string, double>();
		}

		public void InitializeLastQuoteReceived(List<string> symbols) {
			foreach (string symbol in symbols) {
				if (this.LastQuotesReceived.ContainsKey(symbol)) continue;
				this.LastQuotesReceived.Add(symbol, null);
			}
		}
		public void LastQuotePutNull(string symbol) {
			lock (LockLastQuote) {
				if (this.LastQuotesReceived.ContainsKey(symbol)) {
					this.LastQuotesReceived[symbol] = null;
				} else {
					this.LastQuotesReceived.Add(symbol, null);
				}
			}
		}
		protected void LastQuoteUpdate(Quote quote) {
			Quote last = this.LastQuotesReceived[quote.Symbol];
			if (last == null) {
				this.LastQuotesReceived[quote.Symbol] = quote;
				return;
			}
			if (last == quote) {
				string msg = "How come you update twice to the same quote?";
				Debugger.Break();
				return;
			}
			if (last.Absno > quote.Absno) {
				string msg = "DONT_FEED_ME_WITH_OLD_QUOTES";
				Debugger.Break();
				return;
			}
			this.LastQuotesReceived[quote.Symbol] = quote;
		}
		public Quote LastQuoteGetForSymbol(string Symbol) {
			lock (this.LockLastQuote) {
				if (this.LastQuotesReceived.ContainsKey(Symbol) == false) return null;
				return this.LastQuotesReceived[Symbol];
			}
		}
		public double LastQuoteGetPriceForMarketOrder(string Symbol) {
			Quote lastQuote = LastQuoteGetForSymbol(Symbol);
			if (lastQuote == null) return 0;
			return lastQuote.PriceLastDeal;
		}

		public virtual void UpdateLastBidAskSnapFromQuote(Quote quote) {
			this.LastQuoteUpdate(quote);

			if (double.IsNaN(quote.Bid) || double.IsNaN(quote.Ask)) {
				if (false) throw new Exception("You seem to process Bars.LastBar with Partials=NaN");
				return;
			}
			if (quote.Bid != 0 && quote.Ask != 0) {
				this.BestBidAskPutForSymbol(quote.Symbol, quote.Bid, quote.Ask);
			}
		}
		public void BestBidAskPutForSymbol(string Symbol, double bid, double ask) {
			lock (this.BestBid) {
				this.BestBid[Symbol] = bid;
			}
			lock (this.BestAsk) {
				this.BestAsk[Symbol] = ask;
			}
		}
		public double BidOrAskFor(string Symbol, PositionLongShort direction) {
			if (direction == PositionLongShort.Unknown) {
				string msg = "BidOrAskFor(" + Symbol + ", " + direction + "): Bid and Ask are wrong to return for [" + direction + "]";
				throw new Exception(msg);
			}
			double price = (direction == PositionLongShort.Long)
				? this.BestBidGetForMarketOrder(Symbol) : this.BestAskGetForMarketOrder(Symbol);
			return price;
		}
		public double BestBidGetForMarketOrder(string Symbol) {
			double ret = 0;
			lock (this.BestBid) {
				if (this.BestBid.ContainsKey(Symbol)) {
					ret = this.BestBid[Symbol];
				}
			}
			return ret;
		}
		public double BestAskGetForMarketOrder(string Symbol) {
			double ret = 0;
			lock (this.BestAsk) {
				if (this.BestAsk.ContainsKey(Symbol)) {
					ret = this.BestAsk[Symbol];
				}
			}
			return ret;
		}

		public virtual double GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(string symbol, Direction direction
				, out OrderSpreadSide oss, bool forceCrossMarket) {
			double priceLastQuote = this.LastQuoteGetPriceForMarketOrder(symbol);
			if (priceLastQuote == 0) {
				string msg = "QuickCheck ZERO priceLastQuote=" + priceLastQuote + " for Symbol=[" + symbol + "]"
					+ " from streamingProvider[" + this.streamingProvider.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}
			double currentBid = this.BestBidGetForMarketOrder(symbol);
			double currentAsk = this.BestAskGetForMarketOrder(symbol);
			if (currentBid == 0) {
				string msg = "ZERO currentBid=" + currentBid + " for Symbol=[" + symbol + "]"
					+ " while priceLastQuote=[" + priceLastQuote + "]"
					+ " from streamingProvider[" + this.streamingProvider.Name + "].StreamingDataSnapshot";
				;
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}
			if (currentAsk == 0) {
				string msg = "ZERO currentAsk=" + currentAsk + " for Symbol=[" + symbol + "]"
					+ " while priceLastQuote=[" + priceLastQuote + "]"
					+ " from streamingProvider[" + this.streamingProvider.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}

			double price = 0;
			oss = OrderSpreadSide.ERROR;

			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositoryCustomSymbolInfo.FindSymbolInfo(symbol);
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
				Debugger.Break();
			}
			symbolInfo = Assembler.InstanceInitialized.RepositoryCustomSymbolInfo.FindSymbolInfoOrNew(symbol);
			price = symbolInfo.AlignOrderPriceToPriceLevel(price, direction, MarketLimitStop.Market);
			return price;
		}
	}
}
