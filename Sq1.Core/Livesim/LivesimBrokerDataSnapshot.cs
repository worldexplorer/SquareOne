using System;
using System.Collections.Generic;

using Sq1.Core.Backtesting;
using Sq1.Core.Execution;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerDataSnapshot : IDisposable {
		LivesimDataSource	livesimDataSource;
		AlertList			alertsPending { get {
			AlertList ret = new AlertList("ALERTS_PENDING_ORIGINAL_NOT_ACCESSIBLE_YET", null);
			if (livesimDataSource == null) return ret;
			if (livesimDataSource.Executor == null) return ret;
			if (livesimDataSource.Executor.ExecutionDataSnapshot == null) return ret;
			if (livesimDataSource.Executor.ExecutionDataSnapshot.AlertsPending == null) return ret;
			return livesimDataSource.Executor.ExecutionDataSnapshot.AlertsPending;
		} }
		public	AlertList			AlertsScheduledForDelayedFill;
		
		//public	AlertList	AlertsNotYetScheduledForDelayedFill { get {
		//	AlertList ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL");
		//	foreach (Alert eachPending in this.alertsPending.InnerList) {
		//		if (this.AlertsScheduledForDelayedFill.InnerList.Contains(eachPending)) continue;
		//		ret.AddNoDupe(eachPending);
		//	}
		//	return ret;
		//} }
		public AlertList AlertsNotYetScheduledForDelayedFillBy(QuoteGenerated quote) {
			BacktestMarketsim marketsim = this.livesimDataSource.BrokerAsBacktest_nullUnsafe.BacktestMarketsim;

			AlertList ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL", null);
			List<Alert> pendingSafe = this.alertsPending.SafeCopy(this, " //AlertsNotYetScheduledForDelayedFillBy(WAIT)");
			foreach (Alert eachPending in pendingSafe) {
				if (this.AlertsScheduledForDelayedFill.Contains(eachPending, this, "AlertsNotYetScheduledForDelayedFillBy(WAIT)")) continue;
				
				double priceFill = -1;
				double slippageFill = -1;
				bool filled = marketsim.CheckAlertWillBeFilledByQuote(eachPending, quote, out priceFill, out slippageFill);
				if (filled == false) continue;

				ret.AddNoDupe(eachPending, this, "AlertsNotYetScheduledForDelayedFillBy(WAIT)");
			}
			return ret;
		}
		
		public LivesimBrokerDataSnapshot(LivesimDataSource livesimDataSource) {
			string msig = " //LivesimBrokerDataSnapshot(" + livesimDataSource + ")";
			if (livesimDataSource == null) {
				string msg = "YOU_LOST_MY_POINTER_BACK_TO LivesimDataSource";
				Assembler.PopupException(msg + msig);
			}
			this.livesimDataSource = livesimDataSource;	// LAZY_TO_SPLIT_TO_CTOR_AND_INITIALIZE() null for dummies, non-null for clone() { Activator.Create() } 'd LivesimBrokers
			this.AlertsScheduledForDelayedFill = new AlertList("SCHEDULED_FOR_DELAYED_FILL", null);
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
