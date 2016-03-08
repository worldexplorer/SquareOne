using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Core.Streaming {
	public class StreamingDataSnapshot {
		[JsonIgnore]	StreamingAdapter							streamingAdapter;
		[JsonProperty]	ConcurrentDictionary<string, LevelTwo>		level2_lastQuoteUnbound_bySymbol;	// { get; private set; }
				public	long										Level2RefreshRate;
		//[JsonProperty]	public string								SymbolsSubscribedAndReceiving		{ get {
		//        string ret = "";
		//        foreach (string symbol in this.level2_lastQuoteUnbound_bySymbol.SafeCopy(this, "SymbolsSubscribedAndReceiving").Keys) {
		//            if (ret.Length > 0) ret += ",";
		//            ret += symbol;
		//            Quote lastClone = this.LastQuote_getForSymbol(symbol);
		//            ret += ":";
		//            if (lastClone == null) {
		//                ret += "NULL";
		//            } else {
		//                ret += lastClone.AbsnoPerSymbol.ToString();
		//            }
		//        }
		//        return ret;
		//    } }

		[JsonProperty]	public string								SymbolsSubscribedAndReceiving		{ get; private set; }

		StreamingDataSnapshot() {
			level2_lastQuoteUnbound_bySymbol = new ConcurrentDictionary<string, LevelTwo>("level2_lastQuoteUnbound_bySymbol");
			SymbolsSubscribedAndReceiving	 = "";
		}

		public StreamingDataSnapshot(StreamingAdapter streamingAdapter) : this() {
			if (streamingAdapter == null) {
				string msg = "DESERIALIZATION_ANOMALY DONT_FORGET_TO_INVOKE_InitializeWithStreaming()_AFTER_ALL_DATASOURCES_DESERIALIZED";
				//Assembler.PopupException(msg);
			}
			this.streamingAdapter = streamingAdapter;
		}

		public void Initialize_levelTwo_forAllSymbolsInDataSource(List<string> symbols) {
			foreach (string symbol in symbols) {
				this.Initialize_levelTwo_forSymbol(symbol);
			}
		}
		public void Initialize_levelTwo_forSymbol(string symbol) {
			string msig = " //StreamingDataSnapshot.InitializeLastQuoteAndLevelTwoForSymbol(" + symbol + ")";
			try {
				this.level2_lastQuoteUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastQuoteUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) {
					this.level2_lastQuoteUnbound_bySymbol.Add(symbol, new LevelTwo(symbol), this, msig);
				}
				LevelTwo level2 = this.level2_lastQuoteUnbound_bySymbol.GetAtKey(symbol, this, msig);
				Quote quoteBeforeNullification_WHY = level2.Clear();
				this.SymbolsSubscribedAndReceiving += "," + symbol;
			} finally {
				this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, msig);
			}
		}
		public void LastQuote_setForSymbol(Quote quote) {
			string msig = " //StreamingDataSnapshot.LastQuote_setForSymbol(" + quote.ToString() + ")";

			if (quote == null) {
				string msg = "USE_LastQuoteInitialize_INSTEAD_OF_PASSING_NULL_TO_LastQuoteCloneSetForSymbol";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.level2_lastQuoteUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastQuoteUnbound_bySymbol.ContainsKey(quote.Symbol, this, msig) == false) {
					//this.level2_lastQuoteUnbound_bySymbol.Add(quote.Symbol, new LevelTwo(quote.Symbol), this, msig);
					this.Initialize_levelTwo_forSymbol(quote.Symbol);
					string msg = "SUBSCRIBER_SHOULD_HAVE_INVOKED_Initialize_levelTwo_forAllSymbolsInDataSource()__FOLLOW_THIS_LIFECYCLE__ITS_A_RELIGION_NOT_OPEN_FOR_DISCUSSION";
					Assembler.PopupException(msg + msig, null, false);
				}

				LevelTwo level2 = this.level2_lastQuoteUnbound_bySymbol.GetAtKey(quote.Symbol, this, msig);
				Quote lastQuote = level2.LastQuote_unbound_notCloned;
				if (lastQuote == null) {
					string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#2 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
					//Assembler.PopupException(msg, null, false);
					level2.LastQuote_unbound_notCloned = quote;
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
				level2.LastQuote_unbound_notCloned = quote;
			} finally {
				this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, msig);
			}
		}
		string lockReason_getLastQuoteForSymbol;
		public Quote LastQuote_getForSymbol(string symbol) { //HERE_WAS_THE_DEADLOCK lock (this.lockLastQuote) {
			string msig = " //StreamingDataSnapshot.LastQuote_getForSymbol(" + symbol + ")";
			try {
				this.level2_lastQuoteUnbound_bySymbol.WaitAndLockFor(this, msig);
				//this.lockReason_getLastQuoteForSymbol = msig;
                if (this.level2_lastQuoteUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastQuoteUnbound_bySymbol.GetAtKey(symbol, this, msig);
				if (level2 == null) return null;
				Quote weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy = level2.LastQuote_unbound_notCloned;
				if (weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy == null) {
					string msg = "MUST_NOT_BE_NULL_FOR_LIVESIM_TOO levelTwoAndLastQuote.LastQuote";
				}
				return weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy;
			} finally {
				this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, msig);
				//this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, this.lockReason_getLastQuoteForSymbol);
			}
		}
		public LevelTwoHalf LevelTwoAsks_getForSymbol_nullUnsafe(string symbol) {
			string msig = " //StreamingDataSnapshot.LevelTwoAsks_getForSymbol(" + symbol + ")";
			try {
				this.level2_lastQuoteUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastQuoteUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastQuoteUnbound_bySymbol.GetAtKey(symbol, this, msig);
				return level2.Asks;
			} finally {
				this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, msig);
			}
		}
		public LevelTwoHalf LevelTwoBids_getForSymbol_nullUnsafe(string symbol) {
			string msig = " //StreamingDataSnapshot.LevelTwoBids_getForSymbol(" + symbol + ")";
			try {
				this.level2_lastQuoteUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastQuoteUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastQuoteUnbound_bySymbol.GetAtKey(symbol, this, msig);
				return level2.Bids;
			} finally {
				this.level2_lastQuoteUnbound_bySymbol.UnLockFor(this, msig);
			}
		}

		public double LastQuote_getPriceForMarketOrder(string symbol) {
			Quote lastQuote = this.LastQuote_getForSymbol(symbol);
			if (lastQuote == null) return 0;
			if (lastQuote.TradedAt == BidOrAsk.UNKNOWN) {
				string msg = "NEVER_HAPPENED_SO_FAR LAST_QUOTE_MUST_BE_BID_OR_ASK lastQuote.TradeOccuredAt[" + lastQuote.TradedAt + "]=BidOrAsk.UNKNOWN";
				Assembler.PopupException(msg);
				return 0;
			}
			return lastQuote.TradedPrice;
		}

		public double BestBid_getForMarketOrder(string symbol) {
			double ret = -1;
			Quote lastQuote = this.LastQuote_getForSymbol(symbol);
			if (lastQuote == null) {
				string msg = "LAST_TIME_I_HAD_IT_WHEN_Livesimulator_STORED_QUOTES_IN_QuikLivesimStreaming_WHILE_MarketLive_ASKED_QuikStreaming_TO_FILL_ALERT";
				Assembler.PopupException(msg);
				return ret;
			}
			ret = lastQuote.Bid;
			return ret;
		} 
		public double BestAsk_getForMarketOrder(string symbol) {
			double ret = -1;
			Quote lastQuote = this.LastQuote_getForSymbol(symbol);
			if (lastQuote == null) {
				string msg = "LAST_TIME_I_HAD_IT_WHEN_Livesimulator_STORED_QUOTES_IN_QuikLivesimStreaming_WHILE_MarketLive_ASKED_QuikStreaming_TO_FILL_ALERT";
				Assembler.PopupException(msg);
				return ret;
			}
			ret = lastQuote.Ask;
			return ret;
		}

		public double BidOrAsk_forDirection(string Symbol, PositionLongShort direction) {
			if (direction == PositionLongShort.Unknown) {
				string msg = "BidOrAskFor(" + Symbol + ", " + direction + "): Bid and Ask are wrong to return for [" + direction + "]";
				throw new Exception(msg);
			}
			double price = (direction == PositionLongShort.Long)
				? this.BestBid_getForMarketOrder(Symbol) : this.BestAsk_getForMarketOrder(Symbol);
			return price;
		}
		public virtual double BidOrAsk_getAligned_forTidalOrCrossMarket_fromStreamingSnap(string symbol, Direction direction
				, out OrderSpreadSide oss, bool forceCrossMarket) {
			string msig = " //GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(" + symbol + ", " + direction + ")";
			double priceLastQuote = this.LastQuote_getPriceForMarketOrder(symbol);
			if (priceLastQuote == 0) {
				string msg = "QuickCheck ZERO priceLastQuote=" + priceLastQuote + " for Symbol=[" + symbol + "]"
					+ " from streamingAdapter[" + this.streamingAdapter.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
			}
			double currentBid = this.BestBid_getForMarketOrder(symbol);
			double currentAsk = this.BestAsk_getForMarketOrder(symbol);
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

			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(symbol);
			MarketOrderAs spreadSide;
			if (forceCrossMarket) {
				spreadSide = MarketOrderAs.LimitCrossMarket;
			} else {
				spreadSide = (symbolInfo == null) ? MarketOrderAs.LimitCrossMarket : symbolInfo.MarketOrderAs;
			}
			if (spreadSide == MarketOrderAs.ERROR || spreadSide == MarketOrderAs.Unknown) {
				string msg = "CHANGE SymbolInfo[" + symbol + "].LimitCrossMarket; should not be spreadSide[" + spreadSide + "]";
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
							throw new Exception(msg2 + msig);
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
							throw new Exception(msg2 + msig);
					}
					break;
				default:
					string msg = "no handler for direction[" + direction + "]";
					throw new Exception(msg + msig);
			}

			if (double.IsNaN(price)) {
				string msg = "NEVER_HAPPENED_SO_FAR PRICE_MUST_BE_POSITIVE_NOT_NAN";
				Debugger.Break();
			}
			symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(symbol);
			//v2
			price = symbolInfo.Alert_alignToPriceLevel_simplified(price, direction, MarketLimitStop.Market);

			//v1
			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_DECEMBER_15TH_2014
			double price1 = symbolInfo.Order_alignToPriceLevel(price, direction, MarketLimitStop.Market);
			if (price1 != price) {
				string msg3 = "FIX_DEFINITELY_DIFFERENT_POSTPONE_TILL_ORDER_EXECUTOR_BACK_FOR_QUIK_BROKER";
				Assembler.PopupException(msg3 + msig, null);
			}
			#endif
			
			return price;
		}
		public override string ToString() {
			return "StreamingDataSnapshot_FOR_" + this.streamingAdapter;
		}

	}
}
