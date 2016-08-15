using System.Collections.Generic;

namespace Sq1.Core.Support {
	public class ConcurrentDictionary_ofConcurrentLists<ORDER_STATE, ORDER> : ConcurrentDictionary<ORDER_STATE, ConcurrentList<ORDER>> {
		public int CountValues_sumInEachList;

		public ConcurrentDictionary_ofConcurrentLists(string reasonToExist) : base(reasonToExist) {
			CountValues_sumInEachList = 0;
		}

		public bool Add_newKeyWithEmptyList(ORDER_STATE orderState,
				object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			bool added = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				string reasonToAdd = typeof(ORDER_STATE) + " DIDNT_EXIST_IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>";
				base.InnerDictionary.Add(orderState, new ConcurrentList<ORDER>(reasonToAdd));
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return added;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public virtual bool Add_intoListForKey(ORDER_STATE orderState, ORDER order
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			bool added = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				//if (this.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) && duplicateThrowsAnError) {
				//    string msg = this.ReasonToExist + ": CDG_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + orderState.ToString();
				//    Assembler.PopupException(msg, null, false);
				//} else {
				if (base.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) == false) {
					//string reasonToAdd = typeof(ORDER_STATE) + " DIDNT_EXIST_IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>";
					//base.InnerDictionary.Add(orderState, new ConcurrentList<ORDER>(reasonToAdd));
					this.Add_newKeyWithEmptyList(orderState, lockOwner, lockPurpose, waitMillis);
				}
				ConcurrentList<ORDER> orders_forState = base.InnerDictionary[orderState];
				added = orders_forState.InsertUnique(order, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added) this.CountValues_sumInEachList++;
				base.Count = this.InnerDictionary.Count;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return added;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public override bool Remove(ORDER_STATE orderState
						 , object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			bool removed = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				if (this.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + orderState.ToString();
						Assembler.PopupException(msg);
					}
				} else {
					removed = this.InnerDictionary.Remove(orderState);
					if (removed) this.CountValues_sumInEachList--;
					this.Count = this.InnerDictionary.Count;
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return removed;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		protected virtual int RemoveRange_forKey(ORDER_STATE orderState, List<ORDER> ordersToRemove_withSameState
						 , object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			int removed_counter = 0;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				if (base.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) == false) {
					string msg = typeof(ORDER_STATE) + " DIDNT_EXIST_IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>";
					Assembler.PopupException(msg);
					return removed_counter;
				}
				ConcurrentList<ORDER> orders_forState = base.InnerDictionary[orderState];

				foreach (ORDER order_toRemove in ordersToRemove_withSameState) {
					if (orders_forState.Contains(order_toRemove, lockOwner, lockPurpose, waitMillis) == false) {
						if (absenseThrowsAnError) {
							string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + order_toRemove.ToString();
							Assembler.PopupException(msg);
						}
					} else {
						bool removed = orders_forState.RemoveUnique(order_toRemove, this, lockPurpose, waitMillis, absenseThrowsAnError);
						if (removed) {
							this.CountValues_sumInEachList--;
							removed_counter++;
						}
					}
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return removed_counter;
		}
	}
}
