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
			this.olvcSerno = new BrightIdeasSoftware.OLVColumn();
			this.olvcTotalTrades = new BrightIdeasSoftware.OLVColumn();
			this.olvcAverageProfit = new BrightIdeasSoftware.OLVColumn();
			this.olvcNetProfit = new BrightIdeasSoftware.OLVColumn();
			this.olvcWinLoss = new BrightIdeasSoftware.OLVColumn();
			this.olvcProfitFactor = new BrightIdeasSoftware.OLVColumn();
			this.olvcRecoveryFactor = new BrightIdeasSoftware.OLVColumn();
			this.olvcMaxDrawdown = new BrightIdeasSoftware.OLVColumn();
			this.olvcMaxConsecutiveWinners = new BrightIdeasSoftware.OLVColumn();
			this.olvcMaxConsecutiveLosers = new BrightIdeasSoftware.OLVColumn();
			this.ctxOneBacktestResult = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniCopyToDefaultCtxBacktest = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopyToDefaultCtx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.olvBacktests);
			this.splitContainer1.Size = new System.Drawing.Size(685, 671);
			this.splitContainer1.SplitterDistance = 27;
			this.splitContainer1.SplitterIncrement = 27;
			this.splitContainer1.TabIndex = 0;
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
			this.nudThreadsToRun.ValueChanged += new EventHandler(this.nudCpuCoresToUse_ValueChanged);
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
			this.txtPositionSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPositionSize.Enabled = false;
			this.txtPositionSize.Location = new System.Drawing.Point(238, 96);
			this.txtPositionSize.Name = "txtPositionSize";
			this.txtPositionSize.Size = new System.Drawing.Size(442, 20);
			this.txtPositionSize.TabIndex = 28;
			this.txtPositionSize.Text = "SharesConstant=1";
			// 
			// txtDataRange
			// 
			this.txtDataRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDataRange.Enabled = false;
			this.txtDataRange.Location = new System.Drawing.Point(238, 75);
			this.txtDataRange.Name = "txtDataRange";
			this.txtDataRange.Size = new System.Drawing.Size(442, 20);
			this.txtDataRange.TabIndex = 27;
			this.txtDataRange.Text = "LastBars=500";
			// 
			// txtSymbol
			// 
			this.txtSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSymbol.Enabled = false;
			this.txtSymbol.Location = new System.Drawing.Point(238, 54);
			this.txtSymbol.Name = "txtSymbol";
			this.txtSymbol.Size = new System.Drawing.Size(442, 20);
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
			this.txtStrategy.Size = new System.Drawing.Size(442, 20);
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
			this.lblStats.Location = new System.Drawing.Point(528, 7);
			this.lblStats.Name = "lblStats";
			this.lblStats.Size = new System.Drawing.Size(150, 16);
			this.lblStats.TabIndex = 35;
			this.lblStats.Text = "48% complete    450044/18900";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(162, 6);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(360, 16);
			this.progressBar1.TabIndex = 34;
			this.progressBar1.Value = 48;
			// 
			// olvBacktests
			// 
			this.olvBacktests.AllowColumnReorder = true;
			this.olvBacktests.AutoArrange = false;
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
			this.olvBacktests.ContextMenuStrip = this.ctxOneBacktestResult;
			this.olvBacktests.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvBacktests.EmptyListMsg = "";
			this.olvBacktests.FullRowSelect = true;
			this.olvBacktests.HasCollapsibleGroups = false;
			this.olvBacktests.HeaderUsesThemes = false;
			this.olvBacktests.IncludeColumnHeadersInCopy = true;
			this.olvBacktests.IncludeHiddenColumnsInDataTransfer = true;
			this.olvBacktests.Location = new System.Drawing.Point(0, 0);
			this.olvBacktests.Margin = new System.Windows.Forms.Padding(0);
			this.olvBacktests.MultiSelect = false;
			this.olvBacktests.Name = "olvBacktests";
			this.olvBacktests.SelectColumnsOnRightClick = false;
			this.olvBacktests.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
			this.olvBacktests.ShowCommandMenuOnRightClick = true;
			this.olvBacktests.ShowGroups = false;
			this.olvBacktests.Size = new System.Drawing.Size(685, 640);
			this.olvBacktests.TabIndex = 0;
			this.olvBacktests.UseCellFormatEvents = true;
			this.olvBacktests.UseCompatibleStateImageBehavior = false;
			this.olvBacktests.UseCustomSelectionColors = true;
			this.olvBacktests.UseFilterIndicator = true;
			this.olvBacktests.UseFiltering = true;
			this.olvBacktests.View = System.Windows.Forms.View.Details;
			// 
			// olvcSerno
			// 
			this.olvcSerno.CellPadding = null;
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Text = "#";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.olvcSerno.Width = 25;
			// 
			// olvcTotalTrades
			// 
			this.olvcTotalTrades.CellPadding = null;
			this.olvcTotalTrades.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalTrades.Text = "#Pos";
			this.olvcTotalTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalTrades.ToolTipText = "TotalTrades";
			this.olvcTotalTrades.Width = 41;
			// 
			// olvcAverageProfit
			// 
			this.olvcAverageProfit.CellPadding = null;
			this.olvcAverageProfit.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcAverageProfit.Text = "Avg";
			this.olvcAverageProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcAverageProfit.ToolTipText = "AverageProfit";
			this.olvcAverageProfit.Width = 50;
			// 
			// olvcNetProfit
			// 
			this.olvcNetProfit.CellPadding = null;
			this.olvcNetProfit.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfit.Text = "Net";
			this.olvcNetProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfit.ToolTipText = "NetProfit";
			this.olvcNetProfit.Width = 63;
			// 
			// olvcWinLoss
			// 
			this.olvcWinLoss.CellPadding = null;
			this.olvcWinLoss.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLoss.Text = "WL";
			this.olvcWinLoss.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLoss.ToolTipText = "WinLoss";
			this.olvcWinLoss.Width = 35;
			// 
			// olvcProfitFactor
			// 
			this.olvcProfitFactor.CellPadding = null;
			this.olvcProfitFactor.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactor.Text = "PF";
			this.olvcProfitFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactor.ToolTipText = "ProfitFactor";
			this.olvcProfitFactor.Width = 32;
			// 
			// olvcRecoveryFactor
			// 
			this.olvcRecoveryFactor.CellPadding = null;
			this.olvcRecoveryFactor.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactor.Text = "RF";
			this.olvcRecoveryFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactor.ToolTipText = "RecoveryFactor";
			this.olvcRecoveryFactor.Width = 32;
			// 
			// olvcMaxDrawdown
			// 
			this.olvcMaxDrawdown.CellPadding = null;
			this.olvcMaxDrawdown.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdown.Text = "DD";
			this.olvcMaxDrawdown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdown.ToolTipText = "MaxDrawdown";
			this.olvcMaxDrawdown.Width = 50;
			// 
			// olvcMaxConsecutiveWinners
			// 
			this.olvcMaxConsecutiveWinners.CellPadding = null;
			this.olvcMaxConsecutiveWinners.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinners.Text = "CW";
			this.olvcMaxConsecutiveWinners.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinners.ToolTipText = "MaxConsecutiveWinners";
			this.olvcMaxConsecutiveWinners.Width = 30;
			// 
			// olvcMaxConsecutiveLosers
			// 
			this.olvcMaxConsecutiveLosers.CellPadding = null;
			this.olvcMaxConsecutiveLosers.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosers.Text = "CL";
			this.olvcMaxConsecutiveLosers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosers.ToolTipText = "MaxConsecutiveLosers";
			this.olvcMaxConsecutiveLosers.Width = 30;
			// 
			// ctxOneBacktestResult
			// 
			this.ctxOneBacktestResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniCopyToDefaultCtxBacktest,
									this.mniCopyToDefaultCtx,
									this.toolStripMenuItem3});
			this.ctxOneBacktestResult.Name = "ctxOneBacktestResult";
			this.ctxOneBacktestResult.Size = new System.Drawing.Size(263, 92);
			// 
			// mniCopyToDefaultBacktest
			// 
			this.mniCopyToDefaultCtxBacktest.Name = "mniCopyToDefaultBacktest";
			this.mniCopyToDefaultCtxBacktest.Size = new System.Drawing.Size(262, 22);
			this.mniCopyToDefaultCtxBacktest.Text = "Copy To Default + Backtest";
			this.mniCopyToDefaultCtxBacktest.Click += new System.EventHandler(this.mniCopyToDefaultCtxBacktest_Click);
			// 
			// mniCopyToDefaultCtx
			// 
			this.mniCopyToDefaultCtx.Name = "mniCopyToDefaultCtx";
			this.mniCopyToDefaultCtx.Size = new System.Drawing.Size(262, 22);
			this.mniCopyToDefaultCtx.Text = "Copy To Default Context";
			this.mniCopyToDefaultCtx.Click += new System.EventHandler(this.mniCopyToDefaultCtx_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(262, 22);
			this.toolStripMenuItem3.Text = "Copy To New Context [ ___________ ]";
			// 
			// OptimizerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.splitContainer1);
			this.Name = "OptimizerControl";
			this.Size = new System.Drawing.Size(685, 671);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudThreadsToRun)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.olvBacktests)).EndInit();
			this.ctxOneBacktestResult.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
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
	}
}
