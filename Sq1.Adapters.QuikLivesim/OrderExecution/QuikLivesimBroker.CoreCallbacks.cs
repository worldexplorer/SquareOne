using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Sq1.Core.Broker;
using Sq1.Core.Support;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core;
using Sq1.Core.Livesim;

namespace Sq1.Adapters.QuikLivesim {
	public partial class QuikLivesimBroker {
		
		//internal void ConsumeQuoteOfStreamingBarToFillPending(QuoteGenerated quoteUnattachedVolatilePointer, AlertList willBeFilled) { lock (this.threadEntryLockToHaveQuoteSentToThread) {
		//    ScriptExecutor executor = this.livesimDataSource.Executor;
		//    ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
		//    if (snap.AlertsPending.Count == 0) {
		//        string msg = "CHECK_IT_UPSTACK_AND_DONT_INVOKE_ME!!! snap.AlertsPending.Count=0 //ConsumeQuoteOfStreamingBarToFillPending(" + willBeFilled + ") ";
		//        //DISABLED_TO_SEE_WHAT_THAT_WILL_BRING
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }

		//    int delay = 0;
		//    if (this.settings.DelayBeforeFillEnabled) {
		//        delay = settings.DelayBeforeFillMillisMin;
		//        if (settings.DelayBeforeFillMillisMax > 0) {
		//            int range = Math.Abs(settings.DelayBeforeFillMillisMax - settings.DelayBeforeFillMillisMin);
		//            double rnd0to1 = new Random().NextDouble();
		//            int rangePart = (int)Math.Round(range * rnd0to1);
		//            delay += rangePart;
		//        }
		//    }
		//    if (delay == 0) {
		//        this.consumeQuoteOfStreamingBarToFillPendingAsync(quoteUnattachedVolatilePointer, willBeFilled);
		//        return;
		//    }

		//    AlertList priorDelayedFill = snap.AlertsPending;
		//    if (priorDelayedFill.Count == 0) return;
		//    ManualResetEvent quotePointerCaptured = new ManualResetEvent(false);
		//    Task t = new Task(delegate() {
		//        try {
		//            Thread.CurrentThread.Name = "DELAYED_FILL " + quoteUnattachedVolatilePointer;
		//        } catch (Exception ex) {
		//            Assembler.PopupException("CANT_SET_THREAD_NAME //LivesimBroker", ex, false);
		//        }
		//        QuoteGenerated quoteUnattachedLocalScoped = quoteUnattachedVolatilePointer;
		//        quotePointerCaptured.Set();

		//        executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
		//        //Application.DoEvents();
		//        Thread.Sleep(delay);
		//        AlertList afterDelay = snap.AlertsPending;
		//        if (afterDelay.Count == 0) return;
		//        if (priorDelayedFill.Count != afterDelay.Count) {
		//            string msg = "COUNT_MIGHT_HAVE_DECREASED_FOR_MULTIPLE_OPEN_POSITIONS/STRATEGY_IN_ANOTHER_FILLING_THREAD WHO_FILLED_WHILE_I_WAS_SLEEPING???";
		//            //Assembler.PopupException(msg);
		//            return;
		//        }
		//        this.consumeQuoteOfStreamingBarToFillPendingAsync(quoteUnattachedLocalScoped, willBeFilled);
		//        executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		//    });
		//    t.ContinueWith(delegate {
		//        string msg = "TASK_THREW_LivesimBroker.consumeQuoteOfStreamingBarToFillPendingAsync()";
		//        Assembler.PopupException(msg, t.Exception);
		//    }, TaskContinuationOptions.OnlyOnFaulted);
		//    t.Start();

		//    // I Sleep(10) since I wanna get quoteShadow pointer copied/stored inside the Task.Start()ed scope
		//    // before the parent thread (this one here) will drop/change quoteUnattached pointer upstack
		//    // so that after keeping the pointer I could launch another new Task
		//    // that's also why I used lock(this.threadEntryLockToHaveBarQuoteSentToThread)
		//    //Thread.Sleep(10);
		//    bool iCanContinue = quotePointerCaptured.WaitOne(1000);
		//    if (iCanContinue == false) {
		//        string msg = "DELAYED_FILL_THREAD_DIDNT_SIGNAL_THAT_QUOTE_POINTER_WAS_COPIED_DURING_1SECOND";
		//        Assembler.PopupException(msg);
		//    }

