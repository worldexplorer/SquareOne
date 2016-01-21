using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Backtesting {
	public class BacktestQuoteBarConsumer : StreamingConsumer {
				Backtester backtester;
		public	BacktestQuoteBarConsumer(Backtester backtester) {
			this.backtester = backtester;
		}

		#region StreamingConsumer
		public	override ScriptExecutor	Executor			{ get {
				var ret = this.backtester.Executor;
				base.ActionForNullPointer(ret, "this.backtester.Executor=null");
				return ret;
			} }

		public override Bars ConsumerBarsToAppendInto { get { return this.backtester.BarsSimulating; } }
		public override void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		public override void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
		}
		public override void ConsumeQuoteOfStreamingBar(Quote quote) {
			//Bar barLastFormed = quoteToReach.ParentStreamingBar;
			ExecutionDataSnapshot snap = this.backtester.Executor.ExecutionDataSnapshot;

			if (snap.AlertsPending.Count > 0) {
				//var dumped = snap.DumpPendingAlertsIntoPendingHistoryByBar();
				//int dumped = snap.AlertsPending.ByBarPlaced.Count;
				//if (dumped > 0) {
				//    //string msg = "here is at least one reason why dumping on fresh quoteToReach makes sense"
				//    //	+ " if we never reach this breakpoint the remove dump() from here"
				//    //	+ " but I don't see a need to invoke it since we dumped pendings already after OnNewBarCallback";
				//    string msg = "DUMPED_BEFORE_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
				//}
				int pendingCountPre	= this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				int pendingFilled	= this.backtester.Executor.MarketsimBacktest.SimulateFillAllPendingAlerts(quote);
				int pendingCountNow	= this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingCountNow != pendingCountPre - pendingFilled) {
					string msg = "NOT_ONLY it looks like AnnihilateCounterparty worked out!";
				}
				if (pendingCountNow > 0) {
					string msg = "pending=[" + pendingCountNow + "], it must be prototype-induced 2 closing TP & SL";
				}
			}
			//this.backtester.Executor.Script.OnNewQuoteCallback(quoteToReach);
			ReporterPokeUnit pokeUnitNullUnsafe = this.backtester.Executor.ExecuteOnNewBarOrNewQuote(quote);
		}
		public override void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			string msig = " //BacktestQuoteBarConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended";
			if (barLastFormed == null) {
				string msg = "THERE_IS_NO_STATIC_BAR_DURING_FIRST_4_QUOTES_GENERATED__ONLY_STREAMING"
					+ " Backtester starts generating quotes => first StreamingBar is added;"
					+ " for first four Quotes there's no static barsFormed yet!! Isi";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			msig += "(" + barLastFormed.ToString() + ")";
			//v1 this.backtester.Executor.Strategy.Script.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(barLastFormed);
			ReporterPokeUnit pokeUnitNullUnsafe = this.backtester.Executor.ExecuteOnNewBarOrNewQuote(quoteForAlertsCreated, false);
		}
		#endregion

		public override string ToString() {
			string ret = "CONSUMER_FOR_" + this.backtester.ToString();
			return ret;
		}
	}
}
