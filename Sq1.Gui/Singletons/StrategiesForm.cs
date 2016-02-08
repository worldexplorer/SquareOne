using System;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	public partial class StrategiesForm : DockContentSingleton<StrategiesForm> {
//		private static StrategiesForm instance = null;
//		public new static StrategiesForm Instance {
//			get {
//				if (StrategiesForm.instance == null) StrategiesForm.instance = new StrategiesForm();
//				return StrategiesForm.instance;
//			}
//		}
		
		public StrategiesForm() {
			InitializeComponent();
		}

		public void Initialize(RepositoryDllJsonStrategies strategyRepository) {
			StrategiesForm.Instance.StrategiesTreeControl.Initialize(strategyRepository);
		}
	}
}
