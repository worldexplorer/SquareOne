using System;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {

		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { //lock (this.LockConsumersQuote) {
			string msig = " //SymbolScaleStreamChart.PushQuote_toConsumers() " + this.ToString();

			foreach (StreamingConsumerChart consumer in base.Consumers_QuoteAndBar_GroupedInvocation) {
				if (double.IsNaN(quoteDequeued_singleInstance.TradedPrice)) {
					string msg = "PROTECTING_BARS_TO_HAVE_NO_NANs_EVER #1";
					Assembler.PopupException(msg, null, false);
					continue;
				}

				Quote quote_clonedBoundAttached = null;
				try {
					quote_clonedBoundAttached = this.quote_cloneBind_attachToStreamingBar_ofConsumer_nullWhenNoConsumers(quoteDequeued_singleInstance, consumer);
					if (quote_clonedBoundAttached == null) return;
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance + "]";
					Assembler.PopupException(msg + msig, ex);
					continue;
				}

				msig += " #" + quote_clonedBoundAttached.IntraBarSerno + "/" + quote_clonedBoundAttached.AbsnoPerSymbol;

				if (base.ConsumersQuote.Contains(consumer)) {
					try {
						consumer.Consume_quoteOfStreamingBar(quote_clonedBoundAttached);
						string msg = "QUOTE_CONSUMER_FINISHED " + quote_clonedBoundAttached.ToStringShort() + " => " + consumer.ToString();
						//Assembler.PopupException(msg, null, false);
					} catch (Exception ex) {
						string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
						Assembler.PopupException(msg + msig, ex);
					}
				}

				if (quote_clonedBoundAttached.IntraBarSerno > 0) {
					string msg = "THIS_QUOTE_DOESNT_OPEN_NEW_BAR"
						+ " THIS_IS_GROUPED_INVOCATION_IN_ACTION__IF_YOU_REMOVE_OUTER_FOREACH_YOU_WILL_DELIVER_QUOTE_TO_EACH_CONSUMER_THEN_BAR_TO_EACH_CONSUMER"
						+ " WITH_OUTER_FOREACH__I_DELIVER_QUOTE+BAR_TO_ONE_CONSUMER__THEN_TO_THE_NEXT_ONE";
					continue;
				}

				if (base.ConsumersBar.Contains(consumer)) {
					try {
						//lock (this.lockConsumersBar) {
						Bar barStaticLast = consumer.ConsumerBars_toAppendInto.BarStaticLast_nullUnsafe;
						consumer.Consume_barLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(
							barStaticLast, quote_clonedBoundAttached);
						string msg = "BAR_CONSUMER_FINISHED " + barStaticLast.ToString() + " => " + consumer.ToString();
						//Assembler.PopupException(msg, null, false);
						//}
					} catch (Exception ex) {
						string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
						Assembler.PopupException(msg + msig, ex);
						continue;
					}
				}

			}
			//this.RaiseOnQuoteSyncPushedToAllConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
		}

		public override void PushLevelTwoFrozen_toConsumers(LevelTwoFrozen levelTwoFrozenSorted) {
			string msig = " //SymbolScaleStreamChart.PushLevelTwo_frozenSorted_immutableThusNoWatchdog_toConsumers() " + this.ToString();

			foreach (StreamingConsumerChart consumer in base.ConsumersLevelTwoFrozen) {
				try {
					consumer.Consume_levelTwoChanged(levelTwoFrozenSorted);
				} catch (Exception ex) {
					string msg = "PUSH_FAILED levelTwoFrozen[" + levelTwoFrozenSorted + "]";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		}


		int intraBarSerno;
		Quote quote_cloneBind_attachToStreamingBar_ofConsumer_nullWhenNoConsumers(Quote quoteDequeued_singleInstance, StreamingConsumerChart quoteConsumer) {
			string msig = " //SymbolScaleStreamChart.quote_cloneBind_nullWhenNoConsumers() " + this.ToString();
			Quote ret = null;

			Quote quoteClone_unbound = quoteDequeued_singleInstance.Clone_asCoreQuote();
			Bars consumerBars = quoteConsumer.ConsumerBars_toAppendInto;

			bool justCreated_dontMerge = false;
			bool firstEverQuote_ofNewSymbol = quoteConsumer.ConsumerBars_toAppendInto.Count == 0;
			bool barsWereNeverStreamed = consumerBars.BarStreaming_nullUnsafe == null;
			if (firstEverQuote_ofNewSymbol || barsWereNeverStreamed) {
				this.intraBarSerno = 0;
				Bar newBar_prototype = new Bar(consumerBars.Symbol, consumerBars.ScaleInterval, quoteClone_unbound.ServerTime,
										quoteClone_unbound.TradedPrice, quoteClone_unbound.Size, consumerBars.SymbolInfo);
				quoteConsumer.ConsumerBars_toAppendInto.BarStreaming_createNewAttach_orAbsorb(newBar_prototype);
				justCreated_dontMerge = true;
			}

			Bar barStreaming_expandedByQuote = consumerBars.BarStreaming_nullUnsafe;
			if (barStreaming_expandedByQuote == null) {
				string msg = "I_REFUSE_TO_BIND_QUOTE_TO_NULL_STREAMING_BAR";
				Assembler.PopupException(msg);
				return ret;
			}

			if (quoteClone_unbound.ServerTime >= barStreaming_expandedByQuote.DateTime_nextBarOpen_unconditional) {
				this.intraBarSerno = 0;
				Bar newBar_prototype = new Bar(consumerBars.Symbol, consumerBars.ScaleInterval, quoteClone_unbound.ServerTime,
										quoteClone_unbound.TradedPrice, quoteClone_unbound.Size, consumerBars.SymbolInfo);
				quoteConsumer.ConsumerBars_toAppendInto.BarStreaming_createNewAttach_orAbsorb(newBar_prototype);
				barStreaming_expandedByQuote = consumerBars.BarStreaming_nullUnsafe;
			} else {
				if (justCreated_dontMerge == false) {
					// spread can be anywhere outside the bar; but a bar freezes only traded spreads inside (Quotes DDE table from Quik, not Level2-generated with Size=0)
					barStreaming_expandedByQuote.MergeExpandHLCV_forStreamingBarUnattached(quoteClone_unbound);
				}
				this.intraBarSerno++;
			}


			if (quoteClone_unbound.IntraBarSerno == -1) {
				quoteClone_unbound.IntraBarSerno = this.intraBarSerno;
			} else {
				if (quoteClone_unbound.HasGeneratedSource) {
				} else {
					string msg = "DONT_SET_quote.IntraBarSerno_UPSTACK__I_WILL_DO_IT_HERE";
					Assembler.PopupException(msg);
				}
			}

			Quote bound = quoteClone_unbound;
			bound.StreamingBar_Replace(barStreaming_expandedByQuote);
			ret = bound;

			return ret;
		}
	}
}
