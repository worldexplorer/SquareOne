using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Widgets.StrategiesTree {
	public partial class StrategiesTreeControl : UserControl {
		private IContainer components;
		private ImageList imageList;
		public ContextMenuStrip ctxStrategy;
		private ToolStripMenuItem mniStrategyDelete;
		private ToolStripSeparator sepStrategy;
		private ToolStripMenuItem mniStrategyEdit;
		private ToolStripMenuItem mniStrategyMoveToAnotherFolder;
		public ContextMenuStrip ctxFolder;
		public ToolStripMenuItem mniFolderCreate;
		private ToolStripMenuItem mniFolderDelete;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StrategiesTreeControl));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.ctxStrategy = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniStrategyOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStrategyOpenWith = new System.Windows.Forms.ToolStripMenuItem();
			this.mniScriptContext1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniScriptContext2 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniScriptContext3 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStrategyEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStrategyDuplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbStrategyDuplicateTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.sepStrategy = new System.Windows.Forms.ToolStripSeparator();
			this.mniStrategyMoveToAnotherFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStrategyRename = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbStrategyRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniStrategyDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniFolderCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbFolderCreate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniFolderRename = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbFolderRename = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniFolderDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniFolderCreateStrategy = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbStrategyCreate = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniShowHeader = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSearchBar = new System.Windows.Forms.ToolStripMenuItem();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tree = new BrightIdeasSoftware.TreeListView();
			this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumnModified = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
			this.btnClear = new System.Windows.Forms.Button();
			this.ctxStrategy.SuspendLayout();
			this.ctxFolder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
			this.pnlSearch.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
			this.imageList.Images.SetKeyName(0, "folder-closed.png");
			this.imageList.Images.SetKeyName(1, "folder-opened.png");
			this.imageList.Images.SetKeyName(2, "dll-closed.png");
			this.imageList.Images.SetKeyName(3, "dll-opened.png");
			// 
			// ctxStrategy
			// 
			this.ctxStrategy.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.ctxStrategy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniStrategyOpen,
            this.mniStrategyOpenWith,
            this.mniStrategyEdit,
            this.mniStrategyDuplicate,
            this.mniltbStrategyDuplicateTo,
            this.sepStrategy,
            this.mniStrategyMoveToAnotherFolder,
            this.mniStrategyRename,
            this.mniltbStrategyRenameTo,
            this.mniStrategyDelete});
			this.ctxStrategy.Name = "popupStrategy";
			this.ctxStrategy.Size = new System.Drawing.Size(236, 234);
			this.ctxStrategy.Opening += new System.ComponentModel.CancelEventHandler(this.ctxStrategy_Opening);
			// 
			// mniStrategyOpen
			// 
			this.mniStrategyOpen.Name = "mniStrategyOpen";
			this.mniStrategyOpen.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyOpen.Text = "Open Default in New Chart";
			this.mniStrategyOpen.Click += new System.EventHandler(this.mniStrategyOpen_Click);
			// 
			// mniStrategyOpenWith
			// 
			this.mniStrategyOpenWith.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniScriptContext1,
            this.mniScriptContext2,
            this.mniScriptContext3});
			this.mniStrategyOpenWith.Name = "mniStrategyOpenWith";
			this.mniStrategyOpenWith.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyOpenWith.Text = "Replace Current Chart with...";
			this.mniStrategyOpenWith.Click += new System.EventHandler(this.mniStrategyOpenWith_Click);
			// 
			// mniScriptContext1
			// 
			this.mniScriptContext1.Name = "mniScriptContext1";
			this.mniScriptContext1.Size = new System.Drawing.Size(151, 22);
			this.mniScriptContext1.Text = "ScriptContext1";
			this.mniScriptContext1.Click += new System.EventHandler(this.mniStrategyOpenWithScriptContext_Click);
			// 
			// mniScriptContext2
			// 
			this.mniScriptContext2.Name = "mniScriptContext2";
			this.mniScriptContext2.Size = new System.Drawing.Size(151, 22);
			this.mniScriptContext2.Text = "ScriptContext2";
			// 
			// mniScriptContext3
			// 
			this.mniScriptContext3.Name = "mniScriptContext3";
			this.mniScriptContext3.Size = new System.Drawing.Size(151, 22);
			this.mniScriptContext3.Text = "ScriptContext3";
			// 
			// mniStrategyEdit
			// 
			this.mniStrategyEdit.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniStrategyEdit.Name = "mniStrategyEdit";
			this.mniStrategyEdit.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyEdit.Text = "Edit Script";
			this.mniStrategyEdit.Click += new System.EventHandler(this.mniStrategyEdit_Click);
			// 
			// mniStrategyDuplicate
			// 
			this.mniStrategyDuplicate.Name = "mniStrategyDuplicate";
			this.mniStrategyDuplicate.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyDuplicate.Text = "Duplicate";
			this.mniStrategyDuplicate.Visible = false;
			this.mniStrategyDuplicate.Click += new System.EventHandler(this.mniStrategyDuplicate_Click);
			// 
			// mniltbStrategyDuplicateTo
			// 
			this.mniltbStrategyDuplicateTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbStrategyDuplicateTo.InputFieldAlignedRight = false;
			this.mniltbStrategyDuplicateTo.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbStrategyDuplicateTo.InputFieldEditable = true;
			this.mniltbStrategyDuplicateTo.InputFieldMultiline = true;
			this.mniltbStrategyDuplicateTo.InputFieldOffsetX = 80;
			this.mniltbStrategyDuplicateTo.InputFieldValue = "";
			this.mniltbStrategyDuplicateTo.InputFieldWidth = 85;
			this.mniltbStrategyDuplicateTo.Name = "mniltbStrategyDuplicateTo";
			this.mniltbStrategyDuplicateTo.OffsetTop = 0;
			this.mniltbStrategyDuplicateTo.Size = new System.Drawing.Size(175, 21);
			this.mniltbStrategyDuplicateTo.TextLeft = "Duplicate To:";
			this.mniltbStrategyDuplicateTo.TextLeftOffsetX = 0;
			this.mniltbStrategyDuplicateTo.TextLeftWidth = 79;
			this.mniltbStrategyDuplicateTo.TextRed = false;
			this.mniltbStrategyDuplicateTo.TextRight = "";
			this.mniltbStrategyDuplicateTo.TextRightOffsetX = 168;
			this.mniltbStrategyDuplicateTo.TextRightWidth = 4;
			// 
			// sepStrategy
			// 
			this.sepStrategy.Name = "sepStrategy";
			this.sepStrategy.Size = new System.Drawing.Size(232, 6);
			// 
			// mniStrategyMoveToAnotherFolder
			// 
			this.mniStrategyMoveToAnotherFolder.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniStrategyMoveToAnotherFolder.Name = "mniStrategyMoveToAnotherFolder";
			this.mniStrategyMoveToAnotherFolder.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyMoveToAnotherFolder.Text = "Move To...";
			// 
			// mniStrategyRename
			// 
			this.mniStrategyRename.Name = "mniStrategyRename";
			this.mniStrategyRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mniStrategyRename.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyRename.Text = "Rename";
			this.mniStrategyRename.Visible = false;
			this.mniStrategyRename.Click += new System.EventHandler(this.mniStrategyRename_Click);
			// 
			// mniltbStrategyRenameTo
			// 
			this.mniltbStrategyRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbStrategyRenameTo.InputFieldAlignedRight = false;
			this.mniltbStrategyRenameTo.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbStrategyRenameTo.InputFieldEditable = true;
			this.mniltbStrategyRenameTo.InputFieldMultiline = true;
			this.mniltbStrategyRenameTo.InputFieldOffsetX = 80;
			this.mniltbStrategyRenameTo.InputFieldValue = "";
			this.mniltbStrategyRenameTo.InputFieldWidth = 85;
			this.mniltbStrategyRenameTo.Name = "mniltbStrategyRenameTo";
			this.mniltbStrategyRenameTo.OffsetTop = 0;
			this.mniltbStrategyRenameTo.Size = new System.Drawing.Size(175, 21);
			this.mniltbStrategyRenameTo.TextLeft = "Rename To:";
			this.mniltbStrategyRenameTo.TextLeftOffsetX = 0;
			this.mniltbStrategyRenameTo.TextLeftWidth = 72;
			this.mniltbStrategyRenameTo.TextRed = false;
			this.mniltbStrategyRenameTo.TextRight = "";
			this.mniltbStrategyRenameTo.TextRightOffsetX = 168;
			this.mniltbStrategyRenameTo.TextRightWidth = 4;
			// 
			// mniStrategyDelete
			// 
			this.mniStrategyDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniStrategyDelete.Name = "mniStrategyDelete";
			this.mniStrategyDelete.Size = new System.Drawing.Size(235, 22);
			this.mniStrategyDelete.Text = "Delete Strategy";
			this.mniStrategyDelete.Click += new System.EventHandler(this.mniStrategyDelete_Click);
			// 
			// ctxFolder
			// 
			this.ctxFolder.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.ctxFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFolderCreate,
            this.mniltbFolderCreate,
            this.mniFolderRename,
            this.mniltbFolderRename,
            this.mniFolderDelete,
            this.toolStripSeparator1,
            this.mniFolderCreateStrategy,
            this.mniltbStrategyCreate,
            this.toolStripSeparator2,
            this.mniShowHeader,
            this.mniShowSearchBar});
			this.ctxFolder.Name = "popupSymbol";
			this.ctxFolder.Size = new System.Drawing.Size(236, 220);
			// 
			// mniFolderCreate
			// 
			this.mniFolderCreate.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniFolderCreate.Name = "mniFolderCreate";
			this.mniFolderCreate.Size = new System.Drawing.Size(235, 22);
			this.mniFolderCreate.Text = "Create New Folder";
			this.mniFolderCreate.Visible = false;
			this.mniFolderCreate.Click += new System.EventHandler(this.mniFolderCreate_Click);
			// 
			// mniltbFolderCreate
			// 
			this.mniltbFolderCreate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbFolderCreate.InputFieldAlignedRight = false;
			this.mniltbFolderCreate.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbFolderCreate.InputFieldEditable = true;
			this.mniltbFolderCreate.InputFieldMultiline = true;
			this.mniltbFolderCreate.InputFieldOffsetX = 80;
			this.mniltbFolderCreate.InputFieldValue = "";
			this.mniltbFolderCreate.InputFieldWidth = 85;
			this.mniltbFolderCreate.Name = "mniltbFolderCreate";
			this.mniltbFolderCreate.OffsetTop = 0;
			this.mniltbFolderCreate.Size = new System.Drawing.Size(175, 21);
			this.mniltbFolderCreate.TextLeft = "Create Folder";
			this.mniltbFolderCreate.TextLeftOffsetX = 0;
			this.mniltbFolderCreate.TextLeftWidth = 79;
			this.mniltbFolderCreate.TextRed = false;
			this.mniltbFolderCreate.TextRight = "";
			this.mniltbFolderCreate.TextRightOffsetX = 168;
			this.mniltbFolderCreate.TextRightWidth = 4;
			// 
			// mniFolderRename
			// 
			this.mniFolderRename.Name = "mniFolderRename";
			this.mniFolderRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mniFolderRename.Size = new System.Drawing.Size(235, 22);
			this.mniFolderRename.Text = "Rename";
			this.mniFolderRename.Visible = false;
			this.mniFolderRename.Click += new System.EventHandler(this.mniFolderRename_Click);
			// 
			// mniltbFolderRename
			// 
			this.mniltbFolderRename.BackColor = System.Drawing.Color.Transparent;
			this.mniltbFolderRename.InputFieldAlignedRight = false;
			this.mniltbFolderRename.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbFolderRename.InputFieldEditable = true;
			this.mniltbFolderRename.InputFieldMultiline = true;
			this.mniltbFolderRename.InputFieldOffsetX = 80;
			this.mniltbFolderRename.InputFieldValue = "";
			this.mniltbFolderRename.InputFieldWidth = 85;
			this.mniltbFolderRename.Name = "mniltbFolderRename";
			this.mniltbFolderRename.OffsetTop = 0;
			this.mniltbFolderRename.Size = new System.Drawing.Size(175, 21);
			this.mniltbFolderRename.TextLeft = "Rename To";
			this.mniltbFolderRename.TextLeftOffsetX = 0;
			this.mniltbFolderRename.TextLeftWidth = 69;
			this.mniltbFolderRename.TextRed = false;
			this.mniltbFolderRename.TextRight = "";
			this.mniltbFolderRename.TextRightOffsetX = 168;
			this.mniltbFolderRename.TextRightWidth = 4;
			// 
			// mniFolderDelete
			// 
			this.mniFolderDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniFolderDelete.Name = "mniFolderDelete";
			this.mniFolderDelete.Size = new System.Drawing.Size(235, 22);
			this.mniFolderDelete.Text = "Delete Folder";
			this.mniFolderDelete.Click += new System.EventHandler(this.mniFolderDelete_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
			// 
			// mniFolderCreateStrategy
			// 
			this.mniFolderCreateStrategy.Name = "mniFolderCreateStrategy";
			this.mniFolderCreateStrategy.Size = new System.Drawing.Size(235, 22);
			this.mniFolderCreateStrategy.Text = "Create New Strategy";
			this.mniFolderCreateStrategy.Visible = false;
			this.mniFolderCreateStrategy.Click += new System.EventHandler(this.mniFolderCreateStrategy_Click);
			// 
			// mniltbStrategyCreate
			// 
			this.mniltbStrategyCreate.BackColor = System.Drawing.Color.Transparent;
			this.mniltbStrategyCreate.InputFieldAlignedRight = false;
			this.mniltbStrategyCreate.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbStrategyCreate.InputFieldEditable = true;
			this.mniltbStrategyCreate.InputFieldMultiline = true;
			this.mniltbStrategyCreate.InputFieldOffsetX = 80;
			this.mniltbStrategyCreate.InputFieldValue = "";
			this.mniltbStrategyCreate.InputFieldWidth = 85;
			this.mniltbStrategyCreate.Name = "mniltbStrategyCreate";
			this.mniltbStrategyCreate.OffsetTop = 0;
			this.mniltbStrategyCreate.Size = new System.Drawing.Size(175, 21);
			this.mniltbStrategyCreate.TextLeft = "New Strategy";
			this.mniltbStrategyCreate.TextLeftOffsetX = 0;
			this.mniltbStrategyCreate.TextLeftWidth = 79;
			this.mniltbStrategyCreate.TextRed = false;
			this.mniltbStrategyCreate.TextRight = "";
			this.mniltbStrategyCreate.TextRightOffsetX = 168;
			this.mniltbStrategyCreate.TextRightWidth = 4;
			this.mniltbStrategyCreate.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbStrategyCreate_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(232, 6);
			// 
			// mniShowHeader
			// 
			this.mniShowHeader.CheckOnClick = true;
			this.mniShowHeader.Name = "mniShowHeader";
			this.mniShowHeader.Size = new System.Drawing.Size(235, 22);
			this.mniShowHeader.Text = "Show Header";
			this.mniShowHeader.Click += new System.EventHandler(this.mniShowHeader_Click);
			// 
			// mniShowSearchBar
			// 
			this.mniShowSearchBar.CheckOnClick = true;
			this.mniShowSearchBar.Name = "mniShowSearchBar";
			this.mniShowSearchBar.Size = new System.Drawing.Size(235, 22);
			this.mniShowSearchBar.Text = "Show Search Bar";
			this.mniShowSearchBar.Click += new System.EventHandler(this.mniShowSearchBar_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSearch.Location = new System.Drawing.Point(3, 3);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(71, 20);
			this.txtSearch.TabIndex = 3;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// tree
			// 
			this.tree.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.tree.AllColumns.Add(this.olvColumnName);
			this.tree.AllColumns.Add(this.olvColumnSize);
			this.tree.AllColumns.Add(this.olvColumnModified);
			this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.tree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName});
			this.tree.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tree.EmptyListMsg = "Right Click To Create";
			this.tree.FullRowSelect = true;
			this.tree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.tree.HideSelection = false;
			this.tree.Location = new System.Drawing.Point(0, 0);
			this.tree.Name = "tree";
			this.tree.OwnerDraw = true;
			this.tree.ShowCommandMenuOnRightClick = true;
			this.tree.ShowGroups = false;
			this.tree.Size = new System.Drawing.Size(102, 162);
			this.tree.SmallImageList = this.imageList;
			this.tree.TabIndex = 2;
			this.tree.TintSortColumn = true;
			this.tree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.tree.UseCompatibleStateImageBehavior = false;
			this.tree.UseFilterIndicator = true;
			this.tree.UseFiltering = true;
			this.tree.UseHotItem = true;
			this.tree.UseTranslucentHotItem = true;
			this.tree.View = System.Windows.Forms.View.Details;
			this.tree.VirtualMode = true;
			this.tree.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(this.treeListView_CellEditValidating);
			this.tree.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.treeListView_CellClick);
			this.tree.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.treeListView_CellRightClick);
			this.tree.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.treeListView_ModelCanDrop);
			this.tree.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.treeListView_ModelDropped);
			this.tree.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.treeListView_ItemSelectionChanged);
			this.tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeListView_KeyDown);
			// 
			// olvColumnName
			// 
			this.olvColumnName.FillsFreeSpace = true;
			this.olvColumnName.Hideable = false;
			this.olvColumnName.Text = "Name";
			// 
			// olvColumnSize
			// 
			this.olvColumnSize.DisplayIndex = 1;
			this.olvColumnSize.IsVisible = false;
			this.olvColumnSize.Text = "Size";
			this.olvColumnSize.Width = 30;
			// 
			// olvColumnModified
			// 
			this.olvColumnModified.DisplayIndex = 2;
			this.olvColumnModified.IsVisible = false;
			this.olvColumnModified.Text = "Modified";
			// 
			// pnlSearch
			// 
			this.pnlSearch.BackColor = System.Drawing.SystemColors.Control;
			this.pnlSearch.ColumnCount = 2;
			this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.pnlSearch.Controls.Add(this.txtSearch, 0, 0);
			this.pnlSearch.Controls.Add(this.btnClear, 1, 0);
			this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlSearch.Location = new System.Drawing.Point(0, 136);
			this.pnlSearch.MinimumSize = new System.Drawing.Size(0, 26);
			this.pnlSearch.Name = "pnlSearch";
			this.pnlSearch.RowCount = 1;
			this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.Size = new System.Drawing.Size(102, 26);
			this.pnlSearch.TabIndex = 4;
			this.pnlSearch.Visible = false;
			// 
			// btnClear
			// 
			this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnClear.Enabled = false;
			this.btnClear.Location = new System.Drawing.Point(80, 3);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(19, 20);
			this.btnClear.TabIndex = 4;
			this.btnClear.Text = "X";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// StrategiesTreeControl
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.pnlSearch);
			this.Controls.Add(this.tree);
			this.Name = "StrategiesTreeControl";
			this.Size = new System.Drawing.Size(102, 162);
			this.ctxStrategy.ResumeLayout(false);
			this.ctxFolder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
			this.pnlSearch.ResumeLayout(false);
			this.pnlSearch.PerformLayout();
			this.ResumeLayout(false);

		}

		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbStrategyCreate;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbFolderRename;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbFolderCreate;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbStrategyRenameTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbStrategyDuplicateTo;

		public BrightIdeasSoftware.TreeListView tree;
		private TextBox txtSearch;
		private ToolTip toolTip1;
		private ToolStripMenuItem mniStrategyOpen;
		private ToolStripMenuItem mniStrategyOpenWith;
		private ToolStripMenuItem mniStrategyRename;
		private ToolStripMenuItem mniFolderRename;
		private ToolStripMenuItem mniScriptContext1;
		private ToolStripMenuItem mniScriptContext2;
		private ToolStripMenuItem mniScriptContext3;
		private ToolStripMenuItem mniStrategyDuplicate;
		private ToolStripMenuItem mniFolderCreateStrategy;
		private ToolStripSeparator toolStripSeparator1;
		private BrightIdeasSoftware.OLVColumn olvColumnName;
		private BrightIdeasSoftware.OLVColumn olvColumnSize;
		private BrightIdeasSoftware.OLVColumn olvColumnModified;
		private TableLayoutPanel pnlSearch;
		private Button btnClear;		


		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniShowSearchBar;
		private System.Windows.Forms.ToolStripMenuItem mniShowHeader;
	}
}