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
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		public IndicatorAverageTrueRange ATR;
		public IndicatorAtrBand ATRband;
		
		public ATRbandCompiled() {
			// CLEANER_SCRIPT_PARAMETERS
			// CLEANER_INDICATORS
			this.ATR = new IndicatorAverageTrueRange();
			// this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 5, 40, 5);
			//TESTME Slider for IndicatorParameter("Period", 5, 1, 11, 2) has 2-4-6-8 instead of 1-3-5-7
			this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 1, 11, 2);	// 1-3-5-7-9-11
			this.ATR.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			this.ATR.DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			this.ATR.LineColor = Color.Olive;

			this.ATRband = new IndicatorAtrBand(this.ATR);
			// ALREADY_COPIED_FROM_ATR_BY_CTOR this.ATRband.LineColor = Color.RosyBrown;
		}
		public override void InitializeBacktest() {
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
