using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingLateBinder {
		public	StreamingBarFactoryUnattached	StreamingBarFactoryUnattached	{ get; private set;}
		public	IStreamingConsumer				Consumer						{ get; private set;}

		public StreamingLateBinder(StreamingBarFactoryUnattached streamingBarFactoryUnattached, IStreamingConsumer consumer) {
			this.StreamingBarFactoryUnattached = streamingBarFactoryUnattached;
			this.Consumer = consumer;
		}
		public Bar BindBar(Bar barLastFormedUnattached) {
			Bars bars = this.Consumer.ConsumerBarsToAppendInto;
			if (bars == null) {
				if ((this.Consumer is StreamingSolidifier) == false) {
					string msg = "CHECK_UPSTACK_WHETHER_CONSUMER_BARS_ARE_NULL StreamingSolidifier will attach the Bar itself";
					Assembler.PopupException(msg);
				}
				return barLastFormedUnattached;
			}
			//Bar barDetached = barLastFormedUnattached.CloneDetached();
			//Bar barStreamingBound = bars.BarStreamingCreateNewOrAbsorb(barDetached);
			Bar barStreamingBound = bars.BarStreamingCreateNewOrAbsorb(barLastFormedUnattached);

			if (barStreamingBound != bars.BarLast) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound != consumer.ConsumerBarsToAppendInto.BarLast";
				throw new Exception(msg);
			}
			if (barStreamingBound == bars.BarStaticLastNullUnsafe) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound == consumer.ConsumerBarsToAppendInto.BarStaticLastNullUnsafe";
				throw new Exception(msg);
			}
			if (barStreamingBound != bars.BarStreamingNullUnsafe) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingBound != consumer.ConsumerBarsToAppendInto.BarStreaming";
				throw new Exception(msg);
			}
			return barStreamingBound;
		}
		public Quote BindQuote(Quote quoteCloneSernoEnrichedFactoryUnattachedStreamingBar) {
			if (this.Consumer is StreamingSolidifier) {
				string msg = "StreamingSolidifier will attach Quote to the Bar itself";
				//Assembler.PopupException(msg);
				return quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;
			}

			Bars bars = null;
			try {
				bars = this.Consumer.ConsumerBarsToAppendInto;
			} catch (Exception ex) {
				string msg = "CONSUMER_DIDNT_PROVIDE_BARS_TO_APPEND_INTO this.Consumer[" + this.Consumer + "]";
				Assembler.PopupException(msg);
				return quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;
			}
			if (bars == null) {
				string msg = "BARS_MUST_NOT_BE_NULL CANT_BIND_QUOTE CHECK_UPSTACK";
				Assembler.PopupException(msg);
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

			if (bars.BarStreamingNullUnsafe == null) {
				string msg = "INITIALIZING_STREAMING_BAR_TO_NON_NULL_NEW_OR_LASTSTATIC"
					+ " FIRST_STREAMING_QUOTE_PER_BACKTEST_ON_STREAMINGLESS_BARS_JUST_FORKED_FROM_BARS_ORIGINAL_AT_BACKTEST_INITIALIZATION";
				//v1 I_LEFT_QUOTE_UNATTACHED_UPSTACK,ATTACHING_TO_FACTORY_HERE
				//v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
				Bar streamingCreatedUnattached = bars.BarStreamingCreateNewOrAbsorb(this.StreamingBarFactoryUnattached.BarStreamingUnattached);
				if (streamingCreatedUnattached != bars.BarStreamingNullUnsafe) {
					string msg2 = "MUST_BE_THE_SAME_BAR PARANOID_CHECK";
					Assembler.PopupException(msg2);
				}
			} else {
				string msg = "ALL_OTHER_QUOTES_EXCEPT_FIRST_STREAMING_QUOTE_PER_BACKTEST";
				//v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingOverrideDOHLCVwith(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
				//v2 QUOTE_COMES_UNATTACHED_TAKE_STREAMING_HLCV_FROM_FACTORY
				if (quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.IntraBarSerno == 0) {
					string msg2 = "AVOIDING_EXCEPTION NO_NEED_TO_ABSORB_ANYTHING__DESTINATION_HasSameDOHLCV_KOZ_BAR_FACTORY_JUST_STARTED_NEW_STREAMING_BAR";
				} else {
					bars.BarStreamingOverrideDOHLCVwith(this.StreamingBarFactoryUnattached.BarStreamingUnattached);
				}
			}

			//v2-WRONG Bar barAttached = consumer.ConsumerBarsToAppendInto.BarStreaming;
			//v2-WRONG quoteSernoEnrichedWithStreamingBarUnattached.ParentStreamingBar = barAttached;
			//v2-WRONG quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(consumer.ConsumerBarsToAppendInto.BarStreaming);

			//Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;	// already cloned upstack .Clone();
			Quote quoteAttachedToStreamingAttachedToConsumerBars = quoteCloneSernoEnrichedFactoryUnattachedStreamingBar;	//.Clone();
			quoteAttachedToStreamingAttachedToConsumerBars.SetParentBarStreaming(bars.BarStreamingNullUnsafe);
			return quoteAttachedToStreamingAttachedToConsumerBars;
		}
		public override string ToString() {
			string ret = "BarFactory[" + this.StreamingBarFactoryUnattached.ToString() + "]";
			if (this.Consumer.ConsumerBarsToAppendInto != null) {
				ret += " with ConsumerBars[" + this.Consumer.ConsumerBarsToAppendInto.ToString() + "]";
			} else {
				ret += " for Consumer[" + this.Consumer.ToString() + "]";
			}
			return ret;
		}
	}
}