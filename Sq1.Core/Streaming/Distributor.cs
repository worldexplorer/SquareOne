using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class Distributor<STREAMING_CONSUMER_CHILD> where STREAMING_CONSUMER_CHILD : StreamingConsumer {
		public const string LIVE_CHARTS_FOR				= "LIVE_CHARTS_FOR";
		public const string SOLIDIFIERS_FOR				= "SOLIDIFIERS_FOR";
		public const string SUBSTITUTED_LIVESIM_STARTED	= "SUBSTITUTED_LIVESIM_STARTED";

					object				lockConsumersBySymbol;
		public		StreamingAdapter	StreamingAdapter 	{ get; protected set; }
		public		string				ReasonIwasCreated 	{ get; protected set; }
		public		int					InstanceSerno	 	{ get; protected set; }

		Distributor(string reasonIwasCreated) {
			//DistributionChannels	= new Dictionary<string, Dictionary<BarScaleInterval, SymbolScaleDistributionChannel>>();
			ChannelsBySymbol		= new Dictionary<string, SymbolChannel<STREAMING_CONSUMER_CHILD>>();
			lockConsumersBySymbol	= new object();
			ReasonIwasCreated		= reasonIwasCreated;
		}
		public Distributor(StreamingAdapter streamingAdapter, string reasonIwasCreated) : this(reasonIwasCreated) {
			this.StreamingAdapter = streamingAdapter;
			//this.ReasonIwasCreated = this.StreamingAdapter + ":" + this.ReasonIwasCreated;
			this.storeAllInstancesEverCreated(streamingAdapter);
			var keepNecessaryOnly = Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated;
			InstanceSerno = Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated_total;
		}


		public virtual void Push_quoteUnboundUnattached_toChannel(Quote quoteUnboundUnattached) {
			if (string.IsNullOrEmpty(quoteUnboundUnattached.Symbol)) {
				Assembler.PopupException("quote[" + quoteUnboundUnattached + "]'se Symbol is null or empty, returning");
				return;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.GetChannelFor_nullMeansWasntSubscribed(quoteUnboundUnattached.Symbol);
			if (channel == null) {
				string msg = "I_REFUSE_TO_PUSH_QUOTE_FOR_UNSUBSCRIBED_SYMBOL quoteUnboundUnattached.Symbol[" + quoteUnboundUnattached.Symbol + "]"
					+ " DO_YOU_PUSH_QUOTE_TO_DISTRIB_SOLIDIFIERS_THAT_IS_EMPTY_DURING_LIVESIM???";
				Assembler.PopupException(msg, null, false);
				return;
			}
			// don't clone quote here!! IntraBarSerno++ for each StreamingCosumer() in each channel happens downstack
			// 1) distributor pushes quote to each Channel,
			// 2) channel pushes to Pump and unblocks the streaming to return right after (strategies are executed in Pump's thread later SYNCed),
			//		that's where clone happens and StreamingBinder.QuoteBind() to BarStreaming
			//		main idea behind StreamingConsumers is to hold its own bars and bind StreamingBarUnattached.Clone() to quote.Clone(), and then that StreamingAttached into Consumers.Bars
			//		the term "BOUND" applies to QUOTE, "ATTACHED" applies to StreamingBar
			// 3) receiving PumpPerChannel Thread pushes to each Consumer
			channel.PushQuote_viaPumpOrQueue(quoteUnboundUnattached);
			//this.RaiseOnQuoteAsyncPushedToAllDistributionChannels(quote);
		}


		public override string ToString() {
			string ret = this.toStringCommon(false);
			return ret;
		}

		public string ToStringNames { get {
			string ret = this.toStringCommon(true);
			return ret;
		} }

		string toStringCommon(bool consumerNamesOnly = false) {
			string ret = "#" + this.InstanceSerno + " " + this.ReasonIwasCreated + " ForStreamingAdapter[" + this.StreamingAdapter.Name + "]: ";
			foreach (string symbol in this.ChannelsBySymbol.Keys) {
				string consumers = "";
				SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbol];
				foreach (BarScaleInterval scaleInterval in channel.StreamsByScaleInterval.Keys) {
					if (consumers != "") consumers += ",";
					SymbolScaleStream<STREAMING_CONSUMER_CHILD> stream = channel.StreamsByScaleInterval[scaleInterval];
					consumers += consumerNamesOnly ? stream.ToStringNames : channel.ToString();
				}
				ret += symbol + "{" + consumers + "}";
			}
			if (string.IsNullOrEmpty(ret)) ret = "NO_CONSUMERS_SUBSCRIBED";
			return ret;
		}
	}
}