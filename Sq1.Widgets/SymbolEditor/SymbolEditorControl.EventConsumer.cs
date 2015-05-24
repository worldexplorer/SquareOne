using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl {
		//void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
		void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.rebuildingDropdown) return;

			if (symbolInfoSelectedNullUnsafe == null) {
				string msg = "YOU_MUST_INITIALIZE_DROPDOWN_WITH_SYMBOL_INFOs this.comboBox1.SelectedItem as SymbolInfo == null";
				Assembler.PopupException(msg);
			}
			this.Initialize(symbolInfoSelectedNullUnsafe);
			if (this.tsmniSymbol.Pressed) this.toolStripComboBox1.DroppedDown = true;
		}
		void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			this.repositorySerializerSymbolInfo.Serialize();
			this.Initialize(this.symbolInfoSelectedNullUnsafe, true);
		}
		void mniltbDuplicate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo dupe = this.repositorySerializerSymbolInfo.Duplicate(this.symbolInfoSelectedNullUnsafe, e.StringUserTyped);
			this.mniltbDuplicate.TextRed = dupe == null;
			if (dupe == null) return;
			this.Initialize(dupe, true);
		}
		void mniltbRename_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo renamed = this.repositorySerializerSymbolInfo.Rename(this.symbolInfoSelectedNullUnsafe, e.StringUserTyped);
			this.mniltbRename.TextRed = renamed == null;
			if (renamed == null) return;
			this.Initialize(renamed, true);
		}
		void mniDeleteSymbol_Click(object sender, EventArgs e) {
			SymbolInfo priorToDeleted = this.repositorySerializerSymbolInfo.Delete(this.symbolInfoSelectedNullUnsafe);
			if (priorToDeleted == null) return;
			this.Initialize(priorToDeleted, true);
			this.toolStripComboBox1.DroppedDown = true;
		}
		void mniltbAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			SymbolInfo added = this.repositorySerializerSymbolInfo.Add(e.StringUserTyped);
			this.mniltbAddNew.TextRed = added == null;
			if (added == null) return;
			this.Initialize(added, true);
		}
	}
}
