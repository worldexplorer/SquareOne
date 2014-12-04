using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderLaneByType : OrderLane {
		List<MarketLimitStop> typesAllowed;

		public OrderLaneByType(List<MarketLimitStop> orderStatesAllowed) {
			this.typesAllowed = orderStatesAllowed;
		}

		protected override bool checkThrowAdd(Order order) {
			if (typesAllowed.Contains(order.Alert.MarketLimitStop) == false) {
				string msg = "OrderAdding.State[" + order.Alert.MarketLimitStop
					+ "] is not in the list of statesAllowed"
					+ this.ToString();
				throw new Exception(msg);
			}
			return true;
		}
		protected override bool checkThrowRemove(Order order) {
			return true;
		}

		public override string ToString() {
			string ret = "";
			foreach (var status in typesAllowed) {
				ret += status + ",";
			}
			ret = ret.TrimEnd(",".ToCharArray());
			ret = "[" + ret + "]";
			return ret;
		}
	}
}
