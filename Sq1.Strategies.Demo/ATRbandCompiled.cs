using System;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	// REASON_TO_EXIST_NEW_FEATURE: test for ATR in separatePanel: ChartPanelType=ChartPanelType.PanelIndicatorSingle
	// REASON_TO_EXIST_NEW_FEATURE: pull indicator parameters onto ScriptContext's SteppingSliders
	// REASON_TO_EXIST_NEW_FEATURE: test for onPrice IndicatorDualBand(Bars.Close+-ATR)
	public class ATRbandCompiled : Script {
		
		[IndicatorParameterAttribute(Name="Period",
			ValueCurrent=5, ValueMin=1, ValueMax=11, ValueIncrement=2)]
		public IndicatorAverageTrueRange ATR { get; set; }

		public ATRbandCompiled() {
		}
		
		public override void InitializeBacktest() {
			if (this.ATR == null) {
				this.ATR = new IndicatorAverageTrueRange();
				this.ATR.Period = 9;
			}
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
