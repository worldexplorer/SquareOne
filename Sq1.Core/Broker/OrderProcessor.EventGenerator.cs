using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public event EventHandler<OrdersListEventArgs>				OnOrderAdded_executionControlShouldRebuildOLV_scheduled;
		public event EventHandler<OrdersListEventArgs>				OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled;
		public event EventHandler<OrdersListEventArgs>				OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately;
		public event EventHandler<OrderStateMessageEventArgs>		OnOrderMessageAdded_executionControlShouldPopulate_scheduled;
		public event EventHandler<EventArgs>						OnDelaylessLivesimEnded_shouldRebuildOLV_immediately;

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
		public void RaiseOnOrderAdded_executionControlShouldRebuildOLV_scheduled(object sender, List<Order> ordersAdded) {
			if (this.OnOrderAdded_executionControlShouldRebuildOLV_scheduled == null) return;

			//bool shouldRebuild = someAlertsDontHaveTimeToRebuildGui(ordersAdded);
			//if (shouldRebuild == false) {
			//	string msg = "don't rebuild if we do livesimulation with quoteDelay=0 or disabled";
			//	Assembler.PopupException(msg);
			//	return;
			//}

			#region EXPERIMENTAL
			//Task t = new Task(delegate {
			//    this.OnOrderAdded_executionControlShouldRebuildOLV_scheduled(sender, new OrdersListEventArgs(ordersAdded));
			//});
			//t.ContinueWith(delegate {
			//    string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
			//    Assembler.PopupException(msg, t.Exception);
			//}, TaskContinuationOptions.OnlyOnFaulted);
			//t.Start();		// WHO_DOES t.Dispose() ?
			#endregion

			this.OnOrderAdded_executionControlShouldRebuildOLV_scheduled(sender, new OrdersListEventArgs(ordersAdded));
		}
		public void RaiseOnOrdersRemoved_executionControlShouldRebuildOLV_scheduled(object sender, List<Order> ordersToRemove) {
			if (this.OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled == null) return;

			#region EXPERIMENTAL
			//List<Order> ordersStaleScreenedFromClearForNewThread = new List<Order>(ordersToRemove);
			//Task t = new Task(delegate {
			//    //if (ordersToRemove.Count == 0 && ordersStaleScreenedFromClearForNewThread.Count != 0) {
			//    // NO ordersToRemove = ordersStaleScreenedFromClearForNewThread;
			//    //	string msg = "WILL_JUST_OrdersTreeOLV.RebuildAll(true)_IN_OrderRemoveFromListView() ordersToRemove already clear()ed before this task has started";
			//    //}
			//    this.OnOrderRemoved_executionControlShouldRebuildOLV_sheduled(sender, new OrdersListEventArgs(ordersStaleScreenedFromClearForNewThread));
			//});
			//t.ContinueWith(delegate {
			//    string msg = "TASK_THREW_OrderEventDistributor.RaiseAsyncOrderAddedExecutionFormShouldRebuildTree()";
			//    Assembler.PopupException(msg, t.Exception);
			//}, TaskContinuationOptions.OnlyOnFaulted);
			//t.Start();		// WHO_DOES t.Dispose() ?
			#endregion

			this.OnOrdersRemoved_executionControlShouldRebuildOLV_scheduled(sender, new OrdersListEventArgs(ordersToRemove));
		}
		public void RaiseOnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(object sender, List<Order> ordersUpdated) {
			if (this.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately == null) return;
			this.OnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(sender, new OrdersListEventArgs(ordersUpdated));
		}
		public void RaiseOnOrderMessageAdded_executionControlShouldPopulate_scheduled(object sender, OrderStateMessage orderStateMessage) {
			if (this.OnOrderMessageAdded_executionControlShouldPopulate_scheduled == null) return;
			this.OnOrderMessageAdded_executionControlShouldPopulate_scheduled(sender, new OrderStateMessageEventArgs(orderStateMessage));
		}
		public void RaiseOnDelaylessLivesimEnded_shouldRebuildOLV_immediately(object sender) {
			if (this.OnDelaylessLivesimEnded_shouldRebuildOLV_immediately == null) return;
			this.OnDelaylessLivesimEnded_shouldRebuildOLV_immediately(sender, null);
		}
	}
}
