using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Repositories;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Charting {
	public partial class ChartSettingsEditorControl {
		void cbxSettings_SelectedIndexChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.rebuildingDropdown) return;

			if (sender as ComboBox != this.cbxChartsCurrentlyOpen.ComboBox) {
				string msg = "WHATT???? sender as ComboBox != this.toolStripItemComboBox1.ComboBox //toolStripItemComboBox1_SelectedIndexChanged()";
				Assembler.PopupException(msg, null, false);

			}

			ChartControl chartSelected = this.chartControlSelected_nullUnsafe;
			if (chartSelected == null) {
				string msg = "YOU_MUST_ComboBox.SelectValue(CURRENT_CHART_CONTROL.ChartSettings)__USE_PopulateWithChartSettings()"
					+ "; now this.toolStripItemComboBox1.ComboBox.SelectedItem as ChartSettings=null";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.PopulatePropertyGrid_withChartsSettings_selectCurrentChart(chartSelected);

			// I want to keep dropdown open to save the user from monkey-clicking; when I initialize() at startup, this.rebuildingDropdown=true and I don't reach to here;
			// the only time I'm kicked in is when I switch charts => ChartSettingsEditorControl syncs to the current chart's settings;
			// that's when this.openDropDownAfterSelected=false; ChartSettingsEditorControl.cs:64
			if (this.openDropDownAfterSelected == false) {
				this.openDropDownAfterSelected = true;
				return;
			}
			this.cbxChartsCurrentlyOpen.ComboBox.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			ChartControl chartSelected = this.chartControlSelected_nullUnsafe;
			if (chartSelected == null) {
				string msg = "YOU_SHOULD_HAVE_NOTIFIED_ChartSettingsEditor_THAT_YOU_CLOSED_THE_CHART";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettingsTemplated edited = this.settingsCurrent_nullUnsafe;
			if (edited == null) {
				string msg = "I_CAN_NOT_SERIALIZE_CHART_SETTINGS_AFTER_PROPERTY_CHANGE this.mniSettingsForCurrentChart.Tag==null";
				Assembler.PopupException(msg);
				return;
			}

			this.settingsRepo.SerializeSingle(edited);
			edited.PensAndBrushesCached_DisposeAndNullify();

			foreach (ChartControl eachChart in this.allChartControls_currentlyOpen) {
				if (eachChart.ChartSettingsTemplated != edited) continue;
				eachChart.InvalidateAllPanels();
			}
		}

		void mniSettingsImEditing_Click(object sender, EventArgs e) {
			return;
			//this.Cursor = Cursors.WaitCursor;
			//this.PopulatePropertyGrid_withChartSettings_selectCurrentChart();
			////RO this.mniAbsorbFromChart.Pressed = false;
			//this.Cursor = Cursors.Default;
		}





		void ctxAllSettingsAvailable_Opening(object sender, CancelEventArgs e) {
			this.ctxAllSettingsAvailable.Items.Clear();
			List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();

			foreach (ChartSettingsTemplated settingsCachedInstance in this.settingsRepo.ItemsCachedAsList) {
				ToolStripMenuItem mni = new ToolStripMenuItem();
				mni.Text = settingsCachedInstance.Name;
				mni.Name = "ChartSettingsRepoCachedInstance_" + settingsCachedInstance.Name;
				mni.Tag = settingsCachedInstance;

				if (	this.chartControlSelected_nullUnsafe != null &&
						this.chartControlSelected_nullUnsafe.ChartSettingsTemplated != null &&
						this.chartControlSelected_nullUnsafe.ChartSettingsTemplated.Name == settingsCachedInstance.Name) {
					mni.Checked = true;
					//ALREADY_SET_IN_PopulatePropertyGrid() this.mniSettingsImEditing.Text = mni.Text;
				}
				mni.DropDownOpening += new EventHandler(mniEachSettings_DropDownOpening_showLoadSaveRenameDuplicateAddDelete);
				mni.Click += new EventHandler(mniEachSettings_Click);
				ret.Add(mni);
			}
			this.ctxAllSettingsAvailable.Items.AddRange(ret.ToArray());
		}
		void mniEachSettings_Click(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "I_REFUSE_TO_LOAD sender as ToolStripMenuItem = null";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettingsTemplated settingsRepoCachedInstance = mni.Tag as ChartSettingsTemplated;
			if (settingsRepoCachedInstance == null) {
				string msg = "I_REFUSE_TO_LOAD_NON_CHART_SETTINGS__FOUND_IN_MNI.TAG[" + mni.Tag + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(settingsRepoCachedInstance);
		}
		void onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(
				ChartSettingsTemplated settingsCachedInstance, bool keepCtxOpen_level1 = true) {
			ChartControl chartSelected = this.chartControlSelected_nullUnsafe;
			if (chartSelected == null) {
				string msg = "NO_CHART_ATTACHED__NOWHERE_TO_LOAD__this.chartSettingsSelected_nullUnsafe=null";
				Assembler.PopupException(msg);
				return;
			}
			//controlSelected.AbsorbFrom(settingsCachedInstance);
			chartSelected.Set_ChartSettingsTemplated(settingsCachedInstance);
			//controlSelected.ChartSettings.PensAndBrushesCached_DisposeAndNullify();
			chartSelected.InvalidateAllPanels();

			if (this.allChartControls_currentlyOpen.Contains(chartSelected) == false) {
				string msg = "I_CAN_NOT_SERIALIZE_CHART_SETTINGS_AFTER_PROPERTY_CHANGE chartSettings.ContainsKey(" + chartSelected.Name + ")=false";
				Assembler.PopupException(msg);
			} else {
				// SettingsTemplated are not kept in ChartFormSnapshot chartSelected.RaiseOnChartSettingsIndividualChanged_chartManagerShouldSerialize_ChartFormDataSnapshot();
				foreach (var each in this.ctxAllSettingsAvailable.Items) {
					ToolStripMenuItem eachAsMni = each as ToolStripMenuItem;
					if (eachAsMni == null) {
						string msg = "I_REFUSE_TO_CHANGE_CHECKED_AFTER_ABSORBED {this.ctxTemplates.Item[" + each + "] as ToolStripMenuItem = null}";
						continue;
					}
					eachAsMni.Checked = eachAsMni.Text == settingsCachedInstance.Name;
				}
			}
			this.openDropDownAfterSelected = false;
			this.PopulatePropertyGrid_withChartsSettings_selectCurrentChart(chartSelected, true);
			if (keepCtxOpen_level1) this.ctxAllSettingsAvailable.Show();
		}

		void mniEachSettings_DropDownOpening_showLoadSaveRenameDuplicateAddDelete(object sender, EventArgs e) {
			ToolStripMenuItem mniSettings_mouseOvered = sender as ToolStripMenuItem;
			if (mniSettings_mouseOvered == null) {
				string msg = "I_REFUSE_TO_LOAD sender as ToolStripMenuItem = null";
				Assembler.PopupException(msg);
				return;
			}
			mniSettings_mouseOvered.DropDown = this.ctxActions_forOneSettings;

			ChartSettingsTemplated settingsMouseOvered = mniSettings_mouseOvered.Tag as ChartSettingsTemplated;
			if (settingsMouseOvered == null) {
				string msg = "NO_SETTINGS_ATTACHED mniSettings_mouseOvered.Tag=null";
				Assembler.PopupException(msg);
				return;
			}
			string settingsInstanceName = settingsMouseOvered.Name;

			this.mniSettingsMouseOvered_AssignToCurrentChart.Text = "Load into chart [" + this.chartControlSelected_nullUnsafe + "]";

			this.mniltbSettingsMouseOvered_Duplicate.InputFieldValue = settingsInstanceName;
			this.mniltbSettingsMouseOvered_RenameTo	.InputFieldValue = settingsInstanceName;
			this.mniSettings_AddNew		.InputFieldValue = settingsInstanceName;
			this.mniltbSettingsMouseOvered_SaveAs.InputFieldValue = mniSettings_mouseOvered.Text;

			this.mniSettingsMouseOvered_Delete.Text = "Delete [" + mniSettings_mouseOvered.Text + "]";
			this.mniSettingsMouseOvered_Delete.Tag = settingsMouseOvered;

			this.ctxActions_forOneSettings.Tag = settingsMouseOvered;

			if (mniSettings_mouseOvered.Text == ChartSettingsTemplated.NAME_DEFAULT) {
				this.mniltbSettingsMouseOvered_RenameTo.Enabled = false;
				this.mniSettingsMouseOvered_Delete.Text = "Delete [DEFAULT_MUST_STAY]";
				this.mniSettingsMouseOvered_Delete.Enabled = false;
				this.mniltbSettingsMouseOvered_SaveAs.Text = "[DEFAULT_IS_IMMUTABLE]";
				this.mniltbSettingsMouseOvered_SaveAs.Enabled = false;
			} else {
				this.mniltbSettingsMouseOvered_RenameTo.Enabled = true;
				this.mniSettingsMouseOvered_Delete.Enabled = true;
				this.mniltbSettingsMouseOvered_SaveAs.Enabled = true;
			}
		}

		void mniltbSettingsMouseOvered_Duplicate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbSettingsMouseOvered_Duplicate_UserTyped()";
			ChartSettingsTemplated settingsImDoingActionFor = this.ctxActions_forOneSettings.Tag as ChartSettingsTemplated;
			if (settingsImDoingActionFor == null) {
				string msg = "NO_SETTINGS_ATTACHED_TO this.ctxActions_forOneSettings.Tag";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				ChartSettingsTemplated clone = settingsImDoingActionFor.Clone();
				clone.Name = e.StringUserTyped;
				this.settingsRepo.SerializeSingle(clone);
				//RENAMES_INSTEAD_OF_SAVING_CLONE this.settingsRepoTemplates.ItemAdd(clone);
				//this.settingsRepoTemplates.DeserializeJsonsInFolder();
				this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(clone);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		void mniltbSettingsMouseOvered_RenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbSettingsMouseOvered_RenameTo_UserTyped()";
			ChartSettingsTemplated settingsImDoingActionFor = this.ctxActions_forOneSettings.Tag as ChartSettingsTemplated;
			if (settingsImDoingActionFor == null) {
				string msg = "NO_SETTINGS_ATTACHED_TO this.ctxActions_forOneSettings.Tag";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.settingsRepo.ItemRename(settingsImDoingActionFor, e.StringUserTyped);
				this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(settingsImDoingActionFor);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		void mniSettings_AddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniSettings_AddNew_UserTyped()";
			try {
				ChartSettingsTemplated settings_newDefault = new ChartSettingsTemplated(e.StringUserTyped);
				this.settingsRepo.ItemAdd_serialize(settings_newDefault);
				this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(settings_newDefault);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		void mniSettingsMouseOvered_Delete_Click(object sender, EventArgs e) {
			string msig = " //mniltbSettingsMouseOvered_Duplicate_UserTyped()";
			ChartSettingsTemplated settingsImDoingActionFor = this.ctxActions_forOneSettings.Tag as ChartSettingsTemplated;
			if (settingsImDoingActionFor == null) {
				string msg = "NO_SETTINGS_ATTACHED_TO this.ctxActions_forOneSettings.Tag";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.settingsRepo.ItemDelete_jsonFileErase(settingsImDoingActionFor);
				if (this.settingsCurrent_nullUnsafe.Name == settingsImDoingActionFor.Name) {
					// get next
					//this.populateTemplate(clone);
				}
				this.ctxActions_forOneSettings.Close();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		void mniSettingsMouseOvered_AssignToCurrentChart_Click(object sender, EventArgs e) {
			string msig = " //mniltbSettingsMouseOvered_Duplicate_UserTyped()";
			ChartSettingsTemplated settingsImDoingActionFor = this.ctxActions_forOneSettings.Tag as ChartSettingsTemplated;
			if (settingsImDoingActionFor == null) {
				string msg = "I_REFUSE_TO_LOAD_NON_CHART_SETTINGS__FOUND_IN_MNI.TAG[" + this.ctxActions_forOneSettings.Tag + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(settingsImDoingActionFor);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
		void mniltbSettingsMouseOvered_SaveAs_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbSettingsMouseOvered_SaveAs_UserTyped()";
			ChartSettingsTemplated settingsImDoingActionFor = this.ctxActions_forOneSettings.Tag as ChartSettingsTemplated;
			if (settingsImDoingActionFor == null) {
				string msg = "NO_SETTINGS_ATTACHED_TO this.ctxActions_forOneSettings.Tag";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {
				ChartSettingsTemplated clone = settingsImDoingActionFor.Clone();
				clone.Name = e.StringUserTyped;
				this.settingsRepo.SerializeSingle(clone);
				this.onChartSettingsTemplatedSelected_pushSettingsCachedInstance_toCurrentChartControl_toPropertyGrid_setTextForCurrentSettings(clone);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}
	}
}
