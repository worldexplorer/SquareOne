namespace Sq1.Gui.Forms {
	partial class LivesimForm {
		private System.ComponentModel.IContainer components = null;
		public Sq1.Widgets.Livesim.LivesimControl LivesimControl;

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.LivesimControl = new Sq1.Widgets.Livesim.LivesimControl();
			this.SuspendLayout();
			// 
			// LivesimControl
			// 
			this.LivesimControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LivesimControl.Location = new System.Drawing.Point(0, 0);
			this.LivesimControl.Name = "LivesimControl";
			this.LivesimControl.Size = new System.Drawing.Size(681, 538);
			this.LivesimControl.TabIndex = 0;
			// 
			// LivesimForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(681, 538);
			this.Controls.Add(this.LivesimControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "LivesimForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide;
			this.Text = "LivesimForm";
			this.ResumeLayout(false);

		}
	}
}
