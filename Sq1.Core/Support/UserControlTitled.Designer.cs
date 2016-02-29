namespace Sq1.Core.Support {
	partial class UserControlTitled {
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
			this.pnlWindowTitle = new System.Windows.Forms.Panel();
			this.lblWindowTitle = new System.Windows.Forms.Label();
			this.pnlWindowTitle.SuspendLayout();
			this.SuspendLayout();
			// 
			// UserControlInner
			// 
			this.UserControlInner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.UserControlInner.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
			this.UserControlInner.Size = new System.Drawing.Size(206, 402);
			// 
			// pnlWindowTitle
			// 
			this.pnlWindowTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlWindowTitle.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.pnlWindowTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlWindowTitle.Controls.Add(this.lblWindowTitle);
			this.pnlWindowTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.pnlWindowTitle.ForeColor = System.Drawing.SystemColors.Window;
			this.pnlWindowTitle.Location = new System.Drawing.Point(4, 4);
			this.pnlWindowTitle.Name = "pnlWindowTitle";
			this.pnlWindowTitle.Size = new System.Drawing.Size(206, 19);
			this.pnlWindowTitle.TabIndex = 1;
			// 
			// lblWindowTitle
			// 
			this.lblWindowTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblWindowTitle.AutoSize = true;
			this.lblWindowTitle.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblWindowTitle.Location = new System.Drawing.Point(3, 2);
			this.lblWindowTitle.Name = "lblWindowTitle";
			this.lblWindowTitle.Size = new System.Drawing.Size(232, 13);
			this.lblWindowTitle.TabIndex = 0;
			this.lblWindowTitle.Text = "SQ1-RIM3-dom [232323] RECEIVING#2";
			this.lblWindowTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
			this.lblWindowTitle.MouseEnter += new System.EventHandler(this.lblTitle_MouseEnter);
			this.lblWindowTitle.MouseLeave += new System.EventHandler(this.lblTitle_MouseLeave);
			this.lblWindowTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblWindowTitle_MouseMove);
			this.lblWindowTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseUp);
			// 
			// UserControlTitled
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlWindowTitle);
			this.Name = "UserControlTitled";
			this.Size = new System.Drawing.Size(214, 410);
			this.Controls.SetChildIndex(this.UserControlInner, 0);
			this.Controls.SetChildIndex(this.pnlWindowTitle, 0);
			this.pnlWindowTitle.ResumeLayout(false);
			this.pnlWindowTitle.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblWindowTitle;
		protected System.Windows.Forms.Panel pnlWindowTitle;
	}
}
