using System;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class ScriptExecutorEventGenerator {
		public event EventHandler<EventArgs> BacktesterBarsIdenticalButEmptySubstitutedToGrowStep1of4;
		public event EventHandler<EventArgs> BacktesterContextInitializedStep2of4;
		public event EventHandler<EventArgs> BacktesterSimulatedChunkStep3of4;
		public event EventHandler<EventArgs> BacktesterSimulatedAllBarsStep4of4;

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
	}
}
