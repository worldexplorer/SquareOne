using System;
using System.Windows.Forms;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	//public partial class BarsEditorForm : Form {
	public partial class BarsEditorForm : DockContentSingleton<BarsEditorForm> {
		public BarsEditorForm() {
			InitializeComponent();
		}

		internal void Initialize(RepositoryJsonDataSources repositoryJsonDataSources) {
			this.BarsEditorUserControl.Initialize(repositoryJsonDataSources);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "BarsEditor:Sq1.Gui.Singletons.BarsEditorForm.Instance"
				+ ",DataSourceEditing:"	+ this.BarsEditorUserControl.DataSourceName
				+ ",SymbolEditing:"		+ this.BarsEditorUserControl.Symbol
				;
		}
	}
}
