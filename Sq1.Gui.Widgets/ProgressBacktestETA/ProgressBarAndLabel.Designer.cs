namespace Sq1.Widgets.ProgressBacktestETA {
	partial class ProgressBarAndLabelControl {
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
			this.ProgressBarETA = new System.Windows.Forms.ProgressBar();
			this.LabelETA = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ProgressBarETA
			// 
			this.ProgressBarETA.Location = new System.Drawing.Point(13, 3);
			this.ProgressBarETA.Name = "ProgressBarETA";
			this.ProgressBarETA.Size = new System.Drawing.Size(84, 13);
			this.ProgressBarETA.TabIndex = 0;
			this.ProgressBarETA.Value = 25;
			// 
			// LabelETA
			// 
			this.LabelETA.Location = new System.Drawing.Point(103, 2);
			this.LabelETA.Name = "LabelETA";
			this.LabelETA.Size = new System.Drawing.Size(86, 13);
			this.LabelETA.TabIndex = 1;
			this.LabelETA.Text = "400/1600";
			this.LabelETA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ProgressBarAndLabel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.LabelETA);
			this.Controls.Add(this.ProgressBarETA);
			this.Name = "ProgressBarAndLabel";
			this.Size = new System.Drawing.Size(192, 18);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ProgressBar ProgressBarETA;
		public System.Windows.Forms.Label LabelETA;
	}
}
