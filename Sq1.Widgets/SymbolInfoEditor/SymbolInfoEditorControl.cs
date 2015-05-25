using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolInfoEditorControl : UserControl {
		RepositorySerializerSymbolInfo	repositorySerializerSymbolInfo;
		SymbolInfo						symbolInfoSelectedNullUnsafe { get { return this.toolStripItemComboBox1.ComboBox.SelectedItem as SymbolInfo; } }
		bool							rebuildingDropdown;
		bool							openDropDownAfterSelected;

		public SymbolInfoEditorControl() {
			InitializeComponent();

			// DESIGNER_RESETS_TO_EDITABLE__LAZY_TO_TUNNEL_PROPERTIES_AND_EVENTS_IN_ToolStripItemComboBox.cs
			this.toolStripItemComboBox1.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.toolStripItemComboBox1.ComboBox.Sorted = true;
			this.toolStripItemComboBox1.ComboBox.SelectedIndexChanged += new EventHandler(this.toolStripItemComboBox1_SelectedIndexChanged);
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.repositorySerializerSymbolInfo = repositorySerializerSymbolInfo;
			this.rebuildDropdown();
			if (this.repositorySerializerSymbolInfo.SymbolInfos.Count > 0) {
				this.populateWithSymbolInfo(this.repositorySerializerSymbolInfo.SymbolInfos[0]);
			}
		}

		void rebuildDropdown() {
			this.rebuildingDropdown = true;
			try {
				this.toolStripItemComboBox1.ComboBox.Items.Clear();
				foreach (SymbolInfo symbolInfo in this.repositorySerializerSymbolInfo.SymbolInfos) {
					this.toolStripItemComboBox1.ComboBox.Items.Add(symbolInfo);
				}
			} finally {
				this.rebuildingDropdown = false;
			}
		}
		void populateWithSymbolInfo(SymbolInfo symbolInfo, bool rebuildDropdown = false) {
			if (symbolInfo == null) {
				string msg = "I_REFUSE_TO_INITIALIZE_WITH_NULL_SYMBOL_INFO";
				Assembler.PopupException(msg);
				return;
			}
			this.toolStripItemComboBox1.ComboBox.SelectedItem = symbolInfo.ToString();
			Form parent = base.Parent as Form;
			if (parent != null) {
				parent.Text = "Symbol Editor :: " + symbolInfo.ToString();
			}

			this.mniDeleteSymbol.Text				= "Delete [" + symbolInfo.Symbol + "]";
			this.mniltbAddNew.InputFieldValue		= symbolInfo.Symbol;
			this.mniltbDuplicate.InputFieldValue	= symbolInfo.Symbol;
			this.mniltbRename.InputFieldValue		= symbolInfo.Symbol;

			this.propertyGrid1.SelectedObject = symbolInfo;
			if (rebuildDropdown) this.rebuildDropdown();
			if (this.symbolInfoSelectedNullUnsafe != null && this.symbolInfoSelectedNullUnsafe.ToString() == symbolInfo.ToString()) {
				return;
			}
			foreach (SymbolInfo eachSymbolInfo in this.toolStripItemComboBox1.ComboBox.Items) {
				if (eachSymbolInfo.ToString() != symbolInfo.ToString()) continue;
				this.openDropDownAfterSelected = false;
				this.toolStripItemComboBox1.ComboBox.SelectedItem = eachSymbolInfo;	// triggering event to invoke toolStripComboBox1_SelectedIndexChanged => testing chartSettingsSelectedNullUnsafe + Initialize()
				break;
			}
		}
		public void PopulateWithSymbol(DataSourceSymbolEventArgs e) {
			SymbolInfo symbolInfo = this.repositorySerializerSymbolInfo.FindSymbolInfoOrNew(e.Symbol);
			this.populateWithSymbolInfo(symbolInfo);
		}
	}
}
