using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

using Sq1.Charting.OnChart;
using Sq1.Core.Charting;

namespace Sq1.Charting {
	public partial class PanelPrice {
		void renderBarsPrice(Graphics g) {
			//for (int i = 0; i < base.ChartControl.BarsCanFitForCurrentWidth; i++) {
			//	int barFromRight = base.ChartControl.BarsCanFitForCurrentWidth - i - 1;
			//int barX = this.BarTotalWidthPx_localCache * barFromRight;
			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;

			this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			for (int barIndex = base.VisibleBarRight_cached; barIndex > base.VisibleBarLeft_cached; barIndex--) {
				if (barIndex >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderBarsPrice(): " + msg);
					continue;
				}
				
				barX -= base.BarWidthIncludingPadding_cached;
				Bar bar = base.ChartControl.Bars[barIndex];
				//bar.CheckOHLCVthrow();
				int barYOpenInverted = base.ValueToYinverted(bar.Open);
				int barYHighInverted = base.ValueToYinverted(bar.High);
				int barYLowInverted = base.ValueToYinverted(bar.Low);
				int barYCloseInverted = base.ValueToYinverted(bar.Close);
				//base.CheckConvertedBarDataIsNotZero(xOffsetFromRightBorder, barYOpen, barYHigh, barYLow, barYClose);
				bool fillCandleBody = (bar.Open > bar.Close) ? true : false;
				fillCandleBody |= base.ChartControl.ChartSettingsTemplated.BarUpFillCandleBody;
				base.RenderBarCandle(g, barX, barYOpenInverted, barYHighInverted, barYLowInverted, barYCloseInverted, fillCandleBody);

				//int shadowX = 0;
				//MOVED_TO_AlignVisiblePositionArrowsAndCountMaxOutstanding()
				int shadowX = barX + this.BarShadowXoffset_cached;
				this.renderAlertsPending_bigAquaDot_atPriceEmitted_ifExistForBar(barIndex, shadowX, g);
				this.renderPosition_smallBrownDot_atPriceFilled_arrowsLines_ifExistForBar(barIndex, shadowX, g);

				// TODO MOVE_IT_UPSTACK_AND_PLACE_AFTER_renderBarsPrice_SO_THAT_POSITION_LINES_SHOWUP_ON_TOP_OF_BARS 
				AlertArrow arrow = base.ChartControl.TooltipPositionShownForAlertArrow;
				if (arrow == null) continue;
				if (arrow.BarIndexFilled != barIndex) continue;
				this.renderPositionLineForArrow(arrow, g, true);
			}
		}
		void renderAlertsPending_bigAquaDot_atPriceEmitted_ifExistForBar(int barIndex, int shadowX, Graphics g) {
			Dictionary<int, AlertList> alertPendingListByBar = base.ChartControl.ExecutorObjects_frozenForRendering.AlertsPlacedByBar;
			if (alertPendingListByBar.ContainsKey(barIndex) == false) return;
			List<Alert> alertsPending = alertPendingListByBar[barIndex].SafeCopy(this, "//renderPendingAlertsIfExistForBar(WAIT)");

			Pen penPending	= base.ChartControl.ChartSettingsTemplated.PenAlertPendingEllipse;
			Pen penTP		= base.ChartControl.ChartSettingsTemplated.PenAlertPendingProtoTakeProfitEllipse;
			Pen penSL		= base.ChartControl.ChartSettingsTemplated.PenAlertPendingProtoStopLossEllipse;
			int radius		= base.ChartControl.ChartSettingsTemplated.AlertPendingEllipseRadius;
			int diameter	= radius * 2;

			foreach (Alert pending in alertsPending) {
				Pen pen = penPending;
				if (pending.PositionAffected != null && pending.PositionAffected.Prototype != null) {
					if (pending == pending.PositionAffected.Prototype.StopLossAlert_forMoveAndAnnihilation) {
						//Assembler.PopupException("SL_CIRCLE_RED");
						pen = penSL;
					}
					if (pending == pending.PositionAffected.Prototype.TakeProfitAlert_forMoveAndAnnihilation) {
						//Assembler.PopupException("TP_CIRCLE_GREEN");
						pen = penTP;
					}
				}
				double alertPending_priceScript_zeroForMarket = pending.PriceEmitted;
				int pendingY = base.ValueToYinverted(alertPending_priceScript_zeroForMarket);
				Rectangle entryPlannedRect = new Rectangle(shadowX - radius, pendingY - radius, diameter, diameter);
				g.DrawEllipse(pen, entryPlannedRect);

				if (pending.MarketLimitStop == MarketLimitStop.StopLimit) {
					double pendingStopActivationPrice = pending.PriceStopLimitActivation;
					pendingY = base.ValueToYinverted(pendingStopActivationPrice);
					entryPlannedRect = new Rectangle(shadowX - radius, pendingY - radius, diameter, diameter);
					g.DrawEllipse(pen, entryPlannedRect);
				}
			}
		}
		void renderPosition_smallBrownDot_atPriceFilled_arrowsLines_ifExistForBar(int barIndex, int shadowX, Graphics g) {
			Dictionary<int, List<AlertArrow>> alertArrowsListByBar = base.ChartControl.ExecutorObjects_frozenForRendering.AlertArrowsListByBar;
			if (alertArrowsListByBar.ContainsKey(barIndex) == false) return;
			List<AlertArrow> arrows = alertArrowsListByBar[barIndex];

			foreach (AlertArrow arrow in arrows) {
				//g.DrawImage(position.Bitmap, new Point(shadowX, position.Ytransient));
				//MOVED_TO_AlignVisiblePositionArrowsAndCountMaxOutstanding() position.XBarMiddle = shadowX;
				if (arrow.Bitmap != null) {
					g.DrawImage(arrow.Bitmap, arrow.Location);
					//g.DrawImageUnscaled(position.Bitmap, position.Location);
					//http://stackoverflow.com/questions/7690546/replace-gdi-drawimage-with-pinvoked-gdi-and-transparent-pngs
					//http://stackoverflow.com/questions/264720/gdi-graphicsdrawimage-really-slow
					//g.FillRectangle(position.BitmapTextureBrush, position.ClientRectangle);
				}

				this.renderPositionLineForArrow(arrow, g, false);

				Position position = arrow.Position;

				int ellipsePlannedDiameter = base.ChartControl.ChartSettingsTemplated.PositionPlannedEllipseDiameter;	//6
				int ellipsePlannedRadius = (int)(Math.Round(ellipsePlannedDiameter / 2f));
				int ellipseFilledDiameter = base.ChartControl.ChartSettingsTemplated.PositionFilledDotDiameter;		//4
				int ellipseFilledRadius = (int)(Math.Round(ellipseFilledDiameter / 2f));

				if (arrow.ArrowIsForPositionEntry) {
					int entryPlannedX = shadowX;
					int entryPlannedY = base.ValueToYinverted(position.EntryEmitted_price);
					Rectangle entryPlannedRect = new Rectangle(entryPlannedX - ellipsePlannedRadius, entryPlannedY - ellipsePlannedRadius, ellipsePlannedDiameter, ellipsePlannedDiameter);
					g.DrawEllipse(base.ChartControl.ChartSettingsTemplated.PenPositionPlannedEllipse, entryPlannedRect);

					if (position.IsEntryFilled) {
						int entryFilledOnY = base.ValueToYinverted(position.EntryFilled_price);
						Rectangle entryFilledRect = new Rectangle(entryPlannedX - ellipseFilledRadius, entryFilledOnY - ellipseFilledRadius, ellipseFilledDiameter, ellipseFilledDiameter);
						g.FillEllipse(base.ChartControl.ChartSettingsTemplated.BrushPositionFilledDot, entryFilledRect);
					}
				} else {
					int exitPlannedX = base.BarToXshadowBeyondGoInside(position.ExitBar.ParentBarsIndex);
					int exitPlannedY = base.ValueToYinverted(position.ExitEmitted_price);
					Rectangle exitPlannedRect = new Rectangle(exitPlannedX - ellipsePlannedRadius, exitPlannedY - ellipsePlannedRadius, ellipsePlannedDiameter, ellipsePlannedDiameter);
					g.DrawEllipse(base.ChartControl.ChartSettingsTemplated.PenPositionPlannedEllipse, exitPlannedRect);

					if (position.IsExitFilled) {
						int exitFilledOnY = base.ValueToYinverted(position.ExitFilled_price);
						Rectangle exitFilledRect = new Rectangle(exitPlannedX - ellipseFilledRadius, exitFilledOnY - ellipseFilledRadius, ellipseFilledDiameter, ellipseFilledDiameter);
						g.FillEllipse(base.ChartControl.ChartSettingsTemplated.BrushPositionFilledDot, exitFilledRect);
					}
				}
			}
		}
		void renderPositionLineForArrow(AlertArrow arrow, Graphics g, bool highlighted = false) {
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
				mouseEndY = base.ValueToYinverted(position.EntryFilled_price);

				if (position.IsExitFilled) {
					// Exit
					oppositeEndX = base.BarToXshadowBeyondGoInside(position.ExitFilledBarIndex);
					oppositeEndY = base.ValueToYinverted(position.ExitFilled_price);
				} else {
					// BarStreaming, end up on GutterRight
					oppositeEndX = base.ChartControl.ChartWidthMinusGutterRightPrice;
					oppositeEndY = base.ValueToYinverted(position.Bars.BarStreaming_nullUnsafe.Close);
				}
			} else {
				// Exit
				mouseEndX = base.BarToXshadowBeyondGoInside(position.ExitFilledBarIndex);
				mouseEndY = base.ValueToYinverted(position.ExitFilled_price);

				// Entry (must be filled if Arrow was for an Exit)
				oppositeEndX = base.BarToXshadowBeyondGoInside(position.EntryFilledBarIndex);
				oppositeEndY = base.ValueToYinverted(position.EntryFilled_price);
			}

