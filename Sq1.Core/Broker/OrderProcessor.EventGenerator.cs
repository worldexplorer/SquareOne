using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public event EventHandler<OrdersListEventArgs>				OnOrderAddedExecutionFormNotification;
		public event EventHandler<OrdersListEventArgs>				OnOrderRemovedExecutionFormNotification;
		public event EventHandler<OrdersListEventArgs>				OnOrderStateChangedExecutionFormNotification;
		public event EventHandler<OrderStateMessageEventArgs>		OnOrderMessageAddedExecutionFormNotification;
		public event EventHandler<EventArgs>						OnDelaylessLivesimEndedShouldRebuildOLV;

		//bool someAlertsDontHaveTimeToRebuildGui(List<Order> ordersAdded) {
		//	bool shouldRebuild = false;
		//	int liveSimOrdersCount = 0;
		//	foreach (Order o in ordersAdded) {
		//		if (o.Alert.MyBrokerIsLivesim == false) continue;
		//		liveSimOrdersCount++;

		//		ChartShadow chartShadow = Assembler.InstanceInitialized.AlertsForChart.FindContainerFor(o.Alert);
		//		ScriptExecutor executor = chartShadow.Executor;
		//		bool guiHasTime = executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
		//		if (shouldRebuild == false) shouldRebuild = guiHasTime;
		//		if (shouldRebuild) break;
		//	}
		//	if (liveSimOrdersCount == 0) return true;
		//	return shouldRebuild;
		//}
		public void RaiseAsyncOrderAddedExecutionFormShouldRebuildOLV(object sender, List<Order> ordersAdded) {
			if (this.OnOrderAddedExecutionFormNotification == null) return;

			//bool shouldRebuild = someAlertsDontHaveTimeToRebuildGui(ordersAdded);
			//if (shouldRebuild == false) {
			//	string msg = "don't rebuild if we do livesimulation with quoteDelay=0 or disabled";
			//	Assembler.PopupException(msg);
			//	return;
			//}

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
			List<Order> ordersStaleScreenedFromClearForNewThread = new List<Order>(ordersToRemove);
			Task t = new Task(delegate {
				//if (ordersToRemove.Count == 0 && ordersStaleScreenedFromClearForNewThread.Count != 0) {
				// NO ordersToRemove = ordersStaleScreenedFromClearForNewThread;
				//	string msg = "WILL_JUST_OrdersTreeOLV.RebuildAll(true)_IN_OrderRemoveFromListView() ordersToRemove already clear()ed before this task has started";
				//}
				this.OnOrderRemovedExecutionFormNotification(sender, new OrdersListEventArgs(ordersStaleScreenedFromClearForNewThread));
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
		public void RaiseDelaylessLivesimEndedShouldRebuildOLV(object sender) {
			if (this.OnDelaylessLivesimEndedShouldRebuildOLV == null) return;
			this.OnDelaylessLivesimEndedShouldRebuildOLV(sender, null);
		}
	}
}
