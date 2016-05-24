using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;

namespace Sq1.Core.Backtesting {
	public class Backtester : IDisposable {
		public const string				BARS_BACKTEST_CLONE_PREFIX		= "BACKTEST_BARS_CLONED_FROM-";
		public		ScriptExecutor		Executor						{ get; private set; }

		public		Bars				BarsOriginal					{ get; protected set; }
		public		Bars				BarsSimulating					{ get; protected set; }
		public		BacktestDataSource	BacktestDataSource				{ get; protected set; }
			BacktestStreamingConsumer 	backtestQuoteBarConsumer;

		protected	bool				BacktestWasAbortedByUserInGui;
		public		ManualResetEvent	RequestingBacktestAbortMre		{ get; private set; }	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		public		ManualResetEvent	BacktestAbortedMre				{ get; private set; }
		protected	ManualResetEvent	BacktestIsRunningMre			{ get; private set; }
		

		public int						BarsSimulatedSoFar				{ get; protected set; }
		public int						QuotesTotalToGenerate			{ get {
				if (this.BarsOriginal == null) return -1;	// I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL
				return this.BarsOriginal.Count * this.QuotesGenerator.BacktestStrokesPerBarAsInt;
			} }
		public int						QuotesGeneratedSoFar			{ get {
			if (this.QuotesGenerator == null) return 0;
			return BarsSimulatedSoFar * this.QuotesGenerator.BacktestStrokesPerBarAsInt;
		} }
		//public BacktestQuotesPerBar		BacktestQuotesPerBar			{ get {
		//	if (this.QuotesGenerator == null) return BacktestQuotesPerBar.Unknown;
		//	else return this.QuotesGenerator.BacktestStrokesPerBar;
		//} }
		public BacktestQuotesGenerator	QuotesGenerator					{ get; private set; }

		public string					ProgressStats					{ get {
				if (this.QuotesGenerator == null) return "QuotesGenerator=null";
				return this.QuotesGeneratedSoFar + " / " + this.QuotesTotalToGenerate;
			} }

		public bool						IsLivesimulator							{ get { return this is Livesimulator; } }
		public bool						ImBacktestingOrLivesimming				{ get { return this.BacktestIsRunningMre.WaitOne(0); } }
		public bool						ImRunningChartless_backtestOrSequencing	{ get { return this.ImBacktestingOrLivesimming == true && this.IsLivesimulator == false; } }
		public bool						ImRunningLivesim						{ get { return this.ImBacktestingOrLivesimming == true && this.IsLivesimulator == true; } }

		public bool						WasBacktestAborted				{ get {
				if (this.QuotesGenerator == null) {
					string msg = "ABORTION_IS_A_FLAG_IRRELEVANT_TO_QUOTE_GENERATOR_LIFECYCLE WORKED_FOR_BACKTEST_BUT_SPOILED_LATE_LIVESIM_CHECK";
					return false;
				}
				bool signalled = this.BacktestAbortedMre.WaitOne(0);
				return signalled;
			} }
		public int						ExceptionsHappenedSinceBacktestStarted;
		public Stopwatch				Stopwatch						{ get; private set; }

