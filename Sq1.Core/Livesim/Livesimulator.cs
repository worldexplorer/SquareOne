using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public partial class Livesimulator : Backtester, IDisposable {
		const string REASON_TO_EXIST = "VISUALLY CONFIRM THAT CORE CAN COPE WITH FOUR THREADS SIMULTANEOUSLY:"
			+ " 1) STREAMING_ADAPTER"
			+ " 2) ITS QUOTE_PUMP INVOKING SCRIPT.OVERRIDES"
			+ " 3) BROKER_ADAPTER ALSO INVOKING SCRIPT.OVERRIDES WITHOUT TASKS"
			+ " 4) CHARTING/REPORTING IN GUI THREAD;"
			+ " ALL OF THOSE SEQUENTIALLY AND NON-CONTRADICTIVELY DEALING WITH EXECUTION_DATA_SNAPSHOT"
			+ " AND OTHER INTERNALS AT 1000 QUOTES/SECOND WOULD BE CONSIDERED A HUGE SUCCESS";

		public	const string			BARS_LIVESIM_CLONE_PREFIX		= "LIVESIM_BARS_CLONED_FROM-";
		public	Backtester				BacktesterBackup				{ get; private set; }
		public	LivesimDataSource		DataSourceAsLivesim_generator_nullUnsafe	{ get { return base.BacktestDataSource as LivesimDataSource; } }

				ToolStripButton			btnStartStop;
				ToolStripButton			btnPauseResume;

				ChartShadow				chartShadow;

		public	bool					IsPaused_waitForever_untilUnpaused	{ get {
			return this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.UnpausedMre.WaitOne(-1);
		} }


		public Livesimulator(ScriptExecutor executor) : base(executor) {
			base.BacktestDataSource			= new LivesimDataSource(executor);
			this.DataSourceAsLivesim_generator_nullUnsafe.Initialize(Assembler.InstanceInitialized.OrderProcessor);
			// MOVED_TO_DATASOURCE base.SeparatePushingThreadEnabled = false;
			// DONT_MOVE_TO_CONSTRUCTOR!!!WORKSPACE_LOAD_WILL_INVOKE_YOU_THEN!!! base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(executor_BacktesterContextInitializedStep2of4);
		}

		//[JsonIgnore]	public StreamingAdapter				StreamingOriginal;
		protected override void SimulationPreBarsSubstitute_overrideable() {
			if (base.BarsOriginal == base.Executor.Bars) {
				string msg = "DID_YOU_FORGET_TO_RESET_base.BarsOriginal_TO_NULL_AFTER_BACKTEST_FINISHED??";
				Assembler.PopupException(msg);
			}
			try {

				base.BarsOriginal	= base.Executor.Bars;
				base.BarsSimulating = base.Executor.Bars.CloneBars_zeroBarsInside_sameDataSource(BARS_LIVESIM_CLONE_PREFIX);	// + base.BarsOriginal
				//v2 INDICATORS_COMPLAIN_MUST_BE_EMPTY_ON_BACKTEST_STARTING base.BarsSimulating = base.Executor.Bars.CloneBars_firstBarInside_avoidingLastBarNull(BARS_LIVESIM_CLONE_PREFIX);	// + base.BarsOriginal

				string threadName = "LIVESIMMING " + base.Executor.Strategy.WindowTitle + " " + base.BarsSimulating.InstanceScaleCount;
				Assembler.SetThreadName(threadName, "LIVESIM_FAILED_TO_SET_THREAD_NAME OR_NPE");

				// this way orders will go through ownBrokerImplementation
				//v1 ALERTS_GET_GENERATING_STREAMING__INSTEAD_OF_RECEIVING_DDE base.BarsSimulating.SubstituteDataSource_forBarsSimulating(this.DataSourceAsLivesim_nullUnsafe);
				//v2 streamingDdeServer_brokerOwnImplementation
				LivesimDataSource dataSourceConsumer_livesim = new LivesimDataSource(this.Executor);
				base.BarsSimulating.SubstituteDataSource_forBarsSimulating(dataSourceConsumer_livesim);
				//v3
				dataSourceConsumer_livesim.StreamingAdapter = base.BarsOriginal.DataSource.StreamingAdapter;
				//dataSourceConsumer_livesim.BrokerAdapter	= base.BarsOriginal.DataSource.BrokerAdapter.LivesimBroker_ownImplementation;
				dataSourceConsumer_livesim.Propagate_preInstantiated_LivesimBroker_ownImplementation_intoLivesimDataSource();

				// I want alerts generated to have BrokerOwnImplementation, that's it!!!
				dataSourceConsumer_livesim.BrokerAsLivesim_nullUnsafe.InitializeLivesim(dataSourceConsumer_livesim, base.Executor.OrderProcessor);
				dataSourceConsumer_livesim.BrokerAsLivesim_nullUnsafe.InitializeMarketsim(base.Executor);	// for QuikBrokerLivesim, MarketLive isn't needed, but without ScriptExecutor it will throw while failing to get LivesimSettings

				#region NEVER_COMMENT_THIS_OUT__I_SPENT_TWO_DAYS_TO_DEBUG_IT MOVED_BACK_DOWNSTACK
				//string reasonToPauseSymbol = "SYMBOL_PAUSED_LIVESIMMING-" + this.Executor.ToString();
				int pumpsPaused = 0;
				if (base.BarsOriginal.DataSource.BrokerAdapter is LivesimBrokerDefault) {
					//v1 BROKER_IN_TEST_MODE_WILL_NOT_SEND_ORDERS_FOR_NON_LIVESIMMING_SYMBOLS
					pumpsPaused = this.Executor.DataSource_fromBars.LivesimStreaming_PumpPause_forSameSymbolScale_freezeNonSimmingConsumers_whenBrokerOriginalIsLivesimDefault(this.Executor);
				} else {
					base.BarsOriginal.DataSource.BrokerAdapter.ImBeingTested_byOwnLivesimImplementation_set(true);
					pumpsPaused = this.Executor.DataSource_fromBars.LivesimStreaming_PumpsAllPause_forAllSymbolsScales_freezeAllConsumers_whenBrokerOriginalIsReal(this.Executor);
				}
				if (pumpsPaused == 0) {
					string msg = "BARS_UNSUBSCRIBED OR_DONT_YOU_HAVE_CHARTS_OPEN_FOR_STRATEGY_LIVESIMMING?";
					Assembler.PopupException(msg, null, false);
				} else {
					string msg = "DIDNT_YOU_UNPAUSE_PREVIOUS_LIVESIM??? YOU_ASKED_TO_LIVESIM_A_LIVE_CHART__PUMPS_PAUSED [" + pumpsPaused + "]";
					Assembler.PopupException(msg, null, false);
				}
				#endregion

				StreamingAdapter originalStreaming_fromDataSource = this.Executor.DataSource_fromBars.StreamingAdapter;
				if (originalStreaming_fromDataSource == null) {
					string msg = "STREAMING_ADAPTER_MUST_BE_AT_LEAST_StreamingLivesimDefault"
						+ " ASSIGN_IT_IN_DATASOURCE_EDITOR_FOR[" + this.Executor.DataSource_fromBars + "]";
					Assembler.PopupException(msg);
					base.AbortRunningBacktest_waitAborted(msg, 0);
					return;
				}

				
				BacktestSpreadModeler spreadModeler;
				// kept it on the surface and didn't pass ScriptContextCurrent.SpreadModelerPercent to "new DataSourceAsLivesim_nullUnsafe()" because later DataSourceAsLivesim_nullUnsafe:
				// 1) will support different SpreadModelers with not only 1 parameter like SpreadModelerPercentage;
				// 2) will support different BacktestModes like 12strokes, not only 4Stroke 
				// 3) will poke StreamingAdapter-derived implementations 12 times a bar with platform-generated quotes for backtests with regulated poke delay
				// 4) will need to be provide visualized 
				// v1 this.DataSourceAsLivesim_nullUnsafe.BacktestStreamingAdapter.InitializeSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
				// v2 UI-controlled in the future, right now the stub  
				ContextScript ctx = base.Executor.Strategy.ScriptContextCurrent;
				string msig = "STARTING_LIVESIM " + base.Executor.Strategy.ToString() + " //SimulationPreBarsSubstitute_overrideable()";
				switch (ctx.SpreadModelerClassName) {
					case "BacktestSpreadModelerPercentage":
						spreadModeler = new BacktestSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
					default:
						string msg = "SPREAD_MODELER_NOT_YET_SUPPORTED[" + ctx.SpreadModelerClassName + "]"
							+ ", instantiatind default BacktestSpreadModelerPercentage("
							+ base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent + ")";
						Assembler.PopupException(msg + msig);
						spreadModeler = new BacktestSpreadModelerPercentage(base.Executor.Strategy.ScriptContextCurrent.SpreadModelerPercent);
						break;
				}

				this.DataSourceAsLivesim_generator_nullUnsafe.Propagate_preInstantiated_LivesimStreaming_ownImplementation_intoLivesimDataSource();	// no need to restore (this.DataSourceAsLivesim_nullUnsafe will be re-initialized at next Livesim)
				// NO!!! KEEP_THEM_TO_ORIGINAL_DATASOURCE_BECAUSE_DDE_SERVER_DELIVERS_LEVEL2_TO_ORIGINAL_DATA_SNAPSHOT base.BarsSimulating.DataSource = this.DataSourceAsLivesim_nullUnsafe;	// will need to restore (base.BarsSimulating is not needed after Livesim is done)
				this.DataSourceAsLivesim_generator_nullUnsafe.InitializeLivesim(base.Executor.ToString(), base.BarsSimulating, spreadModeler);		// NOW_OWN_IMPLEMENTATION_WILL_GENERATE_QUOTES_FOR_BarsSimulating
				// MAKE_DDE_CLIENT_SERVER_WORK base.BarsSimulating.DataSource = this.DataSourceAsLivesim_nullUnsafe;		// DISTRIBUTORS_INSIDE_SINGLE_STREAMING_ARE_REPLACED_AND_DDE_SERVER_IS_FED_VIA_FAKE_DDE_CLIENT
				//DURING_LIVESIM_I_LEFT_STREAMING_EXACTLY_THE_SAME_AS_FOR_LIVE_TRADING_TO_TEST_IT!!! this.StreamingOriginal = this.Executor.DataSource.StreamingAdapter;		// will have to restore
				
				if (this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe as LivesimStreaming == null) {
					string msg = "LivesimDataSource_MUST_GENERATE_VIA_LivesimStreaming_AND_PUSH_TO_DDE"
						+ " STREAMING_ORIGINAL_WILL_RECEIVE WITH_PAUSED_ORIGINAL_PUMP_PER_SYMBOL AND_REPLACED_DISTRIBUTOR_WITH_MY_LIVESIM_ONLY_AS_CONSUMER"
						+ " OKAY_SINCE_LIVESIM_STREAMING,BROKER_DEFAULT_ARE_LEGIT_PROVIDERS_WHEN_NO_OTHERS_AVAILABLE"
						+ " SHOULD_BE_NO_NPE_IN_eventGenerator_OnStrategyExecutedOneQuote_unblinkDataSourceTree";
					Assembler.PopupException(msg, null, false);
				}
				// now I have those two assigned from Streaming/Broker-own-implemented instantiated adapters

				LivesimStreaming livesimStreaming = this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe;	// = ownLivesimStreaming here (QuikLivesimStreaming, )
				livesimStreaming.InitializeLivesim(this.DataSourceAsLivesim_generator_nullUnsafe, originalStreaming_fromDataSource, base.BarsSimulating.Symbol);

				//livesimming without Distributor replacement routes to wrong Bars and creates problems with ChartMultisplitter
				//FIRST_LINE_OF_QuikStreamingLivesim.UpstreamConnect_LivesimStarting()_MAKE_SENSE_FOR_OTHERS
				livesimStreaming.Original_SubstituteDistributor_forSymbolLivesimming_extractChartIntoSeparateDistributor_subscribe();
				//LivesimDataSource is now having LivesimBacktester and no-solidifier Distributor

				this.CheckSubscribed_ChartStreamingConsumer_toDistributor_replacedForLivesim();

				livesimStreaming.UpstreamConnect_LivesimStarting();

				base.Executor.BacktestContext_initialize(base.BarsSimulating);

				#region looks like {new LivesimulatorDataSource().SeparatePushingThreadEnabled=true} actually SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM; still waiting for the chart to set Bars, otherwize white overstriked ChartControl for a second
				int waitForGuiToSetBars_maxMillis = 5000;
				bool barsAreSetInGui = this.barsAreSetInGuiThread.WaitOne(waitForGuiToSetBars_maxMillis);		// SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM
				if (barsAreSetInGui == false) {
					string msg = "GUI_HASNT_SET_BARS_AFTER_WAITING waitForGuiToSetBars_maxMillis[" + waitForGuiToSetBars_maxMillis + "]"
						+ " HOPING_YOU_WONT_RECEIVE BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM";
					Assembler.PopupException(msg);
				}
				#endregion

				//this.BarsSimulating.DataSource = this.DataSourceAsLivesim_nullUnsafe;	// may not need to restore (base.BarsSimulating is not needed after Livesim is done)
				if (this.BarsSimulating.BarStaticLast_nullUnsafe == null) {
					string msg = "LEFT_FOR_QUIK_LIVESIM_COMPAT_TEST NO!!! DOES_IT_SOLVE__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM?";
					Assembler.PopupException(msg, null, false);
				}

				#region PARANOID
				if (base.BarsOriginal == null) {
					string msg = "consumers will expect base.BarsOriginal != null";
					Assembler.PopupException(msg);
				}
				if (base.BarsOriginal.Count == 0) {
					string msg = "consumers will expect base.BarsOriginal.Count > 0";
					Assembler.PopupException(msg);
				}
				if (base.BarsSimulating == null) {
					string msg = "consumers will expect base.BarsSimulating != null";
					Assembler.PopupException(msg);
				}
				if (base.BarsSimulating.Count > 0) {
					string msg = "consumers will expect base.BarsSimulating.Count = 0";
					Assembler.PopupException(msg);
				}
				if (base.Executor.Bars == null) {
					string msg = "consumers will expect base.Bars != null";
					Assembler.PopupException(msg);
				}
				if (base.Executor.Bars.Count > 0) {
					string msg = "consumers will expect base.Bars.Count = 0";
					Assembler.PopupException(msg);
				}
				#endregion

				if (this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe != null) {
					this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.PushSymbolInfo_toLevelTwoGenerator(this.Executor.Bars.SymbolInfo);
				} else {
					string msg = "YOUR_LIVESIM_DATASOURCE_HAS_NO__LIVESIM_STREAMING"
						+ " Livesim/noStreaming was still generating LevelTwo directly in the StreamingDataSnapshot";
					Assembler.PopupException(msg);
				}

				base.Executor.EventGenerator.RaiseOnBackteste_barsIdenticalButEmpty_substitutedToGrow_step1of4();

			} catch (Exception ex) {
				string msg = "SimulationPreBarsSubstitute_overrideable(): Livesimulator caught a long beard...";
				base.Executor.PopupException(msg, ex);
			} finally {
				base.BarsSimulatedSoFar = 0;
				base.BacktestWasAbortedByUserInGui = false;
				if (base.BacktestAbortedMre.WaitOne(0) == true) Thread.Sleep(10);	// let the Wait() in GUI thread to feel SIGNALLED, before I reset again to NON_SIGNALLED
				base.BacktestAbortedMre.Reset();
				base.RequestingBacktestAbortMre.Reset();
				base.BacktestIsRunningMre.Set();
				if (this.ImBacktestingOrLivesimming == false) {
					string msg = "IN_ORDER_TO_SIGNAL_UNFLAGGED_I_HAVE_TO_RESET_INSTEAD_OF_SET";
					Assembler.PopupException(msg);
				}
			}
		}

		protected override void SimulationPostBarsRestore_overrideable() {
			try {
				if (base.WasBacktestAborted || base.RequestingBacktestAbortMre.WaitOne(0) == true) {
					string msg2 = "NOT_ABSORBING_LAST_LIVESIM_STREAMING_INTO_BARS_ORIGINAL__SOLIDIFIER_WILL_COMPLAIN_OTHERWISE";
					Assembler.PopupException(msg2, null, false);
				} else {
					string msg2 = "LOOKS_LIKE_WE_USED_UP_ALL_BARS_AND_SUCCESSFULLY_FINISHED_LIVESIM___OR_EXCEPTION";
					//streamingOriginal.AbsorbStreamingBarFactoryFromBacktestComplete(
					//	streamingBacktest, base.BarsOriginal.Symbol, base.BarsOriginal.ScaleInterval);
				}

				double sec = Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				string strokesPerBar = base.QuotesGenerator.BacktestStrokesPerBar + "/Bar";
				string stats = "Livesim took [" + sec + "]sec at " + strokesPerBar;
				this.Executor.LastBacktestStatus = stats + this.Executor.LastBacktestStatus;
				base.Executor.BacktestContext_restore();

				StreamingAdapter originalAdapterFromDataSource = this.Executor.DataSource_fromBars.StreamingAdapter;
				if (originalAdapterFromDataSource == null) {
					string msg = "STREAMING_ADAPTER_MUST_BE_AT_LEAST_StreamingLivesimDefault"
						+ " ASSIGN_IT_IN_DATASOURCE_EDITOR_FOR[" + this.Executor.DataSource_fromBars + "]";
					Assembler.PopupException(msg);
					base.AbortRunningBacktest_waitAborted(msg, 0);
					return;
				}

				LivesimStreaming livesimStreaming = this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe;
				livesimStreaming.UpstreamDisconnect_LivesimTerminatedOrAborted();
				livesimStreaming.Original_SubstituteDistributor_forSymbolLivesimming_restoreOriginalDistributor();

				base.BarsOriginal.DataSource.BrokerAdapter.ImBeingTested_byOwnLivesimImplementation_set(false);

				#region MOVED_BACK_DOWNSTACK NEVER_COMMENT_OUT__I_SPENT_TWO_DAYS_TO_DEBUG_IT
				//string reasonToUnPauseSymbol = "SYMBOL_UNPAUSED_LIVESIMMING-" + this.Executor.ToString();
				int pumpsUnpaused;
				if (base.BarsOriginal.DataSource.BrokerAdapter is LivesimBrokerDefault) {
					//v1 BROKER_IN_TEST_MODE_WILL_NOT_SEND_ORDERS_FOR_NON_LIVESIMMING_SYMBOLS
					pumpsUnpaused = this.Executor.DataSource_fromBars.LivesimStreaming_PumpResume_forSameSymbolScale_unfreezeOtherConsumers_whenBrokerOriginalIsLivesimDefault(this.Executor);
				} else {
					pumpsUnpaused = this.Executor.DataSource_fromBars.LivesimStreaming_PumpsAllResume_forAllSymbolsScales_unfreezeAllConsumers_whenBrokerOriginalIsReal(this.Executor);
				}
				if (pumpsUnpaused == 0) {
					string msg = "BARS_UNSUBSCRIBED OR_DONT_YOU_HAVE_CHARTS_OPEN_FOR_STRATEGY_LIVESIMMING?";
					Assembler.PopupException(msg, null, false);
				}
				#endregion

				//if (this.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.settings.DelayBetweenSerialQuotesEnabled) {
				if (base.Executor.Strategy.LivesimStreamingSettings.DelayBetweenSerialQuotesEnabled == false) {
					base.Executor.OrderProcessor.RaiseDelaylessLivesimEnded_shouldRebuildOLV(this);
				}
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			} finally {
				base.Executor.BacktesterOrLivesimulator = this.BacktesterBackup;
				if (base.BacktestWasAbortedByUserInGui) {
					base.BacktestAbortedMre.Set();
					base.RequestingBacktestAbortMre.Reset();
				}
			}
		}

		public void CheckSubscribed_ChartStreamingConsumer_toDistributor_replacedForLivesim() {
			string					symbol		= base.BarsSimulating.Symbol;
			ScriptExecutor			executor	= this.Executor;
			StreamingConsumerChart	chart		= executor.ChartShadow.ChartStreamingConsumer;
			DistributorCharts		distr		= this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.StreamingOriginal.DistributorCharts_substitutedDuringLivesim;

			if (symbol != executor.Bars.Symbol) {
				string msg1 = "WHEN_ARE_SYMBOLS_DIFFERENT??..";
				Assembler.PopupException(msg1);
			}
			List<SymbolScaleStream<StreamingConsumerChart>> mustBeOneTimeframe = distr.GetStreams_allScaleIntervals_forSymbol(symbol);
			if (mustBeOneTimeframe.Count != 1) {
				string msg1 = "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE [" + symbol + "] STARTING_LIVESIM_FOR:" + executor.ToString()
					//+ " BAD_JOB#1_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()"
					;
				Assembler.PopupException(msg1, null, false);
				return;
			}
			SymbolScaleStream<StreamingConsumerChart> mustBeChartSubscribedToQuotesAndBars = mustBeOneTimeframe[0];
			if (mustBeChartSubscribedToQuotesAndBars.ConsumersQuoteCount != 1) {
				string msg1 = "BAD_JOB#2_SubstituteDistributorForSymbolsLivesimming_subscribeLivesimConsumerToLivesimStreamingDistributor()";
				Assembler.PopupException(msg1);
			} else {
				//v1 bool mustBeChartSubscribedToQuotes = mustBeChartSubscribedToQuotesAndBars.ConsumersQuoteContains(chartless);
				bool mustBeChartSubscribedToQuotes = mustBeChartSubscribedToQuotesAndBars.ConsumersQuoteContains(chart);
				if (mustBeChartSubscribedToQuotes == false) {
					string msg1 = "BAD_JOB#3_SubstituteDistributorForSymbolsLivesimming_subscribeLivesimConsumerToLivesimStreamingDistributor()";
					Assembler.PopupException(msg1);
				}
			}

			if (mustBeChartSubscribedToQuotesAndBars.ConsumersBarCount != 1) {
				string msg1 = "BAD_JOB#4_SubstituteDistributorForSymbolsLivesimming_subscribeLivesimConsumerToLivesimStreamingDistributor()";
				Assembler.PopupException(msg1);
			} else {
				//v1 bool mustBeChartSubscribedToBars = mustBeChartSubscribedToQuotesAndBars.ConsumersBarContains(chartless);
				bool mustBeChartSubscribedToBars = mustBeChartSubscribedToQuotesAndBars.ConsumersBarContains(chart);
				if (mustBeChartSubscribedToBars == false) {
					string msg1 = "BAD_JOB#5_SubstituteDistributorForSymbolsLivesimming_subscribeLivesimConsumerToLivesimStreamingDistributor()";
					Assembler.PopupException(msg1);
				}
			}
		}

		public void Start_invokedFromGuiThread(ToolStripButton btnStartStopPassed, ToolStripButton btnPauseResumePassed, ChartShadow chartShadow) {
			this.btnStartStop = btnStartStopPassed;
			this.btnPauseResume = btnPauseResumePassed;
			this.chartShadow = chartShadow;
			this.chartShadow.RangeBarCollapseToAccelerateLivesim();
			this.BacktesterBackup = base.Executor.BacktesterOrLivesimulator;
			base.Executor.BacktesterOrLivesimulator = this;
			//BROKER_AS_LIVESIM_NULL__LIVESIM_DS_NOT_INITIALIZED_YET this.DataSourceAsLivesim_nullUnsafe.BrokerAsLivesim_nullUnsafe.InitializeBacktestBroker(base.Executor);

			// DONT_MOVE_TO_CONSTRUCTOR!!!WORKSPACE_LOAD_WILL_INVOKE_IT_THEN_INAPPROPRIETLY!!!  WITHOUT_UNSUBSCRIPTON_I_WAS_GETTING_MANY_INVOCATIONS_BAD
			base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 -= new EventHandler<EventArgs>(executor_OnBacktesterContextInitialized_step2of4);
			base.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(executor_OnBacktesterContextInitialized_step2of4);

			this.OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange -= new EventHandler<QuoteEventArgs>(livesimulator_OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange);
			this.OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange += new EventHandler<QuoteEventArgs>(livesimulator_OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange);

			try {
				//v1 base.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
				base.Executor.Livesim_compileRun_trampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
			} catch (Exception ex) {
				string msg = "BARS_ARE_NULL YOU_PAUSED_A_WRONG_DISTRIBUTOR";
				Assembler.PopupException(msg);
			}
		}

		void afterBacktesterComplete(ScriptExecutor executorCompletePooled) {
			string msig = " //Livesimulator.afterBacktesterComplete()";

			if (executorCompletePooled == null) {
				string msg = "CAN_NOT_BE_NULL_executorCompletePooled";
				Assembler.PopupException(msg + msig);
				return;
			}
			//IM_UNPAUSED_AFTER_LIVESIM_FINISHED Assembler.PopupException("TIME_TO_UNPAUSE_ORIGNIAL_QUOTE_PUMP_executorCompletePooled: " + executorCompletePooled.ToStringWithCurrentParameters() + msig, null, false);

			base.Executor.BacktesterOrLivesimulator = this.BacktesterBackup;
			base.BarsOriginal = null;	// I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL WILL_AFFECT_ChartForm.TsiProgressBarETA

			//v1 this.btnStartStop.BeginInvoke((MethodInvoker)delegate {
			this.chartShadow.BeginInvoke((MethodInvoker)delegate {
				this.btnStartStop.Text = "Start";
				this.btnStartStop.CheckState = CheckState.Unchecked;

				if (base.Executor.Bars == null) {
					string msg = "LOOKS_LIKE_YOU_DIDNT_RESTORE_CONTEXT_AFTER_BACKTEST_ABORTED WHEN_QUIK_LIVESIM_FAILED_TO_CONNECT_TO_ITS_OWN_SERVER";
					Assembler.PopupException(msg);
				} else {
					this.chartShadow.Initialize(base.Executor.Bars, base.Executor.StrategyName, false, true);
				}

				float seconds = (float)Math.Round(base.Stopwatch.ElapsedMilliseconds / 1000d, 2);
				this.btnPauseResume.Text = seconds.ToString() + " sec";
				this.btnPauseResume.Enabled = false;
			});
		}

		public void Stop_invokedFromGuiThread() {
			string msig = " //Livesimulator.Stop_inGuiThread()";
			if (this.ImRunningLivesim == false) {
				Assembler.PopupException("WHY_DID_YOU_INVOKE_ME? LIVESIM_IS_NOT_RUNNING_NOW" + msig);
				return;
			}
			try {
				if (this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.QuotePumpSeparatePushingThreadEnabled) {
					string msg = "YOU_STOPPED_PAUSED_LIVESIM_FROM_GUI UNPAUSED_LIVESIM_QUOTE_PUMP__WOZU???_ORIGINAL_BARS_SOLIDIFIER_STILL_RUNNING_IN_ITS_THREAD";
					ManualResetEvent unpausedMre = this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.UnpausedMre;
					bool isUnpaused = unpausedMre.WaitOne(0);
					if (isUnpaused == false) {
						msg = "AVOIDING_TO_WAIT_FOREVER: LivesimStreaming.GeneratedQuoteEnrichSymmetricallyAndPush() " + msg;
						unpausedMre.Set();
					}
					base.AbortRunningBacktest_waitAborted(msg + msig);
				} else {
					string msg2 = "LIVESIM_HAS_SINGLE_THREADED_QUOTE_PUMP__NOT_EVEN_USED_IN_QuikLivesimStreaming DONT_REQUEST_AND_DONT_WAIT_JUST_CONTINUE";
					base.AbortRunningBacktest_waitAborted(msg2 + msig, 0);
				}
			} catch (Exception ex) {
				Assembler.PopupException("USER_OR_MAINFORM_CLICKED_STOP_IN_LIVESIM_FORM" + msig, ex);
			}
		}

		public void Pause_invokedFromGuiThread() {
			if (this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe == null) {
				string msg1 = "YOU_DIDNT_START_LIVESIM_CANT_PAUSE add this.TssBtnPauseResume.Enabled=false into LiveSimControl.Designer.cs";
				Assembler.PopupException(msg1, null, false);
				return;
			}
			this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.UnpausedMre.Reset();
			string msg = "LEAKED_HANDLES_HUNTER AlertsScheduledForDelayedFill.Count=";
			if (this.DataSourceAsLivesim_generator_nullUnsafe.BrokerAsLivesim_nullUnsafe != null) {
				msg += this.DataSourceAsLivesim_generator_nullUnsafe.BrokerAsLivesim_nullUnsafe.DataSnapshot.AlertsPending_scheduledForDelayedFill.Count;
			} else {
				msg += "UNKNOWN_COUNT: YOU_DIDNT_SET_ORIGINAL_BROKER_ADAPTER_FOR_LIVESIM_DATASOURCE__WHEN_STREAMING_GENERATING_PUSHES_INTO_DDE";
			}
			//Assembler.PopupException(msg);
		}
		public void Unpause_inGuiThread() {
			this.DataSourceAsLivesim_generator_nullUnsafe.StreamingAsLivesim_nullUnsafe.UnpausedMre.Set();
		}
		public const string TO_STRING_PREFIX = "LIVESIMULATOR_FOR-";
		public bool LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
		public override string ToString() {
			string ret = TO_STRING_PREFIX + base.Executor.ToString();
			return ret;
		}
	}
}
