using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public class OrderProcessorDataSnapshot {
		// REASON_TO_EXIST: having lanes makes order lookups faster; by having order state I know which List I'll be looking for GUID
		// useful application is when 100,000 orders are filled sucessfully and I received a UpdatePending notification for GUID=99,999
		// CemeteryHealty would contain all
		public OrderLaneByState		OrdersSubmitting			{ get; private set; }
		public OrderLaneByState		OrdersPending				{ get; private set; }
		public OrderLaneByState		OrdersPendingFailed			{ get; private set; }
		public OrderLaneByState		OrdersCemeteryHealthy		{ get; private set; }
		public OrderLaneByState		OrdersCemeterySick			{ get; private set; }
		public OrderLane			OrdersAll					{ get; private set; }

			   OrderProcessor		orderProcessor;
		public int					OrderCountThreadSafe		{ get; private set; }
		public int					OrdersExpectingBrokerUpdateCount;		//{ get; private set; }
		public OrdersAutoTree		OrdersAutoTree				{ get; private set; }
			   object				orderSwitchingLanesLock;

		public SerializerLogrotatePeriodic<Order>	SerializerLogrotateOrders	{ get; private set; }
		//public Dictionary<Account, List<Order>>	OrdersByAccount				{ get; private set; }

		protected OrderProcessorDataSnapshot() {
			OrdersSubmitting		= new OrderLaneByState(OrderStatesCollections.AllowedForSubmissionToBrokerProvider);
			OrdersPending			= new OrderLaneByState(OrderStatesCollections.NoInterventionRequired);
			OrdersPendingFailed		= new OrderLaneByState(OrderStatesCollections.InterventionRequired);
			OrdersCemeteryHealthy	= new OrderLaneByState(OrderStatesCollections.CemeteryHealthy);
			OrdersCemeterySick		= new OrderLaneByState(OrderStatesCollections.CemeterySick);
			OrdersAll				= new OrderLane("OrdersAll", this);
			//OrdersByAccount		= new Dictionary<Account, List<Order>>();

			SerializerLogrotateOrders	= new SerializerLogrotatePeriodic<Order>();
			OrdersAutoTree				= new OrdersAutoTree();
			orderSwitchingLanesLock	= new object();
		}
		public OrderProcessorDataSnapshot(OrderProcessor orderProcessor) : this() {
			this.orderProcessor = orderProcessor;
		}
		public void Initialize(string rootPath) {
			bool createdNewFile = this.SerializerLogrotateOrders.Initialize(rootPath, "Orders.json", "Orders", null);
			try {
				this.SerializerLogrotateOrders.Deserialize();
				// OrdersTree was historically introduced the last, but filling Order.DerivedOrders early here, just in case
				//this.OrdersTree.InitializeScanDeserializedMoveDerivedsInsideBuildTreeShadow(this.SerializerLogRotate.OrdersBuffered.ItemsMain);
				List<Order> ordersInit = this.SerializerLogrotateOrders.Entity; 
				foreach (Order current in ordersInit) {
					if (current.InStateExpectingCallbackFromBroker) {
						current.State = OrderState.SubmittedNoFeedback;
					}
				}
				// yeps we spawn the lists with the same content;
				// original, OrdersBuffered.ItemsMain will shrink later due to LogrotateSerializer.safeLogRotate()
				// the copy, this.OrdersAll will stay the longest orderlist (request this.OrdersAll.SafeCopy if you got CollectionModifiedException)
				// OrdersTree will also stay as full as OrdersAll, but serves as DataSource for ExecutionTree in VirtualMode
				// adding/removing to OrdersAll should add/remove to OrdersBuffered and OrdersTree (slow but true)
				this.OrdersAll = new OrderLane("OrdersAll", ordersInit, this);
				this.OrdersAutoTree.InitializeScanDeserializedMoveDerivedsInsideBuildTreeShadow(this.OrdersAll);
			} catch (Exception ex) {
				string msg = "THROWN_OrderProcessorDataSnapshot.Initialize()";
				Assembler.PopupException(msg, ex, false);
			}
			this.SerializerLogrotateOrders.StartSerializerThread();
		}

		public void OrderInsertNotifyGuiAsync(Order orderToAdd) {
			string msg = "HEY_I_REACHED_THIS_POINT__NO_EXCEPTIONS_SO_FAR?";
			//Debugger.Break();
			//#D_HANGS Assembler.PopupException(msg);
			
			this.OrdersAll.Insert(orderToAdd);
			this.SerializerLogrotateOrders.Insert(0, orderToAdd);

			this.OrderCountThreadSafe++;
			if (orderToAdd.InStateExpectingCallbackFromBroker) this.OrdersExpectingBrokerUpdateCount++;
			
			this.OrdersAutoTree.InsertToRoot(orderToAdd);
			this.orderProcessor.RaiseAsyncOrderAddedExecutionFormShouldRebuildOLV(this, new List<Order>(){orderToAdd});
		}
		public void OrdersRemove(List<Order> ordersToRemove, bool externalUpdatesWillBeTriggeredUpstack = false) {
			this.OrdersAll.RemoveAll(ordersToRemove);
			this.OrdersAutoTree.RemoveFromRootLevelKeepOrderPointers(ordersToRemove);
			this.orderProcessor.RaiseAsyncOrderRemovedExecutionFormExecutionFormShouldRebuildOLV(this, ordersToRemove);
			this.SerializerLogrotateOrders.Remove(ordersToRemove);
			if (externalUpdatesWillBeTriggeredUpstack == false) {
				this.SerializerLogrotateOrders.HasChangesToSave = true;
			}
		}
		public void OrdersRemoveNonPendingForAccounts(List<string> accountNumbers) {
			foreach (string accountNumber in accountNumbers) {
				List<Order> ordersForAccount = this.OrdersAll.ScanRecentFindAllForAccount(accountNumber); 
				this.OrdersRemove(ordersForAccount, true);
			}
			this.SerializerLogrotateOrders.HasChangesToSave = true;
		}

		public OrderLaneByState SuggestLaneByOrderStateNullUnsafe(OrderState orderState) {
			if (this.OrdersSubmitting		.StateIsAcceptable(orderState))	return this.OrdersSubmitting;
			if (this.OrdersPending			.StateIsAcceptable(orderState))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.StateIsAcceptable(orderState))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.StateIsAcceptable(orderState))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.StateIsAcceptable(orderState))	return this.OrdersCemeterySick;
			return null;
		}
		public OrderLaneByState ScanLanesForOrderGuidNullUnsafe(Order order) {
			if (this.OrdersSubmitting		.Contains(order))	return this.OrdersSubmitting;
			if (this.OrdersPending			.Contains(order))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.Contains(order))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.Contains(order))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.Contains(order))	return this.OrdersCemeterySick;
			return null;
		}

		public void SwitchLanesForOrderPostStatusUpdate(Order orderNowAfterUpdate, OrderState orderStatePriorToUpdate) { lock (this.orderSwitchingLanesLock) {
				string msig = " //OrderProcessorDataSnapshot::SwitchLanesForOrderPostStatusUpdate()";
				OrderLaneByState orderLaneBeforeStateUpdate = this.SuggestLaneByOrderStateNullUnsafe(orderStatePriorToUpdate);
				OrderLaneByState  orderLaneAfterStateUpdate = this.SuggestLaneByOrderStateNullUnsafe(orderNowAfterUpdate.State);
				if (orderLaneBeforeStateUpdate == orderLaneAfterStateUpdate) return;
				if (orderLaneBeforeStateUpdate != null) {
					try {
						orderLaneBeforeStateUpdate.Remove(orderNowAfterUpdate);
					} catch (Exception ex) {
						Assembler.PopupException("FAILED_TO_REMOVE orderNowAfterUpdate=[" + orderNowAfterUpdate + "]" + msig, ex, false);
					}
				}
				if (orderLaneAfterStateUpdate != null) {
					try {
						orderLaneAfterStateUpdate.Insert(orderNowAfterUpdate);
					} catch (Exception ex) {
						Assembler.PopupException("FAILED_TO_INSERT orderNowAfterUpdate=[" + orderNowAfterUpdate + "]" + msig, ex, false);
					}
				}
			} }
		//public OrderLaneByState FindStateLaneDoesntContain(Order order) {
		//    OrderLaneByState expectedToNotContain = this.SuggestLaneByOrderState(order.State);
		//    if (expectedToNotContain.Contains(order)) {
		//        string msg = "HOW_WILL_YOU_USE_UNKNOWN???";
		//        Assembler.PopupException(msg);
		//        expectedToNotContain = new OrderLaneByState(OrderStatesCollections.Unknown);
		//    }
		//    return expectedToNotContain;
		//}
	}
}