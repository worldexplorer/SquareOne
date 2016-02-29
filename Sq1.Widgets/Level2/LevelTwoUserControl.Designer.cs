namespace Sq1.Widgets.Level2 {
	partial class LevelTwoUserControl {
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
			this.OlvLevelTwo = new BrightIdeasSoftware.ObjectListView();
			this.olvWeirdBehaviourShiftedToFirstColumnFake = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvAskCumulative = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvAsk = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvPrice = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvBid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvBidCumulative = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.OlvLevelTwo)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlWindowTitle
			// 
			this.pnlWindowTitle.Size = new System.Drawing.Size(228, 19);
			// 
			// UserControlInner
			// 
			this.UserControlInner.Location = new System.Drawing.Point(4, 23);
			this.UserControlInner.Padding = new System.Windows.Forms.Padding(0);
			this.UserControlInner.Size = new System.Drawing.Size(228, 383);
			// 
			// OlvLevelTwo
			// 
			this.OlvLevelTwo.AllColumns.Add(this.olvWeirdBehaviourShiftedToFirstColumnFake);
			this.OlvLevelTwo.AllColumns.Add(this.olvAskCumulative);
			this.OlvLevelTwo.AllColumns.Add(this.olvAsk);
			this.OlvLevelTwo.AllColumns.Add(this.olvPrice);
			this.OlvLevelTwo.AllColumns.Add(this.olvBid);
			this.OlvLevelTwo.AllColumns.Add(this.olvBidCumulative);
			this.OlvLevelTwo.AllowColumnReorder = true;
			this.OlvLevelTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.OlvLevelTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.OlvLevelTwo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvWeirdBehaviourShiftedToFirstColumnFake,
            this.olvAsk,
            this.olvPrice,
            this.olvBid});
			this.OlvLevelTwo.Cursor = System.Windows.Forms.Cursors.Default;
			this.OlvLevelTwo.FullRowSelect = true;
			this.OlvLevelTwo.HideSelection = false;
			this.OlvLevelTwo.Location = new System.Drawing.Point(4, 23);
			this.OlvLevelTwo.Margin = new System.Windows.Forms.Padding(3, 30, 3, 3);
			this.OlvLevelTwo.Name = "OlvLevelTwo";
			this.OlvLevelTwo.ShowCommandMenuOnRightClick = true;
			this.OlvLevelTwo.ShowGroups = false;
			this.OlvLevelTwo.Size = new System.Drawing.Size(228, 383);
			this.OlvLevelTwo.TabIndex = 2;
			this.OlvLevelTwo.TintSortColumn = true;
			this.OlvLevelTwo.UseCompatibleStateImageBehavior = false;
			this.OlvLevelTwo.UseFilterIndicator = true;
			this.OlvLevelTwo.UseHotItem = true;
			this.OlvLevelTwo.UseTranslucentHotItem = true;
			this.OlvLevelTwo.UseTranslucentSelection = true;
			this.OlvLevelTwo.View = System.Windows.Forms.View.Details;
			// 
			// olvWeirdBehaviourShiftedToFirstColumnFake
			// 
			this.olvWeirdBehaviourShiftedToFirstColumnFake.IsVisible = false;
			this.olvWeirdBehaviourShiftedToFirstColumnFake.Text = "_WeirdBehaviourShiftedToFirstColumnFake";
			this.olvWeirdBehaviourShiftedToFirstColumnFake.Width = 1;
			// 
			// olvAskCumulative
			// 
			this.olvAskCumulative.DisplayIndex = 0;
			this.olvAskCumulative.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvAskCumulative.IsVisible = false;
			this.olvAskCumulative.Text = "AskCumulative";
			this.olvAskCumulative.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvAsk
			// 
			this.olvAsk.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvAsk.Text = "AskVolume";
			this.olvAsk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvPrice
			// 
			this.olvPrice.FillsFreeSpace = true;
			this.olvPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvPrice.IsEditable = false;
			this.olvPrice.Text = "Price";
			this.olvPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvPrice.Width = 100;
			// 
			// olvBid
			// 
			this.olvBid.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvBid.Text = "BidVolume";
			this.olvBid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvBidCumulative
			// 
			this.olvBidCumulative.DisplayIndex = 4;
			this.olvBidCumulative.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvBidCumulative.IsVisible = false;
			this.olvBidCumulative.Text = "BidCumulative";
			this.olvBidCumulative.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// LevelTwoUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.OlvLevelTwo);
			this.Name = "LevelTwoUserControl";
			this.Size = new System.Drawing.Size(236, 410);
			this.Controls.SetChildIndex(this.pnlWindowTitle, 0);
			this.Controls.SetChildIndex(this.UserControlInner, 0);
			this.Controls.SetChildIndex(this.OlvLevelTwo, 0);
			((System.ComponentModel.ISupportInitialize)(this.OlvLevelTwo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.OLVColumn olvAsk;
		private BrightIdeasSoftware.OLVColumn olvPrice;
		private BrightIdeasSoftware.OLVColumn olvBid;
		private BrightIdeasSoftware.OLVColumn olvAskCumulative;
		private BrightIdeasSoftware.OLVColumn olvBidCumulative;
		private BrightIdeasSoftware.OLVColumn olvWeirdBehaviourShiftedToFirstColumnFake;
		public BrightIdeasSoftware.ObjectListView OlvLevelTwo;
	}
}
