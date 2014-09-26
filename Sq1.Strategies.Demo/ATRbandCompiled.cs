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

		// if an indicator isn't a property it won't show up in Sliders
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		public IndicatorAverageTrueRange ATR { get; set; }

		public ATRbandCompiled() {
			// CLEANER_SCRIPT_PARAMETERS
			// CLEANER_INDICATORS
			this.ATR = new IndicatorAverageTrueRange();
			this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 5, 40, 5);
			this.ATR.ParamMultiplier = new IndicatorParameter("Multiplier", 1.5, 0.1, 4, 0.1);
			// DEFAULT_PANEL_PRICE LEAVE_COMMENTED_OR_WONT_BE_DRAWN this.ATR.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			this.ATR.DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			this.ATR.LineColor = Color.DarkOliveGreen;
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
