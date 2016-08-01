using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {

		public void CallbackCreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably(Alert alert, int barNotSubmittedRelno) {
			//SLOW string msig = " CallbackCreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably(" + alertKilled + ")";
			string msig = " //CallbackCreatedOrder_wontBePlacedPastDue_invokeScript_nonReenterably(WAIT)";

			//this.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			try {
				if (alert.IsEntryAlert) {
					this.removePendingEntry(alert);
					this.closePosition_withAlertClonedFromEntry_backtestEnded(alert);
				} else {
					string msg = "IM_USING_ALERTS_EXIT_BAR_NOW__NOT_STREAMING__DO_I_HAVE_TO_ADJUST_HERE?"
						+ "checkPositionCanBeClosed() will later interrupt the flow saying {Sorry I don't serve alerts.IsExitAlert=true}";
					this.RemovePendingExitAlerts_closePositionsBacktestLeftHanging(alert);
				}
				if (this.Strategy.Script == null) return;
				try {
					this.ExecutionDataSnapshot.IsScriptRunning_OnAlertNotSubmitted_nonBlocking = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);
					this.Strategy.Script.OnAlertNotSubmitted_callback(alert, barNotSubmittedRelno);
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
					this.ExecutionDataSnapshot.IsScriptRunning_OnAlertNotSubmitted_nonBlocking = false;
				}
			} catch (Exception e) {
				string msg = "fix your OnAlertNotSubmittedCallback() in script[" + this.Strategy.Script.StrategyName + "]"
					+ "; was invoked with alert[" + alert + "] and barNotSubmittedRelno["
					+ barNotSubmittedRelno + "]";
				this.PopupException(msg, e);
			}
		}

		public void CallbackOrderKilled_orBrokerDeniedSubmission_addGrayCross_onChart(Order expiredOrderKilled_replaceMe) {
			int barIndex_streamingNow = this.Bars.Count - 1;
			try {
				this.ChartShadow.OrderKilled_addForBar(barIndex_streamingNow, expiredOrderKilled_replaceMe);
			} catch (Exception ex) {
				string msg = " //ChartShadow.OrderKilled_addForBar(barIndex_streamingNow[" + barIndex_streamingNow + "], expiredOrderKilled_replaceMe[" + expiredOrderKilled_replaceMe + "])";
				Assembler.PopupException(msg, ex);
			}
		}

		public void CallbackOrderReplaced_invokeScript_nonReenterably(Order expiredOrderKilled_replaceMe, Order replacement, bool orderScheduled) {
			string msig = " //CallbackOrderReplaced_invokeScript_nonReenterably(WAIT)";

			if (this.Strategy.Script != replacement.Alert.Strategy.Script) {
				Assembler.PopupException("NONSENSE this.Strategy.Script != replacement.Alert.Strategy.Script");
			}
			try {
				this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);
				this.ExecutionDataSnapshot.IsScriptRunning_OnAlertKilled_nonBlocking = true;
				// NEVER_INVOKED_IF_ORDER_REPLACEMENT_TURNED_OFF this.CallbackOrderKilled(expiredOrderKilled_replaceMe);
				this.Strategy.Script.OnOrderReplaced_callback(expiredOrderKilled_replaceMe, replacement);
			} finally {
				this.ExecutionDataSnapshot.IsScriptRunning_OnAlertKilled_nonBlocking = false;
				this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
			}
		}
		public void CallbackAlertKilled_invokeScript_nonReenterably(Alert alertKilled) {
			//SLOW string msig = " CallbackAlertKilled_invokeScript_nonReenterably(" + alertKilled + ")";
			string msig = " //CallbackAlertKilled_invokeScript_nonReenterably(WAIT)";

			if (this.Strategy.Script != alertKilled.Strategy.Script) {
				Assembler.PopupException("NONSENSE this.Strategy.Script != alert.Strategy.Script");
			}
			try {
				//v1 NO!!! DIRECT_KILLED_TO_EXECUTOR_UPSTACK alert.Strategy.Script.OnAlertKilledCallback(alert);
				this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);

				Order victimKilled = alertKilled.OrderFollowed_orCurrentReplacement;

				if (this.ExecutionDataSnapshot.AlertsUnfilled.Contains(alertKilled, this, msig)) {
					bool removed = this.ExecutionDataSnapshot.AlertsUnfilled.Remove(alertKilled, this, msig);
					if (removed) alertKilled.StoreKilledInfo(this.Bars.BarLast);
				} else {
					string msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsPending DELETED_EARLIER_OR_NEVER_BEEN_ADDED;"
						+ " PositionCloseImmediately() kills all PositionPrototype-based PendingAlerts"
						+ " => killing those using AlertKillPending() before/after PositionCloseImmediately() is wrong!";
					Assembler.PopupException(msg, null, false);
					if (victimKilled != null) {
						this.OrderProcessor.AppendMessage_propagateToGui(victimKilled, msg);
					}
				}

				if (this.ExecutionDataSnapshot.AlertsDoomed.Contains(alertKilled, this, msig)) {
					bool removed = this.ExecutionDataSnapshot.AlertsDoomed.Remove(alertKilled, this, msig);
					if (removed) alertKilled.StoreKilledInfo(this.Bars.BarLast, true);
				} else {
					//string msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsDoomed DELETED_EARLIER_OR_NEVER_BEEN_ADDED";
					string msg = "";
					if (alertKilled.OrderFollowed_orCurrentReplacement.State == OrderState.Rejected) {
						msg = "REJECTED_KILLED_WITHOUT_HOOK KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsDoomed";
					} else {
						msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsDoomed DELETED_EARLIER_OR_NEVER_BEEN_ADDED";
						Assembler.PopupException(msg, null, false);
					}
					if (victimKilled != null) {
						this.OrderProcessor.AppendMessage_propagateToGui(victimKilled, msg);
					}
				}


				if (victimKilled.State == OrderState.SLAnnihilated || victimKilled.State == OrderState.TPAnnihilated) {
					string msg = "by now, victimKilled.Alert was PositionPrototyped, filled, and here I remove annihilated from Pendings";
				} else {
					bool removing_fromOpenPositions = true;
					if (victimKilled.Alert.MarketLimitStop == MarketLimitStop.Limit) {
						bool removingAlert_afterLastSlippaged_replacementOrderExpired =	victimKilled != null &&
																					victimKilled.HasSlippagesDefined &&
																	   double.IsNaN(victimKilled.SlippageNextAvailable_forLimitAlertsOnly_NanWhenNoMore);
						removing_fromOpenPositions = removingAlert_afterLastSlippaged_replacementOrderExpired;
					} else {
						string msg = "#3 THE_CONCEPT_OF_SLIPPAGE_IS_NOT_APPLICABLE_FOR_STOP_ORDERS__AND_POSITION_PROTOTYPE_ENTRIES";
					}
					if (removing_fromOpenPositions) {
						if (alertKilled.BuyOrShort) {
							this.ExecutionDataSnapshot.Positions_OpenNow.Remove(alertKilled.PositionAffected, this,
								"REMOVING_ALERT_UNFILLED__AFTER_LAST_SLIPPAGED_REPLACEMENT_ORDER_EXPIRED__ENTRY_ALERT_UNLUCKY_DIDNT_GET_FILL");
						} else {
							string msg = "PROBABLY_EMERGENCY_CLOSE_WILL_KICK_IN... BUT_SHOULD_I_REMOVE_FROM_byExitFilled[]??";
							Assembler.PopupException(msg, null, false);
						}
					}
				}

				this.ExecutionDataSnapshot.IsScriptRunning_OnAlertKilled_nonBlocking = true;
				this.Strategy.Script.OnAlertKilled_callback(alertKilled);
			} finally {
				this.ExecutionDataSnapshot.IsScriptRunning_OnAlertKilled_nonBlocking = false;
				this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
			}
		}

		public void Callback_OrderLimit_Expired(Order orderExpired) {
#if VERBOSE_STRINGS_SLOW
			string msig = " //CallbackOrderExpired(" + orderExpired + ", " + quoteFilledThisAlert_nullForLive + ")";
#else
			string msig = " //CallbackOrderExpired(WAIT)";
#endif

		}

