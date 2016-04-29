using System;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

using Sq1.Indicators;

namespace Sq1.Strategies.Demo {
	public partial class TwoMAindicatorsCompiled : Script {

		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		IndicatorMovingAverageSimple MAslow;
		IndicatorMovingAverageSimple MAfast;

		public TwoMAindicatorsCompiled() {
			MAfast = new IndicatorMovingAverageSimple();
			MAfast.ParamPeriod = new IndicatorParameter("Period", 55, 11, 88, 11);
			MAfast.LineColor = Color.LightSeaGreen;

			MAslow = new IndicatorMovingAverageSimple();
			MAslow.ParamPeriod = new IndicatorParameter("Period", 15, 10, 20, 1);
			MAslow.LineColor = Color.LightCoral;
			fontArial7 = new Font("Arial", 7);
		}

		public override void InitializeBacktest() {
		}
		public override void OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(Bar barStaticFormed) {
			if (this.Executor.Sequencer.IsRunningNow) return;

			this.testBarBackground(barStaticFormed);
			this.testBarAnnotationsMarkBarsShiftedDueToMissedBar(barStaticFormed);
		}
	}
}
