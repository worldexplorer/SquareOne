using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core.Charting;

namespace Sq1.Charting {
	// why ChartSettings inherits Component? F4 on ChartSettings will allow you to edit colors visually
	// REMOVE ": Component" when you're done with visual editing to stop Designer flooding ChartControl.Designer.cs
	public class ChartSettings { //: Component {
		[JsonProperty]	public Color	ChartColorBackground;
		[JsonProperty]	public int		BarWidthIncludingPadding;
		[JsonProperty]	public int		BarWidthIncludingPaddingMax;
		[JsonProperty]	public Font		PanelNameAndSymbolFont;
		[JsonProperty]	public Color	PriceColorBarUp;
		[JsonProperty]	public Color	PriceColorBarDown;
		[JsonProperty]	public Color	VolumeColorBarUp;
		[JsonProperty]	public Color	VolumeColorBarDown;
		[JsonProperty]	public Color	GutterRightColorBackground;
		[JsonProperty]	public Color	GutterRightColorForeground;
		[JsonProperty]	public int		GutterRightPadding;
		[JsonProperty]	public Font		GutterRightFont;
		[JsonProperty]	public Color	GutterBottomColorBackground;
		[JsonProperty]	public Color	GutterBottomColorForeground;
		[JsonProperty]	public Color	GutterBottomNewDateColorForeground;
		[JsonProperty]	public int		GutterBottomPadding;
		[JsonProperty]	public Font		GutterBottomFont;
		[JsonProperty]	public string	GutterBottomDateFormatDayOpener;
		[JsonProperty]	public string	GutterBottomDateFormatIntraday;
		[JsonProperty]	public string	GutterBottomDateFormatDaily;
		[JsonProperty]	public string	GutterBottomDateFormatWeekly;
		[JsonProperty]	public string	GutterBottomDateFormatYearly;
		[JsonProperty]	public Color	GridlinesHorizontalColor;
		[JsonProperty]	public Color	GridlinesVerticalColor;
		[JsonProperty]	public Color	GridlinesVerticalNewDateColor;
		[JsonProperty]	public bool		GridlinesHorizontalShow;
		[JsonProperty]	public bool		GridlinesVerticalShow;
		[JsonProperty]	public int		ScrollSqueezeMouseDragSensitivityPx;
		[JsonProperty]	public int		ScrollNBarsPerOneDragMouseEvent;
		[JsonProperty]	public int		ScrollNBarsPerOneKeyPress;
		[JsonProperty]	public int		SqueezeVerticalPaddingPx;
		[JsonProperty]	public int		SqueezeVerticalPaddingStep;
		[JsonProperty]	public int		SqueezeHorizontalStep;
		[JsonProperty]	public int		SqueezeHorizontalMouse1pxDistanceReceivedToOneStep;
		[JsonProperty]	public int		SqueezeHorizontalKeyOnePressReceivedToOneStep;
		[JsonProperty]	public bool		TooltipPriceShow;
		[JsonProperty]	public bool		TooltipPriceShowOnlyWhenMouseTouchesCandle;
		[JsonProperty]	public bool		TooltipPositionShow;
		[JsonProperty]	public int		TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave;
		[JsonProperty]	public int		PositionArrowPaddingVertical;
		[JsonProperty]	public int		ScrollPositionAtBarIndex;
		[JsonProperty]	public int		TooltipBordersMarginToKeepBordersVisible;
		[JsonProperty]	public Color	PositionPlannedEllipseColor;
		[JsonProperty]	public int		PositionPlannedEllipseColorAlpha;
		[JsonProperty]	public int		PositionPlannedEllipseDiameter;
		[JsonProperty]	public Color	PositionFilledDotColor;
		[JsonProperty]	public int		PositionFilledDotColorAlpha;
		[JsonProperty]	public int		PositionFilledDotDiameter;
		[JsonProperty]	public int		PositionLineHighlightedWidth;
		[JsonProperty]	public int		PositionLineHighlightedAlpha;
		[JsonProperty]	public Color	PositionLineNoExitYetColor;
		[JsonProperty]	public int		PositionLineNoExitYetColorAlpha;
		[JsonProperty]	public Color	PositionLineProfitableColor;
		[JsonProperty]	public int		PositionLineProfitableColorAlpha;
		[JsonProperty]	public Color	PositionLineLossyColor;
		[JsonProperty]	public int		PositionLineLossyColorAlpha;
		[JsonProperty]	public int 		PriceVsVolumeSplitterDistance;
		[JsonProperty]	public Color	AlertPlacedEllipseColor;
		[JsonProperty]	public int		AlertPlacedEllipseColorAlpha;
		[JsonProperty]	public int		AlertPlacedEllipsePenWidth;
		[JsonProperty]	public int		AlertPlacedEllipseRadius;
		[JsonProperty]	public bool		MousePositionTrackOnGutters;
		[JsonProperty]	public Color	MousePositionTrackOnGuttersColor;
		[JsonProperty]	public int		BarsBackgroundTransparencyAlpha;
		[JsonProperty]	public int		ChartLabelsUpperLeftYstartTopmost;
		[JsonProperty]	public int		ChartLabelsUpperLeftX;
		[JsonProperty]	public int		ChartLabelsUpperLeftPlatePadding;
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquarePadding;
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquareSize;
		[JsonProperty]	public int		OnChartBarAnnotationsVerticalAwayFromPositionArrows;

