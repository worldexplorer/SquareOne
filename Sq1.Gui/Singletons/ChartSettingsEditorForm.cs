using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Charting;

using Sq1.Charting;

using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	// TO_ENABLE_DESIGNER
	//public partial class ChartSettingsEditorForm : DockContent {
	// TO_ENABLE_SINGLETON_FUNCTIONALITY
	public partial class ChartSettingsEditorForm : DockContentSingleton<ChartSettingsEditorForm> {
		public ChartSettingsEditorForm() {
			InitializeComponent();
		}

		internal void RebuildChartsDropDown_dueToChartFormAddedOrRemoved() {
			this.ChartSettingsEditorControl.RebuildChartsDropdown();
		}
		public void PopulateWithChartSettings(ChartControl chartControl) {
			this.ChartSettingsEditorControl.PopulatePropertyGrid_withChartsSettings_selectCurrentChart(chartControl);
		}
	}
}
