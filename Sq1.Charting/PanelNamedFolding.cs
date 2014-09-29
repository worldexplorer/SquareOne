using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public partial class PanelNamedFolding :
		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_CLICK_THROUGH #if NON_DOUBLE_BUFFERED
		//Panel
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
		
		public bool ImPaintingForegroundNow = false;
		public bool ImPaintingBackgroundNow = false;

		[Browsable(false)] public string PanelNameAndSymbol { get { return this.PanelName + " " + this.BarsIdent; } }
		[Browsable(false)] public ChartControl ChartControl;	//{ get { return base.Parent as ChartControl; } }
		int chartLabelsUpperLeftYincremental;
		
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
				// if (base.DesignMode) this.ChartControl will be NULL
				if (this.GutterRightDraw) ret -= (this.ChartControl != null) ? this.ChartControl.GutterRightWidth_cached : 60;
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
		public void DrawError(Graphics g, string msg) {
			this.DrawLabelOnNextLine(g, msg, null, Color.Red, Color.Empty);
		}
		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD #if NON_DOUBLE_BUFFERED
		//protected override void OnResize(EventArgs e) {	//PanelDoubleBuffered does this already to DisposeAndNullify managed Graphics
		//	if (base.DesignMode) return;
		//	base.OnResize(e);	// empty inside but who knows what useful job it does?
		//	base.Invalidate();	// SplitterMoved => repaint; Panel and UserControl don't have that (why?)
		//}
		//protected override void OnPaint(PaintEventArgs e) {
		//	base.OnPaint(e);
		//#else
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
		//#endif
		    if (this.DesignMode) return;
			//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
			e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			
			if (this.ChartControl == null) {
				string msg = "PanelNamedFolding[" + this.PanelName + "].ChartControl=null; invoke PanelNamedFolding.Initialize() from derived.ctor()";
				Debugger.Break();
				this.DrawError(e.Graphics, msg);
				return;
			}
			if (this.ChartControl.BarsEmpty) {
				string msig = this.Name + ".OnPaintDoubleBuffered() ";
				string msg = "CHART_CONTROL_BARS_NULL_OR_EMPTY: this.ChartControl.BarsEmpty ";
				//if (this.ChartControl.Bars != null) msg = "BUG: bars=[" + this.ChartControl.Bars + "]";
				//Debugger.Break();
				this.DrawError(e.Graphics, msg + msig);
				return;
			}
			this.ChartControl.SyncHorizontalScrollToBarsCount();
			
			bool skipPaintingBeforeBacktestCompletes = false;		// AVOIDING_CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR in this.RenderIndicators(); I don't need Executor.Backtester.BacktestIsRunning here 
			Dictionary<string, Indicator> indicators = this.ChartControl.ScriptExecutorObjects.Indicators;	// if there's no indicators => I won't go to foreach () and won't skipPaintingBeforeBacktestCompletes 
			foreach (Indicator indicator in indicators.Values) {
				if (indicator.OwnValuesCalculated == null) {
					#if DEBUG
					Debugger.Break();
					#endif
					continue;
				}
				if (indicator.OwnValuesCalculated.Count > 0) continue;
				skipPaintingBeforeBacktestCompletes = true;
				break;
			}
			if (skipPaintingBeforeBacktestCompletes) return;
			
			if (this.ImPaintingForegroundNow) return;
			try {
				this.ImPaintingForegroundNow = true;
				this.PaintWholeSurfaceBarsNotEmpty(e.Graphics);	// GOOD: we get here once per panel
				this.RenderIndicators(e.Graphics);
				// draw Panel Title on top of anything that the panel draws
				if (this.Collapsible) {
					if (this.PanelName != null) {
						using (var brush = new SolidBrush(this.ForeColor)) {
							// if (base.DesignMode) this.ChartControl will be NULL
							Font font = (this.ChartControl != null) ? this.ChartControl.ChartSettings.PanelNameAndSymbolFont : this.Font;
							e.Graphics.DrawString(this.PanelNameAndSymbol, font, brush, new Point(2, 2));
						}
					} else {
						this.DrawError(e.Graphics, "SET_TO_EMPTY_STRING_TO_HIDE: this.PanelName=null");
					}
				}
			} catch (Exception ex) {
				string msg = "OnPaintDoubleBuffered(): caught[" + ex.Message + "]";
				Debugger.Break();
				Assembler.PopupException(msg, ex);
				this.DrawError(e.Graphics, msg);
			} finally {
				this.ImPaintingForegroundNow = false;
			}
		}
		protected virtual void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			//this.PaintError(g, "YOU_DIDNT_OVERRIDE_IN_DERIVED[" + this.PanelName + "]: protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g)");
			this.GutterGridLinesRightBottomDrawForeground(g);
		}


		//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD #if NON_DOUBLE_BUFFERED
		//protected override void OnPaintBackground(PaintEventArgs e) {
		//	base.OnPaintBackground(e);
		//#else
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs e) {
		//#endif
			//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
			//if (this.DesignMode) return;
			e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
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
			this.chartLabelsUpperLeftYincremental = this.ChartControl.ChartSettings.ChartLabelsUpperLeftYstartTopmost;
			//if (this.ChartControl.BarsNotEmpty) {}
			this.ChartControl.SyncHorizontalScrollToBarsCount();
			this.ensureFontMetricsAreCalculated(e.Graphics);
			try {
				this.ImPaintingBackgroundNow = true;
				e.Graphics.Clear(this.ChartControl.ChartSettings.ChartColorBackground);
				// TODO: we get here 4 times per Panel: DockContentHandler.SetVisible, set_FlagClipWindow, WndProc * 2
				
				this.VisibleBarRight_cached = this.ChartControl.VisibleBarRight;
				if (this.VisibleBarRight_cached < 0) {
					Debugger.Break();
				}
				this.VisibleBarLeft_cached = this.ChartControl.VisibleBarLeft;
				this.VisibleBarsCount_cached = VisibleBarRight_cached - VisibleBarLeft_cached;
				if (this.VisibleBarsCount_cached == 0) return;
				
				this.VisibleMin_cached = this.VisibleMin;
				this.VisibleMax_cached = this.VisibleMax;
				this.VisibleRange_cached = this.VisibleMax_cached - this.VisibleMin_cached;

				double pixelsSqueezedToPriceDistance = 0;
				if (this.PaddingVerticalSqueeze > 0 && base.Height > 0) {
					double priceDistanceInOnePixel = this.VisibleRange_cached / base.Height;
					pixelsSqueezedToPriceDistance = this.PaddingVerticalSqueeze * priceDistanceInOnePixel;
				}
				
				this.VisibleMinMinusTopSqueezer_cached = this.VisibleMin_cached - pixelsSqueezedToPriceDistance;
				this.VisibleMaxPlusBottomSqueezer_cached = this.VisibleMax_cached + pixelsSqueezedToPriceDistance;
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
		public int ValueToYinverted(double priceOrVolume) {			//200
			if (this.ChartControl.BarsEmpty) return 666;
			if (double.IsNaN(priceOrVolume)) Debugger.Break();
			double min = this.VisibleMinMinusTopSqueezer_cached;		//100
			double max = this.VisibleMaxPlusBottomSqueezer_cached;		//250
			double rangeMinMax = this.VisibleRangeWithTwoSqueezers_cached;			//150
			
			#region quickCheckFor *_cached variables; veeeeery slow in Debug; that's why *_cached exist
			#if DEBUG_HEAVY
			double min2 = this.VisibleMin;
			double max2 = this.VisibleMax;

			double pixelsSqueezedToPriceDistance = 0;
			if (this.PaddingVerticalSqueeze > 0 && base.Height > 0) {
				double priceDistanceInOnePixel = (max2 - min2) / base.Height;
				pixelsSqueezedToPriceDistance = this.PaddingVerticalSqueeze * priceDistanceInOnePixel;
			}
			
			min2 -= pixelsSqueezedToPriceDistance;
			max2 += pixelsSqueezedToPriceDistance;
			double range2 = max2 - min2;
		
			if (min != min2) {
				//Debugger.Break();
				min = min2;
			}
			if (max != max2) {
				//Debugger.Break();
				max = max2;
			}
			if (rangeMinMax != range2) {
				//Debugger.Break();
				rangeMinMax = range2;
			}
			#endif
			#endregion
			
			if (rangeMinMax == 0) rangeMinMax = 1;
			double distanceFromMin = priceOrVolume - min;	//200 - 100 = 100
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

	}
}