//        public void Callback_OrderMarketLimitStop_Error(Order order_withErrorState) {
//#if VERBOSE_STRINGS_SLOW
//            string msig = " //CallbackOrderError(" + order_withErrorState + ")";
//#else
//            string msig = " //CallbackOrderError(WAIT)";
//#endif

//        }

		public void CallbackAlertFilled_moveAround_invokeScriptCallback_reenterablyProtected(
							Alert alertFilled, Quote quoteFilledThisAlert_nullForLive,
							double priceFill, double qtyFill, double slippageFill, double commissionFill) {
#if VERBOSE_STRINGS_SLOW
			string msig = " //CallbackAlertFilled_moveAround_invokeScriptCallback_reenterablyProtected(" + alertFilled + ", " + quoteFilledThisAlert_nullForLive + ")";
#else
			string msig = " //CallbackAlertFilled_moveAround_invokeScriptCallback_reenterablyProtected(WAIT)";
#endif

			//avoiding two alertsFilled and messing script-overrides; despite all script invocations downstack are sequential, guaranteed for 1 alertFilled
			try {
				this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);
				this.callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(
							alertFilled, quoteFilledThisAlert_nullForLive,
							priceFill, qtyFill, slippageFill, commissionFill);
			} finally {
				this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
			}
			// filled alerts should be immediately be reflected with an arrow on PricePanel
			this.ChartShadow.InvalidateAllPanels();
		}
		void callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(
							Alert alertFilled, Quote quoteFilledThisAlert_nullFromOrderProcessorWhenLive,
							double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			//SLOW string msig = " callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(" + alertFilled + ", " + quoteFilledThisAlertNullForLive + ")";
			string msig = " //callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(" + alertFilled + ", " + quoteFilledThisAlert_nullFromOrderProcessorWhenLive + ")";

			Bar barFill = (this.IsStreamingTriggeringScript)
				? alertFilled.Bars.BarStreaming_nullUnsafeCloneReadonly
				: alertFilled.Bars.BarStaticLast_nullUnsafe;

			#region PARANOID
			if (priceFill == -1) {
				string msg = "won't set priceFill=-1 for alert [" + alertFilled + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			int barFillRelno  = alertFilled.Bars.Count - 1;
			if (barFillRelno != alertFilled.Bars.BarStreaming_nullUnsafe.ParentBarsIndex) {
				string msg = "NONSENSE#3";
				Assembler.PopupException(msg);
			}
			
			//v1
			if (barFillRelno != barFill.ParentBarsIndex) {
				string msg = "barFillRelno[" + barFillRelno + "] != barFill.ParentBarsIndex["
					+ barFill.ParentBarsIndex + "]; barFill=[" + barFill + "]";
				Assembler.PopupException(msg, null, false);
			}
			//v2
			// Limit might get a Fill 2 bars after it was placed; PlacedBarIndex=BarStreaming.ParentIndex = now for past bar signals => not "PlacedBarIndex-1"
			if (barFillRelno < alertFilled.PlacedBarIndex) {
				string msg = "I_REFUSE_MOVE_AROUND__FILLED_BEFORE_PLACED barFillRelno[" + barFillRelno + "] < PlacedBarIndex["
					+ alertFilled.PlacedBarIndex + "]; FilledBar=[" + alertFilled.FilledBar_live + "] PlacedBar=[" + alertFilled.PlacedBar + "]";
				Assembler.PopupException(msg);
			}


			// fresh puff
			if (priceFill <= 0) {
				string msg = "REJECTED_ALERTS_SHOULD_BE_PROCESSED_IN_callBackAlertRejected() WONT_MOVE_AROUND: priceFill[" + priceFill + "] <= 0";
				Assembler.PopupException(msg, null, false);
				//filledIndeed_notRejected_willAddPosition_toExecutionSnapshot = false;
				return;
			}

			bool filledIndeed_notRejected_willAddPosition_toExecutionSnapshot = true;
			if (alertFilled.OrderFollowed_orCurrentReplacement != null) {
				string msg = "dealing with Live or LiveSim; it's not a backtest";
				//Assembler.PopupException(msg, null, false);
				switch (alertFilled.OrderFollowed_orCurrentReplacement.State) {
					case OrderState.Filled:
					case OrderState.FilledPartially:
						filledIndeed_notRejected_willAddPosition_toExecutionSnapshot = true;
						break;

					case OrderState.LimitExpiredRejected:
						string msg2 = "SHOULD_NOT_ADD_REJECTED_POSITION_OPEN_ANYWHERE";
						filledIndeed_notRejected_willAddPosition_toExecutionSnapshot = false;
						break;

					default:
						string msg1 = "WHEN_IS_THIS_APPLICABLE??? implement OrderStatus=initial for QUIK, hopefully these callbacks will bring fill/error after TCP flow resumed";
						Assembler.PopupException(msg1, null, false);
						break;
				}
			} else {
				string msg = "NO_PLACE_FOR_ORDER_REJECTION backtester always fills full size requested with 100% success rate; when no fill => I'm not invoked here";
				//Assembler.PopupException(msg, null, false);
				filledIndeed_notRejected_willAddPosition_toExecutionSnapshot = true;
			}

			if (filledIndeed_notRejected_willAddPosition_toExecutionSnapshot == false) {
				string msg = "NOT_ADDING_POSITION_TO_SNAP_KOZ_STATE[" + alertFilled.OrderFollowed_orCurrentReplacement.State + "]";
				bool closingAlert_wasRejected = alertFilled.SellOrCover;
				if (alertFilled.PositionPrototype_onlyForEntryAlert != null && closingAlert_wasRejected) {
					msg = "CLOSING_ALERT_REJECTED__USE_SymbolInfo.EMERGENCY_CLOSE " + msg;
				}
				Assembler.PopupException(msg);
				return;
			}
			#endregion


			if (quoteFilledThisAlert_nullFromOrderProcessorWhenLive != null) {
				//BACKTEST
				if (quoteFilledThisAlert_nullFromOrderProcessorWhenLive.ParentBarStreaming == null) {
					string msg = "NONSENSE#1";
					Assembler.PopupException(msg);
				}
				if (quoteFilledThisAlert_nullFromOrderProcessorWhenLive.ParentBarStreaming != alertFilled.Bars.BarStreaming_nullUnsafe) {
					string msg = "NONSENSE#4";
					Assembler.PopupException(msg);
				}
				if (alertFilled.Bars != quoteFilledThisAlert_nullFromOrderProcessorWhenLive.ParentBarStreaming.ParentBars) {
					string msg = "NONSENSE#2";
					Assembler.PopupException(msg);
				}
				//THIS_ALREADY_IS_A_CLONE alertFilled.QuoteFilledThisAlertDuringBacktestNotLive = quoteFilledThisAlertNullForLive.Clone();	// CLONE_TO_FREEZE_AS_IT_HAPPENED_IGNORING_WHATEVER_HAPPENED_WITH_ORIGINAL_QUOTE_AFTERWARDS
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive = quoteFilledThisAlert_nullFromOrderProcessorWhenLive;	// CLONE_TO_FREEZE_AS_IT_HAPPENED_IGNORING_WHATEVER_HAPPENED_WITH_ORIGINAL_QUOTE_AFTERWARDS
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive.ItriggeredFillAtBidOrAsk = alertFilled.BidOrAsk_willFillMe;
			} else {
				//LIVEISM
				alertFilled.QuoteCurrent_whenThisAlertFilled = this.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(alertFilled.Symbol);
				alertFilled.QuoteCurrent_whenThisAlertFilled_deserializable = alertFilled.QuoteCurrent_whenThisAlertFilled.Clone_asCoreQuote();
			}

			//DILUTED_30_LINES_BELOW
			//if (priceFill != 0) {
			//    try {
			//        alertFilled.FillPositionAffected_entryOrExit_respectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);
			//        if (alertFilled.IsEntryAlert) {
			//            this.ExecutionDataSnapshot.Positions_OpenNow.AddOpened_step1of2	(alertFilled.PositionAffected, this, "BROKER_FILLED_ENTRY_ALERT__ADDING_TO_POSITIONS_OPEN");
			//        } else {
			//            this.ExecutionDataSnapshot.Positions_OpenNow.RemoveUnique		(alertFilled.PositionAffected, this, "BROKER_FILLED_EXIT_ALERT__REMOVING_FROM_POSITIONS_OPEN");
			//        }
			//    } catch (Exception ex) {
			//        string msg = "REMOVE_FILLED_FROM_PENDING? DONT_USE_Bar.ContainsPrice()?";
			//        Assembler.PopupException(msg + msig, ex);
			//    }
			//}

			bool breakIfAbsent = true;
			//bool wasOnceRejected_nowCanBeFilled = alertFilled.OrderFollowed.FindState_inOrderMessages(OrderState.Rejected);
			//bool replacementForRejectedFilled = alertFilled.OrderFollowed.FindState_inOrderMessages("REPLACEMENT_FOR_REJECTED");
			//if (replacementForRejectedFilled) breakIfAbsent = false;
			int forever = -1;
			bool removed = this.ExecutionDataSnapshot.AlertsUnfilled.Remove(alertFilled, this, msig, forever, breakIfAbsent);

			alertFilled.FillPositionAffected_entryOrExit_respectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);

			string msg_order = "AlertsUnfilled.Count[" + this.ExecutionDataSnapshot.AlertsUnfilled.Count + "]:removed[" + removed + "]";

			Position positionOpened_afterAlertFilled = null;
			Position positionClosed_afterAlertFilled = null;

			PositionList positionsOpened_afterAlertFilled = new PositionList("positionsOpened_afterAlertFilled", this.ExecutionDataSnapshot);
			PositionList positionsClosed_afterAlertFilled = new PositionList("positionsClosed_afterAlertFilled", this.ExecutionDataSnapshot);


			if (alertFilled.IsEntryAlert) {
				Position positionOpening = alertFilled.PositionAffected;
				bool duplicateThrows = true;
				int added2 = this.ExecutionDataSnapshot.Positions_addNew_incrementPositionSerno(positionOpening);
				msg_order += " Positions_OpenNow.Count[" + this.ExecutionDataSnapshot.Positions_OpenNow.Count + "]:added2[" + added2 + "]";

				if (this.DataSource_fromBars.BrokerAsBacktest_nullUnsafe != null) {
					this.ExecutionDataSnapshot.Positions_AllBacktested.AddOpened_step1of2(positionOpening, this,
						"ENTRY_ALERT_GOT_FILL__ADDING_TO_Positions_AllBacktested" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, duplicateThrows);
				}

				positionOpened_afterAlertFilled = positionOpening;
				positionsOpened_afterAlertFilled.AddOpened_step1of2(positionOpened_afterAlertFilled,
					this, msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, duplicateThrows);
				// THIS WAS_WRONG ALREADY_ADDED	this.ExecutionDataSnapshot.Positions_OpenNow.AddOpened_step1of2(positionOpening, this,
				// THIS WAS_WRONG ALREADY_ADDED		"ENTRY_ALERT_GOT_FILL__ADDING_TO_Positions_OpenNow" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, duplicateThrows);		//ONLY_ALERTS_ARE_PENDING_TILL_ORDER_FILL PENDING_POSITIONS_ARENT_KEPT_ANYMORE
			} else {
				Position positionClosing = alertFilled.PositionAffected;
				bool absenceThrows = true;
				if (this.DataSource_fromBars.BrokerAsBacktest_nullUnsafe != null) {
					bool added = this.ExecutionDataSnapshot.Positions_AllBacktested.AddToClosedDictionary_step2of2(positionClosing, this,
						"EXIT_ALERT_GOT_FILL__ADDING_TO_Positions_AllBacktested" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenceThrows);
				}
				positionClosed_afterAlertFilled = positionClosing;
				positionsClosed_afterAlertFilled.AddClosed(positionClosed_afterAlertFilled, this,
					"EXIT_ALERT_GOT_FILL__ADDING_TO_positionsClosed_afterAlertFilled" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenceThrows);


				// not necessary but keeps by-bar-open/close "history" inside PositionList
				bool added2 = this.ExecutionDataSnapshot.Positions_OpenNow.AddToClosedDictionary_step2of2(positionClosing, this,
					"EXIT_ALERT_GOT_FILL__ADDING_TO_Positions_OpenNow_ByBarClosedDictionary" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenceThrows);

				bool removedClosed = this.ExecutionDataSnapshot.Positions_OpenNow.Remove(positionClosing, this,
					"EXIT_ALERT_GOT_FILL__REMOVING_FROM_Positions_OpenNow" + msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenceThrows);
				msg_order += " Positions_OpenNow.Count[" + this.ExecutionDataSnapshot.Positions_OpenNow.Count + "]:removedClosed[" + removedClosed + "]";

				Position mustBeNull = this.Strategy.Script.LastPosition_OpenNow_nullUnsafe;
				if (mustBeNull != null) {
					string msg = "YOUR_STRATEGY_OPENED_MULTIPLE_POSITIONS???";	//DONT_USE_RemoveUnique()_FOR_POSITIONS_LIST";
					Assembler.PopupException(msg, null, false);
				}
			}


			if (alertFilled.OrderFollowed_orCurrentReplacement != null) {
				this.OrderProcessor.AppendMessage_propagateToGui(alertFilled.OrderFollowed_orCurrentReplacement, msg_order);
			}


			bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;

			// MOST_LIKELY_INVOKED_FROM_CALLBACK_WITH_PREVIOUS_BAR_INDEX AND_ONE_MORE_FILTER_PENDING_NOT_EARLIER_THAN_PLACED 
			//Bar barBarStreaming_nullUnsafe = this.Bars.BarStreaming_nullUnsafe;
			//List<Alert> alertsPendingAtCurrentBarSafeCopy = this.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "callbackAlertFilledMoveAroundInvokeScriptReenterablyUnprotected(WAIT)");
			//if (barBarStreaming_nullUnsafe != null && alertsPendingAtCurrentBarSafeCopy.Count > 0) {
			//    this.ChartShadow.AlertsPendingStillNotFilledForBarAdd(barBarStreaming_nullUnsafe.ParentBarsIndex, alertsPendingAtCurrentBarSafeCopy);
			//}

			AlertList alertsNewAfterAlertFilled = new AlertList("alertsNewAfterAlertFilled", this.ExecutionDataSnapshot);
			PositionPrototype proto = alertFilled.PositionPrototype_bothForEntryAndExit;
			if (proto != null) {
				// 0. once again, set ExitAlert to What was actually filled, because prototypeEntry created SL & TP, which were both written into ExitAlert;
				// so if we caught the Loss and SL was executed, position.ExitAlert will still contain TP if we don't set it here
				bool exitIsDifferent = alertFilled.PositionAffected.ExitAlert != null && alertFilled.PositionAffected.ExitAlert != alertFilled;
				if (alertFilled.IsExitAlert && exitIsDifferent) {
					alertFilled.PositionAffected.ExitAlertAttach(alertFilled);
				}
				// 1. alert.PositionPrototype.StopLossAlertForAnnihilation and TP will get assigned
				alertsNewAfterAlertFilled.AddRange(this.PositionPrototypeActivator.PositionActivator_entryPoint__alertFilled_createSlTp_orAnnihilateCounterparty(alertFilled), this, msig);
				// quick check: there must be {SL+TP} OR Annihilator
				//this.Backtester.IsBacktestingNow == false &&
				if (alertFilled.IsEntryAlert) {
					// DONT_SCREAM_SO_MUCH IF_OFFSETS_WERE_ZERO_OR_WRONG_POLARITY_NO_SL_TP_ARE_CREATED CreateStopLossFromPositionPrototype() CreateTakeProfitFromPositionPrototype()
					//if (proto.StopLossAlertForAnnihilation == null) {
					//	string msg = "NONSENSE@Entry: proto.StopLossAlert is NULL???..";
					//	throw new Exception(msg);
					//}
					//if (proto.TakeProfitAlertForAnnihilation == null) {
					//	string msg = "NONSENSE@Entry: proto.TakeProfitAlert is NULL???..";
					//	throw new Exception(msg);
					//}
					//if (alertsNewAfterAlertFilled.Count == 0) {
					//	string msg = "NONSENSE@Entry: alertsNewSlAndTp.Count=0"
					//		+ "; this.PositionPrototypeActivator.AlertFilledCreateSlTpOrAnnihilateCounterparty(alertFilled)"
					//		+ " should return 2 alerts; I don't want to create new list from {proto.SL, proto.TP}";
					//	throw new Exception(msg);
					//}
				}
				if (alertFilled.IsExitAlert) {
					if (alertsNewAfterAlertFilled.Count > 0) {
						string msg = "NONSENSE@Exit: there must be no alerts (got " + alertsNewAfterAlertFilled.Count + "): killer works silently";
						#if DEBUG
						Debugger.Break();
						#endif
						throw new Exception(msg);
					}
				}

				if (alertsNewAfterAlertFilled.Count > 0) {
					List<Alert> alertsNewAfterExecSafeCopy = alertsNewAfterAlertFilled.SafeCopy(this, msig);

					#region copypaste BEGIN, helping GuiHasTimeRebuildReportersAndExecution to find prototyped alerts
					if (this.ChartShadow != null) {
						//bool guiHasTime = false;
						foreach (Alert alert in alertsNewAfterExecSafeCopy) {
							try {
								Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, alert);
								//if (guiHasTime == false) guiHasTime = alert.GuiHasTimeRebuildReportersAndExecution;
							} catch (Exception ex) {
								string msg = "ADDING_TO_DICTIONARY_MANY_TO_ONE";
								Assembler.PopupException(msg, ex);
							}
						}
					} else {
						if (this.Sequencer.IsRunningNow == false) {
							string msg = "CHART_SHADOW_MUST_BE_NULL_ONLY_WHEN_OPTIMIZING__BACKTEST_AND_LIVESIM_MUST_HAVE_CHART_SHADOW_ASSOCIATED";
							Assembler.PopupException(msg);
						}
					}
					#endregion

					Quote quoteHackForLive = quoteFilledThisAlert_nullFromOrderProcessorWhenLive;
					if (quoteHackForLive == null) {
						quoteHackForLive = alertFilled.QuoteCurrent_whenThisAlertFilled;	// unconditionally filled 130 lines above
					}
					this.EnrichAlerts_withQuoteCreated(alertsNewAfterExecSafeCopy, quoteHackForLive);

					if (this.IsStrategyEmittingOrders && this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) {
						string msg1 = "IM_HERE_FOR_LIVE_OR_LIVESIM__BACKTEST_AND_SEQUENCING_IGNORE_IsStrategyEmittingOrders";

						this.OrderProcessor.Emit_createOrders_forScriptGeneratedAlerts_eachInNewThread(alertsNewAfterExecSafeCopy, setStatusSubmitting, true);
	
						// 3. Script using proto might move SL and TP which require ORDERS to be moved, not NULLs
						int twoMinutes = 120000;
						if (alertFilled.IsEntryAlert) {
							// there must be SL.OrderFollowed!=null and TP.OrderFollowed!=null
							if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement == null) {
								string msg = "StopLossAlert.OrderFollowed is NULL!!! CreateOrdersSubmitToBrokerAdapterInNewThreads() didnt do its job; engaging ManualResetEvent.WaitOne()";
								this.PopupException(msg);

								Stopwatch waitedForStopLossOrder = new Stopwatch();
								waitedForStopLossOrder.Start();

								if (this.BacktesterOrLivesimulator.ImRunningLivesim) {
									bool unpaused = this.Livesimulator.IsPaused_waitForever_untilUnpaused;
								} else {
									proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed_isAssignedNow_Mre.WaitOne(twoMinutes);
								}

								waitedForStopLossOrder.Stop();
								msg = "waited " + waitedForStopLossOrder.ElapsedMilliseconds + "ms for StopLossAlert.OrderFollowed";

								if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement == null) {
									msg += ": NO_SUCCESS still null!!!";
									this.PopupException(msg);
								} else {
									proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement.AppendMessage(msg);
									this.PopupException(msg);
								}
							//} else {
							//	string msg = "you are definitely crazy, StopLossAlert.OrderFollowed is a single-threaded assignment";
							//	Assembler.PopupException(msg);
							}
	
							if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement == null) {
								string msg = "TakeProfitAlert.OrderFollowed is NULL!!! CreateOrdersSubmitToBrokerAdapterInNewThreads() didnt do its job; engaging ManualResetEvent.WaitOne()";
								this.PopupException(msg);
								Stopwatch waitedForTakeProfitOrder = new Stopwatch();
								waitedForTakeProfitOrder.Start();
								proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed_isAssignedNow_Mre.WaitOne(twoMinutes);
								waitedForTakeProfitOrder.Stop();
								msg = "waited " + waitedForTakeProfitOrder.ElapsedMilliseconds + "ms for TakeProfitAlert.OrderFollowed";
								if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement == null) {
									msg += ": NO_SUCCESS still null!!!";
									this.PopupException(msg);
								} else {
									proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed_orCurrentReplacement.AppendMessage(msg);
									this.PopupException(msg);
								}
							//} else {
							//	string msg = "you are definitely crazy, TakeProfitAlert.OrderFollowed is a single-threaded assignment";
							//	Assembler.PopupException(msg);
							}
						}
						this.ChartShadow.AlertsPlaced_addRealtime(alertsNewAfterExecSafeCopy);
					}
				}
			}

			// 4. Script event will generate a StopLossMove PostponedHook
			//NOW_INLINE this.invokeScriptEvents(alertFilled);
			if (this.Strategy.Script != null) {
				try {
					this.ExecutionDataSnapshot.IsScriptRunning_OnAlertFilled_nonBlocking = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnAlertFilled_callback(WAIT)");
					this.Strategy.Script.OnAlertFilled_callback(alertFilled);
				} catch (Exception e) {
					string msg = "fix your OnAlertFilled_callback() in script[" + this.Strategy.Script.StrategyName + "]"
						+ "; was invoked with alert[" + alertFilled + "]";
					this.PopupException(msg, e);
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnAlertFilled_callback(WAIT)");
					this.ExecutionDataSnapshot.IsScriptRunning_OnAlertFilled_nonBlocking = false;
				}
				if (alertFilled.IsEntryAlert) {
					if (alertFilled.PositionPrototype_onlyForEntryAlert != null) {
						try {
							this.ExecutionDataSnapshot.IsScriptRunning_onPositionOpenedPrototypeSlTpPlaced_nonBlocking = true;
							this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionOpened_prototypeSlTpPlaced_callback(WAIT)");
							this.Strategy.Script.OnPositionOpened_prototypeSlTpPlaced_callback(alertFilled.PositionAffected);
						} catch (Exception e) {
							string msg = "fix your OnPositionOpened_prototypeSlTpPlaced_callback() in script[" + this.Strategy.Script.StrategyName + "]"
								+ "; was invoked with PositionAffected[" + alertFilled.PositionAffected + "]";
							this.PopupException(msg, e);
						} finally {
							this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionOpened_prototypeSlTpPlaced_callback(WAIT)");
							this.ExecutionDataSnapshot.IsScriptRunning_onPositionOpenedPrototypeSlTpPlaced_nonBlocking = false;
						}
					} else {
						try {
							this.ExecutionDataSnapshot.IsScriptRunning_OnPositionOpened_nonBlocking = true;
							this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionOpened_callback(WAIT)");
							this.Strategy.Script.OnPositionOpened_callback(alertFilled.PositionAffected);
						} catch (Exception e) {
							string msg = "fix your OnPositionOpened_callback() in script[" + this.Strategy.Script.StrategyName + "]"
								+ "; was invoked with PositionAffected[" + alertFilled.PositionAffected + "]";
							this.PopupException(msg, e);
						} finally {
							this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionOpened_callback(WAIT)");
							this.ExecutionDataSnapshot.IsScriptRunning_OnPositionOpened_nonBlocking = false;
						}
					}
				} else {
					if (alertFilled.PositionPrototype_onlyForEntryAlert != null) {
						string msg = "NYI alertFilled.PositionPrototype != null";
						Assembler.PopupException(msg + msig);
					} else {
						try {
							this.ExecutionDataSnapshot.IsScriptRunning_OnPositionClosed_nonBlocking = true;
							this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionClosed_callback(WAIT)");
							this.Strategy.Script.OnPositionClosed_callback(alertFilled.PositionAffected);
						} catch (Exception e) {
							string msg = "fix your OnPositionClosed_callback() in script[" + this.Strategy.Script.StrategyName + "]"
								+ "; was invoked with PositionAffected[" + alertFilled.PositionAffected + "]";
							this.PopupException(msg, e);
						} finally {
							this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionClosed_callback(WAIT)");
							this.ExecutionDataSnapshot.IsScriptRunning_OnPositionClosed_nonBlocking = false;
						}
					}
				}
			}

			if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
				string msg = "AFTER_BACKTEST__THE_HOOK_INVOKES_Performance.BuildStatsOnBacktestFinished()_AND_ReportersFormsManager.BuildReportFullOnBacktestFinished()";
				return;
			}

			ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quoteFilledThisAlert_nullFromOrderProcessorWhenLive, alertsNewAfterAlertFilled,
															 positionsOpened_afterAlertFilled,
															 positionsClosed_afterAlertFilled,
															 null
															);
			using (pokeUnit_dontForgetToDispose) {
				//v1 this.AddPositionsToChartShadowAndPushPositionsOpenedClosedToReportersAsyncUnsafe(pokeUnit);
				if (positionOpened_afterAlertFilled != null) {
					this.PerformanceAfterBacktest.BuildIncremental_brokerFilledAlertsOpening_forPositions_step1of3(positionOpened_afterAlertFilled);
					//if (alertFilled.GuiHasTimeRebuildReportersAndExecution) {
						// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
						this.EventGenerator.RaiseOnBrokerFilledAlertsOpeningForPositions_step1of3(pokeUnit_dontForgetToDispose);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
					//}
				}
				if (positionClosed_afterAlertFilled != null) {
					this.PerformanceAfterBacktest.BuildReportIncremental_brokerFilledAlertsClosing_forPositions_step3of3(positionClosed_afterAlertFilled);
					if (alertFilled.GuiHasTime_toRebuildReportersAndExecution) {
						// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
						this.EventGenerator.RaiseOnBrokerFilledAlertsClosingForPositions_step3of3(pokeUnit_dontForgetToDispose);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
					}
				}

				this.ChartShadow.PositionsRealtimeAdd(pokeUnit_dontForgetToDispose);
			}
		}
		public bool Callback_BrokerDeniedSubmission(Order order_BDS) {
			string msig = " //OrderRejected_byBroker(order_BDS[" + order_BDS + "])";
			Alert alert_BDS = order_BDS.Alert;

			alert_BDS.StoreKilledInfo();

			bool breakIfAbsent = true;
			int forever = -1;
			bool removed = this.ExecutionDataSnapshot.AlertsUnfilled.Remove(alert_BDS, this, msig, forever, breakIfAbsent);
			this.CallbackOrderKilled_orBrokerDeniedSubmission_addGrayCross_onChart(order_BDS);
			this.Strategy.Script.OnBrokerDeniedSubmission_callback(alert_BDS);
			return removed;
		}
		void closePosition_withAlertClonedFromEntry_backtestEnded(Alert alert) {
			string msig = " closePositionWithAlertClonedFromEntryBacktestEnded():";
			this.removePendingExitAlert_backtestEnded(alert, msig);
			bool checkPositionOpenNow = true;
			if (this.check_positionCanBeClosed(alert, msig, checkPositionOpenNow) == false) return;
			if (alert.FilledBar_live == null) {
				string msg = "BACKTEST_ENDED_ALERT_UNFILLED strategy[" + this.Strategy.ToString() + "] alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			//Alert alertExitClonedStub = alert.CloneToRefactor();
			//alertExitClonedStub.SignalName += " ClosePositionWithAlertClonedFromEntryBacktestEnded Forced";
			//alertExitClonedStub.Direction = MarketConverter.ExitDirectionFromLongShort(alert.PositionLongShortFromDirection);
			//// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Position.EntryAlert was filled!!!
			//alert.PositionAffected.ExitAlertAttach(alertExitClonedStub);
			alert.PositionAffected.FillExitWith(alert.FilledBar_live, alert.PriceEmitted, alert.Qty, 0, 0);
			//alertFilled.FillPositionAffectedEntryOrExitRespectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);

			bool absenceThrows = true;
			this.ExecutionDataSnapshot.MovePositionOpen_toClosed_backtestEnded(alert.PositionAffected, absenceThrows);
		}
		bool check_positionCanBeClosed(Alert alert, string msig, bool checkPositionOpenNow = true) {
			if (alert.PositionAffected == null) {
				string msg = "can't close PositionAffected and remove Position from PositionsOpenNow"
					+ ": alert.PositionAffected=null for alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
				return false;
			}
			if (alert.IsExitAlert) {
				string msg = "Sorry I don't serve alerts.IsExitAlert=true, only .IsEntryAlert's: alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
				return false;
			}
			if (checkPositionOpenNow == true) {
				bool shouldRemove = this.ExecutionDataSnapshot.Positions_OpenNow.Contains(alert.PositionAffected, this, "check_positionCanBeClosed(WAIT)");

				if (alert.FilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only BarRelnoFilled==-1"
						+ " otherwize alert.FillPositionAffectedEntryOrExitRespectively() with throw: alert [" + alert + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
					return false;
				}
				if (alert.PositionAffected.ExitFilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only alert.PositionAffected.ExitBar==-1"
						+ " otherwize PositionAffected.FillExitAlert() will throw: alert [" + alert + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
					return false;
				}
			} else {
				Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreaming_nullUnsafeCloneReadonly : alert.Bars.BarStaticLast_nullUnsafe;
				if (alert.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: can't do my job: alert.PositionAffected.EntryBar!=-1 for alert [" + alert + "]";
					//throw new Exception(msig + msg);
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
					//return;
				} else {
					string msg = "Forcibly closing at EntryBar=[" + barFill + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed_orCurrentReplacement, msig + msg);
				}

			}
			return true;
		}
	}
}
