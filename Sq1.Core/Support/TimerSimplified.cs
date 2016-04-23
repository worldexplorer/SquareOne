using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using Sq1.Core;

namespace Sq1.Core.Support {
	public class TimerSimplified : IDisposable {
		System.Windows.Forms.Timer	timer;
				int					forever;
				int					immediately;
				int					tooSoon;
				string				reasonToExist;
				Control				guiInvoker;
		
		public	int					Delay;		// ATOMIC_OPERATION ExceptionsForm may be closed and opened again => Initialize will have to set the TreeRefreshDelayMsec		{ get; private set; }
		public	bool				Scheduled	{ get; private set; }

		public event EventHandler<EventArgs>		OnLastScheduleExpired;

		TimerSimplified() {
			forever			= -1;
			immediately		= 0;
			tooSoon			= 1;
			timer			= new System.Windows.Forms.Timer();
			timer.Tick		+= new EventHandler(timer_expired);
		}

		public TimerSimplified(string reasonToExist_passed, Control guiInvokerPassed, int delayInitial = 200) : this() {
			reasonToExist	= reasonToExist_passed;
			guiInvoker		= guiInvokerPassed;
			Delay			= delayInitial;
		}

		void timer_expired(object sender, EventArgs e) {
			if (this.IsDisposed) return;
			this.Scheduled = false;
			this.timer.Enabled = false;
			this.timer.Stop();		// MAY_NEED_TO_GO_TO_GUI_THREAD otherwize it'll keep running even without rescheduling
			if (this.OnLastScheduleExpired == null) return;
			this.OnLastScheduleExpired(this, null);
		}

		void reschedule() {
			if (this.IsDisposed) return;

			// http://stackoverflow.com/questions/2475435/c-sharp-timer-wont-tick
			// Always stop/start a System.Windows.Forms.Timer on the UI thread, apparently! –  user145426

			//if (this.guiInvoker.InvokeRequired) {
			if (Thread.CurrentThread.ManagedThreadId != 1) {
				if (this.guiInvoker.InvokeRequired == false) {
					string msg = "MOST_LIKELY_I_WILL_FAIL__YOU_MUST_CREATE_ME_IN_GUI_THREAD__WITH_WINDOW_HANDLE_READY";
					#if DEBUG
					Debugger.Break();
					#endif
				}
				if (this.Scheduled) return;
				this.Scheduled = true;		// ignore too frequent without waiting for GUI thread to kick in
				//this.guiInvoker.BeginInvoke((MethodInvoker)this.reschedule);
				this.guiInvoker.BeginInvoke(new MethodInvoker(this.reschedule));
				return;
			}

			this.Scheduled = true;	// if we were invoked from GUI thread

			if (this.timer.Interval != this.Delay) this.timer.Interval = this.Delay;

			// Timer is Enabled until event fired; after that Enabled can be used for a repetitive firing (I don't use repetitive so on every re-use of Start() I set Enabled
			// hint: https://msdn.microsoft.com/en-us/library/system.windows.forms.timer.tick(v=vs.110).aspx
			if (this.timer.Enabled == true) {
				this.timer.Enabled = false;
				this.timer.Stop();
			}
			this.timer.Enabled = true;
			this.timer.Start();
		}

		public void ScheduleOnce_postponeIfAlreadyScheduled() {
			if (this.Scheduled) {
				//this.timer.Enabled = false;
				//this.timer.Stop();
				//this.Scheduled = false;
				//return;
			}
			this.reschedule();
		}

		public override string ToString() {
			return this.reasonToExist;
		}

		public bool IsDisposed	{ get; private set; }
		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				//Assembler.PopupException(msg);
				return;
			}
			this.IsDisposed = true;

			this.timer.Stop();		// are you sure I have to start/stop System.WindowsForm.Timers from GUI thread?
			this.timer.Enabled = false;
			this.timer.Dispose();
			this.timer = null;
		}
	}
}