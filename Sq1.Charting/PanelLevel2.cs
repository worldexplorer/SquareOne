using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using Sq1.Core;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;
using Sq1.Core.Charting;

namespace Sq1.Charting {
	public class PanelLevel2 : PanelBase {
		const string REASON_TO_EXIST = "besides my own Panel, paint also on a clipped Graphics to draw below candles on PanelPrice (intensivity of layers might be controlled by sliders))";

		[Browsable(false)]	public override bool					PanelHasValuesForVisibleBarWindow		{ get { return false;	/* this will cancel drawing right gutter */ } }
		[Browsable(false)]	protected override int					ValueIndexLastAvailableMinusOneUnsafe	{ get { return -1; } }
		[Browsable(false)]	public			StreamingDataSnapshot	StreamingDataSnapshot_nullUnsafe			{ get {
			if (base.ChartControl.Executor == null) return null;		// for TestChartControl
			if (base.ChartControl.Executor.DataSource_fromBars.StreamingAdapter == null) return null;
			return base.ChartControl.Executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot;
		} }

		bool errorDetected;

		public PanelLevel2() : base() {
			//base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
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
			//temp reset to measure through Graphics.MeasureString()
			base.ChartControl.ChartSettings.LevelTwoMinimumPriceLevelThicknessRendered = -1;
		}

		void panelPrice_MouseMove(object sender, MouseEventArgs e) {
			base.Invalidate();
		}
		void panelPrice_OnPanelPriceSqueezed(object sender, EventArgs e) {
			base.Invalidate();
		}

