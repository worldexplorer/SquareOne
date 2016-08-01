using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Indicators {
	public class IndicatorAtrBand : Indicator {
		public IndicatorParameter				ParamMultiplier;	// Indicator searches for IndicatorParameter being fields, not properties
		public DataSeriesTimeBasedColorified	BandLower;
		public DataSeriesTimeBasedColorified	BandUpper;

		Indicator atr;

		public override int FirstValidBarIndex {
			get { return (int)this.atr.FirstValidBarIndex; }
			protected set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAtrBand(Indicator atr) : base() {
			base.Name = atr.Name + "band";
			base.ChartPanelType = ChartPanelType.PanelPrice;
			base.HasValueForStreamingBar_caculatedOnEachQuote = true;
			//base.Decimals = 0;	// "156,752.66" is too long for TooltipPrice

			this.atr = atr;
			base.LineColor = atr.LineColor;
			base.LineWidth = atr.LineWidth;
			//this.atr.OnIndicatorPeriodChanged += new EventHandler(this.atr_OnIndicatorPeriodChanged);
			this.atr.AddDependentIndicator(this);

			this.ParamMultiplier = new IndicatorParameter("Multiplier", 1, 0.1, 3, 0.1);
		}

		//void atr_OnIndicatorPeriodChanged(object sender, EventArgs e) {
		//    this.BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries();
		//}
		
		public override void BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries() {
			string msig = " //BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries() EMPTY_CLONE_BARS_AT_BACKTEST_START ";
			base.BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries();

			//if (base.ClosesProxyEffective.Count != 0) {
			//    string msg = "AT_BACKTEST_CONTEXT_INITIALIZE_ClosesProxyEffective.Count_MUST_BE_0";
			//    Assembler.PopupException(msg);
			//}

			string state = "";
			if (this.BandLower == null) {
				state = "FIRST_BACKTEST_AFTER_APP_RESTART";
				this.BandLower = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, "BandLower for " + base.Name, base.LineColor);
				this.BandUpper = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, "BandUpper for " + base.Name, base.LineColor);
				return;
			}

			state = "SECOND_AND_FOLLOWING__BOTH_DISCONNECTED_OR_LIVE_BACKTESTS_AFTER_APP_RESTART";
			//if (this.smaSeries.Period != this.ParamPeriod.ValueCurrentAsInteger) {
			//    this.smaSeries.Period  = this.ParamPeriod.ValueCurrentAsInteger;
			//}
			
			if (this.BandLower.Count > 0) {
			    string msg1 = "CLEARING_FOR_NEXT_BACKTEST__OTHERWIZE_sma.CalculateOwnValue()_WILL_COMPLAIN_ON_SAME_VALUES_ALREADY_THERE";
			    //Assembler.PopupException(msg1, null, false);
			    this.BandLower.Clear();
			    this.BandUpper.Clear();
			}

			//v1 this.smaSeries.AverageFor = base.ClosesProxyEffective;
			//if (this.smaSeries.AverageFor == base.ClosesProxyEffective) {
			//    string msg = "MISUSE_UPSTACK__NO_POINT_OF_INVOKING_ME MUST_BE_SAME_AND_ARE smaSeries.AverageFor=base.ClosesProxyEffective";
			//    Assembler.PopupException(msg + msig);
			//    return;
			//}
			//this.smaSeries.SubstituteBars_withoutRecalculation(base.ClosesProxyEffective);
			//// NOISY this.checkPopupOnResetAndSync(msig + state);
		}
		public override string InitializeBacktest_beforeStarted_checkErrors() {
			if (this.ParamMultiplier.ValueCurrent <= 0) return "Multiplier[" + this.ParamMultiplier.ValueCurrent + "] MUST BE > 0";
			return null;
		}
		
		public override double CalculateOwnValue_onNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(Bar newStaticBar) {
			double atrValue = double.NaN;
			bool addNan = false;
			
			if (addNan == false && newStaticBar.ParentBarsIndex < this.FirstValidBarIndex) {
				addNan  = true;
			}
			if (addNan == false && newStaticBar.ParentBarsIndex > this.atr.OwnValuesCalculated.Count - 1) {
				addNan  = true;
			}
			//if (addNan == false && double.IsNaN(atrValue)) {
			//    addNan  = true;
			//}
			
			if (addNan) {
				//if (base.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) return double.NaN;

				//this.bandLower.Append(newStaticBar.DateTimeOpen, double.NaN);
				//this.bandUpper.Append(newStaticBar.DateTimeOpen, double.NaN);
				this.BandLower.AppendWithParentBar(newStaticBar.DateTimeOpen, double.NaN, newStaticBar);
				this.BandUpper.AppendWithParentBar(newStaticBar.DateTimeOpen, double.NaN, newStaticBar);
				//base.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, double.NaN);
				return double.NaN;
			}

			atrValue = this.atr.OwnValuesCalculated[newStaticBar.ParentBarsIndex];

			// EPIC_FAIL double lastClose = base.ClosesProxyEffective.StreamingValue;
			double lastClose = newStaticBar.Close;
			double atrMultiplied = atrValue * this.ParamMultiplier.ValueCurrent;	// I_WONT_CHECK_ZERO_RESULT_WILL_BE_DRAWN_AS_WEIRD_1PX_LINE_AT_BOTTOM_CHART
			//v1
			//this.bandLower.Append(newStaticBar.DateTimeOpen, lastClose - atrMultiplied);
			//this.bandUpper.Append(newStaticBar.DateTimeOpen, lastClose + atrMultiplied);
			//v2
			double atrMultipliedAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceStep(atrMultiplied, PriceLevelRoundingMode.RoundToClosest);
			double lower = lastClose - atrMultipliedAligned;
			double upper = lastClose + atrMultipliedAligned;
			//this.bandLower.AppendWithParentBar(newStaticBar.DateTimeOpen, lower, newStaticBar);
			//this.bandUpper.AppendWithParentBar(newStaticBar.DateTimeOpen, upper, newStaticBar);
			//v3 BACK_FROM_V4_AFTER_SIMULATE_ROUND_WAS_FIXED
			double lowerAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceStep(lower, PriceLevelRoundingMode.RoundToClosest);
			double upperAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceStep(upper, PriceLevelRoundingMode.RoundToClosest);
			//v4
			//double lowerAligned = Math.Round(lower, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			//double upperAligned = Math.Round(upper, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			
			double alreadyCalculated_kozImDependent = double.NaN;
			if (this.BandLower.ContainsDate(newStaticBar.DateTimeOpen)) {
				alreadyCalculated_kozImDependent = this.BandLower[newStaticBar.DateTimeOpen];
				if (alreadyCalculated_kozImDependent != lowerAligned) {
					string msg = "YOU_MUST_HAVE_CLEARED_BandLower_WHEN_CHANGING_PERIOD_OR_REBACKTESTING";
					Assembler.PopupException(msg);
				}
				return double.NaN;
			}

			this.BandLower.AppendWithParentBar(newStaticBar.DateTimeOpen, lowerAligned, newStaticBar);
			this.BandUpper.AppendWithParentBar(newStaticBar.DateTimeOpen, upperAligned, newStaticBar);

			
			#if DEBUG
			double valueLower = BandLower[newStaticBar.ParentBarsIndex];
			double valueUpper = BandUpper[newStaticBar.ParentBarsIndex];
			double diffCloseToLower = newStaticBar.Close - valueLower;
			double diffCloseToUpper = valueUpper - newStaticBar.Close;
			
			//double diffCloseToLowerRounded = Math.Round(diffCloseToLower, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			//double diffCloseToUpperRounded = Math.Round(diffCloseToUpper, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			//double roundingError = Math.Abs(diffCloseToLowerRounded - diffCloseToUpperRounded);
			
			double roundingError = Math.Abs(diffCloseToLower - diffCloseToUpper);
			roundingError = Math.Round(roundingError, newStaticBar.ParentBars.SymbolInfo.PriceDecimals);
			if (roundingError > newStaticBar.ParentBars.SymbolInfo.PriceStep) {
			//if (diffCloseToLower != diffCloseToUpper) {
				//greater than BacktestSpreadModelerPercentageOfMedian(0.01) will make ATRband inconsistent! you'll see in TooltipPrice (Close+ATR != C+Upper) & SPREAD_MODELER_SHOULD_GENERATE_TIGHTER_SPREADS
				string msg = "ATR_BAND_ASYMMETRICAL"
					+ " diffCloseToLower[" + diffCloseToLower + "] != diffCloseToUpper[" + diffCloseToUpper + "]"
					+ " for bar.Close[" + BandLower.ParentBarsByDate[newStaticBar.DateTimeOpen].Close + "]"
					+ " " + this.Executor.BacktesterOrLivesimulator.BacktestDataSource.StreamingAsBacktest_nullUnsafe.SpreadModeler.ToString()
					;
				Assembler.PopupException(msg, null, false);
			}
			#endif

			
			// base.OwnValuesCalculated will all be NaNs, because I keep my data in this.bandLower and this.bandUpper;
			// TODO make sure OnPaint will invoke my DrawValueIndicatorSpecific() despite each OwnValue is NaN and it looks like there is nothing to draw
			return double.NaN;
		}

		protected override bool DrawValueIndicatorSpecific(Graphics g, Bar bar) {
			return base.DrawValueBand(g, bar, this.BandLower, this.BandUpper);
		}
		public override SortedDictionary<string, string> OwnValuesForTooltipPrice(Bar bar) {
			string suffix = atr.Name + "*" + this.ParamMultiplier.ValueCurrent;
			SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
			ret.Add("C+" + suffix, this.OwnValueForBar_formatted(bar, this.BandUpper));
			ret.Add("C-" + suffix, this.OwnValueForBar_formatted(bar, this.BandLower));
			return ret;
		}
	}
}
