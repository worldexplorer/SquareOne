using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Strategies.Demo {
	public class TwoMAsCompiled : Script {
		// if an indicator isn't a property it won't show up in Sliders
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		public IndicatorMovingAverageSimple MAslow { get; set; }
		public IndicatorMovingAverageSimple MAfast { get; set; }

		public TwoMAsCompiled() {
			MAfast = new IndicatorMovingAverageSimple();
			MAfast.ParamPeriod = new IndicatorParameter("Period", 55, 11, 88, 11);
			MAfast.LineColor = System.Drawing.Color.LightSeaGreen;

			MAslow = new IndicatorMovingAverageSimple();
			MAslow.ParamPeriod = new IndicatorParameter("Period", 15, 10, 20, 1);
			MAslow.LineColor = System.Drawing.Color.LightCoral;
		}
		
		public int PeriodLargestAmongMAs { get {
				int ret = (int)this.MAfast.ParamPeriod.ValueCurrent;
				if (ret > (int)this.MAslow.ParamPeriod.ValueCurrent) ret = (int)this.MAslow.ParamPeriod.ValueCurrent; 
				return ret;
			} }

		public override void InitializeBacktest() {
		}
		public override void OnNewQuoteOfStreamingBarCallback(Quote quote) {
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
			this.drawLinesSample(barStaticFormed);
			//this.testBarBackground(barStaticFormed);
			//this.testBarAnnotations(barStaticFormed);
			
			Bar barStreaming = barStaticFormed.ParentBars.BarStreaming;
			if (barStaticFormed.ParentBarsIndex <= this.PeriodLargestAmongMAs) return;

			double maSlowThis = this.MAslow.OwnValuesCalculated[barStaticFormed.ParentBarsIndex];
			double maSlowPrev = this.MAslow.OwnValuesCalculated[barStaticFormed.ParentBarsIndex - 1];

			double maFastThis = this.MAfast.OwnValuesCalculated[barStaticFormed.ParentBarsIndex];
			double maFastPrev = this.MAfast.OwnValuesCalculated[barStaticFormed.ParentBarsIndex - 1];

			bool fastCrossedUp = false;
			if (maFastThis > maSlowThis && maFastPrev < maSlowPrev) fastCrossedUp = true; 
				
			bool fastCrossedDown = false;
			if (maFastThis < maSlowThis && maFastPrev > maSlowPrev) fastCrossedDown = true;

			if (fastCrossedUp && fastCrossedDown) {
				string msg = "TWO_CROSSINGS_SHOULD_NEVER_HAPPEN_SIMULTANEOUSLY";
				Assembler.PopupException(msg);
				Debugger.Break();
			}
			bool crossed = fastCrossedUp || fastCrossedDown;
				
			Position lastPos = base.LastPosition;
			bool isLastPositionNotClosedYet = base.IsLastPositionNotClosedYet;
			if (isLastPositionNotClosedYet && crossed) {
				string msg = "ExitAtMarket@" + barStaticFormed.ParentBarsIdent;
				Alert exitPlaced = ExitAtMarket(barStreaming, lastPos, msg);
			}

			if (fastCrossedUp) {
				string msg = "BuyAtMarket@" + barStaticFormed.ParentBarsIdent;
				Position buyPlaced = BuyAtMarket(barStreaming, msg);
			}
			if (fastCrossedDown) {
				string msg = "ShortAtMarket@" + barStaticFormed.ParentBarsIdent;
				Position shortPlaced = ShortAtMarket(barStreaming, msg);
			}
		}
		public override void OnAlertFilledCallback(Alert alertFilled) {
		}
		public override void OnAlertKilledCallback(Alert alertKilled) {
		}
		public override void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
		}
		public override void OnPositionOpenedCallback(Position positionOpened) {
		}
		public override void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedByPrototype) {
		}
		public override void OnPositionClosedCallback(Position positionClosed) {
		}

		void drawLinesSample(Bar barStaticFormed) {
			Bar barFirstForCurrentTradingDay = barStaticFormed.BarMarketOpenedTodayScanBackwardIgnoringMarketInfo;
			double dayOpenedAtPrice = barFirstForCurrentTradingDay.Open;
			
			// one line is drawn across one day regardless of timeframe: just the date is enough to "address" the line 
			string lineId = barFirstForCurrentTradingDay.DateTimeOpen.ToString("yyyy-MMM-dd");
			//Debugger.Break();
			base.Executor.ChartShadow.LineDrawModify(lineId,
				barFirstForCurrentTradingDay.ParentBarsIndex, dayOpenedAtPrice,
				barStaticFormed.ParentBarsIndex, dayOpenedAtPrice,
				Color.Blue, 1);

			
			double upperLimit = dayOpenedAtPrice + dayOpenedAtPrice * 0.005;	//143.200 + 716 = 143.916 - most likely visible on the chart, not beoynd
			double lowerLimit = dayOpenedAtPrice - dayOpenedAtPrice * 0.005;
			
			base.Executor.ChartShadow.LineDrawModify(lineId + "_red",
				barFirstForCurrentTradingDay.ParentBarsIndex, upperLimit,
				barStaticFormed.ParentBarsIndex, upperLimit,
				Color.Red, 2);
			base.Executor.ChartShadow.LineDrawModify(lineId + "_green",
				barFirstForCurrentTradingDay.ParentBarsIndex, lowerLimit,
				barStaticFormed.ParentBarsIndex, lowerLimit,
				Color.Green, 2);
			
			
			if (barStaticFormed == barFirstForCurrentTradingDay) {
				OnChartObjectOperationStatus status = base.Executor.ChartShadow.LineDrawModify(lineId + "_brown",
					barStaticFormed.ParentBarsIndex, lowerLimit,
					barStaticFormed.ParentBarsIndex, upperLimit,
					Color.Brown, 3);
				if (status != OnChartObjectOperationStatus.OnChartObjectJustCreated) {
					Debugger.Break();
				}
			}

			if (base.Bars.Count == base.Executor.Backtester.BarsOriginal.Count) {
				base.Executor.ChartShadow.LineDrawModify("acrossAllBars",
					0, base.Bars.BarStaticFirstNullUnsafe.Open,
					base.Bars.BarStaticLastNullUnsafe.ParentBarsIndex, base.Bars.BarStaticLastNullUnsafe.Open,
					Color.Goldenrod, 1);
			}
		}
		void testBarBackground(Bar barStaticFormed) {
			Color bg = (barStaticFormed.Open > barStaticFormed.Close) ? Color.LightGreen : Color.LightSalmon;
			base.Executor.ChartShadow.BarBackgroundSet(barStaticFormed.ParentBarsIndex, bg);
		}
		void testBarAnnotations(Bar barStaticFormed) {
			int barIndex = barStaticFormed.ParentBarsIndex;
			string labelText = barStaticFormed.DateTimeOpen.ToString("HH:mm");
			labelText += " " + barStaticFormed.BarIndexAfterMidnightReceived + "/";
			labelText += barStaticFormed.BarIndexExpectedSinceTodayMarketOpen + ":" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			Font font = new Font("Arial", 6);
			bool evenAboveOddBelow = (barStaticFormed.ParentBarsIndex % 2) == 0;
			base.Executor.ChartShadow.BarAnnotationDrawModify(
				barIndex, "ann" + barIndex, labelText, font, Color.ForestGreen, Color.Empty, evenAboveOddBelow);
		}
	}
}
