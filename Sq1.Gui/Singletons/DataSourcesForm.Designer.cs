namespace Sq1.Gui.Singletons {
	partial class DataSourcesForm {
		private System.ComponentModel.IContainer components = null;

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
			this.DataSourcesTreeControl = new Sq1.Widgets.DataSourcesTree.DataSourcesTreeControl();
			this.SuspendLayout();
			// 
			// DataSourcesTree
			// 
			this.DataSourcesTreeControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.DataSourcesTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataSourcesTreeControl.Location = new System.Drawing.Point(0, 0);
			this.DataSourcesTreeControl.Name = "DataSourcesTree";
			this.DataSourcesTreeControl.Size = new System.Drawing.Size(284, 262);
			this.DataSourcesTreeControl.TabIndex = 0;
			// 
			// DataSourcesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.DataSourcesTreeControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DataSourcesForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
			this.Text = "Data Sources";
			this.ResumeLayout(false);

		}

		#endregion

		public Sq1.Widgets.DataSourcesTree.DataSourcesTreeControl DataSourcesTreeControl;


	}
}