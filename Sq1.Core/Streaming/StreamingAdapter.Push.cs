using System;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter {

		protected virtual int Quote_incrementAbsnoPerSymbol_fixServerTime_minusOneQuotePrevNewerThanCurrent_munisTwoWrongTimezone(Quote quoteUU) {
			int changesMade = 0;
			string msig = " //StreamingAdapter.Quote_fixServerTime_absnoPerSymbol("
#if VERBOSE_STRINGS_SLOW
				+ quoteUU
#endif
				+ ")" + this.ToString();

			Quote quoteLast	= this.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(quoteUU.Symbol);
			if (quoteLast == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#1 symbol[" + quoteUU.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				Assembler.PopupException(msg + msig, null, false);
				quoteUU.AbsnoPerSymbol = 0;
				changesMade++;
				return changesMade;
			}
			if (quoteUU == quoteLast) {
				string msg = "DONT_FEED_STREAMING_WITH_SAME_QUOTE__NOT_FIXING_ANYTHING";
				Assembler.PopupException(msg + msig, null, true);
				return changesMade;
			}

			//Quote quotePrev		= this.StreamingDataSnapshot.GetQuotePrev_forSymbol_nullUnsafe(quoteUU.Symbol);
			if (quoteUU.ServerTime == DateTime.MinValue) {		// spreadQuote arrived from DdeTableDepth without serverTime koz serverTime of Level2 changed is not transmitted over DDE
				if (this.Name.Contains("NOPE_TEST_REAL_QUIK_TOO___StreamingLivesim") == false) {	// QuikStreamingLivesim
					TimeSpan diff_inLocalTime = quoteUU.LocalTime.Subtract(quoteLast.LocalTime);
					DateTime serverTime_reconstructed_fromLastQuote = quoteLast.ServerTime.Add(diff_inLocalTime);
					quoteUU.ServerTime = serverTime_reconstructed_fromLastQuote;
					changesMade++;
				} else {
					if (quoteLast.ParentBarStreaming == null) {
						string msg = "WILL_BINDER_SET_QUOTE_TO_STREAMING_DATA_SNAPSHOT???";
						Assembler.PopupException(msg, null, false);
					} else {
						DateTime serverTime_fromLocalViaTimezone = this.DataSource.MarketInfo.ServerTimeNow;
						if (quoteUU.ServerTime != serverTime_fromLocalViaTimezone) {
							//quoteUU.ServerTime  = serverFromLocal;
							changesMade = -2;
							return changesMade;
						} else {
							string msg = "Nothing helps!!!";
							quoteUU.ServerTime = quoteUU.ServerTime.AddMilliseconds(1);
						}
						changesMade++;
					}
				}
			}

			//v1 if (quoteUU.ServerTime == quotePrev.ServerTime) {
			//v2
			string quoteMillis		=   quoteUU.ServerTime.ToString("HH:mm:ss.fff");
			string quoteLastMillis  = quoteLast.ServerTime.ToString("HH:mm:ss.fff");
			//if (quoteMillis == quotePrevMillis) {
			if (quoteUU.ServerTime <= quoteLast.ServerTime) {
				// increase granularity of QuikQuotes (they will have the same ServerTime within the same second, while must have increasing milliseconds; I can't force QUIK print fractions of seconds via DDE export)
				TimeSpan diff_inLocalTime = quoteUU.LocalTime.Subtract(quoteLast.LocalTime);
				// diff_localTime.Milliseconds will go to StreamingDataSnapshot with ServerTime fixed, and next diffMillis will be negative for the quote within same second
				int diffMillis_willBeNegative_forSecondQuote_duringSameSecond = diff_inLocalTime.Milliseconds;
				int diffMillis = Math.Abs(diffMillis_willBeNegative_forSecondQuote_duringSameSecond);
				if (diffMillis <= 0) {
					diffMillis = 1;
					//bool sameBidAsk = quoteUU.SameBidAsk(quoteLast);
					//bool sameSize = quoteUU.Size == quoteLast.Size;
					//if (sameSize && sameBidAsk) return -1;		// no I should not drop it; same deal for two different buyers
					string msg = quoteUU.Symbol + " ServerTime++"
						//+ " sameBidAsk[" + sameBidAsk + "] sameSize[" + sameSize + "]"
						;
					//Assembler.PopupException(msg, null, false);

					// DONT_DO_Convert_localTime_toServerTime_marketInfo_HAS_TZ=-3_WHILE_QUIK_JUNIOR_SENDS_TZ=-2_LAGS_ONE_MORE_HOUR
					//MarketInfo marketInfo = this.DataSource.MarketInfo;
					//DateTime serverFromLocal = marketInfo.ServerTimeNow;
					//if (quoteUU.ServerTime != serverFromLocal) {
					//    //quoteUU.ServerTime  = serverFromLocal;
					//    return -2;
					//} else {
					//	string msg = "Nothing helps!!!";
					//	quoteUU.ServerTime = quoteLast.ServerTime.AddMilliseconds(1);
					//}
					//changesMade++;
				}
				quoteUU.ServerTime = quoteLast.ServerTime.AddMilliseconds(diffMillis);
				changesMade++;
			}

			if (quoteUU.ServerTime <= quoteLast.ServerTime) {
				return -1;
			}
			
			if (quoteUU is QuoteGenerated) {
				string msg = "DONT_INVOKE_StreamingAdapter.PushQuoteReceived!!! BACKTESTER_HAS_ITS_OWN_PushQuoteGenerated()";
				Assembler.PopupException(msg + msig, null, false);
				return changesMade;
			}

			if (quoteUU.AbsnoPerSymbol != -1) {
				string msg = "YOUR_STREAMING_ADAPTER_ALREADY_HANDLED quoteUU.AbsnoPerSymbol[" + quoteUU.AbsnoPerSymbol + "]!=-1";
				Assembler.PopupException(msg + msig, null, false);
				return changesMade;
			}

			long absnoPerSymbolNext = -1;
			if (quoteLast.AbsnoPerSymbol == -1) {
				string msg = "LAST_QUOTE_DIDNT_HAVE_ABSNO_SET_BY_STREAMING_ADAPDER_ON_PREV_ITERATION FORCING_ZERO";
				Assembler.PopupException(msg + msig, null, false);
				absnoPerSymbolNext = 0;
			} else {
				absnoPerSymbolNext = quoteLast.AbsnoPerSymbol + 1;	// you must see lock(){} upstack
			}

			if (absnoPerSymbolNext == -1) {
				string msg = "I_REFUSE_TO_FIX quote.AbsnoPerSymbol[-1] && absnoPerSymbolNext[-1]";
				Assembler.PopupException(msg + msig, null, true);
				return changesMade;
			}

			//QUOTE_ABSNO_MUST_BE_-1__HERE_NOT_MODIFIED_AFTER_QUOTE.CTOR()
			string msg1 = "OK_FOR_LIVESIM_VIA_DDE__NOT_FILLING_PENDINGS"
				+ " QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL__INITIALIZED_HERE_IN_STREAMING_ADAPDER";
			quoteUU.AbsnoPerSymbol = absnoPerSymbolNext;
			changesMade++;
			return changesMade;
		}
		public virtual void PushQuoteReceived_positiveSize(Quote quoteUnboundUnattached_absnoPerSymbolMinusOne) {
			bool quotePushable = this.prepareQuoteReceived_forPushing(quoteUnboundUnattached_absnoPerSymbolMinusOne);
			Quote quoteUU = quoteUnboundUnattached_absnoPerSymbolMinusOne;		// same pointer but AbsnoPerSymbol is fixed now
			if (quotePushable == false) {
				this.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(quoteUU);
			}
			this.StreamingDataSnapshot.SetQuoteLast_forSymbol(quoteUU);
			this.pushQuoteReceived_toSolidifier_step1of2(quoteUU);
			this.pushQuoteReceived_toCharts_step2of2(quoteUU);
		}

		bool prepareQuoteReceived_forPushing(Quote quoteUnboundUnattached_absnoPerSymbolMinusOne) {
			string msig = " //StreamingAdapter.prepareQuoteReceived_forPushing("
#if VERBOSE_STRINGS_SLOW
				+ quoteUnboundUnattached_absnoPerSymbolMinusOne
#endif
				+ ")" + this.ToString();

			bool quotePushable = false;

			if (quoteUnboundUnattached_absnoPerSymbolMinusOne.Size <= 0) {
				string msg = "I_REFUSE_TO_PUSH_QUOTE.Size<=0__USE_StreamingAdapter.PushLevelTwoReceived_alreadyInStreamingSnap()_INSTEAD"
					+ " ONLY_QUOTES_WITH_POSITIVE_SIZE_FORM_BAR_OHLCV QUOTES_WITHOUT_DEAL_MIGHT_APPEAR_AS_FREQUENT_AS_YOU_WANT BUT_MUST_NOT_BE_STORED_IN_BARS"
					;
				Assembler.PopupException(msg + msig, null, false);
				return quotePushable;
			}

			Quote quoteUU = quoteUnboundUnattached_absnoPerSymbolMinusOne;		// same pointer but AbsnoPerSymbol is fixed now
			int changesMade = this.Quote_incrementAbsnoPerSymbol_fixServerTime_minusOneQuotePrevNewerThanCurrent_munisTwoWrongTimezone(quoteUnboundUnattached_absnoPerSymbolMinusOne);
			if (changesMade <= -1) {
				string msg = "skipping_TIME_BACK[" + quoteUU.Symbol + " " + quoteUU.ServerTime + "]";
				Assembler.PopupException(msg + msig, null, false);
				return quotePushable;
			}

			if (changesMade == 0 || quoteUU.AbsnoPerSymbol == -1) {
				string msg = "YOU_DIDNT_FIX_Quote.AbsnoPerSymbol[" + quoteUU + "]";
				Assembler.PopupException(msg + msig, null, false);
			}
			if (quoteUU.ServerTime == DateTime.MinValue) {
				//v1 quote.ServerTime = this.DataSource.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
				string msg = "IM_NOT_PUSHING_FURTHER SERVER_TIME_HAS_TO_BE_FILLED_BY_STREAMING_DERIVED";
				Assembler.PopupException(msg + msig, null, false);
				return quotePushable;
			}

			if (quoteUU.AbsnoPerSymbol == 0) {
				string msg = "I_DONT_WANT_TO_DELIVER_FIRST_EVER_QUOTE_TO_STRATEGY_AND_SOLIDIFIERS[" + quoteUU + "]";
				//Assembler.PopupException(msg, null, false);
				this.StreamingDataSnapshot.SetQuoteLast_forSymbol(quoteUU);
				return quotePushable;
			}

			Quote quoteLast = this.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(quoteUU.Symbol);
			if (quoteLast == null) {
				string msg = "QUIK_JUST_CONNECTED_AND_SENDS_NONSENSE[" + quoteUU + "]";
				Assembler.PopupException(msg + msig, null, false);
				this.StreamingDataSnapshot.SetQuoteLast_forSymbol(quoteUU);
				return quotePushable;
			}

			//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
			//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
			string quoteMillis		= quoteUU.ServerTime.ToString("HH:mm:ss.fff");
			string quotePrevMillis  = quoteLast.ServerTime.ToString("HH:mm:ss.fff");
			if (quoteMillis == quotePrevMillis) {
				//if (quoteUU.SameBidAsk(quotePrev) == false) {
				//    quoteUU.ServerTime = quotePrev.ServerTime.AddMilliseconds(10);
				//} else {
					string msg = quoteUU.Symbol + " SERVER_TIMESTAMP_MUST_INCREASE_EACH_NEXT_INCOMING_QUOTE QUIK_OR_BACKTESTER_FORGOT_TO_INCREASE"
						+ " quoteMillis[" + quoteMillis + "] <="
						+ " quoteLastMillis[" + quotePrevMillis + "]"
						+ ": DDE lagged somewhere?..."
						;
					Assembler.PopupException(msg + msig, null, false);
					return quotePushable;
				//}
			}

			string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(quoteUU.ServerTime);
			if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
				string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_QUOTE " + reasonMarketIsClosedNow + " quote=[" + quoteUU + "]";
				Assembler.PopupException(msg + msig, null, false);
				Assembler.DisplayStatus(msg + msig);
				return quotePushable;
			}
			quotePushable = true;
			return quotePushable;
		}

		void pushQuoteReceived_toCharts_step2of2(Quote quoteUU) {
			string msig = " //StreamingAdapter.pushQuoteReceived_toCharts("
#if VERBOSE_STRINGS_SLOW
				+ quoteUnboundUnattached_absnoPerSymbolMinusOne
#endif
				+ ")" + this.ToString();

			if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0) {
				this.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(quoteUU);

				string symbol_fromQuote = quoteUU != null
							  ? quoteUU.Symbol
							  : "QUOTE_NULL__BAD_SIGN";

				string msg = "THERE_MUST_BE_AT_LEAST_SOLIDIFIER_FOR_SYMBOL[" + symbol_fromQuote + "] YOUR_BAR_FILE_IS_ZERO_SIZE_WHILE_MUST_CONTAIN_HEADER"
					+ " I_REFUSE_TO_PUSH_QUOTE NO_CHANNELS_SUBSCRIBED";
				if (		this.LivesimStreaming_ownImplementation != null
						 && this.LivesimStreaming_ownImplementation.DataSource != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator.ImRunningLivesim) {
					this.LivesimStreaming_ownImplementation.Livesimulator.AbortRunningBacktest_waitAborted(msg, 0);
				}
				if (this is LivesimStreaming) return;	//already reported "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE"
				Assembler.PopupException(msg + msig, null, false);
				//NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
				return;
			}

			try {
				this.DistributorCharts_substitutedDuringLivesim.Push_quoteUnboundUnattached_toChannel(quoteUU);
			} catch (Exception ex) {
				string msg = "CHART_OR_STRATEGY__FAILED_INSIDE Distributor.PushQuoteToDistributionChannels(" + quoteUU + ")";
				Assembler.PopupException(msg + msig, ex);
			}
		}

		void pushQuoteReceived_toSolidifier_step1of2(Quote quoteUU) {
			string msig = " //StreamingAdapter.pushQuoteReceived_toSolidifier("
#if VERBOSE_STRINGS_SLOW
				+ quoteUnboundUnattached_absnoPerSymbolMinusOne
#endif
				+ ")" + this.ToString();

			string symbol = quoteUU.Symbol;
			if (this.DistributorSolidifiers_substitutedDuringLivesim == null) {
				if (this is Backtesting.BacktestStreaming) {
					string msg = "YES_I_NULLIFY_SOLIDIFIERS_IN_BACKTEST_STREAMING";
					//Assembler.PopupException(msg + msig, null, false);
				} else {
					string msg = "ADD_THE_CASE_HERE_FOR_NULL_DISTRIBUTOR_SOLIDIFIERS";
					Assembler.PopupException(msg + msig);
				}
				return;
			}

			SymbolChannel<StreamingConsumerSolidifier> channelForSymbol = this.DistributorSolidifiers_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(symbol);
			bool solidifiers_areEmpty_duringLivesim = this.DistributorSolidifiers_substitutedDuringLivesim.ReasonIwasCreated.Contains(Distributor<StreamingConsumerSolidifier>.SUBSTITUTED_LIVESIM_STARTED);
			if (channelForSymbol == null) {
				if (solidifiers_areEmpty_duringLivesim) return;
				if (this.CreateNewSymbolsIfMissing_fromDde == false) {
					string msg = "YOUR_BARS_ARE_NOT_SAVED__SOLIDIFIERS_ARE_NOT_SUBSCRIBED_TO symbol[" + symbol + "] this.CreateNewSymbolsIfMissing_fromDde==false";
					//Assembler.PopupException(msg + msig);
					return;
				}

				try {
					//v1 DOESNT_NOTIFY_DataSourceTree if (this.DataSource.Symbols.Contains(symbol) == false) this.DataSource.SymbolAdd(symbol);
					Assembler.InstanceInitialized.RepositoryJsonDataSources.SymbolAdd(this.DataSource, symbol);
				} catch (Exception ex) {
					string msg = "MAKE_SURE_YOU_SWITCHED_TO_GUI_THREAD_WHILE_POPULATING_DataSourceTree";
					Assembler.PopupException(msg + msig, ex);
				}
				channelForSymbol = this.SymbolCreated_solidifierSubscribe(symbol);
			}
			try {
				this.DistributorSolidifiers_substitutedDuringLivesim.Push_quoteUnboundUnattached_toChannel(quoteUU);
			} catch (Exception ex) {
				string msg = "SOLIDIFIER__FAILED_INSIDE"
					+ " DistributorSolidifiers.PushQuoteToDistributionChannels(" + quoteUU + ")";
				Assembler.PopupException(msg + msig, ex);
			}
		}

		public virtual void PushLevelTwoReceived_alreadyInStreamingSnap(string symbol) {
		    string msig = " //StreamingAdapter.PushLevelTwoReceived_alreadyInStreamingSnap()" + this.ToString();

		    if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0) {
		        string msg = "I_REFUSE_TO_PUSH_LEVEL_TWO NO_CHARTS_SUBSCRIBED";
		        if (		this.LivesimStreaming_ownImplementation != null
		                 && this.LivesimStreaming_ownImplementation.DataSource != null
		                 && this.LivesimStreaming_ownImplementation.Livesimulator != null
		                 && this.LivesimStreaming_ownImplementation.Livesimulator.ImRunningLivesim) {
		            this.LivesimStreaming_ownImplementation.Livesimulator.AbortRunningBacktest_waitAborted(msg, 0);
		        }
		        if (this is LivesimStreaming) return;	//already reported "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE"
		        Assembler.PopupException(msg + msig, null, false);
		        //NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
		        return;
		    }

		    string recipients = "CHARTS_FOR_DATASOURCE[" + this.DataSource.ToString() + "]";
		    LevelTwoFrozen l2frozen = this.StreamingDataSnapshot.GetLevelTwoFrozenSorted_forSymbol_nullUnsafe(symbol, msig, recipients);
		    if (l2frozen == null) {
		        string msg = "QUIK_JUST_CONNECTED_AND_SENDS_NONSENSE[" + symbol + "]";
		        Assembler.PopupException(msg + msig, null, false);
		        return;
		    }

		    Quote quoteLast = this.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(symbol);
		    if (quoteLast == null) {
		        string msg = "RETURNING_WITHOUT_PUSHING_LEVEL_TWO_TO_CONSUMERS"
		            + " UNABLE_TO_CHECK_IF_MARKET_IS_CLOSED_SINCE_LEVEL_TWO_HAS_NO_SERVER_TIME_AND_LAST_QUOTE_IS_NULL";
		        Assembler.PopupException(msg + msig, null, false);
		        return;
		    }

			//v1
			//string msg3 = "now it's 8:45local => 17:45server; QuikJunior sends GMT+3 1 hour later;"
			//    + " what if I set TZ to GMT+4 (Baku) before day open?"
			//    + " first problem was in midday (couldn't overwrite BARS and needed Editor to slide back / delete)";
			//Assembler.PopupException(msg3);
			DateTime guessingServerTime_forLevel2 = DateTime.MinValue;

		    //v2
			Quote lastQuote = this.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(symbol);
			if (lastQuote != null) {
				DateTime lastQuoteServerTime = lastQuote.ServerTime;
				//TimeSpan diff = guessingServerTime_forLevel2.Subtract(lastQuoteServerTime);
				//if (diff.TotalMinutes > 30) {
				//    string msg2 = "lastQuoteServerTime_GOT_PRIORITY_FOR_LEVEL2"
				//        + " PLS_ADJUST_TIMEZONE_PRIOR_TO_MARKET_OPEN_IN_DataSourceEditor.MarketInfo.Timezone"
				//        + " nowServerTime[" + nowServerTime + "] - lastQuote.ServerTime[" + lastQuote.ServerTime + "]"
				//        + " = [" + diff + "].TotalMinutes > 30";
				//    Assembler.PopupException(msg2, null, false);
				//}
				guessingServerTime_forLevel2 = lastQuoteServerTime;
			} else {
				string msg2 = "MUST_NEVER_HAPPEN TAKING_MarketInfo.ServerTimeNow_KOZ_lastQuote==null"
					+ " WHILE_GUESSING_SERVER_TIME_FOR_LEVEL2";
				Assembler.PopupException(msg2, null, false);
				guessingServerTime_forLevel2 = this.DataSource.MarketInfo.ServerTimeNow;
			}

		    string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(guessingServerTime_forLevel2);
		    if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
		        string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_LEVEL_TWO " + reasonMarketIsClosedNow + " quoteLast=[" + quoteLast + "]";
		        Assembler.PopupException(msg + msig, null, false);
		        Assembler.DisplayStatus(msg);
		        return;
		    }

		    try {
		        this.DistributorCharts_substitutedDuringLivesim.Push_levelTwoFrozen_toChannel(symbol, l2frozen);
		    } catch (Exception ex) {
		        string msg = "CHART_OR_STRATEGY__FAILED_INSIDE Distributor.Push_levelTwoFrozen_toChannel(" + l2frozen + ")";
		        Assembler.PopupException(msg + msig, ex);
		    }

		    string msg1 = "SOLIDIFIERS_OF_LEVEL_TWO_DO_NOT_EXIST";
		    //Assembler.PopupException(msg + msig, ex);
		}

	}
} 