		//using (var brush = new SolidBrush(Color.Red)) {
		//	Font font = (base.ChartControl != null) ? base.ChartControl.ChartSettings.PanelNameAndSymbolFont : this.Font;
		//	g.DrawString(msgRepaint, font, brush, new Point(60, 60));
		//}

//		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
//			base.PaintWholeSurfaceBarsNotEmpty(g);

#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD 
		protected override void OnPaintBackground(PaintEventArgs pe) {
			if (base.DesignMode) {
				base.OnPaintBackground(pe);
				return;
			}

			//ONLY_FOR_PRICE_PANEL?? //base.OnPaintBackgroundDoubleBuffered(pe);	 //base.DrawError drew label only lowest 2px shown from top of panel
			if (base.ChartControl == null) {
				base.OnPaintBackground(pe);
				return;
			}
#else
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs pe) {
			if (base.ChartControl == null) {
				base.OnPaintBackgroundDoubleBuffered(pe);
				return;
			}
#endif
			this.ChartLabelsUpperLeftYincremental = base.ChartControl.ChartSettings.ChartLabelsUpperLeftYstartTopmost;
			Graphics g = pe.Graphics;
			g.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			
			try {
				g.Clear(base.ChartControl.ChartSettings.LevelTwoColorBackground);
				
				base.RepaintSernoBackground++;
				if (base.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) {
					base.DrawError(g, "BACKTEST_IS_RUNNING_WAIT");
					string msgRepaint = "repaintFore#" + this.RepaintSernoForeground + "/Back#" + this.RepaintSernoBackground;
					base.DrawError(g, msgRepaint);
					if (this.Cursor != Cursors.WaitCursor) this.Cursor = Cursors.WaitCursor;
					return;
				}
				if (this.Cursor != Cursors.Default) this.Cursor = Cursors.Default;
				
				//temp pickup to measure through Graphics.MeasureString()
				if (base.ChartControl.ChartSettings.LevelTwoMinimumPriceLevelThicknessRendered != -1) return;
	
				// same as PanelBase.ensureFontMetricsAreCalculated(e.Graphics);
				SizeF labelMeasurements = pe.Graphics.MeasureString("ABC123`'jg]", base.ChartControl.ChartSettings.LevelTwoLotFont);
				//int labelMeasurementsWidth = (int)Math.Round(labelMeasurements.Width);
				int labelMeasurementsHeight = (int)Math.Round(labelMeasurements.Height);
				base.ChartControl.ChartSettings.LevelTwoMinimumPriceLevelThicknessRendered = labelMeasurementsHeight;
			} catch (Exception ex) {
				string msg = "OnPaintBackgroundDoubleBuffered(): caught[" + ex.Message + "]";
				Assembler.PopupException(msg, ex);
				base.DrawError(g, msg);
			}

		}
#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD 
		protected override void OnPaint(PaintEventArgs pe) {
			if (base.DesignMode) {
				base.OnPaint(pe);
				return;
			}
			if (base.ChartControl == null) {
				base.OnPaint(pe);	// will e.Graphics.Clear(base.BackColor);
				return;
			}
#else
		protected override void OnPaintDoubleBuffered(PaintEventArgs pe) {
			if (base.ChartControl == null) {
				base.OnPaintDoubleBuffered(pe);	// will e.Graphics.Clear(base.BackColor);
				return;
			}
#endif

			if (base.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) return;

			Graphics g = pe.Graphics;
			g.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"

			this.errorDetected = false;
			if (this.StreamingDataSnapshot_nullUnsafe == null) {
				base.DrawError(g, "EDIT_DATASOURCE_SELECT_STREAMING_ADAPTER");
				this.errorDetected = true;
			}
			if (base.ChartControl.Bars == null) {
				base.DrawError(g, "CHART_CONTROL_HAS_NO_BARS");
				this.errorDetected = true;
			}
			Quote quoteCurrent = null;
			if (this.errorDetected == false) {
				quoteCurrent = this.StreamingDataSnapshot_nullUnsafe.GetQuoteCurrent_forSymbol_nullUnsafe(base.ChartControl.Bars.Symbol);
				if (quoteCurrent == null) {
					base.DrawError(g, "CONNECT_STREAMING__OR__CHART>BARS>SUBSCRIBE");
					this.errorDetected = true;
				}
			}

			base.RepaintSernoForeground++;
			if (this.errorDetected) {
				string whoAmI = "Level2 first[" + base.ParentMultiSplitIamFirst + "] last[" + base.ParentMultiSplitIamLast + "]";
				base.DrawError(g, whoAmI);
				string msgRepaint = "repaintFore#" + this.RepaintSernoForeground + "/Back#" + this.RepaintSernoBackground;
				base.DrawError(g, msgRepaint);
				return;
			}

			try {
				this.renderLevel2(pe.Graphics, quoteCurrent);
				this.renderBidAsk(pe.Graphics);
			} catch (Exception ex) {
				base.DrawError(pe.Graphics, ex.ToString());
			}
		}
		void renderLevel2(Graphics g, Quote quoteLast) {
			PanelPrice panelPrice = base.ChartControl.PanelPrice;
			double priceStep = panelPrice.PriceStep;

			//string msgPixelsPerPriceStep = "Px[" + base.Height + "]/PriceStep[" + priceStep + "]=" 
			//	+ panelPrice.PixelsPerPriceStep3pxLeast_cached;
			//base.DrawError(g, msgPixelsPerPriceStep);

			//string msgCalc = "PixelsPerPriceStepMadeVisible_cached[" + panelPrice.PixelsPerPriceStep3pxLeast_cached
			//	+ "] = PriceLevelsShown_cached[" + panelPrice.PriceLevelsShown_cached
			//	+ "]/VisibleMaxPlusBottomSqueezer_cached[" + panelPrice.VisibleMaxPlusBottomSqueezer_cached + "]";
			//base.DrawError(g, msgCalc);

			//v1
			//LevelTwoHalfSortedFrozen asks_frozenAsc_forOnePaint		= base.ChartControl.ScriptExecutorObjects.Asks_frozenAsc_forOnePaint;
			//LevelTwoHalfSortedFrozen bids_frozenDesc_forOnePaint	= base.ChartControl.ScriptExecutorObjects.Bids_frozenDesc_forOnePaint;
			//v2
			LevelTwoFrozen levelTwo_frozenForOnePaint = base.ChartControl.ExecutorObjects_frozenForRendering.LevelTwo_frozen_forOnePaint;
			LevelTwoHalfSortedFrozen asks_frozenAsc_forOnePaint		= levelTwo_frozenForOnePaint.Asks_sortedAsc;
			LevelTwoHalfSortedFrozen bids_frozenDesc_forOnePaint	= levelTwo_frozenForOnePaint.Bids_sortedDesc;

			if (asks_frozenAsc_forOnePaint == null) {
				string msg = "HAPPENS_AT_FIRST_FEW_QUOTES_OF_STARTING_LIVESIM base.ChartControl.ScriptExecutorObjects.Asks_sortedCachedForOnePaint=null";
				//Assembler.PopupException(msg);
				return;
			}
			if (bids_frozenDesc_forOnePaint == null) {
				string msg = "HAPPENS_AT_FIRST_FEW_QUOTES_OF_STARTING_LIVESIM base.ChartControl.ScriptExecutorObjects.Bids_frozenDesc_forOnePaint=null";
				//Assembler.PopupException(msg);
				return;
			}

			//double	priceMax = Math.Max(bids_sortedCachedForOnePaint.PriceMax, asks_sortedCachedForOnePaint.PriceMax);
			//double	priceMin = Math.Min(bids_sortedCachedForOnePaint.PriceMin, asks_sortedCachedForOnePaint.PriceMin);
			//double	priceRangeToDisplay = priceMax - priceMin;

			int pxPerPriceStep_Height = panelPrice.PixelsPerPriceStep5pxLeast_cached;
			int pxPricePanelVertialOffset	= panelPrice.ParentMultiSplitMyLocationAmongSiblingsPanels.Y;
			if (pxPricePanelVertialOffset == -1) {
				string msg = "PARANOID__PANEL_PRICE_MUST_BE_IN_THE_LIST_OF_MULTISPLITTER_CONTENT_BUT_NOT_FOUND_NONSENSE";
				throw new Exception(msg);
			}

			double quoteAsk = quoteLast.Ask;
			//quoteAsk = priceMin;
			int quoteAskYoffsetted = panelPrice.ValueToYinverted(quoteAsk) + pxPricePanelVertialOffset;

			double quoteBid = quoteLast.Bid;
			//quoteBid = priceMax;
			int quoteBidYofsetted = panelPrice.ValueToYinverted(quoteBid) + pxPricePanelVertialOffset;


			//v1 bool allowUnproportional = true;
			bool stripeHeightWillContainMeasuredText = base.ChartControl.ChartSettings.LevelTwoStripesHeightWrapsVolumeLabel;
			if (stripeHeightWillContainMeasuredText) {
				int howManyAskPriceLevelsWillFit = (int) Math.Round(quoteAskYoffsetted / (double)pxPerPriceStep_Height);
				int howManyBidPriceLevelsWillFit = (int) Math.Round((base.ClientRectangle.Height - quoteBidYofsetted) / (double)pxPerPriceStep_Height);
				// they are not equal because of the height of the DomControl; spread is not accounted so I leave some space for the spread by trimming a longer moustache
				int depthFittingToDisplayedHeight = Math.Min(howManyBidPriceLevelsWillFit, howManyAskPriceLevelsWillFit);
				//v1
				//bool requestingLongerMoustachesThanIhave =
				//	depthFittingToDisplayedHeight >= asks_sortedCachedForOnePaint.Count ||
				//	depthFittingToDisplayedHeight >= bids_sortedCachedForOnePaint.Count;
				//if (requestingLongerMoustachesThanIhave == false) {
				//	asks_sortedCachedForOnePaint = asks_sortedCachedForOnePaint.Clone_noDeeperThan(depthFittingToDisplayedHeight);
				//	bids_sortedCachedForOnePaint = bids_sortedCachedForOnePaint.Clone_noDeeperThan(depthFittingToDisplayedHeight);
				//}
				//v2 cut moustaches separately
				//if (howManyAskPriceLevelsWillFit < depthFittingToDisplayedHeight) {
				if (howManyAskPriceLevelsWillFit < asks_frozenAsc_forOnePaint.Count) {
					asks_frozenAsc_forOnePaint = asks_frozenAsc_forOnePaint.Clone_noDeeperThan(howManyAskPriceLevelsWillFit);
				}
				//if (howManyBidPriceLevelsWillFit < depthFittingToDisplayedHeight) {
				if (howManyBidPriceLevelsWillFit < bids_frozenDesc_forOnePaint.Count) {
					bids_frozenDesc_forOnePaint = bids_frozenDesc_forOnePaint.Clone_noDeeperThan(howManyBidPriceLevelsWillFit);
				}
			}

			double	lotsMax = Math.Max(bids_frozenDesc_forOnePaint.LotSum, asks_frozenAsc_forOnePaint.LotSum);
			double	lotsMin = Math.Min(bids_frozenDesc_forOnePaint.LotSum, asks_frozenAsc_forOnePaint.LotSum);

			double	lotsMaxVisible = lotsMax;
			double	lotsMinVisible = lotsMin;

			double	lotRangeToDisplay = lotsMaxVisible - lotsMinVisible;

			//double	pxPerLot_Width = base.Width / lotRangeToDisplay;
			double pxPerLot_Width = base.Width / lotsMaxVisible;

			//double mouseOveredPrice = panelPrice.PanelValueForBarMouseOveredNaNunsafe;
			//if (double.IsNaN(mouseOveredPrice) == false) {
			//	int mouseOveredPriceY = panelPrice.ValueToYinverted(mouseOveredPrice) + pxPricePanelVertialOffset;
			//	using (Pen brown = new Pen(Color.SaddleBrown)) g.DrawLine(brown, 0, mouseOveredPriceY, base.Width, mouseOveredPriceY);
			//}

			//using (Pen black = new Pen(Color.Black)) g.DrawLine(black, 0, pxPricePanelVertialOffset, base.Width, pxPricePanelVertialOffset);



			#region draw Asks upwards [spread...Y=0]
			//v1 foreach (double ask in asks_cachedForOnePaint.Keys) {		// Keys may be unsorted in a regular Dictionary => rendering price levels randomly
			//	double lotAbsolute = asks_cachedForOnePaint[ask];
#if DEBUG
			double lotCumulativePrev = 0;
#endif
			foreach (KeyValuePair<double, double> keyValue in asks_frozenAsc_forOnePaint) {
				double ask				= keyValue.Key;
				double lotAbsolute		= keyValue.Value;
				double lotCumulative	= asks_frozenAsc_forOnePaint.LotsCumulative[ask];
				double lotsRelative		= lotCumulative;// -lotsMin;

#if DEBUG
				if (lotCumulativePrev != 0) {
					if (lotCumulativePrev >= lotCumulative) {
						string msg = "ASKS_MUST_INCREASE_AND_LOTS_CUMULATIVE_MUST_GROW";
						Assembler.PopupException(msg);
					}
				}
				lotCumulativePrev = lotCumulative;
#endif

				int yAsk = -1;
				if (stripeHeightWillContainMeasuredText) {
					// using panelPrice.PixelsPerPriceStep5pxLeast_cached as minimal step (making price level wider on PanelLevel2 than price level displayed on PanelPrice for visual convenience)
					double diffFromQuoteAsk = ask - quoteAsk;
					if (diffFromQuoteAsk < 0) {
						string msg = "next ask must have greater price";
						Assembler.PopupException(msg);
					}
					int diffFromQuoteAskAsPriceStepsAway = (int)Math.Ceiling(diffFromQuoteAsk / priceStep);
					int diffFromQuoteAskY = diffFromQuoteAskAsPriceStepsAway * pxPerPriceStep_Height;
					yAsk = quoteAskYoffsetted - diffFromQuoteAskY;
					//v1 askY -= diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;
				} else {
					panelPrice.ValueToYinverted(ask);
					if (yAsk <= 0) {
						string msg = "FIRST_EVER_BAR_IS_BUGGY__NOT_PAINTED_PROPERLY_ON_PANEL_PRICE";
						continue;
					}
					yAsk += pxPricePanelVertialOffset;
					//using (Pen gray = new Pen(Color.Gainsboro)) g.DrawLine(gray, 0, askY, base.Width, askY);
					//continue;
				}

				int lotsRelativeWidth = (int)Math.Round(lotsRelative * pxPerLot_Width);

				Rectangle horizontalBar = base.ParentMultiSplitIamLast
					? new Rectangle(0, yAsk - pxPerPriceStep_Height, lotsRelativeWidth, pxPerPriceStep_Height)
					: new Rectangle(base.Width - lotsRelativeWidth, yAsk - pxPerPriceStep_Height, lotsRelativeWidth, pxPerPriceStep_Height);
				//if (base.ClientRectangle.Contains(horizontalBar) == false) continue;
				if (base.ClientRectangle.IntersectsWith(horizontalBar) == false) continue;

				g.FillRectangle(base.ChartControl.ChartSettings.BrushLevelTwoAskColorBackground, horizontalBar);

				Point[] leftUpContour = new Point[3];
				leftUpContour[0] = base.ParentMultiSplitIamLast
					? new Point(0, yAsk)
					: new Point(base.Width, yAsk);
				leftUpContour[1] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, yAsk)
					: new Point(base.Width - lotsRelativeWidth, yAsk);
				leftUpContour[2] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, yAsk - pxPerPriceStep_Height)
					: new Point(base.Width - lotsRelativeWidth, yAsk - pxPerPriceStep_Height);
				g.DrawLines(base.ChartControl.ChartSettings.PenLevelTwoAskColorContour, leftUpContour);

				string lotCumulativeFormatted = lotCumulative.ToString(base.VolumeFormat);
				SizeF labelMeasurements = g.MeasureString(lotCumulativeFormatted, base.ChartControl.ChartSettings.LevelTwoLotFont);
				int labelMeasurementsWidth = (int)Math.Round(labelMeasurements.Width);
				int xLabel = base.ParentMultiSplitIamLast
					? lotsRelativeWidth - labelMeasurementsWidth - base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal
					: base.Width - lotsRelativeWidth + base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal;
				if (xLabel > base.Width) xLabel = base.Width;
				if (base.ParentMultiSplitIamLast) {
					if (xLabel < 0) xLabel = 0;
				} else {
					if (xLabel + labelMeasurementsWidth + base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal > base.Width) {
						xLabel = base.Width - labelMeasurementsWidth - base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal;
					}
				}

				//int middleOfBar = (int)(pxPerPriceStep_Height / 2d);
				//int middleOfLabel = (int)(labelMeasurements.Height / 2d);
				int yLabel = yAsk - (int)labelMeasurements.Height;	//Math.Min(middleOfBar, middleOfLabel);	// volume drawn inside the horizontalBar, above actual ask
				if (yLabel > base.Height) yLabel = base.Height;
				g.DrawString(lotCumulativeFormatted,
					base.ChartControl.ChartSettings.SpreadLabelFont,
					base.ChartControl.ChartSettings.BrushLevelTwoLot, xLabel, yLabel);
			}
			#endregion

