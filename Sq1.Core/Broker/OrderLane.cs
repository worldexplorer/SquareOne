using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	// REASON_TO_EXIST: 1) organize Find* to search RecentlyAdded first;	//Insert(), 
	// Popup when you Find something logically located in another lane;		//suggestLanePopupException()
	public class OrderLane {	// TODO : OrderListWD
		readonly	OrderProcessorDataSnapshot neighborLanesWhenOrdersAll;
		readonly	string			reasonToExist;

					object			ordersLock;
		protected	List<Order>		InnerOrderList_recentFirst	{ get; private set; }
		public		List<Order>		SafeCopy					{ get { lock (this.ordersLock) { return new List<Order>(this.InnerOrderList_recentFirst); } } }
		public		string			SessionSernosAsString		{ get { lock (this.ordersLock) {
					//const string sessionSernos = "";
					//return this.Aggregate(sessionSernos, (current, order) => current + (" " + order.SernoSession));
					string ret = "";
					foreach (Order order in this.InnerOrderList_recentFirst) ret += order.SernoSession + " "; 
					ret.TrimEnd(" ,".ToCharArray());
					return ret;
				} } }
					List<string>	ordersGuids_recentFirst;	//{ get; protected set; }
		public		Order			MostRecent_nullUnsafe		{ get { lock (this.ordersLock) {
			Order ret = null;
			if (this.Count > 0) ret = this.InnerOrderList_recentFirst[0];
			return ret;
		} } }

		protected OrderLane() {
			this.InnerOrderList_recentFirst = new List<Order>();
			this.ordersLock = new Object();
			this.InnerOrderList_recentFirst.Capacity = 2000;
			this.ordersGuids_recentFirst = new List<string>();
		}
		public OrderLane(string reasonToExist_passed, OrderProcessorDataSnapshot neighborLanes = null) : this() {
			this.reasonToExist = reasonToExist_passed;
			this.neighborLanesWhenOrdersAll = neighborLanes;
		}
		public OrderLane(string ident, List<Order> ordersInit, OrderProcessorDataSnapshot neighborLanes = null) : this(ident, neighborLanes) {
			this.InnerOrderList_recentFirst.InsertRange(0, ordersInit);
		}

		OrderLaneByState suggestLane_nullUnsafe_popupException(Order order, out string suggestion, bool popupKoz_imNotSavingSuggestion_inOrderMessages = true) {
			suggestion = "";
			OrderLaneByState ret = null;
			if (this.neighborLanesWhenOrdersAll == null) {
				suggestion = "DONT_ASK_MY_SUGGESTION__IM_NOT_OrdersAll";
				return ret;
			}
			if (order == null) {
				suggestion = "DONT_PASS_ORDER_NULL_TO_SUGGEST_LANE";
				//this.neighborLanesWhenOrdersAll.OrderProcessor.PopupException(new Exception(msg));
				if (popupKoz_imNotSavingSuggestion_inOrderMessages) Assembler.PopupException(suggestion, null, false);
				return ret;
			}
			Stopwatch fullScanTook = new Stopwatch();
			fullScanTook.Start();
			OrderLaneByState laneExpected = this.neighborLanesWhenOrdersAll.SuggestLane_byOrderState_nullUnsafe(order.State);
			if (laneExpected != null) {
				if (laneExpected.ContainsGuid(order)) {
					suggestion += "FOUND_IN_EXPECTED_LANE";
					ret = laneExpected;
				} else {
					suggestion += "NOT_FOUND_WHERE_EXPECTED_TRYING_FULL_SEARCH";
					OrderLaneByState lanesFullScan = neighborLanesWhenOrdersAll.SuggestLane_forOrderGuidScan_nullUnsafe(order);
					if (lanesFullScan == null) {
						suggestion = "NULL_SUGGESTION_FOR[" + order.State + "] instead of OrdersAll: " + suggestion;
					} else {
						if (lanesFullScan.StatesAllowed == OrderStatesCollections.Unknown) {
							suggestion += "; only OrdersAll contains this order, pass suggestLane=false";
						} else {
							ret = lanesFullScan;
							suggestion = "USE [" + ret.ToString() + "] instead of OrdersAll: " + suggestion;
						}
					}
				}
			} else {
				suggestion = "NO_SUGGESTION_FOR[" + order.State + "] instead of OrdersAll: " + suggestion;
			}
			fullScanTook.Stop();
			suggestion += " order[" + order + "]";
			suggestion = "(" + fullScanTook.ElapsedMilliseconds + "ms) " + suggestion;
			if (popupKoz_imNotSavingSuggestion_inOrderMessages) Assembler.PopupException(suggestion, null, false);
			return ret;
		}
		protected virtual bool checkThrowAdd	(Order order) { return true; }
		protected virtual bool checkThrowRemove	(Order order) { return true; }

		public void InsertUnique(Order order) { lock (this.ordersLock) {
			if (this.checkThrowAdd(order) == false) return;
			if (this.ContainsGuid(order) == true) {
				string msg = "Already in " + this.ToString() + ": " + order;
				throw new Exception(msg);
				//break;
			}
			order.AddedToOrdersListCounter++;
			this.InnerOrderList_recentFirst.Insert(0, order);
			this.ordersGuids_recentFirst.Insert(0, order.GUID);
		} }
		public int Count { get { lock (this.ordersLock) {
			return this.InnerOrderList_recentFirst.Count;
		} } } 
		public bool ContainsGuid(Order order) { lock (this.ordersLock) {
			//return base.Contains(order);
			return this.ordersGuids_recentFirst.Contains(order.GUID);
		} }
		public int Clear() { lock (this.ordersLock) {
			int ordersDropped = this.Count;
			this.InnerOrderList_recentFirst.Clear();
			this.ordersGuids_recentFirst.Clear();
			return ordersDropped;
		} }
		public  bool RemoveUnique(Order order) { lock (this.ordersLock) {
			if (this.checkThrowRemove(order) == false) return false;
			if (this.ContainsGuid(order) == false) {
				string msg2 = "REMOVING_ORDER_NOT_FOUND in " + this.ToString()
					+ ": (already removed or never stored before? broker callback Dupe?) " + order;
				throw new Exception(msg2);
				//break;
			}
			order.AddedToOrdersListCounter--;
			this.ordersGuids_recentFirst.Remove(order.GUID);
			return this.InnerOrderList_recentFirst.Remove(order);
		} }
		public int RemoveRange(List<Order> ordersToRemove, bool popupIf_doesntContain = true) { lock (this.ordersLock) {
			int removed = 0;
			foreach (Order orderRemoving in ordersToRemove) {
				if (this.InnerOrderList_recentFirst.Contains(orderRemoving) == false) {
					if (popupIf_doesntContain && orderRemoving.Alert.MyBrokerIsLivesim == false) {
						string msg = "LANE_DOESNT_CONTAIN_ORDER_YOU_WILLING_TO_REMOVE " + this.ToString() + " orderRemoving" + orderRemoving;
						Assembler.PopupException(msg, null, false);
					}
					continue;
				}
				this.InnerOrderList_recentFirst.Remove(orderRemoving);
				this.ordersGuids_recentFirst.Remove(orderRemoving.GUID);
				removed++;
			}
			return removed;
		} }
		//public int RemoveForAccount(string acctNum) { lock (this.ordersLock) {
		//	return this.RemoveAll(this.ScanRecent_findAllForAccount(acctNum));
		//} }
		public Order ScanRecent_forSernoExchange(long sernoExchange, out OrderLane suggestedLane_nullUnsafe, out string suggestion, bool suggestLane = true) { lock (this.ordersLock) {
			suggestedLane_nullUnsafe = null;
			suggestion = "PASS_suggestLane=TRUE";
			Order found = null;
			foreach (Order order in this.InnerOrderList_recentFirst) {
				if (order.SernoExchange != sernoExchange) continue;
				found = order;
				break;
			}
			if (found != null) return found;
			if (suggestLane) suggestedLane_nullUnsafe = this.suggestLane_nullUnsafe_popupException(found, out suggestion);
			return found;
		} }
		public Order ScanRecent_forOrderGuid(string orderGUID, out OrderLane suggestedLane_nullUnsafe, out string suggestion, bool suggestLane = true) { lock (this.ordersLock) {
			suggestedLane_nullUnsafe = null;
			suggestion = "PASS_suggestLane=TRUE";
			Order found = null;
			foreach (Order order in this.InnerOrderList_recentFirst) {
				if (order.GUID != orderGUID) continue;
				found = order;
				break;
			}
			if (found != null) return found;
			if (suggestLane) suggestedLane_nullUnsafe = this.suggestLane_nullUnsafe_popupException(found, out suggestion);
			return found;
		} }

		//public Order FindOrder_matchingForAlert(Alert alert, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) { lock (this.ordersLock) {
		//	Order found = null;
		//	suggestedLane = null;
		//	suggestion = "PASS_suggestLane=TRUE";
		//	if (suggestLane) suggestedLane = this.suggestLane_popupException(found, out suggestion);
		//	// replace with a property if (alert.IsSameBarExit) return null;
		//	foreach (Order order in this.InnerOrderList_recentFirst) {
		//		if (order.Alert == null) {
		//			string msg = "do deserialized Orders only have no reference to its parent Alerts?";
		//			continue;
		//		}
		//		if (order.Alert.Bars == null) {
		//			string msg = "historical Order should not be used for matching";
		//			continue;
		//		}
		//		if (order.Alert == alert) {
		//			found = order; break;
		//		}
		//		if (order.Alert.IsIdentical_orderlessPriceless(alert)) {
		//			found = order; break;
		//		}
		//	}
		//	return found;
		//} }
		public Order ScanRecent_forSimilarPendingOrder_notSame(Order order, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) { lock (this.ordersLock) {
			Order found = null;
			suggestedLane = null;
			suggestion = "PASS_suggestLane=TRUE";
			//if (OrdersPendingBrokerCallbackStore == false) return null;
			foreach (Order orderSimilar in this.InnerOrderList_recentFirst) {
				if (orderSimilar.Alert == null) continue;	// orders deserialized might have no alerts attached
				if (order == orderSimilar) continue;
				if (order.Alert == orderSimilar.Alert) continue;		// killer has been derived from original; both have the same opening/closing Alert (here it was throwing up)
				if (order.Alert.IsIdentical_forOrdersPending(orderSimilar.Alert) == false) continue;
				found = orderSimilar;
				break;
			}
			if (suggestLane) {
				if (found != null) {
					suggestedLane = this.suggestLane_nullUnsafe_popupException(found, out suggestion);
				} else {
					suggestion = "NO_SUGGESTION_FOR_FOUND=NULL";		// "what do you want? bastard" - Taz :))
				}
			}
			return found;
		} }
		public List<Order> ScanRecent_findAllForAccount(string acctNum, bool ignoreExpectingCallbackFromBroker = false, bool suggestLane = true) {
			List<Order> ordersForAccount = new List<Order>();
			if (string.IsNullOrEmpty(acctNum)) {
				string msg = "GetOrdersForAccount([" + acctNum + "]) - returning empty list";
				Assembler.PopupException(msg);
				return ordersForAccount;
			}
			lock (this.ordersLock) {
				foreach (Order current in this.InnerOrderList_recentFirst) {
					if (ignoreExpectingCallbackFromBroker) {
						if (current.InState_expectingBrokerCallback == true) continue;
					}
					if (current.Alert == null) continue;
					//if (order.Alert.AccountNumber != acctNum && acctNum != "") {
					//	continue;
					//}
					if (current.Alert.AccountNumber != acctNum) continue;
					ordersForAccount.Add(current);
				}
			}
			return ordersForAccount;
		}

		public override string ToString() {
			return this.reasonToExist + ".Count=[" + this.Count + "]";
		}
	}
}
