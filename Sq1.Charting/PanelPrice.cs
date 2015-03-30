using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Charting {
	public partial class PanelPrice : PanelBase {
		List<Position> PositionLineAlreadyDrawnFromOneOfTheEnds;

		public override int ValueIndexLastAvailableMinusOneUnsafe	{ get { return base.ChartControl.Bars.Count - 1; } }

		public PanelPrice() : base() {
			this.PositionLineAlreadyDrawnFromOneOfTheEnds = new List<Position>();
			base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
			// startup with narrow AppFormHeight makes PanelPrice.Height too small => gutter disappears => EXCEPTION "Price-PANEL_HEIGHT_MUST_BE_POSITIVE"   
			base.MinimumSize = new Size(20, 25);	// only height matters for MultiSplitContainer
		}
		
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			//v1
			//if (this.VisibleMinDoubleMaxValueUnsafe == double.MaxValue) {
			//	string msg = "this.VisibleBarLeft_cached > this.VisibleBarRight_cached";
			//	return;
			//}
			//v2
			if (this.PanelHasValuesForVisibleBarWindow == false) {
				string msig = " //PanelPrice.PaintWholeSurfaceBarsNotEmpty()";
				string msg = "NEVER_HAPPENED_SO_FAR PanelHasValuesForVisibleBarWindow=false";
				Assembler.PopupException(msg + msig);
				return;
			}

			// 1) uses here-defined VisibleMinDoubleMaxValueUnsafe,VisibleMaxDoubleMinValueUnsafe to set:
			//		base.VisibleMin,Max,Range_cached,
			//		base.VisibleMinMinusTopSqueezer_cached, this.VisibleMaxPlusBottomSqueezer_cached, this.VisibleRangeWithTwoSqueezers_cached
			// 2) paints Right and Bottom gutter foregrounds;
			base.PaintWholeSurfaceBarsNotEmpty(g);
			howManyPositionArrowsBeyondPriceBoundaries = this.alignVisiblePositionArrowsAndCountMaxOutstanding();
			this.renderBarsPrice(g);
			// TODO MOVE_IT_UPSTACK_AND_PLACE_AFTER_renderBarsPrice_SO_THAT_POSITION_LINES_SHOWUP_ON_TOP_OF_BARS 
			//MOVED_TO renderBarsPrice() this.renderPositions(g);
			this.renderOnChartLabels(g);
			this.renderOnChartLines(g);
			this.renderOnChartBarAnnotations(g);
			this.renderBidAsk(g);
		}
		
		//protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
		//	base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
		//}
		int alignVisiblePositionArrowsAndCountMaxOutstanding() {
			int ret = 0;
			Dictionary<int, List<AlertArrow>> arrowListByBar = base.ChartControl.ScriptExecutorObjects.AlertArrowsListByBar;
			
			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;
			for (int i = base.VisibleBarRight_cached; i >= base.VisibleBarLeft_cached; i--) {
				barX -= this.BarWidthIncludingPadding_cached;
				if (arrowListByBar.ContainsKey(i) == false) continue;
								
				int shadowX = barX + this.BarShadowXoffset_cached;

				List<AlertArrow> arrows = arrowListByBar[i];
				if (arrows.Count > 1) {
					//Debugger.Break();
				}
				int positionsAboveBar = 0;
				int positionsBelowBar = 0;
				foreach (AlertArrow arrow in arrows) {
					arrow.Ytransient = base.ValueToYinverted(arrow.PriceAtBeyondBarRectangle);
					arrow.XBarMiddle = shadowX;

					int increment = base.ChartControl.ChartSettings.PositionLineHighlightedWidth;
					if (arrow.AboveBar) {
						positionsAboveBar++;
						increment += base.ChartControl.ChartSettings.PositionArrowPaddingVertical * positionsAboveBar;
						increment += arrow.Bitmap.Height * positionsAboveBar;
						increment = -increment;
					} else {
						positionsBelowBar++;
						increment += base.ChartControl.ChartSettings.PositionArrowPaddingVertical * positionsBelowBar;
						increment += arrow.Bitmap.Height * (positionsBelowBar - 1);
					}
					arrow.Ytransient += increment;
					arrow.Ytransient = base.AdjustToPanelHeight(arrow.Ytransient);
				}
			}
			return ret;
		}
		public override double ValueGetNaNunsafe(int barIndex) {
			if (barIndex < 0) return double.NaN;
			if (barIndex >= base.ChartControl.Bars.Count) return double.NaN; 
			Bar bar = base.ChartControl.Bars[barIndex];
			return bar.Close;
		}
	}
}
