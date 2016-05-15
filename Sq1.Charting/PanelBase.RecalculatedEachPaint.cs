using System;


namespace Sq1.Charting {
	public partial class PanelBase {

		#region cached = recalculated each PaintWholeSurfaceBarsNotEmpty()
							protected		int		VisibleBarRight_cached;
							protected		int		VisibleBarLeft_cached;
							protected		int		VisibleBarsCount_cached;

							protected		double	VisibleMin_cached;
							protected		double	VisibleMax_cached;
							protected		double	VisibleRange_cached;
		
							protected		double	VisibleMinMinusTopSqueezer_cached;
							public			double	VisibleMaxPlusBottomSqueezer_cached;	//public allows PanelLevel2 read it from PanelPrice
							protected		double	VisibleRangeWithTwoSqueezers_cached;
		
		
							protected		int		BarWidth_includingPadding_cached;
							protected		int		BarWidthMinusRightPadding_cached;
							protected		int		BarShadowXoffset_cached;
							protected		int		PanelHeight_minusGutterBottomHeight_cached;
		
	 						protected		int		GutterRightFontHeight_cached = -1;
	 						protected		int		GutterRightFontHeightHalf_cached = -1;
	 						protected		int		GutterBottomFontHeight_cached = -1;
	 						protected		int		GutterBottomHeight_cached = -1;
		#endregion
	}
}