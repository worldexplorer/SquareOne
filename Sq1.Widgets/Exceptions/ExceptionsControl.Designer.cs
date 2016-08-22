using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using BrightIdeasSoftware;

namespace Sq1.Widgets.Exceptions {
	[ToolboxBitmap(typeof(ExceptionsControl), "ExceptionsControl")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]

	public partial class ExceptionsControl {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfgh"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dfghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Occured")]
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.olvTreeExceptions = new BrightIdeasSoftware.TreeListView();
			this.olvcException = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTimestamp = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTimesOccured = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniRecentAlwaysSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniPopupOnIncomingException = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniShowHeader = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowTimestamps = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowCounterAndGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSearchbar = new System.Windows.Forms.ToolStripMenuItem();
			this.mniExcludeKeywords = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxIgnoreKeywords = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniTextAndSave = new Sq1.Widgets.TextAndSave.MenuItemTextAndSave();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniDeleteExceptionsSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniClear = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRebuildAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbFlushToGuiDelayMsec = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbSerializationInterval = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniSerializeNow = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbLogrotateLargerThan = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniDeleteAllLogrotatedExceptionJsons = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
			this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
			this.btnSearchClose = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.btnSearchClear = new System.Windows.Forms.Button();
			this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtExceptionMessage = new System.Windows.Forms.TextBox();
			this.ctxExceptionMessage = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniAddToExcludeKeywordsCsv = new System.Windows.Forms.ToolStripMenuItem();
			this.olvStackTrace = new System.Windows.Forms.ListView();
			this.lvhMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhDeclaringClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ctxCallStack = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCopyStackPosition = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip_search = new System.Windows.Forms.StatusStrip();
			this.tsiBtnClear = new System.Windows.Forms.ToolStripButton();
			this.tsilbl_separator = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsiCbx_SearchApply = new Sq1.Widgets.ToolStripImproved.ToolStripItemCheckBox();
			this.tsiLtb_SearchKeywords = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.tsiCbx_ExcludeApply = new Sq1.Widgets.ToolStripImproved.ToolStripItemCheckBox();
			this.tsiLtb_ExcludeKeywords = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniExpandAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).BeginInit();
			this.ctxTree.SuspendLayout();
			this.ctxIgnoreKeywords.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
			this.splitContainerVertical.Panel1.SuspendLayout();
			this.splitContainerVertical.Panel2.SuspendLayout();
			this.splitContainerVertical.SuspendLayout();
			this.pnlSearch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).BeginInit();
			this.splitContainerHorizontal.Panel1.SuspendLayout();
			this.splitContainerHorizontal.Panel2.SuspendLayout();
			this.splitContainerHorizontal.SuspendLayout();
			this.ctxExceptionMessage.SuspendLayout();
			this.ctxCallStack.SuspendLayout();
			this.statusStrip_search.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvTreeExceptions
			// 
			this.olvTreeExceptions.AllColumns.Add(this.olvcException);
			this.olvTreeExceptions.AllColumns.Add(this.olvcTimestamp);
			this.olvTreeExceptions.AllColumns.Add(this.olvcTimesOccured);
			this.olvTreeExceptions.AllowColumnReorder = true;
			this.olvTreeExceptions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvTreeExceptions.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvTreeExceptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcException,
            this.olvcTimestamp,
            this.olvcTimesOccured});
			this.olvTreeExceptions.ContextMenuStrip = this.ctxTree;
			this.olvTreeExceptions.CopySelectionOnControlC = false;
			this.olvTreeExceptions.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvTreeExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvTreeExceptions.FullRowSelect = true;
			this.olvTreeExceptions.HideSelection = false;
			this.olvTreeExceptions.IncludeColumnHeadersInCopy = true;
			this.olvTreeExceptions.IncludeHiddenColumnsInDataTransfer = true;
			this.olvTreeExceptions.Location = new System.Drawing.Point(0, 0);
			this.olvTreeExceptions.Name = "olvTreeExceptions";
			this.olvTreeExceptions.OwnerDraw = true;
			this.olvTreeExceptions.ShowCommandMenuOnRightClick = true;
			this.olvTreeExceptions.ShowGroups = false;
			this.olvTreeExceptions.ShowItemToolTips = true;
			this.olvTreeExceptions.Size = new System.Drawing.Size(226, 336);
			this.olvTreeExceptions.TabIndex = 3;
			this.olvTreeExceptions.TintSortColumn = true;
			this.olvTreeExceptions.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.olvTreeExceptions.UseCompatibleStateImageBehavior = false;
			this.olvTreeExceptions.UseFilterIndicator = true;
			this.olvTreeExceptions.UseFiltering = true;
			this.olvTreeExceptions.UseHotItem = true;
			this.olvTreeExceptions.UseTranslucentHotItem = true;
			this.olvTreeExceptions.UseTranslucentSelection = true;
			this.olvTreeExceptions.View = System.Windows.Forms.View.Details;
			this.olvTreeExceptions.VirtualMode = true;
			this.olvTreeExceptions.SelectedIndexChanged += new System.EventHandler(this.tree_SelectedIndexChanged);
			// 
			// olvcException
			// 
			this.olvcException.FillsFreeSpace = true;
			this.olvcException.Text = "Exception";
			// 
			// olvcTimestamp
			// 
			this.olvcTimestamp.Text = "Occured";
			this.olvcTimestamp.Width = 80;
			// 
			// olvcTimesOccured
			// 
			this.olvcTimesOccured.IsVisible = false;
			this.olvcTimesOccured.Text = "Times";
			this.olvcTimesOccured.Width = 40;
			// 
			// ctxTree
			// 
			this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniRecentAlwaysSelected,
            this.mniPopupOnIncomingException,
            this.toolStripSeparator3,
            this.mniShowHeader,
            this.mniShowTimestamps,
            this.mniShowCounterAndGroup,
            this.mniShowSearchbar,
            this.mniExcludeKeywords,
            this.toolStripSeparator1,
            this.mniCopy,
            this.mniDeleteExceptionsSelected,
            this.mniClear,
            this.toolStripSeparator4,
            this.mniExpandAll,
            this.mniCollapseAll,
            this.mniRebuildAll,
            this.mniltbFlushToGuiDelayMsec,
            this.toolStripSeparator2,
            this.mniltbSerializationInterval,
            this.mniSerializeNow,
            this.mniltbLogrotateLargerThan,
            this.mniDeleteAllLogrotatedExceptionJsons});
			this.ctxTree.Name = "ctxTree";
			this.ctxTree.Size = new System.Drawing.Size(303, 429);
			this.ctxTree.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTree_Opening);
			// 
			// mniRecentAlwaysSelected
			// 
			this.mniRecentAlwaysSelected.CheckOnClick = true;
			this.mniRecentAlwaysSelected.Name = "mniRecentAlwaysSelected";
			this.mniRecentAlwaysSelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRecentAlwaysSelected.Size = new System.Drawing.Size(302, 22);
			this.mniRecentAlwaysSelected.Text = "Recent Always Selected";
			this.mniRecentAlwaysSelected.Click += new System.EventHandler(this.mniRecentAlwaysSelected_Click);
			// 
			// mniPopupOnIncomingException
			// 
			this.mniPopupOnIncomingException.CheckOnClick = true;
			this.mniPopupOnIncomingException.Name = "mniPopupOnIncomingException";
			this.mniPopupOnIncomingException.Size = new System.Drawing.Size(302, 22);
			this.mniPopupOnIncomingException.Text = "Popup on Every Incoming Exception";
			this.mniPopupOnIncomingException.Click += new System.EventHandler(this.mniPopupOnEveryIncomingException_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(299, 6);
			// 
			// mniShowHeader
			// 
			this.mniShowHeader.CheckOnClick = true;
			this.mniShowHeader.Name = "mniShowHeader";
			this.mniShowHeader.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.mniShowHeader.Size = new System.Drawing.Size(302, 22);
			this.mniShowHeader.Text = "Show Header";
			this.mniShowHeader.Click += new System.EventHandler(this.mniShowHeaders_Click);
			// 
			// mniShowTimestamps
			// 
			this.mniShowTimestamps.CheckOnClick = true;
			this.mniShowTimestamps.Name = "mniShowTimestamps";
			this.mniShowTimestamps.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
			this.mniShowTimestamps.Size = new System.Drawing.Size(302, 22);
			this.mniShowTimestamps.Text = "Show Timestamps";
			this.mniShowTimestamps.Click += new System.EventHandler(this.mniShowTimestamps_Click);
			// 
			// mniShowCounterAndGroup
			// 
			this.mniShowCounterAndGroup.CheckOnClick = true;
			this.mniShowCounterAndGroup.Name = "mniShowCounterAndGroup";
			this.mniShowCounterAndGroup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.mniShowCounterAndGroup.Size = new System.Drawing.Size(302, 22);
			this.mniShowCounterAndGroup.Text = "Show Times Occured";
			this.mniShowCounterAndGroup.Click += new System.EventHandler(this.mniShowCounterAndGroup_Click);
			// 
			// mniShowSearchbar
			// 
			this.mniShowSearchbar.CheckOnClick = true;
			this.mniShowSearchbar.Name = "mniShowSearchbar";
			this.mniShowSearchbar.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.mniShowSearchbar.Size = new System.Drawing.Size(302, 22);
			this.mniShowSearchbar.Text = "Show Search Bar";
			this.mniShowSearchbar.Click += new System.EventHandler(this.mniShowSearchbar_Click);
			// 
			// mniExcludeKeywords
			// 
			this.mniExcludeKeywords.CheckOnClick = true;
			this.mniExcludeKeywords.DropDown = this.ctxIgnoreKeywords;
			this.mniExcludeKeywords.Name = "mniExcludeKeywords";
			this.mniExcludeKeywords.Size = new System.Drawing.Size(302, 22);
			this.mniExcludeKeywords.Text = "Ignore Exceptions Containing";
			this.mniExcludeKeywords.Visible = false;
			this.mniExcludeKeywords.Click += new System.EventHandler(this.mniExcludeKeywordsContaining_Click);
			// 
			// ctxIgnoreKeywords
			// 
			this.ctxIgnoreKeywords.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniTextAndSave});
			this.ctxIgnoreKeywords.Name = "ctxIgnoreKeywords";
			this.ctxIgnoreKeywords.Size = new System.Drawing.Size(261, 307);
			// 
			// mniTextAndSave
			// 
			this.mniTextAndSave.Name = "mniTextAndSave";
			this.mniTextAndSave.Size = new System.Drawing.Size(200, 300);
			this.mniTextAndSave.Text = "mniTextAndSave";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(299, 6);
			// 
			// mniDeleteExceptionsSelected
			// 
			this.mniDeleteExceptionsSelected.Name = "mniDeleteExceptionsSelected";
			this.mniDeleteExceptionsSelected.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniDeleteExceptionsSelected.Size = new System.Drawing.Size(302, 22);
			this.mniDeleteExceptionsSelected.Text = "Delete selected Exceptions";
			this.mniDeleteExceptionsSelected.Click += new System.EventHandler(this.mniDeleteExceptionsSelected_Click);
			// 
			// mniClear
			// 
			this.mniClear.Name = "mniClear";
			this.mniClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniClear.Size = new System.Drawing.Size(302, 22);
			this.mniClear.Text = "Clear";
			this.mniClear.Click += new System.EventHandler(this.mniClear_Click);
			// 
			// mniCopy
			// 
			this.mniCopy.Name = "mniCopy";
			this.mniCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopy.Size = new System.Drawing.Size(302, 22);
			this.mniCopy.Text = "Copy";
			this.mniCopy.Click += new System.EventHandler(this.mniCopy_Click);
			// 
			// mniRebuildAll
			// 
			this.mniRebuildAll.Name = "mniRebuildAll";
			this.mniRebuildAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRebuildAll.Size = new System.Drawing.Size(302, 22);
			this.mniRebuildAll.Text = "Rebuild All";
			this.mniRebuildAll.Click += new System.EventHandler(this.mniTreeRebuildAll_Click);
			// 
			// mniltbFlushToGuiDelayMsec
			// 
			this.mniltbFlushToGuiDelayMsec.BackColor = System.Drawing.Color.Transparent;
			this.mniltbFlushToGuiDelayMsec.InputFieldAlignedRight = false;
			this.mniltbFlushToGuiDelayMsec.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbFlushToGuiDelayMsec.InputFieldEditable = true;
			this.mniltbFlushToGuiDelayMsec.InputFieldMultiline = false;
			this.mniltbFlushToGuiDelayMsec.InputFieldOffsetX = 170;
			this.mniltbFlushToGuiDelayMsec.InputFieldValue = "200";
			this.mniltbFlushToGuiDelayMsec.InputFieldWidth = 40;
			this.mniltbFlushToGuiDelayMsec.Name = "mniltbFlushToGuiDelayMsec";
			this.mniltbFlushToGuiDelayMsec.OffsetTop = 0;
			this.mniltbFlushToGuiDelayMsec.Size = new System.Drawing.Size(240, 18);
			this.mniltbFlushToGuiDelayMsec.TextLeft = "Flush To GUI Delay";
			this.mniltbFlushToGuiDelayMsec.TextLeftOffsetX = 0;
			this.mniltbFlushToGuiDelayMsec.TextLeftWidth = 108;
			this.mniltbFlushToGuiDelayMsec.TextRed = false;
			this.mniltbFlushToGuiDelayMsec.TextRight = "ms";
			this.mniltbFlushToGuiDelayMsec.TextRightOffsetX = 210;
			this.mniltbFlushToGuiDelayMsec.TextRightWidth = 27;
			this.mniltbFlushToGuiDelayMsec.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbFlushToGuiDelayMsec_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(299, 6);
			// 
			// mniltbSerializationInterval
			// 
			this.mniltbSerializationInterval.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSerializationInterval.InputFieldAlignedRight = false;
			this.mniltbSerializationInterval.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSerializationInterval.InputFieldEditable = true;
			this.mniltbSerializationInterval.InputFieldMultiline = true;
			this.mniltbSerializationInterval.InputFieldOffsetX = 170;
			this.mniltbSerializationInterval.InputFieldValue = "3000";
			this.mniltbSerializationInterval.InputFieldWidth = 40;
			this.mniltbSerializationInterval.Name = "mniltbSerializationInterval";
			this.mniltbSerializationInterval.OffsetTop = 0;
			this.mniltbSerializationInterval.Size = new System.Drawing.Size(240, 22);
			this.mniltbSerializationInterval.Text = "mniltbDelaySerializationSync";
			this.mniltbSerializationInterval.TextLeft = "Serialize every (logrotate)";
			this.mniltbSerializationInterval.TextLeftOffsetX = 0;
			this.mniltbSerializationInterval.TextLeftWidth = 141;
			this.mniltbSerializationInterval.TextRed = false;
			this.mniltbSerializationInterval.TextRight = "ms";
			this.mniltbSerializationInterval.TextRightOffsetX = 210;
			this.mniltbSerializationInterval.TextRightWidth = 27;
			// 
			// mniSerializeNow
			// 
			this.mniSerializeNow.Name = "mniSerializeNow";
			this.mniSerializeNow.Size = new System.Drawing.Size(302, 22);
			this.mniSerializeNow.Text = "Serialize now";
			this.mniSerializeNow.Click += new System.EventHandler(this.mniSerializeNow_Click);
			// 
			// mniltbLogrotateLargerThan
			// 
			this.mniltbLogrotateLargerThan.BackColor = System.Drawing.Color.Transparent;
			this.mniltbLogrotateLargerThan.InputFieldAlignedRight = false;
			this.mniltbLogrotateLargerThan.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbLogrotateLargerThan.InputFieldEditable = true;
			this.mniltbLogrotateLargerThan.InputFieldMultiline = true;
			this.mniltbLogrotateLargerThan.InputFieldOffsetX = 170;
			this.mniltbLogrotateLargerThan.InputFieldValue = "3000";
			this.mniltbLogrotateLargerThan.InputFieldWidth = 40;
			this.mniltbLogrotateLargerThan.Name = "mniltbLogrotateLargerThan";
			this.mniltbLogrotateLargerThan.OffsetTop = 0;
			this.mniltbLogrotateLargerThan.Size = new System.Drawing.Size(242, 22);
			this.mniltbLogrotateLargerThan.Text = "mniltbLogrotateLargerThan";
			this.mniltbLogrotateLargerThan.TextLeft = "Logrotate if larger than";
			this.mniltbLogrotateLargerThan.TextLeftOffsetX = 0;
			this.mniltbLogrotateLargerThan.TextLeftWidth = 130;
			this.mniltbLogrotateLargerThan.TextRed = false;
			this.mniltbLogrotateLargerThan.TextRight = "Mb";
			this.mniltbLogrotateLargerThan.TextRightOffsetX = 210;
			this.mniltbLogrotateLargerThan.TextRightWidth = 29;
			this.mniltbLogrotateLargerThan.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbLogrotateLargerThan_UserTyped);
			// 
			// mniDeleteAllLogrotatedExceptionJsons
			// 
			this.mniDeleteAllLogrotatedExceptionJsons.Name = "mniDeleteAllLogrotatedExceptionJsons";
			this.mniDeleteAllLogrotatedExceptionJsons.Size = new System.Drawing.Size(302, 22);
			this.mniDeleteAllLogrotatedExceptionJsons.Text = "Delete All[x] logrotated Exception*.json";
			this.mniDeleteAllLogrotatedExceptionJsons.Click += new System.EventHandler(this.mniDeleteAllLogrotatedExceptionJsons_Click);
			// 
			// splitContainerVertical
			// 
			this.splitContainerVertical.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerVertical.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerVertical.Location = new System.Drawing.Point(0, 1);
			this.splitContainerVertical.Name = "splitContainerVertical";
			// 
			// splitContainerVertical.Panel1
			// 
			this.splitContainerVertical.Panel1.Controls.Add(this.pnlSearch);
			this.splitContainerVertical.Panel1.Controls.Add(this.olvTreeExceptions);
			this.splitContainerVertical.Panel1MinSize = 40;
			// 
			// splitContainerVertical.Panel2
			// 
			this.splitContainerVertical.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerVertical.Panel2.Controls.Add(this.splitContainerHorizontal);
			this.splitContainerVertical.Size = new System.Drawing.Size(543, 336);
			this.splitContainerVertical.SplitterDistance = 226;
			this.splitContainerVertical.SplitterWidth = 5;
			this.splitContainerVertical.TabIndex = 6;
			// 
			// pnlSearch
			// 
			this.pnlSearch.BackColor = System.Drawing.SystemColors.Control;
			this.pnlSearch.ColumnCount = 3;
			this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.pnlSearch.Controls.Add(this.btnSearchClose, 2, 0);
			this.pnlSearch.Controls.Add(this.txtSearch, 1, 0);
			this.pnlSearch.Controls.Add(this.btnSearchClear, 0, 0);
			this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlSearch.Location = new System.Drawing.Point(0, 312);
			this.pnlSearch.MinimumSize = new System.Drawing.Size(0, 23);
			this.pnlSearch.Name = "pnlSearch";
			this.pnlSearch.RowCount = 1;
			this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.Size = new System.Drawing.Size(226, 24);
			this.pnlSearch.TabIndex = 6;
			this.pnlSearch.Visible = false;
			// 
			// btnSearchClose
			// 
			this.btnSearchClose.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearchClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSearchClose.Location = new System.Drawing.Point(201, 0);
			this.btnSearchClose.Margin = new System.Windows.Forms.Padding(0);
			this.btnSearchClose.Name = "btnSearchClose";
			this.btnSearchClose.Size = new System.Drawing.Size(25, 24);
			this.btnSearchClose.TabIndex = 4;
			this.btnSearchClose.Text = "X";
			this.btnSearchClose.UseVisualStyleBackColor = true;
			this.btnSearchClose.Click += new System.EventHandler(this.btnSearchClose_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSearch.Location = new System.Drawing.Point(47, 2);
			this.txtSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(152, 20);
			this.txtSearch.TabIndex = 3;
			this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
			// 
			// btnSearchClear
			// 
			this.btnSearchClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearchClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSearchClear.Location = new System.Drawing.Point(0, 0);
			this.btnSearchClear.Margin = new System.Windows.Forms.Padding(0);
			this.btnSearchClear.Name = "btnSearchClear";
			this.btnSearchClear.Size = new System.Drawing.Size(45, 24);
			this.btnSearchClear.TabIndex = 5;
			this.btnSearchClear.Text = "Clear";
			this.btnSearchClear.UseVisualStyleBackColor = true;
			this.btnSearchClear.Click += new System.EventHandler(this.btnSearchClear_Click);
			// 
			// splitContainerHorizontal
			// 
			this.splitContainerHorizontal.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerHorizontal.Location = new System.Drawing.Point(0, 0);
			this.splitContainerHorizontal.Name = "splitContainerHorizontal";
			this.splitContainerHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerHorizontal.Panel1
			// 
			this.splitContainerHorizontal.Panel1.Controls.Add(this.txtExceptionMessage);
			// 
			// splitContainerHorizontal.Panel2
			// 
			this.splitContainerHorizontal.Panel2.Controls.Add(this.olvStackTrace);
			this.splitContainerHorizontal.Size = new System.Drawing.Size(312, 336);
			this.splitContainerHorizontal.SplitterDistance = 174;
			this.splitContainerHorizontal.SplitterWidth = 5;
			this.splitContainerHorizontal.TabIndex = 15;
			// 
			// txtExceptionMessage
			// 
			this.txtExceptionMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtExceptionMessage.ContextMenuStrip = this.ctxExceptionMessage;
			this.txtExceptionMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtExceptionMessage.Location = new System.Drawing.Point(0, 0);
			this.txtExceptionMessage.Multiline = true;
			this.txtExceptionMessage.Name = "txtExceptionMessage";
			this.txtExceptionMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtExceptionMessage.Size = new System.Drawing.Size(312, 174);
			this.txtExceptionMessage.TabIndex = 1;
			this.txtExceptionMessage.TabStop = false;
			// 
			// ctxExceptionMessage
			// 
			this.ctxExceptionMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniAddToExcludeKeywordsCsv});
			this.ctxExceptionMessage.Name = "ctxExceptionMessage";
			this.ctxExceptionMessage.Size = new System.Drawing.Size(223, 26);
			this.ctxExceptionMessage.Opening += new System.ComponentModel.CancelEventHandler(this.ctxExceptionMessage_Opening);
			// 
			// mniAddToExcludeKeywordsCsv
			// 
			this.mniAddToExcludeKeywordsCsv.Name = "mniAddToExcludeKeywordsCsv";
			this.mniAddToExcludeKeywordsCsv.Size = new System.Drawing.Size(222, 22);
			this.mniAddToExcludeKeywordsCsv.Text = "Add to ExcludeKeywords list";
			this.mniAddToExcludeKeywordsCsv.Click += new System.EventHandler(this.mniAddToExcludeKeywordsCsv_Click);
			// 
			// olvStackTrace
			// 
			this.olvStackTrace.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvStackTrace.AllowColumnReorder = true;
			this.olvStackTrace.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvStackTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvhMethod,
            this.lvhDeclaringClass,
            this.lvhLine,
            this.lvhFile});
			this.olvStackTrace.ContextMenuStrip = this.ctxCallStack;
			this.olvStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvStackTrace.FullRowSelect = true;
			this.olvStackTrace.HideSelection = false;
			this.olvStackTrace.HotTracking = true;
			this.olvStackTrace.HoverSelection = true;
			this.olvStackTrace.Location = new System.Drawing.Point(0, 0);
			this.olvStackTrace.Name = "olvStackTrace";
			this.olvStackTrace.ShowItemToolTips = true;
			this.olvStackTrace.Size = new System.Drawing.Size(312, 157);
			this.olvStackTrace.TabIndex = 12;
			this.olvStackTrace.UseCompatibleStateImageBehavior = false;
			this.olvStackTrace.View = System.Windows.Forms.View.Details;
			// 
			// lvhMethod
			// 
			this.lvhMethod.Text = "Method";
			this.lvhMethod.Width = 260;
			// 
			// lvhDeclaringClass
			// 
			this.lvhDeclaringClass.Text = "Class";
			this.lvhDeclaringClass.Width = 160;
			// 
			// lvhLine
			// 
			this.lvhLine.Text = "Line";
			this.lvhLine.Width = 20;
			// 
			// lvhFile
			// 
			this.lvhFile.Text = "File";
			this.lvhFile.Width = 80;
			// 
			// ctxCallStack
			// 
			this.ctxCallStack.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCopyStackPosition});
			this.ctxCallStack.Name = "ctxCallStack";
			this.ctxCallStack.Size = new System.Drawing.Size(145, 26);
			// 
			// mniCopyStackPosition
			// 
			this.mniCopyStackPosition.Name = "mniCopyStackPosition";
			this.mniCopyStackPosition.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopyStackPosition.Size = new System.Drawing.Size(144, 22);
			this.mniCopyStackPosition.Text = "Copy";
			this.mniCopyStackPosition.Click += new System.EventHandler(this.mniCopyStackPosition_Click);
			// 
			// statusStrip_search
			// 
			this.statusStrip_search.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiBtnClear,
            this.tsilbl_separator,
            this.tsiCbx_SearchApply,
            this.tsiLtb_SearchKeywords,
            this.tsiCbx_ExcludeApply,
            this.tsiLtb_ExcludeKeywords});
			this.statusStrip_search.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip_search.Location = new System.Drawing.Point(0, 337);
			this.statusStrip_search.Name = "statusStrip_search";
			this.statusStrip_search.Size = new System.Drawing.Size(543, 24);
			this.statusStrip_search.SizingGrip = false;
			this.statusStrip_search.TabIndex = 14;
			this.statusStrip_search.Text = "statusStrip1";
			// 
			// tsiBtnClear
			// 
			this.tsiBtnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiBtnClear.Name = "tsiBtnClear";
			this.tsiBtnClear.Size = new System.Drawing.Size(38, 22);
			this.tsiBtnClear.Text = "Clear";
			this.tsiBtnClear.Click += new System.EventHandler(this.tsiBtnClear_Click);
			// 
			// tsilbl_separator
			// 
			this.tsilbl_separator.Name = "tsilbl_separator";
			this.tsilbl_separator.Size = new System.Drawing.Size(16, 19);
			this.tsilbl_separator.Text = "   ";
			// 
			// tsiCbx_SearchApply
			// 
			this.tsiCbx_SearchApply.AutoSize = false;
			this.tsiCbx_SearchApply.CheckBoxChecked = true;
			this.tsiCbx_SearchApply.CheckBoxText = "Search for:";
			this.tsiCbx_SearchApply.Name = "tsiCbx_SearchApply";
			this.tsiCbx_SearchApply.Size = new System.Drawing.Size(82, 22);
			this.tsiCbx_SearchApply.Text = "Search for:";
			// 
			// tsiLtb_SearchKeywords
			// 
			this.tsiLtb_SearchKeywords.AutoSize = false;
			this.tsiLtb_SearchKeywords.BackColor = System.Drawing.SystemColors.Control;
			this.tsiLtb_SearchKeywords.InputFieldAlignedRight = false;
			this.tsiLtb_SearchKeywords.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtb_SearchKeywords.InputFieldEditable = true;
			this.tsiLtb_SearchKeywords.InputFieldMultiline = false;
			this.tsiLtb_SearchKeywords.InputFieldOffsetX = 0;
			this.tsiLtb_SearchKeywords.InputFieldValue = "";
			this.tsiLtb_SearchKeywords.InputFieldWidth = 110;
			this.tsiLtb_SearchKeywords.Margin = new System.Windows.Forms.Padding(0);
			this.tsiLtb_SearchKeywords.Name = "tsiLtb_SearchKeywords";
			this.tsiLtb_SearchKeywords.OffsetTop = 0;
			this.tsiLtb_SearchKeywords.Size = new System.Drawing.Size(130, 24);
			this.tsiLtb_SearchKeywords.Text = "toolStripItemLabeledTextBox1";
			this.tsiLtb_SearchKeywords.TextLeft = "";
			this.tsiLtb_SearchKeywords.TextLeftOffsetX = 0;
			this.tsiLtb_SearchKeywords.TextLeftWidth = 2;
			this.tsiLtb_SearchKeywords.TextRed = false;
			this.tsiLtb_SearchKeywords.TextRight = "";
			this.tsiLtb_SearchKeywords.TextRightOffsetX = 115;
			this.tsiLtb_SearchKeywords.TextRightWidth = 4;
			// 
			// tsiCbx_ExcludeApply
			// 
			this.tsiCbx_ExcludeApply.AutoSize = false;
			this.tsiCbx_ExcludeApply.CheckBoxChecked = true;
			this.tsiCbx_ExcludeApply.CheckBoxText = "Exclude:";
			this.tsiCbx_ExcludeApply.Name = "tsiCbx_ExcludeApply";
			this.tsiCbx_ExcludeApply.Size = new System.Drawing.Size(69, 22);
			this.tsiCbx_ExcludeApply.Text = "Exclude:";
			// 
			// tsiLtb_ExcludeKeywords
			// 
			this.tsiLtb_ExcludeKeywords.AutoSize = false;
			this.tsiLtb_ExcludeKeywords.BackColor = System.Drawing.Color.Transparent;
			this.tsiLtb_ExcludeKeywords.InputFieldAlignedRight = false;
			this.tsiLtb_ExcludeKeywords.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtb_ExcludeKeywords.InputFieldEditable = true;
			this.tsiLtb_ExcludeKeywords.InputFieldMultiline = false;
			this.tsiLtb_ExcludeKeywords.InputFieldOffsetX = 0;
			this.tsiLtb_ExcludeKeywords.InputFieldValue = "";
			this.tsiLtb_ExcludeKeywords.InputFieldWidth = 170;
			this.tsiLtb_ExcludeKeywords.Margin = new System.Windows.Forms.Padding(0, -1, 0, 0);
			this.tsiLtb_ExcludeKeywords.Name = "tsiLtb_ExcludeKeywords";
			this.tsiLtb_ExcludeKeywords.OffsetTop = 0;
			this.tsiLtb_ExcludeKeywords.Size = new System.Drawing.Size(182, 24);
			this.tsiLtb_ExcludeKeywords.Text = "toolStripItemLabeledTextBox1";
			this.tsiLtb_ExcludeKeywords.TextLeft = "";
			this.tsiLtb_ExcludeKeywords.TextLeftOffsetX = 0;
			this.tsiLtb_ExcludeKeywords.TextLeftWidth = 2;
			this.tsiLtb_ExcludeKeywords.TextRed = false;
			this.tsiLtb_ExcludeKeywords.TextRight = "";
			this.tsiLtb_ExcludeKeywords.TextRightOffsetX = 175;
			this.tsiLtb_ExcludeKeywords.TextRightWidth = 4;
			// 
			// mniExpandAll
			// 
			this.mniExpandAll.Name = "mniExpandAll";
			this.mniExpandAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
			this.mniExpandAll.Size = new System.Drawing.Size(302, 22);
			this.mniExpandAll.Text = "Expand All";
			this.mniExpandAll.Click += new System.EventHandler(this.mniTreeExpandAll_Click);
			// 
			// mniCollapseAll
			// 
			this.mniCollapseAll.Name = "mniCollapseAll";
			this.mniCollapseAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniCollapseAll.Size = new System.Drawing.Size(302, 22);
			this.mniCollapseAll.Text = "Collapse All";
			this.mniCollapseAll.Click += new System.EventHandler(this.mniTreeCollapseAll_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(299, 6);
			// 
			// ExceptionsControl
			// 
			this.Controls.Add(this.statusStrip_search);
			this.Controls.Add(this.splitContainerVertical);
			this.Name = "ExceptionsControl";
			this.Size = new System.Drawing.Size(543, 361);
			this.ResizeStopped += new System.EventHandler<System.EventArgs>(this.exceptionsControl_ResizeStopped);
			this.Load += new System.EventHandler(this.form_OnLoad);
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).EndInit();
			this.ctxTree.ResumeLayout(false);
			this.ctxIgnoreKeywords.ResumeLayout(false);
			this.splitContainerVertical.Panel1.ResumeLayout(false);
			this.splitContainerVertical.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
			this.splitContainerVertical.ResumeLayout(false);
			this.pnlSearch.ResumeLayout(false);
			this.pnlSearch.PerformLayout();
			this.splitContainerHorizontal.Panel1.ResumeLayout(false);
			this.splitContainerHorizontal.Panel1.PerformLayout();
			this.splitContainerHorizontal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).EndInit();
			this.splitContainerHorizontal.ResumeLayout(false);
			this.ctxExceptionMessage.ResumeLayout(false);
			this.ctxCallStack.ResumeLayout(false);
			this.statusStrip_search.ResumeLayout(false);
			this.statusStrip_search.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.ToolStripMenuItem mniShowTimestamps;
		private TreeListView olvTreeExceptions;
		private TextBox txtExceptionMessage;
		private ListView olvStackTrace;
		private ColumnHeader lvhFile;
		private ColumnHeader lvhLine;
		private ColumnHeader lvhMethod;
		private ColumnHeader lvhDeclaringClass;
		private SplitContainer splitContainerVertical;
		private SplitContainer splitContainerHorizontal;

		private BrightIdeasSoftware.OLVColumn olvcTimestamp;
		private System.Windows.Forms.ToolStripMenuItem mniCopyStackPosition;
		private System.Windows.Forms.ContextMenuStrip ctxCallStack;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbFlushToGuiDelayMsec;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniRecentAlwaysSelected;
		private System.Windows.Forms.ToolStripMenuItem mniCopy;
		private System.Windows.Forms.ToolStripMenuItem mniClear;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenuStrip ctxTree;
		private ToolStripMenuItem mniRebuildAll;
		private ToolStripMenuItem mniShowHeader;
		private OLVColumn olvcException;
		private ToolStripMenuItem mniPopupOnIncomingException;
		private ToolStripMenuItem mniShowSearchbar;
		private ToolStripMenuItem mniShowCounterAndGroup;
		private TableLayoutPanel pnlSearch;
		private TextBox txtSearch;
		private Button btnSearchClose;
		private OLVColumn olvcTimesOccured;
		private ToolStripSeparator toolStripSeparator3;
		private Button btnSearchClear;
		private ContextMenuStrip ctxIgnoreKeywords;
		private TextAndSave.MenuItemTextAndSave mniTextAndSave;
		private ToolStripMenuItem mniExcludeKeywords;
		private StatusStrip statusStrip_search;
		public ToolStripButton tsiBtnClear;
		private ToolStripImproved.ToolStripItemCheckBox tsiCbx_SearchApply;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtb_SearchKeywords;
		private ToolStripStatusLabel tsilbl_separator;
		private ToolStripImproved.ToolStripItemCheckBox tsiCbx_ExcludeApply;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtb_ExcludeKeywords;
		private ContextMenuStrip ctxExceptionMessage;
		private ToolStripMenuItem mniAddToExcludeKeywordsCsv;
		private LabeledTextBox.ToolStripItemLabeledTextBox mniltbSerializationInterval;
		private ToolStripMenuItem mniSerializeNow;
		private ToolStripMenuItem mniDeleteAllLogrotatedExceptionJsons;
		private LabeledTextBox.ToolStripItemLabeledTextBox mniltbLogrotateLargerThan;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mniDeleteExceptionsSelected;
		private ToolStripMenuItem mniExpandAll;
		private ToolStripMenuItem mniCollapseAll;
		private ToolStripSeparator toolStripSeparator4;
		
	}
}