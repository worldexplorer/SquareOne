using System;
using System.Collections.Generic;

namespace Sq1.Core.Support {
	public class ConcurrentDictionarySorted_ofConcurrentLists<ORDER_STATE, ORDER> : ConcurrentDictionarySorted<ORDER_STATE, ConcurrentList<ORDER>> {
		public int CountValues_sumInEachList;
		List<ORDER> innerList_sequentiallyInserted_orderKeptForFiltering;

		public ConcurrentDictionarySorted_ofConcurrentLists(string reasonToExist, IComparer<ORDER_STATE> orderby) : base(reasonToExist, orderby) {
			innerList_sequentiallyInserted_orderKeptForFiltering = new List<ORDER>();
			CountValues_sumInEachList = 0;
		}


		public override List<ORDER_STATE> Keys(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				//v1 List<ORDER_STATE> ret = new List<ORDER_STATE>(base.InnerDictionary.Keys);
				List<ORDER_STATE> ret = new List<ORDER_STATE>();
				//v5 I_HATE_SortedDictionary
				foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
					ret.Add(keyValue.Key);
				}
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public List<ORDER> ValuesMerged_withInsertedOrder(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				//v1
				//List<ORDER> ret = new List<ORDER>(base.InnerDictionary.Values);
				List<ORDER> ret = new List<ORDER>(this.innerList_sequentiallyInserted_orderKeptForFiltering);

				//v5 I_HATE_SortedDictionary ValuesFlattened_mergedSquentially_brokenOrder
				//List<ORDER> ret = new List<ORDER>();
				//foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
				//    List<ORDER> eachConcurrent = keyValue.Value.SafeCopy(lockOwner, lockPurpose, waitMillis);
				//    ret.AddRange(eachConcurrent);
				//}
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public override bool ContainsKey(ORDER_STATE orderState,
					object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			bool contains = false;
			if (orderState == null) {
				bool nullKeyThrowsAnError = false;
				if (nullKeyThrowsAnError) {
					string msg = "AVOIDING_NPE__NEVER_CHECK_FOR_NULL_AS_KEY";
					Assembler.PopupException(msg);
				}
				return contains;
			}

			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				//v5 I_HATE_SortedDictionary
				foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
					if (keyValue.Key == null) {
						string msg = "AVOIDING_NPE__NEVER_ADD_NULL_AS_KEY";
						Assembler.PopupException(msg);
					}
					if (keyValue.Key.ToString() != orderState.ToString()) continue;
					contains = true;
					break;
				}
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return contains;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public override ConcurrentList<ORDER> GetAtKey_nullUnsafe(ORDER_STATE orderState
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			ConcurrentList<ORDER> ret = null;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				ORDER_STATE keyFound_default = default(ORDER_STATE);
				ORDER_STATE keyFound = keyFound_default;
				foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
					if (keyValue.Key.ToString() != orderState.ToString()) continue;
					keyFound = keyValue.Key;
					ret = keyValue.Value;
					break;
				}
				if (keyFound.Equals(keyFound_default) && absenceThrowsAnError) {
					string msg = this.ReasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + orderState.ToString();
					Assembler.PopupException(msg, null, false);
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public bool Add_newKeyWithEmptyList(ORDER_STATE orderState,
				object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			bool added = false;
			if (orderState == null) {
				string msg = "AVOIDING_NPE__NEVER_ADD_LIST_FOR__NULL_AS_KEY";
				Assembler.PopupException(msg);
				return added;
			}

			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				string reasonToAdd = "CdsCl_KEY: " + typeof(ORDER_STATE).Name + "[" + orderState + "]"
					//+ " DIDNT_EXIST_IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>"
					;
				base.InnerDictionary.Add(orderState, new ConcurrentList<ORDER>(reasonToAdd));
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return added;	//I'm suspiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public virtual bool Insert_intoListForKey(ORDER_STATE orderState, ORDER order
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			string msig = " //ConcurrentDictionarySorted_ofConcurrentLists.Add_intoListForKey()";
			bool added = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				//if (this.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) && duplicateThrowsAnError) {
				//    string msg = this.ReasonToExist + ": CDG_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + orderState.ToString();
				//    Assembler.PopupException(msg, null, false);
				//} else {
				if (this.ContainsKey(orderState, lockOwner, lockPurpose, waitMillis) == false) {
					//string reasonToAdd = typeof(ORDER_STATE) + " DIDNT_EXIST_IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>";
					//base.InnerDictionary.Add(orderState, new ConcurrentList<ORDER>(reasonToAdd));
					bool added_emptyList = this.Add_newKeyWithEmptyList(orderState, lockOwner, lockPurpose, waitMillis);
					if (added_emptyList == false) return added;
				}

				//v1 ConcurrentList<ORDER> orders_forState = base.InnerDictionary[orderState];
				//v2
				ConcurrentList<ORDER> orders_forState = this.GetAtKey_nullUnsafe(orderState, lockOwner, lockPurpose, waitMillis);
				//v3
				//ConcurrentList<ORDER> orders_forState = null;
				//base.InnerDictionary.TryGetValue(orderState, out orders_forState);
				//v4
				//SortedDictionary<ORDER_STATE, ConcurrentList<ORDER>> innerSortedDictionary = base.InnerDictionary as SortedDictionary<ORDER_STATE, ConcurrentList<ORDER>>;
				//ConcurrentList<ORDER> orders_forState = innerSortedDictionary[orderState];
				//v5 I_HATE_SortedDictionary
				//foreach (KeyValuePair<ORDER_STATE, ConcurrentList<ORDER>> keyValue in base.InnerDictionary) {
				//    if (keyValue.Key.ToString() != orderState.ToString()) continue;
				//    orders_forState = keyValue.Value;
				//    break;
				//}

				if (orders_forState == null) {
					string msg = "PARANOID__LIST_FOR_KEY_IS_NULL orderState[" + orderState + "]";
					Assembler.PopupException(msg);
					return added;
				}
				added = orders_forState.InsertUnique(order, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added) {
					this.CountValues_sumInEachList++;
					this.innerList_sequentiallyInserted_orderKeptForFiltering.Insert(0, order);
				}
				base.Count = this.InnerDictionary.Count;
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
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
					string msg = "CAN_NOT_REMOVE_NON_EXISTING_CdsCl_KEY: " + typeof(ORDER_STATE).Name + "[" + orderState + "]"
						//+ " _IN ConcurrentDictionary_ofConcurrentLists<" + typeof(ORDER_STATE) + ",ConcurrentList<" + typeof(ORDER) + ">>"
						;
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
							this.innerList_sequentiallyInserted_orderKeptForFiltering.Remove(order_toRemove);
						}
					}
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return removed_counter;
		}

