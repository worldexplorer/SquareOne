using System;
using System.Drawing;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	// REASON_TO_EXIST_NEW_FEATURE: pull indicator parameters onto ScriptContext's SteppingSliders
	public class TwoMAindicatorsCompiled : Script {
		// TODO: initialize MAslow.LineColor using an attribute? 
		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=55, ValueMin=11, ValueMax=88, ValueIncrement=11)]
		public IndicatorMovingAverageSimple MAslow { get; set; }

		// TODO: show how to construct/tunnelParameters without Attribute
		[IndicatorParameterAttribute(Name = "Period",
			ValueCurrent = 15, ValueMin = 10, ValueMax = 20, ValueIncrement = 1)]
		public IndicatorMovingAverageSimple MAfast { get; set; }
		

		//public TwoMAindicatorsCompiled() { }

		public override void InitializeBacktest() {
			this.MAslow.LineColor = System.Drawing.Color.LightCoral;
			this.MAfast.LineColor = System.Drawing.Color.LightSeaGreen;
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
			this.testBarBackground(barStaticFormed);
			this.testBarAnnotationsMarkBarsShiftedDueToMissedBar(barStaticFormed);
		}
		void testBarBackground(Bar barStaticFormed) {
			Color bg = (barStaticFormed.Open > barStaticFormed.Close) ? Color.LightGreen : Color.LightSalmon;
			base.Executor.ChartShadow.BarBackgroundSet(barStaticFormed.ParentBarsIndex, bg);
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
			//base.Executor.ChartShadow.BarAnnotationDrawModify(
			//	barIndex, "ann1" + barIndex, labelTime, font, Color.Black, Color.Empty, evenAboveOddBelow);

			string labelReceived = "#" + barStaticFormed.BarIndexAfterMidnightReceived;
			base.Executor.ChartShadow.BarAnnotationDrawModify(
				barIndex, "annReceived" + barIndex, labelReceived, font,
				Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpAfterOpen = "ao" + barStaticFormed.BarIndexExpectedSinceTodayMarketOpen;
			base.Executor.ChartShadow.BarAnnotationDrawModify(
				barIndex, "annAfterOpen" + barIndex, labelExpAfterOpen,
				font, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
			
			string labelExpBeforeClose = "bc" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			base.Executor.ChartShadow.BarAnnotationDrawModify(
				barIndex, "annBeforeClose" + barIndex, labelExpBeforeClose,
				font, Color.Black, Color.Empty, evenAboveOddBelow, stickToHorizontalEdgesOfChart);
		}
	}
}
