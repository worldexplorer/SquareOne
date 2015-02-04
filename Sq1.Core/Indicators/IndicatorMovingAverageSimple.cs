using System;

using System.Diagnostics;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Core.Indicators {
	public class IndicatorMovingAverageSimple : Indicator {
		public	IndicatorParameter		ParamPeriod;	// Indicator searches for IndicatorParameter being fields, not properties
				MovingAverageSimple		smaSeries;
		
		public override int FirstValidBarIndex {
			get { return (int)this.ParamPeriod.ValueCurrent; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			protected set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorMovingAverageSimple() : base() {
			base.Name = "MA";
			// NOW DEFAULT base.ChartPanelType = ChartPanelType.PanelPrice;
			ParamPeriod = new IndicatorParameter("Period", 55, 11, 99, 11);
		}
		void checkPopupOnResetAndSync(string msig) {
			string msg = "";
			if (this.BarsEffective.Count != this.ClosesProxyEffective.Count) {
				msg = "IT_WASNT_A_PARANOID_CHECK";
				Debugger.Break();
				Assembler.PopupException(msg, null, false);
			}

			if (this.smaSeries.AverageFor.Count != base.BarsEffective.Count) {
				msg += "BARS_FOR_INDICATOR_AND_INTERNAL_SMA_MUST_BE_EQUAL_LENGTH ";
			}
			if (this.smaSeries.AverageFor.Count != base.ClosesProxyEffective.Count) {
				msg += "BARS_UNDERNEATH_INDICATOR_HAVE_DIFFERENT_LENGTH_WITH_ABSORBED ";
			}
			if (this.smaSeries.AverageFor.Count > 0) {
				if (this.smaSeries.AverageFor.Count != this.smaSeries.Count + (int)this.ParamPeriod.ValueCurrent) {
					msg += "INTERNAL_SMA_MUST_HAVE_COUNT_EQUALS_BARS_MINUS_PERIOD ";
				}
				if (this.smaSeries.AverageFor.Count != this.OwnValuesCalculated.Count + 1) {
					msg += "SOME_BARS_HAVE_NO_MATCHING_INDICATOR_CALCULATED ";
				} else {
					string hint = "indicator value for the current-last-bar will be calculated by next-bar incoming quote meaning official closing of current-last-bar";
				}
			}
			if (string.IsNullOrEmpty(msg)) return;
			Assembler.PopupException(msg + msig);
		}
		public override void BacktestContextRestoreSwitchToOriginalBarsContinueToLiveNorecalculate() {
			base.BacktestContextRestoreSwitchToOriginalBarsContinueToLiveNorecalculate();
			
			if (base.ClosesProxyEffective.Count == 0) {
				string msg = "PARANOID AT_BACKTEST_CONTEXT_RESTORE_ClosesProxyEffective.Count_MUST_BE_NOT_0";
				Assembler.PopupException(msg);
			}
			string msig = " //BacktestStartingResetBarsEffectiveProxy() ";

			if (this.smaSeries.AverageFor == base.ClosesProxyEffective) {
				string msg = "MISUSE_UPSTACK__NO_POINT_OF_INVOKING_ME MUST_BE_SAME_AND_ARE smaSeries.AverageFor=base.ClosesProxyEffective";
				Assembler.PopupException(msg + msig);
				this.checkPopupOnResetAndSync(msig);
				return;
			}
			this.smaSeries.AverageFor = base.ClosesProxyEffective;
			this.checkPopupOnResetAndSync(msig);
		}
		public override void BacktestStartingResetBarsEffectiveProxy() {
			string msig = " //BacktestStartingResetBarsEffectiveProxy() EMPTY_CLONE_BARS_AT_BACKTEST_START ";
			base.BacktestStartingResetBarsEffectiveProxy();

			if (base.ClosesProxyEffective.Count != 0) {
				string msg = "AT_BACKTEST_CONTEXT_INITIALIZE_ClosesProxyEffective.Count_MUST_BE_0";
				Assembler.PopupException(msg);
			}
			
			string state = "";
			//if (this.smaSeries == null) {
			//	state = "FIRST_BACKTEST_AFTER_APP_RESTART";
				this.smaSeries = new MovingAverageSimple(base.ClosesProxyEffective, (int)this.ParamPeriod.ValueCurrent, base.Decimals);
			//	return;
			//}
			//state = "SECOND_AND_FOLLOWING__BOTH_DISCONNECTED_OR_LIVE_BACKTESTS_AFTER_APP_RESTART";
			
			//if (this.smaSeries.AverageFor == base.ClosesProxyEffective) {
			//	string msg = "MISUSE_UPSTACK__NO_POINT_OF_INVOKING_ME MUST_BE_SAME_AND_ARE smaSeries.AverageFor=base.ClosesProxyEffective";
			//	Assembler.PopupException(msg + msig);
			//	return;
			//}

			//this.smaSeries.AverageFor = base.ClosesProxyEffective;
			this.checkPopupOnResetAndSync(msig + state);
		}

		public override string InitializeBacktestStartingPreCheckErrors() {
			if (this.ParamPeriod.ValueCurrent <= 0) return "Period[" + this.ParamPeriod.ValueCurrent + "] MUST BE > 0";
			return null;
		}
		
		public override double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			double ret = double.NaN;

			//if (this.ParamPeriod.ValueCurrent <= 0) {
			//    string msg = "this.ParamPeriod.ValueCurrent <= 0";
			//    Assembler.PopupException(msg);
			//    return ret;
			//}

			#region DELETEME_AFTER_COMPATIBILITY_TEST
			try {
				bool duplicateFound = false;
				double alreadyExistingValue = double.NegativeInfinity;
				if (base.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) {
					duplicateFound = true;
					alreadyExistingValue = base.OwnValuesCalculated[newStaticBar.DateTimeOpen];
				}
				if (duplicateFound && double.IsNegativeInfinity(alreadyExistingValue) == true && double.IsNaN(alreadyExistingValue) == false) {
					string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW";
					if (newStaticBar.DateTimeOpen < base.OwnValuesCalculated.LastDateAppended) {
						msg = "DONT_INVOKE_ME_TWICE [" + base.OwnValuesCalculated.LastValueAppended + "]"
							+ "newStaticBar.DateTimeOpen[" + newStaticBar.DateTimeOpen + "] < LastDateAppended[" + base.OwnValuesCalculated.LastDateAppended + "]";
						Assembler.PopupException(msg);
						return double.NaN;
					} else {
						msg = "DURING_INCUBATION_EACH_QUOTE_ADDS_NAN_SO_ON_STATIC_FORMED_THERE_IS_LEGITIMATE_VALUE ";
					}
				}
			} catch (Exception ex) {
				string msg = "QUE_PASHA???";
				Assembler.PopupException(msg);
			}
			#endregion

			ret = this.smaSeries.CalculateAppendOwnValueForNewStaticBarFormedNanUnsafe(newStaticBar);

			#region DELETEME_AFTER_COMPATIBILITY_TEST
			#if TEST_COMPATIBILITY
			double sum = 0;
			int slidingWindowRightBar = newStaticBar.ParentBarsIndex;
			int slidingWindowLeftBar = slidingWindowRightBar - this.ParamPeriod.ValueCurrent + 1;	// FirstValidBarIndex must be Period+1
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
			if (barsProcessedCheck != this.ParamPeriod.ValueCurrent) {
				Debugger.Break();
			}
			double retOld = sum / this.ParamPeriod.ValueCurrent;
			
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
