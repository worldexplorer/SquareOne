using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;

namespace Sq1.Charting {
	public partial class PanelNamedFolding {
		void renderBarBackgrounds(Graphics g) {
			int barX = this.ChartControl.ChartWidthMinusGutterRightPrice;
			int halfPadding = this.ChartControl.ChartSettings.BarPaddingRight / 2;
			//halfPadding += 1;		// fixes 1-2px spaces between bars background
			barX -= halfPadding;	// emulate bar having paddings from left and right
			for (int barIndex = VisibleBarRight_cached; barIndex > VisibleBarLeft_cached; barIndex--) {
				if (barIndex >= this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderBarsPrice(): " + msg);
					#if DEBUG
					Debugger.Break();
					#endif
					continue;
				}
				
				barX -= this.BarWidthIncludingPadding_cached;
				ScriptExecutorObjects seo = this.ChartControl.ScriptExecutorObjects;
				if (seo.BarBackgroundsByBar.ContainsKey(barIndex) == false) continue;
				
				Rectangle barFullHeight = new Rectangle();
				barFullHeight.X = barX;
				barFullHeight.Width = this.BarWidthIncludingPadding_cached;
				barFullHeight.Y = 0;
				barFullHeight.Height = this.PanelHeightMinusGutterBottomHeight;

				Color backgroundSetByScript = seo.BarBackgroundsByBar[barIndex];
				Color backgroundMoreTransparent = Color.FromArgb(this.ChartControl.ChartSettings.BarsBackgroundTransparencyAlfa, backgroundSetByScript);

				using (Brush backBrush = new SolidBrush(backgroundMoreTransparent)) {
					g.FillRectangle(backBrush, barFullHeight);
				}
			}
		}

		public void DrawLabelOnNextLine(Graphics g, string msg, Font font, Color colorForeground, Color colorBackground, bool drawIndicatorSquare = false) {
			//if (this.DesignMode) return;
			// if (base.DesignMode) this.ChartControl will be NULL
			int x = (this.ChartControl != null) ? this.ChartControl.ChartSettings.ChartLabelsUpperLeftX : 5;
			int y = this.chartLabelsUpperLeftYincremental;

			SizeF measurements = this.DrawLabel(g, x, y, msg, font, colorForeground, colorBackground, drawIndicatorSquare);
			
			//DUPLICATION copypaste from DrawLabel()
			bool drawBackgroundRectangle = (colorBackground == Color.Empty) ? false : true;
			//if (font == null) font = base.Font;
			//SizeF measurements = g.MeasureString(msg, font);
			int labelWidthMeasured = (int) measurements.Width;
			int labelHeightMeasured = (int) measurements.Height; 

			int lineSpacing = (int) labelHeightMeasured / 8;
			if (lineSpacing == 0) lineSpacing = 1; 
			this.chartLabelsUpperLeftYincremental += labelHeightMeasured + lineSpacing;
			if (drawBackgroundRectangle) {
				// if (base.DesignMode) this.ChartControl will be NULL
				this.chartLabelsUpperLeftYincremental += (this.ChartControl != null) ? this.ChartControl.ChartSettings.ChartLabelsUpperLeftPlatePadding * 2 : 15;
			}
		}
		public SizeF DrawLabel(Graphics g, int x, int y, string msg,
		                      Font font, Color colorForeground, Color colorBackground, bool drawIndicatorSquare = false) {
			if (colorForeground == Color.Empty) colorForeground = Color.Red;	// error
			
			//DUPLICATION original for DrawLabelOnNextLine()
			bool drawBackgroundRectangle = (colorBackground == Color.Empty) ? false : true;
			if (font == null) font = base.Font;
			SizeF measurements = g.MeasureString(msg, font);
			int labelMeasuredWidth = (int) measurements.Width;
			int labelMeasuredHeight = (int) measurements.Height;

			if (x < 0) x = 0;
			if (y < 0) y = 0;

			// Y_BEYOUND_VISIBLE_DUE_TO_EXCEEDED_BAR_ANNOTATION_PADDING
			if (y > this.PanelHeightMinusGutterBottomHeight_cached) {
				y = this.PanelHeightMinusGutterBottomHeight_cached - labelMeasuredHeight;
			}
			if (x > this.PanelWidthMinusRightPriceGutter) {
				x = this.PanelHeightMinusGutterBottomHeight_cached - labelMeasuredHeight;
			}
			
			if (drawBackgroundRectangle) {
				int makingHpaddingLookLikeVpadding = 2; 
				Rectangle labelPlate = new Rectangle();
				labelPlate.X = x - this.ChartControl.ChartSettings.ChartLabelsUpperLeftPlatePadding - makingHpaddingLookLikeVpadding;
				labelPlate.Y = y - this.ChartControl.ChartSettings.ChartLabelsUpperLeftPlatePadding;
				labelPlate.Width = labelMeasuredWidth + this.ChartControl.ChartSettings.ChartLabelsUpperLeftPlatePadding * 2 + makingHpaddingLookLikeVpadding * 2;
				labelPlate.Height = labelMeasuredHeight + this.ChartControl.ChartSettings.ChartLabelsUpperLeftPlatePadding * 2;

				if (labelPlate.X < 0) labelPlate.X = 0;
				if (labelPlate.Y < 0) labelPlate.Y = 0;

				if (drawIndicatorSquare) {
					int extendedBySquare = this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquarePadding * 2
						+ this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquareSize;
					labelPlate.Width += extendedBySquare;
				}

				using (var brushLabelPlate = new SolidBrush(colorBackground)) 	g.FillRectangle(brushLabelPlate, labelPlate);
				using (var penLabelPlateBorder = new Pen(Color.LightGray))		g.DrawRectangle(penLabelPlateBorder, labelPlate);
			}
			
			using (var brushLabel = new SolidBrush(colorForeground)) {
				if (drawIndicatorSquare) {
					//x += this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquarePadding;
					Rectangle square = new Rectangle();
					square.X = x;
					square.Y = this.chartLabelsUpperLeftYincremental + this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquarePadding;
					square.Width = this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquareSize;
					square.Height = this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquareSize;
					g.FillRectangle(brushLabel, square);
					
					int squareAndRightPadding = this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquareSize
										 	  + this.ChartControl.ChartSettings.ChartLabelsUpperLeftIndicatorSquarePadding;
					x += squareAndRightPadding;
				}
				y += 1;	// moving text label to align middle vertically for Consolas,8 and Arial,8 (Courier New or some other font looked exactly valign=middle but I won't use it) 
				g.DrawString(msg, font, brushLabel, new Point(x, y));
			}
			return measurements;
		}
	}
}
