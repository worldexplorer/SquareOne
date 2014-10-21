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
				//PanelFillSlider panel = sender as PanelFillSlider;
				//SliderComboControl slider = panel.Parent.Parent.Parent as SliderComboControl;
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
				bool userChoseEnableBorder = slider.EnableBorder;
				IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
				if (indicatorParameterChanged == null) {
					string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_SOMETHING_ELSE";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					return;
				}

				indicatorParameterChanged.BorderShown = userChoseEnableBorder;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ShowBorderChanged()", ex);
			}
		}
		void slider_ShowNumericUpdownChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				bool userChoseEnableNumeric = slider.EnableNumeric;
				IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
				if (indicatorParameterChanged == null) {
					string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_SOMETHING_ELSE";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					return;
				}
	
				indicatorParameterChanged.NumericUpdownShown = userChoseEnableNumeric;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			} catch (Exception ex) {
				Assembler.PopupException("slider_ShowNumericUpdownChanged()", ex);
			}
		}
		
		
		void mniAllParamsShowBorder_Click(object sender, EventArgs e) {
			try {
				bool borderShown = this.mniAllParamsShowBorder.Checked;
				foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
					IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
					if (indicatorParameterChanged == null) {
						string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_SOMETHING_ELSE";
						#if DEBUG
						Debugger.Break();
						#endif
						Assembler.PopupException(msg);
						continue;
					}
					slider.EnableBorder = borderShown;
					indicatorParameterChanged.BorderShown = borderShown;
				}
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			} catch (Exception ex) {
				Assembler.PopupException("mniAllParamsShowBorder_Click()", ex);
			}
		}
		void mniAllParamsShowNumeric_Click(object sender, EventArgs e) {
			try {
				bool numericShown = this.mniAllParamsShowNumeric.Checked;
				foreach (SliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
					IndicatorParameter indicatorParameterChanged = slider.Tag as IndicatorParameter;
					if (indicatorParameterChanged == null) {
						string msg = "SLIDER_TAG_MUST_CONTAIN_INDICATOR_OR_SCRIPT_PARAMETER NOW_SOMETHING_ELSE";
						#if DEBUG
						Debugger.Break();
						#endif
						Assembler.PopupException(msg);
						continue;
					}
					slider.EnableNumeric = numericShown;
					indicatorParameterChanged.NumericUpdownShown = numericShown;
				}
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			} catch (Exception ex) {
				Assembler.PopupException("mniAllParamsShowNumeric_Click()", ex);
			}
		}

	}
}