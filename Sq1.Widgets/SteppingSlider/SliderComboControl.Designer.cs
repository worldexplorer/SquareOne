using System;

namespace Sq1.Widgets.SteppingSlider {
	partial class SliderComboControl {
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
			this.DomainUpDown = new Sq1.Widgets.SteppingSlider.DomainUpDownWithMouseEvents();
			this.PanelFillSlider = new Sq1.Widgets.SteppingSlider.PanelFillSlider();
			this.ctxSlider = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniHeaderNonHighlighted = new Sq1.Widgets.LabeledTextBox.MenuItemLabel();
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
			this.splitContainer1.Panel1.Controls.Add(this.DomainUpDown);
			this.splitContainer1.Panel1MinSize = 30;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.PanelFillSlider);
			this.splitContainer1.Size = new System.Drawing.Size(236, 17);
			this.splitContainer1.SplitterDistance = 41;		// exact size to fit "2.61"
			//this.splitContainer1.SplitterIncrement = 12;	// exact increment to fit "2.61" => "2.6172" => "2.617216" 
			this.splitContainer1.TabIndex = 2;
			// 
			// DomainUpDown
			// 
			this.DomainUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DomainUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DomainUpDown.Location = new System.Drawing.Point(0, 0);
			this.DomainUpDown.Name = "DomainUpDown";
			this.DomainUpDown.Size = new System.Drawing.Size(41, 20);
			this.DomainUpDown.TabIndex = 0;
			this.DomainUpDown.Text = "domainUpDown1";
			this.DomainUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.DomainUpDown.OnArrowUpStepAdd += new System.EventHandler(this.domainUpDown_OnArrowUpStepAdd);
			this.DomainUpDown.OnArrowDownStepSubstract += new System.EventHandler(this.domainUpDown_OnArrowDownStepSubstract);
			this.DomainUpDown.Scroll += new System.Windows.Forms.ScrollEventHandler(this.domainUpDown_Scroll);
			this.DomainUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.domainUpDown_KeyDown);
			this.DomainUpDown.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.domainUpDown_PreviewKeyDown);
			// 
			// PanelFillSlider
			// 
			this.PanelFillSlider.BackColor = System.Drawing.Color.LightGray;
			this.PanelFillSlider.BorderOn = true;
			this.PanelFillSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelFillSlider.ColorBgMouseOverEnabled = System.Drawing.Color.Thistle;
			this.PanelFillSlider.ColorBgValueCurrentDisabled = System.Drawing.Color.DarkGray;
			this.PanelFillSlider.ColorBgValueCurrentEnabled = System.Drawing.Color.LightSteelBlue;
			this.PanelFillSlider.ColorFgParameterLabel = System.Drawing.Color.White;
			this.PanelFillSlider.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelFillSlider.ForeColor = System.Drawing.Color.DeepPink;
			this.PanelFillSlider.LabelText = "Length of Vehicle, cm";
			this.PanelFillSlider.Location = new System.Drawing.Point(0, 0);
			this.PanelFillSlider.Name = "PanelFillSlider";
			this.PanelFillSlider.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.PanelFillSlider.Size = new System.Drawing.Size(191, 17);
			this.PanelFillSlider.TabIndex = 1;
			this.PanelFillSlider.ValueIncrement = new decimal(new int[] {
									100,
									0,
									0,
									0});
			this.PanelFillSlider.ValueMin = new decimal(new int[] {
									0,
									0,
									0,
									0});
			this.PanelFillSlider.ValueMax = new decimal(new int[] {
									500,
									0,
									0,
									0});
			this.PanelFillSlider.ValueCurrent = new decimal(new int[] {
									200,
									0,
									0,
									0});
			this.PanelFillSlider.ValueCurrentChanged += new EventHandler(this.PanelFillSlider_ValueCurrentChanged);
			// 
			// ctxSlider
			// 
			this.ctxSlider.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniHeaderNonHighlighted,
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
			this.ctxSlider.Size = new System.Drawing.Size(225, 181);
			this.ctxSlider.Opening += new System.ComponentModel.CancelEventHandler(this.ctxSlider_Opening);
			// 
			// mniHeaderNonHighlighted
			// 
			this.mniHeaderNonHighlighted.AccessibleName = "";
			this.mniHeaderNonHighlighted.BackColor = System.Drawing.Color.Transparent;
			this.mniHeaderNonHighlighted.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniHeaderNonHighlighted.Name = "mniHeaderNonHighlighted";
			this.mniHeaderNonHighlighted.Size = new System.Drawing.Size(100, 23);
			this.mniHeaderNonHighlighted.Text = "Strategy Parameter Name444";
			// 
			// mniltbValueMin
			// 
			this.mniltbValueMin.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueMin.InputFieldOffsetX = 80;
			this.mniltbValueMin.InputFieldValue = "";
			this.mniltbValueMin.InputFieldWidth = 44;
			this.mniltbValueMin.Margin = new System.Windows.Forms.Padding(0);
			this.mniltbValueMin.Name = "mniltbValueMin";
			this.mniltbValueMin.Size = new System.Drawing.Size(164, 18);
			this.mniltbValueMin.Text = "Min";
			this.mniltbValueMin.TextRed = false;
			this.mniltbValueMin.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueCurrent
			// 
			this.mniltbValueCurrent.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueCurrent.InputFieldOffsetX = 80;
			this.mniltbValueCurrent.InputFieldValue = "";
			this.mniltbValueCurrent.InputFieldWidth = 44;
			this.mniltbValueCurrent.Name = "mniltbValueCurrent";
			this.mniltbValueCurrent.Size = new System.Drawing.Size(164, 18);
			this.mniltbValueCurrent.Text = "Current";
			this.mniltbValueCurrent.TextRed = false;
			this.mniltbValueCurrent.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueMax
			// 
			this.mniltbValueMax.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueMax.InputFieldOffsetX = 80;
			this.mniltbValueMax.InputFieldValue = "";
			this.mniltbValueMax.InputFieldWidth = 44;
			this.mniltbValueMax.Name = "mniltbValueMax";
			this.mniltbValueMax.Size = new System.Drawing.Size(164, 18);
			this.mniltbValueMax.Text = "Max";
			this.mniltbValueMax.TextRed = false;
			this.mniltbValueMax.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mniltbValueStep
			// 
			this.mniltbValueStep.BackColor = System.Drawing.Color.Transparent;
			this.mniltbValueStep.InputFieldOffsetX = 80;
			this.mniltbValueStep.InputFieldValue = "";
			this.mniltbValueStep.InputFieldWidth = 44;
			this.mniltbValueStep.Name = "mniltbValueStep";
			this.mniltbValueStep.Size = new System.Drawing.Size(164, 18);
			this.mniltbValueStep.Text = "Step";
			this.mniltbValueStep.TextRed = false;
			this.mniltbValueStep.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
			// 
			// mniShowNumeric
			// 
			this.mniShowNumeric.Checked = true;
			this.mniShowNumeric.CheckOnClick = true;
			this.mniShowNumeric.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowNumeric.Name = "mniShowNumeric";
			this.mniShowNumeric.Size = new System.Drawing.Size(224, 22);
			this.mniShowNumeric.Text = "Show Numeric Field";
			this.mniShowNumeric.Click += new System.EventHandler(this.mniShowNumeric_Click);
			// 
			// mniShowBorder
			// 
			this.mniShowBorder.Checked = true;
			this.mniShowBorder.CheckOnClick = true;
			this.mniShowBorder.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowBorder.Name = "mniShowBorder";
			this.mniShowBorder.Size = new System.Drawing.Size(224, 22);
			this.mniShowBorder.Text = "Show Border";
			this.mniShowBorder.Click += new System.EventHandler(this.mniShowBorder_Click);
			// 
			// mniAutoClose
			// 
			this.mniAutoClose.Checked = true;
			this.mniAutoClose.CheckOnClick = true;
			this.mniAutoClose.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniAutoClose.Name = "mniAutoClose";
			this.mniAutoClose.Size = new System.Drawing.Size(224, 22);
			this.mniAutoClose.Text = "AutoClose";
			this.mniAutoClose.Visible = false;
			this.mniAutoClose.Click += new System.EventHandler(this.mniAutoClose_Click);
			// 
			// mniSepAddContextScriptsAfter
			// 
			this.mniSepAddContextScriptsAfter.Name = "mniSepAddContextScriptsAfter";
			this.mniSepAddContextScriptsAfter.Size = new System.Drawing.Size(221, 6);
			// 
			// SliderComboControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.ctxSlider;
			this.Controls.Add(this.splitContainer1);
			this.DoubleBuffered = true;
			this.Name = "SliderComboControl";
			this.Size = new System.Drawing.Size(236, 17);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ctxSlider.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripSeparator mniSepAddContextScriptsAfter;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniAutoClose;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabel mniHeaderNonHighlighted;
		private System.Windows.Forms.ToolStripMenuItem mniShowBorder;
		private System.Windows.Forms.ToolStripMenuItem mniShowNumeric;
		private System.Windows.Forms.ContextMenuStrip ctxSlider;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueMin;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueCurrent;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueMax;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbValueStep;
		public PanelFillSlider PanelFillSlider;
		public DomainUpDownWithMouseEvents DomainUpDown;
	}
}
