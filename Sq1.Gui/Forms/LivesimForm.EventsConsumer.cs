using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm {
		void btnStartStop_Click(object sender, EventArgs e) {
			ToolStripButton btnPauseResume = this.LivesimControl.TssBtnPauseResume;
			ToolStripButton btnStartStop = this.LivesimControl.TssBtnStartStop;
			bool clickedStart = btnStartStop.Text.Contains("Start");
			if (clickedStart) {
				btnStartStop.Text = "Starting";
				btnStartStop.Enabled = false;
				this.chartFormManager.LivesimStartedOrUnpaused_HideReportersAndExecution();
				this.chartFormManager.Executor.Livesimulator.Start_invokedFromGuiThread(btnStartStop, btnPauseResume, this.chartFormManager.ChartForm.ChartControl);
				btnStartStop.Text = "Stop";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = true;
				btnPauseResume.Checked = false;
			} else {
				btnStartStop.Text = "Stopping";
				btnStartStop.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Stop_invokedFromGuiThread();
				this.chartFormManager.LivesimEndedOrStoppedOrPaused_RestoreHiddenReportersAndExecution();
				btnStartStop.Text = "Start";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = false;
				btnPauseResume.Checked = false;
				this.chartFormManager.ChartForm.PopulateBtnStreamingTriggersScript_afterBarsLoaded();
			}
		}
		void btnPauseResume_Click(object sender, EventArgs e) {
			ToolStripButton btnPauseResume = this.LivesimControl.TssBtnPauseResume;
			bool clickedPause = btnPauseResume.Text.Contains("Pause");
			if (clickedPause) {
				btnPauseResume.Text = "Pausing";
				btnPauseResume.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Pause_invokedFromGuiThread();
				this.chartFormManager.LivesimEndedOrStoppedOrPaused_RestoreHiddenReportersAndExecution();
				this.chartFormManager.ReportersFormsManager.RebuildingFullReportForced_onLivesimPaused();
				btnPauseResume.Text = "Resume";
				btnPauseResume.Enabled = true;
				
				// when quote delay = 2..4, reporters are staying empty (probably GuiIsBusy) - clear&flush each like afterBacktestEnded
				this.chartFormManager.ReportersFormsManager.BuildReportFull_onBacktestFinished_allReporters();
				//?this.chartFormManager.ReportersFormsManager.RebuildingFullReportForced_onLivesimPausedStoppedEnded();
			} else {
				btnPauseResume.Text = "Resuming";
				btnPauseResume.Enabled = false;
				this.chartFormManager.LivesimStartedOrUnpaused_HideReportersAndExecution();
				this.chartFormManager.Executor.Livesimulator.Unpause_inGuiThread();
				btnPauseResume.Text = "Pause";
				btnPauseResume.Enabled = true;
			}
		}
		//void LivesimForm_Disposed(object sender, EventArgs e) {
		//	if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
		//	// both at FormCloseByX and MainForm.onClose()
		//	this.chartFormManager.ChartForm.MniShowLivesim.Checked = false;
		//	this.chartFormManager.MainForm.MainFormSerialize();
		//}
		void livesimForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (this.chartFormManager.Executor.Livesimulator.ImRunningLivesim) {
				this.chartFormManager.Executor.Livesimulator.Stop_invokedFromGuiThread();
			}

			// only when user closed => allow scriptEditorForm_FormClosed() to serialize
			if (this.chartFormManager.MainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
				//WHEN_DID_YOU_NEED_IT?? MAIN_FORM_STAYS_OPEN_AFTER_USER_CLICKED_X?? e.Cancel = true;
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				//OTHERWISE_I_NEED_TO_X_TWICE e.Cancel = true;
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