using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Sq1.Core.Execution;
using Sq1.Core.Serializers;

namespace Sq1.Core.Broker {
	public partial class OrderProcessorDataSnapshot {
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
				this.suggestedLanes_forBrokerCallbackTradeState	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll_lanesSuggestor };
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
				this.suggestedLanes_forBrokerCallbackOrderState	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll_lanesSuggestor };
				//this.suggestedLanes_forBrokerCallbackOrderState.Add(this.SuggestLane_byOrderState_nullUnsafe(OrderState.Filled));
			}
			return this.suggestedLanes_forBrokerCallbackOrderState;
		} }

				List<OrderLane>		suggestedLanes_forBrokerCallbackTransactionReply;
		public	List<OrderLane>		SuggestedLanes_forBrokerCallbackTransactionReply		{ get {
			if (this.suggestedLanes_forBrokerCallbackTransactionReply == null) {
				this.suggestedLanes_forBrokerCallbackTransactionReply	= new List<OrderLane>() { this.OrdersPending, this.OrdersSubmitting, this.OrdersAll_lanesSuggestor };
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

	}
}
