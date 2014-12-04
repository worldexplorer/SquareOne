/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 03-Dec-14
 * Time: 10:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sq1.Gui.Forms
{
	partial class LivesimForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Sq1.Widgets.LiveSim.LiveSimControl liveSimControl;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.liveSimControl = new Sq1.Widgets.LiveSim.LiveSimControl();
			this.SuspendLayout();
			// 
			// liveSimControl1
			// 
			this.liveSimControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.liveSimControl.Location = new System.Drawing.Point(0, 0);
			this.liveSimControl.Name = "liveSimControl1";
			this.liveSimControl.Size = new System.Drawing.Size(917, 790);
			this.liveSimControl.TabIndex = 0;
			// 
			// LivesimForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(917, 790);
			this.Controls.Add(this.liveSimControl);
			this.Name = "LivesimForm";
			this.Text = "LivesimForm";
			this.ResumeLayout(false);

		}
	}
}
