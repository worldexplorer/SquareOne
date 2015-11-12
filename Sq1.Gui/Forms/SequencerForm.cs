using System;
using System.Windows.Forms;

using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class SequencerForm : DockContentImproved {
		ChartFormsManager chartFormsManager;
		public bool Initialized { get { return this.chartFormsManager != null; } }
		
		public SequencerForm() {
			InitializeComponent();
		}
		
		public SequencerForm(ChartFormsManager chartFormManager) : this() {
			this.Initialize(chartFormManager);
			//ERASES_LINE_IN_DOCK_CONTENT_XML_IF_WITHOUT_IGNORING this.Disposed += this.LivesimForm_Disposed;
			this.FormClosing += new FormClosingEventHandler(this.sequencerForm_FormClosing);
			this.FormClosed += new FormClosedEventHandler(this.sequencerForm_FormClosed);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Sequencer:" + this.SequencerControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormsManager chartFormsManagerPassed) {
			//if (this.chartFormsManager == chartFormsManagerPassed) return;
			this.chartFormsManager = chartFormsManagerPassed;
			this.SequencerControl.Initialize(this.chartFormsManager.Executor.Sequencer);
			this.WindowTitlePullFromStrategy();
		}

		public void WindowTitlePullFromStrategy() {
			//v1
			//string windowTitle = "Sequencer :: " + this.chartFormsManager.Strategy.Name;
			//if (this.chartFormsManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
			//v2
			this.Text = this.SequencerControl.ToString();
		}
	}
}
