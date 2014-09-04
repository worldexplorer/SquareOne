using System;
using System.Collections.Generic;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;

namespace Sq1.Core.Streaming {
	public class SymbolScaleDistributionChannel {
		public string Symbol { get; protected set; }
		public BarScaleInterval ScaleInterval { get; protected set; }

		public StreamingBarFactoryUnattached StreamingBarFactoryUnattached { get; protected set; }
		Dictionary<IStreamingConsumer, StreamingEarlyBinder> earlyBinders;
		List<IStreamingConsumer> consumersQuote;
		List<IStreamingConsumer> consumersBar;
		Object lockConsumersQuote = new Object();
		Object lockConsumersBar = new Object();
		public SymbolScaleDistributionChannel(string symbol, BarScaleInterval scaleInterval) {
			Symbol = symbol;
			ScaleInterval = scaleInterval;
			StreamingBarFactoryUnattached = new StreamingBarFactoryUnattached(symbol, ScaleInterval);
			consumersQuote = new List<IStreamingConsumer>();
			consumersBar = new List<IStreamingConsumer>();
			earlyBinders = new Dictionary<IStreamingConsumer, StreamingEarlyBinder>();
		}
		public void PushQuoteToConsumers(Quote quote2bClonedForEachConsumer) {
			if (String.IsNullOrEmpty(quote2bClonedForEachConsumer.Symbol)) {
				Assembler.PopupException("quote.Symbol[" + quote2bClonedForEachConsumer.Symbol + "] is null or empty, returning");
				return;
			}
			if (quote2bClonedForEachConsumer.Symbol != this.Symbol) {
				Assembler.PopupException("quote.Symbol[" + quote2bClonedForEachConsumer.Symbol + "] != this.Symbol[" + this.Symbol + "], returning");
				return;
			}

			Quote quoteSernoEnrichedWithUnboundStreamingBar = StreamingBarFactoryUnattached.
				EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar(quote2bClonedForEachConsumer);
			if (quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == 0) {
				if (StreamingBarFactoryUnattached.LastBarFormedUnattached != null
						//&& Double.IsNaN(StreamingBarFactoryUnattached.LastBarFormedUnattached.Close) == true
						&& StreamingBarFactoryUnattached.LastBarFormedUnattached.DateTimeOpen != DateTime.MinValue
					) {
					lock (lockConsumersBar) {
						//try {
						this.bindNewStreamingBarAppendPokeConsumersStaticFormed();
						//} catch (Exception e) {
						//	string msg = "is this why barsSimulated.Count=6 while barOriginal=31/50??";
						//	throw new Exception(msg);
						//}
					}
				} else {
					string msg = "I won't push LastStaticBar(DateTime.MinValue, NaN*5) on first quote2bClonedForEachConsumer[" + quote2bClonedForEachConsumer + "]"
						+ " because it has initialized LastStaticBar=StreamingBar.Clone()"
						+ " for " + StreamingBarFactoryUnattached;
					//Assembler.PopupException(msg);
				}
			}
			lock (lockConsumersQuote) {
				this.bindStreamingBarForQuoteAndPushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar.Clone());
			}
		}

