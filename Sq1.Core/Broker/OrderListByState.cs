using System;
using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderListByState : OrderList {
		public OrderStatesCollections StatesAllowed { get; private set; }

		public OrderListByState() {
			throw new Exception("you must supply OrderStatesCollections allows for this OrderList, use another constructor");
		}
		public OrderListByState(OrderStatesCollections orderStatesAllowed) {
			if (orderStatesAllowed == null) {
				throw new Exception("CTOR_FAILED: orderStatesAllowed=NULL: you must supply non-null, non-empty OrderStatesCollections allows for this OrderList");
			}
			if (orderStatesAllowed.Count == 0) {
				throw new Exception("CTOR_FAILED: orderStatesAllowed.OrderStates.Count=0: you must supply non-empty OrderStatesCollections allows for this OrderList");
			}
			this.StatesAllowed = orderStatesAllowed;
			base.Capacity = 2000;
		}

		protected override bool checkThrowAdd(Order order) {
			// don't throw - we are adding into a fake collection anyway (created as new, will be dropped)
			if (this.StatesAllowed == OrderStatesCollections.Unknown) return true;
			if (this.StateIsAcceptable(order.State)) return true;
			string msg = "OrderAdding.State[" + order.State + "] is not in the list of " + StatesAllowed.ToString();
			throw new Exception(msg);
		}
		protected override bool checkThrowRemove(Order order) {
			return true;
		}

		public bool StateIsAcceptable(OrderState orderState) {
			return this.StatesAllowed.Contains(orderState);
		}
		public bool StateIsAcceptableAndDoesntContain(Order order) {
			return this.StateIsAcceptable(order.State) && (base.Contains(order) == false);
		}
		public override string ToString() {
			return "OrderListByState[" + StatesAllowed.ToString() + "]";
		}
		public override string ToShortString() {
			return StatesAllowed.CollectionName;
		}
	}
}