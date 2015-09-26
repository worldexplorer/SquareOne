using System;

using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SteppingSlidersAutoGrowControl {
		public event EventHandler<ScriptParameterEventArgs> SliderChangedParameterValue;
		public event EventHandler<IndicatorParameterEventArgs> SliderChangedIndicatorValue;
		public event EventHandler<EventArgs> ControlHeightChangedDueToNumberOfSliders;
		bool skipSlidersFactory_changingValueDoesntChangeNumberOfSliders;
		
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
				this.skipSlidersFactory_changingValueDoesntChangeNumberOfSliders = true;
				this.SliderChangedIndicatorValue(this, new IndicatorParameterEventArgs(parameter));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnSliderChangedIndicatorParametersValue()<SlidersAutoGrow_SliderValueChanged()", ex);
			} finally {
				this.skipSlidersFactory_changingValueDoesntChangeNumberOfSliders = false;
			}
		}
	}
}
