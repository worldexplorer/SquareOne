using System;
using System.Collections.Generic;
using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorStateChangedTrigger {
		private OrderProcessor orderProcessor;

		private List<OrderPostProcessorStateHook> hooks;
		private Object hooksLock;

		public OrderPostProcessorStateChangedTrigger(OrderProcessor orderProcessor) {
			this.orderProcessor = orderProcessor;
			this.hooks = new List<OrderPostProcessorStateHook>();
			this.hooksLock = new Object();
		}
		public void AddStateChangedHook(OrderPostProcessorStateHook orderWithState) {
			lock (this.hooksLock) {
				if (hooks.Contains(orderWithState)) {
					string msg = "TRIGGER_ALREADY_ADDED: " + orderWithState;
					throw new Exception(msg);
				}
				hooks.Add(orderWithState);
			}
		}
		public int InvokeOnceHooksForOrderStateAndDelete(Order order, ReporterPokeUnit pokeUnit) {
			int hooksInvoked = 0;
			lock (this.hooksLock) {
				foreach (OrderPostProcessorStateHook hook in this.hooks) {
					string msg = "processing hook [" + hook + "] ";
					if (hook.InvokedThusCanBeDeleted) continue;
					if (hook.Order != order) continue;
					if (hook.OrderState != order.State) continue;

					hook.CurrentlyExecuting = true;
					hook.Delegate(order, pokeUnit);
					hook.CurrentlyExecuting = false;
					hook.InvokedThusCanBeDeleted = true;
					msg += " ... done";

					hooksInvoked++;
				}
				int hooksRemoved = this.RemoveAllInvoked();
				if (hooksRemoved != hooksInvoked) {
					string msg = "hooksRemoved[" + hooksRemoved + "] != hooksInvoked[" + hooksInvoked + "]; I don't wanna be stuck on threading issues...";
					throw new Exception(msg);
				}
			}
			return hooksInvoked;
		}
		public int RemoveAllInvoked() {
			int hooksRemoved = 0;
			List<OrderPostProcessorStateHook> hooksInvoked = new List<OrderPostProcessorStateHook>();
			lock (this.hooksLock) {
				foreach (OrderPostProcessorStateHook hook in this.hooks) {
					if (hook.InvokedThusCanBeDeleted == false) continue;
					hooksInvoked.Add(hook);
				}
				foreach (OrderPostProcessorStateHook hookInvoked in hooksInvoked) {
					hooks.Remove(hookInvoked);
					hooksRemoved++;
				}
			}
			return hooksRemoved;
		}
	}
}