using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sq1.Charting.OnChart;
using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Charting {
	public class PanelPrice : PanelNamedFolding {
		List<Position> PositionLineAlreadyDrawnFromOneOfTheEnds;

		public PanelPrice() : base() {
			this.PositionLineAlreadyDrawnFromOneOfTheEnds = new List<Position>();
		}

		private int howManyPositionArrowsBeyondPriceBoundaries = 0; 
		public override int PaddingVerticalSqueeze { get { return this.ChartControl.ChartSettings.SqueezeVerticalPaddingPx; } }
		
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter foregrounds
			howManyPositionArrowsBeyondPriceBoundaries = this.AlignVisiblePositionArrowsAndCountMaxOutstanding();
			this.renderBarsPrice(g);
			// TODO MOVE_IT_UPSTACK_AND_PLACE_AFTER_renderBarsPrice_SO_THAT_POSITION_LINES_SHOWUP_ON_TOP_OF_BARS 
			//this.renderPositions(g);
			this.renderOnChartLines(g);
			//this.RenderBidAsk(g);
			//this.RenderPositions(g);
		}
		protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
		}
		public int AlignVisiblePositionArrowsAndCountMaxOutstanding() {
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

					int increment = this.ChartControl.ChartSettings.PositionLineHighlightedWidth;
					if (arrow.AboveBar) {
						positionsAboveBar++;
						increment += this.ChartControl.ChartSettings.PositionArrowPaddingVertical * positionsAboveBar;
						increment += arrow.Bitmap.Height * positionsAboveBar;
						increment = -increment;
					} else {
						positionsBelowBar++;
						increment += this.ChartControl.ChartSettings.PositionArrowPaddingVertical * positionsBelowBar;
						increment += arrow.Bitmap.Height * (positionsBelowBar - 1);
					}
					arrow.Ytransient += increment;
					arrow.Ytransient = base.AdjustToPanelHeight(arrow.Ytransient);
				}
			}
			return ret;
		}
		void renderBarsPrice(Graphics g) {
			//for (int i = 0; i < base.ChartControl.BarsCanFitForCurrentWidth; i++) {
			//	int barFromRight = base.ChartControl.BarsCanFitForCurrentWidth - i - 1;
			//int barX = this.BarTotalWidthPx_localCache * barFromRight;
			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;

			this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			for (int barIndex = VisibleBarRight_cached; barIndex > VisibleBarLeft_cached; barIndex--) {
				if (barIndex > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderBarsPrice(): " + msg);
					continue;
				}
				Bar bar = base.ChartControl.Bars[barIndex];
				//bar.CheckOHLCVthrow();
				barX -= this.BarWidthIncludingPadding_cached;
				int barYOpenInverted = base.ValueToYinverted(bar.Open);
				int barYHighInverted = base.ValueToYinverted(bar.High);
				int barYLowInverted = base.ValueToYinverted(bar.Low);
				int barYCloseInverted = base.ValueToYinverted(bar.Close);
				//base.CheckConvertedBarDataIsNotZero(xOffsetFromRightBorder, barYOpen, barYHigh, barYLow, barYClose);
				bool fillCandleBody = (bar.Open > bar.Close) ? true : false;
				base.RenderBarCandle(g, barX, barYOpenInverted, barYHighInverted, barYLowInverted, barYCloseInverted, fillCandleBody);

				//int shadowX = 0;
				//MOVED_TO_AlignVisiblePositionArrowsAndCountMaxOutstanding()
				int shadowX = barX + this.BarShadowXoffset_cached;
				this.pendingAlertsDrawIfExistForBar(barIndex, shadowX, g);
				this.positionArrowsLinesDrawIfExistForBar(barIndex, shadowX, g);

				// TODO MOVE_IT_UPSTACK_AND_PLACE_AFTER_renderBarsPrice_SO_THAT_POSITION_LINES_SHOWUP_ON_TOP_OF_BARS 
				AlertArrow arrow = this.ChartControl.TooltipPositionShownForAlertArrow;
				if (arrow == null) continue;
				if (arrow.BarIndexFilled != barIndex) continue;
				this.drawPositionLineForArrow(arrow, g, true);
			}
		}
		public void pendingAlertsDrawIfExistForBar(int barIndex, int shadowX, Graphics g) {
			Dictionary<int, List<Alert>> alertPendingListByBar = base.ChartControl.ScriptExecutorObjects.AlertsPendingHistorySafeCopy;
			if (alertPendingListByBar.ContainsKey(barIndex) == false) return;
			List<Alert> alertsPending = alertPendingListByBar[barIndex];
			if (alertsPending.Count > 1 || barIndex == 443) {
				string msg = "TRYING_TO_DRAW_MISSING_STOP_LOSSES";
				//Debugger.Break();
			}
			foreach (Alert pending in alertsPending) {
				double pendingAlertPrice = pending.PriceScript;
				int pendingY = base.ValueToYinverted(pendingAlertPrice);
				Rectangle entryPlannedRect = new Rectangle(shadowX-2, pendingY-2, 4, 4);
				g.DrawEllipse(this.ChartControl.ChartSettings.PenAlertPendingEllipse, entryPlannedRect);

				if (pending.MarketLimitStop == MarketLimitStop.StopLimit) {
					double pendingStopActivationPrice = pending.PriceStopLimitActivation;
					pendingY = base.ValueToYinverted(pendingStopActivationPrice);
					entryPlannedRect = new Rectangle(shadowX - 2, pendingY - 2, 4, 4);
					g.DrawEllipse(this.ChartControl.ChartSettings.PenAlertPendingEllipse, entryPlannedRect);
				}
			}
		}
		public void positionArrowsLinesDrawIfExistForBar(int barIndex, int shadowX, Graphics g) {
			Dictionary<int, List<AlertArrow>> alertArrowsListByBar = base.ChartControl.ScriptExecutorObjects.AlertArrowsListByBar;
			if (alertArrowsListByBar.ContainsKey(barIndex) == false) return;
			List<AlertArrow> arrows = alertArrowsListByBar[barIndex];

			foreach (AlertArrow arrow in arrows) {
				//g.DrawImage(position.Bitmap, new Point(shadowX, position.Ytransient));
				//MOVED_TO_AlignVisiblePositionArrowsAndCountMaxOutstanding() position.XBarMiddle = shadowX;
				g.DrawImage(arrow.Bitmap, arrow.Location);
				//g.DrawImageUnscaled(position.Bitmap, position.Location);
				//http://stackoverflow.com/questions/7690546/replace-gdi-drawimage-with-pinvoked-gdi-and-transparent-pngs
				//http://stackoverflow.com/questions/264720/gdi-graphicsdrawimage-really-slow
				//g.FillRectangle(position.BitmapTextureBrush, position.ClientRectangle);

				this.drawPositionLineForArrow(arrow, g, false);

				Position position = arrow.Position;

				int ellipsePlannedDiameter = this.ChartControl.ChartSettings.PositionPlannedEllipseDiameter;	//6
				int ellipsePlannedRadius = (int)(Math.Round(ellipsePlannedDiameter / 2f));
				int ellipseFilledDiameter = this.ChartControl.ChartSettings.PositionFilledDotDiameter;		//4
				int ellipseFilledRadius = (int)(Math.Round(ellipseFilledDiameter / 2f));

				if (arrow.ArrowIsForPositionEntry) {
					int entryPlannedX = shadowX;
					int entryPlannedY = base.ValueToYinverted(position.EntryPriceScript);
					Rectangle entryPlannedRect = new Rectangle(entryPlannedX - ellipsePlannedRadius, entryPlannedY - ellipsePlannedRadius, ellipsePlannedDiameter, ellipsePlannedDiameter);
					g.DrawEllipse(this.ChartControl.ChartSettings.PenPositionPlannedEllipse, entryPlannedRect);

					if (position.IsEntryFilled) {
						int entryFilledOnY = base.ValueToYinverted(position.EntryFilledPrice);
						Rectangle entryFilledRect = new Rectangle(entryPlannedX - 2, entryFilledOnY - 2, ellipseFilledDiameter, ellipseFilledDiameter);
						g.FillEllipse(this.ChartControl.ChartSettings.BrushPositionFilledDot, entryFilledRect);
					}
				} else {
					int exitPlannedX = base.BarToXshadowBeyondGoInside(position.ExitBar.ParentBarsIndex);
					int exitPlannedY = base.ValueToYinverted(position.ExitPriceScript);
					Rectangle exitPlannedRect = new Rectangle(exitPlannedX - ellipsePlannedRadius, exitPlannedY - ellipsePlannedRadius, ellipsePlannedDiameter, ellipsePlannedDiameter);
					g.DrawEllipse(this.ChartControl.ChartSettings.PenPositionPlannedEllipse, exitPlannedRect);

					if (position.IsExitFilled) {
						int exitFilledOnY = base.ValueToYinverted(position.ExitFilledPrice);
						Rectangle exitFilledRect = new Rectangle(exitPlannedX - ellipseFilledRadius, exitFilledOnY - ellipseFilledRadius, ellipseFilledDiameter, ellipseFilledDiameter);
						g.FillEllipse(this.ChartControl.ChartSettings.BrushPositionFilledDot, exitFilledRect);
					}
				}
			}
		}
		private void drawPositionLineForArrow(AlertArrow arrow, Graphics g, bool highlighted = false) {
			Position position = arrow.Position;
			if (position.EntryFilledBarIndex == -1) return;		// position without EntryFilled shouldn't have a Line

			if (highlighted == false) {
				if (this.PositionLineAlreadyDrawnFromOneOfTheEnds.Contains(position)) return;
				this.PositionLineAlreadyDrawnFromOneOfTheEnds.Add(position);
			}

			int mouseEndX, mouseEndY, oppositeEndX, oppositeEndY;

			if (arrow.ArrowIsForPositionEntry) {
				// Entry
				mouseEndX = base.BarToXshadowBeyondGoInside(position.EntryFilledBarIndex);
				mouseEndY = base.ValueToYinverted(position.EntryFilledPrice);

				if (position.IsExitFilled) {
					// Exit
					oppositeEndX = base.BarToXshadowBeyondGoInside(position.ExitFilledBarIndex);
					oppositeEndY = base.ValueToYinverted(position.ExitFilledPrice);
				} else {
					// BarStreaming, end up on GutterRight
					oppositeEndX = this.ChartControl.ChartWidthMinusGutterRightPrice;
					oppositeEndY = base.ValueToYinverted(position.Bars.BarStreaming.Close);
				}
			} else {
				// Exit
				mouseEndX = base.BarToXshadowBeyondGoInside(position.ExitFilledBarIndex);
				mouseEndY = base.ValueToYinverted(position.ExitFilledPrice);

				// Entry (must be filled if Arrow was for an Exit)
				oppositeEndX = base.BarToXshadowBeyondGoInside(position.EntryFilledBarIndex);
				oppositeEndY = base.ValueToYinverted(position.EntryFilledPrice);
			}

			Pen penLine = this.ChartControl.ChartSettings.PenPositionLineEntryExitConnectedUnknown;
			if (double.IsNaN(arrow.Position.NetProfit) == false) {
				penLine = (arrow.Position.NetProfit > 0)
					? this.ChartControl.ChartSettings.PenPositionLineEntryExitConnectedProfit
					: this.ChartControl.ChartSettings.PenPositionLineEntryExitConnectedLoss;
			}

			if (highlighted == false) {
				g.DrawLine(penLine, mouseEndX, mouseEndY, oppositeEndX, oppositeEndY);
				return;
			}

			int alpha = this.ChartControl.ChartSettings.PositionLineHighlightedAlpha;
			int width = this.ChartControl.ChartSettings.PositionLineHighlightedWidth;
			Color colorLessTransparent = Color.FromArgb(alpha, penLine.Color.R, penLine.Color.G, penLine.Color.B);
			using (Pen penLineHighlighted = new Pen(colorLessTransparent, width)) {
				g.DrawLine(penLineHighlighted, mouseEndX, mouseEndY, oppositeEndX, oppositeEndY);
			};
		}
		void renderOnChartLines(Graphics g) {
			if (VisibleBarRight_cached > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
				string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
				Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderOnChartLines(): " + msg);
				return;
			}
			// DO_I_NEED_SIMILAR_CHECK_HERE???? MOST_LIKELY_I_DONT this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;

			ScriptExecutorObjects seo = this.ChartControl.ScriptExecutorObjects;
			List<OnChartLine> linesToDraw = new List<OnChartLine>();		// helps to avoid drawing the same line twice

			// v1 - buggy because it doesn't display lines started way before and ended way later the visible barWindow
			//for (int barIndex = VisibleBarRight_cached; barIndex > VisibleBarLeft_cached; barIndex--) {
			//    // 1. render the lines that end on each visible bar and stretch to the left
			//    if (seo.LinesByRightBar.ContainsKey(barIndex)) {
			//        List<OnChartLine> linesEndingHere = seo.LinesByRightBar[barIndex];
			//        foreach (OnChartLine line in linesEndingHere) {
			//            if (linesToDraw.Contains(line)) continue;
			//            linesToDraw.Add(line);
			//        }
			//    }
			//    // 2. render the lines that start from each visible bar and stretch to the right
			//    if (seo.LinesByLeftBar.ContainsKey(barIndex)) {
			//        List<OnChartLine> linesStartingHere = seo.LinesByLeftBar[barIndex];
			//        foreach (OnChartLine line in linesStartingHere) {
			//            if (linesToDraw.Contains(line)) continue;
			//            linesToDraw.Add(line);
			//        }
			//    }
			//}
			// v2,v3: render the lines that start AND end beyond visible bars range
			// v2 - very clean but I expect it to be very slow
			//foreach (OnChartLine line in seo.LinesById.Values) {
			//    if (line.BarLeft > VisibleBarRight_cached) continue;	//whole line is at right of visible bar window
			//    if (line.BarRight < VisibleBarLeft_cached) continue;	//whole line is at  left of visible bar window
			//    if (linesToDraw.Contains(line)) continue;	// should never "continue"
			//    linesToDraw.Add(line);
			//}
			//v3 - will work faster closer to right edge of Chart (fastest when StreamingBar is displayed); will display all lines that start AND end beoynd VisibleBars
			List<int> lineRightEnds = new List<int>(this.ChartControl.ScriptExecutorObjects.LinesByRightBar.Keys);
			lineRightEnds.Sort();
			lineRightEnds.Reverse();
			foreach (int index in lineRightEnds) {				// 5,3,2,0 (sorted & reversed enforced: max => min)
				if (index < base.VisibleBarLeft_cached) break;	// if our VisibleLeft is 3 we should ignore all lines ending at 2 
			    List<OnChartLine> linesByRight = seo.LinesByRightBar[index];
			    foreach (OnChartLine line in linesByRight) {
			        if (line.BarLeft > base.VisibleBarRight_cached) continue;	// line will start after VisibleRight
			        if (linesToDraw.Contains(line)) {
#if DEBUG
						string msg = "should never happen";
						Debugger.Break();
#endif
						continue;
					}
			        linesToDraw.Add(line);
			    }
			}

			foreach (OnChartLine line in linesToDraw) {
				//LINES_DONT_TOUCH_EACHOTHER_CONNECTING_SHADOWS_BUT_BETTER_THAN_line.BarRight+1_BELOW
				int lineLeftX = base.BarToXshadowBeyondGoInside(line.BarLeft);
				int lineRightX = base.BarToXshadowBeyondGoInside(line.BarRight);
				//LINE_SHIFTED_TO_LEFT_SO_LAST_BAR_ISNT_SHOWN
				//int lineLeftX = base.BarToXBeyondGoInside(line.BarLeft);
				//int lineRightX = base.BarToXBeyondGoInside(line.BarRight + 1);
					
				int lineRightYInverted = base.ValueToYinverted(line.PriceRight);
				int lineLeftYInverted = base.ValueToYinverted(line.PriceLeft);

				using (Pen pen = new Pen(line.Color, line.Width)) {		// I hate wasting disposing Pens and Brushes
					g.DrawLine(pen, lineRightX, lineRightYInverted, lineLeftX, lineLeftYInverted);
				}
			}
		}
	}
}
