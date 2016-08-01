using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class OrderList : ConcurrentList<Order>, IDisposable {
		protected Dictionary<int, List<Order>>	ByBarPlaced		{ get; private set; }
		
		public Dictionary<int, OrderList>	ByBarPlacedSafeCopy(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			Dictionary<int, OrderList> ret = new Dictionary<int, OrderList>();
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				foreach (int bar in this.ByBarPlaced.Keys) ret.Add(bar, new OrderList("ByBarPlacedSafeCopy", null, this.ByBarPlaced[bar]));
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public List<Order> SafeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			return base.SafeCopy(lockOwner, lockPurpose, waitMillis);
		}
		public OrderList(string reasonToExist, ExecutorDataSnapshot snap = null, List<Order> copyFrom = null) : this(reasonToExist, snap) {
			if (copyFrom == null) return;
			base.InnerList.AddRange(copyFrom);
		}
		public OrderList(string reasonToExist, ExecutorDataSnapshot snap = null) : base(reasonToExist, snap) {
			ByBarPlaced	= new Dictionary<int, List<Order>>();
		}
		public void Clear(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".Clear()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				base			.Clear(lockOwner, lockPurpose, waitMillis);
				this.ByBarPlaced.Clear();
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public void DisposeWaitHandles_andClearInnerList(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			string err = "YOU_SHOULD_NOT_DISPOSE_ORDERS_WHEN_CLEANING_OrderList " + this.ToString();
			Assembler.PopupException(err);
			return;

		    lockPurpose += " //" + base.ReasonToExist + ".DisposeWaitHandles_andClearInnerList()";
		    try {
		        base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
		        foreach (Order order in base.InnerList) order.Dispose();
		        this.Clear(lockOwner, lockPurpose, waitMillis);
				base.Dispose();		//ConcurrentWatchdog's waitHandles
		    } finally {
		        base.UnLockFor(lockOwner, lockPurpose);
		    }
		}
		public void AddRange(List<Order> orders, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddRange(" + orders.Count + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (Order order in orders) this.AddNoDupe(order, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public ByBarDumpStatus AddNoDupe(Order order, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddNoDupe(" + order.ToString() + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				bool newBarAddedInHistory = false;
				bool added = base.AppendUnique(order, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added == false) return ByBarDumpStatus.BarAlreadyContained_alertYouAdd;

				//int barIndexOrderStillPending = order.Bars.Count - 1;
				int barIndexPlaced = order.Alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced) == false) {
					this.ByBarPlaced.Add(barIndexPlaced, new List<Order>());
					newBarAddedInHistory = true;
				}
				List<Order> slot = this.ByBarPlaced[barIndexPlaced];
				if (slot.Contains(order)) return ByBarDumpStatus.BarAlreadyContained_alertYouAdd;
				if (slot.Count > 0) {
					string msg = "appending second StopLossDot to the same bar [" + order + "]";
				}
				slot.Add(order);
				return (newBarAddedInHistory) ? ByBarDumpStatus.OneNewAlertAdded_forNewBar
					: ByBarDumpStatus.SequentialAlertAdded_forExistingBar;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public bool Remove(Order order, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".Remove(" + order.ToString() + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				bool removed = base.RemoveUnique(order, owner, lockPurpose, waitMillis, absenseThrowsAnError);
				int barIndexPlaced = order.Alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced)) {
					List<Order> slot = this.ByBarPlaced[barIndexPlaced];
					if (slot.Contains(order)) slot.Remove(order);
					if (slot.Count == 0) this.ByBarPlaced.Remove(barIndexPlaced);
				}
				return removed;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}


		//public bool ContainsIdentical(Order maybeAlready, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool onlyUnfilled = true) {
		//    lockPurpose += " //" + base.ReasonToExist + ".ContainsIdentical(" + maybeAlready + ", " + onlyUnfilled + ")";
		//    try {
		//        base.WaitAndLockFor(owner, lockPurpose, waitMillis);
		//        foreach (Order each in base.InnerList) {
		//            if (maybeAlready.IsIdentical_orderlessPriceless(each) == false) continue;
		//            if (onlyUnfilled && each.IsFilled_fromPosition) continue;
		//            return true;
		//        }
		//        return false;
		//    } finally {
		//        base.UnLockFor(owner, lockPurpose);
		//    }
		//}
		//public Order FindIdentical_notSame_forOrdersPending(Order order, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
		//    lockPurpose += " //" + base.ReasonToExist + ".FindIdentical_notSame_forOrdersPending(" + order + ")";
		//    try {
		//        base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
		//        Order similar = null;
		//        foreach (Order orderSimilar in base.InnerList) {
		//            if (orderSimilar == order) continue;
		//            if (orderSimilar.IsIdentical_forOrdersPending(order)) {
		//                if (similar != null) {
		//                    string msg = "there are 2 or more " + this.ReasonToExist + " Orders similar to " + order;
		//                    throw new Exception(msg);
		//                }
		//                similar = orderSimilar;
		//            }
		//        }
		//        return similar;
		//    } finally {
		//        base.UnLockFor(lockOwner, lockPurpose);
		//    }
		//}
		public new OrderList Clone(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + "Clone()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				OrderList ret		= new OrderList("CLONE_" + base.ReasonToExist, base.Snap, base.InnerList);
				//v1 ret.ByBarPlaced		= this.ByBarPlacedSafeCopy(this, "Clone(WAIT)");
				foreach (int bar in this.ByBarPlaced.Keys) {
					ret.ByBarPlaced.Add(bar, new List<Order>(this.ByBarPlaced[bar]));
				}
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public override string ToString() {
			return base.ToString() + " this.ByBarPlaced.Bars[" + this.ByBarPlaced.Keys.Count + "]";
		}

		public void Dispose() {
			string err = "YOU_SHOULD_NOT_DISPOSE_ORDERS_WHEN_CLEANING_OrderList " + this.ToString();
			Assembler.PopupException(err);
			return;

			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.DisposeWaitHandles_andClearInnerList(this, "EXTERNAL_DISPOSE()_CALL");
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }

		internal OrderList Substract_returnClone(OrderList ordersPending_alreadyScheduledForDelayedFill, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + "Substract_returnClone()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				ordersPending_alreadyScheduledForDelayedFill.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				OrderList ret		= new OrderList(base.ReasonToExist + "_MINUS_" + ordersPending_alreadyScheduledForDelayedFill.ReasonToExist, base.Snap);
				foreach(Order eachMine in base.InnerList) {
					if (ordersPending_alreadyScheduledForDelayedFill.Contains(eachMine, lockOwner, lockPurpose)) continue;
					ret.AddNoDupe(eachMine, lockOwner, lockPurpose);
				}
				return ret;
			} finally {
				ordersPending_alreadyScheduledForDelayedFill.UnLockFor(lockOwner, lockPurpose);
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
	}
}
