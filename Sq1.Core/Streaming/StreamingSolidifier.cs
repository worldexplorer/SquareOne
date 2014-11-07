using System;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingSolidifier : IStreamingConsumer {
		DataSource DataSource;
		Bars barsForEarlyBinder;
		public StreamingSolidifier(DataSource dataSource) {
			this.DataSource = dataSource;
		}
		
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get {
				if (this.barsForEarlyBinder == null) return null;
				return this.barsForEarlyBinder;
			} }
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) { }
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			if (this.DataSource == null) return;
			this.barsForEarlyBinder = barLastFormed.ParentBars;
			int barsSaved = this.DataSource.BarAppend(barLastFormed);
			string msg = "Saved [ " + barsSaved + "] bars; DataSource[" + this.DataSource.Name + "] received barLastFormed[" + barLastFormed + "] from streaming";
		}

		public override string ToString() {
			return "StreamingSolidifier[" + this.DataSource.ToString() + "]";
		}
	}
}
