namespace Sq1.Gui.Singletons {
	partial class CsvImporterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.csvImporterControl = new Sq1.Widgets.CsvImporter.CsvImporterControl();
			this.SuspendLayout();
			// 
			// csvImporterControl
			// 
			this.csvImporterControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.csvImporterControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.csvImporterControl.Location = new System.Drawing.Point(0, 0);
			this.csvImporterControl.Name = "csvImporterControl";
			this.csvImporterControl.Size = new System.Drawing.Size(812, 451);
			this.csvImporterControl.TabIndex = 0;
			// 
			// CsvImporterForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(812, 451);
			this.Controls.Add(this.csvImporterControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "CsvImporterForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CsvImporter";
			this.HideOnClose = false;
			this.ResumeLayout(false);
		}
		private Sq1.Widgets.CsvImporter.CsvImporterControl csvImporterControl;
	}
}