using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets;
using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorControl : UserControl {
		QuikStreaming quikStreaming;

		public QuikStreamingMonitorControl() {
			InitializeComponent();
			this.olvQuotesCustomize();
		}
		internal void Initialize(QuikStreaming quikStreamingPassed) {
			this.quikStreaming = quikStreamingPassed;
		}

		internal void DomUserControl_createAddFor(DdeTableDepth tableLevel2) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate() { this.DomUserControl_createAddFor(tableLevel2); });
				return;
			}

			string msig = " //DomUserControl_createAddFor(" + tableLevel2.ToString() + ")";
			LevelTwoUserControl level2userControl = new LevelTwoUserControl();
			DockContentImproved ddeMonitorForm = this.Parent as DockContentImproved;
			if (ddeMonitorForm == null) {
				string msg = "I_NEED_THE_UPPER_LEVEL_FORM_VISIBILITY_TO_NOT_TO_REPAINT_IF_FORM_IS_MINIMIZED_OR_NOT_SHOWN";
				Assembler.PopupException(msg + msig);
			}
			level2userControl.Initialize(this.quikStreaming, tableLevel2.SymbolInfo, tableLevel2.ToString(), ddeMonitorForm);
			tableLevel2.UserControlMonitoringMe = level2userControl;
			this.flpDoms.Controls.Add(level2userControl);
		}
		internal void DomUserControl_deleteFor(DdeTableDepth tableLevel2) {
			if (base.InvokeRequired) {
			   //at System.Windows.Forms.Control.get_Handle()
			   //at System.Windows.Forms.Control.SetParentHandle(IntPtr value)
			   //at System.Windows.Forms.Control.ControlCollection.Remove(Control value)
			   //at Sq1.Adapters.Quik.Streaming.Monitor.QuikStreamingMonitorControl.DomUserControl_deleteFor(DdeTableDepth tableLevel2)
			   //at Sq1.Adapters.Quik.Streaming.Dde.DdeBatchSubscriber.TableIndividual_DepthOfMarket_ForSymbolRemove(String symbol)
			   //at Sq1.Adapters.Quik.Streaming.QuikStreaming.UpstreamUnSubscribe(String symbol)
			   //at Sq1.Adapters.Quik.Streaming.QuikStreaming.upstreamUnsubscribeAllDataSourceSymbols(Boolean avoidDuplicates_openChartsAlreadyUnsubscribed)
			   //at Sq1.Adapters.Quik.Streaming.QuikStreaming.UpstreamDisconnect()
			   //at Sq1.Adapters.Quik.Streaming.Livesim.QuikStreamingLivesim.UpstreamDisconnect_LivesimTerminatedOrAborted()
			   //at Sq1.Core.Livesim.Livesimulator.SimulationPostBarsRestore_overrideable() in C:\SquareOne\Sq1.Core\Livesim\Livesimulator.cs:line 240
				base.BeginInvoke((MethodInvoker)delegate() { this.DomUserControl_deleteFor(tableLevel2); });
				return;
			}

			string msig = " //DomUserControl_deleteFor(" + tableLevel2 + ")";
			LevelTwoUserControl domResizeable = tableLevel2.UserControlMonitoringMe as LevelTwoUserControl;
			if (domResizeable == null) {
				string msg = "I_MUST_HAVE_BEEN_LevelTwoUserControl_tableLevel2.WhereIamMonitored[" + tableLevel2.UserControlMonitoringMe + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.flpDoms.Controls.Remove(domResizeable);
		}

		internal void Populate_grpStatuses() {
			this.grpQuotes.Text = this.quikStreaming.DdeBatchSubscriber.TableQuotes.ToString();
			this.grpDom.Text	= this.quikStreaming.DdeBatchSubscriber.DomGroupboxTitle;
			this.grpTrades.Text = this.quikStreaming.DdeBatchSubscriber.TableTrades.ToString();
		}
	}
}