		[JsonProperty]	public Color	SpreadBidLineColor;
		[JsonProperty]	public int		SpreadBidLineColorAlpha;
		[JsonProperty]	public int		SpreadBidLineWidth;
		[JsonProperty]	public Color	SpreadAskLineColor;
		[JsonProperty]	public int		SpreadAskLineColorAlpha;
		[JsonProperty]	public int		SpreadAskLineWidth;
		[JsonProperty]	public Font		SpreadLabelFont;
		[JsonProperty]	public Color	SpreadLabelColor;

		[JsonProperty]	public Color	LevelTwoColorBackgroundStreamingHasNoLastQuote;
		[JsonProperty]	public Color	LevelTwoColorBackground;
		[JsonProperty]	public Color	LevelTwoLotsColorBackground;
		[JsonProperty]	public Color	LevelTwoLotsColorForeground;
		[JsonProperty]	public Color	LevelTwoAskColorBackground;
		[JsonProperty]	public Color	LevelTwoAskColorContour;
		[JsonProperty]	public Color	LevelTwoBidColorBackground;
		[JsonProperty]	public Color	LevelTwoBidColorContour;

		[JsonProperty]	public int		LevelTwoMinimumPriceLevelThicknessRendered;
		[JsonProperty]	public bool		LevelTwoLotDrawString;
		[JsonProperty]	public Font		LevelTwoLotFont;
		[JsonProperty]	public Color	LevelTwoLotColor;
		[JsonProperty]	public int		LevelTwoLotPaddingHorizontal;



		// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterRowsPropertiesByPanelName;
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterColumnsPropertiesByPanelName;
		
		// DONE_IN_RenderBarsPrice_KISS cache them all until user edits this.BarTotalWidthPx so they won't be calculated again with the same result for each bar
		[JsonIgnore]	public int BarPaddingRight { get {
			if (this.BarWidthIncludingPadding <= 3) return 0;
			//int nominal = (int) (this.BarWidthIncludingPadding * 0.25F);
			int nominal = 1;
			// algo below allows you have this.BarTotalWidthPx both odd and even automatically
			//int compensated = nominal;
			//int keepWidthOdd = this.BarWidthIncludingPadding - compensated;
			//if (keepWidthOdd % 2 == 0) compensated++;	// increase padding to have 1px shadows right in the middle of a bar
			//return compensated;
			return nominal;
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
					new Pen(Color.FromArgb(this.AlertPlacedEllipseColorAlpha, this.AlertPlacedEllipseColor), this.AlertPlacedEllipsePenWidth);
				return this.penAlertPendingEllipse;
			} }

		[JsonIgnore]	Pen penMousePositionTrackOnGutters;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenMousePositionTrackOnGutters { get {
				if (this.penMousePositionTrackOnGutters == null) this.penMousePositionTrackOnGutters = 
					new Pen(this.MousePositionTrackOnGuttersColor);
				return this.penMousePositionTrackOnGutters;
			} }

		[JsonIgnore]	Pen penSpreadBid;
		[JsonIgnore]	public Pen PenSpreadBid { get {
				if (this.penSpreadBid == null) this.penSpreadBid =
					new Pen(Color.FromArgb(this.SpreadBidLineColorAlpha, this.SpreadBidLineColor), this.SpreadAskLineWidth);
				return this.penSpreadBid;
			} }

		[JsonIgnore]	Pen penSpreadAsk;
		[JsonIgnore]	public Pen PenSpreadAsk { get {
				if (this.penSpreadAsk == null) this.penSpreadAsk =
					new Pen(Color.FromArgb(this.SpreadAskLineColorAlpha, this.SpreadAskLineColor), this.SpreadAskLineWidth);
				return this.penSpreadAsk;
			} }

		[JsonIgnore]	Brush brushSpreadLabel;
		[JsonIgnore]	public Brush BrushSpreadLabel { get {
				if (this.brushSpreadLabel == null) this.brushSpreadLabel = new SolidBrush(this.SpreadLabelColor);
				return this.brushSpreadLabel;
			} }

