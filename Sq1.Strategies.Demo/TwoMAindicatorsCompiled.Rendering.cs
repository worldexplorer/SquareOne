using System;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	public partial class TwoMAindicatorsCompiled {
		Font fontArial7;
		
		void testBarBackground(Bar barStaticFormed) {
			if (this.Executor.Optimizer.IsRunningNow) return;
			
			Color bg = (barStaticFormed.Open > barStaticFormed.Close) ? Color.LightGreen : Color.LightSalmon;
			base.Executor.ChartConditionalBarBackgroundSet(barStaticFormed.ParentBarsIndex, bg);
		}
		void testBarAnnotationsMarkBarsShiftedDueToMissedBar(Bar barStaticFormed) {
			if (this.Executor.Optimizer.IsRunningNow) return;
			
			if (barStaticFormed.BarIndexAfterMidnightReceived == barStaticFormed.BarIndexExpectedSinceTodayMarketOpen) return; 
			
			int barIndex = barStaticFormed.ParentBarsIndex;
			//bool evenAboveOddBelow = (barStaticFormed.ParentBarsIndex % 2) == 0;
			bool evenAboveOddBelow = false;
			int stickToHorizontalEdgesOfChart = 0;	//Int32.MaxValue
			stickToHorizontalEdgesOfChart = Int32.MaxValue;
			
			//string labelTime = barStaticFormed.DateTimeOpen.ToString("HH:mm");
			//base.Executor.ChartConditionalBarAnnotationDrawModify(
			//	barIndex, "ann1" + barIndex, labelTime, font, Color.Black, Color.Empty, evenAboveOddBelow);

			string labelReceived = "#" + barStaticFormed.BarIndexAfterMidnightReceived;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annReceived" + barIndex, labelReceived, this.fontArial7,
				Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpAfterOpen = "ao" + barStaticFormed.BarIndexExpectedSinceTodayMarketOpen;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annAfterOpen" + barIndex, labelExpAfterOpen,
				this.fontArial7, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpBeforeClose = "bc" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annBeforeClose" + barIndex, labelExpBeforeClose,
				this.fontArial7, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
		}
	}
}
