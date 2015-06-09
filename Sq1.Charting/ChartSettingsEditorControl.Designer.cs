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
			this.toolStripItemComboBox1 = new Sq1.Widgets.ToolStripImproved.ToolStripItemComboBox();
			this.mniAbsorbFromChart = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxTemplates = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxTemplateActions = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbSaveCurrentAs = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbDuplicate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniAddNew = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1.SuspendLayout();
			this.ctxTemplateActions.SuspendLayout();
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
			this.propertyGrid1.Size = new System.Drawing.Size(234, 605);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemComboBox1,
            this.mniAbsorbFromChart});
			this.statusStrip1.Location = new System.Drawing.Point(0, 515);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(234, 25);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripItemComboBox1
			// 
			this.toolStripItemComboBox1.Name = "toolStripItemComboBox1";
			this.toolStripItemComboBox1.Size = new System.Drawing.Size(160, 23);
			// 
			// mniAbsorbFromChart
			// 
			this.mniAbsorbFromChart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniAbsorbFromChart.DropDown = this.ctxTemplates;
			this.mniAbsorbFromChart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniAbsorbFromChart.Name = "mniAbsorbFromChart";
			this.mniAbsorbFromChart.Size = new System.Drawing.Size(75, 23);
			this.mniAbsorbFromChart.Text = "Templates";
			this.mniAbsorbFromChart.Click += new System.EventHandler(this.mniAbsorbFromChart_Click);
			// 
			// ctxTemplates
			// 
			this.ctxTemplates.Name = "ctxTemplates";
			this.ctxTemplates.OwnerItem = this.mniAbsorbFromChart;
			this.ctxTemplates.Size = new System.Drawing.Size(61, 4);
			this.ctxTemplates.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTemplates_Opening);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem5.Text = "toolStripMenuItem5";
			// 
			// ctxTemplateActions
			// 
			this.ctxTemplateActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniLoad,
            this.mniltbSaveCurrentAs,
            this.toolStripSeparator2,
            this.mniltbDuplicate,
            this.mniltbRenameTo,
            this.toolStripSeparator1,
            this.mniAddNew,
            this.mniDelete});
			this.ctxTemplateActions.MinimumSize = new System.Drawing.Size(300, 0);
			this.ctxTemplateActions.Name = "ctxTemplateActions";
			this.ctxTemplateActions.Size = new System.Drawing.Size(309, 178);
			// 
			// mniLoad
			// 
			this.mniLoad.Name = "mniLoad";
			this.mniLoad.Size = new System.Drawing.Size(308, 22);
			this.mniLoad.Text = "Load [vvv]";
			this.mniLoad.Visible = false;
			this.mniLoad.Click += new System.EventHandler(this.mniLoad_Click);
			// 
			// mniltbSaveCurrentAs
			// 
			this.mniltbSaveCurrentAs.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSaveCurrentAs.InputFieldAlignedRight = false;
			this.mniltbSaveCurrentAs.InputFieldEditable = true;
			this.mniltbSaveCurrentAs.InputFieldOffsetX = 80;
			this.mniltbSaveCurrentAs.InputFieldValue = "www";
			this.mniltbSaveCurrentAs.InputFieldWidth = 160;
			this.mniltbSaveCurrentAs.Name = "mniltbSaveCurrentAs";
			this.mniltbSaveCurrentAs.Size = new System.Drawing.Size(248, 21);
			this.mniltbSaveCurrentAs.Text = "Save Current:";
			this.mniltbSaveCurrentAs.TextLeft = "Save Current:";
			this.mniltbSaveCurrentAs.TextLeftOffsetX = 0;
			this.mniltbSaveCurrentAs.TextLeftWidth = 79;
			this.mniltbSaveCurrentAs.TextRed = false;
			this.mniltbSaveCurrentAs.TextRight = "";
			this.mniltbSaveCurrentAs.TextRightOffsetX = 243;
			this.mniltbSaveCurrentAs.TextRightWidth = 2;
			this.mniltbSaveCurrentAs.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSaveCurrentAs_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(305, 6);
			// 
			// mniltbDuplicate
			// 
			this.mniltbDuplicate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDuplicate.InputFieldAlignedRight = false;
			this.mniltbDuplicate.InputFieldEditable = true;
			this.mniltbDuplicate.InputFieldOffsetX = 80;
			this.mniltbDuplicate.InputFieldValue = "xxxx";
			this.mniltbDuplicate.InputFieldWidth = 160;
			this.mniltbDuplicate.Name = "mniltbDuplicate";
			this.mniltbDuplicate.Size = new System.Drawing.Size(248, 21);
			this.mniltbDuplicate.Text = "Duplicate To:";
			this.mniltbDuplicate.TextLeft = "Duplicate To:";
			this.mniltbDuplicate.TextLeftOffsetX = 0;
			this.mniltbDuplicate.TextLeftWidth = 79;
			this.mniltbDuplicate.TextRed = false;
			this.mniltbDuplicate.TextRight = "";
			this.mniltbDuplicate.TextRightOffsetX = 243;
			this.mniltbDuplicate.TextRightWidth = 2;
			this.mniltbDuplicate.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDuplicate_UserTyped);
			// 
			// mniltbRenameTo
			// 
			this.mniltbRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbRenameTo.InputFieldAlignedRight = false;
			this.mniltbRenameTo.InputFieldEditable = true;
			this.mniltbRenameTo.InputFieldOffsetX = 80;
			this.mniltbRenameTo.InputFieldValue = "yyyy";
			this.mniltbRenameTo.InputFieldWidth = 160;
			this.mniltbRenameTo.Name = "mniltbRenameTo";
			this.mniltbRenameTo.Size = new System.Drawing.Size(248, 21);
			this.mniltbRenameTo.Text = "Rename To:";
			this.mniltbRenameTo.TextLeft = "Rename To:";
			this.mniltbRenameTo.TextLeftOffsetX = 0;
			this.mniltbRenameTo.TextLeftWidth = 72;
			this.mniltbRenameTo.TextRed = false;
			this.mniltbRenameTo.TextRight = "";
			this.mniltbRenameTo.TextRightOffsetX = 243;
			this.mniltbRenameTo.TextRightWidth = 2;
			this.mniltbRenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbRenameTo_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(305, 6);
			// 
			// mniAddNew
			// 
			this.mniAddNew.BackColor = System.Drawing.Color.Transparent;
			this.mniAddNew.InputFieldAlignedRight = false;
			this.mniAddNew.InputFieldEditable = true;
			this.mniAddNew.InputFieldOffsetX = 80;
			this.mniAddNew.InputFieldValue = "zzzz";
			this.mniAddNew.InputFieldWidth = 160;
			this.mniAddNew.Name = "mniAddNew";
			this.mniAddNew.Size = new System.Drawing.Size(248, 21);
			this.mniAddNew.Text = "Add New:";
			this.mniAddNew.TextLeft = "Add New:";
			this.mniAddNew.TextLeftOffsetX = 0;
			this.mniAddNew.TextLeftWidth = 61;
			this.mniAddNew.TextRed = false;
			this.mniAddNew.TextRight = "";
			this.mniAddNew.TextRightOffsetX = 243;
			this.mniAddNew.TextRightWidth = 2;
			this.mniAddNew.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniAddNew_UserTyped);
			// 
			// mniDelete
			// 
			this.mniDelete.Name = "mniDelete";
			this.mniDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniDelete.Size = new System.Drawing.Size(308, 22);
			this.mniDelete.Text = "Delete [Default]";
			this.mniDelete.Click += new System.EventHandler(this.mniDelete_Click);
			// 
			// ChartSettingsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "ChartSettingsEditorControl";
			this.Size = new System.Drawing.Size(234, 540);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ctxTemplateActions.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private ToolStripMenuItem toolStripMenuItem5;
		public Widgets.ToolStripImproved.ToolStripItemComboBox toolStripItemComboBox1;
		private ToolStripDropDownButton mniAbsorbFromChart;
		private ContextMenuStrip ctxTemplates;
		private ContextMenuStrip ctxTemplateActions;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbDuplicate;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbRenameTo;
		private ToolStripSeparator toolStripSeparator1;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniAddNew;
		private ToolStripMenuItem mniDelete;
		private ToolStripMenuItem mniLoad;
		private Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbSaveCurrentAs;
		private ToolStripSeparator toolStripSeparator2;
	}
}
