using System;

using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm : DockContentImproved {
		ChartFormManager chartFormManager;
		
		public LivesimForm() {
			InitializeComponent();
		}
		
		public LivesimForm(ChartFormManager chartFormsManager) : this() {
			this.Initialize(chartFormsManager);
		}
		
		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "LiveSim:" + this.liveSimControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.Text = "LiveSim :: " + this.chartFormManager.Strategy.Name;
			//this.liveSimControl.Initialize(this.chartFormManager.Executor.Optimizer);
		}
	}
}
