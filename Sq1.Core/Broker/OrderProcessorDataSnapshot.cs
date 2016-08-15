using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public partial class OrderProcessorDataSnapshot {
		// REASON_TO_EXIST: having lanes makes order lookups faster; by having order state I know which List I'll be looking for GUID
		// useful when 100,000 orders are filled sucessfully and I received a UpdatePending notification for GUID=99,999 SCAN_FROM_END_OF_LIST?
		// CemeteryHealthy would contain all
		public OrderLaneByState		OrdersSubmitting			{ get; private set; }
		public OrderLaneByState		OrdersPending				{ get; private set; }
		public OrderLaneByState		OrdersPendingFailed			{ get; private set; }
		public OrderLaneByState		OrdersCemeteryHealthy		{ get; private set; }
		public OrderLaneByState		OrdersCemeterySick			{ get; private set; }

		public OrderLane			OrdersAll_lanesSuggestor	{ get; private set; }
		public int					OrdersAll_Count				{ get { return this.OrdersAll_lanesSuggestor.Count; } }

		public OrdersSearchable		OrdersSearchable_forGui		{ get; private set; }
		public int					OrderCount					{ get; private set; }
		
		public OrdersByState		OrdersByState				{ get; private set; }
		public int					OrdersByState_KeysCount		{ get { return this.OrdersByState.Count; } }
		
		public OrdersRootOnly		OrdersRootOnly				{ get; private set; }
		
		public OrderProcessorDataViewProxy	OrdersViewProxy		{ get; private set; }

			   object				orderSwitchingLanesLock;

			   OrderProcessor		orderProcessor;
		public int					OrdersExpectingBrokerUpdateCount_notUsed;			//{ get; private set; }

		public SerializerLogrotatePeriodic<Order>	SerializerLogrotateOrders	{ get; private set; }
		//public Dictionary<Account, List<Order>>	OrdersByAccount				{ get; private set; }

		protected OrderProcessorDataSnapshot() {
			OrdersSubmitting			= new OrderLaneByState(OrderStatesCollections.AllowedForSubmissionToBrokerAdapter);
			OrdersPending				= new OrderLaneByState(OrderStatesCollections.NoInterventionRequired);
			OrdersPendingFailed			= new OrderLaneByState(OrderStatesCollections.InterventionRequired);
			OrdersCemeteryHealthy		= new OrderLaneByState(OrderStatesCollections.CemeteryHealthy);
			OrdersCemeterySick			= new OrderLaneByState(OrderStatesCollections.CemeterySick);

			OrdersAll_lanesSuggestor	= new OrderLane("OrderProcessorDataSnapshot=>OrdersAll_lanesSuggestor", this);
			OrdersSearchable_forGui			= new OrdersSearchable("OrderProcessorDataSnapshot=>OrdersAll_forGui");
			OrdersRootOnly				= new OrdersRootOnly("OrdersRootOnly");
			OrdersByState				= new OrdersByState("OrderProcessorDataSnapshot=>OrdersByState", new OrdersByState.ASC());
			//OrdersByAccount			= new Dictionary<Account, List<Order>>();
			
			OrdersViewProxy				= new OrderProcessorDataViewProxy(this);

			SerializerLogrotateOrders	= new SerializerLogrotatePeriodic<Order>();
			if (SerializerLogrotateOrders.OfWhat != "Order") {
				string msg = "SerializerLogrotateOrders.OfWhat[" + SerializerLogrotateOrders.OfWhat + "] != [Order]";
				Assembler.PopupException(msg);
			}
			orderSwitchingLanesLock		= new object();
		}
		public OrderProcessorDataSnapshot(OrderProcessor orderProcessor) : this() {
			this.orderProcessor = orderProcessor;
		}
		public void Initialize(string rootPath) {
			string msig = " //OrderProcessorDataSnapshot.Initialize(rootPath[" + rootPath + "])";

			bool createdNewFile = this.SerializerLogrotateOrders.Initialize(rootPath, "Orders.json", "Orders", null);
			try {
				this.SerializerLogrotateOrders.Deserialize();
				// OrdersTree was historically introduced the last, but filling Order.DerivedOrders early here, just in case
				//this.OrdersTree.InitializeScanDeserializedMoveDerivedsInsideBuildTreeShadow(this.SerializerLogRotate.OrdersBuffered.ItemsMain);
				List<Order> ordersInit = this.SerializerLogrotateOrders.Orders;
				foreach (Order current in ordersInit) {
					// deserialized_change_state
					if (current.InState_expectingBrokerCallback) {
						current.SetState_localTimeNow(OrderState.SubmittedNoFeedback);
					}
					// 2. deserialized_restore Serno for when Orders didn't have it serialized (once from prev version)
					//ConcurrentStack<OrderStateMessage> stackIsNew_MessagesAreTheSame = current.MessagesSafeCopy;
					//if (stackIsNew_MessagesAreTheSame.Count > 0) {
					//    OrderStateMessage last = null;
					//    if (stackIsNew_MessagesAreTheSame.TryPeek(out last)) {
					//        if (last.Serno == 0) {
					//            List<OrderStateMessage> asList = new List<OrderStateMessage>(stackIsNew_MessagesAreTheSame);
					//            int i = 1;
					//            foreach (OrderStateMessage omsg in asList) {
					//                omsg.Serno = i;
					//                i++;
					//            }
					//        }
					//    }
					//}
					// 3. lookup & set pointer "<TODO: RESTORE_VictimToBeKilled_fromGuid_onTreeBuild>"
				}
				// yeps we spawn the lists with the same content;
				// original, OrdersBuffered.ItemsMain will shrink later due to LogrotateSerializer.safeLogRotate()
				// the copy, this.OrdersAll will stay the longest orderlist (request this.OrdersAll.SafeCopy if you got CollectionModifiedException)
				// OrdersTree will also stay as full as OrdersAll, but serves as DataSource for ExecutionTree in VirtualMode
				// adding/removing to OrdersAll should add/remove to OrdersBuffered and OrdersTree (slow but true)
				this.OrdersAll_lanesSuggestor = new OrderLane("OrdersAll", ordersInit, this);
				this.OrdersRootOnly.InitializeScanDeserialized_moveDerivedsInside_buildTreeShadow(this.OrdersAll_lanesSuggestor);

				this.OrdersByState.Clear_unsubscribe(this, msig);
				this.OrdersByState.InitializeDeserialized(this.OrdersAll_lanesSuggestor);
				this.OrdersSearchable_forGui.AddRange(ordersInit, this, msig);
			} catch (Exception ex) {
				string msg = "THROWN_OrderProcessorDataSnapshot.Initialize()";
				Assembler.PopupException(msg, ex, false);
			}
			this.SerializerLogrotateOrders.StartSerializerThread();
		}

		public void OrderInsert_notifyGuiAsync(Order orderToAdd) {
			string msig = " //OrderInsert_notifyGuiAsync(" + orderToAdd + ")";
			string msg = "HEY_I_REACHED_THIS_POINT__NO_EXCEPTIONS_SO_FAR?";
			//Debugger.Break();
			//#D_HANGS Assembler.PopupException(msg);
			//MOVED_TO_RaiseAsyncOrderAddedExecutionFormShouldRebuildOLV() handler Assembler.PopupExecutionForm();

			this.OrdersAll_lanesSuggestor.InsertUnique(orderToAdd);
			this.OrdersSearchable_forGui.InsertUnique(orderToAdd, this, msig);
			this.OrdersByState.Add_intoListForState(orderToAdd);

			if (orderToAdd.Alert.Strategy.Script.Executor.BacktesterOrLivesimulator.ImRunningLivesim == false) {
				string msg1 = "DONT_SPAM_ORDER_LOG_WITH_LIVESIMULATOR_ORDERS";
				this.SerializerLogrotateOrders.Insert(0, orderToAdd);
			}

			this.OrderCount++;
			if (orderToAdd.InState_expectingBrokerCallback) this.OrdersExpectingBrokerUpdateCount_notUsed++;
			
			this.OrdersRootOnly.InsertUnique_onlyToRoot(orderToAdd);

			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (orderToAdd.Alert.GuiHasTime_toRebuildReportersAndExecution == false) return;
			this.orderProcessor.RaiseOnOrderAdded_executionControlShouldRebuildOLV_scheduled(this, new List<Order>(){orderToAdd});
		}
		public void OrdersRemoveRange_fromAllLanes(List<Order> ordersToRemove, bool serializeSinceThisIsNotBatchRemove = true) {
			string msig = " //OrdersRemoveRange_fromAllLanes(ordersToRemove.Count[" + ordersToRemove.Count + "])";

			this.OrdersByState				.RemoveRange(ordersToRemove);
			this.OrdersSearchable_forGui	.RemoveRange(ordersToRemove, this, msig);
			this.OrdersRootOnly				.Remove_fromRootLevel_keepOrderPointers(ordersToRemove);

			this.OrdersAll_lanesSuggestor	.RemoveRange(ordersToRemove);
			this.OrdersSubmitting			.RemoveRange(ordersToRemove, false);
			this.OrdersPending				.RemoveRange(ordersToRemove, false);
			this.OrdersPendingFailed		.RemoveRange(ordersToRemove, false);
			this.OrdersCemeteryHealthy		.RemoveRange(ordersToRemove, false);
			this.OrdersCemeterySick			.RemoveRange(ordersToRemove, false);

			this.orderProcessor.RaiseOnOrdersRemoved_executionControlShouldRebuildOLV_scheduled(this, ordersToRemove);

			string log_SLO = "before.Count[" + this.SerializerLogrotateOrders.Orders.Count + "]";
			this.SerializerLogrotateOrders.RemoveRange(ordersToRemove);
				  log_SLO += " after.Count[" + this.SerializerLogrotateOrders.Orders.Count + "]";
			this.SerializerLogrotateOrders.HasChangesToSave = true;
			if (serializeSinceThisIsNotBatchRemove) {
				this.SerializerLogrotateOrders.Serialize();
				log_SLO = "SERIALIZED_SLO " + log_SLO;
			} else {
				log_SLO = "NOT_SERIALIZED_SLO " + log_SLO;
			}
			Assembler.PopupException(log_SLO + msig, null, false);
		}
		public void OrdersRemove_forAccounts_nonPending(List<string> accountNumbers) {
			foreach (string accountNumber in accountNumbers) {
				List<Order> ordersForAccount = this.OrdersAll_lanesSuggestor.ScanRecent_findAllForAccount(accountNumber); 
				this.OrdersRemoveRange_fromAllLanes(ordersForAccount);
			}
			this.SerializerLogrotateOrders.HasChangesToSave = true;
		}
		public void Clear_onLivesimStart__TODO_saveAndRestoreIfLivesimLaunchedDuringLive() {
			string msig = " //Clear_onLivesimStart__TODO_saveAndRestoreIfLivesimLaunchedDuringLive()";
			int ordersDropped = 0;
			ordersDropped += this.OrdersSubmitting			.Clear();
			ordersDropped += this.OrdersPending				.Clear();
			ordersDropped += this.OrdersPendingFailed		.Clear();
			ordersDropped += this.OrdersCemeteryHealthy		.Clear();
			ordersDropped += this.OrdersCemeterySick		.Clear();
			ordersDropped += this.OrdersAll_lanesSuggestor	.Clear();
			ordersDropped += this.OrdersByState				.Clear_unsubscribe(this, msig);
			ordersDropped += this.OrdersSearchable_forGui	.Clear(this, msig);

			if (ordersDropped > 0) {
				string msg = "YOU_MUST_HAVE_LOST_LIVE_PENDING_ORDERS__CHECK_FOR_PENDING_ALERTS? ordersDropped[" + ordersDropped + "]";
				Assembler.PopupException(msg, null, false);
			}
		}

		public void SwitchLanes_forOrder_postStatusUpdate(Order orderNowAfterUpdate, OrderState orderStatePriorToUpdate) { lock (this.orderSwitchingLanesLock) {
			string msig = " //OrderProcessorDataSnapshot.SwitchLanes_forOrder_postStatusUpdate()";
			OrderLaneByState orderLaneBeforeStateUpdate = this.SuggestLane_byOrderState_nullUnsafe(orderStatePriorToUpdate);
			OrderLaneByState  orderLaneAfterStateUpdate = this.SuggestLane_byOrderState_nullUnsafe(orderNowAfterUpdate.State);
			if (orderLaneBeforeStateUpdate == orderLaneAfterStateUpdate) return;
			if (orderLaneBeforeStateUpdate != null) {
				try {
					orderLaneBeforeStateUpdate.RemoveUnique(orderNowAfterUpdate);
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

		public override string ToString() {
			string ret = "";

			//int itemsCnt			= this.ExecutionTreeControl.OlvOrdersTree.Items.Count;
			int allCnt				= this.OrdersAll_lanesSuggestor		.Count;
			int guiCnt				= this.OrdersAll_lanesSuggestor		.Count;
			int statesCnt			= this.OrdersByState_KeysCount;
			int submittingCnt		= this.OrdersSubmitting				.Count;
			int pendingCnt			= this.OrdersPending				.Count;
			int pendingFailedCnt	= this.OrdersPendingFailed			.Count;
			int cemeteryHealtyCnt	= this.OrdersCemeteryHealthy		.Count;
			int cemeterySickCnt		= this.OrdersCemeterySick			.Count;
			int fugitive			= allCnt - (submittingCnt + pendingCnt + pendingFailedCnt + cemeteryHealtyCnt + cemeterySickCnt);

										ret +=		   cemeteryHealtyCnt	+ " Filled/Killed/Killers";
										ret += " | " + pendingCnt			+ " Pending";
			if (submittingCnt > 0)		ret += " | " + submittingCnt		+ " Submitting";
			if (pendingFailedCnt > 0)	ret += " | " + pendingFailedCnt		+ " PendingFailed";
			if (cemeterySickCnt > 0)	ret += " | " + cemeterySickCnt		+ " DeadFromSickness";
										ret += " :: "+ allCnt				+ " Total";
										ret += " (" + guiCnt				+ " Gui)";
			if (statesCnt > 0)			ret += " in " + statesCnt			+ " States";
			//if (itemsCnt != allCnt)		ret += " | " + itemsCnt			+ " Displayed";
			if (fugitive > 0)			ret += ", " + fugitive				+ " Deserialized";		//PrevLaunch";

			return ret;
		}
	}
}
