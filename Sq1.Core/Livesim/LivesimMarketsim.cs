using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Sq1.Core.Backtesting;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public class LivesimMarketsim : BacktestMarketsim {
		protected	LivesimBroker			LivesimBroker			{ get { return base.Broker_backtestOrLivesim as LivesimBroker; } }
		protected	LivesimBrokerSpoiler	LivesimBrokerSpoiler	{ get { return this.LivesimBroker.LivesimBrokerSpoiler; } }

		Dictionary<Order, Task> delayedFill_scheduled_forOrder;

		public LivesimMarketsim(LivesimBroker livesimBroker) : base(livesimBroker) {
			delayedFill_scheduled_forOrder = new Dictionary<Order, Task>();
		}

		public override bool SimulateFill_pendingAlert_invokedWhenFillTestedPositive(Alert alert, Quote quote, Action<Alert, Quote, double, double> action_afterAlertFilled_beforeMovedAround = null) {
			string msig = " //LivesimMarketsim.SimulateFill_pendingAlert_invokedWhenFillTestedPositive(alert)";

			int twentySeconds = 20000;
			bool marketOrderCaught = alert.OrderFollowed_isAssignedNow_Mre.WaitOne(twentySeconds);	//Limit proto Entry was already here
			if (marketOrderCaught == false) {
				string msg = "MAKE_SCRIPT's_ON_NEW_QUOTE_LOCKED? PUMP_HAS_LESSER_DELAY_THAN_BROKER_FILL?";
				throw new Exception(msg);
				//return false;
			}
			Order order = alert.OrderFollowed_orCurrentReplacement;

			int delayBeforeFill = this.LivesimBrokerSpoiler.DelayBeforeFill_calculate();
			if (delayBeforeFill == 0) {
				this.LivesimBroker.OrderFillSubmittedImmediately_spoiledContent_threadEntryPoint(order);
			} else {
				if (this.delayedFill_scheduled_forOrder.ContainsKey(order)) {
					string msg = "IM_HERE_DURING_DELAY_BETWEEN_FILL_SCHEDULED_AND_FILL_OCCURED__QUOTES_ARE_BEING_CHECKED_FOR_POTENTIAL_FILL_SO_I_SAY_NO";
					return false;
				}

				Task t = new Task(delegate() {
					Order order_tunnelledToTask_fixedPointer = order; 
					string threadName = "LIVESIM__DELAYED_FILL delayBeforeFill[" + delayBeforeFill + "]ms " + order_tunnelledToTask_fixedPointer.ToString();
					Assembler.SetThreadName(threadName, "CANT_SET_THREAD_NAME" + msig);
					this.LivesimBrokerSpoiler.DelayBeforeFill_threadSleep();

					bool inFillableState =	order_tunnelledToTask_fixedPointer.State == OrderState.WaitingBrokerFill ||
											order_tunnelledToTask_fixedPointer.State == OrderState.VictimBulletFlying;

					if (inFillableState) {
						if (this.delayedFill_scheduled_forOrder.ContainsKey(order) == false) {
							string msg = "ARE_YOU_TRYING_TO_LAUNCH_DELAYED_FILL_TWICE????";
							Assembler.PopupException(msg);
						} else {
							this.delayedFill_scheduled_forOrder.Remove(order);
						}

						this.LivesimBroker.OrderFillSubmittedImmediately_spoiledContent_threadEntryPoint(order_tunnelledToTask_fixedPointer);
					} else {
						if (order_tunnelledToTask_fixedPointer.IsVictim && order_tunnelledToTask_fixedPointer.State == OrderState.VictimKilled) {
							string msg = "definitely ApplySlippageIfNotFilledWithin kicked in";
							if (order_tunnelledToTask_fixedPointer.ReplacedByGUID != "") msg += "WAS_ALREADY_REPLACED_EARLIER order.SlippagesLeftAvailable [" + order_tunnelledToTask_fixedPointer.SlippagesLeftAvailable + "]";
							//TOO_VERBOSE this.OrderProcessor.AppendMessage_propagateToGui(order, msg);
						} else if (order_tunnelledToTask_fixedPointer.State == OrderState.LimitExpiredRejected) {
							string msg = "ALL_SLIPPAGES_WERE_USED_UP [" + order_tunnelledToTask_fixedPointer.Slippages_asCSV + "]";
							if (order_tunnelledToTask_fixedPointer.ReplacedByGUID != "") msg += "WAS_ALREADY_REPLACED_EARLIER order.SlippagesLeftAvailable [" + order_tunnelledToTask_fixedPointer.SlippagesLeftAvailable + "]";
							this.Broker_backtestOrLivesim.OrderProcessor.AppendMessage_propagateToGui(order_tunnelledToTask_fixedPointer, msg);
						} else {
							string msg = "WONT_FILL_KOZ_STATE_IS_NOT_WaitingBrokerFill; ApplySlippageIfNotFilledWithin DIDNT happen 100%, it's something else";
							this.Broker_backtestOrLivesim.OrderProcessor.AppendMessage_propagateToGui(order_tunnelledToTask_fixedPointer, msg);
						}
					}
				});
				t.ContinueWith(delegate {
					string msg = "FILL_TASK_THREW";
					Assembler.PopupException(msg, t.Exception);
				}, TaskContinuationOptions.OnlyOnFaulted);
				this.delayedFill_scheduled_forOrder.Add(order, t);
				t.Start();		// WHO_DOES t.Dispose() ?
			}
			return true;
		}

	}
}
