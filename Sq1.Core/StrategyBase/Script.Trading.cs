using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public partial class Script {
		public Alert BuyAtLimit(Bar bar, double limitPrice, string signalName = "BOUGHT_AT_LIMIT") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, limitPrice, 0, signalName, Direction.Buy, MarketLimitStop.Limit);
		}
		public Alert BuyAtMarket(Bar bar, string signalName = "BOUGHT_AT_MARKET") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, 0, 0, signalName, Direction.Buy, MarketLimitStop.Market);
		}
		public Alert BuyAtStop(Bar bar, double stopPrice, string signalName = "BOUGHT_AT_STOP") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, stopPrice, 0, signalName, Direction.Buy, MarketLimitStop.Stop);
		}
		public Alert BuyAtStopLimit(Bar bar, double limitPrice, double priceStopActivation, string signalName = "BOUGHT_AT_STOPLIMIT_UNTESTED") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, limitPrice, priceStopActivation, signalName, Direction.Buy, MarketLimitStop.StopLimit);
		}
		

		public Alert CoverAtLimit(Bar bar, Position position, double limitPrice, string signalName = "COVERED_AT_LIMIT") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, limitPrice, 0, signalName, Direction.Cover, MarketLimitStop.Limit);
		}
		public Alert CoverAtMarket(Bar bar, Position position, string signalName = "COVERED_AT_MARKET") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, 0, 0, signalName, Direction.Cover, MarketLimitStop.Market);
		}
		public Alert CoverAtStop(Bar bar, Position position, double stopPrice, string signalName = "COVERED_AT_STOP") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, stopPrice, 0, signalName, Direction.Cover, MarketLimitStop.Stop);
		}
		public Alert CoverAtStop(Bar bar, Position position, double limitPrice, double priceStopActivation, string signalName = "COVERED_AT_STOPLIMIT_UNTESTED") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, limitPrice, priceStopActivation, signalName, Direction.Cover, MarketLimitStop.StopLimit);
		}


		public Alert SellAtLimit(Bar bar, Position position, double limitPrice, string signalName = "SOLD_AT_LIMIT") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, limitPrice, 0, signalName, Direction.Sell, MarketLimitStop.Limit);
		}
		public Alert SellAtMarket(Bar bar, Position position, string signalName = "SOLD_AT_MARKET") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, 0, 0, signalName, Direction.Sell, MarketLimitStop.Market);
		}
		public Alert SellAtStop(Bar bar, Position position, double stopPrice, string signalName = "SOLD_AT_STOP") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, stopPrice, 0, signalName, Direction.Sell, MarketLimitStop.Stop);
		}
		public Alert SellAtStopLimit(Bar bar, Position position, double limitPrice, double priceStopActivation, string signalName = "SOLD_AT_STOPLIMIT_UNTESTED") {
			return this.Executor.SellOrCover_alertCreateRegister(bar, position, limitPrice, priceStopActivation, signalName, Direction.Sell, MarketLimitStop.StopLimit);
		}

		
		public Alert ShortAtLimit(Bar bar, double limitPrice, string signalName = "SHORTED_AT_LIMIT") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, limitPrice, 0, signalName, Direction.Short, MarketLimitStop.Limit);
		}
		public Alert ShortAtMarket(Bar bar, string signalName = "SHORTED_AT_MARKET") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, 0, 0, signalName, Direction.Short, MarketLimitStop.Market);
		}
		public Alert ShortAtStop(Bar bar, double stopPrice, string signalName = "SHORTED_AT_STOP") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, stopPrice, 0, signalName, Direction.Short, MarketLimitStop.Stop);
		}
		public Alert ShortAtStopLimit(Bar bar, double limitPrice, double priceStopActivation, string signalName = "SHORTED_AT_STOPLIMIT_UNTESTED") {
			return this.Executor.BuyOrShort_alertCreateRegister(bar, limitPrice, priceStopActivation, signalName, Direction.Short, MarketLimitStop.StopLimit);
		}


		public Alert ExitAtMarket(Bar bar, Position position, string signalName = "EXITED_AT_MARKET") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtMarket(bar, position, signalName);
			} else {
				return this.CoverAtMarket(bar, position, signalName);
			}
		}
		public Alert ExitAtLimit(Bar bar, Position position, double price, string signalName = "EXITED_AT_LIMIT") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtLimit(bar, position, price, signalName);
			} else {
				return this.CoverAtLimit(bar, position, price, signalName);
			}
		}
		public Alert ExitAtStop(Bar bar, Position position, double price, string signalName = "EXITED_AT_STOP") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtStop(bar, position, price, signalName);
			} else {
				return this.CoverAtStop(bar, position, price, signalName);
			}
		}

		#region Kill pending alert
		public void AlertPendingKill_appendToDoomed_inExecutorSnap(Alert alert) {
			this.Executor.AlertPendingKill_appendToDoomed_willBeSubmitted_afterScriptInovcationReturned(alert);
		}
		public List<Alert> PositionClose_immediately(Position position, string signalName, bool annotateAtBars_forEachClosedPosition = false) {
			return this.Executor.Position_closeImmediately_killAllExitAlerts_ExitAtMarket_ordersEmitRequired(position, signalName, annotateAtBars_forEachClosedPosition);
		}
		#endregion
	}
}