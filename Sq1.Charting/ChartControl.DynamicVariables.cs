using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public partial class ChartControl  {
		// cache them all until base.Width/Height changes so they won't be calculated again with the same result for each bar
		public int ChartWidthMinusGutterRightPrice { get {
				if (base.Width <= 0) return 0;						// HAPPENS_WHEN_WINDOW_IS_MINIMIZED... how to disable any OnPaint when app isn't visible?... 
				if (this.GutterRightWidth_cached == -1) {
					//DONT_RETURN_ZERO return 0;	// BEFORE_FIRST_PAINT_SETS_GUTTER_WIDTH ONLY_WHEN_BACKTEST_ON_RESTART_BUT_HOW???
				}
				int ret = base.Width - this.GutterRightWidth_cached;
				return ret;
			} }
		public int BarsCanFitForCurrentWidth { get { return this.ChartWidthMinusGutterRightPrice / this.ChartSettings.BarWidthIncludingPadding; } }
		public int ScrollLargeChange { get { return (int)(this.BarsCanFitForCurrentWidth * 0.9f); } }
		public int VisibleBarRight { get {
				//v1 return this.hScrollBar.Value;
				int ret = this.hScrollBar.Value;
				//v2
				// http://stackoverflow.com/questions/2882789/net-vertical-scrollbar-not-respecting-maximum-property
				float physicalMax = this.hScrollBar.Maximum - this.hScrollBar.LargeChange + 1;
				if (physicalMax <= 0) {
					string msg = "ALL_BARS_CAN_FIT_WITHOUT_SCROLLING ex: 10bars on FullScreen";
					this.hScrollBar.Visible = false;	// LAZY if u change this.hScrollBar.Visible u must trigger Resize() once again (ReLayout somehow otherwize MultiSplitter doesn't get the space freed)  
					#if DEBUG
					//Debugger.Break();
					#endif
					return this.Bars.Count - 1;
				}
				this.hScrollBar.Visible = true;			// LAZY u chang this.hScrollBar.Visible u must trigger Resize() once again
				float part0to1 = this.hScrollBar.Value / physicalMax;
				if (part0to1 > 1) part0to1 = 1;	//	NONSENSE: this.hScrollBar.Value=432, physicalMax=423 - am I still resizing?...
				try {
					ret = (int)(part0to1 * (this.Bars.Count - 1));
				} catch (Exception ex) {
					string msg = "OVERFLOW???";
					Debugger.Break();
				}
				return ret;
			} }
		public int VisibleBarLeft { get {
				int ret = this.VisibleBarRight - this.BarsCanFitForCurrentWidth;				
				if (ret < 0) {
					string msg = "RESIZING_WINDOW_WHEN_ON_FIRST_LEFT_BAR EXTENDING_SO_THAT_ONE_MORE_BAR_CAN_FIT";
					//Debugger.Break();
					ret = 0;
				}
				if (this.hScrollBar.Value == this.hScrollBar.Minimum && ret > 1) {
					string msg = "VisibleBarLeft must be zero at ScrollBar.Minimum, doesn't it?...";
					//Debugger.Break();
				}
				return ret;
			} }

		public int CalculateGutterWidthNecessaryToFitPriceVolumeLabels(Graphics g) {
//v1
//			string visiblePriceMaxFormatted = this.ValueFormattedToSymbolInfoDecimalsOr5(this.panelPrice.VisibleMaxDoubleMinValueUnsafe);
//			int maxPrice = (int)g.MeasureString(visiblePriceMaxFormatted, this.ChartSettings.GutterRightFont).Width;
//			
//			string visibleVolumeMaxFormatted = this.ValueFormattedToSymbolInfoDecimalsOr5(this.panelPrice.VisibleMaxDoubleMinValueUnsafe, false);
//			int maxVolume = (int)g.MeasureString(visibleVolumeMaxFormatted, this.ChartSettings.GutterRightFont).Width;
//			
//			int ret = Math.Max(maxPrice, maxVolume);

			int ret = 0;
			foreach (PanelBase panel in this.panels) {
				double panelMax = panel.VisibleMaxDoubleMinValueUnsafe;
				if (double.IsNaN(panelMax)) continue;
				//if (double.IsPositiveInfinity(panelMax)) continue;
				//if (double.IsNegativeInfinity(panelMax)) continue; 
				PanelIndicator panelIndicator = panel as PanelIndicator; 
				if (panelIndicator != null && panelMax == double.MinValue) {
					#if DEBUG
					string msg = "VISIBLE_WINDOW_NOT_CALCULATED_YET OwnValuesCalculated.Count=" + panelIndicator.Indicator.OwnValuesCalculated.Count
						+ " while VisibleBarLeft[" + this.VisibleBarLeft + "] VisibleBarRight[" + this.VisibleBarRight + "]";
					#endif
					continue;
				}
				string visibleMaxFormatted = panel.FormatValue(panelMax);
				int panelValueFormatted = (int)g.MeasureString(visibleMaxFormatted, this.ChartSettings.GutterRightFont).Width;
				if (panelValueFormatted > base.Width) {
					Debugger.Break();
					double f11intoit = panel.VisibleMaxDoubleMinValueUnsafe;
					continue;
				}
				if (ret >= panelValueFormatted) continue;
				ret = panelValueFormatted;
			}
			ret += this.ChartSettings.GutterRightPadding * 2;
			return ret;
		}

	}
}