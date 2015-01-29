using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Core.Indicators {
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
			base.LineColor = atr.LineColor;
			base.LineWidth = atr.LineWidth;
			base.Decimals = 0;	// "156,752.66" is too long for TooltipPrice
			this.atr = atr;
			this.ParamMultiplier = new IndicatorParameter("Multiplier", 1, 0.1, 10, 0.1);
		}
		
		public override string InitializeBacktestStartingPreCheckErrors() {
			if (this.ParamMultiplier.ValueCurrent <= 0) return "Multiplier[" + this.ParamMultiplier.ValueCurrent + "] MUST BE > 0";
			this.BandLower = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, "BandLower for " + base.Name, base.LineColor);
			this.BandUpper = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, "BandLower for " + base.Name, base.LineColor);
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			double atrValue = double.NaN;
			bool addNan = false;
			
			if (addNan == false && newStaticBar.ParentBarsIndex < this.FirstValidBarIndex) {
				addNan  = true;
			}
			if (addNan == false && newStaticBar.ParentBarsIndex > this.atr.OwnValuesCalculated.Count - 1) {
				addNan  = true;
			} else {
				atrValue = this.atr.OwnValuesCalculated[newStaticBar.ParentBarsIndex];
			}
			if (addNan == false && double.IsNaN(atrValue)) {
				addNan  = true;
			}
			
			if (addNan) {
				//this.bandLower.Append(newStaticBar.DateTimeOpen, double.NaN);
				//this.bandUpper.Append(newStaticBar.DateTimeOpen, double.NaN);
				this.BandLower.AppendWithParentBar(newStaticBar.DateTimeOpen, double.NaN, newStaticBar);
				this.BandUpper.AppendWithParentBar(newStaticBar.DateTimeOpen, double.NaN, newStaticBar);
				return double.NaN;
			}

			// EPIC_FAIL double lastClose = base.ClosesProxyEffective.StreamingValue;
			double lastClose = newStaticBar.Close;
			double atrMultiplied = atrValue * this.ParamMultiplier.ValueCurrent;	// I_WONT_CHECK_ZERO_RESULT_WILL_BE_DRAWN_AS_WEIRD_1PX_LINE_AT_BOTTOM_CHART
			//v1
			//this.bandLower.Append(newStaticBar.DateTimeOpen, lastClose - atrMultiplied);
			//this.bandUpper.Append(newStaticBar.DateTimeOpen, lastClose + atrMultiplied);
			//v2
			double atrMultipliedAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceLevel(atrMultiplied, PriceLevelRoundingMode.RoundToClosest);
			double lower = lastClose - atrMultipliedAligned;
			double upper = lastClose + atrMultipliedAligned;
			//this.bandLower.AppendWithParentBar(newStaticBar.DateTimeOpen, lower, newStaticBar);
			//this.bandUpper.AppendWithParentBar(newStaticBar.DateTimeOpen, upper, newStaticBar);
			//v3 BACK_FROM_V4_AFTER_SIMULATE_ROUND_WAS_FIXED
			double lowerAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceLevel(lower, PriceLevelRoundingMode.RoundToClosest);
			double upperAligned = newStaticBar.ParentBars.SymbolInfo.AlignToPriceLevel(upper, PriceLevelRoundingMode.RoundToClosest);
			//v4
			//double lowerAligned = Math.Round(lower, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			//double upperAligned = Math.Round(upper, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			
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
			roundingError = Math.Round(roundingError, newStaticBar.ParentBars.SymbolInfo.DecimalsPrice);
			if (roundingError > newStaticBar.ParentBars.SymbolInfo.PriceMinimalStepFromDecimal) {
			//if (diffCloseToLower != diffCloseToUpper) {
				//greater than BacktestSpreadModelerPercentageOfMedian(0.01) will make ATRband inconsistent! you'll see in TooltipPrice (Close+ATR != C+Upper) & SPREAD_MODELER_SHOULD_GENERATE_TIGHTER_SPREADS
				string msg = "INDICATOR_SHOULD_STORE_UPPER_LOWER_ROUNDED_SYMMETRICAL"
					+ " diffCloseToLower[" + diffCloseToLower + "] != diffCloseToUpper[" + diffCloseToUpper + "]"
					+ " for bar.Close[" + BandLower.ParentBarsByDate[newStaticBar.DateTimeOpen].Close + "]"
					+ " " + this.Executor.Backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler.ToString()
					;
				int a = 1;
				Debugger.Break();
				//Assembler.PopupException(msg);
			}
			#endif

			
			// base.OwnValuesCalculated will all be NaNs, because I keep my data in this.bandLower and this.bandUpper;
			// TODO make sure OnPaint will invoke my DrawValueIndicatorSpecific() despite each OwnValue is NaN and it looks like there is nothing to draw
			return double.NaN;
		}

		protected override bool DrawValueIndicatorSpecific(Graphics g, Bar bar) {
			return base.DrawValueBand(g, bar, this.BandLower, this.BandUpper);
		}
		public override SortedDictionary<string, string> ValuesForTooltipPrice(Bar bar) {
			string suffix = atr.Name + "*" + this.ParamMultiplier.ValueCurrent;
			SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
			ret.Add("C+" + suffix, this.FormatValueForBar(bar, this.BandUpper));
			ret.Add("C-" + suffix, this.FormatValueForBar(bar, this.BandLower));
			return ret;
		}
	}
}
