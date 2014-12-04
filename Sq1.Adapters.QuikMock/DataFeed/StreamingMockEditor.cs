using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

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

		// Designer will call this
		public StreamingMockEditor()  {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => streamingProviderInstance.StreamingEditorInitialize() will call this
		public StreamingMockEditor(StreamingProvider mockStreamingProvider, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(mockStreamingProvider, dataSourceEditor);
		}
		public override void PushStreamingProviderSettingsToEditor() {
			this.QuoteDelay				= this.mockStreamingProvider.QuoteDelayAutoPropagate;
			this.GenerateOnlySymbols	= this.mockStreamingProvider.GenerateOnlySymbols;
			// NB!!! assignment will trigger this.mockStreamingProvider.AllSymbolsGenerateStart() from cbxGeneratingNow_CheckedChanged
			this.GeneratingNow			= this.mockStreamingProvider.GeneratingNow;
		}
		public override void PushEditedSettingsToStreamingProvider() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			if (this.QuoteDelay == 0) this.QuoteDelay = 1000;
			this.mockStreamingProvider.QuoteDelayAutoPropagate		= this.QuoteDelay;
			this.mockStreamingProvider.GenerateOnlySymbols			= this.GenerateOnlySymbols;
			try {
				this.mockStreamingProvider.GeneratingNowAutoPropagate = this.GeneratingNow;
				Assembler.InstanceInitialized.RepositoryJsonDataSource.SerializeSingle(this.mockStreamingProvider.DataSource);
			} catch (Exception ex) {
				string msg = "PushEditedSettingsToStreamingProvider()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}