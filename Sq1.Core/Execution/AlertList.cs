using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class AlertList : ConcurrentList<Alert>, IDisposable {
		protected Dictionary<int, List<Alert>>	ByBarPlaced		{ get; private set; }
		
		public Dictionary<int, AlertList>	ByBarPlacedSafeCopy(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			Dictionary<int, AlertList> ret = new Dictionary<int, AlertList>();
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				foreach (int bar in this.ByBarPlaced.Keys) ret.Add(bar, new AlertList("ByBarPlacedSafeCopy", null, this.ByBarPlaced[bar]));
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public override List<Alert> SafeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			return base.SafeCopy(lockOwner, lockPurpose, waitMillis);
		}
		public List<Alert> SafeCopy_forPanelPrice_lessStringConcat(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			List<Alert> ret = new List<Alert>();
			//TOO_SLOW lockPurpose += " //" + this.ToString() + ".SafeCopy()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				ret = new List<Alert>(base.InnerList);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}
		public AlertList(string reasonToExist, ExecutorDataSnapshot snap = null, List<Alert> copyFrom = null) : this(reasonToExist, snap) {
			if (copyFrom == null) return;
			base.InnerList.AddRange(copyFrom);
		}
		public AlertList(string reasonToExist, ExecutorDataSnapshot snap = null) : base(reasonToExist, snap) {
			ByBarPlaced	= new Dictionary<int, List<Alert>>();
		}
		public void Clear(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".Clear()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				base			.Clear(lockOwner, lockPurpose, waitMillis);
				this.ByBarPlaced.Clear();
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public void DisposeWaitHandles_andClearInnerList(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".DisposeWaitHandlesAndClear()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (Alert alert in base.InnerList) alert.Dispose();
				this.Clear(lockOwner, lockPurpose, waitMillis);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public void AddRange(List<Alert> alerts, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddRange(" + alerts.Count + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (Alert alert in alerts) {
					ByBarDumpStatus added = this.AddNoDupe_byBarsPlaced(alert, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public ByBarDumpStatus AddNoDupe_byBarsPlaced(Alert alert, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddNoDupe(" + alert.ToString() + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				bool newBarAddedInHistory = false;
				bool added = base.AppendUnique(alert, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added == false) return ByBarDumpStatus.BarAlreadyContained_alertYouAdd;

				//int barIndexAlertStillPending = alert.Bars.Count - 1;
				int barIndexPlaced = alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced) == false) {
					this.ByBarPlaced.Add(barIndexPlaced, new List<Alert>());
					newBarAddedInHistory = true;
				}
				List<Alert> slot = this.ByBarPlaced[barIndexPlaced];
				if (slot.Contains(alert)) return ByBarDumpStatus.BarAlreadyContained_alertYouAdd;
				if (slot.Count > 0) {
					string msg = "appending second StopLossDot to the same bar [" + alert + "]";
				}
				slot.Add(alert);
				return (newBarAddedInHistory) ? ByBarDumpStatus.OneNewAlertAdded_forNewBar
					: ByBarDumpStatus.SequentialAlertAdded_forExistingBar;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public bool Remove(Alert alert, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".Remove(" + alert.ToString() + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				bool removed = base.RemoveUnique(alert, owner, lockPurpose, waitMillis, absenseThrowsAnError);
				int barIndexPlaced = alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced)) {
					List<Alert> slot = this.ByBarPlaced[barIndexPlaced];
					if (slot.Contains(alert)) slot.Remove(alert);
					if (slot.Count == 0) this.ByBarPlaced.Remove(barIndexPlaced);
				}
				return removed;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}


		public bool ContainsIdentical(Alert maybeAlready, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool onlyUnfilled = true) {
			lockPurpose += " //" + base.ReasonToExist + ".ContainsIdentical(" + maybeAlready + ", " + onlyUnfilled + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				foreach (Alert each in base.InnerList) {
					if (maybeAlready.IsIdentical_orderlessPriceless(each) == false) continue;
					if (onlyUnfilled && each.IsFilled_fromPosition) continue;
					return true;
				}
				return false;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public Alert FindIdentical_notSame_forOrdersPending(Alert alert, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".FindIdentical_notSame_forOrdersPending(" + alert + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				Alert similar = null;
				foreach (Alert alertSimilar in base.InnerList) {
					if (alertSimilar == alert) continue;
					if (alertSimilar.IsIdentical_forOrdersPending(alert)) {
						if (similar != null) {
							string msg = "there are 2 or more " + this.ReasonToExist + " Alerts similar to " + alert;
							throw new Exception(msg);
						}
						similar = alertSimilar;
					}
				}
				return similar;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		// UNCOMMENT_WHEN_NEEDED
		//public Dictionary<int, List<Alert>> SafeCopyRangeForRenderer(int barIndexLeftVisible, int barIndexRightVisible) { lock(base.LockObject) {
		//	//if (barIndexRightVisible == -1) barIndexRightVisible = this.executor.Bars.Count;
		//	Dictionary<int, List<Alert>> ret = new Dictionary<int, List<Alert>>();
		//	for (int barNo = barIndexLeftVisible; barNo <= barIndexRightVisible; barNo++) {
		//		if (this.ByBarPlaced.ContainsKey(barNo) == false) continue;
		//		ret.Add(barNo, new List<Alert>(this.ByBarPlaced[barNo]));
		//	}
		//	return ret;
		//} }

		//public bool GuiHasTimeToRebuild(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
		//	lockPurpose += " //" + base.ReasonToExist + ".GuiHasTimeToRebuild";
		//	try {
		//		base.WaitAndLockFor(owner, lockPurpose, waitMillis);
		//		bool guiHasTime = false;
		//		foreach (Alert alert in base.InnerList) {
		//			guiHasTime = alert.GuiHasTimeRebuildReportersAndExecution;
		//			if (guiHasTime) break;
		//		}
		//		return guiHasTime;
		//	} finally {
		//		base.UnLockFor(owner, lockPurpose);
		//	}
		//}
		public new AlertList Clone(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + "Clone()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				AlertList ret		= new AlertList("CLONE_" + base.ReasonToExist, base.Snap, base.InnerList);
				//v1 ret.ByBarPlaced		= this.ByBarPlacedSafeCopy(this, "Clone(WAIT)");
				foreach (int bar in this.ByBarPlaced.Keys) {
					ret.ByBarPlaced.Add(bar, new List<Alert>(this.ByBarPlaced[bar]));
				}
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public override string ToString() {
			return base.ToString() + " this.ByBarPlaced.Bars[" + this.ByBarPlaced.Keys.Count + "]";
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.DisposeWaitHandles_andClearInnerList(this, "EXTERNAL_DISPOSE()_CALL");
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }

		internal AlertList Substract_returnClone(AlertList alertsPending_alreadyScheduledForDelayedFill, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + "Substract_returnClone()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				alertsPending_alreadyScheduledForDelayedFill.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				AlertList ret		= new AlertList(base.ReasonToExist + "_MINUS_" + alertsPending_alreadyScheduledForDelayedFill.ReasonToExist, base.Snap);
				foreach(Alert eachMine in base.InnerList) {
					if (alertsPending_alreadyScheduledForDelayedFill.Contains(eachMine, lockOwner, lockPurpose)) continue;
					ret.AddNoDupe_byBarsPlaced(eachMine, lockOwner, lockPurpose);
				}
				return ret;
			} finally {
				alertsPending_alreadyScheduledForDelayedFill.UnLockFor(lockOwner, lockPurpose);
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
	}
}
