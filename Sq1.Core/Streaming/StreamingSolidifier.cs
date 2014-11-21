using System;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingSolidifier : IStreamingConsumer {
		DataSource dataSource;

		//IMPORTANT_SPLITTED_TO_CTOR+INITIALIZE_FOR_EDITED_DATASOURCE_TO_NOT_SUBSCRIBE_MY_MULTIPLE_INSTANCES public StreamingSolidifier(DataSource dataSource) {

		public void Initialize(DataSource dataSource) {
			if (dataSource == null) {
				string msg = "DATASOURCE_NULL_ABSOLUTELY_INACCEPTABLE_FOR_SOLIDIFIER";
				throw new Exception(msg);
			}
			this.dataSource = dataSource;
		}

		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { throw new Exception("DONT_EARLYBIND_FOR_TYPE_STREAMING_SOLIDIFIER"); } }
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			//bool quoteUpdated = this.dataSource.UpdateCloseForLastBarSaved(quote);
			//string msg = "Updated [ " + quoteUpdated + "] LastBar.Close; DataSource[" + this.dataSource.Name + "] received quote[" + quote.ToString() + "] from streaming";
		}
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			int barsSaved = this.dataSource.BarAppend(barLastFormed);
			string msg = "Saved [ " + barsSaved + "] bars; DataSource[" + this.dataSource.Name + "] received barLastFormed[" + barLastFormed + "] from streaming";
		}

		public override string ToString() {
			return "StreamingSolidifier[" + this.dataSource.ToString() + "]";
		}
	}
}
