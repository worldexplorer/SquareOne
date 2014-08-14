using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public class ScriptExecutorObjects {
		public Dictionary<int, List<AlertArrow>> AlertArrowsListByBar { get; private set; }
		public Dictionary<string, Indicator> Indicators { get; set; }
		public Dictionary<int, List<Alert>> AlertsPendingHistorySafeCopy { get; private set; }
		
		public ScriptExecutorObjects() {
			AlertArrowsListByBar = new Dictionary<int, List<AlertArrow>>();
			Indicators = new Dictionary<string, Indicator>();
			AlertsPendingHistorySafeCopy = new Dictionary<int, List<Alert>>();
		}
		public void PositionsClearBacktestStarting() {
			this.AlertArrowsListByBar.Clear();
		}
		public void PositionArrowsBacktestAdd(List<Position> positionsMaster) {
			foreach (Position pos in positionsMaster) {
				List<AlertArrow> alertArrowsSameBarEntry = this.alertsForSameBarGetSafe(pos.EntryFilledBarIndex);
				if (this.checkIfAlertDoesntHaveAnArrow(alertArrowsSameBarEntry, pos.EntryAlert)) {
					alertArrowsSameBarEntry.Add(new AlertArrow(pos, true));
				}

				if (pos.ExitAlert == null) continue;
				List<AlertArrow> alertArrowsSameBarExit = this.alertsForSameBarGetSafe(pos.ExitFilledBarIndex);
				// what's the point?... 
				if (this.checkIfAlertDoesntHaveAnArrow(alertArrowsSameBarExit, pos.ExitAlert)) {
					alertArrowsSameBarExit.Add(new AlertArrow(pos, false));
				}
			}
		}
		List<AlertArrow> alertsForSameBarGetSafe(int barIndexEntry) {
			if (this.AlertArrowsListByBar.ContainsKey(barIndexEntry) == false) {
				this.AlertArrowsListByBar.Add(barIndexEntry, new List<AlertArrow>());
			}
			List<AlertArrow> alertArrowsSameBarEntry = this.AlertArrowsListByBar[barIndexEntry];
			return alertArrowsSameBarEntry;
		}
		bool checkIfAlertDoesntHaveAnArrow(List<AlertArrow> alertArrowsSameBar, Alert alert) {
			Alert mustBeNull = null;
			foreach (AlertArrow each in alertArrowsSameBar) {
				Alert alertSameDirection = each.ArrowIsForPositionEntry ? each.Position.EntryAlert : each.Position.ExitAlert;
				if (alertSameDirection != alert) continue;
				mustBeNull = alertSameDirection;
				break;
			}
			if (mustBeNull != null) {
				string msg = "DUPLICATE_ALERTARROW mustBeNull=[" + mustBeNull + "])";
				Assembler.PopupException(msg);
			}
			return mustBeNull == null;
		}
		public void PositionArrowsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.PositionArrowsBacktestAdd(pokeUnit.PositionsOpened);
			this.PositionArrowsBacktestAdd(pokeUnit.PositionsClosed);
//		NO_NEED_TO_CACHE_OPENING_ALERT_BITMAP_FOR_JUST_CLOSED_POSITIONS AlertArrow.Bitmap is dynamic for EntryAlerts until the position is closed;
//			foreach (AlertArrow eachNewClosed in pokeUnit.PositionsClosed) {
//				if (this.AlertArrowsListByBar.ContainsKey[eachNewClosed.EntryBarIndex] == false) {
//					
//				}
//				List<AlertArrow> alertArrowsSameBarEntry = this.AlertArrowsListByBar[eachNewClosed.EntryBarIndex];
//				foreach (AlertArrow eachArrowToResetBitmap in alertArrowsSameBarEntry) {
//					if (eachArrowToResetBitmap.AlertArrow != eachNewClosed) return;
//					eachArrowToResetBitmap.BitmapResetDueToPositionClose;
//				}
//			}
		}
		public void SetIndicators(Dictionary<string, Indicator> indicators) {
			this.Indicators = indicators;
			foreach (Indicator indicator in indicators.Values) {
				indicator.DotsDrawnForCurrentSlidingWindow = -1;
			}
		}
		
		public void PendingHistoryClearBacktestStarting() {
			this.AlertsPendingHistorySafeCopy.Clear();
		}
		public void PendingHistoryBacktestAdd(Dictionary<int, List<Alert>> alertsPendingHistorySafeCopy) {
			this.AlertsPendingHistorySafeCopy = alertsPendingHistorySafeCopy;
		}
		public void PendingRealtimeAdd(ReporterPokeUnit pokeUnit) {
			Debugger.Break();	//should I NOT assign this.AlertsPendingHistorySafeCopy=alertsPendingHistorySafeCopy;?
			if (null == pokeUnit.QuoteGeneratedThisUnit) {
				Debugger.Break();
				return;
			}
			if (null == pokeUnit.QuoteGeneratedThisUnit.ParentStreamingBar) {
				Debugger.Break();
				return;
			}
			int barIndex = pokeUnit.QuoteGeneratedThisUnit.ParentStreamingBar.ParentBarsIndex;
			this.AlertsPendingHistorySafeCopy.Add(barIndex, pokeUnit.AlertsNew);
		}
		
	}
}
