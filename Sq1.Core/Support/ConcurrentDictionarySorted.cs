using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentDictionarySorted<PRICE_LEVEL, TOTAL_LOTS> : ConcurrentDictionary<PRICE_LEVEL, TOTAL_LOTS> {
		public class  ASC : IComparer<PRICE_LEVEL> {
			int IComparer<PRICE_LEVEL>.Compare(PRICE_LEVEL x, PRICE_LEVEL y) {
				return double.Parse(x.ToString()) > double.Parse(y.ToString()) ? 1 : -1;
			}
		}
		public class DESC : IComparer<PRICE_LEVEL> {
			int IComparer<PRICE_LEVEL>.Compare(PRICE_LEVEL x, PRICE_LEVEL y) {
				return double.Parse(x.ToString()) > double.Parse(y.ToString()) ? -1 : 1;
			}
		}

		public SortedDictionary<PRICE_LEVEL, TOTAL_LOTS> SafeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			SortedDictionary<PRICE_LEVEL, TOTAL_LOTS> ret = new SortedDictionary<PRICE_LEVEL, TOTAL_LOTS>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				ret = new SortedDictionary<PRICE_LEVEL, TOTAL_LOTS>(this.InnerDictionary);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;	//I'm sucpiscious about returning inside try{}; when outside I know finally{} has unlocked before popping up the stack; otherwize I'm not sure what/when finalizer did
		}


		public ConcurrentDictionarySorted(string reasonToExist, IComparer<PRICE_LEVEL> orderby) : base(reasonToExist) {
			base.InnerDictionary	= new SortedDictionary<PRICE_LEVEL, TOTAL_LOTS>(orderby);
		}
		public override string ToString() {
			return this.ReasonToExist + ":InnerSortedDictionary[" + this.InnerDictionary.Count + "]";
		}
	}
}
