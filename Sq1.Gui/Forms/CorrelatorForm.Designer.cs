namespace Sq1.Gui.Forms {
	partial class CorrelatorForm {
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
			this.CorrelatorControl = new Sq1.Widgets.Correlation.CorrelatorControl();
			this.SuspendLayout();
			// 
			// CorrelatorControl
			// 
			this.CorrelatorControl.AutoScroll = true;
			this.CorrelatorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CorrelatorControl.Location = new System.Drawing.Point(0, 0);
			this.CorrelatorControl.Name = "CorrelatorControl";
			this.CorrelatorControl.Size = new System.Drawing.Size(877, 774);
			this.CorrelatorControl.TabIndex = 0;
			// 
			// CorrelatorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(877, 774);
			this.Controls.Add(this.CorrelatorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "CorrelatorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Text = "AllParametersForm";
			this.ResumeLayout(false);

		}

		#endregion

		public Sq1.Widgets.Correlation.CorrelatorControl CorrelatorControl;
	}
}