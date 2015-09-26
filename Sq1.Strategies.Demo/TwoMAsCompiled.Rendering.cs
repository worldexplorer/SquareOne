using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Charting.OnChart;

namespace Sq1.Strategies.Demo {
	public partial class TwoMAsCompiled {
		Font fontArial6;

		void constructRenderingTools() {
			fontArial6 = new Font("Arial", 6);
		}
		void drawLinesSample(Bar barStaticFormed) {
			Bar barFirstForCurrentTradingDay = barStaticFormed.BarMarketOpenedTodayScanBackwardIgnoringMarketInfo;
			double dayOpenedAtPrice = barFirstForCurrentTradingDay.Open;
			
			// one line is drawn across one day regardless of timeframe: just the date is enough to "address" the line 
			string lineId = barFirstForCurrentTradingDay.DateTimeOpen.ToString("yyyy-MMM-dd");
			//Debugger.Break();
			base.Executor.ChartConditionalLineDrawModify(lineId,
				barFirstForCurrentTradingDay.ParentBarsIndex, dayOpenedAtPrice,
				barStaticFormed.ParentBarsIndex, dayOpenedAtPrice,
				Color.Blue, 1);

			
			double upperLimit = dayOpenedAtPrice + dayOpenedAtPrice * 0.005;	//143.200 + 716 = 143.916 - most likely visible on the chart, not beoynd
			double lowerLimit = dayOpenedAtPrice - dayOpenedAtPrice * 0.005;
			
			base.ChartConditionalLineDrawModify(lineId + "_red",
				barFirstForCurrentTradingDay.ParentBarsIndex, upperLimit,
				barStaticFormed.ParentBarsIndex, upperLimit,
				Color.Red, 2);
			base.ChartConditionalLineDrawModify(lineId + "_green",
				barFirstForCurrentTradingDay.ParentBarsIndex, lowerLimit,
				barStaticFormed.ParentBarsIndex, lowerLimit,
				Color.Green, 2);
			
			
			if (barStaticFormed == barFirstForCurrentTradingDay) {
				OnChartObjectOperationStatus status = base.Executor.ChartConditionalLineDrawModify(lineId + "_brown",
					barStaticFormed.ParentBarsIndex, lowerLimit,
					barStaticFormed.ParentBarsIndex, upperLimit,
					Color.Brown, 3);
				if (status != OnChartObjectOperationStatus.OnChartObjectJustCreated) {
					string msg = "NEVER_HAPPENED_SO_FAR status[" + status + "] != OnChartObjectOperationStatus.OnChartObjectJustCreated";
					Assembler.PopupException(msg);
				}
			}

			if (base.Executor.Backtester.BarsOriginal == null) {
				string msg = "I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL base.Executor.Backtester.BarsOriginal == null";
				//Assembler.PopupException(msg + " //Script=" + this.ToString());
				return;
			}

			if (base.Bars.Count == base.Executor.Backtester.BarsOriginal.Count) {
				base.ChartConditionalLineDrawModify("acrossAllBars",
					0, base.Bars.BarStaticFirstNullUnsafe.Open,
					base.Bars.BarStaticLastNullUnsafe.ParentBarsIndex, base.Bars.BarStaticLastNullUnsafe.Open,
					Color.Goldenrod, 1);
			}
		}
		void testBarBackground(Bar barStaticFormed) {
			Color bg = (barStaticFormed.Open > barStaticFormed.Close) ? Color.LightGreen : Color.LightSalmon;
			base.ChartConditionalBarBackgroundSet(barStaticFormed.ParentBarsIndex, bg);
		}
		void testBarAnnotations(Bar barStaticFormed) {
			int barIndex = barStaticFormed.ParentBarsIndex;
			string labelText = barStaticFormed.DateTimeOpen.ToString("HH:mm");
			labelText += " " + barStaticFormed.BarIndexAfterMidnightReceived + "/";
			labelText += barStaticFormed.BarIndexExpectedSinceTodayMarketOpen + ":" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			bool evenAboveOddBelow = (barStaticFormed.ParentBarsIndex % 2) == 0;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "ann" + barIndex, labelText, this.fontArial6, Color.ForestGreen, Color.Empty, evenAboveOddBelow);
		}
	}
}
