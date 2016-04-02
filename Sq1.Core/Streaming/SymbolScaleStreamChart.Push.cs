using System;
using System.Collections.Generic;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {

		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { lock (this.LockConsumersQuote) {
			//foreach (StreamingConsumer consumer in this.ConsumersAll) {
			//    Quote quote_clonedBoundUnattached = null;
			//    try {
			//        quote_clonedBoundUnattached = this.quote_cloneBind(quoteDequeued_singleInstance);
			//    } catch (Exception ex) {
			//        string msg = "PUSH_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance + "]";
			//        Assembler.PopupException(msg + msig, ex);
			//        continue;
			//    }

			//    if (this.ConsumersQuote.Contains(consumer)) {
			//        try {
			//            consumer.ConsumeQuoteOfStreamingBar(quote_clonedBoundUnattached);
			//            string msg = "CONSUMER_PROCESSED "
			//            //v1+ "#" + quoteWithStreamingBarBound.IntraBarSerno + "/" + quoteWithStreamingBarBound.AbsnoPerSymbol
			//                + quoteClone_boundAttached.ToStringShort()
			//                + " => " + consumer.ToString();
			//            //Assembler.PopupException(msg, null, false);

			//        } catch (Exception ex) {
			//            string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
			//            Assembler.PopupException(msg + msig, ex);
			//        }
			//    }

				
			//    this.quoteClonedBound_attach(quote_clonedBoundUnattached, consumer);
			//    Quote quote_clonedBoundAttached = quote_clonedBoundUnattached;

			//    if (this.ConsumersBar.Contains(consumer)) {
			//        try {

			//            //lock (this.lockConsumersBar) {
			//                this.Bar_lastStaticFormedAttached_consume(quoteCloned_intrabarSernoEnriched_unbound);
			//            //}
			//        } catch (Exception ex) {
			//            string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
			//            Assembler.PopupException(msg + msig, ex);
			//            continue;
			//        }
			//    }
			//}
			//this.RaiseOnQuoteSyncPushedToAllConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
		} }

		protected override void Bar_lastStaticFormedAttached_consume(Quote quoteClone_sernoEnriched_withStreamingBarUnattachedToParents, StreamingConsumerChart barConsumer) {
			//string msig = " //bar_lastStaticFormed_attachConsume() " + this.ToString();
			//Bar barStreamingUnattached_clonedFromFactory = this.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached.Clone();
			//if (this.ConsumersBar.Count == 0) {
			//    string msg = "NO_BARS_CONSUMERS__NOT_PUSHING lastBarFormed[" + barStreamingUnattached_clonedFromFactory + "]";
			//    Assembler.PopupException(msg + msig, null, false);
			//    return;
			//}
			//int consumerSerno = 1;
			//int streamingSolidifiersPoked = 0;

			//msig = " missed barStreamingUnattached[" + barStreamingUnattached_clonedFromFactory + "]: BarConsumer#"
			//    + (consumerSerno++) + "/" + this.ConsumersBar.Count + " " + barConsumer.ToString();

			//if (barConsumer is StreamingConsumerSolidifier) {
			//    try {
			//        barConsumer.ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(barStreamingUnattached_clonedFromFactory, quoteClone_sernoEnriched_withStreamingBarUnattachedToParents);
			//    } catch (Exception ex) {
			//        string msg = "BOUND_BAR_PUSH_FAILED " + barStreamingUnattached_clonedFromFactory.ToString();
			//        Assembler.PopupException(msg + msig, ex);
			//    }
			//    continue;
			//}
				
			//if (barConsumer.ConsumerBars_toAppendInto == null) {
			//    //try {
			//    //	//NOPE_FRESH_STREAMING_CONTAINING_JUST_ONE_QUOTE_I_WILL_POKE_QUOTES_FROM_IT consumer.ConsumeBarLastFormed(barLastFormedBound);
			//    //	consumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(barStreamingUnattached);
			//    //} catch (Exception e) {
			//    //	string msg = "CHART_OR_BACKTESTER_WITHOUT_BARS_TO_BIND_AS_PARENT";
			//    //	Assembler.PopupException(msg + msig, e);
			//    //}
			//    string msg = "INVESTIGATE_THIS";
			//    Assembler.PopupException(msg + msig);
			//    continue;
			//}
			//if (	barConsumer.ConsumerBars_toAppendInto.BarStaticLast_nullUnsafe != null
			//        && barConsumer.ConsumerBars_toAppendInto.BarStaticLast_nullUnsafe.DateTimeOpen == barStreamingUnattached_clonedFromFactory.DateTimeOpen) {
			//    string msg = "KEEP_THIS_NOT_HAPPENING_BY_LEAVING_STATIC_LAST_ON_APPRESTART_NULL_ON_LIVEBACKTEST_CONTAINING_LAST_INCOMING_QUOTE"
			//        + " we are on 1st ever streaming quote: probably shouln't add it to avoid ALREADY_HAVE exception";
			//    //Debugger.Break();
			//    Assembler.PopupException(msg + msig, null, false);
			//    continue;
			//}

			//if (this.binderPerConsumer.ContainsKey(barConsumer) == false) {
			//    string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
			//    Assembler.PopupException(msg + msig);
			//    continue;
			//}

			//BinderAttacher_perStreamingChart binderForConsumer = this.binderPerConsumer[barConsumer];
			//Bar barStreamingAttached = null;
			//try {
			//    barStreamingAttached = binderForConsumer.BarAttach(barStreamingUnattached_clonedFromFactory);
			//} catch (Exception e) {
			//    string msg = "BAR_BINDING_TO_PARENT_FAILED " + barStreamingAttached.ToString();
			//    Assembler.PopupException(msg + msig, e);
			//    continue;
			//}
			//Quote quoteBound_toStreamingBarUnattached = null;
			//try {
			//    if (binderForConsumer.Consumer is StreamingConsumerChart &&
			//            binderForConsumer.Consumer.ConsumerBars_toAppendInto.ScaleInterval != this.ScaleInterval) {
			//        string msg = "CONSUMERS_BARS_AND_MINE_HAVE_DIFFERENT_SCALE_INTERVAL????"
			//            + " SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS SOLVED_BY_RETURNING_CLONE_IN_BINDER";
			//        Assembler.PopupException(msg, null, false);
			//    }

			//    quoteBound_toStreamingBarUnattached = binderForConsumer.QuoteBoundToStreamingBar__streamingBarAttach_toConsumerBars(quoteClone_sernoEnriched_withStreamingBarUnattachedToParents);	//.Clone()
			//} catch (Exception e) {
			//    string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteBound_toStreamingBarUnattached.ToString();
			//    throw new Exception(msg, e);
			//}

			//try {
			//    Bar barStaticLast = barConsumer.ConsumerBars_toAppendInto.BarStaticLast_nullUnsafe;
			//    if (barStaticLast == null) {
			//        string msg = "THERE_IS_NO_STATIC_BAR_DURING_FIRST_4_QUOTES_GENERATED__ONLY_STREAMING";
			//        Assembler.PopupException(msg, null, false);
			//        continue;
			//    }
			//            bool firstQuoteOfABar				= quote_clonedBoundAttached.IntraBarSerno == 0;
			//            bool firstEverQuote					= quote_clonedBoundAttached.AbsnoPerSymbol == 0;
			//            bool barLastStaticFormedIsValid		= !this.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormed_unattached_notYetFormed;
			//            bool streamingBarReadyToSpawn		=  this.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached.DateTimeOpen
			//                                                            < quote_clonedBoundAttached.ServerTime;
			//            bool streamingBarWronglyRestoredAfterBacktest =
			//                this.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached.DateTimeOpen
			//                    > quote_clonedBoundAttached.ServerTime;

			//            //if ((firstQuoteOfABar && firstEverQuote == false) || streamingBarWronglyRestoredAfterBacktest) {
			//            if (firstQuoteOfABar || streamingBarWronglyRestoredAfterBacktest) {
			//                if (streamingBarWronglyRestoredAfterBacktest) {
			//                    string msg = "FORCING_EXECUTE_ON_LASTFORMED_BY_RESETTING_LAST_FORMED_TO_PREVIOUSLY_EXECUTED_AFTER_BACKTEST";
			//                }
			//                if (barLastStaticFormedIsValid
			//                        //this.StreamingBarFactoryUnattached.BarLastFormedUnattached != null
			//                        //&& double.IsNaN(StreamingBarFactoryUnattached.LastBarFormedUnattached.Close) == true
			//                        //PAUSED_APPRESTART_BACKTEST_MinValue_HERE && this.StreamingBarFactoryUnattached.BarLastFormedUnattached.DateTimeOpen != DateTime.MinValue
			//                    ) {



			//    barConsumer.ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(barStaticLast, quoteBound_toStreamingBarUnattached);




			//                } else {
			//                    string msg = "I_REFUSE_BIND_AND_PUSH_LAST_STATIC_FORMED [" + this.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe + "]"
			//                        //+ " I won't push LastStaticBar(DateTime.MinValue, NaN*5) because it has initialized LastStaticBar=StreamingBar.Clone()"
			//                        + " on first quoteClone_intrabarSernoEnriched_unbound[" + quoteCloned_intrabarSernoEnriched_unbound + "]"
			//                        + " for " + this.UnattachedStreamingBar_factoryPerSymbolScale;
			//                    //Assembler.PopupException(msg, null, false);
			//                }
			//            }
			//} catch (Exception ex) {
			//    string msg = "BOUND_BAR_PUSH_FAILED " + barStreamingAttached.ToString();
			//    Assembler.PopupException(msg + msig, ex);
			//}
		}
		protected override void QuoteCloned_bindAttach(Quote quoteCloned_intrabarSernoEnriched_unbound, StreamingConsumerChart quoteConsumer) {
			//string msig = " //quoteCloned_bindAttach_pushToConsumers() " + this.ToString();
			//if (this.ConsumersQuote.Count == 0 && this.ImServingSolidifier == false) {
			//    string msg = "NO_QUOTE_CONSUMERS__NOT_PUSHING quoteSernoEnriched[" + quoteCloned_intrabarSernoEnriched_unbound + "]";
			//    Assembler.PopupException(msg + msig, null, false);
			//    return;
			//}
			//int consumerSerno = 1;
			//int streamingSolidifiersPoked = 0;
			//msig = " //bindConsumeQuote(): quoteSernoEnrichedWithUnboundStreamingBar[" + quoteCloned_intrabarSernoEnriched_unbound.ToString()
			//    + "]: QuoteConsumer#" + (consumerSerno++) + "/" + this.ConsumersQuote.Count + " " + quoteConsumer.ToString();

			//if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) {
			//    string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
			//    Assembler.PopupException(msg + msig);
			//    continue;
			//}
			//// clone 
			//Quote quoteCloned_intrabarSernoEnriched_unbound = this.UnattachedStreamingBar_factoryPerSymbolScale.
			//    Quote_cloneBind_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(quoteDequeued_singleInstance_tillStreamBindsAll);
			//if (quoteCloned_intrabarSernoEnriched_unbound == null) {
			//    string msg = "I_REFUSE_TO_PUSH COULD_NOT_ENRICH_QUOTE quoteClone_intrabarSernoEnriched_unbound[null]"
			//        + " quote2bCloned_bindingToEachConsumer_willHappen_afterDequeued[" + quoteDequeued_singleInstance_tillStreamBindsAll + "]"
			//        + " this[" + this + "]"
			//        ;
			//    //Assembler.PopupException(msg, null, false);
			//    return;
			//}

			//if (	quoteCloned_intrabarSernoEnriched_unbound.ParentBarStreaming == null
			//     || quoteCloned_intrabarSernoEnriched_unbound.ParentBarStreaming.ParentBars == null) {
			//    string msg = "HERE_NULL_IS_OK__UNATTACHED___BINDER_WILL_BE_INVOKED_DOWNSTACK_IN_PUMP_THREAD_SOON_FOR_QUOTE_CLONED"
			//        + " StreamingEarlyBinder.BindStreamingBarForQuote()";
			//} else {
			//    string msg = "STREAMING_FOR_QUOTE_MUST_NOT_BE_INITIALIZED_HERE";
			//    Assembler.PopupException(msg, null, false);
			//}



			//if (quoteCloned_intrabarSernoEnriched_unbound.IntraBarSerno == -1) {
			//    string msg = "QUOTE_WAS_NOT_ENRICHED_BY_StreamingBarFactoryUnattached.EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar()";
			//    Assembler.PopupException(msg);
			//}
	
			
			//BinderAttacher_perStreamingChart binderForConsumer = this.binderPerConsumer[quoteConsumer];

			//Quote quoteClone_boundAttached = null;
			//try {
			//    quoteClone_boundAttached = binderForConsumer.QuoteBoundToStreamingBar__streamingBarAttach_toConsumerBars(quoteCloned_intrabarSernoEnriched_unbound);
			//} catch (Exception e) {
			//    string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteClone_boundAttached.ToString();
			//    Assembler.PopupException(msg + msig, e);
			//    continue;
			//}

			//if (binderForConsumer.Consumer is StreamingConsumerChart &&
			//        binderForConsumer.Consumer.ConsumerBars_toAppendInto.ScaleInterval != this.ScaleInterval) {
			//    string msg2 = "CONSUMERS_BARS_AND_MINE_HAVE_DIFFERENT_SCALE_INTERVAL???? SOLVED_BY_RETURNING_CLONE_IN_BINDER";
			//    Assembler.PopupException(msg2, null, false);
			//}

		}
	}
}
