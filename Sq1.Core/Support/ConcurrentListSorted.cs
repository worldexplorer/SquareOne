using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentListSorted<ORDERS> : ConcurrentList<ORDERS> {
		public ConcurrentListSorted(string reasonToExist, ExecutorDataSnapshot snap = null) : base(reasonToExist, snap) {
			//base.InnerList	= new SortedList<int, Order>();
		}
	}
}
