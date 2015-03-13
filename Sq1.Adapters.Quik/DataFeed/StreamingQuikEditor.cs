using System;

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
		private StreamingQuik quikStreamingAdapter { get { return base.streamingAdapter as StreamingQuik; } }

		public StreamingQuikEditor() {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => streamingAdapterInstance.StreamingEditorInitialize() will call this
		public StreamingQuikEditor(StreamingAdapter quikStreamingAdapter, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(quikStreamingAdapter, dataSourceEditor);
		}
		public override void PushStreamingAdapterSettingsToEditor() {
			this.DdeServerPrefix = this.quikStreamingAdapter.DdeServerPrefix;
			this.DdeTopicQuotes = this.quikStreamingAdapter.DdeTopicQuotes;
			this.DdeTopicTrades = this.quikStreamingAdapter.DdeTopicTrades;
			this.DdeTopicPrefixDom = this.quikStreamingAdapter.DdeTopicPrefixDom;
		}
		public override void PushEditedSettingsToStreamingAdapter() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			this.quikStreamingAdapter.DdeServerPrefix = this.DdeServerPrefix;
			this.quikStreamingAdapter.DdeTopicQuotes = this.DdeTopicQuotes;
			this.quikStreamingAdapter.DdeTopicTrades = this.DdeTopicTrades;
			this.quikStreamingAdapter.DdeTopicPrefixDom = this.DdeTopicPrefixDom;
		}
	
    }
}