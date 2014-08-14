namespace Sq1.Gui.Singletons {
	partial class StrategiesForm {
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
			this.StrategiesTreeControl = new Sq1.Widgets.StrategiesTree.StrategiesTreeControl();
			this.SuspendLayout();
			// 
			// dataSourcesTree
			// 
			this.StrategiesTreeControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.StrategiesTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StrategiesTreeControl.Location = new System.Drawing.Point(0, 0);
			this.StrategiesTreeControl.Name = "StrategiesTree";
			this.StrategiesTreeControl.Size = new System.Drawing.Size(284, 262);
			this.StrategiesTreeControl.TabIndex = 0;
			// 
			// StrategiesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.StrategiesTreeControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "StrategiesForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
			this.Text = "Strategies";
			this.ResumeLayout(false);

		}

		#endregion

		public Sq1.Widgets.StrategiesTree.StrategiesTreeControl StrategiesTreeControl;
	}
}