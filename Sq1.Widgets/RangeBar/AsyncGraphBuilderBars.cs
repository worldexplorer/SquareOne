using System;

using Sq1.Core.DataTypes;

namespace Sq1.Widgets.RangeBar {
	public class AsyncGraphBuilderBars : AsyncGraphBuilder<DateTime> {

		private BarsUnscaled BarsForChartThumbnail;
		private double closeLowest;
		private double closeHighest;
		
		protected override double ValueRange { get { return this.closeHighest - this.closeLowest; }  }
		public override bool HasDataToDraw { get { return this.BarsForChartThumbnail != null && this.BarsForChartThumbnail.Count > 0; } }
		
		public AsyncGraphBuilderBars(RangeBarWithGraph<DateTime> rangeBar, int millisToWait = 300) : base(rangeBar, millisToWait) {
		}
		
		public void ResetLowestHighest() {
			this.closeLowest = double.MaxValue;
			this.closeHighest = double.MinValue;
		}
		public void Initialize(BarsUnscaled barsAllAvailable) {
			this.BarsForChartThumbnail = barsAllAvailable;
			this.ResetLowestHighest();
			if (this.HasDataToDraw) {
				for (int i = 0; i < this.BarsForChartThumbnail.Count; i++) {
					Bar bar = this.BarsForChartThumbnail[i];
					double close = bar.Close;
					if (close < this.closeLowest) this.closeLowest = close;
					if (close > this.closeHighest) this.closeHighest = close;
				}
			}
			//too early because we don't know Control.Height (or it's shadow base.RangeBar.BufferedGraphics.Graphics.VisibleClipBounds.Height)
			//this.BuildGraphTimeConsuming();
			// DONT_FORCE_NOW_ON_PAINT_WILL_BUILD_THANX_HasDataToDraw this.BuildGraphInNewThreadAndInvalidateDelayed();
			// UNCOMMENTED_TO_DRAW_WHATEVER_BARS_INITIALIZED_FIRST_TIME_DOESNT_WORK this.BuildGraphInNewThreadAndInvalidateDelayed();
			base.RangeBarWithGraph.BufferReset();	// will trigger Invalidate()
		}
		
		protected override float PercentageYOnGraphForRangePercentage(float pixelToPercentage0to1) {
			int barFromPercentage = base.RangeBarWithGraph.RoundInt(this.BarsForChartThumbnail.Count * pixelToPercentage0to1);
			if (barFromPercentage >= this.BarsForChartThumbnail.Count) {
				return -1;
			}
			double close = this.BarsForChartThumbnail[barFromPercentage].Close;
			return (float) ((close - this.closeLowest) / this.ValueRange);
		}
	}
}
