using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier {
		BarsEmulator_forSolidifier	barsEmulator_forSolidifier;	//	{ get; protected set; }

		string mustBeStrictly_oneSolidifierSubscribed_bothToQuotesAndBars__probablySame_forAllSymbolsOfDatasource_lazyToLockDuringWrites() {
			int consumersAllCount = base.Consumers_QuoteAndBar_GroupedInvocation.Count;
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
			Quote quoteCurrent = snap.GetQuoteLast_forSymbol_nullUnsafe(quoteDequeued_singleInstance.Symbol);

			// late quote should be within current StreamingBar, otherwize don't deliver for channel
			if (quoteCurrent != null && quoteDequeued_singleInstance.ServerTime < quoteCurrent.ServerTime) {
				Bar pseudoStreamingBar1 = this.barsEmulator_forSolidifier.PseudoBarStreaming_unattached;
				if (quoteDequeued_singleInstance.ServerTime <= pseudoStreamingBar1.DateTimeOpen) {
					string msg = "skipping old quote for quote.ServerTime[" + quoteDequeued_singleInstance.ServerTime + "], can only accept for current"
						+ " pseudoStreamingBar(" + pseudoStreamingBar1.DateTimeOpen + " .. " + pseudoStreamingBar1.DateTime_nextBarOpen_unconditional + "];"
						+ " quote=[" + quoteDequeued_singleInstance + "]";
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
			}


			Quote quote_clonedBoundUnattached_withPseudoExpanded = null;
			try {
				quote_clonedBoundUnattached_withPseudoExpanded = this.barsEmulator_forSolidifier.Quote_cloneBind_toUnattachedPseudoStreamingBar_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(quoteDequeued_singleInstance);
			} catch (Exception ex) {
				string msg = "REPLACE_EXPANDED_BAR_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance + "]";
				Assembler.PopupException(msg + msig, ex);
				return;
			}
			if (quote_clonedBoundUnattached_withPseudoExpanded == null) {
				string msg = "BAR_SIMULATOR_RETURNED_NULL";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}

			Bar pseudoStreamingBar = quote_clonedBoundUnattached_withPseudoExpanded.ParentBarStreaming;
			if (pseudoStreamingBar == null) {		// equivalent to quote_clonedBoundUnattached_withPseudoExpanded.HasParentBarStreaming == false
				string msg = "I_REFUSE_TO_SAVE_EXPANDED_PSEUDO_BAR__BARS_SIMULATOR_DIDNT_ATTACH_PSEUDO_BAR";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (pseudoStreamingBar.ParentBars != null) {
				string msg = "BARS_SIMULATOR_SHOULD_PRODUCE_A_BAR_HANGING_WITHOUT_PARENTS__AND_THATS_WHAT_RepositoryBarsFile_WILL_SAVE";
				Assembler.PopupException(msg + msig);
			}

			Quote quote_clonedBoundAttached = quote_clonedBoundUnattached_withPseudoExpanded;
			msig += " IntraBarSerno#" + quote_clonedBoundAttached.IntraBarSerno + "/" + quote_clonedBoundAttached.AbsnoPerSymbol;

			if (quote_clonedBoundAttached.IntraBarSerno == 0) {
				try {
					StreamingConsumerSolidifier willUpdateLastStatic_andAppendNewFromPseudo = base.ConsumersQuote[0];
					//lock (this.lockConsumersBar) {
					//consumerBars.BarStreaming_overrideDOHLCVwith(quote.ParentBarStreaming);
					//quote.Replace_myStreamingBar_withConsumersStreamingBar(consumerBars.BarStreaming_nullUnsafe);
					Bar barStaticLast = barsEmulator_forSolidifier.BarLastFormedUnattached_nullUnsafe;
					willUpdateLastStatic_andAppendNewFromPseudo.Consume_barLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(
						barStaticLast, quote_clonedBoundAttached);
					string msg1 = "BAR_CONSUMER_FINISHED " + barStaticLast.ToString() + " => " + willUpdateLastStatic_andAppendNewFromPseudo.ToString();
					//Assembler.PopupException(msg1 + msig, null, false);
					//}
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quote_clonedBoundAttached[" + quote_clonedBoundAttached.ToString() + "]";
					Assembler.PopupException(msg + msig, ex);
				}

			} else {
				try {
					StreamingConsumerSolidifier willReplaceLast = base.ConsumersQuote[0];
					willReplaceLast.Consume_quoteOfStreamingBar(quote_clonedBoundAttached);
					string msg1 = "QUOTE_CONSUMER_FINISHED " + quote_clonedBoundAttached.ToStringShort() + " => " + willReplaceLast.ToString();
					//Assembler.PopupException(msg1 + msig, null, false);
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quote_clonedBoundAttached[" + quote_clonedBoundAttached.ToString() + "]";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		} //}

		public override void PushLevelTwoFrozen_toConsumers(LevelTwoFrozen levelTwo_frozenSorted_peeledOffStreamingSnap_singleInstance_sameForAllChartsTimeframes) {
			string msig = " //SymbolScaleStreamSolidifier.PushLevelTwo_frozenSorted_immutableNoWatchdog_toConsumers() " + this.ToString();
			string msg = "REMOVE_THIS_INVOCATION_UPSTACK";
			throw new Exception(msg + msig);
		}
	}
}
