using System;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	public partial class CsvImporterForm : DockContentSingleton<CsvImporterForm> {
		public CsvImporterForm() {
			InitializeComponent();
			base.FloatWindowRecommendedSize = base.Size;
		}
		public void Initialize(RepositoryJsonDataSources dataSourceRepository) {
			this.csvImporterControl.Initialize(dataSourceRepository);
		}
	}
}