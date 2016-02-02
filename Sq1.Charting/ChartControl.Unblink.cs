using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets;

namespace Sq1.Charting {
	public partial class ChartControl  {

		static	Color	colorBackgroundWhite										= Color.White;
		static	Color	colorBackgroundOrange_barsNotSubscribed						= Color.LightSalmon;
		static	Color	colorBackgroundRed_barsSubscribed_scriptNotTriggering		= Color.FromArgb(255, 230, 230);
		static	Color	colorBackgroundGreen_barsSubscribed_scriptIsTriggering		= Color.FromArgb(230, 255, 230);

		TimerSimplified timerUnblink;
		Task			TaskWaitingForTimerExpire_toRevertToWhite;
		
		public void OnStrategyExecutedOneQuote_unblinkDataSourceTree(Action refreshDataSourceTree) {
			if (this.timerUnblink == null) this.timerUnblink = new TimerSimplified(this, 200);	// not started by default
			if (this.TaskWaitingForTimerExpire_toRevertToWhite == null) {
				this.TaskWaitingForTimerExpire_toRevertToWhite = new Task(delegate {
					string msig = " //TaskWaitingForTimerExpire_toRevertToWhite()";
					try {
						Thread.CurrentThread.Name = "UNBLINK_FOR_CHART " + base.Executor.StrategyName;
					} catch (Exception ex) {
						string msg = "SETTING_THREAD_NAME_THREW looks like base.Executor=null";
						Assembler.PopupException(msg + msig, ex);
					}
					try {
						while(this.timerUnblink.IsDisposed == false) {
							this.timerUnblink.WaitForever_forTimerExpired();
							base.ColorBackground_inDataSourceTree = ChartControl.colorBackgroundWhite;
							this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree);
						}
					} catch (Exception ex) {
						string msg = "LOOP_THREW";
						Assembler.PopupException(msg + msig, ex);
					}
				});
				this.TaskWaitingForTimerExpire_toRevertToWhite.Start();
			}

			if (this.timerUnblink.Scheduled) return;

			Color colorize = this.Executor.IsStreamingTriggeringScript
				? ChartControl.colorBackgroundGreen_barsSubscribed_scriptIsTriggering
				: ChartControl.colorBackgroundRed_barsSubscribed_scriptNotTriggering;
			//.Scheduled made reliable after .Stop() in timer_expired() if (base.ColorBackground_inDataSourceTree == colorize) return;
				base.ColorBackground_inDataSourceTree =  colorize;
			this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree);
			this.timerUnblink.ScheduleOnce();
		}

		void switchToGui_executeCodeLinkingTwoUnrelatedDlls(Action refreshDataSourceTree) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree); });
				return;
			}
			refreshDataSourceTree();
		}
	}
}