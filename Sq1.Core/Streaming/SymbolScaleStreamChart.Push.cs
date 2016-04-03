using System;
using System.Collections.Generic;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {
		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { //lock (this.LockConsumersQuote) {
			string msig = " //SymbolScaleStreamChart.PushQuote_toConsumers() " + this.ToString();

			foreach (StreamingConsumerChart consumer in base.ConsumersAll) {
				Quote quote_clonedBoundUnattached = null;
				Quote quote_clonedBoundAttached = quote_clonedBoundUnattached;
				try {
					quote_clonedBoundUnattached = this.quote_cloneBind_attachToStreamingBar_ofConsumer_nullWhenNoConsumers(quoteDequeued_singleInstance, consumer);
				} catch (Exception ex) {
					string msg = "PUSH_FAILED quoteDequeued_singleInstance[" + quoteDequeued_singleInstance + "]";
					Assembler.PopupException(msg + msig, ex);
					continue;
				}

				msig += " #" + quote_clonedBoundAttached.IntraBarSerno + "/" + quote_clonedBoundAttached.AbsnoPerSymbol;

				if (this.ConsumersQuote.Contains(consumer)) {
					try {
						consumer.ConsumeQuoteOfStreamingBar(quote_clonedBoundUnattached);
						string msg = "QUOTE_CONSUMER_FINISHED " + quote_clonedBoundUnattached.ToStringShort() + " => " + consumer.ToString();
						Assembler.PopupException(msg, null, false);
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
						consumer.ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(
							barStaticLast, quote_clonedBoundAttached);
						string msg = "BAR_CONSUMER_FINISHED " + barStaticLast.ToString() + " => " + consumer.ToString();
						Assembler.PopupException(msg, null, false);
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

		Quote quote_cloneBind_attachToStreamingBar_ofConsumer_nullWhenNoConsumers(Quote quoteCloned_intrabarSernoEnriched_unbound, StreamingConsumerChart quoteConsumer) {
			string msig = " //SymbolScaleStreamChart.quote_cloneBind_nullWhenNoConsumers() " + this.ToString();
			Quote ret = null;

			Quote clone = quoteCloned_intrabarSernoEnriched_unbound.Clone_asCoreQuote();
			Bar barStreaming_expandedByQuote = quoteConsumer.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe;
			if (barStreaming_expandedByQuote == null) {
				string msg = "I_REFUSE_TO_BIND_QUOTE_TO_NULL_STREAMING_BAR";
				Assembler.PopupException(msg);
				return ret;
			}
			barStreaming_expandedByQuote.MergeExpandHLCV_forStreamingBarUnattached(clone);
			clone.StreamingBar_Replace(barStreaming_expandedByQuote);
			ret = clone;

			return ret;
		}
	}
}
