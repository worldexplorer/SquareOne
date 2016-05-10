using System;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Indicators.HelperSeries;

namespace Sq1.Indicators {
	public class IndicatorAverageTrueRange : Indicator {
		public	IndicatorParameter		ParamPeriod;	// Indicator searches for IndicatorParameter being fields, not properties
				TrueRangeSeries			trueRangeSeries;
				MovingAverageSimple		smaSeries;

		public override int FirstValidBarIndex {
						get { return (int)this.ParamPeriod.ValueCurrent; }		// Period = 15, 0..14 are NaN, index=15 has valueCalculated
			protected	set { throw new Exception("I_DONT_ACCEPT_SETTING_OF_FirstValidBarIndex " + this.NameWithParameters); }
		}
		
		public IndicatorAverageTrueRange() : base() {
			base.Name = "ATR //will be replaced by Script.cs:179.IndicatorsByName_ReflectedCached";
			base.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			this.ParamPeriod = new IndicatorParameter("Period", 5, 1, 11, 2);
		}
		public override void BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries() {
			string msig = " //BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries() EMPTY_CLONE_BARS_AT_BACKTEST_START ";
			base.BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries();

			//if (base.ClosesProxyEffective.Count != 0) {
			//    string msg = "AT_BACKTEST_CONTEXT_INITIALIZE_ClosesProxyEffective.Count_MUST_BE_0";
			//    Assembler.PopupException(msg);
			//}

			string state = "";
			if (this.trueRangeSeries == null) {
				state = "FIRST_BACKTEST_AFTER_APP_RESTART";
				// not in ctor because base.BarsEffective should not be null; initialized only now in Indicator.BacktestStarting() upstack
				this.trueRangeSeries = new TrueRangeSeries(base.OwnValuesCalculated.ScaleInterval);
				this.smaSeries = new MovingAverageSimple(this.trueRangeSeries, this.ParamPeriod.ValueCurrentAsInteger, base.Decimals);
				return;
			}

			state = "SECOND_AND_FOLLOWING__BOTH_DISCONNECTED_OR_LIVE_BACKTESTS_AFTER_APP_RESTART";
			if (this.smaSeries.Period != this.ParamPeriod.ValueCurrentAsInteger) {
			    this.smaSeries.Period  = this.ParamPeriod.ValueCurrentAsInteger;
				base.RaiseOnIndicatorPeriodChanged();
			}
			
			if (this.smaSeries.Count > 0) {
			    string msg1 = "CLEARING_FOR_NEXT_BACKTEST__OTHERWIZE_sma.CalculateOwnValue()_WILL_COMPLAIN_ON_SAME_VALUES_ALREADY_THERE";
			    //Assembler.PopupException(msg1, null, false);
			    this.smaSeries.Clear();
				this.trueRangeSeries.Clear();
			}

			//v1 this.smaSeries.AverageFor = base.ClosesProxyEffective;
			if (this.smaSeries.AverageFor == this.trueRangeSeries) {
			    string msg = "MISUSE_UPSTACK__NO_POINT_OF_INVOKING_ME MUST_BE_SAME_AND_ARE smaSeries.AverageFor=this.trueRangeSeries";
			    //Assembler.PopupException(msg + msig);
			    return;
			}

			this.smaSeries.SubstituteBars_withoutRecalculation(this.trueRangeSeries);
			// NOISY this.checkPopupOnResetAndSync(msig + state);
		}
		public override string InitializeBacktest_beforeStarted_checkErrors() {
			if (this.ParamPeriod.ValueCurrent <= 0) return "Period[" + this.ParamPeriod.ValueCurrent + "] MUST BE > 0";
			//MOVED_UPSTACK_FOR_ATRBAND_TO_FORMAT_ITS_SMALL_VALUES_AS_WELL  base.Decimals = Math.Max(base.Executor.Bars.SymbolInfo.DecimalsPrice, 1);	// for SBER, constant ATR shows truncated (imprecise) mouseOver value on gutter
			return null;
		}
		
		public override double CalculateOwnValue_onNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(Bar newStaticBar) {
			this.trueRangeSeries.CalculateOwnValue_onNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(newStaticBar);
			if (this.ParamPeriod.ValueCurrent <= 0) return double.NaN;
			double ret = this.smaSeries.CalculateOwnValue_append_forNewStaticBarFormed_NanUnsafe(newStaticBar);
			return ret;
		}
	}
}
