using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public class SymbolScaleDistributionChannel {
		public	string							Symbol							{ get; protected set; }
		public	BarScaleInterval				ScaleInterval					{ get; protected set; }
		public	StreamingBarFactoryUnattached	StreamingBarFactoryUnattached	{ get; protected set; }
				Dictionary<IStreamingConsumer, StreamingEarlyBinder> earlyBinders;
				List<IStreamingConsumer>		consumersQuote;
				List<IStreamingConsumer>		consumersBar;
				object							lockConsumersQuote;
				object							lockConsumersBar;
				List<Backtester>				backtestersRunningCausingPumpingPause;

		//NB#1	QuotePump.PushStraightOrBuffered replaced this.PushQuoteToConsumers to:
		//		1) set Streaming free without necessity to wait for Script.OnNewQuote/Bar and deliver the next quote ASAP;
		//		2) pause the Live trading and re-Backtest with new parameters imported from Optimizer, and continue Live with them (handling open positions at the edge NYI)
		//NB#2	QuotePump.PushConsumersPaused will freeze max all opened charts and one Solidifier per DataSource:Symbol:ScaleInterval;
		//		ability to control on per-consumer level costs more, including dissync between Solidifier.BarsStored and Executor.BarsInMemory
		public	QuotePump						QuotePump						{ get; protected set; }

		public SymbolScaleDistributionChannel() {
			lockConsumersQuote = new object();
			lockConsumersBar = new object();
			consumersQuote = new List<IStreamingConsumer>();
			consumersBar = new List<IStreamingConsumer>();
			earlyBinders = new Dictionary<IStreamingConsumer, StreamingEarlyBinder>();
			backtestersRunningCausingPumpingPause = new List<Backtester>();
			QuotePump = new QuotePump(this);
			// avoiding YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER
			// Assembler instantiates StreamingProviders early enough so these horses 
			// NOPE_ON_APP_RESTART_BACKTESTER_COMPLAINS_ITS_ALREADY_PAUSED
			// moved BacktesterRunningAdd() to QuotePump.PushConsumersPaused = true;
		}
		public SymbolScaleDistributionChannel(string symbol, BarScaleInterval scaleInterval, bool quotePumpSeparatePushingThreadEnabled = true) : this() {
			Symbol = symbol;
			ScaleInterval = scaleInterval;
			StreamingBarFactoryUnattached = new StreamingBarFactoryUnattached(symbol, ScaleInterval);
			// delayed start to 1) give BacktestStreaming-created channels to not start PumpThread (both architecturally and for linear call stack in debugger)
			// 2) set Thread.CurrentThread.Name = QuotePump.channel.ToString() ( == this[SymbolScaleDistributionChannel].ToString), without subscribers it looks lame now
			if (this.QuotePump.SeparatePushingThreadEnabled != quotePumpSeparatePushingThreadEnabled) {
				this.QuotePump.SeparatePushingThreadEnabled  = quotePumpSeparatePushingThreadEnabled;
			}
		}
		public void PushQuoteToPump(Quote quote2bClonedForEachConsumer) {
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

			//v1 this.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
			//v2 let the user re-backtest during live streaming using 1) QuotePump.OnHold=true; 2) RunBacktest(); 3) QuotePump.OnHold=false;
			QuotePump.PushStraightOrBuffered(quoteSernoEnrichedWithUnboundStreamingBar);
		}
		[Obsolete("DANGER!!! QUOTE_MUST_BE_CLONED_AND_ENRICHED MAKE_SURE_this.PushQuoteToConsumers()_IS_INVOKED_THROUGH_PushQuoteToPump.PushStraightOrBuffered()_NOT_STRAIGHT_FROM_this.PushQuoteToDistributionChannels()")]
		public void PushQuoteToConsumers(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == -1) {
				string msg = "QUOTE_WAS_NOT_ENRICHED_BY_StreamingBarFactoryUnattached.EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar()";
				Assembler.PopupException(msg);
			}
			if (quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == 0) {
				if (StreamingBarFactoryUnattached.BarLastFormedUnattached != null
						//&& double.IsNaN(StreamingBarFactoryUnattached.LastBarFormedUnattached.Close) == true
						&& this.StreamingBarFactoryUnattached.BarLastFormedUnattached.DateTimeOpen != DateTime.MinValue
					) {
					lock (lockConsumersBar) {
						this.bindNewStreamingBarAppendPokeConsumersStaticFormed(quoteSernoEnrichedWithUnboundStreamingBar);
					}
				} else {
					string msg = "I won't push LastStaticBar(DateTime.MinValue, NaN*5) on first quoteSernoEnrichedWithUnboundStreamingBar["
						+ quoteSernoEnrichedWithUnboundStreamingBar + "]"
						+ " because it has initialized LastStaticBar=StreamingBar.Clone()"
						+ " for " + this.StreamingBarFactoryUnattached;
					//Assembler.PopupException(msg);
				}
			}
			lock (lockConsumersQuote) {
				this.bindStreamingBarForQuoteAndPushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar.Clone());
			}
		}

		void bindNewStreamingBarAppendPokeConsumersStaticFormed(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			Bar barStreamingUnattached = this.StreamingBarFactoryUnattached.BarStreamingUnattached.Clone();
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
						Bar lastBarFormedUnattached = this.StreamingBarFactoryUnattached.BarLastFormedUnattached.Clone();
						consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(lastBarFormedUnattached, null);
					} catch (Exception e) {
						string msg = "STREAMING_SOLIDIFIER_FAILED_TO_CONSUME_STATIC_JUST_FORMED";
						Assembler.PopupException(msg + msig, e);
					}
					continue;
				}
				#endregion

				if (consumer.ConsumerBarsToAppendInto == null) {
					//try {
					//    //NOPE_FRESH_STREAMING_CONTAINING_JUST_ONE_QUOTE_I_WILL_POKE_QUOTES_FROM_IT consumer.ConsumeBarLastFormed(barLastFormedBound);
					//    consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached);
					//} catch (Exception e) {
					//    string msg = "CHART_OR_BACKTESTER_WITHOUT_BARS_TO_BIND_AS_PARENT";
					//    Assembler.PopupException(msg + msig, e);
					//}
					string msg = "INVESTIGATE_THIS";
					Assembler.PopupException(msg + msig);
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
					if (barLastFormedBound != consumer.ConsumerBarsToAppendInto.BarLast) {
						string msg = "MUST_NEVER_HAPPEN_barLastFormedBound != consumer.ConsumerBarsToAppendInto.BarLast";
						Assembler.PopupException(msg + msig);
					}
					if (barLastFormedBound == consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe) {
						string msg = "MUST_NEVER_HAPPEN_barLastFormedBound == consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe";
						Assembler.PopupException(msg + msig);
					}
					if (barLastFormedBound != consumer.ConsumerBarsToAppendInto.BarStreaming) {
						string msg = "MUST_NEVER_HAPPEN_barLastFormedBound != consumer.ConsumerBarsToAppendInto.BarStreaming";
						Assembler.PopupException(msg + msig);
					}
				} catch (Exception e) {
					string msg = "BAR_BINDING_TO_PARENT_FAILED " + barLastFormedBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				Quote quoteWithStreamingBarBound = null;
				try {
					quoteWithStreamingBarBound = binder.BindStreamingBarForQuote(quoteSernoEnrichedWithUnboundStreamingBar);
				} catch (Exception e) {
					string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				try {
					Bar barStaticLast	= consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe;
					consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStaticLast, quoteWithStreamingBarBound);
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
				string msig = " //bindStreamingBarForQuoteAndPushQuoteToConsumers(): quoteSernoEnrichedWithUnboundStreamingBar[" + quoteSernoEnrichedWithUnboundStreamingBar.ToString()
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
					string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				try {
					consumer.ConsumeQuoteOfStreamingBar(quoteWithStreamingBarBound);
					string msg = "CONSUMER_PROCESSED "
					//v1+ "#" + quoteWithStreamingBarBound.IntraBarSerno + "/" + quoteWithStreamingBarBound.AbsnoPerSymbol
						+ quoteWithStreamingBarBound.ToStringShort()
						+ " => " + consumer.ToString();
					//Assembler.PopupException(msg, null, false);
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
				if (earlyBinders.ContainsKey(consumer)) return;
				earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
			} }
		public void ConsumersQuoteRemove(IStreamingConsumer consumer) { lock (lockConsumersQuote) {
				this.consumersQuote.Remove(consumer);
				//if (earlyBinders.ContainsKey(consumer) && this.consumersBar.Contains(consumer) == false) {
				if (this.consumersBar.Contains(consumer)) return;
				if (earlyBinders.ContainsKey(consumer) == false) return;
				earlyBinders.Remove(consumer);
			} }
		public int ConsumersQuoteCount { get { lock (lockConsumersQuote) { return this.consumersQuote.Count; } } }

		public bool ConsumersBarContains(IStreamingConsumer consumer) { lock (lockConsumersBar) { return this.consumersBar.Contains(consumer); } }
		public void ConsumersBarAdd(IStreamingConsumer consumer) { lock (lockConsumersBar) {
				this.consumersBar.Add(consumer);
				if (earlyBinders.ContainsKey(consumer)) return;
				earlyBinders.Add(consumer, new StreamingEarlyBinder(this.StreamingBarFactoryUnattached, consumer));
			} }
		public void ConsumersBarRemove(IStreamingConsumer consumer) { lock (lockConsumersBar) {
				this.consumersBar.Remove(consumer);
				//if (earlyBinders.ContainsKey(consumer) && this.consumersQuote.Contains(consumer) == false) {
				if (this.consumersQuote.Contains(consumer)) return;
				if (earlyBinders.ContainsKey(consumer) == false) return;
				earlyBinders.Remove(consumer);
			} }
		public int ConsumersBarCount { get { lock (lockConsumersBar) { return this.consumersBar.Count; } } }

		internal void UpstreamSubscribedToSymbolPokeConsumers(string symbol) {
			foreach (IStreamingConsumer quoteConsumer in this.consumersQuote) quoteConsumer.UpstreamSubscribedToSymbolNotification(null);
			foreach (IStreamingConsumer barConsumer in this.consumersBar) barConsumer.UpstreamSubscribedToSymbolNotification(null);
		}
		internal void UpstreamUnSubscribedFromSymbolPokeConsumers(string symbol, Quote lastQuoteReceived) {
			foreach (IStreamingConsumer quoteConsumer in this.consumersQuote) {
				quoteConsumer.UpstreamUnSubscribedFromSymbolNotification(lastQuoteReceived);
			}
			foreach (IStreamingConsumer barConsumer in this.consumersBar) {
				if (barConsumer is StreamingSolidifier == false) {
					lastQuoteReceived.SetParentBarStreaming(barConsumer.ConsumerBarsToAppendInto.BarStreaming);
				}
				barConsumer.UpstreamUnSubscribedFromSymbolNotification(lastQuoteReceived);
			}
		}

		public void PumpAutoPauseBacktesterLaunchingAdd(Backtester backtesterAdding) {	// POTENTINALLY_THREAD_UNSAFE lock(this.lockPump) {
			if (QuotePump.PushConsumersPaused == true) {
				string msg = "PUMP_ALREADY_PAUSED_BY_ANOTHER_CONSUMERS_BACKTEST__LAZY_TO_FIND_ROOT_CAUSE_KOZ_NOT_AN_ERROR backtesterAdding=[" + backtesterAdding + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}
			if (this.backtestersRunningCausingPumpingPause.Contains(backtesterAdding)) {
				string msg = "ADD_BACKTESTER_ONLY_ONCE [" + backtesterAdding + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.backtestersRunningCausingPumpingPause.Add(backtesterAdding);
			this.QuotePump.PushConsumersPaused = true;
		}
		public void PumpAutoResumeBacktesterCompleteRemove(Backtester backtesterRemoving) {
			if (this.QuotePump.PushConsumersPaused == false) {
				string msg = "YOU_RUINED_WHOLE_IDEA_OF_DISTRIBUTOR_CHANNEL_AUTORESUME_ITS_PUMP backtesterRemoving=[" + backtesterRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			if (this.backtestersRunningCausingPumpingPause.Contains(backtesterRemoving) == false) {
				string msg = "YOU_NEVER_ADDED_BACKTESTER [" + backtesterRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.backtestersRunningCausingPumpingPause.Remove(backtesterRemoving);
			if (this.backtestersRunningCausingPumpingPause.Count > 0) {
				string msg = "STILL_HAVE_OTHER_BACKTEST_RUNNING_IN_PARALLEL__WILL_UNPAUSE_PUMP_AFTER_LAST_TERMINATES"
					+ " backtestersRunningCausingPumpingPause.Count=[" + this.backtestersRunningCausingPumpingPause.Count + "]"
					+ " after backtesterRemoved[" + backtesterRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.QuotePump.PushConsumersPaused = false;
		}
	}
}
