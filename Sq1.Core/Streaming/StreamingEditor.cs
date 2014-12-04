using System;
using System.Windows.Forms;

//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Drawing;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Streaming {
	// this class should be abstract; it's not abstract because VS2012 opens MockStreamingEditor and complains
	// "I need to instantiate base class but base class is abstract"
	// ^^^ ABOVE_FIXED_IN_DERIVED_BY_1)_PROVIDING_PARAMETERLESS_CTOR()__2)_DECLARING_TWO_ATTRIBUTES_LIKE_BELOW__EXAMPLE_BrokerLivesimEditor.cs
	//[ToolboxBitmap(typeof(StreamingEditor), "StreamingEditor")]
	//[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	
	public class StreamingEditor : UserControl {
		protected StreamingProvider streamingProvider;
		protected IDataSourceEditor dataSourceEditor;
		protected bool ignoreEditorFieldChangesWhileInitializingEditor;

		public StreamingEditor() {	/* used in Design Mode for the descendands */ }
		public virtual void Initialize(StreamingProvider streamingProvider, IDataSourceEditor dataSourceEditor) {
			this.streamingProvider = streamingProvider;
			this.dataSourceEditor = dataSourceEditor;
			this.InitializeEditorFields();
		}

		// was intended to be abstract but has implementation for Designer to be able to instantiate StreamingEditor
		public virtual void PushStreamingProviderSettingsToEditor() {
			throw new Exception("please override BrokerProvider::PushStreamingProviderSettingsToEditor() for streamingProvider=[" + this.streamingProvider + "]");
		}
		// was intended to be abstract but has implementation for Designer to be able to instantiate StreamingEditor
		public virtual void PushEditedSettingsToStreamingProvider() {
			throw new Exception("please override BrokerProvider::PushEditedSettingsToStreamingProvider() for streamingProvider=[" + this.streamingProvider + "]");
		}

		public void InitializeEditorFields() {
			this.ignoreEditorFieldChangesWhileInitializingEditor = true;
			this.PushStreamingProviderSettingsToEditor();
			this.ignoreEditorFieldChangesWhileInitializingEditor = false;
		}
	}
}