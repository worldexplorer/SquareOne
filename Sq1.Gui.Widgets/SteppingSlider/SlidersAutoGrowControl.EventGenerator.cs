using System;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		public event EventHandler<ScriptParameterEventArgs> SliderChangedParameterValue;
		public event EventHandler<IndicatorParameterEventArgs> SliderChangedIndicatorValue;
		public event EventHandler<EventArgs> ControlHeightChangedDueToNumberOfSliders;
		
		void RaiseOnSliderChangedParameterValue(ScriptParameter parameter) {
			if (this.SliderChangedParameterValue == null) return;
			this.SliderChangedParameterValue(this, new ScriptParameterEventArgs(parameter));
		}
		void RaiseOnSliderChangedIndicatorValue(IndicatorParameter parameter) {
			if (this.SliderChangedIndicatorValue == null) return;
			this.SliderChangedIndicatorValue(this, new IndicatorParameterEventArgs(parameter));
		}
	}
}
