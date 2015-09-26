namespace Sq1.Widgets.Correlation {
	partial class UserControlResizeable {
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
			this.UserControlInner = new Sq1.Widgets.Correlation.UserControlInner();
			this.SuspendLayout();
			// 
			// UserControlInner
			// 
			this.UserControlInner.BackColor = System.Drawing.SystemColors.Control;
			this.UserControlInner.Location = new System.Drawing.Point(0, 0);
			this.UserControlInner.Name = "UserControlInner";
			this.UserControlInner.Size = new System.Drawing.Size(497, 275);
			this.UserControlInner.TabIndex = 0;
			// 
			// UserControlResizeable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.UserControlInner);
			this.Name = "UserControlResizeable";
			this.Size = new System.Drawing.Size(501, 279);
			this.ResumeLayout(false);

		}

		#endregion

		public UserControlInner UserControlInner;


	}
}
