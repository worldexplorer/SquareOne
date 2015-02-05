using System;

using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;

namespace Sq1.Core.Livesim {
	public class Livesimulator : Backtester {
		public Backtester BacktesterBackup { get; private set; }
		public LivesimDataSource LivesimDataSource { get { return base.BacktestDataSource as LivesimDataSource; } }

		public Livesimulator(ScriptExecutor executor) : base(executor) {
			base.BacktestDataSource = new LivesimDataSource();
		}

		public void Start() {
			this.BacktesterBackup = base.Executor.Backtester;
			base.Executor.Backtester = this;
			//base.Executor.BacktesterRunSimulation();
			base.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
		}
		void afterBacktesterComplete(ScriptExecutor executorCompletePooled) {
			string dbg = "complete: " + executorCompletePooled.Strategy.ScriptContextCurrent.Name + " "
				+ executorCompletePooled.Strategy.Script.IndicatorParametersAsString;
			string dbg2 = "";
			foreach (string iName in executorCompletePooled.Performance.ScriptAndIndicatorParameterClonesByName.Keys) {
				IndicatorParameter ip = executorCompletePooled.Performance.ScriptAndIndicatorParameterClonesByName[iName];
				dbg2 += iName + "[" + ip.ValueCurrent + "]";
			}
			Assembler.PopupException(dbg + dbg2, null, false);
		}

		public void Stop() {
			base.AbortRunningBacktestWaitAborted("USER_CLICKED_STOP_IN_LIVESIM");
			base.Executor.Backtester = this.BacktesterBackup;
		}

		public void Pause() {
			this.LivesimDataSource.StreamingLivesim.StreamingLivesimUnpaused.Reset();
		}
		public void Unpause() {
			this.LivesimDataSource.StreamingLivesim.StreamingLivesimUnpaused.Set();
		}
		public override string ToString() {
			string ret = "LIVESIMULATOR_FOR_" + this.Executor.ToString();
			return ret;
		}

	}
}
