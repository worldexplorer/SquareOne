using System;
using System.Diagnostics;

using Sq1.Core;

namespace Sq1.Core.Support {
	public class UserControlPeriodicFlush : UserControlImproved {
		protected Stopwatch			HowLongTreeRebuilds;

		protected TimeredBlockTask	TimedTask_flushingToGui;
		protected int				TimedTask_flushingToGui_Delay = 200;

		public UserControlPeriodicFlush() {}
		
		protected void Initialize_periodicFlushing(string threadName, Action flushingMethod_invokedFrom_timeredTask, int delay = 200) {
			this.HowLongTreeRebuilds	= new Stopwatch();
			this.TimedTask_flushingToGui_Delay = delay;
			this.TimedTask_flushingToGui = new TimeredBlockTask(threadName, this,
				flushingMethod_invokedFrom_timeredTask, this.TimedTask_flushingToGui_Delay);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.TimedTask_flushingToGui	!= null) this.TimedTask_flushingToGui.Dispose();
				if (this.HowLongTreeRebuilds		!= null) this.HowLongTreeRebuilds.Stop();
			}
			base.Dispose(disposing);
		}

		public string FlushingStats { get {
			string ret
				= "   flushed:" + this.HowLongTreeRebuilds.ElapsedMilliseconds + "ms"
				+ "   buffering:" + this.TimedTask_flushingToGui.Delay + "ms";

			if (this.HowLongTreeRebuilds.ElapsedMilliseconds > this.TimedTask_flushingToGui.Delay) {
				string msg = "YOU_MAY_NEED_TO_INCREASE_TIMER.Delay_FOR_FLUSHING " + ret;
				// STACK_OVERFLOW_AGAIN Assembler.PopupException(msg);
				//this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(new Exception(msg));
			}
			return ret;
		} }

	}
}