using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik {
	[ToolboxBitmap(typeof(StreamingQuikEditor), "StreamingQuik")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class StreamingQuikEditor : StreamingEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private Label lblDdePrefix;
		private TextBox txtTopicQuotes;
		private Label lblTopicQuotes;
		private Label lblMinus;
		private TextBox txtTopicTrades;
		private Label lblTopicTrades;
		private Label lblMinus2;
		private Label lblMinus3;
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
			this.lblMinus3 = new System.Windows.Forms.Label();
			this.txtTopicPrefixDOM = new System.Windows.Forms.TextBox();
			this.lblTopicPrefixDOM = new System.Windows.Forms.Label();
			this.lblMinus4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblDdePrefix
			// 
			this.lblDdePrefix.AutoSize = true;
			this.lblDdePrefix.Location = new System.Drawing.Point(0, 0);
			this.lblDdePrefix.Name = "lblDdePrefix";
			this.lblDdePrefix.Size = new System.Drawing.Size(90, 13);
			this.lblDdePrefix.TabIndex = 3;
			this.lblDdePrefix.Text = "DDE server prefix";
			// 
			// txtDdeServerPrefix
			// 
			this.txtDdeServerPrefix.Location = new System.Drawing.Point(3, 16);
			this.txtDdeServerPrefix.Name = "txtDdeServerPrefix";
			this.txtDdeServerPrefix.Size = new System.Drawing.Size(114, 20);
			this.txtDdeServerPrefix.TabIndex = 8;
			// 
			// txtTopicQuotes
			// 
			this.txtTopicQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTopicQuotes.Location = new System.Drawing.Point(123, 16);
			this.txtTopicQuotes.Name = "txtTopicQuotes";
			this.txtTopicQuotes.Size = new System.Drawing.Size(92, 20);
			this.txtTopicQuotes.TabIndex = 10;
			// 
			// lblTopicQuotes
			// 
			this.lblTopicQuotes.AutoSize = true;
			this.lblTopicQuotes.Location = new System.Drawing.Point(120, 0);
			this.lblTopicQuotes.Name = "lblTopicQuotes";
			this.lblTopicQuotes.Size = new System.Drawing.Size(71, 13);
			this.lblTopicQuotes.TabIndex = 9;
			this.lblTopicQuotes.Text = "Quotes Topic";
			// 
			// lblMinus
			// 
			this.lblMinus.AutoSize = true;
			this.lblMinus.Location = new System.Drawing.Point(115, 19);
			this.lblMinus.Name = "lblMinus";
			this.lblMinus.Size = new System.Drawing.Size(10, 13);
			this.lblMinus.TabIndex = 11;
			this.lblMinus.Text = "-";
			// 
			// txtTopicTrades
			// 
			this.txtTopicTrades.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTopicTrades.Enabled = false;
			this.txtTopicTrades.Location = new System.Drawing.Point(123, 55);
			this.txtTopicTrades.Name = "txtTopicTrades";
			this.txtTopicTrades.Size = new System.Drawing.Size(92, 20);
			this.txtTopicTrades.TabIndex = 13;
			// 
			// lblTopicTrades
			// 
			this.lblTopicTrades.AutoSize = true;
			this.lblTopicTrades.Location = new System.Drawing.Point(129, 39);
			this.lblTopicTrades.Name = "lblTopicTrades";
			this.lblTopicTrades.Size = new System.Drawing.Size(86, 13);
			this.lblTopicTrades.TabIndex = 12;
			this.lblTopicTrades.Text = "Historical Trades";
			// 
			// lblMinus2
			// 
			this.lblMinus2.AutoSize = true;
			this.lblMinus2.Location = new System.Drawing.Point(115, 58);
			this.lblMinus2.Name = "lblMinus2";
			this.lblMinus2.Size = new System.Drawing.Size(10, 13);
			this.lblMinus2.TabIndex = 14;
			this.lblMinus2.Text = "-";
			// 
			// lblMinus3
			// 
			this.lblMinus3.AutoSize = true;
			this.lblMinus3.Location = new System.Drawing.Point(115, 97);
			this.lblMinus3.Name = "lblMinus3";
			this.lblMinus3.Size = new System.Drawing.Size(10, 13);
			this.lblMinus3.TabIndex = 17;
			this.lblMinus3.Text = "-";
			// 
			// txtTopicPrefixDOM
			// 
			this.txtTopicPrefixDOM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTopicPrefixDOM.Enabled = false;
			this.txtTopicPrefixDOM.Location = new System.Drawing.Point(123, 94);
			this.txtTopicPrefixDOM.Name = "txtTopicPrefixDOM";
			this.txtTopicPrefixDOM.Size = new System.Drawing.Size(92, 20);
			this.txtTopicPrefixDOM.TabIndex = 16;
			// 
			// lblTopicPrefixDOM
			// 
			this.lblTopicPrefixDOM.AutoSize = true;
			this.lblTopicPrefixDOM.Location = new System.Drawing.Point(102, 78);
			this.lblTopicPrefixDOM.Name = "lblTopicPrefixDOM";
			this.lblTopicPrefixDOM.Size = new System.Drawing.Size(113, 13);
			this.lblTopicPrefixDOM.TabIndex = 15;
			this.lblTopicPrefixDOM.Text = "Depth Of Market suffix";
			// 
			// lblMinus4
			// 
			this.lblMinus4.AutoSize = true;
			this.lblMinus4.Location = new System.Drawing.Point(91, 97);
			this.lblMinus4.Name = "lblMinus4";
			this.lblMinus4.Size = new System.Drawing.Size(10, 13);
			this.lblMinus4.TabIndex = 18;
			this.lblMinus4.Text = "-";
			// 
			// StreamingQuikEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
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
			this.Controls.Add(this.lblMinus3);
			this.Controls.Add(this.lblMinus4);
			this.Name = "StreamingQuikEditor";
			this.Size = new System.Drawing.Size(220, 119);
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
    }
}