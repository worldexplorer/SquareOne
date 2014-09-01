using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Charting;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public partial class PanelNamedFolding :
		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_CLICK_THROUGH #if NON_DOUBLE_BUFFERED
		//	Panel
		//#else
		PanelDoubleBuffered
		//UserControlDoubleBuffered
		//#endif
		, HostPanelForIndicator
	{
		//[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public string PanelName { get; set; }
		[Browsable(false)] public string BarsIdent { get; set; }		//if a public field isn't a property, Designer will crash
		public bool Collapsible { get; set; }
		public bool CollapsedToName { get; set; }
		public bool GutterRightDraw { get; set; }
		public bool GutterBottomDraw { get; set; }
		//public bool GridLinesHorizontalHowManyLinesToDraw { get; set; }
		
		public bool ImPaintingForegroundNow = false;
		public bool ImPaintingBackgroundNow = false;

		[Browsable(false)] public string PanelNameAndSymbol { get { return this.PanelName + " " + this.BarsIdent; } }
		[Browsable(false)] public ChartControl ChartControl;	//{ get { return base.Parent as ChartControl; } }
		int yForNextLabelUpperLeft = 0;
		
		// price, volume or indicator-calculated
		[Browsable(false)] public virtual double VisibleMin { get { return this.ChartControl.VisiblePriceMin; } }
		[Browsable(false)] public virtual double VisibleMax { get { return this.ChartControl.VisiblePriceMax; } }
		// USE_CACHED_VARIABLE_INSTEAD [Browsable(false)] public virtual double VisibleRange { get { return this.VisibleMax - this.VisibleMin; } }

		#region Panel.Width and Panel.Height - dependant (cache and recalculate once per Resize/Paint))
		[Browsable(false)]
		public int PanelHeightMinusGutterBottomHeight { get {
				int ret = base.Height;
				if (this.GutterBottomDraw) ret -= this.GutterBottomHeight_cached;
				return ret;
			} }
		[Browsable(false)]
		private int PanelWidthMinusRightPriceGutter { get {
				int ret = base.Width;
				if (this.GutterRightDraw) ret -= this.ChartControl.GutterRightWidth_cached;
				return ret;
			} }
		
		protected int VisibleBarRight_cached;
		protected int VisibleBarLeft_cached;
		protected int VisibleBarsCount_cached;

		protected double VisibleMin_cached;
		protected double VisibleMax_cached;
		protected double VisibleRange_cached;
		
		protected double VisibleMinMinusTopSqueezer_cached;
		protected double VisibleMaxPlusBottomSqueezer_cached;
		protected double VisibleRangeWithTwoSqueezers_cached;
		
		
		protected int BarWidthIncludingPadding_cached;
		protected int BarWidthMinusRightPadding_cached;
		protected int BarShadowXoffset_cached;
		protected int PanelHeightMinusGutterBottomHeight_cached;
		
	 	protected int GutterRightFontHeight_cached = -1;
	 	protected int GutterRightFontHeightHalf_cached = -1;
	 	protected int GutterBottomFontHeight_cached = -1;
	 	protected int GutterBottomHeight_cached = -1;
		#endregion
		
		public bool ThisPanelIsPricePanel;
		[Browsable(false)] public virtual int PaddingVerticalSqueeze { get { return 0; } }

		[Browsable(false)]
		public int VisibleBarRightExisting { get {
				int ret = this.VisibleBarRight_cached;
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) ret = this.ChartControl.Bars.Count - 1;
				return ret;
			} }
		[Browsable(false)]
		public int VisibleBarLeftExisting { get {
				int ret = this.VisibleBarLeft_cached;
				if (this.VisibleBarLeft_cached >= this.ChartControl.Bars.Count) ret = this.ChartControl.Bars.Count - 1;
				return ret;
			} }

		string formatForBars { get {
				string ret = "FORMAT_FOR_BARS_UNDEFINED";
				Bars bars = this.ChartControl.Bars;
				switch (bars.ScaleInterval.Scale) {
						case BarScale.Minute: ret = this.ChartControl.ChartSettings.GutterBottomDateFormatIntraday; break;
						case BarScale.Daily: ret = this.ChartControl.ChartSettings.GutterBottomDateFormatDaily; break;
					case BarScale.Weekly:
					case BarScale.Monthly:
						case BarScale.Quarterly: ret = this.ChartControl.ChartSettings.GutterBottomDateFormatDaily; break;
						case BarScale.Yearly: ret = this.ChartControl.ChartSettings.GutterBottomDateFormatYearly; break;
						default: ret = "FORMAT_FOR_BARS_UNDEFINED"; break;
				}
				return ret;
			} }
		public int BarShadowOffset { get { return this.ChartControl.ChartSettings.BarShadowXoffset; } }

		public PanelNamedFolding() {
			this.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.Collapsible = true;
			this.CollapsedToName = false;
			this.GutterRightDraw = true;
			this.GutterBottomDraw = false;
			this.AutoScroll = false;
			this.HScroll = true;
			//this.GridLinesHorizontalHowManyLinesToDraw = 10;
			this.ThisPanelIsPricePanel = this.GetType() == typeof(PanelPrice);
		}
		public void Initialize(ChartControl chartControl, string barsIdent = "INITIALIZED_WITH_EMPTY_BARS_IDENT_PanelNamedFolding") {
			this.ChartControl = chartControl;
			this.BarsIdent = barsIdent;
		}
		public void InitializeWithNonEmptyBars(ChartControl chartControl) {
			string barsIdent = chartControl.Bars.ToString();
			barsIdent = chartControl.Bars.SymbolIntervalScale;
			this.Initialize(chartControl, barsIdent);
		}
		public void DrawLabelOnNextLine(Graphics g, string msg, Color color, bool drawIndicatorSquare = false) {
			if (color == null) color = Color.Red;	// error
			this.yForNextLabelUpperLeft += 12;
			int x = 10;
			if (drawIndicatorSquare) x = 4;
			using (var brush = new SolidBrush(color)) {
				if (drawIndicatorSquare) {
					Rectangle square = new Rectangle(x, this.yForNextLabelUpperLeft + 4, 5, 5);
					g.FillRectangle(brush, square);
					x += 8;
				}
				g.DrawString(msg, this.Font, brush, new Point(x, this.yForNextLabelUpperLeft));
			}
		}
		public void DrawError(Graphics g, string msg) {
			this.DrawLabelOnNextLine(g, msg, Color.Red);
		}
		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD #if NON_DOUBLE_BUFFERED
