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
		protected	StreamingAdapter	StreamingAdapter;
		protected	IDataSourceEditor	DataSourceEditor;
		protected	bool				IgnoreEditorFieldChangesWhileInitializingEditor;

		public StreamingEditor() {	/* used in Design Mode for the descendands */ }
		public virtual void Initialize(StreamingAdapter streamingAdapter, IDataSourceEditor dataSourceEditor) {
			this.StreamingAdapter = streamingAdapter;
			this.DataSourceEditor = dataSourceEditor;
			this.InitializeEditorFields();
		}

		// was intended to be abstract but has implementation for Designer to be able to instantiate StreamingEditor
		public virtual void PushStreamingAdapterSettingsToEditor() {
			throw new Exception("please override BrokerAdapter::PushStreamingAdapterSettingsToEditor() for streamingAdapter=[" + this.StreamingAdapter + "]");
		}
		// was intended to be abstract but has implementation for Designer to be able to instantiate StreamingEditor
		public virtual void PushEditedSettingsToStreamingAdapter() {
			throw new Exception("please override BrokerAdapter::PushEditedSettingsToStreamingAdapter() for streamingAdapter=[" + this.StreamingAdapter + "]");
		}

		public void InitializeEditorFields() {
			this.IgnoreEditorFieldChangesWhileInitializingEditor = true;
			this.PushStreamingAdapterSettingsToEditor();
			this.IgnoreEditorFieldChangesWhileInitializingEditor = false;
		}
	}
}