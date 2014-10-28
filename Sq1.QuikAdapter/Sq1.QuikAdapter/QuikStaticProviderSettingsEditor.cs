using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Sq1.QuikAdapter {
	[ToolboxBitmap(typeof(QuikStaticProviderSettingsEditor), "StaticQuik")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public class QuikStaticProviderSettingsEditor : UserControl {
		private Label lblDummy;

		public QuikStaticProviderSettingsEditor() {
			InitializeComponent();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.lblDummy = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblDummy
			// 
			this.lblDummy.AutoSize = true;
			this.lblDummy.Location = new System.Drawing.Point(3, 14);
			this.lblDummy.Name = "lblDummy";
			this.lblDummy.Size = new System.Drawing.Size(133, 13);
			this.lblDummy.TabIndex = 0;
			this.lblDummy.Text = "QuikStaticProviderSettings";
			// 
			// QuikStaticProviderSettingsUC
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.lblDummy);
			this.Location = new System.Drawing.Point(5, 20);
			this.Name = "QuikStaticProviderSettingsUC";
			this.Size = new System.Drawing.Size(381, 178);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}