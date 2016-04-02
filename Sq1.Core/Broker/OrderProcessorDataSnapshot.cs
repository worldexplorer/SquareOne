using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public class OrderProcessorDataSnapshot {
		// REASON_TO_EXIST: having lanes makes order lookups faster; by having order state I know which List I'll be looking for GUID
		// useful when 100,000 orders are filled sucessfully and I received a UpdatePending notification for GUID=99,999 SCAN_FROM_END_OF_LIST?
		// CemeteryHealthy would contain all
		public OrderLaneByState		OrdersSubmitting							{ get; private set; }
		public OrderLaneByState		OrdersPending								{ get; private set; }
		public OrderLaneByState		OrdersPendingFailed							{ get; private set; }
		public OrderLaneByState		OrdersCemeteryHealthy						{ get; private set; }
		public OrderLaneByState		OrdersCemeterySick							{ get; private set; }
		public OrderLane			OrdersAll									{ get; private set; }

			   OrderProcessor		orderProcessor;
		public int					OrderCount									{ get; private set; }
		public int					OrdersExpectingBrokerUpdateCount;			//{ get; private set; }
		public OrdersAutoTree		OrdersAutoTree								{ get; private set; }
			   object				orderSwitchingLanesLock;

		public SerializerLogrotatePeriodic<Order>	SerializerLogrotateOrders	{ get; private set; }
		//public Dictionary<Account, List<Order>>	OrdersByAccount				{ get; private set; }

		public List<OrderLane>		LanesForCallbackOrderState					{ get; private set; }

		protected OrderProcessorDataSnapshot() {
			OrdersSubmitting			= new OrderLaneByState(OrderStatesCollections.AllowedForSubmissionToBrokerAdapter);
			OrdersPending				= new OrderLaneByState(OrderStatesCollections.NoInterventionRequired);
			OrdersPendingFailed			= new OrderLaneByState(OrderStatesCollections.InterventionRequired);
			OrdersCemeteryHealthy		= new OrderLaneByState(OrderStatesCollections.CemeteryHealthy);
			OrdersCemeterySick			= new OrderLaneByState(OrderStatesCollections.CemeterySick);
			OrdersAll					= new OrderLane("OrdersAll", this);
			//OrdersByAccount			= new Dictionary<Account, List<Order>>();

			SerializerLogrotateOrders	= new SerializerLogrotatePeriodic<Order>();
			OrdersAutoTree				= new OrdersAutoTree();
			orderSwitchingLanesLock		= new object();

			LanesForCallbackOrderState	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll };
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
				List<Order> ordersInit = this.SerializerLogrotateOrders.Orders;
				foreach (Order current in ordersInit) {
					if (current.InState_expectingBrokerCallback) {
						current.SetState_localTimeNow(OrderState.SubmittedNoFeedback);
					}
				}
				// yeps we spawn the lists with the same content;
				// original, OrdersBuffered.ItemsMain will shrink later due to LogrotateSerializer.safeLogRotate()
				// the copy, this.OrdersAll will stay the longest orderlist (request this.OrdersAll.SafeCopy if you got CollectionModifiedException)
				// OrdersTree will also stay as full as OrdersAll, but serves as DataSource for ExecutionTree in VirtualMode
				// adding/removing to OrdersAll should add/remove to OrdersBuffered and OrdersTree (slow but true)
				this.OrdersAll = new OrderLane("OrdersAll", ordersInit, this);
				this.OrdersAutoTree.InitializeScanDeserialized_moveDerivedsInside_buildTreeShadow(this.OrdersAll);
			} catch (Exception ex) {
				string msg = "THROWN_OrderProcessorDataSnapshot.Initialize()";
				Assembler.PopupException(msg, ex, false);
			}
			this.SerializerLogrotateOrders.StartSerializerThread();
		}

		public void OrderInsert_notifyGuiAsync(Order orderToAdd) {
			string msg = "HEY_I_REACHED_THIS_POINT__NO_EXCEPTIONS_SO_FAR?";
			//Debugger.Break();
			//#D_HANGS Assembler.PopupException(msg);
			//MOVED_TO_RaiseAsyncOrderAddedExecutionFormShouldRebuildOLV() handler Assembler.PopupExecutionForm();

			this.OrdersAll.InsertUnique(orderToAdd);
			if (orderToAdd.Alert.Strategy.Script.Executor.BacktesterOrLivesimulator.ImRunningLivesim == false) {
				string msg1 = "DONT_SPAM_ORDER_LOG_WITH_LIVESIMULATOR_ORDERS";
				this.SerializerLogrotateOrders.Insert(0, orderToAdd);
			}

			this.OrderCount++;
			if (orderToAdd.InState_expectingBrokerCallback) this.OrdersExpectingBrokerUpdateCount++;
			
			this.OrdersAutoTree.InsertUnique_toRoot(orderToAdd);

			if (orderToAdd.Alert.GuiHasTimeRebuildReportersAndExecution == false) return;
			this.orderProcessor.RaiseAsyncOrderAdded_executionControlShouldRebuildOLV(this, new List<Order>(){orderToAdd});
		}
		public void OrdersRemove(List<Order> ordersToRemove, bool serializeSinceThisIsNotBatchRemove = true) {
			this.OrdersAll				.RemoveAll(ordersToRemove);
			this.OrdersAutoTree			.Remove_fromRootLevel_keepOrderPointers(ordersToRemove);

			this.OrdersSubmitting		.RemoveAll(ordersToRemove, false);
			this.OrdersPending			.RemoveAll(ordersToRemove, false);
			this.OrdersPendingFailed	.RemoveAll(ordersToRemove, false);
			this.OrdersCemeteryHealthy	.RemoveAll(ordersToRemove, false);
			this.OrdersCemeterySick		.RemoveAll(ordersToRemove, false);

			this.orderProcessor.RaiseAsyncOrderRemoved_executionControlShouldRebuildOLV(this, ordersToRemove);
			this.SerializerLogrotateOrders.Remove(ordersToRemove);
			this.SerializerLogrotateOrders.HasChangesToSave = true;
			if (serializeSinceThisIsNotBatchRemove) {
				this.SerializerLogrotateOrders.Serialize();
			}
		}
		public void OrdersRemove_nonPending_forAccounts(List<string> accountNumbers) {
			foreach (string accountNumber in accountNumbers) {
				List<Order> ordersForAccount = this.OrdersAll.ScanRecent_findAllForAccount(accountNumber); 
				this.OrdersRemove(ordersForAccount);
			}
			this.SerializerLogrotateOrders.HasChangesToSave = true;
		}

		public OrderLaneByState SuggestLaneByOrderState_nullUnsafe(OrderState orderState) {
			if (this.OrdersSubmitting		.StateIsAcceptable(orderState))	return this.OrdersSubmitting;
			if (this.OrdersPending			.StateIsAcceptable(orderState))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.StateIsAcceptable(orderState))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.StateIsAcceptable(orderState))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.StateIsAcceptable(orderState))	return this.OrdersCemeterySick;
			return null;
		}
		public OrderLaneByState ScanLanesForOrderGuid_nullUnsafe(Order order) {
			if (this.OrdersSubmitting		.ContainsGuid(order))	return this.OrdersSubmitting;
			if (this.OrdersPending			.ContainsGuid(order))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.ContainsGuid(order))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.ContainsGuid(order))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.ContainsGuid(order))	return this.OrdersCemeterySick;
			return null;
		}

		public void SwitchLanes_forOrder_postStatusUpdate(Order orderNowAfterUpdate, OrderState orderStatePriorToUpdate) { lock (this.orderSwitchingLanesLock) {
			string msig = " //OrderProcessorDataSnapshot.SwitchLanes_forOrder_postStatusUpdate()";
			OrderLaneByState orderLaneBeforeStateUpdate = this.SuggestLaneByOrderState_nullUnsafe(orderStatePriorToUpdate);
			OrderLaneByState  orderLaneAfterStateUpdate = this.SuggestLaneByOrderState_nullUnsafe(orderNowAfterUpdate.State);
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
					orderLaneAfterStateUpdate.InsertUnique(orderNowAfterUpdate);
				} catch (Exception ex) {
					Assembler.PopupException("FAILED_TO_INSERT orderNowAfterUpdate=[" + orderNowAfterUpdate + "]" + msig, ex, false);
				}
			}
		} }
		//public OrderLaneByState FindStateLaneDoesntContain(Order order) {
		//	OrderLaneByState expectedToNotContain = this.SuggestLaneByOrderState(order.State);
		//	if (expectedToNotContain.Contains(order)) {
		//		string msg = "HOW_WILL_YOU_USE_UNKNOWN???";
		//		Assembler.PopupException(msg);
		//		expectedToNotContain = new OrderLaneByState(OrderStatesCollections.Unknown);
		//	}
		//	return expectedToNotContain;
		//}

		public Order ScanRecent_forGUID(string GUID, List<OrderLane> lanes, out string logOrEmpty) {
			Order ret = null;
			logOrEmpty = "";

			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";

			foreach (OrderLane lane in lanes) {
				ret = lane.ScanRecent_forGuid(GUID.ToString(), out suggestedLane, out suggestion, false);
				if (ret != null) break;
				logOrEmpty += lane.ToStringCount() + " sessionSernos(" + lane.SessionSernosAsString + "),";
			}
			logOrEmpty.Trim(",".ToCharArray());

			if (logOrEmpty != "") {
				logOrEmpty = "LANES_SCANNED [" + logOrEmpty + "]";
			}

			return ret;
		}
	}
}
