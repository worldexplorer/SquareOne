namespace Sq1.Widgets.VersionsCredits {
	partial class VersionsCreditsUserControl {
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
			this.olvVersionsCredits = new BrightIdeasSoftware.ObjectListView();
			this.olvcResource = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcVersion = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcAuthors = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLicense = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCommit = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcForkMe = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.olvVersionsCredits)).BeginInit();
			this.SuspendLayout();
			// 
			// olvVersionsCredits
			// 
			this.olvVersionsCredits.AllColumns.Add(this.olvcResource);
			this.olvVersionsCredits.AllColumns.Add(this.olvcVersion);
			this.olvVersionsCredits.AllColumns.Add(this.olvcSize);
			this.olvVersionsCredits.AllColumns.Add(this.olvcDate);
			this.olvVersionsCredits.AllColumns.Add(this.olvcAuthors);
			this.olvVersionsCredits.AllColumns.Add(this.olvcLicense);
			this.olvVersionsCredits.AllColumns.Add(this.olvcCommit);
			this.olvVersionsCredits.AllColumns.Add(this.olvcForkMe);
			this.olvVersionsCredits.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.olvVersionsCredits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcResource,
            this.olvcVersion,
            this.olvcSize,
            this.olvcDate,
            this.olvcAuthors,
            this.olvcLicense,
            this.olvcCommit,
            this.olvcForkMe});
			this.olvVersionsCredits.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvVersionsCredits.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvVersionsCredits.HideSelection = false;
			this.olvVersionsCredits.IncludeColumnHeadersInCopy = true;
			this.olvVersionsCredits.Location = new System.Drawing.Point(0, 0);
			this.olvVersionsCredits.Name = "olvVersionsCredits";
			this.olvVersionsCredits.ShowCommandMenuOnRightClick = true;
			this.olvVersionsCredits.ShowGroups = false;
			this.olvVersionsCredits.Size = new System.Drawing.Size(800, 300);
			this.olvVersionsCredits.TabIndex = 0;
			this.olvVersionsCredits.TintSortColumn = true;
			this.olvVersionsCredits.UseCompatibleStateImageBehavior = false;
			this.olvVersionsCredits.UseFilterIndicator = true;
			this.olvVersionsCredits.UseHotItem = true;
			this.olvVersionsCredits.UseHyperlinks = true;
			this.olvVersionsCredits.UseTranslucentHotItem = true;
			this.olvVersionsCredits.UseTranslucentSelection = true;
			this.olvVersionsCredits.View = System.Windows.Forms.View.Details;
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
			// VersionsCreditsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvVersionsCredits);
			this.Name = "VersionsCreditsUserControl";
			this.Size = new System.Drawing.Size(800, 300);
			((System.ComponentModel.ISupportInitialize)(this.olvVersionsCredits)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView olvVersionsCredits;
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
