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
		public Bar BarStreamingBindToConsumerBarsAndAppend(Bar barLastFormedUnattached) {
			if (this.consumer.ConsumerBarsToAppendInto == null) {
				// StreamingSolidifier will attach the Bar itself
				return barLastFormedUnattached;
			}
			Bar barDetached = barLastFormedUnattached.CloneDetached();
			Bar barStreamingBound = this.consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(barDetached);


			if (barStreamingBound != this.consumer.ConsumerBarsToAppendInto.BarLast) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound != consumer.ConsumerBarsToAppendInto.BarLast";
				throw new Exception(msg);
			}
			if (barStreamingBound == this.consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound == consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe";
				throw new Exception(msg);
			}
			if (barStreamingBound != this.consumer.ConsumerBarsToAppendInto.BarStreaming) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound != consumer.ConsumerBarsToAppendInto.BarStreaming";
				throw new Exception(msg);
			}
			return barStreamingBound;
		}
		public Quote BindStreamingBarForQuote(Quote quoteCloneSernoEnrichedFactoryUnattachedStreamingBar) {
			if (this.consumer.ConsumerBarsToAppendInto == null) {
				// StreamingSolidifier will attach the Bar itself
				return quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;
			}
//			if (this.consumer.ConsumerBarsToAppendInto.BarStreaming != null) {
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

			if (this.consumer.ConsumerBarsToAppendInto.BarStreaming == null) {
				string msg = "INITIALIZING_STREAMING_BAR_TO_NON_NULL_NEW_OR_LASTSTATIC"
					+ " FIRST_STREAMING_QUOTE_PER_BACKTEST_ON_STREAMINGLESS_BARS_JUST_FORKED_FROM_BARS_ORIGINAL_AT_BACKTEST_INITIALIZATION";
				//v1 I_LEFT_QUOTE_UNATTACHED_UPSTACK,ATTACHING_TO_FACTORY_HERE
				//v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
				Bar streamingCreatedUnattached = this.consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(this.streamingBarFactoryUnattached.BarStreamingUnattached);
				if (streamingCreatedUnattached != this.consumer.ConsumerBarsToAppendInto.BarStreaming) {
					string msg2 = "MUST_BE_THE_SAME_BAR PARANOID_CHECK";
					Assembler.PopupException(msg2);
				}
			} else {
				string msg = "ALL_OTHER_QUOTES_EXCEPT_FIRST_STREAMING_QUOTE_PER_BACKTEST";
				//v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingOverrideDOHLCVwith(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
				//v2 QUOTE_COMES_UNATTACHED_TAKE_STREAMING_HLCV_FROM_FACTORY
				if (quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.IntraBarSerno == 0) {
					string msg2 = "AVOIDING_EXCEPTION NO_NEED_TO_ABSORB_ANYTHING__DESTINATION_HasSameDOHLCV";
				} else {
					this.consumer.ConsumerBarsToAppendInto.BarStreamingOverrideDOHLCVwith(this.streamingBarFactoryUnattached.BarStreamingUnattached);
				}
			}

			//v2-WRONG Bar barAttached = consumer.ConsumerBarsToAppendInto.BarStreaming;
			//v2-WRONG quoteSernoEnrichedWithStreamingBarUnattached.ParentStreamingBar = barAttached;
			//v2-WRONG quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(consumer.ConsumerBarsToAppendInto.BarStreaming);

			//Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;	// already cloned upstack .Clone();
			Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.Clone();
			quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(this.consumer.ConsumerBarsToAppendInto.BarStreaming);
			return quoteAttachedToStreamingAttachedToConsumerBars;
		}
		public override string ToString() {
			return "BarFactory[" + this.streamingBarFactoryUnattached.ToString() + "]"
				+ " for ConsumerBars[" + this.consumer.ConsumerBarsToAppendInto + "]";
		}
	}
}