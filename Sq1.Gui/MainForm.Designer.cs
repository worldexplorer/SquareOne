namespace Sq1.Gui {
	partial class MainForm {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin3 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
			WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin3 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient7 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient15 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin3 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient16 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient8 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient17 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient18 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient19 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient9 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient20 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient21 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			this.ctxTools = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCsvImporter = new System.Windows.Forms.ToolStripMenuItem();
			this.mniExecution = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSliders = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStrategies = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSymbols = new System.Windows.Forms.ToolStripMenuItem();
			this.mniExceptions = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniExit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMainMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			this.mainFormStatusStrip = new System.Windows.Forms.StatusStrip();
			this.btnWindows = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxWindows = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.btnWorkSpaces = new System.Windows.Forms.ToolStripDropDownButton();
			this.CtxWorkspaces = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniWorkspaceDeleteCurrent = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbWorklspaceNewBlank = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbWorklspaceCloneTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbWorklspaceRenameTo = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnFullScreen = new System.Windows.Forms.ToolStripButton();
			this.lblSpace = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ctxTools.SuspendLayout();
			this.mainFormStatusStrip.SuspendLayout();
			this.CtxWorkspaces.SuspendLayout();
			this.SuspendLayout();
			// 
			// ctxTools
			// 
			this.ctxTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCsvImporter,
            this.mniExecution,
            this.mniSliders,
            this.mniStrategies,
            this.mniSymbols,
            this.mniExceptions,
            this.toolStripSeparator1,
            this.mniExit});
			this.ctxTools.Name = "ctxmsTools";
			this.ctxTools.OwnerItem = this.mniMainMenu;
			this.ctxTools.Size = new System.Drawing.Size(186, 164);
			// 
			// mniCsvImporter
			// 
			this.mniCsvImporter.Name = "mniCsvImporter";
			this.mniCsvImporter.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.mniCsvImporter.Size = new System.Drawing.Size(185, 22);
			this.mniCsvImporter.Text = "CSV Importer";
			this.mniCsvImporter.Click += new System.EventHandler(this.mniCsvImporter_Click);
			// 
			// mniExecution
			// 
			this.mniExecution.Name = "mniExecution";
			this.mniExecution.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mniExecution.Size = new System.Drawing.Size(185, 22);
			this.mniExecution.Text = "Order Execution";
			this.mniExecution.Click += new System.EventHandler(this.mniExecution_Click);
			// 
			// mniSliders
			// 
			this.mniSliders.Name = "mniSliders";
			this.mniSliders.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.mniSliders.Size = new System.Drawing.Size(185, 22);
			this.mniSliders.Text = "Script Parameters";
			this.mniSliders.Click += new System.EventHandler(this.mniSliders_Click);
			// 
			// mniStrategies
			// 
			this.mniStrategies.Name = "mniStrategies";
			this.mniStrategies.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.mniStrategies.Size = new System.Drawing.Size(185, 22);
			this.mniStrategies.Text = "Strategies";
			this.mniStrategies.Click += new System.EventHandler(this.mniStrategies_Click);
			// 
			// mniSymbols
			// 
			this.mniSymbols.Name = "mniSymbols";
			this.mniSymbols.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mniSymbols.Size = new System.Drawing.Size(185, 22);
			this.mniSymbols.Text = "Symbols";
			this.mniSymbols.Click += new System.EventHandler(this.mniSymbols_Click);
			// 
			// mniExceptions
			// 
			this.mniExceptions.Name = "mniExceptions";
			this.mniExceptions.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.mniExceptions.Size = new System.Drawing.Size(185, 22);
			this.mniExceptions.Text = "Exceptions";
			this.mniExceptions.Click += new System.EventHandler(this.mniExceptions_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
			// 
			// mniExit
			// 
			this.mniExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniExit.Name = "mniExit";
			this.mniExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.mniExit.Size = new System.Drawing.Size(185, 22);
			this.mniExit.Text = "Exit";
			this.mniExit.Click += new System.EventHandler(this.mniExit_Click);
			// 
			// mniMainMenu
			// 
			this.mniMainMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniMainMenu.DropDown = this.ctxTools;
			this.mniMainMenu.Image = ((System.Drawing.Image)(resources.GetObject("mniMainMenu.Image")));
			this.mniMainMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniMainMenu.Name = "mniMainMenu";
			this.mniMainMenu.Size = new System.Drawing.Size(49, 20);
			this.mniMainMenu.Text = "Tools";
			// 
			// DockPanel
			// 
			this.DockPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DockPanel.DockBottomPortion = 0.4D;
			this.DockPanel.DockLeftPortion = 0.15D;
			this.DockPanel.DockRightPortion = 0.35D;
			this.DockPanel.DockTopPortion = 0.3D;
			this.DockPanel.DocumentTabStripLocation = WeifenLuo.WinFormsUI.Docking.DocumentTabStripLocation.Bottom;
			this.DockPanel.Location = new System.Drawing.Point(0, 0);
			this.DockPanel.Name = "DockPanel";
			this.DockPanel.Size = new System.Drawing.Size(774, 423);
			dockPanelGradient7.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient7.StartColor = System.Drawing.SystemColors.ControlLight;
			autoHideStripSkin3.DockStripGradient = dockPanelGradient7;
			tabGradient15.EndColor = System.Drawing.SystemColors.Control;
			tabGradient15.StartColor = System.Drawing.SystemColors.Control;
			tabGradient15.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			autoHideStripSkin3.TabGradient = tabGradient15;
			autoHideStripSkin3.TextFont = new System.Drawing.Font("Segoe UI", 9F);
			dockPanelSkin3.AutoHideStripSkin = autoHideStripSkin3;
			tabGradient16.EndColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient16.StartColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient16.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient3.ActiveTabGradient = tabGradient16;
			dockPanelGradient8.EndColor = System.Drawing.SystemColors.Control;
			dockPanelGradient8.StartColor = System.Drawing.SystemColors.Control;
			dockPaneStripGradient3.DockStripGradient = dockPanelGradient8;
			tabGradient17.EndColor = System.Drawing.SystemColors.ControlLight;
			tabGradient17.StartColor = System.Drawing.SystemColors.ControlLight;
			tabGradient17.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient3.InactiveTabGradient = tabGradient17;
			dockPaneStripSkin3.DocumentGradient = dockPaneStripGradient3;
			dockPaneStripSkin3.TextFont = new System.Drawing.Font("Segoe UI", 9F);
			tabGradient18.EndColor = System.Drawing.SystemColors.ActiveCaption;
			tabGradient18.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient18.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
			tabGradient18.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
			dockPaneStripToolWindowGradient3.ActiveCaptionGradient = tabGradient18;
			tabGradient19.EndColor = System.Drawing.SystemColors.Control;
			tabGradient19.StartColor = System.Drawing.SystemColors.Control;
			tabGradient19.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripToolWindowGradient3.ActiveTabGradient = tabGradient19;
			dockPanelGradient9.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient9.StartColor = System.Drawing.SystemColors.ControlLight;
			dockPaneStripToolWindowGradient3.DockStripGradient = dockPanelGradient9;
			tabGradient20.EndColor = System.Drawing.SystemColors.InactiveCaption;
			tabGradient20.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient20.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
			tabGradient20.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
			dockPaneStripToolWindowGradient3.InactiveCaptionGradient = tabGradient20;
			tabGradient21.EndColor = System.Drawing.Color.Transparent;
			tabGradient21.StartColor = System.Drawing.Color.Transparent;
			tabGradient21.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			dockPaneStripToolWindowGradient3.InactiveTabGradient = tabGradient21;
			dockPaneStripSkin3.ToolWindowGradient = dockPaneStripToolWindowGradient3;
			dockPanelSkin3.DockPaneStripSkin = dockPaneStripSkin3;
			this.DockPanel.Skin = dockPanelSkin3;
			this.DockPanel.TabIndex = 2;
			// 
			// mainFormStatusStrip
			// 
			this.mainFormStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniMainMenu,
            this.btnWindows,
            this.btnWorkSpaces,
            this.btnFullScreen,
            this.lblSpace,
            this.lblStatus});
			this.mainFormStatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.mainFormStatusStrip.Location = new System.Drawing.Point(0, 401);
			this.mainFormStatusStrip.Name = "mainFormStatusStrip";
			this.mainFormStatusStrip.Size = new System.Drawing.Size(774, 22);
			this.mainFormStatusStrip.TabIndex = 5;
			this.mainFormStatusStrip.Text = "mainFormStatusStrip";
			// 
			// btnWindows
			// 
			this.btnWindows.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnWindows.DropDown = this.ctxWindows;
			this.btnWindows.Image = ((System.Drawing.Image)(resources.GetObject("btnWindows.Image")));
			this.btnWindows.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnWindows.Name = "btnWindows";
			this.btnWindows.Size = new System.Drawing.Size(69, 20);
			this.btnWindows.Text = "Windows";
			// 
			// ctxWindows
			// 
			this.ctxWindows.Name = "ctxWindows";
			this.ctxWindows.OwnerItem = this.btnWindows;
			this.ctxWindows.ShowImageMargin = false;
			this.ctxWindows.Size = new System.Drawing.Size(36, 4);
			this.ctxWindows.Opening += new System.ComponentModel.CancelEventHandler(this.ctxWindowsOpening);
			// 
			// btnWorkSpaces
			// 
			this.btnWorkSpaces.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnWorkSpaces.DropDown = this.CtxWorkspaces;
			this.btnWorkSpaces.Image = ((System.Drawing.Image)(resources.GetObject("btnWorkSpaces.Image")));
			this.btnWorkSpaces.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnWorkSpaces.Name = "btnWorkSpaces";
			this.btnWorkSpaces.Size = new System.Drawing.Size(84, 20);
			this.btnWorkSpaces.Text = "WorkSpaces";
			// 
			// CtxWorkspaces
			// 
			this.CtxWorkspaces.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniWorkspaceDeleteCurrent,
            this.mniltbWorklspaceNewBlank,
            this.mniltbWorklspaceCloneTo,
            this.mniltbWorklspaceRenameTo,
            this.toolStripSeparator2});
			this.CtxWorkspaces.Name = "ctxWorkspaces";
			this.CtxWorkspaces.OwnerItem = this.btnWorkSpaces;
			this.CtxWorkspaces.Size = new System.Drawing.Size(223, 101);
			// 
			// mniWorkspaceDeleteCurrent
			// 
			this.mniWorkspaceDeleteCurrent.Name = "mniWorkspaceDeleteCurrent";
			this.mniWorkspaceDeleteCurrent.Size = new System.Drawing.Size(222, 22);
			this.mniWorkspaceDeleteCurrent.Text = "Delete [TO_BE_REPLACED]";
			// 
			// mniltbWorklspaceNewBlank
			// 
			this.mniltbWorklspaceNewBlank.BackColor = System.Drawing.Color.Transparent;
			this.mniltbWorklspaceNewBlank.InputFieldAlignedRight = false;
			this.mniltbWorklspaceNewBlank.InputFieldEditable = true;
			this.mniltbWorklspaceNewBlank.InputFieldOffsetX = 80;
			this.mniltbWorklspaceNewBlank.InputFieldValue = "";
			this.mniltbWorklspaceNewBlank.InputFieldWidth = 79;
			this.mniltbWorklspaceNewBlank.Name = "mniltbWorklspaceNewBlank";
			this.mniltbWorklspaceNewBlank.Size = new System.Drawing.Size(162, 20);
			this.mniltbWorklspaceNewBlank.Text = "New Blank";
			this.mniltbWorklspaceNewBlank.TextOffsetX = 0;
			this.mniltbWorklspaceNewBlank.TextRed = false;
			this.mniltbWorklspaceNewBlank.TextWidth = 65;
			// 
			// mniltbWorklspaceCloneTo
			// 
			this.mniltbWorklspaceCloneTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbWorklspaceCloneTo.InputFieldAlignedRight = false;
			this.mniltbWorklspaceCloneTo.InputFieldEditable = true;
			this.mniltbWorklspaceCloneTo.InputFieldOffsetX = 80;
			this.mniltbWorklspaceCloneTo.InputFieldValue = "";
			this.mniltbWorklspaceCloneTo.InputFieldWidth = 79;
			this.mniltbWorklspaceCloneTo.Name = "mniltbWorklspaceCloneTo";
			this.mniltbWorklspaceCloneTo.Size = new System.Drawing.Size(162, 20);
			this.mniltbWorklspaceCloneTo.Text = "Clone To";
			this.mniltbWorklspaceCloneTo.TextOffsetX = 0;
			this.mniltbWorklspaceCloneTo.TextRed = false;
			this.mniltbWorklspaceCloneTo.TextWidth = 57;
			// 
			// mniltbWorklspaceRenameTo
			// 
			this.mniltbWorklspaceRenameTo.BackColor = System.Drawing.Color.Transparent;
			this.mniltbWorklspaceRenameTo.InputFieldAlignedRight = false;
			this.mniltbWorklspaceRenameTo.InputFieldEditable = true;
			this.mniltbWorklspaceRenameTo.InputFieldOffsetX = 80;
			this.mniltbWorklspaceRenameTo.InputFieldValue = "";
			this.mniltbWorklspaceRenameTo.InputFieldWidth = 79;
			this.mniltbWorklspaceRenameTo.Name = "mniltbWorklspaceRenameTo";
			this.mniltbWorklspaceRenameTo.Size = new System.Drawing.Size(162, 20);
			this.mniltbWorklspaceRenameTo.Text = "RenameTo";
			this.mniltbWorklspaceRenameTo.TextOffsetX = 0;
			this.mniltbWorklspaceRenameTo.TextRed = false;
			this.mniltbWorklspaceRenameTo.TextWidth = 66;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(219, 6);
			// 
			// btnFullScreen
			// 
			this.btnFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnFullScreen.Name = "btnFullScreen";
			this.btnFullScreen.Size = new System.Drawing.Size(65, 20);
			this.btnFullScreen.Text = "FullScreen";
			this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
			// 
			// lblSpace
			// 
			this.lblSpace.Name = "lblSpace";
			this.lblSpace.Size = new System.Drawing.Size(28, 17);
			this.lblSpace.Text = "   |   ";
			// 
			// lblStatus2
			// 
			this.lblStatus.Name = "lblStatus2";
			this.lblStatus.Size = new System.Drawing.Size(148, 17);
			this.lblStatus.Text = "I am stretching to the right";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(774, 423);
			this.Controls.Add(this.mainFormStatusStrip);
			this.Controls.Add(this.DockPanel);
			this.IsMdiContainer = true;
			this.Name = "MainForm";
			this.Text = "SquareOne";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
			this.Load += new System.EventHandler(this.mainForm_Load);
			this.ResizeEnd += new System.EventHandler(this.mainForm_ResizeEnd);
			this.LocationChanged += new System.EventHandler(this.mainForm_LocationChanged);
			this.ctxTools.ResumeLayout(false);
			this.mainFormStatusStrip.ResumeLayout(false);
			this.mainFormStatusStrip.PerformLayout();
			this.CtxWorkspaces.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem mniCsvImporter;
		private System.Windows.Forms.ContextMenuStrip ctxWindows;
		private System.Windows.Forms.ToolStripDropDownButton btnWindows;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbWorklspaceRenameTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbWorklspaceCloneTo;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbWorklspaceNewBlank;
		private System.Windows.Forms.ToolStripMenuItem mniWorkspaceDeleteCurrent;
		public System.Windows.Forms.ContextMenuStrip CtxWorkspaces;

		private System.Windows.Forms.ContextMenuStrip ctxTools;
		private System.Windows.Forms.ToolStripMenuItem mniExceptions;
		private System.Windows.Forms.ToolStripMenuItem mniExecution;
		private System.Windows.Forms.ToolStripMenuItem mniSliders;
		private System.Windows.Forms.ToolStripMenuItem mniSymbols;
		public WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
		private System.Windows.Forms.ToolStripMenuItem mniStrategies;
		private System.Windows.Forms.StatusStrip mainFormStatusStrip;
		private System.Windows.Forms.ToolStripDropDownButton mniMainMenu;
		private System.Windows.Forms.ToolStripStatusLabel lblSpace;
		private System.Windows.Forms.ToolStripButton btnFullScreen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniExit;
		private System.Windows.Forms.ToolStripDropDownButton btnWorkSpaces;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
	}
}
