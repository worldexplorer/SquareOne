using System;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	public class TwoMAindicatorsCompiled : Script {

		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		IndicatorMovingAverageSimple MAslow;
		IndicatorMovingAverageSimple MAfast;

		public TwoMAindicatorsCompiled() {
			MAfast = new IndicatorMovingAverageSimple();
			MAfast.ParamPeriod = new IndicatorParameter("Period", 55, 11, 88, 11);
			MAfast.LineColor = System.Drawing.Color.LightSeaGreen;

			MAslow = new IndicatorMovingAverageSimple();
			MAslow.ParamPeriod = new IndicatorParameter("Period", 15, 10, 20, 1);
			MAslow.LineColor = System.Drawing.Color.LightCoral;
		}

		public override void InitializeBacktest() {
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
			this.testBarBackground(barStaticFormed);
			this.testBarAnnotationsMarkBarsShiftedDueToMissedBar(barStaticFormed);
		}
		void testBarBackground(Bar barStaticFormed) {
			Color bg = (barStaticFormed.Open > barStaticFormed.Close) ? Color.LightGreen : Color.LightSalmon;
			base.Executor.ChartConditionalBarBackgroundSet(barStaticFormed.ParentBarsIndex, bg);
		}
		void testBarAnnotationsMarkBarsShiftedDueToMissedBar(Bar barStaticFormed) {
			if (barStaticFormed.BarIndexAfterMidnightReceived == barStaticFormed.BarIndexExpectedSinceTodayMarketOpen) return; 
			
			int barIndex = barStaticFormed.ParentBarsIndex;
			Font font = new Font("Arial", 7);
			//bool evenAboveOddBelow = (barStaticFormed.ParentBarsIndex % 2) == 0;
			bool evenAboveOddBelow = false;
			int stickToHorizontalEdgesOfChart = 0;	//Int32.MaxValue
			stickToHorizontalEdgesOfChart = Int32.MaxValue;
			
			//string labelTime = barStaticFormed.DateTimeOpen.ToString("HH:mm");
			//base.Executor.ChartConditionalBarAnnotationDrawModify(
			//	barIndex, "ann1" + barIndex, labelTime, font, Color.Black, Color.Empty, evenAboveOddBelow);

			string labelReceived = "#" + barStaticFormed.BarIndexAfterMidnightReceived;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annReceived" + barIndex, labelReceived, font,
				Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpAfterOpen = "ao" + barStaticFormed.BarIndexExpectedSinceTodayMarketOpen;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annAfterOpen" + barIndex, labelExpAfterOpen,
				font, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpBeforeClose = "bc" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "annBeforeClose" + barIndex, labelExpBeforeClose,
				font, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
		}
	}
}
