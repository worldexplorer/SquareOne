using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentDictionaryGeneric<PRICE_LEVEL, TOTAL_LOTS> : ConcurrentWatchdog {
		public		Dictionary<PRICE_LEVEL, TOTAL_LOTS>	InnerDictionary	{ get; protected set; }

		public int Count { get { lock(this.LockObject) { return this.InnerDictionary.Count; } } }

		public Dictionary<PRICE_LEVEL, TOTAL_LOTS> SafeCopy(object owner, string lockPurpose, int waitMillis = 1000) {
			try {
				base.LockFor(owner, lockPurpose, waitMillis, true);
				return new Dictionary<PRICE_LEVEL, TOTAL_LOTS>(this.InnerDictionary);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}

		public ConcurrentDictionaryGeneric(string reasonToExist) : base(reasonToExist) {
			InnerDictionary	= new Dictionary<PRICE_LEVEL, TOTAL_LOTS>();
		}
		public bool ContainsKey(PRICE_LEVEL priceLevel, object owner, string lockPurpose, int waitMillis = 1000) {
			try {
				base.LockFor(owner, lockPurpose, waitMillis, true);
				return this.InnerDictionary.ContainsKey(priceLevel);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual void Clear(object owner, string lockPurpose, int waitMillis = 1000) {
			try {
				base.LockFor(owner, lockPurpose, waitMillis, true);
				this.InnerDictionary.Clear();
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual bool Remove(PRICE_LEVEL priceLevel, object owner, string lockPurpose, int waitMillis = 1000, bool absenseThrowsAnError = true) {
			try {
				bool removed = false;
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + priceLevel.ToString();
						Assembler.PopupException(msg);
						return removed;
					}
				} else {
					removed = this.InnerDictionary.Remove(priceLevel);
				}
				return removed;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual bool Add(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
											, object owner, string lockPurpose, int waitMillis = 1000, bool duplicateThrowsAnError = true) {
			try {
				base.LockFor(owner, lockPurpose, waitMillis, true);
				bool added = false;
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) && duplicateThrowsAnError) {
					string msg = this.ReasonToExist + ": MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
					return added;
				}
				this.InnerDictionary.Add(priceLevel, totalLots);
				added = true;
				return added;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public virtual bool Update(PRICE_LEVEL priceLevel, TOTAL_LOTS totalLots
											, object owner, string lockPurpose, int waitMillis = 1000, bool absenceThrowsAnError = true) {
			try {
				base.LockFor(owner, lockPurpose, waitMillis, true);
				bool added = false;
				if (this.ContainsKey(priceLevel, owner, lockPurpose, waitMillis) == false && absenceThrowsAnError) {
					string msg = this.ReasonToExist + ": I_REFUSE_TO_UPDATE__WAS_NOT_ADDED " + priceLevel.ToString();
					Assembler.PopupException(msg, null, false);
					return added;
				}
				this.InnerDictionary[priceLevel] = totalLots;
				added = true;
				return added;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public override string ToString() { lock(this.LockObject) {
			return string.Format("{0}:InnerDictionary[{1}]", ReasonToExist, this.InnerDictionary.Count);
		} }
	}
}
