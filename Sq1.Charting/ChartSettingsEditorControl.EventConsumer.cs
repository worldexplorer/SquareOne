using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Charting {
	public partial class ChartSettingsEditorControl {
		void toolStripItemComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.rebuildingDropdown) return;

			if (sender as ComboBox != this.toolStripItemComboBox1.ComboBox) {
				string msg = "WHATT????";
				Assembler.PopupException(msg, null, false);

			}

			ChartSettings selected = this.chartSettingsSelectedNullUnsafe;
			if (selected == null) {
				string msg = "YOU_MUST_ComboBox.SelectValue(CURRENT_CHART_CONTROL.ChartSettings)__USE_PopulateWithChartSettings()"
					+ "; now this.toolStripItemComboBox1.ComboBox.SelectedItem as ChartSettings=null";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.PopulateWithChartSettings(selected);

			// I want to keep dropdown open to save the user from monkey-clicking; when I initialize() at startup, this.rebuildingDropdown=true and I don't reach to here;
			// the only time I'm kicked in is when I switch charts => ChartSettingsEditorControl syncs to the current chart's settings;
			// that's when this.openDropDownAfterSelected=false; ChartSettingsEditorControl.cs:64
			if (this.openDropDownAfterSelected == false) {
				this.openDropDownAfterSelected = true;
				return;
			}
			this.toolStripItemComboBox1.ComboBox.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			ChartSettings selected = this.chartSettingsSelectedNullUnsafe;
			if (selected == null) {
				string msg = "YOU_DELETED_CHART_SETTINGS_BEFORE_PROPERTY_GRID_COULD_SAVE_A_CHANGED_PROPERTY???";
				Assembler.PopupException(msg);
				return;
			}
			if (this.chartSettings.ContainsKey(selected) == false) {
				string msg = "I_CAN_NOT_SERIALIZE_CHART_SETTINGS_AFTER_PROPERTY_CHANGE chartSettings.ContainsKey(" + selected.StrategyName + ")=false";
				Assembler.PopupException(msg);
			} else {
				ChartControl canSerialize = this.chartSettings[selected];
				//canSerialize.InvalidateAllPanels();
				canSerialize.ChartSettings.PensAndBrushesCached_DisposeAndNullify();
				canSerialize.RaiseChartSettingsChangedContainerShouldSerialize();
				canSerialize.DisposeBufferedGraphicsAndInvalidateAllPanels();
			}
			this.openDropDownAfterSelected = false;
			this.PopulateWithChartSettings(selected, true);
		}
		void mniAbsorbFromChart_Click(object sender, EventArgs e) {
			this.Cursor = Cursors.WaitCursor;
			this.PopulateWithChartSettings();
			//RO this.mniAbsorbFromChart.Pressed = false;
			this.Cursor = Cursors.Default;
		}
	}
}
