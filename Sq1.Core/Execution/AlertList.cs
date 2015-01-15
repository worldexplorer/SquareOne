using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class AlertList : ConcurrentList<Alert> {
		public Dictionary<int, List<Alert>>	ByBarPlaced		{ get; protected set; }
		
		public Dictionary<int, List<Alert>>	ByBarExpectedFillSafeCopy { get { lock (base.LockObject) {
			Dictionary<int, List<Alert>> ret = new Dictionary<int, List<Alert>>();
			foreach (int bar in this.ByBarPlaced.Keys) ret.Add(bar, new List<Alert>(this.ByBarPlaced[bar]));
			return ret;
		} } }
		public AlertList(string reasonToExist) : base(reasonToExist) {
			ByBarPlaced	= new Dictionary<int, List<Alert>>();
		}
//		public AlertList(string reasonToExist, List<Alert> alerts) : this(reasonToExist) {
//			this.AddRange(alerts);
//		}
		public void AddRange(List<Alert> alerts) {
			foreach (Alert alert in alerts) this.AddNoDupe(alert);
		}
		
		public ByBarDumpStatus AddNoDupe(Alert alert) { lock (base.LockObject) {
			bool newBarAddedInHistory = false;
			//int barIndexAlertStillPending = alert.Bars.Count - 1;
			int barIndexPlaced = alert.PlacedBarIndex;
			if (this.ByBarPlaced.ContainsKey(barIndexPlaced) == false) {
				this.ByBarPlaced.Add(barIndexPlaced, new List<Alert>());
				newBarAddedInHistory = true;
			}
			List<Alert> sameBarAlerts = this.ByBarPlaced[barIndexPlaced];
			if (sameBarAlerts.Contains(alert)) return ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd;
			if (sameBarAlerts.Count > 0) {
				string msg = "appending second StopLossDot to the same bar [" + alert + "]";
			}
			sameBarAlerts.Add(alert);
			base.Add(alert);
			return (newBarAddedInHistory) ? ByBarDumpStatus.OneNewAlertAddedForNewBarInHistory
				: ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory;
		} }
		public AlertList Clone() {
			AlertList ret = new AlertList(this.ReasonToExist + "_CLONE");
			//ret.AddRange(this.InnerList);
			ret.InnerList = this.SafeCopy;
			return ret;
		}
	}
}
