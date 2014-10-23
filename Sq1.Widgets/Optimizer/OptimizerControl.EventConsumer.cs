using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Widgets.Optimizer {
	public partial class OptimizerControl {
		bool isRunningNow;
		void BtnRunCancelClick(object sender, EventArgs e) {
			if (isRunningNow) {
				this.executor.Backtester.AbortRunningBacktestWaitAborted("OPTIMIZER_CANCELLED");
				isRunningNow = false;
			} else {
				this.olvBacktests.UseWaitCursor = true;
				//this.executor.Backtester.RunSimulation();
				this.executor.BacktesterRunSimulationTrampoline(new Action(this.afterBacktesterComplete), true);
				this.olvBacktests.UseWaitCursor = false;
				isRunningNow = true;
			}
		}
		void afterBacktesterComplete() {
			if (this.executor.Bars == null) {
				string msg = "DONT_RUN_BACKTEST_BEFORE_BARS_ARE_LOADED";
				Assembler.PopupException(msg);
				return;
			}
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.afterBacktesterComplete(); });
				return;
			}
			
			this.executor.Performance.BuildStatsOnBacktestFinished();
			this.backtests.Add(this.executor.Performance.CloneForOptimizer);
			this.olvBacktests.SetObjects(this.backtests);
			
			// reschedule next backtest - use lock() to get new Task
			//this.executor.BacktesterRunSimulationTrampoline(new Action(this.afterBacktesterCompleteOnceOnRestart), true);
			
		}
	}
}
