namespace Sq1.Widgets.FuturesMerger {
	partial class BarsEditorUserControl {
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.dataSourcesTreeControl = new Sq1.Widgets.DataSourcesTree.DataSourcesTreeControl();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.olvBarsEditor = new BrightIdeasSoftware.FastObjectListView();
			this.olvcFirstUnalignable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOpen = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHigh = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLow = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcClose = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcVolume = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.rangeBar = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.cbxShowRange = new System.Windows.Forms.ToolStripButton();
			this.cbxShowDatasources = new System.Windows.Forms.ToolStripButton();
			this.btnReload = new System.Windows.Forms.ToolStripButton();
			this.cbxRevert = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvBarsEditor)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.dataSourcesTreeControl);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(643, 198);
			this.splitContainer1.SplitterDistance = 122;
			this.splitContainer1.TabIndex = 1;
			// 
			// dataSourcesTreeControl
			// 
			this.dataSourcesTreeControl.AppendMarketToDataSourceName = true;
			this.dataSourcesTreeControl.BackColor = System.Drawing.SystemColors.Window;
			this.dataSourcesTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataSourcesTreeControl.Location = new System.Drawing.Point(0, 0);
			this.dataSourcesTreeControl.Name = "dataSourcesTreeControl";
			this.dataSourcesTreeControl.Size = new System.Drawing.Size(122, 198);
			this.dataSourcesTreeControl.TabIndex = 0;
			this.dataSourcesTreeControl.TreeFirstColumnNameText = "Symbol/Chart";
			this.dataSourcesTreeControl.OnSymbolSelected += new System.EventHandler<Sq1.Core.DataFeed.DataSourceSymbolEventArgs>(this.dataSourcesTreeControl_OnSymbolSelected);
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
			this.splitContainer2.Panel1.Controls.Add(this.olvBarsEditor);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.rangeBar);
			this.splitContainer2.Size = new System.Drawing.Size(517, 198);
			this.splitContainer2.SplitterDistance = 156;
			this.splitContainer2.TabIndex = 0;
			// 
			// olvBarsEditor
			// 
			this.olvBarsEditor.AllColumns.Add(this.olvcFirstUnalignable);
			this.olvBarsEditor.AllColumns.Add(this.olvcSerno);
			this.olvBarsEditor.AllColumns.Add(this.olvcDateTime);
			this.olvBarsEditor.AllColumns.Add(this.olvcOpen);
			this.olvBarsEditor.AllColumns.Add(this.olvcHigh);
			this.olvBarsEditor.AllColumns.Add(this.olvcLow);
			this.olvBarsEditor.AllColumns.Add(this.olvcClose);
			this.olvBarsEditor.AllColumns.Add(this.olvcVolume);
			this.olvBarsEditor.AllowColumnReorder = true;
			this.olvBarsEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvBarsEditor.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvBarsEditor.CellEditEnterChangesRows = true;
			this.olvBarsEditor.CellEditTabChangesRows = true;
			this.olvBarsEditor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcFirstUnalignable,
            this.olvcSerno,
            this.olvcDateTime,
            this.olvcOpen,
            this.olvcHigh,
            this.olvcLow,
            this.olvcClose,
            this.olvcVolume});
			this.olvBarsEditor.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvBarsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvBarsEditor.EmptyListMsg = "TODO: transpose";
			this.olvBarsEditor.HasCollapsibleGroups = false;
			this.olvBarsEditor.HideSelection = false;
			this.olvBarsEditor.Location = new System.Drawing.Point(0, 0);
			this.olvBarsEditor.Name = "olvBarsEditor";
			this.olvBarsEditor.ShowCommandMenuOnRightClick = true;
			this.olvBarsEditor.ShowGroups = false;
			this.olvBarsEditor.Size = new System.Drawing.Size(517, 156);
			this.olvBarsEditor.SortGroupItemsByPrimaryColumn = false;
			this.olvBarsEditor.TabIndex = 0;
			this.olvBarsEditor.TintSortColumn = true;
			this.olvBarsEditor.UseCompatibleStateImageBehavior = false;
			this.olvBarsEditor.UseExplorerTheme = true;
			this.olvBarsEditor.UseFilterIndicator = true;
			this.olvBarsEditor.UseFiltering = true;
			this.olvBarsEditor.UseHotItem = true;
			this.olvBarsEditor.UseTranslucentHotItem = true;
			this.olvBarsEditor.UseTranslucentSelection = true;
			this.olvBarsEditor.View = System.Windows.Forms.View.Details;
			this.olvBarsEditor.VirtualMode = true;
			// 
			// olvcFirstUnalignable
			// 
			this.olvcFirstUnalignable.ShowTextInHeader = false;
			this.olvcFirstUnalignable.Text = "unalignable";
			this.olvcFirstUnalignable.Width = 1;
			// 
			// olvcSerno
			// 
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Text = "#";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Width = 40;
			// 
			// olvcDateTime
			// 
			this.olvcDateTime.Text = "DateTime";
			this.olvcDateTime.Width = 112;
			// 
			// olvcOpen
			// 
			this.olvcOpen.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcOpen.Text = "Open";
			this.olvcOpen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcOpen.Width = 65;
			// 
			// olvcHigh
			// 
			this.olvcHigh.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcHigh.Text = "High";
			this.olvcHigh.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcHigh.Width = 65;
			// 
			// olvcLow
			// 
			this.olvcLow.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcLow.Text = "Low";
			this.olvcLow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcLow.Width = 65;
			// 
			// olvcClose
			// 
			this.olvcClose.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcClose.Text = "Close";
			this.olvcClose.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcClose.Width = 65;
			// 
			// olvcVolume
			// 
			this.olvcVolume.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcVolume.Text = "Volume";
			this.olvcVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcVolume.Width = 65;
			// 
			// rangeBar
			// 
			this.rangeBar.BackColor = System.Drawing.Color.AliceBlue;
			this.rangeBar.ColorBgOutsideMouseOver = System.Drawing.Color.LightBlue;
			this.rangeBar.ColorBgOutsideRange = System.Drawing.Color.LightSteelBlue;
			this.rangeBar.ColorFgGraph = System.Drawing.Color.DarkSalmon;
			this.rangeBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rangeBar.ForeColor = System.Drawing.Color.Black;
			this.rangeBar.GraphMinHeightGoUnderLabels = 20F;
			this.rangeBar.GraphPenWidth = 1F;
			this.rangeBar.Location = new System.Drawing.Point(0, 0);
			this.rangeBar.Name = "rangeBar";
			this.rangeBar.RangeMax = new System.DateTime(2013, 5, 12, 0, 0, 0, 0);
			this.rangeBar.RangeMin = new System.DateTime(2010, 5, 12, 0, 0, 0, 0);
			this.rangeBar.RangeScaleLabelDistancePx = 0;
			this.rangeBar.ScalePenWidth = 1F;
			this.rangeBar.Size = new System.Drawing.Size(517, 38);
			this.rangeBar.TabIndex = 0;
			this.rangeBar.ValueFormat = "dd-MMM-yy";
			this.rangeBar.ValueMax = new System.DateTime(2012, 5, 12, 0, 0, 0, 0);
			this.rangeBar.ValueMin = new System.DateTime(2011, 5, 12, 0, 0, 0, 0);
			// 
			// statusStrip1
			// 
			this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.cbxShowRange,
            this.cbxShowDatasources,
            this.btnReload,
            this.cbxRevert});
			this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip1.Location = new System.Drawing.Point(0, 198);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.ShowItemToolTips = true;
			this.statusStrip1.Size = new System.Drawing.Size(643, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// btnSave
			// 
			this.btnSave.Enabled = false;
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(35, 20);
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// cbxShowRange
			// 
			this.cbxShowRange.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.cbxShowRange.Checked = true;
			this.cbxShowRange.CheckOnClick = true;
			this.cbxShowRange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxShowRange.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cbxShowRange.Name = "cbxShowRange";
			this.cbxShowRange.Size = new System.Drawing.Size(52, 20);
			this.cbxShowRange.Text = "Preview";
			this.cbxShowRange.Click += new System.EventHandler(this.cbxShowRange_Click);
			// 
			// cbxShowDatasources
			// 
			this.cbxShowDatasources.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.cbxShowDatasources.Checked = true;
			this.cbxShowDatasources.CheckOnClick = true;
			this.cbxShowDatasources.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxShowDatasources.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cbxShowDatasources.Name = "cbxShowDatasources";
			this.cbxShowDatasources.Size = new System.Drawing.Size(76, 20);
			this.cbxShowDatasources.Text = "DataSources";
			this.cbxShowDatasources.Click += new System.EventHandler(this.cbxShowDatasources_Click);
			// 
			// btnReload
			// 
			this.btnReload.Enabled = false;
			this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnReload.Name = "btnReload";
			this.btnReload.Size = new System.Drawing.Size(47, 20);
			this.btnReload.Text = "Reload";
			this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
			// 
			// cbxRevert
			// 
			this.cbxRevert.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.cbxRevert.Checked = true;
			this.cbxRevert.CheckOnClick = true;
			this.cbxRevert.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxRevert.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cbxRevert.Name = "cbxRevert";
			this.cbxRevert.Size = new System.Drawing.Size(44, 20);
			this.cbxRevert.Text = "Revert";
			this.cbxRevert.Click += new System.EventHandler(this.cbxRevert_Click);
			// 
			// BarsEditorUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "BarsEditorUserControl";
			this.Size = new System.Drawing.Size(643, 220);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvBarsEditor)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private DataSourcesTree.DataSourcesTreeControl dataSourcesTreeControl;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private BrightIdeasSoftware.FastObjectListView olvBarsEditor;
		private BrightIdeasSoftware.OLVColumn olvcDateTime;
		private BrightIdeasSoftware.OLVColumn olvcOpen;
		private BrightIdeasSoftware.OLVColumn olvcHigh;
		private BrightIdeasSoftware.OLVColumn olvcLow;
		private BrightIdeasSoftware.OLVColumn olvcClose;
		private BrightIdeasSoftware.OLVColumn olvcVolume;
		private RangeBar.RangeBarDateTime rangeBar;
		private System.Windows.Forms.StatusStrip statusStrip1;
		public System.Windows.Forms.ToolStripButton btnSave;
		public System.Windows.Forms.ToolStripButton cbxShowDatasources;
		public System.Windows.Forms.ToolStripButton cbxShowRange;
		private BrightIdeasSoftware.OLVColumn olvcSerno;
		private BrightIdeasSoftware.OLVColumn olvcFirstUnalignable;
		public System.Windows.Forms.ToolStripButton btnReload;
		public System.Windows.Forms.ToolStripButton cbxRevert;
	}
}
