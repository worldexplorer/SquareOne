using System;
using System.Collections.Generic;

using Sq1.Core.Support;
using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrdersSearchable : ConcurrentListFiltered<Order> {
		public OrdersSearchable(string reasonToExist) : base(reasonToExist) {
		}
		
		public override string ToString_forMatch(Order order) {
			string ret = "";
			foreach (OrderStateMessage eachOsm in order.MessagesSafeCopy) {
				if (ret != "") ret += ",";
				ret += eachOsm.ToString();
			}
			return ret;
		}


		//public override bool AppendUnique(Order order, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
		//    return base.AppendUnique(order, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		//}

		public new bool InsertUnique(Order order, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
		    return base.InsertUnique(order, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}

		public new int AddRange(List<Order> ordersToRemove, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
		    return base.AddRange(ordersToRemove, owner, lockPurpose, waitMillis, absenceThrowsAnError);
		}

		public new int RemoveRange(List<Order> ordersToRemove, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
		    return base.RemoveRange(ordersToRemove, owner, lockPurpose, waitMillis, absenceThrowsAnError);
		}

	}
}
