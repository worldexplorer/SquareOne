using System;
using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		public event EventHandler<ScriptParameterEventArgs> SliderChangedParameterValue;
		public event EventHandler<IndicatorParameterEventArgs> SliderChangedIndicatorValue;
		public event EventHandler<EventArgs> ControlHeightChangedDueToNumberOfSliders;
		
		void RaiseOnSliderChangedParameterValue(ScriptParameter parameter) {
			if (this.SliderChangedParameterValue == null) return;
			try {
				this.SliderChangedParameterValue(this, new ScriptParameterEventArgs(parameter));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnSliderChangedParameterValue()>>SlidersAutoGrow_SliderValueChanged()", ex);
			}
		}
		void RaiseOnSliderChangedIndicatorValue(IndicatorParameter parameter) {
			if (this.SliderChangedIndicatorValue == null) return;
			try {
				this.SliderChangedIndicatorValue(this, new IndicatorParameterEventArgs(parameter));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnSliderChangedIndicatorValue()>>SlidersAutoGrow_SliderValueChanged()", ex);
			}
		}
	}
}
