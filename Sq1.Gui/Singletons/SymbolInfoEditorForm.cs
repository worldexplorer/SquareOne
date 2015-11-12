using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	// TO_ENABLE_DESIGNER
	//public partial class SymbolInfoEditorForm : DockContent {
	// TO_ENABLE_SINGLETON_FUNCTIONALITY
	public partial class SymbolInfoEditorForm : DockContentSingleton<SymbolInfoEditorForm> {
		public SymbolInfoEditorForm() {
			InitializeComponent();
		}
		public void Initialize(RepositorySerializerSymbolInfo repositorySerializerSymbolInfo, RepositoryJsonDataSource repositoryJsonDataSource) {
			this.SymbolEditorControl.Initialize(repositorySerializerSymbolInfo, repositoryJsonDataSource);
		}
	}
}
