using System;
using System.Windows.Forms;

using BrightIdeasSoftware;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl : UserControl {
		#region Windows Form Designer generated code

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfgh"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dfghdfg"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Occured")]
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.treeExceptions = new BrightIdeasSoftware.TreeListView();
			this.olvTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniRecentAlwaysSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniTreeShowExceptionTime = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbDelay = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mniClear = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
			this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtExceptionMessage = new System.Windows.Forms.TextBox();
			this.lvStackTrace = new System.Windows.Forms.ListView();
			this.lvhMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhDeclaringClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvhFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ctxCallStack = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCopyStackPosition = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.treeExceptions)).BeginInit();
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
			// treeExceptions
			// 
			this.treeExceptions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeExceptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.olvTime});
			this.treeExceptions.ContextMenuStrip = this.ctxTree;
			this.treeExceptions.CopySelectionOnControlC = false;
			this.treeExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeExceptions.EmptyListMsg = "";
			this.treeExceptions.FullRowSelect = true;
			this.treeExceptions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.treeExceptions.HeaderUsesThemes = false;
			this.treeExceptions.HideSelection = false;
			this.treeExceptions.IncludeColumnHeadersInCopy = true;
			this.treeExceptions.IncludeHiddenColumnsInDataTransfer = true;
			this.treeExceptions.Location = new System.Drawing.Point(0, 0);
			this.treeExceptions.Name = "treeExceptions";
			this.treeExceptions.OwnerDraw = true;
			this.treeExceptions.ShowCommandMenuOnRightClick = true;
			this.treeExceptions.ShowGroups = false;
			this.treeExceptions.ShowItemToolTips = true;
			this.treeExceptions.Size = new System.Drawing.Size(111, 353);
			this.treeExceptions.TabIndex = 3;
			this.treeExceptions.TintSortColumn = true;
			this.treeExceptions.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.treeExceptions.UseCompatibleStateImageBehavior = false;
			this.treeExceptions.UseFilterIndicator = true;
			this.treeExceptions.UseFiltering = true;
			this.treeExceptions.UseHotItem = true;
			this.treeExceptions.UseTranslucentHotItem = true;
			this.treeExceptions.View = System.Windows.Forms.View.Details;
			this.treeExceptions.VirtualMode = true;
			this.treeExceptions.SelectedIndexChanged += new System.EventHandler(this.tree_SelectedIndexChanged);
			// 
			// olvTime
			// 
			this.olvTime.CellPadding = null;
			this.olvTime.FillsFreeSpace = true;
			this.olvTime.Text = "Time";
			// 
			// ctxTree
			// 
			this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.mniRecentAlwaysSelected,
			this.mniTreeShowExceptionTime,
			this.mniltbDelay,
			this.toolStripSeparator1,
			this.mniCopy,
			this.mniClear});
			this.ctxTree.Name = "ctx";
			this.ctxTree.Size = new System.Drawing.Size(255, 122);
			// 
			// mniRecentAlwaysSelected
			// 
			this.mniRecentAlwaysSelected.CheckOnClick = true;
			this.mniRecentAlwaysSelected.Name = "mniRecentAlwaysSelected";
			this.mniRecentAlwaysSelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRecentAlwaysSelected.Size = new System.Drawing.Size(254, 22);
			this.mniRecentAlwaysSelected.Text = "Recent Always Selected";
			this.mniRecentAlwaysSelected.Click += new System.EventHandler(this.mniRecentAlwaysSelected_Click);
			// 
			// mniTreeShowsTimesInsteadOfMessages
			// 
			this.mniTreeShowExceptionTime.CheckOnClick = true;
			this.mniTreeShowExceptionTime.Name = "mniTreeShowExceptionTime";
			this.mniTreeShowExceptionTime.Size = new System.Drawing.Size(254, 22);
			this.mniTreeShowExceptionTime.Text = "Show Timestamps";
			this.mniTreeShowExceptionTime.Click += new System.EventHandler(this.mniTreeShowsTimesInsteadOfMessages_Click);
			// 
			// mniltbDelay
			// 
			this.mniltbDelay.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDelay.Enabled = false;
			this.mniltbDelay.InputFieldOffsetX = 80;
			this.mniltbDelay.InputFieldValue = "";
			this.mniltbDelay.InputFieldWidth = 0;
			this.mniltbDelay.Name = "mniltbDelay";
			this.mniltbDelay.Size = new System.Drawing.Size(165, 21);
			this.mniltbDelay.Text = "Delay, msec:";
			this.mniltbDelay.TextRed = false;
			this.mniltbDelay.Visible = false;
			this.mniltbDelay.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDelay_UserTyped);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(251, 6);
			// 
			// mniCopy
			// 
			this.mniCopy.Name = "mniCopy";
			this.mniCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopy.Size = new System.Drawing.Size(254, 22);
			this.mniCopy.Text = "Copy";
			this.mniCopy.Click += new System.EventHandler(this.mniCopy_Click);
			// 
			// mniClear
			// 
			this.mniClear.Name = "mniClear";
			this.mniClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniClear.Size = new System.Drawing.Size(254, 22);
			this.mniClear.Text = "Clear";
			this.mniClear.Click += new System.EventHandler(this.mniClear_Click);
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
			this.splitContainerVertical.Panel1.Controls.Add(this.treeExceptions);
			this.splitContainerVertical.Panel1MinSize = 40;
			// 
			// splitContainerVertical.Panel2
			// 
			this.splitContainerVertical.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerVertical.Panel2.Controls.Add(this.splitContainerHorizontal);
			this.splitContainerVertical.Size = new System.Drawing.Size(506, 353);
			this.splitContainerVertical.SplitterDistance = 111;
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
			this.splitContainerHorizontal.Panel2.Controls.Add(this.lvStackTrace);
			this.splitContainerHorizontal.Size = new System.Drawing.Size(390, 353);
			this.splitContainerHorizontal.SplitterDistance = 148;
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
			this.txtExceptionMessage.Size = new System.Drawing.Size(390, 148);
			this.txtExceptionMessage.TabIndex = 1;
			this.txtExceptionMessage.TabStop = false;
			this.txtExceptionMessage.Text = "ghdfg hdfg\r\ndfghdfg hdfgh\r\n9999";
			// 
			// lvStackTrace
			// 
			this.lvStackTrace.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.lvStackTrace.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lvStackTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.lvhMethod,
			this.lvhDeclaringClass,
			this.lvhLine,
			this.lvhFile});
			this.lvStackTrace.ContextMenuStrip = this.ctxCallStack;
			this.lvStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvStackTrace.FullRowSelect = true;
			this.lvStackTrace.GridLines = true;
			this.lvStackTrace.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvStackTrace.Location = new System.Drawing.Point(0, 0);
			this.lvStackTrace.Name = "lvStackTrace";
			this.lvStackTrace.Scrollable = false;
			this.lvStackTrace.ShowItemToolTips = true;
			this.lvStackTrace.Size = new System.Drawing.Size(390, 200);
			this.lvStackTrace.TabIndex = 12;
			this.lvStackTrace.UseCompatibleStateImageBehavior = false;
			this.lvStackTrace.View = System.Windows.Forms.View.Details;
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
			this.Size = new System.Drawing.Size(506, 353);
			this.Load += new System.EventHandler(this.form_OnLoad);
			((System.ComponentModel.ISupportInitialize)(this.treeExceptions)).EndInit();
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
		private System.Windows.Forms.ToolStripMenuItem mniTreeShowExceptionTime;
		private TreeListView treeExceptions;
		private TextBox txtExceptionMessage;
		private ListView lvStackTrace;
		private ColumnHeader lvhFile;
		private ColumnHeader lvhLine;
		private ColumnHeader lvhMethod;
		private ColumnHeader lvhDeclaringClass;
		private SplitContainer splitContainerVertical;
		private SplitContainer splitContainerHorizontal;

		private BrightIdeasSoftware.OLVColumn olvTime;
		private System.Windows.Forms.ToolStripMenuItem mniCopyStackPosition;
		private System.Windows.Forms.ContextMenuStrip ctxCallStack;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbDelay;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniRecentAlwaysSelected;
		private System.Windows.Forms.ToolStripMenuItem mniCopy;
		private System.Windows.Forms.ToolStripMenuItem mniClear;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenuStrip ctxTree;
		#endregion
		
	}
}