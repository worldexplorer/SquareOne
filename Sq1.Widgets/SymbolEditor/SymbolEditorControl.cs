using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Repositories;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl : UserControl {
		public SymbolEditorControl() {
			InitializeComponent();
			
			this.olvSymbolsCustomize();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.olvSymbols.SetObjects(repositorySerializerSymbolInfo.Symbols);
		}
	}
}
