using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Repositories;

namespace Sq1.Gui.Singletons {
	//public partial class FuturesMergerForm : Form {
	public partial class FuturesMergerForm : DockContentSingleton<FuturesMergerForm> {

		public FuturesMergerForm() {
			InitializeComponent();
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
		    return "FuturesMerger:Sq1.Gui.Singletons.FuturesMergerForm.Instance"
		        + ",DataSourceEditing:" + this.FuturesMergerUserControl.BarsEditorUserControl_top.DataSourceName
		        + ",SymbolEditing:"		+ this.FuturesMergerUserControl.BarsEditorUserControl_top.Symbol
		        ;
		}

		internal void Initialize(RepositoryJsonDataSources repositoryJsonDataSources) {
			this.FuturesMergerUserControl.Initialize(repositoryJsonDataSources);
		}
	}
}
