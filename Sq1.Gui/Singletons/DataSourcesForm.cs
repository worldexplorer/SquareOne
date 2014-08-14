using System;
using Sq1.Core.DataFeed;
using Sq1.Core.Repositories;
using Sq1.Core.Support;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	public partial class DataSourcesForm : DockContentSingleton<DataSourcesForm> {
//		private static DataSourcesForm instance = null;
//		public static DataSourcesForm Instance {
//			get {
//				if (DataSourcesForm.instance == null) DataSourcesForm.instance = new DataSourcesForm();
//				return DataSourcesForm.instance;
//			}
//		}

		public DataSourcesForm() {
			InitializeComponent();
		}

		public void Initialize(RepositoryJsonDataSource dataSourceRepository, IStatusReporter statusReporter, DockPanel mainFormDockPanel) {
			base.Initialize(statusReporter, mainFormDockPanel);
			DataSourcesForm.Instance.DataSourcesTreeControl.Initialize(dataSourceRepository, statusReporter);
		}
	}
}
