namespace Sq1.Adapters.Quik.Streaming.Monitor {
	partial class QuikStreamingMonitorDomUserControl {
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblDomTitle = new System.Windows.Forms.Label();
			this.olvcDom = new BrightIdeasSoftware.ObjectListView();
			this.olvAsk = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvPrice = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvBid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvcDom)).BeginInit();
			this.SuspendLayout();
			// 
			// UserControlInner
			// 
			this.UserControlInner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.UserControlInner.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
			this.UserControlInner.Size = new System.Drawing.Size(210, 375);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.lblDomTitle);
			this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.panel1.ForeColor = System.Drawing.SystemColors.Window;
			this.panel1.Location = new System.Drawing.Point(4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(210, 19);
			this.panel1.TabIndex = 1;
			// 
			// lblDomTitle
			// 
			this.lblDomTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDomTitle.AutoSize = true;
			this.lblDomTitle.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblDomTitle.Location = new System.Drawing.Point(3, 2);
			this.lblDomTitle.Name = "lblDomTitle";
			this.lblDomTitle.Size = new System.Drawing.Size(232, 13);
			this.lblDomTitle.TabIndex = 0;
			this.lblDomTitle.Text = "SQ1-RIM3-dom [232323] RECEIVING#2";
			// 
			// olvcDom
			// 
			this.olvcDom.AllColumns.Add(this.olvAsk);
			this.olvcDom.AllColumns.Add(this.olvPrice);
			this.olvcDom.AllColumns.Add(this.olvBid);
			this.olvcDom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.olvcDom.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvAsk,
            this.olvPrice,
            this.olvBid});
			this.olvcDom.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvcDom.Location = new System.Drawing.Point(4, 72);
			this.olvcDom.Margin = new System.Windows.Forms.Padding(3, 30, 3, 3);
			this.olvcDom.Name = "olvcDom";
			this.olvcDom.ShowCommandMenuOnRightClick = true;
			this.olvcDom.ShowGroups = false;
			this.olvcDom.Size = new System.Drawing.Size(210, 261);
			this.olvcDom.TabIndex = 2;
			this.olvcDom.UseCompatibleStateImageBehavior = false;
			this.olvcDom.UseFilterIndicator = true;
			this.olvcDom.UseHotItem = true;
			this.olvcDom.UseTranslucentHotItem = true;
			this.olvcDom.UseTranslucentSelection = true;
			this.olvcDom.View = System.Windows.Forms.View.Details;
			// 
			// olvAsk
			// 
			this.olvAsk.FillsFreeSpace = true;
			this.olvAsk.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvAsk.Text = "Ask";
			this.olvAsk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// olvPrice
			// 
			this.olvPrice.FillsFreeSpace = true;
			this.olvPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvPrice.Text = "Price";
			this.olvPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvPrice.Width = 80;
			// 
			// olvBid
			// 
			this.olvBid.FillsFreeSpace = true;
			this.olvBid.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvBid.Text = "Bid";
			this.olvBid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// QuikStreamingMonitorDomUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvcDom);
			this.Controls.Add(this.panel1);
			this.Name = "QuikStreamingMonitorDomUserControl";
			this.Size = new System.Drawing.Size(218, 383);
			this.Controls.SetChildIndex(this.UserControlInner, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.olvcDom, 0);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvcDom)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblDomTitle;
		private BrightIdeasSoftware.OLVColumn olvAsk;
		private BrightIdeasSoftware.OLVColumn olvPrice;
		private BrightIdeasSoftware.OLVColumn olvBid;
		private BrightIdeasSoftware.ObjectListView olvcDom;
	}
}
