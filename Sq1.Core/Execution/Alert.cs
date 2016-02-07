using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Execution {
	public	class Alert : IDisposable {
		[JsonIgnore]	public const string FORCEFULLY_CLOSED_BACKTEST_LAST_POSITION = "IGNORED_FOR_KPIs__FORCEFULLY_CLOSED_BY_BACKTESTER";

		[JsonIgnore]	public	Bars				Bars;
		[JsonProperty]	public	Bar					PlacedBar						{ get; protected set; }
		[JsonProperty]	public	int					PlacedBarIndex					{ get; protected set; }
		[JsonProperty]	public	Bar					FilledBar						{ get; protected set; }
		[JsonProperty]	public	Bar					FilledBarSnapshotFrozenAtFill	{ get; protected set; }
		[JsonProperty]	public	int					FilledBarIndex					{ get; protected set; }
		[JsonProperty]	public	DateTime			PlacedDateTimeBarAligned		{ get {
				if (this.PlacedBar == null) return DateTime.MinValue;
//				if (this.PlacedBarIndex == -1 || this.PlacedBarIndex > this.Bars.Count) return PlacedDateTime.MinValue;
//				if (this.PlacedBarIndex == this.Bars.Count) {
//					return this.Bars.StreamingBarCloneReadonly.DateTimeOpen;
//				}
//				return this.Bars[this.PlacedBarIndex].DateTimeOpen;
				return this.PlacedBar.DateTimeOpen;
			} }
		[JsonProperty]	public	DateTime			QuoteCreatedThisAlertServerTime;	// EXECUTOR_ENRICHES_ALERT_WITH_QUOTE { get; protected set; }
		[JsonProperty]	public	string				Symbol							{ get; protected set; }
		[JsonProperty]	public	string				SymbolClass						{ get; protected set; }
		[JsonProperty]	public	string				AccountNumber					{ get; protected set; }
		[JsonProperty]	public	string				dataSourceName					{ get; protected set; }		// containsBidAsk BrokerAdapter for further {new Order(Alert)} execution
		[JsonIgnore]	public	DataSource			DataSource						{ get {
				if (this.Bars == null) {
					throw new Exception("alert.Bars=null for alert[" + this + "]");
				}
				if (this.Bars.DataSource == null) {
					throw new Exception("alert.Bars.DataSource=null for alert[" + this + "]");
				}
				dataSourceName = this.Bars.DataSource.Name;
				return this.Bars.DataSource;
			} }
		[JsonProperty]	public	double				PriceScript						{ get; protected set; }				//doesn't contain Slippage
		[JsonProperty]	public	double				PriceScriptAligned				{ get; protected set; }

		[JsonIgnore]		 	double				priceStopLimitActivation;
		[JsonProperty]	public	double				PriceStopLimitActivation {
			get { return this.priceStopLimitActivation; }
			set {
				this.priceStopLimitActivation = value;
//				if (this.Bars == null) {
//					string msg = "DONT_FORGET_TO_GENERATE_PriceStopLimitActivationAligned_AFTER_JSON_DESERIALIZATION";
//					Assembler.PopupException(msg);
//					return;
//				}
//				if (this.Bars.SymbolInfo == null) {
//					string msg = "ATTACH_SYMBOL_INFO_TO_BARS__TO_CALCULATE_PRICE_SCRIPT_ALIGNED";
//					Assembler.PopupException(msg);
//					return;
//				}
//				this.priceStopLimitActivation = this.Bars.SymbolInfo.AlignAlertToPriceLevelSimplified(this.priceStopLimitActivation, this.Direction, this.MarketLimitStop);
			}
		}
		[JsonProperty]	public	double				PriceStopLimitActivationAligned	{ get; protected set; }
		[JsonProperty]	public	double				Qty								{ get; protected set; }
		[JsonProperty]	public	MarketLimitStop		MarketLimitStop;				//BROKER_ADAPDER_CAN_REPLACE_ORIGINAL_ALERT_TYPE { get; protected set; }
		[JsonProperty]	public	MarketOrderAs		MarketOrderAs					{ get; protected set; }
		[JsonProperty]	public	string 				MarketLimitStopAsString;		//BROKER_ADAPDER_LEAVES_COMMENTS_WHEN_CHANGING__ORIGINAL_ALERT_TYPE { get; protected set; }
		[JsonProperty]	public	Direction			Direction						{ get; protected set; }
		[JsonIgnore]	public	string				DirectionAsString				{ get; protected set; }
		[JsonIgnore]	public	PositionLongShort	PositionLongShortFromDirection	{ get { return MarketConverter.LongShortFromDirection(this.Direction); } }
		[JsonIgnore]	public	bool				IsExitAlert						{ get { return !IsEntryAlert; } }
		[JsonIgnore]	public	bool				IsEntryAlert					{ get { return MarketConverter.IsEntryFromDirection(this.Direction); } }

		[JsonProperty]	public	string				SignalName;						//ORDER_SETS_NAME_FOR_KILLER_ALERTS { get; protected set; }
		[JsonIgnore]	public	bool				ForcefullyClosedBacktestLastPosition { get { return this.SignalName.Contains(Alert.FORCEFULLY_CLOSED_BACKTEST_LAST_POSITION); } }

		[JsonProperty]	public	Guid				StrategyID						{ get; protected set; }
		[JsonProperty]	public	string				StrategyName					{ get; protected set; }
		[JsonIgnore]	public	Strategy			Strategy						{ get; protected set; }

		[JsonIgnore]			Backtester			BacktesterNullUnsafeForDeserialized			{ get {
				if (this.Strategy == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.STRATEGY=NULL,BARS=NULL__ADDED_[JsonIgnore]";
					Assembler.PopupException(msg);
					return null;
				}
				if (this.Strategy.Script == null) {
					string msg = "IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script=null for " + this;
					Assembler.PopupException(msg);
					return null;
				}
				if (this.Strategy.Script.Executor == null) {
					string msg = "IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script.Executor=null for " + this;
					Assembler.PopupException(msg);
					return null;
				}
				if (this.Strategy.Script.Executor.BacktesterOrLivesimulator == null) {
					string msg = "IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script.Executor.Backtester=null for " + this;
					Assembler.PopupException(msg);
					return null;
				}
				return this.Strategy.Script.Executor.BacktesterOrLivesimulator;
			} }
		[JsonIgnore]	public	bool				IsBacktestRunning_FalseIfNoBacktester		{ get {
				Backtester	backtester = this.BacktesterNullUnsafeForDeserialized;
				if (backtester == null) return false;
				return backtester.ImBacktestingOrLivesimming;
			} }
		[JsonIgnore]	public	bool				IsBacktestingNoLivesimNow_FalseIfNoBacktester		{ get {
		        Backtester	backtester = this.BacktesterNullUnsafeForDeserialized;
		        if (backtester == null) return false;
		        return backtester.ImRunningChartlessBacktesting;
		    } }
		[JsonIgnore]	public	bool				IsBacktestingLivesimNow_FalseIfNoBacktester		{ get {
		        Backtester	backtester = this.BacktesterNullUnsafeForDeserialized;
		        if (backtester == null) return false;
		        return this.Strategy.Script.Executor.BacktesterOrLivesimulator.ImRunningLivesim;
		    }
		}
		[JsonProperty]	public	BarScaleInterval	BarsScaleInterval				{ get; protected set; }
		[JsonProperty]	public	OrderSpreadSide		OrderSpreadSide;
		[JsonProperty]	public	Quote				QuoteCreatedThisAlert;
		[JsonProperty]	public	Quote				QuoteFilledThisAlertDuringBacktestNotLive;
		[JsonProperty]	public	Quote				QuoteLastWhenThisAlertFilled;
		[JsonIgnore]	public	Position			PositionAffected;
		[JsonIgnore]	public	DateTime			PositionEntryDateTimeBarAligned				{ get {
				if (this.PositionAffected != null) return this.PositionAffected.EntryDateBarTimeOpen;
				return DateTime.MinValue;
			} }
		[JsonIgnore]	public	Order				OrderFollowed;			// set on Order(alert).executed;
		[JsonIgnore]	public	ManualResetEvent	MreOrderFollowedIsAssignedNow		{ get; private set; }
		[JsonProperty]	public	double				PriceDeposited;		// for a Future, we pay less that it's quoted (GUARANTEE DEPOSIT)
		[JsonIgnore]	public	string				IsAlertCreatedOnPreviousBar		{ get {
				string ret = "";
				DateTime serverTimeNow = this.Bars.MarketInfo.ServerTimeNow;
				DateTime nextBarOpen = this.PlacedBar.DateTimeNextBarOpenUnconditional;
				bool alertIsNotForCurrentBar = (serverTimeNow >= nextBarOpen);
				if (alertIsNotForCurrentBar) {
					ret = "serverTimeNow[" + serverTimeNow + "] >= nextBarOpen[" + nextBarOpen + "]";
				}
				return ret;
			} }
		[JsonIgnore]	public	double QtyFilledThroughPosition { get {
				double ret = 0;
				if (this.PositionAffected == null) return ret;
				if (this.IsEntryAlert && this.PositionAffected.EntryAlert == this) {
					ret = this.PositionAffected.Shares;
				}
				if (this.IsExitAlert && this.PositionAffected.ExitAlert == this) {
					ret = this.PositionAffected.Shares;
				}
				return ret;
			} }
		[JsonIgnore]	public	double PriceFilledThroughPosition { get {
				double ret = 0;
				if (this.PositionAffected == null) return ret;
				if (this.IsEntryAlert && this.PositionAffected.EntryAlert == this) {
					ret = this.PositionAffected.EntryFilledPrice;
				}
				if (this.IsExitAlert && this.PositionAffected.ExitAlert == this) {
					ret = this.PositionAffected.ExitFilledPrice;
				}
				return ret;
			} }

		[JsonProperty]	public	bool IsFilled { get {
				if (this.PositionAffected == null) return false;
				return this.IsEntryAlert
					? this.PositionAffected.IsEntryFilled
					: this.PositionAffected.IsExitFilled;
			} }
		[JsonProperty]	public	bool IsKilled;
		[JsonIgnore]	public	bool IsFilledOutsideBarSnapshotFrozen_DEBUG_CHECK { get {
				bool notFilled = (this.FilledBarSnapshotFrozenAtFill == null);
				if (notFilled) {
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}

				bool noQuoteFilled = (this.QuoteFilledThisAlertDuringBacktestNotLive == null);
				if (noQuoteFilled) {
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}


				bool fillAtSlimBarIsWithinSpread = this.FilledBarSnapshotFrozenAtFill.FillAtSlimBarIsWithinSpread(
					this.PriceFilledThroughPosition, this.QuoteFilledThisAlertDuringBacktestNotLive.Spread);
				if (!fillAtSlimBarIsWithinSpread) {
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}

				if (fillAtSlimBarIsWithinSpread == false) {
					bool insideBar = this.FilledBarSnapshotFrozenAtFill.ContainsPrice(this.PriceFilledThroughPosition);
					bool outsideBar = !insideBar;
					if (outsideBar) {
						#if DEBUG
						Debugger.Break();
						#endif
						return true;
					}

					bool containsBidAsk = this.FilledBarSnapshotFrozenAtFill.ContainsBidAskForQuoteGenerated(this.QuoteFilledThisAlertDuringBacktestNotLive);
					if (!containsBidAsk && fillAtSlimBarIsWithinSpread) {
						#if DEBUG
						Debugger.Break();
						#endif
						return true;
					}
				}
				
				bool priceBetweenFilledQuotesBidAsk = this.QuoteFilledThisAlertDuringBacktestNotLive.PriceBetweenBidAsk(this.PriceFilledThroughPosition);
				if (!priceBetweenFilledQuotesBidAsk) {
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}

				return false;	// false = ok, filledInsideBarShapshotFrozen
			} }
		//[JsonIgnore]	public	bool IsFilledOutsideBar_DEBUG_CHECK { get {
		//		if (this.FilledBar == null) return false;
		//		bool insideBar = (this.PriceFilledThroughPosition >= this.FilledBar.Low && this.PriceFilledThroughPosition <= this.FilledBar.High);
		//		bool outsideBar = !insideBar; 
		//		#if DEBUG
		//		if (outsideBar) {
		//			Debugger.Break();
		//		}
		//		#endif
		//		return outsideBar;
		//	} }
		[JsonIgnore]	public	bool IsFilledOutsideQuote_DEBUG_CHECK { get {
				if (this.QuoteFilledThisAlertDuringBacktestNotLive == null) return false;		// this is LIVE - I'm just notified "your order is filled" at a random moment; no way I could possibly figure out
				bool insideQuote = (this.PriceFilledThroughPosition >= this.QuoteFilledThisAlertDuringBacktestNotLive.Bid && this.PriceFilledThroughPosition <= this.QuoteFilledThisAlertDuringBacktestNotLive.Ask);
				bool outsideQuote = !insideQuote; 
				#if DEBUG
				if (outsideQuote) {
					Debugger.Break();
				}
				#endif
				return outsideQuote;
			} }
		[JsonProperty]	public	BidOrAsk BidOrAskWillFillMe { get {
				return MarketConverter.BidOrAskWillFillAlert(this);
			}}
		
		[JsonProperty]	public	bool		MyBrokerIsLivesim { get {
			if (this.Bars == null) return false;
			if (this.DataSource == null) return false;
			if (this.DataSource.BrokerAdapter is LivesimBroker) return true;
			return false;
		} }
		[JsonProperty]	public	bool		GuiHasTimeRebuildReportersAndExecution { get {
			bool ret = true;
			if (this.MyBrokerIsLivesim == false) return ret;
			try {
				ChartShadow chartShadow = Assembler.InstanceInitialized.AlertsForChart.FindContainerFor_throws(this);
					if (chartShadow == null) {
						string msg = "LOOKS_LIKE_PROTOTYPED_ALERTS_WERENT_ADDED_FOR_CHART_IN_callbackAlertFilledMoveAroundInvokeScriptReenterablyUnprotected";
						return false;
					}
				ScriptExecutor executor = chartShadow.Executor;
				bool livesimSleeping = executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
				ret = livesimSleeping;
			} catch (Exception ex) {
				Assembler.PopupException("DESERIALIZED_ALERT_DOESNT_HAVE_CHART " + this.ToString(), ex);
				return ret;
			}
			return ret;
		} }
		[JsonIgnore]	public	bool				IsDisposed;

		public void Dispose() {
			if (this.IsDisposed || this.MreOrderFollowedIsAssignedNow == null) {
				string msg = "ALERT_WAS_ALREADY_DISPOSED__ACCESSING_NULL_WAIT_HANDLE_WILL_THROW_NPE " + this.ToString();
				//Assembler.PopupException(msg);
				return;
			}
			this.MreOrderFollowedIsAssignedNow.Dispose();	// BASTARDO_ESTA_AQUI !!!! LEAKED_HANDLES_HUNTER
			this.MreOrderFollowedIsAssignedNow = null;
			this.IsDisposed = true;
		}
		
		~Alert() { this.Dispose(); }
		public	Alert() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
			
			PlacedBarIndex				= -1;
			FilledBarIndex				= -1;
			//TimeCreatedServerBar		= DateTime.MinValue;
			QuoteCreatedThisAlertServerTime = DateTime.MinValue;
			Symbol						= "UNKNOWN_JUST_DESERIALIZED";
			//SymbolClass				= "";		//QUIK
			//AccountNumber				= "";
			PriceScript					= 0;
			PriceDeposited				= -1;		// for a Future, we pay less that it's quoted (GUARANTEE DEPOSIT)
			Qty							= 0;
			MarketLimitStop				= MarketLimitStop.Unknown;
			MarketOrderAs				= MarketOrderAs.Unknown;
			Direction					= Direction.Unknown;
			SignalName					= "";
			StrategyID					= Guid.Empty;
			StrategyName				= "NO_STRATEGY";
			BarsScaleInterval			= new BarScaleInterval(BarScale.Unknown, 0);
			OrderFollowed				= null;
			MreOrderFollowedIsAssignedNow	= new ManualResetEvent(false);
		}
		public	Alert(Bar bar, double qty, double priceScript, string signalName,
				Direction direction, MarketLimitStop marketLimitStop, OrderSpreadSide orderSpreadSide,
				Strategy strategy) : this() {

			if (direction == Direction.Unknown) {
				string msg = "ALERT_CTOR_DIRECTION_MUST_NOT_BE_UNKNOWN: when creating an Alert, direction parameter can't be null";
				throw new Exception(msg);
			}
			if (bar == null) {
				string msg = "ALERT_CTOR_BAR_MUST_NOT_BE_NULL: when creating an Alert, bar parameter can't be null";
				throw new Exception(msg);
			}
			if (bar.ParentBars == null) {
				string msg = "ALERT_CTOR_PARENT_BARS_MUST_NOT_BE_NULL: when creating an Alert, bar.ParentBars can't be null";
				throw new Exception(msg);
			}
			if (priceScript < 0) {
				string msg = "ALERT_CTOR_PRICE_SCRIPT_CANT_BE_NEGATIVE";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			this.Bars			= bar.ParentBars;
			this.PlacedBar		= bar;
			this.PlacedBarIndex	= bar.ParentBarsIndex;
			this.Symbol			= bar.Symbol;
			
			this.BarsScaleInterval = this.Bars.ScaleInterval;
			if (this.Bars.SymbolInfo != null) {
				SymbolInfo symbolInfo = this.Bars.SymbolInfo;
				this.SymbolClass = (string.IsNullOrEmpty(symbolInfo.SymbolClass) == false) ? symbolInfo.SymbolClass : "UNKNOWN_CLASS";
				this.MarketOrderAs = symbolInfo.MarketOrderAs;
			}
			
			this.AccountNumber = "UNKNOWN_ACCOUNT";
			if (this.DataSource.BrokerAdapter != null && this.DataSource.BrokerAdapter.AccountAutoPropagate != null
				&& string.IsNullOrEmpty(this.Bars.DataSource.BrokerAdapter.AccountAutoPropagate.AccountNumber) != false) {
				this.AccountNumber = this.Bars.DataSource.BrokerAdapter.AccountAutoPropagate.AccountNumber;
			}
			

			this.Qty = qty;
			this.PriceScript = priceScript;
			this.SignalName = signalName;
			this.Direction = direction;
			this.DirectionAsString = this.Direction.ToString();
			this.MarketLimitStop = marketLimitStop;
			this.MarketLimitStopAsString = this.MarketLimitStop.ToString();
			this.OrderSpreadSide = orderSpreadSide;

			if (strategy == null) {
				string msg = "SERIALIZER_LOGROTATE<ORDER>_GOT_A_SUBMITTED_ALERT_WITH_STRATEGY_NULL__HOW_COME?";
				Assembler.PopupException(msg);
			}
			this.Strategy = strategy;
			if (this.Strategy != null) {
				this.StrategyID = this.Strategy.Guid;
				this.StrategyName = this.Strategy.Name;
			}
			
			//if (this.Strategy.Script != null) {
			//	string msg = "Looks like a manual Order submitted from the Chart";
			//	Assembler.PopupException(msg, null, false);
			//}

			if (this.Bars.SymbolInfo == null) {
				string msg = "ATTACH_SYMBOL_INFO_TO_BARS__TO_CALCULATE_PRICE_SCRIPT_ALIGNED";
				Assembler.PopupException(msg);
			} else {
				this.PriceScriptAligned = this.Bars.SymbolInfo.AlignAlertToPriceLevelSimplified(this.PriceScript, this.Direction, this.MarketLimitStop);
			}

			if (this.PriceScriptAligned < 0) {
				string msg = "ALERT_CTOR_PRICE_SCRIPT_CANT_BE_NEGATIVE";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
		}

		public	override string ToString() {
			//v1 PROFILER_SAID_TOO_SLOW
//			string msg = "bar#" + this.PlacedBarIndex + ": "
//				//+ (this.isEntryAlert ? "entry" : "exit ")
//				+ Direction
//				// not Symbol coz stack overflow
//				+ " " + MarketLimitStop + " " + Qty + "*" + this.Symbol
//				+ "@" + PriceScript
//				//+ "/" + this.PriceFilledThroughPosition + "filled"
//				+ " on[" + AccountNumber + "]"
//				//+ " by[" + SignalName + "]"
//				;
//			msg += (null == this.FilledBar) ? ":UNFILLED" : ":FILLED@" + this.PriceFilledThroughPosition + "*" + this.QtyFilledThroughPosition;
//			if (this.PositionAffected != null) {
//				msg += "; PositionAffected=[" + this.PositionAffected + "]";
//			}
			StringBuilder msg = new StringBuilder();
			msg.Append("bar#");
			//return msg.ToString();
			msg.Append(this.PlacedBarIndex);
			msg.Append(": ");
			msg.Append(this.DirectionAsString);
			msg.Append(" ");
			msg.Append(this.MarketLimitStopAsString);
			msg.Append(" ");
			msg.Append(Qty);
			msg.Append("*");
			msg.Append(this.Symbol);
			msg.Append("@");
			msg.Append(this.PriceScript);
			//msg.Append(" on[");
			//msg.Append(this.AccountNumber + "]");
			if (null == this.FilledBar) {
				msg.Append(":UNFILLED");
			} else {
				msg.Append(":FILLED@");
				msg.Append(this.PriceFilledThroughPosition);
				msg.Append("*");
				msg.Append(this.QtyFilledThroughPosition);
			}
			if (this.PositionAffected != null) {
				msg.Append("; PositionAffected=[#");
				//msg.Append(this.PositionAffected.ToString());
				msg.Append(this.PositionAffected.SernoAbs);
				msg.Append("]");
			}
			return msg.ToString();
		}
		public	string ToStringForTooltip() {
			string longOrderType = (MarketLimitStop == MarketLimitStop.StopLimit) ? "" : "\t";

			string msg = DirectionAsString
				+ "\t" + MarketLimitStopAsString
				+ "\t" + longOrderType + Qty + "/" + this.QtyFilledThroughPosition + "filled*" + Symbol
				+ "@" + PriceScript + "/" + this.PriceFilledThroughPosition + "filled"
				;
			if (this.PositionAffected != null && this.PositionAffected.Prototype != null) {
				msg += "\tProto" + this.PositionAffected.Prototype;
			}
			msg += "\t[" + SignalName + "]";
			return msg;
		}
		public	string ToStringForOrder() {
			string msg = Direction
				+ " " + MarketLimitStop
				// not Symbol coz stack overflow
				+ " " + Symbol
				// not SymbolClass coz stack overflow
				+ "/" + SymbolClass;
			//if (this.MyBrokerIsLivesim) msg += " Livesim";
			return msg;
		}
		public	bool IsIdenticalOrderlessPriceless(Alert alert) {
			if (alert == null) {
				throw new Exception("you must've cleaned Executor.MasterAlerts from another thread while enumerating?...");
			}
			bool basic = this.AccountNumber == alert.AccountNumber
				&& this.Direction == alert.Direction
				&& this.MarketLimitStop == alert.MarketLimitStop
				&& this.Symbol == alert.Symbol
				&& this.Qty == alert.Qty
				&& this.PriceScript == alert.PriceScript		// added for SimulateStopLossMoved()
				&& this.SignalName == alert.SignalName
				&& this.PositionEntryDateTimeBarAligned == alert.PositionEntryDateTimeBarAligned
				&& this.PlacedBarIndex == alert.PlacedBarIndex
				;
			if (alert.PlacedBarIndex == alert.Bars.Count) {
				string msg = "AM_I_HERE_DURING_LIVESIM_FOR_DELAYED_FILLS??? IF_NOT_THEN_BARS.COUNT_IS_TOO_EXPENSIVE";
				return basic;
			}
			bool streamingBarMayBeDifferent = this.PriceScript == alert.PriceScript;
			return basic && streamingBarMayBeDifferent;
		}
		public	bool IsIdenticalForOrdersPending(Alert alert) {
			if (alert == null) {
				throw new Exception("you must've cleaned Executor.DataSnapshot from another thread while enumerating?...");
			}
			if (alert == this) {
				throw new Exception("please compare me against another Alert, not myself :)");
			}
			bool basic = this.AccountNumber == alert.AccountNumber
				&& this.Direction == alert.Direction
				&& this.MarketLimitStop == alert.MarketLimitStop
				&& this.Symbol == alert.Symbol
				&& this.Qty == alert.Qty
				;
			bool streamingBarMayBeDifferent = this.PriceScript == alert.PriceScript
				&& this.PlacedBarIndex == alert.PlacedBarIndex
				;
			return basic && streamingBarMayBeDifferent;
		}
		public	virtual void AbsorbFromExecutorAfterCreatedByMarketReal(ScriptExecutor executor) {
		}
		public	void FillPositionAffectedEntryOrExitRespectively(Bar barFill, int barFillRelno,
				double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			//if (this.BarRelnoFilled != -1) {
			if (this.FilledBar != null) {
				string msg = "ALERT_ALREADY_FILLED_EARLIER_CANT_OVERRIDE @FilledBarIndex[" + this.FilledBarIndex + "]"
						+ ", duplicateFill @[" + barFill + "]";
				throw new Exception(msg);
			}
			//WHY_DID_YOU_REWIND_IT??? DELAY_IN_BROKER_EXACTLY_MENT_TO_MODEL_VERY_LATE_FILLS
			//if (this.MyBrokerIsLivesim) {
			//	if (barFill.DateTimeOpen != this.PlacedBar.DateTimeOpen) {
			//		string msg = "FOR_DELAYED_LIVESIM_FILLS_PUT_BAR_FILL_BACK_TO_PLACED";
			//		//Assembler.PopupException(msg, null, false);
			//		barFill = this.PlacedBar;
			//	}
			//}
			this.FilledBarSnapshotFrozenAtFill = barFill.Clone();		//BarsStreaming#130 becomes BarStatic#130
			this.FilledBar = barFill;
			this.FilledBarIndex = barFillRelno;
			if (this.PositionAffected == null) {
				//if (this.IsExecutorBacktestingNow) return;
				throw new Exception("Backtesting or Realtime, an alert always has a PositionAffected, oder?...");
			}
			if (this.IsEntryAlert) {
				this.PositionAffected.FillEntryWith(barFill, priceFill, qtyFill, slippageFill, commissionFill);
				if (this.PositionAffected.EntryFilledBarIndex != barFillRelno) {
					string msg = "ENTRY_ALERT_SIMPLE_CHECK_FAILED_AVOIDING_EXCEPTION_IN_PositionsMasterOpenNewAdd"
						+ "EntryFilledBarIndex[" + this.PositionAffected.EntryFilledBarIndex + "] != barFillRelno[" + barFillRelno + "]";
					Assembler.PopupException(msg, null, false);	//makes #D loose callstack & throw
				}
			} else {
				this.PositionAffected.FillExitWith(barFill, priceFill, qtyFill, slippageFill, commissionFill);
			}
		}
	}
}
