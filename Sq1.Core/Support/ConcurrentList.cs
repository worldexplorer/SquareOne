using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentList<T> : ConcurrentWatchdog {
		protected	List<T>		InnerList	{ get; private set; }
		public		int			Count		{ get; protected set; }

		public T First_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".First_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList.Count > 0) ret = this.InnerList[0];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public T Last_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".Last_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList.Count > 0) ret = this.InnerList[this.InnerList.Count - 1];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public T PreLast_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".PreLast_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList.Count > 1) ret = this.InnerList[this.InnerList.Count - 2];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public List<T> SafeCopy(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			List<T> ret = new List<T>();
			lockPurpose += " //" + this.ToString() + ".SafeCopy()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				ret = new List<T>(this.InnerList);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		ConcurrentList(string reasonToExist, ExecutorDataSnapshot snap, List<T> copyFrom) : this(reasonToExist, snap) {
			InnerList	= new List<T>();
			InnerList.AddRange(copyFrom);
			Count = InnerList.Count;
		}
		public ConcurrentList(string reasonToExist, ExecutorDataSnapshot snap = null) : base(reasonToExist, snap) {
			Snap		= snap;
			InnerList	= new List<T>();
		}
		public bool Contains(T position, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Contains(" + position.ToString() + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				return this.InnerList.Contains(position);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual void Clear(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clear()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				this.InnerList.Clear();
				this.Count = this.InnerList.Count;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool RemoveUnique(T position, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Remove(" + position.ToString() + ")";
			bool removed = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList.Contains(position) == false) {
					if (absenceThrowsAnError == true) {
						string msg = "WAS_REMOVED_EARLIER__OR_NEVER_ADDED position[" + position + "] LIVESIM_SHOULD_NOT_FILL_ORDER_THAT_WAS_ALREADY_KILLED";
						Assembler.PopupException(msg + this.ToString());
					}
				} else {
					removed = this.InnerList.Remove(position);
					this.Count = this.InnerList.Count;
				}
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return removed;
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool AppendUnique(T alertOrPosition, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Add(" + alertOrPosition.ToString() + ")";
			bool added = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = base.ReasonToExist + ": CLWD_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, true);
					return added;
				}
				this.InnerList.Add(alertOrPosition);
				this.Count = this.InnerList.Count;
				added = true;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return added;
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool InsertUnique(int indexToInsertAt, T alertOrPosition, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Add(" + alertOrPosition.ToString() + ")";
			bool added = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = base.ReasonToExist + ": CLWD_MUST_BE_INSERTED_ONLY_ONCE__ALREADY_INSERTED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, true);
					return added;
				}
				this.InnerList.Insert(indexToInsertAt, alertOrPosition);
				this.Count = this.InnerList.Count;
				added = true;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return added;
		}
		public ConcurrentList<T> Clone(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clone()";
			ConcurrentList<T> ret = null;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				ret = new ConcurrentList<T>("CLONE_" + this.ReasonToExist, this.Snap, this.InnerList);
				return ret;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public override string ToString() {
			return base.ReasonToExist + ":" + this.Count;
		}

		public List<T> SubsetContainingKeyword(string keyword) {
			string msig = "FOUND[" + keyword + "] ";
			List<T> ret = new List<T>();
			List<T> clone = this.SafeCopy(this, msig);
			foreach (T ex in clone) {
				//if (ex.Message.Contains(keyword) == false) continue;
				if (ex.ToString().Contains(keyword) == false) continue;
				ret.Add(ex);
			}
			return ret;
		}
	}
}
