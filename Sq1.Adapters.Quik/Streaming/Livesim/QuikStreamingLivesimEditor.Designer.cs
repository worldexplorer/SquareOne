using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik.Streaming.Livesim {
	[ToolboxBitmap(typeof(QuikStreamingLivesimEditor), "QuikStreamingLivesimEditor")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]

	public partial class QuikStreamingLivesimEditor : StreamingEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private void InitializeComponent() {
			this.cbxDdeServerConnect = new System.Windows.Forms.CheckBox();
			this.lblDdeServerFound = new System.Windows.Forms.Label();
			this.lblDdeServerConnected = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbxDdeServerConnect
			// 
			this.cbxDdeServerConnect.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxDdeServerConnect.AutoSize = true;
			this.cbxDdeServerConnect.Location = new System.Drawing.Point(29, 29);
			this.cbxDdeServerConnect.Name = "cbxDdeServerConnect";
			this.cbxDdeServerConnect.Size = new System.Drawing.Size(201, 23);
			this.cbxDdeServerConnect.TabIndex = 0;
			this.cbxDdeServerConnect.Text = "Connect to QuikStreaming DDE Server";
			this.cbxDdeServerConnect.UseVisualStyleBackColor = true;
			// 
			// lblDdeServerFound
			// 
			this.lblDdeServerFound.AutoSize = true;
			this.lblDdeServerFound.Location = new System.Drawing.Point(3, 0);
			this.lblDdeServerFound.Name = "lblDdeServerFound";
			this.lblDdeServerFound.Size = new System.Drawing.Size(249, 13);
			this.lblDdeServerFound.TabIndex = 1;
			this.lblDdeServerFound.Text = "DDE server: \"QuickStreaming waiting for incoming\"";
			// 
			// lblDdeServerConnected
			// 
			this.lblDdeServerConnected.AutoSize = true;
			this.lblDdeServerConnected.Location = new System.Drawing.Point(3, 55);
			this.lblDdeServerConnected.Name = "lblDdeServerConnected";
			this.lblDdeServerConnected.Size = new System.Drawing.Size(158, 13);
			this.lblDdeServerConnected.TabIndex = 2;
			this.lblDdeServerConnected.Text = "Livesim: click CONNECT button";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(210, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Expects: \"lastQuotes, lastDeals, accounts\"";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Generates: lastQuotes";
			// 
			// QuikStreamingLivesimEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblDdeServerConnected);
			this.Controls.Add(this.lblDdeServerFound);
			this.Controls.Add(this.cbxDdeServerConnect);
			this.Name = "QuikStreamingLivesimEditor";
			this.Size = new System.Drawing.Size(302, 105);
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

		private System.Windows.Forms.CheckBox cbxDdeServerConnect;
		private System.Windows.Forms.Label lblDdeServerFound;
		private System.Windows.Forms.Label lblDdeServerConnected;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;


	}
}