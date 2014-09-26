using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.SteppingSlider;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		void slider_ValueCurrentChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				
				if (indicatorParameterChanged != null) {
					// single instance, no need to synchronize
					// ALREADY THERE, SINGLE INSTANCE this.Strategy.ScriptContextCurrent.IndicatorParametersByName[indicatorParameterChanged.Name] = indicatorParameterChanged.ValueCurrent;
					Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
					this.RaiseOnSliderChangedParameterValue(null);	// arg isn't processed downstack; I don't want IndicatorParameter to inherit from ScriptParameter
				} else {
					// multiple instances, synchronize
					scriptParameterChanged.ValueCurrent = (double)slider.ValueCurrent;
					this.Strategy.DropChangedValueToScriptAndCurrentContextAndSerialize(scriptParameterChanged);
					this.RaiseOnSliderChangedParameterValue(scriptParameterChanged);
				}
			} catch (Exception ex) {
				Assembler.PopupException("slider_ValueCurrentChanged()", ex);
			}
		}
		void slider_ShowBorderChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				this.Strategy.SetSliderBordersShownForParameterId(scriptParameterChanged.Id, slider.EnableBorder);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ValueCurrentChanged()", ex);
			}
		}
		void slider_ShowNumericUpdownChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				this.Strategy.SetSliderNumericUpdownShownForParameterId(scriptParameterChanged.Id, slider.EnableNumeric);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ValueCurrentChanged()", ex);
			}
		}
		
		
		void mniAllParamsShowBorder_Click(object sender, EventArgs e) {
			bool borderShown = this.mniAllParamsShowBorder.Checked;
			borderShown = !borderShown;
			foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
				slider.EnableBorder = borderShown; 
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForParameterId(scriptParameterChanged.Id, borderShown);
			}
		}
		void mniAllParamsShowNumeric_Click(object sender, EventArgs e) {
			bool numericShown = this.mniAllParamsShowNumeric.Checked;
			numericShown = !numericShown; 
			foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
				slider.EnableNumeric = numericShown; 
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForParameterId(scriptParameterChanged.Id, numericShown);
			}
		}

	}
}