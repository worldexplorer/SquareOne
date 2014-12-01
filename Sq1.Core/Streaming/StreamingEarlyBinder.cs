using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingEarlyBinder {
		StreamingBarFactoryUnattached streamingBarFactoryUnattached;
		IStreamingConsumer consumer;

		public StreamingEarlyBinder(StreamingBarFactoryUnattached streamingBarFactoryUnattached, IStreamingConsumer consumer) {
			this.streamingBarFactoryUnattached = streamingBarFactoryUnattached;
			this.consumer = consumer;
		}
		public Bar BindBarToConsumerBarsAndAppend(Bar barLastFormedUnattached) {
			if (consumer.ConsumerBarsToAppendInto == null) {
				// StreamingSolidifier will attach the Bar itself
				return barLastFormedUnattached;
			}
			Bar barDetached = barLastFormedUnattached.CloneDetached();
			Bar barAttached = consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(barDetached);
			return barAttached;
		}
		public Quote BindStreamingBarForQuote(Quote quoteCloneSernoEnrichedFactoryUnattachedStreamingBar) {
			if (consumer.ConsumerBarsToAppendInto == null) {
				// StreamingSolidifier will attach the Bar itself
				return quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;
			}
//			if (consumer.ConsumerBarsToAppendInto.BarStreaming != null) {
//				// first four quotes there is no BarStreaming?.../
//				return quoteSernoEnrichedWithStreamingBarUnattached;
//			}

			// each consumer has different Bars (LoadedAll or LoadedFromTill);
			// BindBarToConsumerBarsAndAppend will create new BarStreaming after 1000 of this methods invocation terminate;
			// right now quote has had ParentStreamingBar assigned to streamingBarFactoryUnattached.BarStreaming on ScaleInterval basis (one factory for many 5-min consumers);
			// 1) I clone the quote to leave it untouched for other consumers and I attach it to customers' BarStreaming
			//		(done upstack in SymbolScaleDistributionChannel.bindStreamingBarForQuoteAndPushQuoteToConsumers())
			// 2) I get the customers' BarStreaming and update its DOHLCV
			//v1

//			if (consumer.ConsumerBarsToAppendInto.BarStreaming == null) {
				string msg = "WHEN_SHOULD_I_ASSIGN_BarStreaming_AND_TO_WHAT?... FIRST_STREAMING_QUOTE_EVER_WILL_ADD_NEW_BAR_DOWNSTACK_I_CANT_KNOW_IT_HERE";
				consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
//			} else {
//				consumer.ConsumerBarsToAppendInto.BarStreamingOverrideDOHLCVwith(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
//			}

			//v2-WRONG Bar barAttached = consumer.ConsumerBarsToAppendInto.BarStreaming;
			//v2-WRONG quoteSernoEnrichedWithStreamingBarUnattached.ParentStreamingBar = barAttached;
			//v2-WRONG quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(consumer.ConsumerBarsToAppendInto.BarStreaming);

			//Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;	// already cloned upstack .Clone();
			Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.Clone();
			quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(consumer.ConsumerBarsToAppendInto.BarStreaming);
			return quoteAttachedToStreamingAttachedToConsumerBars;
		}
		public override string ToString() {
			return base.ToString() + ",ParentBars[" + consumer.ConsumerBarsToAppendInto + "]";
		}
	}
}