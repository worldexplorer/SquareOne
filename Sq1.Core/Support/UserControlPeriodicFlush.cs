using System;
using System.Diagnostics;

using System.Threading;

namespace Sq1.Core.Support {
	public class UserControlPeriodicFlush : UserControlImproved {
		protected Stopwatch					HowLongTreeRebuilds;

		protected TimerSimplifiedWinForms	Timed_flushingToGui;
		//protected TimeredBlockTask		Timed_flushingToGui;
		protected int						Timed_flushingToGui_Delay = 200;

		Action flushMethod_invokedAfter_lastScheduleExpired;

		public UserControlPeriodicFlush() {
			this.HowLongTreeRebuilds	= new Stopwatch();
			this.VisibleChanged += new System.EventHandler(this.exceptionsControl_VisibleChanged);
		}
		void exceptionsControl_VisibleChanged(object sender, EventArgs e) {
			//I_WANTED_TO_CATCH_DockContent.AutoHiddenShownByMouseOver(),AutoHiddenNowDocked_BUT_GOT_WINFORMS_GARBAGE__WILL_USE_IT_THOUGH
			//DockContentImproved parentAsDockContentImproved = this.parentAsDockContentImproved_nullUnsafe;
			//if (parentAsDockContentImproved != null) {
			//	if (parentAsDockContentImproved.IsCoveredOrAutoHidden) return;
			//}
			////if (base.Visible == false) return; 
			string msg = "I_WAS_NOT_FLUSHING_TO_GUI_UNLESS_VISIBLE";
			////STACK_OVERFLOW_CONGRATS Assembler.PopupException(msg, null, false);
			//this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(new Exception(msg));

			//v1 this.flushExceptionsToOLV_switchToGuiThread();
			//v2
			if (this.Timed_flushingToGui == null) return;	// .Initialize() will be invoked later, with Delay deserialized
			this.Timed_flushingToGui.ScheduleOnce_postponeIfAlreadyScheduled();
			//base.Timed_flushingToGui.ScheduleOnce();
		}
		
		protected void Initialize_periodicFlushing(string threadName, Action flushingMethod_invokedFrom_timeredTask, int delay = 200) {
		//protected void Initialize_periodicFlushing(string threadName, int delay = 200) {
			if (Thread.CurrentThread.ManagedThreadId != 1) {
				if (base.InvokeRequired) {
					int a = 1;
				}
				string msg = "INVOKE_ME_FROM_GUI_THREAD";
				//NOPE!!! Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			this.Timed_flushingToGui_Delay = delay;
			//v1
			//this.Timed_flushingToGui = new TimeredBlockTask(threadName, this,
			//	flushingMethod_invokedFrom_timeredTask, this.Timed_flushingToGui_Delay);
			//v2
			this.flushMethod_invokedAfter_lastScheduleExpired = flushingMethod_invokedFrom_timeredTask;
			this.Timed_flushingToGui = new TimerSimplifiedWinForms(threadName, this, this.Timed_flushingToGui_Delay);
			this.Timed_flushingToGui.OnLastScheduleExpired += new EventHandler<EventArgs>(timed_flushingToGui_OnLastScheduleExpired);
			this.Timed_flushingToGui.ScheduleOnce_postponeIfAlreadyScheduled();
		}

		void timed_flushingToGui_OnLastScheduleExpired(object sender, EventArgs e) {
		    if (this.flushMethod_invokedAfter_lastScheduleExpired == null) return;
		    this.flushMethod_invokedAfter_lastScheduleExpired();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.Timed_flushingToGui	!= null) this.Timed_flushingToGui.Dispose();
				if (this.HowLongTreeRebuilds	!= null) this.HowLongTreeRebuilds.Stop();
			}
			base.Dispose(disposing);
		}

		public string FlushingStats { get {
			string ret
				= "   flushed:" + this.HowLongTreeRebuilds.ElapsedMilliseconds + "ms";
			if (this.Timed_flushingToGui == null) return ret;

			ret += "   buffering:" + this.Timed_flushingToGui.DelayMillis + "ms";
			if (this.HowLongTreeRebuilds.ElapsedMilliseconds > this.Timed_flushingToGui.DelayMillis) {
				string msg = "YOU_MAY_NEED_TO_INCREASE_TIMER.Delay_FOR_FLUSHING " + ret;
				// STACK_OVERFLOW_AGAIN Assembler.PopupException(msg);
				//this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(new Exception(msg));
			}
			return ret;
		} }

	}
}