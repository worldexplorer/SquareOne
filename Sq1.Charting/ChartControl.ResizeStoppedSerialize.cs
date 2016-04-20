using System;

using Sq1.Core.Support;

namespace Sq1.Charting {
	public partial class ChartControl	{
		TimerSimplified		timerResizeStopped;

		public event EventHandler<EventArgs>		OnResizeStopped;

		public void RaiseOnResizeStopped() {
		    if (this.OnResizeStopped == null) return;
		    this.OnResizeStopped(this, null);
		}

		void onResizeReceived_rescheduleSerializationTimer() {
		    if (this.timerResizeStopped == null) {
				this.timerResizeStopped = new TimerSimplified(this, 2000);	// not started by default
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
