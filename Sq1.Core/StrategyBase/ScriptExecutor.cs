using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;		//StackFrame

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Backtesting;
using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		#region constructed (my own data)
		public	ExecutionDataSnapshot			ExecutionDataSnapshot		{ get; protected set; }
		public	SystemPerformance				Performance					{ get; protected set; }
		public	Backtester						Backtester;//					{ get; private set; }
		public	PositionPrototypeActivator		PositionPrototypeActivator	{ get; private set; }
		public	MarketLive						MarketLive					{ get; private set; }
		public	MarketsimBacktest				MarketsimBacktest			{ get; private set; }
		public	ScriptExecutorEventGenerator	EventGenerator				{ get; private set; }
		public	CommissionCalculator			CommissionCalculator;
		public	Optimizer						Optimizer					{ get; protected set; }
		public	Livesimulator					Livesimulator				{ get; private set; }
		#endregion
		
		#region initialized (sort of Dependency Injection)
		public	ChartShadow						ChartShadow;		// initialized with Sq1.Charting.ChartControl:ChartShadow
		public	Strategy						Strategy;
		public	string							StrategyName				{ get { return (this.Strategy == null) ? "STRATEGY_NULL" : this.Strategy.Name; } }
		public	OrderProcessor					OrderProcessor				{ get; private set; }
		#endregion

		#region volatile Script is recompiled and replaced
		public	Bars							Bars						{ get; private set; }
		public PositionSize PositionSize { get {
				if (this.Strategy == null) {
					string msg = "ScriptExecutor.PositionSize: you should not access PositionSize before you've set Strategy";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return this.Strategy.ScriptContextCurrent.PositionSize;
			} }
		public DataSource DataSource { get {
				if (this.Bars == null) {
					string msg = "ScriptExecutor.DataSource: you should not access DataSource BEFORE you've set ScriptExecutor.Bars";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return this.Bars.DataSource;
			} }
		#endregion

		//bool isStreamingWhenNoStrategyLoaded;
		bool isEmittingOrdersWhenNoStrategyLoaded;

		public bool IsStreamingTriggeringScript {
			get {
				if (this.Strategy == null) {
					//v1 return this.isStreamingWhenNoStrategyLoaded
					string msg = "IsStreamingTriggeringScript__get: CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_STREAMING";
					Assembler.PopupException(msg);
					return false;
				}
				return this.Strategy.ScriptContextCurrent.IsStreamingTriggeringScript;
			}
			set {
				if (this.Strategy == null) {
					//v1 this.isStreamingWhenNoStrategyLoaded = value;
					string msg = "IsStreamingTriggeringScript__set: CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_STREAMING";
					Assembler.PopupException(msg);
					return;
				}
				
				this.Strategy.ScriptContextCurrent.IsStreamingTriggeringScript = value;
				// we are in beginning the backtest and will switch back to preBacktestIsStreaming after backtest finishes;
				// if you AppKill during the backtest, you don't want btnStreaming be pressed (and disabled DataSource.StreamingAdapter=null) after AppRestart 
				if (this.preBacktestBars != null) {
					//string msg = "NOT_SAVING_IsStreamingTriggeringScript=ON_FOR_BACKTEST"
					//	+ " preBacktestIsStreaming[" + this.preBacktestIsStreaming + "] preBacktestBars[" + this.preBacktestBars + "]";
					//Assembler.PopupException(msg, null, false);
					return;
				}
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
				
				if (value == true) {
					this.Strategy.Script.OnStreamingTriggeringScriptTurnedOnCallback();
				} else {
					this.Strategy.Script.OnStreamingTriggeringScriptTurnedOffCallback();
				}
			}
		}
		public bool IsStrategyEmittingOrders {
			get {
				if (this.Strategy == null) {
					//v1 return this.isEmittingOrdersWhenNoStrategyLoaded
					string msg = "IsStrategyEmittingOrders__get: CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_EMITTING_MOUSE_ORDERS";
					Assembler.PopupException(msg);
					return false;
				}
				return this.Strategy.ScriptContextCurrent.StrategyEmittingOrders;
			}
			set {
				if (this.Strategy == null) {
					//v1 this.isEmittingOrdersWhenNoStrategyLoaded = value;
					string msg = "IsStrategyEmittingOrders__set: CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_EMITTING_MOUSE_ORDERS";
					Assembler.PopupException(msg);
					return;
				}
				this.Strategy.ScriptContextCurrent.StrategyEmittingOrders = value;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
				
				if (value == true) {
					this.Strategy.Script.OnStrategyEmittingOrdersTurnedOnCallback();
				} else {
					this.Strategy.Script.OnStrategyEmittingOrdersTurnedOffCallback();
				}
			}
		}

		public ScriptExecutor() {
			// CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_STREAMING				this.IsStreamingTriggeringScript = false;
			// CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_EMITTING_MOUSE_ORDERS	this.IsStrategyEmittingOrders = false;

			this.ExecutionDataSnapshot = new ExecutionDataSnapshot(this);
			//NOW_IRRELEVANT_MOVED_TO_BacktesterRunSimulation this.Performance = new SystemPerformance(this);
			this.Backtester = new Backtester(this);
			this.PositionPrototypeActivator = new PositionPrototypeActivator(this);
			this.MarketLive = new MarketLive(this);
			this.MarketsimBacktest = new MarketsimBacktest(this);
			this.EventGenerator = new ScriptExecutorEventGenerator(this);
			this.CommissionCalculator = new CommissionCalculatorZero(this);
			this.Optimizer = new Optimizer(this);
			this.Livesimulator = new Livesimulator(this);
			this.OrderProcessor = Assembler.InstanceInitialized.OrderProcessor;
			this.Performance = new SystemPerformance(this);
		}

		public void Initialize(ChartShadow chartShadow, Strategy strategy) {
			string msg = " at this time, FOR SURE this.Bars==null, strategy.Script?=null";
			this.ChartShadow = chartShadow;
			this.Strategy = strategy;
			
			this.Optimizer.InitializedProperly = false;

			if (this.Strategy != null) {
				if (this.Bars != null) {
					this.Strategy.ScriptContextCurrent.Symbol = this.Bars.Symbol;
					this.Strategy.ScriptContextCurrent.DataSourceName = this.DataSource.Name;
				}
				if (this.Strategy.Script == null) {
					msg = "I will be compiling this.Strategy.Script when in ChartFormsManager.StrategyCompileActivatePopulateSliders()";
					//} else if (this.Bars == null) {
					//	msg = "InitializeStrategyAfterDeserialization will Script.Initialize(this) later with bars";
				} else {
					this.Strategy.Script.Initialize(this);
					this.Optimizer.Initialize();	//otherwize this.Optimizer.InitializedProperly = false; => can't optimize anything
				}
			}
			this.ExecutionDataSnapshot.Initialize();
			// SO_WHAT??? Executor.Bars are NULL in ScriptExecutor.ctor() and NOT NULL in SetBars
			this.Performance.Initialize();
			this.MarketsimBacktest.Initialize(Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow);
			//v1, ATTACHED_TO_BARS.DATASOURCE.SYMBOLRENAMED_INSTEAD_OF_DATASOURCE_REPOSITORY
			// if I listen to DataSourceRepository, all ScriptExecutors receive same notification including irrelated to my Bars
			// Assembler.InstanceInitialized.RepositoryJsonDataSource.OnSymbolRenamed +=
			//	new EventHandler<DataSourceSymbolEventArgs>(Assembler_InstanceInitialized_RepositoryJsonDataSource_OnSymbolRenamed);
		}
		Bar barStaticExecutedLast;
		public ReporterPokeUnit ExecuteOnNewBarOrNewQuote(Quote quoteForAlertsCreated, bool onNewQuoteTrue_onNewBarFalse = true) {
			if (this.Strategy == null) {
				string msg1 = "I_REFUSE_TO_EXECUTE_SCRIPT YOU_DIDNT_CHECK_MY_STRATEGY_IS_NULL";
				Assembler.PopupException(msg1);
				return null;
			}
			if (this.Strategy.Script == null) return null;
			this.ExecutionDataSnapshot.PreExecutionOnNewBarOrNewQuoteClear();

			//if (quote != null) {
			if (onNewQuoteTrue_onNewBarFalse == true) {
				foreach (Indicator indicator in this.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {
					try {
						indicator.OnNewStreamingQuote(quoteForAlertsCreated);
					} catch (Exception ex) {
						Assembler.PopupException("INDICATOR_ON_NEW_STREAMING_QUOTE " + indicator.ToString(), ex);
					}
				}

				try {
					this.Strategy.Script.OnNewQuoteOfStreamingBarCallback(quoteForAlertsCreated);
					//alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
					//if (alertsDumpedForStreamingBar > 0) {
					//	string msg = "ITS OK HERE since prev quote has created prototype-based alerts"
					//		+ "I WANT DUMP TO BE VALID ONLY IN onNewBar case only!!!"
					//		+ " " + alertsDumpedForStreamingBar + " alerts Dumped for " + quote;
					//}
				} catch (Exception ex) {
					string msig = " //Script[" + this.Strategy.Script.GetType().Name + "].OnNewQuoteCallback(" + quoteForAlertsCreated + ")";
					this.PopupException(ex.Message + msig, ex);
				}
			} else {
				if (this.barStaticExecutedLast != null) {
					int mustBeOne = this.Bars.BarStaticLastNullUnsafe.ParentBarsIndex - this.barStaticExecutedLast.ParentBarsIndex;
					if (mustBeOne == 0) {
						string msg2 = "DUPE_IN_SCRIPT_INVOCATION__INDICATORS_WILL_COMPLAIN_TOO";
						Assembler.PopupException(msg2, null, false);
					}
					if (mustBeOne > 1) {
						int skipped = mustBeOne - 1;
						string msg2 = "HOLE_IN_SCRIPT_INVOCATION INDICATORS_WILL_COMPLAIN_TOO ALERTS_WILL_MISTMATCH_BARS ExecuteOnNewBar()_SKIPPED=[" + skipped + "]";
						Assembler.PopupException(msg2, null, false);
					}
				}
				foreach (Indicator indicator in this.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {
					try {
						int barsAheadOfIndicator = this.Bars.BarStaticLastNullUnsafe.ParentBarsIndex - indicator.OwnValuesCalculated.LastIndex;
						if (barsAheadOfIndicator == 0) continue;

						indicator.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppended(this.Bars.BarStaticLastNullUnsafe);
					} catch (Exception ex) {
						Assembler.PopupException("INDICATOR_ON_NEW_BAR " + indicator.ToString(), ex);
					}
				}

				try {
					this.Strategy.Script.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(this.Bars.BarStaticLastNullUnsafe);
					this.barStaticExecutedLast = this.Bars.BarStaticLastNullUnsafe;
				} catch (Exception ex) {
					string msig = " //Script[" + this.Strategy.Script.GetType().Name + "].OnNewBarCallback(" + quoteForAlertsCreated + ")";
					this.PopupException(ex.Message + msig, ex);
				}
			}
			string msg = "DONT_REMOVE_ALERT_SHOULD_LEAVE_ITS_TRAIL_DURING_LIFETIME_TO_PUT_UNFILLED_DOTS_ON_CHART";
			//int alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
//			int alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.AlertsPending.Count;
//			if (alertsDumpedForStreamingBar > 0) {
//				msg += " DUMPED_AFTER_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
//			}


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

			bool willEmit = this.Backtester.IsBacktestingNow == false && this.OrderProcessor != null && this.IsStrategyEmittingOrders;
			bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;
			if (willEmit) {
				string msg3 = "Breakpoint";
				//Debugger.Break();
				//#D_FREEZE Assembler.PopupException(msg3, null, false);
			}

			List<Alert> alertsNewAfterExecCopy = this.ExecutionDataSnapshot.AlertsNewAfterExec.InnerListSafeCopy;

			if (alertsNewAfterExecCopy.Count > 0) {
				this.enrichAlertsWithQuoteCreated(alertsNewAfterExecCopy, quoteForAlertsCreated);
				if (willEmit) {
					string msg2 = "Breakpoint";
					//#D_FREEZE Assembler.PopupException(msg2);
					//Debugger.Break();
					ContextScript ctx = this.Strategy.ScriptContextCurrent;

					//MOVED_TO_ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()
					// ^^^ this.DataSource.PausePumpingFor(this.Bars, true);		// ONLY_DURING_DEVELOPMENT__FOR_#D_TO_HANDLE_MY_BREAKPOINTS
					bool paused = this.Bars.DataSource.PumpingWaitUntilPaused(this.Bars, 0);
					if (paused == true) {
						string msg3 = "YOU_WANT_ONE_STRATEGY_PER_SYMBOL_LIVE MAKE_SURE_YOU_HAVE_ONLY_ONE_SYMBOL:INTERVAL_ACROSS_ALL_OPEN_CHARTS PUMP_SHOULD_HAVE_BEEN_PAUSED_EARLIER"
							+ " in ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
						Assembler.PopupException(msg3);
					}
					this.OrderProcessor.CreateOrdersSubmitToBrokerAdapterInNewThreads(alertsNewAfterExecCopy, setStatusSubmitting, true);
					//MOVED_TO_ChartFomStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()
					// ^^^ this.DataSource.UnPausePumpingFor(this.Bars, true);	// ONLY_DURING_DEVELOPMENT__FOR_#D_TO_HANDLE_MY_BREAKPOINTS

					foreach (Alert alert in alertsNewAfterExecCopy) {
						if (alert.OrderFollowed != null) continue;
						bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alert);
						if (removed == false) {
							string msg3 = "FAILED_TO_REMOVE_INCONSISTENT_ALERT_FROM_PENDING removed=" + removed;
							Assembler.PopupException(msg3);
						}
					}
				}
			}

			if (this.Backtester.IsBacktestingNow && this.Backtester.WasBacktestAborted) return null;
	
			
			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(quoteForAlertsCreated,
												this.ExecutionDataSnapshot.AlertsNewAfterExec.Clone(),
												this.ExecutionDataSnapshot.PositionsOpenedAfterExec.Clone(),
												this.ExecutionDataSnapshot.PositionsClosedAfterExec.Clone(),
												this.ExecutionDataSnapshot.PositionsOpenNow.Clone()
											);

			if (this.Backtester.IsBacktestingNow == false) {
				// FROM_ChartFormStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
				if (pokeUnit.PositionsOpenNow.Count > 0) {
					this.Performance.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(this.ExecutionDataSnapshot.PositionsOpenNow);
					this.EventGenerator.RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(pokeUnit);
				}
			}

			foreach (Alert alert in pokeUnit.AlertsNew.InnerList) {
				Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, alert);
			}
			//if (this.Backtester.IsBacktestingNow) return pokeUnit;
			// NOPE PositionsMaster grows only in Callback: do this before this.OrderProcessor.CreateOrdersSubmitToBrokerAdapterInNewThreads() to avoid REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert
			// NOPE_REALTIME_FILLS_POSITIONS_ON_CALLBACK this.AddPositionsJustCreatedUnfilledToChartShadowAndPushToReportersAsyncUnsafe(pokeUnit);
			
			this.RaiseStrategyExecutionComplete(quoteForAlertsCreated);
			if (pokeUnit.PositionsPlusAlertsCount == 0) return null;
			return pokeUnit;
		}

		void enrichAlertsWithQuoteCreated(List<Alert> alertsAfterStrategy, Quote quote) {
			if (quote == null) return;
			foreach (Alert alert in alertsAfterStrategy) {
				if (quote.HasParentBar == false) {
					string msg = "I_REFUSE_TO_ENRICH_ALERT_WITH_QUOTE__SINCE_QUOTE_HAS_NO_PARENT_BAR__I_CAN_NOT_CHECK_IF_QUOTE_AND_ALERT_ARE_FOR_THE_SAME_BAR";
					this.PopupException(msg);
					continue;
				}
				int alertIsLateNbars = quote.ParentBarStreaming.ParentBarsIndex - alert.PlacedBarIndex;
				if (alertIsLateNbars != 0) {
					string msg = "I_REFUSE_TO_ENRICH_ALERT_WITH_QUOTE alertIsLateNbars[" + alertIsLateNbars + "] alert[" + alert + "]";
					this.PopupException(msg);
					continue;
				}
				//alert.PositionSize = this.PositionSize;
				alert.QuoteCreatedThisAlertServerTime = quote.ServerTime;
				alert.QuoteCreatedThisAlert = quote;
			}
		}
//		public Position BuyOrShortAlertCreateDontRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
//														  Direction direction, MarketLimitStop entryMarketLimitStop) {
//			return BuyOrShortAlertCreateRegister(entryBar, stopOrLimitPrice, entrySignalName,
//												 direction, entryMarketLimitStop, false);
//		}
		public void CheckThrowAlertCanBeCreated(Bar entryBar, string msig) {
			string invoker = (new StackFrame(3, true).GetMethod().Name) + "(): ";
			if (this.Bars == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msig + " this.Bars=[null] " + invoker);
			}
			if (entryBar == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msig + " for Bars=[" + this.Bars + "]" + invoker);
			}
		}
		public Position BuyOrShortAlertCreateRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
													  Direction direction, MarketLimitStop entryMarketLimitStop, bool registerInNew = true) {
			string msig = " //BuyOrShortAlertCreateRegister(stopOrLimitPrice[" + stopOrLimitPrice+ "], entrySignalName[" + entrySignalName + "], entryBar[" + entryBar + "])";
			this.CheckThrowAlertCanBeCreated(entryBar, msig);

			Alert alert = null;
			// real-time streaming should create its own Position after an Order gets filled
			if (this.IsStreamingTriggeringScript) {
				alert = this.MarketLive.EntryAlertCreate(entryBar, stopOrLimitPrice, entrySignalName,
																  direction, entryMarketLimitStop);
			} else {
				//string msg = "YOU_DONT_EMIT_ORDERS_THEN_CONTINUE_BACKTEST_BASED_ON_LIVE_QUOTES";
				string msg = "BACKTESTS_MUST_RUN_IN_STREAMING_SINCE_MarketSimStatic_WAS_DEPRECATED_INFAVOROF_MarketRealStreaming";
				Assembler.PopupException(msg);
				return null;
			}
			Alert similar = this.ExecutionDataSnapshot.AlertsPending.FindSimilarNotSameIdenticalForOrdersPending(alert);
			if (similar != null) {
				string msg = "DUPLICATE_ALERT_FOUND similar[" + similar + "]";
				Assembler.PopupException(msg + msig);
				return similar.PositionAffected;
			}

			this.ExecutionDataSnapshot.AlertEnrichedRegister(alert, registerInNew);

			// ok for single-entry strategies; nogut if we had many Streaming alerts and none of orders was filled yet...
			// MOVED_TO_ON_ALERT_FILLED_CALBACK
			Position pos = new Position(alert, alert.PriceScript);
			alert.PositionAffected = pos;
			return pos;
		}
		public Alert SellOrCoverAlertCreateDontRegisterInNew(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
														Direction direction, MarketLimitStop exitMarketLimitStop) {
			return this.SellOrCoverAlertCreateRegister(exitBar, position, stopOrLimitPrice, signalName,
													   direction, exitMarketLimitStop, false);
		}
		public Alert SellOrCoverAlertCreateRegister(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
													Direction direction, MarketLimitStop exitMarketLimitStop, bool registerInNewAfterExec = true) {

			this.CheckThrowAlertCanBeCreated(exitBar, "BARS.BARSTREAMING_OR_BARS.BARLASTSTATIC_IS_NULL_SellOrCoverAlertCreateRegister() ");
			if (position == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception("POSITION_CAN_NOT_BE_NULL_SellOrCoverAlertCreateRegister()");
			}

			Alert alert = null;
			if (position.Prototype != null) {
				if (signalName.Contains("protoTakeProfitExit")
					&& position.Prototype.TakeProfitAlertForAnnihilation != null
					&& this.Backtester.IsBacktestingNow == false) {
					string msg = "I won't create another protoTakeProfitExit because"
						+ " position.Prototype.TakeProfitAlertForAnnihilation != null"
						+ " position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
				if (signalName.Contains("protoStopLossExit")
					&& position.Prototype.StopLossAlertForAnnihilation != null
					&& this.Backtester.IsBacktestingNow == false) {
					string msg = "I won't create another protoStopLossExit because"
						+ " position.Prototype.StopLossAlertForAnnihilation != null"
						+ " position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
			} else {
				if (position.ExitAlert != null) {
					string msg = "POSITION_ALREADY_HAS_AN_EXIT_ALERT_REPLACE_INSTEAD_OF_ADDING_SECOND_SellOrCoverAlertCreateRegister();"
						+ " Strategy[" + this.Strategy.ToString() + "] position.Prototype=null position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
				foreach (Alert closingAlertForPosition in this.ExecutionDataSnapshot.AlertsPending.InnerList) {
					if (closingAlertForPosition.PositionAffected == position && closingAlertForPosition.IsExitAlert) {
						string msg = "PENDING_EXIT_ALERT_FOUND_WHILE_POSITION.EXITALERT=NULL"
							+ "; position.ExitAlert[" + position.ExitAlert + "] != closingAlertForPosition[" + closingAlertForPosition + "]";
						this.PopupException(msg);
						return closingAlertForPosition;
					}
				}
			}

			if (this.IsStreamingTriggeringScript) {
				alert = this.MarketLive.ExitAlertCreate(exitBar, position, stopOrLimitPrice, signalName,
																 direction, exitMarketLimitStop);
			} else {
				//string msg = "YOU_DONT_EMIT_ORDERS_THEN_CONTINUE_BACKTEST_BASED_ON_LIVE_QUOTES";
				string msg = "BACKTESTS_MUST_RUN_IN_STREAMING_SINCE_MarketSimStatic_WAS_DEPRECATED_INFAVOROF_MarketRealStreaming";
				Assembler.PopupException(msg);
				return alert;
			}

			this.ExecutionDataSnapshot.AlertEnrichedRegister(alert, registerInNewAfterExec);

			return alert;
		}
		public bool AnnihilateCounterpartyAlertDispatched(Alert alert) {
			if (alert == null) {
				string msg = "don't invoke KillAlert with alert=null; check for TP=0 or SL=0 prior to invocation";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			bool killed = false;
			if (this.IsStreamingTriggeringScript) {
				if (this.Backtester.IsBacktestingNow == true) {
					killed = this.MarketsimBacktest.AnnihilateCounterpartyAlert(alert);
					//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
				} else {
					killed = this.MarketLive.AnnihilateCounterpartyAlert(alert);
				}
			} else {
				//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
				Debugger.Break();
			}
			return killed;
		}


		[Obsolete("REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014 replaced by Alert.PriceAligned,PriceStopActivationAligned - all served by SymbolInfo.AlignAlertToPriceLevelSimplified")]
		public double AlignAlertPriceToPriceLevel(Bars bars, double orderPrice, bool buyOrShort, PositionLongShort positionLongShort0, MarketLimitStop marketLimitStop0) {
			if (this.Strategy.ScriptContextCurrent.NoDecimalRoundingForLimitStopPrice) return orderPrice;
			if (bars == null) bars = this.Bars;
			if (bars.SymbolInfo.PriceLevelSizeForBonds == 0.0) {
				string text = "1";
				if (this.Strategy.ScriptContextCurrent.PriceLevelSizeForBonds > 0) {
					text = text.PadRight(this.Strategy.ScriptContextCurrent.PriceLevelSizeForBonds + 1, '0');
					bars.SymbolInfo.PriceLevelSizeForBonds = 1.0 / (double)Convert.ToInt32(text);
				}
				return orderPrice;
			}
			orderPrice = bars.SymbolInfo.AlignAlertToPriceLevel(orderPrice, buyOrShort, positionLongShort0, marketLimitStop0);
			return orderPrice;
		}
		
		public void PopupException(string msg, Exception ex = null) {
			Assembler.PopupException(msg, ex);
			this.Backtester.AbortBacktestIfExceptionsLimitReached();
		}

		public double OrderCommissionCalculate(Direction direction, MarketLimitStop marketLimitStop, double price, double shares, Bars bars) {
			double ret = 0;
			if (this.Strategy.ScriptContextCurrent.ApplyCommission && this.CommissionCalculator != null) {
				ret = this.CommissionCalculator.CalculateCommission(direction, marketLimitStop, price, shares, bars);
			}
			return ret;
		}
		public double getSlippageOld(double priceAligned, bool isLimitOrder) {
			if (this.Strategy.ScriptContextCurrent.EnableSlippage == false) return 0.0;
			if (isLimitOrder && this.Strategy.ScriptContextCurrent.LimitOrderSlippage == false) return 0.0;
			if (this.Bars.SymbolInfo.SecurityType == SecurityType.Future) {
				return (double)this.Strategy.ScriptContextCurrent.SlippageTicks * this.Bars.SymbolInfo.PriceLevelSizeForBonds;
			}
			double ret = 0.01 * this.Strategy.ScriptContextCurrent.SlippageUnits * priceAligned;
			//if (direction == Direction.Short || direction == Direction.Sell) ret = -ret;
			return ret;
		}
		public double getSlippage(double priceAligned, Direction direction, int slippageIndex, bool isStreaming, bool isLimitOrder) {
			if (isStreaming == false && this.Bars.SymbolInfo.UseFirstSlippageForBacktest == false) {
				return getSlippageOld(priceAligned, isLimitOrder);
			}
			double slippageValue = 0;
			try {
				slippageValue = this.Bars.SymbolInfo.getSlippage(priceAligned, direction, slippageIndex, isStreaming, isLimitOrder);
			} catch (Exception ex) {
				Assembler.PopupException("getSlippage()", ex);
				return getSlippageOld(priceAligned, isLimitOrder);
			}
			return slippageValue;
		}

		public void CreatedOrderWontBePlacedPastDueInvokeScript(Alert alert, int barNotSubmittedRelno) {
			//this.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			if (alert.IsEntryAlert) {
				this.RemovePendingEntry(alert);
				this.ClosePositionWithAlertClonedFromEntryBacktestEnded(alert);
			} else {
				string msg = "checkPositionCanBeClosed() will later interrupt the flow saying {Sorry I don't serve alerts.IsExitAlert=true}";
				this.RemovePendingExitAlertPastDueClosePosition(alert);
			}
			if (this.Strategy.Script == null) return;
			try {
				this.Strategy.Script.OnAlertNotSubmittedCallback(alert, barNotSubmittedRelno);
			} catch (Exception e) {
				string msg = "fix your OnAlertNotSubmittedCallback() in script[" + this.Strategy.Script.StrategyName + "]"
					+ "; was invoked with alert[" + alert + "] and barNotSubmittedRelno["
					+ barNotSubmittedRelno + "]";
				this.PopupException(msg, e);
			}
		}
		public void CallbackAlertKilledInvokeScript(Alert alert) {
			if (this.ExecutionDataSnapshot.AlertsPending.ContainsInInnerList(alert)) {
				bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alert);
				if (removed) alert.IsKilled = true;
			} else {
				string msg = "KILLED_ALERT_WAS_NOT_FOUND_IN_snap.AlertsPending DELETED_EARLIER_OR_NEVER_BEEN_ADDED;"
					+ " PositionCloseImmediately() kills all PositionPrototype-based PendingAlerts"
					+ " => killing those using AlertKillPending() before/after PositionCloseImmediately() is wrong!";
				//throw new Exception(msg);
				Assembler.PopupException(msg);
			}
			alert.Strategy.Script.OnAlertKilledCallback(alert);
		}
		public void CallbackAlertFilledMoveAroundInvokeScript(Alert alertFilled, Quote quoteFilledThisAlertNullForLive,
					 double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			string msig = " CallbackAlertFilledMoveAroundInvokeScript(" + alertFilled + ", " + quoteFilledThisAlertNullForLive + ")";
			
			AlertList alertsNewAfterAlertFilled = new AlertList("alertsNewAfterAlertFilled");
			Position positionOpenedAfterAlertFilled = null;
			Position positionClosedAfterAlertFilled = null;

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

			Bar barFill = (this.IsStreamingTriggeringScript) ? alertFilled.Bars.BarStreamingCloneReadonly : alertFilled.Bars.BarStaticLastNullUnsafe;
			if (alertFilled.IsEntryAlert) {
				if (alertFilled.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.EntryBar!=-1 for alert [" + alertFilled + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				} else {
					string msg = "initializing EntryBar=[" + barFill + "] on AlertFilled";
				}
			} else {
				if (alertFilled.PositionAffected.ExitFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.ExitBar!=-1 for alert [" + alertFilled + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				} else {
					string msg = "initializing ExitBar=[" + barFill + "] on AlertFilled";
				}
			}

			alertFilled.QuoteLastWhenThisAlertFilled = this.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(alertFilled.Symbol);

			int barFillRelno  = alertFilled.Bars.Count - 1;
			if (barFillRelno != alertFilled.Bars.BarStreaming.ParentBarsIndex) {
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
				if (quoteFilledThisAlertNullForLive.ParentBarStreaming != alertFilled.Bars.BarStreaming) {
					string msg = "NONSENSE#4";
					Assembler.PopupException(msg);
				}
				if (alertFilled.Bars != quoteFilledThisAlertNullForLive.ParentBarStreaming.ParentBars) {
					string msg = "NONSENSE#2";
					Assembler.PopupException(msg);
				}
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive = quoteFilledThisAlertNullForLive.Clone();	// CLONE_TO_FREEZE_AS_IT_HAPPENED_IGNORING_WHATEVER_HAPPENED_WITH_ORIGINAL_QUOTE_AFTERWARDS
				alertFilled.QuoteFilledThisAlertDuringBacktestNotLive.ItriggeredFillAtBidOrAsk = alertFilled.BidOrAskWillFillMe;
			}
			
			try {
				alertFilled.FillPositionAffectedEntryOrExitRespectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);
			} catch (Exception ex) {
				string msg = "REMOVE_FILLED_FROM_PENDING? DONT_USE_Bar.ContainsPrice()?";
				Assembler.PopupException(msg + msig, ex);
			}
			bool removed = this.ExecutionDataSnapshot.AlertsPending.Remove(alertFilled);
			if (removed == false) {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			PositionList positionsOpenedAfterAlertFilled = new PositionList("positionsOpenedAfterAlertFilled");
			PositionList positionsClosedAfterAlertFilled = new PositionList("positionsClosedAfterAlertFilled");

			if (alertFilled.IsEntryAlert) {
				this.ExecutionDataSnapshot.PositionsMasterOpenNewAdd(alertFilled.PositionAffected);
				positionOpenedAfterAlertFilled = alertFilled.PositionAffected;
				positionsOpenedAfterAlertFilled.AddOpened_step1of2(positionOpenedAfterAlertFilled);
			} else {
				this.ExecutionDataSnapshot.MovePositionOpenToClosed(alertFilled.PositionAffected);
				positionClosedAfterAlertFilled = alertFilled.PositionAffected;
				positionsClosedAfterAlertFilled.AddClosed(positionClosedAfterAlertFilled);
			}

			bool willEmit = this.Backtester.IsBacktestingNow == false && this.OrderProcessor != null && this.IsStrategyEmittingOrders;
			bool setStatusSubmitting = this.IsStreamingTriggeringScript && this.IsStrategyEmittingOrders;

			PositionPrototype proto = alertFilled.PositionAffected.Prototype;
			if (proto != null) {
				// 0. once again, set ExitAlert to What was actually filled, because prototypeEntry created SL & TP, which were both written into ExitAlert;
				// so if we caught the Loss and SL was executed, position.ExitAlert will still contain TP if we don't set it here
				bool exitIsDifferent = alertFilled.PositionAffected.ExitAlert != null && alertFilled.PositionAffected.ExitAlert != alertFilled;
				if (alertFilled.IsExitAlert && exitIsDifferent) {
					alertFilled.PositionAffected.ExitAlertAttach(alertFilled);
				}
				// 1. alert.PositionAffected.Prototype.StopLossAlertForAnnihilation and TP will get assigned
				alertsNewAfterAlertFilled.AddRange(this.PositionPrototypeActivator.AlertFilledCreateSlTpOrAnnihilateCounterparty(alertFilled));
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

				if (alertsNewAfterAlertFilled.Count > 0 && willEmit) {
					this.OrderProcessor.CreateOrdersSubmitToBrokerAdapterInNewThreads(alertsNewAfterAlertFilled.InnerList, setStatusSubmitting, true);

					// 3. Script using proto might move SL and TP which require ORDERS to be moved, not NULLs
					int twoMinutes = 120000;
					if (alertFilled.IsEntryAlert) {
						// there must be SL.OrderFollowed!=null and TP.OrderFollowed!=null
						if (proto.StopLossAlertForAnnihilation.OrderFollowed == null) {
							string msg = "StopLossAlert.OrderFollowed is NULL!!! engaging ManualResetEvent.WaitOne()";
							this.PopupException(msg);
							Stopwatch waitedForStopLossOrder = new Stopwatch();
							waitedForStopLossOrder.Start();
							proto.StopLossAlertForAnnihilation.MreOrderFollowedIsAssignedNow.WaitOne(twoMinutes);
							waitedForStopLossOrder.Stop();
							msg = "waited " + waitedForStopLossOrder.ElapsedMilliseconds + "ms for StopLossAlert.OrderFollowed";
							if (proto.StopLossAlertForAnnihilation.OrderFollowed == null) {
								msg += ": NO_SUCCESS still null!!!";
								this.PopupException(msg);
							} else {
								proto.StopLossAlertForAnnihilation.OrderFollowed.AppendMessage(msg);
								this.PopupException(msg);
							}
						} else {
							string msg = "you are definitely crazy, StopLossAlert.OrderFollowed is a single-threaded assignment";
							Assembler.PopupException(msg);
						}

						if (proto.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
							string msg = "TakeProfitAlert.OrderFollowed is NULL!!! engaging ManualResetEvent.WaitOne()";
							this.PopupException(msg);
							Stopwatch waitedForTakeProfitOrder = new Stopwatch();
							waitedForTakeProfitOrder.Start();
							proto.TakeProfitAlertForAnnihilation.MreOrderFollowedIsAssignedNow.WaitOne(twoMinutes);
							waitedForTakeProfitOrder.Stop();
							msg = "waited " + waitedForTakeProfitOrder.ElapsedMilliseconds + "ms for TakeProfitAlert.OrderFollowed";
							if (proto.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
								msg += ": NO_SUCCESS still null!!!";
								this.PopupException(msg);
							} else {
								proto.TakeProfitAlertForAnnihilation.OrderFollowed.AppendMessage(msg);
								this.PopupException(msg);
							}
						} else {
							string msg = "you are definitely crazy, TakeProfitAlert.OrderFollowed is a single-threaded assignment";
							Assembler.PopupException(msg);
						}
					}
				}
			}

			if (this.Backtester.IsBacktestingNow) {
				string msg = "AFTER_BACKTEST_HOOK_INVOKES_Performance.BuildStatsOnBacktestFinished()_AND_ReportersFormsManager.BuildReportFullOnBacktestFinished()";
				return;
			}

			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(quoteFilledThisAlertNullForLive, alertsNewAfterAlertFilled,
			                                                 positionsOpenedAfterAlertFilled,
			                                                 positionsClosedAfterAlertFilled,
			                                                 null
			                                                );
			//v1 this.AddPositionsToChartShadowAndPushPositionsOpenedClosedToReportersAsyncUnsafe(pokeUnit);
			if (positionOpenedAfterAlertFilled != null) {
				this.Performance.BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(positionOpenedAfterAlertFilled);
				// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
				this.EventGenerator.RaiseBrokerFilledAlertsOpeningForPositions_step1of3(pokeUnit);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
			}
			if (positionClosedAfterAlertFilled != null) {
				this.Performance.BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(positionClosedAfterAlertFilled);
				// Sq1.Core.DLL doesn't know anything about ReportersFormsManager => Events
				this.EventGenerator.RaiseBrokerFilledAlertsClosingForPositions_step3of3(pokeUnit);		// WHOLE_POKE_UNIT_BECAUSE_EVENT_HANLDER_MAY_NEED_POSITIONS_CLOSED_AND_OPENED_TOGETHER
			}
			this.ChartShadow.PositionsRealtimeAdd(pokeUnit);

			// 4. Script event will generate a StopLossMove PostponedHook
			this.invokeScriptEvents(alertFilled);

			// reasons for (alertsNewAfterExec.Count > 0) include:
			// 2.1. PrototypeActivator::AlertFilledPlaceSlTpOrAnnihilateCounterparty
			// 2.2. Script.OnAlertFilledCallback(alert)
			// 2.3. Script.OnPositionOpenedPrototypeSlTpPlacedCallback(alert.PositionAffected)
			// 2.4. Script.OnPositionClosedCallback(alert.PositionAffected)
		}
		void invokeScriptEvents(Alert alert) {
			if (this.Strategy.Script == null) return;
			try {
				this.Strategy.Script.OnAlertFilledCallback(alert);
			} catch (Exception e) {
				string msg = "fix your OnAlertFilledCallback() in script[" + this.Strategy.Script.StrategyName + "]"
					+ "; was invoked with alert[" + alert + "]";
				this.PopupException(msg, e);
			}
			if (alert.IsEntryAlert) {
				try {
					if (alert.PositionAffected.Prototype != null) {
						this.Strategy.Script.OnPositionOpenedPrototypeSlTpPlacedCallback(alert.PositionAffected);
					} else {
						this.Strategy.Script.OnPositionOpenedCallback(alert.PositionAffected);
					}
				} catch (Exception e) {
					string msg = "fix your ExecuteOnPositionOpened() in script[" + this.Strategy.Script.StrategyName + "]"
						+ "; was invoked with PositionAffected[" + alert.PositionAffected + "]";
					this.PopupException(msg, e);
				}
			} else {
				try {
					this.Strategy.Script.OnPositionClosedCallback(alert.PositionAffected);
				} catch (Exception e) {
					string msg = "fix your OnPositionClosedCallback() in script[" + this.Strategy.Script.StrategyName + "]"
						+ "; was invoked with PositionAffected[" + alert.PositionAffected + "]";
					this.PopupException(msg, e);
				}
			}
		}
		public void RemovePendingExitAlert(Alert alert, string msig) {
			string msg = "";
			ExecutionDataSnapshot snap = alert.Strategy.Script.Executor.ExecutionDataSnapshot;
			//this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			string orderState = (alert.OrderFollowed == null) ? "alert.OrderFollowed=NULL" : alert.OrderFollowed.State.ToString();
			if (snap.AlertsPending.ContainsInInnerList(alert)) {
				bool removed = snap.AlertsPending.Remove(alert);
				msg = "REMOVED " + orderState + " Pending alert[" + alert + "] ";
			} else {
				msg = "CANT_BE_REMOVED " + orderState + " isn't Pending alert[" + alert + "] ";
			}
			if (alert.OrderFollowed == null) {
				if (this.Backtester.IsBacktestingNow == false) {
					msg = "RealTime alerts should NOT have OrderFollowed=null; " + msg;
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return;
			}
			// OrderFollowed=null when executeStrategyBacktestEntryPoint() is in the call stack
			this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
		}
		public void RemovePendingExitAlertPastDueClosePosition(Alert alert) {
			string msig = "RemovePendingEntryCloneToExitAlertClosePosition(): ";
			this.RemovePendingExitAlert(alert, msig);
			//bool checkPositionOpenNow = true;
			//if (this.checkPositionCanBeClosed(alert, msig, checkPositionOpenNow) == false) return;

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerAdapter knows to pass Bar here?...
			//v1 STREAMING DOESNT BELONG??? Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLastNullUnsafe;
			//v2 NO_I_NEED_STREAMING_BAR_NOT_THE_SAME_I_OPENED_THE_LEFTOVER_POSITION
			Bar barFill = alert.Bars.BarStaticLastNullUnsafe;
			// HACK adding streaming LIVE to where BACKTEST_BARS_CLONED_FOR_ just ended; to avoid NPE at "if (exitBar.Open != this.ExitBar.Open) {"
			//v3 alert.Bars.BarCreateAppendBindStatic(barFill.DateTimeOpen, barFill.Open, barFill.High, barFill.Low, barFill.Close, barFill.Volume);
			//v4 alert.Bars.BarStreamingCreateNewOrAbsorb(this.Bars.BarStreaming);
			alert.FillPositionAffectedEntryOrExitRespectively(barFill, barFill.ParentBarsIndex, barFill.Close, alert.Qty, 0, 0);
			alert.SignalName += " RemovePendingExitAlertClosePosition Forced";
			// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Position.EntryAlert was filled!!!
			//alert.PositionAffected.ExitAlertAttach(alert);

			bool absenseInPositionsOpenNowIsAnError = true;
			this.ExecutionDataSnapshot.MovePositionOpenToClosed(alert.PositionAffected, absenseInPositionsOpenNowIsAnError);
		}
		public void RemovePendingEntry(Alert alert) {
			string msig = "RemovePendingEntry(): ";

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerAdapter knows to pass Bar here?...
			Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLastNullUnsafe;
			alert.FillPositionAffectedEntryOrExitRespectively(barFill, barFill.ParentBarsIndex, barFill.Close, alert.Qty, 0, 0);
			alert.SignalName += " RemovePendingEntryAlertClosePosition Forced";
		}
		public void ClosePositionWithAlertClonedFromEntryBacktestEnded(Alert alert) {
			string msig = " ClosePositionWithAlertClonedFromEntryBacktestEnded():";
			this.RemovePendingExitAlert(alert, msig);
			bool checkPositionOpenNow = true;
			if (this.checkPositionCanBeClosed(alert, msig, checkPositionOpenNow) == false) return;
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
			alert.PositionAffected.FillExitWith(alert.FilledBar, alert.PriceScript, alert.Qty, 0, 0);
			//alertFilled.FillPositionAffectedEntryOrExitRespectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);

			bool absenseInPositionsOpenNowIsAnError = false;
			this.ExecutionDataSnapshot.MovePositionOpenToClosed(alert.PositionAffected, absenseInPositionsOpenNowIsAnError);
		}
		private bool checkPositionCanBeClosed(Alert alert, string msig, bool checkPositionOpenNow = true) {
			if (alert.PositionAffected == null) {
				string msg = "can't close PositionAffected and remove Position from PositionsOpenNow"
					+ ": alert.PositionAffected=null for alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
				return false;
			}
			if (alert.IsExitAlert) {
				string msg = "Sorry I don't serve alerts.IsExitAlert=true, only .IsEntryAlert's: alert [" + alert + "]";
				//throw new Exception(msig + msg);
				this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
				return false;
			}
			if (checkPositionOpenNow == true) {
				bool shouldRemove = this.ExecutionDataSnapshot.PositionsOpenNow.ContainsInInnerList(alert.PositionAffected);

				if (alert.FilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only BarRelnoFilled==-1"
						+ " otherwize alert.FillPositionAffectedEntryOrExitRespectively() with throw: alert [" + alert + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
					return false;
				}
				if (alert.PositionAffected.ExitFilledBarIndex > -1) {
					if (shouldRemove) {
						int a = 1;
					}
					string msg = "CHECK_POSITION_OPEN_NOW Sorry I serve only alert.PositionAffected.ExitBar==-1"
						+ " otherwize PositionAffected.FillExitAlert() will throw: alert [" + alert + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
					return false;
				}
			} else {
				Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLastNullUnsafe;
				if (alert.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: can't do my job: alert.PositionAffected.EntryBar!=-1 for alert [" + alert + "]";
					//throw new Exception(msig + msg);
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
					//return;
				} else {
					string msg = "Forcibly closing at EntryBar=[" + barFill + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(alert.OrderFollowed, msig + msg);
				}

			}
			return true;
		}

		Bars		preBacktestBars;
		DataSource	preDataSource;
		bool		preBacktestIsStreaming;
		internal void BacktestContextInitialize(Bars bars) {
			if (this.Bars.DataSource.StreamingAdapter != null) {
				this.Bars.DataSource.PumpingAutoPauseFor(this);
			} else {
				string msg = "NOT_PAUSING_QUOTE_PUMP StreamingAdapter=null //BacktestContextInitialize(" + bars + ")";
				Assembler.PopupException(msg, null, false);
				// WHO_NEEDS_IT? channel.QuotePump.PushConsumersPaused = true;
			}
			
			this.preBacktestBars = this.Bars;	// this.preBacktestBars != null will help ignore this.IsStreaming saving IsStreaming state to json
			this.preDataSource = this.DataSource;
			this.preBacktestIsStreaming = this.IsStreamingTriggeringScript;

			if (this.Bars == bars) {
				string msg = "LIFECYCLE_INCONSISTENT__BARS_ALREADY_INITIALIZED " + this.Bars;
				Assembler.PopupException(msg);
			} else {
				this.Bars = bars;
				bool indicatorsHaveNoErrorsCanStartBacktesting = true;
				foreach (Indicator indicator in this.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {
					indicatorsHaveNoErrorsCanStartBacktesting &= indicator.BacktestStartingConstructOwnValuesValidateParameters(this);
				}
				if (indicatorsHaveNoErrorsCanStartBacktesting == false) {
					string msg = "I_SHOULD_ABORT_BACKTEST_NOW_HERE_BUT_DONT_HAVE_A_MECHANISM indicatorsHaveNoErrorsCanStartBacktesting=false";
					Assembler.PopupException(msg);
					throw new Exception(msg);
				}
			}
			//this.DataSource = bars.DataSource;
			if (this.preBacktestBars != null) {
				string msg = "NOT_SAVING_IsStreamingTriggeringScript=ON_FOR_BACKTEST"
					+ " preBacktestIsStreaming[" + this.preBacktestIsStreaming + "] preBacktestBars[" + this.preBacktestBars + "]";
				//Assembler.PopupException(msg, null, false);
			}
			this.IsStreamingTriggeringScript = true;
			//this.Strategy.ScriptBase.Initialize(this);
		}
		internal void BacktestContextRestore() {
			this.Bars = this.preBacktestBars;
			foreach (Indicator indicator in this.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Values) {
				if (indicator.OwnValuesCalculated.Count != this.Bars.Count - 1) {
					string state = "MA.OwnValues.Count=499, MA.BarsEffective.Count=500[0...499], MA.BarsEffective.BarStreaming=null <= that's why indicator has 1 less";
					string msg = "YOU_ABORTED_LIVESIM_BUT_DIDNT_RECALCULATE_INDICATORS? REMOVE_HOLES_IN_INDICATOR " + indicator;
					Assembler.PopupException(msg, null, false);
				}
				indicator.BacktestContextRestoreSwitchToOriginalBarsContinueToLiveNorecalculate();
			}

			//this.DataSource = this.preDataSource;
			this.IsStreamingTriggeringScript = preBacktestIsStreaming;
			// MOVED_HERE_AFTER_ASSIGNING_IS_STREAMING_TO"avoiding saving strategy each backtest due to streaming simulation switch on/off"
			this.preBacktestBars = null;	// will help ignore this.IsStreaming saving IsStreaming state to json

			StreamingAdapter streaming = this.DataSource.StreamingAdapter;
			if (streaming != null) {
				this.Bars.DataSource.PumpAutoResumeFor(this);
			} else {
				string msg = "NOT_UNPAUSING_QUOTE_PUMP StreamingAdapter=null //BacktestContextRestore(" + this.Bars + ")";
				Assembler.PopupException(msg, null, false);
				// WHO_NEEDS_IT? channel.QuotePump.PushConsumersPaused = false;
			}
		}
		public void BacktesterAbortIfRunningRestoreContext() {
			if (this.Backtester.IsBacktestingNow == false) return;
			// TODO INTRODUCE_NEW_MANUAL_RESET_SO_THAT_NEW_BACKTEST_WAITS_UNTIL_TERMINATION_OF_THIS_METHOD_TO_AVOID_BROKEN_DISTRIBUTION_CHANNELS
			this.Backtester.AbortRunningBacktestWaitAborted("USER_CHANGED_SELECTORS_IN_GUI_NEW_BACKTEST_IS_ALMOST_TASK.SCHEDULED");
			//ALREADY_RESTORED_BY_simulationPostBarsRestore() this.BacktestContextRestore();
		}

		public void BacktesterRunSimulation() {
			try {
				this.barStaticExecutedLast = null;
				this.ExecutionDataSnapshot.Initialize();
				//MOVED_TO_CTOR() this.Performance = new SystemPerformance(this);
				this.Performance.Initialize();
				this.Strategy.Script.InitializeBacktestWrapper();

				if (this.ChartShadow != null) {
					this.ChartShadow.SetIndicators(this.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances);
				}
				
				this.Backtester.Initialize();
				this.Backtester.RunSimulation();
				if (this.Backtester.IsLivesimRunning == false) {
					this.Performance.BuildStatsOnBacktestFinished();
				}
			} catch (Exception ex) {
				string msg = "RUN_SIMULATION_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]";
				Assembler.PopupException(msg, ex);
			} finally {
				this.Backtester.SetRunningFalseNotifyWaitingThreadsBacktestCompleted();
			}
		}
		public void BacktesterRunSimulationTrampoline(Action<ScriptExecutor> executeAfterSimulationEvenIfIFailed = null, bool inNewThread = true) {
			if (this.Strategy == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy=null; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.Strategy.Script == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy.Script=null, didn't compile; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.Bars == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Bars=null; select 1) TimeFrame 2) Range 3) PositionSize - for corresponding Chart; " + this;
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			if (this.Backtester.IsBacktestingNow) {
				this.Backtester.AbortRunningBacktestWaitAborted("ALREADY_BACKTESTING_this.Backtester.IsBacktestingNow");
			}

			//???????
			//AFTER F6 I want to run backtest with one slider changed; I click on the slider and get "did you forget to initialize Executor?..." error
			//TOO_LATE_MOVED_TO_AFTER_Strategy.CompileInstantiate() this.Strategy.Script.Initialize(this);
			// only to reset the Glyphs and Positions
			//this.ChartForm.Chart.Renderer.InitializeBarsInvalidateChart(this.Executor);
			//this.Executor.Renderer.InitializeBarsInvalidateChart(this.Executor);
			//this.ChartShadow.ScriptToChartCommunicator.PositionsBacktestClearAfterChartPickedUp();
			if (this.ChartShadow != null) this.ChartShadow.ClearAllScriptObjectsBeforeBacktest();

			//if (this.Strategy.ActivatedFromDll) {
				// FIXED "EnterEveryBar doesn't draw MAfast"; editor-typed strategies already have indicators in SNAP after pre-backtest compilation
				// DONT_COMMENT_LINE_BELOW indicators get lost when BacktestOnRestart = true
				this.Strategy.Script.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
			//}
			this.Strategy.Script.PushRegisteredScriptParametersIntoCurrentContextSaveStrategy();
			
			//inNewThread = false;
			if (inNewThread) {
				int ThreadPoolAvailablePercentageLimit = 20;
				int threadPoolAvailablePercentage = this.getThreadPoolAvailablePercentage();
				if (threadPoolAvailablePercentage < ThreadPoolAvailablePercentageLimit) {
					string msg = "NOT_SCHEDULING_RUN_SIMULATION QueueUserWorkItem(backtesterRunSimulationThreadEntryPoint)"
						+ " because threadPoolAvailablePercentage[" + threadPoolAvailablePercentage
						+ "]<" + ThreadPoolAvailablePercentageLimit + "%";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				//this.MainForm.PopupException("SCHEDULING_RUN_SIMULATION for Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]");

				//v1
				//ThreadPool.QueueUserWorkItem(new WaitCallback(this.backtesterRunSimulationThreadEntryPoint));
				
				//v2
				//http://stackoverflow.com/questions/7582853/what-wpf-threading-approach-should-i-go-with/7584422#7584422
				//Task.Factory.StartNew(() => {
				//	// Background work
				//	this.backtesterRunSimulationThreadEntryPoint();
				//}).ContinueWith((t) => {
				//	// Update UI thread
				//	executeAfterSimulation();
				//}, TaskScheduler.FromCurrentSynchronizationContext());

				//v3
				Task backtesterTask = new Task(this.BacktesterRunSimulation);
				if (executeAfterSimulationEvenIfIFailed != null) {
					backtesterTask.ContinueWith((t) => { executeAfterSimulationEvenIfIFailed(this); });
				}
				//ON_REQUESTING_ABORT_TASK_DIES_WITHOUT_INVOKING_CONTINUE_WITH started.Start(TaskScheduler.FromCurrentSynchronizationContext());
				backtesterTask.Start();
			} else {
				//this.Executor.BacktesterRunSimulation();
				//this.ChartForm.Chart.DoInvalidate();
				this.BacktesterRunSimulation();
				if (executeAfterSimulationEvenIfIFailed != null) {
					executeAfterSimulationEvenIfIFailed(this);
				}
			}
		}
		int getThreadPoolAvailablePercentage() {
			int workerThreadsAvailable, completionPortThreadsAvailable = 0;
			ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
			int workerThreadsMax, completionPortThreadsMax = 0;
			ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);
			return (completionPortThreadsMax / completionPortThreadsAvailable) * 100;
		}
		void clonePositionsForChartPickupRealtime(ReporterPokeUnit pokeUnit) {
		}
		public void SetBars(Bars barsClicked) {
			if (barsClicked == null) {
				string msg = "don't feed Bars=null into the foodchain!";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.Bars == barsClicked) {
				string msg = "DONT_SET_SAME_BARS";
			}
			if (this.Backtester.IsBacktestingNow) {
				this.Backtester.AbortRunningBacktestWaitAborted("CLICKED_ON_OTHER_BARS_WHILE_BACKTESTING");
			}

			//v2, ATTACHED_TO_BARS.DATASOURCE.SYMBOLRENAMED_INSTEAD_OF_DATASOURCE_REPOSITORY
			// DATASOURCE_WILL_NOT_RENAME_YOUR_INSTANTIATED_BARS_BUT_WILL_RENAME_BARFILE_AND_SYMBOL_IN_BARFILE_HEADER
			if (this.Bars != null && this.Bars.DataSource != null) {
				// unfollowing old Bars (so that it'll be only renaming .BAR file); most likely those Bars will be GarbageCollected
				this.Bars.DataSource.SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars -=
					new EventHandler<DataSourceSymbolRenamedEventArgs>(barDataSource_SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars);
			}
			this.Bars = barsClicked;
			this.Bars.DataSource.SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars +=
				new EventHandler<DataSourceSymbolRenamedEventArgs>(barDataSource_SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars);
		}
		void barDataSource_SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars(object sender, DataSourceSymbolRenamedEventArgs e) {
			if (this.Bars == null) {
				string msg = "INITIALIZED_EXECUTOR_ALWAYS_HAVE_BARS MAKE_SURE_YOU_UNSUBSCRIBED_ME_FROM_PREVIOUS_BARS";
				return;
			}
			if (e.DataSource != this.Bars.DataSource) {
				string msg = "I_SHOULD_NOT_BE_NOTIFIED_ABOUT_OTHER_DATASOURCES";
				return;
			}
			if (e.Symbol == this.Bars.Symbol) {
				string msg = "I_SHOULD_NOT_BE_NOTIFIED_IF_SYMBOL_WAS_NOT_RENAMED";
				return;
			}
			if (this.IsStreamingTriggeringScript) {
				string msg = "EXECUTOR_REFUSED_TO_RENAME_STREAMING_BARS"
					+ " TO_CREATE_BACKUP_TURN_STREAMING_OFF_RENAME_STREAMING_BACK_ON"
					+ " TO_RENAME_PERMANENTLY_IMPLMEMENT_SYMBOL_MAPPING_FOR_STREAMING_SYMBOLS";
				Assembler.PopupException(msg);
				e.CancelRepositoryRenameExecutorRefusedToRenameWasStreamingTheseBars = true;
				return;
			}
			this.Bars.RenameSymbol(e.Symbol);
			this.ChartShadow.SyncBarsIdentDueToSymbolRename();
			
			if (this.Strategy == null) return;
			if (this.Strategy.ScriptContextCurrent.Symbol == this.Bars.Symbol) return;
			this.Strategy.ScriptContextCurrent.Symbol  = this.Bars.Symbol;
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);

		}
		public void AlertKillPending(Alert alert) {
			//if (this.Backtester.IsBacktestingNow) {
			if (this.Backtester.IsBacktestingOrLivesimNow) {
				this.MarketsimBacktest.SimulateAlertKillPending(alert);
			} else {
				this.MarketLive.AlertKillPending(alert);
			}
		}
		public double PositionSizeCalculate(Bar bar, double priceScriptAligned) {
			double ret = 1;
			SymbolInfo symbolInfo = bar.ParentBars.SymbolInfo;
			if (symbolInfo.SecurityType == SecurityType.Future && symbolInfo.LeverageForFutures <= 0.0) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new ArgumentException("Margin must be greater than zero");
			}
			//double currentEquity = this.Performance.SlicesShortAndLong.CurrentEquityForPosSizeCalculator;
			switch (this.PositionSize.Mode) {
				case PositionSizeMode.DollarsConstantForEachTrade:
					//if (symbolInfo.SecurityType == SecurityType.Future) {
					//	ret = this.PositionSize.DollarsConstantForEachTrade / symbolInfo.LeverageForFutures;
					//} else {
						ret = this.PositionSize.DollarsConstantEachTrade / priceScriptAligned;
					//}
					break;
				case PositionSizeMode.SharesConstantEachTrade:
					ret = this.PositionSize.SharesConstantEachTrade;
					break;
				default:
					string msg = "RETURNING_ONE_SHARE_UNSUPPORED_PositionSizeMode [" + this.PositionSize.Mode + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new NotImplementedException();
			}
			if (ret == 0.0) return ret;
			
			if (symbolInfo.SecurityType == SecurityType.Stock) {
				ret = (double)((int)ret);
			}
			ContextScript scriptContext = this.Strategy.ScriptContextCurrent;
			if (symbolInfo.SecurityType == SecurityType.Stock && scriptContext.RoundEquityLots) {
				double roundedLots100 = (ret / 100.0) * 100.0;
				if (ret < 100.0 && scriptContext.RoundEquityLotsToUpperHundred) {
					roundedLots100 = 100.0;
				}
				ret = roundedLots100;
			}
			return ret;
		}
		public ScriptExecutor CloneForOptimizer(ContextScript ctxNext) {	// HACKY_BUT_NOT_TOO_BAD
			//ScriptExecutor clone = base.MemberwiseClone();
			// detach the chart clone.On
			ScriptExecutor executorClone = new ScriptExecutor();
			executorClone.Bars = this.Bars;
			executorClone.OrderProcessor = null;

			Strategy strategyClone = this.Strategy.CloneWithNewScriptInstanceResetContextsToSingle(ctxNext, executorClone);
			if (strategyClone == this.Strategy) {
				Debugger.Break();
			}
			strategyClone.Script.Initialize(executorClone);
			executorClone.Initialize(null, strategyClone);
			//KEEP_DOWNSTACK strategyClone.ContextSwitchCurrentToNamedAndSerialize(ctxNext.Name, false);

			strategyClone.ScriptContextsByName = new Dictionary<string, ContextScript>();
			strategyClone.ScriptContextAdd(ctxNext.Name, ctxNext, true);

			return executorClone;
		}
		public override string ToString() {
			string ret = this.StrategyName;
			if (this.Strategy == null) return ret;
			if (this.Strategy.ScriptContextCurrent == null) return ret;
			ret += " @ " + this.Strategy.ScriptContextCurrent.ToString();
			return ret;
		}
		public string ToStringWithCurrentParameters() {
			string dbg = this.Strategy.ScriptContextCurrent.Name + " "
				+ this.Strategy.Script.IndicatorParametersAsString;
			string dbg2 = "";
			if (this.Performance.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished != null) {
				foreach (string iName in this.Performance.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished.Keys) {
					IndicatorParameter ip = this.Performance.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished[iName];
					dbg2 += iName + "[" + ip.ValueCurrent + "]";
				}
			}
			return dbg + dbg2;
		}
	}
}