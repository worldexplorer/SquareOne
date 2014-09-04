using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;		//StackFrame

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Backtesting;
using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Support;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class ScriptExecutor {
		#region constructed (my own data)
		public ExecutionDataSnapshot ExecutionDataSnapshot { get; protected set; }
		public SystemPerformance Performance { get; protected set; }
		public Backtester Backtester { get; private set; }
		public PositionPrototypeActivator PositionPrototypeActivator { get; private set; }
		//public MarketSimStatic MarketSimStatic { get; private set; }
		public MarketRealStreaming MarketRealStreaming { get; private set; }
		public MarketSimStreaming MarketSimStreaming { get; private set; }
		public ScriptExecutorEventGenerator EventGenerator { get; private set; }
		public NotOnChartBarsHelper NotOnChartBarsHelper { get; private set; }
		public CommissionCalculator CommissionCalculator;
		#endregion
		
		#region initialized (sort of Dependency Injection)
		public ChartShadow ChartShadow;		// initialized with Sq1.Charting.ChartControl:ChartShadow
		// managing ScriptExecutorObjects is ChartControl's responsibility;  
		//public ScriptToChartCommunicator ScriptToChartCommunicator { get { return this.ChartShadow.ScriptToChartCommunicator; } }
		public Strategy Strategy;
		public string StrategyName { get { return (this.Strategy == null) ? "STRATEGY_NULL" : this.Strategy.Name; } }
		public OrderProcessor OrderProcessor { get; private set; }
		public IStatusReporter StatusReporter { get; private set; }
		#endregion

		#region volatile Script is recompiled and replaced
		public Script Script { get; private set; }
		public bool ScriptIsExecuting { get; internal set; }

		public Bars Bars { get; private set; }
		public PositionSize PositionSize {
			get {
				if (this.Strategy == null) {
					string msg = "ScriptExecutor.PositionSize: you should not access PositionSize before you've set Strategy";
					throw new Exception(msg);
				}
				return this.Strategy.ScriptContextCurrent.PositionSize;
			}
		}

		public DataSource DataSource {
			get {
				if (this.Bars == null) {
					string msg = "ScriptExecutor.DataSource: you should not access DataSource BEFORE you've set ScriptExecutor.Bars";
					throw new Exception(msg);
				}
				return this.Bars.DataSource;
			}
		}
		#endregion

		bool isStreamingWhenNoStrategyLoaded;
		bool isAutoSubmittingWhenNoStrategyLoaded;

		public bool IsStreaming {
			get {
				if (this.Strategy == null) return this.isStreamingWhenNoStrategyLoaded;
				else return this.Strategy.ScriptContextCurrent.ChartStreaming;
			}
			set {
				if (this.Strategy == null) {
					this.isStreamingWhenNoStrategyLoaded = value;
					return;
				}
				
				this.Strategy.ScriptContextCurrent.ChartStreaming = value;
				// we are in beginning the backtest and will switch back to preBacktestIsStreaming after backtest finishes;
				// if you AppKill during the backtest, you don't want btnStreaming be pressed (and disabled DataSource.StreamingProvider=null) after AppRestart 
				if (this.preBacktestBars != null) return;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			}
		}
		public bool IsAutoSubmitting {
			get {
				if (this.Strategy == null) return this.isAutoSubmittingWhenNoStrategyLoaded;
				else return this.Strategy.ScriptContextCurrent.ChartAutoSubmitting;
			}
			set {
				if (this.Strategy == null) {
					this.isAutoSubmittingWhenNoStrategyLoaded = value;
					return;
				}
				this.Strategy.ScriptContextCurrent.ChartAutoSubmitting = value;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			}
		}

		protected ScriptExecutor(ChartShadow chartShadow, Strategy strategy, 
		                         OrderProcessor orderProcessor, IStatusReporter statusReporter) : this() {
			this.Initialize(chartShadow, strategy, orderProcessor, statusReporter);
		}
		public ScriptExecutor() {
			this.IsStreaming = false;
			this.IsAutoSubmitting = false;

			this.ExecutionDataSnapshot = new ExecutionDataSnapshot(this);
			this.Performance = new SystemPerformance(this);
			this.Backtester = new Backtester(this);
			this.PositionPrototypeActivator = new PositionPrototypeActivator(this);
			this.MarketRealStreaming = new MarketRealStreaming(this);
			this.MarketSimStreaming = new MarketSimStreaming(this);
			this.EventGenerator = new ScriptExecutorEventGenerator(this);
			this.NotOnChartBarsHelper = new NotOnChartBarsHelper(this);
			this.CommissionCalculator = new CommissionCalculatorZero(this);
		}

		public void Initialize(ChartShadow chartShadow,
		                       Strategy strategy, OrderProcessor orderProcessor, IStatusReporter statusReporter) {

			string msg = " at this time, FOR SURE this.Bars==null, strategy.Script?=null";
			this.ChartShadow = chartShadow;
			this.Strategy = strategy;
			this.OrderProcessor = orderProcessor;
			this.StatusReporter = statusReporter;

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
				}
			}
			this.ExecutionDataSnapshot.Initialize();
			// Executor.Bars are NULL in ScriptExecutor.ctor() and NOT NULL in SetBars
			//this.Performance.Initialize();
			this.MarketSimStreaming.Initialize();
		}
		public ReporterPokeUnit ExecuteOnNewBarOrNewQuote(Quote quote) {
			if (this.Strategy.Script == null) return null;
			ReporterPokeUnit pokeUnit = new ReporterPokeUnit(quote);
			this.ExecutionDataSnapshot.PreExecutionClear();
			int alertsDumpedForStreamingBar = -1;

			if (quote != null) {
				try {
					this.Strategy.Script.OnNewQuoteOfStreamingBarCallback(quote);
					//alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
					//if (alertsDumpedForStreamingBar > 0) {
					//	string msg = "ITS OK HERE since prev quote has created prototype-based alerts"
					//		+ "I WANT DUMP TO BE VALID ONLY IN onNewBar case only!!!"
					//		+ " " + alertsDumpedForStreamingBar + " alerts Dumped for " + quote;
					//}
				} catch (Exception ex) {
					string msig = " Script[" + this.Strategy.Script.GetType().Name + "].OnNewQuoteCallback(" + quote + "): ";
					this.PopupException(ex.Message + msig, ex);
				}
			} else {
				try {
					this.Strategy.Script.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(this.Bars.BarStaticLast);
				} catch (Exception ex) {
					string msig = " Script[" + this.Strategy.Script.GetType().Name + "].OnNewBarCallback(" + quote + "): ";
					this.PopupException(ex.Message + msig, ex);
				}
			}
			string msg = "DONT_REMOVE_ALERT_SHOULD_LEAVE_ITS_TRAIL_DURING_LIFETIME_TO_PUT_UNFILLED_DOTS_ON_CHART";
			alertsDumpedForStreamingBar = this.ExecutionDataSnapshot.DumpPendingAlertsIntoPendingHistoryByBar();
			if (alertsDumpedForStreamingBar > 0) {
				msg += " DUMPED_AFTER_SCRIPT_EXECUTION_ON_NEW_BAR_OR_QUOTE";
			}


			// what's updated after Exec: non-volatile, kept un-reset until executor.Initialize():
			//this.ExecutionDataSnapshot.PositionsMasterByEntryBar (unique)
			//this.ExecutionDataSnapshot.PositionsMaster
			//this.PositionsOnlyActive
			//this.AlertsMaster
			//this.AlertsNewAfterExec

			// what's new for this iteration: volatile, cleared before next Exec):
			//this.AlertsNewAfterExec
			//this.ExecutionDataSnapshot.PositionsOpenedAfterExec
			//this.ExecutionDataSnapshot.PositionsClosedAfterExec

			bool willPlace = this.Backtester.IsBacktestingNow == false && this.OrderProcessor != null && this.IsAutoSubmitting;
			bool setStatusSubmitting = this.IsStreaming && this.IsAutoSubmitting;

			List<Alert> alertsNewAfterExecCopy = this.ExecutionDataSnapshot.AlertsNewAfterExecSafeCopy;
			if (alertsNewAfterExecCopy.Count > 0) {
				this.enrichAlertsWithQuoteCreated(alertsNewAfterExecCopy, quote);
				if (willPlace) {
					this.OrderProcessor.CreateOrdersSubmitToBrokerProviderInNewThreadGroups(alertsNewAfterExecCopy, setStatusSubmitting, true);
				}
			}

			if (this.Backtester.IsBacktestingNow && this.Backtester.WasBacktestAborted) return null;

			pokeUnit.AlertsNew = alertsNewAfterExecCopy;
			pokeUnit.PositionsOpened = this.ExecutionDataSnapshot.PositionsOpenedAfterExecSafeCopy;
			pokeUnit.PositionsClosed = this.ExecutionDataSnapshot.PositionsClosedAfterExecSafeCopy;
			return pokeUnit;
		}

		void enrichAlertsWithQuoteCreated(List<Alert> alertsAfterStrategy, Quote quote) {
			if (quote == null) return;
			foreach (Alert alert in alertsAfterStrategy) {
				if (alert.PlacedBarIndex != this.Bars.Count - 1) {
					string msg = "skipping enriching: alertsNewAfterExec should contain only lastBar alerts"
						+ "; got alertAfterStrategy.BarRelnoPlaced[" + alert.PlacedBarIndex + "]"
						+ "!= this.Bars.Count[" + this.Bars.Count + "] for alert[" + alert + "]";
					this.PopupException(msg);
					continue;
				}
				//log.Debug("quote!=null => setting for TradeHistory:" +
				//	" 1) alert.BrokerQuoteDateTime=quote.ServerTime[" + quote.ServerTime + "]" +
				//	" 2) alert.PriceDeposited=[" + priceDeposited + "] quote.DepositBuy[" + quote.FortsDepositBuy + "] quote.DepositSell[" + quote.FortsDepositSell + "] ");
				//if (alertAfterStrategy.DataRange == null) alertAfterStrategy.DataRange = this.BarDataRange;
				//alertAfterStrategy.PositionSize = this.PositionSize;
				alert.QuoteCreatedThisAlertServerTime = quote.ServerTime;
				alert.QuoteCreatedThisAlert = quote;
			}
		}
		public Position BuyOrShortAlertCreateDontRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
		                                                  Direction direction, MarketLimitStop entryMarketLimitStop) {
			return BuyOrShortAlertCreateRegister(entryBar, stopOrLimitPrice, entrySignalName,
			                                     direction, entryMarketLimitStop, false);
		}
		public void CheckThrowAlertCanBeCreated(Bar entryBar, string msig) {
			string invoker = (new StackFrame(3, true).GetMethod().Name) + "(): ";
			if (this.Bars == null) {
				throw new Exception(msig + " this.Bars=[null] " + invoker);
			}
			if (entryBar == null) {
				throw new Exception(msig + " for Bars=[" + this.Bars + "]" + invoker);
			}
		}
		public Position BuyOrShortAlertCreateRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
		                                              Direction direction, MarketLimitStop entryMarketLimitStop, bool registerInNew = true) {

			this.CheckThrowAlertCanBeCreated(entryBar, "BARS.BARSTREAMING_OR_BARS.BARLASTSTATIC_IS_NULL_BuyOrShortAlertCreateRegister() ");

			Alert alert = null;
			// real-time streaming should create its own Position after an Order gets filled
			if (this.IsStreaming) {
				alert = this.MarketRealStreaming.EntryAlertCreate(entryBar, stopOrLimitPrice, entrySignalName,
				                                                  direction, entryMarketLimitStop);
			} else {
				//alert = this.MarketSimStatic.EntryAlertCreate(entryBar, stopOrLimitPrice, entrySignalName,
				//	direction, entryMarketLimitStop);
				Debugger.Break();
			}
			Alert similar = this.ExecutionDataSnapshot.FindSimilarNotSamePendingAlert(alert);
			if (similar != null) {
				return similar.PositionAffected;
			}

			this.ExecutionDataSnapshot.AlertEnrichedRegister(alert, registerInNew);

			// ok for single-entry strategies; nogut if we had many Streaming alerts and none of orders was filled yet...
			// MOVED_TO_ON_ALERT_FILLED_CALBACK
			Position pos = new Position(alert, alert.PriceScript);
			alert.PositionAffected = pos;
			return pos;
		}
		public Alert SellOrCoverAlertCreateDontRegister(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
		                                                Direction direction, MarketLimitStop exitMarketLimitStop) {
			return this.SellOrCoverAlertCreateRegister(exitBar, position, stopOrLimitPrice, signalName,
			                                           direction, exitMarketLimitStop, false);
		}
		public Alert SellOrCoverAlertCreateRegister(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
		                                            Direction direction, MarketLimitStop exitMarketLimitStop, bool registerInNewAfterExec = true) {

			this.CheckThrowAlertCanBeCreated(exitBar, "BARS.BARSTREAMING_OR_BARS.BARLASTSTATIC_IS_NULL_SellOrCoverAlertCreateRegister() ");
			if (position == null) throw new Exception("POSITION_CAN_NOT_BE_NULL_SellOrCoverAlertCreateRegister()");

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
				foreach (Alert closingAlertForPosition in this.ExecutionDataSnapshot.AlertsPending) {
					if (closingAlertForPosition.PositionAffected == position && closingAlertForPosition.IsExitAlert) {
						string msg = "PENDING_EXIT_ALERT_FOUND_WHILE_POSITION.EXITALERT=NULL"
							+ "; position.ExitAlert[" + position.ExitAlert + "] != closingAlertForPosition[" + closingAlertForPosition + "]";
						this.PopupException(msg);
						return closingAlertForPosition;
					}
				}
			}

			if (this.IsStreaming) {
				alert = this.MarketRealStreaming.ExitAlertCreate(exitBar, position, stopOrLimitPrice, signalName,
				                                                 direction, exitMarketLimitStop);
			} else {
				//alert = this.MarketSimStatic.ExitAlertCreate(exitBar, position, stopOrLimitPrice, signalName,
				//	  direction, exitMarketLimitStop);
				Debugger.Break();
			}

			this.ExecutionDataSnapshot.AlertEnrichedRegister(alert, registerInNewAfterExec);

			return alert;
		}
		public bool AnnihilateCounterpartyAlertDispatched(Alert alert) {
			if (alert == null) {
				string msg = "don't invoke KillAlert with alert=null; check for TP=0 or SL=0 prior to invocation";
				throw new Exception(msg);
			}
			bool killed = false;
			if (this.IsStreaming) {
				if (this.Backtester.IsBacktestingNow == true) {
					killed = this.MarketSimStreaming.AnnihilateCounterpartyAlert(alert);
					//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
				} else {
					killed = this.MarketRealStreaming.AnnihilateCounterpartyAlert(alert);
				}
			} else {
				//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
				Debugger.Break();
			}
			return killed;
		}
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
			orderPrice = bars.SymbolInfo.RoundAlertPriceToPriceLevel(orderPrice, buyOrShort, positionLongShort0, marketLimitStop0);
			return orderPrice;
		}
		
		public void PopupException(string msg, Exception ex = null) {
			ex = new Exception(msg, ex);
			if (this.StatusReporter == null) return;
			this.StatusReporter.PopupException(ex);
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

		//DateTime lastTimeReportersPoked = DateTime.MinValue;
		public void PushPositionsOpenedClosedToReportersAsyncUnsafe(ReporterPokeUnit pokeUnit) {
			// EMPTY_AFTEREXEC check#1
			if (pokeUnit.PositionsChanged == 0) return;

			this.clonePositionsForChartPickupRealtime(pokeUnit);
			this.Performance.BuildStatsIncrementallyOnEachBarExecFinished(pokeUnit);
		}

		public void CreatedOrderWontBePlacedPastDueInvokeScript(Alert alert, int barNotSubmittedRelno) {
			//this.ExecutionDataSnapshot.AlertsPendingRemove(alert);
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
			if (this.ExecutionDataSnapshot.AlertsPendingContains(alert) == false) {
				string msg = "Alert wasn't found in snap.AlertsPending";
				throw new Exception(msg);
			}
			bool removed = this.ExecutionDataSnapshot.AlertsPendingRemove(alert);
			alert.Strategy.Script.OnAlertKilledCallback(alert);
		}
		public void CallbackAlertFilledMoveAroundInvokeScript(Alert alertFilled, Quote quote,
					 int barFillRelno, double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			string msig = " CallbackAlertFilledMoveAroundInvokeScript(" + alertFilled + ", " + quote + ")";
			List<Alert> alertsNewAfterAlertFilled = new List<Alert>();
			List<Position> positionsOpenedAfterAlertFilled = new List<Position>();
			List<Position> positionsClosedAfterAlertFilled = new List<Position>();

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerProvider knows to pass Bar here?...
			Bar barFill = (this.IsStreaming) ? alertFilled.Bars.BarStreamingCloneReadonly : alertFilled.Bars.BarStaticLast;
			if (barFillRelno != barFill.ParentBarsIndex) {
				string msg = "barFillRelno[" + barFillRelno + "] != barFill.ParentBarsIndex["
					+ barFill.ParentBarsIndex + "]; barFill=[" + barFill + "]";
				Assembler.PopupException(msg);
			}
			if (priceFill == -1) {
				string msg = "won't set priceFill=-1 for alert [" + alertFilled + "]";
				throw new Exception(msg);
			}
			if (alertFilled.PositionAffected == null) {
				string msg = "CallbackAlertFilled can't do its job: alert.PositionAffected=null for alert [" + alertFilled + "]";
				throw new Exception(msg);
			}
			if (alertFilled.IsEntryAlert) {
				if (alertFilled.PositionAffected.EntryFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.EntryBar!=-1 for alert [" + alertFilled + "]";
					throw new Exception(msg);
				} else {
					string msg = "initializing EntryBar=[" + barFill + "] on AlertFilled";
				}
			} else {
				if (alertFilled.PositionAffected.ExitFilledBarIndex != -1) {
					string msg = "DUPE: CallbackAlertFilled can't do its job: alert.PositionAffected.ExitBar!=-1 for alert [" + alertFilled + "]";
					throw new Exception(msg);
					return;
				} else {
					string msg = "initializing ExitBar=[" + barFill + "] on AlertFilled";
				}
			}

			if (quote == null) {
				quote = this.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(alertFilled.Symbol);
				// TODO: here quote will have NO_PARENT_BARS, since StreamingDataSnapshot contains anonymous quote;
				// I should keep per-timeframe / per-distributionChannel LastQuote to have ParentBar= different StreamingBar 's
				// bindStreamingBarForQuoteAndPushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar.Clone());
			}
			alertFilled.QuoteLastWhenThisAlertFilled = quote;
			try {
				alertFilled.FillPositionAffectedEntryOrExitRespectively(barFill, barFillRelno, priceFill, qtyFill, slippageFill, commissionFill);
			} catch (Exception ex) {
				string msg = "REMOVE_FILLED_FROM_PENDING? DONT_USE_Bar.ContainsPrice()?";
				Assembler.PopupException(msg + msig, ex);
				#if DEBUG
				Debugger.Break();
				#endif
			}
			bool removed = this.ExecutionDataSnapshot.AlertsPendingRemove(alertFilled);
			if (removed == false) {
				#if DEBUG
				Debugger.Break();
				#endif
			}
			if (alertFilled.IsEntryAlert) {
				// position has its parent alert in Position.ctor()
				//// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR
				//alert.PositionAffected.EntryCopyFromAlert(alert);
				this.ExecutionDataSnapshot.PositionsMasterOpenNewAdd(alertFilled.PositionAffected);
				positionsOpenedAfterAlertFilled.Add(alertFilled.PositionAffected);
			} else {
				//// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Alert was filled!!!
				//alertFilled.PositionAffected.ExitAlertAttach(alertFilled);
				this.ExecutionDataSnapshot.MovePositionOpenToClosed(alertFilled.PositionAffected);
				positionsClosedAfterAlertFilled.Add(alertFilled.PositionAffected);
			}

			bool willPlace = this.Backtester.IsBacktestingNow == false && this.OrderProcessor != null && this.IsAutoSubmitting;
			bool setStatusSubmitting = this.IsStreaming && this.IsAutoSubmitting;

			PositionPrototype proto = alertFilled.PositionAffected.Prototype;
			if (proto != null) {
				// 0. once again, set ExitAlert to What was actually filled, because prototypeEntry created SL & TP, which were both written into ExitAlert;
				// so if we caught the Loss and SL was executed, position.ExitAlert will still contain TP if we don't set it here
				bool exitIsDifferent = alertFilled.PositionAffected.ExitAlert != null && alertFilled.PositionAffected.ExitAlert != alertFilled;
				if (alertFilled.IsExitAlert && exitIsDifferent) {
					alertFilled.PositionAffected.ExitAlertAttach(alertFilled);
				}
				// 1. alert.PositionAffected.Prototype.StopLossAlertForAnnihilation and TP will get assigned
				alertsNewAfterAlertFilled = this.PositionPrototypeActivator.AlertFilledCreateSlTpOrAnnihilateCounterparty(alertFilled);
				// quick check: there must be {SL+TP} OR Annihilator
				//this.BacktesterFacade.IsBacktestingNow == false &&
				if (alertFilled.IsEntryAlert) {
					if (proto.StopLossAlertForAnnihilation == null) {
						string msg = "NONSENSE@Entry: proto.StopLossAlert is NULL???..";
						throw new Exception(msg);
					}
					if (proto.TakeProfitAlertForAnnihilation == null) {
						string msg = "NONSENSE@Entry: proto.TakeProfitAlert is NULL???..";
						throw new Exception(msg);
					}
					if (alertsNewAfterAlertFilled.Count == 0) {
						string msg = "NONSENSE@Entry: alertsNewSlAndTp.Count=0"
							+ "; this.PositionPrototypeActivator.AlertFilledCreateSlTpOrAnnihilateCounterparty(alertFilled)"
							+ " should return 2 alerts; I don't want to create new list from {proto.SL, proto.TP}";
						throw new Exception(msg);
					}
				}
				if (alertFilled.IsExitAlert) {
					if (alertsNewAfterAlertFilled.Count > 0) {
						string msg = "NONSENSE@Exit: there must be no alerts (got " + alertsNewAfterAlertFilled.Count + "): killer works silently";
						throw new Exception(msg);
					}
				}

				if (alertsNewAfterAlertFilled.Count > 0 && willPlace) {
					this.OrderProcessor.CreateOrdersSubmitToBrokerProviderInNewThreadGroups(alertsNewAfterAlertFilled, setStatusSubmitting, true);

					// 3. Script using proto might move SL and TP which require ORDERS to be moved, not NULLs
					int twoMinutes = 120000;
					if (alertFilled.IsEntryAlert) {
						// there must be SL.OrderFollowed!=null and TP.OrderFollowed!=null
						if (proto.StopLossAlertForAnnihilation.OrderFollowed == null) {
							string msg = "StopLossAlert.OrderFollowed is NULL!!! engaging ManualResetEvent.WaitOne()";
							this.PopupException(msg);
							Stopwatch waitedForStopLossOrder = new Stopwatch();
							waitedForStopLossOrder.Start();
							proto.StopLossAlertForAnnihilation.MreOrderFollowedIsNotNull.WaitOne(twoMinutes);
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
							//this.ThrowPopup(new Exception(msg));
						}

						if (proto.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
							string msg = "TakeProfitAlert.OrderFollowed is NULL!!! engaging ManualResetEvent.WaitOne()";
							this.PopupException(msg);
							Stopwatch waitedForTakeProfitOrder = new Stopwatch();
							waitedForTakeProfitOrder.Start();
							proto.TakeProfitAlertForAnnihilation.MreOrderFollowedIsNotNull.WaitOne();
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
							//this.ThrowPopup(new Exception(msg));
						}
					}
				}
			}

			if (this.Backtester.IsBacktestingNow == false) {
				ReporterPokeUnit pokeUnit = new ReporterPokeUnit(quote);
				pokeUnit.AlertsNew = alertsNewAfterAlertFilled;
				pokeUnit.PositionsOpened = positionsOpenedAfterAlertFilled;
				pokeUnit.PositionsClosed = positionsClosedAfterAlertFilled;
				this.PushPositionsOpenedClosedToReportersAsyncUnsafe(pokeUnit);
			}

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
			//this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
			string orderState = (alert.OrderFollowed == null) ? "alert.OrderFollowed=NULL" : alert.OrderFollowed.State.ToString();
			if (snap.AlertsPendingContains(alert)) {
				bool removed = snap.AlertsPendingRemove(alert);
				msg = "REMOVED " + orderState + " Pending alert[" + alert + "] ";
			} else {
				msg = "CANT_BE_REMOVED " + orderState + " isn't Pending alert[" + alert + "] ";
			}
			if (alert.OrderFollowed == null) {
				if (this.Backtester.IsBacktestingNow == false) {
					msg = "RealTime alerts should NOT have OrderFollowed=null; " + msg;
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

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerProvider knows to pass Bar here?...
			Bar barFill = (this.IsStreaming) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLast;
			alert.FillPositionAffectedEntryOrExitRespectively(barFill, barFill.ParentBarsIndex, barFill.Close, alert.Qty, 0, 0);
			alert.SignalName += " RemovePendingExitAlertClosePosition Forced";
			// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Position.EntryAlert was filled!!!
			//alert.PositionAffected.ExitAlertAttach(alert);

			bool absenseInPositionsOpenNowIsAnError = true;
			this.ExecutionDataSnapshot.MovePositionOpenToClosed(alert.PositionAffected, absenseInPositionsOpenNowIsAnError);
		}
		public void RemovePendingEntry(Alert alert) {
			string msig = "RemovePendingEntry(): ";

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerProvider knows to pass Bar here?...
			Bar barFill = (this.IsStreaming) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLast;
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
				bool shouldRemove = this.ExecutionDataSnapshot.HasPositionOpenNow(alert.PositionAffected);

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
				Bar barFill = (this.IsStreaming) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLast;
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

		Bars preBacktestBars;
		DataSource preDataSource;
		bool preBacktestIsStreaming;
		Assembler assembler;
		Strategy strategy;
		internal void BacktestContextInitialize(Bars bars) {
			this.preBacktestBars = this.Bars;	// this.preBacktestBars != null will help ignore this.IsStreaming saving IsStreaming state to json
			this.preDataSource = this.DataSource;
			this.preBacktestIsStreaming = this.IsStreaming;

			this.Bars = bars;
			//this.DataSource = bars.DataSource;
			this.IsStreaming = true;
			//this.Strategy.ScriptBase.Initialize(this);
		}
		internal void BacktestContextRestore() {
			this.Bars = this.preBacktestBars;
			//this.DataSource = this.preDataSource;
			this.preBacktestBars = null;	// will help ignore this.IsStreaming saving IsStreaming state to json
			this.IsStreaming = preBacktestIsStreaming;
		}

		public void BacktesterRunSimulation() {
			try {
				this.ExecutionDataSnapshot.Initialize();
				this.Performance.Initialize();
				this.Strategy.Script.InitializeBacktestWithExecutorsBarsInstantiateIndicators();

				this.ChartShadow.SetIndicators(this.ExecutionDataSnapshot.Indicators);
				bool indicatorsHaveNoErrorsCanStartBacktesting = true;
				foreach (Indicator indicator in this.ExecutionDataSnapshot.Indicators.Values) {
					indicatorsHaveNoErrorsCanStartBacktesting &= indicator.BacktestStarting(this);
				}
				if (indicatorsHaveNoErrorsCanStartBacktesting == false) {
					throw new Exception("indicatorsHaveNoErrorsCanStartBacktesting=false");
				}
				
				this.Backtester.Initialize(this.Strategy.Script.BacktestMode);
				this.Backtester.RunSimulation();
			} catch (Exception ex) {
				string msg = "RUN_SIMULATION_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Bars + "]";
				Assembler.PopupException(msg, ex);
#if DEBUG
				Debugger.Break();
#endif
			} finally {
				this.Backtester.SetRunningFalseNotifyWaitingThreadsBacktestCompleted();
			}
		}
		public void BacktesterRunSimulationTrampoline(Action executeAfterSimulationEvenIfIFailed = null, bool inNewThread = true) {
			if (this.Strategy == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy=null; " + this;
				throw new Exception(msg);
			}
			if (this.Strategy.Script == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Executor.Strategy.Script=null, didn't compile; " + this;
				throw new Exception(msg);
			}
			if (this.Bars == null) {
				string msg = "WILL_NOT_EXECUTE_BACKTESTER: Bars=null; select 1) TimeFrame 2) Range 3) PositionSize - for corresponding Chart; " + this;
				throw new Exception(msg);
			}

			if (this.Backtester.IsBacktestingNow) {
				this.Backtester.AbortRunningBacktestWaitAborted("ALREADY_BACKTESTING_this.BacktesterFacade.IsBacktestingNow");
			}

			//???????
			//AFTER F6 I want to run backtest with one slider changed; I click on the slider and get "did you forget to initialize Executor?..." error
			//TOO_LATE_MOVED_TO_AFTER_Strategy.CompileInstantiate() this.Strategy.Script.Initialize(this);
			// only to reset the Glyphs and Positions
			//this.ChartForm.Chart.Renderer.InitializeBarsInvalidateChart(this.Executor);
			//this.Executor.Renderer.InitializeBarsInvalidateChart(this.Executor);
			//this.ChartShadow.ScriptToChartCommunicator.PositionsBacktestClearAfterChartPickedUp();
			this.ChartShadow.PositionsClearBacktestStarting();
			this.ChartShadow.PendingHistoryClearBacktestStarting();

			//inNewThread = false;
			if (inNewThread) {
				int ThreadPoolAvailablePercentageLimit = 20;
				int threadPoolAvailablePercentage = this.getThreadPoolAvailablePercentage();
				if (threadPoolAvailablePercentage < ThreadPoolAvailablePercentageLimit) {
					string msg = "NOT_SCHEDULING_RUN_SIMULATION QueueUserWorkItem(backtesterRunSimulationThreadEntryPoint)"
						+ " because threadPoolAvailablePercentage[" + threadPoolAvailablePercentage
						+ "]<" + ThreadPoolAvailablePercentageLimit + "%";
					throw new Exception(msg);
				}
				//this.MainForm.PopupException("SCHEDULING_RUN_SIMULATION for Strategy[" + this.Executor.Strategy + "] on Bars[" + this.Executor.Bars + "]");

				//v1
				//ThreadPool.QueueUserWorkItem(new WaitCallback(this.backtesterRunSimulationThreadEntryPoint));
				
				//v2
				//http://stackoverflow.com/questions/7582853/what-wpf-threading-approach-should-i-go-with/7584422#7584422
				//Task.Factory.StartNew(() => {
				//    // Background work
				//    this.backtesterRunSimulationThreadEntryPoint();
				//}).ContinueWith((t) => {
				//    // Update UI thread
				//    executeAfterSimulation();
				//}, TaskScheduler.FromCurrentSynchronizationContext());

				//v3
				Task started = new Task(this.BacktesterRunSimulation);
				if (executeAfterSimulationEvenIfIFailed != null) {
					started.ContinueWith((t) => { executeAfterSimulationEvenIfIFailed(); });
				}
				started.Start(TaskScheduler.FromCurrentSynchronizationContext());
			} else {
				//this.Executor.BacktesterRunSimulation();
				//this.ChartForm.Chart.DoInvalidate();
				this.BacktesterRunSimulation();
				if (executeAfterSimulationEvenIfIFailed != null) {
					executeAfterSimulationEvenIfIFailed();
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
			this.registerAnyAlertForReporterClickedToChartSpotted(pokeUnit.PositionsOpenedClosedMergedTogether);
			this.ChartShadow.PositionsRealtimeAdd(pokeUnit.Clone());
		}
		void registerAnyAlertForReporterClickedToChartSpotted(List<Position> positionsMaster) {
			foreach (Position pos in positionsMaster) {
				Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, pos.EntryAlert);
				if (pos.ExitAlert != null) {
					Assembler.InstanceInitialized.AlertsForChart.Add(this.ChartShadow, pos.ExitAlert);
				}
			}
		}

		public void SetBars(Bars barsClicked) {
			if (barsClicked == null) {
				string msg = "don't feed Bars=null into the foodchain!";
				throw new Exception(msg);
			}
			if (this.Backtester.IsBacktestingNow) {
				this.Backtester.AbortRunningBacktestWaitAborted("CLICKED_ON_OTHER_BARS_WHILE_BACKTESTING");
			}

			this.Bars = barsClicked;
		}
		public void AlertKillPending(Alert alert) {
			if (this.Backtester.IsBacktestingNow) {
				this.MarketSimStreaming.SimulateAlertKillPending(alert);
			} else {
				this.MarketRealStreaming.AlertKillPending(alert);
			}
		}
		public double PositionSizeCalculate(Bar bar, double priceScriptAligned) {
			double ret = 1;
			SymbolInfo symbolInfo = bar.ParentBars.SymbolInfo;
			if (symbolInfo.SecurityType == SecurityType.Future && symbolInfo.LeverageForFutures <= 0.0) {
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
	}
}