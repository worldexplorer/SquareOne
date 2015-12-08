using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public class PanelIndicator : PanelBase {
		public Indicator Indicator;
		public bool IndicatorEmpty { get {
				if (this.Indicator == null) {
					string msg = "CATCH_IT_EARLIER!!! did ctor get indicator=null ?";
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}
				if (this.Indicator.OwnValuesCalculated == null) {
					string msg = "I_JUST_RESTORED_THE_PANEL_WHILE_BACKTEST_HASNT_RUN_YET";
					//string msg = "CATCH_IT_EARLIER!!! this.Indicator.OwnValuesCalculated is created in"
					//	+ " Indicator.BacktestStartingConstructOwnValuesValidateParameters()"
					//	+ " to assure Executor's freedom to feed any bars during backtest; Indicator itself lives a longer life";
					#if DEBUG
					//Debugger.Break();
					#endif
					return true;
				}
				return (this.Indicator.OwnValuesCalculated.Count == 0);
			} }

		public PanelIndicator(Indicator indicator) : base() {
			Indicator = indicator;
			base.PanelName = indicator.ToString();
			//base.HScroll = false;	// I_SAW_THE_DEVIL_ON_PANEL_INDICATOR! is it visible by default??? I_HATE_HACKING_F_WINDOWS_FORMS
			base.ForeColor = indicator.LineColor;
			base.MinimumSize = new Size(20, 15);	// only height matters for MultiSplitContainer
		}

#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD 
		protected override void OnPaintBackground(PaintEventArgs e) {
			if (this.IndicatorEmpty) {
				string msg = "FOR_PANELS_DESERIALIZED_BUT_NO_BACKTEST_RUN_YET_MOUSEOVER_WILL_THROW";
				return;
			}
			base.OnPaint(e);
		}
#else
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
			if (this.IndicatorEmpty) {
				string msg = "FOR_PANELS_DESERIALIZED_BUT_NO_BACKTEST_RUN_YET_MOUSEOVER_WILL_THROW";
				return;
			}
			base.OnPaintDoubleBuffered(e);
		}
