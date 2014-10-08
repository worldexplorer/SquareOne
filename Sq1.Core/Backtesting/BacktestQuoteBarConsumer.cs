using System;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.Indicators;

namespace Sq1.Core.Backtesting {
	public class BacktestQuoteBarConsumer : IStreamingConsumer {
		Backtester backtester;
		public BacktestQuoteBarConsumer(Backtester backtester) {
			this.backtester = backtester;
		}
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { return backtester.BarsSimulating; } }
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			//Bar barLastFormed = quoteToReach.ParentStreamingBar;
			ExecutionDataSnapshot snap = this.backtester.Executor.ExecutionDataSnapshot;

			// INDICATORS_CLEARED_ADDED_AFTER_BACKTEST_STARTED "Collection was modified; enumeration operation may not execute."
			// ALSO_OBSERVED_RELATED: INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS
			foreach (Indicator indicator in snap.Indicators.Values) {
				try {
					indicator.OnNewStreamingQuote(quote);
				} catch (Exception ex) {
					Debugger.Break();
				}
			}

			if (snap.AlertsPending.Count > 0) {
				var dumped = snap.DumpPendingAlertsIntoPendingHistoryByBar();
				if (dumped > 0) {
					//string msg = "here is at least one reason why dumping on fresh quoteToReach makes sense"
					//    + " if we never reach this breakpoint the remove dump() from here"
					//    + " but I don't see a need to invoke it since we dumped pendings already after OnNewBarCallback";
					string msg = "DUMPED_BEFORE_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
				}
				int pendingCountPre = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				int pendingFilled = this.backtester.Executor.MarketSimStreaming.SimulatePendingFill(quote);
				int pendingCountNow = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingCountNow != pendingCountPre - pendingFilled) {
					string msg = "NOT_ONLY it looks like AnnihilateCounterparty worked out!";
				}
				if (pendingCountNow > 0) {
					string msg = "pending=[" + pendingCountNow + "], it must be prototype-induced 2 closing TP & SL";
				}
			}
			//this.backtester.Executor.Script.OnNewQuoteCallback(quoteToReach);
			ReporterPokeUnit pokeUnit = this.backtester.Executor.ExecuteOnNewBarOrNewQuote(quote);
		}
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			if (barLastFormed == null) {
				string msg = "Backtester starts generating quotes => first StreamingBar is added; for first four Quotes there's no static barsFormed yet!! Isi";
				return;
			}
			//INVOCATION_WONT_DO_ANY_JOB this.simulatePendingFillPreExecuteEveryTick(null);
			ExecutionDataSnapshot snap = this.backtester.Executor.ExecutionDataSnapshot;
			foreach (Indicator indicator in snap.Indicators.Values) {
				// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
				//if (indicator.NotOnChartBarsKey != null) {
				//    string msg = "Generate quotes for the Non-Chart-Bars and feed them into your indicators!";
				//    continue;
				//}
				indicator.OnNewStaticBarFormed(barLastFormed);
			}

			this.backtester.Executor.Strategy.Script.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(barLastFormed);
		}
	}
}
