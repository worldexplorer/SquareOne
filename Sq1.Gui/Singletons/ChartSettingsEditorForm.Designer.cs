namespace Sq1.Gui.Singletons {
	partial class ChartSettingsEditorForm {
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
			this.ChartSettingsEditorControl = new Sq1.Charting.ChartSettingsEditorControl();
			this.SuspendLayout();
			// 
			// ChartSettingsEditorControl
			// 
			this.ChartSettingsEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChartSettingsEditorControl.Location = new System.Drawing.Point(0, 0);
			this.ChartSettingsEditorControl.Name = "ChartSettingsEditorControl";
			this.ChartSettingsEditorControl.Size = new System.Drawing.Size(205, 393);
			this.ChartSettingsEditorControl.TabIndex = 0;
			// 
			// ChartSettingsEditorForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(205, 393);
			this.Controls.Add(this.ChartSettingsEditorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HideOnClose = false;
			this.Name = "ChartSettingsEditorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Text = "Chart Editor";
			this.ResumeLayout(false);

		}

		#endregion

		public Sq1.Charting.ChartSettingsEditorControl ChartSettingsEditorControl;
	}
}