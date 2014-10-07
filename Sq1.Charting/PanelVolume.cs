using System;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public class PanelVolume : PanelBase {
		public PanelVolume() : base() {
			base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
		}
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter foregrounds
			this.renderBarsVolume(g);
		}
		protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
			//this.PaintRightVolumeGutterAndGridLines(g);
		}
		
		public override double VisibleMin { get { return base.ChartControl.VisibleVolumeMin; } }
		public override double VisibleMax { get { return base.ChartControl.VisibleVolumeMax; } }

		void renderBarsVolume(Graphics g) {
			//for (int i = 0; i < base.ChartControl.BarsCanFitForCurrentWidth; i++) {
			//	int barFromRight = base.ChartControl.BarsCanFitForCurrentWidth - i - 1;
			//int barX = this.BarTotalWidthPx_localCache * barFromRight;
			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;
			for (int i = base.VisibleBarRight_cached; i > base.VisibleBarLeft_cached; i--) {
				if (i > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("RenderBarsPrice(): " + msg);
					continue;
				}
				barX -= base.BarWidthIncludingPadding_cached;
				Bar bar = base.ChartControl.Bars[i];
				//if (bar.IsStreamingBar) {	// NaNs are possible for all bars, not only for streaming bar
					if (double.IsNaN(bar.Open)) continue;
					if (double.IsNaN(bar.Close)) continue;
					if (double.IsNaN(bar.Volume)) continue;
				//}
				int barYVolume = base.ValueToYinverted(bar.Volume);
				bool fillCandleBody = (bar.Open > bar.Close) ? true : false;
				base.RenderBarHistogram(g, barX, barYVolume, fillCandleBody);
			}
		}
	}
}