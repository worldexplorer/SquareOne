namespace Sq1.Gui.Singletons {
	partial class FuturesMergerForm {
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
			this.FuturesMergerUserControl = new Sq1.Widgets.FuturesMerger.FuturesMergerUserControl();
			this.SuspendLayout();
			// 
			// FuturesMergerUserControl
			// 
			this.FuturesMergerUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FuturesMergerUserControl.Location = new System.Drawing.Point(0, 0);
			this.FuturesMergerUserControl.Name = "FuturesMergerUserControl";
			this.FuturesMergerUserControl.Size = new System.Drawing.Size(857, 290);
			this.FuturesMergerUserControl.TabIndex = 0;
			// 
			// FuturesMergerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(857, 290);
			this.Controls.Add(this.FuturesMergerUserControl);
			this.Name = "FuturesMergerForm";
			this.Text = "Futures Merger";
			this.ResumeLayout(false);

		}

		#endregion

		public Widgets.FuturesMerger.FuturesMergerUserControl FuturesMergerUserControl;
	}
}