		void bindNewStreamingBarAppendPokeConsumersStaticFormed() {
			Bar barStreamingUnattached = StreamingBarFactoryUnattached.StreamingBarUnattached.Clone();
			if (consumersBar.Count == 0) {
				Assembler.PopupException("Can't push lastBarFormed[" + barStreamingUnattached + "]: no BarConsumers for SymbolScaleInterval["
					+ SymbolScaleInterval + "]; returning");
				return;
			}
			int consumerSerno = 1;
			int streamingSolidifiersPoked = 0;
			int backtestProvidersPoked = 0;
			foreach (IStreamingConsumer consumer in consumersBar) {
				string nth = "#" + (consumerSerno++) + "/" + consumersBar.Count;
				if (consumer is Sq1.Core.Backtesting.BacktestQuoteBarConsumer) {
					backtestProvidersPoked++;
				}
				if (consumer is StreamingSolidifier) {
					streamingSolidifiersPoked++;
					if (streamingSolidifiersPoked > 1) {
						string msg = "two streaming charts open with same Symbol/Interval, both with their StreamingSolidifiers subscribed"
							+ " but StreamingSolidifier Should be subscribed only once per Symbol/Interval, in StreamingProvider?...";
						continue;
					}
				}
				if (consumer.ConsumerBarsToAppendInto != null
					&& consumer.ConsumerBarsToAppendInto.BarStaticLast != null
					&& consumer.ConsumerBarsToAppendInto.BarStaticLast.DateTimeOpen == barStreamingUnattached.DateTimeOpen) {
					string msg = "we are on 1st ever streaming quote: probably shouln't add it to avoid ALREADY_HAVE exception";
					continue;
				}

				StreamingEarlyBinder binder = this.earlyBinders[consumer];
				Bar barLastFormedBound = binder.BindBarToConsumerBarsAndAppend(barStreamingUnattached);
				string msg1 = "barLastFormedBound[" + barLastFormedBound + "] pushing to " + nth + ": " + consumer
					//+ " <= barLastFormedUnattached[" + barLastFormedUnattached + "]"
					;
				//Assembler.PopupException(msg1);
				try {
					//NOPE_FRESH_STREAMING_CONTAINING_JUST_ONE_QUOTE_I_WILL_POKE_QUOTES_FROM_IT consumer.ConsumeBarLastFormed(barLastFormedBound);
					consumer.ConsumeBarLastStraticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(consumer.ConsumerBarsToAppendInto.BarStaticLast);
				} catch (Exception e) {
					string msg = "BarConsumer " + nth + ": missed bar [" + barStreamingUnattached + "]: " + consumer;
					throw new Exception(msg, e);
				}
			}
			if (backtestProvidersPoked == 0 && streamingSolidifiersPoked == 0
					&& barStreamingUnattached.ScaleInterval == new BarScaleInterval(BarScale.Minute, 1)) {
				string msg = "this bar wasn't saved in StaticProvider!!!";
				throw new Exception(msg);
			}
		}
		void bindStreamingBarForQuoteAndPushQuoteToConsumers(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (consumersQuote.Count == 0) {
				Assembler.PopupException("Can't push quoteSernoEnriched[" + quoteSernoEnrichedWithUnboundStreamingBar + "]: no QuoteConsumers for symbol["
					+ Symbol + "] + scaleInterval[" + ScaleInterval + "]; returning");
				return;
			}
			int consumerSerno = 1;
			foreach (IStreamingConsumer consumer in consumersQuote) {
				string nth = "#" + (consumerSerno++) + "/" + consumersQuote.Count + ": " + consumer;
				try {
					StreamingEarlyBinder binder = this.earlyBinders[consumer];
					Quote quoteWithStreamingBarBound = binder.BindStreamingBarForQuote(quoteSernoEnrichedWithUnboundStreamingBar);
					//if (consumer.ConsumerBarsToAppendInto != null) {
					//	string msg = "quoteSernoEnrichedWithUnboundStreamingBar[" + quoteSernoEnrichedWithUnboundStreamingBar
					//		+ "] cloned quoteWithStreamingBarBound[" + quoteWithStreamingBarBound + "]; pushing to " + nth;
					//	log.Debug(msg);
					//}
					consumer.ConsumeQuoteOfStreamingBar(quoteWithStreamingBarBound);
				} catch (Exception ex) {
					string msg = "QuoteConsumer " + nth + ": missed quoteSernoEnrichedWithUnboundStreamingBar["
						+ quoteSernoEnrichedWithUnboundStreamingBar + "] ";
					Assembler.PopupException(msg, ex);
				}
			}
		}
		public string SymbolScaleInterval { get { return this.Symbol + "_" + this.ScaleInterval; } }
		public string ConsumersQuoteAsString { get {
				string ret = "";
				lock (lockConsumersQuote) {
					foreach (IStreamingConsumer consumer in consumersQuote) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
				}
				return ret;
			} }
		public string ConsumersBarAsString { get {
				string ret = "";
				lock (lockConsumersBar) {
					foreach (IStreamingConsumer consumer in consumersBar) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
				}
				return ret;
			} }
		public override string ToString() {
			return this.SymbolScaleInterval + ":Quotes[" + ConsumersQuoteAsString + "],Bars[" + ConsumersBarAsString + "]";
		}

		public bool ConsumersQuoteContains(IStreamingConsumer consumer) {
			lock (lockConsumersQuote) {
				return consumersQuote.Contains(consumer);
			}
		}
		public void ConsumersQuoteAdd(IStreamingConsumer consumer) {
			lock (lockConsumersQuote) {
				consumersQuote.Add(consumer);
				if (earlyBinders.ContainsKey(consumer) == false) {
					earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
				}
			}
		}
		public void ConsumersQuoteRemove(IStreamingConsumer consumer) {
			lock (lockConsumersQuote) {
				consumersQuote.Remove(consumer);
				if (earlyBinders.ContainsKey(consumer) && consumersBar.Contains(consumer) == false) {
					earlyBinders.Remove(consumer);
				}
			}
		}
		public int ConsumersQuoteCount { get {
				lock (lockConsumersQuote) {
					return consumersQuote.Count;
				}
			} }

		public bool ConsumersBarContains(IStreamingConsumer consumer) {
			lock (lockConsumersBar) {
				return consumersBar.Contains(consumer);
			}
		}
		public void ConsumersBarAdd(IStreamingConsumer consumer) {
			lock (lockConsumersBar) {
				consumersBar.Add(consumer);
				if (earlyBinders.ContainsKey(consumer) == false) {
					earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
				}
			}
		}
		public void ConsumersBarRemove(IStreamingConsumer consumer) {
			lock (lockConsumersBar) {
				if (consumer is StaticProvider) {
					int a = 1;
				}
				consumersBar.Remove(consumer);
				if (earlyBinders.ContainsKey(consumer) && consumersQuote.Contains(consumer) == false) {
					earlyBinders.Remove(consumer);
				}
			}
		}
		public int ConsumersBarCount { get {
				lock (lockConsumersBar) {
					return consumersBar.Count;
				}
			} }
	}
}
