using System;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public class OrderStateMessage {
		[JsonIgnore]	public Order		Order		{ get; private set; }
		[JsonProperty]	public DateTime		DateTime	{ get; private set; }
		[JsonProperty]	public OrderState	State		{ get; private set; }
		[JsonProperty]	public string		Message		{ get; private set; }
		
		OrderStateMessage(Order order, OrderState state, string message, DateTime dateTime) {
			Order = order;
			State = state;
			Message = message;
			DateTime = dateTime;
		}

		public OrderStateMessage() : this(null, OrderState.Unknown, "JSON_DESERIALIZATION_ERROR", DateTime.Now) { }

		public OrderStateMessage(Order order, string message) : this(order, order.State, message, DateTime.Now) { }
		
		public OrderStateMessage(Order order, OrderState state, string message) : this(order, state, message, DateTime.Now) {
			if (order.State == state) {
				string msg = "USE_SIMPLER_CTOR()_FOR_SAME_ORDER_STATE [" + order + "]"
					+ " this is how I removed Order parameter from UpdateOrderState_postProcess(Order)";
				Assembler.PopupException(msg, null, false);
			}
			return ;
		}
		
		public override string ToString() {
			return this.State + ": " + this.Message;
		}
	}
}
