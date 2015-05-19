using System;
using System.Windows.Forms;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	// TO_ENABLE_DESIGNER public partial class SymbolsEditorForm : Form {
	// TO_ENABLE_SINGLETON_FUNCTIONALITY
	public partial class SymbolsEditorForm : DockContentSingleton<SymbolsEditorForm> {
		public SymbolsEditorForm() {
			InitializeComponent();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.symbolEditorControl1.Initialize(repositorySerializerSymbolInfo);
		}
	}
}
