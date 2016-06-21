using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sq1.Core.Accounting;
using Sq1.Core.Execution;
using Sq1.Core.Livesim;
using Sq1.Core.StrategyBase;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	public partial class OrderProcessor {
		public bool										AlwaysExitAllSharesInPosition;
		public OrderProcessorDataSnapshot				DataSnapshot					{ get; private set; }
		public OrderPostProcessorEmergency				OPPemergency					{ get; private set; }
		public OrderPostProcessorReplacerRejected		OPPrejected						{ get; private set; }
		public OrderPostProcessorReplacerExpired		OPPexpired						{ get; private set; }
		public OrderPostProcessorSequencerCloseThenOpen	OPPsequencer					{ get; private set; }
		public OrderPostProcessorStateChangedTrigger	OPPstatusCallbacks				{ get; private set; }

		public OrderProcessor() {
			this.OPPsequencer			= new OrderPostProcessorSequencerCloseThenOpen(this);
			this.OPPemergency			= new OrderPostProcessorEmergency(this, this.OPPsequencer);
			this.OPPrejected			= new OrderPostProcessorReplacerRejected(this);
			this.OPPexpired				= new OrderPostProcessorReplacerExpired(this);
			this.OPPstatusCallbacks		= new OrderPostProcessorStateChangedTrigger(this);
			this.DataSnapshot			= new OrderProcessorDataSnapshot(this);
		}

		public void Initialize(string rootPath) {
			//if (rootPath.EndsWith(Path.DirectorySeparatorChar) == false) rootPath += Path.DirectorySeparatorChar;
			this.DataSnapshot.Initialize(rootPath);
		}

		bool isExitOrderConsistent_logInconsistency(Order order) {
			bool exitOrderHasNoErrors = true;
			string errormsg = "";
			Position positionShouldBeFilled = order.Alert.PositionAffected;
			if (positionShouldBeFilled == null) {
				errormsg += "positionShouldBeFilled[" + positionShouldBeFilled + "]=null, ERROR filling order.Alert.PositionAffected !!! ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseNonStreamingPosition);
			}
			if (positionShouldBeFilled.Shares <= 0) {
				errormsg += "Shares<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (positionShouldBeFilled.EntryFilled_price <= 0) {
				errormsg += "EntryPrice<=0 for positionShouldBeFilled[" + positionShouldBeFilled + "]; skipping PositionClose ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (positionShouldBeFilled.EntryAlert == null) {
				errormsg += "EntryAlert=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (errormsg == "" && positionShouldBeFilled.EntryAlert.OrderFollowed == null) {
				errormsg += "EntryAlert.OrderFollowed=null for positionShouldBeFilled[" + positionShouldBeFilled + "]; won't close position opened in backtest closing while in streaming ";
				order.SetState_localTimeNow(OrderState.IRefuseToCloseUnfilledEntry);
			}
			if (errormsg == "") {
				//if (positionShouldBeFilled.EntryAlert.OrderFollowed.StateFilledOrPartially == false) {
				//	errormsg += "EntryAlert.OrderFollowed.State[" + positionShouldBeFilled.EntryAlert.OrderFollowed.State + "]"
				//		+ " must be [Filled] or [Partially]; skipping PositionClose"
				//		//+ " for positionShouldBeFilled[" + positionShouldBeFilled + "]"
				//		;
				//	order.State = OrderState.IRefuseToCloseUnfilledEntry;
				//}
				if (positionShouldBeFilled.EntryAlert.OrderFollowed.QtyFill != positionShouldBeFilled.EntryAlert.Qty) {
					errormsg += "EntryAlert.OrderFollowed.QtyFill[" + positionShouldBeFilled.EntryAlert.OrderFollowed.QtyFill + "]"
							+ " EntryAlert.Qty[" + positionShouldBeFilled.EntryAlert.Qty + "]"
							+ "; skipping PositionClose"
							//+ " for positionShouldBeFilled[" + positionShouldBeFilled + "]"
							;
					//order.State = OrderState.IRefuseToCloseUnfilledEntry;
				}
			}
			if (errormsg != "") {
				this.AppendMessage_propagateToGui(order, errormsg);
				exitOrderHasNoErrors = false;
			}
			return exitOrderHasNoErrors;
		}
		Order createOrder_propagateToGui_fromAlert(Alert alert, bool setStatusSubmitting, bool emittedByScript) {
			Order newborn = new Order(alert, emittedByScript, true);
			try {
				newborn.Alert.DataSource_fromBars.BrokerAdapter.Order_modifyOrderType_priceRequesting_accordingToMarketOrderAs(newborn);
			} catch (Exception ex) {
				string msg = " hoping that MarketOrderAs.MarketMinMax influenced order.Alert.MarketLimitStop["
					+ newborn.Alert.MarketLimitStop + "]=MarketLimitStop.Limit for further match; PREV=" + newborn.LastMessage;
				this.AppendMessage_propagateToGui(newborn, ex.Message + msg);
			}
			if (alert.IsExitAlert) {
				if (this.isExitOrderConsistent_logInconsistency(newborn) == false) {
					this.DataSnapshot.OrderInsert_notifyGuiAsync(newborn);
					string reason = newborn.LastMessage;
					string msg = "ALERT_INCONSISTENT_ORDER_PROCESSOR_DIDNT_SUBMIT reason[" + reason + "] " + alert;
					Assembler.PopupException(msg, null, false);

					string msg2 = "IM_USING_ALERTS_EXIT_BAR_NOW__NOT_STREAMING__DO_I_HAVE_TO_ADJUST_HERE?";
					alert.Strategy.Script.Executor.RemovePendingExitAlerts_closePositionsBacktestLeftHanging(alert);
					msg = "DID_I_CLOSE_THIS_PENDING_ALERT_HAVING_NO_LIVE_POSITION? " + alert;
					Assembler.PopupException(msg, null, false);
					return null;
				}
				//adjustExitOrderQtyRequestedToMatchEntry(order);
				alert.PositionAffected.EntryAlert.OrderFollowed.DerivedOrdersAdd(newborn);
			}

			OrderState newbornOrderState = OrderState.EmitOrdersNotClicked;
			string newbornMessage = "alert[" + alert + "]";

			if (setStatusSubmitting == true) {
				if (newborn.hasBrokerAdapter("createOrder_propagateToGui_fromAlert(): ") == false) {
					string msg = "ORDER_HAS_NO_BROKER_ADAPDER__SELECT_AND_CONFIGURE_IN_DATASOURCE_EDITOR__DLL_MIGHT_HAVE_DISAPPEARED";
					Assembler.PopupException(msg);
					return null;
				}
				newbornOrderState = this.isOrderEatable(newborn) ? OrderState.Submitting : OrderState.ErrorSubmittingNotEatable;
				//string isPastDue = newborn.Alert.IsAlertCreatedOnPreviousBar;
				//if (emittedByScript && String.IsNullOrEmpty(isPastDue) == false) {
				//	newbornMessage += "; " + isPastDue;
				//	newbornOrderState = OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted;
				//}
			}
			this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(new OrderStateMessage(newborn, newbornOrderState, newbornMessage));
			this.DataSnapshot.OrderInsert_notifyGuiAsync(newborn);
			return newborn;
		}

		public int SubmitToBroker_inNewThread_waitUntilConnected(List<Order> orders, BrokerAdapter broker, bool inNewThread = true) {
			int orderSubmitted = 0;
			string msig = " //SubmitToBroker_inNewThread_waitUntilConnected(" + broker + ")";
			if (orders.Count == 0) {
				string msg = "DONT_SUMBIT_orders.Count==0";
				Assembler.PopupException(msg);
			}
			Order firstOrder = orders[0];
			msig = " //SubmitToBroker_inNewThread_waitUntilConnected(" + firstOrder.Alert.DataSourceName + ", " + broker + ")";

			bool	wontEmit_cleanPendingAlerts = false;
			string	wontEmit_reason = "UNKNOWN_wontEmit_reason";
			try {
				if (broker.UpstreamConnected == false) {
					if (broker.UpstreamConnect_onFirstOrder) {
						broker.Broker_connect();
						broker.ConnectionState_waitFor_emittingCapable();
					} else {
						wontEmit_cleanPendingAlerts = true;
						wontEmit_reason = "CLEANING_PENDING_ALERTS UpstreamConnected==false && UpstreamConnect_onFirstOrder==false";
						Assembler.PopupException(wontEmit_reason + msig, null, false);
					}
				}
			} catch (Exception ex) {
				wontEmit_cleanPendingAlerts = true;
				wontEmit_reason = "CLEANING_PENDING_ALERTS Broker_connect()__THREW";
				Assembler.PopupException(wontEmit_reason + msig, ex);
			}

			if (wontEmit_cleanPendingAlerts) {
				ScriptExecutor executor = firstOrder.Alert.Strategy.Script.Executor;
				foreach (Order order in orders) {
					bool breakIfAbsent = true;
					int threeSeconds = ConcurrentWatchdog.TIMEOUT_DEFAULT;
					int forever = -1;
					bool removed = executor.ExecutionDataSnapshot.AlertsUnfilled.Remove(order.Alert, this, msig, forever, breakIfAbsent);

					wontEmit_reason = "ALERT_REMOVED_FROM_PENDING[" + removed + "] " + wontEmit_reason;
					OrderStateMessage osm_brokerDisconnected = new OrderStateMessage(order, OrderState.IRefuseEmitting_BrokerDisconnected, wontEmit_reason);
					this.AppendOrderMessage_propagateToGui(osm_brokerDisconnected);
				}
				return orderSubmitted;
			}

			if (inNewThread) {
				Task taskEmittingOrders = new Task(delegate {
					broker.SubmitOrders_liveAndLiveSim_fromProcessor_OPPunlockedSequence_threadEntry(orders);
				});
				taskEmittingOrders.Start();
			} else {
				broker.SubmitOrders_liveAndLiveSim_fromProcessor_OPPunlockedSequence_threadEntry(orders);
			}
			orderSubmitted = orders.Count;
			return orderSubmitted;
		}
		bool isOrderEatable(Order order) {
			if (order.Alert.Strategy == null) return true;
			if (order.IsKiller) return true;
			if (order.Alert.SellOrCover) {
				return true;
			}
			Account account = null;
			if (account == null) return true;
			if (account.CashAvailable <= 0) {
				string msg = "ACCOUNT_CASH_ZERO";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				this.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
				return false;
			}
			return true;
		}

		public void AppendOrderMessage_propagateToGui(OrderStateMessage omsg) {
			//log.Debug(omsg.Message);
			if (string.IsNullOrEmpty(omsg.Message)) {
				string msg = "I_REFUSE_TO_APPEND_AND_DISPLAY_EMPTY_MESSAGE omsg[" + omsg.ToString() + "]";
				Assembler.PopupException(msg);
				return;
			}
			Order order = omsg.Order;
			order.AppendOrderMessage(omsg);
			//v1 BEFORE_INHERITED_FROM_UserControlPeriodicFlush if (order.Alert.GuiHasTime_toRebuildReportersAndExecution == false) return;
			this.RaiseOnOrderMessageAdded_executionControlShouldPopulate_scheduled(this, omsg);
		}
		public void AppendMessage_propagateToGui(Order order, string msg) {
			if (order == null) {
				throw new Exception("order=NULL! you don't want to get NullPointerException and debug it");
			}
			OrderStateMessage omsg = new OrderStateMessage(order, msg);
			this.AppendOrderMessage_propagateToGui(omsg);
		}

		bool postProcess_victimOrder(OrderStateMessage victimNewStateOmsg) {
			bool stillTryOrderFill = false;

			Order victimOrder = victimNewStateOmsg.Order;

			bool victimAlreadyFilled = victimOrder.FilledOrPartially_inOrderMessages;
			if (victimAlreadyFilled) {
				int replacementOrderCreate_hooksUnregistered = this.OPPstatusCallbacks.HooksUnregister_Uninvoked(victimOrder, OrderState.VictimKilled);

				string msg_victim = "STATUS_DEFLECTED[" + victimNewStateOmsg.State + "]";
				if (victimNewStateOmsg.State == OrderState.VictimKilled) {
					msg_victim += " FILLED_CANT_BE_KILLED_RETARDEDLY";
				} else if (victimNewStateOmsg.State == OrderState.Filled) {
					msg_victim += " FILL_DUPLICATE";
				}
				msg_victim += " replacementOrderCreate_hooksUnregistered[" + replacementOrderCreate_hooksUnregistered + "]"
					+ " victimOrder[" + victimOrder + "]";

				//v1 will leave status as VictimBulletArrivedLate
				//this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(
				//	new OrderStateMessage(victimOrder, OrderState.VictimBulletArrivedLate, msg_victim));
				this.AppendMessage_propagateToGui(victimOrder, msg_victim);
				stillTryOrderFill = false;		// because here victimAlreadyFilled=true
				return stillTryOrderFill;		// and don't create a new replacement order (hook just removed)
			}

			// WHILE_HAVING_KILLER_FILL_IS_ASSIGNED_HERE_AND_POST_PROCESS_REMOVING_PENDING_ALERT_ISNT_INVOKED this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(victimNewStateOmsg);
			switch (victimOrder.State) {
				case OrderState.VictimBulletPreSubmit:
				case OrderState.VictimBulletSubmitted:
				case OrderState.VictimBulletConfirmed:
				case OrderState.SLAnnihilated:
				case OrderState.TPAnnihilated:
				case OrderState.Submitting:
					this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(victimNewStateOmsg);
					break;

				case OrderState.VictimBulletFlying:
					// BASTARDO_ESTAVA_AQUI
					if (victimNewStateOmsg.State == OrderState.Filled) {
						stillTryOrderFill = true;
					} else {
						this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(victimNewStateOmsg);
					}
					break;

				case OrderState.WaitingBrokerFill:
					stillTryOrderFill = true;
					break;

				case OrderState.Filled:
					string msg2 = "IM_FILLED_WHILE_BULLET_FLYING___MUST_REMOVE_FROM_PENDING_ALERTS_IN CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably()";
					this.AppendMessage_propagateToGui(victimOrder, msg2);
					stillTryOrderFill = true;
					break;

				case OrderState.VictimKilled:
					if (victimOrder.FindState_inOrderMessages(OrderState.SLAnnihilating)) {
						this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.SLAnnihilated,
								"PROTOTYPE_FILLED__COUNTERPARTY_ANNIHILATION_SUCCEEDED"));
					}
					if (victimOrder.FindState_inOrderMessages(OrderState.TPAnnihilating)) {
						this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(
							new OrderStateMessage(victimOrder, OrderState.TPAnnihilated,
								"PROTOTYPE_FILLED__COUNTERPARTY_ANNIHILATION_SUCCEEDED"));
					}

					Order killerOrder = victimOrder.KillerOrder;
					string msg_killer = "orderKiller[" + killerOrder.SernoExchange + "]=>[" + OrderState.KillerDone + "] <= orderVictim[" + victimOrder.SernoExchange + "][" + victimOrder.State + "]";
					OrderStateMessage omg_done_killer = new OrderStateMessage(killerOrder, OrderState.KillerDone, msg_killer + " //postProcess_victimOrder()");
					this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(omg_done_killer);
					double slippageNext = victimOrder.SlippageNextAvailable_NanWhenNoMore;
					if (victimOrder.DontRemoveMyPending_afterImKilled_IwillBeReplaced && double.IsNaN(slippageNext) == false) {
						string msg1 = "SKIPPING_REMOVAL_OF_PENDING_ALERT; I will remove when NoMoreSlippageAvailable";
						this.AppendMessage_propagateToGui(victimOrder, msg1);
						break;
					}
					this.BrokerCallback_pendingKilled_withKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(victimOrder, msg_killer);
					break;

				default:
					string msg = "postProcess_victimOrder() NO_HANDLER_FOR_ORDER_VICTIM [" + victimOrder + "]'s state[" + victimOrder.State + "]"
						//+ "your BrokerAdapter should call for Victim.States:{"
						//+ OrderState.KillSubmitting + ","
						//+ OrderState.VictimBulletFlying + ","
						//+ OrderState.Killed + ","
						//+ OrderState.SLAnnihilated + ","
						//+ OrderState.TPAnnihilated + "}";
						;
					Assembler.PopupException(msg, null, true);
					//throw new Exception(msg);
					break;
			}
			this.OPPstatusCallbacks.InvokeHooks_forOrderState_unregisterInvoked(victimOrder, null);
			return stillTryOrderFill;// = true
		}
		void postProcess_killerOrder(OrderStateMessage killerNewStateOmsg) {
			Order killerOrder = killerNewStateOmsg.Order;
			Order victimOrder = killerOrder.VictimToBeKilled;

			bool victimAlreadyFilled = victimOrder.FilledOrPartially_inOrderMessages;
			bool killerPossibleStates_forVictimAlreadyFilled =
				 	killerNewStateOmsg.State == OrderState.KillerTransSubmittedOK
				 || killerNewStateOmsg.State == OrderState.Submitted
				 || killerNewStateOmsg.State == OrderState.KillerBulletFlying
				 ||	killerNewStateOmsg.State == OrderState.KillerDone
				 ;
			if (victimAlreadyFilled && killerPossibleStates_forVictimAlreadyFilled) {
				string msg_killer = "STATUS_DEFLECTED[" + killerNewStateOmsg.State + "] FILLED_CANT_BE_KILLED_RETARDEDLY victimOrder.State[" + victimOrder.State + "]"
					+ " orderKiller[" + killerOrder + "]";
				//v1 will leave status as VictimBulletArrivedLate
				this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(
				    new OrderStateMessage(killerOrder, OrderState.KillerBulletArrivedLate, msg_killer));
				//this.AppendMessage_propagateToGui(killerOrder, msg_killer);
				return;		// and don't create a new replacement order (hook just removed)
			}

			switch (killerOrder.State) {
				case OrderState.JustConstructed:
				case OrderState.KillerPreSubmit:
				case OrderState.KillerSubmitting:
				case OrderState.KillerTransSubmittedOK:
				case OrderState.KillerBulletFlying:
				case OrderState.Submitted:
				case OrderState.KillerDone:
					this.BrokerCallback_orderStateUpdate_mustBeDifferent_dontPostProcess(killerNewStateOmsg);
					break;

				default:
					string msg = "postProcess_killerOrder(): NO_HANDLER_FOR_KILLE_ORDER [" + killerOrder + "]'s state[" + killerOrder.State + "]"
						+ "your BrokerAdapter should call for Killer.States:{"
						+ OrderState.KillerTransSubmittedOK + ","
						+ OrderState.KillerBulletFlying + ","
						+ OrderState.KillerDone + "}";
					break;
					//throw new Exception(msg);
			}
		}

		void postProcess_invokeScriptCallback(Order order, double priceFill, double qtyFill) {
			string msig = " " + order.State + " " + order.LastMessage + " //postProcess_invokeScriptCallback()";
			//if (order.Alert.isExitAlert || order.IsEmergencyClose) {
			//	order.State = OrderState.Rejected;
			//}
			//if (order.Alert.isEntryAlert && order.State == OrderState.Rejected) {
			//	order.State = OrderState.Filled;
			//}
			if (OrderStatesCollections.NoInterventionRequired.Contains(order.State) == false) {
				this.DataSnapshot.OrdersExpectingBrokerUpdateCount_notUsed--;
			}

			switch (order.State) {
				case OrderState.Filled:
				case OrderState.FilledPartially:
					double slippageByFact = 0;
					// you should save it to SlippageEffective! calc "implied" slippage from executed price, instead of assumed for LimitCrossMarket
					if (order.SlippageApplied == 0
						// && order.Alert.MarketLimitStop == MarketLimitStop.Market
						// && order.Alert.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker
							) {
						if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
							slippageByFact = priceFill - order.CurrentBid;
							if (slippageByFact < 0) {
								string msg = "do you really want a negative slippage?";
							}
						} else {
							slippageByFact = priceFill - order.CurrentAsk;
							if (slippageByFact < 0) {
								string msg = "do you really want a negative slippage?";
							}
						}
					}

					order.FillWith(priceFill, qtyFill, slippageByFact);
					this.postProcessAccounting(order);

					if (order.IsEmergencyClose) {
						this.OPPemergency.RemoveEmergencyLockFilled(order);
					}
					this.OPPsequencer.OrderFilled_unlockSequence_submitOpening(order);
					try {
						order.Alert.Strategy.Script.Executor.CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably(order.Alert, null,
							order.PriceFilled, order.QtyFill, order.SlippageFilled, order.CommissionFill);
					} catch (Exception ex) {
						string msg3 = "PostProcessOrderState caught from CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably() ";
						Assembler.PopupException(msg3 + msig, ex);
					}

					if (order.PriceFilled > 0 && this.DataSnapshot.OrdersPending.ContainsGuid(order)) {
						string msg = "FILLED_ORDER_MUST_DISAPPEAR_FROM_OrdersPending";
						Assembler.PopupException(msg);
					}
					break;

				case OrderState.ErrorSubmittingOrder_elaborate:
					string msg5 = "ErrorSubmittingOrder_elaborate";
					Assembler.PopupException(msg5 + msig);
					break;

				case OrderState.ErrorCancelReplace:
					this.DataSnapshot.OrdersRemoveRange_fromAllLanes(new List<Order>() { order });
					this.RaiseOnOrdersRemoved_executionControlShouldRebuildOLV_scheduled(this, new List<Order>(){order});
					Assembler.PopupException(msig);
					break;

				case OrderState.Error:
				case OrderState.ErrorMarketPriceZero:
				case OrderState.Error_DealPriceOutOfLimit_weird:
				case OrderState.Error_accountIsTooSmall:
				case OrderState.ErrorSlippageCalc:
					Assembler.PopupException("PostProcess(): order.PriceFill=0 " + msig, null, false);
					//NEVER order.PricePaid = 0;
					if (order.PriceFilled > 0) {
						string msg = "!!! ABNORMAL#1 ZEROIFYING_PriceFilled[" + order.PriceFilled + "]=>0";
						this.AppendMessage_propagateToGui(order, msg);
						order.FillWith(0);
					}

					try {
						//DOESNT_EXPECT_PRICE=0
						//order.Alert.Strategy.Script.Executor.CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably(order.Alert, null,
						//	order.PriceFilled, order.QtyFill, order.SlippageFilled, order.CommissionFill);
						order.Alert.Strategy.Script.Executor.CallbackOrderError(order);
					} catch (Exception ex) {
						string msg3 = "PostProcessOrderState caught from CallbackAlertFilledMoveAroundInvokeScript() ";
						Assembler.PopupException(msg3 + msig, ex);
					}
					break;

				case OrderState.ErrorSubmitting_BrokerTerminalDisconnected:
				case OrderState.LimitExpired:
				case OrderState.Rejected:
				case OrderState.RejectedLimitReached:
					//bool a = order.IsEmergencyClose;
					bool replacementOrder	= string.IsNullOrEmpty(order.EmergencyReplacedByGUID) == false;
					bool orderBeingReplaced	= string.IsNullOrEmpty(order.ReplacedByGUID) == false;
					if (replacementOrder || orderBeingReplaced) {
						string msg = "";
						if (replacementOrder) msg += " BrokerAdapter CALLBACK DUPE: Rejected was already replaced by"
							+ " EmergencyReplacedByGUID[" + order.EmergencyReplacedByGUID + "]"
							//+ "; skipping PostProcess for [" + order + "]"
							;
						if (orderBeingReplaced) msg += " BrokerAdapter CALLBACK DUPE: Rejected was already replaced by"
							+ " ReplacedByGUID[" + order.ReplacedByGUID + "]"
							//+ "; skipping PostProcess for [" + order + "]"
							;
						this.AppendMessage_propagateToGui(order, msg);
						this.RaiseOnOrderStateOrPropertiesChanged_executionControlShouldPopulate_immediately(this, new List<Order>(){order});
						return;
					}
					
					//DONT_LIE Assembler.PopupException("REJECTED_BY_BROKER__FIRST_TIME_NEVER_REPLACED: order.PriceFill=0 " + msig, null, false);
					//NEVER order.PricePaid = 0;
					if (order.PriceFilled > 0) {
						string msg = "!!! ABNORMAL#1 ZEROIFYING_PriceFilled[" + order.PriceFilled + "]=>0";
						this.AppendMessage_propagateToGui(order, msg);
						order.FillWith(0);
					}

					try {
						//DOESNT_EXPECT_PRICE=0
						//order.Alert.Strategy.Script.Executor.CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably(order.Alert, null,
						//	order.PriceFilled, order.QtyFill, order.SlippageFilled, order.CommissionFill);
						order.Alert.Strategy.Script.Executor.CallbackOrderExpired(order);
					} catch (Exception ex) {
						string msg3 = "I_FAILED_TO_REMOVE_FROM_AlertsPending_FOR_REJECTED_ORDER CallbackAlertFilled_moveAround_invokeScriptCallback_nonReenterably() ";
						Assembler.PopupException(msg3 + msig, ex);
					}

					if (order.IsEmergencyClose) {
						this.OPPemergency.CreateEmergencyReplacement_resubmitFor(order);
					} else {
						//if (order.Alert.IsExitAlert) {
						//	this.OPPemergency.AddLockAndCreate_emergencyReplacement_resubmitFor(order);
						//} else {
							this.OPPrejected.ReplaceRejected_ifResubmitRejected_setInSymbolInfo(order);
						//}
					}
					break;

				case OrderState.Submitting:
					string msg2 = "all Orders.State!=Submitting aren't sent to BrokerAdapter;"
						+ " we shouldn't be here in a broker-originated State change handler...";
					break;
				case OrderState.SubmittingSequenced:
				case OrderState.Submitted:
				case OrderState.SubmittedNoFeedback:
					break;

				case OrderState.WaitingBrokerFill:
					bool scheduled = this.OPPexpired.ScheduleReplace_ifExpired(order);
					break;

				case OrderState.IRefuseOpenTillEmergencyCloses:
				case OrderState.IRefuseToCloseNonStreamingPosition:
				case OrderState.IRefuseToCloseUnfilledEntry:
					break;

				case OrderState.EmergencyCloseSheduledForErrorSubmittingBroker:
				case OrderState.EmergencyCloseSheduledForNoReason:
				case OrderState.EmergencyCloseSheduledForRejected:
				case OrderState.EmergencyCloseSheduledForRejectedLimitReached:
				case OrderState.EmergencyCloseComplete:
				case OrderState.EmergencyCloseLimitReached:
				case OrderState.EmergencyCloseUserInterrupted:
					break;

				case OrderState.PreSubmit:
				case OrderState.VictimBulletPreSubmit:
				case OrderState.VictimBulletFlying:
				case OrderState.KillerPreSubmit:
					break;

				case OrderState.JustConstructed:
					break;

				case OrderState._TransactionStatus:	// for Market, Limit, killers
				case OrderState._TradeStatus:		// for Market
				case OrderState._OrderStatus:		// for Limit
					break;

				default:
					string msg4 = "NO_HANDLER_FOR_order.State[" + order.State + "]";
					Assembler.PopupException(msg4 + msig, null, true);
					break;
			}
		}
		public override string ToString() {
			return this.DataSnapshot.ToString();
		}
	}
}
