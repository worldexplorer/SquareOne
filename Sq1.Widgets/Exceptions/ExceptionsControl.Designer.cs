using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using BrightIdeasSoftware;

using Sq1.Core.Support;

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
			this.mniClear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniRecentAlwaysSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniPopupOnIncomingException = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniShowHeader = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowTimestamps = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowCounterAndGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSearchbar = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbFlushToGuiDelayMsec = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
			this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.btnSearchClose = new System.Windows.Forms.Button();
			this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtExceptionMessage = new System.Windows.Forms.TextBox();
			this.olvStackTrace = new System.Windows.Forms.ListView();
			this.lvhMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhDeclaringClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ctxCallStack = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCopyStackPosition = new System.Windows.Forms.ToolStripMenuItem();
			this.btnSearchClear = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).BeginInit();
			this.ctxTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
			this.splitContainerVertical.Panel1.SuspendLayout();
			this.splitContainerVertical.Panel2.SuspendLayout();
			this.splitContainerVertical.SuspendLayout();
			this.pnlSearch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).BeginInit();
			this.splitContainerHorizontal.Panel1.SuspendLayout();
			this.splitContainerHorizontal.Panel2.SuspendLayout();
			this.splitContainerHorizontal.SuspendLayout();
			this.ctxCallStack.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvTreeExceptions
			// 
			this.olvTreeExceptions.AllColumns.Add(this.olvcException);
			this.olvTreeExceptions.AllColumns.Add(this.olvcTimestamp);
			this.olvTreeExceptions.AllColumns.Add(this.olvcTimesOccured);
			this.olvTreeExceptions.AllowColumnReorder = true;
			this.olvTreeExceptions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvTreeExceptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcException,
            this.olvcTimestamp});
			this.olvTreeExceptions.ContextMenuStrip = this.ctxTree;
			this.olvTreeExceptions.CopySelectionOnControlC = false;
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
			this.olvTreeExceptions.Size = new System.Drawing.Size(213, 361);
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
			this.olvcTimesOccured.DisplayIndex = 2;
			this.olvcTimesOccured.IsVisible = false;
			this.olvcTimesOccured.Text = "Times";
			this.olvcTimesOccured.Width = 40;
			// 
			// ctxTree
			// 
			this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniClear,
            this.toolStripSeparator2,
            this.mniRecentAlwaysSelected,
            this.mniPopupOnIncomingException,
            this.toolStripSeparator3,
            this.mniShowHeader,
            this.mniShowTimestamps,
            this.mniShowCounterAndGroup,
            this.mniShowSearchbar,
            this.toolStripSeparator1,
            this.mniCopy,
            this.mniRefresh,
            this.mniltbFlushToGuiDelayMsec});
			this.ctxTree.Name = "ctx";
			this.ctxTree.Size = new System.Drawing.Size(266, 244);
			// 
			// mniClear
			// 
			this.mniClear.Name = "mniClear";
			this.mniClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniClear.Size = new System.Drawing.Size(265, 22);
			this.mniClear.Text = "Clear";
			this.mniClear.Click += new System.EventHandler(this.mniClear_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(262, 6);
			// 
			// mniRecentAlwaysSelected
			// 
			this.mniRecentAlwaysSelected.CheckOnClick = true;
			this.mniRecentAlwaysSelected.Name = "mniRecentAlwaysSelected";
			this.mniRecentAlwaysSelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRecentAlwaysSelected.Size = new System.Drawing.Size(265, 22);
			this.mniRecentAlwaysSelected.Text = "Recent Always Selected";
			this.mniRecentAlwaysSelected.Click += new System.EventHandler(this.mniRecentAlwaysSelected_Click);
			// 
			// mniPopupOnIncomingException
			// 
			this.mniPopupOnIncomingException.CheckOnClick = true;
			this.mniPopupOnIncomingException.Name = "mniPopupOnIncomingException";
			this.mniPopupOnIncomingException.Size = new System.Drawing.Size(265, 22);
			this.mniPopupOnIncomingException.Text = "Popup on Every Incoming Exception";
			this.mniPopupOnIncomingException.Click += new System.EventHandler(this.mniPopupOnEveryIncomingException_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(262, 6);
			// 
			// mniShowHeader
			// 
			this.mniShowHeader.CheckOnClick = true;
			this.mniShowHeader.Name = "mniShowHeader";
			this.mniShowHeader.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.mniShowHeader.Size = new System.Drawing.Size(265, 22);
			this.mniShowHeader.Text = "Show Header";
			this.mniShowHeader.Click += new System.EventHandler(this.mniShowHeaders_Click);
			// 
			// mniShowTimestamps
			// 
			this.mniShowTimestamps.CheckOnClick = true;
			this.mniShowTimestamps.Name = "mniShowTimestamps";
			this.mniShowTimestamps.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
			this.mniShowTimestamps.Size = new System.Drawing.Size(265, 22);
			this.mniShowTimestamps.Text = "Show Timestamps";
			this.mniShowTimestamps.Click += new System.EventHandler(this.mniShowTimestamps_Click);
			// 
			// mniShowCounterAndGroup
			// 
			this.mniShowCounterAndGroup.CheckOnClick = true;
			this.mniShowCounterAndGroup.Name = "mniShowCounterAndGroup";
			this.mniShowCounterAndGroup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.mniShowCounterAndGroup.Size = new System.Drawing.Size(265, 22);
			this.mniShowCounterAndGroup.Text = "Show Times Occured";
			this.mniShowCounterAndGroup.Click += new System.EventHandler(this.mniShowCounterAndGroup_Click);
			// 
			// mniShowSearchbar
			// 
			this.mniShowSearchbar.CheckOnClick = true;
			this.mniShowSearchbar.Name = "mniShowSearchbar";
			this.mniShowSearchbar.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.mniShowSearchbar.Size = new System.Drawing.Size(265, 22);
			this.mniShowSearchbar.Text = "Show Search Bar";
			this.mniShowSearchbar.Click += new System.EventHandler(this.mniShowSearchbar_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
			// 
			// mniCopy
			// 
			this.mniCopy.Name = "mniCopy";
			this.mniCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopy.Size = new System.Drawing.Size(265, 22);
			this.mniCopy.Text = "Copy";
			this.mniCopy.Click += new System.EventHandler(this.mniCopy_Click);
			// 
			// mniRefresh
			// 
			this.mniRefresh.Name = "mniRefresh";
			this.mniRefresh.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRefresh.Size = new System.Drawing.Size(265, 22);
			this.mniRefresh.Text = "Refresh";
			this.mniRefresh.Click += new System.EventHandler(this.mniRefresh_Click);
			// 
			// mniltbFlushToGuiDelayMsec
			// 
			this.mniltbFlushToGuiDelayMsec.BackColor = System.Drawing.Color.Transparent;
			this.mniltbFlushToGuiDelayMsec.InputFieldAlignedRight = false;
			this.mniltbFlushToGuiDelayMsec.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbFlushToGuiDelayMsec.InputFieldEditable = true;
			this.mniltbFlushToGuiDelayMsec.InputFieldMultiline = false;
			this.mniltbFlushToGuiDelayMsec.InputFieldOffsetX = 120;
			this.mniltbFlushToGuiDelayMsec.InputFieldValue = "200";
			this.mniltbFlushToGuiDelayMsec.InputFieldWidth = 40;
			this.mniltbFlushToGuiDelayMsec.Name = "mniltbFlushToGuiDelayMsec";
			this.mniltbFlushToGuiDelayMsec.OffsetTop = 0;
			this.mniltbFlushToGuiDelayMsec.Size = new System.Drawing.Size(187, 21);
			this.mniltbFlushToGuiDelayMsec.TextLeft = "Flush To GUI Delay";
			this.mniltbFlushToGuiDelayMsec.TextLeftOffsetX = 0;
			this.mniltbFlushToGuiDelayMsec.TextLeftWidth = 108;
			this.mniltbFlushToGuiDelayMsec.TextRed = false;
			this.mniltbFlushToGuiDelayMsec.TextRight = "ms";
			this.mniltbFlushToGuiDelayMsec.TextRightOffsetX = 160;
			this.mniltbFlushToGuiDelayMsec.TextRightWidth = 27;
			this.mniltbFlushToGuiDelayMsec.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbFlushToGuiDelayMsec_UserTyped);
			// 
			// splitContainerVertical
			// 
			this.splitContainerVertical.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerVertical.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerVertical.Location = new System.Drawing.Point(0, 0);
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
			this.splitContainerVertical.Size = new System.Drawing.Size(509, 361);
			this.splitContainerVertical.SplitterDistance = 213;
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
			this.pnlSearch.Location = new System.Drawing.Point(0, 335);
			this.pnlSearch.MinimumSize = new System.Drawing.Size(0, 26);
			this.pnlSearch.Name = "pnlSearch";
			this.pnlSearch.RowCount = 1;
			this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlSearch.Size = new System.Drawing.Size(213, 26);
			this.pnlSearch.TabIndex = 6;
			this.pnlSearch.Visible = false;
			// 
			// txtSearch
			// 
			this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSearch.Location = new System.Drawing.Point(48, 3);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(137, 20);
			this.txtSearch.TabIndex = 3;
			this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
			// 
			// btnSearchClose
			// 
			this.btnSearchClose.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearchClose.Location = new System.Drawing.Point(191, 3);
			this.btnSearchClose.Name = "btnSearchClose";
			this.btnSearchClose.Size = new System.Drawing.Size(19, 20);
			this.btnSearchClose.TabIndex = 4;
			this.btnSearchClose.Text = "X";
			this.btnSearchClose.UseVisualStyleBackColor = true;
			this.btnSearchClose.Click += new System.EventHandler(this.btnSearchClose_Click);
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
			this.splitContainerHorizontal.Size = new System.Drawing.Size(291, 361);
			this.splitContainerHorizontal.SplitterDistance = 189;
			this.splitContainerHorizontal.SplitterWidth = 5;
			this.splitContainerHorizontal.TabIndex = 15;
			// 
			// txtExceptionMessage
			// 
			this.txtExceptionMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtExceptionMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtExceptionMessage.Location = new System.Drawing.Point(0, 0);
			this.txtExceptionMessage.Multiline = true;
			this.txtExceptionMessage.Name = "txtExceptionMessage";
			this.txtExceptionMessage.Size = new System.Drawing.Size(291, 189);
			this.txtExceptionMessage.TabIndex = 1;
			this.txtExceptionMessage.TabStop = false;
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
			this.olvStackTrace.Size = new System.Drawing.Size(291, 167);
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
			// btnSearchClear
			// 
			this.btnSearchClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearchClear.Location = new System.Drawing.Point(3, 3);
			this.btnSearchClear.Name = "btnSearchClear";
			this.btnSearchClear.Size = new System.Drawing.Size(39, 20);
			this.btnSearchClear.TabIndex = 5;
			this.btnSearchClear.Text = "Clear";
			this.btnSearchClear.UseVisualStyleBackColor = true;
			this.btnSearchClear.Click += new System.EventHandler(this.btnSearchClear_Click);
			// 
			// ExceptionsControl
			// 
			this.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.Controls.Add(this.splitContainerVertical);
			this.Name = "ExceptionsControl";
			this.Size = new System.Drawing.Size(509, 361);
			this.ResizeStopped += new System.EventHandler<System.EventArgs>(this.exceptionsControl_ResizeStopped);
			this.Load += new System.EventHandler(this.form_OnLoad);
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).EndInit();
			this.ctxTree.ResumeLayout(false);
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
			this.ctxCallStack.ResumeLayout(false);
			this.ResumeLayout(false);

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
		private ToolStripMenuItem mniRefresh;
		private ToolStripMenuItem mniShowHeader;
		private OLVColumn olvcException;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mniPopupOnIncomingException;
		private ToolStripMenuItem mniShowSearchbar;
		private ToolStripMenuItem mniShowCounterAndGroup;
		private TableLayoutPanel pnlSearch;
		private TextBox txtSearch;
		private Button btnSearchClose;
		private OLVColumn olvcTimesOccured;
		private ToolStripSeparator toolStripSeparator3;
		private Button btnSearchClear;
		
	}
}