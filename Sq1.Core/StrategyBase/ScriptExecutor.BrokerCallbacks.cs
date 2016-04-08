using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

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
					this.ExecutionDataSnapshot.IsScriptRunningOnAlertNotSubmittedNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);
					this.Strategy.Script.OnAlertNotSubmitted_callback(alert, barNotSubmittedRelno);
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
					this.ExecutionDataSnapshot.IsScriptRunningOnAlertNotSubmittedNonBlockingRead = false;
				}
			} catch (Exception e) {
				string msg = "fix your OnAlertNotSubmittedCallback() in script[" + this.Strategy.Script.StrategyName + "]"
					+ "; was invoked with alert[" + alert + "] and barNotSubmittedRelno["
					+ barNotSubmittedRelno + "]";
				this.PopupException(msg, e);
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

				if (this.ExecutionDataSnapshot.AlertsPending.Contains(alertKilled, this, msig)) {
					bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alertKilled, this, msig);
					if (removed) alertKilled.IsKilled = true;
				} else {
					string msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsPending DELETED_EARLIER_OR_NEVER_BEEN_ADDED;"
						+ " PositionCloseImmediately() kills all PositionPrototype-based PendingAlerts"
						+ " => killing those using AlertKillPending() before/after PositionCloseImmediately() is wrong!";
					//throw new Exception(msg);
					Assembler.PopupException(msg);
				}

				if (this.ExecutionDataSnapshot.AlertsDoomed.Contains(alertKilled, this, msig)) {
					bool removed = this.ExecutionDataSnapshot.AlertsDoomed.Remove(alertKilled, this, msig);
					if (removed) alertKilled.IsKilled = true;
				} else {
					string msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsDoomed DELETED_EARLIER_OR_NEVER_BEEN_ADDED";
					//throw new Exception(msg);
					Assembler.PopupException(msg, null, false);
					if (alertKilled.OrderFollowed != null) {
						this.OrderProcessor.AppendMessage_propagateToGui(alertKilled.OrderFollowed, msg);
					}
				}

				this.ExecutionDataSnapshot.IsScriptRunningOnAlertKilledNonBlockingRead = true;
				this.Strategy.Script.OnAlertKilled_callback(alertKilled);
			} finally {
				this.ExecutionDataSnapshot.IsScriptRunningOnAlertKilledNonBlockingRead = false;
				this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
			}
		}

		public void CallbackAlertFilled_moveAround_invokeScriptNonReenterably(Alert alertFilled, Quote quoteFilledThisAlert_nullForLive,
																			double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			//SLOW string msig = " //CallbackAlertFilledMoveAroundInvokeScript(" + alertFilled + ", " + quoteFilledThisAlert_nullForLive + ")";
			string msig = " //CallbackAlertFilled_moveAround_invokeScriptNonReenterably(WAIT)";

			//avoiding two alertsFilled and messing script-overrides; despite all script invocations downstack are sequential, guaranteed for 1 alertFilled
			try {
				this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig);
				this.callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(alertFilled, quoteFilledThisAlert_nullForLive,
																					 priceFill, qtyFill, slippageFill, commissionFill);
			} finally {
				this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig);
			}
			// filled alerts should be immediately be reflected with an arrow on PricePanel
			this.ChartShadow.InvalidateAllPanels();
		}
		void callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(Alert alertFilled, Quote quoteFilledThisAlertNullForLive,
																			 double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			//SLOW string msig = " callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(" + alertFilled + ", " + quoteFilledThisAlertNullForLive + ")";
			string msig = " //callbackAlertFilled_moveAround_invokeScript_reenterablyUnprotected(" + alertFilled + ", " + quoteFilledThisAlertNullForLive + ")";

			if (priceFill == -1) {
				string msg = "won't set priceFill=-1 for alert [" + alertFilled + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (alertFilled.PositionAffected == null) {
				string msg = "CallbackAlertFilled can't do its job: alert.PositionAffected=null for alert [" + alertFilled + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			Bar barFill = (this.IsStreamingTriggeringScript)
				? alertFilled.Bars.BarStreaming_nullUnsafeCloneReadonly
				: alertFilled.Bars.BarStaticLast_nullUnsafe;

			if (alertFilled.IsEntryAlert) {
				if (alertFilled.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.EntryBar!=-1 for alert [" + alertFilled + "]";
					Assembler.PopupException(msg, null, false);
				} else {
					string msg = "initializing EntryBar=[" + barFill + "] on AlertFilled";
				}
			} else {
				if (alertFilled.PositionAffected.ExitFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.ExitBar!=-1 for alert [" + alertFilled + "]";
					Assembler.PopupException(msg, null, false);
				} else {
					string msg = "initializing ExitBar=[" + barFill + "] on AlertFilled";
				}
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
					+ alertFilled.PlacedBarIndex + "]; FilledBar=[" + alertFilled.FilledBar + "] PlacedBar=[" + alertFilled.PlacedBar + "]";
				Assembler.PopupException(msg);
			}

			if (quoteFilledThisAlertNullForLive != null) {
				//BACKTEST
				if (quoteFilledThisAlertNullForLive.ParentBarStreaming == null) {
					string msg = "NONSENSE#1";
					Assembler.PopupException(msg);
				}
				if (quoteFilledThisAlertNullForLive.ParentBarStreaming != alertFilled.Bars.BarStreaming_nullUnsafe) {
					string msg = "NONSENSE#4";
					Assembler.PopupException(msg);
				}
				if (alertFilled.Bars != quoteFilledThisAlertNullForLive.ParentBarStreaming.ParentBars) {
					string msg = "NONSENSE#2";
					Assembler.PopupException(msg);
				}
				//THIS_ALREADY_IS_A_CLONE alertFilled.QuoteFilledThisAlertDuringBacktestNotLive = quoteFilledThisAlertNullForLive.Clone();	// CLONE_TO_FREEZE_AS_IT_HAPPENED_IGNORING_WHATEVER_HAPPENED_WITH_ORIGINAL_QUOTE_AFTERWARDS
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive = quoteFilledThisAlertNullForLive;	// CLONE_TO_FREEZE_AS_IT_HAPPENED_IGNORING_WHATEVER_HAPPENED_WITH_ORIGINAL_QUOTE_AFTERWARDS
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive.ItriggeredFillAtBidOrAsk = alertFilled.BidOrAskWillFillMe;
			} else {
				//LIVEISM
				alertFilled.QuoteCurrent_whenThisAlertFilled = this.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(alertFilled.Symbol);
				alertFilled.QuoteCurrent_whenThisAlertFilled_deserializable = alertFilled.QuoteCurrent_whenThisAlertFilled.Clone_asCoreQuote();
			}

			try {
				alertFilled.FillPositionAffected_entryOrExit_respectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);
			} catch (Exception ex) {
				string msg = "REMOVE_FILLED_FROM_PENDING? DONT_USE_Bar.ContainsPrice()?";
				Assembler.PopupException(msg + msig, ex);
			}
			bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alertFilled, this, msig);
			if (removed == false) {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			Position positionOpenedAfterAlertFilled = null;
			Position positionClosedAfterAlertFilled = null;

			PositionList positionsOpenedAfterAlertFilled = new PositionList("positionsOpenedAfterAlertFilled", this.ExecutionDataSnapshot);
			PositionList positionsClosedAfterAlertFilled = new PositionList("positionsClosedAfterAlertFilled", this.ExecutionDataSnapshot);

			if (alertFilled.IsEntryAlert) {
				this.ExecutionDataSnapshot.PositionsMasterOpen_addNew(alertFilled.PositionAffected);
				positionOpenedAfterAlertFilled = alertFilled.PositionAffected;
				positionsOpenedAfterAlertFilled.AddOpened_step1of2(positionOpenedAfterAlertFilled, this, msig);
			} else {
				this.ExecutionDataSnapshot.MovePositionOpen_toClosed(alertFilled.PositionAffected);
				positionClosedAfterAlertFilled = alertFilled.PositionAffected;
				positionsClosedAfterAlertFilled.AddClosed(positionClosedAfterAlertFilled, this, msig);
			}

			bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;

// MOST_LIKELY_INVOKED_FROM_CALLBACK_WITH_PREVIOUS_BAR_INDEX AND_ONE_MORE_FILTER_PENDING_NOT_EARLIER_THAN_PLACED 
//			Bar barBarStreaming_nullUnsafe = this.Bars.BarStreaming_nullUnsafe;
//			List<Alert> alertsPendingAtCurrentBarSafeCopy = this.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "callbackAlertFilledMoveAroundInvokeScriptReenterablyUnprotected(WAIT)");
//			if (barBarStreaming_nullUnsafe != null && alertsPendingAtCurrentBarSafeCopy.Count > 0) {
//				this.ChartShadow.AlertsPendingStillNotFilledForBarAdd(barBarStreaming_nullUnsafe.ParentBarsIndex, alertsPendingAtCurrentBarSafeCopy);
//			}
			

			AlertList alertsNewAfterAlertFilled = new AlertList("alertsNewAfterAlertFilled", this.ExecutionDataSnapshot);
			PositionPrototype proto = alertFilled.PositionAffected.Prototype;
			if (proto != null) {
				// 0. once again, set ExitAlert to What was actually filled, because prototypeEntry created SL & TP, which were both written into ExitAlert;
				// so if we caught the Loss and SL was executed, position.ExitAlert will still contain TP if we don't set it here
				bool exitIsDifferent = alertFilled.PositionAffected.ExitAlert != null && alertFilled.PositionAffected.ExitAlert != alertFilled;
				if (alertFilled.IsExitAlert && exitIsDifferent) {
					alertFilled.PositionAffected.ExitAlertAttach(alertFilled);
				}
				// 1. alert.PositionAffected.Prototype.StopLossAlertForAnnihilation and TP will get assigned
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

					if (this.IsStrategyEmittingOrders) {
						Quote quoteHackForLive = quoteFilledThisAlertNullForLive;
						if (quoteHackForLive == null) {
							quoteHackForLive = alertFilled.QuoteCurrent_whenThisAlertFilled;	// unconditionally filled 130 lines above
						}
						this.EnrichAlerts_withQuoteCreated(alertsNewAfterExecSafeCopy, quoteHackForLive);
						this.OrderProcessor.Emit_createOrders_forScriptGeneratedAlerts_eachInNewThread(alertsNewAfterExecSafeCopy, setStatusSubmitting, true);
	
						// 3. Script using proto might move SL and TP which require ORDERS to be moved, not NULLs
						int twoMinutes = 120000;
						if (alertFilled.IsEntryAlert) {
							// there must be SL.OrderFollowed!=null and TP.OrderFollowed!=null
							if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed == null) {
								string msg = "StopLossAlert.OrderFollowed is NULL!!! CreateOrdersSubmitToBrokerAdapterInNewThreads() didnt do its job; engaging ManualResetEvent.WaitOne()";
								this.PopupException(msg);

								Stopwatch waitedForStopLossOrder = new Stopwatch();
								waitedForStopLossOrder.Start();

								if (this.BacktesterOrLivesimulator.ImRunningLivesim) {
									bool unpaused = this.Livesimulator.IsPaused_waitForever_untilUnpaused;
								} else {
									proto.StopLossAlert_forMoveAndAnnihilation.MreOrderFollowedIsAssignedNow.WaitOne(twoMinutes);
								}

								waitedForStopLossOrder.Stop();
								msg = "waited " + waitedForStopLossOrder.ElapsedMilliseconds + "ms for StopLossAlert.OrderFollowed";

								if (proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed == null) {
									msg += ": NO_SUCCESS still null!!!";
									this.PopupException(msg);
								} else {
									proto.StopLossAlert_forMoveAndAnnihilation.OrderFollowed.appendMessage(msg);
									this.PopupException(msg);
								}
							//} else {
							//	string msg = "you are definitely crazy, StopLossAlert.OrderFollowed is a single-threaded assignment";
							//	Assembler.PopupException(msg);
							}
	
							if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed == null) {
								string msg = "TakeProfitAlert.OrderFollowed is NULL!!! CreateOrdersSubmitToBrokerAdapterInNewThreads() didnt do its job; engaging ManualResetEvent.WaitOne()";
								this.PopupException(msg);
								Stopwatch waitedForTakeProfitOrder = new Stopwatch();
								waitedForTakeProfitOrder.Start();
								proto.TakeProfitAlert_forMoveAndAnnihilation.MreOrderFollowedIsAssignedNow.WaitOne(twoMinutes);
								waitedForTakeProfitOrder.Stop();
								msg = "waited " + waitedForTakeProfitOrder.ElapsedMilliseconds + "ms for TakeProfitAlert.OrderFollowed";
								if (proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed == null) {
									msg += ": NO_SUCCESS still null!!!";
									this.PopupException(msg);
								} else {
									proto.TakeProfitAlert_forMoveAndAnnihilation.OrderFollowed.appendMessage(msg);
									this.PopupException(msg);
								}
							//} else {
							//	string msg = "you are definitely crazy, TakeProfitAlert.OrderFollowed is a single-threaded assignment";
							//	Assembler.PopupException(msg);
							}
						}
						if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == false) {
							this.ChartShadow.AlertsPlaced_addRealtime(alertsNewAfterExecSafeCopy);
						}
					}
				}
			}

			if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				string msg = "AFTER_BACKTEST_HOOK_INVOKES_Performance.BuildStatsOnBacktestFinished()_AND_ReportersFormsManager.BuildReportFullOnBacktestFinished()";
				return;
			}

			ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quoteFilledThisAlertNullForLive, alertsNewAfterAlertFilled,
															 positionsOpenedAfterAlertFilled,
															 positionsClosedAfterAlertFilled,
															 null
															);
			using(pokeUnit_dontForgetToDispose) {
				//v1 this.AddPositionsToChartShadowAndPushPositionsOpenedClosedToReportersAsyncUnsafe(pokeUnit);
				if (positionOpenedAfterAlertFilled != null) {
					this.PerformanceAfterBacktest.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(positionOpenedAfterAlertFilled);
					//if (alertFilled.GuiHasTimeRebuildReportersAndExecution) {
						// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
						this.EventGenerator.RaiseOnBrokerFilledAlertsOpeningForPositions_step1of3(pokeUnit_dontForgetToDispose);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
					//}
				}
				if (positionClosedAfterAlertFilled != null) {
					this.PerformanceAfterBacktest.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(positionClosedAfterAlertFilled);
					if (alertFilled.GuiHasTimeRebuildReportersAndExecution) {
						// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
						this.EventGenerator.RaiseOnBrokerFilledAlertsClosingForPositions_step3of3(pokeUnit_dontForgetToDispose);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
					}
				}
				this.ChartShadow.PositionsRealtimeAdd(pokeUnit_dontForgetToDispose);

				// 4. Script event will generate a StopLossMove PostponedHook
				//NOW_INLINE this.invokeScriptEvents(alertFilled);
				if (this.Strategy.Script == null) return;
				try {
					try {
						this.ExecutionDataSnapshot.IsScriptRunningOnAlertFilledNonBlockingRead = true;
						this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnAlertFilled_callback(WAIT)");
						this.Strategy.Script.OnAlertFilled_callback(alertFilled);
					} finally {
						this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnAlertFilled_callback(WAIT)");
						this.ExecutionDataSnapshot.IsScriptRunningOnAlertFilledNonBlockingRead = false;
					}
				} catch (Exception e) {
					string msg = "fix your OnAlertFilledCallback() in script[" + this.Strategy.Script.StrategyName + "]"
						+ "; was invoked with alert[" + alertFilled + "]";
					this.PopupException(msg, e);
				}
				if (alertFilled.IsEntryAlert) {
					try {
						if (alertFilled.PositionAffected.Prototype != null) {
							try {
								this.ExecutionDataSnapshot.IsScriptRunningOnPositionOpenedPrototypeSlTpPlacedNonBlockingRead = true;
								this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionOpened_prototypeSlTpPlaced_callback(WAIT)");
								this.Strategy.Script.OnPositionOpened_prototypeSlTpPlaced_callback(alertFilled.PositionAffected);
							} finally {
								this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionOpened_prototypeSlTpPlaced_callback(WAIT)");
								this.ExecutionDataSnapshot.IsScriptRunningOnPositionOpenedPrototypeSlTpPlacedNonBlockingRead = false;
							}
						} else {
							try {
								this.ExecutionDataSnapshot.IsScriptRunningOnPositionOpenedNonBlockingRead = true;
								this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionOpened_callback(WAIT)");
								this.Strategy.Script.OnPositionOpened_callback(alertFilled.PositionAffected);
							} finally {
								this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionOpened_callback(WAIT)");
								this.ExecutionDataSnapshot.IsScriptRunningOnPositionOpenedNonBlockingRead = false;
							}
						}
					} catch (Exception e) {
						string msg = "fix your ExecuteOnPositionOpened() in script[" + this.Strategy.Script.StrategyName + "]"
							+ "; was invoked with PositionAffected[" + alertFilled.PositionAffected + "]";
						this.PopupException(msg, e);
					}
				} else {
					try {
						try {
							this.ExecutionDataSnapshot.IsScriptRunningOnPositionClosedNonBlockingRead = true;
							this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnPositionClosed_callback(WAIT)");
							this.Strategy.Script.OnPositionClosed_callback(alertFilled.PositionAffected);
						} finally {
							this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnPositionClosed_callback(WAIT)");
							this.ExecutionDataSnapshot.IsScriptRunningOnPositionClosedNonBlockingRead = false;
						}
					} catch (Exception e) {
						string msg = "fix your OnPositionClosedCallback() in script[" + this.Strategy.Script.StrategyName + "]"
							+ "; was invoked with PositionAffected[" + alertFilled.PositionAffected + "]";
						this.PopupException(msg, e);
					}
				}

				// reasons for (alertsNewAfterExec.Count > 0) include:
				// 2.1. PrototypeActivator::AlertFilledPlaceSlTpOrAnnihilateCounterparty
				// 2.2. Script.OnAlertFilledCallback(alert)
				// 2.3. Script.OnPositionOpenedPrototypeSlTpPlacedCallback(alert.PositionAffected)
				// 2.4. Script.OnPositionClosedCallback(alert.PositionAffected)

	//			if (pokeUnit.AlertsNew.Count > 0) {
	//				string msg = "WANNA_CREATE_AND_SEND_ORDERS???MISSING_IN_LIVESIM invokeScriptEvents_EASILY_ACTIVATES_POSITION_PROTOTYPE_CREATING_TWO_ALERTS_NEW!! I_THOUGHT_NEW_ALERTS_ARE_CREATED_UPSTACK_BUT_APPARENTLY_IM_WRONG";
	//				//Assembler.PopupException(msg, null, false);
	//			}
			}
		}
		void closePosition_withAlertClonedFromEntry_backtestEnded(Alert alert) {
			string msig = " closePositionWithAlertClonedFromEntryBacktestEnded():";
			this.removePendingExitAlert(alert, msig);
			bool checkPositionOpenNow = true;
			if (this.check_positionCanBeClosed(alert, msig, checkPositionOpenNow) == false) return;
			if (alert.FilledBar == null) {
				string msg = "BACKTEST_ENDED_ALERT_UNFILLED strategy[" + this.Strategy.ToString() + "] alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			//Alert alertExitClonedStub = alert.CloneToRefactor();
			//alertExitClonedStub.SignalName += " ClosePositionWithAlertClonedFromEntryBacktestEnded Forced";
			//alertExitClonedStub.Direction = MarketConverter.ExitDirectionFromLongShort(alert.PositionLongShortFromDirection);
			//// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Position.EntryAlert was filled!!!
			//alert.PositionAffected.ExitAlertAttach(alertExitClonedStub);
			alert.PositionAffected.FillExitWith(alert.FilledBar, alert.PriceEmitted, alert.Qty, 0, 0);
			//alertFilled.FillPositionAffectedEntryOrExitRespectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);

			bool absenseInPositionsOpenNowIsAnError = false;
			this.ExecutionDataSnapshot.MovePositionOpen_toClosed(alert.PositionAffected, absenseInPositionsOpenNowIsAnError);
		}
		bool check_positionCanBeClosed(Alert alert, string msig, bool checkPositionOpenNow = true) {
			if (alert.PositionAffected == null) {
				string msg = "can't close PositionAffected and remove Position from PositionsOpenNow"
					+ ": alert.PositionAffected=null for alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
				return false;
			}
			if (alert.IsExitAlert) {
				string msg = "Sorry I don't serve alerts.IsExitAlert=true, only .IsEntryAlert's: alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
				return false;
			}
			if (checkPositionOpenNow == true) {
				bool shouldRemove = this.ExecutionDataSnapshot.PositionsOpenNow.Contains(alert.PositionAffected, this, "check_positionCanBeClosed(WAIT)");

				if (alert.FilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only BarRelnoFilled==-1"
						+ " otherwize alert.FillPositionAffectedEntryOrExitRespectively() with throw: alert [" + alert + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
					return false;
				}
				if (alert.PositionAffected.ExitFilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only alert.PositionAffected.ExitBar==-1"
						+ " otherwize PositionAffected.FillExitAlert() will throw: alert [" + alert + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
					return false;
				}
			} else {
				Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreaming_nullUnsafeCloneReadonly : alert.Bars.BarStaticLast_nullUnsafe;
				if (alert.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: can't do my job: alert.PositionAffected.EntryBar!=-1 for alert [" + alert + "]";
					//throw new Exception(msig + msg);
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
					//return;
				} else {
					string msg = "Forcibly closing at EntryBar=[" + barFill + "]";
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
				}

			}
			return true;
		}
	}
}
