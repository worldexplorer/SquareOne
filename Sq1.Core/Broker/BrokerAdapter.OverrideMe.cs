using System;
using System.Collections.Generic;
using System.Drawing;

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
		public virtual void Broker_disconnect(string reasonToDisconnect = "UNKNOWN_reasonToDisconnect") {
			throw new Exception("please override BrokerAdapter::Connect() for BrokerAdapter.Name=[" + Name + "]");
		}

		public virtual string Order_modifyType_accordingToMarketOrder_asBrokerSpecificInjection(Order order) {
			return "";
		}
		public virtual void Order_submit_oneThread_forAllNewAlerts_trampoline(Order order) {
			throw new Exception("please override BrokerAdapter::Submit() for BrokerAdapter.Name=[" + Name + "]");
		}
		public virtual void Order_killPending_replaceWithNew(Order order, Order newOrder) {
			throw new Exception("please override BrokerAdapter::CancelReplace() for BrokerAdapter.Name=[" + Name + "]");
			//this.OrderProcessor.Emit_oderPending_replace(order, newOrder);
		}

		public abstract void Order_submitKiller_forPending(Order killerOrder_withRefToVictim);

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

		public virtual bool AlertCounterparty_annihilate(Alert alertCounterparty_toAnnihilate) {
			string msig = " //" + this.Name + ".AlertCounterparty_annihilate(" + alertCounterparty_toAnnihilate + ") LIVE+SIM";

			Order orderCounterparty_toAnnihilate = alertCounterparty_toAnnihilate.OrderFollowed;
			if (orderCounterparty_toAnnihilate == null) {
				string msg = "ALERT_MUST_HAVE_ORDER_FOLLOWED";
				Assembler.PopupException(msg);
				return false;
			}

			if (alertCounterparty_toAnnihilate.PositionAffected == null) {
				string msg = "ALERT_MUST_HAVE_POSITION_TO_REACH_PROTOTYPE";
				Assembler.PopupException(msg);
				return false;
			}

			PositionPrototype proto = alertCounterparty_toAnnihilate.PositionPrototype;
			if (proto == null) {
				string msg = "POSITION_MUST_HAVE_A_PROTOTYPE";
				Assembler.PopupException(msg);
				return false;
			}

			if (proto.TakeProfitAlert_forMoveAndAnnihilation == null) {
				string msg = "PROTOTYPE_MUST_HAVE_TAKE_PROFIT_NOT_NULL";
				Assembler.PopupException(msg);
				return false;
			}

			if (proto.StopLossAlert_forMoveAndAnnihilation == null) {
				string msg = "PROTOTYPE_MUST_HAVE_STOP_LOSS_NOT_NULL";
				Assembler.PopupException(msg);
				return false;
			}

			OrderState newState = OrderState.IRefuseToAnnihilateNonPrototyped;
			if (alertCounterparty_toAnnihilate.ImTakeProfit_prototyped) newState =  OrderState.TPAnnihilating;
			if (alertCounterparty_toAnnihilate.ImStopLoss_prototyped)	newState =  OrderState.SLAnnihilating;

			Order whatWasFilled = alertCounterparty_toAnnihilate.PositionAffected.ExitAlert.OrderFollowed;
			string whatWasTheTrigger = "TRIGGER_FOR_ANNIHILATION whatWasFilled[" + whatWasFilled + "]";
			OrderStateMessage omsg_counterParty_annihilating = new OrderStateMessage(orderCounterparty_toAnnihilate, newState, whatWasTheTrigger);
			this.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_counterParty_annihilating);

			bool emitted = this.OrderProcessor.Emit_killOrderPending_usingKiller(orderCounterparty_toAnnihilate, msig);
			return emitted;
		}

		public virtual int AlertPendings_kill(List<Alert> alerts2kill_afterScript_onQuote_onBar) {
			int emitted = this.OrderProcessor.Emit_alertsPending_kill(alerts2kill_afterScript_onQuote_onBar);
			return emitted;
		}

		public virtual Color GetBackGroundColor_forOrderStateMessage_nullUnsafe(OrderStateMessage osm) {
			return Color.Empty;
		}
	}
}