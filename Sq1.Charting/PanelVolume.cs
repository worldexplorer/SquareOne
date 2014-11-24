using System;
using System.Diagnostics;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public class PanelVolume : PanelBase {
		public PanelVolume() : base() {
			base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
			base.MinimumSize = new Size(20, 15);	// only height matters for MultiSplitContainer
		}
		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			if (base.PanelHasValuesForVisibleBarWindow == false) {
				Debugger.Break();
				return;
			}
				
			// 1) uses here-defined VisibleMinDoubleMaxValueUnsafe,VisibleMaxDoubleMinValueUnsafe to set:
			//		base.VisibleMin,Max,Range_cached,
			//		base.VisibleMinMinusTopSqueezer_cached, this.VisibleMaxPlusBottomSqueezer_cached, this.VisibleRangeWithTwoSqueezers_cached
			// 2) paints Right and Bottom gutter foregrounds;
			base.PaintWholeSurfaceBarsNotEmpty(g);
			base.ForeColor = this.ChartControl.ChartSettings.VolumeColorBarDown;
			this.renderBarsVolume(g);
		}
		//protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
		//	base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
		//	//this.PaintRightVolumeGutterAndGridLines(g);
		//}
		
		#if USE_DATASERIES_MINMAX
		public override double VisibleMinDoubleMaxValueUnsafe { get { return this.VisibleVolumeMinNew; } }
		public override double VisibleMaxDoubleMinValueUnsafe { get { return this.VisibleVolumeMaxNew; } }
		#else
		public override double VisibleMinDoubleMaxValueUnsafe { get { return this.VisibleVolumeMinOld; } }
		public override double VisibleMaxDoubleMinValueUnsafe { get { return this.VisibleVolumeMaxOld; } }
		#endif
		
		#region DELETEME_AFTER_COMPATIBILITY_TEST
		private double visibleVolumeMinCurrent;
		public double VisibleVolumeMinOld { get {
				if (base.DesignMode || this.ChartControl.BarsEmpty) return 99;
				this.visibleVolumeMinCurrent = Double.MaxValue;
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisibleVolumeMin(): " + msg);
					return this.visibleVolumeMaxCurrent;
				}
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				for (int i = this.VisibleBarRight_cached; i >= this.VisibleBarLeft_cached; i--) {
					Bar barCanBeStreamingWithNaNs = this.ChartControl.Bars[i];
					double volume = barCanBeStreamingWithNaNs.Volume;
					if (double.IsNaN(volume)) continue;
					if (volume < this.visibleVolumeMinCurrent) this.visibleVolumeMinCurrent = volume;
				}
				#if TEST_COMPATIBILITY
				if (this.visibleVolumeMinCurrent != this.VisibleVolumeMinNew) {
					Debugger.Break();
				} else {
					//Debugger.Break();
				}
				#endif
				return this.visibleVolumeMinCurrent;
			} }
		private double visibleVolumeMaxCurrent;
		public double VisibleVolumeMaxOld { get {
				if (base.DesignMode || this.ChartControl.BarsEmpty) return 658;
				this.visibleVolumeMaxCurrent = Double.MinValue;
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisibleVolumeMax(): " + msg);
					return this.visibleVolumeMaxCurrent;
				}

				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				for (int i = this.VisibleBarRight_cached; i >= this.VisibleBarLeft_cached; i--) {
					Bar barCanBeStreamingWithNaNs = this.ChartControl.Bars[i];
					double volume = barCanBeStreamingWithNaNs.Volume;
					if (double.IsNaN(volume)) continue;
					if (volume > this.visibleVolumeMaxCurrent) this.visibleVolumeMaxCurrent = volume;
				}
				#if TEST_COMPATIBILITY
				if (this.visibleVolumeMaxCurrent != this.VisibleVolumeMaxNew) {
					Debugger.Break();
				} else {
					//Debugger.Break();
				}
				#endif
				return this.visibleVolumeMaxCurrent;
			} }
		#endregion
		
		public double VisibleVolumeMinNew { get {
				if (base.DesignMode || this.ChartControl.BarsEmpty) return 99;
				DataSeriesProxyBars seriesVolume = new DataSeriesProxyBars(this.ChartControl.Bars, DataSeriesProxyableFromBars.Volume);
				double ret = seriesVolume.MinValueBetweenIndexesDoubleMaxValueUnsafe(this.VisibleBarLeft_cached, this.VisibleBarRight_cached);
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisibleVolumeMin(): " + msg);
				}
				return ret;
			} }
		public double VisibleVolumeMaxNew { get {
				if (base.DesignMode || this.ChartControl.BarsEmpty) return 658;
				DataSeriesProxyBars seriesVolume = new DataSeriesProxyBars(this.ChartControl.Bars, DataSeriesProxyableFromBars.Volume);
				double ret = seriesVolume.MaxValueBetweenIndexesDoubleMinValueUnsafe(this.VisibleBarLeft_cached, this.VisibleBarRight_cached);
				if (this.VisibleBarRight_cached >= this.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisibleVolumeMax(): " + msg);
				}
				return ret;
			} }
		
		
		void renderBarsVolume(Graphics g) {
			//for (int i = 0; i < base.ChartControl.BarsCanFitForCurrentWidth; i++) {
			//	int barFromRight = base.ChartControl.BarsCanFitForCurrentWidth - i - 1;
			//int barX = this.BarTotalWidthPx_localCache * barFromRight;
			int barX = base.ChartControl.ChartWidthMinusGutterRightPrice;
			for (int i = base.VisibleBarRight_cached; i > base.VisibleBarLeft_cached; i--) {
				if (i > base.ChartControl.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("RenderBarsPrice(): " + msg);
					continue;
				}
				barX -= base.BarWidthIncludingPadding_cached;
				Bar bar = base.ChartControl.Bars[i];
				//if (bar.IsStreamingBar) {	// NaNs are possible for all bars, not only for streaming bar
					if (double.IsNaN(bar.Open)) continue;
					if (double.IsNaN(bar.Close)) continue;
					if (double.IsNaN(bar.Volume)) continue;
				//}
				int barYVolume = base.ValueToYinverted(bar.Volume);
				bool fillCandleBody = (bar.Open > bar.Close) ? true : false;
				base.RenderBarHistogram(g, barX, barYVolume, fillCandleBody);
			}
		}
		
		public override double ValueGetNaNunsafe(int barIndex) {
			if (barIndex < 0) return double.NaN;
			if (barIndex >= base.ChartControl.Bars.Count) return double.NaN; 
			Bar bar = base.ChartControl.Bars[barIndex];
			return bar.Volume;
		}
		public override int Decimals { get { return (base.ChartControl.Bars.SymbolInfo != null) ? base.ChartControl.Bars.SymbolInfo.DecimalsVolume : 0; } }
		public override int ValueIndexLastAvailableMinusOneUnsafe { get { return base.ChartControl.Bars.Count - 1; } }
	}
}