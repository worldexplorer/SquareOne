using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Serializers;
using System.Collections.Generic;

namespace Sq1.Charting {
	public partial class ChartSettingsEditorControl : UserControl {
		Dictionary<ChartSettings, ChartControl> chartSettings;
		ChartSettings							chartSettingsSelectedNullUnsafe { get { return this.toolStripItemComboBox1.ComboBox.SelectedItem as ChartSettings; } }
		bool									rebuildingDropdown;
		bool									openDropDownAfterSelected;

		public ChartSettingsEditorControl() {
			InitializeComponent();

			// DESIGNER_RESETS_TO_EDITABLE__LAZY_TO_TUNNEL_PROPERTIES_AND_EVENTS_IN_ToolStripItemComboBox.cs
			this.toolStripItemComboBox1.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.toolStripItemComboBox1.ComboBox.Sorted = true;
			this.toolStripItemComboBox1.ComboBox.SelectedIndexChanged += new EventHandler(this.toolStripItemComboBox1_SelectedIndexChanged);
		}

		public void Initialize(Dictionary<ChartSettings, ChartControl> chartControlsPassed) {
			this.chartSettings = chartControlsPassed;	// I_EXPECT_LIST_OF_CHART_FORMS_TO_REMAIN_SAME_INSTANCE__SO_EACH_NEXT_CHART_IS_OPEN_I_JUST_rebuildDropdown()
			this.RebuildDropdown();
			//if (this.repositorySerializerSymbolInfo.SymbolInfos.Count > 0) {
			//    this.Initialize(this.repositorySerializerSymbolInfo.SymbolInfos[0]);
			//}
		}

		public void RebuildDropdown() {
			if (this.chartSettings == null) {
				string msg = "DONT_INVOKE_REBUILD()_PRIOR_TO_INITIALIZE() //ChartSettingsEditorControl.RebuildDropdown()";
				Assembler.PopupException(msg);
				return;
			}
			this.rebuildingDropdown = true;
			try {
				this.toolStripItemComboBox1.ComboBox.Items.Clear();
				foreach (ChartSettings chartSettings in this.chartSettings.Keys) {
					this.toolStripItemComboBox1.ComboBox.Items.Add(chartSettings);
				}
			} finally {
				this.rebuildingDropdown = false;
			}
		}
		public void PopulateWithChartSettings(ChartSettings chartSettings = null, bool forceRebuild = false) {
			if (chartSettings == null) {
				chartSettings = this.chartSettingsSelectedNullUnsafe;
			}
			if (chartSettings == null) {
				string msg = "I_REFUSE_TO_INITIALIZE_WITH_NULL_ChartSettings";
				Assembler.PopupException(msg);
				return;
			}
			Form parent = base.Parent as Form;
			if (parent != null) {
				parent.Text = "Chart Editor :: " + chartSettings.ToString();
			}

			this.propertyGrid1.SelectedObject = chartSettings;
			if (forceRebuild) this.RebuildDropdown();

			ChartSettings selected = this.chartSettingsSelectedNullUnsafe;
			if (selected == null) {
				this.toolStripItemComboBox1.ComboBox.SelectedItem = chartSettings;
				return;
			} else {
				if (selected.ToString() == chartSettings.ToString()) {
					return;
				}
			}
			foreach (ChartSettings eachChartSettings in this.toolStripItemComboBox1.ComboBox.Items) {
				if (eachChartSettings.ToString() != chartSettings.ToString()) continue;
				this.openDropDownAfterSelected = false;
				this.toolStripItemComboBox1.ComboBox.SelectedItem = eachChartSettings;	// triggering event to invoke toolStripComboBox1_SelectedIndexChanged => testing chartSettingsSelectedNullUnsafe + Initialize()
				break;
			}
		}
	}
}
