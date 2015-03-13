using System;

using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class ScriptEditorForm : DockContentImproved {
		ChartFormManager chartFormManager;

		// don't use this constuctor outside this class!
		private ScriptEditorForm() {
			InitializeComponent();
		}

		// chartFormsManager is needed for serialization and the following deserialization
		public ScriptEditorForm(ChartFormManager chartFormsManager) : this() {
			this.Initialize(chartFormsManager);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "ScriptEditor:" + this.ScriptEditorControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.Text = this.chartFormManager.Strategy.Name;
			this.ScriptEditorControl.DocumentChangedIgnoredDuringInitialization = true;
			this.ScriptEditorControl.ScriptSourceCode = this.chartFormManager.Strategy.ScriptSourceCode;
			this.ScriptEditorControl.DocumentChangedIgnoredDuringInitialization = false;
		}
	}
}
