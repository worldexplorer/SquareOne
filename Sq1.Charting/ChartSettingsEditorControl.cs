using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Repositories;

namespace Sq1.Charting {
	public partial class ChartSettingsEditorControl : UserControl {
		List<ChartControl>	allChartControls_currentlyOpen;
		ChartControl		chartControlSelected_nullUnsafe	{ get {
			ChartControl ret = null;
			string stupidComboBox = this.cbxChartsCurrentlyOpen.ComboBox.SelectedItem as string;
			foreach (ChartControl eachChartControl in this.allChartControls_currentlyOpen) {
				if (eachChartControl.ToString() != stupidComboBox) continue;
				ret = eachChartControl;
				break;
			}
			return ret;
		} }
		ChartSettings		settingsCurrent_nullUnsafe		{ get { return this.mniSettingsImEditing.Tag as ChartSettings; } }

		bool				rebuildingDropdown;
		bool				openDropDownAfterSelected;

		RepositoryJsonChartSettings	settingsRepo;

		public ChartSettingsEditorControl() {
			InitializeComponent();

			// DESIGNER_RESETS_TO_EDITABLE__LAZY_TO_TUNNEL_PROPERTIES_AND_EVENTS_IN_ToolStripItemComboBox.cs
			this.cbxChartsCurrentlyOpen.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxChartsCurrentlyOpen.ComboBox.Sorted = true;
			this.cbxChartsCurrentlyOpen.ComboBox.SelectedIndexChanged += new EventHandler(this.cbxSettings_SelectedIndexChanged);

			this.settingsRepo = Assembler.InstanceInitialized.RepositoryJsonChartSettings;
			if (this.settingsRepo == null) {
				string msg = "I_CAN_NOT_CONTINUE_WITHOUT_CACHED_SETTINGS_REPOSITORY";
				Assembler.PopupException(msg);
				return;
			}
			this.settingsRepo.DeserializeJsonsInFolder_ifNotCached();
		}

		public void Initialize(List<ChartControl> chartControls_currentlyOpen_passed) {
			this.allChartControls_currentlyOpen = chartControls_currentlyOpen_passed;	// I_EXPECT_LIST_OF_CHART_FORMS_TO_REMAIN_SAME_INSTANCE__SO_EACH_NEXT_CHART_IS_OPEN_I_JUST_rebuildDropdown()
			this.RebuildChartsDropdown();
		}

		public void RebuildChartsDropdown() {
			if (this.allChartControls_currentlyOpen == null) {
				string msg = "DONT_INVOKE_REBUILD()_PRIOR_TO_INITIALIZE() //ChartSettingsEditorControl.RebuildDropdown()";
				Assembler.PopupException(msg);
				return;
			}
			this.rebuildingDropdown = true;
			try {
				this.cbxChartsCurrentlyOpen.ComboBox.Items.Clear();
				foreach (ChartControl chartControl in this.allChartControls_currentlyOpen) {
					this.cbxChartsCurrentlyOpen.ComboBox.Items.Add(chartControl.ToString());
				}
			} finally {
				this.rebuildingDropdown = false;
			}
		}
		public void PopulatePropertyGrid_withChartsSettings_selectCurrentChart(ChartControl chartControl = null, bool forceRebuild = false) {
			if (chartControl == null) {
				chartControl = this.chartControlSelected_nullUnsafe;
			}
			if (chartControl == null) {
				string msg = "I_REFUSE_TO_INITIALIZE_WITH_NULL_ChartControl";
				Assembler.PopupException(msg);
				return;
			}
			Form parent = base.Parent as Form;
			if (parent != null) {
				parent.Text = "Chart Editor :: " + chartControl.ToString();
			}

			this.propertyGrid1.SelectedObject = chartControl.ChartSettings;
			if (forceRebuild) this.RebuildChartsDropdown();

			ChartControl selected = this.chartControlSelected_nullUnsafe;
			if (selected == null) {
				this.cbxChartsCurrentlyOpen.ComboBox.SelectedItem = chartControl.ToString();		// triggering event to invoke toolStripComboBox1_SelectedIndexChanged => testing chartSettingsSelected_nullUnsafe + Initialize()
				this.mniSettingsImEditing.Text = chartControl.ChartSettings.Name;
				return;
			}
			if (selected.ToString() == chartControl.ToString()) {
				string msg = "DockPanel_ActivateDocument invokes me (and everyone else) twice"
					+ "; first time with Form that was active BEFORE the click on the other form"
					+ "; second time with the CLICKED form <= I need only the SECOND one";
				return;
			}
			this.cbxChartsCurrentlyOpen.ComboBox.SelectedItem = chartControl.ToString();		// triggering event to invoke toolStripComboBox1_SelectedIndexChanged => testing chartSettingsSelected_nullUnsafe + Initialize()
			this.mniSettingsImEditing.Text = chartControl.ChartSettings.Name;
			this.openDropDownAfterSelected = false;
		}
	}
}