		//    List<Alert> safe = willBeFilled.SafeCopy(this, "//ConsumeQuoteOfStreamingBarToFillPending()");
		//    this.DataSnapshot.AlertsScheduledForDelayedFill.AddRange(safe, this, "ConsumeQuoteOfStreamingBarToFillPending(WAIT)");
		//} }
		//void consumeQuoteOfStreamingBarToFillPendingAsync(QuoteGenerated quoteUnattached, AlertList expectingToFill) {
		//    ScriptExecutor executor = this.livesimDataSource.Executor;
		//    Bar barStreaming = executor.Bars.BarStreamingNullUnsafe;
		//    if (barStreaming == null) {
		//        string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_WITH_BAR_STREAMING_NULL__END_OF_LIVESIM?";
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }
		//    if (executor.Backtester.IsBacktestingLivesimNow == false) {
		//        string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_LIVESIM_NOT_RUNNING__PROBABLY_STOPPED/ABORTED?";
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }
		//    ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
		//    if (snap.AlertsPending.Count == 0) {
		//        string msg = "CHECK_IT_UPSTACK_AND_DONT_INVOKE_ME!!! snap.AlertsPending.Count=0 //consumeQuoteOfStreamingBarToFillPendingAsync(" + expectingToFill + ")";
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }

		//    QuoteGenerated quoteAttachedToStreamingToConsumerBars = quoteUnattached.DeriveIdenticalButFresh();
		//    quoteAttachedToStreamingToConsumerBars.SetParentBarStreaming(barStreaming);
		//    // same pointer gets a cloned quote ATTACHED; anyone is listening for the change? it's not OUT
		//    //quoteUnattached = quoteAttachedToStreamingToConsumerBars;
		//    //if (quoteAttachedToStreamingToConsumerBars.ParentBarStreaming.ParentBars == null) {
		//    //	string msg = "STREAMING_BAR_UNATTACHED_REPLACED_TO_SIMULATED_BARS_STREAMING_BAR QUICK_AND_DIRTY_EARLY_BINDER_HERE";
		//    //	Assembler.PopupException(msg);
		//    //	string err = "NOT_FILLED_YET";
		//    //	bool same = quoteAttachedToStreamingToConsumerBars.ParentBarStreaming.HasSameDOHLCVas(
		//    //					barStreaming, "Executor.Bars.BarStreaming", "quote.ParentBarStreaming", ref err);
		//    //	if (same == false) {
		//    //		Assembler.PopupException("CANT_SUBSTITUTE__EXCEPTIONS_COMING" + err);
		//    //	} else {
		//    //		quoteAttachedToStreamingToConsumerBars.SetParentBarStreaming(this.livesimDataSource.Executor.Bars.BarStreaming);
		//    //	}
		//    //}

		//    //var dumped = snap.DumpPendingAlertsIntoPendingHistoryByBar();
		//    //int dumped = snap.AlertsPending.ByBarPlaced.Count;
		//    //if (dumped > 0) {
		//    //    //string msg = "here is at least one reason why dumping on fresh quoteToReach makes sense"
		//    //    //	+ " if we never reach this breakpoint the remove dump() from here"
		//    //    //	+ " but I don't see a need to invoke it since we dumped pendings already after OnNewBarCallback";
		//    //    string msg = "DUMPED_PRIOR_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
		//    //}
		//    int pendingCountPre = executor.ExecutionDataSnapshot.AlertsPending.Count;
		//    int pendingFilled = executor.MarketsimBacktest.SimulateFillAllPendingAlerts(
		//            quoteAttachedToStreamingToConsumerBars, new Action<Alert, double, double>(this.onAlertFilled));
		//    int pendingCountNow = executor.ExecutionDataSnapshot.AlertsPending.Count;
		//    if (pendingCountNow != pendingCountPre - pendingFilled) {
		//        string msg = "NOT_ONLY it looks like AnnihilateCounterparty worked out!";
		//    }
		//    if (pendingCountNow > 0) {
		//        string msg = "pending=[" + pendingCountNow + "], it must be prototype-induced 2 closing TP & SL";
		//    }
		//    //executor.Script.OnNewQuoteCallback(quoteToReach);

		//    ReporterPokeUnit pokeUnitNullUnsafe = executor.ExecuteOnNewBarOrNewQuote(quoteAttachedToStreamingToConsumerBars);
		//    //base.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate);
		//}
		//void onAlertFilled(Alert alertFilled, double priceFilled, double qtyFilled) {
		//    try {
		//        this.DataSnapshot.AlertsScheduledForDelayedFill.WaitAndLockFor(this, "onAlertFilled(WAIT)");
		//        if (this.DataSnapshot.AlertsScheduledForDelayedFill.Contains(alertFilled, this, "onAlertFilled(WAIT)")) {
		//            this.DataSnapshot.AlertsScheduledForDelayedFill.Remove(alertFilled, this, "onAlertFilled(WAIT)");
		//        }
		//    } finally {
		//        this.DataSnapshot.AlertsScheduledForDelayedFill.UnLockFor(this, "onAlertFilled(WAIT)");
		//    }
		//    Order order = alertFilled.OrderFollowed;
		//    if (order == null && alertFilled.SignalName.StartsWith("proto")) {
		//        string msg = "CORE_FORGOT_TO_CREATE_TWO_ORDERS_FOR_POSITION_PROTOTYPE";
		//        Assembler.PopupException(msg);
		//        return;
		//    }
		//    OrderStateMessage osm = new OrderStateMessage(order, OrderState.Filled, "LIVESIM_FILLED_THROUGH_MARKETSIM_BACKTEST");
		//    OrderProcessor orderProcessor = Assembler.InstanceInitialized.OrderProcessor;
		//    orderProcessor.UpdateOrderStateAndPostProcess(order, osm, priceFilled, qtyFilled);
		//    if (alertFilled.PriceFilledThroughPosition != priceFilled) {
		//        string msg = "WHO_FILLS_POSITION_PRICE_FILLED_THEN?";
		//    }
		//    if (alertFilled.QtyFilledThroughPosition != qtyFilled) {
		//        string msg = "WHO_FILLS_POSITION_QTY_FILLED_THEN?";
		//    }
		//}

	}
}