//		protected override void OnResize(EventArgs e) {	//PanelDoubleBuffered does this already to DisposeAndNullify managed Graphics
//		    base.Invalidate();	// SplitterMoved => repaint; Panel and UserControl don't have that (why?)
//		    base.OnResize(e);	// empty inside but who knows what useful job it does?
//		}
//		protected override void OnPaint(PaintEventArgs e) {
//			base.OnPaint(e);
		//#else
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
			//#endif
			//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
			e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			
			if (this.ChartControl == null) {
				this.DrawError(e.Graphics, "PanelNamedFolding[" + this.PanelName + "].ChartControl=null; invoke PanelNamedFolding.Initialize() from derived.ctor()");
				return;
			}
			if (this.ChartControl.BarsEmpty) {
				string msig = this.Name + ".OnPaintDoubleBuffered() ";
				string msg = "CHART_CONTROL_BARS_NULL_OR_EMPTY: this.ChartControl.BarsEmpty ";
				//if (this.ChartControl.Bars != null) msg = "BUG: bars=[" + this.ChartControl.Bars + "]";
				this.DrawError(e.Graphics, msg + msig);
				return;
			}
			this.ChartControl.SyncHorizontalScrollToBarsCount();
			if (this.ImPaintingForegroundNow) return;
			try {
				this.ImPaintingForegroundNow = true;
				this.PaintWholeSurfaceBarsNotEmpty(e.Graphics);	// GOOD: we get here once per panel
				this.PaintIndicators(e.Graphics);
				// draw Panel Title on top of anything that the panel draws
				if (this.Collapsible) {
					if (this.PanelName != null) {
						using (var brush = new SolidBrush(this.ForeColor)) {
							Font font = (this.ChartControl != null) ? this.ChartControl.ChartSettings.PanelNameAndSymbolFont : this.Font;
							e.Graphics.DrawString(this.PanelNameAndSymbol, font, brush, new Point(2, 2));
						}
					} else {
						this.DrawError(e.Graphics, "SET_TO_EMPTY_STRING_TO_HIDE: this.PanelName=null");
					}
				}
			} catch (Exception ex) {
				string msg = "OnPaintDoubleBuffered(): caught[" + ex.Message + "]";
				Assembler.PopupException(msg, ex);
				this.DrawError(e.Graphics, msg);
				Debugger.Break();
			} finally {
				this.ImPaintingForegroundNow = false;
			}
		}

		protected void PaintIndicators(Graphics graphics) {
			Dictionary<string, Indicator> indicators = this.ChartControl.ScriptExecutorObjects.Indicators;
			if (indicators == null) return;
			if (indicators.Values.Count == 0) return;

			foreach (Indicator indicator in indicators.Values) {
				if (indicator.HostPanelForIndicator != this) continue;
				indicator.DotsExistsForCurrentSlidingWindow = 0;
				indicator.DotsDrawnForCurrentSlidingWindow = 0;
			}

			int barX = this.ChartControl.ChartWidthMinusGutterRightPrice;
			// i > this.VisibleBarLeft_cached is enough because Indicator.Draw() takes previous bar
			for (int i = this.VisibleBarRight_cached; i > this.VisibleBarLeft_cached; i--) {
				barX -= this.BarWidthIncludingPadding_cached;
				Bar bar = this.ChartControl.Bars[i];

				int barYHighInverted = this.ValueToYinverted(bar.High);
				int barYLowInverted = this.ValueToYinverted(bar.Low);

				int candleBodyHeight = barYLowInverted - barYHighInverted;		// height is measured DOWN the screen from candleBodyInverted.Y, not UP
				if (candleBodyHeight < 0) Debugger.Break();
				if (candleBodyHeight == 0) candleBodyHeight = 1;

				Rectangle candleBodyInverted = default(Rectangle);
				candleBodyInverted.X = barX;
				candleBodyInverted.Width = this.BarWidthMinusRightPadding_cached;
				candleBodyInverted.Y = barYHighInverted;					// drawing down, since Y grows down the screen from left upper corner (0:0)
				candleBodyInverted.Height = candleBodyHeight;

				foreach (Indicator indicator in indicators.Values) {
					if (indicator.HostPanelForIndicator != this) continue;
					bool indicatorLegDrawn = indicator.DrawValue(graphics, bar, candleBodyInverted);
				}
			}
			foreach (Indicator indicator in indicators.Values) {
				if (indicator.HostPanelForIndicator != this) continue;
				if (indicator.Executor == null) continue;
				string msg = indicator.NameWithParameters;
				if (indicator.DotsDrawnForCurrentSlidingWindow <= 0 && indicator.Executor != null) {
					Bar barLeft = this.ChartControl.Bars[this.VisibleBarLeft_cached];
					string dateRq = barLeft.DateTimeOpen.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor);
					// indicator.BarsEffective will throw if indicator.Executor==null
					string dateFirst = indicator.BarsEffective.BarFirst.DateTimeOpen.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor);
					string dateLast = indicator.BarsEffective.BarLast.DateTimeOpen.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor);
					//TOO_NOISY_SEE_TOOLTIP_PRICE msg += " (" + dateFirst + ")..(" + dateLast + ") <> barRq[" + dateRq + "]";
				}
				this.DrawLabelOnNextLine(graphics, msg, indicator.LineColor, true);
			}
		}
		protected virtual void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			//this.PaintError(g, "YOU_DIDNT_OVERRIDE_IN_DERIVED[" + this.PanelName + "]: protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g)");
			this.GutterGridLinesRightBottomDrawForeground(g);
		}


		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD #if NON_DOUBLE_BUFFERED
