using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl : UserControl {
		RepositorySerializerSymbolInfo	repositorySerializerSymbolInfo;
		SymbolInfo						symbolInfoSelectedNullUnsafe { get { return this.toolStripComboBox1.SelectedItem as SymbolInfo; } }
		bool							rebuildingDropdown;

		public SymbolEditorControl() {
			InitializeComponent();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.repositorySerializerSymbolInfo = repositorySerializerSymbolInfo;
			this.rebuildDropdown();
			if (this.repositorySerializerSymbolInfo.SymbolInfos.Count > 0) {
				this.Initialize(this.repositorySerializerSymbolInfo.SymbolInfos[0]);
			}
		}

		void rebuildDropdown() {
			this.rebuildingDropdown = true;
			try {
				this.toolStripComboBox1.Items.Clear();
				foreach (SymbolInfo symbolInfo in this.repositorySerializerSymbolInfo.SymbolInfos) {
					this.toolStripComboBox1.Items.Add(symbolInfo);
				}
			} finally {
				this.rebuildingDropdown = false;
			}
		}
		public void Initialize(SymbolInfo symbolInfo, bool rebuildDropdown = false) {
			if (symbolInfo == null) {
				string msg = "I_REFUSE_TO_INITIALIZE_WITH_NULL_SYMBOL_INFO";
				Assembler.PopupException(msg);
				return;
			}
			this.tsmniSymbol.Text = symbolInfo.ToString();
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
			foreach (SymbolInfo eachSymbolInfo in this.toolStripComboBox1.Items) {
				if (eachSymbolInfo.ToString() != symbolInfo.ToString()) continue;
				this.toolStripComboBox1.SelectedItem = eachSymbolInfo;
				break;
			}
		}
		public void PopulateWithSymbol(DataSourceSymbolEventArgs e) {
			SymbolInfo symbolInfo = this.repositorySerializerSymbolInfo.FindSymbolInfoOrNew(e.Symbol);
			this.Initialize(symbolInfo);
		}
	}
}
