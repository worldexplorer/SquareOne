using System;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter {
		[JsonIgnore]					IDataSourceEditor	dataSourceEditor;
		[JsonIgnore]	protected		BrokerEditor		BrokerEditorInstance;
		[JsonIgnore]	public virtual	bool				EditorInstanceInitialized { get { return (this.BrokerEditorInstance != null); } }
		[JsonIgnore]	public virtual	BrokerEditor		EditorInstance { get {
				if (this.BrokerEditorInstance == null) {
					string msg = "you didn't invoke BrokerEditorInitialize() prior to accessing EditorInstance property";
					throw new Exception(msg);
				}
				return this.BrokerEditorInstance;
			} }

		[JsonIgnore]	public			string				NameWithVersion						{ get {
			string version = "UNKNOWN";
			var fullNameSplitted = this.GetType().Assembly.FullName.Split(new string[] {", "}, StringSplitOptions.RemoveEmptyEntries);
			if (fullNameSplitted.Length >= 1) version = fullNameSplitted[1];
			if (version.Length >= "Version=".Length) version = version.TrimStart("Version=".ToCharArray());
			return this.Name + " v." + version;
		} }

		
		public virtual BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			throw new Exception("please override BrokerAdapter::BrokerEditorInitialize() for [" + this + "]:"
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