using System;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public class OrderStateMessage {
		[JsonIgnore]	public Order		Order						{ get; private set; }
		[JsonIgnore]	public	bool		OnlyDeserializedHasNoOrder	{ get { return this.Order == null; } }

		[JsonProperty]	public DateTime		DateTime	{ get; private set; }
		[JsonProperty]	public OrderState	State		{ get; private set; }
		[JsonProperty]	public string		Message		{ get; private set; }
		
		OrderStateMessage(Order order, OrderState state, string message, DateTime dateTime) {
			if (order == null) {
				string msg = "ONLY_DESERIALIZED_OrderStateMessage_HAS_ORDER_NULL_TO_AVOID_RECURSION"
					+ " ANY_OTHER_OMSG_YOU_USE_MUST_HAVE_AN_ORDER_INSIDE";
				throw new Exception(msg);
			}
			Order = order;
			State = state;
			Message = message;
			DateTime = dateTime;
		}

		//public OrderStateMessage() : this(null, OrderState.Unknown, "JSON_DESERIALIZATION_ERROR", DateTime.Now) { }
		public OrderStateMessage() {
			Order = null;
			State = OrderState.JustDeserialized;
			Message = "JSON_DESERIALIZATION_ERROR";
			DateTime = DateTime.Now;
		}

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
