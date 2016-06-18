using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Support;

namespace Sq1.Charting {
	public partial class ChartControl  {

		static	Color	colorBackgroundWhite										= Color.White;
		static	Color	colorBackgroundOrange_barsNotSubscribed						= Color.LightSalmon;
		static	Color	colorBackgroundRed_barsSubscribed_scriptNotTriggering		= Color.FromArgb(255, 230, 230);
		static	Color	colorBackgroundGreen_barsSubscribed_scriptIsTriggering		= Color.FromArgb(230, 255, 230);

		TimerSimplifiedWinForms	timerUnblink;
		
		public void OnStrategyExecutedOneQuote_unblinkDataSourceTree(Action refreshDataSourceTree_invokedInGuiThread_afterTimerExpired) {
			if (this.timerUnblink == null) {
				this.timerUnblink = new TimerSimplifiedWinForms("timerUnblink_revertToWhite_afterDelay", this, 200);	// not started by default
				this.timerUnblink.OnLastScheduleExpired += delegate(object sender, EventArgs e) {
					string msig = " //TimerUnblinkExpired_revertingToWhite()";
					try {
						if (this.timerUnblink.IsDisposed) {
							string msg = "timerUnblink.IsDisposed INSIDE_ITS_OWN .OnLastScheduleExpired - EVENT_SHOULD_NOT_HAVE_BEEN_INVOKED";
							Assembler.PopupException(msg);
							return;
						}
						base.ColorBackground_inDataSourceTree = ChartControl.colorBackgroundWhite;
						this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread_afterTimerExpired);
					} catch (Exception ex) {
						string msg = "LOOP_THREW";
						Assembler.PopupException(msg + msig, ex);
					}
				};
			}

			if (this.timerUnblink.Scheduled) return;

			Color colorize = this.Executor.IsStreamingTriggeringScript
				? ChartControl.colorBackgroundGreen_barsSubscribed_scriptIsTriggering
				: ChartControl.colorBackgroundRed_barsSubscribed_scriptNotTriggering;

			if (this.CtxChart.DownstreamSubscribed == false) {
				colorize = ChartControl.colorBackgroundOrange_barsNotSubscribed;
			}

			base.ColorBackground_inDataSourceTree =  colorize;
			this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread_afterTimerExpired);
			this.timerUnblink.ScheduleOnce_dontPostponeIfAlreadyScheduled();
		}

		void switchToGui_executeCodeLinkingTwoUnrelatedDlls(Action refreshDataSourceTree_invokedInGuiThread) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread); });
				return;
			}
			refreshDataSourceTree_invokedInGuiThread();
		}

	}
}