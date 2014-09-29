namespace Sq1.Widgets.CsvImporter {
	partial class ImportTargetDataSource {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.tlvDataSources = new BrightIdeasSoftware.TreeListView();
			this.olvcName = new BrightIdeasSoftware.OLVColumn();
			((System.ComponentModel.ISupportInitialize)(this.tlvDataSources)).BeginInit();
			this.SuspendLayout();
			// 
			// tlvDataSources
			// 
			this.tlvDataSources.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tlvDataSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.olvcName});
			this.tlvDataSources.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlvDataSources.FullRowSelect = true;
			this.tlvDataSources.HeaderUsesThemes = false;
			this.tlvDataSources.Location = new System.Drawing.Point(0, 0);
			this.tlvDataSources.Name = "tlvDataSources";
			this.tlvDataSources.OwnerDraw = true;
			this.tlvDataSources.ShowGroups = false;
			this.tlvDataSources.Size = new System.Drawing.Size(295, 432);
			this.tlvDataSources.TabIndex = 0;
			this.tlvDataSources.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.tlvDataSources.UseCompatibleStateImageBehavior = false;
			this.tlvDataSources.UseFilterIndicator = true;
			this.tlvDataSources.UseFiltering = true;
			this.tlvDataSources.View = System.Windows.Forms.View.Details;
			this.tlvDataSources.VirtualMode = true;
			// 
			// olvcName
			// 
			this.olvcName.CellPadding = null;
			this.olvcName.Text = "Name";
			// 
			// ImportTargetDataSource
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlvDataSources);
			this.Name = "ImportTargetDataSource";
			this.Size = new System.Drawing.Size(295, 432);
			((System.ComponentModel.ISupportInitialize)(this.tlvDataSources)).EndInit();
			this.ResumeLayout(false);
		}
		private BrightIdeasSoftware.OLVColumn olvcName;
		private BrightIdeasSoftware.TreeListView tlvDataSources;
	}
}
