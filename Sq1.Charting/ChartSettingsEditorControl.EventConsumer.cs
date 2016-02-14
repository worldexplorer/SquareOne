using System;
using System.Windows.Forms;
using System.ComponentModel;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Widgets.LabeledTextBox;
using Sq1.Core.Charting;
using System.Collections.Generic;

namespace Sq1.Charting {
	public partial class ChartSettingsEditorControl {
		void cbxSettings_SelectedIndexChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.rebuildingDropdown) return;

			if (sender as ComboBox != this.cbxSettings.ComboBox) {
				string msg = "WHATT???? sender as ComboBox != this.toolStripItemComboBox1.ComboBox //toolStripItemComboBox1_SelectedIndexChanged()";
				Assembler.PopupException(msg, null, false);

			}

			ChartSettings selected = this.chartSettingsSelected_nullUnsafe;
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
			this.cbxSettings.ComboBox.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			ChartSettings selected = this.chartSettingsSelected_nullUnsafe;
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
				canSerialize.ChartSettings.PensAndBrushesCached_DisposeAndNullify();
				canSerialize.RaiseChartSettingsChangedContainerShouldSerialize();
				canSerialize.InvalidateAllPanels();
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





		void ctxTemplates_Opening(object sender, CancelEventArgs e) {
			this.ctxTemplates.Items.Clear();
			List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
			foreach (ChartSettings tpl in Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.ItemsAsList) {
				ToolStripMenuItem mni = new ToolStripMenuItem();
				mni.Text = tpl.Name;
				mni.Name = "chartSettingsTemplate_" + tpl.Name;
				mni.Tag = tpl;
				if (this.chartSettingsSelected_nullUnsafe != null && this.chartSettingsSelected_nullUnsafe.Name == tpl.Name) mni.Checked = true;
				mni.DropDownOpening += new EventHandler(tsi_DropDownOpening);
				mni.Click += new EventHandler(tsi_Click);
				ret.Add(mni);
			}
			this.ctxTemplates.Items.AddRange(ret.ToArray());
		}
		void tsi_Click(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "I_REFUSE_TO_LOAD sender as ToolStripMenuItem = null";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettings tpl = mni.Tag as ChartSettings;
			if (tpl == null) {
				string msg = "I_REFUSE_TO_LOAD_NON_CHART_SETTINGS__FOUND_IN_MNI.TAG[" + mni.Tag + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.populateTemplate(tpl);
		}
		void populateTemplate(ChartSettings tpl) {
			ChartSettings selected = this.chartSettingsSelected_nullUnsafe;
			if (selected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			selected.AbsorbFrom(tpl);
			if (this.chartSettings.ContainsKey(selected) == false) {
				string msg = "I_CAN_NOT_SERIALIZE_CHART_SETTINGS_AFTER_PROPERTY_CHANGE chartSettings.ContainsKey(" + selected.StrategyName + ")=false";
				Assembler.PopupException(msg);
			} else {
				ChartControl canSerialize = this.chartSettings[selected];
				canSerialize.ChartSettings.PensAndBrushesCached_DisposeAndNullify();
				canSerialize.RaiseChartSettingsChangedContainerShouldSerialize();
				canSerialize.InvalidateAllPanels();
				foreach (var each in this.ctxTemplates.Items) {
					ToolStripMenuItem eachAsMni = each as ToolStripMenuItem;
					if (eachAsMni == null) {
						string msg = "I_REFUSE_TO_CHANGE_CHECKED_AFTER_ABSORBED {this.ctxTemplates.Item[" + each + "] as ToolStripMenuItem = null}";
						continue;
					}
					eachAsMni.Checked = eachAsMni.Text == tpl.Name;
				}
			}
			this.openDropDownAfterSelected = false;
			this.PopulateWithChartSettings(selected, true);
			this.ctxTemplates.Show();
		}

		void tsi_DropDownOpening(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "I_REFUSE_TO_LOAD sender as ToolStripMenuItem = null";
				Assembler.PopupException(msg);
				return;
			}
			mni.DropDown = this.ctxTemplateActions;

			ChartSettings settingsMouseOver = mni.Tag as ChartSettings;
			if (settingsMouseOver == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			string strategyName = settingsMouseOver.StrategyName;
			this.mniltbDuplicate.InputFieldValue = strategyName;
			this.mniltbRenameTo.InputFieldValue = strategyName;
			this.mniAddNew.InputFieldValue = strategyName;
			this.mniltbSaveCurrentAs.InputFieldValue = mni.Text;

			this.mniDelete.Text = "Delete [" + mni.Text + "]";
			this.mniDelete.Tag = settingsMouseOver;

			this.ctxTemplateActions.Tag = settingsMouseOver;

			if (mni.Text == ChartSettings.NAME_DEFAULT) {
				this.mniltbRenameTo.Enabled = false;
				this.mniDelete.Text = "Delete [DEFAULT_MUST_STAY]";
				this.mniDelete.Enabled = false;
				this.mniltbSaveCurrentAs.Text = "[DEFAULT_IS_IMMUTABLE]";
				this.mniltbSaveCurrentAs.Enabled = false;
			} else {
				this.mniltbRenameTo.Enabled = true;
				this.mniDelete.Enabled = true;
				this.mniltbSaveCurrentAs.Enabled = true;
			}
		}

		void mniltbDuplicate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			ChartSettings selected = this.ctxTemplateActions.Tag as ChartSettings;
			if (selected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettings clone = selected.Clone();
			clone.Name = e.StringUserTyped;
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.SerializeSingle(clone);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
			this.populateTemplate(clone);
		}
		void mniltbRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			ChartSettings selected = this.ctxTemplateActions.Tag as ChartSettings;
			if (selected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.ItemRename(selected, e.StringUserTyped);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
			if (this.chartSettingsSelected_nullUnsafe.Name == selected.Name) {
				this.populateTemplate(selected);
			}
		}
		void mniAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			ChartSettings selected = new ChartSettings(e.StringUserTyped);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.ItemAdd(selected);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
		}
		void mniDelete_Click(object sender, EventArgs e) {
			ChartSettings selected = this.ctxTemplateActions.Tag as ChartSettings;
			if (selected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.ItemDelete(selected);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
			if (this.chartSettingsSelected_nullUnsafe.Name == selected.Name) {
				// get next
				//this.populateTemplate(clone);
			}
			this.ctxTemplateActions.Close();
		}
		void mniLoad_Click(object sender, EventArgs e) {
			ChartSettings selected = this.ctxTemplateActions.Tag as ChartSettings;
			if (selected == null) {
				string msg = "I_REFUSE_TO_LOAD_NON_CHART_SETTINGS__FOUND_IN_MNI.TAG[" + this.ctxTemplateActions.Tag + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.populateTemplate(selected);
		}
		void mniltbSaveCurrentAs_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			ChartSettings selected = this.chartSettingsSelected_nullUnsafe;
			if (selected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettings clone = selected.Clone();
			clone.Name = e.StringUserTyped;
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.SerializeSingle(clone);
			Assembler.InstanceInitialized.RepositoryJsonChartSettingsTemplates.DeserializeJsonsInFolder();
			this.populateTemplate(clone);
		}
	}
}
