using System;
using System.Diagnostics;

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

		private TrueRangeSeries trueRangeSeries;
		private MovingAverageSimple smaSeries;

		public override int FirstValidBarIndex {
			get { return this.Period; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAverageTrueRange() : base() {
			base.Name = "ATR";
			base.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.Period <= 0) return "Period[" + this.Period + "] MUST BE > 0";
			// not in ctor because base.BarsEffective should not be null; initialized only now in Indicator.BacktestStarting() upstack
			this.trueRangeSeries = new TrueRangeSeries(base.OwnValuesCalculated.ScaleInterval);
			this.smaSeries = new MovingAverageSimple(this.trueRangeSeries, this.Period, base.OwnValuesCalculated.ScaleInterval);
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			this.trueRangeSeries.CalculateAppendOwnValueForNewStaticBarFormed(newStaticBar);
			
			if (this.Period <= 0) return double.NaN;
			if (base.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) return double.NaN;
			if (newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex) return double.NaN;

			double ret = this.smaSeries.CalculateAppendOwnValueForNewStaticBarFormed(newStaticBar);
			return ret;
		}
	}
}
