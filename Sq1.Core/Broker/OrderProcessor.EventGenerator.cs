using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public event EventHandler<OrdersListEventArgs>				OnAsyncOrderAdded_executionControlShouldRebuildOLV;
		public event EventHandler<OrdersListEventArgs>				OnAsyncOrderRemoved_executionControlShouldRebuildOLV;
		public event EventHandler<OrdersListEventArgs>				OnOrderStateOrPropertiesChanged_executionControlShouldPopulate;
		public event EventHandler<OrderStateMessageEventArgs>		OnOrderMessageAdded_executionControlShouldPopulate;
		public event EventHandler<EventArgs>						OnDelaylessLivesimEnded_shouldRebuildOLV;

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
		public void RaiseAsyncOrderAdded_executionControlShouldRebuildOLV(object sender, List<Order> ordersAdded) {
			if (this.OnAsyncOrderAdded_executionControlShouldRebuildOLV == null) return;

			//bool shouldRebuild = someAlertsDontHaveTimeToRebuildGui(ordersAdded);
			//if (shouldRebuild == false) {
			//	string msg = "don't rebuild if we do livesimulation with quoteDelay=0 or disabled";
			//	Assembler.PopupException(msg);
			//	return;
			//}

			#region EXPERIMENTAL
			Task t = new Task(delegate {
				this.OnAsyncOrderAdded_executionControlShouldRebuildOLV(sender, new OrdersListEventArgs(ordersAdded));
			});
			t.ContinueWith(delegate {
				string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();		// WHO_DOES t.Dispose() ?
			#endregion
		}
		public void RaiseAsyncOrderRemoved_executionControlShouldRebuildOLV(object sender, List<Order> ordersToRemove) {
			if (this.OnAsyncOrderRemoved_executionControlShouldRebuildOLV == null) return;
			#region EXPERIMENTAL
			List<Order> ordersStaleScreenedFromClearForNewThread = new List<Order>(ordersToRemove);
			Task t = new Task(delegate {
				//if (ordersToRemove.Count == 0 && ordersStaleScreenedFromClearForNewThread.Count != 0) {
				// NO ordersToRemove = ordersStaleScreenedFromClearForNewThread;
				//	string msg = "WILL_JUST_OrdersTreeOLV.RebuildAll(true)_IN_OrderRemoveFromListView() ordersToRemove already clear()ed before this task has started";
				//}
				this.OnAsyncOrderRemoved_executionControlShouldRebuildOLV(sender, new OrdersListEventArgs(ordersStaleScreenedFromClearForNewThread));
			});
			t.ContinueWith(delegate {
				string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
				Assembler.PopupException(msg, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();		// WHO_DOES t.Dispose() ?
			#endregion
		}
		public void RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(object sender, List<Order> ordersUpdated) {
			if (this.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate == null) return;
			this.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate(sender, new OrdersListEventArgs(ordersUpdated));
		}
		public void RaiseOrderMessageAdded_executionControlShouldPopulate(object sender, OrderStateMessage orderStateMessage) {
			if (this.OnOrderMessageAdded_executionControlShouldPopulate == null) return;
			this.OnOrderMessageAdded_executionControlShouldPopulate(sender, new OrderStateMessageEventArgs(orderStateMessage));
		}
		public void RaiseDelaylessLivesimEnded_shouldRebuildOLV(object sender) {
			if (this.OnDelaylessLivesimEnded_shouldRebuildOLV == null) return;
			this.OnDelaylessLivesimEnded_shouldRebuildOLV(sender, null);
		}
	}
}