#endif

		protected override void PaintWholeSurfaceBarsNotEmpty(Graphics g) {
			// PanelIndicator should not append "ATR (Period:5[1..11/2]) " twice (?) below PanelName 
			// EACH_RENDERS_ITSELF__HAD_OLIVE_INDICATOR_NAME_DRAWN_TWICE_ON_ATR_OWN_PANEL base.RenderIndicators(g);
			// 1) uses here-defined VisibleMinDoubleMaxValueUnsafe,VisibleMaxDoubleMinValueUnsafe to set:
			//		base.VisibleMin,Max,Range_cached,
			//		base.VisibleMinMinusTopSqueezer_cached, this.VisibleMaxPlusBottomSqueezer_cached, this.VisibleRangeWithTwoSqueezers_cached
			// 2) paints Right and Bottom gutter foregrounds;
			//base.PaintWholeSurfaceBarsNotEmpty(g);
			
			string msig = " " + this.PanelName + ".PanelIndicator.PaintWholeSurfaceBarsNotEmpty()";
			double visibleMinCandidateMaxUnsafe = this.VisibleMinDoubleMaxValueUnsafe;
			double visibleMaxCandidateMinUnsafe = this.VisibleMaxDoubleMinValueUnsafe;
			
			bool valuesAreConstantOrNaN = (visibleMinCandidateMaxUnsafe == double.MaxValue && visibleMaxCandidateMinUnsafe == double.MinValue)
				|| visibleMinCandidateMaxUnsafe == visibleMaxCandidateMinUnsafe;
			double firstNonNan = this.FirstNonNanBetweenLeftRight;
			if (valuesAreConstantOrNaN && double.IsNaN(firstNonNan) == false) {
				visibleMinCandidateMaxUnsafe = firstNonNan - firstNonNan * 0.1;		// bandLower: -10% 
				visibleMaxCandidateMinUnsafe = firstNonNan + firstNonNan * 0.1;		// bandUpper: +10%
			} else {
				if (visibleMinCandidateMaxUnsafe == double.MaxValue) {
					string msg = "ALL_OWN_CALCULATED_INDICATOR_VALUES_BETWEEN_BARLEFT_BARRIGHT_ARE_NANS";
					Assembler.PopupException(msg, null, false);
					return;
				}
				if (visibleMaxCandidateMinUnsafe == double.MinValue) {
					string msg = "ALL_OWN_CALCULATED_INDICATOR_VALUES_BETWEEN_BARLEFT_BARRIGHT_ARE_NANS";
					Assembler.PopupException(msg, null, false);
					return;
				}
			}

			this.VisibleMin_cached = visibleMinCandidateMaxUnsafe;
			this.VisibleMax_cached = visibleMaxCandidateMinUnsafe;

			this.VisibleRange_cached = this.VisibleMax_cached - this.VisibleMin_cached;

			double pixelsSqueezedToPriceDistance = 0;
			if (this.PaddingVerticalSqueeze > 0 && base.Height > 0) {
				double priceDistanceInOnePixel = this.VisibleRange_cached / base.Height;
				pixelsSqueezedToPriceDistance = this.PaddingVerticalSqueeze * priceDistanceInOnePixel;
			}
			
			this.VisibleMinMinusTopSqueezer_cached = this.VisibleMin_cached - pixelsSqueezedToPriceDistance;
			this.VisibleMaxPlusBottomSqueezer_cached = this.VisibleMax_cached + pixelsSqueezedToPriceDistance;
			
			// min-Top=10-10=0; max+Bottom=20+10=30; 20+10-(10-10) = 30-0; = max-min+padding*2=20-10+10*2 = 30 
			this.VisibleRangeWithTwoSqueezers_cached = this.VisibleMaxPlusBottomSqueezer_cached - VisibleMinMinusTopSqueezer_cached;

			msig = " this.VisibleRangeWithTwoSqueezers_cached[" + this.VisibleRangeWithTwoSqueezers_cached + "]:"
				+ " [" + this.VisibleMinMinusTopSqueezer_cached + "]...[" + this.VisibleMaxPlusBottomSqueezer_cached + "]" + msig;
			if (double.IsNegativeInfinity(this.VisibleRangeWithTwoSqueezers_cached)) {
				string msg = "NEVER_HAPPENED_SO_FAR [" + this.ToString() + "]-INDICATOR_RANGE_MUST_BE_NON_NEGATIVE_INFINITY";
				Assembler.PopupException(msg + msig);
			}
			if (double.IsNaN(this.VisibleRangeWithTwoSqueezers_cached)) {
				string msg = "[" + this.ToString() + "]-INDICATOR_RANGE_MUST_BE_NON_NAN";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.VisibleRangeWithTwoSqueezers_cached <= 0) {
				// gridStep will throw an ArithmeticException
				string msg = "[" + this.ToString() + "]-INDICATOR_RANGE_MUST_BE_POSITIVE";
				Assembler.PopupException(msg + msig);
				return;
			}
			base.GutterGridLinesRightBottomDrawForeground(g);
		}
		//protected override void PaintBackgroundWholeSurfaceBarsNotEmpty(Graphics g) {
		//	base.PaintBackgroundWholeSurfaceBarsNotEmpty(g);	// paints Right and Bottom gutter backgrounds
		//	//this.PaintRightVolumeGutterAndGridLines(g);
		//}
		
		public override double VisibleMinDoubleMaxValueUnsafe { get {
				if (base.DesignMode || this.IndicatorEmpty) {
					return 99;	// random value; set breakpoint to see why the number doesn't matter
				}
				double ret = Double.MaxValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				//ret = this.Indicator.OwnValuesCalculated.MinValueBetweenIndexesDoubleMaxValueUnsafe(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				if (this.Indicator.OwnValuesCalculated.Count < base.VisibleBarRight_cached) return ret;
				ret = this.Indicator.OwnValuesCalculated.MinValueBetweenIndexesDoubleMaxValueUnsafe(base.VisibleBarLeft_cached, base.VisibleBarRight_cached);
				return ret;
			} }
		public override double VisibleMaxDoubleMinValueUnsafe { get {
				if (base.DesignMode || this.IndicatorEmpty) {
					return 658;	// random value; set breakpoint to see why the number doesn't matter
				}
				double ret = Double.MinValue;
				//int visibleOrReal = (this.VisibleBarRight > this.Bars.Count) ? this.VisibleBarRight : this.Bars.Count;
				//ret = this.Indicator.OwnValuesCalculated.MaxValueBetweenIndexesDoubleMinValueUnsafe(base.ChartControl.VisibleBarLeft, base.ChartControl.VisibleBarRight);
				if (this.Indicator.OwnValuesCalculated.Count < base.VisibleBarRight_cached) return ret;
				ret = this.Indicator.OwnValuesCalculated.MaxValueBetweenIndexesDoubleMinValueUnsafe(base.VisibleBarLeft_cached, base.VisibleBarRight_cached);
				return ret;
			} }
		
		public override int ValueIndexLastAvailableMinusOneUnsafe { get {
				if (this.Indicator == null) return -1; 
				if (this.Indicator.OwnValuesCalculated == null) return -1; 
				return this.Indicator.OwnValuesCalculated.Count - 1; 
			} }
		public override double ValueGetNaNunsafe(int barIndex) {
			if (this.Indicator == null) return double.NaN; 
			if (this.Indicator.OwnValuesCalculated == null) return double.NaN; 
			if (barIndex < 0) return double.NaN;
			if (barIndex >= this.Indicator.OwnValuesCalculated.Count) return double.NaN; 
			double indicatorValue = this.Indicator.OwnValuesCalculated[barIndex];
			return indicatorValue;
		}
		public override int PriceDecimals { get { return this.Indicator.Decimals; } }
	}
}
