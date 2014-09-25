using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Core.Indicators {
	public partial class Indicator {
		// DrawValue() isn't in Sq1.Charting.PanelNamedFolding so that one particular indicator may override and draw more than one line with labels;  
//		public bool DrawValueEntryPoint(Graphics g, Bar bar, Rectangle barPlaceholder) {
		public bool DrawValueEntryPoint(Graphics g, Bar bar) {
			bool indicatorLegDrawn = false;
			// MOVED_UPSTACK if (bar.ParentBarsIndex <= this.FirstValidBarIndex) return indicatorLegDrawn;
			if (Object.ReferenceEquals(this.OwnValuesCalculated, null)) return indicatorLegDrawn;
			string msig = " Indicator[" + this.NameWithParameters + "].DrawValueEntryPoint(" + bar + ")";
			//v1
			//if (this.OwnValuesCalculated.Count < bar.ParentBarsIndex) {
			//    string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR bar[" + bar + "]";
			//    //Assembler.PopupException(msg + msig);
			//    return indicatorLegDrawn;
			//}
			//double calculated = this.OwnValuesCalculated[bar.ParentBarsIndex];
			// v2-BEGIN
			// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
			//if (null == this.NotOnChartBarScaleInterval && bar.ParentBarsIndex < this.FirstValidBarIndex) {
			//    return indicatorLegDrawn;	// INVALID FOR INDICATOR BASED ON NON_CHART_BARS_SCALE_INTERVAL
			//}

			if (bar.IsBarStaticLast && this.Executor.IsStreaming == false) {
				string msg = "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED";
				//Assembler.PopupException(msg);
				return indicatorLegDrawn;
			}
			if (this.OwnValuesCalculated.ContainsDate(bar.DateTimeOpen) == false) {
				if (this.Executor.Backtester.IsBacktestingNow) {
					return indicatorLegDrawn;
				}
				//string msg = "EDIT_DATASOURCE_EXTEND_MARKET_OPEN_CLOSE_HOURS";
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR bar[" + bar.DateTimeOpen + "]";
				// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
				//if (this.OwnValuesCalculated.ScaleInterval == null) {
				//    var scaleIntervalAutoInit = this.NotOnChartBarsKey;
				//}
				//if (this.OwnValuesCalculated.ScaleInterval != bar.ParentBars.ScaleInterval) {
				//    msg += " OwnValuesCalculated.ScaleInterval[" + this.OwnValuesCalculated.ScaleInterval + "] != bar.ParentBars.ScaleInterval[" + bar.ParentBars.ScaleInterval + "]";
				//}
				#if DEBUG
				//Debugger.Break();
				#endif
				Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
//			return this.DrawValueIndicatorSpecific(g, bar, barPlaceholder);
			return this.DrawValueIndicatorSpecific(g, bar);
		}
		protected virtual bool DrawValueIndicatorSpecific(Graphics g, Bar bar) {
			return this.DrawValueSingleLine(g, bar);
		}
		protected bool DrawValueSingleLine(Graphics g, Bar bar, double calculated = double.NaN, double calculatedPrev = double.NaN) {
			string msig = " Indicator[" + this.NameWithParameters + "].DrawValueSingleLine(" + bar + ")";
			bool indicatorLegDrawn = false;
			if (calculated == double.NaN) {
				calculated = this.OwnValuesCalculated[bar.ParentBarsIndex];
			}
			// v2-END
			if (double.IsNaN(calculated)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_BAR bar[" + bar + "]";
				//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			this.DotsExistsForCurrentSlidingWindow++;
			int x = this.HostPanelForIndicator.BarToX(bar.ParentBarsIndex) + this.HostPanelForIndicator.BarShadowOffset;
			int y = this.HostPanelForIndicator.ValueToYinverted(calculated);
			if (y == 0)  {
				string msg = "INDICATOR_VALUE_TOO_BIG_INVERTED SKIPPING_DRAWING_OUTSIDE_HOST_PANEL";
				#if DEBUG
				//Debugger.Break();
				#endif
				//return indicatorLegDrawn;
			}
			int max = this.HostPanelForIndicator.ValueToYinverted(0);
			if (y == max) {
				string msg = "INDICATOR_VALUE_TOO_SMALL_INVERTED SKIPPING_DRAWING_OUTSIDE_HOST_PANEL";
				#if DEBUG
				//Debugger.Break();
				#endif
				//return indicatorLegDrawn;
			}

			Point myDot = new Point(x, y);
			
			int barIndexPrev = bar.ParentBarsIndex - 1;
			Bar barPrev = bar.ParentBars[barIndexPrev];
			if (barIndexPrev < 0 || barIndexPrev > this.OwnValuesCalculated.Count - 1) {
				//string msg = "EDIT_DATASOURCE_EXTEND_MARKET_OPEN_CLOSE_HOURS";
				string msg = "CAN_NOT_DRAW_INDICATOR_CANT_TAKE_VALUE_PREVIOUS_BAR_BEOYND_AVAILABLE barIndexPrev[" + barIndexPrev + "] OwnValuesCalculated.Count[" + this.OwnValuesCalculated.Count + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			
			// EPIC_FAIL_HERE !!!! double calculatedPrev = this.OwnValuesCalculated[barIndexPrev];
			if (this.OwnValuesCalculated.ContainsDate(barPrev.DateTimeOpen) == false) {
				//string msg = "EDIT_DATASOURCE_EXTEND_MARKET_OPEN_CLOSE_HOURS";
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_PREVIOUS_BAR[" + barPrev.DateTimeOpen + "] " + this.ToString();
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			
			if (double.IsNaN(calculatedPrev)) {
				calculatedPrev = this.OwnValuesCalculated[barIndexPrev];
			}
			
			if (double.IsNaN(calculatedPrev)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_PREVBAR barIndexPrev[" + barIndexPrev + "]";
				//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
				
			int yPrev = this.HostPanelForIndicator.ValueToYinverted(calculatedPrev);
			int xPrev = this.HostPanelForIndicator.BarToX(barIndexPrev) + this.HostPanelForIndicator.BarShadowOffset;
			Point myDotPrev = new Point(xPrev, yPrev);
			
			g.DrawLine(this.PenForeground, myDot, myDotPrev);
			this.DotsDrawnForCurrentSlidingWindow++;
			
			indicatorLegDrawn = true;
			return indicatorLegDrawn;
		}
		protected bool DrawValueBand(Graphics g, Bar bar, DataSeriesTimeBasedColorified bandLower, DataSeriesTimeBasedColorified bandUpper) {
			string msig = " Indicator[" + this.NameWithParameters + "].DrawValueBand(" + bar + ")";
			bool indicatorLegDrawn = false;

			double calculated = this.OwnValuesCalculated[bar.ParentBarsIndex];
			if (double.IsNaN(calculated)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_BAR bar[" + bar + "]";
				//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}

			
			int barIndex = bar.ParentBarsIndex;
			
			if (bandLower.Count < 2) {
				return indicatorLegDrawn;
			}
			int differenceMustBeNegative = barIndex - bandLower.Count + 2;
			if (differenceMustBeNegative >= 0) {
				string msg = "INDICATOR_DIDNT_CALCULATE_YET_BAND_LOWER[" + differenceMustBeNegative + "]";
				#if DEBUG
				//Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			
			if (bandUpper.Count < 2) {
				return indicatorLegDrawn;
			}
			differenceMustBeNegative = barIndex - bandUpper.Count + 2;
			if (differenceMustBeNegative >= 0) {
				string msg = "INDICATOR_DIDNT_CALCULATE_YET_BAND_UPPER[" + differenceMustBeNegative + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			
			
			try {
				double valueLower = bandLower[barIndex];
				double  prevLower = bandLower[barIndex-1];
				this.DrawValueSingleLine(g, bar, valueLower, prevLower);
				
				double valueUpper = bandUpper[barIndex];
				double  prevUpper = bandUpper[barIndex-1];
				this.DrawValueSingleLine(g, bar, valueUpper, prevUpper);
			} catch (Exception ex) {
				#if DEBUG
				Debugger.Break();
				#endif
			}
			
			indicatorLegDrawn = true;
			return indicatorLegDrawn;
		}
	}
}
