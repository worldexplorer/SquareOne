using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;

using Sq1.Gui.Singletons;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm {
		void btnStartStop_Click(object sender, EventArgs e) {
			ToolStripButton btnPauseResume = this.LivesimControl.TssBtnPauseResume;
			ToolStripButton btnStartStop = this.LivesimControl.TssBtnStartStop;
			bool clickedStart = btnStartStop.Text.Contains("Start");
			if (clickedStart) {
				btnStartStop.Text = "Starting";
				btnStartStop.Enabled = false;

				//v1 Livesimulator livesimulator = this.chartFormManager.Executor.Livesimulator;
				//v1 LivesimDataSource ds = livesimulator.DataSourceAsLivesim_generator_nullUnsafe;
				//v1 bool clearExecutionExceptions = ds != null && ds.BrokerAsLivesim_nullUnsafe.LivesimBrokerSettings.ClearExecutionExceptions_beforeLivesim;
				bool clearExecutionExceptions = this.chartFormManager.Executor.Strategy.LivesimBrokerSettings.ClearExecutionExceptions_beforeLivesim;
				if (clearExecutionExceptions) {
					//if (MainForm.ExceptionsForm != null && DockContentImproved.
					ExceptionsForm.Instance.ExceptionControl.Clear();
					ExecutionForm.Instance.ExecutionTreeControl.Clear();
				}

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