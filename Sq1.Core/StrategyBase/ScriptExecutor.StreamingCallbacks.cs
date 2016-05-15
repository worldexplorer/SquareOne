using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		Quote quoteExecutedLast;
		Bar barStatic_lastExecuted;


		bool iShould_returnWithoutScriptInvocation_untilMarketOpen_orClearingFinished() {
			bool goFlatNow = false;

			int priorTo_marketClose_clearing_sec = this.Bars.SymbolInfo.GoFlat_priorTo_marketClose_clearing_sec;
			if (priorTo_marketClose_clearing_sec == 0) return goFlatNow;

			string reasonToGoFlat = this.Bars.MarketInfo.GetReason_ifMarket_closedOrSuspended_secondsFromNow(priorTo_marketClose_clearing_sec);
			if (string.IsNullOrEmpty(reasonToGoFlat)) return goFlatNow;

			goFlatNow = true;
			Assembler.PopupException(reasonToGoFlat, null, false);
			
			var really = this.CloseAllOpenPositions_killAllPendingAlerts();

			return goFlatNow;
		}

		void scriptInvoke_Pre_checkThrow_both_onNewQuote_onNewBar(Quote quoteForAlertsCreated) {
			string msig = "I_REFUSE_TO_EXECUTE_SCRIPT: ";
			string ret = "";
			if (quoteForAlertsCreated	== null) ret += "QUOTE_IS_NULL ";
			if (this.Strategy			== null) ret += "STRATEGY_IS_NULL ";
			if (this.Strategy.Script	== null) ret += "SCRIPT_IS_NULL ";
			if (string.IsNullOrEmpty(ret)) return;	// peace!
			throw new Exception(msig + ret);		// war :(
		}

		//public ReporterPokeUnit ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barLastFormed, Quote quote_fromStreaming) { throw new NotImplementedException(); }
		//public ReporterPokeUnit ConsumeQuoteOfStreamingBar(Quote quote_fromStreaming) { throw new NotImplementedException(); }

		public void InvokeIndicators_onNewBar_onNewQuote(Bar barStaticLastFormed, Quote quote_fromStreaming, bool onNewQuoteTrue_onNewBarFalse) {
			string msig = "InvokeIndicators_onNewBar_onNewQuote()";
			foreach (Indicator indicator in this.Strategy.Script.IndicatorsByName_reflectedCached_primary.Values) {
				if (onNewQuoteTrue_onNewBarFalse) {
					try {
						indicator.OnNewQuote(quote_fromStreaming);
					} catch (Exception ex) {
						Assembler.PopupException("INDICATOR_ON_NEW_QUOTE " + indicator.ToString(), ex);
					}
				} else {
					try {
						indicator.OnBarStaticLastFormed_whileStreamingBar_withOneQuote_alreadyAppended(barStaticLastFormed);
					} catch (Exception ex) {
						Assembler.PopupException("INDICATOR_ON_NEW_BAR " + indicator.ToString(), ex, false);
					}
				}
			}
		}

		public ReporterPokeUnit InvokeScript_onNewBar_onNewQuote(Quote quote_fromStreaming, bool onNewQuoteTrue_onNewBarFalse = true) {
			string msig = "InvokeScript_onNewBar_onNewQuote(WAIT)";
			ReporterPokeUnit ret = null;

			try {
				this.scriptInvoke_Pre_checkThrow_both_onNewQuote_onNewBar(quote_fromStreaming);
			} catch(Exception ex) {
				Assembler.PopupException("preScriptInvoke_onNewBar", ex);
				return ret;		//null here ReporterPokeUnit
			}

			this.ExecutionDataSnapshot.Clear_priorTo_InvokeScript_onNewBar_onNewQuote();

			bool returnWithoutScriptInvocation = this.iShould_returnWithoutScriptInvocation_untilMarketOpen_orClearingFinished();
			if (returnWithoutScriptInvocation) return ret;		//null here ReporterPokeUnit

			string scriptInvocationError = "";
			if (onNewQuoteTrue_onNewBarFalse == true) {
				scriptInvocationError = this.invokeScript_onNewQuote_indicatorsAlready(quote_fromStreaming);
				if (this.ExecutionDataSnapshot.AlertsPending.Count > 0) {
					this.fillPendings_onEachQuote_onlyForLivesimBrokerDefault(quote_fromStreaming);
				}
			} else {
				scriptInvocationError = this.invokeScript_onNewBar_indicatorsAlready(quote_fromStreaming);
			}
			if (string.IsNullOrEmpty(scriptInvocationError) == false) {
				Assembler.PopupException(scriptInvocationError + msig);
				if (this.BacktesterOrLivesimulator.ImRunningLivesim) {
					this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted(scriptInvocationError, 10);
				}
				return ret;		//null here ReporterPokeUnit
			}

			// for backtest and LivesimDefault, AlertsNew will be empty by now because of this.fillPendings_onEachQuote_onlyForLivesimBrokerDefault(quote_fromStreaming)
			// for Live and ownLivesimImplementation, we will go through OrderProcessor to submit via BrokerAdapter.OverrideMe
			ret = this.scriptInvoke_Post_dealWithNewAlerts_fillPendings_killDoomed_emitOrders_both_onNewQuote_onNewBar(quote_fromStreaming);
			return ret;
		}

		ReporterPokeUnit scriptInvoke_Post_dealWithNewAlerts_fillPendings_killDoomed_emitOrders_both_onNewQuote_onNewBar(Quote quoteBoundAttached_toEnrichAlerts) {
			string msig = "InvokeScript_onNewBar_onNewQuote(WAIT)";
			ReporterPokeUnit ret = null;
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
			List<Alert> alertsPending_atCurrentBar_safeCopy = this.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, msig);
			if (barStreaming_nullUnsafe != null && alertsPending_atCurrentBar_safeCopy.Count > 0) {
				this.ChartShadow.AlertsPendingStillNotFilledForBarAdd(barStreaming_nullUnsafe.ParentBarsIndex, alertsPending_atCurrentBar_safeCopy);
			}

			List<Alert> alertsDoomed_afterExec_safeCopy = this.ExecutionDataSnapshot.AlertsDoomed.SafeCopy(this, msig);
			if (alertsDoomed_afterExec_safeCopy.Count > 0) {
				//v1
				//if (this.IsStrategyEmittingOrders) {
				//    this.OrderProcessor.Emit_alertsPending_kill(alertsDoomed_afterExec_safeCopy);
				//}
				//v2
				if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
					string msg2 = "BacktestBrokerAdapter just removes doomed from AlertsPending";
					int removedFromPendings = this.DataSource_fromBars.BrokerAdapter.AlertPendings_kill(alertsDoomed_afterExec_safeCopy);
				} else {
					string msg1 = "QuikBrokerAdapter Emits Orders and waits for (simulated for LivesimBrokerDefault) callback";
					if (this.IsStrategyEmittingOrders) {
						int emitted = this.DataSource_fromBars.BrokerAdapter.AlertPendings_kill(alertsDoomed_afterExec_safeCopy);
					}
				}
			}

			List<Alert> alertsNew_afterExec_safeCopy = this.ExecutionDataSnapshot.AlertsNewAfterExec.SafeCopy(this, msig);

			if (this.ChartShadow != null) {
				//bool guiHasTime = false;
				foreach (Alert alert in alertsNew_afterExec_safeCopy) {
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
			if (alertsNew_afterExec_safeCopy.Count > 0) {
				this.EnrichAlerts_withQuoteCreated(alertsNew_afterExec_safeCopy, quoteBoundAttached_toEnrichAlerts);
				//bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;

				// for backtest only => btnEmirOrders.Checked isn't analyzed at all
				if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNew_afterExec_safeCopy);
					this.ExecutionDataSnapshot.AlertsNewAfterExec.Clear(this, msig);
					return ret;		//null here ReporterPokeUnit
				}

				// for 1) LivesimStreamingDefault + DONT_Emit, 2) LivesimStreamingQuik + DONT_Emit
				if (this.BacktesterOrLivesimulator.ImRunningLivesim && this.IsStrategyEmittingOrders == false) {
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNew_afterExec_safeCopy);
					this.ExecutionDataSnapshot.AlertsNewAfterExec.Clear(this, msig);
					return ret;		//null here ReporterPokeUnit
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
					ordersEmitted = this.OrderProcessor.Emit_createOrders_forScriptGeneratedAlerts_eachInNewThread(alertsNew_afterExec_safeCopy
						, true // setStatusSubmitting
						, true);
					//MOVED_TO_ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()
					// ^^^ this.DataSource.UnPausePumpingFor(this.Bars, true);	// ONLY_DURING_DEVELOPMENT__FOR_#D_TO_HANDLE_MY_BREAKPOINTS

					foreach (Alert alert in alertsNew_afterExec_safeCopy) {
						if (alert.OrderFollowed != null) continue;
						bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alert, this, msig);
						if (removed == false) {
							string msg3 = "FAILED_TO_REMOVE_INCONSISTENT_ALERT_FROM_PENDING removed=" + removed;
							Assembler.PopupException(msg3 + msig);
						}
					}
					this.ChartShadow.AlertsPlaced_addRealtime(alertsNew_afterExec_safeCopy);
				}

			}

			if (this.BacktesterOrLivesimulator.WasBacktestAborted)				return  ret;		//null here ReporterPokeUnit
			if (this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing)	return  ret;		//null here ReporterPokeUnit
			
			
			ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quoteBoundAttached_toEnrichAlerts,
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
			if (pokeUnit_dontForgetToDispose.PositionsNow_plusOpened_plusClosedAfterExec_plusAlertsNew_count == 0) return ret;		//null here ReporterPokeUnit

			ret = pokeUnit_dontForgetToDispose;
			return ret;
		}

		void fillPendings_onEachQuote_onlyForLivesimBrokerDefault(Quote quoteForAlertsCreated) {
			LivesimBrokerDefault defaultOrderFiller = this.DataSource_fromBars.BrokerAdapter as LivesimBrokerDefault;
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
				string msg = "CLONED_ATTACHED_SORRY!!! I_MUST_HAVE_IT_UNATTACHED_HERE";
				//Assembler.PopupException(msg);
			}
			defaultOrderFiller.ConsumeQuoteBoundUnattached_toFillPending(quoteForAlertsCreated, willBeFilled);
		}

		string invokeScript_onNewBar_indicatorsAlready(Quote quoteForAlertsCreated) {
			string msig = " //ScriptExecutor.invokeScript_onNewBar(" + quoteForAlertsCreated + ")";
			string error = "";

			Bar barStaticLast = this.Bars.BarStaticLast_nullUnsafe;
			if (barStaticLast == null) {
				error = "FIXME__NO_BAR_TO_PROVOKE_onNewBar()";
				return error + msig;
			}
			if (this.barStatic_lastExecuted != null) {
				if (this.barStatic_lastExecuted.ParentBarsIndex == 5) {
					string aaa = "first hit";
				}
				if (quoteForAlertsCreated.ParentBarStreaming.ParentBars != this.Bars) {
					error = "QUOTE_ATTACHED_TO_NON_EXECUTOR_BARS";
					return error + msig;
				}
				int mustBeOne = barStaticLast.ParentBarsIndex - this.barStatic_lastExecuted.ParentBarsIndex;
				if (mustBeOne == 0) {
					error = "DUPE_IN_SCRIPT_INVOCATION__INDICATORS_WILL_COMPLAIN_TOO";
					return error + msig;
				}
				if (mustBeOne > 1) {
					int skipped = mustBeOne - 1;
					error = "HOLE_IN_SCRIPT_INVOCATION INDICATORS_WILL_COMPLAIN_TOO ALERTS_WILL_MISTMATCH_BARS ExecuteOnNewBar()_SKIPPED=[" + skipped + "]";
					return error + msig;
				}
			}

			string msig_imInvoking = "OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(WAIT)";
			try {
				try {
					this.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig_imInvoking);
					if (this.IsStreamingTriggeringScript) {
						// TODO: What about Script.onQuote, onAlertFilled, onPositionClosed/Opened? - should they also NOT be invoked?
						this.Strategy.Script.OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(barStaticLast);
					}
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig_imInvoking);
					this.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = false;
				}
				this.EventGenerator.RaiseOnStrategyExecuted_oneBar(barStaticLast);
			} catch (Exception ex) {
				msig_imInvoking = "OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(" + quoteForAlertsCreated + ")";
				error = " //Script[" + this.Strategy.Script.GetType().Name + "]." + msig_imInvoking;
				Assembler.PopupException(error, ex);
				error = ex.Message + error;
			} finally {
				this.barStatic_lastExecuted = barStaticLast;
			}
			return error;
		}

		string invokeScript_onNewQuote_indicatorsAlready(Quote quoteForAlertsCreated) {
			string msig = " //ScriptExecutor.invokeScript_onNewQuote(" + quoteForAlertsCreated + ")";
			string error = "";

			if (this.quoteExecutedLast != null) {
				long mustBeOne = quoteForAlertsCreated.AbsnoPerSymbol - this.quoteExecutedLast.AbsnoPerSymbol;
				if (mustBeOne == 0) {
					error = "DUPE_IN_SCRIPT_INVOCATION__INDICATORS_WONT_COMPLAIN_TOO";
					return error + msig;
				}
				if (mustBeOne > 1) {
					long skipped = mustBeOne - 1;
					error = "HOLE_IN_SCRIPT_INVOCATION";
					return error + msig;
				}
			} else {
				if (this.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false) {
					string msg4 = "IM_AT_APPRESTART_BACKTEST_PRIOR_TO_LIVE__HERE_I_SHOULD_HAVE_EXECUTED_ON_LASTBAR__DID_SO_AT_BRO_THIS_IS_NONSENSE!!!FINALLY";
				}
			}

			string msig_imInvoking = "OnNewQuoteOfStreamingBar_callback(WAIT)";
			try {
				try {
					this.ExecutionDataSnapshot.IsScriptRunningOnNewQuoteNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig_imInvoking);
					if (this.IsStreamingTriggeringScript) {
						this.Strategy.Script.OnNewQuoteOfStreamingBar_callback(quoteForAlertsCreated);
					}
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig_imInvoking);
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
				msig_imInvoking = "OnNewQuoteOfStreamingBar_callback(" + quoteForAlertsCreated + ")";
				error = " //Script[" + this.Strategy.Script.GetType().Name + "]." + msig_imInvoking;
				Assembler.PopupException(error, ex);
				error = ex.Message + error;
			}
			return error;
		}

		internal ReporterPokeUnit InvokeScript_onLevelTwoChanged_noNewQuote(LevelTwoFrozen levelTwoFrozen) {
			string msig = "InvokeScript_onNewBar_onNewQuote(WAIT)";
			ReporterPokeUnit ret = null;

			//this.ExecutionDataSnapshot.Clear_priorTo_InvokeScript_onNewBar_onNewQuote();

			bool returnWithoutScriptInvocation = this.iShould_returnWithoutScriptInvocation_untilMarketOpen_orClearingFinished();
			if (returnWithoutScriptInvocation) return ret;		//null here ReporterPokeUnit

			string msig_imInvoking = "OnLevelTwoChanged_noNewQuote_callback(WAIT)";
			try {
				try {
					this.ExecutionDataSnapshot.IsScriptRunningOnNewQuoteNonBlockingRead = true;
					this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, msig_imInvoking);
					if (this.IsStreamingTriggeringScript) {
						this.Strategy.Script.OnLevelTwoChanged_noNewQuote_callback(levelTwoFrozen);
					}
				} finally {
					this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, msig_imInvoking);
					this.ExecutionDataSnapshot.IsScriptRunningOnNewQuoteNonBlockingRead = false;
				}
				//this.EventGenerator.RaiseOnStrategyExecuted_onLevelTwoChanged(levelTwoFrozen);
			} catch (Exception ex) {
				msig_imInvoking = "OnLevelTwoChanged_noNewQuote_callback(" + levelTwoFrozen + ")";
				string error = " //Script[" + this.Strategy.Script.GetType().Name + "]." + msig_imInvoking;
				Assembler.PopupException(error, ex);
				//error = ex.Message + error;
			}

			//ret = scriptInvoke_Post_dealWithNewAlerts_fillPendings_killDoomed_emitOrders_both_onNewQuote_onNewBar(quote_fromStreaming);
			return ret;
		}
	}
}
