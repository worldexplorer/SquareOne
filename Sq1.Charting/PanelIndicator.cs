using System;
using System.Drawing;
using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public class PanelIndicator : PanelNamedFolding, HostPanelForIndicator {
		public Indicator Indicator;
		public bool IndicatorEmpty { get { return this.Indicator == null || this.Indicator.OwnValuesCalculated.Count == 0; } }

		public PanelIndicator(Indicator indicator) : base() {
			Indicator = indicator;
		}
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter foregrounds
			base.RenderIndicators(g);
		}
		protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
			//this.PaintRightVolumeGutterAndGridLines(g);
		}
		
		public override double VisibleMin { get {
				if (base.DesignMode || this.IndicatorEmpty) return 99;
				double ret = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				ret = this.Indicator.OwnValuesCalculated.MinValueBetweenIndexes(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				return ret;
			} }

		private double visibleMax;
		public override double VisibleMax { get {
				if (base.DesignMode || this.IndicatorEmpty) return 658;
				double ret = Double.MinValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				ret = this.Indicator.OwnValuesCalculated.MaxValueBetweenIndexes(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				return ret;
			} }


	}
}
