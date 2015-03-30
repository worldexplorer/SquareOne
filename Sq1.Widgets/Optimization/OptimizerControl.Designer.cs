using System;

namespace Sq1.Widgets.Optimization {
	partial class OptimizerControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing){
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
			this.olvHistory = new BrightIdeasSoftware.FastObjectListView();
			this.olvcPFavg = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistorySymbolScaleRange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistoryDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistorySize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.fastOLVparametersYesNoMinMaxStep = new BrightIdeasSoftware.FastObjectListView();
			this.olvcParamName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamNumberOfRuns = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueMin = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueCurrent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueMax = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamStep = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamWillBeSequenced = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.lblStaleReason = new System.Windows.Forms.Label();
			this.txtStaleReason = new System.Windows.Forms.TextBox();
			this.btnPauseResume = new System.Windows.Forms.Button();
			this.nudThreadsToRun = new System.Windows.Forms.NumericUpDown();
			this.lblIndicatorParameterTotalNr = new System.Windows.Forms.Label();
			this.txtIndicatorParameterTotalNr = new System.Windows.Forms.TextBox();
			this.txtScriptParameterTotalNr = new System.Windows.Forms.TextBox();
			this.lblScriptParameterTotalNr = new System.Windows.Forms.Label();
			this.txtPositionSize = new System.Windows.Forms.TextBox();
			this.txtDataRange = new System.Windows.Forms.TextBox();
			this.txtSymbol = new System.Windows.Forms.TextBox();
			this.txtStrategy = new System.Windows.Forms.TextBox();
			this.lblPositionSize = new System.Windows.Forms.Label();
			this.lblThreadsToRun = new System.Windows.Forms.Label();
			this.lblDataRange = new System.Windows.Forms.Label();
			this.lblSymbol = new System.Windows.Forms.Label();
			this.lblStrategy = new System.Windows.Forms.Label();
			this.btnRunCancel = new System.Windows.Forms.Button();
			this.lblStats = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.olvBacktests = new BrightIdeasSoftware.ObjectListView();
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTotalTrades = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcAverageProfit = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcNetProfit = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcWinLoss = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitFactor = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcRecoveryFactor = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxDrawdown = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveWinners = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveLosers = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxOneBacktestResult = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCopyToDefaultCtxBacktest = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopyToDefaultCtx = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbCopyToNewContextBacktest = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbCopyToNewContext = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSaveCsv = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvHistory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fastOLVparametersYesNoMinMaxStep)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudThreadsToRun)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.olvBacktests)).BeginInit();
			this.ctxOneBacktestResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel1.Controls.Add(this.olvHistory);
			this.splitContainer1.Panel1.Controls.Add(this.fastOLVparametersYesNoMinMaxStep);
			this.splitContainer1.Panel1.Controls.Add(this.lblStaleReason);
			this.splitContainer1.Panel1.Controls.Add(this.txtStaleReason);
			this.splitContainer1.Panel1.Controls.Add(this.btnPauseResume);
			this.splitContainer1.Panel1.Controls.Add(this.nudThreadsToRun);
			this.splitContainer1.Panel1.Controls.Add(this.lblIndicatorParameterTotalNr);
			this.splitContainer1.Panel1.Controls.Add(this.txtIndicatorParameterTotalNr);
			this.splitContainer1.Panel1.Controls.Add(this.txtScriptParameterTotalNr);
			this.splitContainer1.Panel1.Controls.Add(this.lblScriptParameterTotalNr);
			this.splitContainer1.Panel1.Controls.Add(this.txtPositionSize);
			this.splitContainer1.Panel1.Controls.Add(this.txtDataRange);
			this.splitContainer1.Panel1.Controls.Add(this.txtSymbol);
			this.splitContainer1.Panel1.Controls.Add(this.txtStrategy);
			this.splitContainer1.Panel1.Controls.Add(this.lblPositionSize);
			this.splitContainer1.Panel1.Controls.Add(this.lblThreadsToRun);
			this.splitContainer1.Panel1.Controls.Add(this.lblDataRange);
			this.splitContainer1.Panel1.Controls.Add(this.lblSymbol);
			this.splitContainer1.Panel1.Controls.Add(this.lblStrategy);
			this.splitContainer1.Panel1.Controls.Add(this.btnRunCancel);
			this.splitContainer1.Panel1.Controls.Add(this.lblStats);
			this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
			this.splitContainer1.Panel1MinSize = 27;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.olvBacktests);
			this.splitContainer1.Size = new System.Drawing.Size(633, 473);
			this.splitContainer1.SplitterDistance = 220;
			this.splitContainer1.TabIndex = 0;
			// 
			// olvHistory
			// 
			this.olvHistory.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvHistory.AllColumns.Add(this.olvcPFavg);
			this.olvHistory.AllColumns.Add(this.olvcHistorySymbolScaleRange);
			this.olvHistory.AllColumns.Add(this.olvcHistoryDate);
			this.olvHistory.AllColumns.Add(this.olvcHistorySize);
			this.olvHistory.AllowColumnReorder = true;
			this.olvHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.olvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvHistory.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvHistory.CellEditEnterChangesRows = true;
			this.olvHistory.CellEditTabChangesRows = true;
			this.olvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcPFavg,
            this.olvcHistorySymbolScaleRange});
			this.olvHistory.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvHistory.EmptyListMsg = "OPTIMIZATION_HISTORY_IS_EMPTY Never optimized since last script recompilation";
			this.olvHistory.EmptyListMsgFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvHistory.FullRowSelect = true;
			this.olvHistory.HideSelection = false;
			this.olvHistory.IncludeColumnHeadersInCopy = true;
			this.olvHistory.IncludeHiddenColumnsInDataTransfer = true;
			this.olvHistory.Location = new System.Drawing.Point(413, 54);
			this.olvHistory.Name = "olvHistory";
			this.olvHistory.ShowCommandMenuOnRightClick = true;
			this.olvHistory.ShowGroups = false;
			this.olvHistory.ShowImagesOnSubItems = true;
			this.olvHistory.ShowItemCountOnGroups = true;
			this.olvHistory.ShowItemToolTips = true;
			this.olvHistory.Size = new System.Drawing.Size(215, 167);
			this.olvHistory.TabIndex = 42;
			this.olvHistory.TintSortColumn = true;
			this.olvHistory.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.HotTrack;
			this.olvHistory.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.olvHistory.UseCellFormatEvents = true;
			this.olvHistory.UseCompatibleStateImageBehavior = false;
			this.olvHistory.UseCustomSelectionColors = true;
			this.olvHistory.UseFilterIndicator = true;
			this.olvHistory.UseFiltering = true;
			this.olvHistory.UseHotItem = true;
			this.olvHistory.UseTranslucentHotItem = true;
			this.olvHistory.View = System.Windows.Forms.View.Details;
			this.olvHistory.VirtualMode = true;
			this.olvHistory.ItemActivate += new System.EventHandler(this.olvHistory_ItemActivate);
			// 
			// olvcPFavg
			// 
			this.olvcPFavg.IsEditable = false;
			this.olvcPFavg.Text = "PFavg";
			this.olvcPFavg.ToolTipText = "Average Profit Factor among all the backtests in the optimization";
			this.olvcPFavg.Width = 45;
			// 
			// olvcHistorySymbolScaleRange
			// 
			this.olvcHistorySymbolScaleRange.FillsFreeSpace = true;
			this.olvcHistorySymbolScaleRange.Text = "Symbol Scale Range";
			this.olvcHistorySymbolScaleRange.ToolTipText = "Script Context settings taken at the moment of optimization";
			this.olvcHistorySymbolScaleRange.Width = 160;
			// 
			// olvcHistoryDate
			// 
			this.olvcHistoryDate.DisplayIndex = 2;
			this.olvcHistoryDate.IsEditable = false;
			this.olvcHistoryDate.IsVisible = false;
			this.olvcHistoryDate.Text = "Modified";
			this.olvcHistoryDate.ToolTipText = "Reminder when you did it";
			this.olvcHistoryDate.Width = 55;
			// 
			// olvcHistorySize
			// 
			this.olvcHistorySize.DisplayIndex = 3;
			this.olvcHistorySize.IsEditable = false;
			this.olvcHistorySize.IsVisible = false;
			this.olvcHistorySize.Text = "Size";
			this.olvcHistorySize.ToolTipText = "JSON file size";
			this.olvcHistorySize.Width = 55;
			// 
			// fastOLVparametersYesNoMinMaxStep
			// 
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamName);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamNumberOfRuns);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamValueMin);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamValueCurrent);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamValueMax);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamStep);
			this.fastOLVparametersYesNoMinMaxStep.AllColumns.Add(this.olvcParamWillBeSequenced);
			this.fastOLVparametersYesNoMinMaxStep.AllowColumnReorder = true;
			this.fastOLVparametersYesNoMinMaxStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.fastOLVparametersYesNoMinMaxStep.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.fastOLVparametersYesNoMinMaxStep.CellEditEnterChangesRows = true;
			this.fastOLVparametersYesNoMinMaxStep.CellEditTabChangesRows = true;
			this.fastOLVparametersYesNoMinMaxStep.CheckBoxes = true;
			this.fastOLVparametersYesNoMinMaxStep.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcParamName,
            this.olvcParamNumberOfRuns,
            this.olvcParamValueMin,
            this.olvcParamValueCurrent,
            this.olvcParamValueMax,
            this.olvcParamStep});
			this.fastOLVparametersYesNoMinMaxStep.Cursor = System.Windows.Forms.Cursors.Default;
			this.fastOLVparametersYesNoMinMaxStep.EmptyListMsg = "NO_PARAMETERS_TO_OPTIMIZE Edit Strategy\'s Script to add parameters or indicators";
			this.fastOLVparametersYesNoMinMaxStep.EmptyListMsgFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.fastOLVparametersYesNoMinMaxStep.FullRowSelect = true;
			this.fastOLVparametersYesNoMinMaxStep.GridLines = true;
			this.fastOLVparametersYesNoMinMaxStep.HideSelection = false;
			this.fastOLVparametersYesNoMinMaxStep.IncludeColumnHeadersInCopy = true;
			this.fastOLVparametersYesNoMinMaxStep.IncludeHiddenColumnsInDataTransfer = true;
			this.fastOLVparametersYesNoMinMaxStep.Location = new System.Drawing.Point(4, 140);
			this.fastOLVparametersYesNoMinMaxStep.Name = "fastOLVparametersYesNoMinMaxStep";
			this.fastOLVparametersYesNoMinMaxStep.ShowCommandMenuOnRightClick = true;
			this.fastOLVparametersYesNoMinMaxStep.ShowGroups = false;
			this.fastOLVparametersYesNoMinMaxStep.ShowImagesOnSubItems = true;
			this.fastOLVparametersYesNoMinMaxStep.ShowItemToolTips = true;
			this.fastOLVparametersYesNoMinMaxStep.Size = new System.Drawing.Size(403, 77);
			this.fastOLVparametersYesNoMinMaxStep.TabIndex = 41;
			this.fastOLVparametersYesNoMinMaxStep.TintSortColumn = true;
			this.fastOLVparametersYesNoMinMaxStep.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.HotTrack;
			this.fastOLVparametersYesNoMinMaxStep.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.fastOLVparametersYesNoMinMaxStep.UseCompatibleStateImageBehavior = false;
			this.fastOLVparametersYesNoMinMaxStep.UseFilterIndicator = true;
			this.fastOLVparametersYesNoMinMaxStep.UseFiltering = true;
			this.fastOLVparametersYesNoMinMaxStep.UseHotItem = true;
			this.fastOLVparametersYesNoMinMaxStep.UseTranslucentHotItem = true;
			this.fastOLVparametersYesNoMinMaxStep.View = System.Windows.Forms.View.Details;
			this.fastOLVparametersYesNoMinMaxStep.VirtualMode = true;
			this.fastOLVparametersYesNoMinMaxStep.Click += new System.EventHandler(this.fastOLVparametersYesNoMinMaxStep_Click);
			// 
			// olvcParamName
			// 
			this.olvcParamName.FillsFreeSpace = true;
			this.olvcParamName.HeaderCheckBox = true;
			this.olvcParamName.Text = "Parameter";
			this.olvcParamName.Width = 120;
			// 
			// olvcParamNumberOfRuns
			// 
			this.olvcParamNumberOfRuns.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcParamNumberOfRuns.Text = "#Runs";
			this.olvcParamNumberOfRuns.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcParamNumberOfRuns.Width = 46;
			// 
			// olvcParamValueMin
			// 
			this.olvcParamValueMin.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamValueMin.Text = "Minimum";
			this.olvcParamValueMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamValueMin.Width = 55;
			// 
			// olvcParamValueCurrent
			// 
			this.olvcParamValueCurrent.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamValueCurrent.Text = "Current";
			this.olvcParamValueCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamValueCurrent.Width = 55;
			// 
			// olvcParamValueMax
			// 
			this.olvcParamValueMax.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamValueMax.Text = "Maximum";
			this.olvcParamValueMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// olvcParamStep
			// 
			this.olvcParamStep.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamStep.Text = "Step";
			this.olvcParamStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamStep.Width = 35;
			// 
			// olvcParamWillBeSequenced
			// 
			this.olvcParamWillBeSequenced.CheckBoxes = true;
			this.olvcParamWillBeSequenced.DisplayIndex = 5;
			this.olvcParamWillBeSequenced.HeaderCheckState = System.Windows.Forms.CheckState.Checked;
			this.olvcParamWillBeSequenced.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamWillBeSequenced.IsVisible = false;
			this.olvcParamWillBeSequenced.Text = "Optimize";
			this.olvcParamWillBeSequenced.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcParamWillBeSequenced.Width = 55;
			// 
			// lblStaleReason
			// 
			this.lblStaleReason.Location = new System.Drawing.Point(5, 120);
			this.lblStaleReason.Name = "lblStaleReason";
			this.lblStaleReason.Size = new System.Drawing.Size(105, 17);
			this.lblStaleReason.TabIndex = 38;
			this.lblStaleReason.Text = "Stale Reason";
			this.lblStaleReason.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtStaleReason
			// 
			this.txtStaleReason.Enabled = false;
			this.txtStaleReason.Location = new System.Drawing.Point(116, 117);
			this.txtStaleReason.Name = "txtStaleReason";
			this.txtStaleReason.Size = new System.Drawing.Size(291, 20);
			this.txtStaleReason.TabIndex = 37;
			// 
			// btnPauseResume
			// 
			this.btnPauseResume.Location = new System.Drawing.Point(4, 27);
			this.btnPauseResume.Name = "btnPauseResume";
			this.btnPauseResume.Size = new System.Drawing.Size(152, 23);
			this.btnPauseResume.TabIndex = 36;
			this.btnPauseResume.Text = "Pause/Resume";
			this.btnPauseResume.UseVisualStyleBackColor = true;
			// 
			// nudThreadsToRun
			// 
			this.nudThreadsToRun.Location = new System.Drawing.Point(116, 55);
			this.nudThreadsToRun.Name = "nudThreadsToRun";
			this.nudThreadsToRun.Size = new System.Drawing.Size(40, 20);
			this.nudThreadsToRun.TabIndex = 33;
			this.nudThreadsToRun.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.nudThreadsToRun.ValueChanged += new System.EventHandler(this.nudCpuCoresToUse_ValueChanged);
			// 
			// lblIndicatorParameterTotalNr
			// 
			this.lblIndicatorParameterTotalNr.Location = new System.Drawing.Point(5, 99);
			this.lblIndicatorParameterTotalNr.Name = "lblIndicatorParameterTotalNr";
			this.lblIndicatorParameterTotalNr.Size = new System.Drawing.Size(105, 17);
			this.lblIndicatorParameterTotalNr.TabIndex = 32;
			this.lblIndicatorParameterTotalNr.Text = "Indicator Parameters";
			this.lblIndicatorParameterTotalNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtIndicatorParameterTotalNr
			// 
			this.txtIndicatorParameterTotalNr.Enabled = false;
			this.txtIndicatorParameterTotalNr.Location = new System.Drawing.Point(116, 96);
			this.txtIndicatorParameterTotalNr.Name = "txtIndicatorParameterTotalNr";
			this.txtIndicatorParameterTotalNr.Size = new System.Drawing.Size(40, 20);
			this.txtIndicatorParameterTotalNr.TabIndex = 31;
			this.txtIndicatorParameterTotalNr.Text = "600";
			// 
			// txtScriptParameterTotalNr
			// 
			this.txtScriptParameterTotalNr.Enabled = false;
			this.txtScriptParameterTotalNr.Location = new System.Drawing.Point(116, 75);
			this.txtScriptParameterTotalNr.Name = "txtScriptParameterTotalNr";
			this.txtScriptParameterTotalNr.Size = new System.Drawing.Size(40, 20);
			this.txtScriptParameterTotalNr.TabIndex = 30;
			this.txtScriptParameterTotalNr.Text = "148";
			// 
			// lblScriptParameterTotalNr
			// 
			this.lblScriptParameterTotalNr.Location = new System.Drawing.Point(5, 78);
			this.lblScriptParameterTotalNr.Name = "lblScriptParameterTotalNr";
			this.lblScriptParameterTotalNr.Size = new System.Drawing.Size(105, 17);
			this.lblScriptParameterTotalNr.TabIndex = 29;
			this.lblScriptParameterTotalNr.Text = "Script Parameters";
			this.lblScriptParameterTotalNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtPositionSize
			// 
			this.txtPositionSize.Enabled = false;
			this.txtPositionSize.Location = new System.Drawing.Point(238, 96);
			this.txtPositionSize.Name = "txtPositionSize";
			this.txtPositionSize.Size = new System.Drawing.Size(169, 20);
			this.txtPositionSize.TabIndex = 28;
			this.txtPositionSize.Text = "SharesConstant=1";
			// 
			// txtDataRange
			// 
			this.txtDataRange.Enabled = false;
			this.txtDataRange.Location = new System.Drawing.Point(238, 75);
			this.txtDataRange.Name = "txtDataRange";
			this.txtDataRange.Size = new System.Drawing.Size(169, 20);
			this.txtDataRange.TabIndex = 27;
			this.txtDataRange.Text = "LastBars=500";
			// 
			// txtSymbol
			// 
			this.txtSymbol.Enabled = false;
			this.txtSymbol.Location = new System.Drawing.Point(238, 54);
			this.txtSymbol.Name = "txtSymbol";
			this.txtSymbol.Size = new System.Drawing.Size(169, 20);
			this.txtSymbol.TabIndex = 26;
			this.txtSymbol.Text = "MOCK :: RIM3";
			// 
			// txtStrategy
			// 
			this.txtStrategy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtStrategy.Enabled = false;
			this.txtStrategy.Location = new System.Drawing.Point(238, 33);
			this.txtStrategy.Name = "txtStrategy";
			this.txtStrategy.Size = new System.Drawing.Size(390, 20);
			this.txtStrategy.TabIndex = 25;
			this.txtStrategy.Text = "MA_ATRComplied (UserStop=1,ActivateLog=3) (ATR.Period=14,ATRband.Multiplier=1.56)" +
				"";
			// 
			// lblPositionSize
			// 
			this.lblPositionSize.Location = new System.Drawing.Point(164, 99);
			this.lblPositionSize.Name = "lblPositionSize";
			this.lblPositionSize.Size = new System.Drawing.Size(68, 16);
			this.lblPositionSize.TabIndex = 24;
			this.lblPositionSize.Text = "PositionSize";
			this.lblPositionSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblThreadsToRun
			// 
			this.lblThreadsToRun.Location = new System.Drawing.Point(5, 57);
			this.lblThreadsToRun.Name = "lblThreadsToRun";
			this.lblThreadsToRun.Size = new System.Drawing.Size(105, 16);
			this.lblThreadsToRun.TabIndex = 23;
			this.lblThreadsToRun.Text = "Use CPU Cores";
			this.lblThreadsToRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblDataRange
			// 
			this.lblDataRange.Location = new System.Drawing.Point(164, 78);
			this.lblDataRange.Name = "lblDataRange";
			this.lblDataRange.Size = new System.Drawing.Size(68, 16);
			this.lblDataRange.TabIndex = 22;
			this.lblDataRange.Text = "DataRange";
			this.lblDataRange.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblSymbol
			// 
			this.lblSymbol.Location = new System.Drawing.Point(177, 57);
			this.lblSymbol.Name = "lblSymbol";
			this.lblSymbol.Size = new System.Drawing.Size(55, 16);
			this.lblSymbol.TabIndex = 21;
			this.lblSymbol.Text = "Symbol";
			this.lblSymbol.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblStrategy
			// 
			this.lblStrategy.Location = new System.Drawing.Point(177, 36);
			this.lblStrategy.Name = "lblStrategy";
			this.lblStrategy.Size = new System.Drawing.Size(55, 17);
			this.lblStrategy.TabIndex = 20;
			this.lblStrategy.Text = "Strategy";
			this.lblStrategy.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnRunCancel
			// 
			this.btnRunCancel.Enabled = false;
			this.btnRunCancel.Location = new System.Drawing.Point(4, 2);
			this.btnRunCancel.Name = "btnRunCancel";
			this.btnRunCancel.Size = new System.Drawing.Size(152, 23);
			this.btnRunCancel.TabIndex = 19;
			this.btnRunCancel.Text = "Cancel 529832 backtests";
			this.btnRunCancel.UseVisualStyleBackColor = true;
			this.btnRunCancel.Click += new System.EventHandler(this.btnRunCancel_Click);
			// 
			// lblStats
			// 
			this.lblStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStats.Location = new System.Drawing.Point(476, 7);
			this.lblStats.Name = "lblStats";
			this.lblStats.Size = new System.Drawing.Size(150, 16);
			this.lblStats.TabIndex = 35;
			this.lblStats.Text = "48% complete   450044/18900";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(162, 6);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(308, 16);
			this.progressBar1.TabIndex = 34;
			this.progressBar1.Value = 48;
			// 
			// olvBacktests
			// 
			this.olvBacktests.AllowColumnReorder = true;
			this.olvBacktests.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvBacktests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcSerno,
            this.olvcTotalTrades,
            this.olvcAverageProfit,
            this.olvcNetProfit,
            this.olvcWinLoss,
            this.olvcProfitFactor,
            this.olvcRecoveryFactor,
            this.olvcMaxDrawdown,
            this.olvcMaxConsecutiveWinners,
            this.olvcMaxConsecutiveLosers});
			this.olvBacktests.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvBacktests.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvBacktests.EmptyListMsg = "";
			this.olvBacktests.FullRowSelect = true;
			this.olvBacktests.HideSelection = false;
			this.olvBacktests.IncludeColumnHeadersInCopy = true;
			this.olvBacktests.IncludeHiddenColumnsInDataTransfer = true;
			this.olvBacktests.Location = new System.Drawing.Point(0, 0);
			this.olvBacktests.Margin = new System.Windows.Forms.Padding(0);
			this.olvBacktests.MultiSelect = false;
			this.olvBacktests.Name = "olvBacktests";
			this.olvBacktests.ShowCommandMenuOnRightClick = true;
			this.olvBacktests.ShowGroups = false;
			this.olvBacktests.ShowItemCountOnGroups = true;
			this.olvBacktests.Size = new System.Drawing.Size(633, 249);
			this.olvBacktests.TabIndex = 0;
			this.olvBacktests.TintSortColumn = true;
			this.olvBacktests.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.HotTrack;
			this.olvBacktests.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.olvBacktests.UseCellFormatEvents = true;
			this.olvBacktests.UseCompatibleStateImageBehavior = false;
			this.olvBacktests.UseCustomSelectionColors = true;
			this.olvBacktests.UseFilterIndicator = true;
			this.olvBacktests.UseFiltering = true;
			this.olvBacktests.UseHotItem = true;
			this.olvBacktests.UseTranslucentHotItem = true;
			this.olvBacktests.View = System.Windows.Forms.View.Details;
			this.olvBacktests.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.olvBacktests_CellClick);
			this.olvBacktests.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.olvBacktests_CellRightClick);
			// 
			// olvcSerno
			// 
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Text = "#";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.ToolTipText = "Backtest\'s serial number";
			this.olvcSerno.Width = 25;
			// 
			// olvcTotalTrades
			// 
			this.olvcTotalTrades.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalTrades.Text = "#Pos";
			this.olvcTotalTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalTrades.ToolTipText = "TotalPositions generated";
			this.olvcTotalTrades.Width = 41;
			// 
			// olvcAverageProfit
			// 
			this.olvcAverageProfit.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcAverageProfit.Text = "Avg";
			this.olvcAverageProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcAverageProfit.ToolTipText = "AverageProfit per Position Closed";
			this.olvcAverageProfit.Width = 50;
			// 
			// olvcNetProfit
			// 
			this.olvcNetProfit.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfit.Text = "Net";
			this.olvcNetProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfit.ToolTipText = "NetProfit";
			this.olvcNetProfit.Width = 63;
			// 
			// olvcWinLoss
			// 
			this.olvcWinLoss.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLoss.Text = "WL";
			this.olvcWinLoss.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLoss.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss";
			this.olvcWinLoss.Width = 35;
			// 
			// olvcProfitFactor
			// 
			this.olvcProfitFactor.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactor.Text = "PF";
			this.olvcProfitFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactor.ToolTipText = "ProfitFactor = total$won / total$lost";
			this.olvcProfitFactor.Width = 32;
			// 
			// olvcRecoveryFactor
			// 
			this.olvcRecoveryFactor.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactor.Text = "RF";
			this.olvcRecoveryFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactor.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown";
			this.olvcRecoveryFactor.Width = 32;
			// 
			// olvcMaxDrawdown
			// 
			this.olvcMaxDrawdown.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdown.Text = "DD";
			this.olvcMaxDrawdown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdown.ToolTipText = "MaxDrawdown, $";
			this.olvcMaxDrawdown.Width = 63;
			// 
			// olvcMaxConsecutiveWinners
			// 
			this.olvcMaxConsecutiveWinners.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinners.Text = "CW";
			this.olvcMaxConsecutiveWinners.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinners.ToolTipText = "MaxConsecutiveWinners";
			this.olvcMaxConsecutiveWinners.Width = 30;
			// 
			// olvcMaxConsecutiveLosers
			// 
			this.olvcMaxConsecutiveLosers.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosers.Text = "CL";
			this.olvcMaxConsecutiveLosers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosers.ToolTipText = "MaxConsecutiveLosers";
			this.olvcMaxConsecutiveLosers.Width = 30;
			// 
			// ctxOneBacktestResult
			// 
			this.ctxOneBacktestResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniInfo,
            this.toolStripSeparator1,
            this.mniCopyToDefaultCtxBacktest,
            this.mniCopyToDefaultCtx,
            this.mniltbCopyToNewContextBacktest,
            this.mniltbCopyToNewContext,
            this.toolStripSeparator2,
            this.mniCopyToClipboard,
            this.mniSaveCsv});
			this.ctxOneBacktestResult.Name = "ctxOneBacktestResult";
			this.ctxOneBacktestResult.Size = new System.Drawing.Size(444, 176);
			this.ctxOneBacktestResult.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOneBacktestResult_Opening);
			// 
			// mniInfo
			// 
			this.mniInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniInfo.Name = "mniInfo";
			this.mniInfo.Size = new System.Drawing.Size(443, 22);
			this.mniInfo.Text = "Net(-33,5415.00)PF(2.3)RF(5.6) > MA_ATRcompiled-DLL aaa";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(440, 6);
			// 
			// mniCopyToDefaultCtxBacktest
			// 
			this.mniCopyToDefaultCtxBacktest.Name = "mniCopyToDefaultCtxBacktest";
			this.mniCopyToDefaultCtxBacktest.Size = new System.Drawing.Size(443, 22);
			this.mniCopyToDefaultCtxBacktest.Text = "Copy To Default Context, Backtest";
			this.mniCopyToDefaultCtxBacktest.Click += new System.EventHandler(this.mniCopyToDefaultCtxBacktest_Click);
			// 
			// mniCopyToDefaultCtx
			// 
			this.mniCopyToDefaultCtx.Name = "mniCopyToDefaultCtx";
			this.mniCopyToDefaultCtx.Size = new System.Drawing.Size(443, 22);
			this.mniCopyToDefaultCtx.Text = "Copy To Default Context";
			this.mniCopyToDefaultCtx.Click += new System.EventHandler(this.mniCopyToDefaultCtx_Click);
			// 
			// mniltbCopyToNewContextBacktest
			// 
			this.mniltbCopyToNewContextBacktest.BackColor = System.Drawing.Color.Transparent;
			this.mniltbCopyToNewContextBacktest.InputFieldAlignedRight = false;
			this.mniltbCopyToNewContextBacktest.InputFieldEditable = true;
			this.mniltbCopyToNewContextBacktest.InputFieldOffsetX = 180;
			this.mniltbCopyToNewContextBacktest.InputFieldValue = "";
			this.mniltbCopyToNewContextBacktest.InputFieldWidth = 200;
			this.mniltbCopyToNewContextBacktest.Name = "mniltbCopyToNewContextBacktest";
			this.mniltbCopyToNewContextBacktest.Size = new System.Drawing.Size(383, 22);
			this.mniltbCopyToNewContextBacktest.Text = "Copy To New Context, Backtest:";
			this.mniltbCopyToNewContextBacktest.TextOffsetX = 0;
			this.mniltbCopyToNewContextBacktest.TextRed = false;
			this.mniltbCopyToNewContextBacktest.TextWidth = 164;
			this.mniltbCopyToNewContextBacktest.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbCopyToNewContextBacktest_UserTyped);
			// 
			// mniltbCopyToNewContext
			// 
			this.mniltbCopyToNewContext.BackColor = System.Drawing.Color.Transparent;
			this.mniltbCopyToNewContext.InputFieldAlignedRight = false;
			this.mniltbCopyToNewContext.InputFieldEditable = true;
			this.mniltbCopyToNewContext.InputFieldOffsetX = 180;
			this.mniltbCopyToNewContext.InputFieldValue = "";
			this.mniltbCopyToNewContext.InputFieldWidth = 200;
			this.mniltbCopyToNewContext.Name = "mniltbCopyToNewContext";
			this.mniltbCopyToNewContext.Size = new System.Drawing.Size(383, 22);
			this.mniltbCopyToNewContext.Text = "Copy To New Context:";
			this.mniltbCopyToNewContext.TextOffsetX = 0;
			this.mniltbCopyToNewContext.TextRed = false;
			this.mniltbCopyToNewContext.TextWidth = 116;
			this.mniltbCopyToNewContext.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbCopyToNewContext_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(440, 6);
			// 
			// mniCopyToClipboard
			// 
			this.mniCopyToClipboard.Enabled = false;
			this.mniCopyToClipboard.Name = "mniCopyToClipboard";
			this.mniCopyToClipboard.Size = new System.Drawing.Size(443, 22);
			this.mniCopyToClipboard.Text = "Copy To Clipboard (Paste-able to Excel)";
			this.mniCopyToClipboard.Click += new System.EventHandler(this.mniCopyToClipboard_Click);
			// 
			// mniSaveCsv
			// 
			this.mniSaveCsv.Enabled = false;
			this.mniSaveCsv.Name = "mniSaveCsv";
			this.mniSaveCsv.Size = new System.Drawing.Size(443, 22);
			this.mniSaveCsv.Text = "Save as CSV...";
			this.mniSaveCsv.Click += new System.EventHandler(this.mniSaveCsv_Click);
			// 
			// OptimizerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.splitContainer1);
			this.Name = "OptimizerControl";
			this.Size = new System.Drawing.Size(633, 473);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvHistory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fastOLVparametersYesNoMinMaxStep)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudThreadsToRun)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.olvBacktests)).EndInit();
			this.ctxOneBacktestResult.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.TextBox txtStaleReason;
		private System.Windows.Forms.Label lblStaleReason;

		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbCopyToNewContext;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbCopyToNewContextBacktest;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToDefaultCtx;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToDefaultCtxBacktest;
		private BrightIdeasSoftware.OLVColumn olvcSerno;
		private BrightIdeasSoftware.OLVColumn olvcAverageProfit;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveLosers;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveWinners;
		private BrightIdeasSoftware.OLVColumn olvcMaxDrawdown;
		private BrightIdeasSoftware.OLVColumn olvcTotalTrades;
		private BrightIdeasSoftware.OLVColumn olvcRecoveryFactor;
		private BrightIdeasSoftware.OLVColumn olvcProfitFactor;
		private BrightIdeasSoftware.OLVColumn olvcWinLoss;
		private BrightIdeasSoftware.OLVColumn olvcNetProfit;
		private System.Windows.Forms.ContextMenuStrip ctxOneBacktestResult;
		private BrightIdeasSoftware.ObjectListView olvBacktests;
		private System.Windows.Forms.Button btnPauseResume;
		private System.Windows.Forms.Button btnRunCancel;
		private System.Windows.Forms.Label lblStrategy;
		private System.Windows.Forms.Label lblSymbol;
		private System.Windows.Forms.Label lblDataRange;
		private System.Windows.Forms.Label lblThreadsToRun;
		private System.Windows.Forms.Label lblPositionSize;
		private System.Windows.Forms.TextBox txtStrategy;
		private System.Windows.Forms.TextBox txtSymbol;
		private System.Windows.Forms.TextBox txtDataRange;
		private System.Windows.Forms.TextBox txtPositionSize;
		private System.Windows.Forms.Label lblScriptParameterTotalNr;
		private System.Windows.Forms.TextBox txtScriptParameterTotalNr;
		private System.Windows.Forms.TextBox txtIndicatorParameterTotalNr;
		private System.Windows.Forms.Label lblIndicatorParameterTotalNr;
		private System.Windows.Forms.NumericUpDown nudThreadsToRun;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label lblStats;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem mniInfo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToClipboard;
		private System.Windows.Forms.ToolStripMenuItem mniSaveCsv;
		private BrightIdeasSoftware.FastObjectListView fastOLVparametersYesNoMinMaxStep;
		private BrightIdeasSoftware.OLVColumn olvcParamWillBeSequenced;
		private BrightIdeasSoftware.OLVColumn olvcParamName;
		private BrightIdeasSoftware.OLVColumn olvcParamValueMin;
		private BrightIdeasSoftware.OLVColumn olvcParamValueMax;
		private BrightIdeasSoftware.OLVColumn olvcParamStep;
		private BrightIdeasSoftware.FastObjectListView olvHistory;
		private BrightIdeasSoftware.OLVColumn olvcHistoryDate;
		private BrightIdeasSoftware.OLVColumn olvcHistorySymbolScaleRange;
		private BrightIdeasSoftware.OLVColumn olvcHistorySize;
		private BrightIdeasSoftware.OLVColumn olvcPFavg;
		private BrightIdeasSoftware.OLVColumn olvcParamValueCurrent;
		private BrightIdeasSoftware.OLVColumn olvcParamNumberOfRuns;
	}
}
