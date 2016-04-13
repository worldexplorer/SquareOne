using System;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter {

		protected virtual int Quote_incrementAbsnoPerSymbol_fixServerTime(Quote quoteUU) {
			int changesMade = 0;
			string msig = " //StreamingAdapter.Quote_fixServerTime_absnoPerSymbol(" + quoteUU + ")" + this.ToString();

			Quote quoteCurrent	= this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(quoteUU.Symbol);
			if (quoteCurrent == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#1 symbol[" + quoteUU.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				Assembler.PopupException(msg + msig, null, false);
				quoteUU.AbsnoPerSymbol = 0;
				changesMade++;
				return changesMade;
			}
			if (quoteUU == quoteCurrent) {
				string msg = "DONT_FEED_STREAMING_WITH_SAME_QUOTE__NOT_FIXING_ANYTHING";
				Assembler.PopupException(msg + msig, null, true);
				return changesMade;
			}

			//Quote quotePrev		= this.StreamingDataSnapshot.GetQuotePrev_forSymbol_nullUnsafe(quoteUU.Symbol);
			if (quoteUU.ServerTime == DateTime.MinValue) {		// spreadQuote arrived from DdeTableDepth without serverTime koz serverTime of Level2 changed is not transmitted over DDE
				if (this.Name.Contains("NOPE_TEST_REAL_QUIK_TOO___StreamingLivesim") == false) {	// QuikStreamingLivesim
					TimeSpan diff_inLocalTime = quoteUU.LocalTime.Subtract(quoteCurrent.LocalTime);
					DateTime serverTime_reconstructed_fromLastQuote = quoteCurrent.ServerTime.Add(diff_inLocalTime);
					quoteUU.ServerTime = serverTime_reconstructed_fromLastQuote;
					changesMade++;
				} else {
					if (quoteCurrent.ParentBarStreaming == null) {
						string msg = "WILL_BINDER_SET_QUOTE_TO_STREAMING_DATA_SNAPSHOT???";
						Assembler.PopupException(msg, null, false);
					} else {
						quoteUU.ServerTime = quoteCurrent.ParentBarStreaming.ParentBars.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
						changesMade++;
					}
				}
			}

			//v1 if (quoteUU.ServerTime == quotePrev.ServerTime) {
			//v2
			string quoteMillis		= quoteUU.ServerTime.ToString("HH:mm:ss.fff");
			string quotePrevMillis  = quoteCurrent.ServerTime.ToString("HH:mm:ss.fff");
			if (quoteMillis == quotePrevMillis) {
				// increase granularity of QuikQuotes (they will have the same ServerTime within the same second, while must have increasing milliseconds; I can't force QUIK print fractions of seconds via DDE export)
				TimeSpan diff_inLocalTime = quoteUU.LocalTime.Subtract(quoteCurrent.LocalTime);
				// diff_localTime.Milliseconds will go to StreamingDataSnapshot with ServerTime fixed, and next diffMillis will be negative for the quote within same second
				int diffMillis_willBeNegative_forSecondQuote_duringSameSecond = diff_inLocalTime.Milliseconds;
				int diffMillis = Math.Abs(diffMillis_willBeNegative_forSecondQuote_duringSameSecond);
				//if (diff_inLocalTime.Seconds == 0) {
					if (diffMillis == 0) {
						string msg = "ARE_WE_BACKTESTING_OR_LIVESIMMING_WITHOUT_INCREASING_SERVER_TIME_FOR_QUOTES_GENERATED?...";
						Assembler.PopupException(msg, null, false);
					} else {
						quoteUU.ServerTime = quoteUU.ServerTime.AddMilliseconds(diffMillis);
						changesMade++;
					}
				//}
			}

			
			if (quoteUU.HasGeneratedSource) {
				string msg = "DO_SOMETHING_WITH_ME_DURING_BACKTEST!!! YES_ABSNO_IS_ZERO__SHOULD_SKIP_OTHER_CHECKS";
				Assembler.PopupException(msg, null, false);
			}

			long absnoPerSymbolNext = -1;
			if (quoteCurrent.AbsnoPerSymbol == -1) {
				string msg = "LAST_QUOTE_DIDNT_HAVE_ABSNO_SET_BY_STREAMING_ADAPDER_ON_PREV_ITERATION FORCING_ZERO";
				Assembler.PopupException(msg + msig, null, false);
				absnoPerSymbolNext = 0;
			} else {
				absnoPerSymbolNext = quoteCurrent.AbsnoPerSymbol + 1;	// you must see lock(){} upstack
			}

			if (absnoPerSymbolNext == -1) {
				string msg = "I_REFUSE_TO_FIX quote.AbsnoPerSymbol[-1] && absnoPerSymbolNext[-1]";
				Assembler.PopupException(msg + msig, null, true);
				return changesMade;
			}

			if (quoteUU.AbsnoPerSymbol != -1) {
				string msg = " INJECTED_QUOTES_HAVE_AbsnoPerSymbol!=-1_AND_THIS_IS_NOT_AN_ERROR";
				if (quoteUU.IamInjectedToFillPendingAlerts == false) {
					msg = "INJECTED_QUOTE_MUST_HAVE_INTRABAR_SHIFT[" + Quote.IntraBarSernoShift_forGenerated_towardsPendingFill + "]" + msg;
					Assembler.PopupException(msg + msig);
					return changesMade;
				}
				if (quoteUU.HasGeneratedSource == false) {
					msg = "INJECTED_QUOTE_MUST_HAVE_RIGHT_SOURCE" + msg;
					Assembler.PopupException(msg + msig);
					return changesMade;
				}
				if (quoteUU.AbsnoPerSymbol >= absnoPerSymbolNext) {
					msg = "DONT_FEED_ME_WITH_SAME_QUOTE_BACKTESTER quote.AbsnoPerSymbol[" + quoteUU.AbsnoPerSymbol + "]"
						+ " MUST_BE_GREATER_THAN quoteCurrent.AbsnoPerSymbol[" + quoteCurrent.AbsnoPerSymbol + "]" + msg;
					Assembler.PopupException(msg + msig);
					return changesMade;
				}
			}

			//QUOTE_ABSNO_MUST_BE_-1__HERE_NOT_MODIFIED_AFTER_QUOTE.CTOR()
			string msg1 = "OK_FOR_LIVESIM_VIA_DDE__NOT_FILLING_PENDINGS"
				+ " QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL__INITIALIZED_HERE_IN_STREAMING_ADAPDER";
			quoteUU.AbsnoPerSymbol = absnoPerSymbolNext;
			changesMade++;
			return changesMade;
		}
		public virtual void PushQuoteReceived(Quote quoteUnboundUnattached_absnoPerSymbolMinusOne) {
			string msig = " //StreamingAdapter.PushQuoteReceived()" + this.ToString();

			if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0) {
				this.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(quoteUnboundUnattached_absnoPerSymbolMinusOne);

				string msg = "I_REFUSE_TO_PUSH_QUOTE NO_CHARTS_SUBSCRIBED";
				if (		this.LivesimStreaming_ownImplementation != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator.ImRunningLivesim) {
					this.LivesimStreaming_ownImplementation.Livesimulator.AbortRunningBacktest_waitAborted(msg, 0);
				}
				if (this is LivesimStreaming) return;	//already reported "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE"
				Assembler.PopupException(msg + msig, null, false);
				//NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
				return;
			}

			Quote quoteUnboundUnattached = quoteUnboundUnattached_absnoPerSymbolMinusOne;		// same pointer but AbsnoPerSymbol is fixed now
			if (quoteUnboundUnattached_absnoPerSymbolMinusOne.FakeQuote_toDeliverSpread_makeLevel2Repaint == false) {
				int changesMade = this.Quote_incrementAbsnoPerSymbol_fixServerTime(quoteUnboundUnattached_absnoPerSymbolMinusOne);
				if (changesMade == 0 || quoteUnboundUnattached.AbsnoPerSymbol == -1) {
					string msg = "YOU_DIDNT_FIX_Quote.AbsnoPerSymbol[" + quoteUnboundUnattached + "]";
					Assembler.PopupException(msg + msig);
				}

				if (quoteUnboundUnattached.ServerTime == DateTime.MinValue) {
					//v1 quote.ServerTime = this.DataSource.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
					string msg = "IM_NOT_PUSHING_FURTHER SERVER_TIME_HAS_TO_BE_FILLED_BY_STREAMING_DERIVED";
					Assembler.PopupException(msg + msig, null, false);
					return;
				}

				if (quoteUnboundUnattached.AbsnoPerSymbol == 0) {
					string msg = "I_DONT_WANT_TO_DELIVER_FIRST_EVER_QUOTE_TO_STRATEGY_AND_SOLIDIFIERS[" + quoteUnboundUnattached + "]";
					//Assembler.PopupException(msg, null, false);
					this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteUnboundUnattached);
					return;
				}

				Quote quotePrev = this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(quoteUnboundUnattached.Symbol);
				if (quotePrev == null) {
					string msg = "QUIK_JUST_CONNECTED_AND_SENDS_NONSENSE[" + quoteUnboundUnattached + "]";
					Assembler.PopupException(msg + msig, null, false);
					this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteUnboundUnattached);
					return;
				}

				//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
				//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
				string quoteMillis		= quoteUnboundUnattached.ServerTime.ToString("HH:mm:ss.fff");
				string quotePrevMillis  = quotePrev.ServerTime.ToString("HH:mm:ss.fff");
				if (quoteMillis == quotePrevMillis) {
					string msg = quoteUnboundUnattached.Symbol + " SERVER_TIMESTAMP_MUST_INCREASE_EACH_NEXT_INCOMING_QUOTE QUIK_OR_BACKTESTER_FORGOT_TO_INCREASE"
						+ " quoteMillis[" + quoteMillis + "] <="
						+ " quoteLastMillis[" + quotePrevMillis + "]"
						+ ": DDE lagged somewhere?..."
						;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}


				string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(quoteUnboundUnattached.ServerTime);
				if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
					string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_QUOTE " + reasonMarketIsClosedNow + " quote=[" + quoteUnboundUnattached + "]";
					Assembler.PopupException(msg + msig, null, false);
					Assembler.DisplayStatus(msg + msig);
					return;
				}

				if (quoteUnboundUnattached.Size > 0) {
					this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteUnboundUnattached);
				}
			}

			try {
				this.DistributorCharts_substitutedDuringLivesim.Push_quoteUnboundUnattached_toChannel(quoteUnboundUnattached);
			} catch (Exception ex) {
				string msg = "CHART_OR_STRATEGY__FAILED_INSIDE Distributor.PushQuoteToDistributionChannels(" + quoteUnboundUnattached + ")";
				Assembler.PopupException(msg + msig, ex);
			}

			if (quoteUnboundUnattached.Size == 0 || quoteUnboundUnattached.FakeQuote_toDeliverSpread_makeLevel2Repaint) {
				string msg = "SOLIDIFIERS_SHOULD_NEVER_RECEIVE_FakeQuote";
				return;
			}

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

			string symbol = quoteUnboundUnattached.Symbol;
			SymbolChannel<StreamingConsumerSolidifier> channelForSymbol = this.DistributorSolidifiers_substitutedDuringLivesim.GetChannelFor_nullMeansWasntSubscribed(symbol);
			bool okayForDistribSolidifiers_toBe_empty = this.DistributorSolidifiers_substitutedDuringLivesim.ReasonIwasCreated.Contains(Distributor<StreamingConsumerSolidifier>.SUBSTITUTED_LIVESIM_STARTED);
			if (channelForSymbol == null) {
				if (okayForDistribSolidifiers_toBe_empty) return;
				string msg = "YOUR_BARS_ARE_NOT_SAVED__SOLIDIFIERS_ARE_NOT_SUBSCRIBED_TO symbol[" + symbol + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.DistributorSolidifiers_substitutedDuringLivesim	.Push_quoteUnboundUnattached_toChannel(quoteUnboundUnattached);
			} catch (Exception ex) {
				string msg = "SOLIDIFIER__FAILED_INSIDE"
					+ " DistributorSolidifiers.PushQuoteToDistributionChannels(" + quoteUnboundUnattached + ")";
				Assembler.PopupException(msg + msig, ex);
			}
		}

		public virtual void PushLevelTwoReceived_alreadyInStreamingSnap(string symbol) {
		    string msig = " //StreamingAdapter.PushLevelTwoReceived_alreadyInStreamingSnap()" + this.ToString();

		    if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0) {
		        string msg = "I_REFUSE_TO_PUSH_LEVEL_TWO NO_CHARTS_SUBSCRIBED";
		        if (		this.LivesimStreaming_ownImplementation != null
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

		    Quote quoteCurrent = this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(symbol);
		    if (quoteCurrent == null) {
		        string msg = "RETURNING_WITHOUT_PUSHING_LEVEL_TWO_TO_CONSUMERS"
		            + " UNABLE_TO_CHECK_IF_MARKET_IS_CLOSED_SINCE_LEVEL_TWO_HAS_NO_SERVER_TIME_AND_LAST_QUOTE_IS_NULL";
		        Assembler.PopupException(msg + msig, null, false);
		        return;
		    }

			DateTime nowServerTime = this.DataSource.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
		    string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(nowServerTime);
		    if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
		        string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_LEVEL_TWO " + reasonMarketIsClosedNow + " quoteCurrent=[" + quoteCurrent + "]";
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