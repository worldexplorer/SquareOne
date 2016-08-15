using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SteppingSliderComboControl {
		void domainUpDown_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode != Keys.Enter) return;
			this.NumericUpDown.Select(0, this.NumericUpDown.Text.Length);
			decimal parsed;
			try {
				parsed = Decimal.Parse(this.NumericUpDown.Text);
				this.NumericUpDown.BackColor = Color.White;
			} catch (Exception ex) {
				this.NumericUpDown.BackColor = Color.LightSalmon;
				return;
			}
			if (parsed > this.ValueMaxRtlSafe) {
				parsed = this.ValueMaxRtlSafe;
				this.NumericUpDown.BackColor = Color.LightSalmon;
			}
			if (parsed < this.ValueMinRtlSafe) {
				parsed = this.ValueMinRtlSafe;
				this.NumericUpDown.BackColor = Color.LightSalmon;
			}
			this.NumericUpDown.BackColor = Color.White;
			parsed = this.PanelFillSlider.RoundToClosestStep(parsed);
			this.ValueCurrent = parsed;
			this.RaiseValueChanged();
		}
		void domainUpDown_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Up) {
				this.domainUpDown_OnArrowUpStepAdd(sender, null);
			}
			if (e.KeyCode == Keys.Down) {
				this.domainUpDown_OnArrowDownStepSubstract(sender, null);
			}
		}
		void domainUpDown_OnArrowDownStepSubstract(object sender, EventArgs e) {
			try {
				//v1 decimal parsed = Decimal.Parse(this.NumericUpDown.Text);
				decimal parsed = this.NumericUpDown.Value;
				this.NumericUpDown.BackColor = Color.White;
				parsed -= this.ValueIncrement;
				if (parsed < this.ValueMinRtlSafe) return;
				this.ValueCurrent = parsed;		// => RaiseValueCurrentChanged()
				// AVOIDING_DUPLICATED_EVENT__ASSIGNING_ALREADY_RAISED this.RaiseValueChanged();
			} catch (Exception ex) {
				this.NumericUpDown.BackColor = Color.LightSalmon;
			}
		}
		void domainUpDown_OnArrowUpStepAdd(object sender, EventArgs e) {
			try {
				//v1 decimal parsed = Decimal.Parse(this.NumericUpDown.Text);
				decimal parsed = this.NumericUpDown.Value;
				this.NumericUpDown.BackColor = Color.White;
				parsed += this.ValueIncrement;
				if (parsed > this.ValueMaxRtlSafe) return;
				this.ValueCurrent = parsed;		// => RaiseValueCurrentChanged()
				// AVOIDING_DUPLICATED_EVENT__ASSIGNING_ALREADY_RAISED this.RaiseValueChanged();
			} catch (Exception ex) {
				this.NumericUpDown.BackColor = Color.LightSalmon;
			}
		}

		void domainUpDown_Scroll(object sender, ScrollEventArgs e) {
			if (e.ScrollOrientation != ScrollOrientation.VerticalScroll) return;
			if (e.NewValue > e.OldValue) {
				this.domainUpDown_OnArrowUpStepAdd(this, null);
			} else {
				this.domainUpDown_OnArrowDownStepSubstract(this, null);
			}
		}
		void mnitlbAll_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string typed = e.StringUserTyped;
			var senderMinMaxCurrentStep = sender as MenuItemLabeledTextBox;
			try {
				decimal parsed = Decimal.Parse(typed);
				switch (senderMinMaxCurrentStep.Name) {
					case "mniltbValueMin":			this.ValueMin		= parsed; break; 
					case "mniltbValueMax":			this.ValueMax		= parsed; break; 
					case "mniltbValueCurrent":
						if (parsed < this.ValueMinRtlSafe) {
							this.ValueCurrent = this.ValueMinRtlSafe;
							e.HighlightTextWithRed = true;
							return;
						}
						if (parsed > this.ValueMaxRtlSafe) {
							this.ValueCurrent = this.ValueMaxRtlSafe;
							e.HighlightTextWithRed = true;
							return;
						}
						// do you want to save the strategy??? Backtester is already backtesting now, Sliders all repainted by Sliders.Initialize(Strategy) 
						this.ValueCurrent = parsed;
						//YOU_STICK_TO_SETTING_VALUE_CURRENT_IT_WILL_RAISE_THE_SAME_EVENT this.ValueCurrentChanged(this, EventArgs.Empty);
						this.ctxSlider_Opening(this, null);		// not sure how textbox gets multiline input inside!!! may be this will help as for ScriptContexts
						this.RaiseValueChanged();
						break;
					case "mniltbValueStep":			this.ValueIncrement		= parsed; break;
					default:	Assembler.PopupException("mnitlbAll_UserTyped(): add handler for senderMinMaxCurrentStep.Name[" + senderMinMaxCurrentStep.Name + "]"); break;
				}
			} catch (Exception ex) {
				e.HighlightTextWithRed = true;
			}
		}
	
		void domainUpDown_GotFocus (object sender, EventArgs e) {
			this.NumericUpDown.Select(0, this.NumericUpDown.Text.Length);
		}

		void PanelFillSlider_ValueCurrentChanged(object sender, EventArgs e) {
			//v1 string valueClicked = this.PanelFillSlider.ValueCurrent.ToString(this.ValueFormat);
			//v1 if (valueClicked == this.NumericUpDown.Text) return;
			//v1 this.NumericUpDown.Text = valueClicked;
			if (this.PanelFillSlider.ValueCurrent == this.NumericUpDown.Value) return;
			this.NumericUpDown.Value = this.PanelFillSlider.ValueCurrent;
			this.RaiseValueChanged();
		}

