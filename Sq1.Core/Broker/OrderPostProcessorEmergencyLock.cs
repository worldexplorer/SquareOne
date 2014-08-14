using System;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorEmergencyLock : IEquatable<OrderPostProcessorEmergencyLock> {
		public Order OrderReasonForLock;
		public Strategy strategy;
		public Script script;
		public OrderPostProcessorEmergencyLock(Order order) {
			this.OrderReasonForLock = order;
			if (order.Alert == null) {
				//string msg = "order.Alert=null OrderPostProcessorEmergencyLock(" + order + ")";
				//throw new Exception(msg);
				return;
			}
			if (order.Alert.Strategy == null) {
				//string msg = "order.Alert.Strategy=null OrderPostProcessorEmergencyLock(" + order + ")";
				//throw new Exception(msg);
				return;
			}
			this.strategy = order.Alert.Strategy;
			this.script = order.Alert.Strategy.Script;
		}
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				if (strategy != null) hash = hash * 23 + strategy.GetHashCode();
				if (script != null) hash = hash * 23 + script.GetHashCode();
				return hash;
			}
		}
		public override bool Equals(object obj) {
			if ((obj is OrderPostProcessorEmergencyLock) == false) return false;
			return Equals((OrderPostProcessorEmergencyLock)obj);
		}
		public bool Equals(OrderPostProcessorEmergencyLock other) {
			return this.strategy == other.strategy && this.script == other.script
				&& this.OrderReasonForLock.Alert.DataSource == other.OrderReasonForLock.Alert.DataSource
				&& this.OrderReasonForLock.Alert.Symbol == other.OrderReasonForLock.Alert.Symbol
				;
		}
		public override string ToString() {
			return "EmergencyLock strategy[" + strategy + "] script[" + script + "] order[" + OrderReasonForLock + "]";
		}
	}
}
