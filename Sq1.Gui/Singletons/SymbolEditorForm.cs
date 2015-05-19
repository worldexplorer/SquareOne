using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	// TO_ENABLE_DESIGNER public partial class SymbolsEditorForm : DockContent {
	// TO_ENABLE_SINGLETON_FUNCTIONALITY
	public partial class SymbolEditorForm : DockContentSingleton<SymbolEditorForm> {
		public SymbolEditorForm() {
			InitializeComponent();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo) {
			this.SymbolEditorControl.Initialize(repositorySerializerSymbolInfo);
		}
	}
}
