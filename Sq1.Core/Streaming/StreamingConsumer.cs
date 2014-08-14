using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	[Obsolete("Not used anymore", true)]
	public class StreamingConsumer {
		public string Symbol { get; private set; }
		public IStreamingConsumer Consumer { get; private set; }
		public MarketInfo MarketInfo { get; private set; }
		public StreamingConsumer(string symbol, IStreamingConsumer update, MarketInfo info) {
			this.Symbol = symbol;
			this.Consumer = update;
			this.MarketInfo = info;
		}
	}
}
