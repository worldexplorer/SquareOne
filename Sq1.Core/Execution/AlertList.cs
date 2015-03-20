using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class AlertList : ConcurrentList<Alert> {
		public Dictionary<int, List<Alert>>	ByBarPlaced		{ get; protected set; }
		
		public Dictionary<int, List<Alert>>	ByBarPlacedSafeCopy { get { lock (base.LockObject) {
			Dictionary<int, List<Alert>> ret = new Dictionary<int, List<Alert>>();
			foreach (int bar in this.ByBarPlaced.Keys) ret.Add(bar, new List<Alert>(this.ByBarPlaced[bar]));
			return ret;
		} } }
		public AlertList(string reasonToExist, ExecutionDataSnapshot snap = null) : base(reasonToExist, snap) {
			ByBarPlaced	= new Dictionary<int, List<Alert>>();
		}
		public void Clear() { lock(base.LockObject) {
			base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".Clear()");
			lock (base.LockObject) {
				base					.ClearInnerList();
				this.ByBarPlacedSafeCopy.Clear();
			}
		} }
		public void AddRange(List<Alert> alerts) {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".AddRange(" + alerts.Count + ")");
			lock (base.LockObject) {
				foreach (Alert alert in alerts) this.AddNoDupe(alert);
			}
		}
		public ByBarDumpStatus AddNoDupe(Alert alert, bool duplicateThrowsAnError = true) {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".AddNoDupe(" + alert.ToString() + ")");
			lock (base.LockObject) {
				bool newBarAddedInHistory = false;
				bool added = base.AddToInnerList(alert, duplicateThrowsAnError);
				if (added == false) return ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd;

				//int barIndexAlertStillPending = alert.Bars.Count - 1;
				int barIndexPlaced = alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced) == false) {
					this.ByBarPlaced.Add(barIndexPlaced, new List<Alert>());
					newBarAddedInHistory = true;
				}
				List<Alert> slot = this.ByBarPlaced[barIndexPlaced];
				if (slot.Contains(alert)) return ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd;
				if (slot.Count > 0) {
					string msg = "appending second StopLossDot to the same bar [" + alert + "]";
				}
				slot.Add(alert);
				return (newBarAddedInHistory) ? ByBarDumpStatus.OneNewAlertAddedForNewBarInHistory
					: ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory;
			}
		}
		public bool Remove(Alert alert, bool absenseThrowsAnError = true) {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".Remove(" + alert.ToString() + ")");
			lock (base.LockObject) {
				bool removed = base.RemoveFromInnerList(alert, absenseThrowsAnError);
				int barIndexPlaced = alert.PlacedBarIndex;
				if (this.ByBarPlaced.ContainsKey(barIndexPlaced)) {
					List<Alert> slot = this.ByBarPlaced[barIndexPlaced];
					if (slot.Contains(alert)) slot.Remove(alert);
					if (slot.Count == 0) this.ByBarPlaced.Remove(barIndexPlaced);
				}
				return removed;
			}
		}
		public AlertList Clone() {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".Clone()");
			lock (base.LockObject) {
				AlertList ret = new AlertList(this.ReasonToExist + "_CLONE", base.Snap);
				//ret.AddRange(this.InnerList);
				ret.InnerList = base.InnerListSafeCopy;
				ret.ByBarPlaced = this.ByBarPlacedSafeCopy;
				return ret;
			}
		}


		public bool ContainsIdentical(Alert maybeAlready, bool onlyUnfilled = true) { lock(base.LockObject) {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".ContainsIdentical(" + maybeAlready + ", " + onlyUnfilled + ")");
			lock (base.LockObject) {
				foreach (Alert each in this.InnerList) {
					if (maybeAlready.IsIdenticalOrderlessPriceless(each) == false) continue;
					if (onlyUnfilled && each.IsFilled) continue;
					return true;
				}
				return false;
			}
		} }
		public Alert FindSimilarNotSameIdenticalForOrdersPending(Alert alert) { lock(base.LockObject) {
				if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".FindSimilarNotSameIdenticalForOrdersPending(" + alert + ")");
			lock (base.LockObject) {
				Alert similar = null;
				foreach (Alert alertSimilar in this.InnerList) {
					if (alertSimilar == alert) continue;
					if (alertSimilar.IsIdenticalForOrdersPending(alert)) {
						if (similar != null) {
							string msg = "there are 2 or more " + this.ReasonToExist + " Alerts similar to " + alert;
							throw new Exception(msg);
						}
						similar = alertSimilar;
					}
				}
				return similar;
			}
		} }
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

		public bool GuiHasTimeToRebuild { get {
			if (base.Snap != null) base.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".GuiHasTimeToRebuild");
			lock (base.LockObject) {
				bool guiHasTime = false;
				foreach (Alert alert in this.InnerList) {
					guiHasTime = alert.GuiHasTimeRebuildReportersAndExecution;
					if (guiHasTime) break;
				}
				return guiHasTime;
			}
		} }
	}
}
