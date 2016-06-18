using System;
using System.Collections.Generic;
//using System.Threading;
using System.Threading.Tasks;

using Sq1.Core.Accounting;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {

		public List<Order> Emit_createOrders_forScriptGeneratedAlerts_eachInNewThread(List<Alert> alertsBatch, bool setStatusSubmitting, bool emittedByScript) {
			if (alertsBatch.Count == 0) {
				string msg = "no alerts to Add; why did you call me? make sure you invoke using a synchronized Queue";
				Assembler.PopupException(msg);
				//return;
			}

			bool ordersClosingAllSameDirection = true;
			PositionLongShort ordersClosingPositionLongShort = PositionLongShort.Unknown;
			bool ordersOpeningAllSameDirection = true;
			PositionLongShort ordersOpeningPositionLongShort = PositionLongShort.Unknown;

			List<Order> orders_polarSequenceAgnostic = new List<Order>();
			List<Order> orders_polarClosingFirst = new List<Order>();
			List<Order> orders_polarOpeningSecond = new List<Order>();
			BrokerAdapter broker = null;
			foreach (Alert alert in alertsBatch) {
				// I only needed alert.OrderFollowed=newOrder... mb even CreatePropagateOrderFromAlert() should be reduced for backtest
				if (alert.Strategy.Script.Executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
					string msg = "BACKTEST_DOES_NOT_SUBMIT_ORDERS__UNCHECK_CHART=>EMIT_BUTTON";
					Assembler.PopupException(msg, null, false);
					alert.Strategy.Script.Executor.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted(msg);
					return null;
				}

				Order newOrder;
				try {
					newOrder = this.createOrder_propagateToGui_fromAlert(alert, setStatusSubmitting, emittedByScript);
				} catch (Exception ex) {
					string msg = "THROWN_this.createPropagateOrder_fromAlert";
					Assembler.PopupException(msg, ex, true);
					continue;
				}
				if (newOrder == null) {
					string msg = "ALERT_INCONSISTENT_ORDER_PROCESSOR_DIDNT_SUBMIT ";
					//ALREADY_COMPLAINED Assembler.PopupException(msg, null, false);
					continue;
				}
				if (newOrder.State != OrderState.Submitting) {
					if (newOrder.State == OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted) {
						alert.Strategy.Script.Executor.CallbackCreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably(alert, alert.Bars.Count);
						continue;
					}
					if (newOrder.State == OrderState.EmitOrdersNotClicked) continue;
					//if (newOrder.Alert.Strategy.Script.Executor.IsAutoSubmitting == true
					//&& newOrder.Alert.Strategy.Script.Executor.IsStreaming == true
					//&& newOrder.FromAutoTrading == true
					string msg = "Unexpected newOrder.State[" + newOrder.State + "] from CreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably()";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				if (broker == null) {
					broker = alert.DataSource_fromBars.BrokerAdapter;
				} else {
					if (broker != alert.DataSource_fromBars.BrokerAdapter) {
						string msg = "CROSS_EXCHANGE_ALERTS_NYI alertsBatch MUST contain alerts for the same broker"
							+ "; prevAlert.Broker[" + broker + "] while thisAlert.DataSource.BrokerAdapter[" + alert.DataSource_fromBars.BrokerAdapter + "]";
						throw new Exception(msg);
					}
				}
				if (alert.Bars.SymbolInfo.SameBarPolarCloseThenOpen) {
					if (alert.IsExitAlert) {
						orders_polarClosingFirst.Add(newOrder);
						if (ordersClosingPositionLongShort == PositionLongShort.Unknown) {
							ordersClosingPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersClosingPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersClosingAllSameDirection = false;
						}
					} else {
						orders_polarOpeningSecond.Add(newOrder);
						if (ordersOpeningPositionLongShort == PositionLongShort.Unknown) {
							ordersOpeningPositionLongShort = newOrder.Alert.PositionLongShortFromDirection;
						} else {
							if (ordersOpeningPositionLongShort != newOrder.Alert.PositionLongShortFromDirection) ordersOpeningAllSameDirection = false;
						}
					}
				} else {
					orders_polarSequenceAgnostic.Add(newOrder);
				}
			}
			if (orders_polarSequenceAgnostic.Count == 0 && (orders_polarClosingFirst.Count + orders_polarOpeningSecond.Count) == 0) {
				string msg = "NO_ORDERS_TO_SUBMIT newBornOrdersToSubmit.Count=0 while alertsBatch.Count[" + alertsBatch.Count + "]>0 "
					+ ": orders_polarSequenceAgnostic.Count[" + orders_polarSequenceAgnostic.Count + "]"
					+ " && orders_polarClosingFirst.Count[" + orders_polarClosingFirst.Count + "]"
					+   "  orders_polarOpeningSecond.Count[" + orders_polarOpeningSecond.Count + "]";
				//ALREADY_COMPLAINED Assembler.PopupException(msg, null, false);
				return orders_polarSequenceAgnostic;
			}

			if (orders_polarSequenceAgnostic.Count > 0 && (orders_polarClosingFirst.Count > 0 || orders_polarOpeningSecond.Count > 0)) {
				string msg = "got mix of orderAware/Agnostic securities in AlertsBatch"
					+ "orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] :: orders_polarClosingFirst[" + orders_polarClosingFirst.Count
					+ "] orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count+ "]";
				Assembler.PopupException(msg);
				return orders_polarSequenceAgnostic;
			}

			if (orders_polarSequenceAgnostic.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] through [" + broker + "]";
				//Assembler.PopupException(msg, null, false);
				int orderSubmitted = this.SubmitToBroker_inNewThread_waitUntilConnected(orders_polarSequenceAgnostic, broker);
				return orders_polarSequenceAgnostic;
			}
			if (orders_polarClosingFirst.Count > 0 && orders_polarOpeningSecond.Count == 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarClosingFirst[" + orders_polarClosingFirst.Count + "] through [" + broker + "]";
				//Assembler.PopupException(msg, null, false);
				int orderSubmitted = this.SubmitToBroker_inNewThread_waitUntilConnected(orders_polarClosingFirst, broker);
				return orders_polarClosingFirst;
			}
			if (orders_polarClosingFirst.Count == 0 && orders_polarOpeningSecond.Count > 0) {
				string msg = "Scheduling SubmitOrdersThreadEntry orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count + "] through [" + broker + "]";
				//Assembler.PopupException(msg, null, false);
				//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersOpening });
				int orderSubmitted = this.SubmitToBroker_inNewThread_waitUntilConnected(orders_polarOpeningSecond, broker);
				return orders_polarOpeningSecond;
			}

			if (ordersClosingAllSameDirection == true && ordersOpeningAllSameDirection == true) {
				if (ordersClosingPositionLongShort != ordersOpeningPositionLongShort) {
					this.OPPsequencer.InitializeSequence(orders_polarClosingFirst, orders_polarOpeningSecond);
					string msg = "Scheduling SubmitOrdersThreadEntry orders_polarClosingFirst[" + orders_polarClosingFirst.Count
						+ "] through [" + broker + "], then  orders_polarOpeningSecond[" + orders_polarOpeningSecond.Count + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersClosing });
					int orderSubmitted = this.SubmitToBroker_inNewThread_waitUntilConnected(orders_polarClosingFirst, broker);
					return orders_polarClosingFirst;
				} else {
					List<Order> ordersMerged = new List<Order>(orders_polarClosingFirst);
					ordersMerged.AddRange(orders_polarOpeningSecond);
					string msg = "Scheduling SubmitOrdersThreadEntry ordersMerged[" + ordersMerged.Count + "] through [" + broker + "]";
					Assembler.PopupException(msg, null, false);
					//ThreadPool.QueueUserWorkItem(new WaitCallback(broker.SubmitOrdersThreadEntry), new object[] { ordersMerged });
					int orderSubmitted = this.SubmitToBroker_inNewThread_waitUntilConnected(ordersMerged, broker);
					return ordersMerged;
				}
			}
			string msg2 = "UNTESTED_COMPLAIN DONT_PASS_NULL_AS_LIST_OF_ORDERS_TO_EVENT_CONSUMERS"
				+ " DANGEROUS MIX, NOT OPTIMIZED: Scheduling SubmitOrdersThreadEntry"
				+ " orders_polarSequenceAgnostic[" + orders_polarSequenceAgnostic.Count + "] through [" + broker + "]"
				+ " (ordersClosingAllSameDirection=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersClosingAllSameDirection=[" + ordersClosingAllSameDirection + "]"
				+ " && ordersClosingPositionLongShort[" + ordersClosingPositionLongShort + "]"
				+ " != ordersOpeningPositionLongShort[" + ordersOpeningPositionLongShort + "]) == FALSE";
			Assembler.PopupException(msg2);
			return null;
		}

		public void Emit_stopLossMove_byKillingAndSubmittingNew(PositionPrototype proto, double newActivation_negativeOffset, double newStopLoss_negativeOffset) {
			if (proto.StopLossAlert_forMoveAndAnnihilation == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move StopLoss order because proto.StopLossAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = " //OrderProcessor.Emit_stopLossMove_byKillingAndSubmittingNew("
				+ proto.StopLossActivation_negativeOffset + "/" + proto.StopLoss_negativeOffset
				+ "=>" + newActivation_negativeOffset + "/" + newStopLoss_negativeOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook oppHook_stopLossKilled = new OrderPostProcessorStateHook("StopLossGotKilledHook",
				order2killAndReplace, OrderState.KillerTransSubmittedOK,
				delegate(Order stopLossKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "oppHook_stopLossKilled(): invoking oppHook_onStopLossKilled_createNewStopLoss_andAddToPokeUnit() "
						+ "[" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + stopLossKilled.State + "]";
					stopLossKilled.AppendMessage(msg);
					this.oppHook_onStopLossKilled_createNewStopLoss_andAddToPokeUnit(stopLossKilled, newActivation_negativeOffset, newStopLoss_negativeOffset, pokeUnit);
				}
			);

			// 2. hook onActive=>kill
			OrderPostProcessorStateHook oppHook_stopLossReceived_WaitingBrokerFill_state = new OrderPostProcessorStateHook("oppHook_stopLossReceived_WaitingBrokerFill_state",
				order2killAndReplace, OrderState.WaitingBrokerFill,
				delegate(Order stopLossToBeKilled, ReporterPokeUnit pokeUnit) {
					string msg = msig + "oppHook_stopLossReceived_WaitingBrokerFill_state(): invoking Emit_killOrderPending_usingKiller() "
						+ " [" + stateBeforeActiveAssummingSubmitting + "] => "
						+ "[" + stopLossToBeKilled.State + "]";
					stopLossToBeKilled.AppendMessage(msg);
					stateBeforeKilledAssumingActive = stopLossToBeKilled.State;
					//this.Emit_killOrderPending_withoutKiller(order2killAndReplace, msig);
					bool emitted = this.Emit_killOrderPending_usingKiller(order2killAndReplace, msig);
				}
			);

			string TODO = "trigger the HOOK now against the order NOW!!! 99% chance the SL you wanna move is already ready to be changed";
			Assembler.PopupException(TODO);

			this.OPPstatusCallbacks.HookRegister(oppHook_stopLossReceived_WaitingBrokerFill_state);
			this.OPPstatusCallbacks.HookRegister(oppHook_stopLossKilled);

			this.AppendMessage_propagateToGui(proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed, msig + "hooked stopLossReceivedActiveCallback() and stopLossGotKilledHook()");
		}
		void oppHook_onStopLossKilled_createNewStopLoss_andAddToPokeUnit(Order killedStopLoss, double newActivation_negativeOffset, double newStopLoss_negativeOffset, ReporterPokeUnit pokeUnit) {
			string msig = "oppHook_onStopLossKilled_createNewStopLoss_andAddToPokeUnit(): ";
			ScriptExecutor executor = killedStopLoss.Alert.Strategy.Script.Executor;
			Position position = killedStopLoss.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new StopLossAlert for SellOrCoverRegisterAlerts()
			position.Prototype.StopLossAlert_forMoveAndAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewStopLossOffsets(newStopLoss_negativeOffset, newActivation_negativeOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateStopLoss_fromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() {replacement}, true, true);
			pokeUnit.AlertsNew.AddNoDupe_byBarsPlaced(replacement, this, "oppHook_onStopLossKilled_createNewStopLoss_andAddToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedStopLoss.AppendMessage(msg + msig);
		}
		public void Emit_takeProfitMove_byKillingAndSubmittingNew(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			if (proto.TakeProfitAlert_forMoveAndAnnihilation == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed == null) {
				string msg = "I refuse to move TakeProfit order because proto.TakeProfitAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}

			Order order2killAndReplace = proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed;

			OrderState stateBeforeActiveAssummingSubmitting = order2killAndReplace.State;
			OrderState stateBeforeKilledAssumingActive = OrderState.Unknown;

			string msig = " // OrderProcessor.Emit_takeProfitMove_byKillingAndSubmittingNew(" + proto.TakeProfit_positiveOffset + "=>" + newTakeProfit_positiveOffset + "): ";

			// 1. hook onKilled=>submitNew
			OrderPostProcessorStateHook oppHook_takeProfitKilled = new OrderPostProcessorStateHook("oppHook_takeProfitKilled",
				order2killAndReplace, OrderState.KillerTransSubmittedOK,
				delegate(Order takeProfitKilled, ReporterPokeUnit pokeUnit) {
					string msg = "oppHook_takeProfitKilled(): invoking oppHook_onTakeProfitKilled_createNewTakeProfit_addToPokeUnit() "
						+ " [" + stateBeforeKilledAssumingActive + "] => "
						+ "[" + takeProfitKilled.State + "]";
					takeProfitKilled.AppendMessage(msg + msig);
					this.oppHook_onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(takeProfitKilled, newTakeProfit_positiveOffset, pokeUnit);
				}
			);

			// 2. hook onActive=>kill
			OrderPostProcessorStateHook oppHook_takeProfitReceived_WaitingForBrokerFill = new OrderPostProcessorStateHook("oppHook_takeProfitReceived_WaitingForBrokerFill",
				order2killAndReplace, OrderState.WaitingBrokerFill,
				delegate(Order takeProfitToBeKilled, ReporterPokeUnit pokeUnit) {
					string msg = "oppHook_takeProfitReceived_WaitingForBrokerFill(): invoking Emit_killOrderPending_usingKiller() "
						+ " [" + stateBeforeActiveAssummingSubmitting + "] => "
						+ "[" + takeProfitToBeKilled.State + "]";
					takeProfitToBeKilled.AppendMessage(msg + msig);
					stateBeforeKilledAssumingActive = takeProfitToBeKilled.State;
					//this.Emit_killOrderPending_withoutKiller(order2killAndReplace, msig);
					bool emitted = this.Emit_killOrderPending_usingKiller(order2killAndReplace, msig);
				}
			);

			string TODO = "trigger the HOOK now against the order NOW!!! 99% chance the TP you wanna move is already ready to be changed";
			Assembler.PopupException(TODO);

			this.OPPstatusCallbacks.HookRegister(oppHook_takeProfitReceived_WaitingForBrokerFill);
			this.OPPstatusCallbacks.HookRegister(oppHook_takeProfitKilled);

			this.AppendMessage_propagateToGui(proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed, msig + ": hooked takeProfitReceivedActiveCallback() and takeProfitGotKilledHook()");
		}
		void oppHook_onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(Order killedTakeProfit, double newTakeProfit_positiveOffset, ReporterPokeUnit pokeUnit) {
			string msig = "oppHook_onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(): ";
			ScriptExecutor executor = killedTakeProfit.Alert.Strategy.Script.Executor;
			Position position = killedTakeProfit.Alert.PositionAffected;
			// resetting proto.SL to NULL is a legal permission to set new TakeProfitAlert for SellOrCoverRegisterAlerts()
			position.Prototype.TakeProfitAlert_forMoveAndAnnihilation = null;
			// resetting position.ExitAlert is a legal permission to for SimulateRealtimeOrderFill() to not to throw "I refuse to tryFill an ExitOrder"
			position.ExitAlert = null;
			// set new SL+SLa as new targets for Activator
			string msg = position.Prototype.ToString();
			position.Prototype.SetNewTakeProfitOffset(newTakeProfit_positiveOffset);
			msg += " => " + position.Prototype.ToString();
			Alert replacement = executor.PositionPrototypeActivator.CreateTakeProfit_fromPositionPrototype(position);
			// dont CreateAndSubmit, pokeUnit will be submitted with oneNewAlertPerState in InvokeHooksAndSubmitNewAlertsBackToBrokerAdapter();
			//this.CreateOrdersSubmitToBrokerAdapterInNewThreadGroups(new List<Alert>() { replacement }, true, true);
			pokeUnit.AlertsNew.AddNoDupe_byBarsPlaced(replacement, this, "oppHook_onTakeProfitKilled_createNewTakeProfit_addToPokeUnit(WAIT)");
			msg += " newAlert[" + replacement + "]";
			killedTakeProfit.AppendMessage(msg + msig);
		}

		public bool Emit_killOrderPending_usingKiller(Order victimOrder, string msigInvoker) {
			string msig = " //Emit_killOrderPending_usingKiller()<=" + msigInvoker;
			bool emitted = false;
			if (victimOrder == null) {
				string msg = "DONT_ANNOY_THE_KILLER_IF_YOU_DONT_WANT_TO_KILL_ANYONE victimOrder=null";
				Assembler.PopupException(msg + msig);
				return emitted;
			}
			if (victimOrder.hasBrokerAdapter(msig) == false) {
				string msg = "VICTIM_DOESNT_HAVE_BROKER " + victimOrder;
				victimOrder.AppendMessage(msg + msig);
				Assembler.PopupException(msg + msig);
				return emitted;
			}

			bool canBeKilled = OrderStatesCollections.CanBeKilled.Contains(victimOrder.State);
				//victimOrder.State == OrderState.WaitingBrokerFill ||
				//victimOrder.State == OrderState.SLAnnihilating ||
				//victimOrder.State == OrderState.TPAnnihilating;

			if (canBeKilled == false) {
				string msg = "I_REFUSE_TO_KILL__STATE_NOT_IN {" + OrderStatesCollections.CanBeKilled.ToString() + "} [" + victimOrder + "]";
				victimOrder.AppendMessage(msg + msig);
				Assembler.PopupException(msg + msig, null, false);
				return emitted;
			}

			//this.RemovePendingAlertsForVictimOrderMustBePostKill(victimOrder, msig);

			Order killerOrder_withRefToVictim = victimOrder.DeriveKillerOrder();
			this.DataSnapshot.OrderInsert_notifyGuiAsync(killerOrder_withRefToVictim);
			//this.RaiseOrderReplacementOrKillerCreatedForVictim(victimOrder);
			this.RaiseOnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(this, new List<Order>() {victimOrder});
			this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;

			string msg_victim = "YOUR_KILLER_ORDER_IS [" + killerOrder_withRefToVictim + "]";
			OrderStateMessage osm_victim = new OrderStateMessage(victimOrder, OrderState.VictimBulletFlying, msg_victim);
			//victimOrder.AppendMessageSynchronized(osm_victim);
			//this.postProcess_victimOrder(osm_victim);		// SET THE STATE!!!
			this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(osm_victim);

			string msg_killer = "YOUR_VICTIM_ORDER_IS [" + victimOrder + "]";
			OrderStateMessage osm_killer = new OrderStateMessage(killerOrder_withRefToVictim, OrderState.KillerBulletFlying, msg_killer);
			//killerOrder_withRefToVictim.AppendMessageSynchronized(osm_killer);
			//this.postProcess_killerOrder(msg_killer);		// SET THE STATE!!!
			this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(osm_killer);


			if (killerOrder_withRefToVictim.hasBrokerAdapter(msig) == false) {
				string msg = "YOU_ARE_TRYING_TO_KILL_A_DESERIALIZED_ORDER__IT_HAS_NO_BROKER_ADAPTER_RESTORED_AFTER_APPRESTART";
				Assembler.PopupException(msg);
				return emitted;
			};

			BrokerAdapter broker = killerOrder_withRefToVictim.Alert.DataSource_fromBars.BrokerAdapter;
			broker.Order_submitKiller_forPending(killerOrder_withRefToVictim);
			emitted = true;
			return emitted;
		}

		//public void Emit_killOrderPending_withoutKiller(Order orderPending, string msigInvoker) {
		//	string msig = " //Emit_killOrderPending_withoutKiller()<=" + msigInvoker;
		//	if (orderPending.State != OrderState.WaitingBrokerFill) {
		//		string msg = "I_REFUSE_TO_KILL__STATE!=WaitingBrokerFill [" + orderPending + "]";
		//		Assembler.PopupException(msg + msig, null, false);
		//		return;
		//	}

		//	this.DataSnapshot.SerializerLogrotateOrders.HasChangesToSave = true;

		//	string msgPending = "EXPECTING_THREE_MESSAGES_FROM_BROKER_FOR[" + orderPending + "] "
		//		+ OrderState.VictimsBulletSubmitting + ">"
		//		+ OrderState.VictimsBulletSubmitted + ">"
		//		+ OrderState.KillerTransSubmittedOK;

		//	if (orderPending.State == OrderState.VictimsBulletPreSubmit) {
		//		msgPending = "DUPLICATING_ORDERKILL_SEND__EXPECT_ALREADYKILLED_ERRORMSG_FROM_BROKER_ADAPTER " + msgPending;
		//		OrderStateMessage omsg_WontBePostProcessed = new OrderStateMessage(orderPending, msgPending);
		//		//WILL_COMPLAIN_ON_SAME_STATE_AND_SUGGEST_JUST_APPEND_MSG this.Order_appendPropagateMessage_updateStateIfDifferent_switchLanes___dontPostProcess(omsg_WontBePostProcessed);
		//		this.AppendOrderMessage_propagateToGui(orderPending, msgPending + msig);
		//	} else {
		//		OrderStateMessage omsg_WillNotBePostProcessed = new OrderStateMessage(orderPending, OrderState.VictimsBulletPreSubmit, msgPending);
		//		this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg_WillNotBePostProcessed);
		//	}

		//	if (orderPending.Alert.Bars == null) {
		//		string msg = "I_REFUSE_TO_KILL_PENDING__AFTER_APPRESTART orderPending.Alert.Bars=null";
		//		Assembler.PopupException(msg + msig, null, false);
		//		return;
		//	}

		//	BrokerAdapter broker = orderPending.Alert.DataSource.BrokerAdapter;
		//	try {
		//		broker.Order_killPending_withoutKiller(killerOrder);
		//	} catch (Exception ex) {
		//		string msg = "DID_YOU_IMPLEMENT_IN_YOUR_BROKER Order_killPending_withoutKiller(orderPending)?";
		//		Assembler.PopupException(msg + msig, ex, false);
		//	}
		//}
		public void Emit_oderPending_replace(Order pendingOrder, Order replacementOrder, string msigInvoker) {
			string msig = " //Emit_oderPending_replace()<=" + msigInvoker;
			string msg = "NEXT_STEP_IS_TO_KILL_SELECTED_AND_SUBMIT_REPLACEMENT [" + replacementOrder + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		public int Emit_alertsPending_kill(List<Alert> alerts2kill_afterScript_onQuote_onBar) {
			int ret = 0;
			foreach (Alert alert2kill in alerts2kill_afterScript_onQuote_onBar) {
				try {
					bool emitted = this.emit_alertPending_kill(alert2kill);
					if (emitted) ret++;
				} catch (Exception ex) {
					string msg = "YOUR_BROKER_ADAPTER_THREW_WHILE_KILLING_PENDING_ALERT";
					Assembler.PopupException(msg, ex);
				}
			}
			return ret;
		}
		bool emit_alertPending_kill(Alert alert) {
			string msig = " //OrderProcessor.Emit_alertsPending_kill(" + alert + ")";
			bool emitted = false;

			Order victim = alert.OrderFollowed;
			if (victim == null) {
				string msg = "ALERT_MUST_HAVE_ORDER_NON_NULL";
				Assembler.PopupException(msg + msig, null, false);
				return emitted;
			}
			if (victim.IsKiller) {
				string msg = "ALERT_MUST_HAVE_ORDER_NON_NULL";
				//Assembler.PopupException(msg + msig);
				return false;
			}
			if (victim.State != OrderState.WaitingBrokerFill) {
				string msg = "ALERTS_ORDER_MUST_HAVE_KILLABLE_STATUS {" + victim.State + "}MUST_BE{" + OrderState.WaitingBrokerFill + "}";
				Assembler.PopupException(msg + msig, null, false);
				return emitted;
			}

			emitted = this.Emit_killOrderPending_usingKiller(victim, msig);
			return emitted;
		}
	}
}
