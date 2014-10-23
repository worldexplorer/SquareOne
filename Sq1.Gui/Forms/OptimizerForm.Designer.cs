namespace Sq1.Gui.Forms {
	partial class OptimizerForm {
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
			this.optimizerControl = new Sq1.Widgets.Optimizer.OptimizerControl();
			this.SuspendLayout();
			// 
			// optimizerControl1
			// 
			this.optimizerControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.optimizerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optimizerControl.Location = new System.Drawing.Point(0, 0);
			this.optimizerControl.Name = "optimizerControl1";
			this.optimizerControl.Size = new System.Drawing.Size(533, 401);
			this.optimizerControl.TabIndex = 0;
			// 
			// OptimizerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(533, 401);
			this.Controls.Add(this.optimizerControl);
			this.Name = "OptimizerForm";
			this.Text = "OptimizerForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide;
			this.ResumeLayout(false);
		}
		private Sq1.Widgets.Optimizer.OptimizerControl optimizerControl;
	}
}
