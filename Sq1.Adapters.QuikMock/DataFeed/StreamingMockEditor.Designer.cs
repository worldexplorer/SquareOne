using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock {
	[ToolboxBitmap(typeof(StreamingMockEditor), "StreamingMock")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class StreamingMockEditor : StreamingEditor {
        #region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private Label lblQuoteDelay;
		private TextBox txtQuoteDelay;
		private Label lblGenerateOnlySymbols;
		private TextBox txtGenerateOnlySymbols;
		private CheckBox cbxGeneratingNow;
		void InitializeComponent() {
			this.txtQuoteDelay = new System.Windows.Forms.TextBox();
			this.lblQuoteDelay = new System.Windows.Forms.Label();
			this.cbxGeneratingNow = new System.Windows.Forms.CheckBox();
			this.lblGenerateOnlySymbols = new System.Windows.Forms.Label();
			this.txtGenerateOnlySymbols = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtQuoteDelay
			// 
			this.txtQuoteDelay.Location = new System.Drawing.Point(3, 16);
			this.txtQuoteDelay.Name = "txtQuoteDelay";
			this.txtQuoteDelay.Size = new System.Drawing.Size(93, 20);
			this.txtQuoteDelay.TabIndex = 9;
			this.txtQuoteDelay.Text = "500";
			this.txtQuoteDelay.TextChanged += new System.EventHandler(this.txtQuoteDelay_TextChanged);
			this.txtQuoteDelay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuoteDelay_KeyDown);
			// 
			// lblQuoteDelay
			// 
			this.lblQuoteDelay.AutoSize = true;
			this.lblQuoteDelay.Location = new System.Drawing.Point(3, 0);
			this.lblQuoteDelay.Name = "lblQuoteDelay";
			this.lblQuoteDelay.Size = new System.Drawing.Size(93, 13);
			this.lblQuoteDelay.TabIndex = 12;
			this.lblQuoteDelay.Text = "Quote Delay, millis";
			// 
			// cbxGeneratingNow
			// 
			this.cbxGeneratingNow.AutoSize = true;
			this.cbxGeneratingNow.Location = new System.Drawing.Point(3, 81);
			this.cbxGeneratingNow.Name = "cbxGeneratingNow";
			this.cbxGeneratingNow.Size = new System.Drawing.Size(101, 17);
			this.cbxGeneratingNow.TabIndex = 15;
			this.cbxGeneratingNow.Text = "Generating now";
			this.cbxGeneratingNow.UseVisualStyleBackColor = false;
			this.cbxGeneratingNow.CheckedChanged += new System.EventHandler(this.cbxGeneratingNow_CheckedChanged);
			// 
			// lblGenerateOnlySymbols
			// 
			this.lblGenerateOnlySymbols.AutoSize = true;
			this.lblGenerateOnlySymbols.Location = new System.Drawing.Point(3, 39);
			this.lblGenerateOnlySymbols.Name = "lblGenerateOnlySymbols";
			this.lblGenerateOnlySymbols.Size = new System.Drawing.Size(172, 13);
			this.lblGenerateOnlySymbols.TabIndex = 17;
			this.lblGenerateOnlySymbols.Text = "Generate Quotes Only For Symbols";
			// 
			// txtGenerateOnlySymbols
			// 
			this.txtGenerateOnlySymbols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtGenerateOnlySymbols.Location = new System.Drawing.Point(3, 55);
			this.txtGenerateOnlySymbols.Name = "txtGenerateOnlySymbols";
			this.txtGenerateOnlySymbols.Size = new System.Drawing.Size(229, 20);
			this.txtGenerateOnlySymbols.TabIndex = 16;
			this.txtGenerateOnlySymbols.TextChanged += new System.EventHandler(this.txtGenerateOnlySymbols_TextChanged);
			this.txtGenerateOnlySymbols.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGenerateOnlySymbols_KeyDown);
			// 
			// StreamingMockEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.lblGenerateOnlySymbols);
			this.Controls.Add(this.txtGenerateOnlySymbols);
			this.Controls.Add(this.cbxGeneratingNow);
			this.Controls.Add(this.lblQuoteDelay);
			this.Controls.Add(this.txtQuoteDelay);
			this.Name = "StreamingMockEditor";
			this.Size = new System.Drawing.Size(240, 105);
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