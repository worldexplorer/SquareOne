using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public class PanelIndicator : PanelNamedFolding, HostPanelForIndicator {
		public Indicator Indicator;
		public bool IndicatorEmpty { get {
				if (this.Indicator == null) {
					string msg = "CATCH_IT_EARLIER!!! did ctor get indicator=null ?";
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}
				if (this.Indicator.OwnValuesCalculated == null) {
					string msg = "I_JUST_RESTORED_THE_PANEL_WHILE_BACKTEST_HASNT_RUN_YET";
//					string msg = "CATCH_IT_EARLIER!!! this.Indicator.OwnValuesCalculated is created in"
//						+ " Indicator.BacktestStartingConstructOwnValuesValidateParameters()"
//						+ " to assure Executor's freedom to feed any bars during backtest; Indicator itself lives a longer life";
					#if DEBUG
//					Debugger.Break();
					#endif
					return true;
				}
				return (this.Indicator.OwnValuesCalculated.Count == 0);
			} }

		public PanelIndicator(Indicator indicator) : base() {
			Indicator = indicator;
			base.PanelName = indicator.ToString();
			base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
		}
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter foregrounds
			// PanelIndicator should not append "ATR (Period:5[1..11/2]) " twice (?) below PanelName 
			// EACH_RENDERS_ITSELF__HAD_OLIVE_INDICATOR_NAME_DRAWN_TWICE_ON_ATR_OWN_PANEL base.RenderIndicators(g);
		}
		protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
			//this.PaintRightVolumeGutterAndGridLines(g);
		}
		
		public override double VisibleMin { get {
				if (base.DesignMode || this.IndicatorEmpty) {
					return 99;	// random value; set breakpoint to see why the number doesn't matter
				}
				double ret = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				ret = this.Indicator.OwnValuesCalculated.MinValueBetweenIndexes(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				return ret;
			} }

		private double visibleMax;
		public override double VisibleMax { get {
				if (base.DesignMode || this.IndicatorEmpty) {
					return 658;	// random value; set breakpoint to see why the number doesn't matter
				}
				double ret = Double.MinValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				ret = this.Indicator.OwnValuesCalculated.MaxValueBetweenIndexes(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				return ret;
			} }


	}
}
