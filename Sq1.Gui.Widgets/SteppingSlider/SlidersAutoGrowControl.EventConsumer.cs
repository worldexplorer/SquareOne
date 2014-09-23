using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.SteppingSlider;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		void slider_ValueCurrentChanged(object sender, EventArgs e) {
			try {
				SliderComboControl slider = sender as SliderComboControl;
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				scriptParameterChanged.ValueCurrent = (double)slider.ValueCurrent;
				this.Strategy.DropChangedValueToScriptAndCurrentContextAndSerialize(scriptParameterChanged);
				this.RaiseOnSliderValueChanged(scriptParameterChanged);
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
			foreach (SliderComboControl slider in this.SlidersScriptParameters) {
				slider.EnableBorder = borderShown; 
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForParameterId(scriptParameterChanged.Id, borderShown);
			}
			this.mniAllParamsShowBorder.Text = borderShown ? "All Params -> HideBorder" : "All Params -> ShowBorder";
			this.mniAllParamsShowBorder.Checked = borderShown;
		}
		void mniAllParamsShowNumeric_Click(object sender, EventArgs e) {
			bool numericShown = this.mniAllParamsShowNumeric.Checked;
			numericShown = !numericShown; 
			foreach (SliderComboControl slider in this.SlidersScriptParameters) {
				slider.EnableNumeric = numericShown; 
				ScriptParameter scriptParameterChanged = slider.Tag as ScriptParameter;
				// ouch N * StrategySave()...
				this.Strategy.SetSliderNumericUpdownShownForParameterId(scriptParameterChanged.Id, numericShown);
			}
			this.mniAllParamsShowNumeric.Text = numericShown ? "All Params -> HideNumeric" : "All Params -> ShowNumeric";
			this.mniAllParamsShowNumeric.Checked = numericShown;
		}

	}
}