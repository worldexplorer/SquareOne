using System;

using Sq1.Core.DataFeed;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl {
		public event EventHandler<DataSourceEventArgs>				DataSourceEdited_updateDataSourcesTreeControl;

		public void RaiseDataSourceEdited_updateDataSourcesTreeControl() {
			if (this.DataSourceEdited_updateDataSourcesTreeControl == null) return;
			this.DataSourceEdited_updateDataSourcesTreeControl(this, new DataSourceEventArgs(this.dataSourceIamEditing));
		}
	}
}
