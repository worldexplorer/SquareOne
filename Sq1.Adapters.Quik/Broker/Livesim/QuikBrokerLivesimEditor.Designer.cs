using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik.Broker.Livesim {
	[ToolboxBitmap(typeof(QuikBrokerLivesimEditor), "QuikBrokerLivesimEditor")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]

	public partial class QuikBrokerLivesimEditor : BrokerEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private void InitializeComponent() {
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(3, 3);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(147, 23);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "WHAT DO I NEED HERE?";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// QuikBrokerLivesimEditor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.checkBox1);
			this.Name = "QuikBrokerLivesimEditor";
			this.Size = new System.Drawing.Size(251, 85);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion

		private System.Windows.Forms.CheckBox checkBox1;
	}
}