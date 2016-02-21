using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using BrightIdeasSoftware;

namespace Sq1.Widgets.Exceptions {
	[ToolboxBitmap(typeof(ExceptionsControl), "ExceptionsControl")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]

#if USE_CONTROL_IMPROVED
	public partial class ExceptionsControl : UserControlImproved {
#else
	public partial class ExceptionsControl : UserControl {
#endif


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfgh"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dfghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Occured")]
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.olvTreeExceptions = new BrightIdeasSoftware.TreeListView();
			this.olvcException = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniClear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniPopupOnIncomingException = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRecentAlwaysSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowTimestamps = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowHeaders = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbDelay = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
			this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtExceptionMessage = new System.Windows.Forms.TextBox();
			this.olvStackTrace = new System.Windows.Forms.ListView();
			this.lvhMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhDeclaringClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ctxCallStack = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCopyStackPosition = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).BeginInit();
			this.ctxTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
			this.splitContainerVertical.Panel1.SuspendLayout();
			this.splitContainerVertical.Panel2.SuspendLayout();
			this.splitContainerVertical.SuspendLayout();
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
			this.olvTreeExceptions.AllColumns.Add(this.olvcTime);
			this.olvTreeExceptions.AllowColumnReorder = true;
			this.olvTreeExceptions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvTreeExceptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcException,
            this.olvcTime});
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
			// olvcTime
			// 
			this.olvcTime.Text = "Occured";
			this.olvcTime.Width = 80;
			// 
			// ctxTree
			// 
			this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniClear,
            this.toolStripSeparator2,
            this.mniPopupOnIncomingException,
            this.mniRecentAlwaysSelected,
            this.mniShowTimestamps,
            this.mniShowHeaders,
            this.toolStripSeparator1,
            this.mniltbDelay,
            this.mniRefresh,
            this.mniCopy});
			this.ctxTree.Name = "ctx";
			this.ctxTree.Size = new System.Drawing.Size(266, 194);
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
			// mniPopupOnIncomingException
			// 
			this.mniPopupOnIncomingException.CheckOnClick = true;
			this.mniPopupOnIncomingException.Name = "mniPopupOnIncomingException";
			this.mniPopupOnIncomingException.Size = new System.Drawing.Size(265, 22);
			this.mniPopupOnIncomingException.Text = "Popup on Every Incoming Exception";
			this.mniPopupOnIncomingException.Click += new System.EventHandler(this.mniPopupOnEveryIncomingException_Click);
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
			// mniShowTimestamps
			// 
			this.mniShowTimestamps.CheckOnClick = true;
			this.mniShowTimestamps.Name = "mniShowTimestamps";
			this.mniShowTimestamps.Size = new System.Drawing.Size(265, 22);
			this.mniShowTimestamps.Text = "Show Timestamps";
			this.mniShowTimestamps.Click += new System.EventHandler(this.mniShowTimestamps_Click);
			// 
			// mniShowHeaders
			// 
			this.mniShowHeaders.CheckOnClick = true;
			this.mniShowHeaders.Name = "mniShowHeaders";
			this.mniShowHeaders.Size = new System.Drawing.Size(265, 22);
			this.mniShowHeaders.Text = "Show Headers";
			this.mniShowHeaders.Click += new System.EventHandler(this.mniShowHeaders_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
			// 
			// mniltbDelay
			// 
			this.mniltbDelay.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDelay.InputFieldAlignedRight = false;
			this.mniltbDelay.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbDelay.InputFieldEditable = true;
			this.mniltbDelay.InputFieldMultiline = false;
			this.mniltbDelay.InputFieldOffsetX = 113;
			this.mniltbDelay.InputFieldValue = "200";
			this.mniltbDelay.InputFieldWidth = 40;
			this.mniltbDelay.Name = "mniltbDelay";
			this.mniltbDelay.OffsetTop = 0;
			this.mniltbDelay.Size = new System.Drawing.Size(180, 21);
			this.mniltbDelay.TextLeft = "Delay for buffering";
			this.mniltbDelay.TextLeftOffsetX = 0;
			this.mniltbDelay.TextLeftWidth = 108;
			this.mniltbDelay.TextRed = false;
			this.mniltbDelay.TextRight = "ms";
			this.mniltbDelay.TextRightOffsetX = 153;
			this.mniltbDelay.TextRightWidth = 27;
			this.mniltbDelay.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDelay_UserTyped);
			// 
			// mniRefresh
			// 
			this.mniRefresh.Name = "mniRefresh";
			this.mniRefresh.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.mniRefresh.Size = new System.Drawing.Size(265, 22);
			this.mniRefresh.Text = "Refresh";
			this.mniRefresh.Click += new System.EventHandler(this.mniRefresh_Click);
			// 
			// mniCopy
			// 
			this.mniCopy.Name = "mniCopy";
			this.mniCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopy.Size = new System.Drawing.Size(265, 22);
			this.mniCopy.Text = "Copy";
			this.mniCopy.Click += new System.EventHandler(this.mniCopy_Click);
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
			// ExceptionsControl
			// 
			this.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.Controls.Add(this.splitContainerVertical);
			this.Name = "ExceptionsControl";
			this.Size = new System.Drawing.Size(509, 361);
			this.Load += new System.EventHandler(this.form_OnLoad);
			this.VisibleChanged += new System.EventHandler(this.exceptionsControl_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.olvTreeExceptions)).EndInit();
			this.ctxTree.ResumeLayout(false);
			this.splitContainerVertical.Panel1.ResumeLayout(false);
			this.splitContainerVertical.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
			this.splitContainerVertical.ResumeLayout(false);
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

		private BrightIdeasSoftware.OLVColumn olvcTime;
		private System.Windows.Forms.ToolStripMenuItem mniCopyStackPosition;
		private System.Windows.Forms.ContextMenuStrip ctxCallStack;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbDelay;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniRecentAlwaysSelected;
		private System.Windows.Forms.ToolStripMenuItem mniCopy;
		private System.Windows.Forms.ToolStripMenuItem mniClear;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenuStrip ctxTree;
		private ToolStripMenuItem mniRefresh;
		private ToolStripMenuItem mniShowHeaders;
		private OLVColumn olvcException;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mniPopupOnIncomingException;
		
	}
}