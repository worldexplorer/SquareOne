using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class SymbolScaleDistributionChannel {
		public string Symbol { get; protected set; }
		public BarScaleInterval ScaleInterval { get; protected set; }
		public StreamingBarFactoryUnattached StreamingBarFactoryUnattached { get; protected set; }
		Dictionary<IStreamingConsumer, StreamingEarlyBinder> earlyBinders;
		List<IStreamingConsumer> consumersQuote;
		List<IStreamingConsumer> consumersBar;
		object lockConsumersQuote;
		object lockConsumersBar;

		public SymbolScaleDistributionChannel() {
			lockConsumersQuote = new object();
			lockConsumersBar = new object();
			this.consumersQuote = new List<IStreamingConsumer>();
			this.consumersBar = new List<IStreamingConsumer>();
			earlyBinders = new Dictionary<IStreamingConsumer, StreamingEarlyBinder>();
		}
		public SymbolScaleDistributionChannel(string symbol, BarScaleInterval scaleInterval) : this() {
			Symbol = symbol;
			ScaleInterval = scaleInterval;
			StreamingBarFactoryUnattached = new StreamingBarFactoryUnattached(symbol, ScaleInterval);
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

			Quote quoteSernoEnrichedWithUnboundStreamingBar = this.StreamingBarFactoryUnattached.
				EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar(quote2bClonedForEachConsumer);
			if (quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == 0) {
				if (StreamingBarFactoryUnattached.LastBarFormedUnattached != null
						//&& double.IsNaN(StreamingBarFactoryUnattached.LastBarFormedUnattached.Close) == true
						&& this.StreamingBarFactoryUnattached.LastBarFormedUnattached.DateTimeOpen != DateTime.MinValue
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
						+ " for " + this.StreamingBarFactoryUnattached;
					//Assembler.PopupException(msg);
				}
			}
			lock (lockConsumersQuote) {
				this.bindStreamingBarForQuoteAndPushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar.Clone());
			}
		}

		void bindNewStreamingBarAppendPokeConsumersStaticFormed() {
			Bar barStreamingUnattached = this.StreamingBarFactoryUnattached.StreamingBarUnattached.Clone();
			if (this.consumersBar.Count == 0) {
				Assembler.PopupException("NO_BARS_CONSUMERS to push lastBarFormed[" + barStreamingUnattached.ToString() + "] SymbolScaleInterval["
					+ SymbolScaleInterval + "]; returning");
				return;
			}
			int consumerSerno = 1;
			int streamingSolidifiersPoked = 0;
			foreach (IStreamingConsumer consumer in this.consumersBar) {
				string msig = " missed barStreamingUnattached[" + barStreamingUnattached + "]: BarConsumer#" + (consumerSerno++) + "/" + this.consumersBar.Count + " " + consumer.ToString();

				#region SPECIAL_CASE_SINGLE_POSSIBLE_SOLIDIFIER_NO_EARLY_BINDING_NECESSARY
				if (consumer is StreamingSolidifier) {
					streamingSolidifiersPoked++;
					if (streamingSolidifiersPoked > 1) {
						string msg = "two streaming charts open with same Symbol/Interval, both with their StreamingSolidifiers subscribed"
							+ " but StreamingSolidifier Should be subscribed only once per Symbol/Interval, in StreamingProvider?..."
							+ " Save datasource must fully unregister consumers and register again to avoid StreamingSolidifier dupes";
						Assembler.PopupException(msg + msig);
						continue;
					}
					try {
						Bar lastBarFormedUnattached = this.StreamingBarFactoryUnattached.LastBarFormedUnattached.Clone();
						consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(lastBarFormedUnattached);
					} catch (Exception e) {
						string msg = "STREAMING_SOLIDIFIER_FAILED_TO_CONSUME_STATIC_JUST_FORMED";
						Assembler.PopupException(msg + msig, e);
					}
					continue;
				}
				#endregion

				if (consumer.ConsumerBarsToAppendInto == null) {
					try {
						//NOPE_FRESH_STREAMING_CONTAINING_JUST_ONE_QUOTE_I_WILL_POKE_QUOTES_FROM_IT consumer.ConsumeBarLastFormed(barLastFormedBound);
						consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached);
					} catch (Exception e) {
						string msg = "CHART_OR_BACKTESTER_WITHOUT_BARS_TO_BIND_AS_PARENT";
						Assembler.PopupException(msg + msig, e);
					}
					continue;
				}
				if (	consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe != null
					 && consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe.DateTimeOpen == barStreamingUnattached.DateTimeOpen) {
					string msg = "we are on 1st ever streaming quote: probably shouln't add it to avoid ALREADY_HAVE exception";
					Assembler.PopupException(msg + msig);
					continue;
				}

				if (this.earlyBinders.ContainsKey(consumer) == false) {
					string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
					Assembler.PopupException(msg + msig);
					continue;
				}

				StreamingEarlyBinder binder = this.earlyBinders[consumer];
				Bar barLastFormedBound = null;
				try {
					barLastFormedBound = binder.BindBarToConsumerBarsAndAppend(barStreamingUnattached);
				} catch (Exception e) {
					string msg = "BAR_BINDING_TO_PARENT_FAILED " + barLastFormedBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				try {
					consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe);
				} catch (Exception e) {
					string msg = "BOUND_BAR_PUSH_FAILED " + barLastFormedBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}
			}
		}
		void bindStreamingBarForQuoteAndPushQuoteToConsumers(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (this.consumersQuote.Count == 0) {
				Assembler.PopupException("Can't push quoteSernoEnriched[" + quoteSernoEnrichedWithUnboundStreamingBar + "]: no QuoteConsumers for symbol["
					+ Symbol + "] + scaleInterval[" + ScaleInterval + "]; returning");
				return;
			}
			int consumerSerno = 1;
			int streamingSolidifiersPoked = 0;
			foreach (IStreamingConsumer consumer in this.consumersQuote) {
				string msig = " missed quoteSernoEnrichedWithUnboundStreamingBar[" + quoteSernoEnrichedWithUnboundStreamingBar
					+ "]: QuoteConsumer#" + (consumerSerno++) + "/" + this.consumersQuote.Count + " " + consumer.ToString();

				#region SPECIAL_CASE_SINGLE_POSSIBLE_SOLIDIFIER_NO_EARLY_BINDING_NECESSARY
				if (consumer is StreamingSolidifier) {
					streamingSolidifiersPoked++;
					if (streamingSolidifiersPoked > 1) {
						string msg = "two streaming charts open with same Symbol/Interval, both with their StreamingSolidifiers subscribed"
							+ " but StreamingSolidifier Should be subscribed only once per Symbol/Interval, in StreamingProvider?...";
						Assembler.PopupException(msg + msig);
						continue;
					}
					try {
						consumer.ConsumeQuoteOfStreamingBar(quoteSernoEnrichedWithUnboundStreamingBar);
					} catch (Exception e) {
						string msg = "STREAMING_SOLIDIFIER_FAILED_CONSUME_STATIC_JUST_FORMED";
						Assembler.PopupException(msg + msig, e);
					}
					continue;
				}
				#endregion
	

				//if (consumer.ConsumerBarsToAppendInto == null) {
				//    try {
				//        consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached);
				//    } catch (Exception e) {
				//        string msg = "CHART_OR_BACKTESTER_WITHOUT_BARS_TO_BIND_AS_PARENT";
				//        Assembler.PopupException(msg + msig, e);
				//    }
				//    continue;
				//}
				//if (	consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe != null
				//     && consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe.DateTimeOpen == barStreamingUnattached.DateTimeOpen) {
				//    string msg = "we are on 1st ever streaming quote: probably shouln't add it to avoid ALREADY_HAVE exception";
				//    Assembler.PopupException(msg + msig);
				//    continue;
				//}

				if (this.earlyBinders.ContainsKey(consumer) == false) {
					string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
					Assembler.PopupException(msg + msig);
					continue;
				}

				StreamingEarlyBinder binder = this.earlyBinders[consumer];
				Quote quoteWithStreamingBarBound = null;
				try {
					quoteWithStreamingBarBound = binder.BindStreamingBarForQuote(quoteSernoEnrichedWithUnboundStreamingBar);
				} catch (Exception e) {
					string msg = "QUOTE_BINDING_TO_PARENT_STREAMIN_BAR_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				try {
					consumer.ConsumeQuoteOfStreamingBar(quoteWithStreamingBarBound);
				} catch (Exception e) {
					string msg = "BOUND_QUOTE_PUSH_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

			}
		}
		public string SymbolScaleInterval { get { return this.Symbol + "_" + this.ScaleInterval; } }
		public string ConsumersQuoteAsString { get { lock (lockConsumersQuote) {
					string ret = "";
					foreach (IStreamingConsumer consumer in this.consumersQuote) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersBarAsString { get { lock (lockConsumersBar) {
					string ret = "";
					foreach (IStreamingConsumer consumer in this.consumersBar) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public override string ToString() { return this.SymbolScaleInterval + ":Quotes[" + ConsumersQuoteAsString + "],Bars[" + ConsumersBarAsString + "]"; }

		public bool ConsumersQuoteContains(IStreamingConsumer consumer) { lock (lockConsumersQuote) { return this.consumersQuote.Contains(consumer); } }
		public void ConsumersQuoteAdd(IStreamingConsumer consumer) { lock (lockConsumersQuote) {
				this.consumersQuote.Add(consumer);
				if (earlyBinders.ContainsKey(consumer) == false) {
					earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
				}
			} }
		public void ConsumersQuoteRemove(IStreamingConsumer consumer) { lock (lockConsumersQuote) {
				this.consumersQuote.Remove(consumer);
				if (earlyBinders.ContainsKey(consumer) && this.consumersBar.Contains(consumer) == false) {
					earlyBinders.Remove(consumer);
				}
			} }
		public int ConsumersQuoteCount { get { lock (lockConsumersQuote) { return this.consumersQuote.Count; } } }

		public bool ConsumersBarContains(IStreamingConsumer consumer) { lock (lockConsumersBar) { return this.consumersBar.Contains(consumer); } }
		public void ConsumersBarAdd(IStreamingConsumer consumer) { lock (lockConsumersBar) {
				this.consumersBar.Add(consumer);
				if (earlyBinders.ContainsKey(consumer) == false) {
					earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
				}
			} }
		public void ConsumersBarRemove(IStreamingConsumer consumer) { lock (lockConsumersBar) {
				this.consumersBar.Remove(consumer);
				if (earlyBinders.ContainsKey(consumer) && this.consumersQuote.Contains(consumer) == false) {
					earlyBinders.Remove(consumer);
				}
			} }
		public int ConsumersBarCount { get { lock (lockConsumersBar) { return this.consumersBar.Count; } } }
	}
}
