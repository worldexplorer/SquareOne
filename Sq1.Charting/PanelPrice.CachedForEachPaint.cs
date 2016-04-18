using System;
using System.ComponentModel;

namespace Sq1.Charting {
	public partial class PanelPrice {

		//public double	PriceRangeShown_cached					{ get { return this.VisibleMinDoubleMaxValueUnsafe - this.VisibleMaxDoubleMinValueUnsafe; } }
		[Browsable(false)]	public double	PriceRangeShownPlusSqueezers_cached		{ get { return base.VisibleMaxPlusBottomSqueezer_cached - base.VisibleMinMinusTopSqueezer_cached; } }
		[Browsable(false)]	public double	PriceLevelsShown_cached					{ get { return this.PriceRangeShownPlusSqueezers_cached / this.PriceStep; } }
		[Browsable(false)]	public double	PixelsPerPriceStep_cached				{ get { return base.Height / this.PriceLevelsShown_cached; } }
		[Browsable(false)]	public int		PixelsPerPriceStep5pxLeast_cached		{ get {
				int minimumPriceLevelThicknessRendered = this.ChartControl.ChartSettingsTemplated.LevelTwoMinimumPriceLevelThicknessRendered;
				int ret = minimumPriceLevelThicknessRendered;
				if (double.IsInfinity(this.PixelsPerPriceStep_cached) == false) {
					double rounded = (int)Math.Round(this.PixelsPerPriceStep_cached);
					ret = (int)rounded;
				}
				if (ret < minimumPriceLevelThicknessRendered) ret = minimumPriceLevelThicknessRendered;
				return ret;
			} }
	}
}
