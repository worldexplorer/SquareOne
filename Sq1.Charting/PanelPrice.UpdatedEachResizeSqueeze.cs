using System;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public partial class PanelPrice {
		
		#if USE_DATASERIES_MINMAX
		public override double VisibleMinDoubleMaxValueUnsafe { get { return this.VisiblePriceMinNew; } }
		public override double VisibleMaxDoubleMinValueUnsafe { get { return this.VisiblePriceMaxNew; } }
		
		public double VisiblePriceMinNew { get {
				if (base.DesignMode || base.ChartControl.BarsEmpty) return 99;
				DataSeriesProxyBars seriesLow = new DataSeriesProxyBars(base.ChartControl.Bars, DataSeriesProxyableFromBars.Low);
				double ret = seriesLow.MinValueBetweenIndexesDoubleMaxValueUnsafe(this.VisibleBarLeft_cached, this.VisibleBarRight_cached);
				if (this.VisibleBarRight_cached >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisiblePriceMin(): " + msg);
				}
				return ret;
			} }
		public double VisiblePriceMaxNew { get {
				if (base.DesignMode || base.ChartControl.BarsEmpty) return 999;
				DataSeriesProxyBars seriesHigh = new DataSeriesProxyBars(base.ChartControl.Bars, DataSeriesProxyableFromBars.High);
				double ret = seriesHigh.MaxValueBetweenIndexesDoubleMinValueUnsafe(this.VisibleBarLeft_cached, this.VisibleBarRight_cached);
				if (this.VisibleBarRight_cached >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisiblePriceMax(): " + msg);
				}
				return ret;
			} }
		#else
		public override double VisibleMinDoubleMaxValueUnsafe { get { return this.VisiblePriceMinOld; } }
		public override double VisibleMaxDoubleMinValueUnsafe { get { return this.VisiblePriceMaxOld; } }
				
		#region DELETEME_AFTER_COMPATIBILITY_TEST
		private double visiblePriceMaxCurrent;
		public double VisiblePriceMaxOld { get {
				if (base.DesignMode || base.ChartControl.BarsEmpty) return 999;
				this.visiblePriceMaxCurrent = Double.MinValue;
				for (int i = this.VisibleBarRight_cached; i >= this.VisibleBarLeft_cached; i--) {
					if (i >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisiblePriceMax(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = base.ChartControl.Bars[i];
					double high = barCanBeStreamingWithNaNs.High;
					if (double.IsNaN(high)) continue;
					if (high > this.visiblePriceMaxCurrent) this.visiblePriceMaxCurrent = high;
				}
				#if TEST_COMPATIBILITY
				if (this.visiblePriceMaxCurrent != this.VisiblePriceMaxNew) {
					#if DEBUG
					Debugger.Break();
					#endif
				} else {
					#if DEBUG
					//Debugger.Break();
					#endif
				}
				#endif
				return this.visiblePriceMaxCurrent;
			} }
		private double visiblePriceMinCurrent;
		public double VisiblePriceMinOld { get {
				if (base.DesignMode || base.ChartControl.BarsEmpty) return 98;
				this.visiblePriceMinCurrent = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				if (this.VisibleBarLeft_cached > this.VisibleBarRight_cached) {
					string msg = "PAINTING_ZERO_OR_FIRST_BAR_OF_LIVESIMULATION__CONITNUE_UNTIL_IT_WILL_GET_NORMALIZED_SOON";
					//Assembler.PopupException(msg);
					return this.visiblePriceMinCurrent;
				}
				for (int i = this.VisibleBarRight_cached; i >= this.VisibleBarLeft_cached; i--) {
					if (i >= base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisiblePriceMin(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = base.ChartControl.Bars[i];
					double low = barCanBeStreamingWithNaNs.Low;
					if (double.IsNaN(low)) continue;
					if (low < this.visiblePriceMinCurrent) this.visiblePriceMinCurrent = low;
				}
				#if TEST_COMPATIBILITY
				if (this.visiblePriceMinCurrent != this.VisiblePriceMinNew) {
					#if DEBUG
					Debugger.Break();
					#endif
				} else {
					#if DEBUG
					//Debugger.Break();
					#endif
				}
				#endif
				if (Math.Abs(this.visiblePriceMinCurrent) > 1000000) {
					string msg = "this.VisibleBarLeft_cached > this.VisibleBarRight_cached ?";
					Assembler.PopupException(msg);
				}
				if (double.IsPositiveInfinity(this.visiblePriceMinCurrent)) {
					string msg = "PAINTING_ZERO_OR_FIRST_BAR_OF_LIVESIMULATION__CONITNUE_UNTIL_IT_WILL_GET_NORMALIZED_SOON";
					Assembler.PopupException(msg);
				}

				return this.visiblePriceMinCurrent;
			} }
		#endregion
		#endif



		
		private int howManyPositionArrowsBeyondPriceBoundaries = 0; 
		public override int PaddingVerticalSqueeze { get { return base.ChartControl.ChartSettings.SqueezeVerticalPaddingPx; } }

				
	}
}
