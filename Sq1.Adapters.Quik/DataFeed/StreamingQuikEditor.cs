using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;

using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.Quik {
	public partial class StreamingQuikEditor {
		public String DdeServerPrefix {
			get { return this.txtDdeServerPrefix.Text; }
			set { this.txtDdeServerPrefix.Text = value; }
		}
		public String DdeTopicQuotes {
			get { return this.txtTopicQuotes.Text; }
			set { this.txtTopicQuotes.Text = value; }
		}
		public String DdeTopicTrades {
			get { return this.txtTopicTrades.Text; }
			set { this.txtTopicTrades.Text = value; }
		}
		public String DdeTopicPrefixDom {
			get { return this.txtTopicPrefixDOM.Text; }
			set { this.txtTopicPrefixDOM.Text = value; }
		}
		private StreamingQuik quikStreamingProvider {
			get { return base.streamingProvider as StreamingQuik; }
		}

		public StreamingQuikEditor(StreamingQuik quikStreamingProvider, IDataSourceEditor dataSourceEditor)
			: base(quikStreamingProvider, dataSourceEditor) {
			InitializeComponent();
			base.InitializeEditorFields();
		}
		public override void PushStreamingProviderSettingsToEditor() {
			this.DdeServerPrefix = this.quikStreamingProvider.DdeServerPrefix;
			this.DdeTopicQuotes = this.quikStreamingProvider.DdeTopicQuotes;
			this.DdeTopicTrades = this.quikStreamingProvider.DdeTopicTrades;
			this.DdeTopicPrefixDom = this.quikStreamingProvider.DdeTopicPrefixDom;
		}
		public override void PushEditedSettingsToStreamingProvider() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			this.quikStreamingProvider.DdeServerPrefix = this.DdeServerPrefix;
			this.quikStreamingProvider.DdeTopicQuotes = this.DdeTopicQuotes;
			this.quikStreamingProvider.DdeTopicTrades = this.DdeTopicTrades;
			this.quikStreamingProvider.DdeTopicPrefixDom = this.DdeTopicPrefixDom;
		}
	
    }
}