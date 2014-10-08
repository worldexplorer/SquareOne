using System;
using System.Diagnostics;
using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Charting {
	public partial class ChartControl  {
		// cache them all until base.Width/Height changes so they won't be calculated again with the same result for each bar
		public int ChartWidthMinusGutterRightPrice { get {
				if (base.Width <= 0) return 0;	// HAPPENS_WHEN_WINDOW_IS_MINIMIZED... how to disable any OnPaint when app isn't visible?... 
				return base.Width - this.GutterRightWidth_cached; } }
		public int BarsCanFitForCurrentWidth { get { return this.ChartWidthMinusGutterRightPrice / this.ChartSettings.BarWidthIncludingPadding; } }
		public int ScrollLargeChange { get { return (int)(this.BarsCanFitForCurrentWidth * 0.9f); } }
		public int VisibleBarRight { get {
				//v1 return this.hScrollBar.Value;
				int ret = this.hScrollBar.Value;
				//v2
				// http://stackoverflow.com/questions/2882789/net-vertical-scrollbar-not-respecting-maximum-property
				float physicalMax = this.hScrollBar.Maximum - this.hScrollBar.LargeChange + 1;
				if (physicalMax < 0) {
					#if DEBUG
					Debugger.Break();
					#endif
					return ret;
				}
				float part0to1 = this.hScrollBar.Value / physicalMax;
				if (part0to1 > 1) part0to1 = 1;	//	NONSENSE: this.hScrollBar.Value=432, physicalMax=423 - am I still resizing?...
				ret = (int)(part0to1 * (this.Bars.Count - 1));
				return ret;
			} }
		public int VisibleBarLeft { get {
				int ret = this.VisibleBarRight - this.BarsCanFitForCurrentWidth;				
				if (ret < 0) {
					string msg = "RESIZING_WINDOW_WHEN_ON_FIRST_LEFT_BAR EXTENDING_SO_THAT_ONE_MORE_BAR_CAN_FIT";
					//Debugger.Break();
					ret = 0;
				}
				if (this.hScrollBar.Value == this.hScrollBar.Minimum && ret > 1) {
					string msg = "VisibleBarLeft must be zero at ScrollBar.Minimum, doesn't it?...";
					//Debugger.Break();
				}
				return ret;
			} }
		
		#if USE_DATASERIES_MINMAX
		public double VisiblePriceMin { get { return this.VisiblePriceMinNew; } }
		public double VisiblePriceMax { get { return this.VisiblePriceMaxNew; } }
		public double VisibleVolumeMin { get { return this.VisibleVolumeMinNew; } }
		public double VisibleVolumeMax { get { return this.VisibleVolumeMaxNew; } }
		#else
		public double VisiblePriceMin { get { return this.VisiblePriceMinOld; } }
		public double VisiblePriceMax { get { return this.VisiblePriceMaxOld; } }
		public double VisibleVolumeMin { get { return this.VisibleVolumeMinOld; } }
		public double VisibleVolumeMax { get { return this.VisibleVolumeMaxOld; } }
		#endif
		
		#region DELETEME_AFTER_COMPATIBILITY_TEST
		private double visiblePriceMaxCurrent;
		public double VisiblePriceMaxOld { get {
				if (base.DesignMode || this.BarsEmpty) return 999;
				this.visiblePriceMaxCurrent = Double.MinValue;
				for (int i = this.VisibleBarRight; i >= this.VisibleBarLeft; i--) {
					if (i >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisiblePriceMax(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = this.Bars[i];
					double high = barCanBeStreamingWithNaNs.High;
					if (double.IsNaN(high)) continue;
					if (high > this.visiblePriceMaxCurrent) this.visiblePriceMaxCurrent = high;
				}
				#if TEST_COMPATIBILITY
				if (this.visiblePriceMaxCurrent != this.VisiblePriceMaxNew) {
					Debugger.Break();
				} else {
					//Debugger.Break();
				}
				#endif
				return this.visiblePriceMaxCurrent;
			} }
		private double visiblePriceMinCurrent;
		public double VisiblePriceMinOld { get {
				if (base.DesignMode || this.BarsEmpty) return 98;
				this.visiblePriceMinCurrent = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				for (int i = this.VisibleBarRight; i >= this.VisibleBarLeft; i--) {
					if (i >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisiblePriceMin(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = this.Bars[i];
					double low = barCanBeStreamingWithNaNs.Low;
					if (double.IsNaN(low)) continue;
					if (low < this.visiblePriceMinCurrent) this.visiblePriceMinCurrent = low;
				}
				#if TEST_COMPATIBILITY
				if (this.visiblePriceMinCurrent != this.VisiblePriceMinNew) {
					Debugger.Break();
				} else {
					//Debugger.Break();
				}
				#endif
				if (Math.Abs(this.visiblePriceMinCurrent) > 1000000) {
					Debugger.Break();
				}
				return this.visiblePriceMinCurrent;
			} }
		private double visibleVolumeMinCurrent;
		public double VisibleVolumeMinOld { get {
				if (base.DesignMode || this.BarsEmpty) return 99;
				this.visibleVolumeMinCurrent = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				for (int i = this.VisibleBarRight; i >= this.VisibleBarLeft; i--) {
					if (i >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisibleVolumeMin(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = this.Bars[i];
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
				if (base.DesignMode || this.BarsEmpty) return 658;
				this.visibleVolumeMaxCurrent = Double.MinValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				for (int i = this.VisibleBarRight; i >= this.VisibleBarLeft; i--) {
					if (i >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisibleVolumeMax(): " + msg);
						continue;
					}
					Bar barCanBeStreamingWithNaNs = this.Bars[i];
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
		
		public double VisiblePriceMinNew { get {
				if (base.DesignMode || this.BarsEmpty) return 99;
				DataSeriesProxyBars seriesLow = new DataSeriesProxyBars(this.Bars, DataSeriesProxyableFromBars.Low);
				double ret = seriesLow.MinValueBetweenIndexesDoubleMaxValueUnsafe(this.VisibleBarLeft, this.VisibleBarRight);
				if (this.VisibleBarRight >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisiblePriceMin(): " + msg);
				}
				return ret;
			} }
		public double VisiblePriceMaxNew { get {
				if (base.DesignMode || this.BarsEmpty) return 999;
				DataSeriesProxyBars seriesHigh = new DataSeriesProxyBars(this.Bars, DataSeriesProxyableFromBars.High);
				double ret = seriesHigh.MaxValueBetweenIndexesDoubleMinValueUnsafe(this.VisibleBarLeft, this.VisibleBarRight);
				if (this.VisibleBarRight >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisiblePriceMax(): " + msg);
				}
				return ret;
			} }
		public double VisibleVolumeMinNew { get {
				if (base.DesignMode || this.BarsEmpty) return 99;
				DataSeriesProxyBars seriesVolume = new DataSeriesProxyBars(this.Bars, DataSeriesProxyableFromBars.Volume);
				double ret = seriesVolume.MinValueBetweenIndexesDoubleMaxValueUnsafe(this.VisibleBarLeft, this.VisibleBarRight);
				if (this.VisibleBarRight >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
						string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
						Assembler.PopupException("VisibleVolumeMin(): " + msg);
				}
				return ret;
			} }
		public double VisibleVolumeMaxNew { get {
				if (base.DesignMode || this.BarsEmpty) return 658;
				DataSeriesProxyBars seriesVolume = new DataSeriesProxyBars(this.Bars, DataSeriesProxyableFromBars.Volume);
				double ret = seriesVolume.MaxValueBetweenIndexesDoubleMinValueUnsafe(this.VisibleBarLeft, this.VisibleBarRight);
				if (this.VisibleBarRight >= this.Bars.Count) {	// we want to display 0..64, but Bars has only 10 bars inside
					string msg = "YOU_SHOULD_INVOKE_SyncHorizontalScrollToBarsCount_PRIOR_TO_RENDERING_I_DONT_KNOW_ITS_NOT_SYNCED_AFTER_ChartControl.Initialize(Bars)";
					Assembler.PopupException("VisibleVolumeMax(): " + msg);
				}
				return ret;
			} }
		
		public string ValueFormattedToSymbolInfoDecimalsOr5(double value, bool useFormatForPrice = true) {
			string format = "N" + (useFormatForPrice ? this.BarsDecimalsPrice : this.BarsDecimalsVolume);
			if (useFormatForPrice) {
				return value.ToString(format);
			}
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
			return value.ToString(format);
		}
	}
}