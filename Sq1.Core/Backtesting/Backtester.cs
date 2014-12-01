using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Backtesting {
	public class Backtester {
		public ScriptExecutor Executor;

		public Bars BarsOriginal { get; private set; }
		public Bars BarsSimulating { get; private set; }
		public BacktestDataSource BacktestDataSource;
		BacktestQuoteBarConsumer backtestQuoteBarConsumer;

		bool setBacktestAborted;
		public ManualResetEvent RequestingBacktestAbort;		// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		public ManualResetEvent BacktestAborted;
		public ManualResetEvent BacktestIsRunning;
		public ManualResetEvent BacktestCompletedQuotesCanGo;
		

		public int BarsSimulatedSoFar { get; private set; }
		public int QuotesTotalToGenerate { get {
				if (this.BarsOriginal == null) return -1;
				return this.BarsOriginal.Count * this.QuotesGenerator.QuotePerBarGenerates;
			} }
		public int QuotesGeneratedSoFar { get { return BarsSimulatedSoFar * this.QuotesGenerator.QuotePerBarGenerates; } }
		public BacktestMode BacktestMode { get; private set; }
		public BacktestQuotesGenerator QuotesGenerator { get; private set; }

		public string ProgressStats { get {
				if (this.QuotesGenerator == null) return "QuotesGenerator=null";
				return this.QuotesGeneratedSoFar + " / " + this.QuotesTotalToGenerate;
			} }
		public bool IsBacktestingNow { get { return this.BacktestIsRunning.WaitOne(0); } }
		public bool WasBacktestAborted { get {
				if (QuotesGenerator == null) return false;
				bool signalled = this.BacktestAborted.WaitOne(0);
				return signalled;
			} }

		private Backtester() {
			setBacktestAborted = false;
			RequestingBacktestAbort = new ManualResetEvent(false);
			BacktestAborted = new ManualResetEvent(false);
			BacktestIsRunning = new ManualResetEvent(false);
			BacktestCompletedQuotesCanGo = new ManualResetEvent(true);
			backtestQuoteBarConsumer = new BacktestQuoteBarConsumer(this);
			BacktestDataSource = new BacktestDataSource();
		}
		public Backtester(ScriptExecutor executor) : this() {
			this.Executor = executor;
			if (this.Executor.Strategy == null) return;
			if (this.Executor.Strategy.Script == null) return;
			this.Initialize(this.Executor.Strategy.ScriptContextCurrent.BacktestMode);
		}
		public void Initialize(BacktestMode mode = BacktestMode.FourStrokeOHLC) {
			this.BacktestMode = mode;
			if (this.QuotesGenerator != null && this.QuotesGenerator.BacktestModeSuitsFor == mode) {
				return;
			}
			switch (this.BacktestMode) {
				case BacktestMode.FourStrokeOHLC:
					this.QuotesGenerator = new BacktestQuotesGeneratorFourStroke(this);
					break;
				default:
					string msg = "NYI: [" + this.Executor.Strategy.ScriptContextCurrent.BacktestMode + "]RunSimulation";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
			}
		}
		public void RunSimulation() {
			if (this.QuotesGenerator == null) {
				string msg = "backtestQuotesGenerator is not chosen / instantiated";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
				//this.Initialize(BacktestMode.FourStrokeOHLC);
			}
			this.ExceptionsHappenedSinceBacktestStarted = 0;
			this.substituteBarsAndRunSimulation();
		}
		public void AbortRunningBacktestWaitAborted(string whyAborted, int millisecondsToWait = 1000) {
			if (this.IsBacktestingNow == false) return;

			bool abortIsAlreadyRequested = this.RequestingBacktestAbort.WaitOne(0);
			this.RequestingBacktestAbort.Set();
			bool abortAlreadyRequestedNow = this.RequestingBacktestAbort.WaitOne(0);

			string msig = " whyAborted=[" + whyAborted + "]: Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]";

			string msg = "BACKTEST_ABORTING";
			Assembler.PopupException(msg + msig, null, false);

			bool aborted = this.BacktestAborted.WaitOne(millisecondsToWait);
			msg = (aborted) ? "BACKTEST_ABORTED" : "BACKTESTER_DIDNT_ABORT_WITHIN_SECONDS[" + millisecondsToWait + "]";
			Assembler.PopupException(msg + msig, null, false);
		}
		public void WaitUntilBacktestCompletes() {
			if (this.IsBacktestingNow == false) return;
			this.BacktestCompletedQuotesCanGo.WaitOne();
		}
		public void SetRunningFalseNotifyWaitingThreadsBacktestCompleted() {
			this.BacktestIsRunning.Reset();
			//if (this.IsBacktestingNow) {
			//	string msg = "IN_ORDER_TO_SIGNAL_FLAGGED_I_HAVE_TO_SET_INSTEAD_OF_RESET";
			//	Debugger.Break();
			//}

			if (this.Executor.ChartShadow != null) this.Executor.ChartShadow.BacktestIsRunning.Reset();
			// Calling ManualResetEvent.Set opens the gate,
			// allowing any number of threads calling WaitOne to be let through
			this.BacktestCompletedQuotesCanGo.Set();
		}
		public int ExceptionsHappenedSinceBacktestStarted = 0;
		public void AbortBacktestIfExceptionsLimitReached() {
			if (this.IsBacktestingNow == false) return;
			this.ExceptionsHappenedSinceBacktestStarted++;
			if (this.ExceptionsHappenedSinceBacktestStarted < this.Executor.Strategy.ExceptionsLimitToAbortBacktest) return;
			this.AbortRunningBacktestWaitAborted("AbortBacktestIfExceptionsLimitReached[" + this.Executor.Strategy.ExceptionsLimitToAbortBacktest + "]");
		}
		void substituteBarsAndRunSimulation() {
			if (null == this.Executor.Bars) {
				Assembler.PopupException("EXECUTOR_LOST_ITS_BARS_NONSENSE null==this.Executor.Bars SubstituteBarsAndRunSimulation()");
				return;
			}
			if (this.Executor.Bars.Count < 1) return;
			try {
				this.simulationPreBarsSubstitute();
	
				int repaintableChunk = (int)(this.BarsOriginal.Count / 20);
				if (repaintableChunk <= 0) repaintableChunk = 1;
				
				for (int barNo = 0; barNo < this.BarsOriginal.Count; barNo++) {
					Bar bar = this.BarsOriginal[barNo];

					bool abortRequested = this.RequestingBacktestAbort.WaitOne(0);
					if (abortRequested) {
						setBacktestAborted = true;
						break;
					}
					this.generateQuotesForBarAndPokeStreaming(bar);
					this.BarsSimulatedSoFar++;
					if (this.BarsSimulatedSoFar % repaintableChunk == 0) {
						this.Executor.EventGenerator.RaiseBacktesterSimulatedChunkStep3of4();
					}
					
					//MAKE_EXCEPTIONS_FORM_INSERT_DELAYED!!! Application.DoEvents();	// otherwize UI becomes irresponsible;
					//COMMENTED_OUT_TO_SIMULATE_PROFILER_BEHAVIOUR MORE_EXCEPTIONS_DISPLAYED_IN_EXCEPTIONS_FORM_WOW Application.DoEvents();
					#if DEBUG
					// UNCOMMENTED_FOR_SHARP_DEVELOP_TO_NOT_FREAK_OUT_FULLY_EXPAND_LOCAL_VARIABLES_AT_BREAKPOINTS_RANDOMLY_CONTINUE_ETC IRRELATED_TO_EXCEPTIONS_THERE_WAS_NONE
					// COMMENTED_OUT_#DEVELOP_FREAKS_OUT_WHEN_YOU_MOVE_INNER_WINDOWS_SPLITTER Application.DoEvents();
					// UNCOMMENTED_TO_KEEP_MOUSE_OVER_SLIDERS_RESPONSIVE
					// NOT_NEEDED_WHEN_BACKTESTER_STARTS_WITHOUT_PARAMETERS Application.DoEvents();
					#endif
				}

				// see Indicator.DrawValue() "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED"
				// this.QuotesGenerator.InjectFakeQuoteInNonExistingNextBarToSolidifyLastStaticBar(this.Executor.Bars.BarStaticLast);
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			} finally {
				// I won't get here if youser :) closed an app during the backtest / optimization;
				// but in pre-Backtest BacktestContextInitialize() sets preBacktestBars!=null so this.Streaming=true won't save Strategy.ScriptContextCurrent.ChartStreaming
				this.simulationPostBarsRestore();
				this.closePositionsLeftOpenAfterBacktest();
			}
		}
		void closePositionsLeftOpenAfterBacktest() {
			foreach (Alert alertPending in this.Executor.ExecutionDataSnapshot.AlertsPendingSafeCopy) {
				try {
					//if (alertPending.IsEntryAlert) {
					//	this.Executor.ClosePositionWithAlertClonedFromEntryBacktestEnded(alertPending);
					//} else {
					//	string msg = "checkPositionCanBeClosed() will later interrupt the flow saying {Sorry I don't serve alerts.IsExitAlert=true}";
					//	this.Executor.RemovePendingExitAlertPastDueClosePosition(alertPending);
					//}
					//bool removed = this.Executor.ExecutionDataSnapshot.AlertsPendingRemove(alertPending);
					this.Executor.AlertKillPending(alertPending);
				} catch (Exception e) {
					string msg = "NOT_AN_ERROR BACKTEST_POSITION_FINALIZER: check innerException: most likely you got POSITION_ALREADY_CLOSED on counterparty alert's force-close?";
					#if DEBUG
					Debugger.Break();
					#endif
					this.Executor.PopupException(msg, e);
				}
			}
			if (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0) {
				string msg = "KILLING_LEFTOVER_ALERTS_DIDNT_WORK_OUT snap.AlertsPending.Count["
					+ this.Executor.ExecutionDataSnapshot.AlertsPending.Count + "] should be ZERO";
				Assembler.PopupException(msg, null, false);
			}

			foreach (Position positionOpen in this.Executor.ExecutionDataSnapshot.PositionsOpenNowSafeCopy) {
				//v1
				//List<Alert> alertsSubmittedToKill = this.Executor.Strategy.Script.PositionCloseImmediately(positionOpen, );
				//v2
				//this.Executor.Strategy.Script.ExitAtMarket(this.Executor.Bars.BarStaticLastNullUnsafe, positionOpen, "BACKTEST_ENDED_EXIT_FORCED");
				// BETTER WOULD BE KILL PREVIOUS PENDING ALERT FROM A CALBACK AFTER MARKET EXIT ORDER GETS FILLED, IT'S UNRELIABLE EXIT IF WE KILL IT HERE
				// LOOK AT EMERGENCY CLASSES, SOLUTION MIGHT BE THERE ALREADY
				//List<Alert> alertsSubmittedToKill = this.Executor.Strategy.Script.PositionKillExitAlert(positionOpen, "BACKTEST_ENDED_EXIT_FORCED");
				//v3
				//this.Executor.ExecutionDataSnapshot.MovePositionOpenToClosed(positionOpen);
				//v4
				if (positionOpen.ExitAlert == null) continue;
				try {
					this.Executor.RemovePendingExitAlertPastDueClosePosition(positionOpen.ExitAlert);
				} catch (Exception ex) {
					Assembler.PopupException("closePositionsLeftOpenAfterBacktest()", ex, false);
				}
			}
			if (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0) {
				string msg = "CLOSING_LEFTOVER_POSITIONS_DIDNT_WORK_OUT snap.PositionsOpenNow.Count["
					+ this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
		}		
		void simulationPreBarsSubstitute() {
			bool shouldRaise = false;
			if (this.BarsOriginal == null) {
				shouldRaise = true;
			} else {
				if (this.BarsOriginal != this.Executor.Bars) shouldRaise = true;
			}
			try {
				if (shouldRaise) {
					this.BarsOriginal = this.Executor.Bars;
					this.Executor.EventGenerator.RaiseBacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4();
				}

				this.BarsSimulating = this.BarsOriginal.CloneNoBars("BACKTEST for " + this.BarsOriginal);
				
				#region candidate for this.BacktestDataSourceBuildFromUserSelection()
				BacktestSpreadModeler spreadModeler;
				// kept it on the surface and didn't pass ScriptContextCurrent.SpreadModelerPercent to "new BacktestDataSource()" because later BacktestDataSource:
				// 1) will support different SpreadModelers with not only 1 parameter like SpreadModelerPercentage;
				// 2) will support different BacktestModes like 12strokes, not only 4Stroke 
				// 3) will poke StreamingProvider-derived implementations 12 times a bar with platform-generated quotes for backtests with regulated poke delay
				// 4) will need to be provide visualized 
				// v1 this.BacktestDataSource.BacktestStreamingProvider.InitializeSpreadModelerPercentage(this.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
				// v2 UI-controlled in the future, right now the stub  
				ContextScript ctx = this.Executor.Strategy.ScriptContextCurrent;
				string msig = "Strategy[" + this.Executor.Strategy + "].ScriptContextCurrent[" + ctx + "]";
				switch (ctx.SpreadModelerClassName) {
					case "BacktestSpreadModelerPercentage":
						spreadModeler = new BacktestSpreadModelerPercentage(this.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
					default:
						string msg = "SPREAD_MODELER_NOT_YET_SUPPORTED[" + ctx.SpreadModelerClassName + "]"
							+ ", instantiatind default BacktestSpreadModelerPercentage("
							+ this.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent + ")";
						Assembler.PopupException(msg + msig);
						spreadModeler = new BacktestSpreadModelerPercentage(this.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
				}
				this.BacktestDataSource.Initialize(this.BarsSimulating, spreadModeler);
				#endregion

				this.BarsSimulating.DataSource = this.BacktestDataSource;

				this.BacktestDataSource.StreamingProvider.ConsumerQuoteSubscribe(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer, false);
				this.BacktestDataSource.StreamingProvider.ConsumerBarSubscribe(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer, false);
				
				this.Executor.BacktestContextInitialize(this.BarsSimulating);
				
				// consumers will expect this.BarsOriginal != null
				this.Executor.EventGenerator.RaiseBacktesterSimulationContextInitializedStep2of4();
			} catch (Exception e) {
				string msg = "PreBarsSubstitute(): Backtester caught a long beard...";
				this.Executor.PopupException(msg, e);
			} finally {
				this.BarsSimulatedSoFar = 0;
				setBacktestAborted = false;
				this.BacktestAborted.Reset();
				this.RequestingBacktestAbort.Reset();
				this.BacktestIsRunning.Set();
				//if (this.IsBacktestingNow == false) {
				//	string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
				//	Debugger.Break();
				//}

				//COPIED_UPSTACK_FOR_BLOCKING_MOUSEMOVE_AFTER_BACKTEST_NOW_CLICK__BUT_ALSO_STAYS_HERE_FOR_SLIDER_CHANGE_NON_INVALIDATION
				//WONT_BE_RESET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
				if (this.Executor.ChartShadow != null) this.Executor.ChartShadow.BacktestIsRunning.Set();
				// Calling ManualResetEvent.Reset closes the gate.
				// Threads that call WaitOne on a closed gate will block
				this.BacktestCompletedQuotesCanGo.Reset();
			}
		}
		void simulationPostBarsRestore() {
			try {
				this.BacktestDataSource.StreamingProvider.ConsumerQuoteUnSubscribe(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);
				this.BacktestDataSource.StreamingProvider.ConsumerBarUnSubscribe(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);

				this.Executor.BacktestContextRestore();
				this.Executor.EventGenerator.RaiseBacktesterSimulatedAllBarsStep4of4();
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			} finally {
				// Calling ManualResetEvent.Set opens the gate,
				// allowing any number of threads calling WaitOne to be let through
				//moved to this.NotifyWaitingThreads()
				//this.BacktestCompletedQuotesCantGo.Set();
				if (setBacktestAborted) {
					this.BacktestAborted.Set();
					this.RequestingBacktestAbort.Reset();
				}
			}
		}
		void generateQuotesForBarAndPokeStreaming(Bar bar2simulate) {
			if (bar2simulate == null) return;
			if (bar2simulate.IsBarStreaming && double.IsNaN(bar2simulate.Open)) {
				string msg = "it's ok for Bars.LastBar from StaticProvider to have no PartialValues;"
					+ " filled by Streaming, NA for Backtest, skipping LastBar";
				//throw new Exception(msg);
				Assembler.PopupException(msg);
				return;
			}

			List<QuoteGenerated> quotesGenerated = this.QuotesGenerator.GenerateQuotesFromBarAvoidClearing(bar2simulate);
			if (quotesGenerated == null) return;
			for (int i = 0; i < quotesGenerated.Count; i++) {
				QuoteGenerated quote = quotesGenerated[i];
				
				#if DEBUG //TEST_EMEDDED
				if (quote.ParentBarSimulated.ParentBarsIndex != bar2simulate.ParentBarsIndex) {
					Debugger.Break();
				}

				// PREV_QUOTE_ABSNO_SHOULD_NOT_BE_LINEAR_CAN_CONTAIN_HOLES_DUE_TO_QUOTES_INJECTED_TO_FILL_ALERTS
				//QuoteGenerated quotePrev;
				//if (i > 0) {
				//	quotePrev = quotesGenerated[i-1];
				//	if (quote.Absno != quotePrev.Absno + 1) {
				//		//string msg = "IRRELEVANT since GenerateQuotesFromBar() has been upgraded to return SortedList<int, QuoteGenerated> instead of randomized List<QuoteGenerated>";
				//		string msg = "PREV_QUOTE_ABSNO_MUST_BE_LINEAR_WITHOUT_HOLES STILL_RELEVANT FIXME";
				//		//Debugger.Break();
				//	}
				//}
				#endif

				#if DEBUG //TEST_EMBEDDED GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #1/2
				if (bar2simulate.HighLowDistance > 0 && bar2simulate.HighLowDistance > quote.Spread
						&& i > 0 && bar2simulate.ContainsBidAskForQuoteGenerated(quote) == false) {
					Debugger.Break();
				}
				#endif

				int pendingsToFillInitially = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				List<QuoteGenerated> quotesInjected = this.QuotesGenerator.InjectQuotesToFillPendingAlerts(quote, bar2simulate);
				if (quotesInjected.Count > 0 && quote.AbsnoPerSymbol != this.QuotesGenerator.LastGeneratedAbsnoPerSymbol) {
					//DONT_FORGET_TO_ASSIGN_LATEST_ABSNO_TO_QUOTE_TO_REACH
					#if DEBUG //TEST_EMBEDDED
					if (quotesInjected.Count != this.QuotesGenerator.LastGeneratedAbsnoPerSymbol - quote.AbsnoPerSymbol) {
						string msg = "InjectQuotesToFillPendingAlerts()_INCREMENTED_QUOTE_ABSNO";
						//Debugger.Break();
					}
					#endif
					if (quote.AbsnoPerSymbol != this.QuotesGenerator.LastGeneratedAbsnoPerSymbol) {	//DONT_FORGET_TO_ASSIGN_LATEST_ABSNO_TO_QUOTE_TO_REACH
						string msg = "SO_WHY_ABSNO_MUST_BE_SET_HERE_AND_CANT_BE_SET_IN_QUOTE.CTOR?...";
						quote.AbsnoPerSymbol  = this.QuotesGenerator.LastGeneratedAbsnoPerSymbol;
					}
				}
				
				#if DEBUG //TEST_EMBEDDED
				if (quotesInjected.Count == 0) {
					string msg = "SEEMS_ONLY_STOP_ALERTS_FAR_BEYOND_TARGET_ARE_ON_THE_WAY; pendingsToFillInitially[" + pendingsToFillInitially + "]"
						+ "STOPS_ARE_TOO_FAR OR_WILL_BE_FILLED_NEXT_THING_UPSTACK";
				}
				if (quotesInjected.Count == pendingsToFillInitially) {
					int pendingsLeft = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;
					string msg = "GENERATED_EXACTLY_AS_MANY_AS_PENDINGS; PENDINGS_UNFILLED_LEFT_" + pendingsLeft;
				}
				int pendingsStrategyJustGenerated = quotesInjected.Count - pendingsToFillInitially;
				if (pendingsStrategyJustGenerated > 0) {
					string msg = "SEEMS_STRATEGY_GENERATED_NEW_ALERTS_ON_NEW_QUOTES"
						+ "; quotesInjected.Count[" + quotesInjected.Count + "] > pendingsToFillInitially[" + pendingsToFillInitially + "]";
				}
				if (pendingsStrategyJustGenerated < 0) {
					string msg = "STOP_ALERTS_IGNORED_OTHERS_FILLED";
				}
				#endif

				int pendingsLeftAfterInjected = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;

				this.BacktestDataSource.BacktestStreamingProvider.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate);
				quote.WentThroughStreamingToScript = true;

				//nothing poductive below, only breakpoint placeholders
				#if DEBUG //TEST_EMEDDED
				int pendingsLeftAfterTargetQuoteGenerated = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingsToFillInitially == 0) continue;

				int pendingsFilledByInjected = pendingsLeftAfterInjected - pendingsToFillInitially;
				if (pendingsFilledByInjected > 0) {
					string msg = "SEEMS_LIKE_INJECTING_DOES_ITS_JOB; pendingsFilledByInjected[" + pendingsFilledByInjected + "]";
				}
				int targetQuoteIsntExpectedToFillAnything = pendingsLeftAfterTargetQuoteGenerated - pendingsLeftAfterInjected;
				if (targetQuoteIsntExpectedToFillAnything > 0) {
					string msg = "SEEMS_LIKE_TARGET_QUOTE_HAD_OWN_FILL POSSIBLE_BUT_UNLIKELY";
				}
				#endif
			}
		}
	}
}
