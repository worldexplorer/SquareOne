using System;
using System.Diagnostics;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Indicators {
	public class IndicatorAverageMovingSimple : Indicator {

		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=55, ValueMin=11, ValueMax=99, ValueIncrement=11)]
		public int Period { get; set; }
		
		public override int FirstValidBarIndex {
			get { return this.Period; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAverageMovingSimple() : base() {
			base.Name = "MA";
			base.ChartPanelType = ChartPanelType.PanelPrice;
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.Period > 0) return null;
			return "Period[" + this.Period + "] MUST BE > 0";
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			if (this.Period <= 0) return double.NaN;
			if (base.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) return double.NaN;
			if (newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex) return double.NaN;

			double sum = 0;
			int slidingWindowRightBar = newStaticBar.ParentBarsIndex;
			int slidingWindowLeftBar = slidingWindowRightBar - this.Period + 1;	// FirstValidBarIndex must be Period+1
			int barsProcessedCheck = 0;
			for (int i = slidingWindowLeftBar; i <= slidingWindowRightBar; i++) {
				double eachBarCloses = base.ClosesProxyEffective[i];
				if (double.IsNaN(eachBarCloses)) {
					Debugger.Break();
					continue;
				}
				sum += eachBarCloses;
				barsProcessedCheck++;
			}
			if (barsProcessedCheck != this.Period) {
				Debugger.Break();
			}
			double ret = sum / this.Period;
			return ret;
		}
		// default implementation from base class pasted here:
		//public virtual double CalculateOwnValueOnNewStreamingQuote(Quote newStreamingQuote) {
		//	return this.CalculateOwnValueOnNewStaticBarFormed(newStreamingQuote.ParentBar);
		//}
	}
}
