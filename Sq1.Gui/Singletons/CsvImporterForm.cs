using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Repositories;
using Sq1.Core.Support;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	public partial class CsvImporterForm : DockContentSingleton<CsvImporterForm> {
		public CsvImporterForm() {
			InitializeComponent();
			base.FloatWindowRecommendedSize = base.Size;
		}
		public void Initialize(RepositoryJsonDataSource dataSourceRepository) {
			this.csvImporterControl.Initialize(dataSourceRepository);
		}
	}
}