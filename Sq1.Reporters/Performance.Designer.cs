using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Reporters {
	public partial class Performance {
		private IContainer components;
		private BrightIdeasSoftware.ObjectListView olvReport;
		private BrightIdeasSoftware.OLVColumn colKpiTitles;
		private BrightIdeasSoftware.OLVColumn collAllTrades;
		private BrightIdeasSoftware.OLVColumn colLongTrades;
		private BrightIdeasSoftware.OLVColumn colShortTrades;
		private BrightIdeasSoftware.OLVColumn colBuyHold;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.olvReport = new BrightIdeasSoftware.ObjectListView();
			this.colKpiTitles = new BrightIdeasSoftware.OLVColumn();
			this.collAllTrades = new BrightIdeasSoftware.OLVColumn();
			this.colLongTrades = new BrightIdeasSoftware.OLVColumn();
			this.colShortTrades = new BrightIdeasSoftware.OLVColumn();
			this.colBuyHold = new BrightIdeasSoftware.OLVColumn();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.olvReport)).BeginInit();
			this.SuspendLayout();
			// 
			// olvReport
			// 
			this.olvReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvReport.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.colKpiTitles,
									this.collAllTrades,
									this.colLongTrades,
									this.colShortTrades,
									this.colBuyHold});
			this.olvReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvReport.FullRowSelect = true;
			this.olvReport.HeaderUsesThemes = false;
			this.olvReport.HideSelection = false;
			this.olvReport.IncludeColumnHeadersInCopy = true;
			this.olvReport.Location = new System.Drawing.Point(0, 0);
			this.olvReport.MultiSelect = false;
			this.olvReport.Name = "olvReport";
			this.olvReport.ShowCommandMenuOnRightClick = true;
			this.olvReport.ShowItemToolTips = true;
			this.olvReport.Size = new System.Drawing.Size(654, 374);
			this.olvReport.TabIndex = 3;
			this.olvReport.TintSortColumn = true;
			this.olvReport.UseCompatibleStateImageBehavior = false;
			this.olvReport.UseFilterIndicator = true;
			this.olvReport.View = System.Windows.Forms.View.Details;
			this.olvReport.SelectedIndexChanged += new System.EventHandler(this.lvReport_SelectedIndexChanged);
			// 
			// colKpiTitles
			// 
			this.colKpiTitles.CellPadding = null;
			this.colKpiTitles.Text = "";
			this.colKpiTitles.Width = 85;
			// 
			// collAllTrades
			// 
			this.collAllTrades.CellPadding = null;
			this.collAllTrades.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.collAllTrades.Text = "All Trades";
			this.collAllTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.collAllTrades.Width = 63;
			// 
			// colLongTrades
			// 
			this.colLongTrades.CellPadding = null;
			this.colLongTrades.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colLongTrades.Text = "Long Trades";
			this.colLongTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colLongTrades.Width = 63;
			// 
			// colShortTrades
			// 
			this.colShortTrades.CellPadding = null;
			this.colShortTrades.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colShortTrades.Text = "Short Trades";
			this.colShortTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colShortTrades.Width = 63;
			// 
			// colBuyHold
			// 
			this.colBuyHold.CellPadding = null;
			this.colBuyHold.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colBuyHold.Text = "Buy & Hold";
			this.colBuyHold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colBuyHold.Width = 63;
			// 
			// Performance
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.olvReport);
			this.Name = "Performance";
			this.Size = new System.Drawing.Size(654, 374);
			((System.ComponentModel.ISupportInitialize)(this.olvReport)).EndInit();
			this.ResumeLayout(false);
		}

		private ToolTip toolTip1;
	}
}
