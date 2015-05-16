namespace Sq1.Widgets.WalkForward {
	partial class WalkForwardControl {
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
			this.Cbx_Walkforward = new System.Windows.Forms.CheckBox();
			this.SliderComboControl = new Sq1.Widgets.SteppingSlider.SliderComboControl();
			this.SuspendLayout();
			// 
			// Cbx_Walkforward
			// 
			this.Cbx_Walkforward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Cbx_Walkforward.AutoSize = true;
			this.Cbx_Walkforward.Location = new System.Drawing.Point(226, 0);
			this.Cbx_Walkforward.Name = "Cbx_Walkforward";
			this.Cbx_Walkforward.Size = new System.Drawing.Size(89, 17);
			this.Cbx_Walkforward.TabIndex = 2;
			this.Cbx_Walkforward.Text = "WalkForward";
			this.Cbx_Walkforward.UseVisualStyleBackColor = true;
			// 
			// SliderComboControl
			// 
			this.SliderComboControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SliderComboControl.ColorBgMouseOverEnabled = System.Drawing.Color.YellowGreen;
			this.SliderComboControl.ColorBgValueCurrent = System.Drawing.Color.LightSteelBlue;
			this.SliderComboControl.ColorFgParameterLabel = System.Drawing.Color.White;
			this.SliderComboControl.ColorFgValues = System.Drawing.Color.DeepPink;
			this.SliderComboControl.EnableBorder = false;
			this.SliderComboControl.EnableNumeric = true;
			this.SliderComboControl.FilledPercentageCurrentValue = 0;
			this.SliderComboControl.FilledPercentageMouseOver = 0;
			this.SliderComboControl.FillFromCurrentToMax = false;
			this.SliderComboControl.LabelText = "% Backtest Calculated";
			this.SliderComboControl.Location = new System.Drawing.Point(5, 0);
			this.SliderComboControl.MinimumSize = new System.Drawing.Size(210, 16);
			this.SliderComboControl.Name = "SliderComboControl";
			this.SliderComboControl.PaddingPanelSlider = new System.Windows.Forms.Padding(0);
			this.SliderComboControl.Size = new System.Drawing.Size(210, 17);
			this.SliderComboControl.TabIndex = 3;
			this.SliderComboControl.ValueCurrent = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.SliderComboControl.ValueIncrement = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.SliderComboControl.ValueMax = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.SliderComboControl.ValueMin = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// WalkForwardControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.SliderComboControl);
			this.Controls.Add(this.Cbx_Walkforward);
			this.MaximumSize = new System.Drawing.Size(0, 17);
			this.MinimumSize = new System.Drawing.Size(310, 17);
			this.Name = "WalkForwardControl";
			this.Size = new System.Drawing.Size(310, 17);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.CheckBox Cbx_Walkforward;
		public SteppingSlider.SliderComboControl SliderComboControl;
	}
}