		Backtester() {
			BacktestWasAbortedByUserInGui	= false;
			RequestingBacktestAbortMre		= new ManualResetEvent(false);
			BacktestAbortedMre				= new ManualResetEvent(false);
			BacktestIsRunningMre			= new ManualResetEvent(false);
			BacktestDataSource				= new BacktestDataSource();
			ExceptionsHappenedSinceBacktestStarted = 0;
			//this.QuotesGenerator = BacktestQuotesGeneratorFourStroke.CreateForQuotesPerBarAndInitialize(BacktestQuotesPerBar.FourStrokeOHLC, this);
			Stopwatch						= new Stopwatch();
		}
		public Backtester(ScriptExecutor executor) : this() {
			this.Executor = executor;
			backtestQuoteBarConsumer		= new BacktestStreamingConsumer(this);
			if (this.Executor.Strategy == null) return;
			//MIGHT_BE_NULL_IF_NOT_COMPILED_YET if (Executor.Strategy.Script == null) return;
			this.Create_quoteGenerator_eachBacktesterSimulation();
		}
		public void Create_quoteGenerator_eachBacktesterSimulation() {
			if (this.Executor.Bars == null) {
				string msg = "EXECUTOR_LOST_ITS_BARS_NONSENSE null==this.Executor.Bars SubstituteBarsAndRunSimulation()";
				//Assembler.PopupException(msg);
				throw new Exception(msg);
				return;
			}
			if (this.Executor.Bars.Count < 1) {
				string msg = "EXECUTOR_BARS_ARE_EMPTY_CAN_NOT_SUBSTITUTE_AND_REGENERATE";
				//Assembler.PopupException(msg);
				throw new Exception(msg);
				return;
			}
			this.QuotesGenerator = BacktestQuotesGeneratorFourStroke.CreateForQuotesPerBar_initialize(
				this.Executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar, this);
		}
		public string SimulationRun() {
			this.ExceptionsHappenedSinceBacktestStarted = 0;
			//string msg = "MAKE_SURE_WE_WILL_INVOKE_BacktestStartingConstructOwnValuesValidateParameters()";
			//Assembler.PopupException(msg, null, false);

			int repaintableChunk = (int)(this.BarsOriginal.Count / 10);
			if (repaintableChunk <= 0) repaintableChunk = 1;
			if (this is Livesimulator) repaintableChunk = 1;
				
			int excludeLastBarStreamingWillTriggerIt = this.BarsOriginal.Count - 1;
			for (int barNo = 0; barNo <= excludeLastBarStreamingWillTriggerIt; barNo++) {
				bool abortRequested = this.RequestingBacktestAbortMre.WaitOne(0);
				if (abortRequested) {
					this.BacktestWasAbortedByUserInGui = true;
					break;
				}

				Bar bar = this.BarsOriginal[barNo];
				bar.CheckThrowFix_valuesOkay();
				this.generateQuotes_forBar_push(bar);
				this.BarsSimulatedSoFar++;
				if (this.BarsSimulatedSoFar % repaintableChunk == 0) {
					this.Executor.EventGenerator.RaiseOnBacktesterSimulatedChunk_step3of4();
				}
					
				//MAKE_EXCEPTIONS_FORM_INSERT_DELAYED!!! Application.DoEvents();	// otherwize UI becomes irresponsible;
				//COMMENTED_OUT_TO_SIMULATE_PROFILER_BEHAVIOUR MORE_EXCEPTIONS_DISPLAYED_IN_EXCEPTIONS_FORM_WOW Application.DoEvents();
				//#if DEBUG
				// UNCOMMENTED_FOR_SHARP_DEVELOP_TO_NOT_FREAK_OUT_FULLY_EXPAND_LOCAL_VARIABLES_AT_BREAKPOINTS_RANDOMLY_CONTINUE_ETC IRRELATED_TO_EXCEPTIONS_THERE_WAS_NONE
				// COMMENTED_OUT_#DEVELOP_FREAKS_OUT_WHEN_YOU_MOVE_INNER_WINDOWS_SPLITTER Application.DoEvents();
				// UNCOMMENTED_TO_KEEP_MOUSE_OVER_SLIDERS_RESPONSIVE
				// NOT_NEEDED_WHEN_BACKTESTER_STARTS_WITHOUT_PARAMETERS Application.DoEvents();
				//#endif
			}

			// see Indicator.DrawValue() "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED"
			// this.QuotesGenerator.InjectFakeQuoteInNonExistingNextBarToSolidifyLastStaticBar(this.Executor.Bars.BarStaticLast);

			string stats = "[" + this.BarsSimulatedSoFar + "/" + this.BarsOriginal.Count + "]bars";
			string msg = this.BacktestWasAbortedByUserInGui
				? "; ABORTED_AT " + stats	// "ABORTED_IN_GUI_BACKTESTED_ONLY_BARS"
				: "";					// "BACKTESTED_ALL_REQUIRED_BARS";
			return msg;
		}
		public void AbortRunningBacktest_waitAborted(string whyAborted, int millisecondsToWait = 3000) {
			if (this.ImBacktestingOrLivesimming == false) return;

			//bool abortIsAlreadyRequested = this.RequestingBacktestAbort.WaitOne(0);
			this.RequestingBacktestAbortMre.Set();
			//bool abortAlreadyRequestedNow = this.RequestingBacktestAbort.WaitOne(0);

			string msig = " whyAborted=[" + whyAborted + "]: Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]";

			string msg = "BACKTEST_ABORTING";
			Assembler.PopupException(msg + msig, null, false);

			bool aborted = this.BacktestAbortedMre.WaitOne(millisecondsToWait);
			msg = (aborted) ? "BACKTEST_ABORTED" : "BACKTESTER_DIDNT_ABORT_WITHIN_MS[" + millisecondsToWait + "]";
			if (this.ImBacktestingOrLivesimming == true) this.BacktestIsRunningMre.Reset();
			if (this.ImBacktestingOrLivesimming == true) {
				msg = "STILL_RUNNING_INTERNAL_ERROR " + msg;
			}
			Assembler.PopupException(msg + msig, null, false);
		}

