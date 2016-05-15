using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.DoubleBuffered;

using Sq1.Charting.MultiSplit;

namespace Sq1.Charting {
	public partial class PanelBase :
#if NON_DOUBLE_BUFFERED
//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_CLICK_THROUGH
		Panel
#else
			PanelDoubleBuffered //UserControlDoubleBuffered
#endif
			, HostPanelForIndicator {

		//[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
							public	string			PanelName			{ get; set; }
		[Browsable(false)]	public	string			BarsIdent			{ get; set; }		//if a public field isn't a property, Designer will crash
							public	bool			GutterRightDraw		{ get; set; }
							public	bool			GutterBottomDraw	{ get; set; }

		[Browsable(false)]	public	string			PanelNameAndSymbol	{ get { return this.PanelName + " " + this.BarsIdent; } }
		[Browsable(false)]	public	ChartControl	ChartControl;		//{ get { return base.Parent as ChartControl; } }
							protected int			ChartLabelsUpperLeftYincremental;
		
		// price, volume or indicator-calculated; not abstract for Designer to throw less exceptions
		[Browsable(false)]	public	virtual	double	VisibleMinDoubleMaxValueUnsafe { get {
			#if DEBUG
			Debugger.Break();
			#endif
			throw new NotImplementedException(); } }
		[Browsable(false)]	public	virtual	double	VisibleMaxDoubleMinValueUnsafe { get {
			#if DEBUG
			Debugger.Break();
			#endif
			throw new NotImplementedException(); } }
		// USE_CACHED_VARIABLE_INSTEAD [Browsable(false)] public virtual double VisibleRange { get { return this.VisibleMax - this.VisibleMin; } }
		[Browsable(false)]	public	virtual	double	FirstNonNanBetweenLeftRight { get {
				double ret = double.NaN;
				if (this.VisibleBarRight_cached > this.ValueIndexLastAvailable_minusOneUnsafe) return ret;
				for (int i=this.VisibleBarLeft_cached; i<this.VisibleBarRight_cached; i++) {
					ret = this.ValueGetNaNunsafe(i);
					if (double.IsNaN(ret)) continue;
					break;
				}
				return ret;
			} }

		// PanelPrice		must return bars[barIndexMouseOvered].Close
		// PanelVolume		must return bars[barIndexMouseOvered].Volume
		// PanelIndicator	must return OwnValues[barIndexMouseOvered]
		[Browsable(false)]	public	virtual	double	PanelValueForBarMouseOveredNaNunsafe { get {
				double ret = double.NaN;
				if (this.ChartControl.BarIndexMouseIsOverNow == -1) return ret;
				ret = this.ValueGetNaNunsafe(this.ChartControl.BarIndexMouseIsOverNow);
				return ret;
			} }
		[Browsable(false)]	public	virtual	bool	PanelHasValuesForVisibleBarWindow { get {
				string msig = this.ToString();
				bool ret = false;
				if (this.VisibleBarRight_cached == -1) {
					string msg = "VISIBLE_RIGHT_MUST_BE_POSITIVE CHECK_UPSTACK_INVALID_STATE";
					Assembler.PopupException(msg + msig);
					return ret;
				}
				if (this.ValueIndexLastAvailable_minusOneUnsafe == -1) {
					string msg = "BARS_COUNT_ZERO__I_HAVE_NO_VALUES_TO_DRAW";
					if (this as PanelIndicator != null) {
						msg = "INDICATOR_WASNT_ATTACHED_TO_PANEL_YET BACKTEST_ON_APPRESTART=FALSE_AND_WE_JUST_DESERIALIZED" + msg;
						//NOT_AN_ERROR Assembler.PopupException(msg + msig, null, false);
					} else {
						Assembler.PopupException(msg + msig, null, false);
					}
					return ret;
				}
				ret = this.VisibleBarRight_cached <= this.ValueIndexLastAvailable_minusOneUnsafe;
				return ret;
			} }
		[Browsable(false)]	public	virtual	double	ValueGetNaNunsafe(int barIndex) {
			#if DEBUG
			Debugger.Break();
			#endif
			throw new NotImplementedException();
		}


		// REASON_TO_EXIST: for SBER, constant ATR shows truncated (imprecise) mouseOver value on gutter
		[Browsable(false)]	public	virtual	int		PriceDecimals { get {
				return 	  this.ChartControl.Bars.SymbolInfo != null
					? this.ChartControl.Bars.SymbolInfo.PriceDecimals
					: 5; } }
		[Browsable(false)]	public			string	PriceFormat { get {
				return 	  this.ChartControl.Bars.SymbolInfo != null
					? this.ChartControl.Bars.SymbolInfo.PriceFormat
					: "N" + (this.PriceDecimals + 1); } }

		[Browsable(false)]	public	virtual	int		VolumeDecimals { get {
				return 	  this.ChartControl.Bars.SymbolInfo != null
					? this.ChartControl.Bars.SymbolInfo.VolumeDecimals
					: 1; } }
		[Browsable(false)]	public			string	VolumeFormat { get {
				return 	  this.ChartControl.Bars.SymbolInfo != null
					? this.ChartControl.Bars.SymbolInfo.VolumeFormat
					: "N" + (this.VolumeDecimals + 1); } }

		[Browsable(false)]	public 	virtual	double	PriceStep						{ get {
			return	  this.ChartControl.Bars.SymbolInfo != null
					? this.ChartControl.Bars.SymbolInfo.PriceStep
					: -1d; } }


		protected	virtual	int		ValueIndexLastAvailable_minusOneUnsafe { get {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new NotImplementedException();
			} }
		
		#region Panel.Width and Panel.Height - dependant
		[Browsable(false)]	public			int		PanelHeight_minusGutterBottomHeight { get {
				int ret = base.Height;
				if (this.GutterBottomDraw) {
					if (this.GutterBottomHeight_cached <= 0) {
						string msg = "MOVE_UPSTACK_THIS_FONT_HEIGHT_CALCULATION substraction must occur now but it's -1?"
							+ " developer should fix this lifecycle misconcept";
						Assembler.PopupException(msg);
					}
					ret -= this.GutterBottomHeight_cached;
				}
				return ret;
			} }
		[Browsable(false)]	private			int		PanelWidth_minusRightPriceGutter { get {
				int ret = base.Width;
				// if (base.DesignMode) this.ChartControl will be NULL
				if (this.GutterRightDraw) ret -= (this.ChartControl != null) ? this.ChartControl.GutterRightWidth_cached : 60;
				return ret;
			} }

		#endregion
		
		public					bool	ThisPanelIsPricePanel;
							bool	thisPanelIsVolumePanel;
							bool	thisPanelIsIndicatorPanel;

		protected	virtual	int		PaddingVerticalSqueeze { get { return 0; } }

							int		visibleBarRightExisting { get {
				int ret = this.VisibleBarRight_cached;
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) ret = this.ChartControl.Bars.Count - 1;
				return ret;
			} }
							int		visibleBarLeftExisting { get {
				int ret = this.VisibleBarLeft_cached;
				if (this.VisibleBarLeft_cached >= this.ChartControl.Bars.Count) ret = this.ChartControl.Bars.Count - 1;
				return ret;
			} }

