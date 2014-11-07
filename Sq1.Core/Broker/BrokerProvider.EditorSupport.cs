using System;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Broker {
	public partial class BrokerProvider {
		[JsonIgnore]	protected IDataSourceEditor dataSourceEditor;
		[JsonIgnore]	protected BrokerEditor brokerEditorInstance;
		[JsonIgnore]	public virtual bool EditorInstanceInitialized { get { return (brokerEditorInstance != null); } }
		[JsonIgnore]	public virtual BrokerEditor EditorInstance { get {
				if (brokerEditorInstance == null) {
					string msg = "you didn't invoke BrokerEditorInitialize() prior to accessing EditorInstance property";
					throw new Exception(msg);
				}
				return brokerEditorInstance;
			} }
		
		public virtual BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			throw new Exception("please override BrokerProvider::BrokerEditorInitialize() for [" + this + "]:"
				+ " 1) use base.BrokerEditorInitializeHelper()"
				+ " 2) do base.BrokerEditorInstance=new FoobarBrokerEditor()");
		}
		public void BrokerEditorInitializeHelper(IDataSourceEditor dataSourceEditor) {
			if (this.dataSourceEditor != null) {
				if (this.dataSourceEditor == dataSourceEditor) return;
				string msg = "this.dataSourceEditor!=null, already initialized; should I overwrite it with another instance you provided?...";
				throw new Exception(msg);
			}
			this.dataSourceEditor = dataSourceEditor;
		}
	}
}