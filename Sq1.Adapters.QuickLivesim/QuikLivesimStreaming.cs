using System;
using System.Drawing;

using Sq1.Core.Support;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = false)]		// overriding LivesimStreaming's TRUE to have QuikLivesimStreaming appear in DataSourceEditor
	public partial class QuikLivesimStreaming : LivesimStreaming {
		//		QuikLivesimStreamingSettings	settings			{ get { return this.livesimDataSource.Executor.Strategy.LivesimStreamingSettings; } }

		public QuikLivesimStreaming() : base() {
			base.Name = "QuikLivesimStreaming-DllFound";
			base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimStreaming;
		}
		public QuikLivesimStreaming(LivesimDataSource livesimDataSource) : base(livesimDataSource) {
			base.Name = "QuikLivesimStreaming";
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new QuikLivesimStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
		
	}
}
