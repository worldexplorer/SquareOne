using System.Windows.Forms;
namespace Sq1.Widgets.SymbolEditor {
	partial class SymbolEditorControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolEditorControl));
			this.ctxEachSymbolInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniDuplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAppend = new System.Windows.Forms.ToolStripMenuItem();
			this.mniDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsmniSymbol = new System.Windows.Forms.ToolStripDropDownButton();
			this.mniDeleteSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAddNewSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbRename = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbDuplicate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
			this.tsmniSymbolsTree = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.ctxEachSymbolInfo.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ctxEachSymbolInfo
			// 
			this.ctxEachSymbolInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniEdit,
            this.mniDuplicate,
            this.mniAppend,
            this.mniDelete});
			this.ctxEachSymbolInfo.Name = "ctxEachSymbolInfo";
			this.ctxEachSymbolInfo.Size = new System.Drawing.Size(146, 92);
			// 
			// mniEdit
			// 
			this.mniEdit.Name = "mniEdit";
			this.mniEdit.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mniEdit.Size = new System.Drawing.Size(145, 22);
			this.mniEdit.Text = "Edit";
			// 
			// mniDuplicate
			// 
			this.mniDuplicate.Name = "mniDuplicate";
			this.mniDuplicate.Size = new System.Drawing.Size(145, 22);
			this.mniDuplicate.Text = "Duplicate";
			// 
			// mniAppend
			// 
			this.mniAppend.Name = "mniAppend";
			this.mniAppend.ShortcutKeys = System.Windows.Forms.Keys.Insert;
			this.mniAppend.Size = new System.Drawing.Size(145, 22);
			this.mniAppend.Text = "Add New";
			// 
			// mniDelete
			// 
			this.mniDelete.Name = "mniDelete";
			this.mniDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniDelete.Size = new System.Drawing.Size(145, 22);
			this.mniDelete.Text = "Delete";
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
            this.tsmniSymbol,
            this.tsmniSymbolsTree});
			this.statusStrip1.Location = new System.Drawing.Point(0, 518);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(234, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tsmniSymbol
			// 
			this.tsmniSymbol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmniSymbol.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniDeleteSymbol,
            this.mniAddNewSymbol,
            this.toolStripSeparator1,
            this.mniltbRename,
            this.mniltbDuplicate,
            this.toolStripComboBox1});
			this.tsmniSymbol.Image = ((System.Drawing.Image)(resources.GetObject("tsmniSymbol.Image")));
			this.tsmniSymbol.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmniSymbol.Name = "tsmniSymbol";
			this.tsmniSymbol.Size = new System.Drawing.Size(47, 20);
			this.tsmniSymbol.Text = "RIM3";
			this.tsmniSymbol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mniDeleteSymbol
			// 
			this.mniDeleteSymbol.Name = "mniDeleteSymbol";
			this.mniDeleteSymbol.Size = new System.Drawing.Size(210, 22);
			this.mniDeleteSymbol.Text = "Delete [RIM3]";
			// 
			// mniAddNewSymbol
			// 
			this.mniAddNewSymbol.Name = "mniAddNewSymbol";
			this.mniAddNewSymbol.Size = new System.Drawing.Size(210, 22);
			this.mniAddNewSymbol.Text = "Add New Symbol";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
			// 
			// mniltbRename
			// 
			this.mniltbRename.BackColor = System.Drawing.Color.Transparent;
			this.mniltbRename.InputFieldAlignedRight = false;
			this.mniltbRename.InputFieldEditable = true;
			this.mniltbRename.InputFieldOffsetX = 80;
			this.mniltbRename.InputFieldValue = "RIM3";
			this.mniltbRename.InputFieldWidth = 62;
			this.mniltbRename.Name = "mniltbRename";
			this.mniltbRename.Size = new System.Drawing.Size(150, 21);
			this.mniltbRename.Text = "Rename To:";
			this.mniltbRename.TextLeft = "Rename To:";
			this.mniltbRename.TextLeftOffsetX = 0;
			this.mniltbRename.TextLeftWidth = 72;
			this.mniltbRename.TextRed = false;
			this.mniltbRename.TextRight = "";
			this.mniltbRename.TextRightOffsetX = 145;
			this.mniltbRename.TextRightWidth = 2;
			// 
			// mniltbDuplicate
			// 
			this.mniltbDuplicate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDuplicate.InputFieldAlignedRight = false;
			this.mniltbDuplicate.InputFieldEditable = true;
			this.mniltbDuplicate.InputFieldOffsetX = 80;
			this.mniltbDuplicate.InputFieldValue = "RIM3";
			this.mniltbDuplicate.InputFieldWidth = 62;
			this.mniltbDuplicate.Name = "mniltbDuplicate";
			this.mniltbDuplicate.Size = new System.Drawing.Size(150, 21);
			this.mniltbDuplicate.Text = "Duplicate To:";
			this.mniltbDuplicate.TextLeft = "Duplicate To:";
			this.mniltbDuplicate.TextLeftOffsetX = 0;
			this.mniltbDuplicate.TextLeftWidth = 79;
			this.mniltbDuplicate.TextRed = false;
			this.mniltbDuplicate.TextRight = "";
			this.mniltbDuplicate.TextRightOffsetX = 145;
			this.mniltbDuplicate.TextRightWidth = 2;
			// 
			// toolStripComboBox1
			// 
			this.toolStripComboBox1.DropDownHeight = 180;
			this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox1.IntegralHeight = false;
			this.toolStripComboBox1.MaxDropDownItems = 20;
			this.toolStripComboBox1.Name = "toolStripComboBox1";
			this.toolStripComboBox1.Size = new System.Drawing.Size(143, 23);
			this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
			// 
			// tsmniSymbolsTree
			// 
			this.tsmniSymbolsTree.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmniSymbolsTree.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
			this.tsmniSymbolsTree.Image = ((System.Drawing.Image)(resources.GetObject("tsmniSymbolsTree.Image")));
			this.tsmniSymbolsTree.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmniSymbolsTree.Name = "tsmniSymbolsTree";
			this.tsmniSymbolsTree.Size = new System.Drawing.Size(47, 20);
			this.tsmniSymbolsTree.Text = "RIM3";
			this.tsmniSymbolsTree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsmniSymbolsTree.Visible = false;
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem1.Text = "LKOH";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem2.Text = "autogen";
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem3.Text = "RIM3";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem4.Text = "USD/EUR";
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem5.Text = "SPX";
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11});
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem6.Text = "DataSource1";
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem7.Text = "toolStripMenuItem7";
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem8.Text = "toolStripMenuItem8";
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem9.Text = "toolStripMenuItem9";
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem10.Text = "toolStripMenuItem10";
			// 
			// toolStripMenuItem11
			// 
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem11.Text = "toolStripMenuItem11";
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(0, -1);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(234, 21);
			this.comboBox1.TabIndex = 3;
			this.comboBox1.Visible = false;
			// 
			// SymbolEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "SymbolEditorControl";
			this.Size = new System.Drawing.Size(234, 540);
			this.ctxEachSymbolInfo.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip ctxEachSymbolInfo;
		private System.Windows.Forms.ToolStripMenuItem mniEdit;
		private System.Windows.Forms.ToolStripMenuItem mniDuplicate;
		private System.Windows.Forms.ToolStripMenuItem mniAppend;
		private System.Windows.Forms.ToolStripMenuItem mniDelete;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ToolStripDropDownButton tsmniSymbol;
		private System.Windows.Forms.ToolStripMenuItem mniDeleteSymbol;
		private System.Windows.Forms.ToolStripMenuItem mniAddNewSymbol;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbRename;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbDuplicate;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
		private System.Windows.Forms.ToolStripDropDownButton tsmniSymbolsTree;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
	}
}
