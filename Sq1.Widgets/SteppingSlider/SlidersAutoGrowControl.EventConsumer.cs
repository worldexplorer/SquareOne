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
				
				if (scriptParameterChanged != null) {		// check ScriptParameter first because it's derived (same as "catch most fine grained Exceptions first")
					// multiple instances, synchronize
					scriptParameterChanged.ValueCurrent = (double)slider.ValueCurrent;
					this.Strategy.DropChangedValueToScriptAndCurrentContextAndSerialize(scriptParameterChanged);
					this.RaiseOnSliderChangedParameterValue(scriptParameterChanged);
				} else {
					// single instance, no need to synchronize
					indicatorParameterChanged.ValueCurrent = (double) slider.ValueCurrent;
					
					// NOPE_ITS_A_CLONE
//					string indicatorName = slider.LabelText.Substring(0, slider.LabelText.IndexOf('.'));
//					List<IndicatorParameter> list = this.Strategy.ScriptContextCurrent.IndicatorParametersByName[indicatorName];
//					foreach (IndicatorParameter each in list) {
//						if (each.Name != indicatorParameterChanged.Name) continue; 
//						each.ValueCurrent = (double) slider.ValueCurrent;
//						#if DEBUG
//						if (indicatorParameterChanged != each) {
//							string msg = "CLONE_100%";
//							Debugger.Break();
//						}
//						#endif
//						break;
//					}
					
					Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
					this.RaiseOnSliderChangedIndicatorValue(indicatorParameterChanged);
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
				Assembler.PopupException("slider_ShowBorderChanged()", ex);
			}
		}
		void slider_ShowNumericUpdownChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				this.Strategy.SetSliderNumericUpdownShownForScriptParameterId(scriptParameterChanged.Id, slider.EnableNumeric);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ShowNumericUpdownChanged()", ex);
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