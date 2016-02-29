namespace Sq1.Gui.Singletons {
	partial class BarsEditorForm {
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
			this.BarsEditorUserControl = new Sq1.Widgets.FuturesMerger.BarsEditorUserControl();
			this.SuspendLayout();
			// 
			// BarsEditorUserControl
			// 
			this.BarsEditorUserControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.BarsEditorUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BarsEditorUserControl.Location = new System.Drawing.Point(0, 0);
			this.BarsEditorUserControl.Margin = new System.Windows.Forms.Padding(0);
			this.BarsEditorUserControl.Name = "BarsEditorUserControl";
			this.BarsEditorUserControl.Size = new System.Drawing.Size(549, 320);
			this.BarsEditorUserControl.TabIndex = 0;
			// 
			// BarsEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(549, 320);
			this.Controls.Add(this.BarsEditorUserControl);
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Name = "BarsEditorForm";
			this.Text = "Bars Editor";
			this.ResumeLayout(false);

		}

		#endregion

		public Widgets.FuturesMerger.BarsEditorUserControl BarsEditorUserControl;
	}
}