using System;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class TimerSimplifiedThreading_withOrder : TimerSimplifiedThreading {
		public	Order	Order	{ get; private set; }

		public TimerSimplifiedThreading_withOrder(Order order, int expiredMillis) : base("TIMER_FOR_ORDER_" + order, expiredMillis) {
			this.Order = order;
		}
	}
}