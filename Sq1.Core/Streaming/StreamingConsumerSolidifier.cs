using System;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public class StreamingConsumerSolidifier : StreamingConsumer {
		DataSource	dataSource;

		double		barStreaming_dumpIntervalSeconds;
		DateTime	barStreaming_lastDumpedLocal;
		string		barStreaming_lastDumpedLocal_asString;

		//IMPORTANT_SPLITTED_TO_CTOR+INITIALIZE_FOR_EDITED_DATASOURCE_TO_NOT_SUBSCRIBE_MY_MULTIPLE_INSTANCES
		public StreamingConsumerSolidifier() { }

		public void Initialize(DataSource dataSource, double barStreaming_dumpIntervalSeconds = 10) {
			if (dataSource == null) {
				string msg = "DATASOURCE_NULL_ABSOLUTELY_INACCEPTABLE_FOR_SOLIDIFIER";
				throw new Exception(msg);
			}
			this.dataSource = dataSource;
			this.barStreaming_dumpIntervalSeconds = barStreaming_dumpIntervalSeconds;
			this.barStreaming_lastDumpedLocal = DateTime.MinValue;
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
		public override void Consume_quoteOfStreamingBar(Quote quoteWith_pseudoExpanded) {
			this.barsSaveToFile_replaceStreamingBar(quoteWith_pseudoExpanded);
		}
		public override void Consume_barLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barStaticLast_justFormed_byBarsEmulator, Quote quoteWithPseudo) {
			string millisTook_updateLast;
			string millisTook_appendNew;
			int barsReplaced = this.dataSource.BarAppend_orReplaceLast(barStaticLast_justFormed_byBarsEmulator	, out millisTook_updateLast);
			int barsAppended = this.dataSource.BarAppend_orReplaceLast(quoteWithPseudo.ParentBarStreaming		, out millisTook_appendNew);	// will be OHLCV=NaN
			string msg = millisTook_updateLast + " barLastFormed[" + barStaticLast_justFormed_byBarsEmulator + "] into DataSource[" + this.dataSource.Name + "]";
			Assembler.PopupException(msg, null, false);
		}
		public override void Consume_levelTwoChanged_noNewQuote(LevelTwoFrozen levelTwoFrozen) {
			string msg = "SOLIDIFIER_DOESNT_SAVE_LEVELS_TWO____HOW_DO_YOU_WANT_YOUR_COFFEE?...";
		}
		#endregion

		void barsSaveToFile_replaceStreamingBar(Quote quoteWith_pseudoExpanded, bool ignoreInterval_forceReplaceBar_immediately = false) {
			string msig = " //StreamingConsumerSolidifier.barsSaveToFile_replaceStreamingBar(" + quoteWith_pseudoExpanded + ")";
			if (quoteWith_pseudoExpanded is QuoteGenerated) {
				string msg2 = "I_REFUSE_TO_STORE_QuoteGenerated_INTO_FILE [" + quoteWith_pseudoExpanded.Symbol + "].bar";
				Assembler.PopupException(msg2 + msig);
				return;
			}

			if (this.barStreaming_lastDumpedLocal == DateTime.MinValue) {
				this.barStreaming_lastDumpedLocal = quoteWith_pseudoExpanded.LocalTime;
			}
	
			double secondsSinceLastDumped = quoteWith_pseudoExpanded.LocalTime.Subtract(barStreaming_lastDumpedLocal).TotalSeconds;
			if (ignoreInterval_forceReplaceBar_immediately == false && secondsSinceLastDumped <= this.barStreaming_dumpIntervalSeconds) return;

			string millisElapsed;
			

			Bar quoteParentBarStreaming = quoteWith_pseudoExpanded.ParentBarStreaming;
			if (quoteParentBarStreaming == null) {
				string msg2 = "STREAMING_SOLIDIFIER_FAILED_TO_STORE_LAST_QUOTE FIX_quote.ParentBarStreaming=null_HERE";
				Assembler.PopupException(msg2 + msig);
				return;
			}
			if (quoteParentBarStreaming.ParentBars == null) {
				string msg1 = "AVOIDING_NPE_FOR_CHECKS_2_LINES_BELOW";
			} else {
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
			}
			//lastQuoteReceived.SetParentBarStreaming(quoteConsumer.ConsumerBarsToAppendInto.BarStreaming);

			int barsSaved = this.dataSource.BarAppend_orReplaceLast(quoteParentBarStreaming, out millisElapsed);

			msig = " //StreamingConsumerSolidifier.barsSaveToFile_replaceStreamingBar(" + quoteWith_pseudoExpanded.ParentBarStreaming.Close + ")";
			string msg = millisElapsed
				+ " quote[" + quoteWith_pseudoExpanded.LocalTime.ToString("mm:ss.fff") + "]=>[" + this.barStreaming_lastDumpedLocal.ToString("mm:ss.fff") + "]"
				+ " secondsSinceLastDumped[" + secondsSinceLastDumped.ToString("N3") + "] >= dumpInterval[" + this.barStreaming_dumpIntervalSeconds + "]";
			// TOO_NOISY Assembler.PopupException(msg + msig, null, false);

			this.barStreaming_lastDumpedLocal = quoteWith_pseudoExpanded.LocalTime;
			this.barStreaming_lastDumpedLocal_asString = quoteWith_pseudoExpanded.LocalTime.ToString("HH:mm:ss.fff");
		}
		public override string ToString() {
			return "StreamingConsumerSolidifier_FOR_DATASOURCE[" + this.dataSource.ToString() + "]";
		}
	}
}
