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
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.flowLayoutPanel1.Controls.Add(this.oneParameterControl1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(899, 1256);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// oneParameterControl1
			// 
			this.oneParameterControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.oneParameterControl1.Location = new System.Drawing.Point(3, 3);
			this.oneParameterControl1.Name = "oneParameterControl1";
			this.oneParameterControl1.Size = new System.Drawing.Size(890, 355);
			this.oneParameterControl1.TabIndex = 0;
			// 
			// AllParametersControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "AllParametersControl";
			this.Size = new System.Drawing.Size(899, 1256);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private OneParameterControl oneParameterControl1;
	}
}
