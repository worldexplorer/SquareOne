namespace Sq1.Widgets.ToolStripTrackBar {
	partial class TrackBarControl {
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
			this.TrackBar = new System.Windows.Forms.TrackBar();
			this.LabelPercentage = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.sliderComboControl1 = new Sq1.Widgets.SteppingSlider.SliderComboControl();
			((System.ComponentModel.ISupportInitialize)(this.TrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// TrackBar
			// 
			this.TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TrackBar.Location = new System.Drawing.Point(4, -1);
			this.TrackBar.Maximum = 100;
			this.TrackBar.MinimumSize = new System.Drawing.Size(0, 17);
			this.TrackBar.Name = "TrackBar";
			this.TrackBar.Size = new System.Drawing.Size(327, 45);
			this.TrackBar.TabIndex = 0;
			this.TrackBar.Value = 26;
			this.TrackBar.Visible = false;
			// 
			// LabelPercentage
			// 
			this.LabelPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LabelPercentage.AutoSize = true;
			this.LabelPercentage.Location = new System.Drawing.Point(337, 3);
			this.LabelPercentage.Name = "LabelPercentage";
			this.LabelPercentage.Size = new System.Drawing.Size(27, 13);
			this.LabelPercentage.TabIndex = 1;
			this.LabelPercentage.Text = "26%";
			this.LabelPercentage.Visible = false;
			// 
			// checkBox1
			// 
			this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(177, 0);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(89, 17);
			this.checkBox1.TabIndex = 2;
			this.checkBox1.Text = "WalkForward";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// sliderComboControl1
			// 
			this.sliderComboControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sliderComboControl1.ColorBgMouseOverEnabled = System.Drawing.Color.Thistle;
			this.sliderComboControl1.ColorBgValueCurrent = System.Drawing.Color.LightSteelBlue;
			this.sliderComboControl1.ColorFgParameterLabel = System.Drawing.Color.White;
			this.sliderComboControl1.ColorFgValues = System.Drawing.Color.DeepPink;
			this.sliderComboControl1.EnableBorder = false;
			this.sliderComboControl1.EnableNumeric = false;
			this.sliderComboControl1.FilledPercentageCurrentValue = 0;
			this.sliderComboControl1.FilledPercentageMouseOver = 0;
			this.sliderComboControl1.LabelText = "% Backtest Calculated";
			this.sliderComboControl1.Location = new System.Drawing.Point(5, 0);
			this.sliderComboControl1.MaximumSize = new System.Drawing.Size(0, 16);
			this.sliderComboControl1.MinimumSize = new System.Drawing.Size(160, 16);
			this.sliderComboControl1.Name = "sliderComboControl1";
			this.sliderComboControl1.PaddingPanelSlider = new System.Windows.Forms.Padding(0);
			this.sliderComboControl1.Size = new System.Drawing.Size(160, 16);
			this.sliderComboControl1.TabIndex = 3;
			this.sliderComboControl1.ValueCurrent = new decimal(89);
			this.sliderComboControl1.ValueIncrement = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.sliderComboControl1.ValueMax = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.sliderComboControl1.ValueMin = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// TrackBarControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.sliderComboControl1);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.LabelPercentage);
			this.Controls.Add(this.TrackBar);
			this.MaximumSize = new System.Drawing.Size(0, 17);
			this.MinimumSize = new System.Drawing.Size(260, 17);
			this.Name = "TrackBarControl";
			this.Size = new System.Drawing.Size(260, 17);
			((System.ComponentModel.ISupportInitialize)(this.TrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label LabelPercentage;
		public System.Windows.Forms.TrackBar TrackBar;
		private System.Windows.Forms.CheckBox checkBox1;
		private SteppingSlider.SliderComboControl sliderComboControl1;
	}
}
