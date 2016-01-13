using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class DataDistributor {
					object				lockConsumersBySymbol;
		protected	StreamingAdapter	StreamingAdapter;
		public		Dictionary<string, Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>> DistributionChannels { get; protected set; }

		DataDistributor() {
			DistributionChannels	= new Dictionary<string, Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>>();
			lockConsumersBySymbol	= new object();
		}
		public DataDistributor(StreamingAdapter streamingAdapter) : this() {
			this.StreamingAdapter = streamingAdapter;
		}

		public virtual bool ConsumerQuoteSubscribe(string symbol, BarScaleInterval scaleInterval,
				IStreamingConsumer consumer, bool quotePumpSeparatePushingThreadEnabled) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled);
				newChannel.ConsumersQuoteAdd(consumer);
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> newScaleChannels = new Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>();
				newScaleChannels.Add(scaleInterval, newChannel);
				this.DistributionChannels.Add(symbol, newScaleChannels);
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled);
				newChannel.ConsumersQuoteAdd(consumer);
				channels.Add(scaleInterval, newChannel);
				return true;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersQuoteContains(consumer) == false) {
				channel.ConsumersQuoteAdd(consumer);
				return true;
			}
			Assembler.PopupException("QuoteConsumer [" + consumer + "] already registered for [" + channel + "]; returning");
			return false;
		} }
		public virtual bool ConsumerQuoteUnsubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "Can't unregister QuoteConsumer [" + consumer + "]: symbol[" + symbol + "] is not registered for any consumers; returning";
				Assembler.PopupException(msg);
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				string symbolDistributorsAsString = "";
				foreach (SymbolScaleDistributionChannel d in channels.Values) symbolDistributorsAsString += d + ",";
				symbolDistributorsAsString.TrimEnd(',');
				string msg = "Can't unregister QuoteConsumer [" + consumer + "]: scaleInterval[" + scaleInterval + "] not found among distributors [" + symbolDistributorsAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersQuoteContains(consumer) == false) {
				string msg = "Can't unregister QuoteConsumer [" + consumer + "]: consumer not found in [" + channel.ConsumersQuoteAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			channel.ConsumersQuoteRemove(consumer);
			if (channel.ConsumersBarCount == 0 && channel.ConsumersQuoteCount == 0) {
				//Assembler.PopupException("QuoteConsumer [" + consumer + "] was the last one using [" + symbol + "][" + scaleInterval + "]; removing QuoteBarDistributor[" + channel + "]");
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
		public virtual bool ConsumerQuoteIsSubscribed(string symbol, BarScaleInterval scaleInterval_nullForAny, IStreamingConsumer consumer, bool addingTrue_checkingFalse = true) {
			bool ret = false;
			Dictionary<string, List<BarScaleInterval>> symbolsScaleIntervals = this.SymbolsScaleIntervalsQuote_isConsumerRegistered(consumer, addingTrue_checkingFalse);
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
										IStreamingConsumer consumer, bool quotePumpSeparatePushingThreadEnabled) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled);
				newChannel.ConsumersBarAdd(consumer);
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> newScaleChannels = new Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>();
				newScaleChannels.Add(scaleInterval, newChannel);
				this.DistributionChannels.Add(symbol, newScaleChannels);
				if (this.StreamingAdapter.UpstreamIsSubscribed(symbol) == false) {
					this.StreamingAdapter.UpstreamSubscribe(symbol);
				}
				return true;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channels = this.DistributionChannels[symbol];
			if (channels.ContainsKey(scaleInterval) == false) {
				SymbolScaleDistributionChannel newChannel = new SymbolScaleDistributionChannel(symbol, scaleInterval, quotePumpSeparatePushingThreadEnabled);
				newChannel.ConsumersBarAdd(consumer);
				channels.Add(scaleInterval, newChannel);
				return true;
			}
			SymbolScaleDistributionChannel channel = channels[scaleInterval];
			if (channel.ConsumersBarContains(consumer) == false) {
				channel.ConsumersBarAdd(consumer);
				return true;
			}
			Assembler.PopupException("BarConsumer [" + consumer + "] already registered for [" + channel + "]; returning");
			return false;
		} }
		public virtual bool ConsumerBarUnsubscribe(string symbol, BarScaleInterval scaleInterval,
											IStreamingConsumer consumer) { lock (this.lockConsumersBySymbol) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "Can't unregister BarConsumer [" + consumer + "]: symbol[" + symbol + "] is not registered for any consumers; returning";
				Assembler.PopupException(msg);
				return false;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> channelsByScaleInterval = this.DistributionChannels[symbol];
			if (channelsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string symbolDistributorsAsString = "";
				foreach (SymbolScaleDistributionChannel d in channelsByScaleInterval.Values) symbolDistributorsAsString += d + ",";
				symbolDistributorsAsString.TrimEnd(',');
				string msg = "Can't unregister BarConsumer [" + consumer + "]: scaleInterval[" + scaleInterval + "] not found among distributors [" + symbolDistributorsAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleDistributionChannel channel = channelsByScaleInterval[scaleInterval];
			if (channel.ConsumersBarContains(consumer) == false) {
				string msg = "Can't unregister BarConsumer [" + consumer + "]: consumer not found in [" + channel.ConsumersBarAsString + "]; returning";
				Assembler.PopupException(msg);
				return false;
			}
			channel.ConsumersBarRemove(consumer);
			if (channel.ConsumersBarCount == 0 && channel.ConsumersQuoteCount == 0) {
				//Assembler.PopupException("BarConsumer [" + consumer + "] was the last one using [" + symbol + "][" + scaleInterval + "]; removing QuoteBarDistributor[" + distributor + "]");
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
		public virtual bool ConsumerBarIsSubscribed(string symbol, BarScaleInterval scaleInterval_nullForAny, IStreamingConsumer consumer, bool addingTrue_checkingFalse = true) {
			bool ret = false;
			Dictionary<string, List<BarScaleInterval>> symbolsScaleIntervals = this.SymbolsScaleIntervalsBar_isConsumerRegistered(consumer, addingTrue_checkingFalse);
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

		public Dictionary<string, List<BarScaleInterval>> SymbolsScaleIntervalsQuote_isConsumerRegistered(IStreamingConsumer consumer, bool addingTrue_checkingFalse = true) {
			Dictionary<string, List<BarScaleInterval>> ret = null;
			foreach (string symbol in this.DistributionChannels.Keys) {
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> consumersByScaleInterval = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in consumersByScaleInterval.Keys) {
					SymbolScaleDistributionChannel consumers = consumersByScaleInterval[scaleInterval];
					if (consumers.ConsumersQuoteContains(consumer) == false) continue;
					if (ret == null) ret = new Dictionary<string, List<BarScaleInterval>>();
					if (addingTrue_checkingFalse) continue;
					if (ret.ContainsKey(symbol) == false) ret.Add(symbol, new List<BarScaleInterval>());
					ret[symbol].Add(scaleInterval);
				}
			}
			return ret;
		}
		public Dictionary<string, List<BarScaleInterval>> SymbolsScaleIntervalsBar_isConsumerRegistered(IStreamingConsumer consumer, bool addingTrue_checkingFalse = true) {
			Dictionary<string, List<BarScaleInterval>> ret = null;
			foreach (string symbol in this.DistributionChannels.Keys) {
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> consumersByScaleInterval = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in consumersByScaleInterval.Keys) {
					SymbolScaleDistributionChannel consumers = consumersByScaleInterval[scaleInterval];
					if (consumers.ConsumersBarContains(consumer) == false) continue;
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
			List<SymbolScaleDistributionChannel> channelsForSymbol = this.GetDistributionChannelsFor(quote.Symbol);
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
		public List<SymbolScaleDistributionChannel> GetDistributionChannelsFor(string symbol) {
			List<SymbolScaleDistributionChannel> distributors = new List<SymbolScaleDistributionChannel>();
			lock (this.lockConsumersBySymbol) {
				#if DEBUG
				if (this.DistributionChannels.ContainsKey(symbol) == false) {
					string msg = "YOU_DIDNT_SUBSCRIBE_AFTER_DISTRIBUTION_CHANNELS_CLEAR symbol[" + symbol + "] MOST_LIKELY_YOU_ABORTED_BACKTEST_BY_CHANGING_SELECTORS_IN_GUI_FIX_HANDLERS";
					return distributors;
				}
				#endif
				distributors = new List<SymbolScaleDistributionChannel>(this.DistributionChannels[symbol].Values);
			}
			return distributors;
		}

		public SymbolScaleDistributionChannel GetDistributionChannelForNullUnsafe(string symbol, BarScaleInterval barScaleInterval) {
			if (this.DistributionChannels.ContainsKey(symbol) == false) {
				string msg = "NO_SYMBOL_SUBSCRIBER DataDistributor[" + this + "].DistributionChannels.ContainsKey(" + symbol + ")=false";
				//Assembler.PopupException(msg);
				//throw new Exception(msg);
				return null;
			}
			Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> distributionChannels = this.DistributionChannels[symbol];
			if (distributionChannels.ContainsKey(barScaleInterval) == false) {
				string msg = "NO_SCALEINTERVAL_SUBSCRIBER DataDistributor[" + this
					+ "].DistributionChannels[" + symbol + "].ContainsKey(" + barScaleInterval + ")=false";
				Assembler.PopupException(msg);
				//this.StreamingAdapter.StatusReporter.PopupException(new Exception(msg));
				//throw new Exception(msg);
				return null;
			}
			return distributionChannels[barScaleInterval];
		}
		public override string ToString() {
			string ret = "";
			foreach (string symbol in this.DistributionChannels.Keys) {
				string consumers = "";
				Dictionary<BarScaleInterval, SymbolScaleDistributionChannel> distributionChannel = this.DistributionChannels[symbol];
				foreach (BarScaleInterval scaleInterval in distributionChannel.Keys) {
					if (consumers != "") consumers += ",";
					consumers += distributionChannel[scaleInterval];
				}
				ret += symbol + "{" + consumers + "}";
			}
			if (string.IsNullOrEmpty(ret)) ret = "NO_CONSUMERS_SUBSCRIBED";
			return ret;
		}

	}
}