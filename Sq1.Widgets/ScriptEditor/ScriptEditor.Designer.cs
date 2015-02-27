namespace Sq1.Widgets.ScriptEditor {
	partial class ScriptEditor {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
			this.TextEditorControl = new DigitalRune.Windows.TextEditor.TextEditorControl();
			this.ctxMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCompile = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRun = new System.Windows.Forms.ToolStripMenuItem();
			this.mniDebug = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniEditing = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxEditing = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCut = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mniPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.mniSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.txtCompilerErrors = new System.Windows.Forms.TextBox();
			this.ctxCompilerErrors = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniWordWrap = new System.Windows.Forms.ToolStripMenuItem();
			this.mniHide = new System.Windows.Forms.ToolStripMenuItem();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.ctxMain.SuspendLayout();
			this.ctxEditing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.ctxCompilerErrors.SuspendLayout();
			this.SuspendLayout();
			// 
			// TextEditorControl
			// 
			this.TextEditorControl.ContextMenuStrip = this.ctxMain;
			this.TextEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TextEditorControl.IsIconBarVisible = true;
			this.TextEditorControl.LineViewerStyle = DigitalRune.Windows.TextEditor.Properties.LineViewerStyle.FullRow;
			this.TextEditorControl.Location = new System.Drawing.Point(0, 0);
			this.TextEditorControl.Name = "TextEditorControl";
			this.TextEditorControl.ShowVRuler = false;
			this.TextEditorControl.Size = new System.Drawing.Size(401, 211);
			this.TextEditorControl.TabIndex = 0;
			this.TextEditorControl.Text = "textEditorControl1";
			this.TextEditorControl.CompletionRequest += new System.EventHandler<DigitalRune.Windows.TextEditor.Completion.CompletionEventArgs>(this.TextEditorControl_CompletionRequest);
			this.TextEditorControl.InsightRequest += new System.EventHandler<DigitalRune.Windows.TextEditor.Insight.InsightEventArgs>(this.TextEditorControl_InsightRequest);
			this.TextEditorControl.ToolTipRequest += new System.EventHandler<DigitalRune.Windows.TextEditor.ToolTipRequestEventArgs>(this.TextEditorControl_ToolTipRequest);
			this.TextEditorControl.TextAreaDialogKeyPress += new System.Windows.Forms.KeyEventHandler(this.TextEditorControl_TextAreaDialogKeyPress);
			this.TextEditorControl.TextAreaKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextEditorControl_TextAreaKeyPress);
			this.TextEditorControl.DocumentChanged += new System.EventHandler<DigitalRune.Windows.TextEditor.Document.DocumentEventArgs>(this.TextEditorControl_DocumentChanged);
			// 
			// ctxMain
			// 
			this.ctxMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniSave,
									this.mniCompile,
									this.mniRun,
									this.mniDebug,
									this.toolStripSeparator1,
									this.mniEditing,
									this.mniOptions});
			this.ctxMain.Name = "contextMenuStrip";
			this.ctxMain.Size = new System.Drawing.Size(139, 142);
			// 
			// mniSave
			// 
			this.mniSave.Name = "mniSave";
			this.mniSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mniSave.Size = new System.Drawing.Size(138, 22);
			this.mniSave.Text = "Save";
			this.mniSave.Click += new System.EventHandler(this.mniSave_Click);
			// 
			// mniCompile
			// 
			this.mniCompile.Name = "mniCompile";
			this.mniCompile.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.mniCompile.Size = new System.Drawing.Size(138, 22);
			this.mniCompile.Text = "Compile";
			this.mniCompile.Click += new System.EventHandler(this.mniCompile_Click);
			// 
			// mniRun
			// 
			this.mniRun.Name = "mniRun";
			this.mniRun.ShortcutKeys = System.Windows.Forms.Keys.F7;
			this.mniRun.Size = new System.Drawing.Size(138, 22);
			this.mniRun.Text = "Run";
			this.mniRun.Click += new System.EventHandler(this.mniRun_Click);
			// 
			// mniDebug
			// 
			this.mniDebug.Enabled = false;
			this.mniDebug.Name = "mniDebug";
			this.mniDebug.Size = new System.Drawing.Size(138, 22);
			this.mniDebug.Text = "Debug";
			this.mniDebug.Click += new System.EventHandler(this.mniDebug_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(135, 6);
			// 
			// mniEditing
			// 
			this.mniEditing.DropDown = this.ctxEditing;
			this.mniEditing.Name = "mniEditing";
			this.mniEditing.Size = new System.Drawing.Size(138, 22);
			this.mniEditing.Text = "Editing";
			// 
			// ctxEditing
			// 
			this.ctxEditing.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniUndo,
									this.mniRedo,
									this.toolStripSeparator5,
									this.mniCut,
									this.mniCopy,
									this.mniPaste,
									this.toolStripSeparator8,
									this.mniSelectAll});
			this.ctxEditing.Name = "ctxEditing";
			this.ctxEditing.OwnerItem = this.mniEditing;
			this.ctxEditing.Size = new System.Drawing.Size(123, 148);
			// 
			// mniUndo
			// 
			this.mniUndo.Name = "mniUndo";
			this.mniUndo.Size = new System.Drawing.Size(122, 22);
			this.mniUndo.Text = "Undo";
			// 
			// mniRedo
			// 
			this.mniRedo.Name = "mniRedo";
			this.mniRedo.Size = new System.Drawing.Size(122, 22);
			this.mniRedo.Text = "Redo";
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
			// 
			// mniCut
			// 
			this.mniCut.Image = ((System.Drawing.Image)(resources.GetObject("mniCut.Image")));
			this.mniCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniCut.Name = "mniCut";
			this.mniCut.Size = new System.Drawing.Size(122, 22);
			this.mniCut.Text = "Cut";
			// 
			// mniCopy
			// 
			this.mniCopy.Image = ((System.Drawing.Image)(resources.GetObject("mniCopy.Image")));
			this.mniCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniCopy.Name = "mniCopy";
			this.mniCopy.Size = new System.Drawing.Size(122, 22);
			this.mniCopy.Text = "Copy";
			// 
			// mniPaste
			// 
			this.mniPaste.Image = ((System.Drawing.Image)(resources.GetObject("mniPaste.Image")));
			this.mniPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniPaste.Name = "mniPaste";
			this.mniPaste.Size = new System.Drawing.Size(122, 22);
			this.mniPaste.Text = "Paste";
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(119, 6);
			// 
			// mniSelectAll
			// 
			this.mniSelectAll.Name = "mniSelectAll";
			this.mniSelectAll.Size = new System.Drawing.Size(122, 22);
			this.mniSelectAll.Text = "Select All";
			// 
			// mniOptions
			// 
			this.mniOptions.Name = "mniOptions";
			this.mniOptions.Size = new System.Drawing.Size(138, 22);
			this.mniOptions.Text = "Options";
			this.mniOptions.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.TextEditorControl);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtCompilerErrors);
			this.splitContainer1.Panel2MinSize = 12;
			this.splitContainer1.Size = new System.Drawing.Size(401, 240);
			this.splitContainer1.SplitterDistance = 211;
			//this.splitContainer1.SplitterIncrement = 12;
			this.splitContainer1.TabIndex = 2;
			// 
			// txtCompilerErrors
			// 
			this.txtCompilerErrors.BackColor = System.Drawing.SystemColors.Control;
			this.txtCompilerErrors.ContextMenuStrip = this.ctxCompilerErrors;
			this.txtCompilerErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtCompilerErrors.Location = new System.Drawing.Point(0, 0);
			this.txtCompilerErrors.Multiline = true;
			this.txtCompilerErrors.Name = "txtCompilerErrors";
			this.txtCompilerErrors.ReadOnly = true;
			this.txtCompilerErrors.Size = new System.Drawing.Size(401, 25);
			this.txtCompilerErrors.TabIndex = 0;
			this.txtCompilerErrors.WordWrap = false;
			this.txtCompilerErrors.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCompilerErrors_KeyUp);
			this.txtCompilerErrors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtCompilerErrors_MouseUp);
			// 
			// ctxCompilerErrors
			// 
			this.ctxCompilerErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniWordWrap,
									this.mniHide});
			this.ctxCompilerErrors.Name = "ctxCompilerErrors";
			this.ctxCompilerErrors.Size = new System.Drawing.Size(206, 48);
			// 
			// mniWordWrap
			// 
			this.mniWordWrap.Name = "mniWordWrap";
			this.mniWordWrap.Size = new System.Drawing.Size(205, 22);
			this.mniWordWrap.Text = "Wrap lines";
			this.mniWordWrap.Click += new System.EventHandler(this.mniWordWrap_Click);
			// 
			// mniHide
			// 
			this.mniHide.Name = "mniHide";
			this.mniHide.Size = new System.Drawing.Size(205, 22);
			this.mniHide.Text = "Hide Compiler Messages";
			this.mniHide.Click += new System.EventHandler(this.mniHide_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 5000;
			this.timer1.Tick += new System.EventHandler(this.TextEditorControl_UpdateFoldings);
			// 
			// ScriptEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ScriptEditor";
			this.Size = new System.Drawing.Size(401, 240);
			this.ctxMain.ResumeLayout(false);
			this.ctxEditing.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ctxCompilerErrors.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Timer timer1;

		#endregion

		public DigitalRune.Windows.TextEditor.TextEditorControl TextEditorControl;
		private System.Windows.Forms.ContextMenuStrip ctxMain;
		private System.Windows.Forms.ToolStripMenuItem mniOptions;
		private System.Windows.Forms.ToolStripMenuItem mniSave;
		private System.Windows.Forms.ToolStripMenuItem mniCompile;
		private System.Windows.Forms.ToolStripMenuItem mniRun;
		private System.Windows.Forms.ToolStripMenuItem mniDebug;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniEditing;
		private System.Windows.Forms.ContextMenuStrip ctxEditing;
		private System.Windows.Forms.ToolStripMenuItem mniUndo;
		private System.Windows.Forms.ToolStripMenuItem mniRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem mniCut;
		private System.Windows.Forms.ToolStripMenuItem mniCopy;
		private System.Windows.Forms.ToolStripMenuItem mniPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem mniSelectAll;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox txtCompilerErrors;
		private System.Windows.Forms.ContextMenuStrip ctxCompilerErrors;
		private System.Windows.Forms.ToolStripMenuItem mniWordWrap;
		private System.Windows.Forms.ToolStripMenuItem mniHide;
	}
}
