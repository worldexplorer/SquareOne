using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	// REASON_TO_EXIST: 1) organize Find* to search RecentlyAdded first;	//Insert(), 
	// Popup when you Find something logically located in another lane;		//suggestLanePopupException()
	public class OrderLane {	// TODO : OrderListWD
		readonly	OrderProcessorDataSnapshot neighborLanesWhenOrdersAll;
		readonly	string		ident;

		protected	object	OrdersLock;
		protected	List<Order>	InnerOrderList			{ get; private set; }
		public		List<Order>	SafeCopy				{ get { lock (this.OrdersLock) { return new List<Order>(this.InnerOrderList); } } }
		public		string		SessionSernosAsString	{ get { lock (this.OrdersLock) {
					//const string sessionSernos = "";
					//return this.Aggregate(sessionSernos, (current, order) => current + (" " + order.SernoSession));
					string ret = "";
					foreach (Order order in this.InnerOrderList) ret += order.SernoSession + " "; 
					ret.TrimEnd(" ,".ToCharArray());
					return ret;
				} } }
		public		List<string>	OrdersGuids				{ get; protected set; }
		public Order FirstNullUnsafe { get { lock (this.OrdersLock) {
			Order ret = null;
			if (this.Count > 0) ret = this.InnerOrderList[0];
			return ret;
		} } }

		public OrderLane(string ident, List<Order> ordersInit, OrderProcessorDataSnapshot neighborLanes = null) : this(ident, neighborLanes) {
			this.InnerOrderList.InsertRange(0, ordersInit);
		}
		public OrderLane(string ident, OrderProcessorDataSnapshot neighborLanes = null) : this() {
			this.ident = ident;
			this.neighborLanesWhenOrdersAll = neighborLanes;
		}
		protected OrderLane() : base() {
			this.InnerOrderList = new List<Order>();
			this.OrdersLock = new Object();
			this.InnerOrderList.Capacity = 2000;
			this.OrdersGuids = new List<string>();
		}

		void suggestLanePopupException(Order order) {
			if (this.neighborLanesWhenOrdersAll == null) return;
			string msg = "";
			if (order == null) {
				msg = "DONT_PASS_ORDER_NULL_TO_SUGGEST_LANE";
				//this.neighborLanesWhenOrdersAll.OrderProcessor.PopupException(new Exception(msg));
				Assembler.PopupException(msg, null, true);
				return;
			}
			Stopwatch fullScanTook = new Stopwatch();
			fullScanTook.Start();
			OrderLaneByState laneFound;
			OrderLaneByState laneExpected = this.neighborLanesWhenOrdersAll.SuggestLaneByOrderStateNullUnsafe(order.State);
			if (laneExpected != null) {
				if (laneExpected.Contains(order)) {
					msg += "FOUND_IN_EXPECTED_LANE";
					laneFound = laneExpected;
				} else {
					msg += "NOT_FOUND_WHERE_EXPECTED_TRYING_FULL_SEARCH";
					OrderLaneByState lanesFullScan = neighborLanesWhenOrdersAll.ScanLanesForOrderGuidNullUnsafe(order);
					if (lanesFullScan.StatesAllowed == OrderStatesCollections.Unknown) {
						msg += "; only OrdersAll contains this order, pass iDontNeedSuggestionsHere=true";
						return;
					}
					laneFound = lanesFullScan;
				}

				msg = "USE [" + laneFound.ToString() + "] instead of OrdersAll: " + msg;
			} else {
				msg = "NO_SUGGESTION_FOR[" + order.State + "] instead of OrdersAll: " + msg;
			}
			fullScanTook.Stop();
			msg += " order[" + order + "]";
			msg = "(" + fullScanTook.ElapsedMilliseconds + "ms) " + msg;
			Assembler.PopupException(msg, null, true);
		}
		protected virtual bool checkThrowAdd	(Order order) { return true; }
		protected virtual bool checkThrowRemove	(Order order) { return true; }

		public void Insert(Order order) { lock (this.OrdersLock) {
			if (this.checkThrowAdd(order) == false) return;
			if (this.Contains(order) == true) {
				string msg = "Already in " + this.ToString() + ": " + order;
				throw new Exception(msg);
				//break;
			}
			order.AddedToOrdersListCounter++;
			this.InnerOrderList.Insert(0, order);
			this.OrdersGuids.Insert(0, order.GUID);
		} }
		public int Count { get { lock (this.OrdersLock) {
			return this.InnerOrderList.Count;
		} } } 
		public bool Contains(Order order) { lock (this.OrdersLock) {
			//return base.Contains(order);
			return this.OrdersGuids.Contains(order.GUID);
		} }
		public void Clear() { lock (this.OrdersLock) {
			// TODO: foreach (var order in base) order.AddedToOrdersListCounter--
			this.InnerOrderList.Clear();
			this.OrdersGuids.Clear();
		} }
		public  bool Remove(Order order) { lock (this.OrdersLock) {
			if (this.checkThrowRemove(order) == false) return false;
			if (this.Contains(order) == false) {
				string msg2 = "REMOVING_ORDER_NOT_FOUND in " + this.ToString()
					+ ": (already removed or never stored before? broker callback Dupe?) " + order;
				throw new Exception(msg2);
				//break;
			}
			order.AddedToOrdersListCounter--;
			this.OrdersGuids.Remove(order.GUID);
			return this.InnerOrderList.Remove(order);
		} }
		public int RemoveAll(List<Order> ordersToRemove, bool popupIfDoesntContain = true) { lock (this.OrdersLock) {
			int removed = 0;
			foreach (Order orderRemoving in ordersToRemove) {
				if (this.InnerOrderList.Contains(orderRemoving) == false) {
					if (popupIfDoesntContain && orderRemoving.Alert.MyBrokerIsLivesim == false) {
						string msg = "LANE_DOESNT_CONTAIN_ORDER_YOU_WILLING_TO_REMOVE " + this.ToStringCount() + " orderRemoving" + orderRemoving;
						Assembler.PopupException(msg);
					}
					continue;
				}
				this.InnerOrderList.Remove(orderRemoving);
				this.OrdersGuids.Remove(orderRemoving.GUID);
				removed++;
			}
			return removed;
		} }
		public int RemoveForAccount(string acctNum) { lock (this.OrdersLock) {
			return this.RemoveAll(this.ScanRecentFindAllForAccount(acctNum));
		} }
		public Order ScanRecentForSernoExchange(long SernoExchange, bool iDontNeedSuggestionsHere = false) {
			Order found = null;
			lock (this.OrdersLock) {
				foreach (Order order in this.InnerOrderList) {
					if (order.SernoExchange != SernoExchange) continue;
					found = order;
					break;
				}
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		}
		public Order ScanRecentForGUID(string orderGUID, bool iDontNeedSuggestionsHere = false) { lock (this.OrdersLock) {
			Order found = null;
			foreach (Order order in this.InnerOrderList) {
				if (order.GUID != orderGUID) continue;
				found = order;
				break;
			}
			if (found == null) return found;
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		} }

		public Order FindMatchingForAlert(Alert alert, bool iDontNeedSuggestionsHere = false) { lock (this.OrdersLock) {
			Order found = null;
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(alert.OrderFollowed);
			// replace with a property if (alert.IsSameBarExit) return null;
			foreach (Order order in this.InnerOrderList) {
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
				if (order.Alert.IsIdenticalOrderlessPriceless(alert)) {
					found = order; break;
				}
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		} }
		public Order ScanRecentForSimilarNotSamePendingOrder(Order order, bool iDontNeedSuggestionsHere = false) { lock (this.OrdersLock) {
			Order found = null;
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(order);
			//if (OrdersPendingBrokerCallbackStore == false) return null;
			foreach (Order orderSimilar in this.InnerOrderList) {
				if (orderSimilar.Alert == null) continue;	// orders deserialized might have no alerts attached
				if (order == orderSimilar) continue;
				if (order.Alert.IsIdenticalForOrdersPending(orderSimilar.Alert) == false) continue;
				found = orderSimilar;
				break;
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		} }
		public List<Order> ScanRecentFindAllForAccount(string acctNum, bool ignoreExpectingCallbackFromBroker = false, bool iDontNeedSuggestionsHere = false) {
			List<Order> ordersForAccount = new List<Order>();
			if (string.IsNullOrEmpty(acctNum)) {
				string msg = "GetOrdersForAccount([" + acctNum + "]) - returning empty list";
				Assembler.PopupException(msg);
				return ordersForAccount;
			}
			lock (this.OrdersLock) {
				foreach (Order current in this.InnerOrderList) {
					if (ignoreExpectingCallbackFromBroker) {
						if (current.InStateExpectingCallbackFromBroker == true) continue;
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
		public			string	ToStringCount()		{ return this.ToString() + ".Count=[" + this.InnerOrderList.Count + "]"; }
	}
}
