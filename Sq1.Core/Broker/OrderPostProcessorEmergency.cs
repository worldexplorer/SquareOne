using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderPostProcessorEmergency {
		private List<OrderPostProcessorEmergencyLock> emergencyLocks;
		private List<Order> interruptedEmergencyLockReasons;
		private OrderProcessor orderProcessor;
		private OrderPostProcessorSequencerCloseThenOpen OPPsequencer;

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
		public void RemoveEmergencyLockUserInterrupted(Order reason4lock) {
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
		private void removeEmergencyLock(Order filledEmergencyOrder, OrderState stateCompletedOrInterrupted) {
			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(filledEmergencyOrder);
			string msgPost = "EmergencyLock Removed [" + emergencyLock + "]";
			if (this.emergencyLocks.Contains(emergencyLock) == false) {
				string msg = "no EmergencyLock to remove: multiple QUIK callbacks? if u can find [" + msgPost
					+ "] earlier in thisOrder.Messages then it's ok";
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(filledEmergencyOrder, msg);
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return;
			}

			lock (this.emergencyLocks) {
				this.emergencyLocks.Remove(emergencyLock);
			}
			OrderStateMessage omsgPost = new OrderStateMessage(filledEmergencyOrder, stateCompletedOrInterrupted, msgPost);
			this.orderProcessor.UpdateOrderStateAndPostProcess(filledEmergencyOrder, omsgPost);
		}
		public void AddLockAndCreateEmergencyReplacementAndResubmitFor(Order rejectedExitOrder) {
			int emergencyCloseAttemptsMax = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
			if (emergencyCloseAttemptsMax <= 0) return;

			try {
				throwLogIfNotRejectedClosingOrder(rejectedExitOrder);
			} catch (Exception) {
				return;
			}

			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(rejectedExitOrder);
			string msg = "Setting an EmergencyLock[" + emergencyLock.ToString() + "]";
			Order reason4lock = this.GetReasonForLock(rejectedExitOrder);
			bool isEmergencyClosingNow = (reason4lock != null);
			if (isEmergencyClosingNow) {
				msg = "ALREADY LOCKED (Rejected dupe?): " + msg;
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(rejectedExitOrder, msg);
				return;
			}

			lock (this.emergencyLocks) {
				this.emergencyLocks.Add(emergencyLock);
			}
			CreateEmergencyReplacementAndResubmitFor(rejectedExitOrder);
		}
		public void CreateEmergencyReplacementAndResubmitFor(Order rejectedExitOrderOrEmergencyCloseOrder) {
			try {
				throwLogIfNotRejectedClosingOrder(rejectedExitOrderOrEmergencyCloseOrder);
				throwLogAndAppendMessageIfNoEmergencyLockFor(rejectedExitOrderOrEmergencyCloseOrder);
				throwLogAndAppendMessageIfNextAttemptReachesLimit(rejectedExitOrderOrEmergencyCloseOrder);
				throwLogIfEmergencyCloseInterrupted(rejectedExitOrderOrEmergencyCloseOrder);
			} catch (Exception) {
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.closeEmergencyThreadWrapper), new object[] { rejectedExitOrderOrEmergencyCloseOrder });
			//CloseEmergencyThreadEntry(rejectedExitOrder);
		}
		private void closeEmergencyThreadWrapper(Object order_array) {
			object[] array = order_array as object[];
			Order rejectedExitOrder = (Order)array[0];
			try {
				this.closeEmergencyThreadEntry(rejectedExitOrder);
			} catch (Exception e) {
				Assembler.PopupException("CloseEmergencyThreadWrapper(): " + e.ToString(), e);
			}
		}
		private void closeEmergencyThreadEntry(Order rejectedExitOrder) {
			try {
				throwLogIfNotRejectedClosingOrder(rejectedExitOrder);
				throwLogAndAppendMessageIfNoEmergencyLockFor(rejectedExitOrder);
				throwLogAndAppendMessageIfNextAttemptReachesLimit(rejectedExitOrder);
			} catch (Exception) {
				return;
			}

			OrderState newState = rejectedExitOrder.ComplementaryEmergencyStateForError;
			string changeState = "ExitOrderCriticalState[" + rejectedExitOrder.State + "]=>[" + newState + "]";

			int millis = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseDelayMillis;
			if (millis > 0) {
				string msg = "Emergency sleeping millis[" + millis + "] before " + changeState;
				OrderStateMessage omsg = new OrderStateMessage(rejectedExitOrder, newState, msg);
				this.orderProcessor.UpdateOrderStateAndPostProcess(rejectedExitOrder, omsg);
				Thread.Sleep(millis);
			}

			string msg2 = changeState + " after having slept millis[" + millis + "]";
			if (rejectedExitOrder.State == newState) {
				// announced "sleeping xxx before"
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(rejectedExitOrder, msg2);
			} else {
				// didnt announce "sleeping xxx before"
				OrderStateMessage omsg2 = new OrderStateMessage(rejectedExitOrder, newState, msg2);
				this.orderProcessor.UpdateOrderStateAndPostProcess(rejectedExitOrder, omsg2);
			}
			this.submitReplacementOrderFor(rejectedExitOrder);
		}
		private void submitReplacementOrderFor(Order rejectedOrderToReplace) {
			Order replacement = this.createEmergencyCloseOrderInsteadOfRejected(rejectedOrderToReplace);
			if (replacement == null) {
				string msgNoReplacement = "got NULL from CreateEmergencyCloseOrderInsteadOfRejected() for (" + rejectedOrderToReplace + "); ";
				Assembler.PopupException(msgNoReplacement);
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(rejectedOrderToReplace, msgNoReplacement);
				return;
			}

			try {
				this.OPPsequencer.ReplaceLockingCloseOrder(rejectedOrderToReplace, replacement);

				double priceScript = replacement.Alert.DataSource.StreamingProvider.StreamingDataSnapshot
					.GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(
					replacement.Alert.Bars.Symbol, replacement.Alert.Direction, out replacement.SpreadSide, true);
				replacement.Alert.PositionAffected.ExitPriceScript = priceScript;

				if (replacement.Alert.Bars.SymbolInfo.ReSubmittedUsesNextSlippage == true) {
					replacement.SlippageIndex++;
				}

				int emergencyCloseAttemptsMax = replacement.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
				string serno = "#[" + replacement.EmergencyCloseAttemptSerno + "]/[" + emergencyCloseAttemptsMax + "]";
				string msg_replacement = "This is an EMERGENCY replacement " + serno + " for order["
					+ replacement.EmergencyReplacementForGUID + "]; SlippageIndex[" + replacement.SlippageIndex + "]";
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(replacement, msg_replacement);

				if (replacement.hasSlippagesDefined && replacement.noMoreSlippagesAvailable) {
					addMessageNoMoreSlippagesAvailable(replacement);
					replacement.SlippageIndex = replacement.Alert.Bars.SymbolInfo.getSlippageIndexMax(replacement.Alert.Direction);
				}
				double slippage = replacement.Alert.Bars.SymbolInfo.getSlippage(
					priceScript, replacement.Alert.Direction, replacement.SlippageIndex, false, false);
				replacement.SlippageFill = slippage;
				replacement.PriceRequested = priceScript + slippage;

				string msg = "Scheduling SubmitOrdersThreadEntry [" + replacement.ToString() + "] slippageIndex["
					+ replacement.SlippageIndex + "] through [" + replacement.Alert.DataSource.BrokerProvider + "]";
				OrderStateMessage omsg = new OrderStateMessage(replacement, OrderState.PreSubmit, msg);
				this.orderProcessor.UpdateOrderStateAndPostProcess(replacement, omsg);

				ThreadPool.QueueUserWorkItem(new WaitCallback(replacement.Alert.DataSource.BrokerProvider.SubmitOrdersThreadEntry),
					new object[] { new List<Order>() { replacement } });
			} catch (Exception e) {
				Assembler.PopupException("Replacement wasn't submitted [" + replacement + "]", e);
				OrderStateMessage omsg2 = new OrderStateMessage(replacement, OrderState.Error, e.Message);
				this.orderProcessor.UpdateOrderStateAndPostProcess(replacement, omsg2);
			}
		}
		private Order createEmergencyCloseOrderInsteadOfRejected(Order rejectedOrderToReplace) {
			if (rejectedOrderToReplace == null) {
				Assembler.PopupException("rejectedOrderToReplace=null why did you call me?");
				return null;
			}
			Order emergencyReplacement = this.findEmergencyReplacementForRejectedOrder(rejectedOrderToReplace);
			if (emergencyReplacement != null) {
				string msg = "Rejected[" + rejectedOrderToReplace + "] already has a"
					+ " emergencyReplacement[" + emergencyReplacement + "] with State[" + emergencyReplacement.State + "];"
					+ " ignoring rejection duplicates";
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(rejectedOrderToReplace, msg);
				return null;
			}
			if (rejectedOrderToReplace.hasBrokerProvider("CreateEmergencyCloseOrderInsteadOfRejected(): ") == false) {
				return null;
			}
			emergencyReplacement = rejectedOrderToReplace.DeriveReplacementOrder();
			// for a first emergencyReplacement, set slippage=0;
			if (string.IsNullOrEmpty(rejectedOrderToReplace.EmergencyReplacementForGUID)) emergencyReplacement.SlippageIndex = 0;
			rejectedOrderToReplace.EmergencyReplacedByGUID = emergencyReplacement.GUID;
			emergencyReplacement.EmergencyReplacementForGUID = rejectedOrderToReplace.GUID;
			emergencyReplacement.IsEmergencyClose = true;
			if (rejectedOrderToReplace.IsEmergencyClose) {
				emergencyReplacement.EmergencyCloseAttemptSerno = rejectedOrderToReplace.EmergencyCloseAttemptSerno + 1;
			} else {
				emergencyReplacement.EmergencyCloseAttemptSerno = 1;
			}
			DateTime serverTimeNow = rejectedOrderToReplace.Alert.Bars.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
			emergencyReplacement.TimeCreatedBroker = serverTimeNow;

			this.orderProcessor.DataSnapshot.OrderAddSynchronizedAndPropagate(emergencyReplacement);
			this.orderProcessor.EventDistributor.RaiseOrderStateChanged(this, rejectedOrderToReplace);
			this.orderProcessor.EventDistributor.RaiseOrderReplacementOrKillerCreatedForVictim(this, rejectedOrderToReplace);
	
			return emergencyReplacement;
		}
		private Order findEmergencyReplacementForRejectedOrder(Order orderRejected) {
			Order rejected = this.orderProcessor.DataSnapshot.OrdersAll.FindByGUID(orderRejected.GUID);
			if (rejected == null) {
				throw new Exception("Rejected[" + orderRejected + "] wasn't found!!!");
			}
			Order replacement = this.orderProcessor.DataSnapshot.OrdersAll.FindByGUID(rejected.EmergencyReplacedByGUID);
			return replacement;
		}
		private void throwLogIfNotRejectedClosingOrder(Order order) {
			if (order.Alert.IsEntryAlert == true) {
				string msg = "Direction=[" + order.Alert.Direction + "] is not a Sell/Cover; order[" + order + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (order.stateChangeableToEmergency == false) {
				string msg = "State=[" + order.State + "] is not a Rejected/Error*; order[" + order + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
		}
		private void throwLogAndAppendMessageIfNoEmergencyLockFor(Order order) {
			OrderPostProcessorEmergencyLock emergencyLock = new OrderPostProcessorEmergencyLock(order);
			if (this.emergencyLocks.Contains(emergencyLock) == false) {
				string msg = "who removed EmergencyLock before EmergencyCloseComplete?! " + emergencyLock.ToString();
				this.orderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg);
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
		}
		private void throwLogAndAppendMessageIfNextAttemptReachesLimit(Order rejectedExitOrder) {
			if (rejectedExitOrder.IsEmergencyClose == false) return;
			int emergencyCloseAttemptsMax = rejectedExitOrder.Alert.Bars.SymbolInfo.EmergencyCloseAttemptsMax;
			if (rejectedExitOrder.EmergencyCloseAttemptSerno + 1 < emergencyCloseAttemptsMax) return;
			string msg = "no more EmergencyCloseAttempts:"
				+ " EmergencyCloseAttemptSerno[" + rejectedExitOrder.EmergencyCloseAttemptSerno
				+ "]>= EmergencyCloseAttemptsMax[" + emergencyCloseAttemptsMax + "]"
				+ " emergencyReplacement[" + rejectedExitOrder + "]";
			OrderStateMessage omsg = new OrderStateMessage(rejectedExitOrder, OrderState.EmergencyCloseLimitReached, msg);
			this.orderProcessor.UpdateOrderStateAndPostProcess(rejectedExitOrder, omsg);
			throw new Exception(msg);
		}
		private void throwLogIfEmergencyCloseInterrupted(Order replacementOrder) {
			Order reason4lock = this.GetReasonForLock(replacementOrder);
			lock (this.interruptedEmergencyLockReasons) {
				if (this.interruptedEmergencyLockReasons.Contains(reason4lock) == false) return;
			}
			string msg = "InterruptedEmergencyLockReasons.Contains reason4lock[" + reason4lock + "] for replacementOrder[" + replacementOrder + "]";
			Assembler.PopupException(msg);
			OrderStateMessage newOrderStateRejected = new OrderStateMessage(replacementOrder, OrderState.EmergencyCloseUserInterrupted, msg);
			this.orderProcessor.UpdateOrderStateAndPostProcess(replacementOrder, newOrderStateRejected);
			throw new Exception(msg);
		}
		private void addMessageNoMoreSlippagesAvailable(Order order) {
			int slippageIndexMax = order.Alert.Bars.SymbolInfo.getSlippageIndexMax(order.Alert.Direction);
			string msg2 = "EMERGENCY Reached max slippages available for [" + order.Alert.Bars.Symbol + "]"
				+ " order.SlippageIndex[" + order.SlippageIndex + "] > slippageIndexMax[" + slippageIndexMax + "]"
				+ "; Order will have slippageIndexMax[" + slippageIndexMax + "]";
			Assembler.PopupException(msg2);
			//orderProcessor.updateOrderStatusError(orderExecuted, OrderState.RejectedLimitReached, msg2);
			OrderStateMessage newOrderStateRejected = new OrderStateMessage(order, OrderState.RejectedLimitReached, msg2);
			this.orderProcessor.UpdateOrderStateAndPostProcess(order, newOrderStateRejected);
		}
	}
}
