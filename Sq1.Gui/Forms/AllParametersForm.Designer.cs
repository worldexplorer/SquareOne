namespace Sq1.Gui.Forms {
	partial class AllParametersForm {
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
			this.allParametersControl1 = new Sq1.Widgets.Correlation.AllParametersControl();
			this.SuspendLayout();
			// 
			// allParametersControl1
			// 
			this.allParametersControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.allParametersControl1.Location = new System.Drawing.Point(0, 0);
			this.allParametersControl1.Name = "allParametersControl1";
			this.allParametersControl1.Size = new System.Drawing.Size(877, 1302);
			this.allParametersControl1.TabIndex = 0;
			// 
			// AllParametersForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(877, 1302);
			this.Controls.Add(this.allParametersControl1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "AllParametersForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
			this.Text = "AllParametersForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Sq1.Widgets.Correlation.AllParametersControl allParametersControl1;
	}
}