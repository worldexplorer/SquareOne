using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorStateChangedTrigger {
		OrderProcessor						orderProcessor;
		List<OrderPostProcessorStateHook>	hooks;
		object								hooksLock;

		OrderPostProcessorStateChangedTrigger() {
			this.hooks = new List<OrderPostProcessorStateHook>();
			this.hooksLock = new Object();
		}

		public OrderPostProcessorStateChangedTrigger(OrderProcessor orderProcessor) : this() {
			this.orderProcessor = orderProcessor;
		}

		public void HookRegister(OrderPostProcessorStateHook orderWithState) { lock (this.hooksLock) {
			if (this.hooks.Contains(orderWithState)) {
				string msg = "TRIGGER_ALREADY_ADDED: " + orderWithState;
				throw new Exception(msg);
			}
			this.hooks.Add(orderWithState);
		} }

		public int InvokeHooks_forOrderState_unregisterInvoked(Order order, ReporterPokeUnit pokeUnit_nullUnsafe) { lock (this.hooksLock) {
			int hooksInvoked = 0;
			List<OrderPostProcessorStateHook> safeForModification = new List<OrderPostProcessorStateHook>(this.hooks);
			foreach (OrderPostProcessorStateHook hook in safeForModification) {
				if (hook.InvokedThusCanBeDeleted)	continue;
				if (hook.Order		!= order)		continue;
				if (hook.OrderState != order.State)	continue;

				string msg = "HOOK_[" + hook + "] PROCESSING...";
				//Assembler.PopupException(msg, null, false);
				this.orderProcessor.AppendMessage_propagateToGui(order, msg);

				hook.CurrentlyExecuting = true;
				hook.ActionOnState_broughByBroker(order, pokeUnit_nullUnsafe);
				hook.CurrentlyExecuting = false;
				hook.InvokedThusCanBeDeleted = true;

				msg = "HOOK_[" + hook + "] PROCESSING_DONE";
				//Assembler.PopupException(msg, null, false);
				this.orderProcessor.AppendMessage_propagateToGui(order, msg);

				hooksInvoked++;
			}
			int hooksRemoved = this.HooksUnregister_AllInvoked();
			if (hooksRemoved != hooksInvoked) {
				string msg = "hooksRemoved[" + hooksRemoved + "] != hooksInvoked[" + hooksInvoked + "]; I don't wanna be stuck on threading issues...";
				throw new Exception(msg);
			}
			return hooksInvoked;
		} }

		public int HooksUnregister_AllInvoked() { lock (this.hooksLock) {
			int hooksUnregistered = 0;
			List<OrderPostProcessorStateHook> hooksInvoked = new List<OrderPostProcessorStateHook>();
			foreach (OrderPostProcessorStateHook hook in this.hooks) {
				if (hook.InvokedThusCanBeDeleted == false) continue;
				hooksInvoked.Add(hook);
			}
			foreach (OrderPostProcessorStateHook hookInvoked in hooksInvoked) {
				hooks.Remove(hookInvoked);
				hooksUnregistered++;
			}
			return hooksUnregistered;
		} }

		public int HooksUnregister_Uninvoked(Order order, OrderState orderState) { lock (this.hooksLock) {
			int hooksUnregistered = 0;
			List<OrderPostProcessorStateHook> hooksToRemove = new List<OrderPostProcessorStateHook>();
			foreach (OrderPostProcessorStateHook hook in this.hooks) {
				if (hook.Order != order) continue;
				if (hook.InvokedThusCanBeDeleted) {
					string msg = "I_REFUSE_TO_UNREGISTER_INVOKED_HOOK";
					if (hook.OrderState == OrderState.VictimKilled) msg += " WEIRD!!! replacementOrderCreate is being executed now, will be autoremoved after completion";
					Assembler.PopupException(msg);
					continue;
				}
				hooksToRemove.Add(hook);
			}
			foreach (OrderPostProcessorStateHook hookToRemove in hooksToRemove) {
				hooks.Remove(hookToRemove);
				hooksUnregistered++;
			}
			return hooksUnregistered;
		} }
	}
}