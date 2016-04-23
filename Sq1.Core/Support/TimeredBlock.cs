using System;
using System.Threading;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Core.Support {
	public class TimeredBlock : IDisposable {
		System.Windows.Forms.Timer	timer;
				ManualResetEvent	expiredMre;
				int					forever;
				int					immediately;
				int					tooSoon;
				Control				guiInvoker;
		
				bool				continueSelfScheduling;
		public	bool				SelfReschedule	{
			get { return this.continueSelfScheduling; }
			private set {
				if (this.continueSelfScheduling == value) return;
				if (value == true) {
					this.reschedule();
				} else {
					this.timer.Stop();		// will drop the closest timer_expired
					//this.expired = true;	// let the expired_blocking consumers to unblock and 
				}
				this.continueSelfScheduling = value;
			}
		}
		public	int					Delay;		// ATOMIC_OPERATION ExceptionsForm may be closed and opened again => Initialize will have to set the TreeRefreshDelayMsec		{ get; private set; }
		public	bool				Scheduled	{ get; private set; }

		private bool expired {
			get { return this.expiredMre.WaitOne(this.immediately); }
			set {
				if (value == expired) {
					string msg = "DONT_INVOKE_ME_TWICE__I_DONT_WANNA_SIGNAL_AGAIN_THOSE_WHO_ARE_WAITING__YOU_HAVE_TO_FIX_IT";
					//Assembler.PopupException(msg);
					return;
				}
				if (value == true) this.expiredMre.Set();
				else this.expiredMre.Reset();
			}
		}
		public void WaitForever_forTimerExpired() {
			try {
				bool notPausedTest = this.expiredMre.WaitOne(this.tooSoon);
				if (notPausedTest == true) {
					string msg = "SIGNALLED_TOO_FAST LOOKS_LIKE_TIMER_WASNT_SCHEDULED FIXME_HERE_TO_AVOID_100%_CPU";
					Assembler.PopupException(msg, null, false);
					return;
				}
				bool signalledTrue_expiredFalse = this.expiredMre.WaitOne(this.forever);
			} catch (Exception ex) {
				Assembler.PopupException("DISPOSED_WHILE_WAITING_FOREVER", ex);
			}
			return;
		}

		public TimeredBlock(Control guiInvokerPassed, int delayInitial = 200) {
			expiredMre		= new ManualResetEvent(false);	// true allows to check for .Expired and Schedule() upstack
			SelfReschedule	= false;		// not running by default; start it by SelfReschedule=true (you'll never catch Expired externally) or ScheduleOnce() (you'll catch Expired externally and reschedule again)
			forever			= -1;
			immediately		= 0;
			tooSoon			= 1;
			guiInvoker		= guiInvokerPassed;
			Delay			= delayInitial;
			timer			= new System.Windows.Forms.Timer();
			timer.Tick		+= new EventHandler(timer_expired);
		}

		void timer_expired(object sender, EventArgs e) {
			if (this.IsDisposed) return;
			this.expired = true;	// will set colorBackgroundWhite and Action{olv.RefreshObject}
			this.expired = false;	// will lock waiting thread again until scheduled (2 lines below or using ScheduleOnce())
			this.Scheduled = false;
			this.timer.Stop();		// MAY_NEED_TO_GO_TO_GUI_THREAD otherwize it'll keep running even without rescheduling
			if (this.continueSelfScheduling == false) {
				return;
			}
			this.reschedule();
		}

		void reschedule() {
			if (this.IsDisposed) return;

			// http://stackoverflow.com/questions/2475435/c-sharp-timer-wont-tick
			// Always stop/start a System.Windows.Forms.Timer on the UI thread, apparently! –  user145426

			if (this.guiInvoker.InvokeRequired) {
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
			this.timer.Enabled = true;
			this.timer.Start();
		}

		public bool ScheduleOnce(int changedInterval = -1) {
			if (changedInterval != -1) this.Delay = changedInterval;
			this.reschedule();
			return true;
		}


		public	bool				IsDisposed	{ get; private set; }
		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				//YES_IT_HAPPENS_AND_CAUSES__DELAYS_ON_SHUTDOWN
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.IsDisposed = true;

			this.timer.Stop();		// are you sure I have to start/stop System.WindowsForm.Timers from GUI thread?
			this.timer.Enabled = false;
			this.timer.Dispose();
			this.timer = null;

			this.expiredMre.Dispose();
			this.expiredMre = null;
		}
	}
}