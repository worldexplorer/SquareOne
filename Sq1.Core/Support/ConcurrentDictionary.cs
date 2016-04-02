using System.Collections.Generic;

namespace Sq1.Core.Support {
	public class ConcurrentDictionary<PRICE_LEVEL, TOTAL_LOTS> : ConcurrentWatchdog {
		protected	Dictionary<PRICE_LEVEL, TOTAL_LOTS> InnerDictionary		{ get; private set; }
		public		int									Count				{ get; protected set; }

		public int CountBlocking(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			int count = -1;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				count = this.InnerDictionary.Count;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return count;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public Dictionary<PRICE_LEVEL, TOTAL_LOTS> SafeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			Dictionary<PRICE_LEVEL, TOTAL_LOTS> ret = new Dictionary<PRICE_LEVEL, TOTAL_LOTS>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				ret = new Dictionary<PRICE_LEVEL, TOTAL_LOTS>(this.InnerDictionary);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public ConcurrentDictionary(string reasonToExist) : base(reasonToExist) {
			InnerDictionary	= new Dictionary<PRICE_LEVEL, TOTAL_LOTS>();
		}
		public bool ContainsKey(PRICE_LEVEL priceLevel, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			bool contains = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				contains = this.InnerDictionary.ContainsKey(priceLevel);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return contains;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual void Clear(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				this.InnerDictionary.Clear();
				this.Count = this.InnerDictionary.Count;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public virtual bool Remove(PRICE_LEVEL priceLevel, object lockOwner, string lockPurpose
							, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			bool removed = false;
			try {
				if (this.ContainsKey(priceLevel, lockOwner, lockPurpose, waitMillis) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + priceLevel.ToString();
						Assembler.PopupException(msg);
					}
				} else {
					removed = this.InnerDictionary.Remove(priceLevel);
					this.Count = this.InnerDictionary.Count;
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return removed;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual bool Add(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			bool added = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				if (this.ContainsKey(priceLevel, lockOwner, lockPurpose, ConcurrentWatchdog.TIMEOUT_DEFAULT) && duplicateThrowsAnError) {
					string msg = this.ReasonToExist + ": CDG_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
				} else {
					this.InnerDictionary.Add(priceLevel, totalLots);
					added = true;
					this.Count = this.InnerDictionary.Count;
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return added;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual bool UpdateAtKey(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			bool updated = false;
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				if (this.ContainsKey(priceLevel, lockOwner, lockPurpose, ConcurrentWatchdog.TIMEOUT_DEFAULT) == false && absenceThrowsAnError) {
					string msg = this.ReasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
				} else {
					this.InnerDictionary[priceLevel] = totalLots;
					updated = true;
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return updated;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual TOTAL_LOTS GetAtKey(PRICE_LEVEL priceLevel
						, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			TOTAL_LOTS ret = default(TOTAL_LOTS);
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				if (this.ContainsKey(priceLevel, lockOwner, lockPurpose, ConcurrentWatchdog.TIMEOUT_DEFAULT) == false && absenceThrowsAnError) {
					string msg = this.ReasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
				} else {
					ret = this.InnerDictionary[priceLevel];
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual List<PRICE_LEVEL> Keys(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				List<PRICE_LEVEL> ret = new List<PRICE_LEVEL>(this.InnerDictionary.Keys);
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public virtual List<TOTAL_LOTS> Values(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				List<TOTAL_LOTS> ret = new List<TOTAL_LOTS>(this.InnerDictionary.Values);
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public override string ToString() {
			return this.ReasonToExist + ":InnerDictionary[" + this.Count + "]";
		}
	}
}
