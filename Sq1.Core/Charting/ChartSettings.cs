using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

using Newtonsoft.Json;

using Sq1.Core.DataFeed;
using Sq1.Core.Charting;

namespace Sq1.Core.Charting {
	// why ChartSettings inherits Component? F4 on ChartSettings will allow you to edit colors visually
	// REMOVE ": Component" when you're done with visual editing to stop Designer flooding ChartControl.Designer.cs
	public class ChartSettings : NamedObjectJsonSerializable {	//: ChartSettingsBase { //: Component {
		[Browsable(false)]
		[JsonIgnore]	public static string NAME_DEFAULT = "Default";

		[Category("1. Essential"), Description("description to be composed"), ReadOnly(true), Browsable(false)]
		[JsonProperty]	public string	StrategyName											{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	ChartColorBackground									{ get; set; }


		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		BarWidthIncludingPadding								{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		BarWidthIncludingPaddingMax								{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Font		PanelNameAndSymbolFont									{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	PriceColorBarUp											{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	PriceColorBarDown										{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	VolumeColorBarUp										{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	VolumeColorBarDown										{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public Color	VolumeRightGutterColorForeground						{ get; set; }


		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public bool		BarUpFillCandleBody										{ get; set; }


		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GutterRightColorBackground								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GutterRightColorForeground								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public int		GutterRightPadding										{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Font		GutterRightFont											{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GutterBottomColorBackground								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GutterBottomColorForeground								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GutterBottomNewDateColorForeground						{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public int		GutterBottomPadding										{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Font		GutterBottomFont										{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public string	GutterBottomDateFormatDayOpener							{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public string	GutterBottomDateFormatIntraday							{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public string	GutterBottomDateFormatDaily								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public string	GutterBottomDateFormatWeekly							{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public string	GutterBottomDateFormatYearly							{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GridlinesHorizontalColor								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GridlinesVerticalColor									{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	GridlinesVerticalNewDateColor							{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public bool		GridlinesHorizontalShow									{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public bool		GridlinesVerticalShow									{ get; set; }



		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		ScrollSqueezeMouseDragSensitivityPx						{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		ScrollNBarsPerOneDragMouseEvent							{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		ScrollNBarsPerOneKeyPress								{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		SqueezeVerticalPaddingPx								{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		SqueezeVerticalPaddingStep								{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		SqueezeHorizontalStep									{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		SqueezeHorizontalMouse1pxDistanceReceivedToOneStep		{ get; set; }

		[Category("3. Scroll and Squeeze"), Description("description to be composed")]
		[JsonProperty]	public int		SqueezeHorizontalKeyOnePressReceivedToOneStep			{ get; set; }



		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public bool		TooltipPriceShow										{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public bool		TooltipPriceShowOnlyWhenMouseTouchesCandle				{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public bool		TooltipPositionShow										{ get; set; }

		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public int		TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave	{ get; set; }



		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionArrowPaddingVertical							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		ScrollPositionAtBarIndex								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		TooltipBordersMarginToKeepBordersVisible				{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	PositionPlannedEllipseColor								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionPlannedEllipseColorAlpha						{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionPlannedEllipseDiameter							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	PositionFilledDotColor									{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionFilledDotColorAlpha								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionFilledDotDiameter								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionLineHighlightedWidth							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionLineHighlightedAlpha							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	PositionLineNoExitYetColor								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionLineNoExitYetColorAlpha							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	PositionLineProfitableColor								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionLineProfitableColorAlpha						{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	PositionLineLossyColor									{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		PositionLineLossyColorAlpha								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int 		PriceVsVolumeSplitterDistance							{ get; set; }




		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	AlertPendingEllipseColor								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingEllipseColorAlpha							{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingEllipsePenWidth								{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingEllipseRadius								{ get; set; }




		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	AlertPendingProtoTakeProfitEllipseColor					{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingProtoTakeProfitEllipseColorAlpha			{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingProtoTakeProfitEllipsePenWidth				{ get; set; }
		


		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public Color	AlertPendingProtoStopLossEllipseColor					{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingProtoStopLossEllipseColorAlpha				{ get; set; }

		[Category("4. Alerts and Positions"), Description("description to be composed")]
		[JsonProperty]	public int		AlertPendingProtoStopLossEllipsePenWidth				{ get; set; }




		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public bool		MousePositionTrackOnGutters								{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	MousePositionTrackOnGuttersColorBackground				{ get; set; }

		[Category("2. Grids and Gutters"), Description("description to be composed")]
		[JsonProperty]	public Color	MousePositionTrackOnGuttersColorForeground				{ get; set; }


		[Category("1. Essential"), Description("description to be composed")]
		[JsonProperty]	public int		BarsBackgroundTransparencyAlpha							{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		ChartLabelsUpperLeftYstartTopmost						{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		ChartLabelsUpperLeftX									{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		ChartLabelsUpperLeftPlatePadding						{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquarePadding				{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		ChartLabelsUpperLeftIndicatorSquareSize					{ get; set; }

		[Category("5. Annotations and Labels"), Description("description to be composed")]
		[JsonProperty]	public int		OnChartBarAnnotationsVerticalAwayFromPositionArrows		{ get; set; }




		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public Color	SpreadBidLineColor										{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public int		SpreadBidLineColorAlpha									{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public int		SpreadBidLineWidth										{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public Color	SpreadAskLineColor										{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public int		SpreadAskLineColorAlpha									{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public int		SpreadAskLineWidth										{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public Font		SpreadLabelFont											{ get; set; }

		[Category("6. Spread"), Description("description to be composed")]
		[JsonProperty]	public Color	SpreadLabelColor										{ get; set; }




		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoColorBackgroundStreamingHasNoLastQuote			{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoColorBackground									{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoLotsColorBackground								{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoLotsColorForeground								{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoAskColorBackground								{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoAskColorContour									{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoBidColorBackground								{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoBidColorContour									{ get; set; }




		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public int		LevelTwoMinimumPriceLevelThicknessRendered				{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public bool		LevelTwoLotDrawString									{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Font		LevelTwoLotFont											{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public Color	LevelTwoLotColor										{ get; set; }

		[Category("7. Level 2"), Description("description to be composed")]
		[JsonProperty]	public int		LevelTwoLotPaddingHorizontal							{ get; set; }

		[Category("7. Level 2"), Description("proportional => use PricePanel's pricelevel height (squeeze-able); unproportional => fit the volume with LevelTwoFont into the stripe")]
		[JsonProperty]	public	bool	LevelTwoStripesHeightWrapsVolumeLabel							{ get; set; }



		// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
		[Browsable(false)]
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterRowsPropertiesByPanelName;
		[Browsable(false)]
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterColumnsPropertiesByPanelName;
		
		// DONE_IN_RenderBarsPrice_KISS cache them all until user edits this.BarTotalWidthPx so they won't be calculated again with the same result for each bar
		[Browsable(false)]
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
		[Browsable(false)]
		[JsonIgnore]	public int BarWidthMinusRightPadding { get { return this.BarWidthIncludingPadding - this.BarPaddingRight; } }
		[Browsable(false)]
		[JsonIgnore]	public int BarShadowXoffset { get { return this.BarWidthMinusRightPadding / 2; } }
		
		//[Browsable(false)]
		//[JsonIgnore]	SolidBrush brushBackground;
		//[Browsable(false)]
		//[JsonIgnore]	public SolidBrush BrushBackground { get {
		//		if (this.brushBackground == null) this.brushBackground = new SolidBrush(this.ChartColorBackground);
		//		return this.brushBackground;
		//	} }

		//[Browsable(false)]
		//[JsonIgnore]	SolidBrush brushBackgroundReversed;
		//[Browsable(false)]
		//[JsonIgnore]	public SolidBrush BrushBackgroundReversed { get {
		//		if (this.brushBackgroundReversed == null) this.brushBackgroundReversed = new SolidBrush(ColorReverse(this.ChartColorBackground));
		//		return this.brushBackgroundReversed;
		//	} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushGutterRightBackground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterRightBackground { get {
				if (this.brushGutterRightBackground == null) this.brushGutterRightBackground = new SolidBrush(this.GutterRightColorBackground);
				return this.brushGutterRightBackground;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushGutterRightForeground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterRightForeground { get {
				if (this.brushGutterRightForeground == null) this.brushGutterRightForeground = new SolidBrush(this.GutterRightColorForeground);
				return this.brushGutterRightForeground;
			} }


		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushGutterBottomBackground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomBackground { get {
				if (this.brushGutterBottomBackground == null) this.brushGutterBottomBackground = new SolidBrush(this.GutterBottomColorBackground);
				return this.brushGutterBottomBackground;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushGutterBottomForeground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomForeground { get {
				if (this.brushGutterBottomForeground == null) this.brushGutterBottomForeground = new SolidBrush(this.GutterBottomColorForeground);
				return this.brushGutterBottomForeground;
			} }


		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushGutterBottomNewDateForeground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushGutterBottomNewDateForeground { get {
				if (this.brushGutterBottomNewDateForeground == null) this.brushGutterBottomNewDateForeground = new SolidBrush(this.GutterBottomNewDateColorForeground);
				return this.brushGutterBottomNewDateForeground;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushPriceBarUp;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushPriceBarUp { get {
				if (this.brushPriceBarUp == null) this.brushPriceBarUp = 
					new SolidBrush(this.PriceColorBarUp);
				return this.brushPriceBarUp;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penPriceBarUp;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPriceBarUp { get {
				if (this.penPriceBarUp == null) this.penPriceBarUp = 
					new Pen(this.PriceColorBarUp);
				return this.penPriceBarUp;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushPriceBarDown;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushPriceBarDown { get {
				if (this.brushPriceBarDown == null) this.brushPriceBarDown = 
					new SolidBrush(this.PriceColorBarDown);
				return this.brushPriceBarDown;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penPriceBarDown;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPriceBarDown { get {
				if (this.penPriceBarDown == null) this.penPriceBarDown = 
					new Pen(this.PriceColorBarDown);
				return this.penPriceBarDown;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushVolumeBarUp;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushVolumeBarUp { get {
				if (this.brushVolumeBarUp == null) this.brushVolumeBarUp = 
					new SolidBrush(this.VolumeColorBarUp);
				return this.brushVolumeBarUp;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushVolumeBarDown;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushVolumeBarDown { get {
				if (this.brushVolumeBarDown == null) this.brushVolumeBarDown = 
					new SolidBrush(this.VolumeColorBarDown);
				return this.brushVolumeBarDown;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penGridlinesHorizontal;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesHorizontal { get {
				if (this.penGridlinesHorizontal == null) this.penGridlinesHorizontal = 
					new Pen(this.GridlinesHorizontalColor);
				return this.penGridlinesHorizontal;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penGridlinesVertical;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesVertical { get {
				if (this.penGridlinesVertical == null) this.penGridlinesVertical = 
					new Pen(this.GridlinesVerticalColor);
				return this.penGridlinesVertical;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penGridlinesVerticalNewDate;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenGridlinesVerticalNewDate { get {
				if (this.penGridlinesVerticalNewDate == null) this.penGridlinesVerticalNewDate = 
					new Pen(this.GridlinesVerticalNewDateColor);
				return this.penGridlinesVerticalNewDate;
			} }

		
		[Browsable(false)]
		[JsonIgnore]	Pen penPositionPlannedEllipse;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionPlannedEllipse { get {
				if (this.penPositionPlannedEllipse == null) this.penPositionPlannedEllipse =
					new Pen(Color.FromArgb(this.PositionPlannedEllipseColorAlpha, this.PositionPlannedEllipseColor), this.PositionPlannedEllipseDiameter);
				return this.penPositionPlannedEllipse;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penPositionFilledDot;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionFilledDot { get {
				if (this.penPositionFilledDot == null) this.penPositionFilledDot =
					new Pen(Color.FromArgb(this.PositionFilledDotColorAlpha, this.PositionFilledDotColor));
				return this.penPositionFilledDot;
			} }

		[Browsable(false)]
		[JsonIgnore]	Brush brushPositionFilledDot;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushPositionFilledDot { get {
				if (this.brushPositionFilledDot == null) this.brushPositionFilledDot = 
					new SolidBrush(Color.FromArgb(this.PositionFilledDotColorAlpha, this.PositionFilledDotColor));
				return this.brushPositionFilledDot;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penPositionLineEntryExitConnectedUnknown;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedUnknown { get {
				if (this.penPositionLineEntryExitConnectedUnknown == null) this.penPositionLineEntryExitConnectedUnknown =
					new Pen(Color.FromArgb(this.PositionLineNoExitYetColorAlpha, this.PositionLineNoExitYetColor));
				return this.penPositionLineEntryExitConnectedUnknown;
			} }
		[Browsable(false)]
		[JsonIgnore]	Pen penPositionLineEntryExitConnectedProfit;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedProfit { get {
				if (this.penPositionLineEntryExitConnectedProfit == null) this.penPositionLineEntryExitConnectedProfit =
					new Pen(Color.FromArgb(this.PositionLineProfitableColorAlpha, this.PositionLineProfitableColor));
				return this.penPositionLineEntryExitConnectedProfit;
			} }


		[Browsable(false)]
		[JsonIgnore]	Pen penPositionLineEntryExitConnectedLoss;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenPositionLineEntryExitConnectedLoss { get {
				if (this.penPositionLineEntryExitConnectedLoss == null) this.penPositionLineEntryExitConnectedLoss =
					new Pen(Color.FromArgb(this.PositionLineLossyColorAlpha, this.PositionLineLossyColor));
				return this.penPositionLineEntryExitConnectedLoss;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penAlertPendingEllipse;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenAlertPendingEllipse { get {
				if (this.penAlertPendingEllipse == null) this.penAlertPendingEllipse =
					new Pen(Color.FromArgb(this.AlertPendingEllipseColorAlpha, this.AlertPendingEllipseColor), this.AlertPendingEllipsePenWidth);
				return this.penAlertPendingEllipse;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penAlertPendingProtoTakeProfitEllipse;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenAlertPendingProtoTakeProfitEllipse { get {
				if (this.penAlertPendingProtoTakeProfitEllipse == null) this.penAlertPendingProtoTakeProfitEllipse =
					new Pen(Color.FromArgb(this.AlertPendingProtoTakeProfitEllipseColorAlpha, this.AlertPendingProtoTakeProfitEllipseColor), this.AlertPendingProtoTakeProfitEllipsePenWidth);
				return this.penAlertPendingProtoTakeProfitEllipse;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penAlertPendingProtoStopLossEllipse;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenAlertPendingProtoStopLossEllipse { get {
				if (this.penAlertPendingProtoStopLossEllipse == null) this.penAlertPendingProtoStopLossEllipse =
					new Pen(Color.FromArgb(this.AlertPendingProtoStopLossEllipseColorAlpha, this.AlertPendingProtoStopLossEllipseColor), this.AlertPendingProtoStopLossEllipsePenWidth);
				return this.penAlertPendingProtoStopLossEllipse;
			} }
			
		[Browsable(false)]
		[JsonIgnore]	Pen penMousePositionTrackOnGutters;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenMousePositionTrackOnGuttersForeground { get {
				if (this.penMousePositionTrackOnGutters == null) this.penMousePositionTrackOnGutters = 
					new Pen(this.MousePositionTrackOnGuttersColorForeground);
				return this.penMousePositionTrackOnGutters;
			} }

		[Browsable(false)]
		[JsonIgnore]	Brush brushMousePositionTrackOnGuttersBackground;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushMousePositionTrackOnGuttersInverted { get {
			if (this.brushMousePositionTrackOnGuttersBackground == null) this.brushMousePositionTrackOnGuttersBackground = 
					new SolidBrush(this.MousePositionTrackOnGuttersColorBackground);
				return this.brushMousePositionTrackOnGuttersBackground;
			} }


		[Browsable(false)]
		[JsonIgnore]	Pen penSpreadBid;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenSpreadBid { get {
				if (this.penSpreadBid == null) this.penSpreadBid =
					new Pen(Color.FromArgb(this.SpreadBidLineColorAlpha, this.SpreadBidLineColor), this.SpreadAskLineWidth);
				return this.penSpreadBid;
			} }

		[Browsable(false)]
		[JsonIgnore]	Brush brushSpreadBid;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushSpreadBid { get {
				if (this.brushSpreadBid == null) this.brushSpreadBid = new SolidBrush(this.SpreadBidLineColor);
				return this.brushSpreadBid;
			} }


		[Browsable(false)]
		[JsonIgnore]	Pen penSpreadAsk;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenSpreadAsk { get {
				if (this.penSpreadAsk == null) this.penSpreadAsk =
					new Pen(Color.FromArgb(this.SpreadAskLineColorAlpha, this.SpreadAskLineColor), this.SpreadAskLineWidth);
				return this.penSpreadAsk;
			} }

		[Browsable(false)]
		[JsonIgnore]	Brush brushSpreadAsk;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushSpreadAsk { get {
				if (this.brushSpreadAsk == null) this.brushSpreadAsk = new SolidBrush(this.SpreadAskLineColor);
				return this.brushSpreadAsk;
			} }


		[Browsable(false)]
		[JsonIgnore]	Brush brushSpreadLabel;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushSpreadLabel { get {
				if (this.brushSpreadLabel == null) this.brushSpreadLabel = new SolidBrush(this.SpreadLabelColor);
				return this.brushSpreadLabel;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushLevelTwoLotsColorBackground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoLotsColorBackground { get {
				if (this.brushLevelTwoLotsColorBackground == null) this.brushLevelTwoLotsColorBackground =
					new SolidBrush(this.LevelTwoLotsColorBackground);
				return this.brushLevelTwoLotsColorBackground;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushLevelTwoAskColorBackground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoAskColorBackground { get {
				if (this.brushLevelTwoAskColorBackground == null) this.brushLevelTwoAskColorBackground =
					new SolidBrush(this.LevelTwoAskColorBackground);
				return this.brushLevelTwoAskColorBackground;
			} }

		[Browsable(false)]
		[JsonIgnore]	SolidBrush brushLevelTwoBidColorBackground;
		[Browsable(false)]
		[JsonIgnore]	public SolidBrush BrushLevelTwoBidColorBackground { get {
				if (this.brushLevelTwoBidColorBackground == null) this.brushLevelTwoBidColorBackground =
					new SolidBrush(this.LevelTwoBidColorBackground);
				return this.brushLevelTwoBidColorBackground;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penLevelTwoAskColorContour;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenLevelTwoAskColorContour { get {
				if (this.penLevelTwoAskColorContour == null) this.penLevelTwoAskColorContour =
					new Pen(this.LevelTwoAskColorContour);
				return this.penLevelTwoAskColorContour;
			} }

		[Browsable(false)]
		[JsonIgnore]	Pen penLevelTwoBidColorContour;
		[Browsable(false)]
		[JsonIgnore]	public Pen PenLevelTwoBidColorContour { get {
				if (this.penLevelTwoBidColorContour == null) this.penLevelTwoBidColorContour =
					new Pen(this.LevelTwoBidColorContour);
				return this.penLevelTwoBidColorContour;
			} }

		[Browsable(false)]
		[JsonIgnore]	Brush brushLevelTwoLot;
		private string p;
		[Browsable(false)]
		[JsonIgnore]	public Brush BrushLevelTwoLot { get {
				if (this.brushLevelTwoLot == null) this.brushLevelTwoLot = new SolidBrush(this.LevelTwoLotColor);
				return this.brushLevelTwoLot;
			} }

	
		public ChartSettings() {
			ChartColorBackground = Color.White;
			BarWidthIncludingPadding = 8;
			BarWidthIncludingPaddingMax = 100;
			PanelNameAndSymbolFont = new Font("Microsoft Sans Serif", 8.25f);
			PriceColorBarUp = Color.RoyalBlue;
			PriceColorBarDown = Color.IndianRed;
			VolumeColorBarUp = Color.CadetBlue;
			VolumeColorBarDown = Color.CadetBlue;
			VolumeRightGutterColorForeground = Color.White;
			BarUpFillCandleBody = false;

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
			
			AlertPendingEllipseColor = Color.DarkBlue;
			AlertPendingEllipseColorAlpha = 180;
			AlertPendingEllipsePenWidth = 2;
			AlertPendingEllipseRadius = 3;

			AlertPendingProtoTakeProfitEllipseColor = Color.Green;
			AlertPendingProtoTakeProfitEllipseColorAlpha = 100;
			AlertPendingProtoTakeProfitEllipsePenWidth = 2;

			AlertPendingProtoStopLossEllipseColor = Color.Red;
			AlertPendingProtoStopLossEllipseColorAlpha = 100;
			AlertPendingProtoStopLossEllipsePenWidth = 2;

			MousePositionTrackOnGutters = true;
			MousePositionTrackOnGuttersColorBackground = Color.Black;
			MousePositionTrackOnGuttersColorForeground = Color.LightGray;
			BarsBackgroundTransparencyAlpha = 24;
			ChartLabelsUpperLeftYstartTopmost = 20;
			ChartLabelsUpperLeftX = 7;
			ChartLabelsUpperLeftPlatePadding = 1;
			ChartLabelsUpperLeftIndicatorSquarePadding = 4;
			ChartLabelsUpperLeftIndicatorSquareSize = 5;
			OnChartBarAnnotationsVerticalAwayFromPositionArrows = 3;

			// SplitterPositionsByManorder isn't a "Setting" but I don't want to add event into ChartShadow to save/restore this from ChartFormDataSnaptshot
			MultiSplitterRowsPropertiesByPanelName		= new Dictionary<string, MultiSplitterProperties>();
			MultiSplitterColumnsPropertiesByPanelName	= new Dictionary<string, MultiSplitterProperties>();

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
			LevelTwoStripesHeightWrapsVolumeLabel = true;

			base.Name = "UNASSIGNED";
		}

		public ChartSettings(string name) : this() {
			base.Name = name;
		}

		public static Color ColorReverse(Color color) {
			int red = 255 - color.R;
			int green = 255 - color.G;
			int blue = 255 - color.B;
			return Color.FromArgb((int)red, (int)green, (int)blue);
		}
		
		public void DisposeAllGDIs_handlesLeakHunter() {
			if (this.penAlertPendingEllipse						!= null) { this.penAlertPendingEllipse					.Dispose(); this.penAlertPendingEllipse						= null; }
			if (this.penAlertPendingProtoStopLossEllipse		!= null) { this.penAlertPendingProtoStopLossEllipse		.Dispose(); this.penAlertPendingProtoStopLossEllipse		= null; }
			if (this.penAlertPendingProtoTakeProfitEllipse		!= null) { this.penAlertPendingProtoTakeProfitEllipse	.Dispose(); this.penAlertPendingProtoTakeProfitEllipse		= null; }
			if (this.penGridlinesHorizontal						!= null) { this.penGridlinesHorizontal					.Dispose(); this.penGridlinesHorizontal						= null; }
			if (this.penGridlinesVertical						!= null) { this.penGridlinesVertical					.Dispose(); this.penGridlinesVertical						= null; }
			if (this.penGridlinesVerticalNewDate				!= null) { this.penGridlinesVerticalNewDate				.Dispose(); this.penGridlinesVerticalNewDate				= null; }
			if (this.penLevelTwoAskColorContour					!= null) { this.penLevelTwoAskColorContour				.Dispose(); this.penLevelTwoAskColorContour					= null; }
			if (this.penLevelTwoBidColorContour					!= null) { this.penLevelTwoBidColorContour				.Dispose(); this.penLevelTwoBidColorContour					= null; }
			if (this.penMousePositionTrackOnGutters				!= null) { this.penMousePositionTrackOnGutters			.Dispose(); this.penMousePositionTrackOnGutters				= null; }
			if (this.brushMousePositionTrackOnGuttersBackground	!= null) { this.brushMousePositionTrackOnGuttersBackground.Dispose(); this.brushMousePositionTrackOnGuttersBackground	= null; }
			if (this.penPositionFilledDot						!= null) { this.penPositionFilledDot					.Dispose(); this.penPositionFilledDot						= null; }
			if (this.penPositionLineEntryExitConnectedLoss		!= null) { this.penPositionLineEntryExitConnectedLoss	.Dispose(); this.penPositionLineEntryExitConnectedLoss		= null; }
			if (this.penPositionLineEntryExitConnectedProfit	!= null) { this.penPositionLineEntryExitConnectedProfit	.Dispose(); this.penPositionLineEntryExitConnectedProfit	= null; }
			if (this.penPositionLineEntryExitConnectedUnknown	!= null) { this.penPositionLineEntryExitConnectedUnknown.Dispose(); this.penPositionLineEntryExitConnectedUnknown	= null; }
			if (this.penPositionPlannedEllipse					!= null) { this.penPositionPlannedEllipse				.Dispose(); this.penPositionPlannedEllipse					= null; }
			if (this.penPriceBarDown							!= null) { this.penPriceBarDown							.Dispose(); this.penPriceBarDown							= null; }
			if (this.penPriceBarUp								!= null) { this.penPriceBarUp							.Dispose(); this.penPriceBarUp								= null; }
			if (this.penSpreadAsk								!= null) { this.penSpreadAsk							.Dispose(); this.penSpreadAsk								= null; }
			if (this.brushSpreadAsk								!= null) { this.brushSpreadAsk							.Dispose(); this.brushSpreadAsk								= null; }
			if (this.penSpreadBid								!= null) { this.penSpreadBid							.Dispose(); this.penSpreadBid								= null; }
			if (this.brushSpreadBid								!= null) { this.brushSpreadBid							.Dispose(); this.brushSpreadBid								= null; }
			if (this.brushGutterBottomBackground				!= null) { this.brushGutterBottomBackground				.Dispose(); this.brushGutterBottomBackground				= null; }
			if (this.brushGutterBottomForeground				!= null) { this.brushGutterBottomForeground				.Dispose(); this.brushGutterBottomForeground				= null; }
			if (this.brushGutterBottomNewDateForeground			!= null) { this.brushGutterBottomNewDateForeground		.Dispose(); this.brushGutterBottomNewDateForeground			= null; }
			if (this.brushGutterRightBackground					!= null) { this.brushGutterRightBackground				.Dispose(); this.brushGutterRightBackground					= null; }
			if (this.brushGutterRightForeground					!= null) { this.brushGutterRightForeground				.Dispose(); this.brushGutterRightForeground					= null; }
			if (this.brushLevelTwoAskColorBackground			!= null) { this.brushLevelTwoAskColorBackground			.Dispose(); this.brushLevelTwoAskColorBackground			= null; }
			if (this.brushLevelTwoBidColorBackground			!= null) { this.brushLevelTwoBidColorBackground			.Dispose(); this.brushLevelTwoBidColorBackground			= null; }
			if (this.brushLevelTwoLot							!= null) { this.brushLevelTwoLot						.Dispose(); this.brushLevelTwoLot							= null; }
			if (this.brushLevelTwoLotsColorBackground			!= null) { this.brushLevelTwoLotsColorBackground		.Dispose(); this.brushLevelTwoLotsColorBackground			= null; }
			if (this.brushPositionFilledDot						!= null) { this.brushPositionFilledDot					.Dispose(); this.brushPositionFilledDot						= null; }
			if (this.brushPriceBarDown							!= null) { this.brushPriceBarDown						.Dispose(); this.brushPriceBarDown							= null; }
			if (this.brushPriceBarUp							!= null) { this.brushPriceBarUp							.Dispose(); this.brushPriceBarUp							= null; }
			if (this.brushSpreadLabel							!= null) { this.brushSpreadLabel						.Dispose(); this.brushSpreadLabel							= null; }
			if (this.brushVolumeBarDown							!= null) { this.brushVolumeBarDown						.Dispose(); this.brushVolumeBarDown							= null; }
			if (this.brushVolumeBarUp							!= null) { this.brushVolumeBarUp						.Dispose(); this.brushVolumeBarUp							= null; }
		}

		public void PensAndBrushesCached_DisposeAndNullify() {
			this.DisposeAllGDIs_handlesLeakHunter();
		}

		public override string ToString() {
			return this.StrategyName;
		}

		public virtual ChartSettings Clone() {
			return (ChartSettings)base.MemberwiseClone();
		}

		public void AbsorbFrom(ChartSettings tpl) {
			PropertyInfo[] props = this.GetType().GetProperties();
			foreach (PropertyInfo prop in props) {
				bool jsonPropertyFound = false;
				bool browseable = true;
				//foreach (Attribute attr in prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true)) {
				foreach (Attribute attr in ((MemberInfo)prop).GetCustomAttributes(true)) {
					if (attr == null) continue;

					BrowsableAttribute attrAsBrowsable = attr as BrowsableAttribute;
					if (attrAsBrowsable != null) {
						if (attrAsBrowsable.Browsable) continue;
						browseable = false;
						continue;
					}

					JsonPropertyAttribute attrAsJsonProperty = attr as JsonPropertyAttribute;
					if (attrAsJsonProperty != null) {
						jsonPropertyFound = true;
					}
				}
				if (jsonPropertyFound == false || browseable == false) continue;

				PropertyInfo  tplProperty =  tpl.GetType().GetProperty(prop.Name);
				object tplValue = prop.GetValue(tpl, null);
				prop.SetValue(this, tplValue, null);
			}
			this.Name = tpl.Name;
		}
	}
}