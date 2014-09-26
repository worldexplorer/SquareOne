using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Indicators.HelperSeries;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Indicators {
	public class IndicatorAverageTrueRange : Indicator {

		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=5, ValueMin=1, ValueMax=11, ValueIncrement=2)]
		public int Period { get; set; }
		
		// CLEANER_INDICATORS public IndicatorParameter ParamPeriod;

		[IndicatorParameterAttribute(Name="Multiplier",
			ValueCurrent=1, ValueMin=0.1, ValueMax=10, ValueIncrement=0.1)]
		public int Multiplier { get; set; }

		public bool ShowAsBandOnPricePanel { get; set; }

		private TrueRangeSeries trueRangeSeries;
		private MovingAverageSimple smaSeries;
		private DataSeriesTimeBasedColorified bandLower;
		private DataSeriesTimeBasedColorified bandUpper;

		public override int FirstValidBarIndex {
			get { return this.Period; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAverageTrueRange() : base() {
			base.Name = "ATR";
			//base.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			Multiplier = 1;
			ShowAsBandOnPricePanel = true;
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.Period <= 0) return "Period[" + this.Period + "] MUST BE > 0";
			// not in ctor because base.BarsEffective should not be null; initialized only now in Indicator.BacktestStarting() upstack
			this.trueRangeSeries = new TrueRangeSeries(base.OwnValuesCalculated.ScaleInterval);
			this.smaSeries = new MovingAverageSimple(this.trueRangeSeries, this.Period, base.OwnValuesCalculated.ScaleInterval);
			if (this.ShowAsBandOnPricePanel) {
				this.bandLower = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, base.LineColor);
				this.bandUpper = new DataSeriesTimeBasedColorified(base.OwnValuesCalculated.ScaleInterval, base.LineColor);
			}
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			this.trueRangeSeries.CalculateAppendOwnValueForNewStaticBarFormed(newStaticBar);
			
			if (this.Period <= 0) return double.NaN;
			//if (base.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) return double.NaN;
			if (newStaticBar.ParentBarsIndex < this.FirstValidBarIndex) {
				this.bandLower.Append(newStaticBar.DateTimeOpen, double.NaN);
				this.bandUpper.Append(newStaticBar.DateTimeOpen, double.NaN);
				return double.NaN;
			}

			double ret = this.smaSeries.CalculateAppendOwnValueForNewStaticBarFormedNanUnsafe(newStaticBar);
			
			if (this.ShowAsBandOnPricePanel == false) return ret;
			
			if (double.IsNaN(ret)) {
				this.bandLower.Append(newStaticBar.DateTimeOpen, double.NaN);
				this.bandUpper.Append(newStaticBar.DateTimeOpen, double.NaN);
				//Debugger.Break();
				return ret;
			}
			
			if (ret < 0) {
				Debugger.Break();
			}
			
			// EPIC_FAIL double lastClose = base.ClosesProxyEffective.StreamingValue;
			double lastClose = newStaticBar.Close;
			double retMultiplied = ret * this.Multiplier;	// I_WONT_CHECK_ZERO_RESULT_WILL_BE_DRAWN_AS_WEIRD_1PX_LINE_AT_BOTTOM_CHART
			this.bandLower.Append(newStaticBar.DateTimeOpen, lastClose - retMultiplied);
			this.bandUpper.Append(newStaticBar.DateTimeOpen, lastClose + retMultiplied);
			
			return ret;
		}

		protected override bool DrawValueIndicatorSpecific(Graphics g, Bar bar) {
			if (this.ShowAsBandOnPricePanel == false) {
				return this.DrawValueSingleLine(g, bar);
			}
			return this.DrawValueBand(g, bar, this.bandLower, this.bandUpper);
		}

	}
}
