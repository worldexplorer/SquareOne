namespace Sq1.Widgets.CsvImporter {
	partial class ImportSourceFileBrowser {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)  components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportSourceFileBrowser));
			this.lblFolder = new System.Windows.Forms.Label();
			this.txtFolder = new System.Windows.Forms.TextBox();
			this.ctxGoUpBack = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniGo = new System.Windows.Forms.ToolStripMenuItem();
			this.mniUp = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBack = new System.Windows.Forms.ToolStripMenuItem();
			this.olvFiles = new BrightIdeasSoftware.ObjectListView();
			this.olvColumnFileName = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnSize = new BrightIdeasSoftware.OLVColumn();
			this.olvColumnFileModified = new BrightIdeasSoftware.OLVColumn();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.ctxGoUpBack.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvFiles)).BeginInit();
			this.SuspendLayout();
			// 
			// lblFolder
			// 
			this.lblFolder.AutoSize = true;
			this.lblFolder.Location = new System.Drawing.Point(3, 8);
			this.lblFolder.Name = "lblFolder";
			this.lblFolder.Size = new System.Drawing.Size(36, 13);
			this.lblFolder.TabIndex = 2;
			this.lblFolder.Text = "Folder";
			// 
			// txtFolder
			// 
			this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtFolder.ContextMenuStrip = this.ctxGoUpBack;
			this.txtFolder.Location = new System.Drawing.Point(40, 5);
			this.txtFolder.Name = "txtFolder";
			this.txtFolder.Size = new System.Drawing.Size(328, 20);
			this.txtFolder.TabIndex = 3;
			this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
			this.txtFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolder_KeyPress);
			// 
			// ctxGoUpBack
			// 
			this.ctxGoUpBack.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniGo,
									this.mniUp,
									this.mniBack});
			this.ctxGoUpBack.Name = "ctxGoUpBack";
			this.ctxGoUpBack.Size = new System.Drawing.Size(100, 70);
			// 
			// mniGo
			// 
			this.mniGo.Name = "mniGo";
			this.mniGo.Size = new System.Drawing.Size(99, 22);
			this.mniGo.Text = "Go";
			this.mniGo.Click += new System.EventHandler(this.mniGo_Click);
			// 
			// mniUp
			// 
			this.mniUp.Name = "mniUp";
			this.mniUp.Size = new System.Drawing.Size(99, 22);
			this.mniUp.Text = "Up";
			this.mniUp.Click += new System.EventHandler(this.mniUp_Click);
			// 
			// mniBack
			// 
			this.mniBack.Enabled = false;
			this.mniBack.Name = "mniBack";
			this.mniBack.Size = new System.Drawing.Size(99, 22);
			this.mniBack.Text = "Back";
			// 
			// olvFiles
			// 
			this.olvFiles.AllowColumnReorder = true;
			this.olvFiles.AllowDrop = true;
			this.olvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.olvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvFiles.CheckedAspectName = "CheckedForCsvParsing";
			this.olvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.olvColumnFileName,
									this.olvColumnSize,
									this.olvColumnFileModified});
			this.olvFiles.FullRowSelect = true;
			this.olvFiles.HeaderUsesThemes = false;
			this.olvFiles.HideSelection = false;
			this.olvFiles.Location = new System.Drawing.Point(0, 31);
			this.olvFiles.Name = "olvFiles";
			this.olvFiles.OwnerDraw = true;
			this.olvFiles.PersistentCheckBoxes = false;
			this.olvFiles.ShowCommandMenuOnRightClick = true;
			this.olvFiles.ShowGroups = false;
			this.olvFiles.ShowItemToolTips = true;
			this.olvFiles.Size = new System.Drawing.Size(375, 224);
			this.olvFiles.SmallImageList = this.imageList1;
			this.olvFiles.TabIndex = 4;
			this.olvFiles.TintSortColumn = true;
			this.olvFiles.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvFiles.UseCompatibleStateImageBehavior = false;
			this.olvFiles.UseExplorerTheme = true;
			this.olvFiles.UseFilterIndicator = true;
			this.olvFiles.UseFiltering = true;
			this.olvFiles.View = System.Windows.Forms.View.Details;
			this.olvFiles.ItemActivate += new System.EventHandler(this.olvFilesItem_Activate);
			this.olvFiles.SelectedIndexChanged += new System.EventHandler(this.olvFiles_SelectedIndexChanged);
			this.olvFiles.Click += new System.EventHandler(this.olvFilesItem_Activate);
			// 
			// olvColumnFileName
			// 
			this.olvColumnFileName.AspectName = "Name";
			this.olvColumnFileName.AutoCompleteEditor = false;
			this.olvColumnFileName.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
			this.olvColumnFileName.CellPadding = null;
			//this.olvColumnFileName.FillsFreeSpace = true;
			this.olvColumnFileName.Groupable = false;
			this.olvColumnFileName.Text = "CSV File to Parse";
			this.olvColumnFileName.ToolTipText = "- use Control key to select multiple files;\r\n- type to focus on matching item\r\n- " +
			"rightClick on Header to unhide Size and DateModified";
			this.olvColumnFileName.Width = 180;
			// 
			// olvColumnSize
			// 
			this.olvColumnSize.AspectName = "FileSize";
			this.olvColumnSize.AutoCompleteEditor = false;
			this.olvColumnSize.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
			this.olvColumnSize.CellPadding = null;
			this.olvColumnSize.Groupable = false;
			this.olvColumnSize.IsVisible = false;
			this.olvColumnSize.Text = "Size";
			this.olvColumnSize.Width = 70;
			// 
			// olvColumnFileModified
			// 
			this.olvColumnFileModified.AspectName = "LastWriteTime";
			this.olvColumnFileModified.AspectToStringFormat = "";
			this.olvColumnFileModified.AutoCompleteEditor = false;
			this.olvColumnFileModified.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
			this.olvColumnFileModified.CellPadding = null;
			this.olvColumnFileModified.Groupable = false;
			this.olvColumnFileModified.IsVisible = false;
			this.olvColumnFileModified.Text = "DateModified";
			this.olvColumnFileModified.Width = 150;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "compass");
			this.imageList1.Images.SetKeyName(1, "down");
			this.imageList1.Images.SetKeyName(2, "user");
			this.imageList1.Images.SetKeyName(3, "find");
			this.imageList1.Images.SetKeyName(4, "folder");
			this.imageList1.Images.SetKeyName(5, "movie");
			this.imageList1.Images.SetKeyName(6, "music");
			this.imageList1.Images.SetKeyName(7, "no");
			this.imageList1.Images.SetKeyName(8, "readonly");
			this.imageList1.Images.SetKeyName(9, "public");
			this.imageList1.Images.SetKeyName(10, "recycle");
			this.imageList1.Images.SetKeyName(11, "spanner");
			this.imageList1.Images.SetKeyName(12, "star");
			this.imageList1.Images.SetKeyName(13, "tick");
			this.imageList1.Images.SetKeyName(14, "archive");
			this.imageList1.Images.SetKeyName(15, "system");
			this.imageList1.Images.SetKeyName(16, "hidden");
			this.imageList1.Images.SetKeyName(17, "temporary");
			// 
			// ImportSourceFileBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvFiles);
			this.Controls.Add(this.txtFolder);
			this.Controls.Add(this.lblFolder);
			this.Name = "ImportSourceFileBrowser";
			this.Size = new System.Drawing.Size(375, 255);
			this.ctxGoUpBack.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvFiles)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem mniBack;
		private System.Windows.Forms.ToolStripMenuItem mniUp;
		private System.Windows.Forms.ToolStripMenuItem mniGo;
		private System.Windows.Forms.ContextMenuStrip ctxGoUpBack;
		private BrightIdeasSoftware.OLVColumn olvColumnFileModified;
		private BrightIdeasSoftware.OLVColumn olvColumnSize;
		private BrightIdeasSoftware.OLVColumn olvColumnFileName;
		private BrightIdeasSoftware.ObjectListView olvFiles;
		private System.Windows.Forms.TextBox txtFolder;
		private System.Windows.Forms.Label lblFolder;
		private System.Windows.Forms.ImageList imageList1;				
	}
}