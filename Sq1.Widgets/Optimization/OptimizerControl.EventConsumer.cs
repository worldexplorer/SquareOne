using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl {
		void btnRunCancel_Click(object sender, EventArgs e) {
			if (this.optimizer.IsRunningNow) {
				this.optimizer.OptimizationAbort();
				this.btnPauseResume.Enabled = false;
			} else {
				this.backtests.Clear();
				this.olvBacktests.SetObjects(this.backtests);
				int threadsLaunched = this.optimizer.OptimizationRun();
				this.btnRunCancel.Text = "Cancel " + this.optimizer.BacktestsRemaining + " backtests";
				//this.btnPauseResume.Enabled = true;
				this.olvBacktests.EmptyListMsg = threadsLaunched + " threads launched";
				//this.olvBacktests.UseWaitCursor = true;
			}
		}
		void Optimizer_OnBacktestComplete(object sender, SystemPerformanceEventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.Optimizer_OnBacktestComplete(sender, e); });
				return;
			}
			this.backtests.Add(e.SystemPerformance);
			this.olvBacktests.SetObjects(this.backtests);
			//this.olvBacktests.Refresh();
			
			int backtestsRemaninig	= this.optimizer.BacktestsRemaining;
			int backtestsTotal		= this.optimizer.BacktestsTotal;
			int backtestsCompleted	= this.optimizer.BacktestsCompleted;  
			this.btnRunCancel.Text = "Cancel " + backtestsRemaninig + " backtests";
			double pctComplete = (backtestsTotal > 0) ? Math.Round(100 * backtestsCompleted / (double)backtestsTotal) : 0;
			this.lblStats.Text = pctComplete + "% complete    " + backtestsCompleted + "/" + backtestsTotal;
			if (backtestsCompleted >= this.progressBar1.Minimum && backtestsCompleted <= this.progressBar1.Maximum) {
				this.progressBar1.Value = backtestsCompleted;
			}
		}
		void Optimizer_OnOptimizationComplete(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.Optimizer_OnOptimizationComplete(sender, e); });
				return;
			}
			this.btnPauseResume.Enabled = false;
			int totalBacktests = this.optimizer.BacktestsTotal;
			this.btnRunCancel.Text = "Run " + totalBacktests + " backtests";
			this.olvBacktests.EmptyListMsg = "";
			//this.lblStats.Text = "0% complete   0/" + totalBacktests;
			//this.progressBar1.Value = 0;
			//this.olvBacktests.UseWaitCursor = false;
		}
		void Optimizer_OnOptimizationAborted(object sender, EventArgs e) {
			this.Optimizer_OnOptimizationComplete(sender, e);
		}
		void btnPauseResume_Click(object sender, EventArgs e) {
			Assembler.PopupException(null, new NotImplementedException());
		}
		void nudCpuCoresToUse_ValueChanged(object sender, EventArgs e) {
			this.optimizer.CpuCoresToUse = (int)this.nudThreadsToRun.Value;
		}
		void mniCopyToDefaultCtxBacktest_Click(object sender, EventArgs e) {
			throw new NotImplementedException();
		}
		void mniCopyToDefaultCtx_Click(object sender, EventArgs e) {
			throw new NotImplementedException();
		}
	}
}
