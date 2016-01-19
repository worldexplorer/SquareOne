namespace Sq1.Adapters.Quik.Streaming.Monitor {
	partial class QuikStreamingMonitorForm {
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
			this.QuikStreamingMonitorControl = new Sq1.Adapters.Quik.Streaming.Monitor.QuikStreamingMonitorControl();
			this.SuspendLayout();
			// 
			// QuikStreamingMonitorControl
			// 
			this.QuikStreamingMonitorControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.QuikStreamingMonitorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.QuikStreamingMonitorControl.Location = new System.Drawing.Point(0, 0);
			this.QuikStreamingMonitorControl.Name = "QuikStreamingMonitorControl";
			this.QuikStreamingMonitorControl.Size = new System.Drawing.Size(937, 623);
			this.QuikStreamingMonitorControl.TabIndex = 0;
			// 
			// QuikStreamingMonitorForm
			// 
			this.AutoHidePortion = 0.4D;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(937, 623);
			this.Controls.Add(this.QuikStreamingMonitorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "QuikStreamingMonitorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Text = "QUIK :: MOCK [Sq1-quotes]";
			this.ResumeLayout(false);

		}

		#endregion

		public QuikStreamingMonitorControl QuikStreamingMonitorControl;

	}
}