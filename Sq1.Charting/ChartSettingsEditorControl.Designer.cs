using System.Windows.Forms;

namespace Sq1.Charting {
	partial class ChartSettingsEditorControl {
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblChart = new System.Windows.Forms.ToolStripStatusLabel();
			this.cbxChartsCurrentlyOpen = new Sq1.Widgets.ToolStripImproved.ToolStripItemComboBox();
			this.lblSettings = new System.Windows.Forms.ToolStripStatusLabel();
			this.mniSettingsImEditing = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxAllSettingsAvailable = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxActions_forOneSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniSettingsMouseOvered_AssignToCurrentChart = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbSettingsMouseOvered_SaveAs = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbSettingsMouseOvered_Duplicate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbSettingsMouseOvered_RenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniSettings_AddNew = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniSettingsMouseOvered_Delete = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1.SuspendLayout();
			this.ctxActions_forOneSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.CommandsVisibleIfAvailable = false;
			this.propertyGrid1.Location = new System.Drawing.Point(0, -27);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(292, 590);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblChart,
            this.cbxChartsCurrentlyOpen,
            this.lblSettings,
            this.mniSettingsImEditing});
			this.statusStrip1.Location = new System.Drawing.Point(0, 500);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(292, 25);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// lblChart
			// 
			this.lblChart.Name = "lblChart";
			this.lblChart.Size = new System.Drawing.Size(42, 20);
			this.lblChart.Text = "Chart: ";
			this.lblChart.Visible = false;
			// 
			// cbxChartsCurrentlyOpen
			// 
			this.cbxChartsCurrentlyOpen.ComboBoxDropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			this.cbxChartsCurrentlyOpen.ComboBoxFormattingEnabled = false;
			this.cbxChartsCurrentlyOpen.ComboBoxLocation = new System.Drawing.Point(1, 3);
			this.cbxChartsCurrentlyOpen.ComboBoxSelectedIndex = -1;
			this.cbxChartsCurrentlyOpen.ComboBoxSize = new System.Drawing.Size(180, 21);
			this.cbxChartsCurrentlyOpen.ComboBoxSorted = false;
			this.cbxChartsCurrentlyOpen.Name = "cbxChartsCurrentlyOpen";
			this.cbxChartsCurrentlyOpen.Size = new System.Drawing.Size(180, 23);
			// 
			// lblSettings
			// 
			this.lblSettings.Name = "lblSettings";
			this.lblSettings.Size = new System.Drawing.Size(52, 20);
			this.lblSettings.Text = "Settings:";
			this.lblSettings.Visible = false;
			// 
			// mniSettingsImEditing
			// 
			this.mniSettingsImEditing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniSettingsImEditing.DropDown = this.ctxAllSettingsAvailable;
			this.mniSettingsImEditing.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniSettingsImEditing.Name = "mniSettingsImEditing";
			this.mniSettingsImEditing.Size = new System.Drawing.Size(240, 23);
			this.mniSettingsImEditing.Text = "WILL_POPULATE_WITH_SETTINGS_NAME";
			this.mniSettingsImEditing.Click += new System.EventHandler(this.mniSettingsImEditing_Click);
			// 
			// ctxAllSettingsAvailable
			// 
			this.ctxAllSettingsAvailable.Name = "ctxTemplates";
			this.ctxAllSettingsAvailable.OwnerItem = this.mniSettingsImEditing;
			this.ctxAllSettingsAvailable.Size = new System.Drawing.Size(61, 4);
			this.ctxAllSettingsAvailable.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAllSettingsAvailable_Opening);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem5.Text = "toolStripMenuItem5";
			// 
			// ctxActions_forOneSettings
			// 
			this.ctxActions_forOneSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniSettingsMouseOvered_AssignToCurrentChart,
            this.mniltbSettingsMouseOvered_SaveAs,
            this.toolStripSeparator2,
            this.mniltbSettingsMouseOvered_Duplicate,
            this.mniltbSettingsMouseOvered_RenameTo,
            this.toolStripSeparator1,
            this.mniSettings_AddNew,
            this.mniSettingsMouseOvered_Delete});
			this.ctxActions_forOneSettings.MinimumSize = new System.Drawing.Size(300, 0);
			this.ctxActions_forOneSettings.Name = "ctxActions_forOneSettings";
			this.ctxActions_forOneSettings.Size = new System.Drawing.Size(311, 178);
			// 
			// mniSettingsMouseOvered_AssignToCurrentChart
			// 
			this.mniSettingsMouseOvered_AssignToCurrentChart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.mniSettingsMouseOvered_AssignToCurrentChart.Name = "mniSettingsMouseOvered_AssignToCurrentChart";
			this.mniSettingsMouseOvered_AssignToCurrentChart.Size = new System.Drawing.Size(310, 22);
			this.mniSettingsMouseOvered_AssignToCurrentChart.Text = "Load into Chart [vvv]";
			this.mniSettingsMouseOvered_AssignToCurrentChart.Click += new System.EventHandler(this.mniSettingsMouseOvered_AssignToCurrentChart_Click);
			// 
			// mniltbSettingsMouseOvered_SaveAs
			// 
			this.mniltbSettingsMouseOvered_SaveAs.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldAlignedRight = false;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldEditable = true;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldMultiline = true;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldOffsetX = 80;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldValue = "www";
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldWidth = 160;
			this.mniltbSettingsMouseOvered_SaveAs.Name = "mniltbSettingsMouseOvered_SaveAs";
			this.mniltbSettingsMouseOvered_SaveAs.OffsetTop = 0;
			this.mniltbSettingsMouseOvered_SaveAs.Size = new System.Drawing.Size(250, 21);
			this.mniltbSettingsMouseOvered_SaveAs.Text = "Save Current:";
			this.mniltbSettingsMouseOvered_SaveAs.TextLeft = "Save Current:";
			this.mniltbSettingsMouseOvered_SaveAs.TextLeftOffsetX = 0;
			this.mniltbSettingsMouseOvered_SaveAs.TextLeftWidth = 79;
			this.mniltbSettingsMouseOvered_SaveAs.TextRed = false;
			this.mniltbSettingsMouseOvered_SaveAs.TextRight = "";
			this.mniltbSettingsMouseOvered_SaveAs.TextRightOffsetX = 243;
			this.mniltbSettingsMouseOvered_SaveAs.TextRightWidth = 4;
			this.mniltbSettingsMouseOvered_SaveAs.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSettingsMouseOvered_SaveAs_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(307, 6);
			// 
			// mniltbSettingsMouseOvered_Duplicate
			// 
			this.mniltbSettingsMouseOvered_Duplicate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldAlignedRight = false;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldEditable = true;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldMultiline = true;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldOffsetX = 80;
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldValue = "xxxx";
			this.mniltbSettingsMouseOvered_Duplicate.InputFieldWidth = 160;
			this.mniltbSettingsMouseOvered_Duplicate.Name = "mniltbSettingsMouseOvered_Duplicate";
			this.mniltbSettingsMouseOvered_Duplicate.OffsetTop = 0;
			this.mniltbSettingsMouseOvered_Duplicate.Size = new System.Drawing.Size(250, 21);
			this.mniltbSettingsMouseOvered_Duplicate.Text = "Duplicate To:";
			this.mniltbSettingsMouseOvered_Duplicate.TextLeft = "Duplicate To:";
			this.mniltbSettingsMouseOvered_Duplicate.TextLeftOffsetX = 0;
			this.mniltbSettingsMouseOvered_Duplicate.TextLeftWidth = 79;
			this.mniltbSettingsMouseOvered_Duplicate.TextRed = false;
			this.mniltbSettingsMouseOvered_Duplicate.TextRight = "";
			this.mniltbSettingsMouseOvered_Duplicate.TextRightOffsetX = 243;
			this.mniltbSettingsMouseOvered_Duplicate.TextRightWidth = 4;
			this.mniltbSettingsMouseOvered_Duplicate.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSettingsMouseOvered_Duplicate_UserTyped);
			// 
			// mniltbSettingsMouseOvered_RenameTo
			// 
			this.mniltbSettingsMouseOvered_RenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldAlignedRight = false;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldEditable = true;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldMultiline = true;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldOffsetX = 80;
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldValue = "yyyy";
			this.mniltbSettingsMouseOvered_RenameTo.InputFieldWidth = 160;
			this.mniltbSettingsMouseOvered_RenameTo.Name = "mniltbSettingsMouseOvered_RenameTo";
			this.mniltbSettingsMouseOvered_RenameTo.OffsetTop = 0;
			this.mniltbSettingsMouseOvered_RenameTo.Size = new System.Drawing.Size(250, 21);
			this.mniltbSettingsMouseOvered_RenameTo.Text = "Rename To:";
			this.mniltbSettingsMouseOvered_RenameTo.TextLeft = "Rename To:";
			this.mniltbSettingsMouseOvered_RenameTo.TextLeftOffsetX = 0;
			this.mniltbSettingsMouseOvered_RenameTo.TextLeftWidth = 72;
			this.mniltbSettingsMouseOvered_RenameTo.TextRed = false;
			this.mniltbSettingsMouseOvered_RenameTo.TextRight = "";
			this.mniltbSettingsMouseOvered_RenameTo.TextRightOffsetX = 243;
			this.mniltbSettingsMouseOvered_RenameTo.TextRightWidth = 4;
			this.mniltbSettingsMouseOvered_RenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSettingsMouseOvered_RenameTo_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(307, 6);
			// 
			// mniSettings_AddNew
			// 
			this.mniSettings_AddNew.BackColor = System.Drawing.Color.Transparent;
			this.mniSettings_AddNew.InputFieldAlignedRight = false;
			this.mniSettings_AddNew.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniSettings_AddNew.InputFieldEditable = true;
			this.mniSettings_AddNew.InputFieldMultiline = true;
			this.mniSettings_AddNew.InputFieldOffsetX = 80;
			this.mniSettings_AddNew.InputFieldValue = "zzzz";
			this.mniSettings_AddNew.InputFieldWidth = 160;
			this.mniSettings_AddNew.Name = "mniSettings_AddNew";
			this.mniSettings_AddNew.OffsetTop = 0;
			this.mniSettings_AddNew.Size = new System.Drawing.Size(250, 21);
			this.mniSettings_AddNew.Text = "Add New:";
			this.mniSettings_AddNew.TextLeft = "Add New:";
			this.mniSettings_AddNew.TextLeftOffsetX = 0;
			this.mniSettings_AddNew.TextLeftWidth = 61;
			this.mniSettings_AddNew.TextRed = false;
			this.mniSettings_AddNew.TextRight = "";
			this.mniSettings_AddNew.TextRightOffsetX = 243;
			this.mniSettings_AddNew.TextRightWidth = 4;
			this.mniSettings_AddNew.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniSettings_AddNew_UserTyped);
			// 
			// mniSettingsMouseOvered_Delete
			// 
			this.mniSettingsMouseOvered_Delete.Name = "mniSettingsMouseOvered_Delete";
			this.mniSettingsMouseOvered_Delete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniSettingsMouseOvered_Delete.Size = new System.Drawing.Size(310, 22);
			this.mniSettingsMouseOvered_Delete.Text = "Delete [Default]";
			this.mniSettingsMouseOvered_Delete.Click += new System.EventHandler(this.mniSettingsMouseOvered_Delete_Click);
			// 
			// ChartSettingsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "ChartSettingsEditorControl";
			this.Size = new System.Drawing.Size(292, 525);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ctxActions_forOneSettings.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private ToolStripMenuItem toolStripMenuItem5;
		public Widgets.ToolStripImproved.ToolStripItemComboBox cbxChartsCurrentlyOpen;
		private ToolStripDropDownButton mniSettingsImEditing;
		private ContextMenuStrip ctxAllSettingsAvailable;
		private ContextMenuStrip ctxActions_forOneSettings;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbSettingsMouseOvered_Duplicate;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbSettingsMouseOvered_RenameTo;
		private ToolStripSeparator toolStripSeparator1;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniSettings_AddNew;
		private ToolStripMenuItem mniSettingsMouseOvered_Delete;
		private ToolStripMenuItem mniSettingsMouseOvered_AssignToCurrentChart;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbSettingsMouseOvered_SaveAs;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripStatusLabel lblSettings;
		private ToolStripStatusLabel lblChart;
	}
}
