using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class DataDistributorCharts : DataDistributor {
		public const string REASON_TO_EXIST = "I_ALLOW_MULTIPLE_CONSUMERS_FOR_SAME_SYMBOL:SCALEINTERVAL_PAIR__SUITABLE_FOR_MULTIPLE_CHARTS";
		public DataDistributorCharts(StreamingAdapter streamingAdapter, string reasonToExist) : base(streamingAdapter, "DataDistributorCharts-" + reasonToExist) {
		}

		public override bool ConsumerBarSubscribe(string symbol, BarScaleInterval scaleInterval, 
					StreamingConsumer chartConsumer, bool quotePumpSeparatePushingThreadEnabled) {
			bool ret = false;
			string msig = " //ChartConsumerBarSubscribe(" + symbol + ":" + scaleInterval + " [" + chartConsumer + "])";

			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "Failed to BarRegister(): scaleInterval.Scale=Unknown; returning";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return ret;
			}
			bool alreadyRegistered = base.ConsumerBarIsSubscribed(symbol, scaleInterval, chartConsumer);
			if (alreadyRegistered) {
				string msg = "BAR_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg, null, false);
				return ret;
			}

			ret = base.ConsumerBarSubscribe(symbol, scaleInterval, chartConsumer, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SUBSCRIBED_CHART_BARS[" + ret + "]";

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) {
				Assembler.PopupException(msg2 + msig, null, false);
			}

			return ret;
		}
		public override bool ConsumerBarUnsubscribe(string symbol, BarScaleInterval scaleInterval,
					StreamingConsumer chartConsumer) {
			string msig = " //ConsumerBarUnsubscribe(" + symbol + ":" + scaleInterval + " [" + chartConsumer + "])";
			bool ret = base.ConsumerBarUnsubscribe(symbol, scaleInterval, chartConsumer);
			string msg2 = "UNSUBSCRIBED_CHART_BARS[" + ret + "]";

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) {
				Assembler.PopupException(msg2 + msig, null, false);
			}
			return ret;
		}
		public override bool ConsumerQuoteSubscribe(string symbol, BarScaleInterval scaleInterval, 
					StreamingConsumer chartConsumer, bool quotePumpSeparatePushingThreadEnabled) {
			
			bool ret = false;
			string msig = " //ChartConsumerQuoteSubscribe(" + symbol + ":" + scaleInterval + " [" + chartConsumer + "])";

			bool alreadyRegistered = base.ConsumerQuoteIsSubscribed(symbol, scaleInterval, chartConsumer);
			if (alreadyRegistered) {
				string msg = "QUOTE_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			ret = base.ConsumerQuoteSubscribe(symbol, scaleInterval, chartConsumer, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SUBSCRIBED_CHART_QUOTES upstreamUnsubscribed[" + ret + "]";

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) {
				Assembler.PopupException(msg2 + msig, null, false);
			}

			base.StreamingAdapter.StreamingDataSnapshot.Initialize_levelTwo_forSymbol(symbol);
			return ret;
		}
		public override bool ConsumerQuoteUnsubscribe(string symbol, BarScaleInterval scaleInterval, 
					StreamingConsumer chartConsumer) {
						string msig = " //ConsumerQuoteUnsubscribe(" + symbol + ":" + scaleInterval + " [" + chartConsumer + "])";
			bool ret = base.ConsumerQuoteUnsubscribe(symbol, scaleInterval, chartConsumer);
			string msg2 = "UNSUBSCRIBED_CHART_QUOTES upstreamUnsubscribed[" + ret + "]";

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) {
				Assembler.PopupException(msg2 + msig, null, false);
			}

			base.StreamingAdapter.StreamingDataSnapshot.Initialize_levelTwo_forSymbol(symbol);

			return ret;
		}
	}
}
