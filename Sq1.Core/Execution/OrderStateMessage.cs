using System;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public class OrderStateMessage {
		[JsonIgnore]	public Order		Order		{ get; private set; }
		[JsonProperty]	public DateTime		DateTime	{ get; private set; }
		[JsonProperty]	public OrderState	State		{ get; private set; }
		[JsonProperty]	public string		Message		{ get; private set; }
		
		public OrderStateMessage() : this(null, OrderState.Unknown, "JSON_DESERIALIZATION_ERROR", DateTime.Now) { }
		//public OrderStateMessage(string message, DateTime dateTime) : this(null, message, DateTime.Now, OrderState.Unknown) {}
		public OrderStateMessage(Order order, string message)
			: this(order, order.State, message, DateTime.Now) { }
		public OrderStateMessage(Order order, OrderState state, string message)
				: this(order, state, message, DateTime.Now) { }
		public OrderStateMessage(Order order, OrderState state, string message, DateTime dateTime) {
			Order = order;
			State = state;
			Message = message;
			DateTime = dateTime;
		}
		public override string ToString() {
			return this.State + ": " + this.Message;
		}
	}
}
