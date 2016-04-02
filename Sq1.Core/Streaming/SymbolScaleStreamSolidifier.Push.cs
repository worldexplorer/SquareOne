using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier {
		public override void PushQuote_toConsumers(Quote quoteDequeued_singleInstance) { lock (this.LockConsumersQuote) {
			StreamingDataSnapshot snap =  this.SymbolChannel.Distributor.StreamingAdapter.StreamingDataSnapshot;
			Quote quotePrev = snap.GetQuotePrev_forSymbol_nullUnsafe(quoteDequeued_singleInstance.Symbol);

			// late quote should be within current StreamingBar, otherwize don't deliver for channel
			if (quotePrev != null && quoteDequeued_singleInstance.ServerTime < quotePrev.ServerTime) {
				Bar streamingBar = this.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;
				if (quoteDequeued_singleInstance.ServerTime <= streamingBar.DateTimeOpen) {
					string msg = "skipping old quote for quote.ServerTime[" + quoteDequeued_singleInstance.ServerTime + "], can only accept for current"
						+ " StreamingBar (" + streamingBar.DateTimeOpen + " .. " + streamingBar.DateTimeNextBarOpenUnconditional + "];"
						+ " quote=[" + quoteDequeued_singleInstance + "]";
					Assembler.PopupException(msg, null, false);
					return;
				}
			}
		} }

		protected override void Bar_lastStaticFormedAttached_consume(Quote quoteClone_sernoEnriched_withStreamingBarUnattachedToParents, StreamingConsumerSolidifier barConsumer) {
		}

		protected override void QuoteCloned_bindAttach(Quote quoteCloned_intrabarSernoEnriched_unbound, StreamingConsumerSolidifier quoteConsumer) {
		}

	}
}
