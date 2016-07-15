using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.Support;

using Sq1.Charting.OnChart;

namespace Sq1.Charting {
	public class ExecutorObjects_FrozenForRendering {
		public Dictionary<string, Indicator>		Indicators_pointerToReflected_neverClearMe				{ get; private set; }

		public Dictionary<int, List<AlertArrow>>	AlertArrowLists_byBar	{ get; private set; }
		public Dictionary<int, AlertList>			AlertsPlaced_byBar		{ get; private set; }
		public Dictionary<int, OrderList>			OrdersKilled_byBar		{ get; private set; }

		public Dictionary<string, OnChartLine>		Lines_byId				{ get; private set; }
		public Dictionary<int, List<OnChartLine>>	Lines_byLeftBar			{ get; private set; }
		public Dictionary<int, List<OnChartLine>>	Lines_byRightBar		{ get; private set; }

		public Dictionary<int, Color>				BarBackgrounds_byBar	{ get; private set; }
		public Dictionary<int, Color>				BarForegrounds_byBar	{ get; private set; }

		public Dictionary<string, OnChartLabel>		OnChartLabels_byId		{ get; private set; }

		public Dictionary<int, SortedDictionary<string, OnChartBarAnnotation>> OnChartBarAnnotations_byBar { get; private set; }

		public Quote								QuoteLast;
		public LevelTwoFrozen						LevelTwo_frozen_forOnePaint;

