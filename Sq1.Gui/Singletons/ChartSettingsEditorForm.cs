using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Charting;
using Sq1.Charting;

using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	// TO_ENABLE_DESIGNER public partial class ChartSettingsEditorForm : DockContent {
	// TO_ENABLE_SINGLETON_FUNCTIONALITY
	public partial class ChartSettingsEditorForm : DockContentSingleton<ChartSettingsEditorForm> {
		public ChartSettingsEditorForm() {
			InitializeComponent();
		}
		public void Initialize(Dictionary<ChartSettings, ChartControl> chartSettingsPassed) {
			this.ChartSettingsEditorControl.Initialize(chartSettingsPassed);
		}

		internal void RebuildDropDown_dueToChartFormAddedOrRemoved() {
			this.ChartSettingsEditorControl.RebuildDropdown();
		}
		public void PopulateWithChartSettings(ChartSettings chartSettings) {
			this.ChartSettingsEditorControl.PopulateWithChartSettings(chartSettings);
		}
	}
}
