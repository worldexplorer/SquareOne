namespace Sq1.Gui.Forms {
	partial class OptimizerForm {
		private System.ComponentModel.IContainer components = null;
		public Sq1.Widgets.Optimization.OptimizerControl OptimizerControl;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.OptimizerControl = new Sq1.Widgets.Optimization.OptimizerControl();
			this.SuspendLayout();
			// 
			// optimizerControl1
			// 
			this.OptimizerControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.OptimizerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OptimizerControl.Location = new System.Drawing.Point(0, 0);
			this.OptimizerControl.Name = "optimizerControl1";
			this.OptimizerControl.Size = new System.Drawing.Size(533, 401);
			this.OptimizerControl.TabIndex = 0;
			// 
			// OptimizerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(533, 401);
			this.Controls.Add(this.OptimizerControl);
			this.Name = "OptimizerForm";
			this.Text = "OptimizerForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide;
			this.ResumeLayout(false);
		}
	}
}
