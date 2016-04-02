using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	// REASON_TO_EXIST: 1) organize Find* to search RecentlyAdded first;	//Insert(), 
	// Popup when you Find something logically located in another lane;		//suggestLanePopupException()
	public class OrderLane {	// TODO : OrderListWD
		readonly	OrderProcessorDataSnapshot neighborLanesWhenOrdersAll;
		readonly	string			ident;

		protected	object			OrdersLock;
		protected	List<Order>		InnerOrderList_recentFirst			{ get; private set; }
		public		List<Order>		SafeCopy				{ get { lock (this.OrdersLock) { return new List<Order>(this.InnerOrderList_recentFirst); } } }
		public		string			SessionSernosAsString	{ get { lock (this.OrdersLock) {
					//const string sessionSernos = "";
					//return this.Aggregate(sessionSernos, (current, order) => current + (" " + order.SernoSession));
					string ret = "";
					foreach (Order order in this.InnerOrderList_recentFirst) ret += order.SernoSession + " "; 
					ret.TrimEnd(" ,".ToCharArray());
					return ret;
				} } }
		public		List<string>	OrdersGuids_recentFirst				{ get; protected set; }
		public Order First_nullUnsafe { get { lock (this.OrdersLock) {
			Order ret = null;
			if (this.Count > 0) ret = this.InnerOrderList_recentFirst[0];
			return ret;
		} } }

		public OrderLane(string ident, List<Order> ordersInit, OrderProcessorDataSnapshot neighborLanes = null) : this(ident, neighborLanes) {
			this.InnerOrderList_recentFirst.InsertRange(0, ordersInit);
		}
		public OrderLane(string ident, OrderProcessorDataSnapshot neighborLanes = null) : this() {
			this.ident = ident;
			this.neighborLanesWhenOrdersAll = neighborLanes;
		}
		protected OrderLane() : base() {
			this.InnerOrderList_recentFirst = new List<Order>();
			this.OrdersLock = new Object();
			this.InnerOrderList_recentFirst.Capacity = 2000;
			this.OrdersGuids_recentFirst = new List<string>();
		}

		OrderLaneByState suggestLane_popupException(Order order, out string suggestion, bool popupKoz_imNotSavingSuggestion_inOrderMessages = true) {
			suggestion = "";
			OrderLaneByState laneFound = null;
			if (this.neighborLanesWhenOrdersAll == null) {
				suggestion = "DONT_ASK_MY_SUGGESTION__IM_NOT_OrdersAll";
				return laneFound;
			}
			if (order == null) {
				suggestion = "DONT_PASS_ORDER_NULL_TO_SUGGEST_LANE";
				//this.neighborLanesWhenOrdersAll.OrderProcessor.PopupException(new Exception(msg));
				if (popupKoz_imNotSavingSuggestion_inOrderMessages) Assembler.PopupException(suggestion, null, false);
				return laneFound;
			}
			Stopwatch fullScanTook = new Stopwatch();
			fullScanTook.Start();
			OrderLaneByState laneExpected = this.neighborLanesWhenOrdersAll.SuggestLaneByOrderState_nullUnsafe(order.State);
			if (laneExpected != null) {
				if (laneExpected.ContainsGuid(order)) {
					suggestion += "FOUND_IN_EXPECTED_LANE";
					laneFound = laneExpected;
				} else {
					suggestion += "NOT_FOUND_WHERE_EXPECTED_TRYING_FULL_SEARCH";
					OrderLaneByState lanesFullScan = neighborLanesWhenOrdersAll.ScanLanesForOrderGuid_nullUnsafe(order);
					if (lanesFullScan == null) {
						suggestion = "NULL_SUGGESTION_FOR[" + order.State + "] instead of OrdersAll: " + suggestion;
					} else {
						if (lanesFullScan.StatesAllowed == OrderStatesCollections.Unknown) {
							suggestion += "; only OrdersAll contains this order, pass suggestLane=false";
						} else {
							laneFound = lanesFullScan;
							suggestion = "USE [" + laneFound.ToString() + "] instead of OrdersAll: " + suggestion;
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
			return laneFound;
		}
		protected virtual bool checkThrowAdd	(Order order) { return true; }
		protected virtual bool checkThrowRemove	(Order order) { return true; }

		public void InsertUnique(Order order) { lock (this.OrdersLock) {
			if (this.checkThrowAdd(order) == false) return;
			if (this.ContainsGuid(order) == true) {
				string msg = "Already in " + this.ToString() + ": " + order;
				throw new Exception(msg);
				//break;
			}
			order.AddedToOrdersListCounter++;
			this.InnerOrderList_recentFirst.Insert(0, order);
			this.OrdersGuids_recentFirst.Insert(0, order.GUID);
		} }
		public int Count { get { lock (this.OrdersLock) {
			return this.InnerOrderList_recentFirst.Count;
		} } } 
		public bool ContainsGuid(Order order) { lock (this.OrdersLock) {
			//return base.Contains(order);
			return this.OrdersGuids_recentFirst.Contains(order.GUID);
		} }
		public void Clear() { lock (this.OrdersLock) {
			// TODO: foreach (var order in base) order.AddedToOrdersListCounter--
			this.InnerOrderList_recentFirst.Clear();
			this.OrdersGuids_recentFirst.Clear();
		} }
		public  bool Remove(Order order) { lock (this.OrdersLock) {
			if (this.checkThrowRemove(order) == false) return false;
			if (this.ContainsGuid(order) == false) {
				string msg2 = "REMOVING_ORDER_NOT_FOUND in " + this.ToString()
					+ ": (already removed or never stored before? broker callback Dupe?) " + order;
				throw new Exception(msg2);
				//break;
			}
			order.AddedToOrdersListCounter--;
			this.OrdersGuids_recentFirst.Remove(order.GUID);
			return this.InnerOrderList_recentFirst.Remove(order);
		} }
		public int RemoveAll(List<Order> ordersToRemove, bool popupIf_doesntContain = true) { lock (this.OrdersLock) {
			int removed = 0;
			foreach (Order orderRemoving in ordersToRemove) {
				if (this.InnerOrderList_recentFirst.Contains(orderRemoving) == false) {
					if (popupIf_doesntContain && orderRemoving.Alert.MyBrokerIsLivesim == false) {
						string msg = "LANE_DOESNT_CONTAIN_ORDER_YOU_WILLING_TO_REMOVE " + this.ToStringCount() + " orderRemoving" + orderRemoving;
						Assembler.PopupException(msg, null, false);
					}
					continue;
				}
				this.InnerOrderList_recentFirst.Remove(orderRemoving);
				this.OrdersGuids_recentFirst.Remove(orderRemoving.GUID);
				removed++;
			}
			return removed;
		} }
		public int RemoveForAccount(string acctNum) { lock (this.OrdersLock) {
			return this.RemoveAll(this.ScanRecent_findAllForAccount(acctNum));
		} }
		public Order ScanRecent_forSernoExchange(long SernoExchange, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) {
			Order found = null;
			lock (this.OrdersLock) {
				foreach (Order order in this.InnerOrderList_recentFirst) {
					if (order.SernoExchange != SernoExchange) continue;
					found = order;
					break;
				}
			}
			suggestedLane = null;
			suggestion = "PASS_suggestLane=TRUE";
			if (suggestLane) suggestedLane = this.suggestLane_popupException(found, out suggestion);
			return found;
		}
		public Order ScanRecent_forGuid(string orderGUID, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) { lock (this.OrdersLock) {
			Order found = null;
			foreach (Order order in this.InnerOrderList_recentFirst) {
				if (order.GUID != orderGUID) continue;
				found = order;
				break;
			}
			suggestedLane = null;
			suggestion = "PASS_suggestLane=TRUE";
			if (found == null) return found;
			if (suggestLane) suggestedLane = this.suggestLane_popupException(found, out suggestion);
			return found;
		} }

		public Order FindOrder_matchingForAlert(Alert alert, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) { lock (this.OrdersLock) {
			Order found = null;
			suggestedLane = null;
			suggestion = "PASS_suggestLane=TRUE";
			if (suggestLane) suggestedLane = this.suggestLane_popupException(found, out suggestion);
			// replace with a property if (alert.IsSameBarExit) return null;
			foreach (Order order in this.InnerOrderList_recentFirst) {
				if (order.Alert == null) {
					string msg = "do deserialized Orders only have no reference to its parent Alerts?";
					continue;
				}
				if (order.Alert.Bars == null) {
					string msg = "historical Order should not be used for matching";
					continue;
				}
				if (order.Alert == alert) {
					found = order; break;
				}
				if (order.Alert.IsIdentical_orderlessPriceless(alert)) {
					found = order; break;
				}
			}
			return found;
		} }
		public Order ScanRecent_forSimilarPendingOrder_notSame(Order order, out OrderLane suggestedLane, out string suggestion, bool suggestLane = true) { lock (this.OrdersLock) {
			Order found = null;
			suggestedLane = null;
			suggestion = "PASS_suggestLane=TRUE";
			if (suggestLane) suggestedLane = this.suggestLane_popupException(found, out suggestion);
			//if (OrdersPendingBrokerCallbackStore == false) return null;
			foreach (Order orderSimilar in this.InnerOrderList_recentFirst) {
				if (orderSimilar.Alert == null) continue;	// orders deserialized might have no alerts attached
				if (order == orderSimilar) continue;
				if (order.Alert.IsIdentical_forOrdersPending(orderSimilar.Alert) == false) continue;
				found = orderSimilar;
				break;
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
			lock (this.OrdersLock) {
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

		public override string	ToString()			{ return this.ident; }
		public			string	ToStringCount()		{ return this.ToString() + ".Count=[" + this.InnerOrderList_recentFirst.Count + "]"; }
	}
}
