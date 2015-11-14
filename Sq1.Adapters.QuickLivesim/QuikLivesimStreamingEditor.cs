using System;

using Sq1.Core.DataFeed;

namespace Sq1.Adapters.QuikLivesim {
	public partial class QuikLivesimStreamingEditor {
		QuikLivesimStreamingSettings	quikLivesimStreamingSettings;
		QuikLivesimStreaming			quikLivesimStreaming;

		QuikLivesimStreamingEditor() {
			this.InitializeComponent();
		}

		public QuikLivesimStreamingEditor(QuikLivesimStreaming quikLivesimStreaming, IDataSourceEditor dataSourceEditor) : this() {
			this.quikLivesimStreaming = quikLivesimStreaming;
			this.dataSourceEditor = dataSourceEditor;
		}
		public void Initialize(QuikLivesimStreamingSettings quikLivesimStreamingSettings) {
			this.quikLivesimStreamingSettings = quikLivesimStreamingSettings;
		}

		public override void PushStreamingAdapterSettingsToEditor() {
		}
		public override void PushEditedSettingsToStreamingAdapter() {
		}

	}
}
