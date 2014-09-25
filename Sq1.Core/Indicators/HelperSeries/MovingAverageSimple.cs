using System;
using System.Diagnostics;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators.HelperSeries {
	public class MovingAverageSimple : DataSeriesTimeBased {
		public DataSeriesTimeBased AverageFor;	// could be Bars, DataSeriesProxyBars or Indicator.OwnValues
		public int Period;
			
		public MovingAverageSimple(DataSeriesTimeBased averageFor, int period, BarScaleInterval scaleInterval) : base(scaleInterval) {
			AverageFor = averageFor;
			Period = period;
		}
		public double CalculateAppendOwnValueForNewStaticBarFormed(Bar newStaticBar) {
			if (base.ContainsDate(newStaticBar.DateTimeOpen)) {
				string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW DONT_INVOKE_ME_TWICE on[" + newStaticBar.DateTimeOpen + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg);
				return double.NaN;
			}
			double thisBarValue = this.CalculateOwnValue(newStaticBar);
			base.Append(newStaticBar.DateTimeOpen, thisBarValue);
			return thisBarValue;
		}

		public double CalculateOwnValue(Bar newStaticBar) {
			// COPYPASTE_FROM_IndicatorAverageMovingSimple:Indicator BEGIN
			if (this.Period <= 0) return double.NaN;
			if (this.AverageFor.Count - 1 < this.Period) return double.NaN;
			if (newStaticBar.ParentBarsIndex - 1 < this.Period) return double.NaN;

			double sum = 0;
			int slidingWindowRightBar = newStaticBar.ParentBarsIndex;
			int slidingWindowLeftBar = slidingWindowRightBar - this.Period + 1;	// FirstValidBarIndex must be Period+1
			int barsProcessedCheck = 0;
			for (int i = slidingWindowLeftBar; i <= slidingWindowRightBar; i++) {
				double eachBarTrueRange = this.AverageFor[i];
				if (double.IsNaN(eachBarTrueRange)) {
					Debugger.Break();
					continue;
				}
				sum += eachBarTrueRange;
				barsProcessedCheck++;
			}
			if (barsProcessedCheck != this.Period) {
				Debugger.Break();
			}
			double ret = sum / this.Period;
			return ret;
			// COPYPASTE_FROM_IndicatorAverageMovingSimple:Indicator END
		}
	}
}
