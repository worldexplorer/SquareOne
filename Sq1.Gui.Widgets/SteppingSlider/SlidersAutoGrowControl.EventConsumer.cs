using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
					indicatorParameterChanged.ValueCurrent = (double) slider.ValueCurrent;
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
				this.Strategy.SetSliderNumericUpdownShownForScriptParameterId(scriptParameterChanged.Id, slider.EnableNumeric);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ValueCurrentChanged()", ex);
			}
		}
		
		
		void mniAllParamsShowBorder_Click(object sender, EventArgs e) {
			bool borderShown = this.mniAllParamsShowBorder.Checked;
			borderShown = !borderShown;
			foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
				slider.EnableBorder = borderShown; 

				if (slider.Tag == null) {
					string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_NULL";
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}

				IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
				if (indicatorParameterChanged != null) {
					// ouch N * StrategySave()...
					//this.Strategy.SetSliderBorderShownForIndicatorParameterName(indicatorParameterChanged, numericShown);
					string msg = "NYI SetSliderBorderShownForIndicatorParameterName(indicatorParameterChanged[" + indicatorParameterChanged.ToString() + "])";
					Assembler.PopupException(msg);
					return;
				}

				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				if (scriptParameterChanged == null) {
					string msg = "SLIDER_TAG_DIDN_CONTAIN_INDICATOR_NEIGHER_SCRIPT_PARAMETER [" + slider.Tag.GetType().FullName + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}

				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForScriptParameterId(scriptParameterChanged.Id, borderShown);
			}
		}
		void mniAllParamsShowNumeric_Click(object sender, EventArgs e) {
			bool numericShown = this.mniAllParamsShowNumeric.Checked;
			numericShown = !numericShown; 
			foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
				slider.EnableNumeric = numericShown;
				
				if (slider.Tag == null) {
					string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_NULL";
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}

				IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
				if (indicatorParameterChanged != null) {
					// ouch N * StrategySave()...
					//this.Strategy.SetSliderNumericUpdownShownForIndicatorParameterName(indicatorParameterChanged, numericShown);
					string msg = "NYI SetSliderNumericUpdownShownForIndicatorParameterName(indicatorParameterChanged[" + indicatorParameterChanged.ToString() + "])";
					Assembler.PopupException(msg);
					return;
				}

				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				if (scriptParameterChanged == null) {
					string msg = "SLIDER_TAG_DIDN_CONTAIN_INDICATOR_NEIGHER_SCRIPT_PARAMETER [" + slider.Tag.GetType().FullName + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}

				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForScriptParameterId(scriptParameterChanged.Id, numericShown);
			}
		}

	}
}