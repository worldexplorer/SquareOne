using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
//		public Position BuyOrShortAlertCreateDontRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
//														  Direction direction, MarketLimitStop entryMarketLimitStop) {
//			return BuyOrShortAlertCreateRegister(entryBar, stopOrLimitPrice, entrySignalName,
//												 direction, entryMarketLimitStop, false);
//		}
		public Position BuyOrShort_alertCreateRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
													  Direction direction, MarketLimitStop entryMarketLimitStop, bool registerInNew = true) {
			string msig = " //BuyOrShortAlertCreateRegister(stopOrLimitPrice[" + stopOrLimitPrice+ "], entrySignalName[" + entrySignalName + "], entryBar[" + entryBar + "])";
			this.checkThrow_alertCanBeCreated(entryBar, msig);

			Alert alert = null;
			// real-time streaming should create its own Position after an Order gets filled
			if (this.IsStreamingTriggeringScript) {
				alert = this.AlertFactory.EntryAlertCreate(entryBar, stopOrLimitPrice, entrySignalName,
														 direction, entryMarketLimitStop);
			} else {
				//string msg = "YOU_DONT_EMIT_ORDERS_THEN_CONTINUE_BACKTEST_BASED_ON_LIVE_QUOTES";
				string msg = "BACKTESTS_MUST_RUN_IN_STREAMING_SINCE_MarketSimStatic_WAS_DEPRECATED_INFAVOROF_MarketRealStreaming";
				Assembler.PopupException(msg);
				return null;
			}
			Alert similar = this.ExecutionDataSnapshot.AlertsPending.FindSimilarNotSameIdenticalForOrdersPending(alert, this, "BuyOrShortAlertCreateRegister(WAIT)");
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
		public Alert SellOrCover_alertCreate_dontRegisterInNew(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
															 Direction direction, MarketLimitStop exitMarketLimitStop) {
			return this.SellOrCover_alertCreateRegister(exitBar, position, stopOrLimitPrice, signalName,
													   direction, exitMarketLimitStop, false);
		}
		public Alert SellOrCover_alertCreateRegister(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
													Direction direction, MarketLimitStop exitMarketLimitStop, bool registerInNewAfterExec = true) {

			this.checkThrow_alertCanBeCreated(exitBar, "BARS.BARSTREAMING_OR_BARS.BARLASTSTATIC_IS_NULL_SellOrCoverAlertCreateRegister() ");
			if (position == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception("POSITION_CAN_NOT_BE_NULL_SellOrCoverAlertCreateRegister()");
			}

			Alert alert = null;
			if (position.Prototype != null) {
				if (signalName.Contains("protoTakeProfitExit")
					&& position.Prototype.TakeProfitAlert_forMoveAndAnnihilation != null
					&& this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == false) {
					string msg = "I won't create another protoTakeProfitExit because"
						+ " position.Prototype.TakeProfitAlertForAnnihilation != null"
						+ " position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
				if (signalName.Contains("protoStopLossExit")
					&& position.Prototype.StopLossAlert_forMoveAndAnnihilation != null
					&& this.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == false) {
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

				List<Alert> pendingSafe = this.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "//SellOrCoverAlertCreateRegister(WAIT)");
				foreach (Alert closingAlertForPosition in pendingSafe) {
					if (closingAlertForPosition.PositionAffected == position && closingAlertForPosition.IsExitAlert) {
						string msg = "PENDING_EXIT_ALERT_FOUND_WHILE_POSITION.EXITALERT=NULL"
							+ "; position.ExitAlert[" + position.ExitAlert + "] != closingAlertForPosition[" + closingAlertForPosition + "]";
						this.PopupException(msg);
						return closingAlertForPosition;
					}
				}
			}

			if (this.IsStreamingTriggeringScript) {
				alert = this.AlertFactory.ExitAlertCreate(exitBar, position, stopOrLimitPrice, signalName,
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
		void checkThrow_alertCanBeCreated(Bar entryBar, string msig) {
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
		public bool AlertCounterparty_annihilate_dispatched(Alert alert) {
			if (alert == null) {
				string msg = "don't invoke KillAlert with alert=null; check for TP=0 or SL=0 prior to invocation";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			bool killed = false;
			if (this.IsStreamingTriggeringScript == false) {
				//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
				string msg = "NYI_FOR IsStreamingTriggeringScript=false //AnnihilateCounterpartyAlertDispatched()";
				Assembler.PopupException(msg);
				return killed;
			}
			//v1 ScriptExecutor trying be too smart
			//if (this.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow == true) {
			//	killed = this.MarketsimBacktest.AnnihilateCounterpartyAlert(alert);
			//	//killed = this.MarketSimStatic.AnnihilateCounterpartyAlert(alert);
			//} else {
			//	killed = this.AlertGenerator.AnnihilateCounterpartyAlert(alert);
			//}
			//v2 BrokerAdapter is now responsible for the implementation (Backtest/Livesim/Live)
			killed = this.DataSource_fromBars.BrokerAdapter.AlertCounterparty_annihilate(alert);
			return killed;
		}
		public void AlertPending_kill(Alert alert) {
			//v1 ScriptExecutor trying be too smart
			////if (this.Backtester.IsBacktestingNow) {
			//if (this.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
			//	this.MarketsimBacktest.SimulateAlertKillPending(alert);
			//} else {
			//	this.AlertGenerator.AlertKillPending(alert);
			//}
			//v2 BrokerAdapter is now responsible for the implementation (Backtest/Livesim/Live)
			this.DataSource_fromBars.BrokerAdapter.AlertPending_kill(alert);
		}
	}
}
