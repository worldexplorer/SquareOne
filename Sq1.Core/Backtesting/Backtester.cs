using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using System.Diagnostics;

namespace Sq1.Core.Backtesting {
	public class Backtester {
		public ScriptExecutor Executor;

		public Bars BarsOriginal { get; private set; }
		public Bars BarsSimulating { get; private set; }
		public BacktestDataSource BacktestDataSource;
		BacktestQuoteBarConsumer backtestQuoteBarConsumer;

		bool setBacktestAborted = false;
		public ManualResetEvent RequestingBacktestAbort = new ManualResetEvent(false);
		public ManualResetEvent BacktestAborted = new ManualResetEvent(false);
		// Calling ManualResetEvent.Set opens the gate,
		// allowing any number of threads calling WaitOne to be let through
		public ManualResetEvent BacktestIsRunning = new ManualResetEvent(false);
		public ManualResetEvent BacktestCompletedQuotesCanGo = new ManualResetEvent(true);

		public int BarsSimulatedSoFar { get; private set; }
		public int QuotesTotalToGenerate {
			get {
				if (this.BarsOriginal == null) return -1;
				return this.BarsOriginal.Count * this.QuotesGenerator.QuotePerBarGenerates;
			}
		}
		public int QuotesGeneratedSoFar { get { return BarsSimulatedSoFar * this.QuotesGenerator.QuotePerBarGenerates; } }
		public BacktestMode BacktestMode { get; private set; }
		public BacktestQuotesGenerator QuotesGenerator { get; private set; }

		public string ProgressStats {
			get {
				if (QuotesGenerator == null) return "QuotesGenerator=null";
				return this.QuotesGeneratedSoFar + " / " + this.QuotesTotalToGenerate;
			}
		}
		public bool IsBacktestingNow { get { return BacktestIsRunning.WaitOne(0); } }
		public bool WasBacktestAborted {
			get {
				if (QuotesGenerator == null) return false;
				bool signalled = this.BacktestAborted.WaitOne(0);
				return signalled;
			}
		}

		public Backtester(ScriptExecutor executor) {
			this.backtestQuoteBarConsumer = new BacktestQuoteBarConsumer(this);
			this.Executor = executor;
			if (this.Executor.Strategy == null) return;
			if (this.Executor.Strategy.Script == null) return;
			this.Initialize(this.Executor.Strategy.Script.BacktestMode);
		}

		public void Initialize(BacktestMode mode) {
			this.BacktestMode = mode;
			if (this.QuotesGenerator != null && this.QuotesGenerator.BacktestModeSuitsFor == mode) {
				return;
			}
			switch (this.BacktestMode) {
				case BacktestMode.FourStrokeOHLC:
					this.QuotesGenerator = new BacktestQuotesGeneratorFourStroke(this);
					break;
				default:
					string msg = "NYI: [" + this.Executor.Strategy.Script.BacktestMode + "]RunSimulation";
					throw new Exception(msg);
			}
		}

		public void RunSimulation() {
			if (this.QuotesGenerator == null) {
				string msg = "backtestQuotesGenerator is not chosen / instantiated";
				throw new Exception(msg);
				//this.Initialize(BacktestMode.FourStrokeOHLC);
			}
			this.ExceptionsHappenedSinceBacktestStarted = 0;
			this.substituteBarsAndRunSimulation();
		}

		public void AbortRunningBacktestWaitAborted(string whyAborted) {
			if (this.IsBacktestingNow == false) return;
			this.RequestingBacktestAbort.Set();
			//isBacktesting = this.QuotesGenerator.BacktestIsRunning.WaitOne(0, true);
			//if (isBacktesting == false) return;
			bool aborted = this.BacktestAborted.WaitOne(0);
			string msg = "BACKTEST_ABORTED whyAborted=[" + whyAborted + "]: Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]";
			Assembler.PopupException(msg);
			//throw new Exception(msg);
		}
		public void WaitUntilBacktestCompletes() {
			if (this.IsBacktestingNow == false) return;
			this.BacktestCompletedQuotesCanGo.WaitOne();
		}
		public void SetRunningFalseNotifyWaitingThreadsBacktestCompleted() {
			//this.BacktestIsRunningBool = false;
			this.BacktestIsRunning.Reset();
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


		public void substituteBarsAndRunSimulation() {
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
					Application.DoEvents();
				}

				// see Indicator.DrawValue() "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED"
				// this.QuotesGenerator.InjectFakeQuoteInNonExistingNextBarToSolidifyLastStaticBar(this.Executor.Bars.BarStaticLast);
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			} finally {
				this.simulationPostBarsRestore();
				this.closePositionsLeftOpenAfterBacktest();
			}
		}
		void closePositionsLeftOpenAfterBacktest() {
			if (this.Executor.ExecutionDataSnapshot.AlertsPending.Count == 0) return;
			List<Alert> alertsPending = new List<Alert>(this.Executor.ExecutionDataSnapshot.AlertsPending);
			foreach (Alert alertPending in alertsPending) {
				try {
					if (alertPending.IsEntryAlert) {
						this.Executor.ClosePositionWithAlertClonedFromEntryBacktestEnded(alertPending);
					} else {
						string msg = "checkPositionCanBeClosed() will later interrupt the flow saying {Sorry I don't serve alerts.IsExitAlert=true}";
						this.Executor.RemovePendingExitAlertPastDueClosePosition(alertPending);
					}
					bool removed = this.Executor.ExecutionDataSnapshot.AlertsPendingRemove(alertPending);
				} catch (Exception e) {
					string msg = "NOT_AN_ERROR BACKTEST_POSITION_FINALIZER: check innerException: most likely you got POSITION_ALREADY_CLOSED on counterparty alert's force-close?";
					this.Executor.PopupException(msg, e);
				}
			}
			if (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0) {
				string msg = "snap.AlertsPending.Count[" + this.Executor.ExecutionDataSnapshot.AlertsPending.Count + "] should be ZERO before we enter Streaming";
				throw new Exception(msg);
			}
			if (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0) {
				string msg = "snap.PositionsOpenNow.Count[" + this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count + "] should be ZERO before we enter Streaming";
				throw new Exception(msg);
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
				this.BacktestDataSource = new BacktestDataSource(this.BarsSimulating);
				this.BarsSimulating.DataSource = this.BacktestDataSource;

				this.BacktestDataSource.StreamingProvider.ConsumerQuoteRegister(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);
				this.BacktestDataSource.StreamingProvider.ConsumerBarRegister(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);

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
				//this.BacktestIsRunningBool = true;
				this.BacktestIsRunning.Set();
				// Calling ManualResetEvent.Reset closes the gate.
				// Threads that call WaitOne on a closed gate will block
				this.BacktestCompletedQuotesCanGo.Reset();
			}
		}

