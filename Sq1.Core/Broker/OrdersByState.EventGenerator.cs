using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public partial class OrdersByState {		
		public event EventHandler<OrderStateChangedEventArgs> OnOrderStateChanged_ExecutorTreeShouldRefresh_ifStateAffectView;
		
		void raiseOnOrderStateChanged(Order order_justAbsorbedNewState, OrderState orderState_beforeGotAbsorbed) {
		    if (this.OnOrderStateChanged_ExecutorTreeShouldRefresh_ifStateAffectView == null) return;
		    try {
		        this.OnOrderStateChanged_ExecutorTreeShouldRefresh_ifStateAffectView(this, new OrderStateChangedEventArgs(order_justAbsorbedNewState, orderState_beforeGotAbsorbed));
		    } catch (Exception ex) {
		        string msg = "OrdersByState.OnOrderStateChanged(bar[" + order_justAbsorbedNewState + "])";
		        Assembler.PopupException(msg, ex, false);
		    }
		}
	}
}