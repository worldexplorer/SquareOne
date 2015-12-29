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
			this.chartControl1 = new Sq1.Charting.ChartControl();
			this.SuspendLayout();
			// 
			// chartControl1
			// 
			this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartControl1.Location = new System.Drawing.Point(0, 0);
			this.chartControl1.Name = "chartControl1";
			this.chartControl1.PaintAllowedDuringLivesimOrAfterBacktestFinished = true;
			this.chartControl1.RangeBarCollapsed = false;
			this.chartControl1.Size = new System.Drawing.Size(642, 411);
			this.chartControl1.TabIndex = 0;
			// 
			// TestChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(642, 411);
			this.Controls.Add(this.chartControl1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "TestChartForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.Text = "TestChartForm";
			this.ResumeLayout(false);

		}

		#endregion

		private ChartControl chartControl1;
	}
}