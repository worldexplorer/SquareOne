using System.Windows.Forms;
namespace Sq1.Widgets.SymbolEditor {
	partial class SymbolInfoEditorControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolInfoEditorControl));
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsiCbxSymbols = new Sq1.Widgets.ToolStripImproved.ToolStripItemComboBox();
			this.tsmniModify = new System.Windows.Forms.ToolStripDropDownButton();
			this.mniDeleteSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbAddNew = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbRename = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbDuplicate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniPushTo = new System.Windows.Forms.ToolStripMenuItem();
			this.triangleTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mniPullFrom = new System.Windows.Forms.ToolStripMenuItem();
			this.triangleTriggerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1.SuspendLayout();
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
			this.propertyGrid1.Size = new System.Drawing.Size(234, 546);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiCbxSymbols,
            this.tsmniModify});
			this.statusStrip1.Location = new System.Drawing.Point(0, 515);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(234, 25);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tsiCbxSymbols
			// 
			this.tsiCbxSymbols.ComboBoxDropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tsiCbxSymbols.ComboBoxFormattingEnabled = true;
			this.tsiCbxSymbols.ComboBoxLocation = new System.Drawing.Point(1, 3);
			this.tsiCbxSymbols.ComboBoxSelectedIndex = -1;
			this.tsiCbxSymbols.ComboBoxSize = new System.Drawing.Size(140, 21);
			this.tsiCbxSymbols.ComboBoxSorted = true;
			this.tsiCbxSymbols.Name = "tsiCbxSymbols";
			this.tsiCbxSymbols.Size = new System.Drawing.Size(140, 23);
			this.tsiCbxSymbols.ComboBoxDropDown += new System.EventHandler<System.EventArgs>(this.toolStripItemComboBox1_DropDown);
			this.tsiCbxSymbols.ComboBoxSelectedIndexChanged += new System.EventHandler<System.EventArgs>(this.toolStripItemComboBox1_SelectedIndexChanged);
			// 
			// tsmniModify
			// 
			this.tsmniModify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmniModify.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniDeleteSymbol,
            this.mniltbAddNew,
            this.toolStripSeparator1,
            this.mniltbRename,
            this.mniltbDuplicate,
            this.mniPushTo,
            this.mniPullFrom});
			this.tsmniModify.Image = ((System.Drawing.Image)(resources.GetObject("tsmniModify.Image")));
			this.tsmniModify.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmniModify.Name = "tsmniModify";
			this.tsmniModify.Size = new System.Drawing.Size(58, 23);
			this.tsmniModify.Text = "Modify";
			this.tsmniModify.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsmniModify.DropDownOpening += new System.EventHandler(this.tsmniModify_DropDownOpening);
			// 
			// mniDeleteSymbol
			// 
			this.mniDeleteSymbol.Name = "mniDeleteSymbol";
			this.mniDeleteSymbol.Size = new System.Drawing.Size(211, 22);
			this.mniDeleteSymbol.Text = "Delete [RIM3]";
			this.mniDeleteSymbol.Click += new System.EventHandler(this.mniDeleteSymbol_Click);
			// 
			// mniltbAddNew
			// 
			this.mniltbAddNew.BackColor = System.Drawing.Color.Transparent;
			this.mniltbAddNew.InputFieldAlignedRight = false;
			this.mniltbAddNew.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbAddNew.InputFieldEditable = true;
			this.mniltbAddNew.InputFieldMultiline = true;
			this.mniltbAddNew.InputFieldOffsetX = 80;
			this.mniltbAddNew.InputFieldValue = "RIM3";
			this.mniltbAddNew.InputFieldWidth = 60;
			this.mniltbAddNew.Name = "mniltbAddNew";
			this.mniltbAddNew.OffsetTop = 0;
			this.mniltbAddNew.Size = new System.Drawing.Size(150, 21);
			this.mniltbAddNew.Text = "Add New:";
			this.mniltbAddNew.TextLeft = "Add New:";
			this.mniltbAddNew.TextLeftOffsetX = 0;
			this.mniltbAddNew.TextLeftWidth = 61;
			this.mniltbAddNew.TextRed = false;
			this.mniltbAddNew.TextRight = "";
			this.mniltbAddNew.TextRightOffsetX = 143;
			this.mniltbAddNew.TextRightWidth = 4;
			this.mniltbAddNew.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbAddNew_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
			// 
			// mniltbRename
			// 
			this.mniltbRename.BackColor = System.Drawing.Color.Transparent;
			this.mniltbRename.InputFieldAlignedRight = false;
			this.mniltbRename.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbRename.InputFieldEditable = true;
			this.mniltbRename.InputFieldMultiline = true;
			this.mniltbRename.InputFieldOffsetX = 80;
			this.mniltbRename.InputFieldValue = "RIM3";
			this.mniltbRename.InputFieldWidth = 60;
			this.mniltbRename.Name = "mniltbRename";
			this.mniltbRename.OffsetTop = 0;
			this.mniltbRename.Size = new System.Drawing.Size(150, 21);
			this.mniltbRename.Text = "Rename To:";
			this.mniltbRename.TextLeft = "Rename To:";
			this.mniltbRename.TextLeftOffsetX = 0;
			this.mniltbRename.TextLeftWidth = 72;
			this.mniltbRename.TextRed = false;
			this.mniltbRename.TextRight = "";
			this.mniltbRename.TextRightOffsetX = 143;
			this.mniltbRename.TextRightWidth = 4;
			this.mniltbRename.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbRename_UserTyped);
			// 
			// mniltbDuplicate
			// 
			this.mniltbDuplicate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDuplicate.InputFieldAlignedRight = false;
			this.mniltbDuplicate.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbDuplicate.InputFieldEditable = true;
			this.mniltbDuplicate.InputFieldMultiline = true;
			this.mniltbDuplicate.InputFieldOffsetX = 80;
			this.mniltbDuplicate.InputFieldValue = "RIM3";
			this.mniltbDuplicate.InputFieldWidth = 60;
			this.mniltbDuplicate.Name = "mniltbDuplicate";
			this.mniltbDuplicate.OffsetTop = 0;
			this.mniltbDuplicate.Size = new System.Drawing.Size(150, 21);
			this.mniltbDuplicate.Text = "Duplicate To:";
			this.mniltbDuplicate.TextLeft = "Duplicate To:";
			this.mniltbDuplicate.TextLeftOffsetX = 0;
			this.mniltbDuplicate.TextLeftWidth = 79;
			this.mniltbDuplicate.TextRed = false;
			this.mniltbDuplicate.TextRight = "";
			this.mniltbDuplicate.TextRightOffsetX = 143;
			this.mniltbDuplicate.TextRightWidth = 4;
			this.mniltbDuplicate.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDuplicate_UserTyped);
			// 
			// mniPushTo
			// 
			this.mniPushTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.triangleTriggerToolStripMenuItem});
			this.mniPushTo.Name = "mniPushTo";
			this.mniPushTo.Size = new System.Drawing.Size(211, 22);
			this.mniPushTo.Text = "Push To Existing Symbol";
			this.mniPushTo.DropDownOpening += new System.EventHandler(this.mniPushTo_DropDownOpening);
			// 
			// triangleTriggerToolStripMenuItem
			// 
			this.triangleTriggerToolStripMenuItem.Name = "triangleTriggerToolStripMenuItem";
			this.triangleTriggerToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.triangleTriggerToolStripMenuItem.Text = "TriangleTrigger";
			// 
			// mniPullFrom
			// 
			this.mniPullFrom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.triangleTriggerToolStripMenuItem1});
			this.mniPullFrom.Name = "mniPullFrom";
			this.mniPullFrom.Size = new System.Drawing.Size(211, 22);
			this.mniPullFrom.Text = "Pull From Existing Symbol";
			this.mniPullFrom.DropDownOpening += new System.EventHandler(this.mniPullFrom_DropDownOpening);
			// 
			// triangleTriggerToolStripMenuItem1
			// 
			this.triangleTriggerToolStripMenuItem1.Name = "triangleTriggerToolStripMenuItem1";
			this.triangleTriggerToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
			this.triangleTriggerToolStripMenuItem1.Text = "TriangleTrigger";
			// 
			// SymbolInfoEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "SymbolInfoEditorControl";
			this.Size = new System.Drawing.Size(234, 540);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripDropDownButton tsmniModify;
		private System.Windows.Forms.ToolStripMenuItem mniDeleteSymbol;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbRename;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbDuplicate;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbAddNew;
		private ToolStripImproved.ToolStripItemComboBox tsiCbxSymbols;
		private ToolStripMenuItem mniPushTo;
		private ToolStripMenuItem mniPullFrom;
		private ToolStripMenuItem triangleTriggerToolStripMenuItem;
		private ToolStripMenuItem triangleTriggerToolStripMenuItem1;
	}
}
