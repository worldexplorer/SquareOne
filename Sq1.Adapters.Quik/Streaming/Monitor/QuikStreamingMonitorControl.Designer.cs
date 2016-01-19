namespace Sq1.Adapters.Quik.Streaming.Monitor {
	partial class QuikStreamingMonitorControl {
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
			this.grpQuotes = new System.Windows.Forms.GroupBox();
			this.OlvQuotes = new BrightIdeasSoftware.ObjectListView();
			this.olvcQuotesSymbolClass = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesServerTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesQty = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesBid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesAsk = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesLast = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesFortsDepositBuy = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesFortsDepositSell = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesFortsPriceMin = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesFortsPriceMax = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuotesAbsnoPerSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.grpDom = new System.Windows.Forms.GroupBox();
			this.flpDoms = new System.Windows.Forms.FlowLayoutPanel();
			this.grpTrades = new System.Windows.Forms.GroupBox();
			this.olvTrades = new BrightIdeasSoftware.ObjectListView();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.grpQuotes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.OlvQuotes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.grpDom.SuspendLayout();
			this.grpTrades.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvTrades)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel1.Controls.Add(this.grpQuotes);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(856, 474);
			this.splitContainer1.SplitterDistance = 140;
			this.splitContainer1.TabIndex = 0;
			// 
			// grpQuotes
			// 
			this.grpQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpQuotes.Controls.Add(this.OlvQuotes);
			this.grpQuotes.Location = new System.Drawing.Point(9, 3);
			this.grpQuotes.Margin = new System.Windows.Forms.Padding(9, 3, 9, 9);
			this.grpQuotes.Name = "grpQuotes";
			this.grpQuotes.Size = new System.Drawing.Size(838, 128);
			this.grpQuotes.TabIndex = 0;
			this.grpQuotes.TabStop = false;
			this.grpQuotes.Text = "Quotes [Sq1-quotes]:26116 //TableQuotes.DataStructuresParsed_Table_MUST_BE_THE_SE" +
    "NDER";
			// 
			// OlvQuotes
			// 
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesSymbolClass);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesSymbol);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesServerTime);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesQty);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesBid);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesAsk);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesLast);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesFortsDepositBuy);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesFortsDepositSell);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesFortsPriceMin);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesFortsPriceMax);
			this.OlvQuotes.AllColumns.Add(this.olvcQuotesAbsnoPerSymbol);
			this.OlvQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OlvQuotes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcQuotesSymbolClass,
            this.olvcQuotesSymbol,
            this.olvcQuotesServerTime,
            this.olvcQuotesQty,
            this.olvcQuotesBid,
            this.olvcQuotesAsk,
            this.olvcQuotesLast,
            this.olvcQuotesFortsDepositBuy,
            this.olvcQuotesFortsDepositSell,
            this.olvcQuotesFortsPriceMin,
            this.olvcQuotesFortsPriceMax,
            this.olvcQuotesAbsnoPerSymbol});
			this.OlvQuotes.Cursor = System.Windows.Forms.Cursors.Default;
			this.OlvQuotes.Location = new System.Drawing.Point(6, 19);
			this.OlvQuotes.Name = "OlvQuotes";
			this.OlvQuotes.ShowGroups = false;
			this.OlvQuotes.Size = new System.Drawing.Size(826, 103);
			this.OlvQuotes.TabIndex = 2;
			this.OlvQuotes.TintSortColumn = true;
			this.OlvQuotes.UseCompatibleStateImageBehavior = false;
			this.OlvQuotes.UseHotItem = true;
			this.OlvQuotes.UseTranslucentHotItem = true;
			this.OlvQuotes.UseTranslucentSelection = true;
			this.OlvQuotes.View = System.Windows.Forms.View.Details;
			// 
			// olvcQuotesSymbolClass
			// 
			this.olvcQuotesSymbolClass.Text = "Class";
			this.olvcQuotesSymbolClass.Width = 40;
			// 
			// olvcQuotesSymbol
			// 
			this.olvcQuotesSymbol.Text = "Symbol";
			this.olvcQuotesSymbol.Width = 75;
			// 
			// olvcQuotesServerTime
			// 
			this.olvcQuotesServerTime.Text = "Time";
			this.olvcQuotesServerTime.Width = 120;
			// 
			// olvcQuotesQty
			// 
			this.olvcQuotesQty.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesQty.Text = "Qty";
			this.olvcQuotesQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesQty.Width = 40;
			// 
			// olvcQuotesBid
			// 
			this.olvcQuotesBid.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesBid.Text = "Bid";
			this.olvcQuotesBid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesBid.Width = 65;
			// 
			// olvcQuotesAsk
			// 
			this.olvcQuotesAsk.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesAsk.Text = "Ask";
			this.olvcQuotesAsk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesAsk.Width = 65;
			// 
			// olvcQuotesLast
			// 
			this.olvcQuotesLast.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesLast.Text = "Last";
			this.olvcQuotesLast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesLast.Width = 65;
			// 
			// olvcQuotesFortsDepositBuy
			// 
			this.olvcQuotesFortsDepositBuy.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsDepositBuy.Text = "FortsRepoBuy";
			this.olvcQuotesFortsDepositBuy.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsDepositBuy.Width = 85;
			// 
			// olvcQuotesFortsDepositSell
			// 
			this.olvcQuotesFortsDepositSell.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsDepositSell.Text = "FortsRepoSell";
			this.olvcQuotesFortsDepositSell.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsDepositSell.Width = 85;
			// 
			// olvcQuotesFortsPriceMin
			// 
			this.olvcQuotesFortsPriceMin.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsPriceMin.Text = "PriceMin";
			this.olvcQuotesFortsPriceMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsPriceMin.Width = 65;
			// 
			// olvcQuotesFortsPriceMax
			// 
			this.olvcQuotesFortsPriceMax.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsPriceMax.Text = "PriceMax";
			this.olvcQuotesFortsPriceMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcQuotesFortsPriceMax.Width = 65;
			// 
			// olvcQuotesAbsnoPerSymbol
			// 
			this.olvcQuotesAbsnoPerSymbol.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuotesAbsnoPerSymbol.Text = "Absno/Symbol";
			this.olvcQuotesAbsnoPerSymbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuotesAbsnoPerSymbol.Width = 52;
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
			this.splitContainer2.Panel1.Controls.Add(this.grpDom);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel2.Controls.Add(this.grpTrades);
			this.splitContainer2.Size = new System.Drawing.Size(856, 330);
			this.splitContainer2.SplitterDistance = 214;
			this.splitContainer2.TabIndex = 0;
			// 
			// grpDom
			// 
			this.grpDom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpDom.Controls.Add(this.flpDoms);
			this.grpDom.Location = new System.Drawing.Point(9, 3);
			this.grpDom.Margin = new System.Windows.Forms.Padding(9, 3, 9, 9);
			this.grpDom.Name = "grpDom";
			this.grpDom.Size = new System.Drawing.Size(838, 202);
			this.grpDom.TabIndex = 0;
			this.grpDom.TabStop = false;
			this.grpDom.Text = "Depths Of Market [Sq1-RIM3-dom]:3213 [Sq1-LKOH-dom]:5116";
			// 
			// flpDoms
			// 
			this.flpDoms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flpDoms.AutoScroll = true;
			this.flpDoms.BackColor = System.Drawing.SystemColors.Control;
			this.flpDoms.Location = new System.Drawing.Point(6, 19);
			this.flpDoms.Name = "flpDoms";
			this.flpDoms.Size = new System.Drawing.Size(826, 177);
			this.flpDoms.TabIndex = 3;
			// 
			// grpTrades
			// 
			this.grpTrades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpTrades.Controls.Add(this.olvTrades);
			this.grpTrades.Location = new System.Drawing.Point(9, 3);
			this.grpTrades.Margin = new System.Windows.Forms.Padding(9, 3, 9, 9);
			this.grpTrades.Name = "grpTrades";
			this.grpTrades.Size = new System.Drawing.Size(838, 100);
			this.grpTrades.TabIndex = 0;
			this.grpTrades.TabStop = false;
			this.grpTrades.Text = "Trades [Sq1-trades]:41441";
			// 
			// olvTrades
			// 
			this.olvTrades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.olvTrades.Location = new System.Drawing.Point(6, 19);
			this.olvTrades.Name = "olvTrades";
			this.olvTrades.Size = new System.Drawing.Size(826, 75);
			this.olvTrades.TabIndex = 3;
			this.olvTrades.UseCompatibleStateImageBehavior = false;
			this.olvTrades.View = System.Windows.Forms.View.Details;
			// 
			// QuikStreamingMonitorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.splitContainer1);
			this.Name = "QuikStreamingMonitorControl";
			this.Size = new System.Drawing.Size(856, 474);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.grpQuotes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.OlvQuotes)).EndInit();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.grpDom.ResumeLayout(false);
			this.grpTrades.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvTrades)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		public BrightIdeasSoftware.ObjectListView OlvQuotes;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.FlowLayoutPanel flpDoms;
		private BrightIdeasSoftware.ObjectListView olvTrades;
		private BrightIdeasSoftware.OLVColumn olvcQuotesSymbolClass;
		private BrightIdeasSoftware.OLVColumn olvcQuotesSymbol;
		private BrightIdeasSoftware.OLVColumn olvcQuotesServerTime;
		private BrightIdeasSoftware.OLVColumn olvcQuotesBid;
		private BrightIdeasSoftware.OLVColumn olvcQuotesAsk;
		private BrightIdeasSoftware.OLVColumn olvcQuotesLast;
		private BrightIdeasSoftware.OLVColumn olvcQuotesQty;
		private BrightIdeasSoftware.OLVColumn olvcQuotesFortsDepositBuy;
		private BrightIdeasSoftware.OLVColumn olvcQuotesFortsDepositSell;
		private BrightIdeasSoftware.OLVColumn olvcQuotesFortsPriceMin;
		private BrightIdeasSoftware.OLVColumn olvcQuotesFortsPriceMax;
		private BrightIdeasSoftware.OLVColumn olvcQuotesAbsnoPerSymbol;
		public System.Windows.Forms.GroupBox grpQuotes;
		public System.Windows.Forms.GroupBox grpDom;
		public System.Windows.Forms.GroupBox grpTrades;
	}
}
