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

			LockConsumersQuote			= new object();
			LockConsumersBar			= new object();
			LockConsumersLevelTwoFrozen	= new object();
			ConsumersQuote				= new List<STREAMING_CONSUMER_CHILD>();
			ConsumersBar				= new List<STREAMING_CONSUMER_CHILD>();
			ConsumersLevelTwoFrozen		= new List<STREAMING_CONSUMER_CHILD>();
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
			foreach (STREAMING_CONSUMER_CHILD quoteConsumer		in this.ConsumersAll_avoidTriplication)	 quoteConsumer.UpstreamSubscribed_toSymbol_streamNotifiedMe(null);
		}
		internal void UpstreamUnSubscribedFromSymbol_pokeConsumers(string symbol, Quote quoteCurrent) {
			List<STREAMING_CONSUMER_CHILD> alreadyNotified_avoidInvokingThreeTimes = new List<STREAMING_CONSUMER_CHILD>();
			foreach (STREAMING_CONSUMER_CHILD barConsumer in this.ConsumersBar) {
				if (barConsumer is StreamingConsumerSolidifier == false) {
					// this is the only reason for this crazy alreadyNotified_avoidInvokingThreeTimes (simplicity ethalon is in this.UpstreamSubscribedToSymbol_pokeConsumers())
					quoteCurrent.StreamingBar_Replace(barConsumer.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe);
				}
				barConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteCurrent);
				alreadyNotified_avoidInvokingThreeTimes.Add(barConsumer);
			}
			foreach (STREAMING_CONSUMER_CHILD quoteConsumer in this.ConsumersQuote) {
				if (alreadyNotified_avoidInvokingThreeTimes.Contains(quoteConsumer)) continue;
				quoteConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteCurrent);
				alreadyNotified_avoidInvokingThreeTimes.Add(quoteConsumer);
			}
			foreach (STREAMING_CONSUMER_CHILD level2Consumer in this.ConsumersLevelTwoFrozen) {
				if (alreadyNotified_avoidInvokingThreeTimes.Contains(level2Consumer)) continue;
				level2Consumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteCurrent);
				alreadyNotified_avoidInvokingThreeTimes.Add(level2Consumer);
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
			ret.ConsumersLevelTwoFrozen					= new List<STREAMING_CONSUMER_CHILD>(this.ConsumersLevelTwoFrozen);
			//ret.backtestersRunning_causingPumpingPause	= new List<Backtester>(this.backtestersRunning_causingPumpingPause);
			//ret.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim								= this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim;
			return ret;
		}
	}
}