			#region draw Bids downwards [spread...Y=base.Height]
			//int askY = panelPrice.ValueToYinverted(this.quoteLast_cached.Ask);
#if DEBUG
			lotCumulativePrev = 0;
#endif
			foreach (KeyValuePair<double, double> keyValue in bids_frozenDesc_forOnePaint) {
				double bid				= keyValue.Key;
				double lotAbsolute		= keyValue.Value;
				double lotCumulative	= bids_frozenDesc_forOnePaint.LotsCumulative[bid];
				double lotsRelative		= lotCumulative;// -lotsMin;

#if DEBUG
				if (lotCumulativePrev != 0) {
					if (lotCumulativePrev >= lotCumulative) {
						string msg = "BIDS_MUST_DECREAS_BUT_LOTS_CUMULATIVE_MUST_GROW";
						Assembler.PopupException(msg);
					}
				}
				lotCumulativePrev = lotCumulative;
#endif

				int yBid = -1;
				if (stripeHeightWillContainMeasuredText) {
					// using panelPrice.PixelsPerPriceStep5pxLeast_cached as minimal step (making price level wider on PanelLevel2 than price level displayed on PanelPrice for visual convenience)
					double diffFromQuoteBid = quoteBid - bid;
					if (diffFromQuoteBid < 0) {
						string msg = "next bid must have lesser price";
						Assembler.PopupException(msg);
					}
					int diffFromQuoteBidAsPriceStepsAway = (int)Math.Ceiling(diffFromQuoteBid / priceStep);
					int diffFromQuoteBidY = diffFromQuoteBidAsPriceStepsAway * pxPerPriceStep_Height;
					yBid = quoteBidYofsetted + diffFromQuoteBidY;
					//v1 bidY -= diffFromMinAsPriceStepsAway * pxPerPriceStep_Height;
				} else {
					panelPrice.ValueToYinverted(bid);
					if (yBid <= 0) {
						string msg = "FIRST_EVER_BAR_IS_BUGGY__NOT_PAINTED_PROPERLY_ON_PANEL_PRICE";
						continue;
					}
					yBid += pxPricePanelVertialOffset;
					//using (Pen gray = new Pen(Color.Gainsboro)) g.DrawLine(gray, 0, askY, base.Width, askY);
					//continue;
				}

				int lotsRelativeWidth = (int)Math.Round(lotsRelative * pxPerLot_Width);

				Rectangle horizontalBar = base.ParentMultiSplitIamLast
					? new Rectangle(0, yBid, lotsRelativeWidth, pxPerPriceStep_Height)
					: new Rectangle(base.Width - lotsRelativeWidth, yBid, lotsRelativeWidth, pxPerPriceStep_Height);
				//if (base.ClientRectangle.Contains(horizontalBar) == false) continue;
				if (base.ClientRectangle.IntersectsWith(horizontalBar) == false) continue;

				g.FillRectangle(base.ChartControl.ChartSettings.BrushLevelTwoBidColorBackground, horizontalBar);

				Point[] leftDownContour = new Point[3];
				leftDownContour[0] = base.ParentMultiSplitIamLast
					? new Point(0, yBid)
					: new Point(base.Width, yBid);
				leftDownContour[1] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, yBid)
					: new Point(base.Width - lotsRelativeWidth, yBid);
				leftDownContour[2] = base.ParentMultiSplitIamLast
					? new Point(lotsRelativeWidth, yBid + pxPerPriceStep_Height)
					: new Point(base.Width - lotsRelativeWidth, yBid + pxPerPriceStep_Height);
				g.DrawLines(base.ChartControl.ChartSettings.PenLevelTwoBidColorContour, leftDownContour);

