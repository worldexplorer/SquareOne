using System;
using System.Windows.Forms;

using Sq1.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm {
		void livesimForm_StrategyExecutedOneQuoteOrBarOrdersEmitted(object sender, EventArgs e) {
			ChartControl chartControl = this.chartFormManager.ChartForm.ChartControl;
			//v1 SKIPS_REPAINTING_KOZ_NOW_BACKTEST=TRUE chartControl.InvalidateAllPanels();
			chartControl.RefreshAllPanelsNonBlockingRefreshNotYetStarted();
		}

		void btnStartStop_Click(object sender, EventArgs e) {
			Button btnPauseResume = this.LivesimControl.BtnPauseResume;
			Button btnStartStop = this.LivesimControl.BtnStartStop;
			bool clickedStart = btnStartStop.Text.Contains("Start");
			if (clickedStart) {
				btnStartStop.Text = "Starting";
				btnStartStop.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Start_inGuiThread(btnStartStop, this.chartFormManager.ChartForm.ChartControl);
				btnStartStop.Text = "Stop";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = true;
			} else {
				btnStartStop.Text = "Stopping";
				btnStartStop.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Stop_inGuiThread();
				btnStartStop.Text = "Start";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = false;
			}
		}
		void btnPauseResume_Click(object sender, EventArgs e) {
			Button btnPauseResume = this.LivesimControl.BtnPauseResume;
			bool clickedPause = btnPauseResume.Text.Contains("Pause");
			if (clickedPause) {
				btnPauseResume.Text = "Pausing";
				btnPauseResume.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Pause_inGuiThread();
				btnPauseResume.Text = "Resume";
				btnPauseResume.Enabled = true;
			} else {
				btnPauseResume.Text = "Resuming";
				btnPauseResume.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Unpause_inGuiThread();
				btnPauseResume.Text = "Pause";
				btnPauseResume.Enabled = true;
			}
		}
		//void LivesimForm_Disposed(object sender, EventArgs e) {
		//    if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
		//    // both at FormCloseByX and MainForm.onClose()
		//    this.chartFormManager.ChartForm.MniShowLivesim.Checked = false;
		//    this.chartFormManager.MainForm.MainFormSerialize();
		//}
		void livesimForm_FormClosing(object sender, FormClosingEventArgs e) {
			// only when user closed => allow scriptEditorForm_FormClosed() to serialize
			if (this.chartFormManager.MainForm.MainFormClosingSkipChartFormsRemoval) {
				e.Cancel = true;
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				e.Cancel = true;
				return;
			}
		}
		void livesimForm_FormClosed(object sender, FormClosedEventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowLivesim.Checked = false;
			this.chartFormManager.MainForm.MainFormSerialize();
		}
	}
}