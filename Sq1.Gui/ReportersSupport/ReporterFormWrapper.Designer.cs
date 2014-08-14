namespace Sq1.Gui.ReportersSupport {
	partial class ReporterFormWrapper {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// ReporterFormWrapper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(381, 229);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "ReporterFormWrapper";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
			this.Text = "ReporterForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReporterFormWrapper_FormClosing);
			this.ResumeLayout(false);

		}
	}
}
