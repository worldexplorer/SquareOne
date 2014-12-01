using System;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingSolidifier : IStreamingConsumer {
		DataSource	dataSource;

		double		barStreamingDumpIntervalSeconds;
		DateTime	barStreamingLastDumpedLocal;
		string		barStreamingLastDumpedLocalAsString;

		//IMPORTANT_SPLITTED_TO_CTOR+INITIALIZE_FOR_EDITED_DATASOURCE_TO_NOT_SUBSCRIBE_MY_MULTIPLE_INSTANCES public StreamingSolidifier(DataSource dataSource) {

		public void Initialize(DataSource dataSource, double barStreamingDumpIntervalSeconds = 10) {
			if (dataSource == null) {
				string msg = "DATASOURCE_NULL_ABSOLUTELY_INACCEPTABLE_FOR_SOLIDIFIER";
				throw new Exception(msg);
			}
			this.dataSource = dataSource;
			this.barStreamingDumpIntervalSeconds = barStreamingDumpIntervalSeconds;
			this.barStreamingLastDumpedLocal = DateTime.MinValue;
		}

		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { throw new Exception("DONT_EARLYBIND_FOR_TYPE_STREAMING_SOLIDIFIER"); } }

		void IStreamingConsumer.UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		void IStreamingConsumer.UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
			this.replaceStreamingBar(quoteLastBeforeStop, true);
		}
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			this.replaceStreamingBar(quote);
		}

		void replaceStreamingBar(Quote quote, bool ignoreIntervalForceReplaceBarImmediately = false) {
			if (this.barStreamingLastDumpedLocal == DateTime.MinValue) {
				this.barStreamingLastDumpedLocal = quote.LocalTimeCreated;
			}
	
			double secondsSinceLastDumped = quote.LocalTimeCreated.Subtract(barStreamingLastDumpedLocal).TotalSeconds;
			if (ignoreIntervalForceReplaceBarImmediately == false && secondsSinceLastDumped <= this.barStreamingDumpIntervalSeconds) return;

			string millisElapsed;
			int barsSaved = this.dataSource.BarAppendOrReplaceLast(quote.ParentBarStreaming, out millisElapsed);

			string msig = " //StreamingSolidifier.replaceStreamingBar(" + quote.ParentBarStreaming.Close + ")";
			string msg = millisElapsed
				+ " quote[" + quote.LocalTimeCreated.ToString("mm:ss.fff") + "]=>[" + this.barStreamingLastDumpedLocal.ToString("mm:ss.fff") + "]"
				+ " secondsSinceLastDumped[" + secondsSinceLastDumped.ToString("N3") + "] >= dumpInterval[" + this.barStreamingDumpIntervalSeconds + "]";
			Assembler.PopupException(msg + msig, null, false);

			this.barStreamingLastDumpedLocal = quote.LocalTimeCreated;
			this.barStreamingLastDumpedLocalAsString = quote.LocalTimeCreated.ToString("HH:mm:ss.fff");
		}
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated_WILL_BE_NULL) {
			string millisSavingTook;
			int barsSaved = this.dataSource.BarAppendOrReplaceLast(barLastFormed, out millisSavingTook);
			string msg = millisSavingTook + "; DataSource[" + this.dataSource.Name + "] received barLastFormed[" + barLastFormed + "] from streaming";
		}

		public override string ToString() {
			return "StreamingSolidifier[" + this.dataSource.ToString() + "]";
		}
	}
}
