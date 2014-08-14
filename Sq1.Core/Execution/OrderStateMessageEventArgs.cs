using System;
namespace Sq1.Core.Execution {
	public class OrderStateMessageEventArgs : EventArgs {
		public OrderStateMessage OrderStateMessage { get; private set; }
		public OrderStateMessageEventArgs(OrderStateMessage orderStateMessage) {
			this.OrderStateMessage = orderStateMessage;
		}
	}
}
