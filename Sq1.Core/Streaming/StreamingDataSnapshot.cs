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
		[JsonProperty]	ConcurrentDictionary<string, LevelTwo>		level2_lastPrevQuotesUnbound_bySymbol;
		[JsonProperty]	public string								SymbolsSubscribedAndReceiving		{ get; private set; }
		//[JsonProperty]	public string								SymbolsSubscribedAndReceiving		{ get {
		//    List<string> symbols = this.level2_lastPrevQuotesUnbound_bySymbol.Keys("STACK_OVERFLOW_OTHERWIZE", "SymbolsSubscribedAndReceiving");
		//    string ret = string.Join(",", symbols);
		//    return ret;
		//} }

		StreamingDataSnapshot() {
			level2_lastPrevQuotesUnbound_bySymbol = new ConcurrentDictionary<string, LevelTwo>("quoteCurrentUnbound_andLevel2_bySymbol");
			SymbolsSubscribedAndReceiving = "";
		}

		public StreamingDataSnapshot(StreamingAdapter streamingAdapter) : this() {
			if (streamingAdapter == null) {
				string msg = "DESERIALIZATION_ANOMALY DONT_FORGET_TO_INVOKE_InitializeWithStreaming()_AFTER_ALL_DATASOURCES_DESERIALIZED";
				//Assembler.PopupException(msg);
			}
			this.streamingAdapter = streamingAdapter;
		}

		public void Initialize_levelTwo_lastPrevQuotes_forAllSymbols_inDataSource(List<string> symbols) {
			foreach (string symbol in symbols) {
				this.Initialize_levelTwo_lastPrevQuotes_forSymbol(symbol);
			}
		}
		public void Initialize_levelTwo_lastPrevQuotes_forSymbol(string symbol) {
			string msig = " //StreamingDataSnapshot.Initialize_levelTwo_lastPrevQuotes_forSymbol(" + symbol + ")";
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) {
					string streamingAdapterInitialized_asString = this.streamingAdapter.ToString();
					this.level2_lastPrevQuotesUnbound_bySymbol.Add(symbol, new LevelTwo(symbol, streamingAdapterInitialized_asString), this, msig);
				}
				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(symbol, this, msig);
				Quote quoteBeforeNullification_WHY = level2.Clear_QuoteLastPrev();
				level2.Clear_LevelTwo(this, "livesimEnded");
				if (this.SymbolsSubscribedAndReceiving.Contains(symbol) == false) {
					if (this.SymbolsSubscribedAndReceiving != "") this.SymbolsSubscribedAndReceiving += ",";
					this.SymbolsSubscribedAndReceiving += symbol;
				}
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
			}
		}

		public void SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(Quote quote) {
			string msig = " //StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(" + quote.ToString() + ")";

			if (quote == null) {
				string msg = "USE_LastQuoteInitialize_INSTEAD_OF_PASSING_NULL_TO_LastQuoteCloneSetForSymbol";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(quote.Symbol, this, msig) == false) {
					//this.level2_quoteCurrentUnbound_bySymbol.Add(quote.Symbol, new LevelTwo(quote.Symbol), this, msig);
					this.Initialize_levelTwo_lastPrevQuotes_forSymbol(quote.Symbol);
					string msg = "SUBSCRIBER_SHOULD_HAVE_INVOKED_Initialize_levelTwo_forAllSymbolsInDataSource()__FOLLOW_THIS_LIFECYCLE__ITS_A_RELIGION_NOT_OPEN_FOR_DISCUSSION";
					Assembler.PopupException(msg + msig, null, false);
				}

				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(quote.Symbol, this, msig);
				Quote quoteCurrent = level2.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno;
				if (quoteCurrent == null) {
					string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#2 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
					Assembler.PopupException(msg + msig, null, false);
					level2.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno = quote;
					return;
				}
				if (quoteCurrent == quote) {
					string msg = "DONT_PUT_SAME_QUOTE_TWICE";
					Assembler.PopupException(msg + msig);
					return;
				}
				if (quoteCurrent.AbsnoPerSymbol >= quote.AbsnoPerSymbol) {
					string msg = "DONT_FEED_ME_WITH_OLD_QUOTES (????QuoteQuik #-1/0 AUTOGEN)";
					Assembler.PopupException(msg + msig);
					return;
				}
				level2.QuotePrev_unbound_notCloned = quoteCurrent;
				level2.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno = quote;
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
			}
		}
		public Quote GetQuoteCurrent_forSymbol_nullUnsafe(string symbol) { //HERE_WAS_THE_DEADLOCK lock (this.lockLastQuote) {
			string msig = " //StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(" + symbol + ")";
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				//this.lockReason_getLastQuoteForSymbol = msig;
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(symbol, this, msig);
				if (level2 == null) return null;
				Quote attached_toOriginalBars_insteadOfRegeneratedGrowingCopy_forBacktest = level2.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno;
				if (attached_toOriginalBars_insteadOfRegeneratedGrowingCopy_forBacktest == null) {
					string msg = "NULL_IS_OK_FOR_APP_RESTART MUST_NOT_BE_NULL_FOR_LIVESIM_TOO levelTwoAndLastQuote.QuoteLast_unbound_notCloned";
				//} else {
				//	string msg = "RENAME_ME QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno";
				}
				return attached_toOriginalBars_insteadOfRegeneratedGrowingCopy_forBacktest;
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
				//this.level2_quoteCurrentUnbound_bySymbol.UnLockFor(this, this.lockReason_getLastQuoteForSymbol);
			}
		}
		public Quote GetQuotePrev_forSymbol_nullUnsafe(string symbol) { //HERE_WAS_THE_DEADLOCK lock (this.lockLastQuote) {
			string msig = " //StreamingDataSnapshot.GetQuotePrev_forSymbol_nullUnsafe(" + symbol + ")";
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				//this.lockReason_getLastQuoteForSymbol = msig;
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(symbol, this, msig);
				if (level2 == null) return null;
				Quote weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy = level2.QuotePrev_unbound_notCloned;
				if (weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy == null) {
					string msg = "MUST_NOT_BE_NULL_FOR_LIVESIM_TOO levelTwoAndLastQuote.QuotePrev_unbound_notCloned";
				}
				return weirdAttachedToOriginalBarsInsteadOfRegeneratedGrowingCopy;
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
				//this.level2_quoteCurrentUnbound_bySymbol.UnLockFor(this, this.lockReason_getLastQuoteForSymbol);
			}
		}

		public LevelTwo GetLevelTwo_forSymbol_nullUnsafe(string symbol) {
			string msig = " //StreamingDataSnapshot.GetLevelTwo_forSymbol_nullUnsafe(" + symbol + ")";
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(symbol, this, msig);
				return level2;
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
			}
		}
		public LevelTwoFrozen GetLevelTwoFrozenSorted_forSymbol_nullUnsafe(string symbol, string whoFrozeMe, string recipient) {
			string msig = " //StreamingDataSnapshot.GetLevelTwoFrozenSorted_forSymbol_nullUnsafe(" + symbol + ")";
			try {
				this.level2_lastPrevQuotesUnbound_bySymbol.WaitAndLockFor(this, msig);
				if (this.level2_lastPrevQuotesUnbound_bySymbol.ContainsKey(symbol, this, msig) == false) return null;
				LevelTwo level2 = this.level2_lastPrevQuotesUnbound_bySymbol.GetAtKey(symbol, this, msig);
				LevelTwoFrozen levelTwoFrozen = new LevelTwoFrozen(level2, whoFrozeMe, recipient);
				return levelTwoFrozen;
			} finally {
				this.level2_lastPrevQuotesUnbound_bySymbol.UnLockFor(this, msig);
			}
		}

		public double GetPriceForMarketOrder_notAligned_fromQuoteCurrent(string symbol) {
			Quote quoteCurrent = this.GetQuoteCurrent_forSymbol_nullUnsafe(symbol);
			if (quoteCurrent == null) return 0;
			if (quoteCurrent.TradedAt == BidOrAsk.UNKNOWN) {
				string msg = "NEVER_HAPPENED_SO_FAR LAST_QUOTE_MUST_BE_BID_OR_ASK quoteCurrent.TradeOccuredAt[" + quoteCurrent.TradedAt + "]=BidOrAsk.UNKNOWN";
				Assembler.PopupException(msg, null, false);
				return 0;
			}
			return quoteCurrent.TradedPrice;
		}

		public double GetBestBid_notAligned_forMarketOrder_fromQuoteCurrent(string symbol) {
			double ret = -1;
			Quote quoteCurrent = this.GetQuoteCurrent_forSymbol_nullUnsafe(symbol);
			if (quoteCurrent == null) {
				string msg = "LAST_TIME_I_HAD_IT_WHEN_Livesimulator_STORED_QUOTES_IN_QuikLivesimStreaming_WHILE_MarketLive_ASKED_QuikStreaming_TO_FILL_ALERT";
				Assembler.PopupException(msg);
				return ret;
			}
			ret = quoteCurrent.Bid;
			return ret;
		} 
		public double GetBestAsk_notAligned_forMarketOrder_fromQuoteCurrent(string symbol) {
			double ret = -1;
			Quote quoteCurrent = this.GetQuoteCurrent_forSymbol_nullUnsafe(symbol);
			if (quoteCurrent == null) {
				string msg = "LAST_TIME_I_HAD_IT_WHEN_Livesimulator_STORED_QUOTES_IN_QuikLivesimStreaming_WHILE_MarketLive_ASKED_QuikStreaming_TO_FILL_ALERT";
				Assembler.PopupException(msg);
				return ret;
			}
			ret = quoteCurrent.Ask;
			return ret;
		}

		public double GetBidOrAsk_forDirection_fromQuoteCurrent(string Symbol, PositionLongShort direction) {
			if (direction == PositionLongShort.Unknown) {
				string msg = "BidOrAskFor(" + Symbol + ", " + direction + "): Bid and Ask are wrong to return for [" + direction + "]";
				throw new Exception(msg);
			}
			double price = (direction == PositionLongShort.Long)
				? this.GetBestBid_notAligned_forMarketOrder_fromQuoteCurrent(Symbol) : this.GetBestAsk_notAligned_forMarketOrder_fromQuoteCurrent(Symbol);
			return price;
		}
		public virtual double GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteCurrent(string symbol, Direction direction
				, out SpreadSide oss, bool forceCrossMarket) {
			string msig = " //GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteCurrent(" + symbol + ", " + direction + ")";
			double priceLastQuote = this.GetPriceForMarketOrder_notAligned_fromQuoteCurrent(symbol);
			if (priceLastQuote == 0) {
				string msg = "QuickCheck ZERO priceLastQuote=" + priceLastQuote + " for Symbol=[" + symbol + "]"
					+ " from streamingAdapter[" + this.streamingAdapter.Name + "].StreamingDataSnapshot";
				Assembler.PopupException(msg, null, false);
				//throw new Exception(msg);
			}
			double currentBid = this.GetBestBid_notAligned_forMarketOrder_fromQuoteCurrent(symbol);
			double currentAsk = this.GetBestAsk_notAligned_forMarketOrder_fromQuoteCurrent(symbol);
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
			oss = SpreadSide.ERROR;

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
							oss = SpreadSide.BidTidal;
							price = currentBid;
							break;
						case MarketOrderAs.LimitCrossMarket:
							oss = SpreadSide.AskCrossed;
							price = currentAsk;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketMinMaxSentToBroker:
							oss = SpreadSide.MaxPrice;
							price = currentAsk;
							break;
						case MarketOrderAs.MarketZeroSentToBroker:
							oss = SpreadSide.MarketPrice;
							price = currentAsk;		// looks like default, must be crossmarket to fill it right now
							break;
						case MarketOrderAs.MarketUnchanged_DANGEROUS:
							oss = SpreadSide.Unknown;
							//DONT_CHANGE_USELESS price = ???;
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
							oss = SpreadSide.AskTidal;
							price = currentAsk;
							break;
						case MarketOrderAs.LimitCrossMarket:
							oss = SpreadSide.BidCrossed;
							price = currentBid;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketMinMaxSentToBroker:
							oss = SpreadSide.MinPrice;
							price = currentBid;		// Unknown (Order default) becomes CrossMarket
							break;
						case MarketOrderAs.MarketZeroSentToBroker:
							oss = SpreadSide.MarketPrice;
							price = currentBid;		// looks like default, must be crossmarket to fill it right now
							break;
						case MarketOrderAs.MarketUnchanged_DANGEROUS:
							oss = SpreadSide.Unknown;
							//DONT_CHANGE_USELESS price = ???;
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
			price = symbolInfo.Alert_alignToPriceLevel(price, direction, MarketLimitStop.Market);
			return price;
		}

		public override string ToString() {
			return "StreamingDataSnapshot_FOR_" + this.streamingAdapter;
		}
	}
}
