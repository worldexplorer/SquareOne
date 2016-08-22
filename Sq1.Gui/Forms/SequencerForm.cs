using System;
using System.Windows.Forms;

using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;

using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class SequencerForm : DockContentImproved {
				ChartFormManager	chartFormManager;
		public	bool				Initialized			{ get { return this.chartFormManager != null; } }
		
		public SequencerForm() {
			InitializeComponent();
			base.HideOnClose = false;
		}
		
		public SequencerForm(ChartFormManager chartFormManager) : this() {
			this.Initialize(chartFormManager);
			//ERASES_LINE_IN_DOCK_CONTENT_XML_IF_WITHOUT_IGNORING this.Disposed += this.LivesimForm_Disposed;
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Sequencer:" + this.SequencerControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManagerPassed) {
			//if (this.chartFormsManager == chartFormsManagerPassed) return;
			this.chartFormManager = chartFormManagerPassed;
			this.SequencerControl.Initialize(this.chartFormManager.Executor.Sequencer);
			this.WindowTitlePullFromStrategy();

			this.SequencerControl.OnCopyToContextDefault			+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextDefault);
			this.SequencerControl.OnCopyToContextDefaultBacktest	+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextDefaultBacktest);
			this.SequencerControl.OnCopyToContextNew				+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextNew);
			this.SequencerControl.OnCopyToContextNewBacktest		+= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencerControl_OnCopyToContextNewBacktest);
			this.SequencerControl.OnCorrelatorShouldPopulate		+= new EventHandler<SequencedBacktestsEventArgs>(this.sequencerControl_OnCorrelatorShouldPopulate);
			this.FormClosing										+= new FormClosingEventHandler(this.sequencerForm_FormClosing);
			this.FormClosed											+= new FormClosedEventHandler(this.sequencerForm_FormClosed);
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
