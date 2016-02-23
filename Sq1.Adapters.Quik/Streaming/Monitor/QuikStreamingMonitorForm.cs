using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

using Sq1.Widgets;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorForm : DockContentImproved {
		QuikStreaming	quikStreaming;
		Stopwatch		StopwatchRarifyingUIupdates;

		// I_DONT_WANT_MONITOR_TO_STAY_AFTER_APPRESTART__HOPING_NEVER_INVOKED_BY_DESERIALIZER
		public QuikStreamingMonitorForm() {
			InitializeComponent();
			this.StopwatchRarifyingUIupdates = new Stopwatch();
		}

		// HUMAN_INVOKED_CONSTRUCTOR
		public QuikStreamingMonitorForm(QuikStreaming quikStreamingInstantiatedForDataSource) : this() {
			this.quikStreaming = quikStreamingInstantiatedForDataSource;
			this.StopwatchRarifyingUIupdates.Start();
			this.QuikStreamingMonitorControl.Initialize(this.quikStreaming, this.StopwatchRarifyingUIupdates);
		}

		//internal void RaiseOnDdeMonitorClosing(object sender, FormClosingEventArgs e) {
		//    foreach (XlDdeTableMonitoreable<LevelTwo> eachLevel2 in this.Level2BySymbol.Values) {
		//        TODO eachLevel2.Form(sender, e);
		//    }
		//}

		void quikStreaming_OnConnectionStateChanged(object sender, EventArgs e) {
			this.populateWindowTitle_grpStatuses();
		}

		void populateWindowTitle_grpStatuses() {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.populateWindowTitle_grpStatuses(); });
				return;
			}
			string refreshRate = " (" + this.quikStreaming.DdeMonitorRefreshRateMs + "ms refresh rate)";
			base.Text = this.quikStreaming.DdeBatchSubscriber.WindowTitle + refreshRate;
			this.QuikStreamingMonitorControl.Populate_grpStatuses();
		}

		void tableQuotes_DataStructuresParsed_Table(object sender, XlDdeTableMonitoringEventArg<List<QuoteQuik>> e) {
			if (base.IsDisposed) return;
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.tableQuotes_DataStructuresParsed_Table(sender, e); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			//if (this.StopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRateMs) return;
			//this.StopwatchRarifyingUIupdates.Restart();

			this.QuikStreamingMonitorControl.OlvQuotes.SetObjects(e.DataStructureParsed);
			this.populateWindowTitle_grpStatuses();

			// done in QuikStreamingMonitorControl.Populate_grpStatuses()
			//XlDdeTableMonitoreable<QuoteQuik> xlDdeTable = sender as XlDdeTableMonitoreable<QuoteQuik>;
			//if (xlDdeTable == null) return;
			//this.QuikStreamingMonitorControl.grpQuotes.Text = xlDdeTable.ToString();
		}
		void tableQuotes_DataStructureParsed_One(object sender, XlDdeTableMonitoringEventArg<QuoteQuik> e) {
			// dont forget about the stopwatch
			// if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
		}

		internal void PopulateWindowTitle_dataSourceName_market_quotesTopic() {
			base.Text = this.ToString();
		}
		public override string ToString() {
			return this.quikStreaming.IdentForMonitorWindowTitle;
		}
	}
}
