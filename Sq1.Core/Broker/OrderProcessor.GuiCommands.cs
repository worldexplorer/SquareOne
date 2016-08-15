using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public void GuiClick_ordersSubmit_eatable(List<Order> orders) {
			List<Order> ordersEatable = new List<Order>();
			foreach (Order order in orders) {
				if (this.isOrderEatable_notOrdersProperty(order) == false) {
					string msg1 = "I_REFUSE_TO_SUBMIT_NON_EDIBLE_ORDER [" + order + "]";
					order.AppendMessage(msg1);
					Assembler.PopupException(msg1, null, false);
					continue;
				}
				ordersEatable.Add(order);
				string msg = "Submitting Eatable Order From Gui";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.Submitting, msg);
				this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
			}
			if (ordersEatable.Count > 0) {
				BrokerAdapter broker = extractSameBrokerAdapter_throwIfDifferent(ordersEatable, "SubmitEatableOrders(): ");
				broker.EmitOrders_ownOneThread_forAllNewAlerts(ordersEatable);
			}
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
		}
		public void GuiClick_killAll() {
			//List<Order> allOrdersClone = new List<Order>();
			//this.KillSelectedOrders(allOrdersClone);
			Assembler.PopupException("NYI:KillAll()", null, false);
		}
		public void GuiClick_cancelStrategyOrders(string account, Strategy strategy, string symbol, BarScaleInterval dataScale) {
			try {
				Exception e = new Exception("just for call stack trace");
				throw e;
			} catch (Exception ex) {
				string msg = "I won't cancel any orders; reconsider application architecture";
				Assembler.PopupException(msg, ex);
			}
		}
		public void ExecutionTreeControl_OnOrderDoubleClicked_OrderProcessorShouldKillOrder(object sender, OrderEventArgs e) {
			string msig = " //ExecutionTreeControl_OnOrderDoubleClicked_OrderProcessorShouldKillOrder";
			Order orderToBeKilled = e.Order;
			this.GuiClick_killPendingSelected(new List<Order>() { orderToBeKilled }, msig);
		}
		public int GuiClick_killPendingSelected(List<Order> selectedKillable, string invoker = " //GuiClick_killPendingSelected()") {
			int killersEmitted_total = 0;
			if (selectedKillable.Count == 0) {
				Assembler.PopupException("NO_ORDER_TO_KILL selectedKillable.Count==0" + invoker);
				return killersEmitted_total;
			}

			foreach (Order orderToBeKilled in selectedKillable) {
				try {
					bool orderDeserialized_afterAppRestart = Assembler.InstanceInitialized.AlertsForChart.IsItemRegisteredForAnyContainer(orderToBeKilled.Alert) == false;
					if (orderDeserialized_afterAppRestart) {
						string msg = "I_REFUSE_TO_KILL_AN_ORDER_AFTER_APPRESTART orderToBeKilled[" + orderToBeKilled + "]"
							//+ " tree_FormatRow() sets Item.ForeColor=Color.DimGray when "
							//+ " (all JSON-deserialized orders have no chart to get popped-up)"
							;
						orderToBeKilled.AppendMessage(msg);
						Assembler.PopupException(msg + invoker, null, false);
						continue;
					}
					bool killerEmitted = this.Emit_killOrderPending_usingKiller_hookNeededAfterwards(orderToBeKilled, invoker, true);
					if (killerEmitted) killersEmitted_total++;
				} catch (Exception ex) {
					Assembler.PopupException(invoker, ex);
				}
			}
			return killersEmitted_total;
		}
		public void GuiClick_killPendingAll() {
			string msig = " //GuiClick_cancelAllPending()";
			List<Order> ordersPendingToKill = this.DataSnapshot.OrdersPending.SafeCopy;
			if (ordersPendingToKill.Count == 0) {
				string msg = "NO_PENDING_ORDERS_TO_CANCEL__SHOULD_I_CHECK_ANOTHER_LANE?... ordersPendingToKill.Count[" + ordersPendingToKill.Count + "]";
				return;
			}
			BrokerAdapter broker = this.extractSameBrokerAdapter_throwIfDifferent(ordersPendingToKill, msig);
			foreach (Order pendingOrder in ordersPendingToKill) {
				//this.Emit_killOrderPending_withoutKiller(pendingOrder, msig);
				bool emitted = this.Emit_killOrderPending_usingKiller_hookNeededAfterwards(pendingOrder, msig);
			}
		}

		public void GuiClick_orderReplace(Order pendingOrder) {
			string msig = " //GuiClick_orderReplace()";
			Order replacementOrder = pendingOrder.DeriveReplacementOrder();
			string msg = "DID_YOU_WANNA_CHANGE_PRICE_REQUESTED??? TO_CURRENT_BID/ASK???..."
				+ " NEXT_STEP_IS_TO_KILL_SELECTED_AND_SUBMIT_REPLACEMENT [" + replacementOrder + "]";
			Assembler.PopupException(msg + msig, null, false);
			this.Emit_oderPending_replace(pendingOrder, replacementOrder, msig);
		}

		BrokerAdapter extractSameBrokerAdapter_throwIfDifferent(List<Order> orders, string callerMethod) {
			BrokerAdapter broker = null;
			foreach (Order order in orders) {
				if (order.hasBrokerAdapter(callerMethod) == false) {
					string msg = "CRAZY #64";
					Assembler.PopupException(msg);
					continue;
				}
				if (broker == null) broker = order.Alert.DataSource_fromBars.BrokerAdapter;
				if (broker != order.Alert.DataSource_fromBars.BrokerAdapter) {
					throw new Exception(callerMethod + "NIY: orderProcessor can not handle orders for several brokers"
						+ "; prevOrder.Broker[" + broker + "] while someOrderBroker[" + order.Alert.DataSource_fromBars.BrokerAdapter + "]");
				}
			}
			return broker;
		}

//		public void CancelReplaceOrder(Order orderToReplace, Order orderReplacement) {
//			string msgVictim = "expecting callback on successful REPLACEMENT completion [" + orderReplacement + "]";
//			OrderStateMessage newOrderStateVictim = new OrderStateMessage(orderToReplace, OrderState.KillPending, msgVictim);
//			this.UpdateOrderStateAndPostProcess(orderToReplace, newOrderStateVictim);
//
//			orderReplacement.State = OrderState.Submitted;
//			orderReplacement.IsReplacement = true;
//			this.DataSnapshot.OrderInsertNotifyGuiAsync(orderReplacement);
//
//			if (orderToReplace.hasBrokerAdapter("CancelReplaceOrder(): ") == false) {
//				string msg = "CRAZY #65";
//				Assembler.PopupException(msg);
//				return;
//			}
//			orderToReplace.Alert.DataSource.BrokerAdapter.CancelReplace(orderToReplace, orderReplacement);
//
//			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;
//		}
	}
}