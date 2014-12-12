using System;
using Sq1.Core.Execution;
using System.Collections.Generic;

namespace Sq1.Core.StrategyBase {
	public class ScriptExecutorEventGenerator {
		public event EventHandler<EventArgs> BacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4;
		public event EventHandler<EventArgs> BacktesterContextInitializedStep2of4;
		public event EventHandler<EventArgs> BacktesterSimulatedChunkStep3of4;
		public event EventHandler<EventArgs> BacktesterSimulatedAllBarsStep4of4;

		public event EventHandler<ReporterPokeUnitEventArgs>	BrokerOpenedOrClosedPositions;
		//YOU_KNOW_I_HATE_UNNECESSARY_EVENTS!!!__INVOKING_DIRECTLY_UpdateOpenPositionsDueToStreamingNewQuote() public event EventHandler<PositionListEventArgs>		OpenPositionsUpdatedDueToStreamingNewQuote;
		public event EventHandler<ReporterPokeUnitEventArgs>	ExecutorCreatedUnfilledPositions;

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
		public void RaiseBrokerOpenedOrClosedPositions(ReporterPokeUnit pokeUnit) {
			if (this.BrokerOpenedOrClosedPositions == null) return;
			this.BrokerOpenedOrClosedPositions(this, new ReporterPokeUnitEventArgs(pokeUnit));
		}
		//public void RaiseOpenPositionsUpdatedDueToStreamingNewQuote(List<Position> positionsUpdatedDueToStreamingNewQuote) {
		//    if (this.OpenPositionsUpdatedDueToStreamingNewQuote == null) return;
		//    this.OpenPositionsUpdatedDueToStreamingNewQuote(this, new PositionListEventArgs(new List<Position>(positionsUpdatedDueToStreamingNewQuote)));
		//}

		internal void RaiseExecutorCreatedPositions(ReporterPokeUnit pokeUnit) {
			if (this.ExecutorCreatedUnfilledPositions == null) return;
			this.ExecutorCreatedUnfilledPositions(this, new ReporterPokeUnitEventArgs(pokeUnit));
		}
	}
}
