namespace Sq1.Widgets.SymbolEditor {
	partial class SymbolEditorControl {
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
			this.components = new System.ComponentModel.Container();
			this.olvSymbols = new BrightIdeasSoftware.ObjectListView();
			this.olvcSecurityType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbolClass = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceStep = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceDecimals = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcVolumeDecimals = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSameBarPolarCloseThenOpen = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSequencedOpeningAfterClosedDelayMillis = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEmergencyCloseDelayMillis = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEmergencyCloseAttemptsMax = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReSubmitRejected = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReSubmittedUsesNextSlippage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcUseFirstSlippageForBacktest = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippagesBuy = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippagesSell = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMarketOrderAs = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReplaceTidalWithCrossMarket = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReplaceTidalMillis = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSimBugOutOfBarStopsFill = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSimBugOutOfBarLimitsFill = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLeverageForFutures = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPoint2Dollar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxEachSymbolInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniDuplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.mniDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.mniAppend = new System.Windows.Forms.ToolStripMenuItem();
			this.mniEdit = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olvSymbols)).BeginInit();
			this.ctxEachSymbolInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvSymbols
			// 
			this.olvSymbols.AllColumns.Add(this.olvcSecurityType);
			this.olvSymbols.AllColumns.Add(this.olvcSymbolClass);
			this.olvSymbols.AllColumns.Add(this.olvcSymbol);
			this.olvSymbols.AllColumns.Add(this.olvcPriceStep);
			this.olvSymbols.AllColumns.Add(this.olvcPriceDecimals);
			this.olvSymbols.AllColumns.Add(this.olvcVolumeDecimals);
			this.olvSymbols.AllColumns.Add(this.olvcSameBarPolarCloseThenOpen);
			this.olvSymbols.AllColumns.Add(this.olvcSequencedOpeningAfterClosedDelayMillis);
			this.olvSymbols.AllColumns.Add(this.olvcEmergencyCloseDelayMillis);
			this.olvSymbols.AllColumns.Add(this.olvcEmergencyCloseAttemptsMax);
			this.olvSymbols.AllColumns.Add(this.olvcReSubmitRejected);
			this.olvSymbols.AllColumns.Add(this.olvcReSubmittedUsesNextSlippage);
			this.olvSymbols.AllColumns.Add(this.olvcUseFirstSlippageForBacktest);
			this.olvSymbols.AllColumns.Add(this.olvcSlippagesBuy);
			this.olvSymbols.AllColumns.Add(this.olvcSlippagesSell);
			this.olvSymbols.AllColumns.Add(this.olvcMarketOrderAs);
			this.olvSymbols.AllColumns.Add(this.olvcReplaceTidalWithCrossMarket);
			this.olvSymbols.AllColumns.Add(this.olvcReplaceTidalMillis);
			this.olvSymbols.AllColumns.Add(this.olvcSimBugOutOfBarStopsFill);
			this.olvSymbols.AllColumns.Add(this.olvcSimBugOutOfBarLimitsFill);
			this.olvSymbols.AllColumns.Add(this.olvcLeverageForFutures);
			this.olvSymbols.AllColumns.Add(this.olvcPoint2Dollar);
			this.olvSymbols.AllowColumnReorder = true;
			this.olvSymbols.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvSymbols.CausesValidation = false;
			this.olvSymbols.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvSymbols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcSecurityType,
            this.olvcSymbolClass,
            this.olvcSymbol,
            this.olvcPriceStep,
            this.olvcPriceDecimals,
            this.olvcVolumeDecimals,
            this.olvcSameBarPolarCloseThenOpen,
            this.olvcSequencedOpeningAfterClosedDelayMillis,
            this.olvcEmergencyCloseDelayMillis,
            this.olvcEmergencyCloseAttemptsMax,
            this.olvcReSubmitRejected,
            this.olvcReSubmittedUsesNextSlippage,
            this.olvcUseFirstSlippageForBacktest,
            this.olvcSlippagesBuy,
            this.olvcSlippagesSell,
            this.olvcMarketOrderAs,
            this.olvcReplaceTidalWithCrossMarket,
            this.olvcReplaceTidalMillis,
            this.olvcSimBugOutOfBarStopsFill,
            this.olvcSimBugOutOfBarLimitsFill,
            this.olvcLeverageForFutures,
            this.olvcPoint2Dollar});
			this.olvSymbols.ContextMenuStrip = this.ctxEachSymbolInfo;
			this.olvSymbols.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvSymbols.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvSymbols.FullRowSelect = true;
			this.olvSymbols.HeaderWordWrap = true;
			this.olvSymbols.HideSelection = false;
			this.olvSymbols.IncludeColumnHeadersInCopy = true;
			this.olvSymbols.IncludeHiddenColumnsInDataTransfer = true;
			this.olvSymbols.Location = new System.Drawing.Point(0, 0);
			this.olvSymbols.Name = "olvSymbols";
			this.olvSymbols.ShowCommandMenuOnRightClick = true;
			this.olvSymbols.ShowGroups = false;
			this.olvSymbols.ShowItemToolTips = true;
			this.olvSymbols.Size = new System.Drawing.Size(743, 392);
			this.olvSymbols.TabIndex = 1;
			this.olvSymbols.TintSortColumn = true;
			this.olvSymbols.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvSymbols.UseCompatibleStateImageBehavior = false;
			this.olvSymbols.UseFilterIndicator = true;
			this.olvSymbols.UseFiltering = true;
			this.olvSymbols.UseHotItem = true;
			this.olvSymbols.UseTranslucentHotItem = true;
			this.olvSymbols.View = System.Windows.Forms.View.Details;
			// 
			// olvcSecurityType
			// 
			this.olvcSecurityType.Text = "Security Type";
			// 
			// olvcSymbolClass
			// 
			this.olvcSymbolClass.Text = "Symbol Class";
			// 
			// olvcSymbol
			// 
			this.olvcSymbol.Text = "Symbol";
			// 
			// olvcPriceStep
			// 
			this.olvcPriceStep.Text = "Price Step";
			// 
			// olvcPriceDecimals
			// 
			this.olvcPriceDecimals.Text = "Price Decimals";
			// 
			// olvcVolumeDecimals
			// 
			this.olvcVolumeDecimals.Text = "Volume Decimals";
			// 
			// olvcSameBarPolarCloseThenOpen
			// 
			this.olvcSameBarPolarCloseThenOpen.CheckBoxes = true;
			this.olvcSameBarPolarCloseThenOpen.Text = "Same Bar Polar Close Then Open";
			// 
			// olvcSequencedOpeningAfterClosedDelayMillis
			// 
			this.olvcSequencedOpeningAfterClosedDelayMillis.Text = "Sequenced Opening After Closed Delay Millis";
			// 
			// olvcEmergencyCloseDelayMillis
			// 
			this.olvcEmergencyCloseDelayMillis.Text = "Emergency Close Delay Millis";
			// 
			// olvcEmergencyCloseAttemptsMax
			// 
			this.olvcEmergencyCloseAttemptsMax.Text = "Emergency Close Attempts Max";
			// 
			// olvcReSubmitRejected
			// 
			this.olvcReSubmitRejected.Text = "ReSubmit Rejected";
			// 
			// olvcReSubmittedUsesNextSlippage
			// 
			this.olvcReSubmittedUsesNextSlippage.Text = "ReSubmitted Uses Next Slippage";
			// 
			// olvcUseFirstSlippageForBacktest
			// 
			this.olvcUseFirstSlippageForBacktest.Text = "Use First Slippage For Backtest";
			// 
			// olvcSlippagesBuy
			// 
			this.olvcSlippagesBuy.Text = "Buy Slippages";
			// 
			// olvcSlippagesSell
			// 
			this.olvcSlippagesSell.Text = "Slippages Sell";
			// 
			// olvcMarketOrderAs
			// 
			this.olvcMarketOrderAs.Text = "Market Order As";
			// 
			// olvcReplaceTidalWithCrossMarket
			// 
			this.olvcReplaceTidalWithCrossMarket.Text = "Replace Tidal With Cross Market";
			// 
			// olvcReplaceTidalMillis
			// 
			this.olvcReplaceTidalMillis.Text = "Replace Tidal Millis";
			// 
			// olvcSimBugOutOfBarStopsFill
			// 
			this.olvcSimBugOutOfBarStopsFill.AutoCompleteEditor = false;
			this.olvcSimBugOutOfBarStopsFill.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
			this.olvcSimBugOutOfBarStopsFill.CheckBoxes = true;
			this.olvcSimBugOutOfBarStopsFill.Text = "Sim Bug Out Of Bar Stops Fill";
			// 
			// olvcSimBugOutOfBarLimitsFill
			// 
			this.olvcSimBugOutOfBarLimitsFill.Text = "Sim Bug Out Of Bar Limits Fill";
			// 
			// olvcLeverageForFutures
			// 
			this.olvcLeverageForFutures.Text = "Leverage For Futures";
			// 
			// olvcPoint2Dollar
			// 
			this.olvcPoint2Dollar.Text = "Point 2 Dollar";
			// 
			// ctxEachSymbolInfo
			// 
			this.ctxEachSymbolInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniEdit,
            this.mniDuplicate,
            this.mniAppend,
            this.mniDelete});
			this.ctxEachSymbolInfo.Name = "ctxEachSymbolInfo";
			this.ctxEachSymbolInfo.Size = new System.Drawing.Size(146, 92);
			// 
			// mniDuplicate
			// 
			this.mniDuplicate.Name = "mniDuplicate";
			this.mniDuplicate.Size = new System.Drawing.Size(145, 22);
			this.mniDuplicate.Text = "Duplicate";
			// 
			// mniDelete
			// 
			this.mniDelete.Name = "mniDelete";
			this.mniDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniDelete.Size = new System.Drawing.Size(145, 22);
			this.mniDelete.Text = "Delete";
			// 
			// mniAppend
			// 
			this.mniAppend.Name = "mniAppend";
			this.mniAppend.ShortcutKeys = System.Windows.Forms.Keys.Insert;
			this.mniAppend.Size = new System.Drawing.Size(145, 22);
			this.mniAppend.Text = "Add New";
			// 
			// mniEdit
			// 
			this.mniEdit.Name = "mniEdit";
			this.mniEdit.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mniEdit.Size = new System.Drawing.Size(145, 22);
			this.mniEdit.Text = "Edit";
			// 
			// SymbolEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvSymbols);
			this.Name = "SymbolEditorControl";
			this.Size = new System.Drawing.Size(743, 392);
			((System.ComponentModel.ISupportInitialize)(this.olvSymbols)).EndInit();
			this.ctxEachSymbolInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView olvSymbols;
		private BrightIdeasSoftware.OLVColumn olvcSecurityType;
		private BrightIdeasSoftware.OLVColumn olvcSymbolClass;
		private BrightIdeasSoftware.OLVColumn olvcSymbol;
		private BrightIdeasSoftware.OLVColumn olvcPriceStep;
		private BrightIdeasSoftware.OLVColumn olvcPriceDecimals;
		private BrightIdeasSoftware.OLVColumn olvcVolumeDecimals;
		private BrightIdeasSoftware.OLVColumn olvcSameBarPolarCloseThenOpen;
		private BrightIdeasSoftware.OLVColumn olvcSequencedOpeningAfterClosedDelayMillis;
		private BrightIdeasSoftware.OLVColumn olvcEmergencyCloseDelayMillis;
		private BrightIdeasSoftware.OLVColumn olvcEmergencyCloseAttemptsMax;
		private BrightIdeasSoftware.OLVColumn olvcReSubmitRejected;
		private BrightIdeasSoftware.OLVColumn olvcReSubmittedUsesNextSlippage;
		private BrightIdeasSoftware.OLVColumn olvcUseFirstSlippageForBacktest;
		private BrightIdeasSoftware.OLVColumn olvcSlippagesSell;
		private BrightIdeasSoftware.OLVColumn olvcMarketOrderAs;
		private BrightIdeasSoftware.OLVColumn olvcReplaceTidalWithCrossMarket;
		private BrightIdeasSoftware.OLVColumn olvcReplaceTidalMillis;
		private BrightIdeasSoftware.OLVColumn olvcSimBugOutOfBarStopsFill;
		private BrightIdeasSoftware.OLVColumn olvcSimBugOutOfBarLimitsFill;
		private BrightIdeasSoftware.OLVColumn olvcSlippagesBuy;
		private BrightIdeasSoftware.OLVColumn olvcLeverageForFutures;
		private BrightIdeasSoftware.OLVColumn olvcPoint2Dollar;
		private System.Windows.Forms.ContextMenuStrip ctxEachSymbolInfo;
		private System.Windows.Forms.ToolStripMenuItem mniEdit;
		private System.Windows.Forms.ToolStripMenuItem mniDuplicate;
		private System.Windows.Forms.ToolStripMenuItem mniAppend;
		private System.Windows.Forms.ToolStripMenuItem mniDelete;
	}
}
