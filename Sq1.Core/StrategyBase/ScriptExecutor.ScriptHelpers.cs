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
		public Position BuyOrShort_alertAndPosition_createRegister(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
													  Direction direction, MarketLimitStop entryMarketLimitStop, bool registerInNew = true) {
			string msig = " //BuyOrShortAlertCreateRegister(stopOrLimitPrice[" + stopOrLimitPrice+ "], entrySignalName[" + entrySignalName + "], entryBar[" + entryBar + "])";
			this.checkThrow_alertCanBeCreated(entryBar, msig);

			Alert alert = null;
			// real-time streaming should create its own Position after an Order gets filled
			if (this.IsStreamingTriggeringScript) {
				alert = this.AlertFactory.EntryAlert_create(entryBar, stopOrLimitPrice, entrySignalName,
														 direction, entryMarketLimitStop);
			} else {
				//string msg = "YOU_DONT_EMIT_ORDERS_THEN_CONTINUE_BACKTEST_BASED_ON_LIVE_QUOTES";
				string msg = "BACKTESTS_MUST_RUN_IN_STREAMING_SINCE_MarketSimStatic_WAS_DEPRECATED_INFAVOROF_MarketRealStreaming";
				Assembler.PopupException(msg);
				return null;
			}
			Alert similar = this.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.FindSimilarNotSameIdenticalForOrdersPending(alert, this, "BuyOrShortAlertCreateRegister(WAIT)");
			if (similar != null) {
				string msg = "DUPLICATE_ALERT_FOUND similar[" + similar + "]";
				Assembler.PopupException(msg + msig);
				return similar.PositionAffected;
			}

			this.ExecutionDataSnapshot.AlertEnriched_register(alert, registerInNew);

			// ok for single-entry strategies; nogut if we had many Streaming alerts and none of orders was filled yet...
			// MOVED_TO_ON_ALERT_FILLED_CALBACK
			Position pos = new Position(alert, alert.PriceScript);
			alert.PositionAffected = pos;
			return pos;
		}
		public Alert SellOrCover_alertCreate_dontRegisterInNew_prototypeActivator(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
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
					&& this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) {
					string msg = "I won't create another protoTakeProfitExit because"
						+ " position.Prototype.TakeProfitAlertForAnnihilation != null"
						+ " position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
				if (signalName.Contains("protoStopLossExit")
					&& position.Prototype.StopLossAlert_forMoveAndAnnihilation != null
					&& this.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) {
					string msg = "I won't create another protoStopLossExit because"
						+ " position.Prototype.StopLossAlertForAnnihilation != null"
						+ " position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}
			} else {
				if (position.ExitAlert != null && position.ExitAlert.IsKilled == false) {
					string msg = "POSITION_ALREADY_HAS_AN_EXIT_ALERT_REPLACE_INSTEAD_OF_ADDING_SECOND_SellOrCoverAlertCreateRegister();"
						+ " Strategy[" + this.Strategy.ToString() + "] position.Prototype=null position[" + position + "]";
					this.PopupException(msg);
					return position.ExitAlert;
				}

				List<Alert> pendingSafe = this.ExecutionDataSnapshot.AlertsPending_havingOrderFollowed_notYetFilled.SafeCopy(this, "//SellOrCoverAlertCreateRegister(WAIT)");
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
				alert = this.AlertFactory.ExitAlert_create(exitBar, position, stopOrLimitPrice, signalName,
														direction, exitMarketLimitStop);
			} else {
				//string msg = "YOU_DONT_EMIT_ORDERS_THEN_CONTINUE_BACKTEST_BASED_ON_LIVE_QUOTES";
				string msg = "BACKTESTS_MUST_RUN_IN_STREAMING_SINCE_MarketSimStatic_WAS_DEPRECATED_INFAVOROF_MarketRealStreaming";
				Assembler.PopupException(msg);
				return alert;
			}

			this.ExecutionDataSnapshot.AlertEnriched_register(alert, registerInNewAfterExec);

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


		public void AlertPending_kill(Alert alert) {
			string msig = " //AlertPending_kill(WAIT)";
			bool doomedAlready = this.ExecutionDataSnapshot.AlertsDoomed.Contains(alert, this, msig);
			if (doomedAlready) {
				string msg = "ALREADY_DOOMED__YOU_INVOKED_Script.AlertPending_kill()_MORE_THAN_ONCE_FOR_THE_SAME_ALERT";
				Assembler.PopupException(msg + msig);
				if (alert.OrderFollowed != null) {
					this.OrderProcessor.AppendMessage_propagateToGui(alert.OrderFollowed, msg + msig);
				}
				return;
			}
			this.ExecutionDataSnapshot.AlertsDoomed.AddNoDupe(alert, this, msig);
		}



		#region DIRTY userland moved to the Kernel!! wow
		internal List<Alert> CloseAllOpenPositions_killAllPendingAlerts() {
			string msig = " //CloseAllOpenPositions_killAllPendingAlerts(WAIT)";

			Bar barNewStaticArrived = this.Bars.BarStaticLast_nullUnsafe;
			int barIndex = barNewStaticArrived.ParentBarsIndex;

			List<Position> positionsOpenNow = this.ExecutionDataSnapshot.Positions_Pending_orOpenNow.SafeCopy(this, msig);
			List<Alert> alertsSubmittedToKill_forAllOpenPositions = new List<Alert>();
			foreach (Position positionOpen in positionsOpenNow) {
				List<Alert> alertsSubmittedToKill = this.PositionClose_immediately(positionOpen, "EXIT_FORCED_" + barNewStaticArrived.DateTimeOpen.ToString(), true);
				alertsSubmittedToKill_forAllOpenPositions.AddRange(alertsSubmittedToKill);
			}
			// NOTE that PositionCloseImmediately() already killed all positionPrototype-related Pending Alerts;
			// NOTE if you didn't use low level Alerts (like SellByStop()), no need to kill PendingAlerts manually here!
			//List<Alert> alertsPending = base.Executor.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "//Gap2StudiesCompiled.closeAllOpenPositionsKillPendingAlertsAtExitForced(WAIT)");
			//foreach (Alert alertPending in alertsPending) {
			//    if (alertPending.PositionAffected != null && alertPending.PositionAffected.Prototype != null) {
			//        return;
			//    }
			//    base.Executor.AlertPending_kill(alertPending);
			//    base.Executor.ChartShadow.BarAnnotationDrawModify(barIndex, "bar#" + barIndex + alertPending.ToString(),
			//        "ka" + alertPending.PlacedBarIndex,
			//        null, Color.Maroon, Color.Empty, false, 30, true);
			//}
			return alertsSubmittedToKill_forAllOpenPositions;
		}

		public List<Alert> PositionClose_immediately(Position positionOpen, string signalName, bool annotateAtBars_forEachClosedPosition = true) {
			Bar barNewStaticArrived = this.Bars.BarStaticLast_nullUnsafe;
			int barIndex = barNewStaticArrived.ParentBarsIndex;

			List<Alert> killedOnce = this.Position_exitAlert_kill(positionOpen, signalName);
			this.Strategy.Script.ExitAtMarket(this.Bars.BarStreaming_nullUnsafe, positionOpen, signalName);
			// BETTER WOULD BE KILL PREVIOUS PENDING ALERT FROM A CALBACK AFTER MARKET EXIT ORDER GETS FILLED, IT'S UNRELIABLE EXIT IF WE KILL IT HERE
			// LOOK AT EMERGENCY CLASSES, SOLUTION MIGHT BE THERE ALREADY
			if (annotateAtBars_forEachClosedPosition) {
				this.ChartShadow.BarAnnotationDrawModify(barIndex, "bar#" + barIndex + positionOpen.ToString(),
					"cp" + positionOpen.EntryFilledBarIndex + ":" + killedOnce.Count,
					null, Color.Maroon, Color.Empty, false, 30, true);
			}
			return killedOnce;
		}
		public List<Alert> Position_exitAlert_kill(Position position, string signalName) {
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
				alertsSubmittedToKill = this.PositionPrototype_killWhateverIsPending(position.Prototype, signalName);
				return alertsSubmittedToKill;
			}
			this.AlertPending_kill(position.ExitAlert);
			alertsSubmittedToKill.Add(position.ExitAlert);
			return alertsSubmittedToKill;
		}
		
		public List<Alert> PositionPrototype_killWhateverIsPending(PositionPrototype proto, string signalName) {
			List<Alert> alertsSubmittedToKill = new List<Alert>();
			if (proto.StopLossAlert_forMoveAndAnnihilation != null) {
				this.AlertPending_kill(proto.StopLossAlert_forMoveAndAnnihilation);
				alertsSubmittedToKill.Add(proto.StopLossAlert_forMoveAndAnnihilation);
			}
			if (proto.TakeProfitAlert_forMoveAndAnnihilation != null) {
				this.AlertPending_kill(proto.TakeProfitAlert_forMoveAndAnnihilation);
				alertsSubmittedToKill.Add(proto.TakeProfitAlert_forMoveAndAnnihilation);
			}
			return alertsSubmittedToKill;
		}
		#endregion
	}
}
