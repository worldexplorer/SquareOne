using System;
using System.Threading.Tasks;

using Sq1.Core.Execution;
using Sq1.Core.Broker;

namespace Sq1.Core.Livesim {
	public sealed partial class LivesimBrokerDefault {
		public override void Order_submit_oneThread_forAllNewAlerts(Order order) {
			string msig = " //LivesimBrokerDefault.Order_submit()";

			this.OrdersSubmitted_forOneLivesimBacktest.Add(order);
//			string msg = "IS_ASYNC_CALLBACK_NEEDED? THREAD_ID " + Thread.CurrentThread.ManagedThreadId;
//			base.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(order.GUID, OrderState.Submitted, msg);

			OrderStateMessage omsg_emitting = new OrderStateMessage(order, OrderState.Submitted, "SIMULATING_ORDER_EMITTING" + msig);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_emitting);

			OrderStateMessage omsg_confirmed = new OrderStateMessage(order, OrderState.WaitingBrokerFill, "SIMULATING_BROKER_CONFRIMED_SUBMIT" + msig);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed);
		}

		public override void Order_killPending_usingKiller(Order orderKiller) {
			string msig = " //LivesimBrokerDefault.Order_killPending_usingKiller(WAIT)";

			int delayBeforeKill = this.LivesimBrokerSpoiler.DelayBeforeKill_calculate();
			if (delayBeforeKill == 0) {
				this.order_killPending_usingKiller_threadEntry(orderKiller);
				return;
			}

			Task t = new Task(delegate() {
				string threadName = "DELAYED_KILL delayBeforeKill[" + delayBeforeKill + "]ms " + orderKiller;
				Assembler.SetThreadName(threadName, "CANT_SET_THREAD_NAME" + msig);

				this.ScriptExecutor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
				this.LivesimBrokerSpoiler.DelayBeforeKill_threadSleep();
				this.order_killPending_usingKiller_threadEntry(orderKiller);
				this.ScriptExecutor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
			});
			t.ContinueWith(delegate {
				string msg = "KILL_TASK_THREW";
				Assembler.PopupException(msg + msig, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();		// WHO_DOES t.Dispose() ?
		}

		void order_killPending_usingKiller_threadEntry(Order orderKiller) {
			string symbol = " (" + orderKiller.Alert.Symbol + "," + orderKiller.Alert.SymbolClass + ")";
			string msigHead = "State[" + orderKiller.State + "]"
				+ symbol
				+ " VictimToBeKilled.SernoExchange[" + orderKiller.VictimToBeKilled.SernoExchange + "]";
			string msig = msigHead + " //LivesimBroker.Order_killPending_usingKiller()";

			if (string.IsNullOrEmpty(orderKiller.VictimGUID)) {
				string msg = "killerOrder.KillerForGUID=EMPTY";
				orderKiller.appendMessage(msg);
				throw new Exception(msg);
			}
			if (orderKiller.VictimToBeKilled == null) {
				string msg = "killerOrder.VictimToBeKilled=null";
				orderKiller.appendMessage(msg);
				throw new Exception(msg);
			}

			Order orderVictim = orderKiller.VictimToBeKilled;

		
			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerPreSubmit + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omg_preSubmit_killer = new OrderStateMessage(orderKiller, OrderState.KillerPreSubmit, "SIMULATING_PREPARING_KILL_COMMAND_FOR_BROKER" + msigHead);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_preSubmit_killer);

			msigHead = " orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimBulletPreSubmit + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_preSubmit_victim = new OrderStateMessage(orderVictim, OrderState.VictimBulletPreSubmit, "SIMULATING_PREPARING_KILL_COMMAND_FOR_BROKER__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_preSubmit_victim);


			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerTransSubmittedOK + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omg_transSubmitted_killer = new OrderStateMessage(orderKiller, OrderState.KillerTransSubmittedOK, "SIMULATING_SENDING_KILL_TRANSACTION" + msigHead);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_transSubmitted_killer);

			msigHead = " orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimBulletConfirmed + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_confirmed_victim = new OrderStateMessage(orderVictim, OrderState.VictimBulletConfirmed, "SIMULATING_SENDING_KILL_TRANSACTION__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed_victim);


			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.WaitingBrokerFill + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omsg_confirmed_killer = new OrderStateMessage(orderKiller, OrderState.WaitingBrokerFill, "SIMULATING_BROKER_CONFRIMED_RECEPTION_KILL_ORDER (not really expected for killer to get this status)" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_confirmed_killer);

			msigHead = " orderKiller[" + orderKiller.SernoExchange + "]=>[" + OrderState.KillerBulletFlying + "] <= orderVictim[" + orderVictim.SernoExchange + "][" + orderVictim.State + "]";
			OrderStateMessage omsg_waitingFill_killer = new OrderStateMessage(orderKiller, OrderState.KillerBulletFlying, "SIMULATING_WAITING_KILL_DELAY_ON_BROKER_SIDE" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_waitingFill_killer);

			msigHead = " orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimBulletFlying + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
			OrderStateMessage omsg_waitingFill_victim = new OrderStateMessage(orderVictim, OrderState.VictimBulletFlying, "SIMULATING_WAITING_KILL_DELAY_ON_BROKER_SIDE__VICTIM_SHOULD_KNOW" + msigHead);
			base.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_waitingFill_victim);


			//KillerDone is set in victim_PostProcessing on VictimKilled
			//msigHead = " orderKiller[" + orderVictim.SernoExchange + "][" + orderKiller.State + "] => orderVictim[" + orderVictim.SernoExchange + "][" + orderKiller.State + "]";
			//OrderStateMessage omg_done_killer = new OrderStateMessage(orderKiller, OrderState.KillerDone, "SIMULATING_KILLER_ORDER_COMPLETE_CALLBACK" + msigHead);
			//this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omg_done_killer);

			msigHead = " orderVictim[" + orderVictim.SernoExchange + "]=>[" + OrderState.VictimKilled + "] <= orderKiller[" + orderKiller.SernoExchange + "][" + orderKiller.State + "]";
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
	}
}