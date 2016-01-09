using System;

using Sq1.Core.DataFeed;

namespace Sq1.Adapters.Quik.Streaming.Livesim {
	public partial class QuikStreamingLivesimEditor {
		QuikStreamingLivesimSettings	quikLivesimStreamingSettings;
		QuikStreamingLivesim			quikLivesimStreaming;

		QuikStreamingLivesimEditor() {
			this.InitializeComponent();
		}

		public QuikStreamingLivesimEditor(QuikStreamingLivesim quikLivesimStreaming, IDataSourceEditor dataSourceEditor) : this() {
			this.quikLivesimStreaming = quikLivesimStreaming;
			this.DataSourceEditor = dataSourceEditor;
		}
		public void Initialize(QuikStreamingLivesimSettings quikLivesimStreamingSettings) {
			this.quikLivesimStreamingSettings = quikLivesimStreamingSettings;
		}

		public override void PushStreamingAdapterSettingsToEditor() {
		}
		public override void PushEditedSettingsToStreamingAdapter() {
		}

	}
}