				string lotCumulativeFormatted = lotCumulative.ToString(base.VolumeFormat);
				SizeF labelMeasurements = g.MeasureString(lotCumulativeFormatted, base.ChartControl.ChartSettings.LevelTwoLotFont);
				int labelMeasurementsWidth = (int)Math.Round(labelMeasurements.Width);
				int xLabel = base.ParentMultiSplitIamLast
					? lotsRelativeWidth - labelMeasurementsWidth - base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal
					: base.Width - lotsRelativeWidth + base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal;
				if (base.ParentMultiSplitIamLast) {
					if (xLabel < 0) xLabel = 0;
				} else {
					if (xLabel + labelMeasurementsWidth + base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal > base.Width) {
						xLabel = base.Width - labelMeasurementsWidth - base.ChartControl.ChartSettings.LevelTwoLotPaddingHorizontal;
					}
				}

				//int middleOfBar = (int)(pxPerPriceStep_Height / 2d);
				//int middleOfLabel = (int)(labelMeasurements.Height / 2d);
				int yLabel = yBid + 1;	//Math.Min(middleOfBar, middleOfLabel);	// volume drawn inside the horizontalBar, below actual bid
				if (yLabel < 0) yLabel = 0;
				g.DrawString(lotCumulativeFormatted,
					base.ChartControl.ChartSettings.SpreadLabelFont,
					base.ChartControl.ChartSettings.BrushLevelTwoLot, xLabel, yLabel);
			}
			#endregion

		}
		void renderBidAsk(Graphics g) {
			Quote quoteCurrent = base.ChartControl.ExecutorObjects_frozenForRendering.QuoteCurrent;
			if (quoteCurrent == null) return;

			//Quote quoteLastFromDictionary = this.StreamingDataSnapshot_nullUnsafe.LastQuoteCloneGetForSymbol(base.ChartControl.Bars.Symbol);
			//if (quoteLast.SameBidAsk(quoteLastFromDictionary) == false) {
			//	string msg = "GOOD_THAT_YOU_CACHED DATASNAP_UNSYNCED_EXECOBJ quoteLast[" + quoteLast + "] != quoteLastFromDictionary[" + quoteLastFromDictionary + "]";
			//	Assembler.PopupException(msg, null, false);
			//	//return;
			//}

			double spread = quoteCurrent.Spread;
			if (double.IsNaN(spread) == true) return;

			ChartSettings settings = base.ChartControl.ChartSettings;

			PanelPrice panelPrice = base.ChartControl.PanelPrice;
			int		pxPricePanel_vertialOffset	= panelPrice.ParentMultiSplitMyLocationAmongSiblingsPanels.Y;

			string spreadFormatted = spread.ToString(base.PriceFormat);
			string labelSpread = "spread[" + spreadFormatted + "]";
			SizeF labelSpread_measured = g.MeasureString(labelSpread, settings.SpreadLabelFont);
			int labelSpread_measuredWidth	= (int)labelSpread_measured.Width;
			int labelSpread_measuredHeight	= (int)labelSpread_measured.Height;
			int padding = 4;
	
			int spreadLinesWidth = base.Width - labelSpread_measuredWidth - padding * 3;
			if (spreadLinesWidth < 10) spreadLinesWidth = 10;			// im not testing g.DrawString() if it accepts negative values; just want it to draw
			bool showLabels_onRightEdge = base.ParentMultiSplitIamLast;

			int xLinesSpread = showLabels_onRightEdge
				? 0
				: labelSpread_measuredWidth + padding * 2;

			double bid = quoteCurrent.Bid;
			double ask = quoteCurrent.Ask;
			int yBid = 0;
			int yAsk = 0;
			if (double.IsNaN(bid) == false) {
				yBid = panelPrice.ValueToYinverted(bid) + pxPricePanel_vertialOffset;
				g.DrawLine(settings.PenSpreadBid, xLinesSpread, yBid, spreadLinesWidth, yBid);
			}
			if (double.IsNaN(ask) == false) {
				yAsk = panelPrice.ValueToYinverted(ask) + pxPricePanel_vertialOffset;
				g.DrawLine(settings.PenSpreadAsk, xLinesSpread, yAsk, spreadLinesWidth, yAsk);
			}

			int xLabelSpread = showLabels_onRightEdge
				? base.Width - labelSpread_measuredWidth - padding * 2
				: padding;
			if (xLabelSpread < padding) xLabelSpread = padding;		// im not testing g.DrawString() if it accepts negative values; just want it to draw
			int yLabelSpread = (int) ((yBid + yAsk) / 2 - labelSpread_measuredHeight / 2);	// in the middle between spread lines
			if (yLabelSpread > base.Height) yLabelSpread = base.Height - labelSpread_measuredHeight;
			if (yLabelSpread < 0) yLabelSpread = 0;

			if (yLabelSpread < padding) yLabelSpread = padding;		// im not testing g.DrawString() if it accepts negative values; just want it to draw

			g.DrawString(labelSpread, settings.SpreadLabelFont, settings.BrushSpreadLabel, xLabelSpread, yLabelSpread);

			string labelBidsDown = "bids/buy";
			int ylabelBidsDown = yLabelSpread + labelSpread_measuredHeight;		// + padding * 2;
			if (ylabelBidsDown > base.Height) ylabelBidsDown = yLabelSpread;
			g.DrawString(labelBidsDown, settings.SpreadLabelFont, settings.BrushSpreadBid, xLabelSpread, ylabelBidsDown);

			string labelAsksUp = "asks/sell";
			int ylabelAsksUp = yLabelSpread - labelSpread_measuredHeight;		// - padding * 2;
			if (ylabelAsksUp <= 0) ylabelAsksUp = yLabelSpread;
			g.DrawString(labelAsksUp, settings.SpreadLabelFont, settings.BrushSpreadAsk, xLabelSpread, ylabelAsksUp);
		}
	}
}