//		protected override void OnPaintBackground(PaintEventArgs e) {
//		    base.OnPaintBackground(e);
		//#else
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs e) {
			//#endif
			//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
			e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			this.yForNextLabelUpperLeft = 5;
			//if (this.ChartControl.BarsNotEmpty) {}
			if (this.ImPaintingBackgroundNow) return;
			if (this.ChartControl == null) {
				//e.Graphics.Clear(SystemColors.Control);
				e.Graphics.Clear(base.BackColor);
				this.DrawError(e.Graphics, "OnPaintBackgroundDoubleBuffered got this.ChartControl=null");
				return;
			}
			if (this.ChartControl.BarsEmpty) {
				string msig = this.Name + ".OnPaintBackgroundDoubleBuffered() ";
				string msg = "CHART_CONTROL_BARS_NULL_OR_EMPTY: this.ChartControl.BarsEmpty ";
				//if (this.ChartControl.Bars != null) msg = "BUG: bars=[" + this.ChartControl.Bars + "]";
				//e.Graphics.Clear(SystemColors.Control);
				e.Graphics.Clear(base.BackColor);
				this.DrawError(e.Graphics, msig + msg);
				return;
			}
			this.ChartControl.SyncHorizontalScrollToBarsCount();
			this.ensureFontMetricsAreCalculated(e.Graphics);
			try {
				this.ImPaintingBackgroundNow = true;
				e.Graphics.Clear(this.ChartControl.ChartSettings.ChartColorBackground);
				// TODO: we get here 4 times per Panel: DockContentHandler.SetVisible, set_FlagClipWindow, WndProc * 2
				
				this.VisibleBarRight_cached = this.ChartControl.VisibleBarRight;
				this.VisibleBarLeft_cached = this.ChartControl.VisibleBarLeft;
				this.VisibleBarsCount_cached = VisibleBarRight_cached - VisibleBarLeft_cached;
				if (this.VisibleBarsCount_cached == 0) return;
				
				this.VisibleMin_cached = this.VisibleMin;
				this.VisibleMax_cached = this.VisibleMax;
				this.VisibleRange_cached = this.VisibleMax_cached - this.VisibleMin_cached;

				this.VisibleMinMinusTopSqueezer_cached = this.VisibleMin_cached - this.PaddingVerticalSqueeze;
				this.VisibleMaxPlusBottomSqueezer_cached = this.VisibleMax_cached + this.PaddingVerticalSqueeze;
				this.VisibleRangeWithTwoSqueezers_cached = this.VisibleMaxPlusBottomSqueezer_cached - VisibleMinMinusTopSqueezer_cached;


				
				this.BarWidthIncludingPadding_cached = this.ChartControl.ChartSettings.BarWidthIncludingPadding;
				this.BarWidthMinusRightPadding_cached = this.ChartControl.ChartSettings.BarWidthMinusRightPadding;
				this.BarShadowXoffset_cached = this.ChartControl.ChartSettings.BarShadowXoffset;
				this.PanelHeightMinusGutterBottomHeight_cached = this.PanelHeightMinusGutterBottomHeight;
				
				this.PaintBackgroundWholeSurfaceBarsNotEmpty(e.Graphics);
			} catch (Exception ex) {
				string msg = "OnPaintBackgroundDoubleBuffered(): caught[" + ex.Message + "]";
				Assembler.PopupException(msg, ex);
				this.DrawError(e.Graphics, msg);
				Debugger.Break();
			} finally {
				this.ImPaintingBackgroundNow = false;
			}
		}

		protected virtual void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			//this.PaintError(g, "YOU_DIDNT_OVERRIDE_IN_DERIVED[" + this.PanelName + "]: protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g)");
			this.GutterRightBottomDrawBackground(g);
		}

		// virtual will allow indicator panes to have their own backgrounds different to the price&volume backgrounds
		protected virtual void GutterRightBottomDrawBackground(Graphics g) {
			if (this.GutterRightDraw) {
				this.ChartControl.GutterRightWidth_cached = this.calculateGutterWidthNecessaryToFitPriceVolumeLabels(g);
				Rectangle gutterRightRect = default(Rectangle);
				gutterRightRect.X = this.PanelWidthMinusRightPriceGutter;
				gutterRightRect.Width = this.ChartControl.GutterRightWidth_cached;
				gutterRightRect.Y = 0;
				gutterRightRect.Height = this.PanelHeightMinusGutterBottomHeight;
				g.FillRectangle(this.ChartControl.ChartSettings.BrushGutterRightBackground, gutterRightRect);
			}
			if (this.GutterBottomDraw) {
				Rectangle gutterBottomRect = default(Rectangle);
				gutterBottomRect.X = 0;
				gutterBottomRect.Width = base.Width;
				gutterBottomRect.Y = this.PanelHeightMinusGutterBottomHeight;
				gutterBottomRect.Height = this.GutterBottomHeight_cached;
				g.FillRectangle(this.ChartControl.ChartSettings.BrushGutterBottomBackground, gutterBottomRect);
			}
		}
		// virtual will allow indicator panes to have their own backgrounds different to the price&volume backgrounds
		protected virtual void GutterGridLinesRightBottomDrawForeground(Graphics g) {
			if (this.ChartControl.BarsEmpty) return;
			if (this.VisibleMax_cached == 0) return;	//it's a cached version for once-per-render calculation
			
			bool mouseTrack = this.ChartControl.ChartSettings.MousePositionTrackOnGutters;
			if (this.GutterRightDraw) {
				if (this.GutterRightFontHeight_cached <= 0) {
					string msg = "this.ChartControl.GutterRightFontHeight[" + this.GutterRightFontHeight_cached + "] <= 0";
					Assembler.PopupException(msg + "GutterGridLinesRightBottomDrawForeground()");
					return;
				}
				if (this.PanelHeightMinusGutterBottomHeight_cached <= 0) {
					string msg = "this.ChartControl.PanelHeightMinusGutterBottomHeight_cached[" + this.PanelHeightMinusGutterBottomHeight_cached + "] <= 0";
					Assembler.PopupException(msg + "GutterGridLinesRightBottomDrawForeground()");
					return;
				}
				
				int minDistanceInFontHeights = this.ThisPanelIsPricePanel ? 3 : 2;
				int minDistancePixels = minDistanceInFontHeights * this.GutterRightFontHeight_cached;
				int panelHeightPlusSqueezers = this.PanelHeightMinusGutterBottomHeight_cached + this.PaddingVerticalSqueeze * 2;
				double howManyLinesWillFit = panelHeightPlusSqueezers / (double)minDistancePixels;
				double gridStep = 0;
				try {
					gridStep = this.calculateOptimalVeritcalGridStep(this.VisibleRangeWithTwoSqueezers_cached, howManyLinesWillFit);
				} catch (Exception e) {
					Debugger.Break();
				}
				double extraOverFound = this.VisibleMinMinusTopSqueezer_cached % gridStep;	// 6447 % 50 = 47
				double gridStart = this.VisibleMinMinusTopSqueezer_cached - extraOverFound + gridStep;
				int stripesCanFit = (int)(this.VisibleRangeWithTwoSqueezers_cached / gridStep);
				double gridEnd = gridStart + gridStep * stripesCanFit;
				if (gridEnd >= this.VisibleMaxPlusBottomSqueezer_cached) gridEnd -= gridStep;
				
				for (double gridPrice = gridStart; gridPrice <= gridEnd; gridPrice += gridStep) {
					int gridY = this.ValueToYinverted(gridPrice);
					g.DrawLine(this.ChartControl.ChartSettings.PenGridlinesHorizontal, 0, gridY, this.PanelWidthMinusRightPriceGutter-1, gridY);
					int labelYadjustedUp = (int)gridY - this.GutterRightFontHeightHalf_cached;
					labelYadjustedUp = this.AdjustToPanelHeight(labelYadjustedUp);
					string priceFormatted = this.ChartControl.ValueFormattedToSymbolInfoDecimalsOr5(gridPrice, this.ThisPanelIsPricePanel);
					int labelWidth = (int)g.MeasureString(priceFormatted, this.ChartControl.ChartSettings.GutterRightFont).Width;
					int labelXalignedRight = base.Width - this.ChartControl.ChartSettings.GutterRightPadding - labelWidth;
					g.DrawString(priceFormatted, this.ChartControl.ChartSettings.GutterRightFont,
					             this.ChartControl.ChartSettings.BrushGutterRightForeground, labelXalignedRight, labelYadjustedUp);
				}
				
				if (mouseTrack && this.moveHorizontalYprev > -1) {
					int mouseY = this.moveHorizontalYprev;
					g.DrawLine(this.ChartControl.ChartSettings.PenMousePositionTrackOnGutters, 0, mouseY, this.PanelWidthMinusRightPriceGutter+5, mouseY);
					
					double mousePrice = this.YinvertedToValue(mouseY);
					int labelYadjustedUp = (int)mouseY - this.GutterRightFontHeightHalf_cached;
					labelYadjustedUp = this.AdjustToPanelHeight(labelYadjustedUp);
					string priceFormatted = this.ChartControl.ValueFormattedToSymbolInfoDecimalsOr5(mousePrice, this.ThisPanelIsPricePanel);
					int labelWidth = (int)g.MeasureString(priceFormatted, this.ChartControl.ChartSettings.GutterRightFont).Width;
					int labelHeight = (int)g.MeasureString(priceFormatted, this.ChartControl.ChartSettings.GutterRightFont).Height;
					int labelXalignedRight = base.Width - this.ChartControl.ChartSettings.GutterRightPadding - labelWidth;
					
					Rectangle plate = new Rectangle(labelXalignedRight, labelYadjustedUp - 2, labelWidth, labelHeight + 2);
					g.FillRectangle(this.ChartControl.ChartSettings.PenMousePositionTrackOnGutters.Brush, plate);
					
					g.DrawString(priceFormatted, this.ChartControl.ChartSettings.GutterRightFont,
					             this.ChartControl.ChartSettings.BrushGutterRightForeground, labelXalignedRight, labelYadjustedUp);
				}
			}
			
			// before I draw vertical lines for date/time, I paint backgrounds for the bars if set from Script;
			this.renderBarBackgrounds(g);
			// now I draw vertical lines for date/time
			if (this.GutterBottomDraw) {
				if (this.GutterRightFontHeight_cached <= 0) return;	// not initialized yet
				//this.ChartControl.ChartSettings.PenGridlinesVerticalNewDate.Width = 2f;
				int leftPadding = this.ChartControl.ChartSettings.GutterBottomPadding;
				int y = this.PanelHeightMinusGutterBottomHeight_cached + leftPadding;
				List<Rectangle> barDateLabelsAlreadyDrawn = new List<Rectangle>();
				
				// I draw the beginning of the day no matter what
				bool isIntraday_cached = this.ChartControl.Bars.IsIntraday;
				if (isIntraday_cached) {
					//int dayOpenersDrawn = 0;
					
					//if (dayOpenersDrawn == 0) {	//draw in left corner the date anyway (too much eyeballing to find the date I need)
					Bar barLeftmost = this.ChartControl.Bars[this.VisibleBarLeftExisting];
					string barLeftmostFormatted = barLeftmost.DateTimeOpen.ToString(this.ChartControl.ChartSettings.GutterBottomDateFormatDayOpener);
					int barLeftmostX = this.BarToX(this.VisibleBarLeftExisting);
					int barLeftmostWidth = (int)g.MeasureString(barLeftmostFormatted, this.ChartControl.ChartSettings.GutterBottomFont).Width;
					barLeftmostX = this.adjustToBoundariesHorizontalGutter(barLeftmostX, barLeftmostWidth);
					//g.DrawLine(this.ChartControl.ChartSettings.PenGridlinesVerticalNewDate, barLeftmostX, 0, barLeftmostX, this.PanelHeightMinusGutterBottomHeight_cached);
					g.DrawString(barLeftmostFormatted, this.ChartControl.ChartSettings.GutterBottomFont, this.ChartControl.ChartSettings.BrushGutterBottomNewDateForeground, barLeftmostX, y);
					Rectangle rectLeftmost = new Rectangle(leftPadding, y, barLeftmostWidth, this.GutterRightFontHeight_cached);
					barDateLabelsAlreadyDrawn.Add(rectLeftmost);
					//}
					
					Bar barOpenerPrevDay = this.ChartControl.Bars[this.VisibleBarRightExisting];
					int barPrevX = this.PanelWidthMinusRightPriceGutter - this.BarWidthIncludingPadding_cached;
					for (int i = this.VisibleBarRightExisting - 1; i >= this.VisibleBarLeftExisting; i--, barPrevX -= this.BarWidthIncludingPadding_cached) {
						Bar bar = this.ChartControl.Bars[i];
						if (bar.DateTimeOpen.Day == barOpenerPrevDay.DateTimeOpen.Day) continue;
						g.DrawLine(this.ChartControl.ChartSettings.PenGridlinesVerticalNewDate, barPrevX, 0, barPrevX, this.PanelHeightMinusGutterBottomHeight_cached - 1);
						
						string barDayOpener = barOpenerPrevDay.DateTimeOpen.ToString(this.ChartControl.ChartSettings.GutterBottomDateFormatDayOpener);
						int barDayOpenerWidth = (int)g.MeasureString(barDayOpener, this.ChartControl.ChartSettings.GutterBottomFont).Width;
						int barMiddleOrLeftX = barPrevX;	// - barDayOpenerWidth / 2;
						barMiddleOrLeftX = this.adjustToBoundariesHorizontalGutter(barMiddleOrLeftX, barDayOpenerWidth);
						Rectangle rect = new Rectangle(barMiddleOrLeftX, y, barDayOpenerWidth, this.GutterRightFontHeight_cached);
						bool shoulddrawBarOpenerDate = true;
						foreach (Rectangle alreadyDrawn in barDateLabelsAlreadyDrawn) {
							if (rect.IntersectsWith(alreadyDrawn) == false) continue;
							shoulddrawBarOpenerDate = false;
							break;
						}
						if (shoulddrawBarOpenerDate) {
							g.DrawString(barDayOpener, this.ChartControl.ChartSettings.GutterBottomFont, this.ChartControl.ChartSettings.BrushGutterBottomNewDateForeground, rect.Left, rect.Top);
							barDateLabelsAlreadyDrawn.Add(rect);
							//dayOpenersDrawn++;
						}
						barOpenerPrevDay = bar;
					}
				}
				
				//this.ChartControl.ChartSettings.PenGridlinesVertical.Width = 1f;
				int barMiddleX = this.PanelWidthMinusRightPriceGutter - this.BarWidthIncludingPadding_cached + this.BarShadowXoffset_cached;
				for (int i = this.VisibleBarRightExisting; i >= this.VisibleBarLeftExisting; i--, barMiddleX -= this.BarWidthIncludingPadding_cached) {
					Bar bar = this.ChartControl.Bars[i];
					if (isIntraday_cached && bar.DateTimeOpen.Minute > 0) continue;
					string dateFormatted = bar.DateTimeOpen.ToString(this.formatForBars);
					int dateFormattedWidth = (int)g.MeasureString(dateFormatted, this.ChartControl.ChartSettings.GutterBottomFont).Width;
					int xLabel = barMiddleX - dateFormattedWidth / 2;
					
					if (xLabel < 0) xLabel = 0;
					Rectangle proposal = new Rectangle(xLabel, y, dateFormattedWidth, this.GutterRightFontHeight_cached);
					bool tooCrowded = false;
					foreach (Rectangle alreadyDrawn in barDateLabelsAlreadyDrawn) {
						if (proposal.IntersectsWith(alreadyDrawn) == false) continue;
						tooCrowded = true;
						break;
					}
					if (tooCrowded) continue;
					g.DrawLine(this.ChartControl.ChartSettings.PenGridlinesVertical, barMiddleX, 0, barMiddleX, this.PanelHeightMinusGutterBottomHeight_cached - 1);
					g.DrawString(dateFormatted, this.ChartControl.ChartSettings.GutterBottomFont, this.ChartControl.ChartSettings.BrushGutterBottomForeground, xLabel, y);
					barDateLabelsAlreadyDrawn.Add(proposal);
				}

				if (mouseTrack && this.moveHorizontalXprev > -1) {
					int mouseX = this.moveHorizontalXprev;
					//bool adjustMouseTrackToBarShadow = true;
					//if (adjustMouseTrackToBarShadow) {
						int mouseBarIndex = this.XToBar(mouseX);
						int mouseBarX = this.BarToX(mouseBarIndex);
						int mouseBarMiddleX = mouseBarX + this.BarShadowOffset;
					//}
					g.DrawLine(this.ChartControl.ChartSettings.PenMousePositionTrackOnGutters, mouseBarMiddleX, 0, mouseBarMiddleX, this.PanelHeightMinusGutterBottomHeight_cached+5);

					Bar mouseBar = this.ChartControl.Bars[mouseBarIndex];
					if (null != mouseBar) {
						string dateFormatted = mouseBar.DateTimeOpen.ToString(this.formatForBars);
						int dateFormattedWidth = (int)g.MeasureString(dateFormatted, this.ChartControl.ChartSettings.GutterBottomFont).Width;
						int xLabel = mouseBarMiddleX - dateFormattedWidth / 2;
						
						if (xLabel < 0) xLabel = 0;
						Rectangle plate = new Rectangle(xLabel - 2, y, dateFormattedWidth + 2, this.GutterRightFontHeight_cached);
						g.FillRectangle(this.ChartControl.ChartSettings.PenMousePositionTrackOnGutters.Brush, plate);
						g.DrawString(dateFormatted, this.ChartControl.ChartSettings.GutterBottomFont, this.ChartControl.ChartSettings.BrushGutterBottomForeground, xLabel, y);
					}
				}
			}
		}
		int dateMeasuredWidthFormattedByBarScale(Graphics g, DateTime value) {
			string valueFormatted = value.ToString(this.formatForBars);
			int ret = (int)g.MeasureString(valueFormatted, this.ChartControl.ChartSettings.GutterBottomFont).Width;
			return ret;
		}

		// http://stackoverflow.com/questions/361681/algorithm-for-nice-grid-line-intervals-on-a-graph
		double calculateOptimalVeritcalGridStep(double range, double targetSteps) {
			// calculate an initial guess at step size
			double tempStep = range / (double)targetSteps;

			// get the magnitude of the step size
			double mag = (float)Math.Floor(Math.Log10(tempStep));
			double magPow = (float)Math.Pow(10, mag);

			// calculate most significant digit of the new step size
			if (magPow == -0.5) {
				string msg = "DONT_INVOKE_ME_FOR_PANEL_HEIGHT_ZERO";
				Debugger.Break();
				return 0;
			}
			double magMsd = (int)(tempStep / magPow + 0.5);

			// promote the MSD to either 1, 2, or 5
			if (magMsd > 5.0) magMsd = 10.0f;
			else if (magMsd > 2.0) magMsd = 5.0f;
			else if (magMsd > 1.0) magMsd = 2.0f;

			return magMsd * magPow;
		}
		int calculateGutterWidthNecessaryToFitPriceVolumeLabels(Graphics g) {
			string visiblePriceMaxFormatted = this.ChartControl.ValueFormattedToSymbolInfoDecimalsOr5(this.ChartControl.VisiblePriceMax);
			int maxPrice = (int)g.MeasureString(visiblePriceMaxFormatted, this.ChartControl.ChartSettings.GutterRightFont).Width;
			
			string visibleVolumeMaxFormatted = this.ChartControl.ValueFormattedToSymbolInfoDecimalsOr5(this.ChartControl.VisibleVolumeMax, false);
			int maxVolume = (int)g.MeasureString(visibleVolumeMaxFormatted, this.ChartControl.ChartSettings.GutterRightFont).Width;
			
			int ret = Math.Max(maxPrice, maxVolume);
			ret += this.ChartControl.ChartSettings.GutterRightPadding * 2;
			return ret;
		}
		protected void RenderBarHistogram(Graphics graphics, int barX, int barYVolumeInverted, bool fillDownCandleBody) {
			int histogramBarHeight = this.PanelHeightMinusGutterBottomHeight_cached - barYVolumeInverted;		// height is measured DOWN the screen from candleBodyInverted.Y, not UP
			if (histogramBarHeight < 0)  Debugger.Break();
			if (histogramBarHeight == 0) return;	//candleBodyInverted.Height = 1;

			Rectangle histogramBarInverted = default(Rectangle);
			histogramBarInverted.X = barX;
			histogramBarInverted.Width = this.BarWidthMinusRightPadding_cached;
			histogramBarInverted.Y = barYVolumeInverted;					// drawing down, since Y grows down the screen from left upper corner (0:0)
			histogramBarInverted.Height = histogramBarHeight;		// height is measured DOWN the screen from candleBodyInverted.Y, not UP

			var brushDown = (fillDownCandleBody)
				? this.ChartControl.ChartSettings.BrushVolumeBarDown
				: this.ChartControl.ChartSettings.BrushVolumeBarUp;
			//if (fillDownCandleBody) histogramBarInverted.Width--;	// SYNC_WITH_RenderBarCandle drawing using a pen produces 1px narrower rectangle that drawing using a brush???...
			graphics.FillRectangle(brushDown, histogramBarInverted);
		}
		protected void RenderBarCandle(Graphics graphics, int barX, int barYOpenInverted, int barYHighInverted, int barYLowInverted, int barYCloseInverted, bool fillDownCandleBody) {
			int candleBodyLower = barYOpenInverted;	// assuming it is a white candle (rising price)
			int candleBodyHigher = barYCloseInverted;
			if (barYOpenInverted > barYCloseInverted) {
				if (fillDownCandleBody == true) {		//FIXIT: should be false
					Debugger.Break();
				}
				candleBodyLower = barYCloseInverted;	// nope it's a black candle (falling price)
				candleBodyHigher = barYOpenInverted;
			}
			
			int candleBodyHeight = candleBodyHigher - candleBodyLower;		// height is measured DOWN the screen from candleBodyInverted.Y, not UP
			if (candleBodyHeight < 0) Debugger.Break();
			if (candleBodyHeight == 0) candleBodyHeight = 1;

			Rectangle candleBodyInverted = default(Rectangle);
			candleBodyInverted.X = barX;
			candleBodyInverted.Width = this.BarWidthMinusRightPadding_cached;
			candleBodyInverted.Y = candleBodyLower;					// drawing down, since Y grows down the screen from left upper corner (0:0)
			candleBodyInverted.Height = candleBodyHeight;

			int shadowX = barX + this.BarShadowXoffset_cached;
			if (fillDownCandleBody) {
				var brushDown = this.ChartControl.ChartSettings.BrushPriceBarDown;
				var penDown = this.ChartControl.ChartSettings.PenPriceBarDown;
				graphics.FillRectangle(brushDown, candleBodyInverted);
				graphics.DrawLine(penDown, shadowX, barYHighInverted, shadowX, barYLowInverted);
			} else {
				var brushUp = this.ChartControl.ChartSettings.BrushPriceBarUp;
				var penUp = this.ChartControl.ChartSettings.PenPriceBarUp;
				candleBodyInverted.Width--;	// drawing using a pen produces 1px narrower rectangle that drawing using a brush???...
				graphics.DrawRectangle(penUp, candleBodyInverted);
				graphics.DrawLine(penUp, shadowX, barYHighInverted, shadowX, candleBodyInverted.Top);
				graphics.DrawLine(penUp, shadowX, barYLowInverted, shadowX, candleBodyInverted.Bottom);
			}
		}

		public int BarToXshadowBeyondGoInside(int barIndexCanBeBeyondVisibleSlidingWindow) {
			int ret = this.BarToXBeyondGoInside(barIndexCanBeBeyondVisibleSlidingWindow);
			ret += this.BarShadowXoffset_cached;
			return ret;
		}
		public int BarToXBeyondGoInside(int barIndexCanBeBeyondVisibleSlidingWindow) {
			int ret = this.BarToX(barIndexCanBeBeyondVisibleSlidingWindow);
			//barVisible < this.VisibleBarLeft_cached) return -1;
			if (ret == -1) {
				return 0;
			}
			//barVisible > this.VisibleBarRight_cached) return -2;
			if (ret == -2) {
				return this.ChartControl.ChartWidthMinusGutterRightPrice;
			}
			return ret;
		}
		public int BarToX(int barVisible) {
			if (barVisible <= this.VisibleBarLeft_cached) return -1;
			if (barVisible > this.VisibleBarRight_cached) return -2;
			int barRightX = this.PanelWidthMinusRightPriceGutter - this.BarWidthIncludingPadding_cached;
			int barsFromRight = this.VisibleBarRight_cached - barVisible;
			int barWidthdsFromRightMargin = barsFromRight * this.BarWidthIncludingPadding_cached;
			return barRightX - barWidthdsFromRightMargin; // shouldn't be negative
		}
		public int XToBar(int xMouseOver) {
			if (xMouseOver < 0) return -1;
			if (xMouseOver > this.PanelWidthMinusRightPriceGutter) return -2;
			int offsetFromRight = this.PanelWidthMinusRightPriceGutter - this.BarWidthIncludingPadding_cached;
			for (int i = this.VisibleBarRightExisting; i >= this.VisibleBarLeftExisting;
				     i--, offsetFromRight -= this.BarWidthIncludingPadding_cached) {
				if (xMouseOver > offsetFromRight) return i;
			}
			return -3; // negative means ERROR
		}
		public int ValueToYinverted(double volume) {			//200
			if (this.ChartControl.BarsEmpty) return 666;
			if (double.IsNaN(volume)) Debugger.Break();
			double min = this.VisibleMinMinusTopSqueezer_cached;		//100
			double max = this.VisibleMaxPlusBottomSqueezer_cached;		//250
			double rangeMinMax = this.VisibleRangeWithTwoSqueezers_cached;			//150
			if (rangeMinMax == 0) rangeMinMax = 1;
			double distanceFromMin = volume - min;	//200 - 100 = 100
			double priceAsPartOfRange = distanceFromMin / rangeMinMax;	//100 / 150	 = 0.6
			int ret = (int)Math.Round(this.PanelHeightMinusGutterBottomHeight_cached * priceAsPartOfRange);		// 600 * 0.6 = 360px UP
			ret = this.yInverted(ret);
			ret = this.AdjustToPanelHeight(ret);
			return ret;			// should be inverted to screen starting from upper left corner (600 - 360 = 240px DOWN)
		}
		public double YinvertedToValue(int yMouseInverted) {
			if (this.ChartControl.BarsEmpty) return 777;
			if (yMouseInverted <= 0) return this.VisibleMaxPlusBottomSqueezer_cached;
			if (yMouseInverted >= this.PanelHeightMinusGutterBottomHeight_cached) return this.VisibleMinMinusTopSqueezer_cached;
			int yStraight = yInverted(yMouseInverted);
			double yAsPartOfPlot = yStraight / (double) this.PanelHeightMinusGutterBottomHeight_cached;
			double PartOfVisibleRange = this.VisibleRangeWithTwoSqueezers_cached * yAsPartOfPlot;
			double ret = this.VisibleMinMinusTopSqueezer_cached + PartOfVisibleRange;
			return ret;
		}
		int yInverted(int y) {
			int yInverted = this.PanelHeightMinusGutterBottomHeight_cached - y;
			return yInverted;
		}
		public int AdjustToPanelHeight(int y) {
			if (y < 0) y = 0;
			if (y > this.PanelHeightMinusGutterBottomHeight_cached) y = this.PanelHeightMinusGutterBottomHeight_cached;
			return y;
		}
		int adjustToBoundariesHorizontalGutter(int x, int width) {
			if (x < this.ChartControl.ChartSettings.GutterBottomPadding) x = this.ChartControl.ChartSettings.GutterBottomPadding;
			if (x + width > base.Width) x = base.Width - width;
			return x;
		}
		
		
		
		
		//protected bool	mouseOver;
		protected bool	dragButtonPressed;
		
		protected bool	scrollingHorizontally;
		protected int	scrollHorizontalXstarted;
		protected int	scrollHorizontalXprev;
		
		protected bool	squeezingVertically;
		protected int	squeezeVerticalYstarted;
		protected int	squeezeVerticalYprev;

		protected bool	squeezingHorizontally;
		protected int	squeezeHorizontalXstarted;
		protected int	squeezeHorizontalXprev;
		
		protected int	moveHorizontalXprev;
		protected int	moveHorizontalYprev;


		protected override void OnMouseEnter(EventArgs e) {
			//this.mouseOver = true;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			//this.ChartControl.TooltipPrice.ClientRectangle.Contains(e.
			//if (this.ChartControl.TooltipPriceShown && ) {
			//if (this.ChartControl.TooltipPriceShownAndMouseOverIt == true) {
			//	return;
			//}
			//if (this.ChartControl.TooltipPositionShownAndMouseOverIt == true) {
			//	return;
			//}
			
			//this.mouseOver = false;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
			//this.ChartControl.TooltipPriceHide();
			//this.ChartControl.TooltipPositionHide();
			
			// MouseTrack uses this to remove MouseLines from Panel Price & MousePositionLabels from Gutters
			this.moveHorizontalXprev = -1;
			this.moveHorizontalYprev = -1;

			base.OnMouseLeave(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			this.dragButtonPressed = true;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			this.dragButtonPressed = false;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
			this.Cursor = Cursors.Default;
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				if (this.ChartControl.BarsEmpty) return;	// finally {} will invoke base.OnMouseMove()
				
				if (this.dragButtonPressed == true && this.scrollingHorizontally == false && this.squeezingHorizontally == false && this.squeezingVertically == false) {
					// coordinates inverted: (x=0;y=0) is upper left corner
					if (e.X > this.PanelWidthMinusRightPriceGutter) {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.squeezeVerticalYstarted = e.Y;
						this.squeezeVerticalYprev = e.Y;
						this.squeezingVertically = true;
					} else if (e.Y > this.PanelHeightMinusGutterBottomHeight_cached) {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.squeezeHorizontalXstarted = e.Y;
						this.squeezeHorizontalXprev = e.Y;
						this.squeezingHorizontally = true;
					} else {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.scrollHorizontalXstarted = e.X;
						this.scrollHorizontalXprev = e.X;
						this.scrollingHorizontally = true;
					}
					this.ChartControl.TooltipPriceHide();
					this.ChartControl.TooltipPositionHide();
				}

				if (this.scrollingHorizontally) {
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.scrollHorizontalXstarted - e.X) <= this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int XnotBeyond0width = e.X;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEYOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.X < 0) XnotBeyond0width = 0;
					//if (e.X > base.Width) XnotBeyond0width = base.Width;

					if (this.scrollHorizontalXprev == XnotBeyond0width) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool draggingToTheLeft = (this.scrollHorizontalXstarted > XnotBeyond0width) ? true : false;
					if (draggingToTheLeft) {
						if (this.scrollHorizontalXprev < XnotBeyond0width) {	// we reversed drag direction without releasing the mouse button
							this.scrollHorizontalXstarted = XnotBeyond0width;
							this.scrollHorizontalXprev = XnotBeyond0width;
							return;
						}
					} else {
						if (this.scrollHorizontalXprev > XnotBeyond0width) {	// we reversed drag direction without releasing the mouse button
							this.scrollHorizontalXstarted = XnotBeyond0width;
							this.scrollHorizontalXprev = XnotBeyond0width;
							return;
						}
					}
					this.scrollHorizontalXprev = XnotBeyond0width;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (draggingToTheLeft) {
						//if (this.VisibleMin_cached <= 0) return;		//this.HorizontalScroll IS NOT USED!!! use Parent's instead! ChartControl.HorizontalScroll
						//if (this.ChartControl.hScrollBar.ValueCurrent >= this.ChartControl.HorizontalScroll.Maximum) return;
						this.Cursor = Cursors.PanWest;
						this.ChartControl.DragNBarsRight();
					} else {
						//if (this.VisibleMax_cached >= this.ChartControl.Bars.Count) return;
						//if (this.ChartControl.hScrollBar.ValueCurrent <= this.ChartControl.HorizontalScroll.Minimum) return;
						this.Cursor = Cursors.PanEast;
						this.ChartControl.DragNBarsLeft();
					}
				}
				
				if (this.squeezingHorizontally) {
					if (this.ThisPanelIsPricePanel == false) return;
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.squeezeHorizontalXstarted - e.X) <= this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int XnotBeyond0height = e.X;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEXOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.X < 0) XnotBeyond0height = 0;
					//if (e.X > base.Height) XnotBeyond0height = base.Height;
					if (Math.Abs(this.squeezeHorizontalXprev - XnotBeyond0height) < this.ChartControl.ChartSettings.SqueezeHorizontalMouse1pxDistanceReceivedToOneStep) return;

					if (this.squeezeHorizontalXprev == XnotBeyond0height) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool squeezingToTheLeft = (this.squeezeHorizontalXstarted > XnotBeyond0height) ? true : false;
					if (squeezingToTheLeft) {
						if (this.squeezeHorizontalXprev < XnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeHorizontalXstarted = XnotBeyond0height;
							this.squeezeHorizontalXprev = XnotBeyond0height;
							return;
						}
					} else {
						if (this.squeezeHorizontalXprev > XnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeHorizontalXstarted = XnotBeyond0height;
							this.squeezeHorizontalXprev = XnotBeyond0height;
							return;
						}
					}
					this.squeezeHorizontalXprev = XnotBeyond0height;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (squeezingToTheLeft) {
						this.Cursor = Cursors.PanWest;
						this.ChartControl.BarWidthIncrementAtKeyPressRate();
					} else {
						this.Cursor = Cursors.PanEast;
						this.ChartControl.BarWidthDecrementAtKeyPressRate();
					}
				}
				
				if (this.squeezingVertically) {
					if (this.ThisPanelIsPricePanel == false) return;
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.squeezeVerticalYstarted - e.Y) <= this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int YnotBeyond0height = e.Y;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEYOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.Y < 0) YnotBeyond0height = 0;
					//if (e.Y > base.Height) YnotBeyond0height = base.Height;

					if (this.squeezeVerticalYprev == YnotBeyond0height) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool draggingUp = (this.squeezeVerticalYstarted > YnotBeyond0height) ? true : false;
					if (draggingUp) {
						if (this.squeezeVerticalYprev < YnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeVerticalYstarted = YnotBeyond0height;
							this.squeezeVerticalYprev = YnotBeyond0height;
							return;
						}
					} else {
						if (this.squeezeVerticalYprev > YnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeVerticalYstarted = YnotBeyond0height;
							this.squeezeVerticalYprev = YnotBeyond0height;
							return;
						}
					}
					this.squeezeVerticalYprev = YnotBeyond0height;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (draggingUp) {
						this.Cursor = Cursors.PanNorth;
						this.ChartControl.DragUpUnsqueeze();
					} else {
						this.Cursor = Cursors.PanSouth;
						this.ChartControl.DragDownSqueeze();
					}
				}
				
				if (this.ThisPanelIsPricePanel == false) return;
				if (this.dragButtonPressed == true) return;

				if (this.moveHorizontalXprev == e.X && this.moveHorizontalYprev == e.Y) {
					return;
				}
				this.moveHorizontalXprev = e.X;
				this.moveHorizontalYprev = e.Y;

				bool mouseTrack = this.ChartControl.ChartSettings.MousePositionTrackOnGutters;
				if (mouseTrack) {
					//base.Refresh();
					base.Invalidate();
				}

				this.TooltipPositionShown = this.handleTooltipsPositionAndPrice(e);
				if (this.TooltipPositionShown == false) this.handleTooltipPrice(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				//throw ex;
				Debugger.Break();
			} finally {
				base.OnMouseMove(e);
			}
		}
		public bool TooltipPositionShown;
		bool handleTooltipsPositionAndPrice(MouseEventArgs e) {
			bool tooltipPositionShown = false;
			if (this.ChartControl.ChartSettings.TooltipPositionShow == false) return tooltipPositionShown;

			int barIndexMouseIsOverNow = this.XToBar(e.X);
			if (barIndexMouseIsOverNow < this.VisibleBarLeft_cached) return tooltipPositionShown;
			if (barIndexMouseIsOverNow > this.VisibleBarRight_cached) return tooltipPositionShown;	//Debugger.Break();

			Dictionary<int, List<AlertArrow>> alertArrowsListByBar = this.ChartControl.ScriptExecutorObjects.AlertArrowsListByBar;
			if (alertArrowsListByBar.ContainsKey(barIndexMouseIsOverNow) == false) {
				this.ChartControl.TooltipPositionHide();
				return tooltipPositionShown;
			}

			List<AlertArrow> arrowsForBar = alertArrowsListByBar[barIndexMouseIsOverNow];
			AlertArrow arrowFoundForMouse = null;
			foreach (AlertArrow arrow in arrowsForBar) {
				int yInverted = e.Y;
				int arrowYinvertedHigher = arrow.Ytransient;
				int arrowYinvertedLower = arrowYinvertedHigher + arrow.Height;
				if (yInverted < arrowYinvertedHigher || yInverted > arrowYinvertedLower) continue;
				arrowFoundForMouse = arrow;
				break;
			}
			if (arrowFoundForMouse == null) {
				this.ChartControl.TooltipPositionHide();
				return tooltipPositionShown;
			}
			Position positionToPopup = arrowFoundForMouse.Position;
			bool placeAtLeft = arrowFoundForMouse.ArrowIsForPositionEntry;
			Bar bar = arrowFoundForMouse.ArrowIsForPositionEntry ? positionToPopup.EntryBar : positionToPopup.ExitBar;

			int barX = this.BarToX(barIndexMouseIsOverNow);
			Rectangle rectangleYarrowXbar = new Rectangle();
			rectangleYarrowXbar.X		= barX;
			rectangleYarrowXbar.Width	= this.BarWidthIncludingPadding_cached;		//arrowFoundForMouse.Width;
			rectangleYarrowXbar.Y		= arrowFoundForMouse.Ytransient;
			rectangleYarrowXbar.Height	= arrowFoundForMouse.Height;

			bool mouseIsOverArrowIconRectangle = rectangleYarrowXbar.Contains(e.X, e.Y);

			if (mouseIsOverArrowIconRectangle) {
				this.ChartControl.TooltipPositionAndPriceShow(arrowFoundForMouse, bar, rectangleYarrowXbar);
				tooltipPositionShown = true;
			} else {
				this.ChartControl.TooltipPositionHide();
				this.ChartControl.TooltipPriceHide();
			}

			return tooltipPositionShown;
		}
		void handleTooltipPrice(MouseEventArgs e) {
			if (this.ChartControl.ChartSettings.TooltipPriceShow == false) return;
			//if (this.ThisPanelIsPricePanel == false) return;
			//if (this.dragButtonPressed == true) return;
			//if (this.scrollingHorizontally == false || this.squeezingHorizontally == false || this.squeezingVertically == false) return;

			int barIndexMouseIsOverNow = this.XToBar(e.X);
			if (barIndexMouseIsOverNow < this.VisibleBarLeft_cached) return;
			if (barIndexMouseIsOverNow > this.VisibleBarRight_cached) return;	//Debugger.Break();

			Bar barMouseIsOverNow = this.ChartControl.Bars[barIndexMouseIsOverNow];
			if (double.IsNaN(barMouseIsOverNow.Low)) return;
			if (double.IsNaN(barMouseIsOverNow.High)) return;

			int yLow = this.ValueToYinverted(barMouseIsOverNow.Low);
			int yHigh = this.ValueToYinverted(barMouseIsOverNow.High);

			Rectangle rectangleBarWithShadows = new Rectangle();
			rectangleBarWithShadows.X = this.BarToX(barIndexMouseIsOverNow);
			rectangleBarWithShadows.Y = yHigh;
			rectangleBarWithShadows.Width = this.BarWidthIncludingPadding_cached;
			rectangleBarWithShadows.Height = yLow - yHigh;	// due to upperLeft=(0:0), inverted yLow > yHigh
			if (this.ChartControl.ChartSettings.TooltipPriceShowOnlyWhenMouseTouchesCandle) {
				//int verticalSensitivityIncreased = 4;	// doesn't cause flickering, keep TooltipSurfacePaddingFromBarLeftRightToAvoidMouseLeave > 2
				int verticalSensitivityIncreased = 0;	//MOVE_TO_SETTINGS_SO_ARROWS_WONT_OVERLAP
				bool mouseIsOverBarRectangle = e.Y >= rectangleBarWithShadows.Top - verticalSensitivityIncreased
											&& e.Y <= rectangleBarWithShadows.Bottom + verticalSensitivityIncreased;
				if (mouseIsOverBarRectangle) {
					this.ChartControl.TooltipPriceShowAlone(barMouseIsOverNow, rectangleBarWithShadows);
				} else {
					this.ChartControl.TooltipPriceHide();
				}
			} else {
				this.ChartControl.TooltipPriceShowAlone(barMouseIsOverNow, rectangleBarWithShadows);
			}
		}
		//public new void Invalidate() {
			// for unwanted user-initiated Paint()s, watch invoker in the stack; otherwize OS invoked Paint() and you have to deal with it
		//	base.Invalidate();
		//}

	 	void ensureFontMetricsAreCalculated(Graphics g) {
	 		if (this.GutterRightFontHeight_cached == -1) {
				this.GutterRightFontHeight_cached = (int)g.MeasureString("ABC123`'jg]", this.ChartControl.ChartSettings.GutterRightFont).Height;
	 		//}
	 		//if (this.GutterRightFontHeightHalf_cached == -1) {
	 			this.GutterRightFontHeightHalf_cached = (int)(this.GutterRightFontHeight_cached / 2F);
	 		//}
	 		//if (this.GutterBottomFontHeight_cached == -1) {
				this.GutterBottomFontHeight_cached = (int)g.MeasureString("ABC123`'jg]", this.ChartControl.ChartSettings.GutterBottomFont).Height;
	 		//}
	 		//if (this.GutterBottomHeight_cached == -1) {
				this.GutterBottomHeight_cached = this.GutterBottomFontHeight_cached + this.ChartControl.ChartSettings.GutterBottomPadding* 2;
	 		}
		}
		void renderBarBackgrounds(Graphics g) {
			int barX = this.ChartControl.ChartWidthMinusGutterRightPrice;
			int halfPadding = this.ChartControl.ChartSettings.BarPaddingRight / 2;
			//halfPadding += 1;		// fixes 1-2px spaces between bars background
			barX -= halfPadding;	// emulate bar having paddings from left and right
			for (int barIndex = VisibleBarRight_cached; barIndex > VisibleBarLeft_cached; barIndex--) {
				if (barIndex > this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("MOVE_THIS_CHECK_UPSTACK renderBarsPrice(): " + msg);
					continue;
				}
				
				ScriptExecutorObjects seo = this.ChartControl.ScriptExecutorObjects;
				if (seo.BarBackgroundsByBar.ContainsKey(barIndex) == false) continue;
				
				barX -= this.BarWidthIncludingPadding_cached;
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

	}
}