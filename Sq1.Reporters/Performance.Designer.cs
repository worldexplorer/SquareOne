using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Reporters {
	public partial class Performance {
		private IContainer components;
		private System.Windows.Forms.ListView lvPerformance;
		private System.Windows.Forms.ColumnHeader colKpiTitles;
		private System.Windows.Forms.ColumnHeader collAllTrades;
		private System.Windows.Forms.ColumnHeader colLongTrades;
		private System.Windows.Forms.ColumnHeader colShortTrades;
		private System.Windows.Forms.ColumnHeader colBuyHold;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.lvPerformance = new System.Windows.Forms.ListView();
			this.colKpiTitles = new System.Windows.Forms.ColumnHeader();
			this.collAllTrades = new System.Windows.Forms.ColumnHeader();
			this.colLongTrades = new System.Windows.Forms.ColumnHeader();
			this.colShortTrades = new System.Windows.Forms.ColumnHeader();
			this.colBuyHold = new System.Windows.Forms.ColumnHeader();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// olvReport
			// 
			this.lvPerformance.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lvPerformance.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.colKpiTitles,
									this.collAllTrades,
									this.colLongTrades,
									this.colShortTrades,
									this.colBuyHold});
			this.lvPerformance.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvPerformance.FullRowSelect = true;
			this.lvPerformance.HideSelection = false;
			this.lvPerformance.Location = new System.Drawing.Point(0, 0);
			this.lvPerformance.MultiSelect = false;
			this.lvPerformance.Name = "olvReport";
			this.lvPerformance.ShowItemToolTips = true;
			this.lvPerformance.Size = new System.Drawing.Size(654, 374);
			this.lvPerformance.TabIndex = 3;
			this.lvPerformance.UseCompatibleStateImageBehavior = false;
			this.lvPerformance.View = System.Windows.Forms.View.Details;
			this.lvPerformance.SelectedIndexChanged += new System.EventHandler(this.lvReport_SelectedIndexChanged);
			// 
			// colKpiTitles
			// 
			this.colKpiTitles.Text = "";
			this.colKpiTitles.Width = 85;
			// 
			// collAllTrades
			// 
			this.collAllTrades.Text = "All Trades";
			this.collAllTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.collAllTrades.Width = 63;
			// 
			// colLongTrades
			// 
			this.colLongTrades.Text = "Long Trades";
			this.colLongTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colLongTrades.Width = 63;
			// 
			// colShortTrades
			// 
			this.colShortTrades.Text = "Short Trades";
			this.colShortTrades.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colShortTrades.Width = 63;
			// 
			// colBuyHold
			// 
			this.colBuyHold.Text = "Buy & Hold";
			this.colBuyHold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colBuyHold.Width = 63;
			// 
			// Performance
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.lvPerformance);
			this.Name = "Performance";
			this.Size = new System.Drawing.Size(654, 374);
			this.ResumeLayout(false);
		}

		private ToolTip toolTip1;
	}
}
