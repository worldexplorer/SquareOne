using System;

using Newtonsoft.Json;

using Sq1.Core.Execution;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Broker {
	public abstract partial class BrokerAdapter {

		public virtual void InitializeDataSource_inverse(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			this.DataSource			= dataSource;
			this.StreamingAdapter	= streamingAdapter;
			this.OrderProcessor		= orderProcessor;
			this.AccountAutoPropagate.Initialize(this);
			//NULL_UNTIL_QUIK_PROVIDES_OWN_DDE_REDIRECTOR this.LivesimBroker_ownImplementation		= new LivesimBrokerDefault(true);
			//this.LivesimBroker.Initialize(dataSource);
			this.UpstreamConnectionState = ConnectionState.UnknownConnectionState;
		}

		public virtual void Broker_connect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Broker_disconnect() {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual string Order_modifyType_accordingToMarketOrder_asBrokerSpecificInjection(Order order) {
			return "";
		}
		public virtual void Order_submit(Order order) {
			throw new Exception("please override BrokerAdapter::Submit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Order_killPending_replaceWithNew(Order order, Order newOrder) {
			throw new Exception("please override BrokerAdapter::CancelReplace() for BrokerAdapter.Name=[" + Name + "]");
			//this.OrderProcessor.Emit_oderPending_replace(order, newOrder);
		}

		//v1
		//public virtual void Order_kill(Order victimOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_kill() for BrokerAdapter.Name=[" + Name + "]");
		//}
		//public virtual void Order_killPending_withoutKiller(Order victimOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_killPending_withoutKiller() for BrokerAdapter.Name=[" + Name + "]");
		//}
		//public virtual void Order_killPending_usingKiller(Order killerOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_killPending_usingKiller() for BrokerAdapter.Name=[" + Name + "]");
		//}
		//v2
		public abstract void Order_kill_dispatcher(Order killerOrder_withRefToVictim);
		//public abstract void Order_killPending_withoutKiller(Order killerOrder_withRefToVictim);
		public abstract void Order_killPending_usingKiller(Order killerOrder_withRefToVictim);

		public virtual void Order_enrichAlert_brokerSpecificInjection(Order order) {
		}
		public virtual void OrderMoveExisting_stopLoss_overrideable(PositionPrototype proto, double newActivation_negativeOffset, double newStopLoss_negativeOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.Emit_stopLossMove_byKillingAndSubmittingNew(proto, newActivation_negativeOffset, newStopLoss_negativeOffset);
		}
		public virtual void OrderMoveExisting_takeProfit_overrideable(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			// broker adapters might put some additional order processing,
			// but they must call OrderProcessor.MoveStopLoss() or imitate similar mechanism
			this.OrderProcessor.Emit_takeProfitMove_byKillingAndSubmittingNew(proto, newTakeProfit_positiveOffset);
		}

		public virtual bool AlertCounterparty_annihilate(Alert alert) {
			throw new Exception("please override BrokerAdapter::AlertCounterparty_annihilate() for BrokerAdapter.Name=[" + Name + "]");
			//alert.AbsorbFromExecutorAfterCreatedByMarketReal
			//this.OrderProcessor.Aler
		}

		public virtual void AlertPending_kill(Alert alert) {
			throw new Exception("please override BrokerAdapter::AlertPending_kill() for BrokerAdapter.Name=[" + Name + "]");
		}
	}
}