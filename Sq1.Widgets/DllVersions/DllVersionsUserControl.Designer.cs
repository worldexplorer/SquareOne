namespace Sq1.Widgets.DllVersions {
	partial class DllVersionsUserControl {
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
			this.olvDllVersions = new BrightIdeasSoftware.ObjectListView();
			this.olvcResource = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcVersion = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcAuthors = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLicense = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCommit = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcForkMe = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.olvDllVersions)).BeginInit();
			this.SuspendLayout();
			// 
			// olvDllVersions
			// 
			this.olvDllVersions.AllColumns.Add(this.olvcResource);
			this.olvDllVersions.AllColumns.Add(this.olvcVersion);
			this.olvDllVersions.AllColumns.Add(this.olvcSize);
			this.olvDllVersions.AllColumns.Add(this.olvcDate);
			this.olvDllVersions.AllColumns.Add(this.olvcAuthors);
			this.olvDllVersions.AllColumns.Add(this.olvcLicense);
			this.olvDllVersions.AllColumns.Add(this.olvcCommit);
			this.olvDllVersions.AllColumns.Add(this.olvcForkMe);
			this.olvDllVersions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.olvDllVersions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcResource,
            this.olvcVersion,
            this.olvcSize,
            this.olvcDate,
            this.olvcLicense,
            this.olvcCommit,
            this.olvcForkMe});
			this.olvDllVersions.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvDllVersions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvDllVersions.HideSelection = false;
			this.olvDllVersions.IncludeColumnHeadersInCopy = true;
			this.olvDllVersions.Location = new System.Drawing.Point(0, 0);
			this.olvDllVersions.Name = "olvDllVersions";
			this.olvDllVersions.ShowCommandMenuOnRightClick = true;
			this.olvDllVersions.ShowGroups = false;
			this.olvDllVersions.Size = new System.Drawing.Size(800, 300);
			this.olvDllVersions.TabIndex = 0;
			this.olvDllVersions.TintSortColumn = true;
			this.olvDllVersions.UseCompatibleStateImageBehavior = false;
			this.olvDllVersions.UseFilterIndicator = true;
			this.olvDllVersions.UseHotItem = true;
			this.olvDllVersions.UseHyperlinks = true;
			this.olvDllVersions.UseTranslucentHotItem = true;
			this.olvDllVersions.UseTranslucentSelection = true;
			this.olvDllVersions.View = System.Windows.Forms.View.Details;
			// 
			// olvcResource
			// 
			this.olvcResource.Text = "Resource";
			this.olvcResource.Width = 71;
			// 
			// olvcVersion
			// 
			this.olvcVersion.Text = "Version";
			this.olvcVersion.Width = 59;
			// 
			// olvcSize
			// 
			this.olvcSize.Text = "Size";
			// 
			// olvcDate
			// 
			this.olvcDate.Text = "Date";
			this.olvcDate.Width = 92;
			// 
			// olvcAuthors
			// 
			this.olvcAuthors.DisplayIndex = 4;
			this.olvcAuthors.IsVisible = false;
			this.olvcAuthors.Text = "Authors";
			this.olvcAuthors.Width = 100;
			// 
			// olvcLicense
			// 
			this.olvcLicense.Hyperlink = true;
			this.olvcLicense.Text = "License";
			this.olvcLicense.Width = 51;
			// 
			// olvcCommit
			// 
			this.olvcCommit.Text = "Commit";
			this.olvcCommit.Width = 325;
			// 
			// olvcForkMe
			// 
			this.olvcForkMe.Text = "Fork";
			this.olvcForkMe.Width = 40;
			// 
			// DllVersionsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvDllVersions);
			this.Name = "DllVersionsUserControl";
			this.Size = new System.Drawing.Size(800, 300);
			((System.ComponentModel.ISupportInitialize)(this.olvDllVersions)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView olvDllVersions;
		private BrightIdeasSoftware.OLVColumn olvcResource;
		private BrightIdeasSoftware.OLVColumn olvcVersion;
		private BrightIdeasSoftware.OLVColumn olvcCommit;
		private BrightIdeasSoftware.OLVColumn olvcLicense;
		private BrightIdeasSoftware.OLVColumn olvcSize;
		private BrightIdeasSoftware.OLVColumn olvcDate;
		private BrightIdeasSoftware.OLVColumn olvcAuthors;
		private BrightIdeasSoftware.OLVColumn olvcForkMe;
	}
}
