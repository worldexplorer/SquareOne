using System;

using Sq1.Core.Support;

namespace Sq1.Charting {
	public partial class ChartControl	{
		TimerSimplifiedWinForms		timerResizeStopped;

		public event EventHandler<EventArgs>		OnResizeStopped;

		public void RaiseOnResizeStopped() {
		    if (this.OnResizeStopped == null) return;
		    this.OnResizeStopped(this, null);
		}

		int ResizeStopped_delayAfterLastResize = 2000;
		void onResizeReceived_rescheduleSerializationTimer() {
		    if (this.timerResizeStopped == null) {
				string timerReason = "CHART_CONTROL_WILL_SERIALIZE_MULTISPLITTERS EMULATING_ResizeStopped()_WITH_DELAY[" + this.ResizeStopped_delayAfterLastResize + "] for: " + this.ToString();
				this.timerResizeStopped = new TimerSimplifiedWinForms(timerReason, this, this.ResizeStopped_delayAfterLastResize);	// not started by default
				this.timerResizeStopped.OnLastScheduleExpired += new EventHandler<EventArgs>(timerResizeStopped_OnLastScheduleExpired);
			}
		    if (this.timerResizeStopped.Scheduled) return;
		    this.timerResizeStopped.ScheduleOnce_postponeIfAlreadyScheduled();	
		}

		void timerResizeStopped_OnLastScheduleExpired(object sender, EventArgs e) {
			this.RaiseOnChartSettingsIndividualChanged_chartManagerShouldSerialize_ChartFormDataSnapshot();
		}

	}
}
