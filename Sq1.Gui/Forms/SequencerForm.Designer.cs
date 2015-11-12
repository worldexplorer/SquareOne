using Sq1.Widgets.Sequencing;

namespace Sq1.Gui.Forms {
	partial class SequencerForm {
		private System.ComponentModel.IContainer components = null;
		public SequencerControl SequencerControl;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.SequencerControl = new Sq1.Widgets.Sequencing.SequencerControl();
			this.SuspendLayout();
			// 
			// sequencerControl1
			// 
			this.SequencerControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.SequencerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SequencerControl.Location = new System.Drawing.Point(0, 0);
			this.SequencerControl.Name = "sequencerControl1";
			this.SequencerControl.Size = new System.Drawing.Size(533, 401);
			this.SequencerControl.TabIndex = 0;
			// 
			// SequencerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(533, 401);
			this.Controls.Add(this.SequencerControl);
			this.Name = "SequencerForm";
			this.Text = "SequencerForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide;
			this.ResumeLayout(false);
		}
	}
}
