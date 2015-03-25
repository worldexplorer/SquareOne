using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentListWD<T> : ConcurrentWatchdog {
		protected	List<T>		InnerList	{ get; private set; }
		public		int			Count		{ get; protected set; }

		public T FirstNullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".FirstNullUnsafe()";
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
		public T LastNullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".LastNullUnsafe()";
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
		public T PreLastNullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
		    T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".PreLastNullUnsafe()";
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
		ConcurrentListWD(string reasonToExist, ExecutionDataSnapshot snap, List<T> copyFrom) : this(reasonToExist, snap) {
			InnerList	= new List<T>();
			InnerList.AddRange(copyFrom);
			Count = InnerList.Count;
		}
		public ConcurrentListWD(string reasonToExist, ExecutionDataSnapshot snap = null) : base(reasonToExist, snap) {
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
		protected virtual void Clear(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clear()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				this.InnerList.Clear();
				this.Count = this.InnerList.Count;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		protected virtual bool Remove(T position, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Remove(" + position.ToString() + ")";
			bool removed = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList.Contains(position) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + lockPurpose;
						Assembler.PopupException(msg);
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
		protected virtual bool Add(T alertOrPosition, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Add(" + alertOrPosition.ToString() + ")";
			bool added = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = base.ReasonToExist + ": MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, false);
				}
				this.InnerList.Add(alertOrPosition);
				this.Count = this.InnerList.Count;
				added = true;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return added;
		}
		protected ConcurrentListWD<T> Clone(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clone()";
			ConcurrentListWD<T> ret = null;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				ret = new ConcurrentListWD<T>("CLONE_" + this.ReasonToExist, this.Snap, this.InnerList);
				return ret;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public override string ToString() {
			return base.ReasonToExist + ":" + this.Count;
		}
	}
}
