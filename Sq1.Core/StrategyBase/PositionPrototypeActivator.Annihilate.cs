using System;

using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public partial class PositionPrototypeActivator {
		public bool AnnihilateCounterparty_forClosedPosition(Position position) {
			string msig = " //AnnihilateCounterparty_forClosedPosition(" + position + ")";
			bool killedForBacktest_emittedForLive = false;

			if (this.executor.IsStreamingTriggeringScript == false) {
				string msg = "EXECUTOR_SHOULD_NOT_HAVE_INVOKED_ME IsStreamingTriggeringScript=false";
				Assembler.PopupException(msg + msig);
				return killedForBacktest_emittedForLive;
			}

			if (position.IsExitFilled == false) {
				string msg = "POSITION_MUST_BE_FILLED_BEFORE_ANNIHILATING_UNFILLED_COUNTERPARTY_ALERT";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			if (position.Prototype == null) {
				string msg = "POSITION_MUST_BE_PROTOTYPED_TO_ANNIHILATE_COUNTERPARTY_ALERT";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			if (position.IsExitFilled_byTakeProfit_prototyped && position.Prototype.StopLoss_negativeOffset < 0) {
				if (position.Prototype.StopLossAlert_forMoveAndAnnihilation == null) {
					string msg = "PROTOTYPED_STOP_LOSS_ALERT_MUST_NOT_BE_NULL";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg + msig);
				}
				Alert stopLoss_toAnnihilate = position.Prototype.StopLossAlert_forMoveAndAnnihilation;
				killedForBacktest_emittedForLive = this.executor.DataSource_fromBars.BrokerAdapter.AlertCounterparty_annihilate(stopLoss_toAnnihilate);
				bool liveOrLivesim = executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false;
				if (liveOrLivesim) {
					string msg = "COUNTERPARTY_STOPLOSS_ANNIHILATED annihilationScheduled[" + killedForBacktest_emittedForLive + "] alertStopLoss_toAnnihilate[" + stopLoss_toAnnihilate + "]";
					this.appendMessage_toOrderFollowed(stopLoss_toAnnihilate, msg);
				} else {
					string why = "backtest has only alerts but no orders; nowhere to append messages";
				}
				if (killedForBacktest_emittedForLive == false) {
					string msg = "position.ClosedByTakeProfit but StopLoss wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated StopLoss";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg + msig);
				}
			}

			if (position.IsExitFilled_byStopLoss_prototyped && position.Prototype.TakeProfit_positiveOffset > 0) {
				if (position.Prototype.TakeProfitAlert_forMoveAndAnnihilation == null) {
					string msg = "FAILED_ANNIHILATE_TAKEPROFIT Prototype.TakeProfitAlert_forMoveAndAnnihilation=null for position[" + position + "]";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg + msig);
				}
				Alert takeProfit_toAnnihilate = position.Prototype.TakeProfitAlert_forMoveAndAnnihilation;
				killedForBacktest_emittedForLive = this.executor.DataSource_fromBars.BrokerAdapter.AlertCounterparty_annihilate(takeProfit_toAnnihilate);
				if (executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) {
					string msg = "COUNTERPARTY_TAKEPROFIT_ANNIHILATED annihilationScheduled[" + killedForBacktest_emittedForLive + "] takeProfit_toAnnihilate[" + takeProfit_toAnnihilate + "]";
					this.appendMessage_toOrderFollowed(takeProfit_toAnnihilate, msg);
				}
				if (killedForBacktest_emittedForLive == false) {
					string msg = "position.ClosedByStopLoss but TakeProfit wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated TakeProfit";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg + msig);
				}
			}
			return killedForBacktest_emittedForLive;
		}
	
		void appendMessage_toOrderFollowed(Alert alert_takeProfit_or_stopLoss, string msgOrder) {
			string msig = " //appendMessage_toOrderFollowed(" + alert_takeProfit_or_stopLoss + ")";
			if (alert_takeProfit_or_stopLoss == null) {
				string msg = "DONT_INVOKE_ME_FOR_BACKTEST__ONLY_LIVE_AND_LIVESIM_ISSUE_ORDERS_FOR_ALERTS";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			Order orderFollowed = alert_takeProfit_or_stopLoss.OrderFollowed_orCurrentReplacement;
			if (orderFollowed == null) {
				string msg = "MUST_NOT_BE_NULL alert_takeProfit_or_stopLoss.OrderFollowed=NULL DONT_INVOKE_ME_FOR_BACKTEST__ONLY_LIVE_AND_LIVESIM_ISSUE_ORDERS_FOR_ALERTS";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			orderFollowed.AppendMessage(msgOrder);
		}

	}
}