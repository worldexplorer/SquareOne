using System;

using Sq1.Core;

namespace Sq1.Core.Support {
	public partial class UserControlImproved {
		// derived Controls can use base.OnResizeStopped += delegate{this.dataSnapshotSerializer.Serialize();};

		public	int					ResizeStopped_delayAfterLastResize = 2200;	// 2000 was set for MultiSplitters
				TimerSimplified		timerResizeStopped;

		public event EventHandler<EventArgs>		ResizeStopped;
		public void RaiseResizeStopped() {
		    if (this.ResizeStopped == null) return;
		    this.ResizeStopped(this, null);
		}

		void onResizeReceived_rescheduleSerializationTimer() {
		    if (this.timerResizeStopped == null) {
				this.timerResizeStopped = new TimerSimplified(this, this.ResizeStopped_delayAfterLastResize);	// not started by default
				this.timerResizeStopped.OnLastScheduleExpired += new EventHandler<EventArgs>(this.timerResizeStopped_OnLastScheduleExpired);
			}
		    if (this.timerResizeStopped.Scheduled) return;
		    this.timerResizeStopped.ScheduleOnce_postponeIfAlreadyScheduled();	
		}

		void timerResizeStopped_OnLastScheduleExpired(object sender, EventArgs e) {
			this.RaiseResizeStopped();
		}
		
		//protected override void OnResize(EventArgs e) {
		//	base.OnResize(e);
		void userControlImproved_Resize(object sender, EventArgs e) {
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return;
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			this.onResizeReceived_rescheduleSerializationTimer();
		}
	}
}