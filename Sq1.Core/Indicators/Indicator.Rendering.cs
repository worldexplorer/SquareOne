using System;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Core.Indicators {
	public partial class Indicator {
		public	HostPanelForIndicator HostPanelForIndicator		{ get; protected set; }

				Color lineColor;
		public	Color LineColor {
			get { return this.lineColor; }
			set { this.lineColor = value; this.brushForeground = null; this.penForeground = null; }
		}
				int lineWidth;
		public	int LineWidth {
			get { return this.lineWidth; }
			set { this.lineWidth = value; this.brushForeground = null; this.penForeground = null; }
		}

				Brush brushForeground;
		public	Brush BrushForeground							{ get {
				if (this.brushForeground == null) {
					this.brushForeground = new SolidBrush(this.LineColor);
				}
				return this.brushForeground;
			} }
				Pen penForeground;
		public	Pen PenForeground { get {
				if (this.penForeground == null) {
					this.penForeground = new Pen(this.LineColor, this.LineWidth);
				}
				return this.penForeground;
			} }

		
		// DrawValue() isn't in Sq1.Charting.PanelNamedFolding so that one particular indicator may override and draw more than one line with labels;  
//		public bool DrawValueEntryPoint(Graphics g, Bar bar, Rectangle barPlaceholder) {
		public bool DrawValueEntryPoint(Graphics g, Bar bar) {
			bool indicatorLegDrawn = false;
			// MOVED_UPSTACK if (bar.ParentBarsIndex <= this.FirstValidBarIndex) return indicatorLegDrawn;
			//if (Object.ReferenceEquals(this.OwnValuesCalculated, null)) return indicatorLegDrawn;
			if (this.OwnValuesCalculated == null) return indicatorLegDrawn;
			string msig = " Indicator[" + this.Name + "]{" + this.OwnValuesCalculated.ToString() + "}"
				+ ".DrawValueEntryPoint(" + bar + ")";

			if (bar.IsBarStaticLast && this.Executor.IsStreamingTriggeringScript == false) {
				string msg = "DONT_WANT_TO_HACK_WILL_DRAW_LAST_STATIC_BARS_INDICATOR_VALUE_AFTER_YOU_TURN_ON_STREAMING_SO_I_WILL_HAVE_NEW_QUOTE_PROVING_THE_LAST_BAR_IS_FORMED";
				//Assembler.PopupException(msg + msig, null, false);
				//return indicatorLegDrawn;
			}
			if (this.OwnValuesCalculated.ContainsDate(bar.DateTimeOpen) == false) {
				if (bar.IsBarStreaming) {
					if (this.HasValueForStreamingBar_caculatedOnEachQuote == false) return indicatorLegDrawn;

					string msg2 = "YOU_DIDNT_IMPLEMENT_CalculateOwnValue_onNewStreamingQuote_invokedAtEachQuoteNoExceptions_NoPeriodWaiting()";
					Assembler.PopupException(msg2 + msig, null, false);
					return indicatorLegDrawn;
				}
				if (this.Executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
					return indicatorLegDrawn;		// WHY? DONT_DELETE_ME
				}
				if (this.Executor.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false) {
					string msg2 = "YOU_HAD_BACKTESTONSELECTORS_OFF_AND_CLICKED_ANOTHER_SYMBOL? SKIPPING_RENDERING BUT_BETTER_TO_SET_OWN_VALUES_TO_NULL_AT_SYMBOL_CHANGE";
					return indicatorLegDrawn;
				}
				if (bar == this.BarsEffective.BarStaticLast_nullUnsafe && this.BarsEffective.BarStreaming_nullUnsafe == null) {
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
			return this.DrawValueSingleLine(g, bar, double.NaN, double.NaN);
		}
		protected bool DrawValueSingleLine(Graphics g, Bar bar, double calculated_forBands = double.NaN, double calculatedPrev_forBands = double.NaN) {
			string msig = " Indicator[" + this.NameWithParameters + "].DrawValueSingleLine(" + bar + ")";
			bool indicatorLegDrawn = false;
			if (double.IsNaN(calculated_forBands)) {
				calculated_forBands = this.OwnValuesCalculated[bar.ParentBarsIndex];
			}
			// v2-END
			if (double.IsNaN(calculated_forBands)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_BAR bar[" + bar + "]";
				//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			this.DotsExistsForCurrentSlidingWindow++;
			int x = this.HostPanelForIndicator.BarToX(bar.ParentBarsIndex) + this.HostPanelForIndicator.BarShadowOffset;
			int y = this.HostPanelForIndicator.ValueToYinverted(calculated_forBands);
			if (y == 0)  {
				string msg = "INDICATOR_VALUE_TOO_BIG_INVERTED SKIPPING_DRAWING_OUTSIDE_HOST_PANEL";
				//Assembler.PopupException(msg + msig, null, false);
				//return indicatorLegDrawn;
			}
			int max = this.HostPanelForIndicator.ValueToYinverted(0);
			if (y == max) {
				string msg = "INDICATOR_VALUE_TOO_SMALL_INVERTED SKIPPING_DRAWING_OUTSIDE_HOST_PANEL";
				//Assembler.PopupException(msg + msig, null, false);
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
			
			if (double.IsNaN(calculatedPrev_forBands)) {
				calculatedPrev_forBands = this.OwnValuesCalculated[barIndexPrev];
			}
			
			if (double.IsNaN(calculatedPrev_forBands)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_PREVBAR barIndexPrev[" + barIndexPrev + "]";
				//INDICATORS_INCUBATION_PERIOD_NO_NEED_TO_REPORT Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
				
			int yPrev = this.HostPanelForIndicator.ValueToYinverted(calculatedPrev_forBands);
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
					double valueLower	= bandLower[barIndex];
					double prevLower	= bandLower[barIndex - 1];
					this.DrawValueSingleLine(g, bar, valueLower, prevLower);
				}
				if (willDrawUpper) {
					double valueUpper = bandUpper[barIndex];
					double  prevUpper = bandUpper[barIndex-1];
					this.DrawValueSingleLine(g, bar, valueUpper, prevUpper);
				}
			} catch (Exception ex) {
				string msg = "UPPER_OR_LOWER_OUT_OF_BOUNDARY OR DrawValueSingleLine_THROWN";
				Assembler.PopupException(msg, ex);
			}
			
			if (willDrawLower || willDrawUpper) indicatorLegDrawn = true;
			return indicatorLegDrawn;
		}

		bool checkBandValue(int barIndex, DataSeriesTimeBased bandLowerOrUpper, string bandSeriesName = "bandLower") {
			string msig = " //checkBandValue(" + barIndex + ", " + bandLowerOrUpper + ", " + bandSeriesName + ")";
			bool valueOk = true;

			if (bandLowerOrUpper.Count < 2) {
				string msg = "INDICATOR_BAND_PREVIOUS_VALUE_DOESNT_EXIST " + bandSeriesName
					 + ".Count=[" + bandLowerOrUpper.Count + "] must be >= 2";
				Assembler.PopupException(msg + msig);
				valueOk = false;
				return valueOk;
			}
	
			if (bandLowerOrUpper.Count < this.FirstValidBarIndex) {
				string msg = "INDICATOR_BAND_INCUBATOR_STATE " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count
					+ "] must be > this.FirstValidBarIndex[" + this.FirstValidBarIndex + "]";
				Assembler.PopupException(msg + msig);
				valueOk = false;
				return valueOk;
			}

			if (this.HasValueForStreamingBar_caculatedOnEachQuote) {
				if (barIndex >= bandLowerOrUpper.Count) {
					string msg = "INDICATOR_BAND_VALUE_BEYOND_EXISTING barStreaming[" + barIndex
						+ "] must be < " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count + "]";
					Assembler.PopupException(msg + msig, null, false);
					valueOk = false;
					return valueOk;
				}
			} else {
				if (barIndex > bandLowerOrUpper.Count) {
					string msg = "INDICATOR_BAND_VALUE_BEYOND_EXISTING barStaticLast[" + barIndex
						+ "] must be < " + bandSeriesName + ".Count=[" + bandLowerOrUpper.Count + "]";
					Assembler.PopupException(msg + msig, null, false);
					valueOk = false;
					return valueOk;
				}
			}
			
			double value = bandLowerOrUpper[barIndex];
			double valuePrevious = bandLowerOrUpper[barIndex - 1];

			if (double.IsNaN(value)) {
				string msg = "INDICATOR_VALUE_IS_NAN " + bandSeriesName + "[" + barIndex + "]";
				Assembler.PopupException(msg + msig, null, false);
				valueOk = false;
				return valueOk;
			}

			if (double.IsNaN(valuePrevious)) {
				string msg = "INDICATOR_VALUE_PREVIOUS_IS_NAN " + bandSeriesName + "[" + (barIndex - 1) + "]";
				Assembler.PopupException(msg + msig, null, false);
				valueOk = false;
				return valueOk;
			}

			return valueOk;
		}
	
	}
}
