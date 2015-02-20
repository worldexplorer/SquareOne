using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class DataDistributorSolidifiers : DataDistributor {
		public const string REASON_TO_EXIST = "I_DONT_ALLOW_MULTIPLE_CONSUMERS_FOR_SAME_SYMBOL:SCALEINTERVAL_PAIR__SUITABLE_FOR_ONE_SOLIDIFIER_PER_SYMBOL";

		public DataDistributorSolidifiers(StreamingAdapter streamingAdapter) : base(streamingAdapter) {
		}

		public override bool ConsumerBarSubscribe(string symbol, BarScaleInterval scaleInterval,
										IStreamingConsumer solidifier, bool quotePumpSeparatePushingThreadEnabled) {
			bool ret = false;
			string msig = " //SolidifierConsumerBarSubscribe([" + symbol + "] [" + scaleInterval + "] [" + solidifier + "])";

			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "I_REFUSE_TO_SUBSCRIBE_TO_UNINITIALIZED_scaleInterval[" + scaleInterval + "]";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			bool alreadyRegistered = base.ConsumerBarIsSubscribed(symbol, scaleInterval, solidifier);
			if (alreadyRegistered) {
				string msg = "BAR_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg, null, false);
				return ret;
			}

			SymbolScaleDistributionChannel channelNullUnsafe = base.GetDistributionChannelForNullUnsafe(symbol, scaleInterval);
			if (channelNullUnsafe != null && channelNullUnsafe.ConsumersBarCount > 0) {
				string msg = "I_REFUSE_TO_REGISTER_MULTIPLE_SOLIDIFIERS_FOR_SAME_SYMBOL";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			ret = base.ConsumerBarSubscribe(symbol, scaleInterval, solidifier, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SOLIDIFIER_SUBSCRIBED_BARS[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);

			return ret;
		}
		public override bool ConsumerBarUnsubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer solidifier) {
			string msig = " //SolidifierConsumerBarUnsubscribe([" + symbol + "] [" + scaleInterval + "] [" + solidifier + "])";
			bool ret = base.ConsumerBarUnsubscribe(symbol, scaleInterval, solidifier);
			string msg2 = "SOLIDIFIER_UNSUBSCRIBED_BARS[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);
			return ret;
		}
		public override bool ConsumerQuoteSubscribe(string symbol, BarScaleInterval scaleInterval,
										IStreamingConsumer solidifier, bool quotePumpSeparatePushingThreadEnabled) {
			bool ret = false;
			string msig = " //SolidifierConsumerQuoteSubscribe(" + symbol + ":" + scaleInterval + "[" + solidifier + "])";

			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "I_REFUSE_TO_SUBSCRIBE_TO_UNINITIALIZED_scaleInterval[" + scaleInterval + "]";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			SymbolScaleDistributionChannel channelNullUnsafe = base.GetDistributionChannelForNullUnsafe(symbol, scaleInterval);
			if (channelNullUnsafe != null && channelNullUnsafe.ConsumersQuoteCount > 0) {
				string msg = "I_REFUSE_TO_REGISTER_MULTIPLE_SOLIDIFIERS_FOR_SAME_SYMBOL";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			bool alreadyRegistered = base.ConsumerQuoteIsSubscribed(symbol, scaleInterval, solidifier);
			if (alreadyRegistered) {
				string msg = "QUOTE_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			ret = base.ConsumerQuoteSubscribe(symbol, scaleInterval, solidifier, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SOLIDIFIER_SUBSCRIBED_QUOTES[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);

			return ret;
		}
		public override bool ConsumerQuoteUnsubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer solidifier) {
			string msig = " //SolidifierConsumerQuoteUnsubscribe(" + symbol + ":" + scaleInterval + "[" + solidifier + "])";
			bool ret = base.ConsumerQuoteUnsubscribe(symbol, scaleInterval, solidifier);
			string msg2 = "SOLIDIFIER_UNSUBSCRIBED_QUOTES[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);
			return ret;
		}
	}
}
