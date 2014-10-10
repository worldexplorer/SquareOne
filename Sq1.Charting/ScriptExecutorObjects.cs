using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sq1.Charting.OnChart;
using Sq1.Core;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public class ScriptExecutorObjects {
		public Dictionary<int, List<AlertArrow>> AlertArrowsListByBar { get; private set; }
		public Dictionary<string, Indicator> Indicators { get; set; }
		public Dictionary<int, List<Alert>> AlertsPendingHistorySafeCopy { get; private set; }

		public Dictionary<string, OnChartLine> LinesById { get; private set; }
		public Dictionary<int, List<OnChartLine>> LinesByLeftBar { get; private set; }
		public Dictionary<int, List<OnChartLine>> LinesByRightBar { get; private set; }

		public Dictionary<int, Color> BarBackgroundsByBar { get; private set; }
		public Dictionary<int, Color> BarForegroundsByBar { get; private set; }

		public Dictionary<string, OnChartLabel> OnChartLabelsById { get; private set; }
		public Dictionary<int, SortedDictionary<string, OnChartBarAnnotation>> OnChartBarAnnotationsByBar { get; private set; }

		// BT_ONSLIDERS_OFF>BT_NOW>SWITCH_SYMBOL=>INDICATOR.OWNVALUES.COUNT=0=>DONT_RENDER_INDICATORS_BUT_RENDER_BARS
		public bool IndicatorsAllHaveNoOwnValues { get {
				bool ret = false;
				if (this.Indicators == null) return ret;
				if (this.Indicators.Count == 0) return ret;
				foreach (Indicator indicator in this.Indicators.Values) {
					if (indicator.OwnValuesCalculated == null) {
						#if DEBUG
						Debugger.Break();
						#endif
						continue;
					}
					if (indicator.OwnValuesCalculated.Count > 0) continue;
					ret = true;
					break;
				}
				return ret;
			} }
		
		public ScriptExecutorObjects() {
			AlertArrowsListByBar = new Dictionary<int, List<AlertArrow>>();
			Indicators = new Dictionary<string, Indicator>();
			AlertsPendingHistorySafeCopy = new Dictionary<int, List<Alert>>();
			
			LinesById = new Dictionary<string, OnChartLine>();
			LinesByLeftBar = new Dictionary<int, List<OnChartLine>>();
			LinesByRightBar = new Dictionary<int, List<OnChartLine>>();
			
			BarBackgroundsByBar = new Dictionary<int, Color>();
			BarForegroundsByBar = new Dictionary<int, Color>();

			OnChartLabelsById = new Dictionary<string, OnChartLabel>();
			OnChartBarAnnotationsByBar = new Dictionary<int, SortedDictionary<string, OnChartBarAnnotation>>();		
		}
		public void ClearAllBeforeBacktest() {
			this.AlertArrowsListByBar.Clear();
			//this.Indicators.Clear();
			foreach (Indicator each in this.Indicators.Values) {
				each.OwnValuesCalculated.Clear();		// BT_ONSLIDERS_OFF>BT_NOW>SWITCH_SYMBOL=>INDICATOR.OWNVALUES.COUNT=0=>DONT_RENDER_INDICATORS_BUT_RENDER_BARS
			}
			
			this.AlertsPendingHistorySafeCopy.Clear();
			
			this.LinesById.Clear();
			this.LinesByLeftBar.Clear();
			this.LinesByRightBar.Clear();
			
			this.BarBackgroundsByBar.Clear();
			this.BarForegroundsByBar.Clear();

			this.OnChartLabelsById.Clear();
			this.OnChartBarAnnotationsByBar.Clear();
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
		
		public OnChartLine LineAddOrModify(string lineId, int barLeft, double priceLeft, int barRight, double priceRight,
					Color color, int width = 1, bool debugParametersDidntChange = false) {
			
			//LineAdd() candidate starts below
			if (this.LinesById.ContainsKey(lineId) == false) {
				OnChartLine lineCreated = new OnChartLine(lineId, barLeft, priceLeft, barRight, priceRight, color, width);
				this.LinesById.Add(lineId, lineCreated);
				
				if (this.LinesByLeftBar.ContainsKey(lineCreated.BarLeft) == false) {
					this.LinesByLeftBar.Add(lineCreated.BarLeft, new List<OnChartLine>());
				}
				this.LinesByLeftBar[lineCreated.BarLeft].Add(lineCreated);

				if (this.LinesByRightBar.ContainsKey(lineCreated.BarRight) == false) {
					this.LinesByRightBar.Add(lineCreated.BarRight, new List<OnChartLine>());
				}
				this.LinesByRightBar[lineCreated.BarRight].Add(lineCreated);
				
				return lineCreated;
			}
			
			OnChartLine lineToModify = this.LinesById[lineId];
			if (		lineToModify.BarLeft	== barLeft	&& lineToModify.PriceLeft	== priceLeft
			   		 && lineToModify.BarRight	== barRight	&& lineToModify.PriceRight	== priceRight
			   		 && lineToModify.Color		== color	&& lineToModify.Width		== width) {
				lineToModify.Status = OnChartObjectOperationStatus.OnChartObjectNotModifiedSinceParametersDidntChange;
				if (debugParametersDidntChange) {
					Assembler.PopupException(lineToModify.ToString() + " //LineAddOrModify()");
				}
				return lineToModify;
			}
			
			//LineModify() candidate starts below
			lineToModify.Status = OnChartObjectOperationStatus.OnChartObjectModified;

			if (lineToModify.PriceLeft != priceLeft) lineToModify.PriceLeft  = priceLeft;
			if (lineToModify.PriceRight != priceRight) lineToModify.PriceRight  = priceRight;
			
			if (lineToModify.BarLeft != barLeft) {
				// lock (chartIsDrawingNow) {} !!! otherwize 2 threads will delete from different LinesByLeftBar[barLeft] 
				List<OnChartLine> linesByLeftImMovingFrom = this.LinesByLeftBar[lineToModify.BarLeft];
				if (this.LinesByLeftBar.ContainsKey(barLeft) == false) {
					this.LinesByLeftBar.Add(barLeft, new List<OnChartLine>());
				}
				List<OnChartLine> linesByLeftImMovingTo	  = this.LinesByLeftBar[barLeft];
				if (linesByLeftImMovingFrom.Contains(lineToModify) == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					string msg = "LINES_BY_LEFT_MUST_CONTAIN_PREVIOUSLY_ADDED_LINE"
						+ " LinesByLeftBar[" + barLeft + "].Count[" + linesByLeftImMovingTo.Count + "]";
					Assembler.PopupException(msg);
				} else {
					linesByLeftImMovingFrom.Remove(lineToModify);
					linesByLeftImMovingTo.Add(lineToModify);
					lineToModify.BarLeft = barLeft;
				}
			}
			if (lineToModify.BarRight != barRight) {
				// lock (chartIsDrawingNow) {} !!! otherwize 2 threads will delete from different LinesByRightBar[barRight]
				List<OnChartLine> linesByRightImMovingFrom = this.LinesByRightBar[lineToModify.BarRight];
				if (this.LinesByRightBar.ContainsKey(barRight) == false) {
					this.LinesByRightBar.Add(barRight, new List<OnChartLine>());
				}
				List<OnChartLine> linesByRightImMovingTo   = this.LinesByRightBar[barRight];
				if (linesByRightImMovingFrom.Contains(lineToModify) == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					string msg = "LINES_BY_RIGHT_MUST_CONTAIN_PREVIOUSLY_ADDED_LINE"
						+ " LinesByRightBar[" + barRight + "].Count[" + linesByRightImMovingTo.Count + "]";
					Assembler.PopupException(msg);
				} else {
					linesByRightImMovingFrom.Remove(lineToModify);
					linesByRightImMovingTo.Add(lineToModify);
					lineToModify.BarRight = barRight;
				}
			}
			return lineToModify;
		}
		public bool BarBackgroundSet(int barIndex, Color color) {
			bool createdFalseModifiedTrue = false;
			if (this.BarBackgroundsByBar.ContainsKey(barIndex) == false) {
				this.BarBackgroundsByBar.Add(barIndex, color);
				return createdFalseModifiedTrue;
			}
			this.BarBackgroundsByBar[barIndex] = color;
			createdFalseModifiedTrue = true;
			return createdFalseModifiedTrue;
		}
		public Color BarBackgroundGet(int barIndex) {
			Color ret = Color.Empty;
			if (this.BarBackgroundsByBar.ContainsKey(barIndex)) {
				ret = this.BarBackgroundsByBar[barIndex];
			}
			return ret;
		}
		public bool BarForegroundSet(int barIndex, Color color) {
			bool createdFalseModifiedTrue = false;
			if (this.BarForegroundsByBar.ContainsKey(barIndex) == false) {
				this.BarForegroundsByBar.Add(barIndex, color);
				return createdFalseModifiedTrue;
			}
			this.BarForegroundsByBar[barIndex] = color;
			createdFalseModifiedTrue = true;
			return createdFalseModifiedTrue;
		}
		public Color BarForegroundGet(int barIndex) {
			Color ret = Color.Empty;
			if (this.BarForegroundsByBar.ContainsKey(barIndex)) {
				ret = this.BarForegroundsByBar[barIndex];
			}
			return ret;
		}
		public OnChartLabel ChartLabelAddOrModify(string labelId, string labelText, Font font, Color colorFore, Color colorBack) {
			//Add() candidate starts below
			if (this.OnChartLabelsById.ContainsKey(labelId) == false) {
				OnChartLabel labelCreated = new OnChartLabel(labelId, labelText, font, colorFore, colorBack);
				this.OnChartLabelsById.Add(labelId, labelCreated);
				return labelCreated;
			}
			
			//Modify() candidate starts below
			OnChartLabel labelToModify = this.OnChartLabelsById[labelId];
			if (		labelToModify.LabelText			== labelText	&& labelToModify.Font				== font
			   		 && labelToModify.ColorForeground	== colorFore	&& labelToModify.ColorBackground	== colorBack) {
				labelToModify.Status = OnChartObjectOperationStatus.OnChartObjectNotModifiedSinceParametersDidntChange;
				Assembler.PopupException(labelToModify.ToString() + " //ChartLabelAddOrModify()");
				return labelToModify;
			}
			
			labelToModify.Status = OnChartObjectOperationStatus.OnChartObjectModified;

			if (labelToModify.LabelText			!= labelText)	labelToModify.LabelText = labelText;
			if (labelToModify.Font				!= font)		labelToModify.Font = font;
			if (labelToModify.ColorForeground	!= colorFore)	labelToModify.ColorForeground = colorFore;
			if (labelToModify.ColorBackground	!= colorBack)	labelToModify.ColorBackground = colorBack;

			return labelToModify;
		}

		public OnChartBarAnnotation BarAnnotationAddOrModify(int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true, 
				int verticalPadding = 5, bool popupParametersDidntChange = false) {
			//Add() candidate starts below
			if (this.OnChartBarAnnotationsByBar.ContainsKey(barIndex) == false) {
				this.OnChartBarAnnotationsByBar.Add(barIndex, new SortedDictionary<string, OnChartBarAnnotation>());
			}
			SortedDictionary<string, OnChartBarAnnotation> annotationsForBar = this.OnChartBarAnnotationsByBar[barIndex];

			if (annotationsForBar.ContainsKey(barAnnotationId) == false) {
				OnChartBarAnnotation barAnnotationCreated = new OnChartBarAnnotation(
					barAnnotationId, barAnnotationText, font, colorForeground, colorBackground,
					aboveBar, verticalPadding, popupParametersDidntChange);
				annotationsForBar.Add(barAnnotationId, barAnnotationCreated);

				return barAnnotationCreated;
			}
			
			//Modify() candidate starts below
			OnChartBarAnnotation barAnnotationToModify = annotationsForBar[barAnnotationId];
			if (	   barAnnotationToModify.BarAnnotationText			== barAnnotationText
					&& barAnnotationToModify.Font						== font
			   		&& barAnnotationToModify.ColorForeground			== colorForeground
					&& barAnnotationToModify.ColorBackground			== colorBackground
					&& barAnnotationToModify.AboveBar					== aboveBar
					&& barAnnotationToModify.VerticalPaddingPx			== verticalPadding
					//NOT_VALUABLE_PARAMETER_TO_HIDE_REPORTING && barAnnotationToModify.ReportDidntChangeStatus	== reportDidntChangeStatus
				) {
				barAnnotationToModify.Status = OnChartObjectOperationStatus.OnChartObjectNotModifiedSinceParametersDidntChange;
				if (barAnnotationToModify.DebugParametersDidntChange) {
					Assembler.PopupException(barAnnotationToModify.ToString() + " //BarAnnotationAddOrModify()");
				}
				return barAnnotationToModify;
			}
			
			barAnnotationToModify.Status = OnChartObjectOperationStatus.OnChartObjectModified;

			if (barAnnotationToModify.BarAnnotationText			!= barAnnotationText)			barAnnotationToModify.BarAnnotationText				= barAnnotationText;
			if (barAnnotationToModify.Font						!= font)						barAnnotationToModify.Font							= font;
			if (barAnnotationToModify.ColorForeground			!= colorForeground)				barAnnotationToModify.ColorForeground				= colorForeground;
			if (barAnnotationToModify.ColorBackground			!= colorBackground)				barAnnotationToModify.ColorBackground				= colorBackground;
			if (barAnnotationToModify.AboveBar					!= aboveBar)					barAnnotationToModify.AboveBar						= aboveBar;
			if (barAnnotationToModify.VerticalPaddingPx			!= verticalPadding)				barAnnotationToModify.VerticalPaddingPx				= verticalPadding;
			if (barAnnotationToModify.DebugParametersDidntChange!= popupParametersDidntChange)	barAnnotationToModify.DebugParametersDidntChange	= popupParametersDidntChange;

			return barAnnotationToModify;
		}
	}
}