							string	formatForBars { get {
				string ret = "FORMAT_FOR_BARS_UNDEFINED";
				Bars bars = this.ChartControl.Bars;
				switch (bars.ScaleInterval.Scale) {
					case BarScale.Minute: ret = this.ChartControl.ChartSettingsTemplated.GutterBottomDateFormatIntraday; break;
					case BarScale.Daily: ret = this.ChartControl.ChartSettingsTemplated.GutterBottomDateFormatDaily; break;
					case BarScale.Weekly:
					case BarScale.Monthly:
					case BarScale.Quarterly: ret = this.ChartControl.ChartSettingsTemplated.GutterBottomDateFormatDaily; break;
					case BarScale.Yearly: ret = this.ChartControl.ChartSettingsTemplated.GutterBottomDateFormatYearly; break;
					default: ret = "FORMAT_FOR_BARS_UNDEFINED"; break;
				}
				return ret;
			} }

			[Browsable(false)]	public			int		BarShadowOffset { get { return this.ChartControl.ChartSettingsIndividual.BarShadowXoffset; } }

							// only used in PanelLevel2 to grow to the left if PanelLeve2 is on the left of PanelPrice, or grow to the right if PanelLeve2 is on the right of PanelPrice
							public			MultiSplitContainer ParentMultiSplitContainer_nullUnsafe { get {return base.Parent as MultiSplitContainer; } }
							protected		List<Control>		ParentMultiSplitSiblings { get {
				return this.ParentMultiSplitContainer_nullUnsafe != null
					? this.ParentMultiSplitContainer_nullUnsafe.ControlsContained
					: new List<Control>();
			} }
							protected		bool	ParentMultiSplitIamFirst { get { return this.ParentMultiSplitSiblings.IndexOf(this) == 0; } }
							public			bool	ParentMultiSplitIamLast  { get { return this.ParentMultiSplitSiblings.IndexOf(this) == this.ParentMultiSplitSiblings.Count - 1; } }
							[Obsolete("RETURNS_LOCATION_AMONG_SAME_MULTISPLITTER_PANELS; TO_GET_X_OF_ALL_PANELS_WHEN_LEVEL2_IS_IN_LEFT_COLUMN_USE_")]
							public			Point	ParentMultiSplitMyLocationAmongSiblingsPanels { get {
			Point ret = new Point(-1, -1);
			foreach (Control meOrNeighbourPanels in this.ParentMultiSplitSiblings) {
				if (meOrNeighbourPanels != this) continue;
				ret = meOrNeighbourPanels.Location;
				break;
			}
			if (ret.X == -1) {
				string msg = "I_MUST_BE_IN_THE_LIST_OF_MULTISPLITTER_CONTENT_BUT_NOT_FOUND_NONSENSE";
				Assembler.PopupException(msg);
			}
			return ret;
		} }

