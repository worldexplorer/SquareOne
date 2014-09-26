using System;
using System.Drawing;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	// REASON_TO_EXIST_NEW_FEATURE: test for ATR in separatePanel: ChartPanelType=ChartPanelType.PanelIndicatorSingle
	// REASON_TO_EXIST_NEW_FEATURE: pull indicator parameters onto ScriptContext's SteppingSliders
	// REASON_TO_EXIST_NEW_FEATURE: test for onPrice IndicatorDualBand(Bars.Close+-ATR)

	[ScriptParameterAttribute(Id=1, Name="test", ValueMin=0, ValueMax=10, ValueIncrement=1 )]

	public class ATRbandCompiled : Script {
		
		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=5, ValueMin=1, ValueMax=11, ValueIncrement=2)]
		[IndicatorParameterAttribute(Name="Multiplier",
			ValueCurrent=1.5, ValueMin=0.1, ValueMax=4, ValueIncrement=0.1)]
		public IndicatorAverageTrueRange ATR { get; set; }

		public ATRbandCompiled() {
			// CLEANER_SCRIPT_PARAMETERS
			// CLEANER_INDICATORS
//			this.ATR = new IndicatorAverageTrueRange();
//			this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 1, 11, 2);
//			this.ATR.ParamMultiplier = new IndicatorParameter("Multiplier", 1.5, 0.1, 4, 0.1);
//			this.ATR.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
//			this.ATR.DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
//			this.ATR.LineColor = Color.DarkOliveGreen;
		}
		
		public override void InitializeBacktest() {
//			if (this.ATR == null) {
//				this.ATR = new IndicatorAverageTrueRange();
//				this.ATR.Period = 9;
//			}
			this.ATR.LineColor = Color.DarkOliveGreen;
		}

		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
		}
//		public override void OnNewQuoteOfStreamingBarCallback(Quote quote) {
//		}
//		public override void OnAlertFilledCallback(Alert alertFilled) {
//		}
//		public override void OnAlertKilledCallback(Alert alertKilled) {
//		}
//		public override void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
//		}
//		public override void OnPositionOpenedCallback(Position positionOpened) {
//		}
//		public override void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedByPrototype) {
//		}
//		public override void OnPositionClosedCallback(Position positionClosed) {
//		}
	}
}
