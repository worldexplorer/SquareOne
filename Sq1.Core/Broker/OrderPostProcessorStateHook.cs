using System;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorStateHook {
		public string		Purpose				{ get; private set; }
		public Order		Order				{ get; private set; }
		public OrderState	OrderState			{ get; private set; }
		public Action<Order, ReporterPokeUnit> ActionOnState_broughByBroker { get; private set; }
		public bool			CurrentlyExecuting;
		public bool			InvokedThusCanBeDeleted;

		public OrderPostProcessorStateHook(string purpose, Order order, OrderState orderState, Action<Order, ReporterPokeUnit> hook) {
			this.Purpose	= purpose;
			this.Order		= order;
			this.OrderState	= orderState;
			this.ActionOnState_broughByBroker	= hook;
			this.InvokedThusCanBeDeleted = false;
		}

		public override bool Equals(Object o) {
			OrderPostProcessorStateHook anotherTriggerCondition = (OrderPostProcessorStateHook)o;
			bool ret =
				this.Order		== anotherTriggerCondition.Order &&
				this.OrderState	== anotherTriggerCondition.OrderState;
			return ret;
		}
		public override string ToString() {
			//return "when order[" + this.Order + "] becomes [" + this.OrderState + "]";
			return this.Purpose + ":[" + this.OrderState + "]<=order[" + this.Order + "]";
		}
	}
}