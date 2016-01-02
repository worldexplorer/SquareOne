using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;

namespace Sq1.Gui.Singletons {
	public partial class DataSourceEditorForm : DockContentSingleton<DataSourceEditorForm> {
		public string windowTitleDefault;

		public DataSourceEditorForm() {
			this.windowTitleDefault = "New DataSource";
			this.InitializeComponent();
		}
		
		public void Initialize(string dsName) {
			if (string.IsNullOrEmpty(dsName)) return;
			
			DataSource found = Assembler.InstanceInitialized.RepositoryJsonDataSource.DataSourceFindNullUnsafe(dsName);
			if (found == null) {
				string msg = "DATA_SOURCE_FORM_EDITOR_INITIALIZED_WITH_DATASOURCE_THAT_CAN_NOT_BE_FOUND [" + dsName + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.DataSourceEditorControl.Initialize(found);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "DataSourceEditor:Sq1.Gui.Singletons.DataSourceEditorForm.Instance,DataSourceEditing:" + this.DataSourceEditorControl.DataSourceName;
		}
	}
}