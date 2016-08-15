using System;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public class OrderStateDisplayed {
		[JsonProperty]	public	OrderState	OrderState	{ get; private set; }
		[JsonProperty]	public	bool		Displayed;

		public OrderStateDisplayed(OrderState orderState, bool displayed = true) {
			this.OrderState	= orderState;
			this.Displayed	= displayed;
		}

		public override string ToString() {
			string ret = this.OrderState.ToString() + ":";
			ret += this.Displayed ? "displayed" : "notDisplayed";
			return ret;
		}
	}
}
