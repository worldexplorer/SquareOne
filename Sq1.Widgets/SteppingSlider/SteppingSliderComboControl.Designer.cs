using System;

namespace Sq1.Widgets.SteppingSlider {
	partial class SteppingSliderComboControl {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.NumericUpDown = new Sq1.Widgets.SteppingSlider.NumericUpDownWithMouseEvents();
			this.PanelFillSlider = new Sq1.Widgets.SteppingSlider.PanelFillSlider();
			this.ctxSlider = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniParameterVariableName = new Sq1.Widgets.LabeledTextBox.MenuItemLabel();
			this.mniParameterVariableDescription = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbValueMin = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbValueCurrent = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbValueMax = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbValueStep = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniShowNumeric = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowBorder = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAutoClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSepAddContextScriptsAfter = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown)).BeginInit();
			this.ctxSlider.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.NumericUpDown);
			this.splitContainer1.Panel1MinSize = 30;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.PanelFillSlider);
			this.splitContainer1.Size = new System.Drawing.Size(236, 18);
			this.splitContainer1.SplitterDistance = 41;
			this.splitContainer1.TabIndex = 2;
			// 
			// NumericUpDown
			// 
			this.NumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.NumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NumericUpDown.Location = new System.Drawing.Point(0, 0);
			this.NumericUpDown.Name = "NumericUpDown";
			this.NumericUpDown.Size = new System.Drawing.Size(41, 20);
			this.NumericUpDown.TabIndex = 0;
			this.NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.NumericUpDown.OnArrowUpStepAdd += new System.EventHandler(this.domainUpDown_OnArrowUpStepAdd);
			this.NumericUpDown.OnArrowDownStepSubstract += new System.EventHandler(this.domainUpDown_OnArrowDownStepSubstract);
			this.NumericUpDown.Scroll += new System.Windows.Forms.ScrollEventHandler(this.domainUpDown_Scroll);
			this.NumericUpDown.GotFocus += new System.EventHandler(this.domainUpDown_GotFocus);
			this.NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.domainUpDown_KeyDown);
			this.NumericUpDown.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.domainUpDown_PreviewKeyDown);
			// 
			// PanelFillSlider
			// 
			this.PanelFillSlider.BackColor = System.Drawing.Color.LightGray;
			this.PanelFillSlider.BorderOn = true;
			this.PanelFillSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelFillSlider.ColorBgMouseOverEnabled = System.Drawing.Color.YellowGreen;
			this.PanelFillSlider.ColorBgValueCurrentDisabled = System.Drawing.Color.DarkGray;
			this.PanelFillSlider.ColorBgValueCurrentEnabled = System.Drawing.Color.LightSteelBlue;
			this.PanelFillSlider.ColorFgParameterLabel = System.Drawing.Color.White;
			this.PanelFillSlider.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelFillSlider.FillFromCurrentToMax = false;
			this.PanelFillSlider.ForeColor = System.Drawing.Color.DeepPink;
			this.PanelFillSlider.LabelText = "Length of Vehicle, cm";
			this.PanelFillSlider.Location = new System.Drawing.Point(0, 0);
			this.PanelFillSlider.Name = "PanelFillSlider";
			this.PanelFillSlider.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.PanelFillSlider.Size = new System.Drawing.Size(191, 18);
			this.PanelFillSlider.TabIndex = 1;
			this.PanelFillSlider.ValueCurrent = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.PanelFillSlider.ValueIncrement = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.PanelFillSlider.ValueMax = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.PanelFillSlider.ValueMin = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// ctxSlider
			// 
			this.ctxSlider.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniParameterVariableName,
            this.mniParameterVariableDescription,
            this.toolStripSeparator2,
            this.mniltbValueMin,
            this.mniltbValueCurrent,
            this.mniltbValueMax,
            this.mniltbValueStep,
            this.toolStripSeparator1,
            this.mniShowNumeric,
            this.mniShowBorder,
            this.mniAutoClose,
            this.mniSepAddContextScriptsAfter});
			this.ctxSlider.Name = "contextMenuStrip1";
			this.ctxSlider.Size = new System.Drawing.Size(385, 221);
			this.ctxSlider.Opening += new System.ComponentModel.CancelEventHandler(this.ctxSlider_Opening);
			// 
			// mniParameterVariableName
			// 
			this.mniParameterVariableName.AccessibleName = "";
			this.mniParameterVariableName.BackColor = System.Drawing.Color.Transparent;
			this.mniParameterVariableName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniParameterVariableName.Name = "mniParameterVariableName";
			this.mniParameterVariableName.Size = new System.Drawing.Size(100, 23);
			this.mniParameterVariableName.Text = "vLength_reflectedVarName";
			// 
			// mniParameterVariableDescription
			// 
			this.mniParameterVariableDescription.Name = "mniParameterVariableDescription";
			this.mniParameterVariableDescription.Size = new System.Drawing.Size(384, 22);
			this.mniParameterVariableDescription.Text = "Vehicle length is variable here to backtest maneuvre-ability";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(381, 6);
			// 
			// mniltbValueMin
			// 
			this.mniltbValueMin.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueMin.InputFieldAlignedRight = false;
			this.mniltbValueMin.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbValueMin.InputFieldEditable = true;
			this.mniltbValueMin.InputFieldMultiline = true;
			this.mniltbValueMin.InputFieldOffsetX = 80;
			this.mniltbValueMin.InputFieldValue = "";
			this.mniltbValueMin.InputFieldWidth = 44;
			this.mniltbValueMin.Margin = new System.Windows.Forms.Padding(0);
			this.mniltbValueMin.Name = "mniltbValueMin";
			this.mniltbValueMin.OffsetTop = 0;
			this.mniltbValueMin.Size = new System.Drawing.Size(134, 21);
			this.mniltbValueMin.TextLeft = "Min";
			this.mniltbValueMin.TextLeftOffsetX = 0;
			this.mniltbValueMin.TextLeftWidth = 30;
			this.mniltbValueMin.TextRed = false;
			this.mniltbValueMin.TextRight = "";
			this.mniltbValueMin.TextRightOffsetX = 127;
			this.mniltbValueMin.TextRightWidth = 4;
			this.mniltbValueMin.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueCurrent
			// 
			this.mniltbValueCurrent.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueCurrent.InputFieldAlignedRight = false;
			this.mniltbValueCurrent.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbValueCurrent.InputFieldEditable = true;
			this.mniltbValueCurrent.InputFieldMultiline = true;
			this.mniltbValueCurrent.InputFieldOffsetX = 80;
			this.mniltbValueCurrent.InputFieldValue = "";
			this.mniltbValueCurrent.InputFieldWidth = 44;
			this.mniltbValueCurrent.Name = "mniltbValueCurrent";
			this.mniltbValueCurrent.OffsetTop = 0;
			this.mniltbValueCurrent.Size = new System.Drawing.Size(134, 21);
			this.mniltbValueCurrent.TextLeft = "Current";
			this.mniltbValueCurrent.TextLeftOffsetX = 0;
			this.mniltbValueCurrent.TextLeftWidth = 49;
			this.mniltbValueCurrent.TextRed = false;
			this.mniltbValueCurrent.TextRight = "";
			this.mniltbValueCurrent.TextRightOffsetX = 127;
			this.mniltbValueCurrent.TextRightWidth = 4;
			this.mniltbValueCurrent.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueMax
			// 
			this.mniltbValueMax.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueMax.InputFieldAlignedRight = false;
			this.mniltbValueMax.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbValueMax.InputFieldEditable = true;
			this.mniltbValueMax.InputFieldMultiline = true;
			this.mniltbValueMax.InputFieldOffsetX = 80;
			this.mniltbValueMax.InputFieldValue = "";
			this.mniltbValueMax.InputFieldWidth = 44;
			this.mniltbValueMax.Name = "mniltbValueMax";
			this.mniltbValueMax.OffsetTop = 0;
			this.mniltbValueMax.Size = new System.Drawing.Size(134, 21);
			this.mniltbValueMax.TextLeft = "Max";
			this.mniltbValueMax.TextLeftOffsetX = 0;
			this.mniltbValueMax.TextLeftWidth = 31;
			this.mniltbValueMax.TextRed = false;
			this.mniltbValueMax.TextRight = "";
			this.mniltbValueMax.TextRightOffsetX = 127;
			this.mniltbValueMax.TextRightWidth = 4;
			this.mniltbValueMax.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueStep
			// 
			this.mniltbValueStep.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueStep.InputFieldAlignedRight = false;
			this.mniltbValueStep.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbValueStep.InputFieldEditable = true;
			this.mniltbValueStep.InputFieldMultiline = true;
			this.mniltbValueStep.InputFieldOffsetX = 80;
			this.mniltbValueStep.InputFieldValue = "";
			this.mniltbValueStep.InputFieldWidth = 44;
			this.mniltbValueStep.Name = "mniltbValueStep";
			this.mniltbValueStep.OffsetTop = 0;
			this.mniltbValueStep.Size = new System.Drawing.Size(134, 21);
			this.mniltbValueStep.TextLeft = "Step";
			this.mniltbValueStep.TextLeftOffsetX = 0;
			this.mniltbValueStep.TextLeftWidth = 32;
			this.mniltbValueStep.TextRed = false;
			this.mniltbValueStep.TextRight = "";
			this.mniltbValueStep.TextRightOffsetX = 127;
			this.mniltbValueStep.TextRightWidth = 4;
			this.mniltbValueStep.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(381, 6);
			// 
			// mniShowNumeric
			// 
			this.mniShowNumeric.Checked = true;
			this.mniShowNumeric.CheckOnClick = true;
			this.mniShowNumeric.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowNumeric.Name = "mniShowNumeric";
			this.mniShowNumeric.Size = new System.Drawing.Size(384, 22);
			this.mniShowNumeric.Text = "Show Numeric Field";
			this.mniShowNumeric.Click += new System.EventHandler(this.mniShowNumeric_Click);
			// 
			// mniShowBorder
			// 
			this.mniShowBorder.Checked = true;
			this.mniShowBorder.CheckOnClick = true;
			this.mniShowBorder.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowBorder.Name = "mniShowBorder";
			this.mniShowBorder.Size = new System.Drawing.Size(384, 22);
			this.mniShowBorder.Text = "Show Border";
			this.mniShowBorder.Click += new System.EventHandler(this.mniShowBorder_Click);
			// 
			// mniAutoClose
			// 
			this.mniAutoClose.Checked = true;
			this.mniAutoClose.CheckOnClick = true;
			this.mniAutoClose.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniAutoClose.Name = "mniAutoClose";
			this.mniAutoClose.Size = new System.Drawing.Size(384, 22);
			this.mniAutoClose.Text = "AutoClose";
			this.mniAutoClose.Visible = false;
			this.mniAutoClose.Click += new System.EventHandler(this.mniAutoClose_Click);
			// 
			// mniSepAddContextScriptsAfter
			// 
			this.mniSepAddContextScriptsAfter.Name = "mniSepAddContextScriptsAfter";
			this.mniSepAddContextScriptsAfter.Size = new System.Drawing.Size(381, 6);
			// 
			// SteppingSliderComboControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.ctxSlider;
			this.Controls.Add(this.splitContainer1);
			this.DoubleBuffered = true;
			this.Name = "SteppingSliderComboControl";
			this.Size = new System.Drawing.Size(236, 18);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown)).EndInit();
			this.ctxSlider.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.ToolStripSeparator mniSepAddContextScriptsAfter;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniAutoClose;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabel mniParameterVariableName;
		private System.Windows.Forms.ToolStripMenuItem mniShowBorder;
		private System.Windows.Forms.ToolStripMenuItem mniShowNumeric;
		private System.Windows.Forms.ContextMenuStrip ctxSlider;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueMin;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueCurrent;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueMax;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueStep;
		public PanelFillSlider PanelFillSlider;
		public NumericUpDownWithMouseEvents NumericUpDown;
		private System.Windows.Forms.ToolStripMenuItem mniParameterVariableDescription;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	}
}
