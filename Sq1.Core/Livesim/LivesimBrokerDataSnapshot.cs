using System;

using Sq1.Core.Execution;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerDataSnapshot : IDisposable {
		DataSource			dataSource;
		LivesimDataSource	dataSource_asLivesim_nullUnsafe { get { return this.dataSource as LivesimDataSource; } }

		AlertList			alertsPending { get {
			AlertList ret = new AlertList("ALERTS_PENDING_ORIGINAL_NOT_ACCESSIBLE_YET AVAILABLE_ONLY_UNDER_LIVESIM_NOT_LIVE", null);
			if (this.dataSource_asLivesim_nullUnsafe == null) return ret;
			if (this.dataSource_asLivesim_nullUnsafe.Executor == null) return ret;
			if (this.dataSource_asLivesim_nullUnsafe.Executor.ExecutionDataSnapshot == null) return ret;
			if (this.dataSource_asLivesim_nullUnsafe.Executor.ExecutionDataSnapshot.AlertsPending == null) return ret;
			return this.dataSource_asLivesim_nullUnsafe.Executor.ExecutionDataSnapshot.AlertsPending;
		} }
		public	AlertList			AlertsPending_scheduledForDelayedFill;
		
		//public	AlertList	AlertsNotYetScheduledForDelayedFill { get {
		//	AlertList ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL");
		//	foreach (Alert eachPending in this.alertsPending.InnerList) {
		//		if (this.AlertsScheduledForDelayedFill.InnerList.Contains(eachPending)) continue;
		//		ret.AddNoDupe(eachPending);
		//	}
		//	return ret;
		//} }

		//MOVED_TO_ExecutionDataSnapshot.Alerts_thatQuoteWillFill()
		//public AlertList Alerts_thatQuoteWillFill_forSchedulingDelayedFill(Quote quote) {		//QuoteGenerated quote
		//    BacktestMarketsim marketsim = this.dataSource.BrokerAsBacktest_nullUnsafe.BacktestMarketsim;

		//    AlertList ret = new AlertList("THERE_WERE_NO_ALERTS_PENDING_TO_FILL_ON_EACH_QUOTE", null);
		//    if (this.alertsPending.Count == 0) return ret;

		//    ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL", null);
		//    List<Alert> pendingSafe = this.alertsPending.SafeCopy(this, " //AlertsNotYetScheduledForDelayedFillBy(WAIT)");
		//    foreach (Alert eachPending in pendingSafe) {
		//        if (this.AlertsScheduledForDelayedFill.Contains(eachPending, this, "AlertsNotYetScheduledForDelayedFillBy(WAIT)")) continue;
				
		//        double priceFill = -1;
		//        double slippageFill = -1;
		//        bool filled = marketsim.CheckAlertWillBeFilledByQuote(eachPending, quote, out priceFill, out slippageFill);
		//        if (filled == false) continue;

		//        ret.AddNoDupe(eachPending, this, "Alerts_thatQuoteWillFill_forSchedulingDelayedFill(WAIT)");
		//    }
		//    return ret;
		//}
		
		public LivesimBrokerDataSnapshot(DataSource dataSource_realOrLivesim) {
			string msig = " //LivesimBrokerDataSnapshot(" + dataSource_realOrLivesim + ")";
			if (dataSource_realOrLivesim == null) {
				string msg = "YOU_LOST_MY_POINTER_BACK_TO LivesimDataSource";
				Assembler.PopupException(msg + msig);
			}
			this.dataSource = dataSource_realOrLivesim;	// LAZY_TO_SPLIT_TO_CTOR_AND_INITIALIZE() null for dummies, non-null for clone() { Activator.Create() } 'd LivesimBrokers
			this.AlertsPending_scheduledForDelayedFill = new AlertList("SCHEDULED_FOR_DELAYED_FILL", null);
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.alertsPending	.Dispose();
			//this.livesimDataSource.Executor.ExecutionDataSnapshot.AlertsPending = null;
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
