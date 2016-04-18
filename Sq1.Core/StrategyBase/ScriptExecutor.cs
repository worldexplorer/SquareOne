using System;
using System.Collections.Generic;
using System.Diagnostics;		//StackFrame

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Backtesting;
using Sq1.Core.Sequencing;
using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;
using Sq1.Core.Support;
using Sq1.Core.Correlation;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor : IDisposable {
		#region constructed (my own data)
		public	string							ReasonToExist				{ get; protected set; }
		public	ExecutorDataSnapshot			ExecutionDataSnapshot		{ get; protected set; }
		public	SystemPerformance				PerformanceAfterBacktest	{ get; protected set; }
		public	Backtester						BacktesterOrLivesimulator;					//{ get; private set; }
		public	PositionPrototypeActivator		PositionPrototypeActivator	{ get; protected set; }
		public	AlertFactory					AlertFactory				{ get; protected set; }
		public	ScriptExecutorEventGenerator	EventGenerator				{ get; protected set; }
		public	CommissionCalculator			CommissionCalculator;
		public	Sequencer						Sequencer					{ get; protected set; }
		public	Livesimulator					Livesimulator				{ get; protected set; }
		public	string							LastBacktestStatus;
		public	ConcurrentWatchdog				ScriptIsRunning_cantAlterInternalLists	{ get; protected set; }
		public	Correlator						Correlator					{ get; private set; }
		#endregion
		
		#region initialized (sort of Dependency Injection)
		public	ChartShadow						ChartShadow;		// initialized with Sq1.Charting.ChartControl:ChartShadow
		public	Strategy						Strategy;
		public	string							StrategyName				{ get { return (this.Strategy == null) ? "STRATEGY_NULL" : this.Strategy.Name; } }
		public	OrderProcessor					OrderProcessor				{ get; protected set; }
		#endregion

		#region volatile Script is recompiled and replaced
		public	Bars							Bars						{ get; private set; }
		public	PositionSize PositionSize { get {
				if (this.Strategy == null) {
					string msg = "ScriptExecutor.PositionSize: you should not access PositionSize before you've set Strategy";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return this.Strategy.ScriptContextCurrent.PositionSize;
			} }
		public	DataSource DataSource_fromBars { get {
				if (this.Bars == null) {
					string msg = "ScriptExecutor.DataSource: you should not access DataSource BEFORE you've set ScriptExecutor.Bars"
						+ "; " + Assembler.InstanceInitialized.RepositoryJsonDataSources.AbsPath + "\\xxx.json/xxxFolder deleted between AppRestart?..";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return this.Bars.DataSource;
			} }
		#endregion

		public	bool	IsStreamingTriggeringScript {
			get {
				if (this.Strategy == null) {
					//v1 return this.isStreamingWhenNoStrategyLoaded
					string msg = "IsStreamingTriggeringScript__get: CHART_WITHOUT_STRATEGY_CAN_STREAM/NOT_BUT_NEVER_TRIGGERS_THE_SCRIPT(NO_SCRIPT_WITHOUT_STRATEGY)";
					Assembler.PopupException(msg);
					return false;
				}
				return this.Strategy.ScriptContextCurrent.StreamingIsTriggeringScript;
			}
			set {
				if (this.Strategy == null) {
					//v1 this.isStreamingWhenNoStrategyLoaded = value;
					string msg = "IsStreamingTriggeringScript__set[" + value + "]: CHANGE_OF_CONCEPT__CHART_WITHOUT_STRATEGY_IS_ALWAYS_STREAMING";
					Assembler.PopupException(msg, null, false);
					return;
				}
				
				this.Strategy.ScriptContextCurrent.StreamingIsTriggeringScript = value;
				// we are in beginning the backtest and will switch back to preBacktestIsStreaming after backtest finishes;
				// if you AppKill during the backtest, you don't want btnStreaming be pressed (and disabled DataSource.StreamingAdapter=null) after AppRestart
				if (this.preBacktestBars != null) {
					//string msg = "NOT_SAVING_IsStreamingTriggeringScript=ON_FOR_BACKTEST"
					//	+ " preBacktestIsStreaming[" + this.preBacktestIsStreaming + "] preBacktestBars[" + this.preBacktestBars + "]";
					//Assembler.PopupException(msg, null, false);
					return;
				}
				if (this is ReusableExecutor == false) {
					this.Strategy.Serialize();
				}
				
				if (this.Strategy.Script == null) return;	// koz the rest is only about notifying script (which may not be compiled if user typed an error in the ScriptEditor and Script didn't compile)
				if (value == true) {
					try {
						this.ExecutionDataSnapshot.IsScriptRunningOnStrategyEmittingOrdersTurnedOnNonBlockingRead = true;
						this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnStreamingTriggeringScriptTurnedOnCallback(WAIT)");
						this.Strategy.Script.OnStreamingTriggeringScript_turnedOn_callback();
					} finally {
						this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnStreamingTriggeringScriptTurnedOnCallback(WAIT)");
						this.ExecutionDataSnapshot.IsScriptRunningOnStrategyEmittingOrdersTurnedOnNonBlockingRead = false;
					}
				} else {
					try {
						this.ExecutionDataSnapshot.IsScriptRunningOnStrategyEmittingOrdersTurnedOffNonBlockingRead = true;
						this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnStreamingTriggeringScriptTurnedOffCallback(WAIT)");
						this.Strategy.Script.OnStreamingTriggeringScript_turnedOff_callback();
					} finally {
						this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnStreamingTriggeringScriptTurnedOffCallback(WAIT)");
						this.ExecutionDataSnapshot.IsScriptRunningOnStrategyEmittingOrdersTurnedOffNonBlockingRead = false;
					}
				}
			}
		}
		public	bool	IsStrategyEmittingOrders {
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
				this.Strategy.Serialize();
				
				if (this.Strategy.Script == null) return;	// koz the rest is only about notifying script (which may not be compiled if user typed an error in the ScriptEditor and Script didn't compile)
				if (value == true) {
					try {
						this.ExecutionDataSnapshot.IsScriptRunningOnStreamingTriggeringScriptTurnedOnNonBlockingRead = true;
						this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnStrategyEmittingOrdersTurnedOnCallback(WAIT)");
						this.Strategy.Script.OnStrategyEmittingOrders_turnedOn_callback();
					} finally {
						this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnStrategyEmittingOrdersTurnedOnCallback(WAIT)");
						this.ExecutionDataSnapshot.IsScriptRunningOnStreamingTriggeringScriptTurnedOnNonBlockingRead = false;
					}
				} else {
					try {
						this.ExecutionDataSnapshot.IsScriptRunningOnStreamingTriggeringScriptTurnedOffNonBlockingRead = true;
						this.ScriptIsRunning_cantAlterInternalLists.WaitAndLockFor(this, "OnStrategyEmittingOrdersTurnedOffCallback(WAIT)");
						this.Strategy.Script.OnStrategyEmittingOrders_turnedOff_callback();
					} finally {
						this.ScriptIsRunning_cantAlterInternalLists.UnLockFor(this, "OnStrategyEmittingOrdersTurnedOffCallback(WAIT)");
						this.ExecutionDataSnapshot.IsScriptRunningOnStreamingTriggeringScriptTurnedOffNonBlockingRead = false;
					}
				}
			}
		}
		public	string	SpreadPips	{ get {
				string ret = "CANT_REACH_BARS_NOR_CURRENT_CONTEXT";
				double pct = -1;
				if (this.Strategy != null) {
					pct = this.Strategy.ScriptContextCurrent.SpreadModelerPercent;
					ret = pct + "%";
				}
				if (this.Bars == null) return ret;
				Bar lastBar = this.Bars.BarStaticLast_nullUnsafe;
				if (lastBar == null) return ret;
				double lastClose = lastBar.Close;
				double spreadPips = lastClose * pct / 100d;
				spreadPips = this.Bars.SymbolInfo.AlignToPriceLevel(spreadPips);
				ret = "~= " + spreadPips + "pips, for lastClose[" + lastBar.Close_formatted + "]";
				return ret;
			} }

		public ScriptExecutor(string reasonToExist) {
			ReasonToExist				= reasonToExist;
			ExecutionDataSnapshot		= new ExecutorDataSnapshot(this);
			BacktesterOrLivesimulator	= new Backtester(this);
			PositionPrototypeActivator	= new PositionPrototypeActivator(this);
			AlertFactory				= new AlertFactory(this);
			EventGenerator				= new ScriptExecutorEventGenerator(this);
			CommissionCalculator		= new CommissionCalculatorZero(this);
			Sequencer					= new Sequencer(this);
			Livesimulator				= new Livesimulator(this);
			OrderProcessor				= Assembler.InstanceInitialized.OrderProcessor;
			PerformanceAfterBacktest	= new SystemPerformance(this);
			ScriptIsRunning_cantAlterInternalLists = new ConcurrentWatchdog("WAITING_FOR_SCRIPT_OVERRIDDEN_METHOD_TO_RETURN");
			Correlator					= new Correlator(this);
		}

		public void Initialize(Strategy strategy, ChartShadow chartContol = null, bool saveStrategy_falseForSequencer = false) {
			string msg = " at this time, FOR SURE this.Bars==null, strategy.Script?=null";
			if (chartContol == null) {
				string msg2 = "EXECUTOR_MUST_HAVE_CHART_SHADOW__CREATING_STUB_TO_ROUTE_DRAWING_COMMANDS_FROM_STRATEGY";
				Assembler.PopupException(msg, null, false);
				chartContol = new ChartShadow();
			}
			this.ChartShadow = chartContol;
			this.ChartShadow.SetExecutor(this);
			this.Strategy = strategy;
			
			this.Sequencer.InitializedProperly_executorHasScript_readyToOptimize = false;

			this.ExecutionDataSnapshot.Initialize();
			// SO_WHAT??? Executor.Bars are NULL in ScriptExecutor.ctor() and NOT NULL in SetBars
			this.PerformanceAfterBacktest.Initialize();

			if (this.Strategy == null) return;
			if (this.Bars != null) {
				this.Strategy.ScriptContextCurrent.Symbol = this.Bars.Symbol;
				this.Strategy.ScriptContextCurrent.DataSourceName = this.DataSource_fromBars.Name;
			}
			if (this.Strategy.Script == null) {
				msg = "I will be compiling this.Strategy.Script when in ChartFormsManager.StrategyCompileActivatePopulateSliders()";
				//} else if (this.Bars == null) {
				//	msg = "InitializeStrategyAfterDeserialization will Script.Initialize(this) later with bars";
			} else {
				this.Strategy.Script.Initialize(this, saveStrategy_falseForSequencer);

				#region PARANOID
				#if DEBUG
				var reflected = this.Strategy.Script.ScriptParametersById_ReflectedCached;
				var ctx = this.Strategy.ScriptContextCurrent.ScriptParametersById;
				if (reflected.Count != ctx.Count) {
					string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
					Assembler.PopupException(msg2);
				}
				foreach (int id in reflected.Keys) {
					if (ctx.ContainsKey(id) == false) {
						string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
						Assembler.PopupException(msg2);
						continue;
					}
					if (ctx[id] != reflected[id]) {
						string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
						Assembler.PopupException(msg2);
						continue;
					}
				}

				//v1: CRAZY_TYPES_CONVERSION,CONSIDER_CONTEXT_SAVE_IN_SAME_FORMAT_AS_REFLECTED_OR_INTRODUCE_ARTIFICIAL_SIMILAR_NEXT_TO_REFLECTED
				//var iReflected = this.Strategy.Script.IndicatorsByName_ReflectedCached;
				//var iCtx = this.Strategy.ScriptContextCurrent.IndicatorParametersByName;
				//if (reflected.Count != ctx.Count) {
				//	string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
				//	Assembler.PopupException(msg2);
				//}
				//foreach (int id in reflected.Keys) {
				//	if (ctx.ContainsKey(id) == false) {
				//		string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
				//		Assembler.PopupException(msg2);
				//		continue;
				//	}
				//	if (ctx[id] != reflected[id]) {
				//		string msg2 = "here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference";
				//		Assembler.PopupException(msg2);
				//		continue;
				//	}
				//}
				//v2 just force it and find duplicate calls in debugger...
				//SEQUENCER_ALREADY_DONE_IT_CloneForSequencer this.Strategy.Script.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
				#endif
				#endregion

				// here Reflected must have ValueCurrents absorbed CurrentContext and all params pushed back to CurrentContext by reference
				this.Sequencer.Initialize();	//otherwize this.Sequencer.InitializedProperly = false; => can't optimize anything
			}
			//v1 dynamically taken now in BacktestMarketsim.cs:476 this.MarketsimBacktest.Initialize(this.Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow);
			//v2
			this.BacktesterOrLivesimulator.BacktestDataSource.BrokerAsBacktest_nullUnsafe.InitializeMarketsim(this);

			//v1, ATTACHED_TO_BARS.DATASOURCE.SYMBOLRENAMED_INSTEAD_OF_DATASOURCE_REPOSITORY
			// if I listen to DataSourceRepository, all ScriptExecutors receive same notification including irrelated to my Bars
			// Assembler.InstanceInitialized.RepositoryJsonDataSource.OnSymbolRenamed +=
			//	new EventHandler<DataSourceSymbolEventArgs>(Assembler_InstanceInitialized_RepositoryJsonDataSource_OnSymbolRenamed);
		}

		public void EnrichAlerts_withQuoteCreated(List<Alert> alertsAfterStrategy, Quote quote) {
			if (quote == null) {
				string msg = "WHEN_AM_I_NULL_BTW?";
				Assembler.PopupException(msg);
				return;
			}
			if (quote.HasParentBarStreaming == false && this.BacktesterOrLivesimulator.ImRunningLivesim == false) {
				string msg = "BACKTEST I_REFUSE_TO_ENRICH_ALERT_WITH_QUOTE__SINCE_QUOTE_HAS_NO_PARENT_BAR__I_CAN_NOT_CHECK_IF_QUOTE_AND_ALERT_ARE_FOR_THE_SAME_BAR";
				Assembler.PopupException(msg, null, false);
				return;
			}
			foreach (Alert alert in alertsAfterStrategy) {
				if (alert.QuoteCreatedThisAlert != null) {		//quote.HasParentBar == false && 
					if (this.BacktesterOrLivesimulator.ImRunningLivesim) {
						string msg = "LIVESIM_GOOD__I_HAVE_ALREADY_ENRICHED_EACH_ALERT";
						//Assembler.PopupException(msg, null, false);
					} else {
						string msg = "BACKTEST_BAD__I_SHOULD_NOT_HAVE_ALREADY_ENRICHED_EACH_ALERT";
						Assembler.PopupException(msg, null, false);
					}
					continue;
				}
				if (quote.ParentBarStreaming == null) {
					// essential for a postProcess to have a quoteFilled inside the alert already, to moveAround (illogical I know)
					string msg = "I_REFUSE_TO_ENRICH_ALERT_WITH_QUOTE_HAVING_NO_PARENT_STREAMING_BAR";
					Assembler.PopupException(msg, null, false);
					continue;
				}

				int alertIsLateNbars = alert.PlacedBarIndex - quote.ParentBarStreaming.ParentBarsIndex;
				if (alertIsLateNbars > 0) {
					string msg = "I_REFUSE_TO_ENRICH_ALERT_WITH_QUOTE alertIsLateNbars[" + alertIsLateNbars + "] alert[" + alert + "]";
					Assembler.PopupException(msg, null, false);
					continue;
				}

				//alert.PositionSize = this.PositionSize;
				alert.QuoteCreatedThisAlertServerTime = quote.ServerTime;
				alert.QuoteCreatedThisAlert = quote;
				alert.QuoteCreatedThisAlert_deserializable = alert.QuoteCreatedThisAlert.Clone_asCoreQuote();
			}
		}

		public void PopupException(string msg, Exception ex = null, bool debuggingBreak = true) {
			Assembler.PopupException(msg, ex, debuggingBreak);
			//this.Backtester.AbortBacktestIfExceptionsLimitReached();
		}

		//NOW_INLINE void invokeScriptEvents(Alert alertFilled) {}
		void removePendingExitAlert(Alert alert, string msig) {
			string msg = "";
			ExecutorDataSnapshot snap = alert.Strategy.Script.Executor.ExecutionDataSnapshot;
			//this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			string orderState = (alert.OrderFollowed == null) ? "alert.OrderFollowed=NULL" : alert.OrderFollowed.State.ToString();
			if (snap.AlertsPending.Contains(alert, this, "RemovePendingExitAlert(WAIT)")) {
				bool removed = snap.AlertsPending.Remove(alert, this, "RemovePendingExitAlert(WAIT)");
				msg = "REMOVED " + orderState + " Pending alert[" + alert + "] ";
			} else {
				msg = "CANT_BE_REMOVED " + orderState + " isn't Pending alert[" + alert + "] ";
			}
			if (alert.OrderFollowed == null) {
				if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == false) {
					msg = "RealTime alerts should NOT have OrderFollowed=null; " + msg;
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				return;
			}
			// OrderFollowed=null when executeStrategyBacktestEntryPoint() is in the call stack
			this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msig + msg);
		}
		public void RemovePendingExitAlerts_closePositionsBacktestLeftHanging(Alert alert) {
			string msig = "RemovePendingExitAlertAndClosePositionAfterBacktestLeftItHanging(): ";
			this.removePendingExitAlert(alert, msig);
			//bool checkPositionOpenNow = true;
			//if (this.checkPositionCanBeClosed(alert, msig, checkPositionOpenNow) == false) return;

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerAdapter knows to pass Bar here?...
			//v1 STREAMING DOESNT BELONG??? Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreamingCloneReadonly : alert.Bars.BarStaticLast_nullUnsafe;
			//v2 NO_I_NEED_STREAMING_BAR_NOT_THE_SAME_I_OPENED_THE_LEFTOVER_POSITION Bar barFill = alert.Bars.BarStaticLast_nullUnsafe;
			// HACK adding streaming LIVE to where BACKTEST_BARS_CLONED_FOR_ just ended; to avoid NPE at "if (exitBar.Open != this.ExitBar.Open) {"
			//v3 alert.Bars.BarCreateAppendBindStatic(barFill.DateTimeOpen, barFill.Open, barFill.High, barFill.Low, barFill.Close, barFill.Volume);
			//v4 alert.Bars.BarStreamingCreateNewOrAbsorb(this.Bars.BarStreaming);
			//v5 IM_USING_ALERTS_EXIT_BAR_NOW__NOT_STREAMING
			Bar barFill = (alert.PlacedBar != null) ? alert.PlacedBar : alert.Bars.BarStaticLast_nullUnsafe;
			alert.FillPositionAffected_entryOrExit_respectively(barFill, barFill.ParentBarsIndex, barFill.Close, alert.Qty, 0, 0);
			alert.SignalName += " RemovePendingExitAlertClosePosition " + Alert.FORCEFULLY_CLOSED_BACKTEST_LAST_POSITION;
			// REFACTORED_POSITION_HAS_AN_ALERT_AFTER_ALERTS_CONSTRUCTOR we can exit by TP or SL - position doesn't have an ExitAlert assigned until Position.EntryAlert was filled!!!
			//alert.PositionAffected.ExitAlertAttach(alert);

			bool absenseInPositionsOpenNowIsAnError = true;
			this.ExecutionDataSnapshot.MovePositionOpen_toClosed(alert.PositionAffected, absenseInPositionsOpenNowIsAnError);
		}
		void removePendingEntry(Alert alert) {
			string msig = "RemovePendingEntry(): ";

			//"Excuse me, what bar is it now?" I'm just guessing! does BrokerAdapter knows to pass Bar here?...
			Bar barFill = (this.IsStreamingTriggeringScript) ? alert.Bars.BarStreaming_nullUnsafeCloneReadonly : alert.Bars.BarStaticLast_nullUnsafe;
			alert.FillPositionAffected_entryOrExit_respectively(barFill, barFill.ParentBarsIndex, barFill.Close, alert.Qty, 0, 0);
			alert.SignalName += " RemovePendingEntryAlertClosePosition " + Alert.FORCEFULLY_CLOSED_BACKTEST_LAST_POSITION;
		}


		public void SetBars(Bars barsClicked, bool subscribeDownstream) {
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
			if (this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				this.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted("CLICKED_ON_OTHER_BARS_WHILE_BACKTESTING");
			}

			//v2, ATTACHED_TO_BARS.DATASOURCE.SYMBOLRENAMED_INSTEAD_OF_DATASOURCE_REPOSITORY
			// DATASOURCE_WILL_NOT_RENAME_YOUR_INSTANTIATED_BARS_BUT_WILL_RENAME_BARFILE_AND_SYMBOL_IN_BARFILE_HEADER
			if (this.Bars != null && this.Bars.DataSource != null) {
				// unfollowing old Bars (so that it'll be only renaming .BAR file); most likely those Bars will be GarbageCollected
				this.Bars.DataSource.OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull -=
					new EventHandler<DataSourceSymbolRenamedEventArgs>(barDataSource_OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull);
				if (this.ChartShadow != null && this.ChartShadow.ChartStreamingConsumer != null && subscribeDownstream == true) {
					this.ChartShadow.ChartStreamingConsumer.StreamingUnsubscribe("UNSUBSCRIBING_FROM_OLD_BARS oldBars[" + this.Bars + "]=>newBars[" + barsClicked + "]");
				} else {
					string msg = "SWITCHED_BARS_FOR_OPEN_CHART_WHILE_NON_SUBSCRIBED LOOKS_LIKE_WE_SPAWN_REUSABLE_EXECUTORS_FOR_PARALLEL_CHARTLESS_BACKTESTING";
					Assembler.PopupException(msg, null, false);
				}
			}
			Bars oldBars = this.Bars;
			this.Bars = barsClicked;
			if (this.Bars.Symbol.Contains(Bars.RANDOM_GENERATED_BARS)) {
				string msg = "IF_I_CONTINUE_QUIK_STREAMING_WILL_REGISTER_A_DDE_SERVER_FOR_THIS_SYMBOL";
				return;
			}

			if (this.ChartShadow != null && this.ChartShadow.ChartStreamingConsumer != null) {
				if (subscribeDownstream) {
					this.ChartShadow.ChartStreamingConsumer.StreamingSubscribe("SUBSCRIBING_TO_NEW_BARS oldBars[" + oldBars + "]=>this.Bars[" + this.Bars + "]");
				}
			} else {
				string msg1 = "LOOKS_LIKE_WE_SPAWN_REUSABLE_EXECUTORS_FOR_PARALLEL_CHARTLESS_BACKTESTING";
				Assembler.PopupException(msg1);
			}

			if (this.Bars.DataSource == null) {
				string msg = "BARS_CLONED_FOR_SEQUENCER_DONT_HAVE_DATASOURCE[" + this.Bars.ReasonToExist + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.Bars.DataSource.OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull +=
				new EventHandler<DataSourceSymbolRenamedEventArgs>(barDataSource_OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull);

			//nope, multiple script executors will compete inside brokeradapter; check your price alignment via SymbolInfo
			//if (this.DataSource_fromBars.BrokerAsLivesim_nullUnsafe != null) {
			//    string msg = "AVOIDING_NPE_IN_CheckEntryAlertWillBeFilledByQuote WHILE_PAPER_TRADING";
			//    this.DataSource_fromBars.BrokerAsLivesim_nullUnsafe.InitializeBacktestBroker(this);
			//}
		}
		void barDataSource_OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull(object sender, DataSourceSymbolRenamedEventArgs e) {
			if (this.Bars == null) {
				string msg = "INITIALIZED_EXECUTOR_ALWAYS_HAVE_BARS MAKE_SURE_YOU_UNSUBSCRIBED_ME_FROM_PREVIOUS_BARS";
				Assembler.PopupException(msg, null, false);
				return;
			}
			if (e.DataSource != this.Bars.DataSource) {
				string msg = "I_SHOULD_NOT_BE_NOTIFIED_ABOUT_OTHER_DATASOURCES";
				Assembler.PopupException(msg, null, false);
				e.CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars = false;
				return;
			}
			if (e.Symbol == this.Bars.Symbol) {
				string msg = "I_SHOULD_NOT_BE_NOTIFIED_IF_SYMBOL_WAS_NOT_RENAMED";
				Assembler.PopupException(msg, null, false);
				e.CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars = false;
				return;
			}
			if (this.Strategy != null) {
				if (this.IsStreamingTriggeringScript) {
					string msg = "EXECUTOR_REFUSED_TO_RENAME_STREAMING_BARS"
						+ " TO_CREATE_BACKUP_TURN_STREAMING_OFF_RENAME_STREAMING_BACK_ON"
						+ " TO_RENAME_PERMANENTLY_IMPLMEMENT_SYMBOL_MAPPING_FOR_STREAMING_SYMBOLS";
					Assembler.PopupException(msg);
					e.CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars = true;
					return;
				}
			}
			this.Bars.RenameSymbol(e.Symbol);

			if (this.ChartShadow == null) {
				string msg = "YOU_FORGOT_TO_INVOKE_ScriptExecutor.Initialize() MANDATORY_FOR_ANY_CHART_EVEN_WITH_STRATEGY_NULL";
				Assembler.PopupException(msg);
			} else {
				this.ChartShadow.SyncBarsIdentDueToSymbolRename();		// can I delete propagation to Strategy.ScriptContextCurrent?
				this.ChartShadow.RaiseOnChartSettingsIndividualChanged_chartManagerShouldSerialize_ChartFormDataSnapshot();
			}
			if (this.Strategy == null) return;
			if (this.Strategy.ScriptContextCurrent.Symbol == this.Bars.Symbol) return;
			this.Strategy.ScriptContextCurrent.Symbol  = this.Bars.Symbol;
			//PAS_BESOIN__ALREADY_RAISED_RaiseChartSettingsChangedContainerShouldSerialize() this.Strategy.Serialize();
		}
		public double PositionSizeCalculate(Bar bar, double priceScriptAligned) {
			double ret = 1;
			SymbolInfo symbolInfo = bar.ParentBars.SymbolInfo;
			if (symbolInfo.SecurityType == SecurityType.Futures && symbolInfo.Point2Dollar <= 0.0) {
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
		public double OrderCommissionCalculate(Direction direction, MarketLimitStop marketLimitStop, double price, double shares, Bars bars) {
			double ret = 0;
			if (this.Strategy.ScriptContextCurrent.ApplyCommission && this.CommissionCalculator != null) {
				ret = this.CommissionCalculator.CalculateCommission(direction, marketLimitStop, price, shares, bars);
			}
			return ret;
		}
		public override string ToString() {
			//string ret = this.StrategyName;
			//if (this.Strategy != null) {
			//	if (this.Strategy.Name == null) return ret;
			//	ret += " @ " + this.Strategy.ScriptContextCurrent.ToString();
			//} else {
			//}
			//return ret;
			//return this.Strategy.WindowTitle;
			return (this.Strategy != null) ? this.Strategy.WindowTitle : "CHART_ONLY [" + this.Bars.ToString() + "]";
		}
		public string ToStringWithCurrentParameters() {
			string ret = "";
			ret += this.ToString();
			// this.Strategy.Script==null for an {editor-based + compilation failed} Script
			if (this.Strategy.Script != null) ret += " " + this.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName_AsString;
			//ret += " why???PerformanceAfterBacktest:" + this.PerformanceAfterBacktest.ScriptAndIndicatorParameterClonesByName_BuiltOnBacktestFinished_AsString;
			return ret;
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.ExecutionDataSnapshot					.Dispose();
			this.BacktesterOrLivesimulator				.Dispose();
			this.Sequencer								.Dispose();
			this.Livesimulator							.Dispose();
			this.ScriptIsRunning_cantAlterInternalLists	.Dispose();

			// ALREADY_DISPOSED_IN_ChartFormsManager.Dispose_workspaceReloading() this.ChartShadow							.Dispose();

			this.ExecutionDataSnapshot					= null;
			this.BacktesterOrLivesimulator				= null;
			this.Sequencer								= null;
			this.Livesimulator							= null;
			this.ScriptIsRunning_cantAlterInternalLists	= null;
			this.ChartShadow							= null;

			this.OrderProcessor		= null;
			this.Bars				= null;		// if this.Bars are subscribed to anything, the event generator will keep Bars' handler and so this.Bars wont get GC'ed
			this.IsDisposed			= true;
		}
		public bool IsDisposed { get; private set; }
	}
}
