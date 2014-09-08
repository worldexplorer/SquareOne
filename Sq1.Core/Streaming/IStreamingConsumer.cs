using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public interface IStreamingConsumer {
		void ConsumeQuoteOfStreamingBar(Quote quote);
		void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed);
		Bars ConsumerBarsToAppendInto { get; }
	}
}
