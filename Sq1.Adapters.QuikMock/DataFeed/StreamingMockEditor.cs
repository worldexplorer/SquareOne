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
	public class StreamingMockEditor : StreamingEditor {
		public int QuoteDelay {
			get {
				int ret = 0;
				try {
					ret = Convert.ToInt32(this.txtQuoteDelay.Text);
					this.txtQuoteDelay.BackColor = Color.White;
				} catch (Exception e) {
					this.txtQuoteDelay.BackColor = Color.LightCoral;
					this.txtQuoteDelay.Text = "1000";	// induce one more event?...
				}
				return ret;
			}
			set { this.txtQuoteDelay.Text = value.ToString(); }
		}
		public bool SaveToStatic {
			get { return this.cbxSaveToStatic.Checked; }
			set { this.cbxSaveToStatic.Checked = value; }
		}
		public List<string> GenerateOnlySymbols {
			get { return SymbolParser.ParseSymbols(this.txtGenerateOnlySymbols.Text); }
			set {
				string ret = "";
				foreach (string symbol in value) ret += symbol + ",";
				ret = ret.TrimEnd(',');
				this.txtGenerateOnlySymbols.Text = ret;
			}
		}
		private StreamingMock mockStreamingProvider {
			get { return base.streamingProvider as StreamingMock; }
		}

		public StreamingMockEditor(StreamingMock mockStreamingProvider, IDataSourceEditor dataSourceEditor)
			: base(mockStreamingProvider, dataSourceEditor) {
			InitializeComponent();
			base.InitializeEditorFields();
		}
		public override void PushStreamingProviderSettingsToEditor() {
			this.QuoteDelay = this.mockStreamingProvider.QuoteDelayAutoPropagate;
			this.SaveToStatic = this.mockStreamingProvider.SaveToStatic;
			this.GenerateOnlySymbols = this.mockStreamingProvider.GenerateOnlySymbols;
		}
		public override void PushEditedSettingsToStreamingProvider() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			if (this.QuoteDelay == 0) this.QuoteDelay = 1000;
			this.mockStreamingProvider.QuoteDelayAutoPropagate = this.QuoteDelay;
			this.mockStreamingProvider.SaveToStatic = this.SaveToStatic;
			this.mockStreamingProvider.GenerateOnlySymbols = this.GenerateOnlySymbols;
		}

        #region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private Label lblQuoteDelay;
		private TextBox txtQuoteDelay;
		private Label lblGenerateOnlySymbols;
		private TextBox txtGenerateOnlySymbols;
		private CheckBox cbxSaveToStatic;
		private void InitializeComponent() {
			this.txtQuoteDelay = new System.Windows.Forms.TextBox();
			this.lblQuoteDelay = new System.Windows.Forms.Label();
			this.cbxSaveToStatic = new System.Windows.Forms.CheckBox();
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
			// cbxSaveToStatic
			// 
			this.cbxSaveToStatic.AutoSize = true;
			this.cbxSaveToStatic.Enabled = false;
			this.cbxSaveToStatic.Location = new System.Drawing.Point(0, 81);
			this.cbxSaveToStatic.Name = "cbxSaveToStatic";
			this.cbxSaveToStatic.Size = new System.Drawing.Size(232, 17);
			this.cbxSaveToStatic.TabIndex = 15;
			this.cbxSaveToStatic.Text = "Save Streaming Bars To QuikStaticProvider";
			this.cbxSaveToStatic.UseVisualStyleBackColor = false;
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
			this.txtGenerateOnlySymbols.Location = new System.Drawing.Point(3, 55);
			this.txtGenerateOnlySymbols.Name = "txtGenerateOnlySymbols";
			this.txtGenerateOnlySymbols.Size = new System.Drawing.Size(229, 20);
			this.txtGenerateOnlySymbols.TabIndex = 16;
			this.txtGenerateOnlySymbols.TextChanged += new System.EventHandler(this.txtGenerateOnlySymbols_TextChanged);
			this.txtGenerateOnlySymbols.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGenerateOnlySymbols_KeyDown);
			// 
			// MockStreamingEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.lblGenerateOnlySymbols);
			this.Controls.Add(this.txtGenerateOnlySymbols);
			this.Controls.Add(this.cbxSaveToStatic);
			this.Controls.Add(this.lblQuoteDelay);
			this.Controls.Add(this.txtQuoteDelay);
			this.Name = "MockStreamingEditor";
			this.Size = new System.Drawing.Size(243, 105);
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

		private void txtQuoteDelay_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			} 
		}
		private void txtQuoteDelay_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingProvider();
		}
		private void txtGenerateOnlySymbols_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			}
		}
		private void txtGenerateOnlySymbols_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingProvider();
		}
	}
}