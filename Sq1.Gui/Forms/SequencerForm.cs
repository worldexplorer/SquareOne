using System;

using Sq1.Core.Sequencing;
using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class SequencerForm : DockContentImproved {
		ChartFormsManager chartFormManager;
		
		public SequencerForm() {
			InitializeComponent();
		}
		
		public SequencerForm(ChartFormsManager chartFormManager) : this() {
			this.Initialize(chartFormManager);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Sequencer:" + this.SequencerControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormsManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.WindowTitlePullFromStrategy();
			this.SequencerControl.Initialize(this.chartFormManager.Executor.Sequencer);
		}

		public void WindowTitlePullFromStrategy() {
			string windowTitle = "Sequencer :: " + this.chartFormManager.Strategy.Name;
			if (this.chartFormManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
			if (this.chartFormManager.ScriptEditedNeedsSaving) {
				windowTitle = ChartFormsManager.PREFIX_FOR_UNSAVED_STRATEGY_SOURCE_CODE + windowTitle;
			}
			this.Text = windowTitle;
		}
	}
}
