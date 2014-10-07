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
		public IndicatorParameter ParamPeriod { get; set; }	// IndicatorParameter must be a property

		private TrueRangeSeries trueRangeSeries;
		private MovingAverageSimple smaSeries;

		public override int FirstValidBarIndex {
			get { return (int)this.ParamPeriod.ValueCurrent; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAverageTrueRange() : base() {
			base.Name = "ATR";
			base.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			ParamPeriod = new IndicatorParameter("Period", 5, 1, 11, 2);
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.ParamPeriod.ValueCurrent <= 0) return "Period[" + this.ParamPeriod.ValueCurrent + "] MUST BE > 0";
			// not in ctor because base.BarsEffective should not be null; initialized only now in Indicator.BacktestStarting() upstack
			this.trueRangeSeries = new TrueRangeSeries(base.OwnValuesCalculated.ScaleInterval);
			this.smaSeries = new MovingAverageSimple(this.trueRangeSeries, (int)this.ParamPeriod.ValueCurrent, base.OwnValuesCalculated.ScaleInterval);
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			this.trueRangeSeries.CalculateAppendOwnValueForNewStaticBarFormed(newStaticBar);
			if (this.ParamPeriod.ValueCurrent <= 0) return double.NaN;
			double ret = this.smaSeries.CalculateAppendOwnValueForNewStaticBarFormedNanUnsafe(newStaticBar);
			return ret;
		}
	}
}
