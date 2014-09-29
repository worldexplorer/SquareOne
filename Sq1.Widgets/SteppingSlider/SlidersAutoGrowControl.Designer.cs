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
			this.mniParameterBagLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbParameterBagRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbParameterBagDuplicateTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniParameterBagDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxParameterBags = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniParameterBagsNotHighlighted = new Sq1.Widgets.LabeledTextBox.MenuItemLabel();
			this.mniParameterBag_test1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniParameterBag_test2 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbParametersBagNewWithDefaults = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniAllParamsResetToScriptDefaults = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAllParamsShowBorder = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAllParamsShowNumeric = new System.Windows.Forms.ToolStripMenuItem();
			this.templateSliderControl = new Sq1.Widgets.SteppingSlider.SliderComboControl();
			this.ctxOperations.SuspendLayout();
			this.ctxParameterBags.SuspendLayout();
			this.SuspendLayout();
			// 
			// ctxOperations
			// 
			this.ctxOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									//FIRST_LEVEL_MNI_HAS_VISUAL_TICK this.mniParameterBagLoad,
									this.mniltbParameterBagRenameTo,
									this.mniltbParameterBagDuplicateTo,
									this.mniParameterBagDelete});
			this.ctxOperations.Name = "contextMenuStrip1";
			this.ctxOperations.Size = new System.Drawing.Size(241, 96);
			// 
			// mniParameterBagLoad
			// 
			this.mniParameterBagLoad.Name = "mniParameterBagLoad";
			this.mniParameterBagLoad.Size = new System.Drawing.Size(240, 22);
			this.mniParameterBagLoad.Text = "Load [REPLACE_WITH_NAME]";
			this.mniParameterBagLoad.Click += new System.EventHandler(this.mniScriptContextLoad_Click);
			// 
			// mniltbParameterBagRenameTo
			// 
			this.mniltbParameterBagRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbParameterBagRenameTo.InputFieldOffsetX = 80;
			this.mniltbParameterBagRenameTo.InputFieldValue = "";
			this.mniltbParameterBagRenameTo.InputFieldWidth = 85;
			this.mniltbParameterBagRenameTo.Name = "mniltbParameterBagRenameTo";
			this.mniltbParameterBagRenameTo.Size = new System.Drawing.Size(165, 21);
			this.mniltbParameterBagRenameTo.Text = "Rename To:";
			this.mniltbParameterBagRenameTo.TextRed = false;
			this.mniltbParameterBagRenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextRenameTo_UserTyped);
			// 
			// mniltbParameterBagDuplicateTo
			// 
			this.mniltbParameterBagDuplicateTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbParameterBagDuplicateTo.InputFieldOffsetX = 80;
			this.mniltbParameterBagDuplicateTo.InputFieldValue = "";
			this.mniltbParameterBagDuplicateTo.InputFieldWidth = 85;
			this.mniltbParameterBagDuplicateTo.Name = "mniltbParameterBagDuplicateTo";
			this.mniltbParameterBagDuplicateTo.Size = new System.Drawing.Size(165, 21);
			this.mniltbParameterBagDuplicateTo.Text = "Duplicate To:";
			this.mniltbParameterBagDuplicateTo.TextRed = false;
			this.mniltbParameterBagDuplicateTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextDuplicateTo_UserTyped);
			// 
			// mniParameterBagDelete
			// 
			this.mniParameterBagDelete.Name = "mniParameterBagDelete";
			this.mniParameterBagDelete.Size = new System.Drawing.Size(240, 22);
			this.mniParameterBagDelete.Text = "Delete [REPLACE_WITH_NAME]";
			this.mniParameterBagDelete.Click += new System.EventHandler(this.mniScriptContextDelete_Click);
			// 
			// ctxParameterBags
			// 
			this.ctxParameterBags.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniParameterBagsNotHighlighted,
									this.mniParameterBag_test1,
									this.mniParameterBag_test2,
									this.mniltbParametersBagNewWithDefaults,
									this.toolStripSeparator2,
									this.mniAllParamsResetToScriptDefaults,
									this.mniAllParamsShowBorder,
									this.mniAllParamsShowNumeric});
			this.ctxParameterBags.Name = "ctxParameterSets";
			this.ctxParameterBags.Size = new System.Drawing.Size(226, 140);
			this.ctxParameterBags.Opening += new System.ComponentModel.CancelEventHandler(this.ctxParameterBags_Opening);
			// 
			// mniParameterBagsNotHighlighted
			// 
			this.mniParameterBagsNotHighlighted.BackColor = System.Drawing.Color.Transparent;
			this.mniParameterBagsNotHighlighted.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniParameterBagsNotHighlighted.Name = "mniParameterBagsNotHighlighted";
			this.mniParameterBagsNotHighlighted.Size = new System.Drawing.Size(95, 15);
			this.mniParameterBagsNotHighlighted.Text = "Parameter Bags";
			// 
			// mniParameterBag_test1
			// 
			this.mniParameterBag_test1.CheckOnClick = true;
			this.mniParameterBag_test1.Enabled = false;
			this.mniParameterBag_test1.Name = "mniParameterBag_test1";
			this.mniParameterBag_test1.Size = new System.Drawing.Size(225, 22);
			this.mniParameterBag_test1.Text = "RIZ-15min-100Bars-1share";
			// 
			// mniParameterBag_test2
			// 
			this.mniParameterBag_test2.CheckOnClick = true;
			this.mniParameterBag_test2.Enabled = false;
			this.mniParameterBag_test2.Name = "mniParameterBag_test2";
			this.mniParameterBag_test2.Size = new System.Drawing.Size(225, 22);
			this.mniParameterBag_test2.Text = "RIM-5min-All-$5000";
			// 
			// mniltbParametersBagNewWithDefaults
			// 
			this.mniltbParametersBagNewWithDefaults.BackColor = System.Drawing.Color.Transparent;
			this.mniltbParametersBagNewWithDefaults.InputFieldOffsetX = 80;
			this.mniltbParametersBagNewWithDefaults.InputFieldValue = "";
			this.mniltbParametersBagNewWithDefaults.InputFieldWidth = 85;
			this.mniltbParametersBagNewWithDefaults.Name = "mniltbParametersBagNewWithDefaults";
			this.mniltbParametersBagNewWithDefaults.Size = new System.Drawing.Size(165, 21);
			this.mniltbParametersBagNewWithDefaults.Text = "New clean";
			this.mniltbParametersBagNewWithDefaults.TextRed = false;
			this.mniltbParametersBagNewWithDefaults.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextNewWithDefaults_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
			// 
			// mniAllParamsResetToScriptDefaults
			// 
			this.mniAllParamsResetToScriptDefaults.Name = "mniAllParamsResetToScriptDefaults";
			this.mniAllParamsResetToScriptDefaults.Size = new System.Drawing.Size(225, 22);
			this.mniAllParamsResetToScriptDefaults.Text = "All Params -> Script Defaults";
			// 
			// mniAllParamsShowBorder
			// 
			this.mniAllParamsShowBorder.Name = "mniAllParamsShowBorder";
			this.mniAllParamsShowBorder.Size = new System.Drawing.Size(225, 22);
			this.mniAllParamsShowBorder.Text = "All Params -> ShowBorder";
			this.mniAllParamsShowBorder.Click += new EventHandler(mniAllParamsShowBorder_Click);
			// 
			// mniAllParamsShowNumeric
			// 
			this.mniAllParamsShowNumeric.Name = "mniAllParamsShowNumeric";
			this.mniAllParamsShowNumeric.Size = new System.Drawing.Size(225, 22);
			this.mniAllParamsShowNumeric.Text = "All Params -> ShowNumeric";
			this.mniAllParamsShowNumeric.Click += new EventHandler(mniAllParamsShowNumeric_Click);
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
			this.templateSliderControl.ValueIncrement = new decimal(new int[] {
									100,
									0,
									0,
									0});
			// 
			// SlidersAutoGrow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ContextMenuStrip = this.ctxParameterBags;
			this.Controls.Add(this.templateSliderControl);
			this.Name = "SlidersAutoGrow";
			this.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Size = new System.Drawing.Size(245, 61);
			this.ctxOperations.ResumeLayout(false);
			this.ctxParameterBags.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbParameterBagDuplicateTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabel mniParameterBagsNotHighlighted;

		#endregion

		private Sq1.Widgets.SteppingSlider.SliderComboControl templateSliderControl;
		private System.Windows.Forms.ContextMenuStrip ctxOperations;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBagLoad;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBagDelete;
		private System.Windows.Forms.ContextMenuStrip ctxParameterBags;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbParameterBagRenameTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbParametersBagNewWithDefaults;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBag_test1;
		private System.Windows.Forms.ToolStripMenuItem mniParameterBag_test2;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsResetToScriptDefaults;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsShowBorder;
		private System.Windows.Forms.ToolStripMenuItem mniAllParamsShowNumeric;

	}
}