		public PanelBase() {
			this.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.GutterRightDraw = true;
			this.GutterBottomDraw = false;
			
			if (this.AutoScroll != false) {
				this.AutoScroll  = false;		// IM_CAUSING_this.OnLayout()
			}
			base.HScroll = false;
			base.VScroll = false;
			base.AutoSize = false;

			// keep false or wont appear proportionally distributed base.AutoSize = true;
			//base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			
			this.ThisPanelIsPricePanel = this.GetType() == typeof(PanelPrice);		//panels don't change their type; hopefully a boolean calculated once will work faster than dynamic evaluation 
			this.thisPanelIsVolumePanel = this.GetType() == typeof(PanelVolume);
			this.thisPanelIsIndicatorPanel = this.GetType() == typeof(PanelIndicator);
		}
		public void Initialize(ChartControl chartControl, string barsIdent = "INITIALIZED_WITH_EMPTY_BARS_IDENT_PanelNamedFolding") {
			this.ChartControl = chartControl;
			this.BarsIdent = barsIdent;
		}
		public virtual void InitializeWithNonEmptyBars(ChartControl chartControl) {
			string barsIdent = chartControl.Bars.ToString();
			barsIdent = chartControl.Bars.SymbolIntervalScaleDSN;
			this.Initialize(chartControl, barsIdent);
		}
		public void DrawError(Graphics g, string msg) {
			this.DrawLabelOnNextLine(g, msg, null, Color.Red, Color.Empty);
		}

		protected virtual void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			// while overriding, include base.PaintWholeSurfaceBarsNotEmpty(g) as first line in derived's implementation
			// 1) uses derived's (PanelPrice,Volume) VisibleMinDoubleMaxValueUnsafe,VisibleMaxDoubleMinValueUnsafe to set:
			//		base.VisibleMin,Max,Range_cached,
			//		base.VisibleMinMinusTopSqueezer_cached, this.VisibleMaxPlusBottomSqueezer_cached, this.VisibleRangeWithTwoSqueezers_cached
			// 2) paints Right and Bottom gutter foregrounds;

			
			string msig = " " + this.PanelName + ".PanelPrice,Volume.PaintWholeSurfaceBarsNotEmpty() "
				+ this.BarsIdent + " " + this.Parent.ToString();

			this.VisibleMin_cached = this.VisibleMinDoubleMaxValueUnsafe;
			this.VisibleMax_cached = this.VisibleMaxDoubleMinValueUnsafe;
			this.VisibleRange_cached = this.VisibleMax_cached - this.VisibleMin_cached;

			if (this.VisibleMin_cached == Double.MaxValue) {
				string msg = "PAINTING_ZERO_OR_FIRST_BAR_OF_LIVESIMULATION__CONITNUE_UNTIL_IT_WILL_GET_NORMALIZED_SOON";
				return;
			}
			if (this.VisibleRange_cached == 0 && this.ChartControl.Bars.Count > 0) {
				string msg = "RANGE_CAN_NOT_BE_ZERO_WHEN_YOU_HAVE_BARS.COUNT[" + this.ChartControl.Bars.Count + "] [" + this.ToString() + "].VisibleRange_cached=0";
				//Assembler.PopupException(msg, null, false);
				return;
			}

			double pixelsSqueezedToPriceDistance = 0;
			if (this.PaddingVerticalSqueeze > 0 && base.Height > 0) {
				double priceDistanceInOnePixel = this.VisibleRange_cached / base.Height;
				pixelsSqueezedToPriceDistance = this.PaddingVerticalSqueeze * priceDistanceInOnePixel;
			}
			
