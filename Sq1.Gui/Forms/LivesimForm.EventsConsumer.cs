using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm {
		// ALREADY_HANDLED_BY_chartControl_BarAddedUpdated_ShouldTriggerRepaint
		//void livesimForm_StrategyExecutedOneQuoteOrBarOrdersEmitted(object sender, EventArgs e) {
		//	ChartControl chartControl = this.chartFormManager.ChartForm.ChartControl;
		//	//v1 SKIPS_REPAINTING_KOZ_NOW_BACKTEST=TRUE chartControl.InvalidateAllPanels();
		//	//chartControl.RefreshAllPanelsNonBlockingRefreshNotYetStarted();
		//}

		void btnStartStop_Click(object sender, EventArgs e) {
			//Button btnPauseResume = this.LivesimControl.BtnPauseResume;
			//Button btnStartStop = this.LivesimControl.BtnStartStop;
			ToolStripButton btnPauseResume = this.LivesimControl.TssBtnPauseResume;
			ToolStripButton btnStartStop = this.LivesimControl.TssBtnStartStop;
			bool clickedStart = btnStartStop.Text.Contains("Start");
			if (clickedStart) {
				//NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
				ScriptExecutor executor = this.chartFormManager.Executor;
				StreamingAdapter streamingAdapter = executor.DataSource_fromBars.StreamingAdapter;
				if (streamingAdapter != null) {
					string reasonWhyLivesimCanNotBeStartedForSymbol = streamingAdapter.ReasonWhyLivesimCanNotBeStartedForSymbol(executor.Bars.Symbol, executor.ChartShadow);
					if (string.IsNullOrEmpty(reasonWhyLivesimCanNotBeStartedForSymbol) == false) {
						string msg = "I_REFUSE_TO_START_LIVESIM_FOR[" + this.chartFormManager.WhoImServing_moveMeToExecutor + "]: " + reasonWhyLivesimCanNotBeStartedForSymbol;
						Assembler.PopupException(msg, null, false);
						btnStartStop.Checked = false;
						return;
					}
				}

				btnStartStop.Text = "Starting";
				btnStartStop.Enabled = false;
				this.chartFormManager.LivesimStartedOrUnpaused_AutoHiddeExecutionAndReporters();
				this.chartFormManager.Executor.Livesimulator.Start_inGuiThread(btnStartStop, btnPauseResume, this.chartFormManager.ChartForm.ChartControl);
				btnStartStop.Text = "Stop";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = true;
				btnPauseResume.Checked = false;

				// VERBOSE_LIVESIM_PART1/2_BTN_STREAMING_DISPLAYS_QUOTE_DETAILS-ACTIVATE
				//this.chartFormManager.ChartForm.BtnStrategyEmittingOrders.Enabled = false;
				//this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = true;
				this.chartFormManager.ChartForm.BtnStreamingTriggersScript.Enabled = false;
				//this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = true;
			} else {
				btnStartStop.Text = "Stopping";
				btnStartStop.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Stop_inGuiThread();
				this.chartFormManager.LivesimEndedOrStoppedOrPaused_RestoreAutoHiddenExecutionAndReporters();
				btnStartStop.Text = "Start";
				btnStartStop.Enabled = true;
				btnPauseResume.Enabled = false;
				btnPauseResume.Checked = false;

				// VERBOSE_LIVESIM_PART1/2_BTN_STREAMING_DISPLAYS_QUOTE_DETAILS-RESTORE
				this.chartFormManager.ChartForm.PopulateBtnStreamingTriggersScript_afterBarsLoaded();
			}
		}
		void btnPauseResume_Click(object sender, EventArgs e) {
			//Button btnPauseResume = this.LivesimControl.BtnPauseResume;
			ToolStripButton btnPauseResume = this.LivesimControl.TssBtnPauseResume;
			bool clickedPause = btnPauseResume.Text.Contains("Pause");
			if (clickedPause) {
				btnPauseResume.Text = "Pausing";
				btnPauseResume.Enabled = false;
				this.chartFormManager.Executor.Livesimulator.Pause_inGuiThread();
				this.chartFormManager.LivesimEndedOrStoppedOrPaused_RestoreAutoHiddenExecutionAndReporters();
				this.chartFormManager.ReportersFormsManager.RebuildingFullReportForced_onLivesimPaused();
				btnPauseResume.Text = "Resume";
				btnPauseResume.Enabled = true;
				
				// when quote delay = 2..4, reporters are staying empty (probably GuiIsBusy) - clear&flush each like afterBacktestEnded
				this.chartFormManager.ReportersFormsManager.BuildReportFullOnBacktestFinishedAllReporters();
				//?this.chartFormManager.ReportersFormsManager.RebuildingFullReportForced_onLivesimPausedStoppedEnded();
			} else {
				btnPauseResume.Text = "Resuming";
				btnPauseResume.Enabled = false;
				this.chartFormManager.LivesimStartedOrUnpaused_AutoHiddeExecutionAndReporters();
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
			if (this.chartFormManager.Executor.Livesimulator.IsBacktestingLivesimNow) {
				this.chartFormManager.Executor.Livesimulator.Stop_inGuiThread();
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