using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Execution {
	public class ExecutionDataSnapshot {
			   ScriptExecutor	executor;
			   object			alertsMasterLock;
			   object			positionsMasterLock;
		
		public AlertList						AlertsMaster				{ get; private set; }
		public AlertList						AlertsNewAfterExec			{ get; private set; }
		public AlertList						AlertsPending				{ get; private set; }
		//public Dictionary<int, List<Alert>>		AlertsPendingHistorySafeCopy { get { return this.AlertsPendingHistorySafeCopyForRenderer(0, -1); } }

		public int								positionSernoAbs			{ get; private set; }
		public PositionList						PositionsMaster				{ get; private set; }
		public PositionList						PositionsOpenedAfterExec	{ get; private set; }
		public PositionList						PositionsClosedAfterExec	{ get; private set; }
		public PositionList						PositionsOpenNow			{ get; private set; }

		public Dictionary<string, Indicator>	IndicatorsReflectedScriptInstances;

		public ExecutionDataSnapshot(ScriptExecutor strategyExecutor) {
			this.executor = strategyExecutor;
			alertsMasterLock = new object();
			positionsMasterLock = new object();
			AlertsPending					= new AlertList("AlertsPending");
			AlertsMaster					= new AlertList("AlertsMaster");
			AlertsNewAfterExec				= new AlertList("AlertsNewAfterExec");
			positionSernoAbs				= 0;
			PositionsMaster					= new PositionList("PositionsMaster");
			PositionsOpenNow				= new PositionList("PositionsOpenNow");
			PositionsOpenedAfterExec		= new PositionList("PositionsOpenedAfterExec");
			PositionsClosedAfterExec		= new PositionList("PositionsClosedAfterExec");
			IndicatorsReflectedScriptInstances = new Dictionary<string, Indicator>();
		}

		public void Initialize() {
			this.AlertsMaster				.Clear();
			this.AlertsNewAfterExec			.Clear();
			this.AlertsPending				.Clear();
			this.positionSernoAbs			= 0;
			this.PositionsMaster			.Clear();
			this.PositionsOpenedAfterExec	.Clear();
			this.PositionsClosedAfterExec	.Clear();
			this.PositionsOpenNow			.Clear();
		}
		internal void PreExecutionOnNewBarOrNewQuoteClear() {
			this.AlertsNewAfterExec.Clear();
			this.PositionsOpenedAfterExec.Clear();
			this.PositionsClosedAfterExec.Clear();
		}
		internal void PositionsMasterOpenNewAdd(Position positionOpening) {
			if (positionOpening.EntryFilledBarIndex == -1) {
				string msg = "ENTRY_BAR_NEGATIVE_CAN_NOT_STORE_POSITION_IN_PositionsMaster.ByEntryBarFilled"
					+ " Strategy[" + this.executor.Strategy.ToString() + "] EntryBar=-1 for position[" + positionOpening + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				//throw new Exception(msg);
				this.executor.PopupException(msg);
				return;
			}
			//lock (this.PositionsMaster.ByEntryBarFilledLock) {
				//if (PositionsMasterContainsIdenticalPosition(position)) return;
				//position.SernoInMasterList = this.PositionsMaster.Count;

//				List<Position> sameBarPositions = null;
//				if (this.PositionsMaster.ByEntryBarFilled.ContainsKey(positionOpening.EntryFilledBarIndex) == false) {
//					sameBarPositions = new List<Position>();
//					this.PositionsMaster.ByEntryBarFilled.Add(positionOpening.EntryFilledBarIndex, sameBarPositions);
//				} else {
//					sameBarPositions = this.PositionsMaster.ByEntryBarFilled[positionOpening.EntryFilledBarIndex];
//					//foreach (Position each in sameBarPositions) {
//					//	if (position == each) {int a = 1;}
//					//	if (position.Equals(each)) { int b = 2; }
//					//}
//					bool brokerDupe = sameBarPositions.Contains(positionOpening); // see Position.Equals()
//					if (brokerDupe) {
//						string msg = "positionOpening already added everywhere; create a dupe-filtering! " + positionOpening;
//						this.executor.PopupException(msg);
//						return;
//					}
//				}
//				sameBarPositions.Add(positionOpening);
				
				this.PositionsMaster.AddOpening_step1of2(positionOpening);
				positionOpening.SernoAbs = ++this.positionSernoAbs;

				this.PositionsOpenedAfterExec.AddOpening_step1of2(positionOpening);
				this.PositionsOpenNow.AddOpening_step1of2(positionOpening);

//				if (this.PositionsMaster.Contains(positionOpening)) {
//					string msg = "position is already in PositionsMaster, won't add a dupe; create a dupe-filtering!";
//					this.executor.PopupException(msg);
//				} else {
//					this.PositionsMaster.Add(positionOpening);
//				}
//
//				if (this.PositionsOpenedAfterExec.Contains(positionOpening)) {
//					string msg = "position is already in PositionsOpenedAfterExec, won't add a dupe; create a dupe-filtering!";
//					this.executor.PopupException(msg);
//				} else {
//					this.PositionsOpenedAfterExec.Add(positionOpening);
//				}
//
//				if (this.PositionsOpenNow.Contains(positionOpening)) {
//					string msg = "position is already in PositionsOpenNow, won't add a dupe; create a dupe-filtering!";
//					this.executor.PopupException(msg);
//				} else {
//					this.PositionsOpenNow.Add(positionOpening);
//				}
			//}
		}
		public void AlertEnrichedRegister(Alert alert, bool registerInNewAfterExec = false) {
			if (alert.Qty == 0.0) {
				string msg = "alert[" + alert + "].Qty==0; hopefully will be displayed but not executed...";
				throw new Exception(msg);
			}
			if (alert.Strategy.Script == null) {
				string msg = "TODO NYI alert submitted from mni / onChartTrading";
			}
			lock (this.alertsMasterLock) {
				if (this.AlertsMasterContainsIdentical(alert)) {
					string msg = "AlertsMasterContainsIdentical=>won't add NewPending;"
						+ " 1) broker's order status dupe? 2) are you using CoverAtStop() in your strategy?"
						+ " //" + alert;
					this.executor.PopupException(msg);
					return;
				}
				this.AlertsMaster.AddNoDupe(alert);
				if (registerInNewAfterExec == true) this.AlertsNewAfterExec.AddNoDupe(alert);
				ByBarDumpStatus dumped = this.AlertsPending.AddNoDupe(alert);
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
			foreach (Alert each in this.AlertsMaster.InnerList) {
				if (maybeAlready.IsIdenticalOrderlessPriceless(each) == false) continue;
				if (each.IsFilled) continue;
				return true;
			}
			return false;
		}
		public void MovePositionOpenToClosed(Position positionClosing, bool absenseInPositionsOpenNowIsAnError = true) {
			lock (this.positionsMasterLock) {
				this.PositionsMaster.AddToClosedDictionary_step2of2(positionClosing);
				this.PositionsClosedAfterExec.AddClosed(positionClosing);
				this.PositionsOpenNow.Remove(positionClosing);
			}
		}
		// replaced to AlertsMasterNewPendingAddEnriched:AlertsPending.ByBarExpectedFillAddNoDupe()
		// but still used in ExecuteOnNewBarOrNewQuote() to dump old pending Stops/Limits
		// if the stop/limit wasn't filled on the prev bar and stays in pending,
		// I want a DOT on the chart for every bar while this alert was still pending/valid/onMarket
//		public int DumpPendingAlertsIntoPendingHistoryByBar() {
//			int alertsDumpedForStreamingBar = 0;
//			//lock (this.AlertsPendingLock) {
//				if (this.AlertsPending.Count == 2) {
//					string msg = "LOOKS_LIKE_TP_AND_SL; CATCHING WHY SL ISNT SHOWN ON THE CHART - IS IT DUMPED?";
//				}
//				foreach (Alert alert in this.AlertsPending.InnerList) {
//					ByBarDumpStatus dumped = this.AlertsPending.AddNoDupe(alert);
//					switch (dumped) {
//						case ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd:
//							string msg1 = "ALERT_PREVIOUSLY_DUMPED_TO_HISTORY; DUPE while adding JUST CREATED alert[" + alert.ToString() + "]";
//							//Debugger.Break();
//							//throw new Exception(msg1);
//							break;
//						case ByBarDumpStatus.OneNewAlertAddedForNewBarInHistory:
//							alertsDumpedForStreamingBar++;
//							break;
//						case ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory:
//							alertsDumpedForStreamingBar++;
//							string msg2 = "Here is the case when PrototypeActivator changed alert[" + alert.ToString() + "]";
//							break;
//					}
//				}
//			//}
//			return alertsDumpedForStreamingBar;
//		}
		public Alert FindSimilarNotSamePendingAlert(Alert alert) { lock (this.alertsMasterLock) {
				Alert similar = null;
				foreach (Alert alertSimilar in this.AlertsPending.InnerList) {
					if (alertSimilar == alert) continue;
					if (alertSimilar.IsIdenticalForOrdersPending(alert)) {
						if (similar != null) {
							string msg = "there are 2 or more Pending Alerts similar to " + alert;
							throw new Exception(msg);
						}
						similar = alertSimilar;
					}
				}
				return similar;
			} }
		public Dictionary<int, List<Alert>> AlertsPendingHistorySafeCopyForRenderer(int barNoLeftVisible, int barNoRightVisible) {
			if (barNoRightVisible == -1) barNoRightVisible = this.executor.Bars.Count;
			Dictionary<int, List<Alert>> ret = new Dictionary<int, List<Alert>>();
			lock (this.alertsMasterLock) {
				for (int barNo=barNoLeftVisible; barNo<=barNoRightVisible; barNo++) {
					if (this.AlertsPending.ByBarPlaced.ContainsKey(barNo) == false) continue;
					ret.Add(barNo, new List<Alert>(this.AlertsPending.ByBarPlaced[barNo]));
				}
			}
			return ret;
		}
	}
}