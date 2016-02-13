using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class DataDistributor {
					object																				lockConsumersBySymbol;
		protected	StreamingAdapter																	StreamingAdapter;
		public		Dictionary<string, Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>>	DistributionChannels	{ get; protected set; }

		public		string	ReasonIwasCreated 	{ get; protected set; }

		DataDistributor(string reasonIwasCreated) {
			DistributionChannels	= new Dictionary<string, Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>>();
			lockConsumersBySymbol	= new object();
			ReasonIwasCreated		= reasonIwasCreated;
		}
		public DataDistributor(StreamingAdapter streamingAdapter, string reasonIwasCreated) : this(reasonIwasCreated) {
			this.StreamingAdapter = streamingAdapter;
			//this.ReasonIwasCreated = this.StreamingAdapter + ":" + this.ReasonIwasCreated;
		}

		public virtual bool ConsumerQuoteSubscribe(string symbol, BarScaleInterval scaleInterval,
				StreamingConsumer quoteConsumer, bool quotePumpSeparatePushingThreadEnabled) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled, this.ReasonIwasCreated);
				newChannel.ConsumersQuoteAdd(quoteConsumer);
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> newScaleChannels = new Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>();
				newScaleChannels.Add(scaleInterval, newChannel);
				this.DistributionChannels.Add(symbol, newScaleChannels);
				// first-deserialized: Strategy on RIM3_5-minutes => Pump/Thread should be started as well
				if (newChannel.QuotePump_nullUnsafe != null && newChannel.QuotePump_nullUnsafe.Paused) newChannel.QuotePump_nullUnsafe.PusherUnpause();
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled, this.ReasonIwasCreated);
				newChannel.ConsumersQuoteAdd(quoteConsumer);
				channels.Add(scaleInterval, newChannel);
				// second-deserialized: chartNoStrategy on RIM3_20-minutes => Pump/Thread should be started as well
				if (newChannel.QuotePump_nullUnsafe != null && newChannel.QuotePump_nullUnsafe.Paused) newChannel.QuotePump_nullUnsafe.PusherUnpause();
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersQuoteContains(quoteConsumer) == false) {
				channel.ConsumersQuoteAdd(quoteConsumer);
				return true;
			}
			Assembler.PopupException("quoteConsumer[" + quoteConsumer + "] already registered for [" + channel + "]; returning");
			return false;
		} }
		public virtual bool ConsumerQuoteUnsubscribe(string symbol, BarScaleInterval scaleInterval, StreamingConsumer quoteConsumer) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: symbol[" + symbol + "] is not registered for any consumers; returning";
				Assembler.PopupException(msg);
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				string symbolDistributorsAsString = "";
				foreach (SymbolScaleDistributionChannel d in channels.Values) symbolDistributorsAsString += d + ",";
				symbolDistributorsAsString.TrimEnd(',');
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: scaleInterval[" + scaleInterval + "] not found among distributors [" + symbolDistributorsAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersQuoteContains(quoteConsumer) == false) {
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: consumer not found in [" + channel.ConsumersQuoteAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			channel.ConsumersQuoteRemove(quoteConsumer);
			if (channel.ConsumersBarCount == 0 && channel.ConsumersQuoteCount == 0) {
				//Assembler.PopupException("QuoteConsumer [" + consumer + "] was the last one using [" + symbol + "][" + scaleInterval + "]; removing QuoteBarDistributor[" + channel + "]");
				if (channel.QuotePump_nullUnsafe != null) channel.QuotePump_nullUnsafe.PushingThreadStop();
				channels.Remove(scaleInterval);
				if (channels.Count == 0) {
					//Assembler.PopupException("QuoteConsumer [" + scaleInterval + "] was the last one listening for [" + symbol + "]");
					//Assembler.PopupException("...removing[" + symbol + "] from this.DistributionChannels[" + this.DistributionChannels + "]");
					this.DistributionChannels.Remove(symbol);
					//Assembler.PopupException("...UpstreamUnSubscribing [" + symbol + "]");
					this.StreamingAdapter.UpstreamUnSubscribe(symbol);
				}
				return true;
			}
			return false;
		} }
		public virtual bool ConsumerQuoteIsSubscribed(string symbol, BarScaleInterval scaleInterval_nullForAny, StreamingConsumer consumer, bool addingTrue_checkingFalse = false) {
			bool ret = false;
			Dictionary<string, List<BarScaleInterval>> symbolsScaleIntervals = this.SymbolsScaleIntervals_QuoteConsumerIsRegisteredFor(consumer, addingTrue_checkingFalse);
			if (symbolsScaleIntervals == null) return ret;
			if (symbolsScaleIntervals.ContainsKey(symbol)) {
				if (scaleInterval_nullForAny == null) {
					ret = true;
				} else {
					List<BarScaleInterval> timeframes = symbolsScaleIntervals[symbol];
					if (timeframes == null) return ret;
					ret = timeframes.Contains(scaleInterval_nullForAny);
				}
			}
			return ret;
		}

		public virtual bool ConsumerBarSubscribe(string symbol, BarScaleInterval scaleInterval,
										StreamingConsumer barConsumer, bool quotePumpSeparatePushingThreadEnabled) { lock (this.lockConsumersBySymbol) {
			if (barConsumer is StreamingSolidifier) {
				string msg = "StreamingSolidifier_DOESNT_SUPPORT_ConsumerBarsToAppendInto";
			} else {
				Bar barStaticLast = barConsumer.ConsumerBarsToAppendInto.BarStaticLast_nullUnsafe;
				bool isLive				= barConsumer			is ChartStreamingConsumer;
				bool isBacktest			= barConsumer			is BacktestQuoteBarConsumer;
				bool isLivesim			= barConsumer			is LivesimQuoteBarConsumer;
				bool isLivesimDefault	= this.StreamingAdapter is LivesimStreamingDefault;
				if (barStaticLast == null && isLivesimDefault) {	// isBacktest,isLivesim are magically fine; where did you notice the problem?
					string msg = "YOUR_BAR_CONSUMER_SHOULD_HAVE_BarStaticLast_NON_NULL"
						+ " MOST_LIKELY_YOU_WILL_GET_MESSAGE__THERE_IS_NO_STATIC_BAR_DURING_FIRST_4_QUOTES_GENERATED__ONLY_STREAMING";
					Assembler.PopupException(msg, null, false);
				}
			}
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled, this.ReasonIwasCreated);
				newChannel.ConsumersBarAdd(barConsumer);
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> newScaleChannels = new Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>();
				newScaleChannels.Add(scaleInterval, newChannel);
				this.DistributionChannels.Add(symbol, newScaleChannels);
				// first-deserialized: Strategy on RIM3_5-minutes => Pump/Thread should be started as well
				if (newChannel.QuotePump_nullUnsafe != null && newChannel.QuotePump_nullUnsafe.Paused) newChannel.QuotePump_nullUnsafe.PusherUnpause();
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled, this.ReasonIwasCreated);
				newChannel.ConsumersBarAdd(barConsumer);
				channels.Add(scaleInterval, newChannel);
				// second-deserialized: chartNoStrategy on RIM3_20-minutes => Pump/Thread should be started as well
				if (newChannel.QuotePump_nullUnsafe != null && newChannel.QuotePump_nullUnsafe.Paused) newChannel.QuotePump_nullUnsafe.PusherUnpause();
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersBarContains(barConsumer) == false) {
				channel.ConsumersBarAdd(barConsumer);
				return true;
			}
			Assembler.PopupException("BarConsumer [" + barConsumer + "] already registered for [" + channel + "]; returning");
			return false;
		} }
		public virtual bool ConsumerBarUnsubscribe(string symbol, BarScaleInterval scaleInterval,
										StreamingConsumer quoteConsumer) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: symbol[" + symbol + "] is not registered for any consumers; returning";
				Assembler.PopupException(msg);
				return false;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channelsByScaleInterval = this.DistributionChannels[symbol];
			if (channelsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string symbolDistributorsAsString = "";
				foreach (SymbolScaleDistributionChannel d in channelsByScaleInterval.Values) symbolDistributorsAsString += d + ",";
				symbolDistributorsAsString.TrimEnd(',');
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: scaleInterval[" + scaleInterval + "] not found among distributors [" + symbolDistributorsAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleDistributionChannel channel = channelsByScaleInterval[scaleInterval];
			if (channel.ConsumersBarContains(quoteConsumer) == false) {
				string msg = "Can't unregister quoteConsumer[" + quoteConsumer + "]: consumer not found in [" + channel.ConsumersBarAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			channel.ConsumersBarRemove(quoteConsumer);
			if (channel.ConsumersBarCount == 0 && channel.ConsumersQuoteCount == 0) {
				//Assembler.PopupException("BarConsumer [" + consumer + "] was the last one using [" + symbol + "][" + scaleInterval + "]; removing QuoteBarDistributor[" + distributor + "]");
				if (channel.QuotePump_nullUnsafe != null) channel.QuotePump_nullUnsafe.PushingThreadStop();
				channelsByScaleInterval.Remove(scaleInterval);
				if (channelsByScaleInterval.Count == 0) {
					//Assembler.PopupException("BarConsumer [" + scaleInterval + "] was the last one listening for [" + symbol + "]");
					//Assembler.PopupException("...removing[" + symbol + "] from this.DistributionChannels[" + this.DistributionChannels + "]");
					this.DistributionChannels.Remove(symbol);
					//Assembler.PopupException("...UpstreamUnSubscribing [" + symbol + "]");
					this.StreamingAdapter.UpstreamUnSubscribe(symbol);
				}
				return true;
			}
			return false;
		} }
		public virtual bool ConsumerBarIsSubscribed(string symbol, BarScaleInterval scaleInterval_nullForAny,
										StreamingConsumer barConsumer, bool addingTrue_checkingFalse = false) {
			bool ret = false;
			Dictionary<string, List<BarScaleInterval>> symbolsScaleIntervals = this.SymbolsScaleIntervals_BarConsumerIsRegisteredFor(barConsumer, addingTrue_checkingFalse);
			if (symbolsScaleIntervals == null) return ret;
			if (symbolsScaleIntervals.ContainsKey(symbol)) {
				if (scaleInterval_nullForAny == null) {
					ret = true;
				} else {
					List<BarScaleInterval> timeframes = symbolsScaleIntervals[symbol];
					if (timeframes == null) return ret;
					ret = timeframes.Contains(scaleInterval_nullForAny);
				}
			}
			return ret;
		}

		public Dictionary<string, List<BarScaleInterval>> SymbolsScaleIntervals_QuoteConsumerIsRegisteredFor(
										StreamingConsumer quoteConsumer, bool addingTrue_checkingFalse = false) {
			Dictionary<string, List<BarScaleInterval>> ret = null;
			foreach (string symbol in this.DistributionChannels.Keys) {
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> consumersByScaleInterval = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in consumersByScaleInterval.Keys) {
					SymbolScaleDistributionChannel consumers = consumersByScaleInterval[scaleInterval];
					if (consumers.ConsumersQuoteContains(quoteConsumer) == false) continue;
					if (ret == null) ret = new Dictionary<string, List<BarScaleInterval>>();
					if (addingTrue_checkingFalse) continue;
					if (ret.ContainsKey(symbol) == false) ret.Add(symbol, new List<BarScaleInterval>());
					ret[symbol].Add(scaleInterval);
				}
			}
			return ret;
		}
		public Dictionary<string, List<BarScaleInterval>> SymbolsScaleIntervals_BarConsumerIsRegisteredFor(
										StreamingConsumer barConsumer, bool addingTrue_checkingFalse = true) {
			Dictionary<string, List<BarScaleInterval>> ret = null;
			foreach (string symbol in this.DistributionChannels.Keys) {
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> consumersByScaleInterval = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in consumersByScaleInterval.Keys) {
					SymbolScaleDistributionChannel consumers = consumersByScaleInterval[scaleInterval];
					if (consumers.ConsumersBarContains(barConsumer) == false) continue;
					if (ret == null) ret = new Dictionary<string, List<BarScaleInterval>>();
					if (addingTrue_checkingFalse) continue;
					if (ret.ContainsKey(symbol) == false) ret.Add(symbol, new List<BarScaleInterval>());
					ret[symbol].Add(scaleInterval);
				}
			}
			return ret;
		}

		public virtual void PushQuoteToDistributionChannels(Quote quote) {
			if (string.IsNullOrEmpty(quote.Symbol)) {
				Assembler.PopupException("quote[" + quote + "]'se Symbol is null or empty, returning");
				return;
			}
			Quote lastQuote = this.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(quote.Symbol);
			List<SymbolScaleDistributionChannel> channelsForSymbol = this.GetDistributionChannels_allScaleIntervals_forSymbol(quote.Symbol);
			foreach (SymbolScaleDistributionChannel channel in channelsForSymbol) {
				// late quote should be within current StreamingBar, otherwize don't deliver for channel
				if (lastQuote != null && quote.ServerTime < lastQuote.ServerTime) {
					Bar streamingBar = channel.StreamingBarFactoryUnattached.BarStreamingUnattached;
					if (quote.ServerTime <= streamingBar.DateTimeOpen) {
						string msg = "skipping old quote for quote.ServerTime[" + quote.ServerTime + "], can only accept for current"
							+ " StreamingBar (" + streamingBar.DateTimeOpen + " .. " + streamingBar.DateTimeNextBarOpenUnconditional + "];"
							+ " quote=[" + quote + "]";
						Assembler.PopupException(msg);
						continue;
					}
				}
				// don't clone quote here!! enrich inside each channel => IntraBarSerno++,
				// then clone quote for every consumer and earlyBind() to BarStreaming, link it to BarsParent (variable on length of the history loaded into Bars)
				channel.PushQuoteToPump(quote);
			}
			//this.RaiseOnQuoteAsyncPushedToAllDistributionChannels(quote);
		}
		public List<SymbolScaleDistributionChannel> GetDistributionChannels_allScaleIntervals_forSymbol(string symbol) { lock (this.lockConsumersBySymbol) {
			List<SymbolScaleDistributionChannel> channels = new List<SymbolScaleDistributionChannel>();
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "STARTING_LIVESIM:CLICK_CHART>BARS>SUBSCRIBE symbol[" + symbol + "]"
					//+ " YOU_DIDNT_SUBSCRIBE_AFTER_DISTRIBUTION_CHANNELS_CLEAR"
					//+ " MOST_LIKELY_YOU_ABORTED_BACKTEST_BY_CHANGING_SELECTORS_IN_GUI_FIX_HANDLERS"
					;
				Assembler.PopupException(msg, null, false);
				return channels;
			}
			channels = new List<SymbolScaleDistributionChannel>(this.DistributionChannels[symbol].Values);
			return channels;
		} }
		public SymbolScaleDistributionChannel GetDistributionChannelFor_nullUnsafe(string symbol, BarScaleInterval barScaleInterval) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "NO_SYMBOL_SUBSCRIBED DataDistributor[" + this + "].DistributionChannels.ContainsKey(" + symbol + ")=false INVOKER_NULL_CHECK_EYEBALLED";
				//Assembler.PopupException(msg, null, false);
				return null;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> distributionChannels = this.DistributionChannels[symbol];
			if (distributionChannels.ContainsKey(barScaleInterval) == false) {
				string msg = "NO_SCALEINTERVAL_SUBSCRIBED DataDistributor[" + this
					+ "].DistributionChannels[" + symbol + "].ContainsKey(" + barScaleInterval + ")=false";
				Assembler.PopupException(msg);
				return null;
			}
			return distributionChannels[barScaleInterval];
		} }
		public List<SymbolScaleDistributionChannel> GetDistributionChannels_forSymbol_exceptForChartLivesimming(string symbol
					, BarScaleInterval scaleIntervalOnly_anyIfNull, StreamingConsumer chartShadowToExclude) { lock (this.lockConsumersBySymbol) {
			List<SymbolScaleDistributionChannel> ret = new List<SymbolScaleDistributionChannel>();
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "YOU_DIDNT_SUBSCRIBE_AFTER_DISTRIBUTION_CHANNELS_CLEAR symbol[" + symbol + "] MOST_LIKELY_YOU_ABORTED_BACKTEST_BY_CHANGING_SELECTORS_IN_GUI_FIX_HANDLERS";
				Assembler.PopupException(msg, null, false);
				return null;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> distributionChannelsForSymbol = this.DistributionChannels[symbol];
			if (scaleIntervalOnly_anyIfNull != null && distributionChannelsForSymbol.ContainsKey(scaleIntervalOnly_anyIfNull) == false) {
				string msg = "NO_SCALEINTERVAL_SUBSCRIBED DataDistributor[" + this
					+ "].DistributionChannels[" + symbol + "].ContainsKey(" + scaleIntervalOnly_anyIfNull + ")=false";
				Assembler.PopupException(msg);
				return null;
			}
			foreach (SymbolScaleDistributionChannel channel in distributionChannelsForSymbol.Values) {
				if (scaleIntervalOnly_anyIfNull != null && channel.ScaleInterval != scaleIntervalOnly_anyIfNull) continue;
				SymbolScaleDistributionChannel channelClone = channel.CloneFullyFunctional_withNewDictioniariesAndLists_toPossiblyRemoveMatchingConsumers();
				if (chartShadowToExclude != null) {
					if (channelClone.ConsumersBarContains	(chartShadowToExclude)) channelClone.ConsumersBarRemove		(chartShadowToExclude);
					if (channelClone.ConsumersQuoteContains	(chartShadowToExclude)) channelClone.ConsumersQuoteRemove	(chartShadowToExclude);
				}
				if (channelClone.ConsumersBarCount == 0 && channelClone.ConsumersQuoteCount == 0) continue;
				ret.Add(channelClone);
			}
			return ret;
		} }

		internal void AllQuotePumps_Pause(string livesimCausingPauseName) {
			string msig = " //AllQuotePumps_Pause(" + livesimCausingPauseName + ") DATADISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "";
			foreach(SymbolScaleDistributionChannel eachChannel in this.flattenDistributionChannels()) {
				if (eachChannel.QuotePump_nullUnsafe == null) {
					string msg1 = "CHANNEL_IS_A_QUEUE_NOT_PUMP__CAN_NOT_PAUSE eachChannel=[" + eachChannel + "]";
					Assembler.PopupException(msg1 + msig);
				}
				if (eachChannel.QuotePump_nullUnsafe.Paused == true) {
					string msg1 = "PUMP_ALREADY_PAUSED livesimCausingPauseName=[" + livesimCausingPauseName + "]";
					Assembler.PopupException(msg1 + msig, null, false);
				} else {
					eachChannel.QuotePump_nullUnsafe.PusherPause();
				}
				if (msg != "") msg += ",";
				msg += "[" + eachChannel.ToStringShort + "]";
			}
			if (string.IsNullOrEmpty(msg)) {
				msg = "NO_QUOTE_PUMPS_PAUSED";
			} else {
				msg = "QUOTE_PUMPS_PAUSED_INSIDE_REPLACED_DATADISTRIBUTOR " + msg;
			}
			Assembler.PopupException(msg + msig, null, false);
		}

		internal void AllQuotePumps_Unpause(string livesimCausedPauseName) {
			string msig = " //AllQuotePumps_Unpause(" + livesimCausedPauseName + ") DATADISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "";
			foreach(SymbolScaleDistributionChannel eachChannel in this.flattenDistributionChannels()) {
				if (eachChannel.QuotePump_nullUnsafe == null) {
					string msg1 = "CHANNEL_IS_A_QUEUE_NOT_PUMP__CAN_NOT_PAUSE eachChannel=[" + eachChannel + "]";
					Assembler.PopupException(msg1 + msig);
				}
				if (eachChannel.QuotePump_nullUnsafe.Paused == false) {
					string msg1 = "PUMP_ALREADY_UNPAUSED livesimCausedPauseName=[" + livesimCausedPauseName + "]";
					Assembler.PopupException(msg1 + msig);
				} else {
					eachChannel.QuotePump_nullUnsafe.PusherUnpause();
				}
				if (msg != "") msg += ",";
				msg += "[" + eachChannel.ToStringShort + "]";
			}
			if (string.IsNullOrEmpty(msg)) {
				msg = "NO_QUOTE_PUMPS_UNPAUSED";
			} else {
				msg = "QUOTE_PUMPS_UNPAUSED_INSIDE_RESTORED_DATADISTRIBUTOR " + msg;
			}
			Assembler.PopupException(msg + msig, null, false);
		}


		internal void AllQuotePumps_Stop(string livesimCausedPauseName) {
			string msig = " //AllQuotePumps_Stop(" + livesimCausedPauseName + ") DATADISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "";
			foreach(SymbolScaleDistributionChannel eachChannel in this.flattenDistributionChannels()) {
				//v1 if (eachChannel.QuoteQueue.HasSeparatePushingThread == false) {
				if (eachChannel.QuotePump_nullUnsafe == null) {
					string msg1 = "QUOTE_PUMP_IS_A_QUEUE_MUST_BE_PUMP livesimCausedPauseName=[" + livesimCausedPauseName + "]";
					Assembler.PopupException(msg1 + msig);
					continue;
				}
				eachChannel.QuotePump_nullUnsafe.PushingThreadStop();
				if (msg != "") msg += ",";
				msg += "[" + eachChannel.ToStringShort + "]";
			}
			if (string.IsNullOrEmpty(msg)) {
				msg = "NO_QUOTE_PUMPS_STOPPED_THEIR_THREADS";
			} else {
				msg = "LIVESIM_RECEIVING_PUMPS_STOPPED_INSIDE_REPLACED " + msg;
			}
			Assembler.PopupException(msg + msig, null, false);
		}

		internal void SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(string symbol, BarScaleInterval barScaleInterval) {
			SymbolScaleDistributionChannel channel = this.GetDistributionChannelFor_nullUnsafe(symbol, barScaleInterval);
			if (channel == null) {
				string msg = "SPLIT_QUOTE_PUMP_TO_SINGLE_THREADED_AND_SELF_LAUNCHING";
				Assembler.PopupException(msg);
				return;
			}
			if (this.StreamingAdapter.QuotePumpSeparatePushingThreadEnabled) {
			    channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.UpdateThreadNameAfterMaxConsumersSubscribed = true;
				// SELF_MANAGED_BY_CHANNEL channel.QuoteQueue.PusherUnpause();
			    channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.SetThreadName();
			}
		}

		List<SymbolScaleDistributionChannel> flattenDistributionChannels() { lock (this.lockConsumersBySymbol) {
			List<SymbolScaleDistributionChannel> ret = new List<SymbolScaleDistributionChannel>();
			foreach(Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channelsForEachSymbol in this.DistributionChannels.Values) {
				foreach(SymbolScaleDistributionChannel eachChannel in channelsForEachSymbol.Values) {
					if (ret.Contains(eachChannel)) continue;
					ret.Add(eachChannel);
				}
			}
			return ret;
		} }

		public override string ToString() {
			string ret = this.ToStringCommon(false);
			return ret;
		}

		public string ToStringNames { get {
			string ret = this.ToStringCommon(true);
			return ret;
		} }

		private string ToStringCommon(bool consumerNamesOnly = false) {
			string ret = this.ReasonIwasCreated + " DataDistributorFor[" + this.StreamingAdapter.Name + "]: ";
			foreach (string symbol in this.DistributionChannels.Keys) {
				string consumers = "";
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> distributionChannel = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in distributionChannel.Keys) {
					if (consumers != "") consumers += ",";
					SymbolScaleDistributionChannel channel = distributionChannel[scaleInterval];
					consumers += consumerNamesOnly ? channel.ToStringNames : channel.ToString();
				}
				ret += symbol + "{" + consumers + "}";
			}
			if (string.IsNullOrEmpty(ret)) ret = "NO_CONSUMERS_SUBSCRIBED";
			return ret;
		}
	}
}