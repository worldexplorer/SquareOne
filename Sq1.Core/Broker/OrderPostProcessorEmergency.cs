using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorEmergency {
		List<OrderPostProcessorEmergencyLock>		emergencyLocks;
		List<Order>									interruptedEmergencyLockReasons;
		OrderProcessor								orderProcessor;
		OrderPostProcessorSequencerCloseThenOpen	OPPsequencer;

		public OrderPostProcessorEmergency(OrderProcessor orderProcessor, OrderPostProcessorSequencerCloseThenOpen OPPSequencer) {
			this.emergencyLocks = new List<OrderPostProcessorEmergencyLock>();
			this.interruptedEmergencyLockReasons = new List<Order>();
			this.orderProcessor = orderProcessor;
			this.OPPsequencer = OPPSequencer;
		}

		public Order GetReasonForLock(Order order2check4similarLock) {
			Order ret = null;
			OrderPostProcessorEmergencyLock lock4check = new OrderPostProcessorEmergencyLock(order2check4similarLock);
			lock (this.emergencyLocks) {
				//bool contains = EmergencyLocks.Contains(orderGenerator);
				//bool ret = false;
				foreach (OrderPostProcessorEmergencyLock locks in this.emergencyLocks) {
					if (locks.Equals(lock4check)) ret = locks.OrderReasonForLock;
				}
				//if (contains != ret) throw new Exception("contains[" + contains + "] != ret[" + ret + "]");
			}
			return ret;
		}
		public void RemoveEmergencyLock_userInterrupted(Order reason4lock) {
			if (this.GetReasonForLock(reason4lock) == null) {
				throw new Exception("CRAZY#54 I was called when the reason4lock still existed?... reason4lock=" + reason4lock);
			}
			lock (this.interruptedEmergencyLockReasons) {
				this.interruptedEmergencyLockReasons.Add(reason4lock);
			}
			this.removeEmergencyLock(reason4lock, OrderState.EmergencyCloseUserInterrupted);
		}
		public void RemoveEmergencyLockFilled(Order filledEmergencyOrder) {
			this.removeEmergencyLock(filledEmergencyOrder, OrderState.EmergencyCloseComplete);
		}
		void removeEmergencyLock(Order filledEmergencyOrder, OrderState stateCompletedOrInterrupted) {
			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(filledEmergencyOrder);
			string msgPost = "EmergencyLock Removed [" + emergencyLock + "]";
			if (this.emergencyLocks.Contains(emergencyLock) == false) {
				string msg = "no EmergencyLock to remove: multiple QUIK callbacks? if u can find [" + msgPost
					+ "] earlier in thisOrder.Messages then it's ok";
				this.orderProcessor.AppendMessage_propagateToGui(filledEmergencyOrder, msg);
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return;
			}

			lock (this.emergencyLocks) {
				this.emergencyLocks.Remove(emergencyLock);
			}
			OrderStateMessage omsgPost = new OrderStateMessage(filledEmergencyOrder, stateCompletedOrInterrupted, msgPost);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsgPost);
		}
		public void AddLockAndCreate_emergencyReplacement_resubmitFor(Order rejectedExitOrder) {
			int emergencyCloseAttemptsMax = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
			if (emergencyCloseAttemptsMax <= 0) return;

			try {
				throwLog_ifNotRejected_closingOrder(rejectedExitOrder);
			} catch (Exception) {
				return;
			}

			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(rejectedExitOrder);
			string msg = "Setting an EmergencyLock[" + emergencyLock.ToString() + "]";
			Order reason4lock = this.GetReasonForLock(rejectedExitOrder);
			bool isEmergencyClosingNow = (reason4lock != null);
			if (isEmergencyClosingNow) {
				msg = "ALREADY LOCKED (Rejected dupe?): " + msg;
				this.orderProcessor.AppendMessage_propagateToGui(rejectedExitOrder, msg);
				return;
			}

			lock (this.emergencyLocks) {
				this.emergencyLocks.Add(emergencyLock);
			}
			this.CreateEmergencyReplacement_resubmitFor(rejectedExitOrder);
		}
		public void CreateEmergencyReplacement_resubmitFor(Order rejectedExitOrderOrEmergencyCloseOrder) {
			try {
				throwLog_ifNotRejected_closingOrder(rejectedExitOrderOrEmergencyCloseOrder);
				throwLogAndAppendMessage_ifNoEmergencyLockFor(rejectedExitOrderOrEmergencyCloseOrder);
				throwLogAndAppendMessage_ifNextAttemptReachesLimit(rejectedExitOrderOrEmergencyCloseOrder);
				throwLog_ifEmergencyCloseInterrupted(rejectedExitOrderOrEmergencyCloseOrder);
			} catch (Exception) {
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.closeEmergency_threadWrapper), new object[] { rejectedExitOrderOrEmergencyCloseOrder });
			//CloseEmergencyThreadEntry(rejectedExitOrder);
		}
		void closeEmergency_threadWrapper(Object order_array) {
			object[] array = order_array as object[];
			Order rejectedExitOrder = (Order)array[0];
			try {
				this.closeEmergency_threadEntry(rejectedExitOrder);
			} catch (Exception e) {
				Assembler.PopupException("CloseEmergencyThreadWrapper(): " + e.ToString(), e);
			}
		}
		void closeEmergency_threadEntry(Order rejectedExitOrder) {
			try {
				throwLog_ifNotRejected_closingOrder(rejectedExitOrder);
				throwLogAndAppendMessage_ifNoEmergencyLockFor(rejectedExitOrder);
				throwLogAndAppendMessage_ifNextAttemptReachesLimit(rejectedExitOrder);
			} catch (Exception) {
				return;
			}

			OrderState newState = rejectedExitOrder.InState_errorOrRejected_convertToComplementaryEmergencyState;
			string changeState = "ExitOrderCriticalState[" + rejectedExitOrder.State + "]=>[" + newState + "]";

			int millis = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseInterAttemptDelayMillis;
			if (millis > 0) {
				string msg = "Emergency sleeping millis[" + millis + "] before " + changeState;
				OrderStateMessage omsg = new OrderStateMessage(rejectedExitOrder, newState, msg);
				this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
				Thread.Sleep(millis);
			}

			string msg2 = changeState + " after having slept millis[" + millis + "]";
			if (rejectedExitOrder.State == newState) {
				// announced "sleeping xxx before"
				this.orderProcessor.AppendMessage_propagateToGui(rejectedExitOrder, msg2);
			} else {
				// didnt announce "sleeping xxx before"
				OrderStateMessage omsg2 = new OrderStateMessage(rejectedExitOrder, newState, msg2);
				this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg2);
			}
			this.submitReplacementOrderFor(rejectedExitOrder);
		}
		void submitReplacementOrderFor(Order rejectedOrderToReplace) {
			Order replacement = this.createEmergencyCloseOrder_insteadOfRejected(rejectedOrderToReplace);
			if (replacement == null) {
				string msgNoReplacement = "got NULL from CreateEmergencyCloseOrderInsteadOfRejected() for (" + rejectedOrderToReplace + "); ";
				Assembler.PopupException(msgNoReplacement);
				this.orderProcessor.AppendMessage_propagateToGui(rejectedOrderToReplace, msgNoReplacement);
				return;
			}

			try {
				this.OPPsequencer.ReplaceLockingCloseOrder(rejectedOrderToReplace, replacement);

				double priceScript = replacement.Alert.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot
					.GetBidOrAsk_aligned_forTidalOrCrossMarket_fromQuoteLast(
					replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, true);
				replacement.Alert.PositionAffected.ExitEmitted_price = priceScript;

				if (replacement.Alert.Bars.SymbolInfo.ReSubmittedUsesNextSlippage == true) {
					replacement.SlippageAppliedIndex++;
				}

				int emergencyCloseAttemptsMax = replacement.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
				string serno = "#[" + replacement.EmergencyCloseAttemptSerno + "]/[" + emergencyCloseAttemptsMax + "]";
				string msg_replacement = "This is an EMERGENCY replacement " + serno + " for order["
					+ replacement.EmergencyReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageAppliedIndex + "]";
				this.orderProcessor.AppendMessage_propagateToGui(replacement, msg_replacement);

				if (replacement.HasSlippagesDefined && replacement.SlippagesLeftAvailable_noMore) {
					addMessage_noMoreSlippagesAvailable(replacement);
					//replacement.SlippageAppliedIndex = replacement.Alert.Bars.SymbolInfo.GetSlippage_maxIndex_forLimitOrdersOnly(replacement.Alert);
					replacement.SlippageAppliedIndex = replacement.Alert.Slippage_maxIndex_forLimitOrdersOnly;
				}
				//double slippage = replacement.Alert.Bars.SymbolInfo.GetSlippage_signAware_forLimitOrdersOnly(
				//	priceScript, replacement.Alert.Direction, replacement.Alert.MarketOrderAs, replacement.SlippageAppliedIndex);
				double slippageNext_NaNunsafe = replacement.Alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(replacement.SlippageAppliedIndex);
				replacement.SlippageApplied = slippageNext_NaNunsafe;
				replacement.PriceEmitted = priceScript + slippageNext_NaNunsafe;

				string msg = "Scheduling SubmitOrdersThreadEntry [" + replacement.ToString() + "] slippageIndex["
					+ replacement.SlippageAppliedIndex + "] through [" + replacement.Alert.DataSource_fromBars.BrokerAdapter + "]";
				OrderStateMessage omsg = new OrderStateMessage(replacement, OrderState.PreSubmit, msg);
				this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);

				//ThreadPool.QueueUserWorkItem(new WaitCallback(replacement.Alert.DataSource.BrokerAdapter.SubmitOrdersThreadEntry),
				//	new object[] { new List<Order>() { replacement } });
				List<Order> replacementOrder_oneInTheList = new List<Order>() { replacement };
				BrokerAdapter broker = replacement.Alert.DataSource_fromBars.BrokerAdapter;
				int orderSubmitted = this.orderProcessor.SubmitToBroker_waitForConnected(replacementOrder_oneInTheList, broker);
			} catch (Exception e) {
				Assembler.PopupException("Replacement wasn't submitted [" + replacement + "]", e);
				OrderStateMessage omsg2 = new OrderStateMessage(replacement, OrderState.Error, e.Message);
				this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg2);
			}
		}
		Order createEmergencyCloseOrder_insteadOfRejected(Order rejectedOrderToReplace) {
			if (rejectedOrderToReplace == null) {
				Assembler.PopupException("rejectedOrderToReplace=null why did you call me?");
				return null;
			}
			Order emergencyReplacement = this.findEmergencyReplacement_forRejectedOrder(rejectedOrderToReplace);
			if (emergencyReplacement != null) {
				string msg = "Rejected[" + rejectedOrderToReplace + "] already has a"
					+ " emergencyReplacement[" + emergencyReplacement + "] with State[" + emergencyReplacement.State + "];"
					+ " ignoring rejection duplicates";
				this.orderProcessor.AppendMessage_propagateToGui(rejectedOrderToReplace, msg);
				return null;
			}
			if (rejectedOrderToReplace.hasBrokerAdapter("CreateEmergencyCloseOrderInsteadOfRejected(): ") == false) {
				return null;
			}
			emergencyReplacement = rejectedOrderToReplace.DeriveReplacementOrder();
			// for a first emergencyReplacement, set slippage=0;
			if (string.IsNullOrEmpty(rejectedOrderToReplace.EmergencyReplacementForGUID)) emergencyReplacement.SlippageAppliedIndex = 0;
			rejectedOrderToReplace.EmergencyReplacedByGUID = emergencyReplacement.GUID;
			emergencyReplacement.EmergencyReplacementForGUID = rejectedOrderToReplace.GUID;
			emergencyReplacement.IsEmergencyClose = true;
			if (rejectedOrderToReplace.IsEmergencyClose) {
				emergencyReplacement.EmergencyCloseAttemptSerno = rejectedOrderToReplace.EmergencyCloseAttemptSerno + 1;
			} else {
				emergencyReplacement.EmergencyCloseAttemptSerno = 1;
			}
			DateTime serverTimeNow = rejectedOrderToReplace.Alert.Bars.MarketInfo.ServerTimeNow;
			emergencyReplacement.CreatedBrokerTime = serverTimeNow;

			this.orderProcessor.DataSnapshot.OrderInsert_notifyGuiAsync(emergencyReplacement);
			this.orderProcessor.RaiseOrderStateOrPropertiesChanged_executionControlShouldPopulate(this, new List<Order>(){rejectedOrderToReplace});
	
			return emergencyReplacement;
		}
		Order findEmergencyReplacement_forRejectedOrder(Order orderRejected) {
			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			Order rejected = this.orderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(orderRejected.GUID, out suggestedLane, out suggestion, true);
			if (rejected == null) {
				throw new Exception("OrderRejected[" + orderRejected + "] wasn't found!!! suggestion[" + suggestion + "]");
			}
			Order replacement = this.orderProcessor.DataSnapshot.OrdersAll.ScanRecent_forGuid(rejected.EmergencyReplacedByGUID, out suggestedLane, out suggestion, true);
			return replacement;
		}
		void throwLog_ifNotRejected_closingOrder(Order order) {
			if (order.Alert.IsEntryAlert == true) {
				string msg = "Direction=[" + order.Alert.Direction + "] is not a Sell/Cover; order[" + order + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (order.InState_changeableToEmergency == false) {
				string msg = "State=[" + order.State + "] is not a Rejected/Error*; order[" + order + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
		}
		void throwLogAndAppendMessage_ifNoEmergencyLockFor(Order order) {
			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(order);
			if (this.emergencyLocks.Contains(emergencyLock) == false) {
				string msg = "who removed EmergencyLock before EmergencyCloseComplete?! " + emergencyLock.ToString();
				this.orderProcessor.AppendMessage_propagateToGui(order, msg);
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
		}
		void throwLogAndAppendMessage_ifNextAttemptReachesLimit(Order rejectedExitOrder) {
			if (rejectedExitOrder.IsEmergencyClose == false) return;
			int emergencyCloseAttemptsMax = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
			if (rejectedExitOrder.EmergencyCloseAttemptSerno + 1 < emergencyCloseAttemptsMax) return;
			string msg = "no more EmergencyCloseAttempts:"
				+ " EmergencyCloseAttemptSerno[" + rejectedExitOrder.EmergencyCloseAttemptSerno
				+ "]>= EmergencyCloseAttemptsMax[" + emergencyCloseAttemptsMax + "]"
				+ " emergencyReplacement[" + rejectedExitOrder + "]";
			OrderStateMessage omsg = new OrderStateMessage(rejectedExitOrder, OrderState.EmergencyCloseLimitReached, msg);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
			throw new Exception(msg);
		}
		void throwLog_ifEmergencyCloseInterrupted(Order replacementOrder) {
			Order reason4lock = this.GetReasonForLock(replacementOrder);
			lock (this.interruptedEmergencyLockReasons) {
				if (this.interruptedEmergencyLockReasons.Contains(reason4lock) == false) return;
			}
			string msg = "InterruptedEmergencyLockReasons.Contains reason4lock[" + reason4lock + "] for replacementOrder[" + replacementOrder + "]";
			Assembler.PopupException(msg);
			OrderStateMessage newOrderStateRejected = new OrderStateMessage(replacementOrder, OrderState.EmergencyCloseUserInterrupted, msg);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderStateRejected);
			throw new Exception(msg);
		}
		void addMessage_noMoreSlippagesAvailable(Order order) {
			int slippageIndexMax = order.Alert.Slippage_maxIndex_forLimitOrdersOnly;
			string msg2 = "EMERGENCY Reached max slippages available for [" + order.Alert.Bars.Symbol + "]"
				+ " order.SlippageIndex[" + order.SlippageAppliedIndex + "] > slippageIndexMax[" + slippageIndexMax + "]"
				+ "; Order will have slippageIndexMax[" + slippageIndexMax + "]";
			Assembler.PopupException(msg2);
			//orderProcessor.updateOrderStatusError(orderExecuted, OrderState.RejectedLimitReached, msg2);
			OrderStateMessage newOrderStateRejected = new OrderStateMessage(order, OrderState.RejectedLimitReached, msg2);
			this.orderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderStateRejected);
		}
	}
}
