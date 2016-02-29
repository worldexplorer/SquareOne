using System;

using Sq1.Widgets.DataSourcesTree;

namespace Sq1.Widgets.CsvImporter {
	partial class CsvImporterControl 	{
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lnkDownload = new System.Windows.Forms.LinkLabel();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.grpPreviewParsedRaw = new System.Windows.Forms.GroupBox();
			this.olvCsvParsedRaw = new BrightIdeasSoftware.FastObjectListView();
			this.columnHeader1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxRaw = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.grpPreviewParsedByFormat = new System.Windows.Forms.GroupBox();
			this.olvParsedByFormat = new BrightIdeasSoftware.FastObjectListView();
			this.olvcUnalignable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOpen = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHigh = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLow = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcClose = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcVolume = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.grpImportSettings = new System.Windows.Forms.GroupBox();
			this.olvFieldSetup = new BrightIdeasSoftware.FastObjectListView();
			this.columnHeader5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.columnHeader8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.btnImport = new System.Windows.Forms.Button();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.importSourceFileBrowser = new Sq1.Widgets.CsvImporter.ImportSourceFileBrowser();
			this.mniltbCsvSeparator = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.rangeBar = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			this.dataSourcesTree = new Sq1.Widgets.DataSourcesTree.DataSourcesTreeControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.grpPreviewParsedRaw.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvCsvParsedRaw)).BeginInit();
			this.ctxRaw.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.grpPreviewParsedByFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvParsedByFormat)).BeginInit();
			this.grpImportSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvFieldSetup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lnkDownload);
			this.splitContainer1.Panel1.Controls.Add(this.importSourceFileBrowser);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(699, 462);
			this.splitContainer1.SplitterDistance = 136;
			this.splitContainer1.TabIndex = 0;
			// 
			// lnkDownload
			// 
			this.lnkDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lnkDownload.BackColor = System.Drawing.SystemColors.ControlLight;
			this.lnkDownload.Location = new System.Drawing.Point(0, 441);
			this.lnkDownload.Name = "lnkDownload";
			this.lnkDownload.Size = new System.Drawing.Size(136, 21);
			this.lnkDownload.TabIndex = 1;
			this.lnkDownload.TabStop = true;
			this.lnkDownload.Text = "Download";
			this.lnkDownload.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lnkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDownload_LinkClicked);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel1.Controls.Add(this.grpPreviewParsedRaw);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer5);
			this.splitContainer2.Size = new System.Drawing.Size(559, 462);
			this.splitContainer2.SplitterDistance = 130;
			this.splitContainer2.TabIndex = 26;
			// 
			// grpPreviewParsedRaw
			// 
			this.grpPreviewParsedRaw.BackColor = System.Drawing.SystemColors.Control;
			this.grpPreviewParsedRaw.Controls.Add(this.olvCsvParsedRaw);
			this.grpPreviewParsedRaw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpPreviewParsedRaw.Location = new System.Drawing.Point(0, 0);
			this.grpPreviewParsedRaw.Name = "grpPreviewParsedRaw";
			this.grpPreviewParsedRaw.Size = new System.Drawing.Size(559, 130);
			this.grpPreviewParsedRaw.TabIndex = 25;
			this.grpPreviewParsedRaw.TabStop = false;
			this.grpPreviewParsedRaw.Text = "Raw CSV | Select File in Filesystem in Left Panel";
			// 
			// olvCsvParsedRaw
			// 
			this.olvCsvParsedRaw.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvCsvParsedRaw.AllColumns.Add(this.columnHeader1);
			this.olvCsvParsedRaw.AllColumns.Add(this.columnHeader2);
			this.olvCsvParsedRaw.AllColumns.Add(this.columnHeader3);
			this.olvCsvParsedRaw.AllColumns.Add(this.columnHeader4);
			this.olvCsvParsedRaw.AllowColumnReorder = true;
			this.olvCsvParsedRaw.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvCsvParsedRaw.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvCsvParsedRaw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.olvCsvParsedRaw.ContextMenuStrip = this.ctxRaw;
			this.olvCsvParsedRaw.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvCsvParsedRaw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvCsvParsedRaw.EmptyListMsgFont = new System.Drawing.Font("Consolas", 9.75F);
			this.olvCsvParsedRaw.HasCollapsibleGroups = false;
			this.olvCsvParsedRaw.IncludeColumnHeadersInCopy = true;
			this.olvCsvParsedRaw.Location = new System.Drawing.Point(3, 16);
			this.olvCsvParsedRaw.Name = "olvCsvParsedRaw";
			this.olvCsvParsedRaw.ShowCommandMenuOnRightClick = true;
			this.olvCsvParsedRaw.ShowGroups = false;
			this.olvCsvParsedRaw.Size = new System.Drawing.Size(553, 111);
			this.olvCsvParsedRaw.TabIndex = 0;
			this.olvCsvParsedRaw.TintSortColumn = true;
			this.olvCsvParsedRaw.UseCompatibleStateImageBehavior = false;
			this.olvCsvParsedRaw.UseExplorerTheme = true;
			this.olvCsvParsedRaw.UseFilterIndicator = true;
			this.olvCsvParsedRaw.UseFiltering = true;
			this.olvCsvParsedRaw.UseHotItem = true;
			this.olvCsvParsedRaw.UseTranslucentHotItem = true;
			this.olvCsvParsedRaw.UseTranslucentSelection = true;
			this.olvCsvParsedRaw.View = System.Windows.Forms.View.Details;
			this.olvCsvParsedRaw.VirtualMode = true;
			this.olvCsvParsedRaw.DoubleClick += new System.EventHandler(this.csvPreviewParsedRaw_DoubleClick);
			// 
			// ctxRaw
			// 
			this.ctxRaw.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniltbCsvSeparator,
            this.mniEdit});
			this.ctxRaw.Name = "ctxRaw";
			this.ctxRaw.Size = new System.Drawing.Size(181, 50);
			// 
			// mniEdit
			// 
			this.mniEdit.Name = "mniEdit";
			this.mniEdit.Size = new System.Drawing.Size(180, 22);
			this.mniEdit.Text = "Edit";
			this.mniEdit.Click += new System.EventHandler(this.mniEdit_Click);
			// 
			// splitContainer5
			// 
			this.splitContainer5.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer5.Panel1.Controls.Add(this.grpPreviewParsedByFormat);
			this.splitContainer5.Panel1.Controls.Add(this.grpImportSettings);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer5.Panel2.Controls.Add(this.rangeBar);
			this.splitContainer5.Panel2.Controls.Add(this.btnImport);
			this.splitContainer5.Size = new System.Drawing.Size(559, 328);
			this.splitContainer5.SplitterDistance = 247;
			this.splitContainer5.TabIndex = 11;
			// 
			// grpPreviewParsedByFormat
			// 
			this.grpPreviewParsedByFormat.BackColor = System.Drawing.SystemColors.Control;
			this.grpPreviewParsedByFormat.Controls.Add(this.olvParsedByFormat);
			this.grpPreviewParsedByFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpPreviewParsedByFormat.Location = new System.Drawing.Point(0, 63);
			this.grpPreviewParsedByFormat.Name = "grpPreviewParsedByFormat";
			this.grpPreviewParsedByFormat.Size = new System.Drawing.Size(559, 184);
			this.grpPreviewParsedByFormat.TabIndex = 30;
			this.grpPreviewParsedByFormat.TabStop = false;
			this.grpPreviewParsedByFormat.Text = "Parsed By Format | Symbol[]  ScaleInerval[] BarsParsed.Count[]";
			// 
			// olvParsedByFormat
			// 
			this.olvParsedByFormat.AllColumns.Add(this.olvcUnalignable);
			this.olvParsedByFormat.AllColumns.Add(this.olvcSerno);
			this.olvParsedByFormat.AllColumns.Add(this.olvcDateTime);
			this.olvParsedByFormat.AllColumns.Add(this.olvcOpen);
			this.olvParsedByFormat.AllColumns.Add(this.olvcHigh);
			this.olvParsedByFormat.AllColumns.Add(this.olvcLow);
			this.olvParsedByFormat.AllColumns.Add(this.olvcClose);
			this.olvParsedByFormat.AllColumns.Add(this.olvcVolume);
			this.olvParsedByFormat.AllowColumnReorder = true;
			this.olvParsedByFormat.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvParsedByFormat.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvParsedByFormat.CellEditEnterChangesRows = true;
			this.olvParsedByFormat.CellEditTabChangesRows = true;
			this.olvParsedByFormat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcUnalignable,
            this.olvcSerno,
            this.olvcDateTime,
            this.olvcOpen,
            this.olvcHigh,
            this.olvcLow,
            this.olvcClose,
            this.olvcVolume});
			this.olvParsedByFormat.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvParsedByFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvParsedByFormat.EmptyListMsgFont = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvParsedByFormat.HasCollapsibleGroups = false;
			this.olvParsedByFormat.Location = new System.Drawing.Point(3, 16);
			this.olvParsedByFormat.Name = "olvParsedByFormat";
			this.olvParsedByFormat.ShowCommandMenuOnRightClick = true;
			this.olvParsedByFormat.ShowItemCountOnGroups = true;
			this.olvParsedByFormat.Size = new System.Drawing.Size(553, 165);
			this.olvParsedByFormat.TabIndex = 10;
			this.olvParsedByFormat.TintSortColumn = true;
			this.olvParsedByFormat.UseCompatibleStateImageBehavior = false;
			this.olvParsedByFormat.UseExplorerTheme = true;
			this.olvParsedByFormat.UseFilterIndicator = true;
			this.olvParsedByFormat.UseFiltering = true;
			this.olvParsedByFormat.UseHotItem = true;
			this.olvParsedByFormat.UseTranslucentHotItem = true;
			this.olvParsedByFormat.UseTranslucentSelection = true;
			this.olvParsedByFormat.View = System.Windows.Forms.View.Details;
			this.olvParsedByFormat.VirtualMode = true;
			// 
			// olvcUnalignable
			// 
			this.olvcUnalignable.Text = "unalignable";
			this.olvcUnalignable.Width = 1;
			// 
			// olvcSerno
			// 
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Text = "#";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Width = 45;
			// 
			// olvcDateTime
			// 
			this.olvcDateTime.AspectToStringFormat = "{0:yyyy-MMM-dd HH:mm:ss}";
			this.olvcDateTime.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcDateTime.Text = "DateTime";
			this.olvcDateTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcDateTime.Width = 100;
			// 
			// olvcOpen
			// 
			this.olvcOpen.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcOpen.Text = "Open";
			this.olvcOpen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcOpen.Width = 75;
			// 
			// olvcHigh
			// 
			this.olvcHigh.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcHigh.Text = "High";
			this.olvcHigh.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcHigh.Width = 75;
			// 
			// olvcLow
			// 
			this.olvcLow.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcLow.Text = "Low";
			this.olvcLow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcLow.Width = 75;
			// 
			// olvcClose
			// 
			this.olvcClose.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcClose.Text = "Close";
			this.olvcClose.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcClose.Width = 75;
			// 
			// olvcVolume
			// 
			this.olvcVolume.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcVolume.Text = "Volume";
			this.olvcVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// grpImportSettings
			// 
			this.grpImportSettings.BackColor = System.Drawing.SystemColors.Control;
			this.grpImportSettings.Controls.Add(this.olvFieldSetup);
			this.grpImportSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpImportSettings.Location = new System.Drawing.Point(0, 0);
			this.grpImportSettings.Name = "grpImportSettings";
			this.grpImportSettings.Size = new System.Drawing.Size(559, 63);
			this.grpImportSettings.TabIndex = 28;
			this.grpImportSettings.TabStop = false;
			this.grpImportSettings.Text = "CSV Columns => DateTime, Open, High, Low, Close, Volume settings";
			// 
			// olvFieldSetup
			// 
			this.olvFieldSetup.AllColumns.Add(this.columnHeader5);
			this.olvFieldSetup.AllColumns.Add(this.columnHeader6);
			this.olvFieldSetup.AllColumns.Add(this.columnHeader7);
			this.olvFieldSetup.AllColumns.Add(this.columnHeader8);
			this.olvFieldSetup.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvFieldSetup.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvFieldSetup.CellEditEnterChangesRows = true;
			this.olvFieldSetup.CellEditTabChangesRows = true;
			this.olvFieldSetup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
			this.olvFieldSetup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvFieldSetup.GridLines = true;
			this.olvFieldSetup.HasCollapsibleGroups = false;
			this.olvFieldSetup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.olvFieldSetup.Location = new System.Drawing.Point(3, 16);
			this.olvFieldSetup.Name = "olvFieldSetup";
			this.olvFieldSetup.ShowCommandMenuOnRightClick = true;
			this.olvFieldSetup.ShowGroups = false;
			this.olvFieldSetup.Size = new System.Drawing.Size(553, 44);
			this.olvFieldSetup.TabIndex = 6;
			this.olvFieldSetup.TintSortColumn = true;
			this.olvFieldSetup.UseCompatibleStateImageBehavior = false;
			this.olvFieldSetup.UseExplorerTheme = true;
			this.olvFieldSetup.UseFiltering = true;
			this.olvFieldSetup.UseHotItem = true;
			this.olvFieldSetup.UseTranslucentHotItem = true;
			this.olvFieldSetup.UseTranslucentSelection = true;
			this.olvFieldSetup.View = System.Windows.Forms.View.Details;
			this.olvFieldSetup.VirtualMode = true;
			this.olvFieldSetup.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.olvFieldSetup_CellEditFinishing);
			this.olvFieldSetup.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.olvFieldSetup_CellEditStarting);
			// 
			// btnImport
			// 
			this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnImport.Enabled = false;
			this.btnImport.Location = new System.Drawing.Point(-1, 55);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(560, 23);
			this.btnImport.TabIndex = 9;
			this.btnImport.Text = "Map CSV columns to Date,Open,High,Low,Close,Volume";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer1);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.dataSourcesTree);
			this.splitContainer3.Size = new System.Drawing.Size(802, 462);
			this.splitContainer3.SplitterDistance = 699;
			this.splitContainer3.TabIndex = 1;
			// 
			// importSourceFileBrowser
			// 
			this.importSourceFileBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.importSourceFileBrowser.BackColor = System.Drawing.SystemColors.Control;
			this.importSourceFileBrowser.Location = new System.Drawing.Point(0, 0);
			this.importSourceFileBrowser.Name = "importSourceFileBrowser";
			this.importSourceFileBrowser.Size = new System.Drawing.Size(136, 440);
			this.importSourceFileBrowser.TabIndex = 0;
			this.importSourceFileBrowser.OnFileSelectedTryParse += new System.EventHandler<Sq1.Widgets.CsvImporter.ImportSourcePathInfo>(this.importSourceFileBrowser_OnFileSelectedTryParse);
			this.importSourceFileBrowser.OnDirectoryChanged += new System.EventHandler<Sq1.Widgets.CsvImporter.DirectoryInfoEventArgs>(this.importSourceFileBrowser_OnDirectoryChanged);
			// 
			// mniltbCsvSeparator
			// 
			this.mniltbCsvSeparator.BackColor = System.Drawing.Color.Transparent;
			this.mniltbCsvSeparator.InputFieldAlignedRight = false;
			this.mniltbCsvSeparator.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbCsvSeparator.InputFieldEditable = true;
			this.mniltbCsvSeparator.InputFieldMultiline = true;
			this.mniltbCsvSeparator.InputFieldOffsetX = 90;
			this.mniltbCsvSeparator.InputFieldValue = ",";
			this.mniltbCsvSeparator.InputFieldWidth = 20;
			this.mniltbCsvSeparator.Name = "mniltbCsvSeparator";
			this.mniltbCsvSeparator.OffsetTop = 0;
			this.mniltbCsvSeparator.Size = new System.Drawing.Size(120, 21);
			this.mniltbCsvSeparator.TextLeft = "CSV Separator";
			this.mniltbCsvSeparator.TextLeftOffsetX = 0;
			this.mniltbCsvSeparator.TextLeftWidth = 83;
			this.mniltbCsvSeparator.TextRed = false;
			this.mniltbCsvSeparator.TextRight = "";
			this.mniltbCsvSeparator.TextRightOffsetX = 113;
			this.mniltbCsvSeparator.TextRightWidth = 4;
			this.mniltbCsvSeparator.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbCsvSeparator_UserTyped);
			// 
			// rangeBar
			// 
			this.rangeBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.rangeBar.BackColor = System.Drawing.Color.AliceBlue;
			this.rangeBar.ColorBgOutsideMouseOver = System.Drawing.Color.LightBlue;
			this.rangeBar.ColorBgOutsideRange = System.Drawing.Color.LightSteelBlue;
			this.rangeBar.ColorFgGraph = System.Drawing.Color.DarkSalmon;
			this.rangeBar.Enabled = false;
			this.rangeBar.ForeColor = System.Drawing.Color.Black;
			this.rangeBar.GraphMinHeightGoUnderLabels = 20F;
			this.rangeBar.GraphPenWidth = 1F;
			this.rangeBar.Location = new System.Drawing.Point(0, 0);
			this.rangeBar.Name = "rangeBar";
			this.rangeBar.RangeMax = new System.DateTime(2013, 5, 12, 0, 0, 0, 0);
			this.rangeBar.RangeMin = new System.DateTime(2010, 5, 12, 0, 0, 0, 0);
			this.rangeBar.RangeScaleLabelDistancePx = 0;
			this.rangeBar.ScalePenWidth = 1F;
			this.rangeBar.Size = new System.Drawing.Size(559, 56);
			this.rangeBar.TabIndex = 10;
			this.rangeBar.ValueFormat = "dd-MMM-yy";
			this.rangeBar.ValueMax = new System.DateTime(2012, 5, 12, 0, 0, 0, 0);
			this.rangeBar.ValueMin = new System.DateTime(2011, 5, 12, 0, 0, 0, 0);
			// 
			// dataSourcesTree
			// 
			this.dataSourcesTree.AppendMarketToDataSourceName = false;
			this.dataSourcesTree.BackColor = System.Drawing.SystemColors.Window;
			this.dataSourcesTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataSourcesTree.Location = new System.Drawing.Point(0, 0);
			this.dataSourcesTree.Name = "dataSourcesTree";
			this.dataSourcesTree.Size = new System.Drawing.Size(99, 462);
			this.dataSourcesTree.TabIndex = 0;
			this.dataSourcesTree.TreeFirstColumnNameText = "Name";
			this.dataSourcesTree.OnDataSourceSelected += new System.EventHandler<Sq1.Core.DataFeed.DataSourceEventArgs>(this.dataSourcesTree_OnDataSourceSelected);
			this.dataSourcesTree.OnSymbolSelected += new System.EventHandler<Sq1.Core.DataFeed.DataSourceSymbolEventArgs>(this.dataSourcesTree_OnSymbolSelected);
			// 
			// CsvImporterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.splitContainer3);
			this.Name = "CsvImporterControl";
			this.Size = new System.Drawing.Size(802, 462);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.grpPreviewParsedRaw.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvCsvParsedRaw)).EndInit();
			this.ctxRaw.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.grpPreviewParsedByFormat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvParsedByFormat)).EndInit();
			this.grpImportSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvFieldSetup)).EndInit();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.LinkLabel lnkDownload;
		private System.Windows.Forms.ToolStripMenuItem mniEdit;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbCsvSeparator;
		private System.Windows.Forms.ContextMenuStrip ctxRaw;
		private Sq1.Widgets.CsvImporter.ImportSourceFileBrowser importSourceFileBrowser;
		private BrightIdeasSoftware.OLVColumn columnHeader8;
		private BrightIdeasSoftware.OLVColumn columnHeader7;
		private BrightIdeasSoftware.OLVColumn columnHeader6;
		private BrightIdeasSoftware.OLVColumn columnHeader5;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private BrightIdeasSoftware.OLVColumn columnHeader4;
		private BrightIdeasSoftware.OLVColumn columnHeader3;
		private BrightIdeasSoftware.OLVColumn columnHeader2;
		private BrightIdeasSoftware.OLVColumn columnHeader1;
		private BrightIdeasSoftware.FastObjectListView olvCsvParsedRaw;
		private BrightIdeasSoftware.FastObjectListView olvFieldSetup;
		private System.Windows.Forms.GroupBox grpImportSettings;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.GroupBox grpPreviewParsedRaw;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.GroupBox grpPreviewParsedByFormat;
		private BrightIdeasSoftware.FastObjectListView olvParsedByFormat;
		private BrightIdeasSoftware.OLVColumn olvcDateTime;
		private BrightIdeasSoftware.OLVColumn olvcOpen;
		private BrightIdeasSoftware.OLVColumn olvcHigh;
		private BrightIdeasSoftware.OLVColumn olvcLow;
		private BrightIdeasSoftware.OLVColumn olvcClose;
		private BrightIdeasSoftware.OLVColumn olvcVolume;
		private RangeBar.RangeBarDateTime rangeBar;
		private System.Windows.Forms.Button btnImport;
		private DataSourcesTreeControl dataSourcesTree;
		private BrightIdeasSoftware.OLVColumn olvcUnalignable;
		private BrightIdeasSoftware.OLVColumn olvcSerno;
	}
}