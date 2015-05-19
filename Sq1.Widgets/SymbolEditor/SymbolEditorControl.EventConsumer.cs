using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl {
		//void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
		void toolStripComboBox1_SelectedIndexChanged(object sender, System.EventArgs e) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;

			if (symbolInfoSelected == null) {
				string msg = "YOU_MUST_INITIALIZE_DROPDOWN_WITH_SYMBOL_INFOs this.comboBox1.SelectedItem as SymbolInfo == null";
				Assembler.PopupException(msg);
			}
			this.Initialize(symbolInfoSelected);
			this.toolStripComboBox1.DroppedDown = true;
		}

		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			this.repositorySerializerSymbolInfo.Serialize();
		}
	}
}
