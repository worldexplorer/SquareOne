using BrightIdeasSoftware;
namespace Sq1.Widgets.Optimization {
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
			this.olvcProfitPerPositionLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcNetProfitLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcWinLossLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitFactorLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcRecoveryFactorLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxDrawdownLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveWinnersLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaxConsecutiveLosersLocal = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.olv)).BeginInit();
			this.SuspendLayout();
			// 
			// olvcParamValues
			// 
			this.olvcParamValues.HeaderCheckBox = true;
			this.olvcParamValues.HeaderCheckState = System.Windows.Forms.CheckState.Checked;
			this.olvcParamValues.HeaderTriStateCheckBox = true;
			this.olvcParamValues.Text = "MaSlow.Period";
			this.olvcParamValues.ToolTipText = "Parameter Values Optimized";
			this.olvcParamValues.Width = 120;
			// 
			// olvcTotalPositionsGlobal
			// 
			this.olvcTotalPositionsGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsGlobal.Text = "#POS";
			this.olvcTotalPositionsGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsGlobal.ToolTipText = "TotalPositions generated";
			this.olvcTotalPositionsGlobal.Width = 43;
			// 
			// olvcProfitPerPositionGlobal
			// 
			this.olvcProfitPerPositionGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionGlobal.Text = "$/POS";
			this.olvcProfitPerPositionGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionGlobal.ToolTipText = "AverageProfit per Position Closed";
			this.olvcProfitPerPositionGlobal.Width = 52;
			// 
			// olvcNetProfitGlobal
			// 
			this.olvcNetProfitGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitGlobal.Text = "NET";
			this.olvcNetProfitGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitGlobal.ToolTipText = "NetProfit";
			this.olvcNetProfitGlobal.Width = 63;
			// 
			// olvcWinLossGlobal
			// 
			this.olvcWinLossGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossGlobal.Text = "WL";
			this.olvcWinLossGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossGlobal.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss";
			this.olvcWinLossGlobal.Width = 35;
			// 
			// olvcProfitFactorGlobal
			// 
			this.olvcProfitFactorGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorGlobal.Text = "PF";
			this.olvcProfitFactorGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorGlobal.ToolTipText = "ProfitFactor = total$won / total$lost";
			this.olvcProfitFactorGlobal.Width = 32;
			// 
			// olvcRecoveryFactorGlobal
			// 
			this.olvcRecoveryFactorGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorGlobal.Text = "RF";
			this.olvcRecoveryFactorGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorGlobal.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown";
			this.olvcRecoveryFactorGlobal.Width = 32;
			// 
			// olvcMaxDrawdownGlobal
			// 
			this.olvcMaxDrawdownGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownGlobal.Text = "DD";
			this.olvcMaxDrawdownGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownGlobal.ToolTipText = "MaxDrawdown, $";
			this.olvcMaxDrawdownGlobal.Width = 63;
			// 
			// olvcMaxConsecutiveWinnersGlobal
			// 
			this.olvcMaxConsecutiveWinnersGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersGlobal.Text = "CW";
			this.olvcMaxConsecutiveWinnersGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersGlobal.ToolTipText = "MaxConsecutiveWinners";
			this.olvcMaxConsecutiveWinnersGlobal.Width = 31;
			// 
			// olvcMaxConsecutiveLosersGlobal
			// 
			this.olvcMaxConsecutiveLosersGlobal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersGlobal.Text = "CL";
			this.olvcMaxConsecutiveLosersGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersGlobal.ToolTipText = "MaxConsecutiveLosers";
			this.olvcMaxConsecutiveLosersGlobal.Width = 30;
			// 
			// olv
			// 
			this.olv.AllColumns.Add(this.olvcParamValues);
			this.olv.AllColumns.Add(this.olvcTotalPositionsGlobal);
			this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			this.olv.AllColumns.Add(this.olvcProfitPerPositionGlobal);
			this.olv.AllColumns.Add(this.olvcProfitPerPositionLocal);
			this.olv.AllColumns.Add(this.olvcNetProfitGlobal);
			this.olv.AllColumns.Add(this.olvcNetProfitLocal);
			this.olv.AllColumns.Add(this.olvcWinLossGlobal);
			this.olv.AllColumns.Add(this.olvcWinLossLocal);
			this.olv.AllColumns.Add(this.olvcProfitFactorGlobal);
			this.olv.AllColumns.Add(this.olvcProfitFactorLocal);
			this.olv.AllColumns.Add(this.olvcRecoveryFactorGlobal);
			this.olv.AllColumns.Add(this.olvcRecoveryFactorLocal);
			this.olv.AllColumns.Add(this.olvcMaxDrawdownGlobal);
			this.olv.AllColumns.Add(this.olvcMaxDrawdownLocal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersGlobal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersLocal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersGlobal);
			this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersLocal);
			this.olv.AllowColumnReorder = true;
			this.olv.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olv.CheckBoxes = true;
			this.olv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcParamValues,
            this.olvcTotalPositionsGlobal,
            this.olvcTotalPositionsLocal,
            this.olvcProfitPerPositionGlobal,
            this.olvcProfitPerPositionLocal,
            this.olvcNetProfitGlobal,
            this.olvcNetProfitLocal,
            this.olvcWinLossGlobal,
            this.olvcWinLossLocal,
            this.olvcProfitFactorGlobal,
            this.olvcProfitFactorLocal,
            this.olvcRecoveryFactorGlobal,
            this.olvcRecoveryFactorLocal,
            this.olvcMaxDrawdownGlobal,
            this.olvcMaxDrawdownLocal,
            this.olvcMaxConsecutiveWinnersGlobal,
            this.olvcMaxConsecutiveWinnersLocal,
            this.olvcMaxConsecutiveLosersGlobal,
            this.olvcMaxConsecutiveLosersLocal});
			this.olv.Cursor = System.Windows.Forms.Cursors.Default;
			this.olv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olv.EmptyListMsg = "";
			this.olv.GridLines = true;
			this.olv.IncludeColumnHeadersInCopy = true;
			this.olv.IncludeHiddenColumnsInDataTransfer = true;
			this.olv.Location = new System.Drawing.Point(0, 0);
			this.olv.Margin = new System.Windows.Forms.Padding(0);
			this.olv.Name = "olv";
			this.olv.OwnerDraw = true;
			this.olv.ShowCommandMenuOnRightClick = true;
			this.olv.ShowGroups = false;
			this.olv.Size = new System.Drawing.Size(887, 355);
			this.olv.TabIndex = 1;
			this.olv.TintSortColumn = true;
			this.olv.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olv.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
			this.olv.UseCellFormatEvents = true;
			this.olv.UseCompatibleStateImageBehavior = false;
			this.olv.UseCustomSelectionColors = true;
			this.olv.UseFilterIndicator = true;
			this.olv.UseFiltering = true;
			this.olv.UseTranslucentHotItem = true;
			this.olv.View = System.Windows.Forms.View.Details;
			// 
			// olvcTotalPositionsLocal
			// 
			this.olvcTotalPositionsLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsLocal.Text = "#pos";
			this.olvcTotalPositionsLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcTotalPositionsLocal.ToolTipText = "TotalPositions generated";
			this.olvcTotalPositionsLocal.Width = 43;
			// 
			// olvcProfitPerPositionLocal
			// 
			this.olvcProfitPerPositionLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionLocal.Text = "$/pos";
			this.olvcProfitPerPositionLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerPositionLocal.ToolTipText = "AverageProfit per Position Closed";
			this.olvcProfitPerPositionLocal.Width = 52;
			// 
			// olvcNetProfitLocal
			// 
			this.olvcNetProfitLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitLocal.Text = "net";
			this.olvcNetProfitLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcNetProfitLocal.ToolTipText = "NetProfit";
			this.olvcNetProfitLocal.Width = 63;
			// 
			// olvcWinLossLocal
			// 
			this.olvcWinLossLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossLocal.Text = "wl";
			this.olvcWinLossLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcWinLossLocal.ToolTipText = "Win/Loss; WL=1 <= 50%win,50%loss";
			this.olvcWinLossLocal.Width = 35;
			// 
			// olvcProfitFactorLocal
			// 
			this.olvcProfitFactorLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorLocal.Text = "pf";
			this.olvcProfitFactorLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitFactorLocal.ToolTipText = "ProfitFactor = total$won / total$lost";
			this.olvcProfitFactorLocal.Width = 32;
			// 
			// olvcRecoveryFactorLocal
			// 
			this.olvcRecoveryFactorLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorLocal.Text = "rf";
			this.olvcRecoveryFactorLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcRecoveryFactorLocal.ToolTipText = "RecoveryFactor = NetProfitForClosedPositionsBoth / MaxDrawDown";
			this.olvcRecoveryFactorLocal.Width = 32;
			// 
			// olvcMaxDrawdownLocal
			// 
			this.olvcMaxDrawdownLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownLocal.Text = "dd";
			this.olvcMaxDrawdownLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxDrawdownLocal.ToolTipText = "MaxDrawdown, $";
			this.olvcMaxDrawdownLocal.Width = 63;
			// 
			// olvcMaxConsecutiveWinnersLocal
			// 
			this.olvcMaxConsecutiveWinnersLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersLocal.Text = "cw";
			this.olvcMaxConsecutiveWinnersLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveWinnersLocal.ToolTipText = "MaxConsecutiveWinners";
			this.olvcMaxConsecutiveWinnersLocal.Width = 31;
			// 
			// olvcMaxConsecutiveLosersLocal
			// 
			this.olvcMaxConsecutiveLosersLocal.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersLocal.Text = "cl";
			this.olvcMaxConsecutiveLosersLocal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaxConsecutiveLosersLocal.ToolTipText = "MaxConsecutiveLosers";
			this.olvcMaxConsecutiveLosersLocal.Width = 30;
			// 
			// OneParameterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.olv);
			this.Name = "OneParameterControl";
			this.Size = new System.Drawing.Size(887, 355);
			((System.ComponentModel.ISupportInitialize)(this.olv)).EndInit();
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
	}
}
