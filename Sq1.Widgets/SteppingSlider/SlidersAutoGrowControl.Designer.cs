using System;
using System.Windows.Forms;

namespace Sq1.Widgets.SteppingSlider {
	partial class SlidersAutoGrowControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.ctxOperations = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniScriptContextDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.mniParameterBagLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxScriptContexts = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniParameterBag_test1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniParameterBag_test2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniAllParamsResetToScriptDefaults = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAllParamsShowBorder = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAllParamsShowNumeric = new System.Windows.Forms.ToolStripMenuItem();
			this.mniScriptContextsMniHeader = new Sq1.Widgets.LabeledTextBox.MenuItemLabel();
			this.mniltbScriptContextNewWithDefaults = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.templateSliderControl = new Sq1.Widgets.SteppingSlider.SliderComboControl();
			this.mniltbScriptContextRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbScriptContextDuplicateTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.ctxOperations.SuspendLayout();
			this.ctxScriptContexts.SuspendLayout();
			this.SuspendLayout();
			// 
			// ctxOperations
			// 
			this.ctxOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.mniltbScriptContextRenameTo,
            this.mniltbScriptContextDuplicateTo,
            this.mniScriptContextDelete});
			this.ctxOperations.Name = "ctxOperations";
			this.ctxOperations.Size = new System.Drawing.Size(314, 80);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(310, 6);
			// 
			// mniScriptContextDelete
			// 
			this.mniScriptContextDelete.Name = "mniScriptContextDelete";
			this.mniScriptContextDelete.Size = new System.Drawing.Size(313, 22);
			this.mniScriptContextDelete.Text = "Delete [REPLACE_WITH_NAME]";
			this.mniScriptContextDelete.Click += new System.EventHandler(this.mniScriptContextDelete_Click);
			// 
			// mniParameterBagLoad
			// 
			this.mniParameterBagLoad.Name = "mniParameterBagLoad";
			this.mniParameterBagLoad.Size = new System.Drawing.Size(240, 22);
			this.mniParameterBagLoad.Text = "Load [REPLACE_WITH_NAME]";
			this.mniParameterBagLoad.Click += new System.EventHandler(this.mniScriptContextLoad_Click);
			// 
			// ctxScriptContexts
			// 
			this.ctxScriptContexts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniScriptContextsMniHeader,
            this.toolStripSeparator3,
            this.mniParameterBag_test1,
            this.mniParameterBag_test2,
            this.mniltbScriptContextNewWithDefaults,
            this.toolStripSeparator2,
            this.mniAllParamsResetToScriptDefaults,
            this.mniAllParamsShowBorder,
            this.mniAllParamsShowNumeric});
			this.ctxScriptContexts.Name = "ctxScriptContexts";
			this.ctxScriptContexts.Size = new System.Drawing.Size(300, 190);
			this.ctxScriptContexts.Opening += new System.ComponentModel.CancelEventHandler(this.ctxScriptContexts_Opening);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(296, 6);
			// 
			// mniParameterBag_test1
			// 
			this.mniParameterBag_test1.CheckOnClick = true;
			this.mniParameterBag_test1.Enabled = false;
			this.mniParameterBag_test1.Name = "mniParameterBag_test1";
			this.mniParameterBag_test1.Size = new System.Drawing.Size(299, 22);
			this.mniParameterBag_test1.Text = "RIZ-15min-100Bars-1share";
			// 
			// mniParameterBag_test2
			// 
			this.mniParameterBag_test2.CheckOnClick = true;
			this.mniParameterBag_test2.Enabled = false;
			this.mniParameterBag_test2.Name = "mniParameterBag_test2";
			this.mniParameterBag_test2.Size = new System.Drawing.Size(299, 22);
			this.mniParameterBag_test2.Text = "RIM-5min-All-$5000";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(296, 6);
			// 
			// mniAllParamsResetToScriptDefaults
			// 
			this.mniAllParamsResetToScriptDefaults.Name = "mniAllParamsResetToScriptDefaults";
			this.mniAllParamsResetToScriptDefaults.Size = new System.Drawing.Size(299, 22);
			this.mniAllParamsResetToScriptDefaults.Text = "All Params -> Reset To Script Defaults";
			this.mniAllParamsResetToScriptDefaults.Click += new System.EventHandler(this.mniAllParamsResetToScriptDefaults_Click);
			// 
			// mniAllParamsShowBorder
			// 
			this.mniAllParamsShowBorder.CheckOnClick = true;
			this.mniAllParamsShowBorder.Name = "mniAllParamsShowBorder";
			this.mniAllParamsShowBorder.Size = new System.Drawing.Size(299, 22);
			this.mniAllParamsShowBorder.Text = "All Params -> ShowBorder";
			this.mniAllParamsShowBorder.Click += new System.EventHandler(this.mniAllParamsShowBorder_Click);
			// 
			// mniAllParamsShowNumeric
			// 
			this.mniAllParamsShowNumeric.CheckOnClick = true;
			this.mniAllParamsShowNumeric.Name = "mniAllParamsShowNumeric";
			this.mniAllParamsShowNumeric.Size = new System.Drawing.Size(299, 22);
			this.mniAllParamsShowNumeric.Text = "All Params -> ShowNumeric";
			this.mniAllParamsShowNumeric.Click += new System.EventHandler(this.mniAllParamsShowNumeric_Click);
			// 
			// mniScriptContextsMniHeader
			// 
			this.mniScriptContextsMniHeader.BackColor = System.Drawing.Color.Transparent;
			this.mniScriptContextsMniHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniScriptContextsMniHeader.Name = "mniScriptContextsMniHeader";
			this.mniScriptContextsMniHeader.Size = new System.Drawing.Size(93, 15);
			this.mniScriptContextsMniHeader.Text = "Script Contexts";
			// 
			// mniltbScriptContextNewWithDefaults
			// 
			this.mniltbScriptContextNewWithDefaults.BackColor = System.Drawing.Color.Transparent;
			this.mniltbScriptContextNewWithDefaults.InputFieldAlignedRight = false;
			this.mniltbScriptContextNewWithDefaults.InputFieldEditable = true;
			this.mniltbScriptContextNewWithDefaults.InputFieldOffsetX = 85;
			this.mniltbScriptContextNewWithDefaults.InputFieldValue = "";
			this.mniltbScriptContextNewWithDefaults.InputFieldWidth = 90;
			this.mniltbScriptContextNewWithDefaults.Name = "mniltbScriptContextNewWithDefaults";
			this.mniltbScriptContextNewWithDefaults.Size = new System.Drawing.Size(240, 21);
			this.mniltbScriptContextNewWithDefaults.TextLeft = "Create Default";
			this.mniltbScriptContextNewWithDefaults.TextLeftOffsetX = 0;
			this.mniltbScriptContextNewWithDefaults.TextLeftWidth = 84;
			this.mniltbScriptContextNewWithDefaults.TextRed = false;
			this.mniltbScriptContextNewWithDefaults.TextRight = "~= Xpips";
			this.mniltbScriptContextNewWithDefaults.TextRightOffsetX = 180;
			this.mniltbScriptContextNewWithDefaults.TextRightWidth = 57;
			this.mniltbScriptContextNewWithDefaults.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextNewWithDefaults_UserTyped);
			// 
			// templateSliderControl
			// 
			this.templateSliderControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.templateSliderControl.ColorBgMouseOverEnabled = System.Drawing.Color.YellowGreen;
			this.templateSliderControl.ColorBgValueCurrent = System.Drawing.Color.LightSteelBlue;
			this.templateSliderControl.ColorFgParameterLabel = System.Drawing.Color.White;
			this.templateSliderControl.ColorFgValues = System.Drawing.Color.DeepPink;
			this.templateSliderControl.EnableBorder = true;
			this.templateSliderControl.EnableNumeric = true;
			this.templateSliderControl.FilledPercentageCurrentValue = 0;
			this.templateSliderControl.FilledPercentageMouseOver = 0;
			this.templateSliderControl.FillFromCurrentToMax = false;
			this.templateSliderControl.LabelText = "Length of Vehicle, cm";
			this.templateSliderControl.Location = new System.Drawing.Point(0, 0);
			this.templateSliderControl.Name = "templateSliderControl";
			this.templateSliderControl.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.templateSliderControl.PaddingPanelSlider = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.templateSliderControl.Size = new System.Drawing.Size(245, 17);
			this.templateSliderControl.TabIndex = 0;
			this.templateSliderControl.ValueCurrent = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.templateSliderControl.ValueIncrement = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.templateSliderControl.ValueMax = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.templateSliderControl.ValueMin = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// mniltbScriptContextRenameTo
			// 
			this.mniltbScriptContextRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbScriptContextRenameTo.InputFieldAlignedRight = false;
			this.mniltbScriptContextRenameTo.InputFieldEditable = true;
			this.mniltbScriptContextRenameTo.InputFieldOffsetX = 80;
			this.mniltbScriptContextRenameTo.InputFieldValue = "";
			this.mniltbScriptContextRenameTo.InputFieldWidth = 110;
			this.mniltbScriptContextRenameTo.Name = "mniltbScriptContextRenameTo";
			this.mniltbScriptContextRenameTo.Size = new System.Drawing.Size(247, 21);
			this.mniltbScriptContextRenameTo.TextLeft = "Rename To:";
			this.mniltbScriptContextRenameTo.TextLeftOffsetX = 0;
			this.mniltbScriptContextRenameTo.TextLeftWidth = 72;
			this.mniltbScriptContextRenameTo.TextRed = false;
			this.mniltbScriptContextRenameTo.TextRight = "~= Xpips";
			this.mniltbScriptContextRenameTo.TextRightOffsetX = 193;
			this.mniltbScriptContextRenameTo.TextRightWidth = 57;
			this.mniltbScriptContextRenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextRenameTo_UserTyped);
			// 
			// mniltbScriptContextDuplicateTo
			// 
			this.mniltbScriptContextDuplicateTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbScriptContextDuplicateTo.InputFieldAlignedRight = false;
			this.mniltbScriptContextDuplicateTo.InputFieldEditable = true;
			this.mniltbScriptContextDuplicateTo.InputFieldOffsetX = 80;
			this.mniltbScriptContextDuplicateTo.InputFieldValue = "";
			this.mniltbScriptContextDuplicateTo.InputFieldWidth = 110;
			this.mniltbScriptContextDuplicateTo.Name = "mniltbScriptContextDuplicateTo";
			this.mniltbScriptContextDuplicateTo.Size = new System.Drawing.Size(247, 21);
			this.mniltbScriptContextDuplicateTo.TextLeft = "Duplicate To:";
			this.mniltbScriptContextDuplicateTo.TextLeftOffsetX = 0;
			this.mniltbScriptContextDuplicateTo.TextLeftWidth = 79;
			this.mniltbScriptContextDuplicateTo.TextRed = false;
			this.mniltbScriptContextDuplicateTo.TextRight = "~= Xpips";
			this.mniltbScriptContextDuplicateTo.TextRightOffsetX = 193;
			this.mniltbScriptContextDuplicateTo.TextRightWidth = 57;
			this.mniltbScriptContextDuplicateTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextDuplicateTo_UserTyped);
			// 
			// SlidersAutoGrowControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ContextMenuStrip = this.ctxScriptContexts;
			this.Controls.Add(this.templateSliderControl);
			this.Name = "SlidersAutoGrowControl";
			this.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Size = new System.Drawing.Size(245, 61);
			this.ctxOperations.ResumeLayout(false);
			this.ctxScriptContexts.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbScriptContextDuplicateTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabel mniScriptContextsMniHeader;

		#endregion

		private Sq1.Widgets.SteppingSlider.SliderComboControl templateSliderControl;
		private System.Windows.Forms.ContextMenuStrip ctxOperations;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBagLoad;
		private System.Windows.Forms.ToolStripMenuItem mniScriptContextDelete;
		private System.Windows.Forms.ContextMenuStrip ctxScriptContexts;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbScriptContextRenameTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbScriptContextNewWithDefaults;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBag_test1;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBag_test2;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsResetToScriptDefaults;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsShowBorder;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsShowNumeric;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator3;

	}
}
