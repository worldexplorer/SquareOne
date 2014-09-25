using System;
using System.Diagnostics;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Indicators.HelperSeries;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Indicators {
	public class IndicatorMovingAverageSimple : Indicator {

		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=55, ValueMin=11, ValueMax=99, ValueIncrement=11)]
		public int Period { get; set; }
		private MovingAverageSimple smaSeries;
		
		public override int FirstValidBarIndex {
			get { return this.Period; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorMovingAverageSimple() : base() {
			base.Name = "MA";
			base.ChartPanelType = ChartPanelType.PanelPrice;
		}
		
		public override string BacktestStartingPreCheckErrors() {
			if (this.Period <= 0) return "Period[" + this.Period + "] MUST BE > 0";
			this.smaSeries = new MovingAverageSimple(base.ClosesProxyEffective, this.Period, base.OwnValuesCalculated.ScaleInterval);
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			#region DELETEME_AFTER_COMPATIBILITY_TEST
			if (base.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) {
				string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW";
				if (base.OwnValuesCalculated.LastDateAppended > newStaticBar.DateTimeOpen) {
					msg = "DONT_INVOKE_ME_TWICE on[" + newStaticBar.DateTimeOpen + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					return double.NaN;
				} else {
					msg = "DURING_INCUBATION_EACH_QUOTE_ADDS_NAN_SO_ON_STATIC_FORMED_THERE_IS_LEGITIMATE_VALUE ";
				}
			}
			#endregion

			if (this.Period <= 0) return double.NaN;
			if (base.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) return double.NaN;
			if (newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex) return double.NaN;

			double ret = this.smaSeries.CalculateAppendOwnValueForNewStaticBarFormed(newStaticBar);

			#region DELETEME_AFTER_COMPATIBILITY_TEST
			#if TEST_COMPATIBILITY
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
			double retOld = sum / this.Period;
			
			if (retOld != ret) {
				Debugger.Break();
			} else {
				//Debugger.Break();
			}
			#endif
			#endregion

			return ret;
		}
	}
}
