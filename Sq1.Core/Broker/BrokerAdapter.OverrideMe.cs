using System;

using Newtonsoft.Json;

using Sq1.Core.Execution;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter {

		public virtual void InitializeDataSource_inverse(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			this.DataSource			= dataSource;
			this.StreamingAdapter	= streamingAdapter;
			this.OrderProcessor		= orderProcessor;
			this.AccountAutoPropagate.Initialize(this);
			//NULL_UNTIL_QUIK_PROVIDES_OWN_DDE_REDIRECTOR this.LivesimBroker_ownImplementation		= new LivesimBrokerDefault(true);
			//this.LivesimBroker.Initialize(dataSource);
			this.UpstreamConnectionState = ConnectionState.UnknownConnectionState;
		}

		public virtual void Connect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Disconnect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual string ModifyOrderType_accordingToMarketOrder_asBrokerSpecificInjection(Order order) {
			return "";
		}
		public virtual void Submit(Order order) {
			throw new Exception("please override BrokerAdapter::Submit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void CancelReplace(Order order, Order newOrder) {
			throw new Exception("please override BrokerAdapter::CancelReplace() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Kill(Order victimOrder) {
			throw new Exception("please override BrokerAdapter::Kill() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void KillPending_withoutKiller(Order victimOrder) {
			throw new Exception("please override BrokerAdapter::KillPending_withoutKiller() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Kill_usingKiller(Order killerOrder) {
			throw new Exception("please override BrokerAdapter::Kill_usingKiller() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual void OrderEnrich_preSubmit_brokerSpecificInjection(Order order) {
		}
		public virtual void StopLossMove_overrideable(PositionPrototype proto, double newActivationOffset, double newStopLoss_negativeOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.StopLossMove(proto, newActivationOffset, newStopLoss_negativeOffset);
		}
		public virtual void TakeProfitMove_overrideable(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.TakeProfitMove(proto, newTakeProfit_positiveOffset);
		}

		public virtual bool AlertCounterparty_annihilate(Alert alert) {
			throw new Exception("please override BrokerAdapter::AnnihilateCounterpartyAlert() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual void AlertPending_kill(Alert alert) {
			throw new Exception("please override BrokerAdapter::AlertKillPending() for BrokerAdapter.Name=[" + Name + "]");
		}
	}
}