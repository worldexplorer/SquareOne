using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using Sq1.Core;

namespace Sq1.Widgets {
	public class TimerSimplifiedTask {
		string				reasonToExist;
		Control				guiInvoker;
		TimerSimplified		timerUnblink;
		Task				taskWaitingForTimerExpire;
		Action				actionOnTimerExpired;

		public TimerSimplifiedTask(string reasonToExist_passed, Control guiInvoker_passed, Action invokedInGuiThread_afterTimerExpired_passed) {
			this.reasonToExist = reasonToExist_passed;
			this.guiInvoker = guiInvoker_passed;
			this.actionOnTimerExpired = invokedInGuiThread_afterTimerExpired_passed;

			this.timerUnblink = new TimerSimplified(guiInvoker);

			this.taskWaitingForTimerExpire = new Task(delegate {
				string msig = " //TaskWaitingForTimerExpire_toRevertToWhite()";
				try {
					Thread.CurrentThread.Name = this.reasonToExist;
				} catch (Exception ex) {
					string msg = "SETTING_THREAD_NAME_THREW looks like base.Executor=null";
					Assembler.PopupException(msg + msig, ex);
				}
				try {
					while (this.timerUnblink.IsDisposed == false) {
						this.timerUnblink.WaitForever_forTimerExpired();
						this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(this.actionOnTimerExpired);
					}
				} catch (Exception ex) {
					string msg = "LOOP_THREW //TimerSimplifiedTask.taskWaitingForTimerExpire delegate";
					Assembler.PopupException(msg + msig, ex);
				}
			});
		}

		void switchToGui_executeCodeLinkingTwoUnrelatedDlls(Action refreshDataSourceTree_invokedInGuiThread) {
			if (guiInvoker.InvokeRequired) {
				guiInvoker.BeginInvoke((MethodInvoker)delegate { switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread); });
				return;
			}
			refreshDataSourceTree_invokedInGuiThread();
		}

		public void Start() {
			this.taskWaitingForTimerExpire.Start();
		}
		public bool Scheduled {
			get { return this.timerUnblink.Scheduled; }
		}
		public void ScheduleOnce() {
			this.timerUnblink.ScheduleOnce();
		}
		public void Dispose() {
			if (this.taskWaitingForTimerExpire.IsCompleted == true) {
				this.taskWaitingForTimerExpire.Dispose();
			}
			this.timerUnblink.Dispose();
		}
		public int Delay {
			get { return this.timerUnblink.Delay; }
			set { this.timerUnblink.Delay = value; }
		}
	}
}
