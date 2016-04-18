using System;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamCharts {

		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { //lock (this.LockConsumersQuote) {
			string msig = " //SymbolScaleStreamChart.PushQuote_toConsumers() " + this.ToString();
			if (double.IsNaN(quoteDequeued_singleInstance.TradedPrice)) {
				string msg = "PROTECTING_BARS_TO_HAVE_NO_NANs_EVER #1";
				Assembler.PopupException(msg, null, false);
				return;
			}

			foreach (StreamingConsumerChart consumer in base.Consumers_QuoteAndBar_GroupedInvocation) {
				Quote quote_clonedBoundAttached = null;
				try {
					quote_clonedBoundAttached = consumer
						.Quote_cloneIncrement_bindToStreamingBar__createStreaming_whenBeyond_barCloseTime__nullWhenFirstBarEver(quoteDequeued_singleInstance);
					if (quote_clonedBoundAttached == null) continue;
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



	}
}
