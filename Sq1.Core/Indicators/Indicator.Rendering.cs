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
			string msig = " Indicator[" + this.Name + "]{" + this.OwnValuesCalculated.ToString() + "}"
				+ ".DrawValueEntryPoint(" + bar + ")";

			if (bar.IsBarStaticLast && this.Executor.IsStreamingTriggeringScript == false) {
				string msg = "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED";
				//Assembler.PopupException(msg);
				//return indicatorLegDrawn;
			}
			if (this.OwnValuesCalculated.ContainsDate(bar.DateTimeOpen) == false) {
				if (bar.IsBarStreaming) {
					string msg2 = "INDICATORS_HAVING_OWN_VALUE_FOR_STREAMING_BAR_IS_NYI";
					//Assembler.PopupException(msg);
					return indicatorLegDrawn;
				}
				if (this.Executor.Backtester.IsBacktestingNoLivesimNow) {
					return indicatorLegDrawn;		// DONT_DELETE_ME
				}
				if (this.Executor.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false) {
					string msg2 = "YOU_HAD_BACKTESTONSELECTORS_OFF_AND_CLICKED_ANOTHER_SYMBOL? SKIPPING_RENDERING BUT_BETTER_TO_SET_OWN_VALUES_TO_NULL_AT_SYMBOL_CHANGE";
					return indicatorLegDrawn;
				}
				if (bar == this.BarsEffective.BarStaticLastNullUnsafe && this.BarsEffective.BarStreaming == null) {
					string msg2 = "WHEN_YOU_START_STREAMING_LAST_STATIC_BAR_WILL_BE_DRAWN";
					return indicatorLegDrawn;
				}
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR bar[" + bar.DateTimeOpen + "]";
				string howToFix = " SYNC_IMPLEMENTATION_WITH_IndicatorMovingAverageSimple OR_FRAMEWORK_BUG_ON_EDGE_BTW_BACKTEST_AND_LIVE";
				Assembler.PopupException(msg + msig + howToFix, null, false);
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
			if (double.IsNaN(calculated)) {
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
				Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			
			// EPIC_FAIL_HERE !!!! double calculatedPrev = this.OwnValuesCalculated[barIndexPrev];
			if (this.OwnValuesCalculated.ContainsDate(barPrev.DateTimeOpen) == false) {
				//string msg = "EDIT_DATASOURCE_EXTEND_MARKET_OPEN_CLOSE_HOURS";
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_PREVIOUS_BAR[" + barPrev.DateTimeOpen + "] " + this.ToString();
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

			int barIndex = bar.ParentBarsIndex;

			// base.OwnValuesCalculated will all be NaNs, because I keep my data in this.bandLower and this.bandUpper;
			//double calculated = this.OwnValuesCalculated[barIndex];
			//if (double.IsNaN(calculated)) {
			//	string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_BAR bar[" + bar + "]";
			//	//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
			//	return indicatorLegDrawn;
			//}

			bool willDrawLower = this.checkBandValue(barIndex, bandLower, "bandLower");
			bool willDrawUpper = this.checkBandValue(barIndex, bandUpper, "bandUpper");
			try {
				if (willDrawLower) {
					double valueLower = bandLower[barIndex];
					double prevLower = bandLower[barIndex - 1];
					this.DrawValueSingleLine(g, bar, valueLower, prevLower);
				}
				if (willDrawUpper) {
					double valueUpper = bandUpper[barIndex];
					double  prevUpper = bandUpper[barIndex-1];
					this.DrawValueSingleLine(g, bar, valueUpper, prevUpper);
				}
			} catch (Exception ex) {
				#if DEBUG
				Debugger.Break();
				#endif
			}
			
			if (willDrawLower || willDrawUpper) indicatorLegDrawn = true;
			return indicatorLegDrawn;
		}

		bool checkBandValue(int barIndex, DataSeriesTimeBased bandLowerOrUpper, string bandSeriesName = "bandLower") {
			bool valueOk = true;
			if (valueOk == true && bandLowerOrUpper.Count < 2) {
				string msg = "INDICATOR_BAND_PREVIOUS_VALUE_DOESNT_EXIST " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count
					+ "] must be >= 2";
				#if DEBUG
				Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				valueOk = false;
			}
	
			if (valueOk == true && bandLowerOrUpper.Count < this.FirstValidBarIndex) {
				string msg = "INDICATOR_BAND_INCUBATOR_STATE " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count
					+ "] must be > this.FirstValidBarIndex[" + this.FirstValidBarIndex + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				valueOk = false;
			}

			if (valueOk == true && barIndex >= bandLowerOrUpper.Count) {
				string msg = "INDICATOR_BAND_VALUE_REQUESTED_BEYOND_EXISTING barIndexRequested[" + barIndex
					+ "] must be < " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count + "]";
				#if DEBUG
				//Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				valueOk = false;
			}
			
			if (valueOk == false) return valueOk;

			double value = bandLowerOrUpper[barIndex];
			double valuePrevious = bandLowerOrUpper[barIndex - 1];

			if (double.IsNaN(value)) {
				string msg = "INDICATOR_VALUE_IS_NAN " + bandSeriesName + "[" + barIndex + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				valueOk = false;
			}

			if (double.IsNaN(valuePrevious)) {
				string msg = "INDICATOR_VALUE_PREVIOUS_IS_NAN " + bandSeriesName + "[" + (barIndex - 1) + "]";
				#if DEBUG
				//Debugger.Break();
				#endif
				//Assembler.PopupException(msg + msig);
				valueOk = false;
			}

			return valueOk;
		}
	
	}
}
