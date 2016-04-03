using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier {
		BarsEmulator_forSolidifier	barsEmulator_forSolidifier;	//	{ get; protected set; }

		string mustBeStrictly_oneSolidifierSubscribed_bothToQuotesAndBars__probablySame_forAllSymbolsOfDatasource_lazyToLockDuringWrites() {
			int consumersAllCount = base.ConsumersAll.Count;
			if (consumersAllCount != 1) {
				return " consumersAllCount[" + consumersAllCount + "]";
			}

			int consumersQuoteCount = base.ConsumersQuote.Count;
			if (consumersQuoteCount != 1) {
				return " consumersQuoteCount[" + consumersQuoteCount + "]";
			}

			int consumersBarCount = base.ConsumersBar.Count;
			if (consumersBarCount != 1) {
				return " consumersBarCount[" + consumersBarCount + "]";
			}

			if (base.ConsumersQuote[0] != base.ConsumersBar[0]) {
				return "MUST_BE_EQUAL__base.ConsumersQuote[0]!=base.ConsumersBar[0]";
			}

			return null;
		}

		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { //lock (this.LockConsumersQuote) {
			string msig = " //SymbolScaleStreamSolidifier.PushQuote_toConsumers(" + quoteDequeued_singleInstance + ") " + this.ToString();

			string reason_IcanNotContinue = this.mustBeStrictly_oneSolidifierSubscribed_bothToQuotesAndBars__probablySame_forAllSymbolsOfDatasource_lazyToLockDuringWrites();
			if (string.IsNullOrEmpty(reason_IcanNotContinue) == false) {
				string msg = "I_REFUSE_TO_PUSH_QUOTE__THERE_MUST_BE_STRICTLY_ONE_SOLIDIFIER_PER_SYMBOL " + reason_IcanNotContinue;
				Assembler.PopupException(msg + msig);
				return;
			}


			StreamingDataSnapshot snap =  this.SymbolChannel.Distributor.StreamingAdapter.StreamingDataSnapshot;
			Quote quotePrev = snap.GetQuotePrev_forSymbol_nullUnsafe(quoteDequeued_singleInstance.Symbol);

			// late quote should be within current StreamingBar, otherwize don't deliver for channel
			if (quotePrev != null && quoteDequeued_singleInstance.ServerTime < quotePrev.ServerTime) {
				Bar pseudoStreamingBar1 = this.barsEmulator_forSolidifier.PseudoBarStreaming_unattached;
				if (quoteDequeued_singleInstance.ServerTime <= pseudoStreamingBar1.DateTimeOpen) {
					string msg = "skipping old quote for quote.ServerTime[" + quoteDequeued_singleInstance.ServerTime + "], can only accept for current"
						+ " pseudoStreamingBar(" + pseudoStreamingBar1.DateTimeOpen + " .. " + pseudoStreamingBar1.DateTimeNextBarOpenUnconditional + "];"
						+ " quote=[" + quoteDequeued_singleInstance + "]";
					Assembler.PopupException(msg, null, false);
					return;
				}
			}


			Quote quote_clonedBoundUnattached_withPseudoExpanded = null;
			Quote quote_clonedBoundAttached = quote_clonedBoundUnattached_withPseudoExpanded;
			try {
				quote_clonedBoundUnattached_withPseudoExpanded = this.barsEmulator_forSolidifier.Quote_cloneBind_toUnattachedPseudoStreamingBar_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(quoteDequeued_singleInstance);
			} catch (Exception ex) {
				string msg = "REPLACE_EXPANDED_BAR_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance + "]";
				Assembler.PopupException(msg + msig, ex);
				return;
			}

			Bar pseudoStreamingBar = quote_clonedBoundUnattached_withPseudoExpanded.ParentBarStreaming;
			if (pseudoStreamingBar == null) {		// equivalent to quote_clonedBoundUnattached_withPseudoExpanded.HasParentBarStreaming == false
				string msg = "I_REFUSE_TO_SAVE_EXPANDED_PSEUDO_BAR__BARS_SIMULATOR_DIDNT_ATTACH_PSEUDO_BAR";
				Assembler.PopupException(msg);
				return;
			}
			if (pseudoStreamingBar.ParentBars != null) {
				string msg = "BARS_SIMULATOR_SHOULD_PRODUCE_A_BAR_HANGING_WITHOUT_PARENTS__AND_THATS_WHAT_RepositoryBarsFile_WILL_SAVE";
				Assembler.PopupException(msg);
			}

			msig += " IntraBarSerno#" + quote_clonedBoundAttached.IntraBarSerno + "/" + quote_clonedBoundAttached.AbsnoPerSymbol;

			if (quote_clonedBoundAttached.IntraBarSerno == 0) {
				try {
					StreamingConsumerSolidifier willUpdateLastStatic_andAppendNewFromPseudo = base.ConsumersQuote[0];
					//lock (this.lockConsumersBar) {
					//consumerBars.BarStreaming_overrideDOHLCVwith(quote.ParentBarStreaming);
					//quote.Replace_myStreamingBar_withConsumersStreamingBar(consumerBars.BarStreaming_nullUnsafe);
					Bar barStaticLast = barsEmulator_forSolidifier.BarLastFormedUnattached_nullUnsafe;
					willUpdateLastStatic_andAppendNewFromPseudo.ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(
						barStaticLast, quote_clonedBoundAttached);
					string msg1 = "BAR_CONSUMER_FINISHED " + barStaticLast.ToString() + " => " + willUpdateLastStatic_andAppendNewFromPseudo.ToString();
					Assembler.PopupException(msg1 + msig, null, false);
					//}
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance.ToString() + "]";
					Assembler.PopupException(msg + msig, ex);
				}

			} else {
				try {
					StreamingConsumerSolidifier willReplaceLast = base.ConsumersQuote[0];
					willReplaceLast.ConsumeQuoteOfStreamingBar(quote_clonedBoundUnattached_withPseudoExpanded);
					string msg1 = "QUOTE_CONSUMER_FINISHED " + quote_clonedBoundUnattached_withPseudoExpanded.ToStringShort() + " => " + willReplaceLast.ToString();
					Assembler.PopupException(msg1 + msig, null, false);
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		} //}

		//void QuoteClonedBound_attachToStreamingBar_ofCosumer(Quote quoteClone_sernoEnriched_withStreamingBarUnattachedToParents, StreamingConsumerSolidifier barConsumer) {
		//    string msig = " //SymbolScaleStreamSolidifier.Bar_lastStaticFormedAttached_consume() " + this.ToString();
		//    Bar barStreamingUnattached = this.barsEmulator_forSolidifier.PseudoBarStreaming_unattached;
		//    if (this.ConsumersBar.Count == 0) {
		//        string msg = "NO_BARS_CONSUMERS__NOT_PUSHING lastBarFormed[" + barStreamingUnattached + "]";
		//        Assembler.PopupException(msg + msig, null, false);
		//        return;
		//    }
		//    msig += " missed barStreamingUnattached[" + barStreamingUnattached + "]: " + barConsumer.ToString();

		//    try {
		//        barConsumer.ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(barStreamingUnattached, null);
		//    } catch (Exception ex) {
		//        string msg = "BOUND_BAR_PUSH_FAILED " + barStreamingUnattached.ToString();
		//        Assembler.PopupException(msg + msig, ex);
		//    }
		//}

		//Quote Quote_cloneBind_nullWhenNoConsumers(Quote quoteCloned_intrabarSernoEnriched_unbound, StreamingConsumerSolidifier quoteConsumer) {
		//    string msig = " //SymbolScaleStreamSolidifier.QuoteCloned_bindAttach() " + this.ToString();
		//    Quote ret = null;

		//    if (this.ConsumersQuote.Count == 0) {
		//        string msg = "NO_QUOTE_CONSUMERS__NOT_PUSHING quoteSernoEnriched[" + quoteCloned_intrabarSernoEnriched_unbound + "]";
		//        Assembler.PopupException(msg + msig, null, false);
		//        return null;
		//    }
		//    int consumerSerno = 1;
		//    int streamingSolidifiersPoked = 0;
		//    msig = " //bindConsumeQuote(): quoteSernoEnrichedWithUnboundStreamingBar[" + quoteCloned_intrabarSernoEnriched_unbound.ToString()
		//        + "]: QuoteConsumer#" + (consumerSerno++) + "/" + this.ConsumersQuote.Count + " " + quoteConsumer.ToString();

		//    if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) {
		//        string msg = "CONSUMER_WASNT_REGISTERED_IN_earlyBinders_INVOKE_ConsumersQuoteAdd()";
		//        Assembler.PopupException(msg + msig);
		//        continue;
		//    }
		//    // clone 
		//    Quote quoteCloned_intrabarSernoEnriched_unbound = this.barsEmulator_forSolidifier.
		//        Quote_cloneBind_toUnattachedPseudeStreamingBar_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(quoteDequeued_singleInstance_tillStreamBindsAll);
		//    if (quoteCloned_intrabarSernoEnriched_unbound == null) {
		//        string msg = "I_REFUSE_TO_PUSH COULD_NOT_ENRICH_QUOTE quoteClone_intrabarSernoEnriched_unbound[null]"
		//            + " quote2bCloned_bindingToEachConsumer_willHappen_afterDequeued[" + quoteDequeued_singleInstance_tillStreamBindsAll + "]"
		//            + " this[" + this + "]"
		//            ;
		//        //Assembler.PopupException(msg, null, false);
		//        return;
		//    }

		//    if (	quoteCloned_intrabarSernoEnriched_unbound.ParentBarStreaming == null
		//         || quoteCloned_intrabarSernoEnriched_unbound.ParentBarStreaming.ParentBars == null) {
		//        string msg = "HERE_NULL_IS_OK__UNATTACHED___BINDER_WILL_BE_INVOKED_DOWNSTACK_IN_PUMP_THREAD_SOON_FOR_QUOTE_CLONED"
		//            + " StreamingEarlyBinder.BindStreamingBarForQuote()";
		//    } else {
		//        string msg = "STREAMING_FOR_QUOTE_MUST_NOT_BE_INITIALIZED_HERE";
		//        Assembler.PopupException(msg, null, false);
		//    }



		//    if (quoteCloned_intrabarSernoEnriched_unbound.IntraBarSerno == -1) {
		//        string msg = "QUOTE_WAS_NOT_ENRICHED_BY_StreamingBarFactoryUnattached.EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar()";
		//        Assembler.PopupException(msg);
		//    }
	
			
		//    BinderAttacher_perStreamingChart binderForConsumer = this.binderPerConsumer[quoteConsumer];

		//    Quote quoteClone_boundAttached = null;
		//    try {
		//        quoteClone_boundAttached = binderForConsumer.QuoteBoundToStreamingBar__streamingBarAttach_toConsumerBars(quoteCloned_intrabarSernoEnriched_unbound);
		//    } catch (Exception e) {
		//        string msg = "QUOTE_BINDING_TO_PARENT_STREAMING_BAR_FAILED " + quoteClone_boundAttached.ToString();
		//        Assembler.PopupException(msg + msig, e);
		//        continue;
		//    }

		//    if (binderForConsumer.Consumer is StreamingConsumerChart &&
		//            binderForConsumer.Consumer.ConsumerBars_toAppendInto.ScaleInterval != this.ScaleInterval) {
		//        string msg2 = "CONSUMERS_BARS_AND_MINE_HAVE_DIFFERENT_SCALE_INTERVAL???? SOLVED_BY_RETURNING_CLONE_IN_BINDER";
		//        Assembler.PopupException(msg2, null, false);
		//    //}

		//    return ret;
		//}

	}
}
