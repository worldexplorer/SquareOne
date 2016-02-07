using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter {

		public virtual void Connect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Disconnect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual string ModifyOrderTypeAccordingToMarketOrderAsBrokerSpecificInjection(Order order) {
			return "";
		}
		public virtual void OrderSubmit(Order order) {
			throw new Exception("please override BrokerAdapter::SubmitOrder() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void CancelReplace(Order order, Order newOrder) {
			throw new Exception("please override BrokerAdapter::CancelReplace() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void OrderKillSubmit(Order victimOrder) {
			throw new Exception("please override BrokerAdapter::OrderKillSubmit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void OrderPendingKillWithoutKillerSubmit(Order victimOrder) {
			throw new Exception("please override BrokerAdapter::OrderPendingKillSubmit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void OrderKillSubmitUsingKillerOrder(Order killerOrder) {
			throw new Exception("please override BrokerAdapter::OrderKillSubmitUsingKillerOrder() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual void OrderPreSubmitEnrichBrokerSpecificInjection(Order order) {
		}
		public virtual void MoveStopLossOverrideable(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.MoveStopLoss(proto, newActivationOffset, newStopLossNegativeOffset);
		}
		public void MoveTakeProfitOverrideable(PositionPrototype proto, double newTakeProfitPositiveOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.MoveTakeProfit(proto, newTakeProfitPositiveOffset);
		}

		public virtual bool AnnihilateCounterpartyAlert(Alert alert) {
			throw new Exception("please override BrokerAdapter::AnnihilateCounterpartyAlert() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual  void AlertKillPending(Alert alert) {
			throw new Exception("please override BrokerAdapter::AlertKillPending() for BrokerAdapter.Name=[" + Name + "]");
		}
	}
}