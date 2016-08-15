using System;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core.Broker;

namespace Sq1.Core.Execution {
	public class OrderStateMessage {
		[JsonProperty]	public		int				Serno;
		[JsonProperty]	public		DateTime		DateTime					{ get; private set; }
		[JsonProperty]	public		OrderState		State						{ get; private set; }
		[JsonIgnore]	public		Order			Order						{ get; private set; }
		[JsonProperty]	public		string			Message						{ get; private set; }
		[JsonProperty]	public		bool			Message_ChangedOrderState	{ get; internal set; }
		[JsonProperty]	public		bool			Message_LedToPostProcessing	{ get; internal set; }

		[JsonIgnore]	public	BrokerAdapter	BrokerAdapter_nullForDeserialized	{ get {
			BrokerAdapter ret = null;
			if (this.Order											== null) return ret;
			if (this.Order.Alert									== null) return ret;
			if (this.Order.Alert.Bars								== null) return ret;
			if (this.Order.Alert.DataSource_fromBars				== null) return ret;
			if (this.Order.Alert.DataSource_fromBars.BrokerAdapter	== null) return ret;
			ret = this.Order.Alert.DataSource_fromBars.BrokerAdapter;
			return ret;
		} }
		[JsonProperty]			Color			backColor_fromBroker_cached;
		[JsonIgnore]	public	Color			BackColor_fromBroker_neverNull	{ get {
			if (this.backColor_fromBroker_cached == null) {
				BrokerAdapter broker = this.BrokerAdapter_nullForDeserialized;
				if (broker != null) {
					this.backColor_fromBroker_cached = broker.GetBackGroundColor_forOrderStateMessage_nullUnsafe(this);
				}
			}
			if (this.backColor_fromBroker_cached == null) {
				this.backColor_fromBroker_cached = Color.Empty;
			}
			return this.backColor_fromBroker_cached;
		} }


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
			Message_LedToPostProcessing = false;
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
		}
		
		public override string ToString() {
			return "#" + this.Serno + " " + this.State + ": " + this.Message;
		}

		public bool DateTime_forProperSorting_incrementOneMillis_ifEqualsTo(DateTime lastDateTime_shouldBeOlder) {
			bool incremented = false;
			TimeSpan oneMillisecond = new TimeSpan(0, 0, 0, 1);
			TimeSpan diff = this.DateTime.Subtract(lastDateTime_shouldBeOlder);
			if (diff > oneMillisecond) return incremented;
			this.DateTime = this.DateTime.AddMilliseconds(1);
			incremented = true;
			return incremented;
		}
	}
}
