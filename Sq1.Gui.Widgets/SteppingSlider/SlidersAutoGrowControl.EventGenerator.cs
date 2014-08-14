using System;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		public event EventHandler<ScriptParameterEventArgs> SliderValueChanged;
		public event EventHandler<EventArgs> ControlHeightChangedDueToNumberOfSliders;
		
		void RaiseOnSliderValueChanged(ScriptParameter parameter) {
			if (this.SliderValueChanged == null) return;
			this.SliderValueChanged(this, new ScriptParameterEventArgs(parameter));
		}
	}
}
