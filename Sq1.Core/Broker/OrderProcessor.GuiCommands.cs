using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public void SubmitEatableOrdersFromGui(List<Order> orders) {
			List<Order> ordersEatable = new List<Order>();
			foreach (Order order in orders) {
				if (this.IsOrderEatable(order) == false) continue;
				ordersEatable.Add(order);
				string msg = "Submitting Eatable Order From Gui";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Submitting, msg);
				this.UpdateOrderStateAndPostProcess(order, newOrderState);
			}
			if (ordersEatable.Count > 0) {
				BrokerProvider broker = extractSameBrokerProviderThrowIfDifferent(ordersEatable, "SubmitEatableOrders(): ");
				broker.SubmitOrders(ordersEatable);
			}
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
		}
		public void KillAll() {
			//List<Order> allOrdersClone = new List<Order>();
			//this.KillSelectedOrders(allOrdersClone);
			Assembler.PopupException("DOESNT_MAKE_SENSE_NOT_IMPLEMETED KillAll()");
		}
		public void KillPending(List<Order> orders) {
			if (orders.Count == 0) return;
			List<Order> ordersToCancel = new List<Order>();
			for (int i = orders.Count - 1; i >= 0; i--) {
				Order victimOrder = orders[i];
				this.KillOrderUsingKillerOrder(victimOrder);
			}
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
		}
		public void CancelStrategyOrders(string account, Strategy strategy, string symbol, BarScaleInterval dataScale) {
			try {
				Exception e = new Exception("just for call stack trace");
				throw e;
			} catch (Exception ex) {
				string msg = "I won't cancel any orders; reconsider application architecture";
				Assembler.PopupException(msg, ex);
			}
		}
		public void CancelAllPending() {
			List<Order> ordersToCancel = this.DataSnapshot.OrdersPending.InnerOrderList;
			if (ordersToCancel.Count == 0) {
				string msg = "NO_PENDING_ORDERS_TO_CANCEL__SHOULD_I_CHECK_ANOTHER_LANE?... OrdersPending.Count[" + ordersToCancel.Count + "]";
				return;
			}
			BrokerProvider broker = this.extractSameBrokerProviderThrowIfDifferent(ordersToCancel, "CancelAll(): ");
			this.KillPending(ordersToCancel);
		}
		public void CancelReplaceOrder(Order orderToReplace, Order orderReplacement) {
			string msgVictim = "expecting callback on successful REPLACEMENT completion [" + orderReplacement + "]";
			OrderStateMessage newOrderStateVictim = new OrderStateMessage(orderToReplace, OrderState.KillPending, msgVictim);
			this.UpdateOrderStateAndPostProcess(orderToReplace, newOrderStateVictim);

			orderReplacement.State = OrderState.Submitted;
			orderReplacement.IsReplacement = true;
			this.DataSnapshot.OrderInsertNotifyGuiAsync(orderReplacement);

			if (orderToReplace.hasBrokerProvider("CancelReplaceOrder(): ") == false) {
				string msg = "CRAZY #65";
				Assembler.PopupException(msg);
				return;
			}
			orderToReplace.Alert.DataSource.BrokerProvider.CancelReplace(orderToReplace, orderReplacement);

			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
		}
	}
}