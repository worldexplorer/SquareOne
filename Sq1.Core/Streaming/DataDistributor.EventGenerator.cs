using System;

namespace Sq1.Core.Streaming {
	public partial class DataDistributor {
		public event EventHandler<QuoteEventArgs> OnQuoteAsyncPushedToAllDistributionChannels;
		
		public void RaiseOnQuoteAsyncPushedToAllDistributionChannels(Quote quote) {
			if (this.OnQuoteAsyncPushedToAllDistributionChannels == null) return;
			try {
				this.OnQuoteAsyncPushedToAllDistributionChannels(this, new QuoteEventArgs(quote));
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER(USED_ONLY_FOR_LIVE_SIMULATOR)_THROWN //DataDistributor.RaiseQuotePushedToDistributor(" + quote + ")";
				Assembler.PopupException(msg, e);
			}
		}
	}
}
