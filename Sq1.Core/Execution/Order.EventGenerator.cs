using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public partial class Order {		
		public event EventHandler<OrderStateChangedEventArgs> OnOrderStateChanged;
		
		void raiseOnOrderStateChanged(Order order_justAbsorbedNewState, OrderState orderState_beforeGotAbsorbed) {
		    if (this.OnOrderStateChanged == null) return;
		    try {
		        this.OnOrderStateChanged(this, new OrderStateChangedEventArgs(order_justAbsorbedNewState, orderState_beforeGotAbsorbed));
		    } catch (Exception ex) {
		        string msg = "Order.OnOrderStateChanged(bar[" + order_justAbsorbedNewState + "])";
		        Assembler.PopupException(msg, ex, false);
		    }
		}
	}
}