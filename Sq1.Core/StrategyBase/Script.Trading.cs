using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public abstract partial class Script {
		public Position BuyAtLimit(Bar bar, double limitPrice, string signalName = "BOUGHT_AT_LIMIT") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, limitPrice, signalName, Direction.Buy, MarketLimitStop.Limit);
		}
		public Position BuyAtMarket(Bar bar, string signalName = "BOUGHT_AT_MARKET") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.Market);
		}
		public Position BuyAtStop(Bar bar, double stopPrice, string signalName = "BOUGHT_AT_STOP") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, stopPrice, signalName, Direction.Buy, MarketLimitStop.Stop);
		}
		
		public Alert CoverAtLimit(Bar bar, Position position, double limitPrice, string signalName = "COVERED_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Cover, MarketLimitStop.Limit);
		}
		public Alert CoverAtMarket(Bar bar, Position position, string signalName = "COVERED_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.Market);
		}
		public Alert CoverAtStop(Bar bar, Position position, double stopPrice, string signalName = "COVERED_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Cover, MarketLimitStop.Stop);
		}

		public Alert SellAtLimit(Bar bar, Position position, double limitPrice, string signalName = "SOLD_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Sell, MarketLimitStop.Limit);
		}
		public Alert SellAtMarket(Bar bar, Position position, string signalName = "SOLD_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.Market);
		}
		public Alert SellAtStop(Bar bar, Position position, double stopPrice, string signalName = "SOLD_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Sell, MarketLimitStop.Stop);
		}
		
		public Position ShortAtLimit(Bar bar, double limitPrice, string signalName = "SHORTED_AT_LIMIT") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, limitPrice, signalName, Direction.Short, MarketLimitStop.Limit);
		}
		public Position ShortAtMarket(Bar bar, string signalName = "SHORTED_AT_MARKET") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Short, MarketLimitStop.Market);
		}
		public Position ShortAtStop(Bar bar, double stopPrice, string signalName = "SHORTED_AT_STOP") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, stopPrice, signalName, Direction.Short, MarketLimitStop.Stop);
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
		
		#region *AtClose: NYI
//		public Position BuyAtClose(Bar bar, string signalName = "BOUGHT_AT_CLOSE") {
//			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.AtClose);
//		}
//		public Alert CoverAtClose(Bar bar, Position position, string signalName = "COVERED_AT_CLOSE") {
//			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.AtClose);
//		}
//		public Alert SellAtClose(Bar bar, Position position, string signalName = "SOLD_AT_CLOSE") {
//			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.AtClose);
//		}
//		public Position ShortAtClose(Bar bar, string signalName = "SHORTED_AT_CLOSE") {
//			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Short, MarketLimitStop.AtClose);
//		}
//		public Alert ExitAtClose(Bar bar, Position position, string signalName = "EXITED_AT_CLOSE") {
//			if (position.PositionLongShort == PositionLongShort.Long) {
//				return this.SellAtClose(bar, position, signalName);
//			} else {
//				return this.CoverAtClose(bar, position, signalName);
//			}
//		}
		#endregion

		#region Kill pending alert
		public void AlertKillPending(Alert alert) {
			this.Executor.AlertKillPending(alert);
		}

		[Obsolete("looks unreliable until refactored; must kill previous alertExit AFTER killing market completes => userland callback or more intelligent management in CORE level")]
		public List<Alert> PositionCloseImmediately(Position position, string signalName) {
			this.ExitAtMarket(this.Bars.BarStreaming_nullUnsafe, position, signalName);
			// BETTER WOULD BE KILL PREVIOUS PENDING ALERT FROM A CALBACK AFTER MARKET EXIT ORDER GETS FILLED, IT'S UNRELIABLE EXIT IF WE KILL IT HERE
			// LOOK AT EMERGENCY CLASSES, SOLUTION MIGHT BE THERE ALREADY
			return this.PositionKillExitAlert(position, signalName);
		}
		public List<Alert> PositionKillExitAlert(Position position, string signalName) {
			List<Alert> alertsSubmittedToKill = new List<Alert>();
			if (position.IsEntryFilled == false) {
				string msg = "I_REFUSE_TO_KILL_UNFILLED_ENTRY_ALERT position[" + position + "]";
				Assembler.PopupException(msg);
				return alertsSubmittedToKill;
			}
			if (null == position.ExitAlert) {
				string msg = "FIXME I_REFUSE_TO_KILL_UNFILLED_EXIT_ALERT {for prototyped position, position.ExitAlert contains TakeProfit} position[" + position + "]";
				Debugger.Break();
				return alertsSubmittedToKill;
			}
			if (string.IsNullOrEmpty(signalName)) signalName = "PositionCloseImmediately()";
			if (position.Prototype != null) {
				alertsSubmittedToKill = this.PositionPrototypeKillWhateverIsPending(position.Prototype, signalName);
				return alertsSubmittedToKill;
			}
			this.AlertKillPending(position.ExitAlert);
			alertsSubmittedToKill.Add(position.ExitAlert);
			return alertsSubmittedToKill;
		}
		
		public List<Alert> PositionPrototypeKillWhateverIsPending(PositionPrototype proto, string signalName) {
			List<Alert> alertsSubmittedToKill = new List<Alert>();
			if (proto.StopLossAlertForAnnihilation != null) {
				this.AlertKillPending(proto.StopLossAlertForAnnihilation);
				alertsSubmittedToKill.Add(proto.StopLossAlertForAnnihilation);
			}
			if (proto.TakeProfitAlertForAnnihilation != null) {
				this.AlertKillPending(proto.TakeProfitAlertForAnnihilation);
				alertsSubmittedToKill.Add(proto.TakeProfitAlertForAnnihilation);
			}
			return alertsSubmittedToKill;
		}
		#endregion
	}
}