		void simulationPostBarsRestore() {
			try {
				this.BacktestDataSource.StreamingProvider.ConsumerQuoteUnRegister(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);
				this.BacktestDataSource.StreamingProvider.ConsumerBarUnRegister(
					this.BarsSimulating.Symbol, this.BarsSimulating.ScaleInterval, this.backtestQuoteBarConsumer);

				this.Executor.BacktestContextRestore();
				this.Executor.EventGenerator.RaiseBacktesterSimulatedAllBarsStep4of4();
			} catch (Exception e) {
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
				return;
			}
			if (bar2simulate.ParentBarsIndex == 12) {
				//Debugger.Break();
			}
			SortedList<int, QuoteGenerated> quotesGenerated = this.QuotesGenerator.GenerateQuotesFromBar(bar2simulate);
			if (quotesGenerated == null) return;
			for (int i = 0; i < quotesGenerated.Count; i++) {
				QuoteGenerated quote = quotesGenerated[i];
				if (quote.ParentBarSimulated.ParentBarsIndex != bar2simulate.ParentBarsIndex) {
					#if DEBUG
					Debugger.Break();
					#endif
				}
				QuoteGenerated quotePrev;
				if (i > 0) {
					quotePrev = quotesGenerated[i-1];
					if (quote.Absno != quotePrev.Absno + 1) {
						#if DEBUG
						string msg = "IRRELEVANT since GenerateQuotesFromBar() has been upgraded to return SortedList<int, QuoteGenerated> instead of randomized List<QuoteGenerated>";
						//Debugger.Break();
						#endif
					}
				}
				this.BacktestDataSource.BacktestStreamingProvider.SpreadModeler.GeneratedQuoteFillBidAsk(quote);

				// GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #1/2
				if (bar2simulate.ContainsQuoteGenerated(quote) == false) {
					Debugger.Break();
					continue;
				}

				int pendingsToFillInitially = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				int quotesInjected = this.QuotesGenerator.InjectQuotesToFillPendingAlerts(quote, bar2simulate);
				if (quotesInjected == 0) {
					string msg = "SEEMS_ONLY_STOP_ALERTS_FAR_BEYOND_TARGET_ARE_ON_THE_WAY; pendingsToFillInitially[" + pendingsToFillInitially + "]"
						+ "STOPS_ARE_TOO_FAR OR_WILL_BE_FILLED_NEXT_THING_UPSTACK";
				}
				if (quotesInjected == pendingsToFillInitially) {
					int pendingsLeft = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;
					string msg = "GENERATED_EXACTLY_AS_MANY_AS_PENDINGS; PENDINGS_UNFILLED_LEFT_" + pendingsLeft;
				}
				int pendingsStrategyJustGenerated = quotesInjected - pendingsToFillInitially;
				if (pendingsStrategyJustGenerated > 0) {
					string msg = "SEEMS_STRATEGY_GENERATED_NEW_ALERTS_ON_NEW_QUOTES"
						+ "; quotesInjected[" + quotesInjected + "] > pendingsToFillInitially[" + pendingsToFillInitially + "]";
				}
				if (pendingsStrategyJustGenerated < 0) {
					string msg = "STOP_ALERTS_IGNORED_OTHERS_FILLED";
				}

				int pendingsLeftAfterInjected = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;

				this.BacktestDataSource.BacktestStreamingProvider.GeneratedQuoteEnrichSymmetricallyAndPush(quote);
				quote.WentThroughStreamingToScript = true;

				int pendingsLeftAfterTargetQuoteGenerated = this.Executor.ExecutionDataSnapshot.AlertsPending.Count;

				//stats, nothing poductive, only monitoring
				if (pendingsToFillInitially == 0) {
					continue;
				}

				int pendingsFilledByInjected = pendingsLeftAfterInjected - pendingsToFillInitially;
				if (pendingsFilledByInjected > 0) {
					string msg = "SEEMS_LIKE_INJECTING_DOES_ITS_JOB; pendingsFilledByInjected[" + pendingsFilledByInjected + "]";
				}
				int targetQuoteIsntExpectedToFillAnything = pendingsLeftAfterTargetQuoteGenerated - pendingsLeftAfterInjected;
				if (targetQuoteIsntExpectedToFillAnything > 0) {
					string msg = "SEEMS_LIKE_TARGET_QUOTE_HAD_OWN_FILL POSSIBLE_BUT_UNLIKELY";
				}
			}
		}
	}
}
