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
		// SEPARATE_CTOR_FOR_LIVESIM_STREAMING_CHILDREN
		//public QuikLivesimStreaming(DataSource deserializedDataSource) : base(deserializedDataSource) {
		//    string msg = "U_USED_Activate.CreateInstance(datasource) to avoid ctor(empty)+Initialize(livesimDataSource)";
		//    base.Name = "QuikLivesimStreaming";	// THIS_MUST_BE_INVOKED_AFTER_DESERIALIZATION_RESTORED_DATASOURCE__NOT_INITIALIZE
		//    base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimStreaming;
		//}
		public override void Initialize(DataSource deserializedDataSource) {
			base.Name = "QuikLivesimStreaming";
			base.Initialize(deserializedDataSource);
		}
		protected override void SubscribeSolidifier() {
			return;
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new QuikLivesimStreamingEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}
		
	}
}
