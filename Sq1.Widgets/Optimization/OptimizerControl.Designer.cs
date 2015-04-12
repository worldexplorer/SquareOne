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
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.lblQuotesGenerator = new System.Windows.Forms.Label();
			this.lblSpread = new System.Windows.Forms.Label();
			this.txtQuotesGenerator = new System.Windows.Forms.TextBox();
			this.txtSpread = new System.Windows.Forms.TextBox();
			this.olvParameters = new BrightIdeasSoftware.FastObjectListView();
			this.olvcParamName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamNumberOfRuns = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueMin = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueCurrent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamValueMax = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamStep = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcParamWillBeSequenced = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.lblDataRange = new System.Windows.Forms.Label();
			this.txtStrategy = new System.Windows.Forms.TextBox();
			this.lblSymbol = new System.Windows.Forms.Label();
			this.lblStrategy = new System.Windows.Forms.Label();
			this.nudThreadsToRun = new System.Windows.Forms.NumericUpDown();
			this.lblPositionSize = new System.Windows.Forms.Label();
			this.lblThreadsToRun = new System.Windows.Forms.Label();
			this.txtSymbol = new System.Windows.Forms.TextBox();
			this.lblIndicatorParameterTotalNr = new System.Windows.Forms.Label();
			this.txtDataRange = new System.Windows.Forms.TextBox();
			this.lblScriptParameterTotalNr = new System.Windows.Forms.Label();
			this.txtPositionSize = new System.Windows.Forms.TextBox();
			this.txtScriptParameterTotalNr = new System.Windows.Forms.TextBox();
			this.txtIndicatorParameterTotalNr = new System.Windows.Forms.TextBox();
			this.olvHistory = new BrightIdeasSoftware.FastObjectListView();
			this.olvcPFavg = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistorySymbolScaleRange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistoryDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcHistorySize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.cbxPauseResume = new System.Windows.Forms.CheckBox();
			this.cbxRunCancel = new System.Windows.Forms.CheckBox();
			this.lblStats = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.cbxExpandCollapse = new System.Windows.Forms.CheckBox();
			this.olvBacktests = new BrightIdeasSoftware.ObjectListView();
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTotalPositions = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPerPosition = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
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
			this.mni_showAllScriptIndicatorParametersInOptimizationResults = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSaveCsv = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvParameters)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudThreadsToRun)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.olvHistory)).BeginInit();
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
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1.Controls.Add(this.cbxPauseResume);
			this.splitContainer1.Panel1.Controls.Add(this.cbxRunCancel);
			this.splitContainer1.Panel1.Controls.Add(this.lblStats);
			this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
			this.splitContainer1.Panel1.Controls.Add(this.cbxExpandCollapse);
			this.splitContainer1.Panel1MinSize = 27;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.olvBacktests);
			this.splitContainer1.Size = new System.Drawing.Size(641, 373);
			this.splitContainer1.SplitterDistance = 212;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(0, 27);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel1.Controls.Add(this.lblQuotesGenerator);
			this.splitContainer2.Panel1.Controls.Add(this.lblSpread);
			this.splitContainer2.Panel1.Controls.Add(this.txtQuotesGenerator);
			this.splitContainer2.Panel1.Controls.Add(this.txtSpread);
			this.splitContainer2.Panel1.Controls.Add(this.olvParameters);
			this.splitContainer2.Panel1.Controls.Add(this.lblDataRange);
			this.splitContainer2.Panel1.Controls.Add(this.txtStrategy);
			this.splitContainer2.Panel1.Controls.Add(this.lblSymbol);
			this.splitContainer2.Panel1.Controls.Add(this.lblStrategy);
			this.splitContainer2.Panel1.Controls.Add(this.nudThreadsToRun);
			this.splitContainer2.Panel1.Controls.Add(this.lblPositionSize);
			this.splitContainer2.Panel1.Controls.Add(this.lblThreadsToRun);
			this.splitContainer2.Panel1.Controls.Add(this.txtSymbol);
			this.splitContainer2.Panel1.Controls.Add(this.lblIndicatorParameterTotalNr);
			this.splitContainer2.Panel1.Controls.Add(this.txtDataRange);
			this.splitContainer2.Panel1.Controls.Add(this.lblScriptParameterTotalNr);
			this.splitContainer2.Panel1.Controls.Add(this.txtPositionSize);
			this.splitContainer2.Panel1.Controls.Add(this.txtScriptParameterTotalNr);
			this.splitContainer2.Panel1.Controls.Add(this.txtIndicatorParameterTotalNr);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.olvHistory);
			this.splitContainer2.Size = new System.Drawing.Size(638, 186);
			this.splitContainer2.SplitterDistance = 418;
			this.splitContainer2.TabIndex = 1;
			// 
			// lblQuotesGenerator
			// 
			this.lblQuotesGenerator.Location = new System.Drawing.Point(4, 90);
			this.lblQuotesGenerator.Name = "lblQuotesGenerator";
			this.lblQuotesGenerator.Size = new System.Drawing.Size(68, 16);
			this.lblQuotesGenerator.TabIndex = 42;
			this.lblQuotesGenerator.Text = "Quotes/Bar";
			this.lblQuotesGenerator.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblSpread
			// 
			this.lblSpread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSpread.Location = new System.Drawing.Point(262, 90);
			this.lblSpread.Name = "lblSpread";
			this.lblSpread.Size = new System.Drawing.Size(105, 17);
			this.lblSpread.TabIndex = 45;
			this.lblSpread.Text = "Spread, pips";
			this.lblSpread.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtQuotesGenerator
			// 
			this.txtQuotesGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuotesGenerator.Enabled = false;
			this.txtQuotesGenerator.Location = new System.Drawing.Point(78, 87);
			this.txtQuotesGenerator.Name = "txtQuotesGenerator";
			this.txtQuotesGenerator.Size = new System.Drawing.Size(178, 20);
			this.txtQuotesGenerator.TabIndex = 43;
			this.txtQuotesGenerator.Text = "SixteenStrokes";
			// 
			// txtSpread
			// 
			this.txtSpread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSpread.Enabled = false;
			this.txtSpread.Location = new System.Drawing.Point(373, 87);
			this.txtSpread.Name = "txtSpread";
			this.txtSpread.Size = new System.Drawing.Size(40, 20);
			this.txtSpread.TabIndex = 44;
			this.txtSpread.Text = "7";
			// 
			// olvParameters
			// 
			this.olvParameters.AllColumns.Add(this.olvcParamName);
			this.olvParameters.AllColumns.Add(this.olvcParamNumberOfRuns);
			this.olvParameters.AllColumns.Add(this.olvcParamValueMin);
			this.olvParameters.AllColumns.Add(this.olvcParamValueCurrent);
			this.olvParameters.AllColumns.Add(this.olvcParamValueMax);
			this.olvParameters.AllColumns.Add(this.olvcParamStep);
			this.olvParameters.AllColumns.Add(this.olvcParamWillBeSequenced);
			this.olvParameters.AllowColumnReorder = true;
			this.olvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.olvParameters.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvParameters.CellEditEnterChangesRows = true;
			this.olvParameters.CellEditTabChangesRows = true;
			this.olvParameters.CheckBoxes = true;
			this.olvParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcParamName,
            this.olvcParamNumberOfRuns,
            this.olvcParamValueMin,
            this.olvcParamValueCurrent,
            this.olvcParamValueMax,
            this.olvcParamStep});
			this.olvParameters.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvParameters.EmptyListMsg = "NO_PARAMETERS_TO_OPTIMIZE Edit Strategy\'s Script to add parameters or indicators";
			this.olvParameters.EmptyListMsgFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvParameters.FullRowSelect = true;
			this.olvParameters.GridLines = true;
			this.olvParameters.HideSelection = false;
			this.olvParameters.HighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvParameters.HighlightForegroundColor = System.Drawing.Color.Black;
			this.olvParameters.IncludeColumnHeadersInCopy = true;
			this.olvParameters.IncludeHiddenColumnsInDataTransfer = true;
			this.olvParameters.Location = new System.Drawing.Point(0, 109);
			this.olvParameters.Name = "olvParameters";
			this.olvParameters.ShowCommandMenuOnRightClick = true;
			this.olvParameters.ShowGroups = false;
			this.olvParameters.ShowImagesOnSubItems = true;
			this.olvParameters.ShowItemToolTips = true;
			this.olvParameters.Size = new System.Drawing.Size(415, 77);
			this.olvParameters.TabIndex = 41;
			this.olvParameters.TintSortColumn = true;
			this.olvParameters.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvParameters.UnfocusedHighlightForegroundColor = System.Drawing.Color.Black;
			this.olvParameters.UseCompatibleStateImageBehavior = false;
			this.olvParameters.UseFilterIndicator = true;
			this.olvParameters.UseFiltering = true;
			this.olvParameters.UseHotItem = true;
			this.olvParameters.UseTranslucentHotItem = true;
			this.olvParameters.View = System.Windows.Forms.View.Details;
			this.olvParameters.VirtualMode = true;
			this.olvParameters.Click += new System.EventHandler(this.olvParameters_Click);
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
			// lblDataRange
			// 
			this.lblDataRange.Location = new System.Drawing.Point(5, 48);
			this.lblDataRange.Name = "lblDataRange";
			this.lblDataRange.Size = new System.Drawing.Size(68, 16);
			this.lblDataRange.TabIndex = 22;
			this.lblDataRange.Text = "DataRange";
			this.lblDataRange.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtStrategy
			// 
			this.txtStrategy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtStrategy.Enabled = false;
			this.txtStrategy.Location = new System.Drawing.Point(79, 3);
			this.txtStrategy.Name = "txtStrategy";
			this.txtStrategy.Size = new System.Drawing.Size(336, 20);
			this.txtStrategy.TabIndex = 25;
			this.txtStrategy.Text = "MA_ATRComplied (UserStop=1,ActivateLog=3) (ATR.Period=14,ATRband.Multiplier=1.56)" +
				"";
			// 
			// lblSymbol
			// 
			this.lblSymbol.Location = new System.Drawing.Point(18, 28);
			this.lblSymbol.Name = "lblSymbol";
			this.lblSymbol.Size = new System.Drawing.Size(55, 16);
			this.lblSymbol.TabIndex = 21;
			this.lblSymbol.Text = "Symbol";
			this.lblSymbol.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblStrategy
			// 
			this.lblStrategy.Location = new System.Drawing.Point(18, 6);
			this.lblStrategy.Name = "lblStrategy";
			this.lblStrategy.Size = new System.Drawing.Size(55, 16);
			this.lblStrategy.TabIndex = 20;
			this.lblStrategy.Text = "Strategy";
			this.lblStrategy.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// nudThreadsToRun
			// 
			this.nudThreadsToRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudThreadsToRun.Location = new System.Drawing.Point(374, 24);
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
			// lblPositionSize
			// 
			this.lblPositionSize.Location = new System.Drawing.Point(5, 69);
			this.lblPositionSize.Name = "lblPositionSize";
			this.lblPositionSize.Size = new System.Drawing.Size(68, 16);
			this.lblPositionSize.TabIndex = 24;
			this.lblPositionSize.Text = "PositionSize";
			this.lblPositionSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblThreadsToRun
			// 
			this.lblThreadsToRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblThreadsToRun.Location = new System.Drawing.Point(263, 27);
			this.lblThreadsToRun.Name = "lblThreadsToRun";
			this.lblThreadsToRun.Size = new System.Drawing.Size(105, 16);
			this.lblThreadsToRun.TabIndex = 23;
			this.lblThreadsToRun.Text = "Use CPU Cores";
			this.lblThreadsToRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtSymbol
			// 
			this.txtSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSymbol.Enabled = false;
			this.txtSymbol.Location = new System.Drawing.Point(79, 24);
			this.txtSymbol.Name = "txtSymbol";
			this.txtSymbol.Size = new System.Drawing.Size(178, 20);
			this.txtSymbol.TabIndex = 26;
			this.txtSymbol.Text = "MOCK :: RIM3";
			// 
			// lblIndicatorParameterTotalNr
			// 
			this.lblIndicatorParameterTotalNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblIndicatorParameterTotalNr.Location = new System.Drawing.Point(263, 69);
			this.lblIndicatorParameterTotalNr.Name = "lblIndicatorParameterTotalNr";
			this.lblIndicatorParameterTotalNr.Size = new System.Drawing.Size(105, 17);
			this.lblIndicatorParameterTotalNr.TabIndex = 32;
			this.lblIndicatorParameterTotalNr.Text = "Indicator Parameters";
			this.lblIndicatorParameterTotalNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtDataRange
			// 
			this.txtDataRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDataRange.Enabled = false;
			this.txtDataRange.Location = new System.Drawing.Point(79, 45);
			this.txtDataRange.Name = "txtDataRange";
			this.txtDataRange.Size = new System.Drawing.Size(178, 20);
			this.txtDataRange.TabIndex = 27;
			this.txtDataRange.Text = "LastBars=500";
			// 
			// lblScriptParameterTotalNr
			// 
			this.lblScriptParameterTotalNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblScriptParameterTotalNr.Location = new System.Drawing.Point(263, 48);
			this.lblScriptParameterTotalNr.Name = "lblScriptParameterTotalNr";
			this.lblScriptParameterTotalNr.Size = new System.Drawing.Size(105, 17);
			this.lblScriptParameterTotalNr.TabIndex = 29;
			this.lblScriptParameterTotalNr.Text = "Script Parameters";
			this.lblScriptParameterTotalNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtPositionSize
			// 
			this.txtPositionSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPositionSize.Enabled = false;
			this.txtPositionSize.Location = new System.Drawing.Point(79, 66);
			this.txtPositionSize.Name = "txtPositionSize";
			this.txtPositionSize.Size = new System.Drawing.Size(178, 20);
			this.txtPositionSize.TabIndex = 28;
			this.txtPositionSize.Text = "SharesConstant=1";
			// 
			// txtScriptParameterTotalNr
			// 
			this.txtScriptParameterTotalNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtScriptParameterTotalNr.Enabled = false;
			this.txtScriptParameterTotalNr.Location = new System.Drawing.Point(374, 45);
			this.txtScriptParameterTotalNr.Name = "txtScriptParameterTotalNr";
			this.txtScriptParameterTotalNr.Size = new System.Drawing.Size(40, 20);
			this.txtScriptParameterTotalNr.TabIndex = 30;
			this.txtScriptParameterTotalNr.Text = "148";
			// 
			// txtIndicatorParameterTotalNr
			// 
			this.txtIndicatorParameterTotalNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtIndicatorParameterTotalNr.Enabled = false;
			this.txtIndicatorParameterTotalNr.Location = new System.Drawing.Point(374, 66);
			this.txtIndicatorParameterTotalNr.Name = "txtIndicatorParameterTotalNr";
			this.txtIndicatorParameterTotalNr.Size = new System.Drawing.Size(40, 20);
			this.txtIndicatorParameterTotalNr.TabIndex = 31;
			this.txtIndicatorParameterTotalNr.Text = "600";
			// 
			// olvHistory
			// 
			this.olvHistory.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvHistory.AllColumns.Add(this.olvcPFavg);
			this.olvHistory.AllColumns.Add(this.olvcHistorySymbolScaleRange);
			this.olvHistory.AllColumns.Add(this.olvcHistoryDate);
			this.olvHistory.AllColumns.Add(this.olvcHistorySize);
			this.olvHistory.AllowColumnReorder = true;
			this.olvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvHistory.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
			this.olvHistory.CellEditEnterChangesRows = true;
			this.olvHistory.CellEditTabChangesRows = true;
			this.olvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcPFavg,
            this.olvcHistorySymbolScaleRange,
			this.olvcHistoryDate,
			this.olvcHistorySize});
			this.olvHistory.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvHistory.EmptyListMsg = "OPTIMIZATION_HISTORY_IS_EMPTY Never optimized since last script recompilation";
			this.olvHistory.EmptyListMsgFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvHistory.FullRowSelect = true;
			this.olvHistory.HideSelection = false;
			this.olvHistory.IncludeColumnHeadersInCopy = true;
			this.olvHistory.IncludeHiddenColumnsInDataTransfer = true;
			this.olvHistory.Location = new System.Drawing.Point(0, 0);
			this.olvHistory.Name = "olvHistory";
			this.olvHistory.ShowCommandMenuOnRightClick = true;
			this.olvHistory.ShowGroups = false;
			this.olvHistory.ShowImagesOnSubItems = true;
			this.olvHistory.ShowItemCountOnGroups = true;
			this.olvHistory.ShowItemToolTips = true;
			this.olvHistory.Size = new System.Drawing.Size(216, 186);
			this.olvHistory.TabIndex = 42;
			this.olvHistory.TintSortColumn = true;
			this.olvHistory.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
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
			this.olvHistory.DoubleClick += new System.EventHandler(this.olvHistory_DoubleClick);
			this.olvHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.olvHistory_KeyDown);
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
			this.olvcHistoryDate.IsVisible = true;
			this.olvcHistoryDate.Text = "Modified";
			this.olvcHistoryDate.ToolTipText = "Reminder when you did it";
			this.olvcHistoryDate.Width = 55;
			// 
			// olvcHistorySize
			// 
			this.olvcHistorySize.DisplayIndex = 3;
			this.olvcHistorySize.IsEditable = false;
			this.olvcHistorySize.IsVisible = true;
			this.olvcHistorySize.Text = "Size";
			this.olvcHistorySize.ToolTipText = "JSON file size";
			this.olvcHistorySize.Width = 55;
			// 
			// cbxPauseResume
			// 
			this.cbxPauseResume.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxPauseResume.Location = new System.Drawing.Point(176, 2);
			this.cbxPauseResume.Name = "cbxPauseResume";
			this.cbxPauseResume.Size = new System.Drawing.Size(136, 23);
			this.cbxPauseResume.TabIndex = 36;
			this.cbxPauseResume.Text = "Pause/Resume";
			this.cbxPauseResume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbxPauseResume.UseVisualStyleBackColor = true;
			this.cbxPauseResume.Click += new System.EventHandler(this.cbxPauseResume_Click);
			// 
			// cbxRunCancel
			// 
			this.cbxRunCancel.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxRunCancel.Enabled = false;
			this.cbxRunCancel.Location = new System.Drawing.Point(34, 2);
			this.cbxRunCancel.Name = "cbxRunCancel";
			this.cbxRunCancel.Size = new System.Drawing.Size(136, 23);
			this.cbxRunCancel.TabIndex = 19;
			this.cbxRunCancel.Text = "Cancel 528397 backtests";
			this.cbxRunCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbxRunCancel.UseVisualStyleBackColor = true;
			this.cbxRunCancel.Click += new System.EventHandler(this.cbxRunCancel_Click);
			// 
			// lblStats
			// 
			this.lblStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStats.Location = new System.Drawing.Point(485, 7);
			this.lblStats.Name = "lblStats";
			this.lblStats.Size = new System.Drawing.Size(153, 16);
			this.lblStats.TabIndex = 35;
			this.lblStats.Text = "48% complete   450044/18900";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(318, 6);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(161, 16);
			this.progressBar1.TabIndex = 34;
			this.progressBar1.Value = 48;
			// 
			// cbxExpandCollapse
			// 
			this.cbxExpandCollapse.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxExpandCollapse.Checked = true;
			this.cbxExpandCollapse.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxExpandCollapse.Location = new System.Drawing.Point(5, 2);
			this.cbxExpandCollapse.Name = "cbxExpandCollapse";
			this.cbxExpandCollapse.Size = new System.Drawing.Size(23, 23);
			this.cbxExpandCollapse.TabIndex = 44;
			this.cbxExpandCollapse.Text = "-";
			this.cbxExpandCollapse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbxExpandCollapse.UseVisualStyleBackColor = true;
			this.cbxExpandCollapse.CheckedChanged += new System.EventHandler(this.cbxExpandCollapse_CheckedChanged);
			// 
			// olvBacktests
			// 
			this.olvBacktests.AllowColumnReorder = true;
			this.olvBacktests.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvBacktests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcSerno,
            this.olvcTotalPositions,
            this.olvcProfitPerPosition,
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
			this.olvBacktests.Size = new System.Drawing.Size(641, 157);
			this.olvBacktests.TabIndex = 0;
			this.olvBacktests.TintSortColumn = true;
			this.olvBacktests.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvBacktests.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.olvBacktests.UseCellFormatEvents = true;
			this.olvBacktests.UseCompatibleStateImageBehavior = false;
			this.olvBacktests.UseCustomSelectionColors = true;
			this.olvBacktests.UseFilterIndicator = true;
			this.olvBacktests.UseFiltering = true;
			this.olvBacktests.UseHotItem = true;
			this.olvBacktests.UseTranslucentHotItem = true;
			this.olvBacktests.View = System.Windows.Forms.View.Details;
			this.olvBacktests.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.olvBacktests_CellRightClick);
			// 
			// olvcSerno
			// 
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Text = "#";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.ToolTipText = "Backtest\'s serial number";
			this.olvcSerno.Width = 28;
			// 
			// olvcTotalPositions
			// 
			this.olvcTotalPositions.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositions.Text = "#Pos";
			this.olvcTotalPositions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositions.ToolTipText = "TotalPositions generated";
			this.olvcTotalPositions.Width = 41;
			// 
			// olvcProfitPerPosition
			// 
			this.olvcProfitPerPosition.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPosition.Text = "$/pos";
			this.olvcProfitPerPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPosition.ToolTipText = "AverageProfit per Position Closed";
			this.olvcProfitPerPosition.Width = 50;
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
            this.mni_showAllScriptIndicatorParametersInOptimizationResults,
            this.mniCopyToClipboard,
            this.mniSaveCsv});
			this.ctxOneBacktestResult.Name = "ctxOneBacktestResult";
			this.ctxOneBacktestResult.Size = new System.Drawing.Size(509, 198);
			this.ctxOneBacktestResult.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOneBacktestResult_Opening);
			// 
			// mniInfo
			// 
			this.mniInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniInfo.Name = "mniInfo";
			this.mniInfo.Size = new System.Drawing.Size(508, 22);
			this.mniInfo.Text = "Net(-33,5415.00)PF(2.3)RF(5.6) > MA_ATRcompiled-DLL aaa";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(505, 6);
			// 
			// mniCopyToDefaultCtxBacktest
			// 
			this.mniCopyToDefaultCtxBacktest.Name = "mniCopyToDefaultCtxBacktest";
			this.mniCopyToDefaultCtxBacktest.Size = new System.Drawing.Size(508, 22);
			this.mniCopyToDefaultCtxBacktest.Text = "Copy To Default Context, Backtest";
			this.mniCopyToDefaultCtxBacktest.Click += new System.EventHandler(this.mniCopyToDefaultCtxBacktest_Click);
			// 
			// mniCopyToDefaultCtx
			// 
			this.mniCopyToDefaultCtx.Name = "mniCopyToDefaultCtx";
			this.mniCopyToDefaultCtx.Size = new System.Drawing.Size(508, 22);
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
			this.mniltbCopyToNewContextBacktest.Size = new System.Drawing.Size(448, 22);
			this.mniltbCopyToNewContextBacktest.TextLeft = "Copy To New Context, Backtest:";
			this.mniltbCopyToNewContextBacktest.TextLeftOffsetX = 0;
			this.mniltbCopyToNewContextBacktest.TextLeftWidth = 164;
			this.mniltbCopyToNewContextBacktest.TextRed = false;
			this.mniltbCopyToNewContextBacktest.TextRight = "~= 121pips";
			this.mniltbCopyToNewContextBacktest.TextRightOffsetX = 383;
			this.mniltbCopyToNewContextBacktest.TextRightWidth = 62;
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
			this.mniltbCopyToNewContext.Size = new System.Drawing.Size(448, 22);
			this.mniltbCopyToNewContext.TextLeft = "Copy To New Context:";
			this.mniltbCopyToNewContext.TextLeftOffsetX = 0;
			this.mniltbCopyToNewContext.TextLeftWidth = 116;
			this.mniltbCopyToNewContext.TextRed = false;
			this.mniltbCopyToNewContext.TextRight = "~= 121pips";
			this.mniltbCopyToNewContext.TextRightOffsetX = 383;
			this.mniltbCopyToNewContext.TextRightWidth = 62;
			this.mniltbCopyToNewContext.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbCopyToNewContext_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(505, 6);
			// 
			// mni_showAllScriptIndicatorParametersInOptimizationResults
			// 
			this.mni_showAllScriptIndicatorParametersInOptimizationResults.CheckOnClick = true;
			this.mni_showAllScriptIndicatorParametersInOptimizationResults.Name = "mni_showAllScriptIndicatorParametersInOptimizationResults";
			this.mni_showAllScriptIndicatorParametersInOptimizationResults.Size = new System.Drawing.Size(508, 22);
			this.mni_showAllScriptIndicatorParametersInOptimizationResults.Text = "Show All Script + Indicator Parameters In Optimization Results";
			this.mni_showAllScriptIndicatorParametersInOptimizationResults.Click += new System.EventHandler(this.mni_showAllScriptIndicatorParametersInOptimizationResultsClick);
			// 
			// mniCopyToClipboard
			// 
			this.mniCopyToClipboard.Enabled = false;
			this.mniCopyToClipboard.Name = "mniCopyToClipboard";
			this.mniCopyToClipboard.Size = new System.Drawing.Size(508, 22);
			this.mniCopyToClipboard.Text = "Copy To Clipboard (Paste-able to Excel)";
			this.mniCopyToClipboard.Click += new System.EventHandler(this.mniCopyToClipboard_Click);
			// 
			// mniSaveCsv
			// 
			this.mniSaveCsv.Enabled = false;
			this.mniSaveCsv.Name = "mniSaveCsv";
			this.mniSaveCsv.Size = new System.Drawing.Size(508, 22);
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
			this.Size = new System.Drawing.Size(641, 373);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvParameters)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudThreadsToRun)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.olvHistory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.olvBacktests)).EndInit();
			this.ctxOneBacktestResult.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		
		private System.Windows.Forms.TextBox txtSpread;
		private System.Windows.Forms.TextBox txtQuotesGenerator;
		private System.Windows.Forms.Label lblSpread;
		private System.Windows.Forms.Label lblQuotesGenerator;


		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbCopyToNewContext;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mniltbCopyToNewContextBacktest;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToDefaultCtx;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToDefaultCtxBacktest;
		private BrightIdeasSoftware.OLVColumn olvcSerno;
		private BrightIdeasSoftware.OLVColumn olvcProfitPerPosition;
		private BrightIdeasSoftware.OLVColumn olvcTotalPositions;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveLosers;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveWinners;
		private BrightIdeasSoftware.OLVColumn olvcMaxDrawdown;
		private BrightIdeasSoftware.OLVColumn olvcProfitFactor;
		private BrightIdeasSoftware.OLVColumn olvcRecoveryFactor;
		private BrightIdeasSoftware.OLVColumn olvcWinLoss;
		private BrightIdeasSoftware.OLVColumn olvcNetProfit;
		private System.Windows.Forms.ContextMenuStrip ctxOneBacktestResult;
		private BrightIdeasSoftware.ObjectListView olvBacktests;
		private System.Windows.Forms.CheckBox cbxPauseResume;
		private System.Windows.Forms.CheckBox cbxRunCancel;
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
		private System.Windows.Forms.CheckBox cbxExpandCollapse;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem mniInfo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniCopyToClipboard;
		private System.Windows.Forms.ToolStripMenuItem mniSaveCsv;
		private BrightIdeasSoftware.FastObjectListView olvParameters;
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
		private System.Windows.Forms.SplitContainer splitContainer2;	
		private System.Windows.Forms.ToolStripMenuItem mni_showAllScriptIndicatorParametersInOptimizationResults;
	}
}
