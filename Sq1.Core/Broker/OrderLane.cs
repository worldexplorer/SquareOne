using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderLane {
		readonly OrderProcessorDataSnapshot neighborLanesWhenOrdersAll;
		readonly string		ident;
		protected Object	ordersLock;
		public List<Order>	InnerOrderList		{ get; protected set; }
		public List<Order>	SafeCopy			{ get { lock (this.ordersLock) { return new List<Order>(this.InnerOrderList); } } }
		public string		SessionSernos		{ get { lock (this.ordersLock) {
					//const string sessionSernos = "";
					//return this.Aggregate(sessionSernos, (current, order) => current + (" " + order.SernoSession));
					string ret = "";
					foreach (Order order in this.InnerOrderList.FindAll(null)) {
						ret += order.SernoSession + " "; 
					}
					ret.TrimEnd(" ,".ToCharArray());
					return ret;
				} } }
		public List<string>	OrdersGuids;

		public OrderLane(string ident, List<Order> ordersInit, OrderProcessorDataSnapshot neighborLanes = null) : this(ident, neighborLanes) {
			this.InnerOrderList.InsertRange(0, ordersInit);
		}
		public OrderLane(string ident, OrderProcessorDataSnapshot neighborLanes = null) : this() {
			this.ident = ident;
			this.neighborLanesWhenOrdersAll = neighborLanes;
		}
		protected OrderLane() : base() {
			this.InnerOrderList = new List<Order>();
			this.ordersLock = new Object();
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
			OrderLaneByState laneExpected = this.neighborLanesWhenOrdersAll.FindStateLaneExpectedByOrderState(order.State);
			if (laneExpected.Contains(order)) {
				msg += "FOUND_IN_EXPECTED_LANE";
				laneFound = laneExpected;
			} else {
				msg += "NOT_FOUND_WHERE_EXPECTED_TRYING_FULL_SEARCH";
				OrderLaneByState lanesFullScan = neighborLanesWhenOrdersAll.FindStateLaneWhichContainsOrder(order);
				if (lanesFullScan.StatesAllowed == OrderStatesCollections.Unknown) {
					msg += "; only OrdersAll contains this order, pass iDontNeedSuggestionsHere=true";
					return;
				}
				laneFound = lanesFullScan;
			}
			fullScanTook.Stop();
			msg += " (fullScanTook " + fullScanTook.ElapsedMilliseconds + "ms)";
			msg = "USE [" + laneFound.ToShortString() + "] instead of OrdersAll: " + msg;
			//this.neighborLanesWhenOrdersAll.OrderProcessor.PopupException(new Exception(msg));
			Assembler.PopupException(msg, null, false);
		}
		protected virtual bool checkThrowAdd(Order order) {
			return true;
		}
		protected virtual bool checkThrowRemove(Order order) {
			return true;
		}
		// List<Order>.Add wouldn't be neededed to override if inherited from ConcurrentQueue
		public new void Add(Order order) {
			string msg = "replace Add() with Insert(0), since you like to use foreach() which starts from the beginning";
			throw new Exception(msg);
		}

		// List<Order>.Insert wouldn't be neededed to override if inherited from ConcurrentQueue
		public new void Insert(int index, Order order) {
			if (checkThrowAdd(order) == false) return;
			lock (this.ordersLock) {
				if (this.Contains(order) == true) {
					string msg = "Already in " + this.ToString() + ": " + order;
					throw new Exception(msg);
					//break;
				}
				order.AddedToOrdersListCounter++;
				this.InnerOrderList.Insert(index, order);
				this.OrdersGuids.Insert(0, order.GUID);
			}
		}
		// List<Order>.Contains wouldn't be neededed to override if inherited from ConcurrentQueue
		public new bool Contains(Order order) {
			lock (this.ordersLock) {
				//return base.Contains(order);
				return this.OrdersGuids.Contains(order.GUID);
			}
		}
		// List<Order>.Clear wouldn't be neededed to override if inherited from ConcurrentQueue
		public new void Clear() {
			lock (this.ordersLock) {
				// TODO: foreach (var order in base) order.AddedToOrdersListCounter--
				this.InnerOrderList.Clear();
				this.OrdersGuids.Clear();
			}
		}
		// List<Order>.Remove wouldn't be neededed to override if inherited from ConcurrentQueue
		public new bool Remove(Order order) {
			if (checkThrowRemove(order) == false) return false;
			lock (this.ordersLock) {
				if (this.Contains(order) == false) {
					string msg2 = "REMOVING_ORDER_NOT_FOUND in " + this.ToString()
						+ ": (already removed or never stored before? broker callback Dupe?) " + order;
					throw new Exception(msg2);
					//break;
				}
				order.AddedToOrdersListCounter--;
				this.OrdersGuids.Remove(order.GUID);
				return this.InnerOrderList.Remove(order);
			}
		}
		// List<Order>.RemoveAll wouldn't be neededed to override if inherited from ConcurrentQueue
		public int RemoveAll(List<Order> ordersToRemove) {
			int removed = 0;
			lock (this.ordersLock) {
				foreach (Order orderRemoving in ordersToRemove) {
					this.InnerOrderList.Remove(orderRemoving);
					this.OrdersGuids.Remove(orderRemoving.GUID);
					removed++;
				}
			}
			return removed;
		}
		public int RemoveForAccount(string acctNum) {
			lock (this.ordersLock) {
				return this.RemoveAll(this.FindAllForAccount(acctNum));
			}
		}
		public Order FindBySernoExchange(long SernoExchange, bool iDontNeedSuggestionsHere = false) {
			Order found = null;
			lock (this.ordersLock) {
				foreach (Order order in this.InnerOrderList) {
					if (order.SernoExchange != SernoExchange) continue;
					found = order;
					break;
				}
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		}
		public Order FindByGUID(string orderGUID, bool iDontNeedSuggestionsHere = false) {
			Order found = null;
			lock (this.ordersLock) {
				foreach (Order order in this.InnerOrderList) {
					if (order.GUID != orderGUID) continue;
					found = order;
					break;
				}
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		}
		public Order FindMatchingForAlert(Alert alert, bool iDontNeedSuggestionsHere = false) {
			Order found = null;
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(alert.OrderFollowed);
			// replace with a property if (alert.IsSameBarExit) return null;
			lock (this.ordersLock) {
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
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		}
		public Order FindSimilarNotSamePendingOrder(Order order, bool iDontNeedSuggestionsHere = false) {
			Order found = null;
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(order);
			//if (OrdersPendingBrokerCallbackStore == false) return null;
			lock (this.ordersLock) {
				foreach (Order orderSimilar in this.InnerOrderList) {
					if (orderSimilar.Alert == null) continue;	// orders deserialized might have no alerts attached
					if (order == orderSimilar) continue;
					if (order.Alert.IsIdenticalForOrdersPending(orderSimilar.Alert) == false) continue;
					found = orderSimilar;
					break;
				}
			}
			if (iDontNeedSuggestionsHere == false) suggestLanePopupException(found);
			return found;
		}
		public List<Order> FindAllForAccount(string acctNum, bool ignoreExpectingCallbackFromBroker = false, bool iDontNeedSuggestionsHere = false) {
			List<Order> ordersForAccount = new List<Order>();
			if (string.IsNullOrEmpty(acctNum)) {
				string msg = "GetOrdersForAccount([" + acctNum + "]) - returning empty list";
				Assembler.PopupException(msg);
				return ordersForAccount;
			}
			lock (this.ordersLock) {
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

		public override string ToString() {
			return "OrderLane[" + this.ident + "]";
		}
		public virtual string ToShortString() {
			return this.ident;
		}
		public string ToStringSummary() {
			return this.ToString() + ".Count=[" + this.InnerOrderList.Count + "]";
		}
	}
}