//		protected override void OnLoad(EventArgs e) {
//			this.DomainUpDown.Text = this.format(this.ValueCurrent);
//			this.mniltbValueCurrent.InputFieldValue = this.format(this.ValueCurrent);
//			this.mniltbValueMin.InputFieldValue = this.format(this.ValueMin);
//			this.mniltbValueMax.InputFieldValue = this.format(this.ValueMax);
//			this.mniltbValueStep.InputFieldValue = this.format(this.ValueIncrement);
//			this.mniHeaderNonHighlighted.Text = this.LabelText;
//		}

		void mniAutoClose_Click(object sender, EventArgs e) {
			this.ctxSlider.AutoClose = this.mniAutoClose.Checked; 
		}
		void mniShowBorder_Click(object sender, EventArgs e) {
			this.EnableBorder = this.mniShowBorder.Checked;
			this.RaiseShowBorderChanged();
		}
		void mniShowNumeric_Click(object sender, EventArgs e) {
			this.EnableNumeric = this.mniShowNumeric.Checked;
			this.RaiseShowNumericUpdownChanged();
		}

		bool ctxSlider_IalreadyAppended_myCrazyMenuItems = false;
		void ctxSlider_Opening(object sender, CancelEventArgs e) {
			//v1 this.NumericUpDown.Text = this.format(this.ValueCurrent);
			this.NumericUpDown.Value = this.ValueCurrent;

			this.mniltbValueCurrent	.InputFieldValue = this.format(this.ValueCurrent);
			this.mniltbValueMin		.InputFieldValue = this.format(this.ValueMin);
			this.mniltbValueMax		.InputFieldValue = this.format(this.ValueMax);
			this.mniltbValueStep	.InputFieldValue = this.format(this.ValueIncrement);

			IndicatorParameter paramImHolding = this.Tag as IndicatorParameter;
			if (paramImHolding != null && paramImHolding.ValueConverter_meaningfulToStrategy != null) {
				string format = this.ValueFormat;
				if (paramImHolding.ValueFormat_meaningfulToStrategy != null) {
					format = paramImHolding.ValueFormat_meaningfulToStrategy();
				}

				double current_strategyMeaningful = paramImHolding.ValueConverter_meaningfulToStrategy(paramImHolding.ValueCurrent);
				if (double.IsNaN(current_strategyMeaningful) == false) {
					this.mniltbValueCurrent	.LabeledTextBoxControl.LabelRight.Text = "[" + current_strategyMeaningful.ToString(format) + "]";
				}

				double min_strategyMeaningful = paramImHolding.ValueConverter_meaningfulToStrategy(paramImHolding.ValueMin);
				if (double.IsNaN(min_strategyMeaningful) == false) {
					this.mniltbValueMin		.LabeledTextBoxControl.LabelRight.Text = "[" + min_strategyMeaningful.ToString(format) + "]";
				}

				double max_strategyMeaningful = paramImHolding.ValueConverter_meaningfulToStrategy(paramImHolding.ValueMax);
				if (double.IsNaN(max_strategyMeaningful) == false) {
					this.mniltbValueMax		.LabeledTextBoxControl.LabelRight.Text = "[" + max_strategyMeaningful.ToString(format) + "]";
				}

				double step_strategyMeaningful = paramImHolding.ValueConverter_meaningfulToStrategy(paramImHolding.ValueIncrement);
				if (double.IsNaN(step_strategyMeaningful) == false) {
					this.mniltbValueStep	.LabeledTextBoxControl.LabelRight.Text = "[" + step_strategyMeaningful.ToString(format) + "]";
				}

			}

			this.mniParameterVariableName		.Text = this.LabelText;
			this.mniParameterVariableDescription.Text = this.ParameterDescription;

			//v1 ???base.Parent is NULL here:  SlidersAutoGrowControl slidersAutoGrow = base.Parent as SlidersAutoGrowControl;
			SteppingSlidersAutoGrowControl slidersAutoGrow = this.ParentAutoGrowControl;
			if (slidersAutoGrow == null) {
				string msg = "SliderCombo should be added into SlidersAutoGrow"
					+ " to get SlidersAutoGrow's menu and append it to rightClick on a slider";
				Assembler.PopupException(msg);
				return;
			}

			if (this.ctxSlider_IalreadyAppended_myCrazyMenuItems) return; 
			try {
				this.ctxSlider.SuspendLayout();
				//this.ctxSlider.Items.AddRange(slidersAutoGrow.TsiScriptContextsDynamic);
				//int indexToInsert = this.ctxSlider.Items.IndexOf(this.mniSepAddContextScriptsAfter) + 1;
				//this.ctxSlider.Items.AddRange(slidersAutoGrow.TsiScriptContextsDynamic);
				//foreach (var mni in slidersAutoGrow.TsiScriptContextsDynamic) {
				foreach (var mni in slidersAutoGrow.TsiDynamic) {
					//this.ctxSlider.Items.Insert(indexToInsert++, mni);
					if (mni.IsDisposed) {
						continue;
					}
					this.ctxSlider.Items.Add(mni);
				}

				// otherwize I have to scroll vertically my trying to avoid this.ctxSlider.LayoutEngine.Layout(x, y);
				int height_before = this.ctxSlider.Height;
				this.ctxSlider.AutoSize = false;
				this.ctxSlider.AutoSize = true;
				int height_after = this.ctxSlider.Height;
				bool shouldGrow = height_after > height_before;
			} catch (Exception ex) {
				Assembler.PopupException("ctxSlider_Opening()", ex);
			} finally {
				this.ctxSlider.ResumeLayout(true);
				this.ctxSlider_IalreadyAppended_myCrazyMenuItems = true;
			}
		}
	}
}