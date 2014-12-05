using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public event EventHandler<OrdersListEventArgs>				OnOrderAddedExecutionFormNotification;
		public event EventHandler<OrdersListEventArgs>				OnOrderRemovedExecutionFormNotification;
		public event EventHandler<OrdersListEventArgs>				OnOrderStateChangedExecutionFormNotification;
		public event EventHandler<OrderStateMessageEventArgs>	OnOrderMessageAddedExecutionFormNotification;
		
		public void RaiseAsyncOrderAddedExecutionFormShouldRebuildOLV(object sender, List<Order> ordersAdded) {
			if (this.OnOrderAddedExecutionFormNotification == null) return;
			#region EXPERIMENTAL
			Task t = new Task(delegate {
				this.OnOrderAddedExecutionFormNotification(sender, new OrdersListEventArgs(ordersAdded));
			});
			t.ContinueWith(delegate {
				string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();
			#endregion
		}
		public void RaiseAsyncOrderRemovedExecutionFormExecutionFormShouldRebuildOLV(object sender, List<Order> ordersToRemove) {
			if (this.OnOrderRemovedExecutionFormNotification == null) return;
			#region EXPERIMENTAL
			Task t = new Task(delegate {
				this.OnOrderRemovedExecutionFormNotification(sender, new OrdersListEventArgs(ordersToRemove));
			});
			t.ContinueWith(delegate {
				string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();
			#endregion
		}
		public void RaiseOrderStateOrPropertiesChangedExecutionFormShouldDisplay(object sender, List<Order> ordersUpdated) {
			if (this.OnOrderStateChangedExecutionFormNotification == null) return;
			this.OnOrderStateChangedExecutionFormNotification(sender, new OrdersListEventArgs(ordersUpdated));
		}
		public void RaiseOrderMessageAddedExecutionFormNotification(object sender, OrderStateMessage orderStateMessage) {
			if (this.OnOrderMessageAddedExecutionFormNotification == null) return;
			this.OnOrderMessageAddedExecutionFormNotification(sender, new OrderStateMessageEventArgs(orderStateMessage));
		}
	}
}
