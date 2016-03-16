using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	// I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR [SkipInstantiationAt(Startup = true)]
	public abstract partial class LivesimBroker : BacktestBroker, IDisposable {
		[JsonIgnore]	public		List<Order>					OrdersSubmittedForOneLivesimBacktest	{ get; private set; }
		[JsonIgnore]	protected	LivesimDataSource			LivesimDataSource						{ get { return base.DataSource as LivesimDataSource; } }
		[JsonIgnore]	internal	LivesimBrokerSettings		LivesimBrokerSettings					{ get { return this.LivesimDataSource.Executor.Strategy.LivesimBrokerSettings; } }
		[JsonIgnore]				object						threadEntryLockToHaveQuoteSentToThread;
		[JsonIgnore]	public		LivesimBrokerDataSnapshot	DataSnapshot;
		[JsonIgnore]	protected	LivesimBrokerSpoiler		LivesimBrokerSpoiler;

		[JsonIgnore]	public		bool						IsDisposed								{ get; private set; }

		//protected LivesimBroker() : base("DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING") {
		//	string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_BROKER"
		//		//+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		//		+ "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_BROKER_ADAPTERS_FOUND"
		//		+ " example:QuikBrokerLivesim()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
		//	base.Name = "LivesimBroker-child_ACTIVATOR_DLL-SCANNED";
		//}
		public LivesimBroker(string reasonToExist) : base(reasonToExist) {
			base.Name									= "LivesimBroker";
			base.AccountAutoPropagate					= new Account("LIVESIM_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
			this.OrdersSubmittedForOneLivesimBacktest	= new List<Order>();
			this.threadEntryLockToHaveQuoteSentToThread	= new object();
			this.LivesimBrokerSpoiler					= new LivesimBrokerSpoiler(this);
		}
		public virtual void InitializeLivesim(LivesimDataSource livesimDataSource, OrderProcessor orderProcessor) {
			base.DataSource		= livesimDataSource;
			this.DataSnapshot	= new LivesimBrokerDataSnapshot(this.LivesimDataSource);
			base.InitializeDataSource_inverse(livesimDataSource, this.LivesimDataSource.StreamingAsLivesim_nullUnsafe,  orderProcessor);
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			LivesimBrokerEditorEmpty emptyEditor = new LivesimBrokerEditorEmpty();
			emptyEditor.Initialize(this, dataSourceEditor);
			this.BrokerEditorInstance = emptyEditor;
			return emptyEditor;
		}

		public override void Order_submit(Order order) {
			string msig = " //LivesimBroker.Order_submit()";

			this.OrdersSubmittedForOneLivesimBacktest.Add(order);
//			string msg = "IS_ASYNC_CALLBACK_NEEDED? THREAD_ID " + Thread.CurrentThread.ManagedThreadId;
//			base.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(order.GUID, OrderState.Submitted, msg);

			OrderStateMessage omsg_emitting = new OrderStateMessage(order, OrderState.Submitted, "SIMULATING_ORDER_EMITTING" + msig);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_emitting);

			OrderStateMessage omsg_confirmed = new OrderStateMessage(order, OrderState.WaitingBrokerFill, "SIMULATING_BROKER_CONFRIMED_SUBMIT" + msig);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed);
		}

		public override void Order_killPending_usingKiller(Order orderKiller) {
			string symbol = " (" + orderKiller.Alert.Symbol + "," + orderKiller.Alert.SymbolClass + ")";
			string msigHead = "State[" + orderKiller.State + "]"
				+ symbol
				+ " VictimToBeKilled.SernoExchange[" + orderKiller.VictimToBeKilled.SernoExchange + "]";
			string msig = msigHead + " //LivesimBroker.Order_killPending_usingKiller()";

			if (string.IsNullOrEmpty(orderKiller.VictimGUID)) {
				string msg = "killerOrder.KillerForGUID=EMPTY";
				orderKiller.AppendMessage(msg);
				throw new Exception(msg);
			}
			if (orderKiller.VictimToBeKilled == null) {
				string msg = "killerOrder.VictimToBeKilled=null";
				orderKiller.AppendMessage(msg);
				throw new Exception(msg);
			}

			Order orderVictim = orderKiller.VictimToBeKilled;

		
			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerPreSubmit + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omg_preSubmit_killer = new OrderStateMessage(orderKiller, OrderState.KillerPreSubmit, "SIMULATING_PREPARING_KILL_COMMAND_FOR_BROKER" + msigHead);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_preSubmit_killer);

			msigHead = "orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimsBulletPreSubmit + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_preSubmit_victim = new OrderStateMessage(orderVictim, OrderState.VictimsBulletPreSubmit, "SIMULATING_PREPARING_KILL_COMMAND_FOR_BROKER__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_preSubmit_victim);


			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerTransSubmittedOK + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omg_transSubmitted_killer = new OrderStateMessage(orderKiller, OrderState.KillerTransSubmittedOK, "SIMULATING_SENDING_KILL_TRANSACTION" + msigHead);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_transSubmitted_killer);

			msigHead = "orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimsBulletConfirmed + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_confirmed_victim = new OrderStateMessage(orderVictim, OrderState.VictimsBulletConfirmed, "SIMULATING_SENDING_KILL_TRANSACTION__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed_victim);


			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.WaitingBrokerFill + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omsg_confirmed_killer = new OrderStateMessage(orderKiller, OrderState.WaitingBrokerFill, "SIMULATING_BROKER_CONFRIMED_RECEPTION_KILL_ORDER (not really expected for killer to get this status)" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed_killer);

			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerBulletFlying + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omsg_waitingFill_killer = new OrderStateMessage(orderKiller, OrderState.KillerBulletFlying, "SIMULATING_WAITING_KILL_DELAY_ON_BROKER_SIDE" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_waitingFill_killer);

			msigHead = "orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimsBulletFlying + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_waitingFill_victim = new OrderStateMessage(orderVictim, OrderState.VictimsBulletFlying, "SIMULATING_WAITING_KILL_DELAY_ON_BROKER_SIDE__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_waitingFill_victim);


			//KillerDone is set in victim_PostProcessing on VictimKilled
			//msigHead = " orderKiller[" + orderVictim.SernoExchange + "][" + orderKiller.State + "] => orderVictim[" + orderVictim.SernoExchange + "][" + orderKiller.State + "]";
			//OrderStateMessage omg_done_killer = new OrderStateMessage(orderKiller, OrderState.KillerDone, "SIMULATING_KILLER_ORDER_COMPLETE_CALLBACK" + msigHead);
			//this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_done_killer);

			msigHead = "orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimKilled + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_killed_victim = new OrderStateMessage(orderVictim, OrderState.VictimKilled, "SIMULATING_VICTIM_KILLED_CALLBACK" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_killed_victim);

		}

		//public override void Order_killPending_withoutKiller(Order orderPendingToKill) {
		//    string msig = " //LivesimBroker.Order_killPending_withoutKiller()";
			
		//    string orderGUID = orderPendingToKill.GUID;
		//    Order orderPendingFound = base.ScanEvidentLanes_forGuid_nullUnsafe(orderGUID);
		//    if (orderPendingFound != orderPendingToKill) {
		//        string msg = "PARANOID_SCAN_FAILED orderPendingFound[" + orderPendingFound + "] != orderPendingToKill[" + orderPendingToKill + "]";
		//        Assembler.PopupException(msg);
		//    }

		//    var omsg2 = new OrderStateMessage(orderPendingToKill, OrderState.VictimsBulletSubmitting, "Step#2");
		//    base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(omsg2);

		//    var omsg3 = new OrderStateMessage(orderPendingToKill, OrderState.VictimsBulletSubmitted, "Step#3");
		//    base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(omsg3);

		//    int delay = 0;
		//    if (this.LivesimBrokerSettings.KillPendingDelayEnabled) {
		//        delay = LivesimBrokerSettings.KillPendingDelayMillisMin;
		//        if (LivesimBrokerSettings.KillPendingDelayMillisMax > 0) {
		//            int range = Math.Abs(LivesimBrokerSettings.KillPendingDelayMillisMax - LivesimBrokerSettings.KillPendingDelayMillisMin);
		//            double rnd0to1 = new Random().NextDouble();
		//            int rangePart = (int)Math.Round(range * rnd0to1);
		//            delay += rangePart;
		//        }
		//    }
		//    if (delay == 0) {
		//        var omsg = new OrderStateMessage(orderPendingToKill, OrderState.VictimsBulletFlying, "DELAY_PENDING_KILL_ZERO");
		//        base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(omsg);
		//        base.OrderProcessor.Emit_killOrderPending_withoutKiller(orderPendingToKill, msig);
		//        return;
		//    }

		//    Task t = new Task(delegate() {
		//        string threadName = "DELAYED_PENDING_KILL " + orderPendingToKill.ToString();
		//        Assembler.SetThreadName(threadName, "CANT_SET_THREAD_NAME " + msig);

		//        Thread.Sleep(delay);
		//        var omsg = new OrderStateMessage(orderPendingToKill, OrderState.VictimKilled, "DELAY_PENDING_KILL[" + delay + "]ms");
		//        base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeTheSame_dontPostProcess(omsg);
		//        base.OrderProcessor.Emit_killOrderPending_withoutKiller(orderPendingToKill, msig);
		//    });
		//    t.ContinueWith(delegate {
		//        string msg = "TASK_THREW";
		//        Assembler.PopupException(msg + msig, t.Exception);
		//    }, TaskContinuationOptions.OnlyOnFaulted);
		//    t.Start();		// WHO_DOES t.Dispose() ?
		//}

		//public void ConsumeQuoteUnattached_toFillPending(QuoteGenerated quoteUnattachedVolatilePointer, AlertList willBeFilled) { lock (this.threadEntryLockToHaveQuoteSentToThread) {
		public void ConsumeQuoteUnattached_toFillPending(Quote quoteUnattachedVolatilePointer, AlertList willBeFilled) { lock (this.threadEntryLockToHaveQuoteSentToThread) {
			ScriptExecutor executor = this.LivesimDataSource.Executor;
			ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
	
			//v1
			//if (snap.AlertsPending.Count == 0) {
			//    string msg = "CHECK_IT_UPSTACK_AND_DONT_INVOKE_ME!!! snap.AlertsPending.Count=0 //ConsumeQuoteUnattached_toFillPending(" + willBeFilled + ") ";
			//    //DISABLED_TO_SEE_WHAT_THAT_WILL_BRING
			//    Assembler.PopupException(msg, null, false);
			//    return;
			//}
			//v2
			if (willBeFilled.Count == 0) return;

			//MOVED_TO_LivesimBrokerSpoiler
			//int delayBeforeFill = 0;
			//if (this.settings.DelayBeforeFillEnabled) {
			//	delayBeforeFill = settings.DelayBeforeFillMillisMin;
			//	if (settings.DelayBeforeFillMillisMax > 0) {
			//		int range = Math.Abs(settings.DelayBeforeFillMillisMax - settings.DelayBeforeFillMillisMin);
			//		double rnd0to1 = new Random().NextDouble();
			//		int rangePart = (int)Math.Round(range * rnd0to1);
			//		delayBeforeFill += rangePart;
			//	}
			//}

			int delayBeforeFill = this.LivesimBrokerSpoiler.DelayBeforeFill_Calculate();
			if (delayBeforeFill == 0) {
				this.consumeQuoteUnattached_toFillPendingAsync(quoteUnattachedVolatilePointer, willBeFilled);
				return;
			}

			AlertList willBeFilled_minusAlreadyScheduled = willBeFilled.Substract_returnClone(this.DataSnapshot.AlertsPending_scheduledForDelayedFill, this, "willBeFilled_minusAlreadyScheduled");

			AlertList priorDelayedFill = snap.AlertsPending;
			if (priorDelayedFill.Count == 0) return;

			ManualResetEvent quotePointerCaptured = new ManualResetEvent(false);
			Task t = new Task(delegate() {
				string threadName = "DELAYED_FILL " + quoteUnattachedVolatilePointer;
				Assembler.SetThreadName(threadName, "CANT_SET_THREAD_NAME //LivesimBroker");

				//QuoteGenerated quoteUnattachedLocalScoped = quoteUnattachedVolatilePointer;
				Quote quoteUnattachedLocalScoped = quoteUnattachedVolatilePointer;
				quotePointerCaptured.Set();

				//executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
				//Application.DoEvents();
				//MOVED_TO_LivesimBrokerSpoiler Thread.Sleep(delayBeforeFill);
				this.LivesimBrokerSpoiler.DelayBeforeFill_ThreadSleep();
				AlertList afterDelay = snap.AlertsPending;
				if (afterDelay.Count == 0) return;
				if (priorDelayedFill.Count != afterDelay.Count) {
				    string msg = "COUNT_MIGHT_HAVE_DECREASED_FOR_MULTIPLE_OPEN_POSITIONS/STRATEGY_IN_ANOTHER_FILLING_THREAD WHO_FILLED_WHILE_I_WAS_SLEEPING???";
				    //Assembler.PopupException(msg);
				    return;
				}
				this.consumeQuoteUnattached_toFillPendingAsync(quoteUnattachedLocalScoped, willBeFilled_minusAlreadyScheduled);
				executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			});
			t.ContinueWith(delegate {
				string msg = "TASK_THREW_LivesimBroker.consumeQuoteUnattached_toFillPendingAsync()";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();		// WHO_DOES t.Dispose() ?

			// I Sleep(10) since I wanna get quoteShadow pointer copied/stored inside the Task.Start()ed scope
			// before the parent thread (this one here) will drop/change quoteUnattached pointer upstack
			// so that after keeping the pointer I could launch another new Task
			// that's also why I used lock(this.threadEntryLockToHaveBarQuoteSentToThread)
			//Thread.Sleep(10);
			bool iCanContinue = quotePointerCaptured.WaitOne(1000);
			if (iCanContinue == false) {
				string msg = "DELAYED_FILL_THREAD_DIDNT_SIGNAL_THAT_QUOTE_POINTER_WAS_COPIED_DURING_1SECOND";
				Assembler.PopupException(msg);
			}

			//List<Alert> safe = willBeFilled.SafeCopy(this, "//ConsumeQuoteUnattached_toFillPending()");
			List<Alert> safe = willBeFilled_minusAlreadyScheduled.SafeCopy(this, "//ConsumeQuoteUnattached_toFillPending()");
			this.DataSnapshot.AlertsPending_scheduledForDelayedFill.AddRange(safe, this, "ConsumeQuoteUnattached_toFillPending(WAIT)");
		} }
		//void consumeQuoteUnattached_toFillPendingAsync(QuoteGenerated quoteUnattached, AlertList expectingToFill) {
		void consumeQuoteUnattached_toFillPendingAsync(Quote quoteUnattached, AlertList expectingToFill) {
			ScriptExecutor executor = this.LivesimDataSource.Executor;
			Bar barStreaming = executor.Bars.BarStreaming_nullUnsafe;
			if (barStreaming == null) {
				string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_WITH_BAR_STREAMING_NULL__END_OF_LIVESIM?";
				Assembler.PopupException(msg, null, false);
				return;
			}
			if (executor.BacktesterOrLivesimulator.ImRunningLivesim == false) {
				string msg = "I_REFUSE_TO_SIMULATE_FILL_PENDING_ALERTS_LIVESIM_NOT_RUNNING__PROBABLY_STOPPED/ABORTED?";
				Assembler.PopupException(msg, null, false);
				return;
			}
			ExecutionDataSnapshot snap = executor.ExecutionDataSnapshot;
			if (snap.AlertsPending.Count == 0) {
				string msg = "CHECK_IT_UPSTACK_AND_DONT_INVOKE_ME!!! snap.AlertsPending.Count=0 //consumeQuoteUnattached_toFillPendingAsync(" + expectingToFill + ")";
				Assembler.PopupException(msg, null, false);
				return;
			}

			//QuoteGenerated quoteAttachedToStreamingToConsumerBars = quoteUnattached.DeriveIdenticalButFresh();
			Quote quoteAttachedToStreamingToConsumerBars = quoteUnattached.Clone();
			quoteAttachedToStreamingToConsumerBars.SetParentBarStreaming(barStreaming);
			// same pointer gets a cloned quote ATTACHED; anyone is listening for the change? it's not OUT
			//quoteUnattached = quoteAttachedToStreamingToConsumerBars;
			//if (quoteAttachedToStreamingToConsumerBars.ParentBarStreaming.ParentBars == null) {
			//	string msg = "STREAMING_BAR_UNATTACHED_REPLACED_TO_SIMULATED_BARS_STREAMING_BAR QUICK_AND_DIRTY_EARLY_BINDER_HERE";
			//	Assembler.PopupException(msg);
			//	string err = "NOT_FILLED_YET";
			//	bool same = quoteAttachedToStreamingToConsumerBars.ParentBarStreaming.HasSameDOHLCVas(
			//					barStreaming, "Executor.Bars.BarStreaming", "quote.ParentBarStreaming", ref err);
			//	if (same == false) {
			//		Assembler.PopupException("CANT_SUBSTITUTE__EXCEPTIONS_COMING" + err);
			//	} else {
			//		quoteAttachedToStreamingToConsumerBars.SetParentBarStreaming(this.livesimDataSource.Executor.Bars.BarStreaming);
			//	}
			//}

			//var dumped = snap.DumpPendingAlertsIntoPendingHistoryByBar();
			//int dumped = snap.AlertsPending.ByBarPlaced.Count;
			//if (dumped > 0) {
			//	//string msg = "here is at least one reason why dumping on fresh quoteToReach makes sense"
			//	//	+ " if we never reach this breakpoint the remove dump() from here"
			//	//	+ " but I don't see a need to invoke it since we dumped pendings already after OnNewBarCallback";
			//	string msg = "DUMPED_PRIOR_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
			//}
			int pendingCountPre = executor.ExecutionDataSnapshot.AlertsPending.Count;
			//int pendingFilled = executor.MarketsimBacktest.SimulateFillAllPendingAlerts(
			int pendingFilled = base.BacktestMarketsim.SimulateFill_allPendingAlerts(
					quoteAttachedToStreamingToConsumerBars, new Action<Alert, double, double>(this.action_afterAlertFilled_inducePostProcessing_movedAroundOnReturn));
			int pendingCountNow = executor.ExecutionDataSnapshot.AlertsPending.Count;
			if (pendingCountNow != pendingCountPre - pendingFilled) {
				string msg = "NOT_ONLY it looks like AnnihilateCounterparty worked out!";
			}
			if (pendingCountNow > 0) {
				string msg = "pending=[" + pendingCountNow + "], it must be prototype-induced 2 closing TP & SL";
			}
			//executor.Script.OnNewQuoteCallback(quoteToReach);

			ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = executor.InvokeScript_onNewBar_onNewQuote(quoteAttachedToStreamingToConsumerBars);
			//base.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate);
			if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
				pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
			}
		}
		void action_afterAlertFilled_inducePostProcessing_movedAroundOnReturn(Alert alertFilled, double priceFilled, double qtyFilled) {
			string msig = " //onAlertFilled(WAIT)";

			try {
				this.DataSnapshot.AlertsPending_scheduledForDelayedFill.WaitAndLockFor(this, msig);
				if (this.DataSnapshot.AlertsPending_scheduledForDelayedFill.Contains(alertFilled, this, msig)) {
					this.DataSnapshot.AlertsPending_scheduledForDelayedFill.Remove(alertFilled, this, msig);
				}
			} finally {
				this.DataSnapshot.AlertsPending_scheduledForDelayedFill.UnLockFor(this, msig);
			}
			Order order = alertFilled.OrderFollowed;
			if (order == null && alertFilled.SignalName.StartsWith("proto")) {
				string msg = "CORE_FORGOT_TO_CREATE_TWO_ORDERS_FOR_POSITION_PROTOTYPE";
				Assembler.PopupException(msg);
				return;
			}

			//v1
			//OSM_CTOR_COMPLAINS_SAME_STATE OrderStateMessage osm = new OrderStateMessage(order, OrderState.Filled, "LIVESIM_FILLED_THROUGH_MARKETSIM_BACKTEST");
			//OrderProcessor orderProcessor = Assembler.InstanceInitialized.OrderProcessor;
			//orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(osm, priceFilled, qtyFilled);
			//v2
			order.AppendMessage("LIVESIM_FILLED_THROUGH_MARKETSIM_BACKTEST");
			
			if (alertFilled.PriceFilledThroughPosition != priceFilled) {
				string msg = "WHO_FILLS_POSITION_PRICE_FILLED_THEN?";
			}
			if (alertFilled.QtyFilledThroughPosition != qtyFilled) {
				string msg = "WHO_FILLS_POSITION_QTY_FILLED_THEN?";
			}
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (this.LivesimDataSource.IsDisposed == false) {
				this.LivesimDataSource	.Dispose();
			} else {
				string msg = "ITS_OKAY this.livesimDataSource might have been already disposed by LivesimStreaming.Dispose()";
			}
			this.DataSnapshot	.Dispose();
			base.DataSource		= null;
			this.DataSnapshot	= null;
			this.IsDisposed = true;
		}
	}
}