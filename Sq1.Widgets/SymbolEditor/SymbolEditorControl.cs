using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl : UserControl {
		RepositorySerializerSymbolInfo repositorySerializerSymbolInfo;
		SymbolInfo symbolInfoSelected { get { return this.toolStripComboBox1.SelectedItem as SymbolInfo; } }

		public SymbolEditorControl() {
			InitializeComponent();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.repositorySerializerSymbolInfo = repositorySerializerSymbolInfo;
			this.rebuildDropdown();
			if (this.repositorySerializerSymbolInfo.Symbols.Count > 0) {
				this.Initialize(this.repositorySerializerSymbolInfo.Symbols[0]);
			}
		}
		void rebuildDropdown() {
			this.toolStripComboBox1.Items.Clear();
			foreach (SymbolInfo symbolInfo in this.repositorySerializerSymbolInfo.Symbols) {
				this.toolStripComboBox1.Items.Add(symbolInfo);
			}
		}
		public void Initialize(SymbolInfo symbolInfo) {
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

			this.propertyGrid1.SelectedObject = symbolInfo;
			if (this.symbolInfoSelected != null && this.symbolInfoSelected.ToString() != symbolInfo.ToString()) {
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
