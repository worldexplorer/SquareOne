namespace Sq1.Charting.Demo {
	partial class TestChartForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestChartForm));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.item3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.item2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.item1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chartControl1 = new Sq1.Charting.ChartControl();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripStatusLabel1,
			this.toolStripProgressBar1,
			this.toolStripDropDownButton1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 223);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(476, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.item3ToolStripMenuItem,
			this.item2ToolStripMenuItem,
			this.item1ToolStripMenuItem});
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(180, 20);
			this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
			// 
			// item3ToolStripMenuItem
			// 
			this.item3ToolStripMenuItem.Name = "item3ToolStripMenuItem";
			this.item3ToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
			this.item3ToolStripMenuItem.Text = "item3";
			// 
			// item2ToolStripMenuItem
			// 
			this.item2ToolStripMenuItem.Name = "item2ToolStripMenuItem";
			this.item2ToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
			this.item2ToolStripMenuItem.Text = "item2";
			// 
			// item1ToolStripMenuItem
			// 
			this.item1ToolStripMenuItem.Name = "item1ToolStripMenuItem";
			this.item1ToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
			this.item1ToolStripMenuItem.Text = "item1";
			// 
			// chartControl1
			// 
			this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartControl1.Location = new System.Drawing.Point(0, 0);
			this.chartControl1.Margin = new System.Windows.Forms.Padding(0);
			this.chartControl1.Name = "chartControl1";
			this.chartControl1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 22);
			this.chartControl1.PaintAllowedDuringLivesimOrAfterBacktestFinished = true;
			this.chartControl1.RangeBarCollapsed = true;
			this.chartControl1.Size = new System.Drawing.Size(476, 245);
			this.chartControl1.TabIndex = 0;
			// 
			// TestChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(476, 245);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.chartControl1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "TestChartForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.Text = "TestChartForm";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ChartControl chartControl1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem item3ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem item2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem item1ToolStripMenuItem;
	}
}