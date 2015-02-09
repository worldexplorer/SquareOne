using System;

namespace Sq1.Core.Livesim {
	public partial class LivesimStreamingEditor {
//		public String DdeServerPrefix {
//			get { return this.txtDdeServerPrefix.Text; }
//			set { this.txtDdeServerPrefix.Text = value; }
//		}
//		public String DdeTopicQuotes {
//			get { return this.txtTopicQuotes.Text; }
//			set { this.txtTopicQuotes.Text = value; }
//		}
//		public String DdeTopicTrades {
//			get { return this.txtTopicTrades.Text; }
//			set { this.txtTopicTrades.Text = value; }
//		}
//		public String DdeTopicPrefixDom {
//			get { return this.txtTopicPrefixDOM.Text; }
//			set { this.txtTopicPrefixDOM.Text = value; }
//		}
		private LivesimStreaming streamingLivesim {
			get { return base.streamingProvider as LivesimStreaming; }
		}

		public LivesimStreamingEditor() {	//used in Design Mode for the descendands
			this.InitializeComponent();
		}
//		public override void Initialize(StreamingProvider quikStreamingProvider, IDataSourceEditor dataSourceEditor) {
//			base.Initialize(quikStreamingProvider, dataSourceEditor);
//			base.InitializeEditorFields();
//		}
		public override void PushStreamingProviderSettingsToEditor() {
//			this.DdeServerPrefix = this.streamingLivesim.DdeServerPrefix;
//			this.DdeTopicQuotes = this.streamingLivesim.DdeTopicQuotes;
//			this.DdeTopicTrades = this.streamingLivesim.DdeTopicTrades;
//			this.DdeTopicPrefixDom = this.streamingLivesim.DdeTopicPrefixDom;
		}
		public override void PushEditedSettingsToStreamingProvider() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
//			this.streamingLivesim.DdeServerPrefix = this.DdeServerPrefix;
//			this.streamingLivesim.DdeTopicQuotes = this.DdeTopicQuotes;
//			this.streamingLivesim.DdeTopicTrades = this.DdeTopicTrades;
//			this.streamingLivesim.DdeTopicPrefixDom = this.DdeTopicPrefixDom;
		}

    }
}