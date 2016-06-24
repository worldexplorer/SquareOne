using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class DistributorSolidifier : Distributor<StreamingConsumerSolidifier> {
		public const string REASON_TO_EXIST = "I_DONT_ALLOW_MULTIPLE_CONSUMERS_FOR_SAME_SYMBOL:SCALEINTERVAL_PAIR__SUITABLE_FOR_ONE_SOLIDIFIER_PER_SYMBOL";
		public DistributorSolidifier(StreamingAdapter streamingAdapter, string reasonToExist)
							   : base(streamingAdapter, Distributor<StreamingConsumerSolidifier>.SOLIDIFIERS_FOR + " " + reasonToExist) { }

		public override bool ConsumerBarSubscribe_solidifiers(string symbol, BarScaleInterval scaleInterval,
										StreamingConsumerSolidifier solidifier, bool quotePumpSeparatePushingThreadEnabled) {
			bool ret = false;
			string msig = " //SolidifierConsumerBarSubscribe([" + symbol + "] [" + scaleInterval + "] [" + solidifier + "])";

			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "I_REFUSE_TO_SUBSCRIBE_TO_UNINITIALIZED_scaleInterval[" + scaleInterval + "]";
				Assembler.PopupException(msg + msig);
				return ret;
			}

			bool alreadyRegistered = base.ConsumerBarIsSubscribed_solidifiers(symbol, scaleInterval, solidifier);
			if (alreadyRegistered) {
				string msg = "BAR_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg, null, false);
				return ret;
			}

			SymbolScaleStream<StreamingConsumerSolidifier> channel_nullUnsafe = base.GetSymbolScaleStreamFor_nullUnsafe(symbol, scaleInterval);
			if (channel_nullUnsafe != null && channel_nullUnsafe.ConsumersBarCount > 0) {
				string msg = "I_REFUSE_TO_REGISTER_MULTIPLE_SOLIDIFIERS_FOR_SAME_SYMBOL";
				Assembler.PopupException(msg + msig);
				return ret;
			}

			ret = base.ConsumerBarSubscribe_solidifiers(symbol, scaleInterval, solidifier, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SOLIDIFIER_SUBSCRIBED_BARS[" + ret + "]";
			//Assembler.PopupException(msg2 + msig, null, false);

			return ret;
		}
		#if DEBUG
		public override bool ConsumerBarUnsubscribe_solidifiers(string symbol, BarScaleInterval scaleInterval, StreamingConsumerSolidifier solidifier) {
			string msig = " //SolidifierConsumerBarUnsubscribe([" + symbol + "] [" + scaleInterval + "] [" + solidifier + "])";
			bool ret = base.ConsumerBarUnsubscribe_solidifiers(symbol, scaleInterval, solidifier);
			string msg2 = "SOLIDIFIER_UNSUBSCRIBED_BARS[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);
			return ret;
		}
		#endif
		public override bool ConsumerQuoteSubscribe_solidifiers(string symbol, BarScaleInterval scaleInterval,
										StreamingConsumerSolidifier solidifier, bool quotePumpSeparatePushingThreadEnabled) {
			bool ret = false;
			string msig = " //SolidifierConsumerQuoteSubscribe(" + symbol + ":" + scaleInterval + "[" + solidifier + "])";

			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "I_REFUSE_TO_SUBSCRIBE_TO_UNINITIALIZED_scaleInterval[" + scaleInterval + "]";
				Assembler.PopupException(msg + msig);
				return ret;
			}

			SymbolScaleStream<StreamingConsumerSolidifier> channel_nullUnsafe = base.GetSymbolScaleStreamFor_nullUnsafe(symbol, scaleInterval);
			if (channel_nullUnsafe != null && channel_nullUnsafe.ConsumersQuoteCount > 0) {
				string msg = "I_REFUSE_TO_REGISTER_MULTIPLE_SOLIDIFIERS_FOR_SAME_SYMBOL";
				Assembler.PopupException(msg + msig);
				return ret;
			}

			bool alreadyRegistered = base.ConsumerQuoteIsSubscribed_solidifiers(symbol, scaleInterval, solidifier);
			if (alreadyRegistered) {
				string msg = "QUOTE_CONSUMER_ALREADY_REGISTERED";
				Assembler.PopupException(msg + msig, null, false);
				return ret;
			}

			ret = base.ConsumerQuoteSubscribe_solidifiers(symbol, scaleInterval, solidifier, quotePumpSeparatePushingThreadEnabled);
			string msg2 = "SOLIDIFIER_SUBSCRIBED_QUOTES[" + ret + "]";
			//Assembler.PopupException(msg2 + msig, null, false);

			return ret;
		}
		#if DEBUG
		public override bool ConsumerQuoteUnsubscribe_solidifiers(string symbol, BarScaleInterval scaleInterval, StreamingConsumerSolidifier solidifier) {
			string msig = " //SolidifierConsumerQuoteUnsubscribe(" + symbol + ":" + scaleInterval + "[" + solidifier + "])";
			bool ret = base.ConsumerQuoteUnsubscribe_solidifiers(symbol, scaleInterval, solidifier);
			string msg2 = "SOLIDIFIER_UNSUBSCRIBED_QUOTES[" + ret + "]";
			Assembler.PopupException(msg2 + msig, null, false);
			return ret;
		}
		#endif
	}
}
