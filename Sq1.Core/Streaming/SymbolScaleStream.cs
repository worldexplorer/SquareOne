using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public abstract partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD>
											  where STREAMING_CONSUMER_CHILD : StreamingConsumer {

					Type						ofWhatAmI;
		public		string						Symbol			{ get; protected set; }
		public		BarScaleInterval			ScaleInterval	{ get; protected set; }

		public		string										ReasonIwasCreated_propagatedFromDistributor;
		protected	SymbolChannel<STREAMING_CONSUMER_CHILD>		SymbolChannel;

		SymbolScaleStream() {
			ofWhatAmI			= typeof(STREAMING_CONSUMER_CHILD);
			LockConsumersQuote	= new object();
			LockConsumersBar	= new object();
			ConsumersQuote		= new List<STREAMING_CONSUMER_CHILD>();
			ConsumersBar		= new List<STREAMING_CONSUMER_CHILD>();
			//QuotePump = new QuotePump(this);
			// avoiding YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER
			// Assembler instantiates StreamingAdapters early enough so these horses 
			// NOPE_ON_APP_RESTART_BACKTESTER_COMPLAINS_ITS_ALREADY_PAUSED
			// moved BacktesterRunningAdd() to QuotePump.PushConsumersPaused = true;
		}
		public SymbolScaleStream(SymbolChannel<STREAMING_CONSUMER_CHILD> symbolChannel,
								string symbol, BarScaleInterval scaleInterval, string reasonIwasCreated = "REASON_UNKNOWN") : this() {
			this.SymbolChannel = symbolChannel;
			this.Symbol = symbol;
			this.ScaleInterval = scaleInterval;
			this.ReasonIwasCreated_propagatedFromDistributor = reasonIwasCreated;
		}

		public string SymbolScaleInterval { get { return "<" + this.ofWhatAmI.Name + "> " + this.Symbol + "_" + this.ScaleInterval; } }
		public override string ToString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsString + "],Bars[" + this.ConsumersBarAsString + "]"; }
		public string ToStringShort { get { return this.SymbolScaleInterval + "(b" + this.ConsumersBar.Count + "+" + this.ConsumersQuote.Count + "q)"; } }
		public string ToStringNames { get { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteNames + "],Bars[" + this.ConsumersBarNames + "]"; } }

		internal void UpstreamSubscribedToSymbol_pokeConsumers(string symbol) {
			foreach (STREAMING_CONSUMER_CHILD quoteConsumer	in this.ConsumersQuote)	quoteConsumer.UpstreamSubscribed_toSymbol_streamNotifiedMe(null);
			foreach (STREAMING_CONSUMER_CHILD barConsumer	in this.ConsumersBar)	  barConsumer.UpstreamSubscribed_toSymbol_streamNotifiedMe(null);
		}
		internal void UpstreamUnSubscribedFromSymbolPokeConsumers(string symbol, Quote quoteCurrent) {
			foreach (STREAMING_CONSUMER_CHILD quoteConsumer in this.ConsumersQuote) {
				quoteConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteCurrent);
			}
			foreach (STREAMING_CONSUMER_CHILD barConsumer in this.ConsumersBar) {
				if (barConsumer is StreamingConsumerSolidifier == false) {
					quoteCurrent.StreamingBar_Replace(barConsumer.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe);
				}
				barConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteCurrent);
			}
		}

		internal SymbolScaleStream<STREAMING_CONSUMER_CHILD> CloneFullyFunctional_withNewDictioniariesAndLists_toPossiblyRemoveMatchingConsumers() {
			//SymbolScaleStream<STREAMING_CONSUMER_CHILD> ret			= new SymbolScaleStream<STREAMING_CONSUMER_CHILD>();
			SymbolScaleStream<STREAMING_CONSUMER_CHILD> ret			= (SymbolScaleStream<STREAMING_CONSUMER_CHILD>) base.MemberwiseClone();
			ret.Symbol									= this.Symbol;
			ret.ScaleInterval							= this.ScaleInterval;
			//ret.UnattachedStreamingBar_factoryPerSymbolScale			= this.UnattachedStreamingBar_factoryPerSymbolScale;
			ret.ConsumersQuote							= new List<STREAMING_CONSUMER_CHILD>(this.ConsumersQuote);
			ret.ConsumersBar							= new List<STREAMING_CONSUMER_CHILD>(this.ConsumersBar);
			//ret.backtestersRunning_causingPumpingPause	= new List<Backtester>(this.backtestersRunning_causingPumpingPause);
			//ret.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim								= this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim;
			return ret;
		}
	}
}
