using System;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {
		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { //lock (this.LockConsumersQuote) {
			string msig = " //SymbolScaleStreamChart.PushQuote_toConsumers() " + this.ToString();

			if (double.IsNaN(quoteDequeued_singleInstance.TradedPrice)) {
				string msg = "PROTECTING_BARS_TO_HAVE_NO_NANs_EVER #1";
				Assembler.PopupException(msg, null, false);
				return;
			}

			LevelTwoFrozen levelTwoFrozen = null;
			if (quoteDequeued_singleInstance.Size <= 0) {
				//string msg = "PROTECTING_BARS_TO_HAVE_NO_NANs_EVER #2";
				//Assembler.PopupException(msg, null, false);
				//return;
				try {
					levelTwoFrozen = this.SymbolChannel.Distributor.StreamingAdapter
						.StreamingDataSnapshot.GetLevelTwoFrozenSorted_forSymbol_nullUnsafe(
							quoteDequeued_singleInstance.Symbol, msig, "ALL_SCRIPTS[" + this.SymbolScaleInterval + "]");
				} catch (Exception ex) {
					string msg = "PUSH_FAILED levelTwoFrozen[" + levelTwoFrozen + "]";
					Assembler.PopupException(msg + msig, ex);
					return;
				}
			}


			foreach (StreamingConsumerChart consumer in base.ConsumersAll) {
				if (quoteDequeued_singleInstance.Size <= 0) {
					try {
						consumer.Consume_levelTwoChanged_noNewQuote(levelTwoFrozen);
					} catch (Exception ex) {
						string msg = "PUSH_FAILED levelTwoFrozen[" + levelTwoFrozen + "]";
						Assembler.PopupException(msg + msig, ex);
					}
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

				if (this.ConsumersQuote.Contains(consumer)) {
					try {
						consumer.Consume_quoteOfStreamingBar(quote_clonedBoundAttached);
						string msg = "QUOTE_CONSUMER_FINISHED " + quote_clonedBoundAttached.ToStringShort() + " => " + consumer.ToString();
						//Assembler.PopupException(msg, null, false);
					} catch (Exception ex) {
						string msg = "PUSH_FAILED quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance.ToString() + "]";
						Assembler.PopupException(msg + msig, ex);
					}
				}

				if (quote_clonedBoundAttached.IntraBarSerno > 0) return;

				if (this.ConsumersBar.Contains(consumer)) {
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

			if (quoteClone_unbound.ServerTime >= barStreaming_expandedByQuote.DateTimeNextBarOpenUnconditional) {
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
