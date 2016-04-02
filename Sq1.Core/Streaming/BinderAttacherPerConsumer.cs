using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class BinderAttacherPerConsumer {
				SymbolScaleStream	symbolScaleStream;
		public	StreamingConsumer	Consumer				{ get; private set;}

		public BinderAttacherPerConsumer(SymbolScaleStream symbolScaleStream, StreamingConsumer consumer) {
			if (consumer is StreamingSolidifier) {
				string msg = "NEVER_INVOKE_ME_FOR_consumer=StreamingSolidifier //StreamingBinderPerConsumer()";
				throw new Exception(msg);
			}
			this.Consumer = consumer;
		}
		public Bar BarAttach(Bar barStreamingUnattached_clonedFromFactory) {
			if (this.Consumer is StreamingSolidifier) {
				string msg = "NEVER_INVOKE_ME_WHEN_this.Consumer_is_StreamingSolidifier //BarAttach(" + barStreamingUnattached_clonedFromFactory + ")";
				throw new Exception(msg);
			}

			Bars bars = this.Consumer.ConsumerBars_toAppendInto;
			if (bars == null) {
				string msg = "BAR_NOT_ATTACHED CHECK_UPSTACK_WHETHER_CONSUMER_BARS_ARE_NULL StreamingSolidifier will attach the Bar itself";
				Assembler.PopupException(msg);
				return barStreamingUnattached_clonedFromFactory;
			}
			//Bar barDetached = barLastFormedUnattached.CloneDetached();
			//Bar barStreamingAttached = bars.BarStreamingCreateNewOrAbsorb(barDetached);
			Bar barStreamingAttached = bars.BarStreaming_createNewAttach_orAbsorb(barStreamingUnattached_clonedFromFactory);

			if (barStreamingAttached != bars.BarLast) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingAttached != consumer.ConsumerBarsToAppendInto.BarLast";
				throw new Exception(msg);
			}
			if (barStreamingAttached == bars.BarStaticLast_nullUnsafe) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingAttached == consumer.ConsumerBarsToAppendInto.BarStaticLast_nullUnsafe";
				throw new Exception(msg);
			}
			if (barStreamingAttached != bars.BarStreaming_nullUnsafe) {
				string msg = "MUST_NEVER_HAPPEN_barStreamingAttached != consumer.ConsumerBarsToAppendInto.BarStreaming";
				throw new Exception(msg);
			}
			return barStreamingAttached;
		}
		public Quote QuoteBoundToStreamingBar__streamingBarAttach_toConsumerBars(Quote quoteClone_sernoEnriched_unbound) {
			Quote quote = quoteClone_sernoEnriched_unbound;	// to avoid SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS

			if (this.Consumer is StreamingSolidifier) {
				string msg = "NEVER_INVOKE_ME_WHEN_this.Consumer_is_StreamingSolidifier //QuoteBind_toStreamingBar__streamingBarAttach_toConsumerBars(" + quoteClone_sernoEnriched_unbound + ")";
				throw new Exception(msg);
			}

			Bars consumerBars = null;
			try {
				consumerBars = this.Consumer.ConsumerBars_toAppendInto;
			} catch (Exception ex) {
				string msg = "CONSUMER_DIDNT_PROVIDE_BARS_TO_APPEND_INTO this.Consumer[" + this.Consumer + "]";
				Assembler.PopupException(msg);
				return quote;
			}

			if (consumerBars == null) {
				string msg = "BARS_MUST_NOT_BE_NULL CANT_BIND_QUOTE CHECK_UPSTACK";
				Assembler.PopupException(msg);
				return quote;
			}


			//if (quote.ParentBarStreaming == null) {
			//    string msg = "YOU_DIDNT_REALLY_NEED_Quote_BEING_ALREADY_BOUND_UPSTACK_TILL_NOW__RIGHT??? quote.ParentBarStreaming=null";
			//    Assembler.PopupException(msg);
			//    //quote.Bind_streamingBar_unattached(this.Factory_ofUnattached_streamingBars.BarStreaming_unattached);
			//    return quote;
			//}
			if (this.symbolScaleStream.UnattachedStreamingBar_factory.BarStreaming_unattached == null) {
				string msg = "AM_I_ON_FIRST_QUOTE_OF_THE_BAR";
				Assembler.PopupException(msg);
				return quote;
			}
			//if (quote.ParentBarStreaming != this.Factory_ofUnattached_streamingBars.BarStreaming_unattached) {
			//    string msg = "AM_I_ON_FIRST_QUOTE_OF_THE_BAR??? WAS_QUOTE_NOT_YET_CLONED??";
			//    Assembler.PopupException(msg);
			//    return quote;
			//}


			// each consumer has different Bars (LoadedAll or LoadedFromTill);
			// QuoteBoundToStreamingBar__streamingBarAttach_toConsumerBars() will create new BarStreaming after 1000 of this methods invocation terminate;
			// right now quote has had ParentStreamingBar assigned to streamingBarFactoryUnattached.BarStreaming on ScaleInterval basis (one factory for many 5-min consumers);
			// 1) I clone the quote to leave it untouched for other consumers and I attach it to customers' BarStreaming
			//		(done upstack in SymbolScaleDistributionChannel.bindStreamingBarForQuoteAndPushQuoteToConsumers())
			// 2) I get the customers' BarStreaming and update its DOHLCV
			//v1
			if (consumerBars.BarStreaming_nullUnsafe == null) {
			    string msg = "INITIALIZING_STREAMING_BAR_TO_NON_NULL_NEW_OR_LAST_STATIC"
			        + " FIRST_STREAMING_QUOTE_PER_BACKTEST_ON_STREAMINGLESS_BARS_JUST_FORKED_FROM_BARS_ORIGINAL_AT_BACKTEST_INITIALIZATION";
			    //v1 I_LEFT_QUOTE_UNATTACHED_UPSTACK,ATTACHING_TO_FACTORY_HERE
			    //v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingCreateNewOrAbsorb(quoteCloneSernoEnrichedFactoryUnattachedStreamingBar.ParentBarStreaming);
			    Bar streamingCreatedAttached = consumerBars.BarStreaming_createNewAttach_orAbsorb(this.Factory_ofUnattached_streamingBars.BarStreaming_unattached);
			    if (streamingCreatedAttached != consumerBars.BarStreaming_nullUnsafe) {
			        string msg2 = "MUST_BE_THE_SAME_BAR PARANOID_CHECK";
			        Assembler.PopupException(msg2);
			    }
				quote.Replace_myStreamingBar_withConsumersStreamingBar(consumerBars.BarStreaming_nullUnsafe);
				return quote;
			}

			//string msg4 = "ALL_OTHER_QUOTES_EXCEPT_FIRST_STREAMING_QUOTE_PER_BACKTEST";
			//v1 this.consumer.ConsumerBarsToAppendInto.BarStreamingOverrideDOHLCVwith(quote.ParentBarStreaming);
			//v2 QUOTE_COMES_UNATTACHED_TAKE_STREAMING_HLCV_FROM_FACTORY
			//if (quote.IntraBarSerno == 0) {
			//    string msg2 = "AVOIDING_EXCEPTION NO_NEED_TO_ABSORB_ANYTHING__DESTINATION_HasSameDOHLCV_KOZ_BAR_FACTORY_JUST_STARTED_NEW_STREAMING_BAR";
			//    //return quote;
			//}

			if (quote.ParentBarStreaming == null) {
			}


			consumerBars.BarStreaming_overrideDOHLCVwith(quote.ParentBarStreaming);
			quote.Replace_myStreamingBar_withConsumersStreamingBar(consumerBars.BarStreaming_nullUnsafe);

			return quote;
		}
		public override string ToString() {
			string ret = "BarFactory[" + this.symbolScaleStream.UnattachedStreamingBar_factory.ToString() + "]";
			if (this.Consumer.ConsumerBars_toAppendInto != null) {
				ret += " with ConsumerBars[" + this.Consumer.ConsumerBars_toAppendInto.ToString() + "]";
			} else {
				ret += " for Consumer[" + this.Consumer.ToString() + "]";
			}
			return ret;
		}
	}
}