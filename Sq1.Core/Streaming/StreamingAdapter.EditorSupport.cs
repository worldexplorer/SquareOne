using System;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		[JsonIgnore]	protected IDataSourceEditor dataSourceEditor;
		[JsonIgnore]	protected StreamingEditor streamingEditorInstance;
		[JsonIgnore]	public virtual bool EditorInstanceInitialized { get { return (streamingEditorInstance != null); } }
		[JsonIgnore]	public virtual StreamingEditor EditorInstance { get {
				if (streamingEditorInstance == null) {
					string msg = "you didn't invoke StreamingEditorInitialize() prior to accessing EditorInstance property";
					throw new Exception(msg);
				}
				return streamingEditorInstance;
			} }
		public virtual StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			throw new Exception("please override StreamingAdapter::StreamingEditorInitialize():"
				+ " 1) use base.StreamingEditorInitializeHelper()"
				+ " 2) do base.streamingEditorInstance=new FoobarStreamingEditor()");
		}
		public void StreamingEditorInitializeHelper(IDataSourceEditor dataSourceEditor) {
			if (this.dataSourceEditor != null) {
				if (this.dataSourceEditor == dataSourceEditor) return;
				string msg = "this.dataSourceEditor!=null, already initialized; should I overwrite it with another instance you provided?...";
				throw new Exception(msg);
			}
			this.dataSourceEditor = dataSourceEditor;
		}

	}
}
