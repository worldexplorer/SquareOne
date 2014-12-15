using System;
using Sq1.Core.Execution;
using System.Collections.Generic;

namespace Sq1.Core.StrategyBase {
	public class ScriptExecutorEventGenerator {
		public event EventHandler<EventArgs> BacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4;
		public event EventHandler<EventArgs> BacktesterContextInitializedStep2of4;
		public event EventHandler<EventArgs> BacktesterSimulatedChunkStep3of4;
		public event EventHandler<EventArgs> BacktesterSimulatedAllBarsStep4of4;

		public event EventHandler<ReporterPokeUnitEventArgs>	BrokerFilledAlertsOpeningForPositions_step1of3;
		//YOU_KNOW_I_HATE_UNNECESSARY_EVENTS!!!__INVOKING_DIRECTLY_UpdateOpenPositionsDueToStreamingNewQuote()
		public event EventHandler<PositionListEventArgs>		OpenPositionsUpdatedDueToStreamingNewQuote_step2of3;
		public event EventHandler<ReporterPokeUnitEventArgs>	BrokerFilledAlertsClosingForPositions_step3of3;

		private ScriptExecutor scriptExecutor;

		public ScriptExecutorEventGenerator(ScriptExecutor scriptExecutor) {
			this.scriptExecutor = scriptExecutor;
		}
		public void RaiseBacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4() {
			if (this.BacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4 == null) return;
			this.BacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4(this, null);
		}
		internal void RaiseBacktesterSimulationContextInitializedStep2of4() {
			if (this.BacktesterContextInitializedStep2of4 == null) return;
			this.BacktesterContextInitializedStep2of4(this, null);
		}
		public void RaiseBacktesterSimulatedChunkStep3of4() {
			if (this.BacktesterSimulatedChunkStep3of4 == null) return;
			this.BacktesterSimulatedChunkStep3of4(this, null);
		}
		public void RaiseBacktesterSimulatedAllBarsStep4of4() {
			if (this.BacktesterSimulatedAllBarsStep4of4 == null) return;
			this.BacktesterSimulatedAllBarsStep4of4(this, null);
		}
		public void RaiseBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			try {
				if (this.BrokerFilledAlertsOpeningForPositions_step1of3 == null) return;
				this.BrokerFilledAlertsOpeningForPositions_step1of3(this, new ReporterPokeUnitEventArgs(pokeUnit));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__BrokerFilledAlertsOpeningForPositions_step1of3()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(List<Position> positionsUpdatedDueToStreamingNewQuote) {
			try {
				if (this.OpenPositionsUpdatedDueToStreamingNewQuote_step2of3 == null) return;
				this.OpenPositionsUpdatedDueToStreamingNewQuote_step2of3(this, new PositionListEventArgs(new List<Position>(positionsUpdatedDueToStreamingNewQuote)));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__OpenPositionsUpdatedDueToStreamingNewQuote_step2of3()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseBrokerFilledAlertsClosingForPositions_step3of3(ReporterPokeUnit pokeUnit) {
			try {
				if (this.BrokerFilledAlertsClosingForPositions_step3of3 == null) return;
				this.BrokerFilledAlertsClosingForPositions_step3of3(this, new ReporterPokeUnitEventArgs(pokeUnit));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__BrokerFilledAlertsClosingForPositions_step3of3()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
