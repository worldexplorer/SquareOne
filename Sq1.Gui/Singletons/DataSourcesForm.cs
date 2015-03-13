using System;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	public partial class DataSourcesForm : DockContentSingleton<DataSourcesForm> {
		public DataSourcesForm() {
			InitializeComponent();
		}

		public void Initialize(RepositoryJsonDataSource dataSourceRepository) {
			DataSourcesForm.Instance.DataSourcesTreeControl.Initialize(dataSourceRepository);
		}
	}
}
