using System;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

using Sq1.Indicators;

namespace Sq1.Strategies.Demo {
	public class ATRbandCompiled : Script {
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 

		IndicatorAverageTrueRange ATR;	// first  declared => first  vcalculated, although Reflection doesn't guarantee the order of variables
		IndicatorAtrBand ATRband;		// second declared => second calculated
		ScriptParameter test;			// ScriptParameters follow after Indicators' parameters in Sliders
		
		public ATRbandCompiled() {
			this.ATR = new IndicatorAverageTrueRange();
			// this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 5, 40, 5);

			//TESTME Slider for IndicatorParameter("Period", 5, 1, 11, 2) has 2-4-6-8 instead of 1-3-5-7
			//WHEN_I_WANT_OVERRIDE_CREATED_IN_Atr.Ctor()	this.ATR.ParamPeriod = new IndicatorParameter("Period", 5, 1, 11, 2);	// 1-3-5-7-9-11
			//AREADY_CREATED_IN_Atr.Ctor()					this.ATR.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			//AREADY_CREATED_IN_Atr.Ctor()					this.ATR.DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			this.ATR.LineColor = Color.Olive;

			this.ATRband = new IndicatorAtrBand(this.ATR);
			this.ATRband.LineColor = Color.RosyBrown;		// ALREADY_COPIED_FROM_ATR_BY_CTOR

			//base.ScriptParameterCreateRegister(1, "test", 0, 0, 10, 1);
			this.test = new ScriptParameter(1, "test", 0, 0, 10, 1, "??hopefully this will go to the tooltip");
		}
		public override void InitializeBacktest() {
		}
		public override void OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(Bar barStaticFormed) {
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
