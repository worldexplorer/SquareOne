using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Charting.MultiSplit;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Charting {
	// why ChartSettings inherits Component? F4 on ChartSettings will allow you to edit colors visually
	// REMOVE ": Component" when you're done with visual editing to stop Designer flooding ChartControl.Designer.cs
	public class ChartSettings {	//: Component {
		[JsonProperty]	public Color	ChartColorBackground { get; set; }
		[JsonProperty]	public int		BarWidthIncludingPadding { get; set; }
		[JsonProperty]	public int		BarWidthIncludingPaddingMax { get; set; }
		[JsonProperty]	public Font		PanelNameAndSymbolFont { get; set; }
		[JsonProperty]	public Color	PriceColorBarUp { get; set; }
		[JsonProperty]	public Color	PriceColorBarDown { get; set; }
		[JsonProperty]	public Color	VolumeColorBarUp { get; set; }
		[JsonProperty]	public Color	VolumeColorBarDown { get; set; }
		[JsonProperty]	public Color	GutterRightColorBackground { get; set; }
		[JsonProperty]	public Color	GutterRightColorForeground { get; set; }
		[JsonProperty]	public int		GutterRightPadding { get; set; }
		[JsonProperty]	public Font		GutterRightFont { get; set; }
		[JsonProperty]	public Color	GutterBottomColorBackground { get; set; }
		[JsonProperty]	public Color	GutterBottomColorForeground { get; set; }
		[JsonProperty]	public Color	GutterBottomNewDateColorForeground { get; set; }
		[JsonProperty]	public int		GutterBottomPadding { get; set; }
		[JsonProperty]	public Font		GutterBottomFont { get; set; }
		[JsonProperty]	public string	GutterBottomDateFormatDayOpener;
		[JsonProperty]	public string	GutterBottomDateFormatIntraday;
		[JsonProperty]	public string	GutterBottomDateFormatDaily;
		[JsonProperty]	public string	GutterBottomDateFormatWeekly;
		[JsonProperty]	public string	GutterBottomDateFormatYearly;
		[JsonProperty]	public Color	GridlinesHorizontalColor { get; set; }
		[JsonProperty]	public Color	GridlinesVerticalColor { get; set; }
		[JsonProperty]	public Color	GridlinesVerticalNewDateColor { get; set; }
		[JsonProperty]	public bool		GridlinesHorizontalShow { get; set; }
		[JsonProperty]	public bool		GridlinesVerticalShow { get; set; }
		[JsonProperty]	public int		ScrollSqueezeMouseDragSensitivityPx { get; set; }
		[JsonProperty]	public int		ScrollNBarsPerOneDragMouseEvent { get; set; }
		[JsonProperty]	public int		ScrollNBarsPerOneKeyPress { get; set; }
		[JsonProperty]	public int		SqueezeVerticalPaddingPx { get; set; }
		[JsonProperty]	public int		SqueezeVerticalPaddingStep { get; set; }
		[JsonProperty]	public int		SqueezeHorizontalStep { get; set; }
		[JsonProperty]	public int		SqueezeHorizontalMouse1pxDistanceReceivedToOneStep { get; set; }
		[JsonProperty]	public int		SqueezeHorizontalKeyOnePressReceivedToOneStep { get; set; }
		[JsonProperty]	public bool		TooltipPriceShow { get; set; }
		[JsonProperty]	public bool		TooltipPriceShowOnlyWhenMouseTouchesCandle { get; set; }
		[JsonProperty]	public bool		TooltipPositionShow { get; set; }
		[JsonProperty]	public int		TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave { get; set; }
		[JsonProperty]	public int		PositionArrowPaddingVertical { get; set; }
		[JsonProperty]	public int		ScrollPositionAtBarIndex { get; set; }
		[JsonProperty]	public int		TooltipBordersMarginToKeepBordersVisible { get; set; }
		[JsonProperty]	public Color	PositionPlannedEllipseColor { get; set; }
		[JsonProperty]	public int		PositionPlannedEllipseColorAlpha { get; set; }
		[JsonProperty]	public int		PositionPlannedEllipseDiameter { get; set; }
		[JsonProperty]	public Color	PositionFilledDotColor { get; set; }
		[JsonProperty]	public int		PositionFilledDotColorAlpha { get; set; }
		[JsonProperty]	public int		PositionFilledDotDiameter { get; set; }
		[JsonProperty]	public int		PositionLineHighlightedWidth { get; set; }
		[JsonProperty]	public int		PositionLineHighlightedAlpha { get; set; }
		[JsonProperty]	public Color	PositionLineNoExitYetColor { get; set; }
		[JsonProperty]	public int		PositionLineNoExitYetColorAlpha { get; set; }
		[JsonProperty]	public Color	PositionLineProfitableColor { get; set; }
		[JsonProperty]	public int		PositionLineProfitableColorAlpha { get; set; }
		[JsonProperty]	public Color	PositionLineLossyColor { get; set; }
		[JsonProperty]	public int		PositionLineLossyColorAlpha { get; set; }
		[JsonProperty]	public int 		PriceVsVolumeSplitterDistance { get; set; }
		[JsonProperty]	public Color	AlertPendingEllipseColor { get; set; }
		[JsonProperty]	public int		AlertPendingEllipseColorAlpha { get; set; }
		[JsonProperty]	public int		AlertPendingEllipseWidth { get; set; }
		[JsonProperty]	public bool		MousePositionTrackOnGutters { get; set; }
		[JsonProperty]	public Color	MousePositionTrackOnGuttersColor { get; set; }
		[JsonProperty]	public int		BarsBackgroundTransparencyAlfa { get; set; }
		[JsonProperty]	public int		ChartLabelsUpperLeftYstartTopmost { get; set; }
		[JsonProperty]	public int		ChartLabelsUpperLeftX { get; set; }
		[JsonProperty]	public int		ChartLabelsUpperLeftPlatePadding { get; set; }
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquarePadding { get; set; }
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquareSize { get; set; }
		[JsonProperty]	public int		OnChartBarAnnotationsVerticalAwayFromPositionArrows { get; set; }
		
		// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterPropertiesByPanelName;
		
		// DONE_IN_RenderBarsPrice_KISS cache them all until user edits this.BarTotalWidthPx so they won't be calculated again with the same result for each bar
		[JsonIgnore]	public int BarPaddingRight { get {
			if (this.BarWidthIncludingPadding <= 3) return 0;
			//int nominal = (int) (this.BarWidthIncludingPadding * 0.25F);
			int nominal = 1;
			// algo below allows you have this.BarTotalWidthPx both odd and even automatically
			int compensated = nominal;
			int keepWidthOdd = this.BarWidthIncludingPadding - compensated;
			if (keepWidthOdd % 2 == 0) compensated++;	// increase padding to have 1px shadows right in the middle of a bar
			return compensated;
		} }
		[JsonIgnore]	public int BarWidthMinusRightPadding { get { return this.BarWidthIncludingPadding - this.BarPaddingRight; } }
		[JsonIgnore]	public int BarShadowXoffset { get { return this.BarWidthMinusRightPadding / 2; } }
		
		[JsonIgnore]	SolidBrush brushBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushBackground { get {
				if (this.brushBackground == null) this.brushBackground = new SolidBrush(this.ChartColorBackground);
				return this.brushBackground;
			} }

		[JsonIgnore]	SolidBrush brushBackgroundReversed;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushBackgroundReversed { get {
				if (this.brushBackgroundReversed == null) this.brushBackgroundReversed = new SolidBrush(ColorReverse(this.ChartColorBackground));
				return this.brushBackgroundReversed;
			} }

		[JsonIgnore]	SolidBrush brushGutterRightBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterRightBackground { get {
				if (this.brushGutterRightBackground == null) this.brushGutterRightBackground = new SolidBrush(this.GutterRightColorBackground);
				return this.brushGutterRightBackground;
			} }

		[JsonIgnore]	SolidBrush brushGutterRightForeground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterRightForeground { get {
				if (this.brushGutterRightForeground == null) this.brushGutterRightForeground = new SolidBrush(this.GutterRightColorForeground);
				return this.brushGutterRightForeground;
			} }

		[JsonIgnore]	SolidBrush brushGutterBottomBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomBackground { get {
				if (this.brushGutterBottomBackground == null) this.brushGutterBottomBackground = new SolidBrush(this.GutterBottomColorBackground);
				return this.brushGutterBottomBackground;
			} }

		[JsonIgnore]	SolidBrush brushGutterBottomForeground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomForeground { get {
				if (this.brushGutterBottomForeground == null) this.brushGutterBottomForeground = new SolidBrush(this.GutterBottomColorForeground);
				return this.brushGutterBottomForeground;
			} }

		[JsonIgnore]	SolidBrush brushGutterBottomNewDateForeground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomNewDateForeground { get {
				if (this.brushGutterBottomNewDateForeground == null) this.brushGutterBottomNewDateForeground = new SolidBrush(this.GutterBottomNewDateColorForeground);
				return this.brushGutterBottomNewDateForeground;
			} }

		[JsonIgnore]	SolidBrush brushPriceBarUp;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushPriceBarUp { get {
				if (this.brushPriceBarUp == null) this.brushPriceBarUp = 
					new SolidBrush(this.PriceColorBarUp);
				return this.brushPriceBarUp;
			} }

		[JsonIgnore]	Pen penPriceBarUp;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPriceBarUp { get {
				if (this.penPriceBarUp == null) this.penPriceBarUp = 
					new Pen(this.PriceColorBarUp);
				return this.penPriceBarUp;
			} }

		[JsonIgnore]	SolidBrush brushPriceBarDown;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushPriceBarDown { get {
				if (this.brushPriceBarDown == null) this.brushPriceBarDown = 
					new SolidBrush(this.PriceColorBarDown);
				return this.brushPriceBarDown;
			} }

		[JsonIgnore]	Pen penPriceBarDown;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPriceBarDown { get {
				if (this.penPriceBarDown == null) this.penPriceBarDown = 
					new Pen(this.PriceColorBarDown);
				return this.penPriceBarDown;
			} }

		[JsonIgnore]	SolidBrush brushVolumeBarUp;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushVolumeBarUp { get {
				if (this.brushVolumeBarUp == null) this.brushVolumeBarUp = 
					new SolidBrush(this.VolumeColorBarUp);
				return this.brushVolumeBarUp;
			} }

		[JsonIgnore]	SolidBrush brushVolumeBarDown;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushVolumeBarDown { get {
				if (this.brushVolumeBarDown == null) this.brushVolumeBarDown = 
					new SolidBrush(this.VolumeColorBarDown);
				return this.brushVolumeBarDown;
			} }

		[JsonIgnore]	Pen penGridlinesHorizontal;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesHorizontal { get {
				if (this.penGridlinesHorizontal == null) this.penGridlinesHorizontal = 
					new Pen(this.GridlinesHorizontalColor);
				return this.penGridlinesHorizontal;
			} }

		[JsonIgnore]	Pen penGridlinesVertical;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesVertical { get {
				if (this.penGridlinesVertical == null) this.penGridlinesVertical = 
					new Pen(this.GridlinesVerticalColor);
				return this.penGridlinesVertical;
			} }

		[JsonIgnore]	Pen penGridlinesVerticalNewDate;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesVerticalNewDate { get {
				if (this.penGridlinesVerticalNewDate == null) this.penGridlinesVerticalNewDate = 
					new Pen(this.GridlinesVerticalNewDateColor);
				return this.penGridlinesVerticalNewDate;
			} }

		
		[JsonIgnore]	Pen penPositionPlannedEllipse;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionPlannedEllipse { get {
				if (this.penPositionPlannedEllipse == null) this.penPositionPlannedEllipse =
					new Pen(Color.FromArgb(this.PositionPlannedEllipseColorAlpha, this.PositionPlannedEllipseColor), this.PositionPlannedEllipseDiameter);
				return this.penPositionPlannedEllipse;
			} }

		[JsonIgnore]	Pen penPositionFilledDot;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionFilledDot { get {
				if (this.penPositionFilledDot == null) this.penPositionFilledDot =
					new Pen(Color.FromArgb(this.PositionFilledDotColorAlpha, this.PositionFilledDotColor));
				return this.penPositionFilledDot;
			} }

		[JsonIgnore]	Brush brushPositionFilledDot;
		//[Browsable(false)]
		[JsonIgnore]	public Brush BrushPositionFilledDot { get {
				if (this.brushPositionFilledDot == null) this.brushPositionFilledDot = 
					new SolidBrush(Color.FromArgb(this.PositionFilledDotColorAlpha, this.PositionFilledDotColor));
				return this.brushPositionFilledDot;
			} }

		[JsonIgnore]	Pen penPositionLineEntryExitConnectedUnknown;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedUnknown { get {
				if (this.penPositionLineEntryExitConnectedUnknown == null) this.penPositionLineEntryExitConnectedUnknown =
					new Pen(Color.FromArgb(this.PositionLineNoExitYetColorAlpha, this.PositionLineNoExitYetColor));
				return this.penPositionLineEntryExitConnectedUnknown;
			} }
		[JsonIgnore]	Pen penPositionLineEntryExitConnectedProfit;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedProfit { get {
				if (this.penPositionLineEntryExitConnectedProfit == null) this.penPositionLineEntryExitConnectedProfit =
					new Pen(Color.FromArgb(this.PositionLineProfitableColorAlpha, this.PositionLineProfitableColor));
				return this.penPositionLineEntryExitConnectedProfit;
			} }


		[JsonIgnore]	Pen penPositionLineEntryExitConnectedLoss;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedLoss { get {
				if (this.penPositionLineEntryExitConnectedLoss == null) this.penPositionLineEntryExitConnectedLoss =
					new Pen(Color.FromArgb(this.PositionLineLossyColorAlpha, this.PositionLineLossyColor));
				return this.penPositionLineEntryExitConnectedLoss;
			} }

		[JsonIgnore]	Pen penAlertPendingEllipse;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenAlertPendingEllipse { get {
				if (this.penAlertPendingEllipse == null) this.penAlertPendingEllipse =
					new Pen(Color.FromArgb(this.AlertPendingEllipseColorAlpha, this.AlertPendingEllipseColor), this.AlertPendingEllipseWidth);
				return this.penAlertPendingEllipse;
			} }

		[JsonIgnore]	Pen penMousePositionTrackOnGutters;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenMousePositionTrackOnGutters { get {
				if (this.penMousePositionTrackOnGutters == null) this.penMousePositionTrackOnGutters = 
					new Pen(this.MousePositionTrackOnGuttersColor);
				return this.penMousePositionTrackOnGutters;
			} }

		
		public ChartSettings()	{
			ChartColorBackground = Color.White;
			BarWidthIncludingPadding = 8;
			BarWidthIncludingPaddingMax = 100;
			PanelNameAndSymbolFont = new Font("Microsoft Sans Serif", 8.25f);
			PriceColorBarUp = Color.RoyalBlue;
			PriceColorBarDown = Color.IndianRed;
			VolumeColorBarUp = Color.CadetBlue;
			VolumeColorBarDown = Color.CadetBlue;
			GutterRightColorBackground = Color.Gainsboro;
			GutterRightColorForeground = Color.Black;
			GutterRightFont = new Font("Consolas", 8f);
			GutterRightPadding = 5;
			GutterBottomColorBackground = Color.Gainsboro;
			GutterBottomColorForeground = Color.Black;
			GutterBottomNewDateColorForeground = Color.Green;
			GutterBottomFont = new Font("Consolas", 8f);
			GutterBottomPadding = 2;
			GutterBottomDateFormatDayOpener = "ddd dd-MMM-yyyy";
			GutterBottomDateFormatIntraday = "HH:mm";
			GutterBottomDateFormatDaily = "ddd dd-MMM";
			GutterBottomDateFormatWeekly = "MMM-yy";
			GutterBottomDateFormatYearly = "yyyy";
			GridlinesHorizontalShow = true;
			GridlinesHorizontalColor = Color.WhiteSmoke;
			GridlinesVerticalShow = true;
			GridlinesVerticalColor = Color.WhiteSmoke;
			GridlinesVerticalNewDateColor = Color.Lime;
			ScrollSqueezeMouseDragSensitivityPx = 1;
			ScrollNBarsPerOneDragMouseEvent = 3;
			ScrollNBarsPerOneKeyPress = 1;
			SqueezeVerticalPaddingPx = 0;
			SqueezeVerticalPaddingStep = 10;	// in VerticalPixels (later converted using pixelsSqueezedToPriceDistance)
			SqueezeHorizontalStep = 2;			// in BarWidthPixels
			SqueezeHorizontalMouse1pxDistanceReceivedToOneStep = 5;
			SqueezeHorizontalKeyOnePressReceivedToOneStep = 1;
			TooltipPriceShow = true;
			TooltipPositionShow = true;
			TooltipPriceShowOnlyWhenMouseTouchesCandle = true;
			TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave = 3;		// MouseX will never go over tooltip => PanelNamedFolding.OnMouseLeave() never invoked
			PositionArrowPaddingVertical = 2;
			TooltipBordersMarginToKeepBordersVisible = 2;
			PositionPlannedEllipseColor = Color.Aqua;
			PositionPlannedEllipseColorAlpha = 90;
			PositionPlannedEllipseDiameter = 6;
			PositionFilledDotColor = Color.Olive;
			PositionFilledDotColorAlpha = 200;
			PositionFilledDotDiameter = 4;
			PositionLineHighlightedWidth = 2;
			PositionLineHighlightedAlpha = 230;
			PositionLineNoExitYetColor = Color.Gray;
			PositionLineNoExitYetColorAlpha = 100;
			PositionLineProfitableColor = Color.Green;
			PositionLineProfitableColorAlpha = 100;
			PositionLineLossyColor = Color.Salmon;
			PositionLineLossyColorAlpha = 100;
			PriceVsVolumeSplitterDistance = 0;
			AlertPendingEllipseColor = Color.Maroon;
			AlertPendingEllipseColorAlpha = 160;
			AlertPendingEllipseWidth = 1;
			MousePositionTrackOnGutters = true;
			MousePositionTrackOnGuttersColor = Color.LightGray;
			BarsBackgroundTransparencyAlfa = 128;
			ChartLabelsUpperLeftYstartTopmost = 18;
			ChartLabelsUpperLeftX = 5;
			ChartLabelsUpperLeftPlatePadding = 1;
			ChartLabelsUpperLeftIndicatorSquarePadding = 4;
			ChartLabelsUpperLeftIndicatorSquareSize = 5;
			OnChartBarAnnotationsVerticalAwayFromPositionArrows = 3;

			// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
			MultiSplitterPropertiesByPanelName = new Dictionary<string, MultiSplitterProperties>();
		}
		
		public static Color ColorReverse(Color color) {
			int red = 255 - color.R;
			int green = 255 - color.G;
			int blue = 255 - color.B;
			return Color.FromArgb((int)red, (int)green, (int)blue);
		}
	}
}