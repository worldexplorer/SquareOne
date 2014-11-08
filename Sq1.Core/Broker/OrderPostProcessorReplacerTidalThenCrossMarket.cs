using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorReplacerTidalThenCrossMarket {
		private Dictionary<List<Order>, List<Order>> ordersCloseOpen;
		private List<Order> ordersClose;
		private OrderProcessor orderProcessor;

		public OrderPostProcessorReplacerTidalThenCrossMarket(OrderProcessor orderProcessor) {
			this.ordersCloseOpen = new Dictionary<List<Order>, List<Order>>();
			this.orderProcessor = orderProcessor;
		}

		public void InitializeSequence(List<Order> ordersClose, List<Order> ordersOpen) {
			lock (this.ordersCloseOpen) {
				this.ordersCloseOpen.Add(ordersClose, ordersOpen);
			}
			foreach (Order sequenced in ordersOpen) {
				OrderStateMessage omsg = new OrderStateMessage(sequenced, OrderState.SubmittingSequenced,
					"sequence initialized: [" + sequenced.State + "]=>[" + OrderState.SubmittingSequenced + "]"
					+ " for [" + ordersOpen.Count + "] fellow ordersOpen"
					+ " by [" + ordersClose.Count + "]ordersClose");
				this.orderProcessor.UpdateOrderStateAndPostProcess(sequenced, omsg);
			}
		}
		public void OrderFilledUnlockSequenceSubmitOpening(Order order) {
			List<List<Order>> ordersCloseFoundInKeys = new List<List<Order>>();
			lock (this.ordersCloseOpen) {
				foreach (List<Order> ordersClose in this.ordersCloseOpen.Keys) {
					if (ordersClose.Contains(order) == false) continue;
					ordersCloseFoundInKeys.Add(ordersClose);
				}
				foreach (List<Order> ordersCloseFound in ordersCloseFoundInKeys) {
					ordersCloseFound.Remove(order);
					if (ordersCloseFound.Count == 0) {
						List<Order> ordersOpen = this.ordersCloseOpen[ordersCloseFound];
						this.ordersCloseOpen.Remove(ordersCloseFound);
						if (ordersOpen.Count == 0) continue;
						this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order,
							"last CloseOpenSequence order filled, unlocking submission of [" + ordersOpen.Count + "]ordersOpen");
						foreach (Order submitting in ordersOpen) {
							OrderStateMessage omsg = new OrderStateMessage(submitting, OrderState.Submitting,
								"sequence cleared: [" + submitting.State + "]=>[" + OrderState.Submitting + "]"
								+ " for [" + ordersOpen.Count + "] fellow ordersOpen"
								+ " by orderClose=[" + order + "]");
							this.orderProcessor.UpdateOrderStateAndPostProcess(submitting, omsg);
						}
						BrokerProvider broker = ordersOpen[0].Alert.DataSource.BrokerProvider;
						ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersOpen });
					}
				}
			}
		}
	}
}
