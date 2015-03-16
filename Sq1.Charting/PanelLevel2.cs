using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public class PanelLevel2 : PanelBase {
		const string REASON_TO_EXIST = "besides my own Panel, paint also on a clipped Graphics to draw below candles on PanelPrice (intensivity of layers might be controlled by sliders))";

		public override bool					PanelHasValuesForVisibleBarWindow { get { return false;	/* this will cancel drawing right gutter */ } }
		public override int						ValueIndexLastAvailableMinusOneUnsafe { get { return -1; } }
		public			StreamingDataSnapshot	StreamingDataSnapshotNullUnsafe { get {
			if (base.ChartControl.Executor.DataSource.StreamingAdapter == null) return null;
			return base.ChartControl.Executor.DataSource.StreamingAdapter.StreamingDataSnapshot;
		} }

		bool errorDetected;

		public PanelLevel2() : base() {
			base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
			base.MinimumSize = new Size(1, 15);	// only width matters when this.multiSplitContainerColumns.VerticalizeAllLogic = true;
		}

		public override void InitializeWithNonEmptyBars(ChartControl chartControl) {
			base.InitializeWithNonEmptyBars(chartControl);
			// unsubscribing first in order to avoid duplicate events
			// LAZY to check if ChartControl is initialized with every new Bars selected by user and ChartControl pushes itself again to its Panels
			base.ChartControl.PanelPrice.OnPanelPriceSqueezed -= new EventHandler<EventArgs>(panelPrice_OnPanelPriceSqueezed);
			base.ChartControl.PanelPrice.OnPanelPriceSqueezed += new EventHandler<EventArgs>(panelPrice_OnPanelPriceSqueezed);
			base.ChartControl.PanelPrice.MouseMove -= new MouseEventHandler(panelPrice_MouseMove);
			base.ChartControl.PanelPrice.MouseMove += new MouseEventHandler(panelPrice_MouseMove);
		}

		void panelPrice_MouseMove(object sender, MouseEventArgs e) {
			base.Invalidate();
		}
		void panelPrice_OnPanelPriceSqueezed(object sender, EventArgs e) {
			base.Invalidate();
		}

		int repaintSerno;

		//using (var brush = new SolidBrush(Color.Red)) {
		//	Font font = (this.ChartControl != null) ? this.ChartControl.ChartSettings.PanelNameAndSymbolFont : this.Font;
		//	g.DrawString(msgRepaint, font, brush, new Point(60, 60));
		//}

//		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
//			base.PaintWholeSurfaceBarsNotEmpty(g);
//		protected override void OnPaint(PaintEventArgs pe) {
			//ONLY_FOR_PRICE_PANEL?? //base.OnPaintBackgroundDoubleBuffered(pe);	 //base.DrawError drew label only lowest 2px shown from top of panel
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs pe) {
		}
		protected override void OnPaintDoubleBuffered(PaintEventArgs pe) {
			this.ChartLabelsUpperLeftYincremental = this.ChartControl.ChartSettings.ChartLabelsUpperLeftYstartTopmost;
			pe.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"

			Graphics g = pe.Graphics;
			g.Clear(this.BackColor);

			this.errorDetected = false;
			if (this.StreamingDataSnapshotNullUnsafe == null) {
				base.DrawError(g, "EDIT_DATASOURCE_SELECT_STREAMING_ADAPTER");
				this.errorDetected = true;
			}
			if (this.ChartControl.Bars == null) {
				base.DrawError(g, "CHART_CONTROL_HAS_NO_BARS");
				this.errorDetected = true;
			}
			Quote lastQuote = null;
			if (this.errorDetected == false) {
				lastQuote = this.StreamingDataSnapshotNullUnsafe.LastQuoteCloneGetForSymbol(this.ChartControl.Bars.Symbol);
				if (lastQuote == null) {
					//g.Clear(this.ChartControl.ChartSettings.LevelTwoColorBackgroundStreamingHasNoLastQuote);
					base.DrawError(g, "CONNECT_STREAMING__OR__CHART>BARS>SUBSCRIBE");
					this.errorDetected = true;
				}
			}

			this.repaintSerno++;
			string msgRepaint = "L2 repaint#" + this.repaintSerno + "//" + base.ParentMultiSplitIamFirst + ":" + base.ParentMultiSplitIamLast;
			base.DrawError(g, msgRepaint);
			if (this.errorDetected) return;

			g.Clear(this.ChartControl.ChartSettings.LevelTwoColorBackground);
			//base.DrawError(g, msgRepaint);

			try {
				this.renderLevel2(pe.Graphics, lastQuote);
			} catch (Exception ex) {
				base.DrawError(pe.Graphics, ex.ToString());
			}
		}
		void renderLevel2(Graphics g, Quote lastQuote) {
			PanelPrice panelPrice = this.ChartControl.PanelPrice;
			double priceStep = panelPrice.PriceStep;

			//string msgPixelsPerPriceStep = "Px[" + base.Height + "]/PriceStep[" + priceStep + "]=" 
			//    + panelPrice.PixelsPerPriceStep3pxLeast_cached;
			//base.DrawError(g, msgPixelsPerPriceStep);

			//string msgCalc = "PixelsPerPriceStepMadeVisible_cached[" + panelPrice.PixelsPerPriceStep3pxLeast_cached
			//    + "] = PriceLevelsShown_cached[" + panelPrice.PriceLevelsShown_cached
			//    + "]/VisibleMaxPlusBottomSqueezer_cached[" + panelPrice.VisibleMaxPlusBottomSqueezer_cached + "]";
			//base.DrawError(g, msgCalc);

			LevelTwoHalfFrozen bids_cachedForOnePaint = new LevelTwoHalfFrozen(
				"BIDS_FROZEN",
				this.StreamingDataSnapshotNullUnsafe.LevelTwoBids.SafeCopy(this, "CLONING_BIDS_FOR_PAINT_FOREGROUND_ON_PanelLevel2"),
				new LevelTwoHalfFrozen.DESC());
			LevelTwoHalfFrozen asks_cachedForOnePaint = new LevelTwoHalfFrozen(
				"ASKS_FROZEN",
				this.StreamingDataSnapshotNullUnsafe.LevelTwoAsks.SafeCopy(this, "CLONING_ASKS_FOR_PAINT_FOREGROUND_ON_PanelLevel2"),
				new LevelTwoHalfFrozen.ASC());

			double	priceMax = Math.Max(bids_cachedForOnePaint.PriceMax, asks_cachedForOnePaint.PriceMax);
			double	priceMin = Math.Min(bids_cachedForOnePaint.PriceMin, asks_cachedForOnePaint.PriceMin);
			//double	priceRangeToDisplay = priceMax - priceMin;

			double	lotsMax = Math.Max(bids_cachedForOnePaint.LotSum, asks_cachedForOnePaint.LotSum);
			double	lotsMin = Math.Min(bids_cachedForOnePaint.LotSum, asks_cachedForOnePaint.LotSum);
			double	lotRangeToDisplay = lotsMax - lotsMin;

			//double	pxPerLot_Width = base.Width / lotRangeToDisplay;
			double pxPerLot_Width = base.Width / lotsMax;
			int pxPerPriceStep_Height = panelPrice.PixelsPerPriceStep3pxLeast_cached;
			int		pxPricePanelVertialOffset	= panelPrice.ParentMultiSplitMyLocationAmongAllPanels.Y;
			if     (pxPricePanelVertialOffset == -1) {
				string msg = "PARANOID__PANEL_PRICE_MUST_BE_IN_THE_LIST_OF_MULTISPLITTER_CONTENT_BUT_NOT_FOUND_NONSENSE";
				throw new Exception(msg);
			}

			//double mouseOveredPrice = panelPrice.PanelValueForBarMouseOveredNaNunsafe;
			//if (double.IsNaN(mouseOveredPrice) == false) {
			//    int mouseOveredPriceY = panelPrice.ValueToYinverted(mouseOveredPrice) + pxPricePanelVertialOffset;
			//    using (Pen brown = new Pen(Color.SaddleBrown)) g.DrawLine(brown, 0, mouseOveredPriceY, base.Width, mouseOveredPriceY);
			//}

			//using (Pen black = new Pen(Color.Black)) g.DrawLine(black, 0, pxPricePanelVertialOffset, base.Width, pxPricePanelVertialOffset);

			double askQuote = lastQuote.Ask;
			askQuote = priceMin;
			int priceMinY = panelPrice.ValueToYinverted(priceMin) + pxPricePanelVertialOffset;

			//v1 foreach (double ask in asks_cachedForOnePaint.Keys) {		// Keys may be unsorted in a regular Dictionary => rendering price levels randomly
			//	double lotAbsolute = asks_cachedForOnePaint[ask];
			foreach (KeyValuePair<double, double> keyValue in asks_cachedForOnePaint) {
				double ask = keyValue.Key;
				double lotAbsolute = keyValue.Value;
				double lotCumulative	= asks_cachedForOnePaint.LotsCumulative[ask];
				double lotsRelative		= lotCumulative;// -lotsMin;
				int askY = panelPrice.ValueToYinverted(ask);
				if (askY <= 0) {
					string msg = "FIRST_EVER_BAR_IS_BUGGY__NOT_PAINTED_PROPERLY_ON_PANEL_PRICE";
					continue;
				}
				askY += pxPricePanelVertialOffset;
				//using (Pen gray = new Pen(Color.Gainsboro)) g.DrawLine(gray, 0, askY, base.Width, askY);
				//continue;

				//double diffFromMin = ask - priceMin;
				//int diffFromMinAsPriceStepsAway = (int)Math.Ceiling(diffFromMin / priceStep);
				//int diffFromMinY = diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;
				//askY = priceMinY - diffFromMinY + pxPricePanelVertialOffset;
				//askY -= diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;

				int lotsRelativeWidth = (int)Math.Round(lotsRelative * pxPerLot_Width);

				Rectangle horizontalBar =  base.ParentMultiSplitIamLast
					? new Rectangle(0, askY - pxPerPriceStep_Height, lotsRelativeWidth, pxPerPriceStep_Height)
					: new Rectangle(base.Width - lotsRelativeWidth, askY - pxPerPriceStep_Height, lotsRelativeWidth, pxPerPriceStep_Height);

				g.FillRectangle(this.ChartControl.ChartSettings.BrushLevelTwoAskColorBackground, horizontalBar);

				Point[] leftUpContour = new Point[3];
				leftUpContour[0] = base.ParentMultiSplitIamLast
					? new Point(0, askY)
					: new Point(base.Width, askY);
				leftUpContour[1] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, askY)
					: new Point(base.Width - lotsRelativeWidth, askY);
				leftUpContour[2] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, askY - pxPerPriceStep_Height)
					: new Point(base.Width - lotsRelativeWidth, askY - pxPerPriceStep_Height);
				g.DrawLines(this.ChartControl.ChartSettings.PenLevelTwoAskColorContour, leftUpContour);
			}

			//int askY = panelPrice.ValueToYinverted(this.lastQuote_cached.Ask);
			foreach (KeyValuePair<double, double> keyValue in bids_cachedForOnePaint) {
				double bid = keyValue.Key;
				double lotAbsolute = keyValue.Value;
				double lotCumulative = bids_cachedForOnePaint.LotsCumulative[bid];
				double lotsRelative = lotCumulative;// -lotsMin;
				int bidY = panelPrice.ValueToYinverted(bid);
				if (bidY <= 0) {
					string msg = "FIRST_EVER_BAR_IS_BUGGY__NOT_PAINTED_PROPERLY_ON_PANEL_PRICE";
					continue;
				}
				bidY += pxPricePanelVertialOffset;
				//using (Pen gray = new Pen(Color.Gainsboro)) g.DrawLine(gray, 0, askY, base.Width, askY);
				//continue;

				//double diffFromMin = ask - priceMin;
				//int diffFromMinAsPriceStepsAway = (int)Math.Ceiling(diffFromMin / priceStep);
				//int diffFromMinY = diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;
				//askY = priceMinY - diffFromMinY + pxPricePanelVertialOffset;
				//askY -= diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;

				int lotsRelativeWidth = (int)Math.Round(lotsRelative * pxPerLot_Width);

				Rectangle horizontalBar = base.ParentMultiSplitIamLast
					? new Rectangle(0, bidY, lotsRelativeWidth, pxPerPriceStep_Height)
					: new Rectangle(base.Width - lotsRelativeWidth, bidY, lotsRelativeWidth, pxPerPriceStep_Height);

				g.FillRectangle(this.ChartControl.ChartSettings.BrushLevelTwoBidColorBackground, horizontalBar);

				Point[] leftDownContour = new Point[3];
				leftDownContour[0] = base.ParentMultiSplitIamLast
					? new Point(0, bidY)
					: new Point(base.Width, bidY);
				leftDownContour[1] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, bidY)
					: new Point(base.Width - lotsRelativeWidth, bidY);
				leftDownContour[2] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, bidY + pxPerPriceStep_Height)
					: new Point(base.Width - lotsRelativeWidth, bidY + pxPerPriceStep_Height);
				g.DrawLines(this.ChartControl.ChartSettings.PenLevelTwoBidColorContour, leftDownContour);
			}

		}
	}
}
