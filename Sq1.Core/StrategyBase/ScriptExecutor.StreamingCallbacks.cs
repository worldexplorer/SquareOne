using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		Quote quoteExecutedLast;
		Bar barStaticExecutedLast;

		public ReporterPokeUnit InvokeScript_onNewBar_onNewQuote(Quote quoteForAlertsCreated, bool onNewQuoteTrue_onNewBarFalse = true) {
			string msig = "InvokeScript_onNewBar_onNewQuote(WAIT)";

			if (this.Strategy == null) {
				string msg1 = "I_REFUSE_TO_EXECUTE_SCRIPT STRATEGY_IS_NULL";
				Assembler.PopupException(msg1 + msig);
				return null;
			}
			if (this.Strategy.Script == null) {
				string msg1 = "I_REFUSE_TO_EXECUTE_SCRIPT_IS_NULL";
				Assembler.PopupException(msg1 + msig);
				return null;
			}

			this.ExecutionDataSnapshot.Clear_priorTo_InvokeScript_onNewBar_onNewQuote();

			//if (quote != null) {
			bool invokedNoErrors = false;
			if (onNewQuoteTrue_onNewBarFalse == true) {
				invokedNoErrors = this.invokeScript_onNewQuote(quoteForAlertsCreated);
				if (this.ExecutionDataSnapshot.AlertsPending.Count > 0) {
					this.fillPendings_onEachQuote_skipIfNonLivesimBroker(quoteForAlertsCreated);
				}
			} else {
				invokedNoErrors = this.invokeScript_onNewBar(quoteForAlertsCreated);
			}
			if (invokedNoErrors == false) return null;

			string msg5 = "DONT_REMOVE_ALERT_SHOULD_LEAVE_ITS_TRAIL_DURING_LIFETIME_TO_PUT_UNFILLED_DOTS_ON_CHART";
			//int alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
			//int alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.AlertsPending.Count;
			//if (alertsDumpedForStreamingBar > 0) {
			//    msg += " DUMPED_AFTER_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
			//}

			// what's updated after Exec: non-volatile, kept un-reset until executor.Initialize():
			//this.ExecutionDataSnapshot.PositionsMaster.ByEntryBarFilled (unique)
			//this.ExecutionDataSnapshot.PositionsMaster
			//this.PositionsOnlyActive
			//this.AlertsMaster
			//this.AlertsNewAfterExec

			// what's new for this iteration: volatile, cleared before next Exec):
			//this.AlertsNewAfterExec
			//this.ExecutionDataSnapshot.PositionsOpenedAfterExec
			//this.ExecutionDataSnapshot.PositionsClosedAfterExec

			Bar barStreaming_nullUnsafe = this.Bars.BarStreaming_nullUnsafe;
			List<Alert> alertsPendingAtCurrentBarSafeCopy = this.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, msig);
			if (barStreaming_nullUnsafe != null && alertsPendingAtCurrentBarSafeCopy.Count > 0) {
				this.ChartShadow.AlertsPendingStillNotFilledForBarAdd(barStreaming_nullUnsafe.ParentBarsIndex, alertsPendingAtCurrentBarSafeCopy);
			}

			List<Alert> alertsDoomedAfterExec_safeCopy = this.ExecutionDataSnapshot.AlertsDoomed.SafeCopy(this, msig);
			if (alertsDoomedAfterExec_safeCopy.Count > 0) {
				if (this.IsStrategyEmittingOrders) {
					this.OrderProcessor.Emit_alertsPending_kill(alertsDoomedAfterExec_safeCopy);
				}
			}

			List<Alert> alertsNewAfterExec_safeCopy = this.ExecutionDataSnapshot.AlertsNewAfterExec.SafeCopy(this, msig);

			if (this.ChartShadow != null) {
				//bool guiHasTime = false;
				foreach (Alert alert in alertsNewAfterExec_safeCopy) {
					try {
						Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, alert);
						//if (guiHasTime == false) guiHasTime = alert.GuiHasTimeRebuildReportersAndExecution;
					} catch (Exception ex) {
						string msg = "ADDING_TO_DICTIONARY_MANY_TO_ONE";
						Assembler.PopupException(msg + msig, ex);
					}
				}
			} else {
				if (this.Sequencer.IsRunningNow == false) {
					string msg = "CHART_SHADOW_MUST_BE_NULL_ONLY_WHEN_OPTIMIZING__BACKTEST_AND_LIVESIM_MUST_HAVE_CHART_SHADOW_ASSOCIATED";
					Assembler.PopupException(msg + msig);
				}
			}

			List<Order> ordersEmitted = null;
			if (alertsNewAfterExec_safeCopy.Count > 0) {
				this.EnrichAlerts_withQuoteCreated(alertsNewAfterExec_safeCopy, quoteForAlertsCreated);
				//bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;

				// for backtest only => btnEmirOrders.Checked isn't analyzed at all
				if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNewAfterExec_safeCopy);
					this.ExecutionDataSnapshot.AlertsNewAfterExec.Clear(this, msig);
					return null;
				}

				// for 1) LivesimStreamingDefault + DONT_Emit, 2) LivesimStreamingQuik + DONT_Emit
				if (this.BacktesterOrLivesimulator.ImRunningLivesim && this.IsStrategyEmittingOrders == false) {
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNewAfterExec_safeCopy);
					this.ExecutionDataSnapshot.AlertsNewAfterExec.Clear(this, msig);
					return null;
				}

				// for LivesimStreamingDefault + EMIT, LivesimStreamingQuik + EMIT, Live with/without EMIT => goes here
				if (this.IsStrategyEmittingOrders) {
					string msg2 = "Breakpoint";
					//#D_FREEZE Assembler.PopupException(msg2);
					//Debugger.Break();
					//ContextScript ctx = this.Strategy.ScriptContextCurrent;

					//bool noNeedToUnpauseLivesimKozItsNeverPaused = this.Bars.DataSource is LivesimDataSource;
					bool noNeedToUnpauseLivesim_kozItsNeverPaused = this.DataSource_fromBars.BrokerAsLivesim_nullUnsafe != null;
					if (noNeedToUnpauseLivesim_kozItsNeverPaused == false) {
						//MOVED_TO_ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()
						// ^^^ this.DataSource.PausePumpingFor(this.Bars, true);		// ONLY_DURING_DEVELOPMENT__FOR_#D_TO_HANDLE_MY_BREAKPOINTS
						bool paused = this.Bars.DataSource.PumpingWaitUntilPaused(this.Bars, 0);
						if (paused == true) {
							string msg3 = "YES_I_PAUSED_THIS_PUMP_MYSELF_UPSTACK_IN_PumpPauseNeighborsIfAnyFor()"
								+ "YOU_WANT_ONE_STRATEGY_PER_SYMBOL_LIVE MAKE_SURE_YOU_HAVE_ONLY_ONE_SYMBOL:INTERVAL_ACROSS_ALL_OPEN_CHARTS PUMP_SHOULD_HAVE_BEEN_PAUSED_EARLIER"
								+ " in ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
							//Assembler.PopupException(msg3 + msig, null, false);
						}
					}
					ordersEmitted = this.OrderProcessor.Emit_createOrders_forScriptGeneratedAlerts_eachInNewThread(alertsNewAfterExec_safeCopy
						, true // setStatusSubmitting
						, true);
					//MOVED_TO_ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()
					// ^^^ this.DataSource.UnPausePumpingFor(this.Bars, true);	// ONLY_DURING_DEVELOPMENT__FOR_#D_TO_HANDLE_MY_BREAKPOINTS

					foreach (Alert alert in alertsNewAfterExec_safeCopy) {
						if (alert.OrderFollowed != null) continue;
						bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alert, this, msig);
						if (removed == false) {
							string msg3 = "FAILED_TO_REMOVE_INCONSISTENT_ALERT_FROM_PENDING removed=" + removed;
							Assembler.PopupException(msg3 + msig);
						}
					}
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNewAfterExec_safeCopy);
				}

			}

			if (this.BacktesterOrLivesimulator.WasBacktestAborted) return null;
			if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) return null;
			
			
			ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quoteForAlertsCreated,
												this.ExecutionDataSnapshot.AlertsNewAfterExec		.Clone(this, msig),
												this.ExecutionDataSnapshot.PositionsOpenedAfterExec	.Clone(this, msig),
												this.ExecutionDataSnapshot.PositionsClosedAfterExec	.Clone(this, msig),
												this.ExecutionDataSnapshot.PositionsOpenNow			.Clone(this, msig) );

			//MOVED_UPSTACK_TO_LivesimQuoteBarConsumer
			//if (this.Backtester.IsBacktestRunning && this.Backtester.IsLivesimRunning) {
			//	// FROM_ChartFormStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
			//	if (pokeUnit.PositionsOpenNow.Count > 0) {
			//		this.Performance.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(this.ExecutionDataSnapshot.PositionsOpenNow);
			//		if (guiHasTime) {
			//			this.EventGenerator.RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(pokeUnit);
			//		}
			//	}
			//}

			//if (this.Backtester.IsBacktestingNow) return pokeUnit;
			// NOPE PositionsMaster grows only in Callback: do this before this.OrderProcessor.CreateOrdersSubmitToBrokerAdapterInNewThreads() to avoid REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert
			// NOPE_REALTIME_FILLS_POSITIONS_ON_CALLBACK this.AddPositionsJustCreatedUnfilledToChartShadowAndPushToReportersAsyncUnsafe(pokeUnit);

			this.EventGenerator.RaiseOnStrategyExecuted_oneQuoteOrBar_ordersEmitted(ordersEmitted);
			
			// lets Execute() return non-null PokeUnit => Reporters are notified on quoteUpdatedPositions if !GuiIsBusy
			if (pokeUnit_dontForgetToDispose.PositionsNow_plusOpened_plusClosedAfterExec_plusAlertsNew_count == 0) return null;
			return pokeUnit_dontForgetToDispose;
		}

		void fillPendings_onEachQuote_skipIfNonLivesimBroker(Quote quoteForAlertsCreated) {
			LivesimBroker defaultOrderFiller = this.DataSource_fromBars.BrokerAdapter as LivesimBroker;
			if (defaultOrderFiller == null) return;

			if (defaultOrderFiller.DataSnapshot == null) {
				string msg = "CHANGED_DATASOURCE_DIDNT_INITIALIZE_FULLY RESTART defaultOrderFiller.DataSnapshot=NULL IM_STORING_SCHEDULED_PENDINGS_THERE";
				Assembler.PopupException(msg, null, false);
				return;
			}

			if (quoteForAlertsCreated.HasParentBarStreaming == false) {
				string msg = "CAN_YOU_HAVE_IT_BOUND_HERE?? EnrichQuote will complain";
				Assembler.PopupException(msg, null, false);
			}

			AlertList willBeFilled = this.ExecutionDataSnapshot.AlertsPending_thatQuoteWillFill(quoteForAlertsCreated);
			if (willBeFilled.Count == 0) {
				string msg1 = "NO_NEED_TO_PING_BROKER_EACH_NEW_QUOTE__EVERY_PENDING_ALREADY_SCHEDULED";
				return;
			}
			if (quoteForAlertsCreated.ParentBarStreaming != null) {
				string msg = "I_MUST_HAVE_IT_UNATTACHED_HERE";
				//Assembler.PopupException(msg);
			}
			defaultOrderFiller.ConsumeQuoteUnattached_toFillPending(quoteForAlertsCreated, willBeFilled);
		}

		bool invokeScript_onNewBar(Quote quoteForAlertsCreated) {
			bool invokedNoErrors = false;
			if (this.barStaticExecutedLast != null) {
				int mustBeOne = this.Bars.BarStaticLast_nullUnsafe.ParentBarsIndex - this.barStaticExecutedLast.ParentBarsIndex;
				if (mustBeOne == 0) {
					string msg2 = "DUPE_IN_SCRIPT_INVOCATION__INDICATORS_WILL_COMPLAIN_TOO";
					//Assembler.PopupException(msg2, null, false);
					return invokedNoErrors;
				}
				if (mustBeOne > 1) {
					int skipped = mustBeOne - 1;
					string msg2 = "HOLE_IN_SCRIPT_INVOCATION INDICATORS_WILL_COMPLAIN_TOO ALERTS_WILL_MISTMATCH_BARS ExecuteOnNewBar()_SKIPPED=[" + skipped + "]";
					Assembler.PopupException(msg2, null, false);
					return invokedNoErrors;
				}
			}
			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_ReflectedCached.Values) {
				try {
					indicator.OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended(this.Bars.BarStaticLast_nullUnsafe);
				} catch (Exception ex) {
					Assembler.PopupException("INDICATOR_ON_NEW_BAR " + indicator.ToString(), ex);
				}
			}

			string msig_imInvoking = "OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(WAIT)";
			try {
				try {
					this.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig_imInvoking);
					if (this.IsStreamingTriggeringScript) {
						// TODO: What about Script.onQuote, onAlertFilled, onPositionClosed/Opened? - should they also NOT be invoked?
						this.Strategy.Script.OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(this.Bars.BarStaticLast_nullUnsafe);
					}
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig_imInvoking);
					this.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = false;
				}
				this.EventGenerator.RaiseOnStrategyExecuted_oneBar(this.Bars.BarStaticLast_nullUnsafe);
				this.barStaticExecutedLast = this.Bars.BarStaticLast_nullUnsafe;
			} catch (Exception ex) {
				string msig = " //Script[" + this.Strategy.Script.GetType().Name
					+ "].OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(" + quoteForAlertsCreated + ")";
				this.PopupException(ex.Message + msig, ex);
			}
			invokedNoErrors = true;
			return invokedNoErrors;
		}

		bool invokeScript_onNewQuote(Quote quoteForAlertsCreated) {
			bool invokedNoErrors = false;
			if (this.quoteExecutedLast != null) {
				long mustBeOne = quoteForAlertsCreated.AbsnoPerSymbol - this.quoteExecutedLast.AbsnoPerSymbol;
				if (mustBeOne == 0) {
					string msg2 = "DUPE_IN_SCRIPT_INVOCATION__INDICATORS_WONT_COMPLAIN_TOO";
					Assembler.PopupException(msg2, null, false);
					return invokedNoErrors;
				}
				if (mustBeOne > 1) {
					long skipped = mustBeOne - 1;
					string msg2 = "HOLE_IN_SCRIPT_INVOCATION";
					Assembler.PopupException(msg2, null, false);
					return invokedNoErrors;
				}
			} else {
				if (this.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false) {
					string msg4 = "IM_AT_APPRESTART_BACKTEST_PRIOR_TO_LIVE__HERE_I_SHOULD_HAVE_EXECUTED_ON_LASTBAR__DID_SO_AT_BRO_THIS_IS_NONSENSE!!!FINALLY";
				}
			}
			//INDICATOR_ADDING_STREAMING_DOESNT_KNOW_FROM_QUOTE_WHAT_DATE_OPEN_TO_PUT
			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_ReflectedCached.Values) {
				try {
					indicator.OnNewStreamingQuote(quoteForAlertsCreated);
				} catch (Exception ex) {
					Assembler.PopupException("INDICATOR_ON_NEW_STREAMING_QUOTE " + indicator.ToString(), ex);
				}
			}

			try {
				try {
					this.ExecutionDataSnapshot.IsScriptRunningOnNewQuoteNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnNewQuoteOfStreamingBarCallback(WAIT)");
					if (this.IsStreamingTriggeringScript) {
						this.Strategy.Script.OnNewQuoteOfStreamingBar_callback(quoteForAlertsCreated);
					}
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnNewQuoteOfStreamingBarCallback(WAIT)");
					this.ExecutionDataSnapshot.IsScriptRunningOnNewQuoteNonBlockingRead = false;
				}
				//alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
				//if (alertsDumpedForStreamingBar > 0) {
				//	string msg = "ITS OK HERE since prev quote has created prototype-based alerts"
				//		+ "I WANT DUMP TO BE VALID ONLY IN onNewBar case only!!!"
				//		+ " " + alertsDumpedForStreamingBar + " alerts Dumped for " + quote;
				//}
				this.EventGenerator.RaiseOnStrategyExecuted_oneQuote(quoteForAlertsCreated);
			} catch (Exception ex) {
				string msig = " //Script[" + this.Strategy.Script.GetType().Name + "].OnNewQuoteOfStreamingBar_callback(" + quoteForAlertsCreated + ")";
				this.PopupException(ex.Message + msig, ex);
			}
			invokedNoErrors = true;
			return invokedNoErrors;
		}
	}
}
