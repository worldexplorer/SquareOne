using System;

using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public class LivesimStreamingEditorEmpty : StreamingEditor {
		public override void Initialize(StreamingAdapter streamingAdapter, IDataSourceEditor dataSourceEditor) {
			base.Initialize(streamingAdapter, dataSourceEditor);
		}
		public override void PullStreamingAdapterSettings_toEditor() {
			string msg = "INAPPLICABLE_FOR_A_LIVESIMSTREAMING_ENFORCED_TO_BE_MANDATORY_WHEN_NO_OTHER_STREAMING_IS_ASSIGNED_BY_USER_TO_A_DATASOURCE for streamingAdapter=[" + base.StreamingAdapter + "]";
		}
		public override void PushEditedSettings_toStreamingAdapter_serializeDataSource() {
			string msg = "INAPPLICABLE_FOR_A_LIVESIMSTREAMING_ENFORCED_TO_BE_MANDATORY_WHEN_NO_OTHER_STREAMING_IS_ASSIGNED_BY_USER_TO_A_DATASOURCE for streamingAdapter=[" + base.StreamingAdapter + "]";
		}
		protected override void StreamingAdapter_OnStreamingConnectionStateChanged(object sender, EventArgs e) {
			string msg = "COPY_CONTENT_FROM_QUIK_STREAMING_EDITOR UPDATE_YOUR_CONNECT_BUTTON_STATE_BECAUSE_IT_WAS_CHANGED_SOMEWHERE_ELSE";
		}
	}
}
