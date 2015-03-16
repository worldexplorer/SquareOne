using System;

using Sq1.Widgets.Exceptions;

namespace Sq1.Charting.Demo {
	public partial class ExceptionsForm {
		#region Windows Form Designer generated code

		public ExceptionsControl ExceptionControl;
		//private PerformanceCounter performanceCounter1;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.ExceptionControl = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.SuspendLayout();
			// 
			// ExceptionControl
			// 
			this.ExceptionControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ExceptionControl.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.ExceptionControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ExceptionControl.Location = new System.Drawing.Point(0, 0);
			this.ExceptionControl.Margin = new System.Windows.Forms.Padding(0);
			this.ExceptionControl.Name = "ExceptionControl";
			this.ExceptionControl.Size = new System.Drawing.Size(612, 402);
			this.ExceptionControl.TabIndex = 5;
			// 
			// ExceptionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(612, 402);
			this.Controls.Add(this.ExceptionControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "ExceptionsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Exceptions";
			this.ResumeLayout(false);
		}
		#endregion
	}
}
