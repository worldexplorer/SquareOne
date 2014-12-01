using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public interface IStreamingConsumer {
		void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart);
		void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop);
		Bars ConsumerBarsToAppendInto { get; }
		void ConsumeQuoteOfStreamingBar(Quote quote);
		void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated);
	}
}