		[JsonIgnore]	SolidBrush brushLevelTwoLotsColorBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoLotsColorBackground { get {
				if (this.brushLevelTwoLotsColorBackground == null) this.brushLevelTwoLotsColorBackground =
					new SolidBrush(this.LevelTwoLotsColorBackground);
				return this.brushLevelTwoLotsColorBackground;
			} }

		[JsonIgnore]	SolidBrush brushLevelTwoAskColorBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoAskColorBackground { get {
				if (this.brushLevelTwoAskColorBackground == null) this.brushLevelTwoAskColorBackground =
					new SolidBrush(this.LevelTwoAskColorBackground);
				return this.brushLevelTwoAskColorBackground;
			} }

		[JsonIgnore]	SolidBrush brushLevelTwoBidColorBackground;
		//[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoBidColorBackground { get {
				if (this.brushLevelTwoBidColorBackground == null) this.brushLevelTwoBidColorBackground =
					new SolidBrush(this.LevelTwoBidColorBackground);
				return this.brushLevelTwoBidColorBackground;
			} }

		[JsonIgnore]	Pen penLevelTwoAskColorContour;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenLevelTwoAskColorContour { get {
				if (this.penLevelTwoAskColorContour == null) this.penLevelTwoAskColorContour =
					new Pen(this.LevelTwoAskColorContour);
				return this.penLevelTwoAskColorContour;
			} }

		[JsonIgnore]	Pen penLevelTwoBidColorContour;
		//[Browsable(false)]
		[JsonIgnore]	public Pen PenLevelTwoBidColorContour { get {
				if (this.penLevelTwoBidColorContour == null) this.penLevelTwoBidColorContour =
					new Pen(this.LevelTwoBidColorContour);
				return this.penLevelTwoBidColorContour;
			} }

		[JsonIgnore]	Brush brushLevelTwoLot;
		[JsonIgnore]	public Brush BrushLevelTwoLot { get {
				if (this.brushLevelTwoLot == null) this.brushLevelTwoLot = new SolidBrush(this.LevelTwoLotColor);
				return this.brushLevelTwoLot;
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
			PositionFilledDotColor = Color.Chocolate;
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
			AlertPlacedEllipseColor = Color.DarkBlue;
			AlertPlacedEllipseColorAlpha = 180;
			AlertPlacedEllipsePenWidth = 2;
			AlertPlacedEllipseRadius = 3;
			MousePositionTrackOnGutters = true;
			MousePositionTrackOnGuttersColor = Color.LightGray;
			BarsBackgroundTransparencyAlpha = 24;
			ChartLabelsUpperLeftYstartTopmost = 18;
			ChartLabelsUpperLeftX = 5;
			ChartLabelsUpperLeftPlatePadding = 1;
			ChartLabelsUpperLeftIndicatorSquarePadding = 4;
			ChartLabelsUpperLeftIndicatorSquareSize = 5;
			OnChartBarAnnotationsVerticalAwayFromPositionArrows = 3;

			// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
			MultiSplitterRowsPropertiesByPanelName = new Dictionary<string, MultiSplitterProperties>();

			SpreadBidLineColor = Color.Gray;
			SpreadBidLineColorAlpha = 64;
			SpreadBidLineWidth = 1;
			SpreadAskLineColor = Color.Orange;
			SpreadAskLineColorAlpha = 64;
			SpreadAskLineWidth = 1;
			SpreadLabelFont = new Font("Consolas", 8f);
			SpreadLabelColor = Color.DarkGray;

			LevelTwoColorBackgroundStreamingHasNoLastQuote = Color.DarkGray;
			LevelTwoColorBackground		= Color.White;
			LevelTwoLotsColorBackground = Color.WhiteSmoke;
			LevelTwoLotsColorForeground	= Color.Black;
			LevelTwoAskColorBackground	= Color.FromArgb(255, 230, 230);
			LevelTwoAskColorContour		= Color.FromArgb(this.LevelTwoAskColorBackground.R - 50, this.LevelTwoAskColorBackground.G - 50, this.LevelTwoAskColorBackground.B - 50);
			LevelTwoBidColorBackground	= Color.FromArgb(230, 255, 230);
			LevelTwoBidColorContour		= Color.FromArgb(this.LevelTwoBidColorBackground.R - 50, this.LevelTwoBidColorBackground.G - 50, this.LevelTwoBidColorBackground.B - 50);

			LevelTwoMinimumPriceLevelThicknessRendered = 14;
			LevelTwoLotDrawString = true;
			LevelTwoLotFont = new Font("Microsoft Sans Serif", 8.25f);
			LevelTwoLotColor = Color.Black;
			LevelTwoLotPaddingHorizontal = 3;
		}

		public static Color ColorReverse(Color color) {
			int red = 255 - color.R;
			int green = 255 - color.G;
			int blue = 255 - color.B;
			return Color.FromArgb((int)red, (int)green, (int)blue);
		}
	}
}