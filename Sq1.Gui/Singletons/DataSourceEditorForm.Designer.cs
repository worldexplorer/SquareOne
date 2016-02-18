using System.ComponentModel;

namespace Sq1.Gui.Singletons {
	public partial class DataSourceEditorForm {
		private IContainer components;
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.DataSourceEditorControl = new Sq1.Widgets.DataSourceEditor.DataSourceEditorControl();
			this.SuspendLayout();
			// 
			// dataSourceEditorControl1
			// 
			this.DataSourceEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataSourceEditorControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.DataSourceEditorControl.Location = new System.Drawing.Point(0, 0);
			this.DataSourceEditorControl.Name = "dataSourceEditorControl1";
			this.DataSourceEditorControl.Size = new System.Drawing.Size(737, 485);
			this.DataSourceEditorControl.TabIndex = 0;
			// 
			// DataSourceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(737, 485);
			this.Controls.Add(this.DataSourceEditorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DataSourceEditorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Creating New DataSource";
			this.ResumeLayout(false);
		}
		public Sq1.Widgets.DataSourceEditor.DataSourceEditorControl DataSourceEditorControl;
	}
}