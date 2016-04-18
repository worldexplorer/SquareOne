using System;
using System.Windows.Forms;
using System.Threading.Tasks;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Charting {
	public partial class ChartControl	{
		//TimeredBlock	timerUnblink;
		//Task			TaskWaitingForTimerExpire_toRevertToSave;



		public event EventHandler<EventArgs>		OnResizeNotReceivedWithin;

		//public void RaiseOnResizeNotReceivedWithin() {
		//    if (this.OnResizeNotReceivedWithin == null) return;
		//    this.OnResizeNotReceivedWithin(this, null);
		//}

		//void onResizeReceived_rescheduleTimer() {
		//    if (this.timerUnblink == null) this.timerUnblink = new TimeredBlock(this, 200);	// not started by default
		//    if (this.TaskWaitingForTimerExpire_toRevertToWhite == null) {
		//        this.TaskWaitingForTimerExpire_toRevertToWhite = new Task(delegate {
		//            string msig = " //TaskWaitingForTimerExpire_toRevertToWhite()";
		//            string threadName = "UNBLINK_FOR_CHART " + base.ToString();
		//            Assembler.SetThreadName(threadName, "SETTING_THREAD_NAME_THREW looks like base.Executor=null");
		//            try {
		//                while(this.timerUnblink.IsDisposed == false) {
		//                    this.timerUnblink.WaitForever_forTimerExpired();
		//                    base.ColorBackground_inDataSourceTree = ChartControl.colorBackgroundWhite;
		//                    this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread_afterTimerExpired);
		//                }
		//            } catch (Exception ex) {
		//                string msg = "LOOP_THREW";
		//                Assembler.PopupException(msg + msig, ex);
		//            }
		//        });
		//        this.TaskWaitingForTimerExpire_toRevertToWhite.Start();
		//    }

		//    if (this.timerUnblink.Scheduled) return;

		//    Color colorize = this.Executor.IsStreamingTriggeringScript
		//        ? ChartControl.colorBackgroundGreen_barsSubscribed_scriptIsTriggering
		//        : ChartControl.colorBackgroundRed_barsSubscribed_scriptNotTriggering;

		//    //v1 if (this.ChartIsSubscribed_toOwnNonNullBars_expensiveForEachQuote_useCtxChartDownstreamSubscribed == false) {
		//    if (this.CtxChart.DownstreamSubscribed == false) {
		//        colorize = ChartControl.colorBackgroundOrange_barsNotSubscribed;
		//    }

		//    base.ColorBackground_inDataSourceTree =  colorize;
		//    this.switchToGui_executeCodeLinkingTwoUnrelatedDlls(refreshDataSourceTree_invokedInGuiThread_afterTimerExpired);
		//    this.timerUnblink.ScheduleOnce();
	
		//}

	}
}
