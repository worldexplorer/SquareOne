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
		public ToolStripMenuItem mniBarsAnalyzerSymbol;
		public ToolStripMenuItem mniOpenStrategySymbol;
		private ToolStripSeparator sepSymbol;
		private ToolStripMenuItem mniRemoveSymbol;

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
			this.mniltbDataSourceAddNew = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniDataSourceEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbDataSourceRename = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniDataSourceDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbSymbolAdd = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.sepNewDS = new System.Windows.Forms.ToolStripSeparator();
			this.mniRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowHeader = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSearchBar = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxSymbol = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniNewChartSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOpenStrategySymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBarsAnalyzerSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.sepSymbol = new System.Windows.Forms.ToolStripSeparator();
			this.mniRemoveSymbol = new System.Windows.Forms.ToolStripMenuItem();
			this.mnitlbSymbolRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.textBoxFilterTree = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnClear = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tree = new BrightIdeasSoftware.TreeListView();
			this.olvColumnName = new BrightIdeasSoftware.OLVColumn();
			this.ctxDataSource.SuspendLayout();
			this.ctxSymbol.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
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
									this.mniltbDataSourceAddNew,
									this.toolStripSeparator1,
									this.mniDataSourceEdit,
									this.mniltbDataSourceRename,
									this.mniDataSourceDelete,
									this.mniltbSymbolAdd,
									this.sepNewDS,
									this.mniRefresh,
									this.mniShowHeader,
									this.mniShowSearchBar});
			this.ctxDataSource.Name = "popupDataSource";
			this.ctxDataSource.Size = new System.Drawing.Size(226, 154);
			this.ctxDataSource.Opening += new System.ComponentModel.CancelEventHandler(this.ctxDataSource_Opening);
			// 
			// mniltbDataSourceAddNew
			// 
			this.mniltbDataSourceAddNew.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDataSourceAddNew.InputFieldAlignedRight = false;
			this.mniltbDataSourceAddNew.InputFieldEditable = true;
			this.mniltbDataSourceAddNew.InputFieldOffsetX = 80;
			this.mniltbDataSourceAddNew.InputFieldValue = "";
			this.mniltbDataSourceAddNew.InputFieldWidth = 85;
			this.mniltbDataSourceAddNew.Name = "mniltbDataSourceAddNew";
			this.mniltbDataSourceAddNew.Size = new System.Drawing.Size(165, 21);
			this.mniltbDataSourceAddNew.Text = "Add New:";
			this.mniltbDataSourceAddNew.TextOffsetX = 0;
			this.mniltbDataSourceAddNew.TextRed = false;
			this.mniltbDataSourceAddNew.TextWidth = 56;
			this.mniltbDataSourceAddNew.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDataSourceAddNew_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
			// 
			// mniDataSourceEdit
			// 
			this.mniDataSourceEdit.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniDataSourceEdit.Name = "mniDataSourceEdit";
			this.mniDataSourceEdit.Size = new System.Drawing.Size(225, 22);
			this.mniDataSourceEdit.Text = "Edit DataSource";
			this.mniDataSourceEdit.Click += new System.EventHandler(this.mniDataSourceEdit_Click);
			// 
			// mniltbDataSourceRename
			// 
			this.mniltbDataSourceRename.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDataSourceRename.InputFieldAlignedRight = false;
			this.mniltbDataSourceRename.InputFieldEditable = true;
			this.mniltbDataSourceRename.InputFieldOffsetX = 80;
			this.mniltbDataSourceRename.InputFieldValue = "";
			this.mniltbDataSourceRename.InputFieldWidth = 85;
			this.mniltbDataSourceRename.Name = "mniltbDataSourceRename";
			this.mniltbDataSourceRename.Size = new System.Drawing.Size(165, 21);
			this.mniltbDataSourceRename.Text = "Rename To:";
			this.mniltbDataSourceRename.TextOffsetX = 0;
			this.mniltbDataSourceRename.TextRed = false;
			this.mniltbDataSourceRename.TextWidth = 68;
			this.mniltbDataSourceRename.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDataSourceRename_UserTyped);
			// 
			// mniDataSourceDelete
			// 
			this.mniDataSourceDelete.Enabled = false;
			this.mniDataSourceDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniDataSourceDelete.Name = "mniDataSourceDelete";
			this.mniDataSourceDelete.Size = new System.Drawing.Size(225, 22);
			this.mniDataSourceDelete.Text = "Delete this DataSource";
			this.mniDataSourceDelete.Click += new System.EventHandler(this.mniDataSourceDelete_Click);
			// 
			// mniltbSymbolAdd
			// 
			this.mniltbSymbolAdd.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSymbolAdd.InputFieldAlignedRight = false;
			this.mniltbSymbolAdd.InputFieldEditable = true;
			this.mniltbSymbolAdd.InputFieldOffsetX = 80;
			this.mniltbSymbolAdd.InputFieldValue = "";
			this.mniltbSymbolAdd.InputFieldWidth = 85;
			this.mniltbSymbolAdd.Name = "mniltbSymbolAdd";
			this.mniltbSymbolAdd.Size = new System.Drawing.Size(165, 21);
			this.mniltbSymbolAdd.Text = "Add Symbol:";
			this.mniltbSymbolAdd.TextOffsetX = 0;
			this.mniltbSymbolAdd.TextRed = false;
			this.mniltbSymbolAdd.TextWidth = 68;
			this.mniltbSymbolAdd.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSymbolAdd_UserTyped);
			// 
			// sepNewDS
			// 
			this.sepNewDS.Name = "sepNewDS";
			this.sepNewDS.Size = new System.Drawing.Size(222, 6);
			// 
			// mniRefresh
			// 
			this.mniRefresh.Name = "mniRefresh";
			this.mniRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mniRefresh.Size = new System.Drawing.Size(225, 22);
			this.mniRefresh.Text = "Refresh";
			this.mniRefresh.Click += new System.EventHandler(this.mniRefresh_Click);
			// 
			// ctxSymbol
			// 
			this.ctxSymbol.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.ctxSymbol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniNewChartSymbol,
									this.mniOpenStrategySymbol,
									this.mniBarsAnalyzerSymbol,
									this.sepSymbol,
									this.mniRemoveSymbol,
									this.mnitlbSymbolRenameTo});
			this.ctxSymbol.Name = "popupSymbol";
			this.ctxSymbol.Size = new System.Drawing.Size(253, 122);
			// 
			// mniNewChartSymbol
			// 
			this.mniNewChartSymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniNewChartSymbol.Name = "mniNewChartSymbol";
			this.mniNewChartSymbol.Size = new System.Drawing.Size(252, 22);
			this.mniNewChartSymbol.Text = "New Chart for ";
			this.mniNewChartSymbol.Click += new System.EventHandler(this.mniNewChartSymbol_Click);
			// 
			// mniOpenStrategySymbol
			// 
			this.mniOpenStrategySymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniOpenStrategySymbol.Name = "mniOpenStrategySymbol";
			this.mniOpenStrategySymbol.Size = new System.Drawing.Size(252, 22);
			this.mniOpenStrategySymbol.Text = "New Strategy for ";
			this.mniOpenStrategySymbol.Click += new System.EventHandler(this.mniOpenStrategySymbol_Click);
			// 
			// mniBarsAnalyzerSymbol
			// 
			this.mniBarsAnalyzerSymbol.Enabled = false;
			this.mniBarsAnalyzerSymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniBarsAnalyzerSymbol.Name = "mniBarsAnalyzerSymbol";
			this.mniBarsAnalyzerSymbol.Size = new System.Drawing.Size(252, 22);
			this.mniBarsAnalyzerSymbol.Text = "Bar Analyzer for  ";
			this.mniBarsAnalyzerSymbol.Click += new System.EventHandler(this.mniBarsAnalyzerSymbol_Click);
			// 
			// sepSymbol
			// 
			this.sepSymbol.Name = "sepSymbol";
			this.sepSymbol.Size = new System.Drawing.Size(249, 6);
			// 
			// mniRemoveSymbol
			// 
			this.mniRemoveSymbol.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mniRemoveSymbol.Name = "mniRemoveSymbol";
			this.mniRemoveSymbol.Size = new System.Drawing.Size(252, 22);
			this.mniRemoveSymbol.Text = "Remove Symbol from DataSource";
			this.mniRemoveSymbol.Click += new System.EventHandler(this.mniRemoveSymbol_Click);
			// 
			// mnitlbSymbolRenameTo
			// 
			this.mnitlbSymbolRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbSymbolRenameTo.InputFieldAlignedRight = false;
			this.mnitlbSymbolRenameTo.InputFieldEditable = true;
			this.mnitlbSymbolRenameTo.InputFieldOffsetX = 80;
			this.mnitlbSymbolRenameTo.InputFieldValue = "";
			this.mnitlbSymbolRenameTo.InputFieldWidth = 85;
			this.mnitlbSymbolRenameTo.Name = "mnitlbSymbolRenameTo";
			this.mnitlbSymbolRenameTo.Size = new System.Drawing.Size(165, 21);
			this.mnitlbSymbolRenameTo.Text = "Rename To:";
			this.mnitlbSymbolRenameTo.TextOffsetX = 0;
			this.mnitlbSymbolRenameTo.TextRed = false;
			this.mnitlbSymbolRenameTo.TextWidth = 68;
			this.mnitlbSymbolRenameTo.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbSymbolRenameTo_UserTyped);
			// 
			// mniShowHeader
			// 
			this.mniShowHeader.CheckOnClick = true;
			this.mniShowHeader.Name = "mniShowHeader";
			this.mniShowHeader.Size = new System.Drawing.Size(228, 22);
			this.mniShowHeader.Text = "Show Header";
			this.mniShowHeader.Click += new System.EventHandler(this.mniShowHeader_Click);
			// 
			// mniShowSearchBar
			// 
			this.mniShowSearchBar.CheckOnClick = true;
			this.mniShowSearchBar.Name = "mniShowSearchBar";
			this.mniShowSearchBar.Size = new System.Drawing.Size(228, 22);
			this.mniShowSearchBar.Text = "Show Search Bar";
			this.mniShowSearchBar.Click += new System.EventHandler(mniShowSearchBar_Click);
			// 
			// textBoxFilterTree
			// 
			this.textBoxFilterTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxFilterTree.Location = new System.Drawing.Point(3, 3);
			this.textBoxFilterTree.Name = "textBoxFilterTree";
			this.textBoxFilterTree.Size = new System.Drawing.Size(99, 20);
			this.textBoxFilterTree.TabIndex = 3;
			this.textBoxFilterTree.TextChanged += new System.EventHandler(this.txtFilterSymbol_TextChanged);
			// 
			// btnClear
			// 
			this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnClear.Enabled = false;
			this.btnClear.Location = new System.Drawing.Point(108, 3);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(19, 20);
			this.btnClear.TabIndex = 4;
			this.btnClear.Text = "X";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.Controls.Add(this.textBoxFilterTree, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnClear, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 211);
			this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 26);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(130, 26);
			this.tableLayoutPanel1.TabIndex = 5;
			this.tableLayoutPanel1.Visible = false;
			// 
			// tree
			// 
			this.tree.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.tree.AllColumns.Add(this.olvColumnName);
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
			this.tree.MultiSelect = false;
			this.tree.Name = "tree";
			this.tree.OwnerDraw = true;
			this.tree.SelectAllOnControlA = false;
			this.tree.SelectColumnsMenuStaysOpen = false;
			this.tree.SelectColumnsOnRightClick = false;
			this.tree.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
			this.tree.ShowCommandMenuOnRightClick = true;
			this.tree.ShowGroups = false;
			this.tree.Size = new System.Drawing.Size(130, 237);
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
			this.tree.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.tree_CellClick);
			this.tree.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.tree_CellRightClick);
			this.tree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDoubleClick);
			// 
			// olvColumnName
			// 
			this.olvColumnName.FillsFreeSpace = true;
			this.olvColumnName.Text = "Name";
			// 
			// DataSourcesTreeControl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.tree);
			this.Name = "DataSourcesTreeControl";
			this.Size = new System.Drawing.Size(130, 237);
			this.ctxDataSource.ResumeLayout(false);
			this.ctxSymbol.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
			this.ResumeLayout(false);
		}
		private BrightIdeasSoftware.TreeListView tree;
		private BrightIdeasSoftware.OLVColumn olvColumnName;
		private TextBox textBoxFilterTree;
		private ToolTip toolTip1;
		private Button btnClear;
		private TableLayoutPanel tableLayoutPanel1;
		private MenuItemLabeledTextBox mniltbSymbolAdd;
		private MenuItemLabeledTextBox mniltbDataSourceRename;
		private MenuItemLabeledTextBox mnitlbSymbolRenameTo;
		private ToolStripMenuItem mniRefresh;
		private ToolStripSeparator toolStripSeparator1;
		private MenuItemLabeledTextBox mniltbDataSourceAddNew;

		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniShowSearchBar;
		private System.Windows.Forms.ToolStripMenuItem mniShowHeader;

	}
}
