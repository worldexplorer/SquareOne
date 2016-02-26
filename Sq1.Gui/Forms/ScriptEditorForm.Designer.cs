namespace Sq1.Gui.Forms {
	partial class ScriptEditorForm {
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
			this.ScriptEditorControl = new Sq1.Widgets.ScriptEditor.ScriptEditor();
			this.SuspendLayout();
			// 
			// ScriptEditorControl
			// 
			this.ScriptEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ScriptEditorControl.Location = new System.Drawing.Point(0, 0);
			this.ScriptEditorControl.Name = "ScriptEditorControl";
			this.ScriptEditorControl.ScriptSourceCode = "textEditorControl1";
			this.ScriptEditorControl.Size = new System.Drawing.Size(407, 261);
			this.ScriptEditorControl.TabIndex = 0;
			// 
			// ScriptEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(407, 261);
			this.Controls.Add(this.ScriptEditorControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "ScriptEditorForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.Text = "ScriptEditorForm";
			this.ResumeLayout(false);

		}

		#endregion

		public Widgets.ScriptEditor.ScriptEditor ScriptEditorControl;
	}
}