using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleDistributionChannel {
		public	string							ReasonIwasCreated_propagatedFromDistributor;
		public	string							Symbol							{ get; protected set; }
		public	BarScaleInterval				ScaleInterval					{ get; protected set; }
		public	List<StreamingConsumer>			ConsumersQuote					{ get; protected set; }
		public	List<StreamingConsumer>			ConsumersBar					{ get; protected set; }

		public	StreamingBarFactoryUnattached	StreamingBarFactoryUnattached	{ get; protected set; }
				Dictionary<StreamingConsumer, StreamingLateBinder> binderPerConsumer;

				object							lockConsumersQuote;
				object							lockConsumersBar;
				List<Backtester>				backtestersRunningCausingPumpingPause;

		//NB#1	QuotePump.PushStraightOrBuffered replaced this.PushQuoteToConsumers to:
		//		1) set Streaming free without necessity to wait for Script.OnNewQuote/Bar and deliver the next quote ASAP;
		//		2) pause the Live trading and re-Backtest with new parameters imported from Sequencer, and continue Live with them (handling open positions at the edge NYI)
		//NB#2	QuotePump.PushConsumersPaused will freeze max all opened charts and one Solidifier per DataSource:Symbol:ScaleInterval;
		//		ability to control on per-consumer level costs more, including dissync between Solidifier.BarsStored and Executor.BarsInMemory
		public	QuoteQueuePerChannel			QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim	{ get; protected set; }
		public	QuotePumpPerChannel				QuotePump_nullUnsafe									{ get { return this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim as QuotePumpPerChannel; } }
		public	bool							ImQueueNotPump_trueOnlyForBacktest						{ get { return this.QuotePump_nullUnsafe == null; } }

		SymbolScaleDistributionChannel(string reasonIwasCreated = "REASON_UNKNOWN") {
			lockConsumersQuote	= new object();
			lockConsumersBar	= new object();
			ConsumersQuote		= new List<StreamingConsumer>();
			ConsumersBar		= new List<StreamingConsumer>();
			binderPerConsumer	= new Dictionary<StreamingConsumer, StreamingLateBinder>();
			backtestersRunningCausingPumpingPause = new List<Backtester>();
			//QuotePump = new QuotePump(this);
			// avoiding YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER
			// Assembler instantiates StreamingAdapters early enough so these horses 
			// NOPE_ON_APP_RESTART_BACKTESTER_COMPLAINS_ITS_ALREADY_PAUSED
			// moved BacktesterRunningAdd() to QuotePump.PushConsumersPaused = true;
			ReasonIwasCreated_propagatedFromDistributor = reasonIwasCreated;
		}
		public SymbolScaleDistributionChannel(string symbol, BarScaleInterval scaleInterval, bool quotePumpSeparatePushingThreadEnabled
						, string reasonIwasCreated) : this(reasonIwasCreated) {
			this.Symbol = symbol;
			this.ScaleInterval = scaleInterval;
			this.StreamingBarFactoryUnattached = new StreamingBarFactoryUnattached(symbol, ScaleInterval);

			//v1
			//// delayed start to 1) give BacktestStreaming-created channels to not start PumpThread (both architecturally and for linear call stack in debugger)
			//// 2) set Thread.CurrentThread.Name = QuotePump.channel.ToString() ( == this[SymbolScaleDistributionChannel].ToString), without subscribers it looks lame now
			//if (this.QuotePump.SeparatePushingThreadEnabled != quotePumpSeparatePushingThreadEnabled) {
			//	string msg = "I_MADE_IT_PROTECTED_TO_SET_THREAD_NAME_FOR_EASIER_DEBUGGING_IN_VISUAL_STUDIO";
			//	//this.QuotePump.SeparatePushingThreadEnabled  = quotePumpSeparatePushingThreadEnabled;
			//}
			//v2 : SET_THREAD_NAME_FOR_EASIER_DEBUGGING_IN_VISUAL_STUDIO 1) default constructor this() made private, 2) QuotePump knows if it should launch pusherEntryPoint() now
			//QuotePump = new QuotePumpPerChannel(this, quotePumpSeparatePushingThreadEnabled);
			//v3 UNMESSING_QuotePump
			if (quotePumpSeparatePushingThreadEnabled) {
				this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim = new QuotePumpPerChannel(this);
			} else {
				this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim = new QuoteQueuePerChannel(this);
			}
		}
		public void PushQuoteToPump(Quote quote2bClonedForEachConsumer) {
			if (string.IsNullOrEmpty(quote2bClonedForEachConsumer.Symbol)) {
				Assembler.PopupException("quote.Symbol[" + quote2bClonedForEachConsumer.Symbol + "] is null or empty, returning");
				return;
			}
			if (quote2bClonedForEachConsumer.Symbol != this.Symbol) {
				Assembler.PopupException("quote.Symbol[" + quote2bClonedForEachConsumer.Symbol + "] != this.Symbol[" + this.Symbol + "], returning");
				return;
			}

			Quote quoteSernoEnrichedWithUnboundStreamingBar = this.StreamingBarFactoryUnattached.
				EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar(quote2bClonedForEachConsumer);
			if (	quoteSernoEnrichedWithUnboundStreamingBar == null) {
				string msg = "I_REFUSE_TO_PUSH COULD_NOT_ENRICH_QUOTE quoteSernoEnrichedWithUnboundStreamingBar[null]"
					+ " quote2bClonedForEachConsumer[" + quote2bClonedForEachConsumer + "]"
					+ " this[" + this + "]"
					;
				//Assembler.PopupException(msg, null, false);
				return;
			}

			if (	quoteSernoEnrichedWithUnboundStreamingBar.ParentBarStreaming == null
				 || quoteSernoEnrichedWithUnboundStreamingBar.ParentBarStreaming.ParentBars == null) {
				string msg = "HERE_NULL_IS_OK___BINDER_WILL_BE_INVOKED_DOWNSTACK_SOON_FOR_QUOTE_CLONED StreamingEarlyBinder.BindStreamingBarForQuote()";
			}

			//v1 this.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
			//v2 let the user re-backtest during live streaming using 1) QuotePump.OnHold=true; 2) RunBacktest(); 3) QuotePump.OnHold=false;
			int straightOrBuffered = this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.PushStraightOrBuffered(quoteSernoEnrichedWithUnboundStreamingBar);
			if (this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread) {
				int pushedBuffered = straightOrBuffered;
			} else {
				int pushedStraight = straightOrBuffered;
			}

			if (	quoteSernoEnrichedWithUnboundStreamingBar.ParentBarStreaming == null
				 || quoteSernoEnrichedWithUnboundStreamingBar.ParentBarStreaming.ParentBars == null) {
				string msg = "HERE_NULL_IS_OK___BINDER_WILL_BE_INVOKED_DOWNSTACK_SOON_FOR_QUOTE_CLONED StreamingEarlyBinder.BindStreamingBarForQuote()";
			} else {
				string msg = "STREAMING_FOR_QUOTE_MUST_NOT_BE_INITIALIZED_HERE";
				//Assembler.PopupException(msg);
			}
		}
		public void PushQuoteToConsumers(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == -1) {
				string msg = "QUOTE_WAS_NOT_ENRICHED_BY_StreamingBarFactoryUnattached.EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar()";
				Assembler.PopupException(msg);
			}
	
			bool firstQuoteOfABar	= quoteSernoEnrichedWithUnboundStreamingBar.IntraBarSerno == 0;
			bool firstEverQuote		= quoteSernoEnrichedWithUnboundStreamingBar.AbsnoPerSymbol == 0;
			bool barLastStaticFormedIsValid		= !this.StreamingBarFactoryUnattached.BarLastFormedUnattachedNotYetFormed;
			bool streamingBarReadyToSpawn		=  this.StreamingBarFactoryUnattached.BarStreamingUnattached.DateTimeOpen
															< quoteSernoEnrichedWithUnboundStreamingBar.ServerTime;
			bool streamingBarWronglyRestoredAfterBacktest =
				this.StreamingBarFactoryUnattached.BarStreamingUnattached.DateTimeOpen
					> quoteSernoEnrichedWithUnboundStreamingBar.ServerTime;

			//if ((firstQuoteOfABar && firstEverQuote == false) || streamingBarWronglyRestoredAfterBacktest) {
			if (firstQuoteOfABar || streamingBarWronglyRestoredAfterBacktest) {
				if (streamingBarWronglyRestoredAfterBacktest) {
					string msg = "FORCING_EXECUTE_ON_LASTFORMED_BY_RESETTING_LAST_FORMED_TO_PREVIOUSLY_EXECUTED_AFTER_BACKTEST";
				}
				if (barLastStaticFormedIsValid
						//this.StreamingBarFactoryUnattached.BarLastFormedUnattached != null
						//&& double.IsNaN(StreamingBarFactoryUnattached.LastBarFormedUnattached.Close) == true
						//PAUSED_APPRESTART_BACKTEST_MinValue_HERE && this.StreamingBarFactoryUnattached.BarLastFormedUnattached.DateTimeOpen != DateTime.MinValue
					) {
					lock (this.lockConsumersBar) {
						this.bindConsumeLastStaticFormed(quoteSernoEnrichedWithUnboundStreamingBar);
					}
				} else {
					string msg = "I_REFUSE_BIND_AND_PUSH_LAST_STATIC_FORMED [" + this.StreamingBarFactoryUnattached.BarLastFormedUnattached_nullUnsafe + "]"
						//+ " I won't push LastStaticBar(DateTime.MinValue, NaN*5) because it has initialized LastStaticBar=StreamingBar.Clone()"
						+ " on first quoteSernoEnrichedWithUnboundStreamingBar[" + quoteSernoEnrichedWithUnboundStreamingBar + "]"
						+ " for " + this.StreamingBarFactoryUnattached;
					//Assembler.PopupException(msg, null, false);
				}
			}

			lock (this.lockConsumersQuote) {
				this.bindConsumeQuote(quoteSernoEnrichedWithUnboundStreamingBar);
			}
			//this.RaiseOnQuoteSyncPushedToAllConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
		}

		void bindConsumeLastStaticFormed(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			string msig = " //bindConsumeLastStaticFormed() " + this.ToString();
			Bar barStreamingUnattached = this.StreamingBarFactoryUnattached.BarStreamingUnattached.Clone();
			if (this.ConsumersBar.Count == 0) {
				string msg = "NO_BARS_CONSUMERS__NOT_PUSHING lastBarFormed[" + barStreamingUnattached + "]";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			int consumerSerno = 1;
			int streamingSolidifiersPoked = 0;
			foreach (StreamingConsumer barConsumer in this.ConsumersBar) {
				msig = " missed barStreamingUnattached[" + barStreamingUnattached + "]: BarConsumer#"
					+ (consumerSerno++) + "/" + this.ConsumersBar.Count + " " + barConsumer.ToString();

				#if DEBUG
				#region MOVED_TO_DataDistributorSolidifiers__SPECIAL_CASE_SINGLE_POSSIBLE_SOLIDIFIER_DOESNT_HAVE_ConsumerBarsToAppendInto__NO_EARLY_BINDING_NECESSARY
				//if (barConsumer is StreamingSolidifier) {
				//	streamingSolidifiersPoked++;
				//	if (streamingSolidifiersPoked > 1) {
				//		string msg = "two streaming charts open with same Symbol/Interval, both with their StreamingSolidifiers subscribed"
				//			+ " but StreamingSolidifier Should be subscribed only once per Symbol/Interval, in StreamingAdapter?..."
				//			+ " Save datasource must fully unregister consumers and register again to avoid StreamingSolidifier dupes";
				//		Assembler.PopupException(msg + msig);
				//		continue;
				//	}
				//	try {
				//		if (this.StreamingBarFactoryUnattached.BarLastFormedUnattachedNotYetFormed) {
				//			string msg = "NONSENSE_HERE";
				//			Assembler.PopupException(msg);
				//			continue;
				//		}
				//		Bar lastBarFormedUnattached = this.StreamingBarFactoryUnattached.BarLastFormedUnattached_nullUnsafe.Clone();
				//		barConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(lastBarFormedUnattached, null);
				//	} catch (Exception e) {
				//		string msg = "STREAMING_SOLIDIFIER_FAILED_TO_CONSUME_STATIC_JUST_FORMED";
				//		Assembler.PopupException(msg + msig, e);
				//	}
				//	continue;
				//}
				#endregion
				#endif

				if (barConsumer is StreamingSolidifier) {
					try {
						barConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached, quoteSernoEnrichedWithUnboundStreamingBar);
					} catch (Exception ex) {
						string msg = "BOUND_BAR_PUSH_FAILED " + barStreamingUnattached.ToString();
						Assembler.PopupException(msg + msig, ex);
					}
					continue;
				}
				
				if (barConsumer.ConsumerBarsToAppendInto == null) {
					//try {
					//	//NOPE_FRESH_STREAMING_CONTAINING_JUST_ONE_QUOTE_I_WILL_POKE_QUOTES_FROM_IT consumer.ConsumeBarLastFormed(barLastFormedBound);
					//	consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached);
					//} catch (Exception e) {
					//	string msg = "CHART_OR_BACKTESTER_WITHOUT_BARS_TO_BIND_AS_PARENT";
					//	Assembler.PopupException(msg + msig, e);
					//}
					string msg = "INVESTIGATE_THIS";
					Assembler.PopupException(msg + msig);
					continue;
				}
				if (	barConsumer.ConsumerBarsToAppendInto.BarStaticLast_nullUnsafe != null
					 && barConsumer.ConsumerBarsToAppendInto.BarStaticLast_nullUnsafe.DateTimeOpen == barStreamingUnattached.DateTimeOpen) {
					string msg = "KEEP_THIS_NOT_HAPPENING_BY_LEAVING_STATIC_LAST_ON_APPRESTART_NULL_ON_LIVEBACKTEST_CONTAINING_LAST_INCOMING_QUOTE"
						+ " we are on 1st ever streaming quote: probably shouln't add it to avoid ALREADY_HAVE exception";
					//Debugger.Break();
					Assembler.PopupException(msg + msig, null, false);
					continue;
				}

				if (this.binderPerConsumer.ContainsKey(barConsumer) == false) {
					string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
					Assembler.PopupException(msg + msig);
					continue;
				}

				StreamingLateBinder binderForConsumer = this.binderPerConsumer[barConsumer];
				Bar barStreamingAttached = null;
				try {
					barStreamingAttached = binderForConsumer.BindBar(barStreamingUnattached);
				} catch (Exception e) {
					string msg = "BAR_BINDING_TO_PARENT_FAILED " + barStreamingAttached.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}
				Quote quoteWithStreamingBarBound = null;
				try {
					quoteWithStreamingBarBound = binderForConsumer.BindQuote(quoteSernoEnrichedWithUnboundStreamingBar);	//.Clone()
				} catch (Exception e) {
					string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteWithStreamingBarBound.ToString();
					throw new Exception(msg, e);
				}

				try {
					Bar barStaticLast = barConsumer.ConsumerBarsToAppendInto.BarStaticLast_nullUnsafe;
					if (barStaticLast == null) {
						string msg = "THERE_IS_NO_STATIC_BAR_DURING_FIRST_4_QUOTES_GENERATED__ONLY_STREAMING";
						Assembler.PopupException(msg, null, false);
						continue;
					}
					barConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStaticLast, quoteWithStreamingBarBound);
				} catch (Exception ex) {
					string msg = "BOUND_BAR_PUSH_FAILED " + barStreamingAttached.ToString();
					Assembler.PopupException(msg + msig, ex);
				}
			}
		}
		public bool ImServingSolidifier { get { return this.ConsumersBarAsString.Contains("StreaminSolidifier"); } }
		void bindConsumeQuote(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			string msig = " //bindConsumeQuote() " + this.ToString();
			if (this.ConsumersQuote.Count == 0 && this.ImServingSolidifier == false) {
				string msg = "NO_QUOTE_CONSUMERS__NOT_PUSHING quoteSernoEnriched[" + quoteSernoEnrichedWithUnboundStreamingBar + "]";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			int consumerSerno = 1;
			int streamingSolidifiersPoked = 0;
			foreach (StreamingConsumer quoteConsumer in this.ConsumersQuote) {
				msig = " //bindConsumeQuote(): quoteSernoEnrichedWithUnboundStreamingBar[" + quoteSernoEnrichedWithUnboundStreamingBar.ToString()
					+ "]: QuoteConsumer#" + (consumerSerno++) + "/" + this.ConsumersQuote.Count + " " + quoteConsumer.ToString();

				if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) {
					string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
					Assembler.PopupException(msg + msig);
					continue;
				}
				StreamingLateBinder binderForConsumer = this.binderPerConsumer[quoteConsumer];

				#if DEBUG
				#region MOVED_TO_DataDistributorSolidifiers__SPECIAL_CASE_SINGLE_POSSIBLE_SOLIDIFIER_DOESNT_HAVE_ConsumerBarsToAppendInto__NO_EARLY_BINDING_NECESSARY
				//if (quoteConsumer is StreamingSolidifier) {
				//	streamingSolidifiersPoked++;
				//	if (streamingSolidifiersPoked > 1) {
				//		string msg = "two streaming charts open with same Symbol/Interval, both with their StreamingSolidifiers subscribed"
				//			+ " but StreamingSolidifier Should be subscribed only once per Symbol/Interval, in StreamingAdapter?...";
				//		Assembler.PopupException(msg + msig);
				//		continue;
				//	}
				//	try {
				//		//MADE_PUBLIC_TO_EXAMINE_IN_DEBUGGER binderForConsumer.StreamingBarFactoryUnattached;
				//		quoteSernoEnrichedWithUnboundStreamingBar.SetParentBarStreaming(binderForConsumer.StreamingBarFactoryUnattached.BarStreamingUnattached);
				//		quoteConsumer.ConsumeQuoteOfStreamingBar(quoteSernoEnrichedWithUnboundStreamingBar);
				//	} catch (Exception e) {
				//		string msg = "STREAMING_SOLIDIFIER_FAILED_CONSUME_STATIC_JUST_FORMED";
				//		Assembler.PopupException(msg + msig, e);
				//	}
				//	continue;
				//}
				#endregion
				#endif
				
				Quote quoteWithStreamingBarBound = null;
				try {
					quoteWithStreamingBarBound = binderForConsumer.BindQuote(quoteSernoEnrichedWithUnboundStreamingBar);
				} catch (Exception e) {
					string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}

				try {
					quoteConsumer.ConsumeQuoteOfStreamingBar(quoteWithStreamingBarBound);
					string msg = "CONSUMER_PROCESSED "
					//v1+ "#" + quoteWithStreamingBarBound.IntraBarSerno + "/" + quoteWithStreamingBarBound.AbsnoPerSymbol
						+ quoteWithStreamingBarBound.ToStringShort()
						+ " => " + quoteConsumer.ToString();
					//Assembler.PopupException(msg, null, false);
				} catch (Exception e) {
					string msg = "BOUND_QUOTE_PUSH_FAILED " + quoteWithStreamingBarBound.ToString();
					Assembler.PopupException(msg + msig, e);
					continue;
				}
			}
		}
		public string SymbolScaleInterval { get { return this.Symbol + "_" + this.ScaleInterval; } }

		public string ConsumersQuoteAsString { get { lock (this.lockConsumersQuote) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersQuote) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersBarAsString { get { lock (this.lockConsumersBar) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersBar) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersQuoteNames { get { lock (this.lockConsumersQuote) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersQuote) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }
		public string ConsumersBarNames { get { lock (this.lockConsumersBar) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersBar) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }

		public override string ToString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsString + "],Bars[" + this.ConsumersBarAsString + "]"; }
		public string ToStringShort { get { return this.SymbolScaleInterval + "(b" + this.ConsumersBar.Count + "+" + this.ConsumersQuote.Count + "q)"; } }
		public string ToStringNames { get { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteNames + "],Bars[" + this.ConsumersBarNames + "]"; } }
		public string ConsumerNames	{ get {
			List<StreamingConsumer>	merged = new List<StreamingConsumer>();
			merged.AddRange(this.ConsumersQuote);
			merged.AddRange(this.ConsumersBar);

			string ret = "";
			foreach (StreamingConsumer consumerEach in merged) {
				string reasonOrFullDump = string.IsNullOrEmpty(consumerEach.ReasonToExist) ? consumerEach.ToString() : consumerEach.ReasonToExist;
				if (string.IsNullOrEmpty(reasonOrFullDump)) continue;
				if (ret.Contains(reasonOrFullDump)) continue;
				if (ret != "") ret += ",";
				ret += reasonOrFullDump;
			}
			if (ret == "") ret = "NO_CONSUMERS";
			ret = this.ReasonIwasCreated_propagatedFromDistributor + " :: " + ret;
			ret += " " + this.SymbolScaleInterval;
			return ret;
		} }

		//public string ConsumersQuoteAsShortString { get { lock (this.lockConsumersQuote) {
		//			string ret = "";
		//			foreach (IStreamingConsumer consumer in this.ConsumersQuote) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.Na();
		//			}
		//			return ret;
		//		} } }
		//public string ConsumersBarAsShortString { get { lock (this.lockConsumersBar) {
		//			string ret = "";
		//			foreach (IStreamingConsumer consumer in this.ConsumersBar) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.ToString();
		//			}
		//			return ret;
		//		} } }
		//public override string ToShortString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsShortString + "],Bars[" + this.ConsumersBarAsShortString + "]"; }

		public bool ConsumersQuoteContains(StreamingConsumer consumer) { lock (this.lockConsumersQuote) { return this.ConsumersQuote.Contains(consumer); } }
		public void ConsumersQuoteAdd(StreamingConsumer consumer) { lock (this.lockConsumersQuote) {
				this.ConsumersQuote.Add(consumer);
				if (binderPerConsumer.ContainsKey(consumer)) return;
				binderPerConsumer.Add(consumer, new StreamingLateBinder(this.StreamingBarFactoryUnattached, consumer));
			} }
		public void ConsumersQuoteRemove(StreamingConsumer consumer) { lock (this.lockConsumersQuote) {
				this.ConsumersQuote.Remove(consumer);
				//if (earlyBinders.ContainsKey(consumer) && this.consumersBar.Contains(consumer) == false) {
				if (this.ConsumersBar.Contains(consumer)) return;
				if (binderPerConsumer.ContainsKey(consumer) == false) return;
				binderPerConsumer.Remove(consumer);
			} }
		public int ConsumersQuoteCount { get { lock (this.lockConsumersQuote) { return this.ConsumersQuote.Count; } } }

		public bool ConsumersBarContains(StreamingConsumer consumer) { lock (this.lockConsumersBar) { return this.ConsumersBar.Contains(consumer); } }
		public void ConsumersBarAdd(StreamingConsumer consumer) { lock (this.lockConsumersBar) {
				this.ConsumersBar.Add(consumer);
				if (binderPerConsumer.ContainsKey(consumer)) return;
				binderPerConsumer.Add(consumer, new StreamingLateBinder(this.StreamingBarFactoryUnattached, consumer));
			} }
		public void ConsumersBarRemove(StreamingConsumer consumer) { lock (this.lockConsumersBar) {
				this.ConsumersBar.Remove(consumer);
				//if (earlyBinders.ContainsKey(consumer) && this.consumersQuote.Contains(consumer) == false) {
				if (this.ConsumersQuote.Contains(consumer)) return;
				if (binderPerConsumer.ContainsKey(consumer) == false) return;
				binderPerConsumer.Remove(consumer);
			} }
		public int ConsumersBarCount { get { lock (this.lockConsumersBar) { return this.ConsumersBar.Count; } } }

		internal void UpstreamSubscribedToSymbolPokeConsumers(string symbol) {
			foreach (StreamingConsumer quoteConsumer in this.ConsumersQuote) quoteConsumer.UpstreamSubscribedToSymbolNotification(null);
			foreach (StreamingConsumer barConsumer in this.ConsumersBar) barConsumer.UpstreamSubscribedToSymbolNotification(null);
		}
		internal void UpstreamUnSubscribedFromSymbolPokeConsumers(string symbol, Quote lastQuoteReceived) {
			foreach (StreamingConsumer quoteConsumer in this.ConsumersQuote) {
				quoteConsumer.UpstreamUnSubscribedFromSymbolNotification(lastQuoteReceived);
			}
			foreach (StreamingConsumer barConsumer in this.ConsumersBar) {
				if (barConsumer is StreamingSolidifier == false) {
					lastQuoteReceived.SetParentBarStreaming(barConsumer.ConsumerBarsToAppendInto.BarStreaming_nullUnsafe);
				}
				barConsumer.UpstreamUnSubscribedFromSymbolNotification(lastQuoteReceived);
			}
		}

		public void QueueOrPumpPause_addBacktesterLaunchingScript_eachQuote(Backtester backtesterOrLivesimAdding) {	// POTENTINALLY_THREAD_UNSAFE lock(this.lockPump) {
			bool addedFirstBacktester = false;
			// #1/3 add to backtesters running me
			if (this.backtestersRunningCausingPumpingPause.Contains(backtesterOrLivesimAdding)) {
				string msg = "ADD_BACKTESTER_ONLY_ONCE [" + backtesterOrLivesimAdding + "]"
					+ " 1)YOU_INVOKE_Script.OnNewQuote()_WITHOUT_WAITING_FOR_IT_TO_FINISH_PREV_INVOCATION (TRYING_TO_ASSURE_NON_REENTERABILITY_OF_SCRIPT_HOOKS_HERE)"
					+ " 2)Script.OnNewQuote()_THREW_EXCEPTION_AND_YOU_DIDNT_CATCH_IT_AND_DIDNT_REMOVE_BACKTESTER_POSSIBLY_DIDNT_UNPAUSE";
				Assembler.PopupException(msg);
				return;
			} else {
				this.backtestersRunningCausingPumpingPause.Add(backtesterOrLivesimAdding);
				if (this.backtestersRunningCausingPumpingPause.Count == 1) {
					addedFirstBacktester = true;
				}
			}
			// #2/3 livesim => don't pause, just exit
			if (backtesterOrLivesimAdding is Livesimulator) {
				//if (this.ImPumpNotQueue) {
					string msg = "YOU_RUINED_THE_CONCEPT_OF_HAVING_LIVESIM_AS_A_TEST_FOR_LIVE_STREAMING"
						//+ " YOU_SUBSCRIBED_LIVESIM_BARS_IN_SINGLE_THREADED_QUEUE__MUST_BE_A_PUMP_JUST_LIKE_FOR_QUIK_STREAMING"
						+ " SimulationPreBarsSubstitute_overrideable()<=Livesimulator.cs:105"
						+ " ANYWAY_NO_NEED_TO_PAUSE_COMPETITORS NO_COMPETITORS_FOR_LIVESIM_BAR_EVENTS";
					Assembler.PopupException(msg);
				//}
				return;
			}
			// #3/3 it's a backtest => pause
			if (this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.Paused == true) {
				if (addedFirstBacktester) {
					string msg = "WE_ARE_ON_APP_RESTART_RIGHT??? ALL_PUMPS_ARE_BORN_PAUSED  backtesterAdding=[" + backtesterOrLivesimAdding + "]";
				} else {
					string msg = "PUMP_ALREADY_PAUSED_BY_ANOTHER_CONSUMERS_BACKTEST backtesterAdding=[" + backtesterOrLivesimAdding + "]";
					Assembler.PopupException(msg, null, false);
					//return;
				}
			} else {
				this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.PusherPause();
			}
		}
		public void QueueOrPumpResume_removeBacktesterFinishedScript_eachQuote(Backtester backtesterOrLivesimRemoving) {
			// #1/3 remove from backtesters running me, because backtest/livesim terminated anyway
			if (this.backtestersRunningCausingPumpingPause.Contains(backtesterOrLivesimRemoving)) {
				this.backtestersRunningCausingPumpingPause.Remove(backtesterOrLivesimRemoving);
			} else {
				string msg = "YOU_NEVER_ADDED_BACKTESTER [" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				//return;
			}
			// #2/3 livesim => don't unpause, just exit
			if (backtesterOrLivesimRemoving is Livesimulator) {
				//if (this.ImPumpNotQueue) {
					string msg = "YOU_RUINED_THE_CONCEPT_OF_HAVING_LIVESIM_AS_A_TEST_FOR_LIVE_STREAMING"
						//+ " YOU_SUBSCRIBED_LIVESIM_BARS_IN_SINGLE_THREADED_QUEUE__MUST_BE_A_PUMP_JUST_LIKE_FOR_QUIK_STREAMING"
						+ " SimulationPreBarsSubstitute_overrideable()<=Livesimulator.cs:105"
						+ " ANYWAY_NO_NEED_TO_UNPAUSE_COMPETITORS NO_COMPETITORS_FOR_LIVESIM_BAR_EVENTS";
					Assembler.PopupException(msg);
				//}
				return;
			}

			// #3/3 it's a backtest => pause
			if (this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.Paused == false) {
				string msg = "YOU_RUINED_WHOLE_IDEA_OF_DISTRIBUTOR_CHANNEL_TO_AUTORESUME_ITS_OWN_PUMP backtesterRemoving=[" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			if (this.backtestersRunningCausingPumpingPause.Count > 0) {
				string msg = "STILL_HAVE_ANOTHER_BACKTEST_RUNNING_IN_PARALLEL__WILL_UNPAUSE_PUMP_AFTER_LAST_TERMINATES"
					+ " backtestersRunningCausingPumpingPause.Count=[" + this.backtestersRunningCausingPumpingPause.Count + "]"
					+ " after backtesterRemoved[" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.PusherUnpause();
		}

		internal SymbolScaleDistributionChannel CloneFullyFunctional_withNewDictioniariesAndLists_toPossiblyRemoveMatchingConsumers() {
			SymbolScaleDistributionChannel ret			= new SymbolScaleDistributionChannel();
			ret.Symbol									= this.Symbol;
			ret.ScaleInterval							= this.ScaleInterval;
			ret.StreamingBarFactoryUnattached			= this.StreamingBarFactoryUnattached;
			ret.binderPerConsumer						= new Dictionary<StreamingConsumer, StreamingLateBinder>(this.binderPerConsumer);
			ret.ConsumersQuote							= new List<StreamingConsumer>(this.ConsumersQuote);
			ret.ConsumersBar							= new List<StreamingConsumer>(this.ConsumersBar);
			ret.backtestersRunningCausingPumpingPause	= new List<Backtester>(this.backtestersRunningCausingPumpingPause);
			ret.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim								= this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim;
			return ret;
		}
	}
}
