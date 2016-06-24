namespace Sq1.Gui.Singletons {
	partial class SymbolInfoEditorForm {
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
			this.SymbolEditorControl = new Sq1.Widgets.SymbolEditor.SymbolInfoEditorControl();
			this.SuspendLayout();
			// 
			// SymbolEditorControl
			// 
			this.SymbolEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SymbolEditorControl.Location = new System.Drawing.Point(0, 0);
			this.SymbolEditorControl.Name = "SymbolEditorControl";
			this.SymbolEditorControl.Size = new System.Drawing.Size(205, 393);
			this.SymbolEditorControl.TabIndex = 0;
			// 
			// SymbolInfoEditorForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(205, 393);
			this.Controls.Add(this.SymbolEditorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HideOnClose = false;
			this.Name = "SymbolInfoEditorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Text = "Symbol Editor";
			this.ResumeLayout(false);

		}

		#endregion

		public Widgets.SymbolEditor.SymbolInfoEditorControl SymbolEditorControl;
	}
}