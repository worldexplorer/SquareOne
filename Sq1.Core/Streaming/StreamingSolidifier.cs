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
		public override Bars ConsumerBars_toAppendInto { get {
			throw new Exception("YOU_SHOULD_NOT_ACCESS_BARS_OF_STREAMING_SOLIDIFIER__THEY_ARE_WRITE_ONLY_THOUGH_SOLIDIFIER_METHODS");
		} }

		public override void UpstreamSubscribed_toSymbol_streamNotifiedMe(Quote quoteFirstAfterStart) {
		}
		public override void UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(Quote quoteLastBeforeStop) {
			this.barsSaveToFile_replaceStreamingBar(quoteLastBeforeStop, true);
		}
		public override void ConsumeQuoteOfStreamingBar(Quote quoteClone_boundAttached) {
			this.barsSaveToFile_replaceStreamingBar(quoteClone_boundAttached);
		}
		public override void ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barStaticLast_justFormed_byEarlyBinder, Quote quoteForAlertsCreated_WILL_BE_NULL) {
			string millisSavingTook;
			if (barStaticLast_justFormed_byEarlyBinder.IsBarStreaming) {
				string msg1 = "DONT_PASS_STREAMING_BAR_HERE__UNMESS_FILESEEKS_OR_BINDER";
				Assembler.PopupException(msg1);
			}
			int barsSaved = this.dataSource.BarAppend_orReplaceLast(barStaticLast_justFormed_byEarlyBinder, out millisSavingTook);
			string msg = millisSavingTook + " barLastFormed[" + barStaticLast_justFormed_byEarlyBinder + "] from streaming DataSource[" + this.dataSource.Name + "]";
			Assembler.PopupException(msg, null, false);
		}
		#endregion

		void barsSaveToFile_replaceStreamingBar(Quote quote, bool ignoreInterval_forceReplaceBar_immediately = false) {
			string msig = " //StreamingSolidifier.replaceStreamingBar(" + quote + ")";
			if (quote is QuoteGenerated) {
				string msg2 = "I_REFUSE_TO_STORE_QuoteGenerated_INTO_FILE [" + quote.Symbol + "].bar";
				Assembler.PopupException(msg2 + msig);
				return;
			}

			if (this.barStreamingLastDumpedLocal == DateTime.MinValue) {
				this.barStreamingLastDumpedLocal = quote.LocalTime;
			}
	
			double secondsSinceLastDumped = quote.LocalTime.Subtract(barStreamingLastDumpedLocal).TotalSeconds;
			if (ignoreInterval_forceReplaceBar_immediately == false && secondsSinceLastDumped <= this.barStreamingDumpIntervalSeconds) return;

			string millisElapsed;
			

			Bar quoteParentBarStreaming = quote.ParentBarStreaming;
			if (quoteParentBarStreaming == null) {
				string msg2 = "STREAMING_SOLIDIFIER_FAILED_TO_STORE_LAST_QUOTE FIX_quote.ParentBarStreaming=null_HERE";
				Assembler.PopupException(msg2 + msig);
				return;
			}
			if (quoteParentBarStreaming.IsBarStaticLast) {
				string msg1 = "DONT_PASS_STATIC_BAR_HERE__UNMESS_FILESEEKS_OR_BINDER";
				Assembler.PopupException(msg1);
				return;
			}
			if (quoteParentBarStreaming.IsBarStreaming == false) {
				string msg1 = "I_REFUSE_TO_SAVE NOT_STATIC NOT_STREAMING WHAT_BAR_IS_THIS???";
				Assembler.PopupException(msg1);
				return;
			}
			
			//lastQuoteReceived.SetParentBarStreaming(quoteConsumer.ConsumerBarsToAppendInto.BarStreaming);

			int barsSaved = this.dataSource.BarAppend_orReplaceLast(quoteParentBarStreaming, out millisElapsed);

			msig = " //StreamingSolidifier.replaceStreamingBar(" + quote.ParentBarStreaming.Close + ")";
			string msg = millisElapsed
				+ " quote[" + quote.LocalTime.ToString("mm:ss.fff") + "]=>[" + this.barStreamingLastDumpedLocal.ToString("mm:ss.fff") + "]"
				+ " secondsSinceLastDumped[" + secondsSinceLastDumped.ToString("N3") + "] >= dumpInterval[" + this.barStreamingDumpIntervalSeconds + "]";
			// TOO_NOISY Assembler.PopupException(msg + msig, null, false);

			this.barStreamingLastDumpedLocal = quote.LocalTime;
			this.barStreamingLastDumpedLocalAsString = quote.LocalTime.ToString("HH:mm:ss.fff");
		}
		public override string ToString() {
			return "StreamingSolidifier[" + this.dataSource.ToString() + "]";
		}
	}
}
