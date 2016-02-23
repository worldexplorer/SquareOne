using System;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public class StreamingSolidifier : StreamingConsumer {
		DataSource	dataSource;

		double		barStreamingDumpIntervalSeconds;
		DateTime	barStreamingLastDumpedLocal;
		string		barStreamingLastDumpedLocalAsString;

		//IMPORTANT_SPLITTED_TO_CTOR+INITIALIZE_FOR_EDITED_DATASOURCE_TO_NOT_SUBSCRIBE_MY_MULTIPLE_INSTANCES
		public StreamingSolidifier() { }

		public void Initialize(DataSource dataSource, double barStreamingDumpIntervalSeconds = 10) {
			if (dataSource == null) {
				string msg = "DATASOURCE_NULL_ABSOLUTELY_INACCEPTABLE_FOR_SOLIDIFIER";
				throw new Exception(msg);
			}
			this.dataSource = dataSource;
			this.barStreamingDumpIntervalSeconds = barStreamingDumpIntervalSeconds;
			this.barStreamingLastDumpedLocal = DateTime.MinValue;
			base.ReasonToExist = "SOLIDIFIER[" + dataSource.ToString() + "]";
		}

		#region StreamingConsumer
		public override ScriptExecutor Executor { get {
			throw new Exception("YOU_SHOULD_NOT_ACCESS_EXECUTOR_OF_STREAMING_SOLIDIFIER");
		} }
		public override Bars ConsumerBarsToAppendInto { get {
			throw new Exception("YOU_SHOULD_NOT_ACCESS_BARS_OF_STREAMING_SOLIDIFIER__THEY_ARE_WRITE_ONLY_THOUGH_SOLIDIFIER_METHODS");
		} }

		public override void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		public override void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
			this.replaceStreamingBar(quoteLastBeforeStop, true);
		}
		public override void ConsumeQuoteOfStreamingBar(Quote quote) {
			this.replaceStreamingBar(quote);
		}
		public override void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated_WILL_BE_NULL) {
			string millisSavingTook;
			int barsSaved = this.dataSource.BarAppendOrReplaceLast(barLastFormed, out millisSavingTook);
			string msg = millisSavingTook + "; DataSource[" + this.dataSource.Name + "] received barLastFormed[" + barLastFormed + "] from streaming";
		}
		#endregion

		void replaceStreamingBar(Quote quote, bool ignoreIntervalForceReplaceBarImmediately = false) {
			string msig = " //StreamingSolidifier.replaceStreamingBar(" + quote + ")";
			if (quote is QuoteGenerated) {
				string msg2 = "I_REFUSE_TO_STORE_QuoteGenerated_INTO_FILE [" + quote.Symbol + "].bar";
				Assembler.PopupException(msg2 + msig);
				return;
			}

			if (this.barStreamingLastDumpedLocal == DateTime.MinValue) {
				this.barStreamingLastDumpedLocal = quote.LocalTimeCreated;
			}
	
			double secondsSinceLastDumped = quote.LocalTimeCreated.Subtract(barStreamingLastDumpedLocal).TotalSeconds;
			if (ignoreIntervalForceReplaceBarImmediately == false && secondsSinceLastDumped <= this.barStreamingDumpIntervalSeconds) return;

			string millisElapsed;
			
			if (quote.ParentBarStreaming == null) {
				string msg2 = "STREAMING_SOLIDIFIER_FAILED_TO_STORE_LAST_QUOTE FIX_quote.ParentBarStreaming=null_HERE";
				Assembler.PopupException(msg2 + msig, null, false);
				return;
			}
			
			//lastQuoteReceived.SetParentBarStreaming(quoteConsumer.ConsumerBarsToAppendInto.BarStreaming);

			int barsSaved = this.dataSource.BarAppendOrReplaceLast(quote.ParentBarStreaming, out millisElapsed);

			msig = " //StreamingSolidifier.replaceStreamingBar(" + quote.ParentBarStreaming.Close + ")";
			string msg = millisElapsed
				+ " quote[" + quote.LocalTimeCreated.ToString("mm:ss.fff") + "]=>[" + this.barStreamingLastDumpedLocal.ToString("mm:ss.fff") + "]"
				+ " secondsSinceLastDumped[" + secondsSinceLastDumped.ToString("N3") + "] >= dumpInterval[" + this.barStreamingDumpIntervalSeconds + "]";
			// TOO_NOISY Assembler.PopupException(msg + msig, null, false);

			this.barStreamingLastDumpedLocal = quote.LocalTimeCreated;
			this.barStreamingLastDumpedLocalAsString = quote.LocalTimeCreated.ToString("HH:mm:ss.fff");
		}
		public override string ToString() {
			return "StreamingSolidifier[" + this.dataSource.ToString() + "]";
		}
	}
}
