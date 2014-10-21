using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Execution {
	public class ExecutionDataSnapshot {
		private ScriptExecutor executor;

		public List<Position> PositionsMaster { get; private set; }
		public Dictionary<int, List<Position>> PositionsMasterByEntryBar { get; private set; }
		public Dictionary<int, List<Position>> PositionsMasterByExitBar { get; private set; }
		private object PositionsMasterByEntryBarLock = new object();
		private object PositionsMasterByExitBarLock = new object();

		public int positionSernoAbs { get; private set; }
		public List<Position> PositionsOpenedAfterExec { get; private set; }
		public List<Position> PositionsClosedAfterExec { get; private set; }
		public List<Position> PositionsOpenNow { get; private set; }
		public List<Position> PositionsOpenNowSafeCopy { get { return new List<Position>(this.PositionsOpenNow); } }
		public List<Alert> AlertsPending { get; private set; }
		public List<Alert> AlertsPendingSafeCopy { get { return new List<Alert>(this.AlertsPending); } }

		private object AlertsPendingLock = new object();
		public Dictionary<int, List<Alert>> AlertsPendingHistoryByBar { get; private set; }
		private object AlertsPendingHistoryByBarLock = new object();

		public List<Alert> AlertsMaster { get; private set; }
		public List<Alert> AlertsNewAfterExec { get; private set; }
		public Dictionary<string, Indicator> IndicatorsReflectedScriptInstances { get; set; }

		public ExecutionDataSnapshot(ScriptExecutor strategyExecutor) {
			this.executor = strategyExecutor;

			this.PositionsOpenNow = new List<Position>();
			this.AlertsPending = new List<Alert>();
			this.AlertsPendingHistoryByBar = new Dictionary<int, List<Alert>>();
			this.AlertsMaster = new List<Alert>();
			this.AlertsNewAfterExec = new List<Alert>();
			this.PositionsMaster = new List<Position>();
			this.PositionsMasterByEntryBar = new Dictionary<int, List<Position>>();
			this.PositionsMasterByExitBar = new Dictionary<int, List<Position>>();
			this.positionSernoAbs = 0;
			this.PositionsOpenedAfterExec = new List<Position>();
			this.PositionsClosedAfterExec = new List<Position>();
			this.IndicatorsReflectedScriptInstances = new Dictionary<string, Indicator>();
		}

		public void Initialize() {
			//this.Clear();
			this.PositionsMaster.Clear();
			this.PositionsMasterByEntryBar.Clear();
			this.PositionsMasterByExitBar.Clear();
			this.positionSernoAbs = 0;

			this.PositionsOpenedAfterExec.Clear();
			this.PositionsClosedAfterExec.Clear();
			this.PositionsOpenNow.Clear();

			this.AlertsMaster.Clear();
			this.AlertsNewAfterExec.Clear();
			this.AlertsPending.Clear();
			this.AlertsPendingHistoryByBar.Clear();
		}
		internal void PreExecutionOnNewBarOrNewQuoteClear() {
			if (this.AlertsNewAfterExec.Count > 0) {
				int a = 1;
			}
			this.AlertsNewAfterExec.Clear();
			this.PositionsOpenedAfterExec.Clear();
			this.PositionsClosedAfterExec.Clear();
		}
		internal void PositionsMasterOpenNewAdd(Position positionOpening) {
			if (positionOpening.EntryFilledBarIndex == -1) {
				string msg = "ENTRY_BAR_NEGATIVE_CAN_NOT_STORE_POSITION_IN_PositionsMasterByEntryBar"
					+ " Strategy[" + this.executor.Strategy.ToString() + "] EntryBar=-1 for position[" + positionOpening + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				//throw new Exception(msg);
				this.executor.PopupException(msg);
				return;
			}
			//if (positionOpening.EntryFilledBarIndex < 10) { int a = 1; }
			lock (this.PositionsMasterByEntryBarLock) {
				//if (PositionsMasterContainsIdenticalPosition(position)) return;
				//position.SernoInMasterList = this.PositionsMaster.Count;

				List<Position> sameBarPositions = null;
				if (this.PositionsMasterByEntryBar.ContainsKey(positionOpening.EntryFilledBarIndex) == false) {
					sameBarPositions = new List<Position>();
					this.PositionsMasterByEntryBar.Add(positionOpening.EntryFilledBarIndex, sameBarPositions);
				} else {
					sameBarPositions = this.PositionsMasterByEntryBar[positionOpening.EntryFilledBarIndex];
					//foreach (Position each in sameBarPositions) {
					//	if (position == each) {int a = 1;}
					//	if (position.Equals(each)) { int b = 2; }
					//}
					bool brokerDupe = sameBarPositions.Contains(positionOpening); // see Position.Equals()
					if (brokerDupe) {
						string msg = "positionOpening already added everywhere; create a dupe-filtering! " + positionOpening;
						this.executor.PopupException(msg);
						return;
					}
				}
				positionOpening.SernoAbs = ++this.positionSernoAbs;
				sameBarPositions.Add(positionOpening);

				if (this.PositionsMaster.Contains(positionOpening)) {
					string msg = "position is already in PositionsMaster, won't add a dupe; create a dupe-filtering!";
					this.executor.PopupException(msg);
				} else {
					this.PositionsMaster.Add(positionOpening);
				}

				if (this.PositionsOpenedAfterExec.Contains(positionOpening)) {
					string msg = "position is already in PositionsOpenedAfterExec, won't add a dupe; create a dupe-filtering!";
					this.executor.PopupException(msg);
				} else {
					this.PositionsOpenedAfterExec.Add(positionOpening);
				}

				if (this.PositionsOpenNow.Contains(positionOpening)) {
					string msg = "position is already in PositionsOpenNow, won't add a dupe; create a dupe-filtering!";
					this.executor.PopupException(msg);
				} else {
					this.PositionsOpenNow.Add(positionOpening);
				}
			}
		}
		//public bool PositionsMasterContainsIdenticalPosition(Position maybeAlready) {
		//	foreach (Position each in this.PositionsMaster) {
		//		if (maybeAlready.isIdentical(each)) return true;
		//	}
		//	return false;
		//}
		private object alertsMasterLock = new object();
		public void AlertEnrichedRegister(Alert alert, bool registerInNewAfterExec = false) {
			if (alert.Qty == 0.0) {
				string msg = "alert[" + alert + "].Qty==0; hopefully will be displayed but not executed...";
				throw new Exception(msg);
			}
			if (alert.Strategy.Script == null) {
				string msg = "TODO NYI alert submitted from mni / onChartTrading";
			}
			lock (alertsMasterLock) {
				if (this.AlertsMasterContainsIdentical(alert)) {
					string msg = "AlertsMasterContainsIdentical=>won't add NewPending;"
						+ " 1) broker's order status dupe? 2) are you using CoverAtStop() in your strategy?"
						+ " //" + alert;
					this.executor.PopupException(msg);
					return;
				}
				this.AlertsMaster.Add(alert);
				if (registerInNewAfterExec == true) this.AlertsNewAfterExec.Add(alert);
				this.AlertsPendingAdd(alert);

				ByBarDumpStatus dumped = this.AlertsPendingHistoryByBarAddNoDupe(alert);
				switch (dumped) {
					case ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd:
						string msg1 = "DUPE while adding JUST CREATED??? alert[" + alert + "]";
						throw new Exception(msg1);
						break;
					case ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory:
						string msg2 = "Here is the case when PrototypeActivator changed alert[" + alert + "]";
						break;
				}
			}
		}
		public bool AlertsMasterContainsIdentical(Alert maybeAlready) {
			foreach (Alert each in this.AlertsMaster) {
				if (maybeAlready.IsIdenticalOrderlessPriceless(each) == false) continue;
				if (each.IsFilled) continue;
				return true;
			}
			return false;
		}
		public void MovePositionOpenToClosed(Position positionClosing, bool absenseInPositionsOpenNowIsAnError = true) {
			lock (this.PositionsMasterByExitBarLock) {
				if (this.PositionsMasterByExitBar.ContainsKey(positionClosing.ExitFilledBarIndex) == false) {
					this.PositionsMasterByExitBar.Add(positionClosing.ExitFilledBarIndex, new List<Position>());
				}
				List<Position> sameBarPositions = this.PositionsMasterByExitBar[positionClosing.ExitFilledBarIndex];
				if (sameBarPositions.Contains(positionClosing) == false) {
					sameBarPositions.Add(positionClosing);
				} else {
					// StopLoss fill moved position earlier, now TakeProfit kill is filled => consider it's ok
					if (positionClosing.IsExitFilledWithPrototypedAlert == false) {
						string msg = "CRITICAL: already added positionClosing[" + positionClosing
							+ "] to bar[" + positionClosing.ExitFilledBarIndex + "]";
						this.executor.PopupException(msg);
						return;
					}
				}

				if (this.PositionsClosedAfterExec.Contains(positionClosing) == true) {
					string msg = "PositionsClosedAfterExec already has position[" + positionClosing + "]; double-run? edge between backtest and streaming?";
					this.executor.PopupException(msg);
				} else {
					this.PositionsClosedAfterExec.Add(positionClosing);
				}

				if (this.PositionsOpenNow.Contains(positionClosing) == false) {
					if (absenseInPositionsOpenNowIsAnError == true) {
						string msg = "PositionsOpenNow should have had this position["
							+ positionClosing + "] before I could remove it here; why it wasn't added or who removed it?";
						this.executor.PopupException(msg);
					}
				} else {
					this.PositionsOpenNow.Remove(positionClosing);
				}

			}
		}
		public bool HasPositionOpenNow(Position positionClosing) {
			lock (this.PositionsMasterByExitBarLock) {
				return this.PositionsOpenNow.Contains(positionClosing);
			}
		}
		// replaced to AlertsMasterNewPendingAddEnriched:AlertsPendingHistoryByBarAddNoDupe()
		// but still used in ExecuteOnNewBarOrNewQuote() to dump old pending Stops/Limits
		// if the stop/limit wasn't filled on the prev bar and stays in pending,
		// I want a DOT on the chart for every bar while this alert was still pending/valid/onMarket
		public int DumpPendingAlertsIntoPendingHistoryByBar() {
			int alertsDumpedForStreamingBar = 0;
			//lock (this.AlertsPendingLock) {
				if (this.AlertsPending.Count == 2) {
					string msg = "LOOKS_LIKE_TP_AND_SL; CATCHING WHY SL ISNT SHOWN ON THE CHART - IS IT DUMPED?";
				}
				foreach (Alert alert in this.AlertsPending) {
					ByBarDumpStatus dumped = this.AlertsPendingHistoryByBarAddNoDupe(alert);
					switch (dumped) {
						case ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd:
							string msg1 = "ALERT_PREVIOUSLY_DUMPED_TO_HISTORY; DUPE while adding JUST CREATED alert[" + alert.ToString() + "]";
							//Debugger.Break();
							//throw new Exception(msg1);
							break;
						case ByBarDumpStatus.OneNewAlertAddedForNewBarInHistory:
							alertsDumpedForStreamingBar++;
							break;
						case ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory:
							alertsDumpedForStreamingBar++;
							string msg2 = "Here is the case when PrototypeActivator changed alert[" + alert.ToString() + "]";
							break;
					}
				}
			//}
			return alertsDumpedForStreamingBar;
		}
		public ByBarDumpStatus AlertsPendingHistoryByBarAddNoDupe(Alert alert) {
			int barIndexAlertStillPending = alert.Bars.Count - 1;
			bool newBarAddedInHistory = false;
			//lock (this.AlertsPendingHistoryByBarLock) {
				if (this.AlertsPendingHistoryByBar.ContainsKey(barIndexAlertStillPending) == false) {
					this.AlertsPendingHistoryByBar.Add(barIndexAlertStillPending, new List<Alert>());
					newBarAddedInHistory = true;
				}
				List<Alert> sameBarAlerts = this.AlertsPendingHistoryByBar[barIndexAlertStillPending];
				if (sameBarAlerts.Contains(alert)) return ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd;
				if (sameBarAlerts.Count > 0) {
					string msg = "appending second StopLossDot to the same bar [" + alert + "]";
				}
				sameBarAlerts.Add(alert);
				return (newBarAddedInHistory) ? ByBarDumpStatus.OneNewAlertAddedForNewBarInHistory
					: ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory;
			//}
		}
		public Alert FindSimilarNotSamePendingAlert(Alert alert) {
			Alert similar = null;
			lock (AlertsPendingLock) {
				foreach (Alert alertSimilar in this.AlertsPending) {
					if (alertSimilar == alert) continue;
					if (alertSimilar.IsIdenticalForOrdersPending(alert)) {
						if (similar != null) {
							string msg = "there are 2 or more Pending Alerts similar to " + alert;
							throw new Exception(msg);
						}
						similar = alertSimilar;
					}
				}
			}
			return similar;
		}
		public Dictionary<int, List<Alert>> AlertsPendingHistorySafeCopy {
			get { return this.AlertsPendingHistorySafeCopyForRenderer(0, -1); }
		}
		public Dictionary<int, List<Alert>> AlertsPendingHistorySafeCopyForRenderer(int barNoLeftVisible, int barNoRightVisible) {
			if (barNoRightVisible == -1) barNoRightVisible = this.executor.Bars.Count;
			Dictionary<int, List<Alert>> ret = new Dictionary<int, List<Alert>>();
			lock (this.AlertsPendingHistoryByBarLock) {
				for (int barNo=barNoLeftVisible; barNo<=barNoRightVisible; barNo++) {
					if (this.AlertsPendingHistoryByBar.ContainsKey(barNo) == false) continue;
					ret.Add(barNo, new List<Alert>(this.AlertsPendingHistoryByBar[barNo]));
				}
			}
			return ret;
		}
		public void AlertsPendingAdd(Alert alert) {
			lock (this.AlertsPendingLock) {
				this.AlertsPending.Add(alert);
			}
		}
		public bool AlertsPendingContains(Alert alert) {
			lock (this.AlertsPendingLock) {
				return this.AlertsPending.Contains(alert);
			}
		}
		public bool AlertsPendingRemove(Alert alert) {
			lock (this.AlertsPendingLock) {
				bool removed = this.AlertsPending.Remove(alert);
				if (removed == false) {
					string msg = "you did't remove anything, what you did expect upstack?...";
				}
				return removed;
			}
		}
		public Dictionary<int, List<Position>> PositionsMasterByEntryBarSafeCopy {
			get {
				Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
				lock (this.PositionsMasterByEntryBarLock) {
					foreach (int bar in this.PositionsMasterByEntryBar.Keys) {
						ret.Add(bar, new List<Position>(this.PositionsMasterByEntryBar[bar]));
					}
				}
				return ret;
			}
		}
		public Dictionary<int, List<Position>> PositionsMasterByExitBarSafeCopy {
			get {
				Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
				lock (this.PositionsMasterByExitBarLock) {
					foreach (int bar in this.PositionsMasterByExitBar.Keys) {
						ret.Add(bar, new List<Position>(this.PositionsMasterByExitBar[bar]));
					}
				}
				return ret;
			}
		}
		public List<Position> PositionsOpenedAfterExecSafeCopy {
			get {
				lock (this.PositionsOpenedAfterExec) {
					return new List<Position>(this.PositionsOpenedAfterExec);
				}
			}
		}
		public List<Position> PositionsClosedAfterExecSafeCopy {
			get {
				lock (this.PositionsClosedAfterExec) {
					return new List<Position>(this.PositionsClosedAfterExec);
				}
			}
		}
		public List<Alert> AlertsNewAfterExecSafeCopy {
			get {
				lock (this.AlertsNewAfterExec) {
					return new List<Alert>(this.AlertsNewAfterExec);
				}
			}
		}
	}
}