			Pen penLine = base.ChartControl.ChartSettingsTemplated.PenPositionLineEntryExitConnectedUnknown;
			if (double.IsNaN(arrow.Position.NetProfit) == false) {
				penLine = (arrow.Position.NetProfit > 0)
					? base.ChartControl.ChartSettingsTemplated.PenPositionLineEntryExitConnectedProfit
					: base.ChartControl.ChartSettingsTemplated.PenPositionLineEntryExitConnectedLoss;
			}

			if (highlighted == false) {
				g.DrawLine(penLine, mouseEndX, mouseEndY, oppositeEndX, oppositeEndY);
				return;
			}

			int alpha = base.ChartControl.ChartSettingsTemplated.PositionLineHighlightedAlpha;
			int width = base.ChartControl.ChartSettingsTemplated.PositionLineHighlightedWidth;
			Color colorLessTransparent = Color.FromArgb(alpha, penLine.Color.R, penLine.Color.G, penLine.Color.B);
			using (Pen penLineHighlighted = new Pen(colorLessTransparent, width)) {
				g.DrawLine(penLineHighlighted, mouseEndX, mouseEndY, oppositeEndX, oppositeEndY);
			};
		}
		void renderOnChartLines(Graphics g) {
			// avoiding throwing "Dictionary.CopyTo target array wrong size" below during backtest & chartMouseOver
			//if (base.ChartControl != null && base.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) return;
			
			if (base.VisibleBarRight_cached > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
				string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
				Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderOnChartLines(): " + msg);
				return;
			}
			// DO_I_NEED_SIMILAR_CHECK_HERE???? MOST_LIKELY_I_DONT this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			//int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;

			ExecutorObjects_FrozenForRendering eoFrozen = base.ChartControl.ExecutorObjects_frozenForRendering;
			List<OnChartLine> linesToDraw = new List<OnChartLine>();		// helps to avoid drawing the same line twice

			// v1 - buggy because it doesn't display lines started way before and ended way later the visible barWindow
			//for (int barIndex = base.VisibleBarRight_cached; barIndex > base.VisibleBarLeft_cached; barIndex--) {
			//	// 1. render the lines that end on each visible bar and stretch to the left
			//	if (seo.LinesByRightBar.ContainsKey(barIndex)) {
			//		List<OnChartLine> linesEndingHere = seo.LinesByRightBar[barIndex];
			//		foreach (OnChartLine line in linesEndingHere) {
			//			if (linesToDraw.Contains(line)) continue;
			//			linesToDraw.Add(line);
			//		}
			//	}
			//	// 2. render the lines that start from each visible bar and stretch to the right
			//	if (seo.LinesByLeftBar.ContainsKey(barIndex)) {
			//		List<OnChartLine> linesStartingHere = seo.LinesByLeftBar[barIndex];
			//		foreach (OnChartLine line in linesStartingHere) {
			//			if (linesToDraw.Contains(line)) continue;
			//			linesToDraw.Add(line);
			//		}
			//	}
			//}
			// v2,v3: render the lines that start AND end beyond visible bars range
			// v2 - very clean but I expect it to be very slow
			//foreach (OnChartLine line in seo.LinesById.Values) {
			//	if (line.BarLeft > base.VisibleBarRight_cached) continue;	//whole line is at right of visible bar window
			//	if (line.BarRight < base.VisibleBarLeft_cached) continue;	//whole line is at  left of visible bar window
			//	if (linesToDraw.Contains(line)) continue;	// should never "continue"
			//	linesToDraw.Add(line);
			//}
			//v3 - will work faster closer to right edge of Chart (fastest when StreamingBar is displayed); will display all lines that start AND end beoynd VisibleBars
			// throwing "Dictionary.CopyTo target array wrong size" during backtest & chartMouseOver
			List<int> lineRightEnds = new List<int>(base.ChartControl.ExecutorObjects_frozenForRendering.LinesByRightBar.Keys);
			lineRightEnds.Sort();
			lineRightEnds.Reverse();
			foreach (int index in lineRightEnds) {				// 5,3,2,0 (sorted & reversed enforced: max => min)
				if (index < base.VisibleBarLeft_cached) break;	// if our VisibleLeft is 3 we should ignore all lines ending at 2 
				List<OnChartLine> linesByRight = eoFrozen.LinesByRightBar[index];
				foreach (OnChartLine line in linesByRight) {
					if (line.BarLeft > base.VisibleBarRight_cached) continue;	// line will start after VisibleRight
					if (linesToDraw.Contains(line)) {
						string msg = "should never happen";
						Assembler.PopupException(msg);
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
		void renderOnChartLabels(Graphics g) {
			if (base.VisibleBarRight_cached > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
				string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
				Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderOnChartLabels(): " + msg);
				return;
			}
			// DO_I_NEED_SIMILAR_CHECK_HERE???? MOST_LIKELY_I_DONT this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			ExecutorObjects_FrozenForRendering eoFrozen = base.ChartControl.ExecutorObjects_frozenForRendering;
			foreach (OnChartLabel label in eoFrozen.OnChartLabelsById.Values) {
				try {
					base.DrawLabelOnNextLine(g, label.LabelText, label.Font, label.ColorForeground, label.ColorBackground);
				} catch (Exception ex) {
					string msg = "WHAT_IS_NULL_HERE?";
					Assembler.PopupException(msg, ex);
				}
			}
		}
		void renderOnChartBarAnnotations(Graphics g) {
			// TODO remove dupes from render*, move loop upstack
			if (base.VisibleBarRight_cached > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
				string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
				Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderOnChartLabels(): " + msg);
				return;
			}
			// DO_I_NEED_SIMILAR_CHECK_HERE???? MOST_LIKELY_I_DONT this.PositionLineAlreadyDrawnFromOneOfTheEnds.Clear();

			int barXshadow = base.ChartControl.ChartWidthMinusGutterRightPrice + base.BarShadowXoffset_cached;
			ExecutorObjects_FrozenForRendering eoFrozen = base.ChartControl.ExecutorObjects_frozenForRendering;

			for (int barIndex = base.VisibleBarRight_cached; barIndex > base.VisibleBarLeft_cached; barIndex--) {
				if (barIndex >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderBarsPrice(): " + msg);
					continue;
				}
				
				barXshadow -= base.BarWidthIncludingPadding_cached;
				if (eoFrozen.OnChartBarAnnotationsByBar.ContainsKey(barIndex) == false) continue;
				
				Bar bar = base.ChartControl.Bars[barIndex];
				int yForLabelsAbove = base.ValueToYinverted(bar.High);
				int yForLabelsBelow = base.ValueToYinverted(bar.Low);
				int paddingFromSettings = base.ChartControl.ChartSettingsTemplated.ChartLabelsUpperLeftPlatePadding;

				// UNCLUTTER_ADD_POSITIONS_ARROWS_OFFSET begin
				Dictionary<int, List<AlertArrow>> alertArrowsListByBar = base.ChartControl.ExecutorObjects_frozenForRendering.AlertArrowsListByBar;
				if (alertArrowsListByBar.ContainsKey(barIndex)) {
					List<AlertArrow> arrows = alertArrowsListByBar[barIndex];
					foreach (AlertArrow arrow in arrows) {
						int arrowHeight = arrow.Height + base.ChartControl.ChartSettingsTemplated.PositionArrowPaddingVertical;
						if (arrow.AboveBar) yForLabelsAbove -= arrowHeight + base.ChartControl.ChartSettingsTemplated.PositionArrowPaddingVertical; 
						else yForLabelsBelow += arrowHeight; 
					}
				}
				// UNCLUTTER_ADD_POSITIONS_ARROWS_OFFSET end
				
				int verticalOffsetForNextStackedAnnotationsAboveSameBar = 0;
				int verticalOffsetForMextStackedAnnotationsBelowSameBar = 0;
				SortedDictionary<string, OnChartBarAnnotation> barAnnotationsById =  eoFrozen.OnChartBarAnnotationsByBar[barIndex];
				foreach (OnChartBarAnnotation barAnnotation in barAnnotationsById.Values) {
					//DUPLICATION copypaste from DrawLabel
					Font font = (barAnnotation.Font != null) ? barAnnotation.Font : base.Font;
					SizeF measurements = g.MeasureString(barAnnotation.BarAnnotationText, font);
					int labelWidthMeasured = (int) measurements.Width;
					int labelHeightMeasured = (int) measurements.Height;
	
					int x = barXshadow - labelWidthMeasured / 2;
					int xPadding = barAnnotation.ShouldDrawBackground ? paddingFromSettings : 0;
					// WEIRD_BUT_IF_I_DONT_INCLUDE_PADDING_WHO_LABELS_WITH_AND_WITHOUTH_PADDING_ARE_PERFECTLY_CENTER_ALIGNED -= xPadding;
					
					int y = barAnnotation.AboveBar ? yForLabelsAbove - labelHeightMeasured : yForLabelsBelow;
					
					if (barAnnotation.VerticalPaddingPx == Int32.MaxValue) {
						string msg = "PREVENT_Y_BEYOUND_VISIBLE_DUE_TO_EXCEEDED_BAR_ANNOTATION_PADDING (due to barAnnotation.VerticalPaddingPx = Int32.MaxValue)";
						y = barAnnotation.AboveBar
							? verticalOffsetForNextStackedAnnotationsAboveSameBar
							: this.PanelHeightMinusGutterBottomHeight_cached - labelHeightMeasured - verticalOffsetForMextStackedAnnotationsBelowSameBar - 3;
						if (barAnnotation.AboveBar) verticalOffsetForNextStackedAnnotationsAboveSameBar += labelHeightMeasured;
						else						verticalOffsetForMextStackedAnnotationsBelowSameBar += labelHeightMeasured;
//					} else {
//						if (verticalPaddingDueToManyStackedAnnotationsAboveSameBar > 0) {
//							string msg = "TESTME_WHEN_REASONABLE_PADDING_MIXED_WITH_INT.MAXVALUE_FOR_SAME_BAR";
//							Assembler.PopupException(msg);
//						}
					}
					int yPadding = 0;
					if (barAnnotation.ShouldDrawBackground) {
						yPadding += barAnnotation.AboveBar ? -paddingFromSettings * 2 :  paddingFromSettings;
					}
					y += yPadding;
					if (barAnnotation.VerticalPaddingPx == Int32.MaxValue) {
						if (barAnnotation.AboveBar) verticalOffsetForNextStackedAnnotationsAboveSameBar += yPadding;
						else						verticalOffsetForMextStackedAnnotationsBelowSameBar += yPadding;
					} else {
						y += (barAnnotation.AboveBar) ? -barAnnotation.VerticalPaddingPx : barAnnotation.VerticalPaddingPx;
					}
					base.DrawLabel(g, x, y,
								   barAnnotation.BarAnnotationText, barAnnotation.Font,
								   barAnnotation.ColorForeground, barAnnotation.ColorBackground, false);

				
					int labelHeightWithPadding = labelHeightMeasured;
					if (barAnnotation.ShouldDrawBackground) {
						labelHeightWithPadding += paddingFromSettings * 2;
					}
					
					if (barAnnotation.AboveBar) {
						yForLabelsAbove	-= labelHeightWithPadding;  
					} else {
						yForLabelsBelow += labelHeightWithPadding;
					}
				}
			}
		}
		void renderBidAsk(Graphics g) {
			Quote quoteCurrent = base.ChartControl.ExecutorObjects_frozenForRendering.QuoteCurrent;
			if (quoteCurrent == null) return;
			double spread = quoteCurrent.Spread;
			if (double.IsNaN(spread) == true) return;

			ChartSettingsTemplated settings = base.ChartControl.ChartSettingsTemplated;

			int chartWidth = base.ChartControl.ChartWidthMinusGutterRightPrice;
			//	chartWidth = base.ChartControl.Width;
				chartWidth = base.Width;
			
			string spreadFormatted = spread.ToString(base.PriceFormat);
			string labelSpread = "spread[" + spreadFormatted + "]";
			SizeF labelSpread_measured = g.MeasureString(labelSpread, settings.SpreadLabelFont);
			int labelSpread_measuredWidth	= (int)labelSpread_measured.Width;
			int labelSpread_measuredHeight	= (int)labelSpread_measured.Height;
			int padding = 4;
	
			int spreadLinesWidth = chartWidth - labelSpread_measuredWidth - padding * 3;
			if (spreadLinesWidth < 10) spreadLinesWidth = 10;			// im not testing g.DrawString() if it accepts negative values; just want it to draw
			bool showLabels_onRightEdge = false;

			int xLinesSpread = showLabels_onRightEdge
				? 0
				: labelSpread_measuredWidth + padding * 2;

			double bid = quoteCurrent.Bid;
			double ask = quoteCurrent.Ask;
			int yBid = 0;
			int yAsk = 0;
			if (double.IsNaN(bid) == false) {
				yBid = base.ValueToYinverted(bid);
				g.DrawLine(settings.PenSpreadBid, xLinesSpread, yBid, spreadLinesWidth, yBid);
			}
			if (double.IsNaN(ask) == false) {
				yAsk = base.ValueToYinverted(ask);
				g.DrawLine(settings.PenSpreadAsk, xLinesSpread, yAsk, spreadLinesWidth, yAsk);
			}

			int xLabelSpread = showLabels_onRightEdge
				? base.Width - labelSpread_measuredWidth - padding * 2
				: 5;
			if (xLabelSpread < 5) xLabelSpread = 5;		// im not testing g.DrawString() if it accepts negative values; just want it to draw
			int yLabelSpread = (int) ((yBid + yAsk) / 2 - labelSpread_measuredHeight / 2);	// in the middle between spread lines
			if (yLabelSpread > base.Height) yLabelSpread = base.Height - labelSpread_measuredHeight;
			if (yLabelSpread < 0) yLabelSpread = 0;

			if (yLabelSpread < 5) yLabelSpread = 5;		// im not testing g.DrawString() if it accepts negative values; just want it to draw

			g.DrawString(labelSpread, settings.SpreadLabelFont, settings.BrushSpreadLabel, xLabelSpread, yLabelSpread);

		}
	}
}
