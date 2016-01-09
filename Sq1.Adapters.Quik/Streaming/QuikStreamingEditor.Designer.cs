using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik.Streaming {
	[ToolboxBitmap(typeof(QuikStreamingEditor), "StreamingQuik")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class QuikStreamingEditor : StreamingEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private Label lblDdePrefix;
		private TextBox txtTopicQuotes;
		private Label lblTopicQuotes;
		private Label lblMinus;
		private TextBox txtTopicTrades;
		private Label lblTopicTrades;
		private Label lblMinus2;
		private TextBox txtTopicPrefixDOM;
		private Label lblTopicPrefixDOM;
		private Label lblMinus4;
		private TextBox txtDdeServerPrefix;
		void InitializeComponent() {
			this.lblDdePrefix = new System.Windows.Forms.Label();
			this.txtDdeServerPrefix = new System.Windows.Forms.TextBox();
			this.txtTopicQuotes = new System.Windows.Forms.TextBox();
			this.lblTopicQuotes = new System.Windows.Forms.Label();
			this.lblMinus = new System.Windows.Forms.Label();
			this.txtTopicTrades = new System.Windows.Forms.TextBox();
			this.lblTopicTrades = new System.Windows.Forms.Label();
			this.lblMinus2 = new System.Windows.Forms.Label();
			this.txtTopicPrefixDOM = new System.Windows.Forms.TextBox();
			this.lblTopicPrefixDOM = new System.Windows.Forms.Label();
			this.lblMinus4 = new System.Windows.Forms.Label();
			this.cbxStartDde = new System.Windows.Forms.CheckBox();
			this.lnkDdeMonitor = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lblDdePrefix
			// 
			this.lblDdePrefix.AutoSize = true;
			this.lblDdePrefix.Location = new System.Drawing.Point(0, 6);
			this.lblDdePrefix.Name = "lblDdePrefix";
			this.lblDdePrefix.Size = new System.Drawing.Size(90, 13);
			this.lblDdePrefix.TabIndex = 3;
			this.lblDdePrefix.Text = "DDE server prefix";
			// 
			// txtDdeServerPrefix
			// 
			this.txtDdeServerPrefix.Location = new System.Drawing.Point(3, 22);
			this.txtDdeServerPrefix.Name = "txtDdeServerPrefix";
			this.txtDdeServerPrefix.Size = new System.Drawing.Size(89, 20);
			this.txtDdeServerPrefix.TabIndex = 8;
			this.txtDdeServerPrefix.TextChanged += new System.EventHandler(this.txtDdeServerPrefix_TextChanged);
			// 
			// txtTopicQuotes
			// 
			this.txtTopicQuotes.Location = new System.Drawing.Point(102, 22);
			this.txtTopicQuotes.Name = "txtTopicQuotes";
			this.txtTopicQuotes.Size = new System.Drawing.Size(108, 20);
			this.txtTopicQuotes.TabIndex = 10;
			// 
			// lblTopicQuotes
			// 
			this.lblTopicQuotes.AutoSize = true;
			this.lblTopicQuotes.Location = new System.Drawing.Point(99, 6);
			this.lblTopicQuotes.Name = "lblTopicQuotes";
			this.lblTopicQuotes.Size = new System.Drawing.Size(98, 13);
			this.lblTopicQuotes.TabIndex = 9;
			this.lblTopicQuotes.Text = "Quotes Topic suffix";
			// 
			// lblMinus
			// 
			this.lblMinus.AutoSize = true;
			this.lblMinus.Location = new System.Drawing.Point(92, 25);
			this.lblMinus.Name = "lblMinus";
			this.lblMinus.Size = new System.Drawing.Size(10, 13);
			this.lblMinus.TabIndex = 11;
			this.lblMinus.Text = "-";
			// 
			// txtTopicTrades
			// 
			this.txtTopicTrades.Location = new System.Drawing.Point(102, 100);
			this.txtTopicTrades.Name = "txtTopicTrades";
			this.txtTopicTrades.Size = new System.Drawing.Size(108, 20);
			this.txtTopicTrades.TabIndex = 13;
			// 
			// lblTopicTrades
			// 
			this.lblTopicTrades.AutoSize = true;
			this.lblTopicTrades.Location = new System.Drawing.Point(99, 84);
			this.lblTopicTrades.Name = "lblTopicTrades";
			this.lblTopicTrades.Size = new System.Drawing.Size(113, 13);
			this.lblTopicTrades.TabIndex = 12;
			this.lblTopicTrades.Text = "Historical Trades suffix";
			// 
			// lblMinus2
			// 
			this.lblMinus2.Location = new System.Drawing.Point(3, 103);
			this.lblMinus2.Name = "lblMinus2";
			this.lblMinus2.Size = new System.Drawing.Size(99, 13);
			this.lblMinus2.TabIndex = 14;
			this.lblMinus2.Text = "-";
			this.lblMinus2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtTopicPrefixDOM
			// 
			this.txtTopicPrefixDOM.Location = new System.Drawing.Point(102, 61);
			this.txtTopicPrefixDOM.Name = "txtTopicPrefixDOM";
			this.txtTopicPrefixDOM.Size = new System.Drawing.Size(108, 20);
			this.txtTopicPrefixDOM.TabIndex = 16;
			// 
			// lblTopicPrefixDOM
			// 
			this.lblTopicPrefixDOM.AutoSize = true;
			this.lblTopicPrefixDOM.Location = new System.Drawing.Point(99, 45);
			this.lblTopicPrefixDOM.Name = "lblTopicPrefixDOM";
			this.lblTopicPrefixDOM.Size = new System.Drawing.Size(113, 13);
			this.lblTopicPrefixDOM.TabIndex = 15;
			this.lblTopicPrefixDOM.Text = "Depth Of Market suffix";
			// 
			// lblMinus4
			// 
			this.lblMinus4.Location = new System.Drawing.Point(6, 64);
			this.lblMinus4.Name = "lblMinus4";
			this.lblMinus4.Size = new System.Drawing.Size(96, 13);
			this.lblMinus4.TabIndex = 18;
			this.lblMinus4.Text = "-SYMBOL-";
			this.lblMinus4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbxStartDde
			// 
			this.cbxStartDde.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxStartDde.Location = new System.Drawing.Point(3, 126);
			this.cbxStartDde.Name = "cbxStartDde";
			this.cbxStartDde.Size = new System.Drawing.Size(207, 23);
			this.cbxStartDde.TabIndex = 20;
			this.cbxStartDde.Text = "Start DDE Server";
			this.cbxStartDde.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbxStartDde.UseVisualStyleBackColor = true;
			this.cbxStartDde.CheckedChanged += new System.EventHandler(this.cbxStartDde_CheckedChanged);
			// 
			// lnkDdeMonitor
			// 
			this.lnkDdeMonitor.AutoSize = true;
			this.lnkDdeMonitor.Location = new System.Drawing.Point(71, 152);
			this.lnkDdeMonitor.Name = "lnkDdeMonitor";
			this.lnkDdeMonitor.Size = new System.Drawing.Size(68, 13);
			this.lnkDdeMonitor.TabIndex = 21;
			this.lnkDdeMonitor.TabStop = true;
			this.lnkDdeMonitor.Text = "DDE Monitor";
			this.lnkDdeMonitor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDdeMonitor_LinkClicked);
			// 
			// QuikStreamingEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.lnkDdeMonitor);
			this.Controls.Add(this.cbxStartDde);
			this.Controls.Add(this.txtTopicPrefixDOM);
			this.Controls.Add(this.lblTopicPrefixDOM);
			this.Controls.Add(this.txtTopicTrades);
			this.Controls.Add(this.lblTopicTrades);
			this.Controls.Add(this.txtTopicQuotes);
			this.Controls.Add(this.lblTopicQuotes);
			this.Controls.Add(this.txtDdeServerPrefix);
			this.Controls.Add(this.lblDdePrefix);
			this.Controls.Add(this.lblMinus);
			this.Controls.Add(this.lblMinus2);
			this.Controls.Add(this.lblMinus4);
			this.Name = "QuikStreamingEditor";
			this.Size = new System.Drawing.Size(214, 169);
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

		private CheckBox cbxStartDde;
		private LinkLabel lnkDdeMonitor;
    }
}