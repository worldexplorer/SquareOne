using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public class OrderProcessorDataSnapshot {
		// REASON_TO_EXIST: having lanes makes order lookups faster; by having order state I know which List I'll be looking for GUID
		// useful when 100,000 orders are filled sucessfully and I received a UpdatePending notification for GUID=99,999 SCAN_FROM_END_OF_LIST?
		// CemeteryHealthy would contain all
		public OrderLaneByState		OrdersSubmitting			{ get; private set; }
		public OrderLaneByState		OrdersPending				{ get; private set; }
		public OrderLaneByState		OrdersPendingFailed			{ get; private set; }
		public OrderLaneByState		OrdersCemeteryHealthy		{ get; private set; }
		public OrderLaneByState		OrdersCemeterySick			{ get; private set; }
		public OrderLane			OrdersAll					{ get; private set; }

		public int					OrdersAll_Count				{ get { return this.OrdersAll.Count; } }

			   OrderProcessor		orderProcessor;
		public int					OrderCount					{ get; private set; }
		public int					OrdersExpectingBrokerUpdateCount_notUsed;			//{ get; private set; }
		public OrdersRootOnly		OrdersRootOnly				{ get; private set; }
			   object				orderSwitchingLanesLock;

		public SerializerLogrotatePeriodic<Order>	SerializerLogrotateOrders	{ get; private set; }
		//public Dictionary<Account, List<Order>>	OrdersByAccount				{ get; private set; }

		protected OrderProcessorDataSnapshot() {
			OrdersSubmitting			= new OrderLaneByState(OrderStatesCollections.AllowedForSubmissionToBrokerAdapter);
			OrdersPending				= new OrderLaneByState(OrderStatesCollections.NoInterventionRequired);
			OrdersPendingFailed			= new OrderLaneByState(OrderStatesCollections.InterventionRequired);
			OrdersCemeteryHealthy		= new OrderLaneByState(OrderStatesCollections.CemeteryHealthy);
			OrdersCemeterySick			= new OrderLaneByState(OrderStatesCollections.CemeterySick);
			OrdersAll					= new OrderLane("OrdersAll", this);
			//OrdersByAccount			= new Dictionary<Account, List<Order>>();

			SerializerLogrotateOrders	= new SerializerLogrotatePeriodic<Order>();
			if (SerializerLogrotateOrders.OfWhat != "Order") {
				string msg = "SerializerLogrotateOrders.OfWhat[" + SerializerLogrotateOrders.OfWhat + "] != [Order]";
				Assembler.PopupException(msg);
			}
			OrdersRootOnly				= new OrdersRootOnly("OrdersRootOnly");
			orderSwitchingLanesLock		= new object();
		}
		public void Clear_onLivesimStart__TODO_saveAndRestoreIfLivesimLaunchedDuringLive() {
			int ordersDropped = 0;
			ordersDropped += OrdersSubmitting		.Clear();
			ordersDropped += OrdersPending			.Clear();
			ordersDropped += OrdersPendingFailed	.Clear();
			ordersDropped += OrdersCemeteryHealthy	.Clear();
			ordersDropped += OrdersCemeterySick		.Clear();
			ordersDropped += OrdersAll				.Clear();
			if (ordersDropped > 0) {
				string msg = "YOU_MUST_HAVE_LOST_LIVE_PENDING_ORDERS__CHECK_FOR_PENDING_ALERTS? ordersDropped[" + ordersDropped + "]";
				Assembler.PopupException(msg, null, false);
			}
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
				this.OrdersRootOnly.InitializeScanDeserialized_moveDerivedsInside_buildTreeShadow(this.OrdersAll);
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
			if (orderToAdd.InState_expectingBrokerCallback) this.OrdersExpectingBrokerUpdateCount_notUsed++;
			
			this.OrdersRootOnly.InsertUnique_onlyToRoot(orderToAdd);

			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (orderToAdd.Alert.GuiHasTime_toRebuildReportersAndExecution == false) return;
			this.orderProcessor.RaiseOnOrderAdded_executionControlShouldRebuildOLV_scheduled(this, new List<Order>(){orderToAdd});
		}
		public void OrdersRemoveRange_fromAllLanes(List<Order> ordersToRemove, bool serializeSinceThisIsNotBatchRemove = true) {
			string msig = " //OrdersRemoveRange_fromAllLanes(ordersToRemove.Count[" + ordersToRemove.Count + "])";

			this.OrdersAll				.RemoveRange(ordersToRemove);
			this.OrdersRootOnly			.Remove_fromRootLevel_keepOrderPointers(ordersToRemove);

			this.OrdersSubmitting		.RemoveRange(ordersToRemove, false);
			this.OrdersPending			.RemoveRange(ordersToRemove, false);
			this.OrdersPendingFailed	.RemoveRange(ordersToRemove, false);
			this.OrdersCemeteryHealthy	.RemoveRange(ordersToRemove, false);
			this.OrdersCemeterySick		.RemoveRange(ordersToRemove, false);

			this.orderProcessor.RaiseOnOrdersRemoved_executionControlShouldRebuildOLV_scheduled(this, ordersToRemove);

			string log_SLO = "before.Count[" + this.SerializerLogrotateOrders.Orders.Count + "]";
			this.SerializerLogrotateOrders.Remove(ordersToRemove);
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
				List<Order> ordersForAccount = this.OrdersAll.ScanRecent_findAllForAccount(accountNumber); 
				this.OrdersRemoveRange_fromAllLanes(ordersForAccount);
			}
			this.SerializerLogrotateOrders.HasChangesToSave = true;
		}

		public OrderLaneByState SuggestLane_byOrderState_nullUnsafe(OrderState orderState) {
			if (this.OrdersSubmitting		.StateIsAcceptable(orderState))	return this.OrdersSubmitting;
			if (this.OrdersPending			.StateIsAcceptable(orderState))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.StateIsAcceptable(orderState))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.StateIsAcceptable(orderState))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.StateIsAcceptable(orderState))	return this.OrdersCemeterySick;
			return null;
		}
		public OrderLaneByState SuggestLane_forOrderGuidScan_nullUnsafe(Order order) {
			if (this.OrdersSubmitting		.ContainsGuid(order))	return this.OrdersSubmitting;
			if (this.OrdersPending			.ContainsGuid(order))	return this.OrdersPending;
			if (this.OrdersPendingFailed	.ContainsGuid(order))	return this.OrdersPendingFailed;
			if (this.OrdersCemeteryHealthy	.ContainsGuid(order))	return this.OrdersCemeteryHealthy;
			if (this.OrdersCemeterySick		.ContainsGuid(order))	return this.OrdersCemeterySick;
			return null;
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
		//public OrderLaneByState FindStateLaneDoesntContain(Order order) {
		//	OrderLaneByState expectedToNotContain = this.SuggestLaneByOrderState(order.State);
		//	if (expectedToNotContain.Contains(order)) {
		//		string msg = "HOW_WILL_YOU_USE_UNKNOWN???";
		//		Assembler.PopupException(msg);
		//		expectedToNotContain = new OrderLaneByState(OrderStatesCollections.Unknown);
		//	}
		//	return expectedToNotContain;
		//}

		Order scanLanes_forOrderGuid(string orderGuid, List<OrderLane> lanes, out string log_orEmptyWhenFound, string msig_invoker,
				bool followFirstSuggestion = false, bool nullifyDeserialized = true) {
			Order ret = null;
			log_orEmptyWhenFound = "";

			OrderLane	suggestedLane_nullUnsafe = null;
			string		suggestion = "PASS_suggestLane=TRUE";

			try {
				int i = 0;
				foreach (OrderLane lane in lanes) {
					ret = lane.ScanRecent_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion, followFirstSuggestion);
					if (ret != null) break;

					if (log_orEmptyWhenFound != "") log_orEmptyWhenFound += ",";
					log_orEmptyWhenFound += "SCAN_ATTEMPT_" + i
						+ " orderGuid_NOT_FOUND[" + orderGuid + "] " + lane.ToString()
						+ " sessionSernos[" + lane.SessionSernosAsString + "]";

					if (followFirstSuggestion == false) continue;
					if (suggestedLane_nullUnsafe != null) {
						ret = lane.ScanRecent_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion, followFirstSuggestion);
						if (ret != null) break;

						if (log_orEmptyWhenFound != "") log_orEmptyWhenFound += ",";
						log_orEmptyWhenFound += "SUGGESTION_ATTEMPT_" + i
							+ " orderGuid_NOT_FOUND[" + orderGuid + "] " + lane.ToString()
							+ " sessionSernos[" + lane.SessionSernosAsString + "]";
					}
					i++;
				}
			} catch (Exception ex) {
				log_orEmptyWhenFound += " orderGuid_NOT_FOUND_IN_PENDINGS_AND_SUGGESTED ex[" + ex.Message + "]"
						+ " suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]";
				Assembler.PopupException(log_orEmptyWhenFound + msig_invoker, ex);
			}


			if (log_orEmptyWhenFound != "") {
				log_orEmptyWhenFound = "LANES_SCANNED [" + log_orEmptyWhenFound + "]";
			}

			if (ret != null && nullifyDeserialized) ret = ret.EnsureOrder_isLiveOrLivesim_nullIfDeserialized();

			return ret;
		}

		Order scanLanes_forSernoExchange(long sernoExchange, List<OrderLane> lanes, out string log_orEmptyWhenFound, string msig_invoker,
				bool followFirstSuggestion = false, bool nullifyDeserialized = true) {
			Order ret = null;
			log_orEmptyWhenFound = "";

			OrderLane	suggestedLane_nullUnsafe = null;
			string		suggestion = "PASS_suggestLane=TRUE";

			try {
				int i = 0;
				foreach (OrderLane lane in lanes) {
					ret = lane.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion, followFirstSuggestion);
					if (ret != null) break;

					if (log_orEmptyWhenFound != "") log_orEmptyWhenFound += ",";
					log_orEmptyWhenFound += "SCAN_ATTEMPT_" + i
						+ " sernoExchange_NOT_FOUND[" + sernoExchange + "] " + lane.ToString()
						+ " sessionSernos[" + lane.SessionSernosAsString + "]";

					if (followFirstSuggestion == false) continue;
					if (suggestedLane_nullUnsafe != null) {
						ret = lane.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion, followFirstSuggestion);
						if (ret != null) break;

						if (log_orEmptyWhenFound != "") log_orEmptyWhenFound += ",";
						log_orEmptyWhenFound += "SUGGESTION_ATTEMPT_" + i
							+ " sernoExchange_NOT_FOUND[" + sernoExchange + "] " + lane.ToString()
							+ " sessionSernos[" + lane.SessionSernosAsString + "]";
					}
					i++;
				}
			} catch (Exception ex) {
				log_orEmptyWhenFound += " sernoExchange_NOT_FOUND_IN_PENDINGS_AND_SUGGESTED ex[" + ex.Message + "]"
						+ " suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]";
				Assembler.PopupException(log_orEmptyWhenFound + msig_invoker, ex);
			}


			if (log_orEmptyWhenFound != "") {
				log_orEmptyWhenFound = "LANES_SCANNED [" + log_orEmptyWhenFound + "]";
			}

			if (ret != null && nullifyDeserialized) ret = ret.EnsureOrder_isLiveOrLivesim_nullIfDeserialized();

			return ret;
		}


				List<OrderLane>		suggestedLanes_forBrokerCallbackTradeState;
		public	List<OrderLane>		SuggestedLanes_forBrokerCallbackTradeState		{ get {
			if (this.suggestedLanes_forBrokerCallbackTradeState == null) {
				this.suggestedLanes_forBrokerCallbackTradeState	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll };
				//this.suggestedLanes_forBrokerCallbackTradeState.Add(this.SuggestLane_byTradeState_nullUnsafe(TradeState.Filled));
			}
			return this.suggestedLanes_forBrokerCallbackTradeState;
		} }

		public Order ScanLanes_bySernoExchange_forBrokerCallbackTradeState_nullUnsafe(long sernoExchange, out string log_orEmptyWhenFound,
																		string msig_invoker = null, List<OrderLane> lanesToScan = null) {
			log_orEmptyWhenFound = "";

			if (msig_invoker == null) msig_invoker = " //ScanLanes_bySernoExchange_forBrokerCallbackTradeState_nullUnsafe(" + sernoExchange + ")";
			if (lanesToScan == null) lanesToScan = this.SuggestedLanes_forBrokerCallbackOrderState;

			Order ret = this.scanLanes_forSernoExchange(sernoExchange, lanesToScan, out log_orEmptyWhenFound, msig_invoker);

			log_orEmptyWhenFound += ret == null
				? " NOT_FOUND_BY_SERNO_EXCHANGE[" + sernoExchange + "] [" + log_orEmptyWhenFound + "]"
				: " FOUND_BY_SERNO_EXCHANGE[" + sernoExchange + "] [" + log_orEmptyWhenFound + "] order[" + ret + "]";

			return ret;


			//string		suggestion					= "PASS_suggestLane=TRUE";
			//OrderLane	suggestedLane_nullUnsafe	= null;
			//Order ret = this.OrdersPending.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion);
			//msg_findOrder = "";
			//if (ret == null) {
			//    msg_findOrder += " SIMULATING_weirdRetardedTradeStatusCallback_preventingEnteringNextPosition"
			//        + " sometimes, quik admits TradeCommitted after having already filled => everything gets stuck";
			//    ret = this.OrdersAll.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion);
			//}
			//if (ret == null) {
			//    msg_findOrder = "FAILED_TO_FIND_ORDER_IN_OrdersPending&all sernoExchange[" + sernoExchange + "]"
			//        + " suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]"
			//        + msg_findOrder;
			//}
			//return ret;
		}
		public Order ScanLanes_bySernoExchange_forBrokerCallbackTradeStatusTestable_nullUnsafe(long sernoExchange, out string log_orEmptyWhenFound, //int nMode,
																		string msig_invoker = null, List<OrderLane> lanesToScan = null) {
			log_orEmptyWhenFound = "";

			if (msig_invoker == null) msig_invoker = " //ScanLanes_bySernoExchange_forBrokerCallbackTradeStatusTestable_nullUnsafe(" + sernoExchange + ")";
			if (lanesToScan == null) lanesToScan = this.SuggestedLanes_forBrokerCallbackOrderState;

			Order ret = this.scanLanes_forSernoExchange(sernoExchange, lanesToScan, out log_orEmptyWhenFound, msig_invoker, false, false);

			log_orEmptyWhenFound += ret == null
				? " NOT_FOUND_BY_SERNO_EXCHANGE[" + sernoExchange + "] [" + log_orEmptyWhenFound + "]"
				: " FOUND_BY_SERNO_EXCHANGE[" + sernoExchange + "] [" + log_orEmptyWhenFound + "] order[" + ret + "]";

			return ret;
			
			
			////OrderLane	laneToSearch = nMode == 0 ? snap.OrdersPending : snap.OrdersAll;	// you may want to wrap this method in lock(){} <= despite all Lanes are sync'ed, two async callbacks may not find the order while moving
			//OrderLane	laneToSearch = this.OrdersAll;
			//OrderLane	suggestedLane_nullUnsafe = null;
			//string		suggestion = "PASS_suggestLane=TRUE";
			//msg_findingOrder = "";
			//Order ret = null;
			//try {
			//    msg_findingOrder += " SEARCHING_IN_laneToSearch[" + laneToSearch + "] ";
			//    ret = laneToSearch.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion);

			//    if (ret == null) {
			//        msg_findingOrder += ": NOT_FOUND; searching suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]"
			//            //+ " hoping to find order if TradeStatus arrived for Limit after OrderStatus already moved order around"
			//            ;
			//        if (suggestedLane_nullUnsafe != null && suggestedLane_nullUnsafe != laneToSearch) {
			//            ret = suggestedLane_nullUnsafe.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion);
			//            if (ret == null && nMode == 0) {
			//                msg_findingOrder += ": NOT_FOUND_LAST_CHANCE_MODE_0; searching OrdersAll"
			//                    //+ " hoping to find order if TradeStatus arrived for Limit after OrderStatus already moved order around"
			//                    ;
			//                ret = this.OrdersAll.ScanRecent_forSernoExchange(sernoExchange, out suggestedLane_nullUnsafe, out suggestion);
			//            }
			//        }
			//    }
			//} catch (Exception ex) {
			//    msg_findingOrder += " sernoExchange_NOT_FOUND_IN_PENDINGS_AND_SUGGESTED ex[" + ex.Message + "]"
			//            + " suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]";
			//    Assembler.PopupException(msg_findingOrder + msig_invoker, ex);
			//}

			return ret;
		}


				List<OrderLane>		suggestedLanes_forBrokerCallbackOrderState;
		public	List<OrderLane>		SuggestedLanes_forBrokerCallbackOrderState		{ get {
			if (this.suggestedLanes_forBrokerCallbackOrderState == null) {
				this.suggestedLanes_forBrokerCallbackOrderState	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll };
				//this.suggestedLanes_forBrokerCallbackOrderState.Add(this.SuggestLane_byOrderState_nullUnsafe(OrderState.Filled));
			}
			return this.suggestedLanes_forBrokerCallbackOrderState;
		} }

				List<OrderLane>		suggestedLanes_forBrokerCallbackTransactionReply;
		public	List<OrderLane>		SuggestedLanes_forBrokerCallbackTransactionReply		{ get {
			if (this.suggestedLanes_forBrokerCallbackTransactionReply == null) {
				this.suggestedLanes_forBrokerCallbackTransactionReply	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll };
				//this.suggestedLanes_forBrokerCallbackTransactionReply.Add(this.SuggestLane_byTransactionReply_nullUnsafe(TransactionReply.Filled));
			}
			return this.suggestedLanes_forBrokerCallbackTransactionReply;
		} }


		public Order ScanLanes_byOrderGuid_forBrokerCallbackOrderState_nullUnsafe(string orderGuid, out string log_orEmptyWhenFound,
																		string msig_invoker = null, List<OrderLane> lanesToScan = null) {
			log_orEmptyWhenFound = "";

			if (msig_invoker == null) msig_invoker = " //ScanLanes_byOrderGuid_forBrokerCallbackOrderState_nullUnsafe(" + orderGuid + ")";
			if (lanesToScan == null) lanesToScan = this.SuggestedLanes_forBrokerCallbackOrderState;

			Order ret = this.scanLanes_forOrderGuid(orderGuid, lanesToScan, out log_orEmptyWhenFound, msig_invoker, false, false);

			log_orEmptyWhenFound += ret == null
				? " NOT_FOUND_BY_GUID[" + orderGuid + "] [" + log_orEmptyWhenFound + "]"
				: " FOUND_BY_GUID[" + orderGuid + "] order[" + ret + "]";

			return ret;

			//OrderLane	suggestedLane_nullUnsafe = null;
			//string		suggestion = "PASS_suggestLane=TRUE";
			//string		logOrEmpty = "";
			//if (ret == null) {
			//    msg_findingOrder += "NOT_FOUND_IN_LanesForCallbackOrderState__LOOKING_IN_FILLED ";
			//    var hopefullyCemeteryHealthy = new List<OrderLane>() {
			//        this.SuggestLane_byOrderState_nullUnsafe(OrderState.Filled)
			//    };
			//    ret = this.scanLanes_forOrderGuid(orderGuid, hopefullyCemeteryHealthy, out logOrEmpty);
			//    if (ret == null) {
			//        msg_findingOrder += " NOT_FOUND_IN_LANES__LOOKING_IN_ALL ";
			//        ret = this.OrdersAll.Scan_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion);
			//        if (ret == null && suggestedLane_nullUnsafe != null) {
			//            msg_findingOrder += ": NOT_FOUND; searching suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]"
			//                //+ " hoping to find order if TradeStatus arrived for Limit after OrderStatus already moved order around"
			//                ;
			//            ret = suggestedLane_nullUnsafe.Scan_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion);
			//        }
			//    }
			//}
		}
		public Order ScanLanes_byOrderGuid_forBrokerCallbackTransactionReply_nullUnsafe(string orderGuid, out string log_orEmptyWhenFound,
																		string msig_invoker = null, List<OrderLane> lanesToScan = null) {
			log_orEmptyWhenFound = "";

			if (msig_invoker == null) msig_invoker = " //ScanLanes_byOrderGuid_forBrokerCallbackTransactionReply_nullUnsafe(" + orderGuid + ")";
			if (lanesToScan == null) lanesToScan = this.SuggestedLanes_forBrokerCallbackTransactionReply;

			Order ret = this.scanLanes_forOrderGuid(orderGuid, lanesToScan, out log_orEmptyWhenFound, msig_invoker);

			log_orEmptyWhenFound += ret == null
				? " NOT_FOUND_BY_GUID[" + orderGuid + "] [" + log_orEmptyWhenFound + "]"
				: " FOUND_BY_GUID[" + orderGuid + "] order[" + ret + "]";

			return ret;

			//OrderLane	suggestedLane_nullUnsafe = null;
			//string		suggestion = "PASS_suggestLane=TRUE";
			//Order ret = this.OrdersPending.Scan_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion);
			//if (ret == null) {
			//    ret = this.OrdersAll.Scan_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion);
			//    ret = this.OrdersPending.Scan_forOrderGuid(orderGuid, out suggestedLane_nullUnsafe, out suggestion);

			//    log_orEmptyWhenFound = "NOT_FOUND Guid[" + orderGuid + "] ; orderSernos=[" + this.OrdersPending.SessionSernosAsString + "] Count=[" + this.OrdersPending.Count + "]"
			//            + " suggestedLane[" + suggestedLane_nullUnsafe + "] suggestion[" + suggestion + "]";
			//    //Assembler.PopupException(msigHead + msg_findingOrder, null, false);
			//    //return;
			//}
		}

		public override string ToString() {
			string ret = "";

			//int itemsCnt			= this.ExecutionTreeControl.OlvOrdersTree.Items.Count;
			int allCnt				= this.OrdersAll				.Count;
			int submittingCnt		= this.OrdersSubmitting			.Count;
			int pendingCnt			= this.OrdersPending			.Count;
			int pendingFailedCnt	= this.OrdersPendingFailed		.Count;
			int cemeteryHealtyCnt	= this.OrdersCemeteryHealthy	.Count;
			int cemeterySickCnt		= this.OrdersCemeterySick		.Count;
			int fugitive			= allCnt - (submittingCnt + pendingCnt + pendingFailedCnt + cemeteryHealtyCnt + cemeterySickCnt);

										ret +=		   cemeteryHealtyCnt	+ " Filled/Killed/Killers";
										ret += " | " + pendingCnt			+ " Pending";
			if (submittingCnt > 0)		ret += " | " + submittingCnt		+ " Submitting";
			if (pendingFailedCnt > 0)	ret += " | " + pendingFailedCnt		+ " PendingFailed";
			if (cemeterySickCnt > 0)	ret += " | " + cemeterySickCnt		+ " DeadFromSickness";
										ret += " :: "+ allCnt				+ " Total";
			//if (itemsCnt != allCnt)		ret += " | " + itemsCnt			+ " Displayed";
			if (fugitive > 0)			ret += ", " + fugitive				+ " Deserialized";		//PrevLaunch";

			return ret;
		}
	}
}
