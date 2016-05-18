using System;
using System.Threading;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerSpoiler {
				LivesimBroker livesimBroker;

		public	int		DelayBeforeFill		{ get; private set; }
		public	int		DelayBeforeKill		{ get; private set; }

		public	bool	RejectOrderNow		{ get; private set; }

		LivesimBrokerSpoiler() {
			this.DelayBeforeFill = 0;
			this.RejectOrderNow = false;
		}

		public LivesimBrokerSpoiler(LivesimBroker livesimBroker) : this() {
			this.livesimBroker = livesimBroker;
		}

		public int DelayBeforeFill_calculate() {
			this.DelayBeforeFill = 0;
			if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillEnabled) {
				this.DelayBeforeFill = this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin;
				if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax > 0) {
					int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin -
										 this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					this.DelayBeforeFill += rangePart;
				}
			}
			return this.DelayBeforeFill;
		}
		public void DelayBeforeFill_threadSleep() {
			if (this.DelayBeforeFill <= 0) return;
			//Application.DoEvents();
			Thread.Sleep(this.DelayBeforeFill);
		}

		public int DelayBeforeKill_calculate() {
			this.DelayBeforeKill = 0;
			if (this.livesimBroker.LivesimBrokerSettings.KillPendingDelayEnabled) {
				this.DelayBeforeKill = this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMin;
				if (this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMax > 0) {
					int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMin -
										 this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMax);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					this.DelayBeforeKill += rangePart;
				}
			}
			return this.DelayBeforeKill;
		}
		public void DelayBeforeKill_threadSleep() {
			if (this.DelayBeforeKill <= 0) return;
			Thread.Sleep(this.DelayBeforeKill);
		}

	
		int howManyOrders_wereNotRejected = 0;
		public bool RejectNow_calculate() {
			this.RejectOrderNow = false;
			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionEnabled == false) return this.RejectOrderNow;

			int planned_nonRejectedLimit = this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMin;
			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMin -
									 this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				planned_nonRejectedLimit += rangePart;
			}

			howManyOrders_wereNotRejected++;
			if (howManyOrders_wereNotRejected >= planned_nonRejectedLimit) {
				this.RejectOrderNow = true;
				howManyOrders_wereNotRejected = 0;
			}
			return this.RejectOrderNow;
		}

	}
}
