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
	public partial class StreamingMockEditor {
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
			get { return this.cbxGeneratingNow.Checked; }
			set { this.cbxGeneratingNow.Checked = value; }
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
		public bool GeneratingNow {
			get { return this.cbxGeneratingNow.Checked; }
			set { this.cbxGeneratingNow.Checked = value; }
		}
		StreamingMock mockStreamingProvider { get { return base.streamingProvider as StreamingMock; } }

		public StreamingMockEditor(StreamingMock mockStreamingProvider, IDataSourceEditor dataSourceEditor) : base(mockStreamingProvider, dataSourceEditor) {
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
			this.mockStreamingProvider.GeneratingNow = this.GeneratingNow;
		}
	}
}