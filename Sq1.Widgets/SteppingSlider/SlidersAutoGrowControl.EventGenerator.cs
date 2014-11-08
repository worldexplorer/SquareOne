using System;

using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		public event EventHandler<ScriptParameterEventArgs> SliderChangedParameterValue;
		public event EventHandler<IndicatorParameterEventArgs> SliderChangedIndicatorValue;
		public event EventHandler<EventArgs> ControlHeightChangedDueToNumberOfSliders;
		
		void RaiseOnSliderChangedScriptParameterValue(ScriptParameter parameter) {
			if (this.SliderChangedParameterValue == null) return;
			try {
				this.SliderChangedParameterValue(this, new ScriptParameterEventArgs(parameter));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnSliderChangedScriptParameterValue()<SlidersAutoGrow_SliderValueChanged()", ex);
			}
		}
		void RaiseOnSliderChangedIndicatorParametersValue(IndicatorParameter parameter) {
			if (this.SliderChangedIndicatorValue == null) return;
			try {
				this.SliderChangedIndicatorValue(this, new IndicatorParameterEventArgs(parameter));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnSliderChangedIndicatorParametersValue()<SlidersAutoGrow_SliderValueChanged()", ex);
			}
		}
	}
}
