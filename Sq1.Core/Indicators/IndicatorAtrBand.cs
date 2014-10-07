using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Core.Indicators {
	public class IndicatorAtrBand : Indicator {
		public IndicatorParameter ParamMultiplier { get; set; }	// IndicatorParameter must be a property
		DataSeriesTimeBasedColorified bandLower;
		DataSeriesTimeBasedColorified bandUpper;

		Indicator atr;

		public override int FirstValidBarIndex {
			get { return (int)this.atr.FirstValidBarIndex; }
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAtrBand(Indicator atr) : base() {
			base.Name = atr.Name + "band";
			base.ChartPanelType = ChartPanelType.PanelPrice;
			base.LineColor = atr.LineColor;
			base.LineWidth = atr.LineWidth;
			base.Decimals = 0;	// "156,752.66" is too long for TooltipPrice
			this.atr = atr;
			ParamMultiplier = new IndicatorParameter("Multiplier", 1, 0.1, 10, 0.1);
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.ParamMultiplier.ValueCurrent <= 0) return "Multiplier[" + this.ParamMultiplier.ValueCurrent + "] MUST BE > 0";
			this.bandLower = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, base.LineColor);
			this.bandUpper = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, base.LineColor);
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			double atrValue = double.NaN;
			bool addNan = false;
			
			if (addNan = false && newStaticBar.ParentBarsIndex < this.FirstValidBarIndex) {
				addNan = true;
			}
			if (addNan = false && newStaticBar.ParentBarsIndex > this.atr.OwnValuesCalculated.Count - 1) {
				addNan = true;
			} else {
				atrValue = this.atr.OwnValuesCalculated[newStaticBar.ParentBarsIndex];
			}
			if (addNan = false &&  double.IsNaN(atrValue)) {
				addNan = true;
			}
			
			if (addNan) {
				this.bandLower.Append(newStaticBar.DateTimeOpen, double.NaN);
				this.bandUpper.Append(newStaticBar.DateTimeOpen, double.NaN);
				return double.NaN;
			}

			// EPIC_FAIL double lastClose = base.ClosesProxyEffective.StreamingValue;
			double lastClose = newStaticBar.Close;
			double retMultiplied = atrValue * this.ParamMultiplier.ValueCurrent;	// I_WONT_CHECK_ZERO_RESULT_WILL_BE_DRAWN_AS_WEIRD_1PX_LINE_AT_BOTTOM_CHART
			this.bandLower.Append(newStaticBar.DateTimeOpen, lastClose - retMultiplied);
			this.bandUpper.Append(newStaticBar.DateTimeOpen, lastClose + retMultiplied);
			
			// base.OwnValuesCalculated will all be NaNs, because I keep my data in this.bandLower and this.bandUpper;
			// TODO make sure OnPaint will invoke my DrawValueIndicatorSpecific() despite each OwnValue is NaN and it looks like there is nothing to draw
			return double.NaN;
		}

		protected override bool DrawValueIndicatorSpecific(Graphics g, Bar bar) {
			return base.DrawValueBand(g, bar, this.bandLower, this.bandUpper);
		}
		public override SortedDictionary<string, string> ValuesForTooltipPrice(Bar bar) {
			string suffix = atr.Name + "*" + this.ParamMultiplier.ValueCurrent;
			SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
			ret.Add("C+" + suffix, this.FormatValueForBar(bar, this.bandUpper));
			ret.Add("C-" + suffix, this.FormatValueForBar(bar, this.bandLower));
			return ret;
		}
	}
}
