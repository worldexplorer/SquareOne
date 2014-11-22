namespace Sq1.Widgets.LabeledTextBox {
	partial class LabeledTextBoxControl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.Label = new System.Windows.Forms.Label();
			this.TextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// Label
			// 
			this.Label.AutoSize = true;
			this.Label.Location = new System.Drawing.Point(0, 3);
			this.Label.Name = "Label";
			this.Label.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.Label.Size = new System.Drawing.Size(73, 13);
			this.Label.TabIndex = 0;
			this.Label.Text = "Duplicate To:";
			// 
			// TextBox
			// 
			this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.TextBox.BackColor = System.Drawing.SystemColors.Info;
			this.TextBox.Location = new System.Drawing.Point(90, 1);
			this.TextBox.Multiline = true;
			this.TextBox.Name = "TextBox";
			this.TextBox.Size = new System.Drawing.Size(117, 19);
			this.TextBox.TabIndex = 1;
			// 
			// LabeledTextBoxControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.TextBox);
			this.Controls.Add(this.Label);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "LabeledTextBoxControl";
			this.Size = new System.Drawing.Size(210, 21);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label Label;
		public System.Windows.Forms.TextBox TextBox;
	}
}
