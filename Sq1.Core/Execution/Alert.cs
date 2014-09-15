using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Execution {
	[DataContract]
	public class Alert {
		public Bars Bars;
		[DataMember] public Bar PlacedBar { get; protected set; }
		[DataMember] public int PlacedBarIndex { get; protected set; }
		[DataMember] public Bar FilledBar { get; protected set; }
		[DataMember] public int FilledBarIndex { get; protected set; }
		[DataMember] public DateTime PlacedDateTime { get {
				if (this.PlacedBar == null) return DateTime.MinValue;
//				if (this.PlacedBarIndex == -1 || this.PlacedBarIndex > this.Bars.Count) return PlacedDateTime.MinValue;
//				if (this.PlacedBarIndex == this.Bars.Count) {
//					return this.Bars.StreamingBarCloneReadonly.DateTimeOpen;
//				}
//				return this.Bars[this.PlacedBarIndex].DateTimeOpen;
				return this.PlacedBar.DateTimeOpen;
			} }
		[DataMember] public DateTime QuoteCreatedThisAlertServerTime;	// EXECUTOR_ENRICHES_ALERT_WITH_QUOTE { get; protected set; }
		[DataMember] public string Symbol;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC  { get; protected set; }
		[DataMember] public string SymbolClass;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC   { get; protected set; }
		[DataMember] public string AccountNumber;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC   { get; protected set; }

		// contains BrokerProvider for further {new Order(Alert)} execution
		[DataMember] public string dataSourceName { get; protected set; }
		//public DataSource dataSource;
		public DataSource DataSource { get {
				if (this.Bars == null) {
					throw new Exception("alert.Bars=null for alert[" + this + "]");
				}
				if (this.Bars.DataSource == null) {
					throw new Exception("alert.Bars.DataSource=null for alert[" + this + "]");
				}
				dataSourceName = this.Bars.DataSource.Name;
				//dataSource = this.Bars.DataSource;
				return this.Bars.DataSource;
			} }
		
		//doesn't contain Slippage
		[DataMember] public double PriceScript;
		[DataMember] public double PriceStopLimitActivation;
		[DataMember] public double Qty;
		[DataMember] public MarketLimitStop MarketLimitStop;	//BROKER_PROVIDER_CAN_REPLACE_ORIGINAL_ALERT_TYPE { get; protected set; }
		[DataMember] public MarketOrderAs MarketOrderAs;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC   { get; protected set; }

		[DataMember] public Direction Direction;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC { get; protected set; }
		public PositionLongShort PositionLongShortFromDirection { get { return MarketConverter.LongShortFromDirection(this.Direction); } }
		public bool IsExitAlert { get { return !IsEntryAlert; } }
		public bool IsEntryAlert { get { return MarketConverter.IsEntryFromDirection(this.Direction); } }
		[DataMember] public string SignalName;	//ORDER_SETS_NAME_FOR_KILLER_ALERTS { get; protected set; }
		[DataMember] public Guid StrategyID;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC   { get; protected set; }
		[DataMember] public string StrategyName;	// JSON_DESERIALIZER_WANTS_MEMBERS_FULLY_PUBLIC   { get; protected set; }
		public Strategy Strategy { get; protected set; }
		public bool IsExecutorBacktestingNow { get {
				if (this.Strategy.Script == null) {
					throw new Exception("IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script=null for " + this);
				}
				if (this.Strategy.Script.Executor == null) {
					throw new Exception("IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script.Executor=null for " + this);
				}
				if (this.Strategy.Script.Executor.Backtester == null) {
					throw new Exception("IsExecutorBacktesting Couldn't be calculated because Alert.Strategy.Script.Executor.BacktesterFacade=null for " + this);
				}
				return this.Strategy.Script.Executor.Backtester.IsBacktestingNow;
			} }
		[DataMember] public BarScaleInterval BarsScaleInterval { get; protected set; }
		[DataMember] public OrderSpreadSide OrderSpreadSide;
		[DataMember] public Quote QuoteCreatedThisAlert;
		[DataMember] public Quote QuoteLastWhenThisAlertFilled;

		public Position PositionAffected;
		public DateTime PositionEntryDate { get {
				if (this.PositionAffected != null) {
					return this.PositionAffected.EntryDate;
				}
				return DateTime.MinValue;
			} }

		// set on Order(alert).executed;
		public Order OrderFollowed;
		public ManualResetEvent MreOrderFollowedIsNotNull { get; private set; }
		[DataMember] public double PriceDeposited;		// for a Future, we pay less that it's quoted (GUARANTEE DEPOSIT)
		public string IsAlertCreatedOnPreviousBar { get {
				string ret = "";
				DateTime serverTimeNow = this.Bars.MarketInfo.ServerTimeNow;
				DateTime nextBarOpen = this.PlacedBar.DateTimeNextBarOpenUnconditional;
				bool alertIsNotForCurrentBar = (serverTimeNow >= nextBarOpen);
				if (alertIsNotForCurrentBar) {
					ret = "serverTimeNow[" + serverTimeNow + "] >= nextBarOpen[" + nextBarOpen + "]";
				}
				return ret;
			} }

		public Alert() {	// called by Json.Deserialize()
			PlacedBarIndex = -1;
			FilledBarIndex = -1;
			//TimeCreatedServerBar = DateTime.MinValue;
			this.QuoteCreatedThisAlertServerTime = DateTime.MinValue;
			Symbol = "UNKNOWN_JUST_DESERIALIZED";
			//SymbolClass = "";		//QUIK
			//AccountNumber = "";
			PriceScript = 0;
			PriceDeposited = -1;		// for a Future, we pay less that it's quoted (GUARANTEE DEPOSIT)
			Qty = 0;
			MarketLimitStop = MarketLimitStop.Unknown;
			MarketOrderAs = MarketOrderAs.Unknown;
			Direction = Direction.Unknown;
			SignalName = "";
			StrategyID = Guid.Empty;
			StrategyName = "NO_STRATEGY"; 
			BarsScaleInterval = new BarScaleInterval(BarScale.Unknown, 0);
			OrderFollowed = null;
			MreOrderFollowedIsNotNull = new ManualResetEvent(false);
		}
		public Alert(Bar bar, double qty, double priceScript, string signalName,
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
			this.Bars = bar.ParentBars;
			this.PlacedBar = bar;
			this.PlacedBarIndex = bar.ParentBarsIndex;
			this.Symbol = bar.Symbol;
			
			this.BarsScaleInterval = this.Bars.ScaleInterval;
			if (this.Bars.SymbolInfo != null) {
				SymbolInfo symbolInfo = this.Bars.SymbolInfo;
				this.SymbolClass = (string.IsNullOrEmpty(symbolInfo.SymbolClass) == false) ? symbolInfo.SymbolClass : "UNKNOWN_CLASS";
				this.MarketOrderAs = symbolInfo.MarketOrderAs;
			}
			
			this.AccountNumber = "UNKNOWN_ACCOUNT";
			if (this.DataSource.BrokerProvider != null && this.DataSource.BrokerProvider.AccountAutoPropagate != null
			    && string.IsNullOrEmpty(this.Bars.DataSource.BrokerProvider.AccountAutoPropagate.AccountNumber) != false) {
				this.AccountNumber = this.Bars.DataSource.BrokerProvider.AccountAutoPropagate.AccountNumber;
			}
			

			this.Qty = qty;
			this.PriceScript = priceScript;
			this.SignalName = signalName;
			this.Direction = direction;
			this.MarketLimitStop = marketLimitStop;
			this.OrderSpreadSide = orderSpreadSide;

			this.Strategy = strategy;
			if (this.Strategy != null) {
				this.StrategyID = this.Strategy.Guid;
				this.StrategyName = this.Strategy.Name;
			}
			
			if (this.Strategy.Script != null) {
				string msg = "Looks like a manual Order submitted from the Chart";
				return;
			}
		}
		void initByFetchingForSerialization() {
			string firstReadInits4Serialization;
			//firstReadInits4Serialization = this.Symbol;
			firstReadInits4Serialization = this.SymbolClass;
			firstReadInits4Serialization = this.AccountNumber;
		}
		public string ToStringForOrder() {
			string msg = Direction
				+ " " + MarketLimitStop
				// not Symbol coz stack overflow
				+ " " + Symbol
				// not SymbolClass coz stack overflow
				+ "/" + SymbolClass;
			return msg;
		}
		public double QtyFilledThroughPosition { get {
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
		public double PriceFilledThroughPosition { get {
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
		public string ToStringForTooltip() {
			string longOrderType = (MarketLimitStop == MarketLimitStop.StopLimit) ? "" : "\t";

			string msg = Direction
				+ "\t" + MarketLimitStop
				+ "\t" + longOrderType + Qty + "/" + this.QtyFilledThroughPosition + "filled*" + Symbol
				+ "@" + PriceScript + "/" + this.PriceFilledThroughPosition + "filled"
				;
			if (this.PositionAffected != null && this.PositionAffected.Prototype != null) {
				msg += "\tProto" + this.PositionAffected.Prototype;
			}
			msg += "\t[" + SignalName + "]";
			return msg;
		}
		public override string ToString() {
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
			msg.Append(Direction);
			msg.Append(" ");
			msg.Append(MarketLimitStop);
			msg.Append(" ");
			msg.Append(Qty);
			msg.Append("*");
			msg.Append(this.Symbol);
			msg.Append("@");
			msg.Append(PriceScript);
			msg.Append(" on[");
			msg.Append(AccountNumber + "]");
			if (null == this.FilledBar) {
				msg.Append(":UNFILLED");
			} else {
				msg.Append(":FILLED@");
				msg.Append(this.PriceFilledThroughPosition);
				msg.Append("*");
				msg.Append(this.QtyFilledThroughPosition);
			}
			if (this.PositionAffected != null) {
				msg.Append("; PositionAffected=[");
				//msg.Append(this.PositionAffected.ToString());
				msg.Append("]");
			}
			return msg.ToString();;
		}
		public bool IsIdenticalOrderlessPriceless(Alert alert) {
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
				&& this.PositionEntryDate == alert.PositionEntryDate
				&& this.PlacedBarIndex == alert.PlacedBarIndex
				;
			if (alert.PlacedBarIndex == alert.Bars.Count) {
				return basic;
			}
			bool streamingBarMayBeDifferent = this.PriceScript == alert.PriceScript;
			return basic && streamingBarMayBeDifferent;
		}
		public bool IsIdenticalForOrdersPending(Alert alert) {
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
		public virtual void AbsorbFromExecutor(ScriptExecutor executor) {
		}
		public void FillPositionAffectedEntryOrExitRespectively(Bar barFill, int barFillRelno,
				double priceFill, double qtyFill, double slippageFill, double commissionFill) {
			//if (this.BarRelnoFilled != -1) {
			if (this.FilledBar != null) {
				string msg = "ALERT_ALREADY_FILLED_EARLIER_CANT_OVERRIDE @FilledBarIndex[" + this.FilledBarIndex + "]"
						+ ", duplicateFill @[" + barFill + "]";
				throw new Exception(msg);
			}
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
					#if DEBUG
					Debugger.Break();
					#endif
				}
			} else {
				this.PositionAffected.FillExitWith(barFill, priceFill, qtyFill, slippageFill, commissionFill);
			}
		}
		public bool IsFilled { get {
				if (this.PositionAffected == null) return false;
				return this.IsEntryAlert
					? this.PositionAffected.IsEntryFilled
					: this.PositionAffected.IsExitFilled;
			} }
		public bool IsKilled;
	}
}