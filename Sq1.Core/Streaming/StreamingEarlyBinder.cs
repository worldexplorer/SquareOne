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
			Bar barAttached = consumer.ConsumerBarsToAppendInto.CreateNewOrAbsorbIntoStreaming(barDetached);
			return barAttached;
		}
		public Quote BindStreamingBarForQuote(Quote quoteSernoEnrichedWithStreamingBarUnattached) {
			if (consumer.ConsumerBarsToAppendInto == null) {
				// StreamingSolidifier will attach the Bar itself
				return quoteSernoEnrichedWithStreamingBarUnattached;
			}
//			if (consumer.ConsumerBarsToAppendInto.BarStreaming != null) {
//				// first four quotes there is no BarStreaming?.../
//				return quoteSernoEnrichedWithStreamingBarUnattached;
//			}
			
			//v1 consumer.ConsumerBarsToAppendInto.OverrideStreamingDOHLCVwith(quoteSernoEnrichedWithStreamingBarUnattached.ParentStreamingBar);
			consumer.ConsumerBarsToAppendInto.CreateNewOrAbsorbIntoStreaming(quoteSernoEnrichedWithStreamingBarUnattached.ParentStreamingBar);
			Quote quoteAttached = quoteSernoEnrichedWithStreamingBarUnattached.Clone();
			quoteAttached.SetParentBar(consumer.ConsumerBarsToAppendInto.BarStreaming);
			return quoteAttached;
		}
		public override string ToString() {
			return base.ToString() + ",ParentBars[" + consumer.ConsumerBarsToAppendInto + "]";
		}
	}
}