			this.VisibleMinMinusTopSqueezer_cached = this.VisibleMin_cached - pixelsSqueezedToPriceDistance;
			this.VisibleMaxPlusBottomSqueezer_cached = this.VisibleMax_cached + pixelsSqueezedToPriceDistance;
			// min-Top=10-10=0; max+Bottom=20+10=30; 20+10-(10-10) = 30-0; = max-min+padding*2=20-10+10*2 = 30 
			this.VisibleRangeWithTwoSqueezers_cached = this.VisibleMaxPlusBottomSqueezer_cached - this.VisibleMinMinusTopSqueezer_cached;
			
			msig = " this.VisibleRangeWithTwoSqueezers_cached[" + this.VisibleRangeWithTwoSqueezers_cached + "]:"
				+ " [" + this.VisibleMinMinusTopSqueezer_cached + "]...[" + this.VisibleMaxPlusBottomSqueezer_cached + "]" + msig;
			if (double.IsNegativeInfinity(this.VisibleRangeWithTwoSqueezers_cached)) {
				string msg = "[" + this.PanelName + "]-RANGE_MUST_BE_NON_NEGATIVE_INFINITY";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (double.IsNaN(this.VisibleRangeWithTwoSqueezers_cached)) {
				string msg = "[" + this.PanelName + "]-RANGE_MUST_BE_NON_NAN";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.VisibleRangeWithTwoSqueezers_cached <= 0) {
				// gridStep will throw an ArithmeticException
				string msg = "[" + this.PanelName + "]-RANGE_MUST_BE_POSITIVE";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			
			this.GutterGridLinesRightBottomDrawForeground(g);
		}

		protected virtual void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
			if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) {
				return;
			}
			if (this.PanelHasValuesForVisibleBarWindow == false) {
				return;
			}

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
			int barRightX = this.PanelWidth_minusRightPriceGutter - this.BarWidth_includingPadding_cached;
			int barsFromRight = this.VisibleBarRight_cached - barVisible;
			int barWidthdsFromRightMargin = barsFromRight * this.BarWidth_includingPadding_cached;
			return barRightX - barWidthdsFromRightMargin; // shouldn't be negative
		}
		public int XToBar(int xMouseOver) {
			if (xMouseOver < 0) return -1;
			if (xMouseOver > this.PanelWidth_minusRightPriceGutter) return -2;
			int offsetFromRight = this.PanelWidth_minusRightPriceGutter - this.BarWidth_includingPadding_cached;
			for (int i = this.visibleBarRightExisting; i >= this.visibleBarLeftExisting;
					 i--, offsetFromRight -= this.BarWidth_includingPadding_cached) {
				if (xMouseOver > offsetFromRight) return i;
			}
			return -3; // negative means ERROR
		}
		public int ValueToYinverted(double priceOrVolume) {			//200
			if (this.ChartControl.BarsEmpty) return 666;
			if (double.IsNaN(priceOrVolume)) {
				string msg = "CHECK_IT_UPSTACK INDICATOR_MAY_HAS_NAN_FOR_BARS<PERIOD priceOrVolume[" + priceOrVolume + "]";
				Assembler.PopupException(msg, null, false);
				return 0;
			}
			double min = this.VisibleMinMinusTopSqueezer_cached;		//100
			double max = this.VisibleMaxPlusBottomSqueezer_cached;		//250
			double rangeMinMax = this.VisibleRangeWithTwoSqueezers_cached;			//150
			
			#region quickCheckFor *_cached variables; veeeeery slow in Debug; that's why *_cached exist
			#if DEBUG_HEAVY
			double min2 = this.VisibleMinDoubleMaxValueUnsafe;
			double max2 = this.VisibleMaxDoubleMinValueUnsafe;

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
			int ret = (int)Math.Round(this.PanelHeight_minusGutterBottomHeight_cached * priceAsPartOfRange);		// 600 * 0.6 = 360px UP
			ret = this.yInverted(ret);
			ret = this.AdjustToPanelHeight(ret);
			return ret;			// should be inverted to screen starting from upper left corner (600 - 360 = 240px DOWN)
		}
		public double YinvertedToValue(int yMouseInverted) {
			if (this.ChartControl.BarsEmpty) return 777;
			if (yMouseInverted <= 0) return this.VisibleMaxPlusBottomSqueezer_cached;
			if (yMouseInverted >= this.PanelHeight_minusGutterBottomHeight_cached) return this.VisibleMinMinusTopSqueezer_cached;
			int yStraight = yInverted(yMouseInverted);
			double yAsPartOfPlot = yStraight / (double) this.PanelHeight_minusGutterBottomHeight_cached;
			double PartOfVisibleRange = this.VisibleRangeWithTwoSqueezers_cached * yAsPartOfPlot;
			double ret = this.VisibleMinMinusTopSqueezer_cached + PartOfVisibleRange;
			return ret;
		}
		int yInverted(int y) {
			int yInverted = this.PanelHeight_minusGutterBottomHeight_cached - y;
			return yInverted;
		}
		public int AdjustToPanelHeight(int y) {
			if (y < 0) y = 0;
			if (y > this.PanelHeight_minusGutterBottomHeight_cached) y = this.PanelHeight_minusGutterBottomHeight_cached;
			return y;
		}
		int adjustToBoundariesHorizontalGutter(int x, int width) {
			if (x < this.ChartControl.ChartSettingsTemplated.GutterBottomPadding) x = this.ChartControl.ChartSettingsTemplated.GutterBottomPadding;
			if (x + width > base.Width) x = base.Width - width;
			return x;
		}
		
	 	void ensureFontMetricsAreCalculated(Graphics g) {
	 		if (this.GutterRightFontHeight_cached == -1) {
				this.GutterRightFontHeight_cached = (int)g.MeasureString("ABC123`'jg]", this.ChartControl.ChartSettingsTemplated.GutterRightFont).Height;
	 		//}
	 		//if (this.GutterRightFontHeightHalf_cached == -1) {
	 			this.GutterRightFontHeightHalf_cached = (int)(this.GutterRightFontHeight_cached / 2F);
	 		//}
	 		//if (this.GutterBottomFontHeight_cached == -1) {
				this.GutterBottomFontHeight_cached = (int)g.MeasureString("ABC123`'jg]", this.ChartControl.ChartSettingsTemplated.GutterBottomFont).Height;
	 		//}
	 		//if (this.GutterBottomHeight_cached == -1) {
				this.GutterBottomHeight_cached = this.GutterBottomFontHeight_cached + this.ChartControl.ChartSettingsTemplated.GutterBottomPadding * 2;
	 		}
		}
		public override string ToString() {
			string ret = this.PanelName;
			//ret += ": Location[" + this.Location + "]; Size[" + this.Size + "] (" + this.Height + ");"
			ret += " ClientRectangle[" + this.ClientRectangle + "]";
			return ret;
		}

		public string FormatValue(double value, bool shorten = false) {
			// on right gutter, indicators will also show MA="13" for SymbolInfo.DecimalsPrice=0
			string format = this.thisPanelIsVolumePanel ? this.VolumeFormat : this.PriceFormat;

			if (shorten) {
				double num = Math.Abs(value);
				if (num >= 1000000000000.0) {
					value /= 1000000000000.0;
					return value.ToString(format) + "T";
				}
				if (num >= 1000000000.0) {
					value /= 1000000000.0;
					return value.ToString(format) + "B";
				}
				if (num >= 1000000.0) {
					value /= 1000000.0;
					return value.ToString(format) + "M";
				}
				if (num >= 10000.0) {
					value /= 1000.0;
					return value.ToString(format) + "K";
				}
			}
			return value.ToString(format);
		}

		bool ignoreResizeImSettingWidthOrHeight;
		internal bool SetWidthIgnoreResize(int panelWidth) {
			if (base.Width == panelWidth) return false;
			if (this.ignoreResizeImSettingWidthOrHeight) return false;
			try {
				this.ignoreResizeImSettingWidthOrHeight = true;
				if (base.Parent != null) {
					if (panelWidth > base.Parent.Width) {
						string msg = "BEOYND_PARENTS_WIDTH__NOT_TALKING_ABOUT_BORDERS panelWidth[" + panelWidth + "] >= base.Parent.Width[" + base.Parent.Width + "]";
						Assembler.PopupException(msg, null, false);
					}
				}
				base.Width  = panelWidth;
			} finally {
				this.ignoreResizeImSettingWidthOrHeight = false;
			}
			return true;
		}
		internal bool SetHeightIgnoreResize(int panelHeight) {
			if (base.Height == panelHeight) return false;
			if (this.ignoreResizeImSettingWidthOrHeight) return false;
			try {
				this.ignoreResizeImSettingWidthOrHeight = true;
				if (base.Parent != null) {
					if (panelHeight > base.Parent.Height) {
						string msg = "BEOYND_PARENTS_HEIGHT__NOT_TALKING_ABOUT_BORDERS panelWidth[" + panelHeight + "] >= base.Parent.Width[" + base.Parent.Height + "]";
						Assembler.PopupException(msg, null, false);
					}
				}
				base.Height  = panelHeight;
			} finally {
				this.ignoreResizeImSettingWidthOrHeight = false;
			}
			return true;
		}
	}
}