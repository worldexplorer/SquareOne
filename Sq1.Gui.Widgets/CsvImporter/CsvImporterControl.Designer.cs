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
			this.importSourceFileBrowser1 = new Sq1.Widgets.CsvImporter.ImportSourceFileBrowser();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.grpPreviewParsedRaw = new System.Windows.Forms.GroupBox();
			this.olvCsvParsedRaw = new BrightIdeasSoftware.FastObjectListView();
			this.columnHeader1 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader2 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader3 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader4 = new BrightIdeasSoftware.OLVColumn();
			this.ctxRaw = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniltbCsvSeparator = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.grpPreviewParsedByFormat = new System.Windows.Forms.GroupBox();
			this.olvParsedByFormat = new BrightIdeasSoftware.FastObjectListView();
			this.olvColumnDate = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnOpen = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnHigh = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnLow = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnClose = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnVolume = new BrightIdeasSoftware.OLVColumn();
			this.grpImportSettings = new System.Windows.Forms.GroupBox();
			this.olvFieldSetup = new BrightIdeasSoftware.FastObjectListView();
			this.columnHeader5 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader6 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader7 = new BrightIdeasSoftware.OLVColumn();
			this.columnHeader8 = new BrightIdeasSoftware.OLVColumn();
			this.rangeBarDateTime1 = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			this.btnImport = new System.Windows.Forms.Button();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.dataSourcesTree1 = new Sq1.Widgets.DataSourcesTree.DataSourcesTreeControl();
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
			this.splitContainer1.Panel1.Controls.Add(this.importSourceFileBrowser1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(657, 462);
			this.splitContainer1.SplitterDistance = 170;
			this.splitContainer1.TabIndex = 0;
			// 
			// lnkDownload
			// 
			this.lnkDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lnkDownload.BackColor = System.Drawing.SystemColors.ControlLight;
			this.lnkDownload.Location = new System.Drawing.Point(0, 441);
			this.lnkDownload.Name = "lnkDownload";
			this.lnkDownload.Size = new System.Drawing.Size(170, 21);
			this.lnkDownload.TabIndex = 1;
			this.lnkDownload.TabStop = true;
			this.lnkDownload.Text = "Download";
			this.lnkDownload.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lnkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDownload_LinkClicked);
			// 
			// importSourceFileBrowser1
			// 
			this.importSourceFileBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.importSourceFileBrowser1.BackColor = System.Drawing.SystemColors.Control;
			this.importSourceFileBrowser1.Location = new System.Drawing.Point(0, 0);
			this.importSourceFileBrowser1.Name = "importSourceFileBrowser1";
			this.importSourceFileBrowser1.Size = new System.Drawing.Size(170, 440);
			this.importSourceFileBrowser1.TabIndex = 0;
			this.importSourceFileBrowser1.OnFileSelectedTryParse += new System.EventHandler<Sq1.Widgets.CsvImporter.ImportSourcePathInfo>(this.importSourceFileBrowser_OnFileSelectedTryParse);
			this.importSourceFileBrowser1.OnDirectoryChanged += new System.EventHandler<Sq1.Widgets.CsvImporter.DirectoryInfoEventArgs>(this.importSourceFileBrowser_OnDirectoryChanged);
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
			this.splitContainer2.Size = new System.Drawing.Size(483, 462);
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
			this.grpPreviewParsedRaw.Size = new System.Drawing.Size(483, 130);
			this.grpPreviewParsedRaw.TabIndex = 25;
			this.grpPreviewParsedRaw.TabStop = false;
			this.grpPreviewParsedRaw.Text = "Raw CSV | Select File in Filesystem in Left Panel";
			// 
			// olvCsvParsedRaw
			// 
			this.olvCsvParsedRaw.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvCsvParsedRaw.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvCsvParsedRaw.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvCsvParsedRaw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader1,
									this.columnHeader2,
									this.columnHeader3,
									this.columnHeader4});
			this.olvCsvParsedRaw.ContextMenuStrip = this.ctxRaw;
			this.olvCsvParsedRaw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvCsvParsedRaw.EmptyListMsgFont = new System.Drawing.Font("Consolas", 9.75F);
			this.olvCsvParsedRaw.FullRowSelect = true;
			this.olvCsvParsedRaw.GridLines = true;
			this.olvCsvParsedRaw.HasCollapsibleGroups = false;
			this.olvCsvParsedRaw.Location = new System.Drawing.Point(3, 16);
			this.olvCsvParsedRaw.Name = "olvCsvParsedRaw";
			this.olvCsvParsedRaw.ShowGroups = false;
			this.olvCsvParsedRaw.Size = new System.Drawing.Size(477, 111);
			this.olvCsvParsedRaw.TabIndex = 0;
			this.olvCsvParsedRaw.UseCompatibleStateImageBehavior = false;
			this.olvCsvParsedRaw.View = System.Windows.Forms.View.Details;
			this.olvCsvParsedRaw.VirtualMode = true;
			this.olvCsvParsedRaw.DoubleClick += new System.EventHandler(this.csvPreviewParsedRaw_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.CellPadding = null;
			// 
			// columnHeader2
			// 
			this.columnHeader2.CellPadding = null;
			// 
			// columnHeader3
			// 
			this.columnHeader3.CellPadding = null;
			// 
			// columnHeader4
			// 
			this.columnHeader4.CellPadding = null;
			// 
			// ctxRaw
			// 
			this.ctxRaw.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniltbCsvSeparator,
									this.mniEdit});
			this.ctxRaw.Name = "ctxRaw";
			this.ctxRaw.Size = new System.Drawing.Size(226, 50);
			// 
			// mniltbCsvSeparator
			// 
			this.mniltbCsvSeparator.BackColor = System.Drawing.Color.Transparent;
			this.mniltbCsvSeparator.InputFieldOffsetX = 90;
			this.mniltbCsvSeparator.InputFieldValue = ",";
			this.mniltbCsvSeparator.InputFieldWidth = 20;
			this.mniltbCsvSeparator.Name = "mniltbCsvSeparator";
			this.mniltbCsvSeparator.Size = new System.Drawing.Size(165, 21);
			this.mniltbCsvSeparator.Text = "CSV Separator:";
			this.mniltbCsvSeparator.TextRed = false;
			this.mniltbCsvSeparator.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbCsvSeparator_UserTyped);
			// 
			// mniEdit
			// 
			this.mniEdit.Name = "mniEdit";
			this.mniEdit.Size = new System.Drawing.Size(225, 22);
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
			this.splitContainer5.Panel2.Controls.Add(this.rangeBarDateTime1);
			this.splitContainer5.Panel2.Controls.Add(this.btnImport);
			this.splitContainer5.Size = new System.Drawing.Size(483, 328);
			this.splitContainer5.SplitterDistance = 247;
			this.splitContainer5.TabIndex = 11;
			// 
			// grpPreviewParsedByFormat
			// 
			this.grpPreviewParsedByFormat.BackColor = System.Drawing.SystemColors.Control;
			this.grpPreviewParsedByFormat.Controls.Add(this.olvParsedByFormat);
			this.grpPreviewParsedByFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpPreviewParsedByFormat.Location = new System.Drawing.Point(0, 59);
			this.grpPreviewParsedByFormat.Name = "grpPreviewParsedByFormat";
			this.grpPreviewParsedByFormat.Size = new System.Drawing.Size(483, 188);
			this.grpPreviewParsedByFormat.TabIndex = 30;
			this.grpPreviewParsedByFormat.TabStop = false;
			this.grpPreviewParsedByFormat.Text = "Parsed By Format | Symbol[]  ScaleInerval[] BarsParsed.Count[]";
			// 
			// olvParsedByFormat
			// 
			this.olvParsedByFormat.AutoArrange = false;
			this.olvParsedByFormat.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvParsedByFormat.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvParsedByFormat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.olvColumnDate,
									this.olvColumnOpen,
									this.olvColumnHigh,
									this.olvColumnLow,
									this.olvColumnClose,
									this.olvColumnVolume});
			this.olvParsedByFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvParsedByFormat.EmptyListMsgFont = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvParsedByFormat.FullRowSelect = true;
			this.olvParsedByFormat.GridLines = true;
			this.olvParsedByFormat.HasCollapsibleGroups = false;
			this.olvParsedByFormat.Location = new System.Drawing.Point(3, 16);
			this.olvParsedByFormat.Name = "olvParsedByFormat";
			this.olvParsedByFormat.ShowGroups = false;
			this.olvParsedByFormat.Size = new System.Drawing.Size(477, 169);
			this.olvParsedByFormat.TabIndex = 10;
			this.olvParsedByFormat.UseCompatibleStateImageBehavior = false;
			this.olvParsedByFormat.View = System.Windows.Forms.View.Details;
			this.olvParsedByFormat.VirtualMode = true;
			// 
			// olvColumnDate
			// 
			this.olvColumnDate.AspectName = "DateTime";
			this.olvColumnDate.AspectToStringFormat = "{0:yyyy-MMM-dd HH:mm:ss}";
			this.olvColumnDate.CellPadding = null;
			this.olvColumnDate.Text = "DateTime";
			this.olvColumnDate.Width = 120;
			// 
			// olvColumnOpen
			// 
			this.olvColumnOpen.AspectName = "Open";
			this.olvColumnOpen.CellPadding = null;
			this.olvColumnOpen.Text = "Open";
			this.olvColumnOpen.Width = 75;
			// 
			// olvColumnHigh
			// 
			this.olvColumnHigh.AspectName = "High";
			this.olvColumnHigh.CellPadding = null;
			this.olvColumnHigh.Text = "High";
			this.olvColumnHigh.Width = 75;
			// 
			// olvColumnLow
			// 
			this.olvColumnLow.AspectName = "Low";
			this.olvColumnLow.CellPadding = null;
			this.olvColumnLow.Text = "Low";
			this.olvColumnLow.Width = 75;
			// 
			// olvColumnClose
			// 
			this.olvColumnClose.AspectName = "Close";
			this.olvColumnClose.CellPadding = null;
			this.olvColumnClose.Text = "Close";
			this.olvColumnClose.Width = 75;
			// 
			// olvColumnVolume
			// 
			this.olvColumnVolume.AspectName = "Volume";
			this.olvColumnVolume.CellPadding = null;
			this.olvColumnVolume.Text = "Volume";
			// 
			// grpImportSettings
			// 
			this.grpImportSettings.BackColor = System.Drawing.SystemColors.Control;
			this.grpImportSettings.Controls.Add(this.olvFieldSetup);
			this.grpImportSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpImportSettings.Location = new System.Drawing.Point(0, 0);
			this.grpImportSettings.Name = "grpImportSettings";
			this.grpImportSettings.Size = new System.Drawing.Size(483, 59);
			this.grpImportSettings.TabIndex = 28;
			this.grpImportSettings.TabStop = false;
			this.grpImportSettings.Text = "CSV Columns => DateTime, Open, High, Low, Close, Volume settings";
			// 
			// olvFieldSetup
			// 
			this.olvFieldSetup.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvFieldSetup.AutoArrange = false;
			this.olvFieldSetup.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvFieldSetup.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
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
			this.olvFieldSetup.Scrollable = false;
			this.olvFieldSetup.ShowGroups = false;
			this.olvFieldSetup.Size = new System.Drawing.Size(477, 40);
			this.olvFieldSetup.TabIndex = 6;
			this.olvFieldSetup.UseCompatibleStateImageBehavior = false;
			this.olvFieldSetup.View = System.Windows.Forms.View.Details;
			this.olvFieldSetup.VirtualMode = true;
			this.olvFieldSetup.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.olvFieldSetup_CellEditFinishing);
			this.olvFieldSetup.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.olvFieldSetup_CellEditStarting);
			// 
			// columnHeader5
			// 
			this.columnHeader5.CellPadding = null;
			// 
			// columnHeader6
			// 
			this.columnHeader6.CellPadding = null;
			// 
			// columnHeader7
			// 
			this.columnHeader7.CellPadding = null;
			// 
			// columnHeader8
			// 
			this.columnHeader8.CellPadding = null;
			// 
			// rangeBarDateTime1
			// 
			this.rangeBarDateTime1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.rangeBarDateTime1.BackColor = System.Drawing.Color.AliceBlue;
			this.rangeBarDateTime1.ColorBgOutsideMouseOver = System.Drawing.Color.LightBlue;
			this.rangeBarDateTime1.ColorBgOutsideRange = System.Drawing.Color.LightSteelBlue;
			this.rangeBarDateTime1.ColorFgGraph = System.Drawing.Color.DarkSalmon;
			this.rangeBarDateTime1.Enabled = false;
			this.rangeBarDateTime1.ForeColor = System.Drawing.Color.Black;
			this.rangeBarDateTime1.GraphMinHeightGoUnderLabels = 20F;
			this.rangeBarDateTime1.GraphPenWidth = 1F;
			this.rangeBarDateTime1.Location = new System.Drawing.Point(0, 2);
			this.rangeBarDateTime1.Name = "rangeBarDateTime1";
			this.rangeBarDateTime1.RangeMax = new System.DateTime(2013, 5, 12, 0, 0, 0, 0);
			this.rangeBarDateTime1.RangeMin = new System.DateTime(2010, 5, 12, 0, 0, 0, 0);
			this.rangeBarDateTime1.RangeScaleLabelDistancePx = 0;
			this.rangeBarDateTime1.ScalePenWidth = 1F;
			this.rangeBarDateTime1.Size = new System.Drawing.Size(483, 47);
			this.rangeBarDateTime1.TabIndex = 10;
			this.rangeBarDateTime1.ValueFormat = "dd-MMM-yy";
			this.rangeBarDateTime1.ValueMax = new System.DateTime(2012, 5, 12, 0, 0, 0, 0);
			this.rangeBarDateTime1.ValueMin = new System.DateTime(2011, 5, 12, 0, 0, 0, 0);
			// 
			// btnImport
			// 
			this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.btnImport.Enabled = false;
			this.btnImport.Location = new System.Drawing.Point(-1, 55);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(484, 23);
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
			this.splitContainer3.Panel2.Controls.Add(this.dataSourcesTree1);
			this.splitContainer3.Size = new System.Drawing.Size(802, 462);
			this.splitContainer3.SplitterDistance = 657;
			this.splitContainer3.TabIndex = 1;
			// 
			// dataSourcesTree1
			// 
			this.dataSourcesTree1.BackColor = System.Drawing.SystemColors.Window;
			this.dataSourcesTree1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataSourcesTree1.Location = new System.Drawing.Point(0, 0);
			this.dataSourcesTree1.Name = "dataSourcesTree1";
			this.dataSourcesTree1.ShowScaleIntervalInsteadOfMarket = true;
			this.dataSourcesTree1.Size = new System.Drawing.Size(141, 462);
			this.dataSourcesTree1.TabIndex = 0;
			this.dataSourcesTree1.TreeFirstColumnNameText = "Name";
			this.dataSourcesTree1.OnDataSourceSelected += new System.EventHandler<Sq1.Core.DataFeed.DataSourceEventArgs>(this.dataSourcesTree1_OnDataSourceSelected);
			this.dataSourcesTree1.OnSymbolSelected += new System.EventHandler<Sq1.Core.DataFeed.DataSourceSymbolEventArgs>(this.dataSourcesTree1_OnSymbolSelected);
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
		private Sq1.Widgets.CsvImporter.ImportSourceFileBrowser importSourceFileBrowser1;
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
		private BrightIdeasSoftware.OLVColumn olvColumnDate;
		private BrightIdeasSoftware.OLVColumn olvColumnOpen;
		private BrightIdeasSoftware.OLVColumn olvColumnHigh;
		private BrightIdeasSoftware.OLVColumn olvColumnLow;
		private BrightIdeasSoftware.OLVColumn olvColumnClose;
		private BrightIdeasSoftware.OLVColumn olvColumnVolume;
		private RangeBar.RangeBarDateTime rangeBarDateTime1;
		private System.Windows.Forms.Button btnImport;
		private DataSourcesTreeControl dataSourcesTree1;
	}
}