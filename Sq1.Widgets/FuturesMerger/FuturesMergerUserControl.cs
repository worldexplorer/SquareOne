using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.FuturesMerger {
	public partial class FuturesMergerUserControl : UserControl {
		public FuturesMergerUserControl() {
			InitializeComponent();
		}

		public void Initialize(RepositoryJsonDataSources repositoryJsonDataSources) {
			this.BarsEditorUserControl_top		.Initialize(repositoryJsonDataSources);
			this.BarsEditorUserControl_bottom	.Initialize(repositoryJsonDataSources);
		}
	}
}
