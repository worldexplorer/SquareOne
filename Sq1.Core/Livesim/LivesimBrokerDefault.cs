using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Support;
using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Livesim {
	//I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR => SkipInstantiationAt(Startup = false)
	[SkipInstantiationAt(Startup = false)]
	public sealed partial class LivesimBrokerDefault : LivesimBroker, IDisposable {
		[JsonIgnore]				object						threadEntryLockToHaveQuoteSentToThread;

		public LivesimBrokerDefault() : base("USED_FOR_DATASOURCE_EDITOR_DUMMY") {
			string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING"
				//+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
				+ "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
				+ " example:QuikLivesimBroker()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
			base.Name = "LivesimBrokerDefault-child_ACTIVATOR_DLL-SCANNED";
			this.threadEntryLockToHaveQuoteSentToThread	= new object();
		}

		public LivesimBrokerDefault(string reasonToExist) : base(reasonToExist) {
			base.Name						= "LivesimBrokerDefault_WILL_BE_RENAMED_IN-InitializeDataSource_inverse()";
			this.threadEntryLockToHaveQuoteSentToThread	= new object();
		}

		public override void InitializeLivesim(LivesimDataSource livesimDataSource, OrderProcessor orderProcessor) {
			base.Name						= "LivesimBrokerDefault";
			base.ReasonToExist				= "USED_FOR_LIVESIMMING_INITED_FROM_DATASOURCE";
			base.InitializeLivesim(livesimDataSource, orderProcessor);
		}




		//public void ConsumeQuoteUnattached_toFillPending(QuoteGenerated quoteUnattachedVolatilePointer, AlertList willBeFilled) { lock (this.threadEntryLockToHaveQuoteSentToThread) {
		public void ConsumeQuoteBoundUnattached_toFillPending(Quote quoteBoundUnattached_volatilePointer, AlertList willBeFilled) { lock (this.threadEntryLockToHaveQuoteSentToThread) {
			string msig = " //LivesimBroker.ConsumeQuoteBoundUnattached_toFillPending(WAIT)";

			//if (quoteUnattached_volatilePointer.ParentBarStreaming != null) {
			//    string msg = "QUOTE_ALREADY_BOUND_TO_STREAMING_BAR__UPSTACK_DIDNT_REALIZE_THIS";
			//    Assembler.PopupException(msg + msig, null, false);
			//}
			if (quoteBoundUnattached_volatilePointer.ParentBarStreaming.HasParentBars == false) {
				string msg = "QUOTE_BOUND_TO_A_STREAMING_BAR_THAT_MUST_BE_ATTACHED_TO_BARS_LIVEMMING__UPSTACK_DIDNT_REALIZE_THIS";
				Assembler.PopupException(msg + msig, null, false);
			}

			if (willBeFilled.Count == 0) return;

			int delayBeforeFill = base.LivesimBrokerSpoiler.DelayBeforeFill_calculate();
			if (delayBeforeFill == 0) {
				this.consumeQuoteUnattached_attach_fillPendingAsync(quoteBoundUnattached_volatilePointer, willBeFilled);
				return;
			}

			AlertList willBeFilled_minusAlreadyScheduled_volatilePointer = willBeFilled.Substract_returnClone(base.DataSnapshot.AlertsPending_scheduledForDelayedFill, this, "willBeFilled_minusAlreadyScheduled");

			AlertList priorDelayedFill = base.ScriptExecutor.ExecutionDataSnapshot.AlertsPending;
			if (priorDelayedFill.Count == 0) return;

			ManualResetEvent quotePointerCaptured = new ManualResetEvent(false);
			Task t = new Task(delegate() {
				string threadName = "DELAYED_FILL delayBeforeFill[" + delayBeforeFill + "]ms " + quoteBoundUnattached_volatilePointer;
				Assembler.SetThreadName(threadName, "CANT_SET_THREAD_NAME" + msig);

				AlertList willBeFilled_minusAlreadyScheduled_localScoped = willBeFilled_minusAlreadyScheduled_volatilePointer;
				//QuoteGenerated quoteUnattachedLocalScoped = quoteUnattachedVolatilePointer;
				Quote quoteBoundUnattached_localScoped = quoteBoundUnattached_volatilePointer;
				quotePointerCaptured.Set();

				//base.ScriptExecutor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
				base.LivesimBrokerSpoiler.DelayBeforeFill_threadSleep();
				AlertList afterDelay = base.ScriptExecutor.ExecutionDataSnapshot.AlertsPending;
				if (afterDelay.Count == 0) return;
				if (priorDelayedFill.Count != afterDelay.Count) {
				    string msg = "COUNT_MIGHT_HAVE_DECREASED_FOR_MULTIPLE_OPEN_POSITIONS/STRATEGY_IN_ANOTHER_FILLING_THREAD WHO_FILLED_WHILE_I_WAS_SLEEPING???";
				    //Assembler.PopupException(msg);
				    return;
				}
				this.consumeQuoteUnattached_attach_fillPendingAsync(quoteBoundUnattached_localScoped, willBeFilled_minusAlreadyScheduled_localScoped);
				base.ScriptExecutor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			});
			t.ContinueWith(delegate {
				string msg = "FILL_TASK_THREW";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();		// WHO_DOES t.Dispose() ?

			// I Sleep(10) since I wanna get quoteShadow pointer copied/stored inside the Task.Start()ed scope
			// before the parent thread (this one here) will drop/change quoteUnattached pointer upstack
			// so that after keeping the pointer I could launch another new Task
			// that's also why I used lock(base.threadEntryLockToHaveBarQuoteSentToThread)
			//Thread.Sleep(10);

			int waitingForever_becauseMreWillBeDisposed_ifVisualStudioIsPaused_forAnotherExceptionFromAnotherThread = -1;
			#if DEBUG
				//waitingForever_becauseMreWillBeDisposed_ifVisualStudioIsPaused_forAnotherExceptionFromAnotherThread = 1000;
			#endif

			bool iCanContinue = quotePointerCaptured.WaitOne(waitingForever_becauseMreWillBeDisposed_ifVisualStudioIsPaused_forAnotherExceptionFromAnotherThread);
			quotePointerCaptured.Dispose();
			//"CAN_BE_DISPOSED_ONLY_IF_RAN_TILL_COMPLETION...." t.Dispose();
			quotePointerCaptured = null;
			if (iCanContinue == false) {
				string msg = "HAPPENS_WHEN_ANOTHER_THREAD_HITS_BREAKPOINT_IN_VS DELAYED_FILL_THREAD_DIDNT_SIGNAL_THAT_QUOTE_POINTER_WAS_COPIED_DURING_1SECOND";
				Assembler.PopupException(msg, null, false);
			}

			//List<Alert> safe = willBeFilled.SafeCopy(this, "//ConsumeQuoteUnattached_toFillPending()");
			List<Alert> safe = willBeFilled_minusAlreadyScheduled_volatilePointer.SafeCopy(this, msig);
			base.DataSnapshot.AlertsPending_scheduledForDelayedFill.AddRange(safe, this, msig);
		} }
		//void consumeQuoteUnattached_toFillPendingAsync(QuoteGenerated quoteUnattached, AlertList expectingToFill) {
		void consumeQuoteUnattached_attach_fillPendingAsync(Quote quoteBoundUnattached, AlertList expectingToFill) {
			Quote quoteBoundAttached = null;
			if (quoteBoundUnattached.ParentBarStreaming.HasParentBars) {
				string msg = "IM_FILLING_PENDING__NOT_SETTING_TO_STREAMING_SNAP__IT_WOULD_THROW"
					+ " QUOTE_BOUND_TO_A_STREAMING_BAR_THAT_IS_ALREADY_ATTACHED_TO_BARS_LIVEMMING__UPSTACK_DIDNT_REALIZE_THIS";
				//Assembler.PopupException(msg, null, false);
				quoteBoundAttached = quoteBoundUnattached;
			} else {
				Bar barStreaming = base.ScriptExecutor.Bars.BarStreaming_nullUnsafe;
				if (barStreaming == null) {
					string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_WITH_BAR_STREAMING_NULL__END_OF_LIVESIM?";
					Assembler.PopupException(msg, null, false);
					return;
				}
				if (base.ScriptExecutor.BacktesterOrLivesimulator.ImRunningLivesim == false) {
					string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_LIVESIM_NOT_RUNNING__PROBABLY_STOPPED/ABORTED?";
					Assembler.PopupException(msg, null, false);
					return;
				}
				ExecutorDataSnapshot snap = base.ScriptExecutor.ExecutionDataSnapshot;
				if (snap.AlertsPending.Count == 0) {
					string msg = "CHECK_IT_UPSTACK_AND_DONT_INVOKE_ME!!! snap.AlertsPending.Count=0 //consumeQuoteUnattached_toFillPendingAsync(" + expectingToFill + ")";
					Assembler.PopupException(msg, null, false);
					return;
				}

				//QuoteGenerated quoteAttachedToStreamingToConsumerBars = quoteUnattached.DeriveIdenticalButFresh();
				quoteBoundAttached = quoteBoundUnattached.Clone_asCoreQuote();	// do you really need a clone here???
				quoteBoundAttached.StreamingBar_Replace(barStreaming);
			}

			int pendingCountPre = base.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Count;
			//int pendingFilled = executor.MarketsimBacktest.SimulateFillAllPendingAlerts(
			int pendingFilled = base.LivesimMarketsim.SimulateFill_allPendingAlerts(quoteBoundAttached,
					new Action<Alert, Quote, double, double>(this.action_afterAlertFilled_inducePostProcessing_movedAroundOnReturn));
			int pendingCountNow = base.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Count;
			if (pendingCountNow != pendingCountPre - pendingFilled) {
				string msg = "NOT_ONLY it looks like AnnihilateCounterparty worked out!";
			}
			if (pendingCountNow > 0) {
				string msg = "pending=[" + pendingCountNow + "], it must be prototype-induced 2 closing TP & SL";
			}
			//executor.Script.OnNewQuoteCallback(quoteToReach);

			this.ScriptExecutor.InvokeIndicators_onNewBar_onNewQuote(null, quoteBoundAttached, true);
			ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = base.ScriptExecutor.InvokeScript_onNewBar_onNewQuote(quoteBoundAttached);
			//base.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate);
			if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
				pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
			}
		}
		void action_afterAlertFilled_inducePostProcessing_movedAroundOnReturn(Alert alertFilled, Quote quoteFilledThisAlert, double priceFilled, double qtyFilled) {
			string msig = " //onAlertFilled(WAIT)";

			try {
				base.DataSnapshot.AlertsPending_scheduledForDelayedFill.WaitAndLockFor(this, msig);
				if (base.DataSnapshot.AlertsPending_scheduledForDelayedFill.Contains(alertFilled, this, msig)) {
					base.DataSnapshot.AlertsPending_scheduledForDelayedFill.Remove(alertFilled, this, msig);
				}
			} finally {
				base.DataSnapshot.AlertsPending_scheduledForDelayedFill.UnLockFor(this, msig);
			}
			Order order = alertFilled.OrderFollowed;
			if (order == null && alertFilled.SignalName.StartsWith("proto")) {
				string msg = "CORE_FORGOT_TO_CREATE_TWO_ORDERS_FOR_POSITION_PROTOTYPE";
				Assembler.PopupException(msg);
				return;
			}

			// essential for a postProcess to have a quoteFilled inside the alert already, to moveAround (illogical I know)
			base.ScriptExecutor.EnrichAlerts_withQuoteCreated(new List<Alert>() {alertFilled}, quoteFilledThisAlert);

			OrderStateMessage osm = new OrderStateMessage(order, OrderState.Filled, "LIVESIM_FILLED_THROUGH_MARKETSIM_BACKTEST");
			OrderProcessor orderProcessor = Assembler.InstanceInitialized.OrderProcessor;
			orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(osm, priceFilled, qtyFilled);
			
			if (alertFilled.PriceFilledThroughPosition != priceFilled) {
				string msg = "WHO_FILLS_POSITION_PRICE_FILLED_THEN?";
				Assembler.PopupException(msg);
			}
			if (alertFilled.QtyFilledThroughPosition != qtyFilled) {
				string msg = "WHO_FILLS_POSITION_QTY_FILLED_THEN?";
				Assembler.PopupException(msg);
			}
		}
	}
}
