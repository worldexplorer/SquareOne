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
		StreamingMock mockStreamingAdapter { get { return base.streamingAdapter as StreamingMock; } }

		// Designer will call this
		public StreamingMockEditor()  {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => streamingAdapterInstance.StreamingEditorInitialize() will call this
		public StreamingMockEditor(StreamingAdapter mockStreamingAdapter, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(mockStreamingAdapter, dataSourceEditor);
		}
		public override void PushStreamingAdapterSettingsToEditor() {
			this.QuoteDelay				= this.mockStreamingAdapter.QuoteDelayAutoPropagate;
			this.GenerateOnlySymbols	= this.mockStreamingAdapter.GenerateOnlySymbols;
			// NB!!! assignment will trigger this.mockStreamingAdapter.AllSymbolsGenerateStart() from cbxGeneratingNow_CheckedChanged
			this.GeneratingNow			= this.mockStreamingAdapter.GeneratingNow;
		}
		public override void PushEditedSettingsToStreamingAdapter() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			if (this.QuoteDelay == 0) this.QuoteDelay = 1000;
			this.mockStreamingAdapter.QuoteDelayAutoPropagate		= this.QuoteDelay;
			this.mockStreamingAdapter.GenerateOnlySymbols			= this.GenerateOnlySymbols;
			try {
				this.mockStreamingAdapter.GeneratingNowAutoPropagate = this.GeneratingNow;
				Assembler.InstanceInitialized.RepositoryJsonDataSource.SerializeSingle(this.mockStreamingAdapter.DataSource);
			} catch (Exception ex) {
				string msg = "PushEditedSettingsToStreamingAdapter()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}