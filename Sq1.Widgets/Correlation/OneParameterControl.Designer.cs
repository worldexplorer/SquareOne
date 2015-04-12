using BrightIdeasSoftware;

namespace Sq1.Widgets.Correlation {
	partial class OneParameterControl {
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
			this.olvcParamValues = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTotalPositionsGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPerPositionGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcNetProfitGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcWinLossGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitFactorGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcRecoveryFactorGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxDrawdownGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveWinnersGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveLosersGlobal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olv = new BrightIdeasSoftware.ObjectListView();
			this.olvcTotalPositionsLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcTotalPositionsDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPerPositionLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPerPositionDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcNetProfitLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcNetProfitDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcWinLossLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcWinLossDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitFactorLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitFactorDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcRecoveryFactorLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcRecoveryFactorDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxDrawdownLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxDrawdownDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveWinnersLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveWinnersDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveLosersLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveLosersDelta = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxOneParameterControl = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniShowAllBacktestedParams = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowChosenParams = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowDeltasBtwAllAndChosenParams = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniMaximiseDeltaTotalPositions = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaProfitPerPosition = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaNet = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaWinLoss = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaProfitFactor = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaRecoveryFactor = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaMaxDrawdown = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaMaxConsecutiveWinners = new System.Windows.Forms.ToolStripMenuItem();
			this.mniMaximiseDeltaMaxConsecutiveLosers = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olv)).BeginInit();
			this.ctxOneParameterControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvcParamValues
			// 
			this.olvcParamValues.HeaderCheckBox = true;
			this.olvcParamValues.HeaderCheckState = System.Windows.Forms.CheckState.Checked;
			this.olvcParamValues.HeaderTriStateCheckBox = true;
			this.olvcParamValues.Text = "MaSlow.Period=13";
			this.olvcParamValues.ToolTipText = "Parameter Values Optimized";
			this.olvcParamValues.Width = 200;
			// 
			// olvcTotalPositionsGlobal
			// 
			this.olvcTotalPositionsGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsGlobal.Text = "#POS AllBacktested";
			this.olvcTotalPositionsGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsGlobal.ToolTipText = "TotalPositions generated AllBacktested";
			this.olvcTotalPositionsGlobal.Width = 43;
			// 
			// olvcProfitPerPositionGlobal
			// 
			this.olvcProfitPerPositionGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionGlobal.Text = "$/POS AllBacktested";
			this.olvcProfitPerPositionGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionGlobal.ToolTipText = "AverageProfit per Position Closed AllBacktested";
			this.olvcProfitPerPositionGlobal.Width = 52;
			// 
			// olvcNetProfitGlobal
			// 
			this.olvcNetProfitGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitGlobal.Text = "NET AllBacktested";
			this.olvcNetProfitGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitGlobal.ToolTipText = "NetProfit AllBacktested";
			this.olvcNetProfitGlobal.Width = 63;
			// 
			// olvcWinLossGlobal
			// 
			this.olvcWinLossGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossGlobal.Text = "WL AllBacktested";
			this.olvcWinLossGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossGlobal.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss AllBacktested";
			this.olvcWinLossGlobal.Width = 35;
			// 
			// olvcProfitFactorGlobal
			// 
			this.olvcProfitFactorGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorGlobal.Text = "PF AllBacktested";
			this.olvcProfitFactorGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorGlobal.ToolTipText = "ProfitFactor = total$won / total$lost AllBacktested";
			this.olvcProfitFactorGlobal.Width = 32;
			// 
			// olvcRecoveryFactorGlobal
			// 
			this.olvcRecoveryFactorGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorGlobal.Text = "RF AllBacktested";
			this.olvcRecoveryFactorGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorGlobal.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown AllBacktested";
			this.olvcRecoveryFactorGlobal.Width = 32;
			// 
			// olvcMaxDrawdownGlobal
			// 
			this.olvcMaxDrawdownGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownGlobal.Text = "DD AllBacktested";
			this.olvcMaxDrawdownGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownGlobal.ToolTipText = "MaxDrawdown, $ AllBacktested";
			this.olvcMaxDrawdownGlobal.Width = 63;
			// 
			// olvcMaxConsecutiveWinnersGlobal
			// 
			this.olvcMaxConsecutiveWinnersGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersGlobal.Text = "CW AllBacktested";
			this.olvcMaxConsecutiveWinnersGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersGlobal.ToolTipText = "MaxConsecutiveWinners AllBacktested";
			this.olvcMaxConsecutiveWinnersGlobal.Width = 31;
			// 
			// olvcMaxConsecutiveLosersGlobal
			// 
			this.olvcMaxConsecutiveLosersGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersGlobal.Text = "CL AllBacktested";
			this.olvcMaxConsecutiveLosersGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersGlobal.ToolTipText = "MaxConsecutiveLosers AllBacktested";
			this.olvcMaxConsecutiveLosersGlobal.Width = 30;
			// 
			// olvAllValuesForOneParam
			// 
			this.olv.AllColumns.Add(this.olvcParamValues);
			this.olv.AllColumns.Add(this.olvcTotalPositionsGlobal);
			this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			this.olv.AllColumns.Add(this.olvcTotalPositionsDelta);
			this.olv.AllColumns.Add(this.olvcProfitPerPositionGlobal);
			this.olv.AllColumns.Add(this.olvcProfitPerPositionLocal);
			this.olv.AllColumns.Add(this.olvcProfitPerPositionDelta);
			this.olv.AllColumns.Add(this.olvcNetProfitGlobal);
			this.olv.AllColumns.Add(this.olvcNetProfitLocal);
			this.olv.AllColumns.Add(this.olvcNetProfitDelta);
			this.olv.AllColumns.Add(this.olvcWinLossGlobal);
			this.olv.AllColumns.Add(this.olvcWinLossLocal);
			this.olv.AllColumns.Add(this.olvcWinLossDelta);
			this.olv.AllColumns.Add(this.olvcProfitFactorGlobal);
			this.olv.AllColumns.Add(this.olvcProfitFactorLocal);
			this.olv.AllColumns.Add(this.olvcProfitFactorDelta);
			this.olv.AllColumns.Add(this.olvcRecoveryFactorGlobal);
			this.olv.AllColumns.Add(this.olvcRecoveryFactorLocal);
			this.olv.AllColumns.Add(this.olvcRecoveryFactorDelta);
			this.olv.AllColumns.Add(this.olvcMaxDrawdownGlobal);
			this.olv.AllColumns.Add(this.olvcMaxDrawdownLocal);
			this.olv.AllColumns.Add(this.olvcMaxDrawdownDelta);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersGlobal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersLocal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersDelta);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersGlobal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersLocal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersDelta);
			this.olv.AllowColumnReorder = true;
			this.olv.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olv.CheckBoxes = true;
			this.olv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcParamValues,
            this.olvcTotalPositionsGlobal,
            this.olvcTotalPositionsLocal,
            this.olvcTotalPositionsDelta,
            this.olvcProfitPerPositionGlobal,
            this.olvcProfitPerPositionLocal,
            this.olvcProfitPerPositionDelta,
            this.olvcNetProfitGlobal,
            this.olvcNetProfitLocal,
            this.olvcNetProfitDelta,
            this.olvcWinLossGlobal,
            this.olvcWinLossLocal,
            this.olvcWinLossDelta,
            this.olvcProfitFactorGlobal,
            this.olvcProfitFactorLocal,
            this.olvcProfitFactorDelta,
            this.olvcRecoveryFactorGlobal,
            this.olvcRecoveryFactorLocal,
            this.olvcRecoveryFactorDelta,
            this.olvcMaxDrawdownGlobal,
            this.olvcMaxDrawdownLocal,
            this.olvcMaxDrawdownDelta,
            this.olvcMaxConsecutiveWinnersGlobal,
            this.olvcMaxConsecutiveWinnersLocal,
            this.olvcMaxConsecutiveWinnersDelta,
            this.olvcMaxConsecutiveLosersGlobal,
            this.olvcMaxConsecutiveLosersLocal,
            this.olvcMaxConsecutiveLosersDelta});
			this.olv.ContextMenuStrip = this.ctxOneParameterControl;
			this.olv.Cursor = System.Windows.Forms.Cursors.Default;
			this.olv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olv.EmptyListMsg = "";
			this.olv.HideSelection = false;
			this.olv.IncludeColumnHeadersInCopy = true;
			this.olv.IncludeHiddenColumnsInDataTransfer = true;
			this.olv.Location = new System.Drawing.Point(0, 0);
			this.olv.Margin = new System.Windows.Forms.Padding(0);
			this.olv.Name = "olvAllValuesForOneParam";
			this.olv.ShowCommandMenuOnRightClick = true;
			this.olv.ShowGroups = false;
			this.olv.Size = new System.Drawing.Size(1007, 300);
			this.olv.TabIndex = 1;
			this.olv.TintSortColumn = true;
			this.olv.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olv.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.olv.UseCellFormatEvents = true;
			this.olv.UseCompatibleStateImageBehavior = false;
			this.olv.UseCustomSelectionColors = true;
			this.olv.UseFilterIndicator = true;
			this.olv.UseFiltering = true;
			this.olv.UseHotItem = true;
			this.olv.UseTranslucentHotItem = true;
			this.olv.View = System.Windows.Forms.View.Details;
			// 
			// olvcTotalPositionsLocal
			// 
			this.olvcTotalPositionsLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsLocal.Text = "#pos chosen";
			this.olvcTotalPositionsLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsLocal.ToolTipText = "TotalPositions generated chosen(selected with checkboxes))";
			this.olvcTotalPositionsLocal.Width = 43;
			// 
			// olvcTotalPositionsDelta
			// 
			this.olvcTotalPositionsDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsDelta.Text = "#pos delta";
			this.olvcTotalPositionsDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsDelta.ToolTipText = "TotalPositions generated delta=AllBacktested-Chosen";
			this.olvcTotalPositionsDelta.Width = 43;
			// 
			// olvcProfitPerPositionLocal
			// 
			this.olvcProfitPerPositionLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionLocal.Text = "$/pos chosen";
			this.olvcProfitPerPositionLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionLocal.ToolTipText = "AverageProfit per Position Closed chosen(selected with checkboxes)";
			this.olvcProfitPerPositionLocal.Width = 52;
			// 
			// olvcProfitPerPositionDelta
			// 
			this.olvcProfitPerPositionDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionDelta.Text = "$/pos delta";
			this.olvcProfitPerPositionDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionDelta.ToolTipText = "AverageProfit per Position Closed delta=AllBacktested-Chosen";
			this.olvcProfitPerPositionDelta.Width = 52;
			// 
			// olvcNetProfitLocal
			// 
			this.olvcNetProfitLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitLocal.Text = "net chosen";
			this.olvcNetProfitLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitLocal.ToolTipText = "NetProfit chosen(selected with checkboxes)";
			this.olvcNetProfitLocal.Width = 63;
			// 
			// olvcNetProfitDelta
			// 
			this.olvcNetProfitDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitDelta.Text = "net delta";
			this.olvcNetProfitDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitDelta.ToolTipText = "NetProfit delta=AllBacktested-Chosen";
			this.olvcNetProfitDelta.Width = 63;
			// 
			// olvcWinLossLocal
			// 
			this.olvcWinLossLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossLocal.Text = "wl chosen";
			this.olvcWinLossLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossLocal.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss chosen(selected with checkboxes)";
			this.olvcWinLossLocal.Width = 35;
			// 
			// olvcWinLossDelta
			// 
			this.olvcWinLossDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossDelta.Text = "wl delta";
			this.olvcWinLossDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossDelta.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss delta=AllBacktested-Chosen";
			this.olvcWinLossDelta.Width = 35;
			// 
			// olvcProfitFactorLocal
			// 
			this.olvcProfitFactorLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorLocal.Text = "pf chosen";
			this.olvcProfitFactorLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorLocal.ToolTipText = "ProfitFactor = total$won / total$lost chosen(selected with checkboxes)";
			this.olvcProfitFactorLocal.Width = 32;
			// 
			// olvcProfitFactorDelta
			// 
			this.olvcProfitFactorDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorDelta.Text = "pf delta";
			this.olvcProfitFactorDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorDelta.ToolTipText = "ProfitFactor = total$won / total$lost delta=AllBacktested-Chosen";
			this.olvcProfitFactorDelta.Width = 32;
			// 
			// olvcRecoveryFactorLocal
			// 
			this.olvcRecoveryFactorLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorLocal.Text = "rf chosen";
			this.olvcRecoveryFactorLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorLocal.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown chosen(selected wi" +
    "th checkboxes)";
			this.olvcRecoveryFactorLocal.Width = 32;
			// 
			// olvcRecoveryFactorDelta
			// 
			this.olvcRecoveryFactorDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorDelta.Text = "rf delta";
			this.olvcRecoveryFactorDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorDelta.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown delta=AllBackteste" +
    "d-Chosen";
			this.olvcRecoveryFactorDelta.Width = 32;
			// 
			// olvcMaxDrawdownLocal
			// 
			this.olvcMaxDrawdownLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownLocal.Text = "dd chosen";
			this.olvcMaxDrawdownLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownLocal.ToolTipText = "MaxDrawdown, $ chosen(selected with checkboxes)";
			this.olvcMaxDrawdownLocal.Width = 63;
			// 
			// olvcMaxDrawdownDelta
			// 
			this.olvcMaxDrawdownDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownDelta.Text = "dd delta";
			this.olvcMaxDrawdownDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownDelta.ToolTipText = "MaxDrawdown, $ delta=AllBacktested-Chosen";
			this.olvcMaxDrawdownDelta.Width = 63;
			// 
			// olvcMaxConsecutiveWinnersLocal
			// 
			this.olvcMaxConsecutiveWinnersLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersLocal.Text = "cw chosen";
			this.olvcMaxConsecutiveWinnersLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersLocal.ToolTipText = "MaxConsecutiveWinners chosen(selected with checkboxes)";
			this.olvcMaxConsecutiveWinnersLocal.Width = 31;
			// 
			// olvcMaxConsecutiveWinnersDelta
			// 
			this.olvcMaxConsecutiveWinnersDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersDelta.Text = "cw delta";
			this.olvcMaxConsecutiveWinnersDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersDelta.ToolTipText = "MaxConsecutiveWinners delta=AllBacktested-Chosen";
			this.olvcMaxConsecutiveWinnersDelta.Width = 31;
			// 
			// olvcMaxConsecutiveLosersLocal
			// 
			this.olvcMaxConsecutiveLosersLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersLocal.Text = "cl chosen";
			this.olvcMaxConsecutiveLosersLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersLocal.ToolTipText = "MaxConsecutiveLosers chosen(selected with checkboxes)";
			this.olvcMaxConsecutiveLosersLocal.Width = 30;
			// 
			// olvcMaxConsecutiveLosersDelta
			// 
			this.olvcMaxConsecutiveLosersDelta.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersDelta.Text = "cl delta";
			this.olvcMaxConsecutiveLosersDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersDelta.ToolTipText = "MaxConsecutiveLosers delta=AllBacktested-Chosen";
			this.olvcMaxConsecutiveLosersDelta.Width = 30;
			// 
			// ctxOneParameterControl
			// 
			this.ctxOneParameterControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniShowAllBacktestedParams,
            this.mniShowChosenParams,
            this.mniShowDeltasBtwAllAndChosenParams,
            this.toolStripSeparator1,
            this.mniMaximiseDeltaTotalPositions,
            this.mniMaximiseDeltaProfitPerPosition,
            this.mniMaximiseDeltaNet,
            this.mniMaximiseDeltaWinLoss,
            this.mniMaximiseDeltaProfitFactor,
            this.mniMaximiseDeltaRecoveryFactor,
            this.mniMaximiseDeltaMaxDrawdown,
            this.mniMaximiseDeltaMaxConsecutiveWinners,
            this.mniMaximiseDeltaMaxConsecutiveLosers,
            this.toolStripSeparator2,
            this.mniMaximiseDeltaAutoRunAfterSequencerFinished});
			this.ctxOneParameterControl.Name = "ctxOneParameterControl";
			this.ctxOneParameterControl.Size = new System.Drawing.Size(343, 324);
			// 
			// mniShowAllBacktestedParams
			// 
			this.mniShowAllBacktestedParams.CheckOnClick = true;
			this.mniShowAllBacktestedParams.Name = "mniShowAllBacktestedParams";
			this.mniShowAllBacktestedParams.Size = new System.Drawing.Size(342, 22);
			this.mniShowAllBacktestedParams.Text = "Show KPIs: All Backtested Params";
			// 
			// mniShowChosenParams
			// 
			this.mniShowChosenParams.CheckOnClick = true;
			this.mniShowChosenParams.Name = "mniShowChosenParams";
			this.mniShowChosenParams.Size = new System.Drawing.Size(342, 22);
			this.mniShowChosenParams.Text = "Show KPIs: Chosen Parameters";
			// 
			// mniShowDeltasBtwAllAndChosenParams
			// 
			this.mniShowDeltasBtwAllAndChosenParams.Checked = true;
			this.mniShowDeltasBtwAllAndChosenParams.CheckOnClick = true;
			this.mniShowDeltasBtwAllAndChosenParams.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowDeltasBtwAllAndChosenParams.Name = "mniShowDeltasBtwAllAndChosenParams";
			this.mniShowDeltasBtwAllAndChosenParams.Size = new System.Drawing.Size(342, 22);
			this.mniShowDeltasBtwAllAndChosenParams.Text = "Show KPIs: Deltas = (Chosen - AllBacktested)";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(339, 6);
			// 
			// mniMaximiseDeltaTotalPositions
			// 
			this.mniMaximiseDeltaTotalPositions.Name = "mniMaximiseDeltaTotalPositions";
			this.mniMaximiseDeltaTotalPositions.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaTotalPositions.Text = "Maximise Delta: Total Positions";
			// 
			// mniMaximiseDeltaProfitPerPosition
			// 
			this.mniMaximiseDeltaProfitPerPosition.Name = "mniMaximiseDeltaProfitPerPosition";
			this.mniMaximiseDeltaProfitPerPosition.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaProfitPerPosition.Text = "Maximise Delta: Profit Per Position";
			// 
			// mniMaximiseDeltaNet
			// 
			this.mniMaximiseDeltaNet.Name = "mniMaximiseDeltaNet";
			this.mniMaximiseDeltaNet.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaNet.Text = "Maximise Delta: Net";
			// 
			// mniMaximiseDeltaWinLoss
			// 
			this.mniMaximiseDeltaWinLoss.Name = "mniMaximiseDeltaWinLoss";
			this.mniMaximiseDeltaWinLoss.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaWinLoss.Text = "Maximise Delta: Win/Loss";
			// 
			// mniMaximiseDeltaProfitFactor
			// 
			this.mniMaximiseDeltaProfitFactor.Name = "mniMaximiseDeltaProfitFactor";
			this.mniMaximiseDeltaProfitFactor.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaProfitFactor.Text = "Maximise Delta: Profit Factor";
			// 
			// mniMaximiseDeltaRecoveryFactor
			// 
			this.mniMaximiseDeltaRecoveryFactor.Name = "mniMaximiseDeltaRecoveryFactor";
			this.mniMaximiseDeltaRecoveryFactor.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaRecoveryFactor.Text = "Maximise Delta: Recovery Factor";
			// 
			// mniMaximiseDeltaMaxDrawdown
			// 
			this.mniMaximiseDeltaMaxDrawdown.Name = "mniMaximiseDeltaMaxDrawdown";
			this.mniMaximiseDeltaMaxDrawdown.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaMaxDrawdown.Text = "Maximise Delta: Max Drawdown";
			// 
			// mniMaximiseDeltaMaxConsecutiveWinners
			// 
			this.mniMaximiseDeltaMaxConsecutiveWinners.Name = "mniMaximiseDeltaMaxConsecutiveWinners";
			this.mniMaximiseDeltaMaxConsecutiveWinners.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaMaxConsecutiveWinners.Text = "Maximise Delta: Max Consecutive Winners";
			// 
			// mniMaximiseDeltaMaxConsecutiveLosers
			// 
			this.mniMaximiseDeltaMaxConsecutiveLosers.Name = "mniMaximiseDeltaMaxConsecutiveLosers";
			this.mniMaximiseDeltaMaxConsecutiveLosers.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaMaxConsecutiveLosers.Text = "Maximise Delta: MaxConsecutiveLosers";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(339, 6);
			this.toolStripSeparator2.Visible = false;
			// 
			// mniMaximiseDeltaAutoRunAfterSequencerFinished
			// 
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Checked = true;
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.CheckOnClick = true;
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Name = "mniMaximiseDeltaAutoRunAfterSequencerFinished";
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Size = new System.Drawing.Size(342, 22);
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Text = "Maximise Delta: Auto-Run after Sequencer finished";
			this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Visible = false;
			// 
			// OneParameterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.olv);
			this.Name = "OneParameterControl";
			this.Size = new System.Drawing.Size(1007, 300);
			((System.ComponentModel.ISupportInitialize)(this.olv)).EndInit();
			this.ctxOneParameterControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.OLVColumn olvcParamValues;
		private BrightIdeasSoftware.OLVColumn olvcTotalPositionsGlobal;
		private BrightIdeasSoftware.OLVColumn olvcProfitPerPositionGlobal;
		private BrightIdeasSoftware.OLVColumn olvcNetProfitGlobal;
		private BrightIdeasSoftware.OLVColumn olvcWinLossGlobal;
		private BrightIdeasSoftware.OLVColumn olvcProfitFactorGlobal;
		private BrightIdeasSoftware.OLVColumn olvcRecoveryFactorGlobal;
		private BrightIdeasSoftware.OLVColumn olvcMaxDrawdownGlobal;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveWinnersGlobal;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveLosersGlobal;
		private BrightIdeasSoftware.ObjectListView olv;
		private BrightIdeasSoftware.OLVColumn olvcTotalPositionsLocal;
		private BrightIdeasSoftware.OLVColumn olvcProfitPerPositionLocal;
		private BrightIdeasSoftware.OLVColumn olvcNetProfitLocal;
		private BrightIdeasSoftware.OLVColumn olvcWinLossLocal;
		private BrightIdeasSoftware.OLVColumn olvcProfitFactorLocal;
		private BrightIdeasSoftware.OLVColumn olvcRecoveryFactorLocal;
		private BrightIdeasSoftware.OLVColumn olvcMaxDrawdownLocal;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveWinnersLocal;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveLosersLocal;

		private BrightIdeasSoftware.OLVColumn olvcTotalPositionsDelta;
		private BrightIdeasSoftware.OLVColumn olvcProfitPerPositionDelta;
		private BrightIdeasSoftware.OLVColumn olvcNetProfitDelta;
		private BrightIdeasSoftware.OLVColumn olvcWinLossDelta;
		private BrightIdeasSoftware.OLVColumn olvcProfitFactorDelta;
		private BrightIdeasSoftware.OLVColumn olvcRecoveryFactorDelta;
		private BrightIdeasSoftware.OLVColumn olvcMaxDrawdownDelta;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveWinnersDelta;
		private BrightIdeasSoftware.OLVColumn olvcMaxConsecutiveLosersDelta;
		private System.Windows.Forms.ContextMenuStrip ctxOneParameterControl;
		private System.Windows.Forms.ToolStripMenuItem mniShowAllBacktestedParams;
		private System.Windows.Forms.ToolStripMenuItem mniShowChosenParams;
		private System.Windows.Forms.ToolStripMenuItem mniShowDeltasBtwAllAndChosenParams;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaTotalPositions;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaProfitPerPosition;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaNet;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaWinLoss;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaProfitFactor;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaRecoveryFactor;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaMaxDrawdown;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaMaxConsecutiveWinners;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaMaxConsecutiveLosers;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniMaximiseDeltaAutoRunAfterSequencerFinished;
	}
}
