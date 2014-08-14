using System;
using System.Diagnostics;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Strategies.Demo {
	public class TwoMAsCompiled : Script {
		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=55, ValueMin=11, ValueMax=88, ValueIncrement=11)]
		public IndicatorAverageMovingSimple MAslow { get; set; }

		[IndicatorParameterAttribute(Name = "Period",
			ValueCurrent = 15, ValueMin = 10, ValueMax = 20, ValueIncrement = 1)]
		public IndicatorAverageMovingSimple MAfast { get; set; }
		
		public int PeriodLargestAmongMAs { get {
				int ret = this.MAfast.Period;
				if (ret > this.MAslow.Period) ret = this.MAslow.Period; 
				return ret;
			} }

		public override void InitializeBacktest() {
			this.MAslow.LineColor = System.Drawing.Color.LightCoral;
			this.MAfast.LineColor = System.Drawing.Color.LightSeaGreen;
		}
		public override void OnNewQuoteOfStreamingBarCallback(Quote quote) {
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
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

	}
}
