namespace Sq1.Gui.Singletons {
	partial class SymbolsEditorForm {
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
			this.symbolEditorControl1 = new Sq1.Widgets.SymbolEditor.SymbolEditorControl();
			this.SuspendLayout();
			// 
			// symbolEditorControl1
			// 
			this.symbolEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.symbolEditorControl1.Location = new System.Drawing.Point(0, 0);
			this.symbolEditorControl1.Name = "symbolEditorControl1";
			this.symbolEditorControl1.Size = new System.Drawing.Size(1171, 205);
			this.symbolEditorControl1.TabIndex = 0;
			// 
			// SymbolsEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1171, 205);
			this.Controls.Add(this.symbolEditorControl1);
			this.Name = "SymbolsEditorForm";
			this.Text = "SymbolsEditorForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Widgets.SymbolEditor.SymbolEditorControl symbolEditorControl1;
	}
}