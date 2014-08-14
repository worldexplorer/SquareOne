using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SliderComboControl {

		void domainUpDown_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode != Keys.Enter) return;
			this.DomainUpDown.Select();
			decimal parsed;
			try {
				parsed = Decimal.Parse(this.DomainUpDown.Text);
				this.DomainUpDown.BackColor = Color.White;
			} catch (Exception ex) {
				this.DomainUpDown.BackColor = Color.LightSalmon;
				return;
			}
			if (parsed > this.ValueMax) {
				parsed = this.ValueMax;
				this.DomainUpDown.BackColor = Color.LightSalmon;
			}
			if (parsed < this.ValueMin) {
				parsed = this.ValueMin;
				this.DomainUpDown.BackColor = Color.LightSalmon;
			}
			this.DomainUpDown.BackColor = Color.White;
			parsed = this.PanelFillSlider.RoundToClosestStep(parsed);
			this.ValueCurrent = parsed;
		}
		void domainUpDown_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Up) {
				this.domainUpDown_onDomainUp(sender, null);
			}
			if (e.KeyCode == Keys.Down) {
				this.domainUpDown_onDomainDown(sender, null);
			}
		}
		void domainUpDown_onDomainDown(object sender, EventArgs e) {
			try {
				decimal parsed = Decimal.Parse(this.DomainUpDown.Text);
				this.DomainUpDown.BackColor = Color.White;
				parsed -= this.ValueStep;
				if (parsed < this.ValueMin) return;
				this.ValueCurrent = parsed;
			} catch (Exception ex) {
				this.DomainUpDown.BackColor = Color.LightSalmon;
			}
		}
		void domainUpDown_onDomainUp(object sender, EventArgs e) {
			try {
				decimal parsed = Decimal.Parse(this.DomainUpDown.Text);
				this.DomainUpDown.BackColor = Color.White;
				parsed += this.ValueStep;
				if (parsed > this.ValueMax) return;
				this.ValueCurrent = parsed;
			} catch (Exception ex) {
				this.DomainUpDown.BackColor = Color.LightSalmon;
			}
		}

		void domainUpDown_Scroll(object sender, ScrollEventArgs e) {
			int a = 1;
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
						if (parsed < this.ValueMin) {
							this.ValueCurrent = this.ValueMin;
							e.HighlightTextWithRed = true;
							return;
						}
						if (parsed > this.ValueMax) {
							this.ValueCurrent = this.ValueMax;
							e.HighlightTextWithRed = true;
							return;
						}
						this.ValueCurrent = parsed;
						this.ValueCurrentChanged(this, EventArgs.Empty);
						this.ctxSlider_Opening(this, null);		// not sure how textbox gets multiline input inside!!! may be this will help as for ScriptContexts
						break;
					case "mniltbValueStep":			this.ValueStep		= parsed; break;
					default:	Assembler.PopupException("mnitlbAll_UserTyped(): add handler for senderMinMaxCurrentStep.Name[" + senderMinMaxCurrentStep.Name + "]"); break;
				}
			} catch (Exception ex) {
				e.HighlightTextWithRed = true;
			}
		}

		void PanelFillSlider_MouseUp(object sender, MouseEventArgs e) {
			string valueClicked = this.PanelFillSlider.ValueCurrent.ToString(this.ValueFormat);
			if (valueClicked == this.DomainUpDown.Text) return;
			this.DomainUpDown.Text = valueClicked;
			if (ValueCurrentChanged == null) return;
			this.ValueCurrentChanged(this, EventArgs.Empty);
		}

		protected override void OnLoad(EventArgs e) {
			this.DomainUpDown.Text = this.format(this.ValueCurrent);
			this.mniltbValueCurrent.InputFieldValue = this.format(this.ValueCurrent);
			this.mniltbValueMin.InputFieldValue = this.format(this.ValueMin);
			this.mniltbValueMax.InputFieldValue = this.format(this.ValueMax);
			this.mniltbValueStep.InputFieldValue = this.format(this.ValueStep);
			this.mniHeaderNonHighlighted.Text = this.LabelText;
		}

		void mniAutoClose_Click(object sender, EventArgs e) {
			this.ctxSlider.AutoClose = this.mniAutoClose.Checked; 
		}
		void mniShowBorder_Click(object sender, EventArgs e) {
			this.EnableBorder = this.mniShowBorder.Checked; 
		}
		void mniShowNumeric_Click(object sender, EventArgs e) {
			this.EnableNumeric = this.mniShowNumeric.Checked; 
		}

		void ctxSlider_Opening(object sender, CancelEventArgs e) {
			SlidersAutoGrowControl slidersAutoGrow = base.Parent as SlidersAutoGrowControl;
			if (slidersAutoGrow == null) {
				string msg = "SliderCombo should be added into SlidersAutoGrow"
					+ " to get SlidersAutoGrow's menu and append it to rightClick on a slider";
				Assembler.PopupException(msg);
				return;
			}

			try {
				this.ctxSlider.SuspendLayout();
				//this.ctxSlider.Items.AddRange(slidersAutoGrow.TsiScriptContextsDynamic);
				//int indexToInsert = this.ctxSlider.Items.IndexOf(this.mniSepAddContextScriptsAfter) + 1;
				//this.ctxSlider.Items.AddRange(slidersAutoGrow.TsiScriptContextsDynamic);
				//foreach (var mni in slidersAutoGrow.TsiScriptContextsDynamic) {
				foreach (var mni in slidersAutoGrow.TsiDynamic) {
					//this.ctxSlider.Items.Insert(indexToInsert++, mni);
					this.ctxSlider.Items.Add(mni);
				}
			} catch (Exception ex) {
				Assembler.PopupException("ctxSlider_Opening()", ex);
			} finally {
				this.ctxSlider.ResumeLayout(true);
			}
		}
	}
}