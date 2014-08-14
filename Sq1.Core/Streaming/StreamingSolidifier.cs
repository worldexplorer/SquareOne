using System;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingSolidifier : IStreamingConsumer {
		DataSource DataSource; 
		public StreamingSolidifier(DataSource dataSource) {
			this.DataSource = dataSource;
		}
		
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get {
				throw new NotImplementedException();
			} }
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Sq1.Core.DataTypes.Quote quote) {
		}
		void IStreamingConsumer.ConsumeBarLastStraticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			if (this.DataSource == null) return;
			int barsSaved = this.DataSource.BarAppend(barLastFormed);
			string msg = "Saved [ " + barsSaved + "] bars; DataSource[" + this.DataSource.Name + "] received barLastFormed[" + barLastFormed + "] from streaming";
		}
	}
}