		public int RemoveFromKey_addToKey(ORDER order, ORDER_STATE removeFrom_Key, ORDER_STATE addTo_Key
						 , object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = false) {

			int operationsDone = 0;
			string msig = " //RemoveFromKey_addToKey(removeFrom_Key[" + removeFrom_Key + "] => addTo_Key[" + addTo_Key + "] for [" + order + "])";

			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);

				bool doesntExist = addTo_Key == null || this.ContainsKey(addTo_Key, lockOwner, lockPurpose, waitMillis) == false;
				if (doesntExist) {
					this.Add_newKeyWithEmptyList(addTo_Key, lockOwner, lockPurpose, waitMillis);
					operationsDone++;
				}

				ConcurrentList<ORDER> addTo_newList			= this.GetAtKey_nullUnsafe(addTo_Key, lockOwner, lockPurpose, waitMillis);
				bool duplicateThrowsAnError = absenseThrowsAnError;
				bool added = addTo_newList.InsertUnique(order, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added) operationsDone++;

				bool oldState_keyExists = this.ContainsKey(removeFrom_Key, lockOwner, lockPurpose, waitMillis);
				if (oldState_keyExists == false) {
					string msg = "OLD_STATE_KEY_MUST_HAVE_BEEN_THERE__OTHERWIZE_WHERE_ORDER_LIVED";
					Assembler.PopupException(msg + lockPurpose);
					return operationsDone;
				}

				ConcurrentList<ORDER> removeFrom_oldList_mustNotBeNull_afterKeyCheckDone = this.GetAtKey_nullUnsafe(removeFrom_Key, lockOwner, lockPurpose, waitMillis);
				if (removeFrom_oldList_mustNotBeNull_afterKeyCheckDone == null) {
					string msg = "OLD_STATE_LIST_MUST_NOT_BE_NULL__OTHERWIZE_WHERE_ORDER_LIVED";
					Assembler.PopupException(msg + lockPurpose);
					return operationsDone;
				}
				bool removed = removeFrom_oldList_mustNotBeNull_afterKeyCheckDone.RemoveUnique(order, lockOwner, lockPurpose, waitMillis, absenseThrowsAnError);
				if (removed) operationsDone++;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}

			return operationsDone;
		}

		public override string ToString() {
			return this.ReasonToExist + ":InnerDictionarySorted[" + this.Count + "]";
		}
	}
}
