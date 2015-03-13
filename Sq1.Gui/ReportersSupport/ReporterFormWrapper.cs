using System;
using System.Windows.Forms;

using Sq1.Widgets;
using Sq1.Core.StrategyBase;

namespace Sq1.Gui.ReportersSupport {
	public partial class ReporterFormWrapper : DockContentImproved {
		Reporter reporter;
		ReportersFormsManager reportersFormsManager;

		protected ReporterFormWrapper() {
			InitializeComponent();
		}
		public ReporterFormWrapper(ReportersFormsManager reportersFormsManager, Reporter reporterActivated) : this() {
			this.reportersFormsManager = reportersFormsManager;
			this.reporter = reporterActivated;

			this.SuspendLayout();
			this.Controls.Clear();
			this.Controls.Add(this.reporter);
			this.reporter.Dock = DockStyle.Fill;
			this.ResumeLayout();
		}
		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "ReporterWrapped:" + this.reporter.GetType().FullName + ",ChartSerno:" + this.reportersFormsManager.ChartFormManager.DataSnapshot.ChartSerno;
		}
		private void ReporterFormWrapper_FormClosing(object sender, FormClosingEventArgs e) {
			this.reportersFormsManager.ReporterClosingUnregisterMniUntick(reporter.GetType().Name);
		}
	}
}
