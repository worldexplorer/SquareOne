namespace Sq1.Widgets.Correlation {
	partial class CorrelatorControl {
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.oneParameterControl1 = new Sq1.Widgets.Correlation.OneParameterControl();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripItemTrackBarWalkForward = new Sq1.Widgets.ToolStripImproved.ToolStripItemWalkForward();
			this.flowLayoutPanel1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoScroll = true;
			this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.flowLayoutPanel1.Controls.Add(this.oneParameterControl1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(478, 405);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// oneParameterControl1
			// 
			this.oneParameterControl1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.oneParameterControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.oneParameterControl1.Location = new System.Drawing.Point(3, 3);
			this.oneParameterControl1.Name = "oneParameterControl1";
			this.oneParameterControl1.Size = new System.Drawing.Size(325, 248);
			this.oneParameterControl1.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemTrackBarWalkForward});
			this.statusStrip1.Location = new System.Drawing.Point(0, 383);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(478, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripItemTrackBarWalkForward
			// 
			this.toolStripItemTrackBarWalkForward.FillFromCurrentToMax = false;
			this.toolStripItemTrackBarWalkForward.LabelText = "% Backtest Calculated";
			this.toolStripItemTrackBarWalkForward.Name = "toolStripItemTrackBarWalkForward";
			this.toolStripItemTrackBarWalkForward.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.toolStripItemTrackBarWalkForward.Size = new System.Drawing.Size(310, 20);
			this.toolStripItemTrackBarWalkForward.Text = "toolStripItemTrackBar1";
			this.toolStripItemTrackBarWalkForward.ValueCurrent = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.toolStripItemTrackBarWalkForward.WalkForwardChecked = false;
			// 
			// CorrelatorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "CorrelatorControl";
			this.Size = new System.Drawing.Size(478, 405);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private OneParameterControl oneParameterControl1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private Sq1.Widgets.ToolStripImproved.ToolStripItemWalkForward toolStripItemTrackBarWalkForward;
	}
}