		public void AbortBacktestIfExceptionsLimitReached() {
			if (this.ImRunningLivesim) return;
			this.ExceptionsHappenedSinceBacktestStarted++;
			if (this.ExceptionsHappenedSinceBacktestStarted < this.Executor.Strategy.ExceptionsLimitToAbortBacktest) return;
			this.AbortRunningBacktest_waitAborted("AbortBacktestIfExceptionsLimitReached[" + this.Executor.Strategy.ExceptionsLimitToAbortBacktest + "]");
		}
		protected virtual void SimulationPreBarsSubstitute_overrideable() {
			if (this.BarsOriginal == this.Executor.Bars) {
				string msg = "DID_YOU_FORGET_TO_RESET_this.BarsOriginal_TO_NULL_AFTER_BACKTEST_FINISHED??";
				Assembler.PopupException(msg);
			}
			try {
				//COPIED_UPSTACK_FOR_BLOCKING_MOUSEMOVE_AFTER_BACKTEST_NOW_CLICK__BUT_ALSO_STAYS_HERE_FOR_SLIDER_CHANGE_NON_INVALIDATION
				//WONT_BE_RESET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
				if (this.Executor.ChartShadow != null) this.Executor.ChartShadow.PaintAllowedDuringLivesimOrAfterBacktestFinished = false;

				#region candidate for this.BacktestDataSourceBuildFromUserSelection()
				this.BarsOriginal = this.Executor.Bars;
				this.BarsSimulating = this.Executor.Bars.CloneBars_zeroBarsInside_sameDataSource(BARS_BACKTEST_CLONE_PREFIX);	// + this.BarsOriginal
				this.Executor.EventGenerator.RaiseOnBackteste_barsIdenticalButEmpty_substitutedToGrow_step1of4();
				
				BacktestSpreadModeler spreadModeler;
				// kept it on the surface and didn't pass ScriptContextCurrent.SpreadModelerPercent to "new BacktestDataSource()" because later BacktestDataSource:
				// 1) will support different SpreadModelers with not only 1 parameter like SpreadModelerPercentage;
				// 2) will support different BacktestModes like 12strokes, not only 4Stroke 
				// 3) will poke StreamingAdapter-derived implementations 12 times a bar with platform-generated quotes for backtests with regulated poke delay
				// 4) will need to be provide visualized 
				// v1 this.BacktestDataSource.BacktestStreamingAdapter.InitializeSpreadModelerPercentage(this.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
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
				#endregion

				string threadName = "BACKTESTING " + this.Executor.Strategy.WindowTitle + " " + this.BarsSimulating.InstanceScaleCount;
				Assembler.SetThreadName(threadName, "LIVESIM_FAILED_TO_SET_THREAD_NAME OR_NPE");

				this.BacktestDataSource.InitializeBacktest(this.Executor.ToString() , this.BarsSimulating, spreadModeler);
				this.BarsSimulating.SubstituteDataSource_forBarsSimulating(BacktestDataSource);

				StreamingAdapter streaming = this.BacktestDataSource.StreamingAdapter;
				DistributorCharts distr = streaming.DistributorCharts_substitutedDuringLivesim;
				if (distr == null) {
					string msg = "YOU_DIDNT_RESTORE_DISTRIBUTOR_PROPERLY_AFTER_LIVESIM";
					Assembler.PopupException(msg);
				}
				distr.ConsumerQuoteSubscribe(this.backtestQuoteBarConsumer, false);
				distr.ConsumerBarSubscribe  (this.backtestQuoteBarConsumer, false);

				//v2
				//BacktestStreaming streaming = this.BacktestDataSource.StreamingAsBacktest_nullUnsafe;
				//DistributorBacktest distr = streaming.DistributorBacktest;
	
				distr.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(this.BarsSimulating.Symbol);
				
				this.Executor.BacktestContext_initialize(this.BarsSimulating);

				#region PARANOID
				if (this.BarsOriginal == null) {
					string msg = "consumers will expect this.BarsOriginal != null";
					Assembler.PopupException(msg);
				}
				if (this.BarsOriginal.Count == 0) {
					string msg = "consumers will expect this.BarsOriginal.Count > 0";
					Assembler.PopupException(msg);
				}
				if (this.BarsSimulating == null) {
					string msg = "consumers will expect this.BarsSimulating != null";
					Assembler.PopupException(msg);
				}
				if (this.BarsSimulating.Count > 0) {
					string msg = "consumers will expect this.BarsSimulating.Count = 0";
					Assembler.PopupException(msg);
				}
				if (this.Executor.Bars == null) {
					string msg = "consumers will expect this.Bars != null";
					Assembler.PopupException(msg);
				}
				if (this.Executor.Bars.Count > 0) {
					string msg = "consumers will expect this.Bars.Count = 0";
					Assembler.PopupException(msg);
				}
				#endregion

				//ALREADY_RAISED_INSIDE_CONTEXT_INITIALIZE() this.Executor.EventGenerator.RaiseOnBacktesterSimulationContextInitialized_step2of4();
			} catch (Exception ex) {
				string msg = "SimulationPreBarsSubstitute_overrideable(): Backtester caught a long beard...";
				this.Executor.PopupException(msg, ex);
			} finally {
				this.BarsSimulatedSoFar = 0;
				this.BacktestWasAbortedByUserInGui = false;
				if (this.BacktestAbortedMre.WaitOne(0) == true) Thread.Sleep(10);	// let the Wait() in GUI thread to feel SIGNALLED, before I reset again to NON_SIGNALLED
				this.BacktestAbortedMre.Reset();
				this.RequestingBacktestAbortMre.Reset();
				this.BacktestIsRunningMre.Set();
				if (this.ImBacktestingOrLivesimming == false) {
					string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
					Assembler.PopupException(msg);
				}
			}
		}
		protected virtual void SimulationPostBarsRestore_overrideable() {
			try {
				StreamingAdapter streamingBacktest = this.BacktestDataSource.StreamingAdapter;
				StreamingAdapter streamingOriginal = this.BarsOriginal.DataSource.StreamingAdapter;
				string msg = "NOW_INSERT_BREAKPOINT_TO_this.channel.PushQuoteToConsumers(quoteDequeued) CATCHING_BACKTEST_END_UNPAUSE_PUMP";

				// what Livesim-related code does in Backtester???
				//if (streamingOriginal != null && streamingOriginal is LivesimStreaming == false) {
				//	string msg2 = "MUST_BE_EMPTY_ORIGINAL_BARS.STREAMING ABSORBING_FROM_BACKTEST_COMPLETE";
				//	Assembler.PopupException(msg2);
				//	streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(streamingBacktest, this.BarsOriginal.Symbol, this.BarsOriginal.ScaleInterval);
				//}

				DistributorCharts distr = this.BacktestDataSource.StreamingAdapter.DistributorCharts_substitutedDuringLivesim;
				distr.ConsumerQuoteUnsubscribe	(this.backtestQuoteBarConsumer);
				distr.ConsumerBarUnsubscribe	(this.backtestQuoteBarConsumer);

				double sec = Math.Round(this.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				string strokesPerBar = this.QuotesGenerator.BacktestStrokesPerBar + "/Bar";
				string stats = "Backtest took [" + sec + "]sec at " + strokesPerBar;
				this.Executor.LastBacktestStatus = stats + this.Executor.LastBacktestStatus;

				this.Executor.BacktestContext_restore();
				this.BarsOriginal = null;	// I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL WILL_AFFECT_ChartForm.TsiProgressBarETA
				if (this.Executor.ChartShadow == null) {
					string msg3 = "IAM_IN_SEQUENCER_HAVING_NO_CHART_ASSOCIATED";
					return;
				}
				this.Executor.ChartShadow.PaintAllowedDuringLivesimOrAfterBacktestFinished = true;
				// DOESNT_HELP_HOPE_ON_OnBacktestedAllBars() in InterForm this.Executor.ChartShadow.Invalidate();
			} catch (Exception ex) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw ex;
			} finally {
				// Calling ManualResetEvent.Set opens the gate,
				// allowing any number of threads calling WaitOne to be let through
				//moved to this.NotifyWaitingThreads()
				//this.BacktestCompletedQuotesCantGo.Set();
				if (this.BacktestWasAbortedByUserInGui) {
					this.BacktestAbortedMre.Set();
					this.RequestingBacktestAbortMre.Reset();
				}
			}
		}
		void generateQuotes_forBar_push(Bar bar2simulate) {
			if (bar2simulate == null) return;
			if (bar2simulate.IsBarStreaming && double.IsNaN(bar2simulate.Open)) {
				string msg = "IRRELEVANT_PARTIAL_VALUES_WERE_DEPRECATED it's ok for Bars.LastBar from Repository to have no PartialValues;"
					+ " filled by Streaming, NA for Backtest, skipping LastBar";
				//throw new Exception(msg);
				Assembler.PopupException(msg);
				return;
			}

			List<QuoteGenerated> quotesGenerated = this.QuotesGenerator.Generate_quotesFromBar_avoidClearing_StreamingAdaterWontPushOutOfMarket(bar2simulate);
			if (quotesGenerated == null) return;

			for (int i = 0; i < quotesGenerated.Count; i++) {
				bool abortRequested = this.RequestingBacktestAbortMre.WaitOne(0);
				if (abortRequested) break;

				if (this.ImBacktestingOrLivesimming == false) {
					string msg = "BACKTEST_INTERRUPTED_ON quote[" + (i+1) + "/" + quotesGenerated.Count + "] IsBacktestRunning == false";
					Assembler.PopupException(msg, null, false);
					break;
				}

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
				if (bar2simulate.ContainsBidAsk_forQuoteGenerated(quote, true) == false) {
					Debugger.Break();
				}
				#endif

				int pendingsToFillInitially = this.Executor.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.Count;
				List<QuoteGenerated> quotesInjected = this.QuotesGenerator.InjectQuotes_toFillPendingAlerts_push(quote, bar2simulate);
				
				#if DEBUG //TEST_EMBEDDED
				if (quotesInjected.Count == 0) {
					if (pendingsToFillInitially > 0) {
						string msg = "SEEMS_ONLY_STOP_ALERTS_FAR_BEYOND_TARGET_ARE_ON_THE_WAY; pendingsToFillInitially[" + pendingsToFillInitially + "]"
							+ "STOPS_ARE_TOO_FAR OR_WILL_BE_FILLED_NEXT_THING_UPSTACK";
					}
				} else {
					if (quotesInjected.Count == pendingsToFillInitially) {
						int pendingsLeft = this.Executor.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.Count;
						string msg = "GENERATED_EXACTLY_AS_MANY_AS_PENDINGS; PENDINGS_UNFILLED_LEFT_" + pendingsLeft;
					}
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

				int pendingsLeftAfterInjected = this.Executor.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.Count;

				this.BacktestDataSource.StreamingAsBacktest_nullUnsafe.PushQuoteGenerated(quote);

				//nothing poductive below, only breakpoint placeholders
				#if DEBUG //TEST_EMEDDED
				int pendingsLeftAfterTargetQuoteGenerated = this.Executor.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.Count;
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
		public const string TO_STRING_PREFIX = "BACKTESTER_FOR_";
		public override string ToString() {
			string ret = TO_STRING_PREFIX + this.Executor.ToString();
			return ret;
		}

		public void Initialize_runSimulation_backtestAndLivesim_step1of2() {
			this.Executor.LastBacktestStatus = "INITIALIZING";
			this.Stopwatch.Restart();
			this.Create_quoteGenerator_eachBacktesterSimulation();
			this.Executor.LastBacktestStatus = "SUBSTITUTING_BARS";
			this.SimulationPreBarsSubstitute_overrideable();
			this.Executor.LastBacktestStatus = "RUNNING_SIMULATION";
			this.Executor.LastBacktestStatus = this.SimulationRun();
		}
		public void BacktestRestore_step2of2() {
			if (this.ImRunningLivesim == false) {
				this.Executor.BacktestEnded_closeOpenPositions();
			} else {
				this.Executor.LivesimEnded_invalidateUnfilledOrders_ClearPendingAlerts();
			}
			this.SimulationPostBarsRestore_overrideable();
			this.BacktestIsRunningMre.Reset();
			if (this.ImBacktestingOrLivesimming) {
				string msg = "IN_ORDER_TO_SIGNAL_FLAGGED_I_HAVE_TO_SET_INSTEAD_OF_RESET";
				Assembler.PopupException(msg);
			}
			this.Stopwatch.Stop();
		}

		public void SetQuoteGeneratorAndConditionallyRebacktest_invokedInGuiThread(BacktestQuotesGenerator clone) {
			this.Executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar = clone.BacktestStrokesPerBar;
			this.Executor.Strategy.Serialize();
			this.Create_quoteGenerator_eachBacktesterSimulation();	// activate right now (while Livesim is running)
			if (this.Executor.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false) return;
			this.Executor.BacktesterRun_trampoline(null, true);
		}

		public virtual void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.RequestingBacktestAbortMre	.Dispose();
			this.BacktestAbortedMre			.Dispose();
			this.BacktestIsRunningMre		.Dispose();

			this.RequestingBacktestAbortMre	= null;
			this.BacktestAbortedMre			= null;
			this.BacktestIsRunningMre		= null;
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
