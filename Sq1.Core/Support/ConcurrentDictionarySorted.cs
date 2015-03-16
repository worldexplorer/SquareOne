using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentDictionarySorted<PRICE_LEVEL, TOTAL_LOTS> : ConcurrentDictionary<PRICE_LEVEL, TOTAL_LOTS> {

		// slow & ugly with generics!! hopefully wont be a pita for level2, otherwise use unsorted Dictionary and paint while enumerating pricelevels
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

		public ConcurrentDictionarySorted(string reasonToExist, IComparer<PRICE_LEVEL> orderby) : base(reasonToExist) {
			//QUICK_HACK_FAILED
			base.InnerDictionary	= new SortedDictionary<PRICE_LEVEL, TOTAL_LOTS>(orderby);
		}
		public override string ToString() { lock(this.LockObject) {
			return string.Format("{0}:InnerSortedDictionary[{1}]", ReasonToExist, base.InnerDictionary.Count);
		} }
	}
}
