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
			this.LabelLeft = new System.Windows.Forms.Label();
			this.TextBox = new System.Windows.Forms.TextBox();
			this.LabelRight = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Label
			// 
			this.LabelLeft.AutoSize = true;
			this.LabelLeft.Location = new System.Drawing.Point(0, 3);
			this.LabelLeft.Name = "Label";
			this.LabelLeft.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.LabelLeft.Size = new System.Drawing.Size(80, 13);
			this.LabelLeft.TabIndex = 0;
			this.LabelLeft.Text = "Spread, %price";
			// 
			// TextBox
			// 
			this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.TextBox.BackColor = System.Drawing.SystemColors.Info;
			this.TextBox.Location = new System.Drawing.Point(90, 1);
			this.TextBox.Multiline = true;
			this.TextBox.Name = "TextBox";
			this.TextBox.Size = new System.Drawing.Size(46, 19);
			this.TextBox.TabIndex = 1;
			this.TextBox.Text = "0.0005";
			// 
			// Comment
			// 
			this.LabelRight.AutoSize = true;
			this.LabelRight.Location = new System.Drawing.Point(137, 3);
			this.LabelRight.Name = "Comment";
			this.LabelRight.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.LabelRight.Size = new System.Drawing.Size(62, 13);
			this.LabelRight.TabIndex = 2;
			this.LabelRight.Text = "~= 121pips";
			// 
			// LabeledTextBoxControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.LabelRight);
			this.Controls.Add(this.TextBox);
			this.Controls.Add(this.LabelLeft);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "LabeledTextBoxControl";
			this.Size = new System.Drawing.Size(219, 21);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		public System.Windows.Forms.Label LabelRight;

		#endregion

		public System.Windows.Forms.Label LabelLeft;
		public System.Windows.Forms.TextBox TextBox;
	}
}
