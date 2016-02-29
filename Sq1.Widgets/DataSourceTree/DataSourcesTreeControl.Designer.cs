using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		private IContainer components;
		private ImageList imageList;
		public ContextMenuStrip ctxDataSource;
		private ToolStripMenuItem mniDataSourceDelete;
		private ToolStripSeparator sepNewDS;
		private ToolStripMenuItem mniDataSourceEdit;
		public ContextMenuStrip ctxSymbol;
		public ToolStripMenuItem mniNewChartSymbol;
		public ToolStripMenuItem mniSymbolBarsEditor;
		public ToolStripMenuItem mniOpenStrategySymbol;
		private ToolStripSeparator sepSymbol;
		private ToolStripMenuItem mniSymbolRemove;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.ctxDataSource = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniDataSourceBrief = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniDataSourceEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbDataSourceRename = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniDataSourceDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbDataSourceAddNew = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbSymbolAdd = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.sepNewDS = new System.Windows.Forms.ToolStripSeparator();
			this.mniRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowHeader = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSearchBar = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAppendMarketNameToDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxSymbol = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniNewChartSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOpenStrategySymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniSymbolInfoEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSymbolBarsEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSymbolFuturesMerger = new System.Windows.Forms.ToolStripMenuItem();
			this.sepSymbol = new System.Windows.Forms.ToolStripSeparator();
			this.mniSymbolRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbSymbolRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniSymbolCopyToAnotherDataSource = new System.Windows.Forms.ToolStripMenuItem();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnClear = new System.Windows.Forms.Button();
			this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
			this.OlvTree = new BrightIdeasSoftware.TreeListView();
			this.olvcName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTimeFrame = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxDataSource.SuspendLayout();
			this.ctxSymbol.SuspendLayout();
			this.pnlSearch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.OlvTree)).BeginInit();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
			// 
			// ctxDataSource
			// 
			this.ctxDataSource.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.ctxDataSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniDataSourceBrief,
            this.toolStripSeparator2,
            this.mniDataSourceEdit,
            this.mniltbDataSourceRename,
            this.mniDataSourceDelete,
            this.mniltbDataSourceAddNew,
            this.toolStripSeparator1,
            this.mniltbSymbolAdd,
            this.sepNewDS,
            this.mniRefresh,
            this.mniShowHeader,
            this.mniShowSearchBar,
            this.mniAppendMarketNameToDataSourceToolStripMenuItem});
			this.ctxDataSource.Name = "popupDataSource";
			this.ctxDataSource.Size = new System.Drawing.Size(291, 248);
			this.ctxDataSource.Opening += new System.ComponentModel.CancelEventHandler(this.ctxDataSource_Opening);
			// 
			// mniDataSourceBrief
			// 
			this.mniDataSourceBrief.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mniDataSourceBrief.Name = "mniDataSourceBrief";
			this.mniDataSourceBrief.Size = new System.Drawing.Size(290, 22);
			this.mniDataSourceBrief.Text = "[quik] [5-minutes] [32 Symbols]";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(287, 6);
			// 
			// mniDataSourceEdit
			// 
			this.mniDataSourceEdit.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniDataSourceEdit.Name = "mniDataSourceEdit";
			this.mniDataSourceEdit.Size = new System.Drawing.Size(290, 22);
			this.mniDataSourceEdit.Text = "Edit [MOCK] DataSource";
			this.mniDataSourceEdit.Click += new System.EventHandler(this.mniDataSourceEdit_Click);
			// 
			// mniltbDataSourceRename
			// 
			this.mniltbDataSourceRename.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDataSourceRename.InputFieldAlignedRight = false;
			this.mniltbDataSourceRename.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbDataSourceRename.InputFieldEditable = true;
			this.mniltbDataSourceRename.InputFieldMultiline = true;
			this.mniltbDataSourceRename.InputFieldOffsetX = 150;
			this.mniltbDataSourceRename.InputFieldValue = "";
			this.mniltbDataSourceRename.InputFieldWidth = 70;
			this.mniltbDataSourceRename.Name = "mniltbDataSourceRename";
			this.mniltbDataSourceRename.OffsetTop = 0;
			this.mniltbDataSourceRename.Size = new System.Drawing.Size(230, 21);
			this.mniltbDataSourceRename.TextLeft = "Rename [MOCK] to";
			this.mniltbDataSourceRename.TextLeftOffsetX = 0;
			this.mniltbDataSourceRename.TextLeftWidth = 112;
			this.mniltbDataSourceRename.TextRed = false;
			this.mniltbDataSourceRename.TextRight = "";
			this.mniltbDataSourceRename.TextRightOffsetX = 223;
			this.mniltbDataSourceRename.TextRightWidth = 4;
			this.mniltbDataSourceRename.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDataSourceRename_UserTyped);
			// 
			// mniDataSourceDelete
			// 
			this.mniDataSourceDelete.Enabled = false;
			this.mniDataSourceDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniDataSourceDelete.Name = "mniDataSourceDelete";
			this.mniDataSourceDelete.Size = new System.Drawing.Size(290, 22);
			this.mniDataSourceDelete.Text = "Delete [MOCK] DataSource";
			this.mniDataSourceDelete.Click += new System.EventHandler(this.mniDataSourceDelete_Click);
			// 
			// mniltbDataSourceAddNew
			// 
			this.mniltbDataSourceAddNew.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDataSourceAddNew.InputFieldAlignedRight = false;
			this.mniltbDataSourceAddNew.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbDataSourceAddNew.InputFieldEditable = true;
			this.mniltbDataSourceAddNew.InputFieldMultiline = true;
			this.mniltbDataSourceAddNew.InputFieldOffsetX = 150;
			this.mniltbDataSourceAddNew.InputFieldValue = "";
			this.mniltbDataSourceAddNew.InputFieldWidth = 70;
			this.mniltbDataSourceAddNew.Name = "mniltbDataSourceAddNew";
			this.mniltbDataSourceAddNew.OffsetTop = 0;
			this.mniltbDataSourceAddNew.Size = new System.Drawing.Size(230, 21);
			this.mniltbDataSourceAddNew.TextLeft = "Add New DataSource";
			this.mniltbDataSourceAddNew.TextLeftOffsetX = 0;
			this.mniltbDataSourceAddNew.TextLeftWidth = 121;
			this.mniltbDataSourceAddNew.TextRed = false;
			this.mniltbDataSourceAddNew.TextRight = "";
			this.mniltbDataSourceAddNew.TextRightOffsetX = 223;
			this.mniltbDataSourceAddNew.TextRightWidth = 4;
			this.mniltbDataSourceAddNew.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDataSourceAddNew_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(287, 6);
			// 
			// mniltbSymbolAdd
			// 
			this.mniltbSymbolAdd.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSymbolAdd.InputFieldAlignedRight = false;
			this.mniltbSymbolAdd.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSymbolAdd.InputFieldEditable = true;
			this.mniltbSymbolAdd.InputFieldMultiline = true;
			this.mniltbSymbolAdd.InputFieldOffsetX = 150;
			this.mniltbSymbolAdd.InputFieldValue = "";
			this.mniltbSymbolAdd.InputFieldWidth = 70;
			this.mniltbSymbolAdd.Name = "mniltbSymbolAdd";
			this.mniltbSymbolAdd.OffsetTop = 0;
			this.mniltbSymbolAdd.Size = new System.Drawing.Size(230, 21);
			this.mniltbSymbolAdd.TextLeft = "Add Symbol to [MOCK]";
			this.mniltbSymbolAdd.TextLeftOffsetX = 0;
			this.mniltbSymbolAdd.TextLeftWidth = 134;
			this.mniltbSymbolAdd.TextRed = false;
			this.mniltbSymbolAdd.TextRight = "";
			this.mniltbSymbolAdd.TextRightOffsetX = 223;
			this.mniltbSymbolAdd.TextRightWidth = 4;
			this.mniltbSymbolAdd.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSymbolAdd_UserTyped);
			// 
			// sepNewDS
			// 
			this.sepNewDS.Name = "sepNewDS";
			this.sepNewDS.Size = new System.Drawing.Size(287, 6);
			// 
			// mniRefresh
			// 
			this.mniRefresh.Name = "mniRefresh";
			this.mniRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mniRefresh.Size = new System.Drawing.Size(290, 22);
			this.mniRefresh.Text = "Refresh tree from Repository";
			this.mniRefresh.Click += new System.EventHandler(this.mniRefresh_Click);
			// 
			// mniShowHeader
			// 
			this.mniShowHeader.CheckOnClick = true;
			this.mniShowHeader.Name = "mniShowHeader";
			this.mniShowHeader.Size = new System.Drawing.Size(290, 22);
			this.mniShowHeader.Text = "Show Header";
			this.mniShowHeader.Click += new System.EventHandler(this.mniShowHeader_Click);
			// 
			// mniShowSearchBar
			// 
			this.mniShowSearchBar.CheckOnClick = true;
			this.mniShowSearchBar.Name = "mniShowSearchBar";
			this.mniShowSearchBar.Size = new System.Drawing.Size(290, 22);
			this.mniShowSearchBar.Text = "Show Search Bar";
			this.mniShowSearchBar.Click += new System.EventHandler(this.mniShowSearchBar_Click);
			// 
			// mniAppendMarketNameToDataSourceToolStripMenuItem
			// 
			this.mniAppendMarketNameToDataSourceToolStripMenuItem.CheckOnClick = true;
			this.mniAppendMarketNameToDataSourceToolStripMenuItem.Name = "mniAppendMarketNameToDataSourceToolStripMenuItem";
			this.mniAppendMarketNameToDataSourceToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.mniAppendMarketNameToDataSourceToolStripMenuItem.Text = "Append Market Name To DataSource";
			this.mniAppendMarketNameToDataSourceToolStripMenuItem.Click += new System.EventHandler(this.mniAppendMarketNameToDataSourceToolStripMenuItem_Click);
			// 
			// ctxSymbol
			// 
			this.ctxSymbol.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.ctxSymbol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniNewChartSymbol,
            this.mniOpenStrategySymbol,
            this.toolStripSeparator3,
            this.mniSymbolInfoEditor,
            this.mniSymbolBarsEditor,
            this.mniSymbolFuturesMerger,
            this.sepSymbol,
            this.mniSymbolRemove,
            this.mniltbSymbolRenameTo,
            this.mniSymbolCopyToAnotherDataSource});
			this.ctxSymbol.Name = "popupSymbol";
			this.ctxSymbol.Size = new System.Drawing.Size(306, 194);
			// 
			// mniNewChartSymbol
			// 
			this.mniNewChartSymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniNewChartSymbol.Name = "mniNewChartSymbol";
			this.mniNewChartSymbol.Size = new System.Drawing.Size(305, 22);
			this.mniNewChartSymbol.Text = "New Chart for [RIM3]";
			this.mniNewChartSymbol.Click += new System.EventHandler(this.mniNewChartSymbol_Click);
			// 
			// mniOpenStrategySymbol
			// 
			this.mniOpenStrategySymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniOpenStrategySymbol.Name = "mniOpenStrategySymbol";
			this.mniOpenStrategySymbol.Size = new System.Drawing.Size(305, 22);
			this.mniOpenStrategySymbol.Text = "New Strategy for [RIM3]";
			this.mniOpenStrategySymbol.Click += new System.EventHandler(this.mniOpenStrategySymbol_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(302, 6);
			// 
			// mniSymbolInfoEditor
			// 
			this.mniSymbolInfoEditor.Name = "mniSymbolInfoEditor";
			this.mniSymbolInfoEditor.Size = new System.Drawing.Size(305, 22);
			this.mniSymbolInfoEditor.Text = "Symbol Editor for [RIM3]";
			this.mniSymbolInfoEditor.Click += new System.EventHandler(this.mniSymbolInfoEditor_Click);
			// 
			// mniSymbolBarsEditor
			// 
			this.mniSymbolBarsEditor.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniSymbolBarsEditor.Name = "mniSymbolBarsEditor";
			this.mniSymbolBarsEditor.Size = new System.Drawing.Size(305, 22);
			this.mniSymbolBarsEditor.Text = "Bars Editor for [RIM3]";
			this.mniSymbolBarsEditor.Click += new System.EventHandler(this.mniBarsEditorSymbol_Click);
			// 
			// mniSymbolFuturesMerger
			// 
			this.mniSymbolFuturesMerger.Enabled = false;
			this.mniSymbolFuturesMerger.Name = "mniSymbolFuturesMerger";
			this.mniSymbolFuturesMerger.Size = new System.Drawing.Size(305, 22);
			this.mniSymbolFuturesMerger.Text = "Futures Merger for [RIM3]";
			// 
			// sepSymbol
			// 
			this.sepSymbol.Name = "sepSymbol";
			this.sepSymbol.Size = new System.Drawing.Size(302, 6);
			// 
			// mniSymbolRemove
			// 
			this.mniSymbolRemove.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniSymbolRemove.Name = "mniSymbolRemove";
			this.mniSymbolRemove.Size = new System.Drawing.Size(305, 22);
			this.mniSymbolRemove.Text = "Remove [RIM3] from [MOCK] DataSource";
			this.mniSymbolRemove.Click += new System.EventHandler(this.mniRemoveSymbol_Click);
			// 
			// mniltbSymbolRenameTo
			// 
			this.mniltbSymbolRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSymbolRenameTo.InputFieldAlignedRight = false;
			this.mniltbSymbolRenameTo.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSymbolRenameTo.InputFieldEditable = true;
			this.mniltbSymbolRenameTo.InputFieldMultiline = true;
			this.mniltbSymbolRenameTo.InputFieldOffsetX = 150;
			this.mniltbSymbolRenameTo.InputFieldValue = "";
			this.mniltbSymbolRenameTo.InputFieldWidth = 85;
			this.mniltbSymbolRenameTo.Name = "mniltbSymbolRenameTo";
			this.mniltbSymbolRenameTo.OffsetTop = 0;
			this.mniltbSymbolRenameTo.Size = new System.Drawing.Size(245, 21);
			this.mniltbSymbolRenameTo.TextLeft = "Rename [RIM3] to";
			this.mniltbSymbolRenameTo.TextLeftOffsetX = 0;
			this.mniltbSymbolRenameTo.TextLeftWidth = 104;
			this.mniltbSymbolRenameTo.TextRed = false;
			this.mniltbSymbolRenameTo.TextRight = "";
			this.mniltbSymbolRenameTo.TextRightOffsetX = 238;
			this.mniltbSymbolRenameTo.TextRightWidth = 4;
			this.mniltbSymbolRenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSymbolRenameTo_UserTyped);
			// 
			// mniSymbolCopyToAnotherDataSource
			// 
			this.mniSymbolCopyToAnotherDataSource.Name = "mniSymbolCopyToAnotherDataSource";
			this.mniSymbolCopyToAnotherDataSource.Size = new System.Drawing.Size(305, 22);
			this.mniSymbolCopyToAnotherDataSource.Text = "Copy [RIM3] to another DataSource";
			this.mniSymbolCopyToAnotherDataSource.DropDownClosed += new System.EventHandler(this.mniSymbolCopyToAnotherDataSource_DropDownClosed);
			this.mniSymbolCopyToAnotherDataSource.DropDownOpening += new System.EventHandler(this.mniSymbolCopyToAnotherDataSource_DropDownOpening);
			this.mniSymbolCopyToAnotherDataSource.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mniSymbolCopyToAnotherDataSource_DropDownItemClicked);
			// 
			// txtSearch
			// 
			this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSearch.Location = new System.Drawing.Point(3, 3);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(123, 20);
			this.txtSearch.TabIndex = 3;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// btnClear
			// 
			this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnClear.Enabled = false;
			this.btnClear.Location = new System.Drawing.Point(132, 3);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(19, 20);
			this.btnClear.TabIndex = 4;
			this.btnClear.Text = "X";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
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
			this.pnlSearch.Location = new System.Drawing.Point(0, 211);
			this.pnlSearch.MinimumSize = new System.Drawing.Size(0, 26);
			this.pnlSearch.Name = "pnlSearch";
			this.pnlSearch.RowCount = 1;
			this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.Size = new System.Drawing.Size(154, 26);
			this.pnlSearch.TabIndex = 5;
			this.pnlSearch.Visible = false;
			// 
			// OlvTree
			// 
			this.OlvTree.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.OlvTree.AllColumns.Add(this.olvcName);
			this.OlvTree.AllColumns.Add(this.olvcTimeFrame);
			this.OlvTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.OlvTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.OlvTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcName,
            this.olvcTimeFrame});
			this.OlvTree.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.OlvTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OlvTree.EmptyListMsg = "Right Click To Create";
			this.OlvTree.FullRowSelect = true;
			this.OlvTree.HideSelection = false;
			this.OlvTree.Location = new System.Drawing.Point(0, 0);
			this.OlvTree.MultiSelect = false;
			this.OlvTree.Name = "OlvTree";
			this.OlvTree.SelectAllOnControlA = false;
			this.OlvTree.ShowCommandMenuOnRightClick = true;
			this.OlvTree.ShowGroups = false;
			this.OlvTree.ShowItemToolTips = true;
			this.OlvTree.Size = new System.Drawing.Size(154, 237);
			this.OlvTree.SmallImageList = this.imageList;
			this.OlvTree.TabIndex = 2;
			this.OlvTree.TintSortColumn = true;
			this.OlvTree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.OlvTree.UseCompatibleStateImageBehavior = false;
			this.OlvTree.UseFilterIndicator = true;
			this.OlvTree.UseFiltering = true;
			this.OlvTree.UseHotItem = true;
			this.OlvTree.UseTranslucentHotItem = true;
			this.OlvTree.View = System.Windows.Forms.View.Details;
			this.OlvTree.VirtualMode = true;
			this.OlvTree.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.tree_CellClick);
			this.OlvTree.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.tree_CellRightClick);
			this.OlvTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDoubleClick);
			// 
			// olvcName
			// 
			this.olvcName.FillsFreeSpace = true;
			this.olvcName.Text = "Symbol/Chart";
			this.olvcName.Width = 80;
			// 
			// olvcTimeFrame
			// 
			this.olvcTimeFrame.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTimeFrame.Text = "TimeFrame";
			this.olvcTimeFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTimeFrame.Width = 40;
			// 
			// DataSourcesTreeControl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.pnlSearch);
			this.Controls.Add(this.OlvTree);
			this.Name = "DataSourcesTreeControl";
			this.Size = new System.Drawing.Size(154, 237);
			this.ctxDataSource.ResumeLayout(false);
			this.ctxSymbol.ResumeLayout(false);
			this.pnlSearch.ResumeLayout(false);
			this.pnlSearch.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.OlvTree)).EndInit();
			this.ResumeLayout(false);

		}

		public BrightIdeasSoftware.TreeListView OlvTree;
		private BrightIdeasSoftware.OLVColumn olvcName;
		private TextBox txtSearch;
		private ToolTip toolTip1;
		private Button btnClear;
		private TableLayoutPanel pnlSearch;
		private MenuItemLabeledTextBox mniltbSymbolAdd;
		private MenuItemLabeledTextBox mniltbDataSourceRename;
		private MenuItemLabeledTextBox mniltbSymbolRenameTo;
		private ToolStripMenuItem mniRefresh;
		private ToolStripSeparator toolStripSeparator1;
		private MenuItemLabeledTextBox mniltbDataSourceAddNew;

		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniShowSearchBar;
		private System.Windows.Forms.ToolStripMenuItem mniShowHeader;
		private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem mniSymbolInfoEditor;
        private ToolStripMenuItem mniSymbolCopyToAnotherDataSource;
		private ToolStripMenuItem mniDataSourceBrief;
		private ToolStripMenuItem mniSymbolFuturesMerger;
		private BrightIdeasSoftware.OLVColumn olvcTimeFrame;
		private ToolStripMenuItem mniAppendMarketNameToDataSourceToolStripMenuItem;

	}
}
