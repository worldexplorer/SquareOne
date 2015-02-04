using System;
using System.Diagnostics;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators.HelperSeries {
	public class MovingAverageSimple : DataSeriesTimeBased {
		public DataSeriesTimeBased AverageFor;	// could be Bars, DataSeriesProxyBars or Indicator.OwnValues
		public int Period;
			
		public MovingAverageSimple(DataSeriesTimeBased averageFor, int period, int decimals = 2) : base(averageFor.ScaleInterval, decimals) {
			AverageFor = averageFor;
			Period = period;
		}
		public double CalculateAppendOwnValueForNewStaticBarFormedNanUnsafe(Bar newStaticBar, bool allowExistingValueSame = false) {
			double valueCalculated = this.CalculateOwnValue(newStaticBar);
			if (base.ContainsDate(newStaticBar.DateTimeOpen)) {
				double valueWeAlreadyHave = base[newStaticBar.DateTimeOpen];
				if (valueCalculated == valueWeAlreadyHave && allowExistingValueSame) {
					return valueCalculated;
				}
				string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW DONT_INVOKE_ME_TWICE on[" + newStaticBar.DateTimeOpen + "]"
					+ " thisBarValue[" + valueCalculated.ToString(base.Format) + "] valueWeAlreadyHave[" + valueWeAlreadyHave + "]";
				Assembler.PopupException(msg);
				return double.NaN;
			}
			base.Append(newStaticBar.DateTimeOpen, valueCalculated);
			return valueCalculated;
		}

		public void SubstituteBarsWithoutRecalculation(DataSeriesTimeBased averageFor) {
			this.AverageFor = averageFor;
		}

		public double CalculateOwnValue(Bar newStaticBar) {
			// COPYPASTE_FROM_IndicatorAverageMovingSimple:Indicator BEGIN
			if (this.Period <= 0) return double.NaN;
			if (this.AverageFor.Count - 1 < this.Period) return double.NaN;
			if (newStaticBar.ParentBarsIndex  < this.Period - 1) return double.NaN;
			
			DataSeriesProxyBars barsBehind = this.AverageFor as DataSeriesProxyBars;
			if (barsBehind != null) {
				if (barsBehind.BarsBeingProxied != newStaticBar.ParentBars) {
					string msg = "YOU_FORGOT_TO_RESTORE_ORIGINAL_BARS_BEFORE_UNPAUSING_QUOTE_PUMP";
					if (newStaticBar.ParentBarsIndex >= barsBehind.Count) {
						msg = "AVOIDING_OUT_OF_BOUNDARY_EXCEPTION_FOR_this.AverageFor[i] " + msg;
					}
					Debugger.Break();
					Assembler.PopupException(msg, null, false);
				}
			}

			double sum = 0;
			int slidingWindowRightBar = newStaticBar.ParentBarsIndex;
			int slidingWindowLeftBar = slidingWindowRightBar - this.Period + 1;	// FirstValidBarIndex must be Period+1
			int barsProcessedCheck = 0;
			for (int i = slidingWindowLeftBar; i <= slidingWindowRightBar; i++) {
				double eachBarInSlidingWindow = this.AverageFor[i];
				if (double.IsNaN(eachBarInSlidingWindow)) {
					Debugger.Break();
					continue;
				}
				sum += eachBarInSlidingWindow;
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
