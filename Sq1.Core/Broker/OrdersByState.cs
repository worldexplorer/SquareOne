using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public partial class OrdersByState : ConcurrentDictionarySorted_ofConcurrentLists<OrderStateDisplayed, Order> {
		public new class  ASC : IComparer<OrderStateDisplayed> {
			int IComparer<OrderStateDisplayed>.Compare(OrderStateDisplayed x, OrderStateDisplayed y) {
				return x.OrderState > y.OrderState ? 1 : -1;
			}
		}
		public new class DESC : IComparer<OrderStateDisplayed> {
			int IComparer<OrderStateDisplayed>.Compare(OrderStateDisplayed x, OrderStateDisplayed y) {
				return x.OrderState > y.OrderState ? -1 : 1;
			}
		}


		public OrdersByState(string reasonToExist, IComparer<OrderStateDisplayed> orderby) : base(reasonToExist, orderby) {
		}

		public int Clear_unsubscribe(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			int unsubscribed = 0;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				List<OrderStateDisplayed> states = base.Keys(lockOwner, lockPurpose, waitMillis);
				foreach (OrderStateDisplayed eachState in states) {
					//ConcurrentList<Order> ordersWithState = base.InnerDictionary[eachState];
					ConcurrentList<Order> ordersWithState = this.GetAtKey_nullUnsafe(eachState, lockOwner, lockPurpose, waitMillis);
					if (ordersWithState == null) continue;
					foreach (Order eachOrder_noDuplicates in ordersWithState.SafeCopy(lockOwner, lockPurpose, waitMillis)) {
						eachOrder_noDuplicates.OnOrderStateChanged -= this.order_OnOrderStateChanged;
					}
				}
				base.Clear(lockOwner, lockPurpose, waitMillis);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return unsubscribed;
		}

		public bool Add_intoListForState(Order order) {
			string msig = " //Add_intoListForState(" + order + ")";
			bool added = false;

			int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT;
			bool duplicateThrowsAnError = true;

			try {
				base.WaitAndLockFor(this, msig, waitMillis);
				OrderState orderState = order.State;
				//v1
				//bool contains = this.containsState(orderState);
				//if (contains == false) {
				//    OrderStateDisplayed newStateDisplayed = new OrderStateDisplayed(orderState);
				//    added = base.Insert_intoListForKey(newStateDisplayed, order, this, msig, waitMillis, duplicateThrowsAnError);
				//    //TESTED bool containsNow = this.containsState(orderState);
				//} else {
				//    ConcurrentList<Order> alreadyExisting_forState = this.ordersForState_nullUnsafe(orderState);
				//    alreadyExisting_forState.InsertUnique(order, this, msig, waitMillis, duplicateThrowsAnError);
				//}
				//v2
				OrderStateDisplayed osd = this.findOrderStateDisplayed_amongUniqueKeys_forOrderState_nullWhenNotFound(orderState, this, msig, waitMillis);
				if (osd == null) {
					osd = new OrderStateDisplayed(orderState);
				}
				added = base.Insert_intoListForKey(osd, order, this, msig, waitMillis, duplicateThrowsAnError);
				order.OnOrderStateChanged += new EventHandler<OrderStateChangedEventArgs>(this.order_OnOrderStateChanged);
			} finally {
				base.UnLockFor(this, msig);
			}
			return added;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		void order_OnOrderStateChanged(object sender, OrderStateChangedEventArgs e) {
			OrderState oldState = e.OrderState_beforeChanged;
			OrderState newState = e.Order.State;

			string	msig				= " //OrdersByState.order_OnOrderStateChanged()";
			string	lockPurpose			= "NEED_EXCLUSIVE_ACCESS_TO_MOVE_ORDER_UNDER_NEW_KEY oldState[" + oldState + "]=>newState[" + newState + "]";
			int		waitMillis			= ConcurrentWatchdog.TIMEOUT_DEFAULT;
			bool	reportViolation		= true;
			bool	engageWaitingForEva	= true;

			try {
				base.WaitAndLockFor(msig, lockPurpose, waitMillis);

				//v1
				//bool will_removeFromOld = true;
				//if (this.containsState(oldState) == false) {
				//    string msg = "OLD_STATE_MUST_HAVE_BEEN_ADDED_ALREADY";
				//    Assembler.PopupException(msg + msig);
				//    //return;
				//    will_removeFromOld = false;
				//}
				//if (this.containsState(newState) == false) {
				//    OrderStateDisplayed newStateDisplayed = new OrderStateDisplayed(newState);
				//    base.Add_newKeyWithEmptyList(newStateDisplayed, this, lockPurpose, waitMillis);
				//}
				//ConcurrentList<Order> addTo_newList			= this.ordersForState_nullUnsafe(newState);
				//ConcurrentList<Order> removeFrom_oldList	= this.ordersForState_nullUnsafe(oldState);
				//bool added = addTo_newList.InsertUnique(e.Order, this, lockPurpose, waitMillis, true);
				//if (will_removeFromOld) {
				//    bool removed = removeFrom_oldList.RemoveUnique(e.Order, this, lockPurpose, waitMillis, true);
				//}

				//v2 - keeps track of innerList_sequentiallyInserted_orderKeptForFiltering
				OrderStateDisplayed addTo_Key		= this.findOrderStateDisplayed_amongUniqueKeys_forOrderState_nullWhenNotFound(newState, this, msig, waitMillis);
				OrderStateDisplayed removeFrom_Key	= this.findOrderStateDisplayed_amongUniqueKeys_forOrderState_nullWhenNotFound(oldState, this, msig, waitMillis);
				if (addTo_Key == null) {
					string msg = "WILL_BE_ADDED_IN_RemoveFromKey_addToKey() newState[" + newState + "] => addTo_Key=NULL";
					//Assembler.PopupException(msg + msig);
					//return;
					addTo_Key = new OrderStateDisplayed(newState);
				}
				if (removeFrom_Key == null) {
					string msg = "PARANOID__ORDERLIST_FOR__OLD_STATE_NOT_FOUND oldState[" + oldState + "] => removeFrom_Key=NULL";
					Assembler.PopupException(msg + msig);
					return;
				}

				int operationsDone = base.RemoveFromKey_addToKey(e.Order, removeFrom_Key, addTo_Key, this, lockPurpose, waitMillis);

				this.raiseOnOrderStateChanged(e.Order, e.OrderState_beforeChanged);
			} catch (Exception ex) {
				string msg = "MOVING_BETWEEN_STATES__OR_DELEGATED_EVENT_SUBSCRIBER__THREW";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				base.UnLockFor(msig, lockPurpose, reportViolation, waitMillis, engageWaitingForEva);
			}

		}

		bool containsState(OrderState state) {
			string msig = " //containsState(OrderState[" + state + "])";
			bool ret = false;
			List<OrderStateDisplayed> states = base.Keys(this, msig);
			foreach (OrderStateDisplayed stateDisplayed in states) {
				if (stateDisplayed.OrderState != state) continue;
				ret = true;
				break;
			}
			return ret;
		}

		// use findOrderStateDisplayed_amongKeys_for()
		//ConcurrentList<Order> ordersForState_nullUnsafe(OrderState state) {
		//    string msig = " //ordersForState_nullUnsafe(OrderState[" + state + "])";
		//    ConcurrentList<Order> ret = null;
		//    try {
		//        base.WaitAndLockFor(this, msig);
		//        //v5 I_HATE_SortedDictionary
		//        foreach (KeyValuePair<OrderStateDisplayed, ConcurrentList<Order>> keyValue in base.InnerDictionary) {
		//            if (keyValue.Key.OrderState != state) continue;
		//            ret = keyValue.Value;
		//            break;
		//        }
		//    } finally {
		//        base.UnLockFor(this, msig);
		//    }
		//    return ret;
		//}

		OrderStateDisplayed findOrderStateDisplayed_amongUniqueKeys_forOrderState_nullWhenNotFound(OrderState state
					, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			OrderStateDisplayed ret = null;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				//v5 I_HATE_SortedDictionary
				foreach (KeyValuePair<OrderStateDisplayed, ConcurrentList<Order>> keyValue in base.InnerDictionary) {
					if (keyValue.Key.OrderState != state) continue;
					ret = keyValue.Key;
					break;
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}


		//public override ConcurrentList<ORDER> GetAtKey_nullUnsafe(ORDER_STATE orderState
		//                , object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
		//    ConcurrentList<ORDER> ret = null;
		//    try {
		//        base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
		//        ORDER_STATE keyFound_default = default(ORDER_STATE);
		//        ORDER_STATE keyFound = keyFound_default;
		//        foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
		//            if (keyValue.Key.ToString() != orderState.ToString()) continue;
		//            keyFound = keyValue.Key;
		//            ret = keyValue.Value;
		//            break;
		//        }
		//        if (keyFound.Equals(keyFound_default) && absenceThrowsAnError) {
		//            string msg = this.ReasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + orderState.ToString();
		//            Assembler.PopupException(msg, null, false);
		//        }
		//    } finally {
		//        base.UnLockFor(lockOwner, lockPurpose);
		//    }
		//    return ret;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		//}

		internal int RemoveRange(List<Order> ordersToRemove) {
			string	msig = " //OrdersByState.RemoveRange(" + ordersToRemove.Count + ")";
			int removed_counter = 0;

			Dictionary<OrderState, List<Order>> ordersByState_local = new Dictionary<OrderState, List<Order>>();
			foreach (Order eachOrder in ordersToRemove) {
				if (eachOrder == null) continue;
				if (ordersByState_local.ContainsKey(eachOrder.State) == false) {
					ordersByState_local.Add(eachOrder.State, new List<Order>());
				}
				List<Order> orders_forState = ordersByState_local[eachOrder.State];
				orders_forState.Add(eachOrder);
			}

			foreach (OrderState eachState in ordersByState_local.Keys) {
				List<Order> orders_forState = ordersByState_local[eachState];
				string	lockPurpose				= "NEED_EXCLUSIVE_ACCESS_TO_REMOVE_ORDERS_FOR_KEY state[" + eachState + "].Count[" + orders_forState.Count + "]";
				removed_counter += this.removeRange_forState(eachState, orders_forState, ConcurrentWatchdog.TIMEOUT_DEFAULT, false);
			}
			return removed_counter;
		}

		int removeRange_forState(OrderState orderState, List<Order> ordersToRemove_withSameState,
					int		waitMillis				= ConcurrentWatchdog.TIMEOUT_DEFAULT,
					bool	absenseThrowsAnError	= true
			) {
			string	msig = " //OrdersByState.removeRange_forState(" + ordersToRemove_withSameState.Count + ")";
			int removed_counter = 0;
			try {
				base.WaitAndLockFor(this, msig, waitMillis);
				if (this.containsState(orderState) == false) {
					string msg = "DIDNT_EXIST_IN_OrdersByState: [" + orderState + "]";
					Assembler.PopupException(msg);
					return removed_counter;
				}
				//v1
				//ConcurrentList<Order> orders_forState = this.ordersForState_nullUnsafe(orderState);
				//v2
				OrderStateDisplayed osd = this.findOrderStateDisplayed_amongUniqueKeys_forOrderState_nullWhenNotFound(orderState, this, msig, waitMillis);
				ConcurrentList<Order> orders_forState = base.GetAtKey_nullUnsafe(osd, this, msig, waitMillis);

				foreach (Order order_toRemove in ordersToRemove_withSameState) {
					if (orders_forState.Contains(order_toRemove, this, msig, waitMillis) == false) {
						if (absenseThrowsAnError) {
							string msg = "ORDER_NOT_FOUND_IN_STATELIST__WAS_REMOVED_EARLIER_OR_WASNT_ADDED " + order_toRemove.ToString();
							Assembler.PopupException(msg + msig);
						}
					} else {
						bool removed = orders_forState.RemoveUnique(order_toRemove, this, msig, waitMillis, absenseThrowsAnError);
						if (removed) {
							this.CountValues_sumInEachList--;
							removed_counter++;
						}
					}
				}
			} finally {
				base.UnLockFor(this, msig);
			}
			return removed_counter;
		}

		internal int AddRange(List<Order> ordersToAdd) {
			string	msig = " //OrdersByState.AddRange(" + ordersToAdd.Count + ")";
			int added_forAllStates = 0;

			Dictionary<OrderState, List<Order>> ordersByState_local = new Dictionary<OrderState, List<Order>>();
			foreach (Order eachOrder in ordersToAdd) {
				if (eachOrder == null) continue;
				if (ordersByState_local.ContainsKey(eachOrder.State) == false) {
					ordersByState_local.Add(eachOrder.State, new List<Order>());
				}
				List<Order> orders_forState = ordersByState_local[eachOrder.State];
				orders_forState.Add(eachOrder);
			}

			foreach (OrderState eachState in ordersByState_local.Keys) {
				List<Order> orders_forState = ordersByState_local[eachState];
				string	lockPurpose				= "NEED_EXCLUSIVE_ACCESS_TO_ADD_ORDERS_FOR_KEY state[" + eachState + "].Count[" + orders_forState.Count + "]";
				foreach (Order eachOrder_withSameState in orders_forState) {
					bool added = this.Add_intoListForState(eachOrder_withSameState);
					if (added) added_forAllStates++;
				}
			}
			return added_forAllStates;
		}

		internal void InitializeDeserialized(OrderLane orderLane) {
			this.orderLaneOrdered_toApplyFiltering = orderLane;
			this.AddRange(orderLane.SafeCopy);
		}

		OrderLane orderLaneOrdered_toApplyFiltering;
		public List<Order> Orders_withStatesDisplayed(
					object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			List<Order> ret = new List<Order>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				List<OrderState> statesDisplayed = this.statesDisplayed(lockOwner, lockPurpose, waitMillis);
				List<Order> orders_withAllStates = this.orderLaneOrdered_toApplyFiltering.SafeCopy;	// isolating add/remove into original
				foreach (Order everyOrder in orders_withAllStates) {
					if (statesDisplayed.Contains(everyOrder.State) == false) continue;
					ret.Add(everyOrder);
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}
		List<OrderState> statesDisplayed(
					object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			List<OrderState> ret = new List<OrderState>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				List<OrderStateDisplayed> states = base.Keys(lockOwner, lockPurpose, waitMillis);
				foreach (OrderStateDisplayed eachState in states) {
					if (eachState.Displayed == false) continue;
					ret.Add(eachState.OrderState);
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}

	}
}
