using System;

using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;
using Sq1.Core.Support;
using WeifenLuo.WinFormsUI.Docking;

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

		public void Initialize(RepositoryDllJsonStrategy strategyRepository, IStatusReporter statusReporter, DockPanel mainFormDockPanel) {
			base.Initialize(statusReporter, mainFormDockPanel);
			StrategiesForm.Instance.StrategiesTreeControl.Initialize(strategyRepository, statusReporter);
		}
	}
}
