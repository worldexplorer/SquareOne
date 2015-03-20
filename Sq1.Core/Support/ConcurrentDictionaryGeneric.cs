using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentDictionaryGeneric<PRICE_LEVEL, TOTAL_LOTS> : ConcurrentWatchdog {
		public		Dictionary<PRICE_LEVEL, TOTAL_LOTS>	InnerDictionary	{ get; protected set; }

		public int Count(object owner, string lockPurpose, int waitMillis = 1000) {
			int count = -1;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				count = this.InnerDictionary.Count;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return count;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public Dictionary<PRICE_LEVEL, TOTAL_LOTS> SafeCopy(object owner, string lockPurpose, int waitMillis = 1000) {
			Dictionary<PRICE_LEVEL, TOTAL_LOTS> ret = new Dictionary<PRICE_LEVEL, TOTAL_LOTS>();
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				ret = new Dictionary<PRICE_LEVEL, TOTAL_LOTS>(this.InnerDictionary);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}

		public ConcurrentDictionaryGeneric(string reasonToExist) : base(reasonToExist) {
			InnerDictionary	= new Dictionary<PRICE_LEVEL, TOTAL_LOTS>();
		}
		public bool ContainsKey(PRICE_LEVEL priceLevel, object owner, string lockPurpose, int waitMillis = 1000) {
			bool contains = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				contains = this.InnerDictionary.ContainsKey(priceLevel);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return contains;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual void Clear(object owner, string lockPurpose, int waitMillis = 1000) {
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				this.InnerDictionary.Clear();
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual bool Remove(PRICE_LEVEL priceLevel, object owner, string lockPurpose, int waitMillis = 1000, bool absenseThrowsAnError = true) {
			bool removed = false;
			try {
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + priceLevel.ToString();
						Assembler.PopupException(msg);
					}
				} else {
					removed = this.InnerDictionary.Remove(priceLevel);
				}
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return removed;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual bool Add(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
											, object owner, string lockPurpose, int waitMillis = 1000, bool duplicateThrowsAnError = true) {
			bool added = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) && duplicateThrowsAnError) {
					string msg = this.reasonToExist + ": MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
				} else {
					this.InnerDictionary.Add(priceLevel, totalLots);
					added = true;
				}
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return added;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public virtual bool Update(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
											, object owner, string lockPurpose, int waitMillis = 1000, bool absenceThrowsAnError = true) {
			bool updated = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis, true);
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) == false && absenceThrowsAnError) {
					string msg = this.reasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
				} else {
					this.InnerDictionary[priceLevel] = totalLots;
					updated = true;
				}
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return updated;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
		public override string ToString() {
			string ret = string.Format("{0}:InnerDictionary[{1}]", reasonToExist, this.InnerDictionary.Count);
			return ret;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}
	}
}
