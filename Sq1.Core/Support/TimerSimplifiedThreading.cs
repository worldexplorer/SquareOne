using System;
using System.Threading;
using System.Diagnostics;

using Sq1.Core;

namespace Sq1.Core.Support {
	public class TimerSimplifiedThreading : IDisposable {
				System.Threading.Timer	timer;
				string					reasonToExist;
		
		public	int		DelayMillis;		// ATOMIC_OPERATION ExceptionsForm may be closed and opened again => Initialize will have to set the TreeRefreshDelayMsec		{ get; private set; }
		public	bool	Scheduled	{ get; private set; }

		public event EventHandler<EventArgs>		OnLastScheduleExpired;

		TimerSimplifiedThreading() {
			timer			= new System.Threading.Timer(this.timer_expired);
		}

		public TimerSimplifiedThreading(string reasonToExist_passed, int delayMillis_Initial = 200) : this() {
			reasonToExist	= reasonToExist_passed;
			DelayMillis		= delayMillis_Initial;
		}

		void timer_expired(object state) {
			if (this.IsDisposed) return;
			this.Scheduled = false;
			this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
			Assembler.SetThreadName(this.reasonToExist);
			if (this.OnLastScheduleExpired == null) return;
			try {
				this.OnLastScheduleExpired(this, null);
			} catch (Exception ex) {
				string msg = "REPORTING_FROM_TIMER_THREAD";
				Assembler.PopupException(msg, ex);
			}
		}

		void reschedule() {
			if (this.IsDisposed) return;
			this.Scheduled = true;
			this.timer.Change(this.DelayMillis, System.Threading.Timeout.Infinite);
		}

		public void ScheduleOnce_postponeIfAlreadyScheduled() {
			if (this.Scheduled) {
				//this.timer.Enabled = false;
				//this.timer.Stop();
				//this.Scheduled = false;
				return;
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

			this.Scheduled = false;
			this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
			this.timer.Dispose();
			this.timer = null;
		}
	}
}