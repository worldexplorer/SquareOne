using System;

using Sq1.Core.Backtesting;
using Sq1.Core.Execution;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public class LivesimBrokerDataSnapshot {
				LivesimDataSource	livesimDataSource;
		private	AlertList			alertsPending { get {
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
			MarketsimBacktest marketsim = this.livesimDataSource.Executor.MarketsimBacktest;

			AlertList ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL", null);
			foreach (Alert eachPending in this.alertsPending.InnerList) {
				if (this.AlertsScheduledForDelayedFill.InnerList.Contains(eachPending)) continue;
				
				double priceFill = -1;
				double slippageFill = -1;
				bool filled = marketsim.CheckAlertWillBeFilledByQuote(eachPending, quote, out priceFill, out slippageFill);
				if (filled == false) continue;

				ret.AddNoDupe(eachPending);
			}
			return ret;
		}
		
		public LivesimBrokerDataSnapshot(LivesimDataSource livesimDataSource) {
			this.livesimDataSource = livesimDataSource;	// LAZY_TO_SPLIT_TO_CTOR_AND_INITIALIZE() null for dummies, non-null for clone() { Activator.Create() } 'd LivesimBrokers
			AlertsScheduledForDelayedFill = new AlertList("SCHEDULED_FOR_DELAYED_FILL", null);
		}
	}
}
