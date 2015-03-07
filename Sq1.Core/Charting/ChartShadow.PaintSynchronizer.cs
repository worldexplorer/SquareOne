using System;

using System.Threading;

namespace Sq1.Core.Charting {
	public partial class ChartShadow  {
		ManualResetEvent paintAllPanelsComplete = new ManualResetEvent(false);
		
		//protected void RefreshAllPanelsFinishedWaiterReset() {
		//	bool signalledAlready = this.paintAllPanelsComplete.WaitOne(0);
		//	bool resetAlreadyInNonGuiThread = !signalledAlready;
		//	if (resetAlreadyInNonGuiThread) return;
		//	this.paintAllPanelsComplete.Reset();
		//}
		public bool RefreshAllPanelsIsSignalled { get {
			return this.paintAllPanelsComplete.WaitOne(0);
		} }
		protected void RefreshAllPanelsFinishedWaiterNotifyAll() {
			this.paintAllPanelsComplete.Set();
		}
		public void RefreshAllPanelsWaitFinishedSoLivesimCouldGenerateNewQuote(int timeout = -1) {
			bool signalledAlready = this.paintAllPanelsComplete.WaitOne(0);
			if (signalledAlready == true) {
				string msg = "WHO_SIGNALLED_IF_NOT_ME?? NONSENSE_NO_QUOTE_SHOULD_TRIGGER_REPAINT_KOZ_IM_WAITING IM_THE_ONLY_CONSUMER_I_RESET_AND_I_WAIT_SIGNALLED_PAINT_FINISHED";
				//Assembler.PopupException(msg, null, false);
				this.paintAllPanelsComplete.Reset();	// I_WONT_USE_AUTORESET_THANX
				return;
			}
			this.paintAllPanelsComplete.WaitOne(timeout);
			this.paintAllPanelsComplete.Reset();	// I_WONT_USE_AUTORESET_THANX
		}
	}
}