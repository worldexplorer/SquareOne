using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

using Sq1.Widgets.LabeledTextBox;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolInfoEditorControl {
		void toolStripItemComboBox1_DropDown(object sender, EventArgs e) {
			if (this.symbolInfoSelected_nullUnsafe != null) {
				bool rebuildToRemove_noSymbolSelected = this.symbolInfoSelected_nullUnsafe.Symbol == noSymbolSelected_symbol;
				if (rebuildToRemove_noSymbolSelected == false) return;
				this.PopulateWithSymbolInfo(this.symbolInfoSelected_nullUnsafe, rebuildToRemove_noSymbolSelected);
			} else {
				this.rebuildDropdown_select();
			}
		}
		void toolStripItemComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			// IM_EXECUTED_TWICE___FIRST_SELECTING_NEW_SECOND__DESELECTING_OLD__WINFORM_FULL_OF_PARADOXES_FRIENDS_OF_GENIUS_APPARENTLY
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ignoreEvent_SelectedIndexChanged_resetInHandler) {
				this.ignoreEvent_SelectedIndexChanged_resetInHandler = false;
				return;
			}

			if (this.symbolInfoSelected_nullUnsafe != null) {
				bool rebuild_onFirstEventAfterNoSymbolWasChosenAfterDeletion = this.symbolInfoSelected_nullUnsafe == this.noSymbolSelected_symbolInfo;
				this.PopulateWithSymbolInfo(this.symbolInfoSelected_nullUnsafe, rebuild_onFirstEventAfterNoSymbolWasChosenAfterDeletion);
			} else {
				string msg = "YOU_MUST_INITIALIZE_DROPDOWN_WITH_CURRENT_SYMBOL_INFO__USE_PopulateWithSymbol()"
					+ "; now this.toolStripItemComboBox1.ComboBox.SelectedItem as SymbolInfo=null";
				Assembler.PopupException(msg);
			}

			// I want to keep dropdown open to save user from monkey-clicking; when I initialize() at startup, this.rebuildingDropdown=true and I don't reach to here;
			// the only time I'm kicked in is when I switch charts => SymbolInfoEditor syncs to the current chart's symbol;
			// that's when this.openDropDownAfterSelected=false; SymbolInfoEditorControl.cs:67
			if (this.openDropDownAfterSelected == false) {
				this.openDropDownAfterSelected = true;
				return;
			}
			this.tsiCbxSymbols.ComboBox.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			this.repositorySerializerSymbolInfo.Serialize();
			this.PopulateWithSymbolInfo(this.symbolInfoSelected_nullUnsafe, true);
		}
		void mniltbDuplicate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo dupe = this.repositorySerializerSymbolInfo.Duplicate(this.symbolInfoSelected_nullUnsafe, e.StringUserTyped);
			this.mniltbDuplicate.TextRed = dupe == null;
			if (dupe == null) return;
			this.PopulateWithSymbolInfo(dupe, true);
		}
		void mniltbRename_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo renamed = this.repositorySerializerSymbolInfo.Rename(this.symbolInfoSelected_nullUnsafe, e.StringUserTyped);
			this.mniltbRename.TextRed = renamed == null;
			if (renamed == null) return;
			this.PopulateWithSymbolInfo(renamed, true);
		}
		void mniDeleteSymbol_Click(object sender, EventArgs e) {
			//if (this.symbolInfoSelected_nullUnsafe == null) return;
			//SymbolInfo priorToDeleted = null;
			//foreach (SymbolInfo each in Assembler.InstanceInitialized.RepositorySymbolInfos.EntityDeserialized) {
			//    if (this.symbolInfoSelected_nullUnsafe == each) break;
			//    priorToDeleted = each;
			//}
			//if (priorToDeleted == this.symbolInfoSelected_nullUnsafe) priorToDeleted = null;	// we are deleting firstInList => search for "next" instead

			SymbolInfo priorToDeleted = this.repositorySerializerSymbolInfo.Delete(this.symbolInfoSelected_nullUnsafe);
			// make it still clear property editor and rebuild if (priorToDeleted == null) return;
			this.PopulateWithSymbolInfo(priorToDeleted, true);
			this.tsiCbxSymbols.ComboBox.DroppedDown = true;
		}
		void mniltbAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo added = this.repositorySerializerSymbolInfo.Add(e.StringUserTyped);
			this.mniltbAddNew.TextRed = added == null;
			if (added == null) return;
			this.PopulateWithSymbolInfo(added, true);
		}

		//void repositoryJsonDataSource_OnDataSourceRenamed_refreshTitle(object sender, NamedObjectJsonEventArgs<DataSource> e) {
		//	this.PopulateWithSymbol(e.Item
		//}
		//void repositoryJsonDataSource_OnDataSourceDeleted_closeSymbolInfoEditor(object sender, NamedObjectJsonEventArgs<DataSource> e) {
		//	if (repositoryJsonDataSource_OnDataSourceDeleted_closeSymbolInfoEditor
		//	this.ParentForm.Close();
		//}
		void repositoryJsonDataSource_OnSymbolRenamed_refresh(object sender, DataSourceSymbolRenamedEventArgs e) {
			string msig = " //repositoryJsonDataSource_OnSymbolRenamed_refresh(" + e.SymbolOld + "=>" + e.Symbol + ")";
			if (e.SymbolOld == e.Symbol) {
				string msg = "SYMBOL_NOT_CHANGED_WHY_DID_YOU_INVOKE_ME? this.symbolInfoSelected_nullUnsafe.Symbol[" + this.symbolInfoSelected_nullUnsafe.Symbol + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.PopulateRenamedSymbol_rebuildDropdown(e);
		}
		void repositoryJsonDataSource_OnSymbolRemoved_clean(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //repositoryJsonDataSource_OnSymbolRemoved_clean(" + e.Symbol + ")";
			if (this.symbolInfoSelected_nullUnsafe.Symbol != e.Symbol) {
				string msg1 = "IGNORING_DELETION_OTHER_SYMBOL_NOT_IM_ACTUALLY_DISPLAYING"
					+ " this.symbolInfoSelected_nullUnsafe.Symbol[" + this.symbolInfoSelected_nullUnsafe.Symbol + "] != e.Symbol[" + e.Symbol + "]";
				//I_REALLY_DONT_CARE__WHY_SHOULD_I_KNOW?? Assembler.PopupException(msg1 + msig, null, false);
				return;
			}
			string msg = "MY_SYMBOL_DELETED_CLEARING this.symbolInfoSelected_nullUnsafe.Symbol[" + this.symbolInfoSelected_nullUnsafe.Symbol + "]";
			Assembler.PopupException(msg + msig, null, false);
			this.CleanPropertyEditor();
		}

		void tsmniModify_DropDownOpening(object sender, EventArgs e) {
			bool pushAndPull_enabled =
					this.symbolInfoSelected_nullUnsafe != null
				 && this.tsiCbxSymbols.ComboBoxItems.Count >= 2;
			this.mniPullFrom.Enabled = pushAndPull_enabled;
			this.mniPushTo	.Enabled = pushAndPull_enabled;
		}

		void mniPullFrom_DropDownOpening(object sender, EventArgs e) {
			RepositorySerializerSymbolInfos repo = Assembler.InstanceInitialized.RepositorySymbolInfos;
			List<ToolStripItem> dropDownItems = repo		//.Tsi_Dynamic_Collection(this.mniPullFrom,
				.Tsi_Dynamic_MniList(
					"mniPullFrom_", 
					null,
					this.mniPullFromSymbol_Click,
					this.symbolInfoSelected_nullUnsafe,
					this.symbolInfoSelected_nullUnsafe);
			this.mniPullFrom.DropDownItems.Clear();
			this.mniPullFrom.DropDownItems.AddRange(dropDownItems.ToArray());
		}

		void mniPullFromSymbol_Click(object sender, EventArgs e) {
			ToolStripMenuItem mniPullFrom_clicked = sender as ToolStripMenuItem;
			if (mniPullFrom_clicked == null) {
				string msg = "sender_MUST_BE_ToolStripMenuItem mniPullFrom_clicked=null sender[" + sender + "]";
				Assembler.PopupException(msg);
				return;
			}
			SymbolInfo symbolInfo_toPullFrom = mniPullFrom_clicked.Tag as SymbolInfo;
			if (symbolInfo_toPullFrom == null) {
				string msg = "Tag_MUST_BE_SymbolInfo mniPullFrom_clicked[" + mniPullFrom_clicked.Text + "]";
				Assembler.PopupException(msg);
				return;
			}
			
			int absorbed = this.symbolInfoSelected_nullUnsafe.AbsorbEvery_JsonProperty_exceptSymbol_from(symbolInfo_toPullFrom);
			string msg1 = "PROPERTIES_ABSORBED[" + absorbed + "] [" + this.symbolInfoSelected_nullUnsafe + "]<=[" + symbolInfo_toPullFrom + "]";
			Assembler.PopupException(msg1, null, false);

			this.PopulateWithSymbolInfo(this.symbolInfoSelected_nullUnsafe, true);
		}


		void mniPushTo_DropDownOpening(object sender, EventArgs e) {
			RepositorySerializerSymbolInfos repo = Assembler.InstanceInitialized.RepositorySymbolInfos;
			List<ToolStripItem> dropDownItems = repo		//.Tsi_Dynamic_Collection(this.mniPushTo,
				.Tsi_Dynamic_MniList(
					"mniPushTo_", 
					null,
					this.mniPushToSymbol_Click,
					this.symbolInfoSelected_nullUnsafe,
					this.symbolInfoSelected_nullUnsafe);
			this.mniPushTo.DropDownItems.Clear();
			this.mniPushTo.DropDownItems.AddRange(dropDownItems.ToArray());
		}

		void mniPushToSymbol_Click(object sender, EventArgs e) {
			ToolStripMenuItem mniPushTo_clicked = sender as ToolStripMenuItem;
			if (mniPushTo_clicked == null) {
				string msg = "sender_MUST_BE_ToolStripMenuItem mniPushTo_clicked=null sender[" + sender + "]";
				Assembler.PopupException(msg);
				return;
			}
			SymbolInfo symbolInfo_toPushTo = mniPushTo_clicked.Tag as SymbolInfo;
			if (symbolInfo_toPushTo == null) {
				string msg = "Tag_MUST_BE_SymbolInfo mniPushTo_clicked[" + mniPushTo_clicked.Text + "]";
				Assembler.PopupException(msg);
				return;
			}
			
			int absorbed = symbolInfo_toPushTo.AbsorbEvery_JsonProperty_exceptSymbol_from(this.symbolInfoSelected_nullUnsafe);
			string msg1 = "PROPERTIES_ABSORBED[" + absorbed + "] [" + this.symbolInfoSelected_nullUnsafe + "]=>[" + symbolInfo_toPushTo + "]";
			Assembler.PopupException(msg1, null, false);

			this.rebuildDropdown_select(this.symbolInfoSelected_nullUnsafe);
		}

	}
}
