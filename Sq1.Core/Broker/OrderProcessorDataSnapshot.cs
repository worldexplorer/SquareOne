using System;
using System.Collections.Generic;

using System.Diagnostics;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public class OrderProcessorDataSnapshot {
		// REASON_TO_EXIST: having lanes makes order lookups faster; by having order state I know which List I'll be looking for GUID
		// useful application is when 100,000 orders are filled sucessfully and I received a UpdatePending notification for GUID=99,999
		// CemeteryHealty would contain all
		public OrderListByState						OrdersSubmitting			{ get; private set; }
		public OrderListByState						OrdersPending				{ get; private set; }
		public OrderListByState						OrdersPendingFailed			{ get; private set; }
		public OrderListByState						OrdersCemeteryHealthy		{ get; private set; }
		public OrderListByState						OrdersCemeterySick			{ get; private set; }
		public OrderList							OrdersAll					{ get; private set; }
		//public Dictionary<Account, List<Order>>	OrdersByAccount				{ get; private set; }

		public int									OrderCountThreadSafe		{ get; private set; }
		public int									OrdersCurrentlyProcessingByBrokerCount		{ get; private set; }
		public OrderProcessor						OrderProcessor				{ get; private set; }
		public SerializerLogrotatePeriodic<Order>	SerializerLogrotateOrders	{ get; private set; }
		public OrdersShadowTreeDerived				OrdersTree					{ get; private set; }

		protected OrderProcessorDataSnapshot() {
			this.OrdersSubmitting		= new OrderListByState(OrderStatesCollections.AllowedForSubmissionToBrokerProvider);
			this.OrdersPending			= new OrderListByState(OrderStatesCollections.NoInterventionRequired);
			this.OrdersPendingFailed	= new OrderListByState(OrderStatesCollections.InterventionRequired);
			this.OrdersCemeteryHealthy	= new OrderListByState(OrderStatesCollections.CemeteryHealthy);
			this.OrdersCemeterySick		= new OrderListByState(OrderStatesCollections.CemeterySick);
			this.OrdersAll				= new OrderList("OrdersAll", this);
			//this.OrdersByAccount		= new Dictionary<Account, List<Order>>();

			this.SerializerLogrotateOrders	= new SerializerLogrotatePeriodic<Order>();
			this.OrdersTree				= new OrdersShadowTreeDerived();
		}
		public OrderProcessorDataSnapshot(OrderProcessor orderProcessor) : this() {
			this.OrderProcessor = orderProcessor;
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
				this.OrdersAll = new OrderList("OrdersAll", ordersInit, this);
				this.OrdersTree.InitializeScanDeserializedMoveDerivedsInsideBuildTreeShadow(this.OrdersAll);
			} catch (Exception ex) {
				string msg = "THROWN_OrderProcessorDataSnapshot.Initialize()";
				Assembler.PopupException(msg, ex, false);
			}
			this.SerializerLogrotateOrders.StartSerializerThread();
			this.UpdateActiveOrdersCountEvent();
		}

		public void OrderAddSynchronizedAndPropagate(Order orderToAdd) {
			string msg = "HEY_I_REACHED_THIS_POINT__NO_EXCEPTIONS_SO_FAR?";
			//Debugger.Break();
			//#D_HANGS Assembler.PopupException(msg);
			
			this.OrdersAll.Insert(0, orderToAdd);
			this.SerializerLogrotateOrders.Insert(0, orderToAdd);

			this.OrderCountThreadSafe++;
			if (orderToAdd.InStateExpectingCallbackFromBroker) this.OrdersCurrentlyProcessingByBrokerCount++;
			
			this.OrdersTree.InsertToShadowTreeRaiseExecutionFormNotification(0, orderToAdd);
			//OrdersTree RaisesTheSameEvent!!! make sure you won't receive it twice
			this.OrderProcessor.EventDistributor.RaiseOrderAddedExecutionFormNotification(this, orderToAdd);
		}


		public void OrdersRemove(List<Order> ordersToRemove, bool externalUpdatesWillBeTriggeredUpstack = false) {
			this.OrdersAll.RemoveAll(ordersToRemove);
			this.OrdersTree.RemoveFromShadowTree(ordersToRemove);
			this.SerializerLogrotateOrders.Remove(ordersToRemove);
			if (externalUpdatesWillBeTriggeredUpstack == false) {
				this.SerializerLogrotateOrders.HasChangesToSave = true;
				this.UpdateActiveOrdersCountEvent();
			}
		}
		public void OrdersRemoveNonPendingForAccounts(List<string> accountNumbers) {
			foreach (string accountNumber in accountNumbers) {
				List<Order> ordersForAccount = this.OrdersAll.FindAllForAccount(accountNumber); 
				this.OrdersRemove(ordersForAccount, true);
			}
			this.SerializerLogrotateOrders.HasChangesToSave = true;
			this.UpdateActiveOrdersCountEvent();
		}
		public void UpdateActiveOrdersCountEvent() {
			return;
			//lock (this.ordersLock) {
			//	this.OrderCountThreadSafe = this.Orders.Count;
			//	this.OrdersCurrentlyProcessingByBrokerCount = 0;
			//	foreach (Order order in this.Orders) {
			//		if (order.ExpectingCallbackFromBroker) {
			//			this.OrdersCurrentlyProcessingByBrokerCount++;
			//		}
			//	}
			//}
			//this.OrderProcessor.EventDistributor.RaiseOrderAddedConsumedByMainModuleUpdateOrdersTotalAndActive();
		}

		public OrderListByState FindStateLaneExpectedByOrderState(OrderState orderState) {
			if (this.OrdersSubmitting		.StateIsAcceptable(orderState))	return this.OrdersSubmitting;
			if (this.OrdersPending			.StateIsAcceptable(orderState))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.StateIsAcceptable(orderState))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.StateIsAcceptable(orderState))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.StateIsAcceptable(orderState))	return this.OrdersCemeterySick;
			return new OrderListByState(OrderStatesCollections.Unknown);
		}
		public OrderListByState FindStateLaneWhichContainsOrder(Order order) {
			if (this.OrdersSubmitting.Contains(order))		return this.OrdersSubmitting;
			if (this.OrdersPending.Contains(order))			return this.OrdersPending;
			if (this.OrdersPendingFailed.Contains(order))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy.Contains(order))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick.Contains(order))	return this.OrdersCemeterySick;
			return new OrderListByState(OrderStatesCollections.Unknown);
		}

		private object orderStateUpdateAtomicJustInCase = new object();
		public void SwitchLanesForOrderPostStatusUpdate(Order orderNowAfterUpdate, OrderState orderStatePriorToUpdate) {
			string msig = " OrderProcessorDataSnapshot::SwitchLanesForOrderPostStatusUpdate() ";
			lock (this.orderStateUpdateAtomicJustInCase) {
				OrderListByState orderLaneBeforeStateUpdate = this.FindStateLaneExpectedByOrderState(orderStatePriorToUpdate);
				OrderListByState  orderLaneAfterStateUpdate = this.FindStateLaneExpectedByOrderState(orderNowAfterUpdate.State);
				if (orderLaneBeforeStateUpdate == orderLaneAfterStateUpdate) return;
				try {
					orderLaneBeforeStateUpdate.Remove(orderNowAfterUpdate);
				} catch (Exception e) {
					Assembler.PopupException(msig, e, false);
				}
				try {
					orderLaneAfterStateUpdate.Insert(0, orderNowAfterUpdate);
				} catch (Exception e) {
					Assembler.PopupException(msig, e, false);
				}
			}
		}

		public OrderListByState FindStateLaneDoesntContain(Order order) {
			OrderListByState expectedToNotContain = this.FindStateLaneExpectedByOrderState(order.State);
			if (expectedToNotContain.Contains(order)) {
				expectedToNotContain = new OrderListByState(OrderStatesCollections.Unknown);
			}
			return expectedToNotContain;
		}
	}
}