		// BT_ONSLIDERS_OFF>BT_NOW>SWITCH_SYMBOL=>INDICATOR.OWNVALUES.COUNT=0=>DONT_RENDER_INDICATORS_BUT_RENDER_BARS
		public bool IndicatorsAllHaveNoOwnValues { get {
				bool ret = false;
				if (this.Indicators_pointerToReflected_neverClearMe == null) return ret;
				if (this.Indicators_pointerToReflected_neverClearMe.Count == 0) return ret;
				foreach (Indicator indicator in this.Indicators_pointerToReflected_neverClearMe.Values) {
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
		
		public ExecutorObjects_FrozenForRendering() {
			//Indicators_pointerToReflected_neverClearMe					= new Dictionary<string, Indicator>();

			AlertArrowLists_byBar		= new Dictionary<int, List<AlertArrow>>();
			AlertsPlaced_byBar			= new Dictionary<int, AlertList>();
			OrdersKilled_byBar			= new Dictionary<int, OrderList>();
			
			Lines_byId					= new Dictionary<string, OnChartLine>();
			Lines_byLeftBar				= new Dictionary<int, List<OnChartLine>>();
			Lines_byRightBar			= new Dictionary<int, List<OnChartLine>>();
			
			BarBackgrounds_byBar		= new Dictionary<int, Color>();
			BarForegrounds_byBar		= new Dictionary<int, Color>();

			OnChartLabels_byId			= new Dictionary<string, OnChartLabel>();
			OnChartBarAnnotations_byBar	= new Dictionary<int, SortedDictionary<string, OnChartBarAnnotation>>();		
		}
		public void ClearAll_beforeBacktest() {
			int bitmapsDisposed = 0;
			foreach (List<AlertArrow> eachBarsArrows in this.AlertArrowLists_byBar.Values) {
				foreach (AlertArrow handlerHolder in eachBarsArrows) {
					bool disposed = handlerHolder.DisposeBitmap();
					if (disposed) bitmapsDisposed++;
				}
			}
			string msg = "bitmapsDisposed[" + bitmapsDisposed + "] LEAKED_HANDLERS_HUNTER //ClearAll_beforeBacktest()";
			//Assembler.PopupException(msg, null, false);

			//STUPPIDO_DES_NE???
			//if (this.Indicators_pointerToReflected_neverClearMe.Count > 0) {
			//    this.Indicators_pointerToReflected_neverClearMe.Clear();
			//} else {
			//    string msg2 = "WEIRD__MUST_BE_ONE_SMA_INSIDE_ON_RE-BACKTEST";
			//    Assembler.PopupException(msg2, null, false);
			//}
			//v2
			//foreach (Indicator eachIndicator in this.Indicators.Values) {
			//    if (eachIndicator.OwnValuesCalculated == null) continue;
			//    eachIndicator.OwnValuesCalculated.Clear();		// BT_ONSLIDERS_OFF>BT_NOW>SWITCH_SYMBOL=>INDICATOR.OWNVALUES.COUNT=0=>DONT_RENDER_INDICATORS_BUT_RENDER_BARS
			//    //each.BacktestStartingResetBarsEffectiveProxy();
			//    //each.BacktestStartingConstructOwnValuesValidateParameters();
			//}


			this.AlertArrowLists_byBar	.Clear();
			
			this.AlertsPlaced_byBar		.Clear();
			this.OrdersKilled_byBar		.Clear();
			
			this.Lines_byId				.Clear();
			this.Lines_byLeftBar		.Clear();
			this.Lines_byRightBar		.Clear();
			
			this.BarBackgrounds_byBar	.Clear();
			this.BarForegrounds_byBar	.Clear();

			this.OnChartLabels_byId		.Clear();
			this.OnChartBarAnnotations_byBar.Clear();
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
			if (this.AlertArrowLists_byBar.ContainsKey(barIndexEntry) == false) {
				this.AlertArrowLists_byBar.Add(barIndexEntry, new List<AlertArrow>());
			}
			List<AlertArrow> alertArrowsSameBarEntry = this.AlertArrowLists_byBar[barIndexEntry];
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
				//Assembler.PopupException(msg);
			}
			return mustBeNull == null;
		}
		public void PositionArrowsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.PositionArrowsBacktestAdd(pokeUnit.PositionsOpened.SafeCopy(this, "PositionArrowsRealtimeAdd(WAIT)"));
			this.PositionArrowsBacktestAdd(pokeUnit.PositionsClosed.SafeCopy(this, "PositionArrowsRealtimeAdd(WAIT)"));
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
		public void SetIndicators_pointerToReflected(Dictionary<string, Indicator> indicatorsReflected) {
			if (this.Indicators_pointerToReflected_neverClearMe != null && this.Indicators_pointerToReflected_neverClearMe != indicatorsReflected) {
				string msg = "YOU_MUST_HAVE_RECOMPILED_THE_SCRIPT_FOR_STRATEGY___OR_REPLACED_STRATEGY_FOR_CHART DICTIONARY_REFLECTED_ONLY_CLEARS_WHEN_NEEDED";
				Assembler.PopupException(msg, null, false);
			}
			this.Indicators_pointerToReflected_neverClearMe = indicatorsReflected;
			foreach (Indicator indicator in indicatorsReflected.Values) {
				indicator.DotsDrawnForCurrentSlidingWindow = -1;
			}
		}
		public void AlertsPlacedBacktest_add(Dictionary<int, AlertList> alertsPendingHistorySafeCopy) {
			this.AlertsPlaced_byBar = alertsPendingHistorySafeCopy;
		}
		public void AlertsPlacedRealtime_add(List<Alert> alertsNewPlacedSafeCopy) {
			//string msig = " // PendingRealtimeAdd(" + pokeUnit + ")";
			//string msg = "should I NOT assign this.AlertsPendingHistorySafeCopy=alertsPendingHistorySafeCopy?";
			//Assembler.PopupException(msg + msig);
			//if (null == pokeUnit.QuoteGeneratedThisUnit) {
			//	msg = "NEVER_HAPPENED_SO_FAR pokeUnit.QuoteGeneratedThisUnit=null";
			//	Assembler.PopupException(msg + msig);
			//	return;
			//}
			//if (null == pokeUnit.QuoteGeneratedThisUnit.ParentBarStreaming) {
			//	msg = "NEVER_HAPPENED_SO_FAR pokeUnit.QuoteGeneratedThisUnit.ParentBarStreaming=null";
			//	Assembler.PopupException(msg + msig);
			//	return;
			//}
			//int barIndex = pokeUnit.QuoteGeneratedThisUnit.ParentBarStreaming.ParentBarsIndex;
			//this.AlertsPendingHistorySafeCopy.Add(barIndex, pokeUnit.AlertsNew.SafeCopy(this, "PendingRealtimeAdd(WAIT)"));
			foreach (Alert alert in alertsNewPlacedSafeCopy) {
				int placedBarIndex = alert.PlacedBarIndex;
				if (this.AlertsPlaced_byBar.ContainsKey(placedBarIndex) == false) {
					this.AlertsPlaced_byBar.Add(placedBarIndex, new AlertList("ALERTS_FOR_PlacedBarIndex[" + placedBarIndex + "]", null, null));
				}
				AlertList pendingsForBar = this.AlertsPlaced_byBar[placedBarIndex];
				pendingsForBar.AddNoDupe_byBarsPlaced(alert, this, "//AlertsPlacedRealtimeAdd(WAIT)[" + placedBarIndex + "]", ConcurrentWatchdog.TIMEOUT_DEFAULT, false);
			}
		}
		public void AlertsPending_stillNotFilled_addForBar(int barIndexStillNotFilled, List<Alert> alertsPendingAtCurrentBar_safeCopy) {
			if (this.AlertsPlaced_byBar.ContainsKey(barIndexStillNotFilled) == false) {
				//string msg = "MOST_LIKELY_INVOKED_FROM_CALLBACK_WITH_PREVIOUS_BAR_INDEX MUST_HAVE_BEEN_ADDED_BY_AlertsPlacedRealtimeAdd(): AlertsPlaced_byBar[" + barIndexStillNotFilled + "]";
				//Assembler.PopupException(msg);
				this.AlertsPlaced_byBar.Add(barIndexStillNotFilled, new AlertList("ALERTS_FOR_barIndexStillNotFilled[" + barIndexStillNotFilled + "]", null, null));
			}
			AlertList pendingsForBar = this.AlertsPlaced_byBar[barIndexStillNotFilled];
			pendingsForBar.AddRange(alertsPendingAtCurrentBar_safeCopy, this
									, "//AlertsPendingStillNotFilledForBarAdd(WAIT)[" + barIndexStillNotFilled + "]"
									, ConcurrentWatchdog.TIMEOUT_DEFAULT, false);
		}
		public virtual void OrderKilled_addForBar(int barIndex, Order orderKilled) {
			if (this.OrdersKilled_byBar.ContainsKey(barIndex) == false) {
				//string msg = "MOST_LIKELY_INVOKED_FROM_CALLBACK_WITH_PREVIOUS_BAR_INDEX MUST_HAVE_BEEN_ADDED_BY_OrderKilled_addForBar(): OrdersKilled_byBar[" + barIndex + "]";
				//Assembler.PopupException(msg);
				this.OrdersKilled_byBar.Add(barIndex, new OrderList("ORDERLISTS_FOR_barIndexStillNotFilled[" + barIndex + "]", null, null));
			}
			OrderList victimsForBar = this.OrdersKilled_byBar[barIndex];
			victimsForBar.AddNoDupe(orderKilled, this
									, "//OrderKilled_addForBar(WAIT)[" + barIndex + "]"
									, ConcurrentWatchdog.TIMEOUT_DEFAULT, false);
		}

		public OnChartLine LineAddOrModify(string lineId, int barLeft, double priceLeft, int barRight, double priceRight,
					Color color, int width = 1, bool debugParametersDidntChange = false) {
			
			//LineAdd() candidate starts below
			if (this.Lines_byId.ContainsKey(lineId) == false) {
				OnChartLine lineCreated = new OnChartLine(lineId, barLeft, priceLeft, barRight, priceRight, color, width);
				this.Lines_byId.Add(lineId, lineCreated);
				
				if (this.Lines_byLeftBar.ContainsKey(lineCreated.BarLeft) == false) {
					this.Lines_byLeftBar.Add(lineCreated.BarLeft, new List<OnChartLine>());
				}
				this.Lines_byLeftBar[lineCreated.BarLeft].Add(lineCreated);

				if (this.Lines_byRightBar.ContainsKey(lineCreated.BarRight) == false) {
					this.Lines_byRightBar.Add(lineCreated.BarRight, new List<OnChartLine>());
				}
				this.Lines_byRightBar[lineCreated.BarRight].Add(lineCreated);
				
				return lineCreated;
			}
			
			OnChartLine lineToModify = this.Lines_byId[lineId];
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
				List<OnChartLine> linesByLeftImMovingFrom = this.Lines_byLeftBar[lineToModify.BarLeft];
				if (this.Lines_byLeftBar.ContainsKey(barLeft) == false) {
					this.Lines_byLeftBar.Add(barLeft, new List<OnChartLine>());
				}
				List<OnChartLine> linesByLeftImMovingTo	  = this.Lines_byLeftBar[barLeft];
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
				List<OnChartLine> linesByRightImMovingFrom = this.Lines_byRightBar[lineToModify.BarRight];
				if (this.Lines_byRightBar.ContainsKey(barRight) == false) {
					this.Lines_byRightBar.Add(barRight, new List<OnChartLine>());
				}
				List<OnChartLine> linesByRightImMovingTo   = this.Lines_byRightBar[barRight];
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
			if (this.BarBackgrounds_byBar.ContainsKey(barIndex) == false) {
				this.BarBackgrounds_byBar.Add(barIndex, color);
				return createdFalseModifiedTrue;
			}
			this.BarBackgrounds_byBar[barIndex] = color;
			createdFalseModifiedTrue = true;
			return createdFalseModifiedTrue;
		}
		public Color BarBackgroundGet(int barIndex) {
			Color ret = Color.Empty;
			if (this.BarBackgrounds_byBar.ContainsKey(barIndex)) {
				ret = this.BarBackgrounds_byBar[barIndex];
			}
			return ret;
		}
		public bool BarForegroundSet(int barIndex, Color color) {
			bool createdFalseModifiedTrue = false;
			if (this.BarForegrounds_byBar.ContainsKey(barIndex) == false) {
				this.BarForegrounds_byBar.Add(barIndex, color);
				return createdFalseModifiedTrue;
			}
			this.BarForegrounds_byBar[barIndex] = color;
			createdFalseModifiedTrue = true;
			return createdFalseModifiedTrue;
		}
		public Color BarForegroundGet(int barIndex) {
			Color ret = Color.Empty;
			if (this.BarForegrounds_byBar.ContainsKey(barIndex)) {
				ret = this.BarForegrounds_byBar[barIndex];
			}
			return ret;
		}
		public OnChartLabel ChartLabelAddOrModify(string labelId, string labelText, Font font, Color colorFore, Color colorBack) {
			//Add() candidate starts below
			if (this.OnChartLabels_byId.ContainsKey(labelId) == false) {
				OnChartLabel labelCreated = new OnChartLabel(labelId, labelText, font, colorFore, colorBack);
				this.OnChartLabels_byId.Add(labelId, labelCreated);
				return labelCreated;
			}
			
			//Modify() candidate starts below
			OnChartLabel labelToModify = this.OnChartLabels_byId[labelId];
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
			if (this.OnChartBarAnnotations_byBar.ContainsKey(barIndex) == false) {
				this.OnChartBarAnnotations_byBar.Add(barIndex, new SortedDictionary<string, OnChartBarAnnotation>());
			}
			SortedDictionary<string, OnChartBarAnnotation> annotationsForBar = this.OnChartBarAnnotations_byBar[barIndex];

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