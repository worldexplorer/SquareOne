using System;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;
using Sq1.Adapters.Quik;

namespace Sq1.Strategies.Demo {
	public partial class TwoMAsCompiled : Script {
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		IndicatorMovingAverageSimple MAslow;
		IndicatorMovingAverageSimple MAfast;

		public TwoMAsCompiled() {
			MAslow = new IndicatorMovingAverageSimple();
			MAslow.ParamPeriod = new IndicatorParameter("Period", 16, 10, 20, 2);	//5);
			MAslow.LineColor = System.Drawing.Color.LightCoral;

			MAfast = new IndicatorMovingAverageSimple();
			MAfast.ParamPeriod = new IndicatorParameter("Period", 22, 11, 32, 3);	//11);
			MAfast.LineColor = System.Drawing.Color.LightSeaGreen;
			this.constructRenderingTools();
		}
		
		public int PeriodLargestAmongMAs { get {
				int ret = (int)this.MAfast.ParamPeriod.ValueCurrent;
				if (ret > (int)this.MAslow.ParamPeriod.ValueCurrent) ret = (int)this.MAslow.ParamPeriod.ValueCurrent; 
				return ret;
			} }

		public override void InitializeBacktest() {
			string msg = "HERE_I_SHOULD_CATCH_NEW_MAS_PERIODS_CHANGED_AFTER_CLICK_ON_PARAMETERS_SLIDERS";
			//Assembler.PopupException(msg, null, false);
			this.printedQuoteTypeOncePerBacktest = false;
		}

		bool printedQuoteTypeOncePerBacktest;
		public void printQuoteTypeOncePerBacktest(Quote quote) {
			if (this.printedQuoteTypeOncePerBacktest) return;

				this.printedQuoteTypeOncePerBacktest = true;

			QuoteGenerated quoteGenerated = quote as QuoteGenerated;
			if (quoteGenerated != null) {
				Assembler.PopupException("WE_ARE_RUNNING_BACKTEST_OR_LIVESIM [" + quoteGenerated.GetType() + "] //" + base.StrategyName);
			}
			QuoteQuik quoteQuik = quote as QuoteQuik;
			if (quoteQuik != null) {
				Assembler.PopupException("WE_ARE_RUNNING_QuikLIVESIM_OR_QuikREALTIME [" + quoteQuik.GetType() + "] //" + base.StrategyName);
			}
		}

		public override void OnNewQuoteOfStreamingBarCallback(Quote quote) {
			this.printQuoteTypeOncePerBacktest(quote);
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
			if (this.Executor.Sequencer.IsRunningNow == false) {
				this.drawLinesSample(barStaticFormed);
				//this.testBarBackground(barStaticFormed);
				//this.testBarAnnotations(barStaticFormed);
			}
			
			Bar barStreaming = barStaticFormed.ParentBars.BarStreamingNullUnsafe;
			if (barStaticFormed.ParentBarsIndex <= this.PeriodLargestAmongMAs) return;

			if (this.MAslow.OwnValuesCalculated == null) {
				string msg = "MAslow[" + this.MAslow + ".OwnValuesCalculate = null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.MAslow.OwnValuesCalculated.Count <= barStaticFormed.ParentBarsIndex) {
				string msg = "MAslow[" + this.MAslow + ".OwnValuesCalculate.Count[" + this.MAslow.OwnValuesCalculated.Count
					+ "] >= barStaticFormed.ParentBarsIndex[" + barStaticFormed.ParentBarsIndex + "]";
				Assembler.PopupException(msg);
				return;
			}

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
