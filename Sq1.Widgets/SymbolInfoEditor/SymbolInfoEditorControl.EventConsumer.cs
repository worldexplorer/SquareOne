using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolInfoEditorControl {
		void toolStripItemComboBox1_DropDown(object sender, EventArgs e) {
			this.rebuildDropdown();
		}
		void toolStripItemComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			// IM_EXECUTED_TWICE__WHEN_DESELECTING_OLD_AND_WHEN_SELECTING_NEW
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.rebuildingDropdown) return;

			if (this.symbolInfoSelectedNullUnsafe == null) {
				string msg = "YOU_MUST_INITIALIZE_DROPDOWN_WITH_CURRENT_SYMBOL_INFO__USE_PopulateWithSymbol()"
					+ "; now this.toolStripItemComboBox1.ComboBox.SelectedItem as SymbolInfo=null";
				Assembler.PopupException(msg);
			}
			this.populateWithSymbolInfo(this.symbolInfoSelectedNullUnsafe);

			// I want to keep dropdown open to save user from monkey-clicking; when I initialize() at startup, this.rebuildingDropdown=true and I don't reach to here;
			// the only time I'm kicked in is when I switch charts => SymbolInfoEditor syncs to the current chart's symbol;
			// that's when this.openDropDownAfterSelected=false; SymbolInfoEditorControl.cs:67
			if (this.openDropDownAfterSelected == false) {
				this.openDropDownAfterSelected = true;
				return;
			}
			this.toolStripItemComboBox1.ComboBox.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			this.repositorySerializerSymbolInfo.Serialize();
			this.populateWithSymbolInfo(this.symbolInfoSelectedNullUnsafe, true);
		}
		void mniltbDuplicate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo dupe = this.repositorySerializerSymbolInfo.Duplicate(this.symbolInfoSelectedNullUnsafe, e.StringUserTyped);
			this.mniltbDuplicate.TextRed = dupe == null;
			if (dupe == null) return;
			this.populateWithSymbolInfo(dupe, true);
		}
		void mniltbRename_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo renamed = this.repositorySerializerSymbolInfo.Rename(this.symbolInfoSelectedNullUnsafe, e.StringUserTyped);
			this.mniltbRename.TextRed = renamed == null;
			if (renamed == null) return;
			this.populateWithSymbolInfo(renamed, true);
		}
		void mniDeleteSymbol_Click(object sender, EventArgs e) {
			SymbolInfo priorToDeleted = this.repositorySerializerSymbolInfo.Delete(this.symbolInfoSelectedNullUnsafe);
			if (priorToDeleted == null) return;
			this.populateWithSymbolInfo(priorToDeleted, true);
			this.toolStripItemComboBox1.ComboBox.DroppedDown = true;
		}
		void mniltbAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo added = this.repositorySerializerSymbolInfo.Add(e.StringUserTyped);
			this.mniltbAddNew.TextRed = added == null;
			if (added == null) return;
			this.populateWithSymbolInfo(added, true);
		}
	}
}
