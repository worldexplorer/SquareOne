using System;
using System.Collections.Generic;
using System.Threading;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerSpoiler {
		//public int delayBeforeFill { get; private set; }
		public int delayBeforeFill;
		LivesimBroker livesimBroker;

		public LivesimBrokerSpoiler(LivesimBroker livesimBroker) {
			this.delayBeforeFill = 0;
			this.livesimBroker = livesimBroker;
		}

		public int DelayBeforeFill_Calculate() {
			this.delayBeforeFill = 0;
			if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillEnabled) {
				this.delayBeforeFill = this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin;
				if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax > 0) {
					int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin -
										 this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					this.delayBeforeFill += rangePart;
				}
			}
			return this.delayBeforeFill;
		}

		public void DelayBeforeFill_ThreadSleep() {
			if (this.delayBeforeFill <= 0) return;
			//Application.DoEvents();
			Thread.Sleep(this.delayBeforeFill);
		}
	}
}
